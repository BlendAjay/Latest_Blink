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
    public class DepartmentController : Controller
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

        // GET: Admin/Department
        //[Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            return View(db.DepartmentMaster.ToList());
        }

        // GET: Admin/Department/Details/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Details(string id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentMaster departmentMaster = db.DepartmentMaster.Find(id);
            if (departmentMaster == null)
            {
                return HttpNotFound();
            }
          
            return View(departmentMaster);
        }

        // GET: Admin/Department/Create
        //[Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            PopulateRoles();
            return View();
        }

        // POST: Admin/Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Create([Bind(Include = "DepartmentId,Department,RoleId,Role,Isvisible")] DepartmentMaster departmentMaster)
        {
            if (ModelState.IsValid)
            {
                db.DepartmentMaster.Add(departmentMaster);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(departmentMaster);
        }

        // GET: Admin/Department/Edit/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Edit(string id,string roleId)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentMaster departmentMaster = db.DepartmentMaster.Find(id);
            if (departmentMaster == null)
            {
                return HttpNotFound();
            }
          
            PopulateRoles(roleId);
            return View(departmentMaster);
        }

        // POST: Admin/Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Edit([Bind(Include = "DepartmentId,Department,RoleId,Isvisible")] DepartmentMaster departmentMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(departmentMaster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(departmentMaster);
        }

        // GET: Admin/Department/Delete/5
        //[Authorize(Roles = "Administrator")]
        public ActionResult Delete(string id)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentMaster departmentMaster = db.DepartmentMaster.Find(id);
            if (departmentMaster == null)
            {
                return HttpNotFound();
            }
         
            return View(departmentMaster);
        }

        // POST: Admin/Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(string id)
        {
            DepartmentMaster departmentMaster = db.DepartmentMaster.Find(id);
            db.DepartmentMaster.Remove(departmentMaster);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Summary: 
        // Populate SpecificSkills with single option
        private void PopulateRoles(object selectedRole = null)
        {
            Generic generic = new Generic();
            var query = generic.GetRoles();
            SelectList RoleList = new SelectList(query, "RoleId", "Role", selectedRole);
            ViewBag.RoleId = RoleList;
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

