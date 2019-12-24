using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using AJSolutions.DAL;
using Microsoft.Owin.Security;
using AJSolutions.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Net;

namespace AJSolutions.Areas.Candidate.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Candidate/Dashboard

        Student student = new Student();
        Generic generic = new Generic();
        UserDBContext db = new UserDBContext();
        LMSManager lms = new LMSManager();
        CMSManager cms = new CMSManager();
        AdminManager admin = new AdminManager();
        TMSManager tms = new TMSManager();

        //[Authorize(Roles = "Candidate")]
        public ActionResult Index()
        {
            string UserId = User.Identity.GetUserId();

            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;

            ViewData["CandidateCourse"] = student.GetCandidateWiseCourseDetail(UserId, true);
            ViewData["LMSCourseMasters"] = lms.GetLMSCourseMasters(userdetails.SubscriberId);
            ViewData["CandidateTrngAssmt"] = tms.GetCandidateTrainingAssessments(UserId);
            return View();
        }

        public ActionResult UpdateLikes(string BatchId)
        {
            string UserId = User.Identity.GetUserId();

            bool status = student.UpdateCandidateLikesforCourse(UserId, BatchId);

            return RedirectToAction("Index", "Dashboard", new { area = "Candidate" });
        }

        //[Authorize(Roles = "Candidate")]
        public ActionResult CourseContent(string Id, int CurrentSno = 0, string Result = "NA")
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = UserId;
            ViewData["UserProfile"] = userDetails;
            string LMSCourseCode = tms.IsCourseIntegrated(Id);
            ViewBag.Result = Result;
            ViewBag.CourseCode = Id;
            ViewBag.LMSCourseCode = LMSCourseCode;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            LMS.Models.TopicLecturesView Lecture = new LMS.Models.TopicLecturesView();
            var TopicLetures = lms.GetCandidateTopicLecture(LMSCourseCode, UserId).OrderBy(rowdata => rowdata.TopicId).ToList();
            var AdminSettings = lms.GetNavigationSettings(LMSCourseCode);
            ViewBag.RandomNavigation = AdminSettings.RandomNavigation;
            ViewBag.ShowNavigation = AdminSettings.ShowNavigation;
            int sno = 0;

            ViewBag.LectureId = "NA";
            ViewBag.FirstSno = 1;
            int NC = 0;
            Int64 IncompleteSno = 0;
            
            for (int temp = 0; temp < TopicLetures.Count; temp++)
            {
                sno = sno + 1;
                TopicLetures[temp].SNo = sno;
                ViewBag.LastSno = sno;
                if(TopicLetures[temp].Status == "Not Completed")
                {
                    NC++;
                    if(NC==1)
                    {
                        IncompleteSno = TopicLetures[temp].SNo;
                        ViewBag.LastSno = IncompleteSno;
                    }
                }
            }
            
            if(TopicLetures.Count != 0)
            {
                Lecture = TopicLetures[0];
            }
            ViewBag.CurrentSno = 1;
          
            if (CurrentSno != 0)
            {
                ViewBag.CurrentSno = CurrentSno;
                Lecture = TopicLetures.Where(a=>a.SNo == CurrentSno).FirstOrDefault();
            }
            else
            {
                ViewBag.CurrentSno = IncompleteSno;
                //IncompleteSno = IncompleteSno - 1;
                if(IncompleteSno<0)
                {
                    IncompleteSno = 0;
                }
                if (IncompleteSno >= 0 && TopicLetures.Count != 0)
                {
                    Lecture = TopicLetures.Where(a => a.SNo == IncompleteSno).FirstOrDefault();//[Convert.ToInt32(IncompleteSno)];
                }                
            }

            ViewData["TopicLetures"] = TopicLetures;
            ViewData["CourseView"] = lms.GetUserReckWikiCourseSubscriptionDetails(UserId).Where(a => a.CourseCode == LMSCourseCode).ToList();//admin.GetCourseMasters(userDetails.SubscriberId).Where(b => b.CourseCode == CourseCode).FirstOrDefault();
            ViewData["CourseTopics"] = lms.GetCourseTopic(LMSCourseCode).ToList();
            return View(Lecture);
        }

        [HttpPost]
        public async Task<ActionResult> CourseContent(LMS.Models.CandidateResponseView CandidateResponse, string CourseCode)
        {
            bool result = false;
            string apiUrl = Global.WikipianUrl() + "/Api/Value/PostCandidateResponse";
            HttpResponseMessage responsePostMethod = new HttpResponseMessage();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                responsePostMethod = await client.PostAsJsonAsync(apiUrl, CandidateResponse);
                if (responsePostMethod.IsSuccessStatusCode)
                {
                    result = true;
                }                
            }
            return RedirectToAction("CourseContent", "Dashboard", new { area = "Candidate", Id = CourseCode, Result = result });
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        // POST: /Account/LogOff
        [HttpPost]

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        #endregion

    }

}