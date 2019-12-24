using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AJSolutions.Models;
using AJSolutions.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.Areas.CMS.Models;
using System.Globalization;
namespace AJSolutions.Areas.CMS.Controllers
{
    public class JobOrderController : Controller
    {
        Generic generic = new Generic();
        CMSManager cmsMgr = new CMSManager();
        AdminManager admin = new AdminManager();
        UserDBContext udb = new UserDBContext();

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

        ///// <summary>
        ///// By: Ajay Kumar Choudhary 
        ///// on: 17-07-2017
        ///// For: Team Member Rights
        ///// </summary>
        ///// 
        //public ClientTeamMemberProfileView GetUserCredential(string UserId, string Role)
        //{
        //    ClientTeamMemberProfileView userAccess = null;



        //    if (Role != AJSolutions.DAL.Global.UserRoles.Administrator || Role != AJSolutions.DAL.Global.UserRoles.Candidate || Role != AJSolutions.DAL.Global.UserRoles.Admin)
        //    {

        //        var userDetail = udb.CorporateProfile.Find(UserId);
        //        var CompanyDetail = udb.CompanyProfile.Find(UserId);

        //        if (userDetail != null && CompanyDetail != null)
        //        {
        //            userAccess = new ClientTeamMemberProfileView();
        //            //Validation for Account owner Employer/Cosultant
        //            // userAccess.UserId = userDetail.CorporateId;
        //            userAccess.CorporateId = userDetail.CorporateId;
        //            userAccess.Name = userDetail.Name;
        //            //  Employer Role "Admin for Account Owner
        //            var CorporateRole = udb.ClientTeamRoles.Find(1).EmpRoleId;

        //            userAccess.EmpRoleId = CorporateRole;

        //            // Check Employer Type to view some specific rights

        //            var EmployerType = udb.CorporateProfile.Find(userDetail.CorporateId);

        //            var RightList = (from r in udb.ClientTeamRights.Where(r => (r.GroupType == 99) && r.Visibility == true)
        //                             select r).AsEnumerable();


        //            List<ClientTeamMemberRights> HirerRights = new List<ClientTeamMemberRights>();

        //            foreach (var right in RightList)
        //            {
        //                HirerRights.Add(new ClientTeamMemberRights { UserId = userDetail.CorporateId, EmpRightsId = right.EmpRightsId });
        //            }

        //            userAccess.ClientTeamRights = HirerRights;

        //        }

        //        else
        //        {

        //            var CorporateHirer = udb.ClientTeamMemberProfile.Where(u => u.MemberId == UserId).FirstOrDefault();

        //            if (CorporateHirer != null)
        //            {

        //                userAccess = new ClientTeamMemberProfileView();

        //                userAccess.CorporateId = CorporateHirer.CorporateId;
        //                userAccess.EmpRoleId = CorporateHirer.EmpRoleId;
        //                userAccess.Name = CorporateHirer.Name;
        //                userAccess.UpdatedBy = CorporateHirer.UpdatedBy;
        //                userAccess.UpdatedOn = CorporateHirer.UpdatedOn;
        //                userAccess.MemberId = CorporateHirer.MemberId;

        //                // Check Employer Type to view some specific rights
        //                var EmployerType = udb.CorporateProfile.Find(CorporateHirer.CorporateId);


        //                List<ClientTeamMemberRights> HirerRights = (from r in udb.ClientTeamMemberRights
        //                                                            where r.UserId == userAccess.MemberId
        //                                                            select r).ToList();

        //                userAccess.ClientTeamRights = HirerRights;

        //            }
        //            else
        //            {

        //                userAccess = new ClientTeamMemberProfileView();

        //                userAccess.CorporateId = UserId;

        //                var CorporateRole = udb.ClientTeamRoles.Find(1).EmpRoleId;

        //                userAccess.EmpRoleId = CorporateRole;

        //                var RightList = (from r in udb.ClientTeamRights.Where(r => (r.GroupType == 99) && r.Visibility == true)
        //                                 select r).AsEnumerable();


        //                List<ClientTeamMemberRights> HirerRights = new List<ClientTeamMemberRights>();

        //                foreach (var right in RightList)
        //                {
        //                    HirerRights.Add(new ClientTeamMemberRights { UserId = UserId, EmpRightsId = right.EmpRightsId });
        //                }

        //                userAccess.ClientTeamRights = HirerRights;

        //            }

        //        }


        //    }
        //    return userAccess;
        //}


        //public ClientTeamMemberProfileView AuthorizeUser(short Right = 0)
        //{
        //    CorporateProfile corpo = new CorporateProfile();
        //    var role = UserManager.GetRoles(User.Identity.GetUserId()).FirstOrDefault();
        //    ClientTeamMemberProfileView userAccess = GetUserCredential(User.Identity.GetUserId(), role);

        //    if (!cmsMgr.GetCorporateExist(userAccess.CorporateId) && role != AJSolutions.DAL.Global.UserRoles.Candidate)
        //    {

        //        ViewBag.Condition = "Sorry! You are not authorized to access this page";

        //        return null;
        //    }
        //    else
        //    {
        //        if (Right > 0)
        //        {
        //            if (userAccess.ClientTeamRights.FirstOrDefault(r => r.EmpRightsId == Right) != null || role == AJSolutions.DAL.Global.UserRoles.Candidate)
        //            {
        //                //Execute controller code
        //            }
        //            else
        //            {
        //                ViewBag.Message = "Sorry! You are not authorized to access this page";
        //                //RedirectToAction("Info");
        //                return null;
        //            }
        //        }
        //    }
        //    return userAccess;
        //}




        // GET: CMS/JobOrder
        public ActionResult Index()
        {
            string UserId = User.Identity.GetUserId();
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Client")]
        public ActionResult Create(string JobOrderNumber, string result = "")
        {
            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();

            ViewBag.Result = result;
            var joborder = udb.JobOrder.Find(JobOrderNumber);

            ViewData["JOItems"] = cmsMgr.GetJobOrderItems(JobOrderNumber).AsEnumerable();

            ViewData["ItemType"] = (from i in udb.ItemTypeMasters.Where(i => i.CorporateId == userDetails.SubscriberId) select i).AsEnumerable();

            ViewData["Content"] = (from i in udb.JobOrderAttachment.Where(i => i.JobOrderNumber == JobOrderNumber) select i).FirstOrDefault();

            ViewData["ItemDuration"] = Global.GetDuration();

            if (joborder != null)
            {
                PopulateOrderType(userDetails.SubscriberId, joborder.JobOrderTypeId);
                PopulateCurrency(joborder.Currency);
                PopulateSalaryRange(joborder.SalaryRange);
                PopulateExperienceRange(joborder.ExpRange);

                if (joborder.StartDate != null)
                    ViewBag.startDate = joborder.StartDate.Value.ToString("dd-MM-yyyy");
            }
            else
            {
                PopulateOrderType(userDetails.SubscriberId);
                PopulateCurrency();
                PopulateSalaryRange();
                PopulateExperienceRange();
            }
            return View(joborder);
        }



        [HttpPost]
        //[Authorize(Roles = "Client")]
        public ActionResult Create(JobOrder jobOrder, string StartDate, string[] ItemId, string[] ItemType, string[] ItemDescription, string[] Unit, string[] UnitPrice, string[] ItemDuration, string[] Actions, string fileName, bool Status = false)
        {
            string body = " has assigned you a job order: ";
            string jobOrderNo = string.Empty;
            string useraction = "Created";
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            if (userDetails.CorporateId != null && userDetails.CorporateId != userDetails.SubscriberId)
            {
                jobOrder.ClientId = userDetails.CorporateId;
            }
            else
            {
                jobOrder.ClientId = User.Identity.GetUserId();
            }
            
            //var startDate = Convert.ToDateTime(StartDate);

            jobOrder.StartDate = null;
            if (!String.IsNullOrEmpty(StartDate))
            {
                jobOrder.StartDate = DateTime.ParseExact(StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            PopulateOrderType(userDetails.SubscriberId);
            PopulateCurrency(jobOrder.Currency);
            PopulateSalaryRange(jobOrder.SalaryRange);
            PopulateExperienceRange(jobOrder.ExpRange);

            if (string.IsNullOrEmpty(jobOrder.ClientId))
                return RedirectToAction("Login", "Account", new { area = "" });
            else
            {
                string UserId = User.Identity.GetUserId();
                var userdetails = generic.GetUserDetail(UserId);
                jobOrder.UpdatedBy = User.Identity.GetUserId();
                jobOrder.SubscriberId = userdetails.SubscriberId;
                jobOrder.JobOrderStatus = "Unassigned";

                if (!string.IsNullOrEmpty(jobOrder.JobOrderNumber))
                {
                    useraction = "Updated";
                    body = " has modified job order: ";
                }



                jobOrderNo = cmsMgr.CreatJobOrder(jobOrder, ItemId, ItemType, ItemDescription, Unit, UnitPrice, ItemDuration, Actions);

                if (!string.IsNullOrEmpty(jobOrderNo))
                {
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
                        cmsMgr.uploadFile(jobOrderNo, attachment);
                    }
                }
                string message1 = "A Job Order : " + jobOrderNo + " has been assigned to you by " + userDetails.Name; //eg "message hello ";

               // generic.sendSMSMessage(message1, generic.GetUserDetail(userdetails.SubscriberId).PhoneNumber);

                generic.sendSMS(message1, generic.GetUserDetail(userDetails.SubscriberId).PhoneNumber);
                admin.AddNotification(userdetails.SubscriberId, UserId, body + jobOrderNo, "JobOrder", jobOrderNo, Status, DateTime.Now);

                return RedirectToAction("Create", "JobOrder", new { area = "CMS", result = useraction });

            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Client")]
        public ActionResult MyJobOrders(string Id, string JobOrderStatus, string clientId, bool IsClientView, string Status, string UpdatedBy, string JobOrderNumber, DateTime? CompletedOn, int JobOrderTypeId = 0, string UserAction = "Add")
        {
            UserViewModel userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;

            ViewBag.UserId = userDetail.UserId;
            ViewData["Content"] = cmsMgr.GetJobOrderAttachments(JobOrderNumber);
            ViewData["FinalAttach"] = cmsMgr.GetJobOrderFinalAttachments(JobOrderNumber);
            PopulateStatus(JobOrderStatus);
            PopulateOrderType(userDetail.SubscriberId, JobOrderTypeId);
            PopulateClient(userDetail.SubscriberId, clientId);
            //If Client has team members with all rights
            if (userDetail.CorporateId != null && userDetail.CorporateId != userDetail.SubscriberId)
            {
                userDetail.UserId = userDetail.CorporateId;
            }
            if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Status))
            {
                if (userDetail.DepartmentId == "ADI")
                {
                    //Jobs = Jobs.Where(c => c.JobOrderTypeId == JobOrderTypeId && c.JobOrderStatus == JobOrderStatus).ToList();
                    UpdatedBy = User.Identity.GetUserId();
                    if (Status == "Completed")
                    {
                        cmsMgr.UpdateJobStatus(userDetail.SubscriberId, Id, Status, DateTime.Now, UpdatedBy, DateTime.Now);
                    }
                    else
                    {
                        cmsMgr.UpdateJobStatus(userDetail.SubscriberId, Id, Status, DateTime.Now, UpdatedBy, null);
                    }
                }
            }
            if (UserAction == "Delete" && JobOrderNumber != null)
            {
                if (cmsMgr.RemoveJobOrder(JobOrderNumber, userDetail.SubscriberId))
                {
                    admin.AddNotification(userDetail.SubscriberId, userDetail.UserId, " has deleted Job Order: " + JobOrderNumber, "JobOrderDelete", JobOrderNumber, false, DateTime.Now);
                }
            }

            if (IsClientView)
                userDetail.UserId = userDetail.UserId;
            else
                userDetail.UserId = userDetail.SubscriberId;

            var Jobs = cmsMgr.GetJobOrders(userDetail.UserId, IsClientView);

            if (!string.IsNullOrEmpty(clientId))
                Jobs = Jobs.Where(c => c.ClientId == clientId).ToList();

            if (!string.IsNullOrEmpty(JobOrderStatus))
                Jobs = Jobs.Where(c => c.JobOrderStatus == JobOrderStatus).ToList();

            if (JobOrderTypeId != 0)
                Jobs = Jobs.Where(c => c.JobOrderTypeId == JobOrderTypeId).ToList();

            return View(Jobs);
        }



        [HttpPost]
        //[Authorize(Roles = "Admin,Client")]
        public ActionResult MyJobOrders(string clientId, string JobOrderStatus, int JobOrderTypeId = 0)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetail;
            return RedirectToAction("MyJobOrders", "JobOrder", new { area = "CMS", IsClientView = true, clientId, JobOrderStatus, JobOrderTypeId });
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Client")]
        public ActionResult JobOrderDetails(string Id, string JobOrderNumber, string JobOrderStatus, string UpdatedBy, bool CV = true, string UserAction = "Add")
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewBag.UserId = UserDetail.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            //If Client has team members with all rights
            if (UserDetail.CorporateId != null && UserDetail.CorporateId != UserDetail.SubscriberId)
            {
                UserDetail.UserId = UserDetail.CorporateId;
            }

            var JOCommentsCount = new JOCommentsCount();
            ViewData["JobOrderItems"] = cmsMgr.GetJobOrderItems(Id).AsEnumerable();
            ViewData["JOComments"] = cmsMgr.GetJOComments(Id);
            ViewData["JOReplies"] = cmsMgr.GetJOReplies();
            ViewBag.userId = UserDetail.UserId;
            ViewBag.JobOrderNumber = Id;
            ViewBag.JobOrderStatus = JobOrderStatus;
            ViewBag.IsClientView = CV;
            JOCommentsCount = cmsMgr.GetJOCommentsCount(Id);
            ViewBag.CommentsCount = JOCommentsCount.TotalJOComments;
            ViewData["Content"] = cmsMgr.GetJobOrderAttachments(Id).FirstOrDefault(); //(from i in udb.JobOrderAttachment.Where(i => i.JobOrderNumber == Id) select i).FirstOrDefault();
            ViewData["FinalAttach"] = cmsMgr.GetJobOrderFinalAttachments(Id);
            if (UserAction == "Delete" && JobOrderNumber != null)
            {
                if (cmsMgr.RemoveJobOrder(JobOrderNumber, UserDetail.SubscriberId))
                {
                    admin.AddNotification(UserDetail.SubscriberId, UserDetail.UserId, " has deleted Job Order: " + JobOrderNumber, "JobOrderDelete", JobOrderNumber, false, DateTime.Now);
                }
                return RedirectToAction("MyJobOrders", "JobOrder", new { area = "CMS", IsClientView = true });
            }
            if (Id != null && JobOrderStatus != null)
            {
                UpdatedBy = User.Identity.GetUserId();
                cmsMgr.UpdateJobStatus(UserDetail.UserId, Id, JobOrderStatus, DateTime.Now, UpdatedBy, DateTime.UtcNow);
                return RedirectToAction("MyJobOrders", "JobOrder", new { area = "CMS", IsClientView = false });
            }
            else
            {
                return View(cmsMgr.GetJobOrderByJONumber(Id));
                //return View(cmsMgr.GetJobOrderDetails(UserId, Id, CV));
            }
        }

        //return View(cmsMgr.GetJobOrderDetails(User.Identity.GetUserId(), Id, CV));


        [HttpPost]
        public ActionResult JobOrderDetails(string Id, string Comment, string JOReply, string JobOrderStatus, bool CV, Int64 JOCommentId = 0)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            //If Client has team members with all rights
            if (UserDetail.CorporateId != null && UserDetail.CorporateId != UserDetail.SubscriberId)
            {
                UserDetail.UserId = UserDetail.CorporateId;
            }
            var joOrder = new JobOrderViewModel();
            joOrder = cmsMgr.GetJobOrderByJONumber(Id);
            string body = " has commented for the Job Order: ";
            bool Status = false;
            string CommentOrReplyBy = UserDetail.UserId;
            var result = false;

            if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Comment))
            {
                result = cmsMgr.AddJOComments(Id, Comment, DateTime.Now, UserDetail.UserId);
                if (UserDetail.Role == "Admin")
                {
                    admin.AddNotification(joOrder.ClientId, CommentOrReplyBy, body + Id, "JobOrder", Id, Status, DateTime.Now);
                }
                else
                {
                    admin.AddNotification(joOrder.SubscriberId, CommentOrReplyBy, body + Id, "JobOrder", Id, Status, DateTime.Now);
                }
            }
            if (JOCommentId != 0 && !string.IsNullOrEmpty(JOReply))
            {
                body = " has replied for the comment of Job Order: ";
                result = cmsMgr.AddJOReplies(JOCommentId, JOReply, DateTime.Now, UserDetail.UserId);
                if (UserDetail.Role == "Admin")
                {
                    admin.AddNotification(joOrder.ClientId, CommentOrReplyBy, body + Id, "JobOrder", Id, Status, DateTime.Now);
                }
                else
                {
                    admin.AddNotification(joOrder.SubscriberId, CommentOrReplyBy, body + Id, "JobOrder", Id, Status, DateTime.Now);
                }
            }
            return RedirectToAction("JobOrderDetails", "JobOrder", new { area = "CMS", Id = Id, JobOrderStatus = JobOrderStatus, UpdatedBy = "", CV = CV });
        }

        [HttpPost]
        public ActionResult RemoveJOComment(string Id, Int64 JOCommentId, string JobOrderStatus, bool CV)
        {
            cmsMgr.RemoveJOComment(JOCommentId);

            return Json("Succeed", JsonRequestBehavior.AllowGet);
            //return RedirectToAction("JobOrderDetails", "JobOrder", new { area = "CMS", Id = Id, JobOrderStatus = JobOrderStatus, UpdatedBy = "", CV = CV });
        }

        [HttpPost]
        public ActionResult RemoveJOReply(string Id, Int64 JOReplyId, string JobOrderStatus, bool CV)
        {
            cmsMgr.RemoveJOReply(JOReplyId);

            return Json("Succeed", JsonRequestBehavior.AllowGet);
            //return RedirectToAction("JobOrderDetails", "JobOrder", new { area = "CMS", Id = Id, JobOrderStatus = JobOrderStatus, UpdatedBy = "", CV = CV });
        }

        [HttpGet]
        public ActionResult UploadAttachment(string JobOrderNumber)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.JobOrderNumber = JobOrderNumber;
            return View();
        }

        [HttpPost]
        public ActionResult UploadAttachment(string JobOrderNumber, string fileName, string JobOrderStatus = "Completed")
        {
            if (!string.IsNullOrEmpty(JobOrderNumber))
            {
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
                    cmsMgr.uploadJobOrderFile(JobOrderNumber, attachment);
                }
            }
            if (JobOrderNumber != null && JobOrderStatus != null)
            {
                string UpdatedBy = User.Identity.GetUserId();
                cmsMgr.UpdateJobStatus(User.Identity.GetUserId(), JobOrderNumber, JobOrderStatus, DateTime.Now, UpdatedBy, DateTime.UtcNow);
            }

            return RedirectToAction("MyJobOrders", "JobOrder", new { area = "CMS", IsClientView = false });
        }

        private void PopulateOrderType(string SubscriberId, object selectedOrderType = null)
        {

            var query = generic.GetJobOrderType(SubscriberId);
            SelectList OrderTypes = new SelectList(query, "JobOrderTypeId", "JobOrderType", selectedOrderType);
            ViewBag.JobOrderTypeId = OrderTypes;
        }

        //private void PopulateOrderTypeFill(object selectedOrderType = null)
        //{

        //    var query = generic.GetJobOrderType();
        //    SelectList OrderTypes = new SelectList(query, "JobOrderTypeId", "JobOrderType", selectedOrderType);
        //    ViewBag.JobOrderTypeId = OrderTypes;
        //}

        private void PopulateClient(string SubscriberId, object selectedOrderType = null)
        {

            var query = generic.GetSubscriberWiseClientList(SubscriberId);
            SelectList OrderTypes = new SelectList(query, "CorporateId", "Name", selectedOrderType);
            ViewBag.CorporateId = OrderTypes;
        }

        private void PopulateSalaryRange(Object selectedRange = null)
        {
            var SalaryRangeList = Global.GetSalaryRange();
            SelectList SalaryRange = new SelectList(SalaryRangeList, "SalaryRange", "SalaryRange", selectedRange);
            ViewBag.SalaryRange = SalaryRange;
        }

        private void PopulateExperienceRange(Object selectRange = null)
        {
            var ExperienceRangeList = Global.GetExperienceRange();
            SelectList ExpRange = new SelectList(ExperienceRangeList, "ExpRange", "ExpRange", selectRange);
            ViewBag.ExpRange = ExpRange;
        }

        private void PopulateCurrency(Object selectedCurrency = null)
        {
            var query = generic.GetCurrency();
            SelectList CurrencyList = new SelectList(query, "Currency", "Currency", selectedCurrency);
            ViewBag.Currency = CurrencyList;
        }

        private void PopulateStatus(object selectedStatus)
        {
            var query = Global.GetStatusList();
            SelectList StatusList = new SelectList(query, "StatusType", "StatusType", selectedStatus);
            ViewBag.Status = StatusList;
        }

    }
}