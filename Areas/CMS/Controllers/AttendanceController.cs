using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AJSolutions.Models;
using AJSolutions.Areas.Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.DAL;
using System.Globalization;


namespace AJSolutions.Areas.CMS.Controllers
{
    public class AttendanceController : Controller
    {
        // GET: CMS/Attendance

        private UserDBContext db = new UserDBContext();
        HMSManager hms = new HMSManager();
        EMSManager ems = new EMSManager();
        CMSManager cms = new CMSManager();
        AdminManager admin = new AdminManager();
        TMSManager tms = new TMSManager();
        Generic generic = new Generic();
        Student student = new Student();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Record(string AtendenceDate, DateTime? AttendenceDate, string Id, bool Status = false)
        {

            ViewBag.Status = Status;

            string UserId = User.Identity.GetUserId();
            UserViewModel UserDetails = generic.GetUserDetail(UserId);
            ViewData["userprofile"] = UserDetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            //var plandetail = admin.GetUserplanDetails(UserDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            //ViewData["plandetail"] = plandetail;
            PopulateAttedanceSessions();
            if (!String.IsNullOrEmpty(AtendenceDate))
            {
                AttendenceDate = DateTime.ParseExact(AtendenceDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var attendance = db.CandidateAttendance.Where(a => a.AttendenceDate == AttendenceDate && a.TrainingId == Id).ToList();
                ViewData["attendancerecord"] = attendance;
                ViewBag.date = AtendenceDate;
            }  
            var trainingDetail = db.TrainingSchedule.Find(Id);
            ViewBag.BatchId = trainingDetail.BatchId;
            var date = db.CourseBatch.Find(trainingDetail.BatchId);
            ViewBag.StartDate = date.FromDate;
            ViewBag.Enddate = date.ToDate;
            ViewBag.TrainingId = Id;
            //ViewData["CourseBatch"] = db.CourseBatch.Find(trainingDetail.BatchId);

            ViewData["TrainerDetail"] = ems.GetSubscriberWiseEmployeeList(UserDetails.SubscriberId).Where(e => e.UserId == trainingDetail.TrainerId).FirstOrDefault();
            var candidate = student.GetSubscriberWiseCandidateList(UserDetails.SubscriberId,trainingDetail.BatchId).OrderBy(c => c.Name).ToList();
            ViewBag.CandidateCount = candidate.Count;
            ViewData["Candidate"] = candidate;

            return View();
        }

        [HttpPost]
        public ActionResult Record(string TrainingId, string AtendenceDate, string UId, DateTime? AttendenceDate, string URemark, string UAttendance, string Comment, string Sessions = "Full Day")
        {
            UId = UId.TrimEnd(',');
            string[] userid = UId.Trim().Split(',');
            string[] remarks = URemark.Split(',');
            UAttendance = UAttendance.TrimEnd(',');
            string[] isPresent = UAttendance.Trim().Split(',');
            bool result = false;
            //var CommentDate = AttendenceDate;
            if (!String.IsNullOrEmpty(AtendenceDate))
            {
                AttendenceDate = DateTime.ParseExact(AtendenceDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }           

            for (int i = 0; i < userid.Length; i++)
            {
                string UserId = userid[i];
                var attendanceExist = db.CandidateAttendance.Where(a => a.TrainingId == TrainingId && a.AttendenceDate == AttendenceDate && a.UserId == UserId).FirstOrDefault();

                string status = isPresent[i].Trim();
                if (Sessions == "Morning")
                {
                    if (status == "P")
                    {
                        status = "H";
                    }
                }
                else if (Sessions == "Noon")
                {
                    if (attendanceExist == null)
                    {
                        status = "H";
                    }
                    else if (status == "P" && attendanceExist.IsPresent == "A")
                    {
                        status = "H";
                    }
                    else if(status == "A" && attendanceExist.IsPresent == "H")
                    {
                        status = "H";
                    }
                   
                    
                }
                if (string.IsNullOrEmpty(Sessions))
                {
                    Sessions = "Full Day";
                }
                result = hms.AddCandidateAttendances(0, TrainingId, UserId, AttendenceDate, status, remarks[i], Sessions);

            }
            var CommentDate = AttendenceDate;
            if (!string.IsNullOrEmpty(Comment))
                result = hms.AddTrainerComments(TrainingId, CommentDate, Comment);

            return RedirectToAction("Record", "Attendance", new { area = "CMS", Id = TrainingId, Status = result });
        }


        [HttpGet]
        public ActionResult GetAttendanceExist(string TrainingId, string AtendenceDate)
        {
            bool exist = false;
            if (!String.IsNullOrEmpty(AtendenceDate))
            {
                DateTime? Date = DateTime.ParseExact(AtendenceDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                var attendanceExist = from a in db.CandidateAttendance.Where(a => a.TrainingId == TrainingId && a.AttendenceDate == Date) select a;

                if (attendanceExist != null)
                {
                    if (attendanceExist.Count() > 0)
                        exist = true;
                }

            }
            return Json(exist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult trainnerComment(string TrainingId, Int64 CommentId)
        {
            string UserId = User.Identity.GetUserId();
            return View();
        }

        [HttpPost]
        public ActionResult trainnerComment(string TrainingId, Int64 CommentId, DateTime CommentDate, string Remarks)
        {
            // var result = false;
            return View();
        }

        //http get method for course page checkin checkout     
        public ActionResult checkin(string UserId)
        {
            var checkdetails = hms.GetCandidateCheckIn(UserId);
            ViewData["checkdetails"] = checkdetails.AsEnumerable();
            return View(checkdetails);
        }

        private void PopulateBatchByDate(string SubscriberId = null, string CourseCode = null, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetBatches(SubscriberId, CourseCode);
            SelectList FromDate = new SelectList(query, "FromDate", "FromDate", selectedValue);
            ViewBag.FromDate = FromDate;
        }

        private void PopulateAttedanceSessions(object selectedValue = null)
        {
            var query = generic.GetAttedanceSessions();
            SelectList Sessions = new SelectList(query, "Session", "Session", selectedValue);
            ViewBag.Sessions = Sessions;
        }

    }
}
