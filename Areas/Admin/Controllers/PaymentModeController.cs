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
    public class PaymentModeController : Controller
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

        // GET: TMS/PaymentMode
        //[Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            return View(db.PaymentModeMaster.ToList());
        }

        // GET: TMS/PaymentMode/Details/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Details(short? id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentModeMaster paymentModeMaster = db.PaymentModeMaster.Find(id);
            if (paymentModeMaster == null)
            {
                return HttpNotFound();
            }
          
            return View(paymentModeMaster);
        }

        // GET: TMS/PaymentMode/Create
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

        // POST: TMS/PaymentMode/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Create([Bind(Include = "PaymentModeId,PaymentMode")] PaymentModeMaster paymentModeMaster)
        {
            if (ModelState.IsValid)
            {
                db.PaymentModeMaster.Add(paymentModeMaster);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(paymentModeMaster);
        }

        // GET: TMS/PaymentMode/Edit/5
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
            PaymentModeMaster paymentModeMaster = db.PaymentModeMaster.Find(id);
            if (paymentModeMaster == null)
            {
                return HttpNotFound();
            }
          
            return View(paymentModeMaster);
        }

        // POST: TMS/PaymentMode/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Edit([Bind(Include = "PaymentModeId,PaymentMode")] PaymentModeMaster paymentModeMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paymentModeMaster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(paymentModeMaster);
        }

        // GET: TMS/PaymentMode/Delete/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Delete(short? id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentModeMaster paymentModeMaster = db.PaymentModeMaster.Find(id);
            if (paymentModeMaster == null)
            {
                return HttpNotFound();
            }
          
            return View(paymentModeMaster);
        }

        // POST: TMS/PaymentMode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(short id)
        {
            PaymentModeMaster paymentModeMaster = db.PaymentModeMaster.Find(id);
            db.PaymentModeMaster.Remove(paymentModeMaster);
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
