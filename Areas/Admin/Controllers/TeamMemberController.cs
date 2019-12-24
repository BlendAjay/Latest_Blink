using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AJSolutions.Models;
using Microsoft.AspNet.Identity;
using AJSolutions.DAL;
using Microsoft.Owin.Security;
using System.Globalization;

namespace AJSolutions.Areas.Admin.Controllers
{
    public class TeamMemberController : Controller
    {
        Generic generic = new Generic();
        AdminManager admin = new AdminManager();

        private UserDBContext db = new UserDBContext();
        // GET: Admin/TeamMember
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public ActionResult TeamRoles(Int16 Id = 0, string UserAction = "Add")
        {
            string UserId = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetail;
            var Roles = admin.GetempRoles();
            ViewData["Roles"] = Roles;
            if (UserAction == "Delete" &&  Id > 0)
            {
                admin.Deleteroles(Id);
                return View();
            }

            var rolesId = Roles.Where(i => i.EmpRoleId == Id).FirstOrDefault();
            return View(rolesId);
        }

        [HttpPost]
        //[Authorize(Roles = "Candidate")]
        public ActionResult TeamRoles(ClientTeamRoles ClientTeamRoles)
        {
            var UserId = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(UserId);
            bool result = admin.AddEmproles(ClientTeamRoles.EmpRoleId, ClientTeamRoles.EmpRole, ClientTeamRoles.Visibility, DateTime.Now, UserId);
           
            return RedirectToAction("TeamRoles", "TeamMember", new { Area = "Admin"});
        }

        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public ActionResult TeamRights(Int16 Id = 0, string UserAction = "Add")
        {
            string UserId = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetail;
            var Rights = admin.GetempRights();
            ViewData["Rights"] = Rights;
            var rightsId = Rights.Where(i => i.EmpRightsId == Id).FirstOrDefault();
            if (UserAction == "Delete" && Id > 0)
            {
                admin.Deleterights(Id);
                return View();
            }

            return View(rightsId);
        }

        [HttpPost]
        //[Authorize(Roles = "Candidate")]
        public ActionResult TeamRights(ClientTeamRights ClientTeamRights)
        {
            var UserId = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(UserId);
            bool result = admin.AddEmprights(ClientTeamRights.EmpRightsId, ClientTeamRights.Rights, ClientTeamRights.GroupType, ClientTeamRights.Visibility, DateTime.Now, UserId);

            return RedirectToAction("TeamRights", "TeamMember", new { Area = "Admin" });
        }
    }
}