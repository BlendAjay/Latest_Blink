
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using AJSolutions.DAL;
using System.Net;
using Microsoft.Owin.Security;

namespace AJSolutions.Areas.EMS.Controllers
{
    public class DashboardController : Controller
    {
        // GET: EMS/Dashboard
        EMSManager ems = new EMSManager();
        CMSManager cms = new CMSManager();
        AdminManager admin = new AdminManager();
        Generic generic = new Generic();
        //[Authorize(Roles = "Employee")]
        public ActionResult Index(bool Status = false)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;
            //ViewData["EmpInvoiceStatus"] = cms.GetEMPInvoicetatusCount(UserId);
            //ViewData["TaskStatus"] = cms.GetTaskCount(UserId);
            ViewData["TrainingStatus"] = cms.GetTrainingCount(UserId);
            ViewBag.NotificationCount = admin.SPCountNotification(UserId).TOTALNOTIFICATION;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            //var plandetail = admin.GetUserplanDetails(UserDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            //ViewData["plandetail"] = plandetail;
            if (Status == true)
            {
                ViewBag.Result = Status;
            }
            return View(admin.GetUserwiseTasksInvoicesAndTrainigsCount(UserId));
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        // POST: /Account/LogOff
        [HttpPost]

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        #endregion
    }
}
