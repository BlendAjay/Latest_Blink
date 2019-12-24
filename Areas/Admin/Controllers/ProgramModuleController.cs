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
    public class ProgramModuleController : Controller
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

        // GET: Admin/ProgramModule
        //[Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
          
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;

            return View(db.ModuleMaster.ToList());
        }

        // GET: Admin/ProgramModule/Details/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Details(string id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleMaster moduleMaster = db.ModuleMaster.Find(id);
            if (moduleMaster == null)
            {
                return HttpNotFound();
            }
        
            return View(moduleMaster);
        }

        // GET: Admin/ProgramModule/Create
        //[Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            return View();
        }

        // POST: Admin/ProgramModule/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Create([Bind(Include = "ModuleId,Module")] ModuleMaster moduleMaster)
        {
            if (ModelState.IsValid)
            {
                db.ModuleMaster.Add(moduleMaster);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(moduleMaster);
        }

        // GET: Admin/ProgramModule/Edit/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Edit(string id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleMaster moduleMaster = db.ModuleMaster.Find(id);
            if (moduleMaster == null)
            {
                return HttpNotFound();
            }
           
            return View(moduleMaster);
        }

        // POST: Admin/ProgramModule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Edit([Bind(Include = "ModuleId,Module")] ModuleMaster moduleMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(moduleMaster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(moduleMaster);
        }

        // GET: Admin/ProgramModule/Delete/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Delete(string id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleMaster moduleMaster = db.ModuleMaster.Find(id);
            if (moduleMaster == null)
            {
                return HttpNotFound();
            }
        
            return View(moduleMaster);
        }

        // POST: Admin/ProgramModule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(string id)
        {
            ModuleMaster moduleMaster = db.ModuleMaster.Find(id);
            db.ModuleMaster.Remove(moduleMaster);
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
