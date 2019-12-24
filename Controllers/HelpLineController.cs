using AJSolutions.DAL;
using AJSolutions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Configuration;
using SparkPost;

namespace AJSolutions.Controllers
{
    public class HelpLineController : Controller
    {
        AdminManager admin = new AdminManager();
        Generic generic = new Generic();
        UserDBContext db = new UserDBContext();
        CMSManager cms = new CMSManager();
        EMSManager ems = new EMSManager();

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: HelpLine
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel UserDetails = new UserViewModel();
            if (!string.IsNullOrEmpty(UserId))
            {
                UserDetails = generic.GetUserDetail(UserId);
                ViewData["UserProfile"] = UserDetails;
                ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
                ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            }
            List<HelpLineLayers> Layers = db.HelpLineLayers.ToList();
            ViewBag.SubscriberId = Layers.First().SubscriberId;

            PopulateCategory();

            List<HelpLineLayerDetails> LayerSCDetails = db.HelpLineLayerDetails.Where(l => l.LayerId == 0).ToList();
            ViewBag.SubCategory = new SelectList(LayerSCDetails, "LayerDetailsId", "LayerText", null);

            List<HelpLineLayerDetails> LayerQDetails = db.HelpLineLayerDetails.Where(l => l.LayerId == 0).ToList();
            ViewBag.Query = new SelectList(LayerQDetails, "LayerDetailsId", "LayerText", null);

            return View(UserDetails);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult Tracker(int? page, int PageSize = 10, long Category = 0, long SubCategory = 0)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;

            ViewBag.CategoryId = Category;
            ViewBag.SubCategoryId = SubCategory;

            PopulateCategory(Category);

            List<HelpLineLayerDetails> LayerSCDetails;
            if (Category != 0)
                LayerSCDetails = db.HelpLineLayerDetails.Where(l => l.LayerId == 2).ToList();
            else
                LayerSCDetails = db.HelpLineLayerDetails.Where(l => l.LayerId == 0).ToList();

            ViewBag.SubCategory = new SelectList(LayerSCDetails, "LayerDetailsId", "LayerText", SubCategory);

            var tracker = admin.GetHelpLineData(null, Category, SubCategory);
            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);

            return View(tracker.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult Tracker(long TrackerReplyId, string QueryResponse, string Replied)
        {
            bool result = admin.UpdateHelplineResponse(TrackerReplyId, QueryResponse, Replied);

            return RedirectToAction("Tracker", "HelpLine");
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult Responses(long TrackerId)
        {
            EditHelpLineTrackerViewModel reply = db.HelpLineTracker.Where(t => t.TrackerId == TrackerId)
                                                                    .Select(t => new EditHelpLineTrackerViewModel
                                                                    {
                                                                        TrackerId = t.TrackerId
                                                                        ,
                                                                        DynamicQuery = t.DynamicQuery
                                                                        ,
                                                                        OtherQueryResolution = t.OtherQueryResolution
                                                                        ,
                                                                        ReplyBy = t.ReplyBy
                                                                        ,
                                                                        RepliedOn = t.RepliedOn
                                                                    }
                                                                            ).FirstOrDefault();

            return Json(reply, JsonRequestBehavior.AllowGet);
        }

        private void PopulatePaging(object selectedValue = null)
        {
            var PageList = generic.GetPaging();
            ViewBag.PageSize = new SelectList(PageList, "PageSize", "PageSize", selectedValue);
        }

        [HttpPost]
        public ActionResult GetNextLayerDetails(string LayerDetailsId)
        {
            Int64 LayerDetId;
            List<SelectListItem> Layer = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(LayerDetailsId))
            {
                LayerDetId = Convert.ToInt64(LayerDetailsId);
                List<HelpLineLayerDetails> LayerDetList = db.HelpLineLayerDetails.Where(l => l.LayerDetailsParentId == LayerDetId).ToList();
                LayerDetList.ForEach(x =>
                {
                    Layer.Add(new SelectListItem { Text = x.LayerText, Value = x.LayerDetailsId.ToString() });
                });
            }
            return Json(Layer, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetResolution(string Name, string PhoneNumber, string Email, string UserName, Int64 Category, Int64 SubCategory, Int64 Query, string DynamicQuery, string SubscriberId)
        {
            //Save to tracker
            HelpLineTracker HelpQuery = new HelpLineTracker();
            HelpQuery.SubscriberId = SubscriberId;
            HelpQuery.UserName = UserName;
            HelpQuery.Name = Name;
            HelpQuery.PhoneNumber = PhoneNumber;
            HelpQuery.EmailId = Email;
            HelpQuery.Category = Convert.ToInt64(Category);
            HelpQuery.SubCategory = Convert.ToInt64(SubCategory);
            HelpQuery.Query = Query;
            HelpQuery.QueriedOn = DateTime.UtcNow;
            HelpQuery.RepliedOn = DateTime.UtcNow;

            string text = "";
            if (Query == 0)
            {
                HelpQuery.Resolution = 0;
                HelpQuery.DynamicQuery = DynamicQuery;
                HelpQuery.Replied = false;
                text = "Thank You for the query. Our Support team will get back to you with the resolution soon.";

                //Send Email to IPPBSupport@nibf.in
                string msgBody = "";
                if (!string.IsNullOrEmpty(Email))
                    msgBody = "Hi Team, <br /><br />Query: " + DynamicQuery + " <br />By: " + Name + ", " + PhoneNumber + ", " + Email + " <br /><br />Thanks,<br />Blink.";
                else
                    msgBody = "Hi Team, <br /><br />Query: " + DynamicQuery + " <br />By: " + Name + ", " + PhoneNumber + " <br /><br />Thanks,<br />Blink.";
                SendEmail("ippbsupport@nibf.in", "IPPB Query from HelpLine", msgBody);
            }
            else
            {
                HelpLineLayerDetails LayerDet = db.HelpLineLayerDetails.Where(l => l.LayerDetailsParentId == Query).FirstOrDefault();
                HelpQuery.Resolution = LayerDet.LayerDetailsId;
                HelpQuery.DynamicQuery = null;
                HelpQuery.Replied = true;
                HelpQuery.ReplyBy = "SYSTEM";
                text = LayerDet.LayerText;
            }
            admin.AddHelpLineTracker(HelpQuery);

            return Json(text, JsonRequestBehavior.AllowGet);
        }

        public static void SendEmail(string email, string Subject, string MsgBody)
        {
            try
            {
                var settings = ConfigurationManager.AppSettings;
                var transmission = new Transmission();
                transmission.Content.From.Email = ConfigurationManager.AppSettings["From_Email"].ToString();
                transmission.Content.Subject = Subject;
                transmission.Content.Text = MsgBody;
                transmission.Content.Html = MsgBody;
                transmission.Content.Html = MsgBody;
                var recipient = new Recipient
                {
                    Address = new SparkPost.Address { Email = email }
                };
                transmission.Recipients.Add(recipient);

                var client = new Client(ConfigurationManager.AppSettings["Spark_Post_API"].ToString());
                client.ApiKey = ConfigurationManager.AppSettings["Spark_Post_API"].ToString();
                client.CustomSettings.SendingMode = SendingModes.Sync;

                var response = client.Transmissions.Send(transmission);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        #region "Helpers"

        private void PopulateCategory(object selectCategory = null)
        {
            List<HelpLineLayerDetails> LayerCDetails;
            //if (selectCategory != 0)
            //    LayerCDetails = db.HelpLineLayerDetails.Where(l => l.LayerName == "Category" && l.LayerDetailsId == selectCategory).ToList();
            //else
            LayerCDetails = db.HelpLineLayerDetails.Where(l => l.LayerName == "Category").ToList();
            SelectList LayerList = new SelectList(LayerCDetails, "LayerDetailsId", "LayerText", selectCategory);
            ViewBag.Category = LayerList;
        }

        #endregion
    }
}