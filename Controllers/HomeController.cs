using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AJSolutions.DAL;
using AJSolutions.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Net;
using System.Web.Script.Serialization;

namespace AJSolutions.Controllers
{
    public class HomeController : Controller
    {
        AdminManager admin = new AdminManager();
        Generic generic = new Generic();
        UserDBContext udb = new UserDBContext();
        TMSManager tms = new TMSManager();
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


        public ActionResult Index(string CandidateId, string PublicationId, string TrainingId)
        {
            try
            {

                if (!string.IsNullOrEmpty(CandidateId) && !string.IsNullOrEmpty(PublicationId) && !string.IsNullOrEmpty(TrainingId))
                {
                    var AsstTraining = udb.TrainingAssessment.Where(t => t.PublicationId == PublicationId &&
                                                                    t.TrainingId == TrainingId).FirstOrDefault();
                    int marks = 0;

                    using (WebClient webClient = new System.Net.WebClient())
                    {
                        try
                        {
                            string URL = Global.PreloreUrl() + "/Api/Value/GetPercentage?CandidateId=" + CandidateId + "&PublicationId=" + PublicationId + "&TrainingId=" + TrainingId;
                            var json = webClient.DownloadString(URL);
                            JavaScriptSerializer js = new JavaScriptSerializer();
                            float myData = js.Deserialize<float>(json);
                            marks = Convert.ToInt32(Math.Round(myData, 0));
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }
                    var result = tms.AddEvaluationMarks(AsstTraining.AssessmentId, CandidateId, TrainingId, marks, DateTime.Now, CandidateId);
                    return Redirect(Global.PreloreUrl() + "Admin/Admin/CandidateResults?CandidateId=" + CandidateId +
                                    "&PublicationId=" + PublicationId + "&TrainingId=" + TrainingId);
                }

                string role = GetUserRole(User.Identity.GetUserId());
                ViewBag.UserRole = role;
                ViewBag.UserName = GetUserName(User.Identity.GetUserId(), role);
                ViewBag.DeptId = GetDeptId(User.Identity.GetUserId(), role);
                //ViewBag.url = Global.WikipianUrl();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { error = ex.ToString() });
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Test()
        {
            PopulateCountry();
            PopulateState();
            return View();
        }

        [HttpPost]
        public ActionResult GetState(string CountryId)
        {
            int countryId;
            List<SelectListItem> StateId = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(CountryId))
            {
                countryId = Convert.ToInt32(CountryId);
                List<StatesMaster> States = udb.StatesMaster.Where(x => x.CountryId == countryId).ToList();
                States.ForEach(x =>
                {
                    StateId.Add(new SelectListItem { Text = x.State, Value = x.StateId.ToString() });
                });
            }
            return Json(StateId, JsonRequestBehavior.AllowGet);
        }

        private void PopulateCountry(object selectedCountry = null)
        {

            var query = generic.GetCountry();
            SelectList Countries = new SelectList(query, "CountryId", "Country", selectedCountry);
            ViewBag.CountryId = Countries;
        }

        private void PopulateState(int CountryId = 0, object selectedState = null)
        {
            var query = generic.GetState(CountryId);
            SelectList States = new SelectList(query, "StateId", "State", selectedState);
            ViewBag.StateId = States;
        }

        private string GetUserName(string UserId, string Role)
        {
            CMSManager cms = new CMSManager();
            EMSManager ems = new EMSManager();
            Student candidate = new Student();

            if (Role.Equals("Admin") || Role.Equals("Client") || Role.Equals("Administrator"))
                return cms.GetUserName(UserId);
            else if (Role.Equals("Employee"))
                return ems.GetUserName(UserId);
            else if (Role.Equals("Candidate"))
                return candidate.GetUserName(UserId);

            return string.Empty;
        }

        private string GetUserRole(string UserId)
        {
            if (!String.IsNullOrEmpty(UserId))
            {
                if (UserManager.IsInRole(UserId, "Administrator"))
                    return "Administrator";
                else if (UserManager.IsInRole(UserId, "Admin"))
                    return "Admin";
                else if (UserManager.IsInRole(UserId, "Client") || UserManager.IsInRole(UserId, "Partner"))
                    return "Client";
                else if (UserManager.IsInRole(UserId, "Employee"))
                    return "Employee";
                else if (UserManager.IsInRole(UserId, "Candidate"))
                    return "Candidate";
            }
            return string.Empty;
        }

        private string GetDeptId(string UserId, string Role)
        {
            CMSManager cms = new CMSManager();

            if (Role.Equals("Admin") || Role.Equals("Client") || Role.Equals("Administrator"))
                return cms.GetCorporateProfile(UserId).FirstOrDefault().DepartmentId;

            return string.Empty;
        }
    }
}