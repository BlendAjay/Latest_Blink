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
    public class BankDetailsController : Controller
    {
        private UserDBContext db = new UserDBContext();
        Generic generic = new Generic();
        // GET: Admin/BankDetails
        public ActionResult Index()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            var bankDetails = db.BankDetails;
            return View(bankDetails.ToList());
        }

        // GET: Admin/BankDetails/Details/5
        public ActionResult Details(string id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankDetails bankDetails = db.BankDetails.Find(id);
            if (bankDetails == null)
            {
                return HttpNotFound();
            }
            return View(bankDetails);
        }

        // GET: Admin/BankDetails/Create
        public ActionResult Create()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = new SelectList(db.UserProfile, "UserId", "Name");
            return View();
        }

        // POST: Admin/BankDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,BankName,AccountNumber,AccountOwner,IfscCode,BranchCode,BranchAddress,ContactNumber")] BankDetails bankDetails)
        {
            if (ModelState.IsValid)
            {
                db.BankDetails.Add(bankDetails);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.UserProfile, "UserId", "Name", bankDetails.CorporateId);
            return View(bankDetails);
        }

        // GET: Admin/BankDetails/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankDetails bankDetails = db.BankDetails.Find(id);
            if (bankDetails == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.UserProfile, "UserId", "Name", bankDetails.CorporateId);
            return View(bankDetails);
        }

        // POST: Admin/BankDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,BankName,AccountNumber,AccountOwner,IfscCode,BranchCode,BranchAddress,ContactNumber")] BankDetails bankDetails)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankDetails).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.UserProfile, "UserId", "Name", bankDetails.CorporateId);
            return View(bankDetails);
        }

        // GET: Admin/BankDetails/Delete/5
        public ActionResult Delete(string id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankDetails bankDetails = db.BankDetails.Find(id);
            if (bankDetails == null)
            {
                return HttpNotFound();
            }
            return View(bankDetails);
        }

        // POST: Admin/BankDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            BankDetails bankDetails = db.BankDetails.Find(id);
            db.BankDetails.Remove(bankDetails);
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
