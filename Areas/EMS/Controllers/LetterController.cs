using AJSolutions.DAL;
using AJSolutions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;


namespace AJSolutions.Areas.EMS.Controllers
{
    public class LetterController : Controller
    {
        Generic generic = new Generic();
        AdminManager admin = new AdminManager();
        EMSManager ems = new EMSManager();
        CMSManager cms = new CMSManager();
        UserDBContext db = new UserDBContext();
        // GET: EMS/Letter
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

        public ActionResult Index(int? page, int PageSize = 10)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["UserProfile"] = userDetail;
            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            var resignation = ems.GetResignation(userDetail.UserId, "Admin").ToPagedList(pageNumber, pageSize);
            if (userDetail.Role == "Employee")
            {
                var empdetails = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
                ViewData["EmpDetails"] = empdetails;
                ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
                resignation = ems.GetResignation(userDetail.UserId, "Admin").ToPagedList(pageNumber, pageSize);

            }
            else
            {
                resignation = ems.GetResignation(userDetail.SubscriberId, "Admin").ToPagedList(pageNumber, pageSize);
            }


            return View(resignation);
        }


        public ActionResult Resignation(int? page, int PageSize = 10)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId);
            var empdetails = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["EmpDetails"] = empdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["UserProfile"] = userDetail;
            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            if (userDetail.Role == "Employee")
            {
               
                ViewData["Resignation"] = ems.GetEmployeeResignation().Where(e=>e.UserId==UserId).ToPagedList(pageNumber, pageSize);
            }
            PopulateLeavingReason(userDetail.SubscriberId);

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Resignation(ResignationViewModel reg, Int64 ReasonId, string RelievingReason)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId); 
               
            
                var empjoiningdetails = db.EmpJoiningDetail.Where(e => e.UserId == UserId).FirstOrDefault();
                var dateofresignation = DateTime.UtcNow;
                DateTime lastworkingday = dateofresignation.AddDays(empjoiningdetails.NoticePeriod);
                reg.DateofResignation = dateofresignation;
                reg.RelievingReason = RelievingReason;
                reg.ReasonId = ReasonId;
                reg.LastWorkingDate = lastworkingday;
                reg.Status = 0;
                reg.UserId = UserId;
                reg.AprrovedOn = DateTime.UtcNow;
               bool res = ems.Addresignation(reg);
               if (userDetail.Role != "Admin")
               {
                   string callbackUrl = await SendResignationEmailTokenAsync(userDetail.ReportingAuthority, userDetail.ReportingAuthorityname,userDetail.Name,reg.Reason,reg.RelievingReason,reg.DateofResignation,"Req");
               }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        private async Task<string> SendResignationEmailTokenAsync(string UserId, string UserName, string Employeename, string Reason,string msg,DateTime ResignationDate,string Purpose)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(UserId);
            var msgBody = "";
            var subject = "";
            var callbackUrl = Url.Action("Index", "Letter",
               new { area = "EMS", userId = UserId, code = code }, protocol: Request.Url.Scheme);
            if(Purpose=="Req")
            {
                subject = "Resignation";
                msgBody = "Hello  " + UserName + ", <br/> <br/>" +
                " You received Resignation request from " + Employeename + "  ." +
                "<br/><br/><b>Reason:</b> " + Reason +
                "<br/><br/><b>Message:</b> " + msg +
                 "<br/><br/><b>Resignation Date:</b> " + ResignationDate +
                 "<br/><br/>Thanks & Regards" +
                "<br/>RECKONN";
                
            }
            else if (Purpose=="Cen")
            {
                subject = "Canceled Resignation";
                msgBody = "Hello  " + UserName + ", <br/> <br/>" +
               " You received Cancel Resignation  request from " + Employeename + "  ." + 
                "<br/><br/>Thanks & Regards" +
               "<br/>RECKONN";
            }


            await UserManager.SendEmailAsync(UserId,subject,msgBody);

            return callbackUrl;
        }
        public async Task<ActionResult> CencelResignation(Int64 ResignationId = 0)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId); 
            var query = (from q in db.Resignation
                         where q.UserId == UserId && q.ResignationId == ResignationId
                         select q).First();
            query.Status = 3;
            db.SaveChanges();
            if (userDetail.Role != "Admin")
            {
                string callbackUrl = await SendResignationEmailTokenAsync(userDetail.ReportingAuthority, userDetail.ReportingAuthorityname, userDetail.Name, ""," ", DateTime.Now, "Cen");
            }
            return RedirectToAction("Resignation", "Letter", new { Areas = "EMS" });
        }
        private void PopulateLeavingReason(string SubscriberId, object selectedValue = null)
        {
            var query = admin.GetLeavingReason(SubscriberId);

            SelectList LeavingReason = new SelectList(query, "ReasonId", "Reason", selectedValue);
            ViewBag.LeavingReason = LeavingReason;
        }
        //for approvel
        [HttpPost]
        //[Authorize(Roles = "Employee,Admin")]
        public ActionResult ApproveResignation(string UserId, Int64 ResignationId, string action)
        {
            string userId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(userId);
            if (action == "App")
            {
                var query = (from q in db.Resignation
                             where q.UserId == UserId && q.ResignationId == ResignationId
                             select q).First();
                query.Status = 1;
                query.ApprovedBy = userId;
                query.AprrovedOn = DateTime.UtcNow;
                db.SaveChanges();
            }
            else
            {
                var query = (from q in db.Resignation
                             where q.UserId == UserId && q.ResignationId == ResignationId
                             select q).First();
                query.Status = 2;
                query.ApprovedBy = userId;
                query.AprrovedOn = DateTime.UtcNow;
                db.SaveChanges();
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        private void PopulatePaging(object selectedValue = null)
        {

            var PageList = generic.GetPaging();
            ViewBag.PageSize = new SelectList(PageList, "PageSize", "PageSize", selectedValue);
        }
    }
}