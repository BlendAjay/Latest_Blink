
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
    public class AddressesController : Controller
    {
        private UserDBContext db = new UserDBContext();
        Generic generic = new Generic();
        // GET: Admin/Addresses
        public ActionResult Index()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            var address = db.Address.Include(a => a.CorporateProfile);
            return View(address.ToList());
        }

        // GET: Admin/Addresses/Details/5
        public ActionResult Details(string id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = db.Address.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        // GET: Admin/Addresses/Create
        public ActionResult Create()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = new SelectList(db.UserProfile, "UserId", "Name");
            return View();
        }

        // POST: Admin/Addresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,AddressType,AddressLine1,AddressLine2,City,State,PostalCode,Country,FaxNo")] Address address)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (ModelState.IsValid)
            {
                db.Address.Add(address);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.UserProfile, "UserId", "Name", address.CorporateId);
            return View(address);
        }

        // GET: Admin/Addresses/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = db.Address.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.UserProfile, "UserId", "Name", address.CorporateId);
            return View(address);
        }

        // POST: Admin/Addresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,AddressType,AddressLine1,AddressLine2,City,State,PostalCode,Country,FaxNo")] Address address)
        {
            if (ModelState.IsValid)
            {
                db.Entry(address).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.UserProfile, "UserId", "Name", address.CorporateId);
            return View(address);
        }

        // GET: Admin/Addresses/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = db.Address.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        // POST: Admin/Addresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Address address = db.Address.Find(id);
            db.Address.Remove(address);
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
