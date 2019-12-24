using AJSolutions.DAL;
using AJSolutions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.Areas.CMS.Models;
using System.Globalization;

namespace AJSolutions.Areas.CMS.Controllers
{
    public class ClientController : Controller
    {
        Generic generic = new Generic();
        AdminManager admin = new AdminManager();
        CMSManager cms = new CMSManager();

        private ApplicationUserManager _userManager;

        // GET: CMS/Client
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IppbReport()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddTeamMember(string MemberId)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();

            ViewBag.MemberId = MemberId;
            PopulateTeamRoles();
            ViewData["Rights"] = admin.GetempRights();
            ViewData["TeamMember"] = cms.GetTeamMember(UserId).OrderBy(c => c.Name);
            if (MemberId != null)
            {
                ClientTeamMemberProfileView Team = cms.GetTeamMember(UserId, MemberId).FirstOrDefault();
                var Trights = cms.GetTeamMemberRights(MemberId).AsEnumerable();
                Team.ClientTeamRights = Trights;
                ViewData["TeamRights"] = Trights;
                PopulateTeamRoles(Team.EmpRoleId);
                return View(Team);
            }
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> AddTeamMember(string MemberId, string Name, string EmailId, string PhoneNumber, string Designation, IEnumerable<Int16> EmpRightsId, Int16 EmpRoleId = 0)
        {
            string CorporateId = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(CorporateId);
            var ClientDetail = cms.GetCorporateProfile(CorporateId).FirstOrDefault();
            if (String.IsNullOrEmpty(MemberId))
            {
                //User Add Mode
                string userName = admin.GenerateUserName();
                var user = new ApplicationUser { UserName = userName, Email = EmailId, PhoneNumber = PhoneNumber, EmailConfirmed = true };

                var result = await UserManager.CreateAsync(user, "changeme");

                if (result.Succeeded)
                {
                    string RoleId = "Client";

                    var status = UserManager.AddToRole(user.Id, RoleId);
                    if (status.Succeeded)
                    {
                        bool add = cms.AddTeamMember(user.Id, CorporateId, UserDetail.SubscriberId, Name, EmailId, PhoneNumber, EmpRoleId, Designation, DateTime.UtcNow, CorporateId);
                        string callbackUrl = await SendEmailConfirmationTokenAsync(ClientDetail.Name, user.Id, "Account activation", userName, PhoneNumber, Name);
                    }
                    if (EmpRightsId != null)
                    {
                        foreach (var right in EmpRightsId)
                        {
                            var success = cms.AddTeamMemberRights(user.Id, right, DateTime.UtcNow, CorporateId);
                        }
                    }
                }
            }
            else
            {
                var regUser = UserManager.FindById(MemberId);
                if (regUser != null)
                {
                    bool result = admin.UpdateUserEmailPhone(regUser.UserName, EmailId, PhoneNumber, true);
                    bool add = cms.AddTeamMember(MemberId, CorporateId, UserDetail.SubscriberId, Name, EmailId, PhoneNumber, EmpRoleId, Designation, DateTime.UtcNow, CorporateId);
                }
                foreach (var right in EmpRightsId)
                {
                    var success = cms.AddTeamMemberRights(MemberId, right, DateTime.UtcNow, CorporateId);
                }
            }
            return RedirectToAction("AddTeamMember", "Client");
        }

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


        private async Task<string> SendEmailConfirmationTokenAsync(string SubScriber, string userID, string subject, string userName, string phoneNumber, string Name = "User")
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { area = "", userId = userID, code = code }, protocol: Request.Url.Scheme);
            var msgBody = "Dear " + Name + " <br/> <br/>" + SubScriber +
                " has added you as their USER in RECKONN. Your User Name is " + userName + " and Phone Number is " + phoneNumber + "." +
                "<br><br> <a href='" + callbackUrl + "' > CLICK HERE</a> to Verify your email." +
                "<br/><br/>You can login to your account using the password 'changeme'." +
            "<br/><br/>RECKONN";
            //   "<br/> Token will be valid for 48 hours. To regenerate token go to" + " <a href='http://www.jobenablers.com' target='_blank'>Login</a>" + " and put your credentials then it will regenerate your token.";

            //  msgBody = generic.AllEmailFormat(msgBody, callbackUrl, "Verify Now", "Dear", Name, "Compulsary", "Failure to verify your account within 15 days may lead to removal of your registration from our database.", "");

            await UserManager.SendEmailAsync(userID, subject, msgBody);

            return callbackUrl;
        }

        [HttpPost]
        public async Task<ActionResult> ResendToken(string UserId, string UserName)
        {
            string callbackUrl = await SendEmailConfirmationTokenAsync(UserId, UserName);
            return Json(callbackUrl, JsonRequestBehavior.AllowGet);
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string userName)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { area = "", userId = userID, code = code }, protocol: Request.Url.Scheme);
            var msgBody = "Dear " + userName + ", <br/> <br/>" +
                " Your email verification is pending." +
                "<br><br> <a href='" + callbackUrl + "' > CLICK HERE</a> to Verify your email." +
            "<br/><br/>RECKONN";

            await UserManager.SendEmailAsync(userID, "Email Verification", msgBody);

            return callbackUrl;
        }

        private void PopulateTeamRoles(object selectedRoles = null)
        {
            var query = admin.GetempRoles();
            SelectList EmpRoleId = new SelectList(query, "EmpRoleId", "EmpRole", selectedRoles);
            ViewBag.EmpRoleId = EmpRoleId;
        }
    }
}