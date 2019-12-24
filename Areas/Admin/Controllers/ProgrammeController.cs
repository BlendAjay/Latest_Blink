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
    public class ProgrammeController : Controller
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

        // GET: Admin/Programme
        //[Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            return View(db.ProgrammeMaster.ToList());
        }

        // GET: Admin/Programme/Details/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Details(string id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgrammeMaster programmeMaster = db.ProgrammeMaster.Find(id);
            if (programmeMaster == null)
            {
                return HttpNotFound();
            }
          
            return View(programmeMaster);
        }

        // GET: Admin/Programme/Create
        //[Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            PopulateModule();
         
            return View();
        }

        // POST: Admin/Programme/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Create([Bind(Include = "ProgrammeId,ProgrammeName,ModuleId")] ProgrammeMaster programmeMaster)
        {
            if (ModelState.IsValid)
            {
                db.ProgrammeMaster.Add(programmeMaster);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(programmeMaster);
        }

        // GET: Admin/Programme/Edit/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Edit(string id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgrammeMaster programmeMaster = db.ProgrammeMaster.Find(id);
            if (programmeMaster == null)
            {
                return HttpNotFound();
            }
            PopulateModule(programmeMaster.ModuleId);
          
            return View(programmeMaster);
        }

        // POST: Admin/Programme/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Edit([Bind(Include = "ProgrammeId,ProgrammeName,ModuleId")] ProgrammeMaster programmeMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(programmeMaster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(programmeMaster);
        }

        // GET: Admin/Programme/Delete/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Delete(string id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgrammeMaster programmeMaster = db.ProgrammeMaster.Find(id);
            if (programmeMaster == null)
            {
                return HttpNotFound();
            }
           
            return View(programmeMaster);
        }

        // POST: Admin/Programme/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(string id)
        {
            ProgrammeMaster programmeMaster = db.ProgrammeMaster.Find(id);
            db.ProgrammeMaster.Remove(programmeMaster);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void PopulateModule(object selectedModule = null)
        {
            Generic generic = new Generic();
            var query = generic.GetModules();
            SelectList ModuleList = new SelectList(query, "ModuleId", "Module", selectedModule);
            ViewBag.ModuleId = ModuleList;
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
