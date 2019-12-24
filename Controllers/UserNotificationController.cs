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


namespace AJSolutions.Controllers
{
    public class UserNotificationController : Controller
    {
        Generic generic = new Generic();
        AdminManager admin = new AdminManager();
        UserDBContext userContext = new UserDBContext();
        EMSManager ems = new EMSManager();
        CMSManager cms = new CMSManager();
        // GET: UserNotification

        [HttpGet]
        public ActionResult Notifications(string Id = "", string UserAction = "Add", long NotificationId = 0, string NType = null)
        {
            string UserId = User.Identity.GetUserId();

            var userdetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            if (userdetails.Role == "Employee")
            {
                var confirmationdate = userContext.EmpJoiningDetail.Where(c => c.UserId == UserId).Select(c => c.ConfirmationDate).FirstOrDefault();
                ViewBag.Date = DateTime.Now.Date;
                if (confirmationdate != null)
                {
                    ViewBag.cdate = confirmationdate.Value.Date;
                }

            }
            //for checking employeeconfirmation details
            ViewData["ConfirmEmployee"] = userContext.EmployeeConfirmation.Where(c => c.ApprovedBy == UserId).ToList();
            ViewData["plandetail"] = plandetail;
            if (UserAction == "Delete" && NotificationId != 0)
            {
                admin.RemoveNotification(NotificationId);
                return RedirectToAction("Notifications", "UserNotification");
            }
            if (NotificationId != 0)
                admin.UpdateNotification(NotificationId, DateTime.Now);
            if (NType == "JobOrder")
                return RedirectToAction("JobOrderDetails", "JobOrder", new { area = "CMS", id = Id, CV = false });
            else if (NType == "Task")
                return RedirectToAction("TaskDetails", "Task", new { area = "CMS", Id = Id });

            List<UserNotificationView> Details = admin.GetUserNotification(UserId);
            return View(Details);
        }


    }
}