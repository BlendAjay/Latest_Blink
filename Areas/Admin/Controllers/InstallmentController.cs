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
    public class InstallmentController : Controller
    {
        private UserDBContext db = new UserDBContext();
        CMSManager cms = new CMSManager();
        Generic generic = new Generic();

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

        // GET: TMS/Installment
        //[Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            return View(db.InstallmentMaster.ToList());
        }

        // GET: TMS/Installment/Details/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Details(short? id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstallmentMaster installmentMaster = db.InstallmentMaster.Find(id);
            if (installmentMaster == null)
            {
                return HttpNotFound();
            }
           
            return View(installmentMaster);
        }

        // GET: TMS/Installment/Create
        //[Authorize(Roles = "Administrator")]
        public ActionResult Create(bool status = false)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
         
            return View();
        }

        // POST: TMS/Installment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Create([Bind(Include = "InstallmentId,Installment")] InstallmentMaster installmentMaster)
        {
            if (ModelState.IsValid)
            {
                db.InstallmentMaster.Add(installmentMaster);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(installmentMaster);
        }

        // GET: TMS/Installment/Edit/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Edit(short? id, bool status = false)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            InstallmentMaster installmentMaster = db.InstallmentMaster.Find(id);
            if (installmentMaster == null)
            {
                return HttpNotFound();
            }
           
            return View(installmentMaster);
        }

        // POST: TMS/Installment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Edit([Bind(Include = "InstallmentId,Installment")] InstallmentMaster installmentMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(installmentMaster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(installmentMaster);
        }

        // GET: TMS/Installment/Delete/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Delete(short? id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstallmentMaster installmentMaster = db.InstallmentMaster.Find(id);
            if (installmentMaster == null)
            {
                return HttpNotFound();
            }
          
            return View(installmentMaster);
        }

        // POST: TMS/Installment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(short id)
        {
            InstallmentMaster installmentMaster = db.InstallmentMaster.Find(id);
            db.InstallmentMaster.Remove(installmentMaster);
            db.SaveChanges();
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
