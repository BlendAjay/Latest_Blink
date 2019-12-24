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

namespace AJSolutions.Areas.Admin.Controllers
{
    public class JobOrderTypeController : Controller
    {
        private UserDBContext db = new UserDBContext();
        CMSManager cms = new CMSManager();
        EMSManager ems = new EMSManager();
        Generic generic = new Generic();
        AdminManager admin = new AdminManager();

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

        // GET: Admin/JobOrderType
        //[Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            //ViewBag.UserId = User.Identity.GetUserId();
            //ViewBag.UserName = cms.GetUserName(User.Identity.GetUserId());
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetail.UserId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            return View(db.JobOrderTypeMaster.Where( j => j.CorporateId  == userDetail.SubscriberId).ToList());
        }

        // GET: Admin/JobOrderType/Details/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobOrderTypeMaster jobOrderTypeMaster = db.JobOrderTypeMaster.Find(id);
            if (jobOrderTypeMaster == null)
            {
                return HttpNotFound();
            }
            //ViewBag.UserId = User.Identity.GetUserId();
            //ViewBag.UserName = cms.GetUserName(User.Identity.GetUserId());
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetail.UserId).FirstOrDefault();
            return View(jobOrderTypeMaster);
        }

        // GET: Admin/JobOrderType/Create
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult Create()
        {
            //ViewBag.UserId = User.Identity.GetUserId();
            //ViewBag.UserName = cms.GetUserName(User.Identity.GetUserId());
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetail.UserId).FirstOrDefault();
            return View();
        }

        // POST: Admin/JobOrderType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult Create([Bind(Include = "JobOrderType")] JobOrderTypeMaster jobOrderTypeMaster)
        {
            if (ModelState.IsValid)
            {
                AdminManager admin = new AdminManager();
                admin.AddJobOrderType(jobOrderTypeMaster.JobOrderType, generic.GetUserDetail(User.Identity.GetUserId()).SubscriberId);
                return RedirectToAction("Index");
            }

            return View(jobOrderTypeMaster);
        }

        // GET: Admin/JobOrderType/Edit/5
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobOrderTypeMaster jobOrderTypeMaster = db.JobOrderTypeMaster.Find(id);
            if (jobOrderTypeMaster == null)
            {
                return HttpNotFound();
            }
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetail.UserId).FirstOrDefault();
            return View(jobOrderTypeMaster);
        }

        // POST: Admin/JobOrderType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult Edit([Bind(Include = "JobOrderTypeId,JobOrderType,CorporateId")] JobOrderTypeMaster jobOrderTypeMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jobOrderTypeMaster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(jobOrderTypeMaster);
        }

        // GET: Admin/JobOrderType/Delete/5
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult Delete(int? id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobOrderTypeMaster jobOrderTypeMaster = db.JobOrderTypeMaster.Find(id);
            if (jobOrderTypeMaster == null)
            {
                return HttpNotFound();
            }
           
            return View(jobOrderTypeMaster);
        }

        // POST: Admin/JobOrderType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult DeleteConfirmed(int id)
        {
           
            JobOrderTypeMaster jobOrderTypeMaster = db.JobOrderTypeMaster.Find(id);
            db.JobOrderTypeMaster.Remove(jobOrderTypeMaster);
            db.SaveChanges();
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.UserName = cms.GetUserName(User.Identity.GetUserId());
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
