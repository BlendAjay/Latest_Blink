using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AJSolutions.DAL;
using AJSolutions.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Globalization;
using System.Threading.Tasks;
using System.Net.Http;
namespace AJSolutions.Areas.Admin.Controllers
{
    public class Plan_PricingController : Controller
    {
        Generic generic = new Generic();
        AdminManager admin = new AdminManager();
        UserDBContext db = new UserDBContext();
        private ApplicationUserManager _userManager;
        // GET: Admin/Plan_Pricing
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Plan()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (userdetails != null)
            {
                ViewBag.UserRole = GetUserRole(userdetails.SubscriberId);

                var userplan = db.UserPlan.Where(c => c.CorporateId == userdetails.UserId).FirstOrDefault();
                if (userplan != null)
                {
                    ViewBag.TrailExpired = "True";
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult CheckOut(string PlanType, string TMS)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (userdetails == null)
            {
                return RedirectToAction("Register", "Account", new { Area = "", PlanType = PlanType, TMS = TMS });
            }
            ViewBag.UserRole = GetUserRole(userdetails.SubscriberId);
            ViewBag.Message = PlanType;

            ViewBag.TMS = Convert.ToBoolean(TMS);

            return View();
        }

        [HttpPost]
        public ActionResult CheckOut(string PlanType, string CorporateId, string Comments, string PlanStatus, string TMS)
        {

            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            //var plandetail = db.UserPlan.Where(c => c.CorporateId == CorporateId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).FirstOrDefault();
            if (plandetail == null || plandetail.Status != "Succeeded" || plandetail.PlanEndDate <= DateTime.Now)
            {
                // for Paid Plan PlanId will be 3

                string UserPlanId = admin.GetUserplanId();

                int PlanId = 3;
                DateTime PlanStartDate = DateTime.UtcNow;
                DateTime PlanEndDate = DateTime.Today.AddDays(365); ;
                DateTime NextDueDate = PlanEndDate;
                DateTime UpdatedOn = DateTime.UtcNow;
                string UpdatedBy = CorporateId;

                bool result = admin.AddUserPlan(UserPlanId, CorporateId, PlanId, PlanStartDate, PlanEndDate, NextDueDate, Comments, PlanStatus, UpdatedOn, UpdatedBy);
                
                ///commented due to change in Plan & Pricing
                ////Adding In PlanAddons
                //DateTime AddonStartDate = DateTime.UtcNow;
                //string UserAdonId = admin.GetUserAddonsId();

                //bool result1 = admin.AddUserPlanAddons(UserAdonId, UserPlanId, 3, AddonStartDate, Comments, UpdatedOn, UpdatedBy);
                ////END

                //Payment Start
                double Amount = 12000.0;

                string referenceId = admin.GetReferenceId();
                Int64 TransactionId = 0;

                string Currency = "INR";
                string PayeeName = userdetails.Name;
                string PayeeEmail = userdetails.Email;
                string PayeePhoneNumber = userdetails.PhoneNumber;
                DateTime PaymentDate = DateTime.UtcNow;
                string Status = null;
                string TransactionReferenceNumber = null;
                string BankCode = null;
                string PGComment = null;
                string ClientTxnRefNo = null;
                string TSPLTxnId = null;
                string txn_status = null;
                string txn_msg = null;
                string strReturnUrl = Global.WebsiteUrl() + "Account/PaymentConfirmation";
                string Result = admin.PaymentRequest(referenceId, PayeeName, Convert.ToString(Amount), PayeeEmail, PayeePhoneNumber, null, strReturnUrl);

                if (!Result.ToUpper().StartsWith("ERROR"))
                {
                    bool res = admin.AddRegistrationDetails(TransactionId, UserPlanId, userdetails.UserId, referenceId, Currency, Amount, Comments, PayeeName, PayeeEmail, PayeePhoneNumber, PaymentDate, Status, TransactionReferenceNumber, BankCode, PGComment, ClientTxnRefNo, TSPLTxnId, txn_status, txn_msg);
                    if (res)
                        return Json(Result, JsonRequestBehavior.AllowGet);
                    //return Redirect(Url.Content(Global.WebsiteUrl() + "Request.aspx?Id=" + Result));
                }
                //End
            }
            return RedirectToAction("CheckOut", "Plan_Pricing", new { Area = "Admin" });
        }



        [HttpGet]
        public ActionResult CreatePlan(string UserAction, int PlanId = 0, bool status = false)
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            if (status == true)
            {
                ViewBag.Status = "Succeed";
            }
            if (UserAction == "Delete" && PlanId > 0)
            {
                admin.DeletePlan(PlanId);
                ViewBag.Deleted = "Deleted";
                return View();
            }
            var Plans = db.Plan.ToList();
            ViewData["Plans"] = Plans;
            var SelectedPlan = Plans.Where(c => c.PlanId == PlanId).FirstOrDefault();
            if (SelectedPlan != null)
                ViewBag.CommenceDate = SelectedPlan.CommenceMentDate.ToString("dd-MM-yyyy");

            if (SelectedPlan != null && SelectedPlan.RetiredDate != null)
                ViewBag.RetiredDate = SelectedPlan.RetiredDate.Value.ToString("dd-MM-yyyy");

            return View(SelectedPlan);
        }

        [HttpPost]
        public ActionResult CreatePlan(string PlanName, string CommenceMentDate, string RetiredDate, string ShortDescription, double INRAmount, double INRDiscount, double OtherAmount, double OtherDiscount, Int16 PlanSequence = 0, int PlanId = 0)
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            DateTime CommenceDT = DateTime.ParseExact(CommenceMentDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime? RetiredDt = null;
            if (!String.IsNullOrEmpty(RetiredDate))
            {
                RetiredDt = DateTime.ParseExact(RetiredDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            var result = admin.AddPlan(PlanId, PlanName, ShortDescription, PlanSequence, CommenceDT, RetiredDt, INRAmount, INRDiscount, OtherAmount, OtherDiscount, DateTime.UtcNow, userDetails.UserId);
            return RedirectToAction("CreatePlan", "Plan_Pricing", new { status = result });
        }

        [HttpGet]
        public ActionResult CreatePlanAddOns(string AddOnName, string UserAction, double INRAmount = 0, double INRDiscount = 0, double OtherAmount = 0, double OtherDiscount = 0, int AddOnId = 0, bool status = false)
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            if (status == true)
            {
                ViewBag.Status = "Succeed";
            }
            if (UserAction == "Delete" && AddOnId > 0)
            {
                admin.DeletePlanAddons(AddOnId);
                ViewBag.Deleted = "Deleted";
                return View();
            }
            var PlanAddons = db.PlanAddOns.ToList();
            ViewData["PlanAddons"] = PlanAddons;
            var SelectedPlan = PlanAddons.Where(c => c.AddOnId == AddOnId).FirstOrDefault();

            return View(SelectedPlan);
        }

        [HttpPost]
        public ActionResult CreatePlanAddOns(string AddOnName, double INRAmount, double INRDiscount, double OtherAmount, double OtherDiscount, int AddOnId = 0)
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());

            var result = admin.AddPlanAddOns(AddOnId, AddOnName, INRAmount, INRDiscount, OtherAmount, OtherDiscount);
            return RedirectToAction("CreatePlan", "Plan_Pricing", new { status = result });
        }


        [HttpGet]
        public ActionResult AddFeatures(string UserAction, Int64 FeatureId = 0, bool status = false)
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            if (status == true)
            {
                ViewBag.Status = "Succeed";
            }
            if (UserAction == "Delete" && FeatureId > 0)
            {
                admin.DeleteFeatures(FeatureId);
                ViewBag.Deleted = "Deleted";
                return View();
            }
            var Features = db.Features.ToList();
            ViewData["Features"] = Features;
            var SelectedFeatures = Features.Where(c => c.FeatureId == FeatureId).FirstOrDefault();

            return View(SelectedFeatures);
        }

        [HttpPost]
        public ActionResult AddFeatures(string Feature, Int16 FeatureSequence = 0, Int64 FeatureId = 0)
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());

            var result = admin.AddFeatures(FeatureId, Feature, FeatureSequence);
            return RedirectToAction("AddFeatures", "Plan_Pricing", new { status = result });
        }

        [HttpGet]
        public ActionResult PlanFeatures(string UserAction, Int64 FeatureId = 0, int PlanId = 0, bool status = false)
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            if (status == true)
            {
                ViewBag.Status = "Succeed";
            }
            if (UserAction == "Delete" && FeatureId > 0)
            {
                admin.DeleteFeatures(FeatureId);
                ViewBag.Deleted = "Deleted";
                return View();
            }
            PopulatePlans();
            ViewData["features"] = db.Features.ToList(); ;
            var planFeatures = admin.GetPlanFeatures().ToList();
            ViewData["planFeatures"] = planFeatures;
            var SelectedplanFeatures = planFeatures.Where(c => c.FeatureId == FeatureId).Where(d => d.PlanId == PlanId).FirstOrDefault();
            return View(SelectedplanFeatures);
        }

        [HttpPost]
        public ActionResult PlanFeatures(Int64[] FeatureId, int PlanId = 0)
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());

            foreach (var features in FeatureId)
            {
                var result = admin.AddPlanFeatures(PlanId, features);
            }
            return RedirectToAction("PlanFeatures", "Plan_Pricing");
        }

        private void PopulatePlans(object selectedOrderType = null)
        {
            var query = db.Plan.ToList();
            SelectList OrderTypes = new SelectList(query, "PlanId", "PlanName", selectedOrderType);
            ViewBag.PlanId = OrderTypes;
        }

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

        private string GetUserRole(string UserId)
        {
            if (!String.IsNullOrEmpty(UserId))
            {
                if (UserManager.IsInRole(UserId, "Admin"))
                    return "Admin";
                else if (UserManager.IsInRole(UserId, "Client"))
                    return "Client";
                else if (UserManager.IsInRole(UserId, "Employee"))
                    return "Employee";
            }
            return string.Empty;
        }

        ////private async Task<bool> RegisterToLMS(RegisterViewModel model, string UserId)
        //private async Task<bool> RegisterToLMS(string UserId, string UserName, string UserRole, string Name, string Email, string PhoneNumber, string SubscriberId)
        //{
        //    bool result = false;
        //    try
        //    {
        //        UserRegView userReg = new UserRegView();
        //        userReg.UserId = UserId;
        //        userReg.UserName = UserName;
        //        userReg.UserRole = UserRole;
        //        userReg.Name = Name;
        //        userReg.Email = Email;
        //        userReg.MobileNumber = PhoneNumber;
        //        userReg.Redirectionurl = "reckonn.com";
        //        userReg.SubscriberId = SubscriberId;
        //        userReg.Password = Global.RandomString(8);

        //        string apiUrl = Global.WikipianUrl() + "Api/Value/PostUserReg";
        //        HttpResponseMessage responsePostMethod = new HttpResponseMessage();
        //        using (HttpClient client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(apiUrl);
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //            responsePostMethod = await client.PostAsJsonAsync(apiUrl, userReg);
        //            if (responsePostMethod.IsSuccessStatusCode)
        //            {
        //                result = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return result;
        //}

        //public async Task<bool> RegisterToPrelore(string Name, string Email, string PhoneNumber, string SubscriberId)
        //{
        //    bool result = false;
        //    try
        //    {
        //        string apiUrl = Global.PreloreUrl() + "Api/Value/PostRegisterClient";

        //        PreloreAdminView adminProfile = new PreloreAdminView();
        //        adminProfile.SubscriberId = SubscriberId;
        //        adminProfile.Name = Name;
        //        adminProfile.MobileNumber = PhoneNumber;
        //        adminProfile.Email = Email;
        //        adminProfile.Password = Global.RandomString(8);

        //        HttpResponseMessage responsePostMethod = new HttpResponseMessage();
        //        using (HttpClient client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(apiUrl);
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //            responsePostMethod = await client.PostAsJsonAsync(apiUrl, adminProfile);
        //        }
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return result;
        //}

    }
}