using AJSolutions.DAL;
using AJSolutions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.Areas.LMS.Models;
using AJSolutions.Areas.Candidate.Models;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Web.UI;
using Microsoft.SqlServer;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Threading.Tasks;
using System.Net;
using System.Web.Script.Serialization;
using System.Net.Http;
using PagedList;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AJSolutions.Areas.PMS.Models;

namespace AJSolutions.Areas.TMS.Controllers
{
    public class TMSController : Controller
    {
        // GET: TMS/TMS

        private UserDBContext db = new UserDBContext();
        AdminManager admin = new AdminManager();
        TMSManager tms = new TMSManager();
        CMSManager cms = new CMSManager();
        EMSManager ems = new EMSManager();
        LMSManager lms = new LMSManager();
        Student student = new Student();
        HMSManager hms = new HMSManager();
        Generic generic = new Generic();
        UserDBContext userContext = new UserDBContext();
        DataTable dt = new DataTable();
        int SuccessCount, FailureCount;

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

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Mycourses(string ClientId)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetails.SubscriberId;
            ViewBag.LMSUrl = Global.WikipianUrl();
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;

            ViewData["TrainingDetail"] = ems.GetSubscriberWiseEmployeeList(userDetails.SubscriberId);
            List<CourseMasterView> courseMasters = new List<CourseMasterView>();
            List<CourseMasterViewModel> lmsCourseMasters = new List<CourseMasterViewModel>();
            if (userDetails.Role == "Client")
            {
                if (userDetails.CorporateId != null && userDetails.CorporateId != userDetails.SubscriberId)
                {
                    courseMasters = admin.GetCourseMasters(null, userDetails.CorporateId);
                    lmsCourseMasters = lms.GetLMSCourseMasters(userDetails.CorporateId);
                }
                else
                {
                    courseMasters = admin.GetCourseMasters(null, userDetails.UserId);
                    lmsCourseMasters = lms.GetLMSCourseMasters(userDetails.UserId);
                }

            }
            else
            {
                courseMasters = admin.GetCourseMasters(userDetails.SubscriberId);
                lmsCourseMasters = lms.GetLMSCourseMasters(userDetails.SubscriberId);
            }
            ViewData["LMSCourse"] = lmsCourseMasters;
            if (ClientId != null)
            {
                var courses = courseMasters.Where(b => b.CorporateId == ClientId).OrderByDescending(c => c.CourseCode);
                PopulateClient(userDetails.SubscriberId, ClientId);
                return View(courses);
            }
            else
            {
                var courses = courseMasters.OrderByDescending(c => c.CourseCode);
                PopulateClient(userDetails.SubscriberId);
                return View(courses);
            }

        }
        [HttpGet]
        public ActionResult CourseDetail(string CourseCode, string TrainingId, Int64 BatchId = 0, Int16 NavTab = 0, string UserAction = "NoAction")
        {
            ViewBag.Tab = NavTab;
            ViewBag.Url = Global.WebsiteUrl();
            ViewBag.LMSUrl = Global.WikipianUrl();
            ViewBag.IsLMS = true;

            ViewBag.BatchList = "NA";
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetails.SubscriberId;
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            //If Client has team members with all rights
            if (userDetails.CorporateId != null && userDetails.CorporateId != userDetails.SubscriberId)
            {
                userDetails.UserId = userDetails.CorporateId;
            }
            //string LMSCourseCode = "NA";
            //LMSCourseCode = tms.IsCourseIntegrated(CourseCode);
            ViewBag.User = AJSolutions.DAL.Global.IsStatusReportAccess(userDetails.SubscriberId);
            // PopulateCourse(userDetails.SubscriberId);
            PopulateBatchByCourse(userDetails.SubscriberId, CourseCode);
            PopulateTrainer(userDetails.SubscriberId);
            PopulateTrainings(0, null);

            //if (LMSCourseCode == "NA")
            //{
            //    PopulateWikipianCourse(userDetails.SubscriberId);
            //}
            //else
            //{
            //    ViewData["LMSCourseView"] = tms.GetLMSCourseDetails(LMSCourseCode);
            //}
            ViewData["CourseBatches"] = tms.GetCBatches(userDetails.SubscriberId, CourseCode);
            ViewData["CourseBatch"] = tms.GetCourseBatches(userDetails.SubscriberId).Where(b => b.CourseCode == CourseCode).OrderByDescending(c => c.BatchId).ToList();
            ViewData["Training"] = tms.GetTrainingSchedule(userDetails.SubscriberId, TrainingId).Where(b => b.CourseCode == CourseCode).OrderByDescending(c => c.TrainingId).ToList();
            //ViewData["Candidate"] = student.GetSubscriberWiseCandidateList(userDetails.SubscriberId).Where(b => b.CourseCode == CourseCode).ToList();
            if (UserAction == "Delete" && TrainingId != null)
            {
                var trainingattach = admin.GetTrainingAttachments(TrainingId).FirstOrDefault();
                if (trainingattach != null)
                {
                    tms.RemoveTrainingAttach(trainingattach.FileId);
                }
                tms.RemoveTraining(TrainingId);
                return RedirectToAction("CourseDetail", "TMS", new { area = "TMS", CourseCode = CourseCode });
            }
            List<CandidateViewModel> Details = student.GetSubscriberWiseCandidateList(userDetails.SubscriberId, BatchId).ToList();
            //if (BatchId != 0)
            //{
            //    Details = Details.Where(c => c.BatchId == BatchId).ToList();
            //}
            // ViewData["topic"] = lms.GetTopicMaster(userDetails.SubscriberId) ;

            //ViewData["Lecture"] = lms.GetTopicLecture(LMSCourseCode);
            //ViewData["Reply"] = lms.GetReviewReply();
            //ViewData["CheckInDate"] = hms.GetCandidateCheckIn(userDetails.SubscriberId);
            // ViewData["TrainerDetail"] = ems.GetSubscriberWiseEmployeeList(userDetails.SubscriberId);
            ViewData["attendence"] = tms.GetTrainingScheduleById(userDetails.SubscriberId, TrainingId);
            //ViewData["AttendenceRecord"] = hms.GetCandidateAttendancelist(TrainingId).ToList();
            //ViewBag.CommentsCount = lms.SPCountComments(CourseCode).TOTALCOMMENTS;
            ViewData["Forum"] = lms.GetReview(CourseCode);
            ViewData["CourseView"] = admin.GetCourseMasters(userDetails.SubscriberId).Where(b => b.CourseCode == CourseCode).FirstOrDefault();
            //var coursetopics = lms.GetCourseTopic(LMSCourseCode).ToList();
            //ViewData["CourseTopic"] = coursetopics;
            var attach = admin.GetTrainingAttachments(TrainingId);
            ViewData["Attachment"] = attach;
            ViewData["FinalAttach"] = tms.GetTrainingFinalAttachments(TrainingId);
            ViewData["AnalyticsAssessment"] = tms.GetAnalyticsAssessment(CourseCode);
            return View(Details.OrderBy(c => c.Name));

        }

        [HttpPost]
        public ActionResult CourseDetail(string CourseCode, string Comments, string Reply, Int64 BatchId = 0, Int64 CommentId = 0)
        {
            string userId = User.Identity.GetUserId();
            var result = false;

            TimeZoneInfo CurrentTime = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime CurrentUTCTIme = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CurrentTime);

            if (!string.IsNullOrEmpty(CourseCode) && !string.IsNullOrEmpty(Comments))
                result = lms.AddReview(userId, CourseCode, Comments, CurrentUTCTIme);

            if (CommentId != 0 && !string.IsNullOrEmpty(Reply))
                lms.AddReply(CommentId, Reply, userId, CurrentUTCTIme);

            //if (!string.IsNullOrEmpty(CourseCode) || !string.IsNullOrEmpty(TrainnerId) || BatchId != 0)
            //    tms.GetTrainingScheduleById(userId, TrainingId);

            return RedirectToAction("CourseDetail", "TMS", new { area = "TMS", CourseCode = CourseCode });
        }

        [HttpPost]
        public ActionResult GetTrainingReports(string TrainingId, Int64 BatchId)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);

            var CourseCode = db.CourseBatch.Find(BatchId);
            //Checking Attendance 
            var Attendance = db.CandidateAttendance.Where(c => c.TrainingId == TrainingId).ToList();

            //Checking Assessment
            var Evaluations = db.TrainingAssessment.Where(c => c.TrainingId == TrainingId).ToList();
            var Online = Evaluations.Where(c => c.PublicationId != "0").ToList();
            var Offline = Evaluations.Where(c => c.PublicationId == "0").ToList();


            return Json(new { CandidateAttendance = Attendance, OnlineTrainingAssessments = Online, OfflineTrainingAssessments = Offline, TrainingId = TrainingId, BatchId = BatchId }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetBatchWiseStudent(string CourseCode, Int64 BatchId)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);

            ViewData["UserProfile"] = userDetails;
            List<CandidateViewModel> Details = student.GetSubscriberWiseCandidateList(userDetails.SubscriberId, BatchId).ToList();
            ViewBag.TotalCandidate = Details.Count();
            ViewBag.StartDate = Details.FirstOrDefault().CourseStartDate;

            return Json(Details, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetsCourseDetail(Int64 ABatchId, string TrainingId)
        {
            List<CandidateTraining> CandidateTraining = tms.GetCandiateTraining(ABatchId, TrainingId);
            var htmlContent = "";
            var tabcontent = "";
            string Id = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(Id);
            foreach (var training in CandidateTraining)
            {
                //Show each training in a table

                List<CandidateAttendanceView> CandAtt = training.CandidateList.OrderBy(list => list.Name).ToList();

                if (CandAtt.Count() > 0)
                {
                    List<String> UserId = new List<String>();
                    foreach (var row in CandAtt)
                    {
                        UserId.Add(row.UserId);
                    }
                    UserId = UserId.Distinct().ToList();

                    List<DateTime> AttendanceDates = new List<DateTime>();
                    var fromDate = training.FromDate.Date;
                    var toDate = training.ToDate.Date;
                    var diff = toDate.Subtract(fromDate).Days + 1;
                    var date = fromDate.Date.AddDays(-1);
                    var comment = db.TrainerComments.Where(c => c.TrainingId == training.TrainingId).FirstOrDefault();
                    string comments = "NA";
                    if (comment != null)
                    {
                        comments = comment.Comment;
                    }
                    for (int z = 0; z < diff; z++)
                    {
                        date = date.AddDays(1);
                        AttendanceDates.Add(date);
                    }
                    AttendanceDates = AttendanceDates.Distinct().OrderBy(d => d.Date).ToList();

                    tabcontent = "<div class='row'><div class='fancy-title title-bottom-border'><h6>" + training.TrainingId + " : " + training.TrainingName + "</h6></div><small style='float:right'>Total Candidate : " + training.TotalStudent + "</small><br/><div class='table-responsive'><table id='fixTable_" + training.TrainingId + "' class='table table-bordered nobottommargin'>";
                    tabcontent = tabcontent + "<tr><th>Candidate Name</th>";

                    foreach (var dt in AttendanceDates)
                    {
                        if (fromDate <= dt && dt <= toDate)
                            tabcontent = tabcontent + "<th title=" + comments + ">" + dt.ToString("dd-MMM-yyyy") + "</th>";
                    }
                    tabcontent = tabcontent + "<th>Present/Total Days</th></tr>";

                    for (int i = 0; i < UserId.Count(); i++)
                    {
                        string CandidateId = UserId[i];
                        var Candidatedetails = db.UserProfile.Where(a => a.UserId == CandidateId).FirstOrDefault();
                        double CandP = 0;

                        if (string.IsNullOrEmpty(Candidatedetails.RegistrationId))
                        {
                            tabcontent = tabcontent + "<tr><td><b>" + Candidatedetails.Name + "</b></td>";
                        }
                        else
                        {
                            tabcontent = tabcontent + "<tr><td><b>" + Candidatedetails.Name + '(' + Candidatedetails.RegistrationId + ')' + "</b></td>";
                        }

                        for (int j = 0; j < AttendanceDates.Count(); j++)
                        {
                            CandidateAttendanceView AttByNameNDate = training.CandidateList.Where(list => list.UserId == UserId[i] && list.AttendenceDate == AttendanceDates[j]).FirstOrDefault();
                            if (AttByNameNDate != null)
                            {
                                if (AttByNameNDate.IsPresent == "P")
                                {
                                    tabcontent = tabcontent + "<td title=" + AttByNameNDate.Remarks + ">P</td>";
                                    CandP = CandP + 1;
                                }
                                else if (AttByNameNDate.IsPresent == "H")
                                {
                                    tabcontent = tabcontent + "<td style='color: red;' title=" + AttByNameNDate.Remarks + ">H</td>";
                                    CandP = CandP + 0.5;
                                }
                                else if (AttByNameNDate.IsPresent == "A")
                                {
                                    tabcontent = tabcontent + "<td style='color: red;'>A</td>";
                                }
                                //if (AttByNameNDate.IsPresent)
                                //{
                                //    tabcontent = tabcontent + "<td title=" + AttByNameNDate.Remarks + ">P</td>";
                                //    CandP = CandP + 1;
                                //}
                                //else
                                //{
                                //    tabcontent = tabcontent + "<td style='color: red;'>A</td>";
                                //}

                            }
                            else
                            {
                                tabcontent = tabcontent + "<td>-</td>";
                            }
                        }
                        tabcontent = tabcontent + "<td><b>" + CandP + "/" + AttendanceDates.Count() + "</b></td>" + "</tr>";
                    }
                    tabcontent = tabcontent + "<tr><td><b>Present/Total Candidates</b></td>";

                    for (int k = 0; k < AttendanceDates.Count(); k++)
                    {
                        List<CandidateAttendanceView> AttByDate = training.CandidateList.Where(list => list.AttendenceDate == AttendanceDates[k]).ToList();
                        double DateP = 0;
                        foreach (var cand in AttByDate)
                        {
                            if (cand.IsPresent == "P")
                            {
                                DateP = DateP + 1;
                            }
                            else if (cand.IsPresent == "H")
                            {
                                DateP = DateP + 0.5;
                            }
                        }
                        tabcontent = tabcontent + "<td><b>" + DateP + "/" + UserId.Count() + "</b></td>";

                    }
                    tabcontent = tabcontent + "</tr></table></div></div><div><table><tr><td colspan=2 align=right><a href='/TMS/TMS/DownloadCandidateAttendence?BatchId=" + ABatchId + "&SubscriberId=" + userDetails.SubscriberId + "&TrainingId=" + training.TrainingId + "'>Download Analytics</a></td></tr></table></div><br /><br /><script>$('#fixTable_" + training.TrainingId + "').tableHeadFixer({'head' : false, 'left' : 1});</script>";
                    htmlContent = htmlContent + tabcontent;
                }
                else
                {
                    tabcontent = "<div class='row'><div class='fancy-title title-bottom-border'><h5>" + training.TrainingId + " : " + training.TrainingName + "</h5></div><small style='float:right'>Total Students : " + training.TotalStudent + "</small><br/><div class='feature-box fbox-center fbox-bg fbox-border fbox-effect'><div class='fbox-icon'>";
                    tabcontent = tabcontent + "<i class='icon-thumbs-down2'></i></div><h3>No Attendance Recorded<span class='subtitle'></span></h3></div><br /><br />";
                    htmlContent = htmlContent + tabcontent;
                }
                //tabcontent = tabcontent + "</tr></table></div></div><div><table><tr><td colspan=2 align=right><a href='/TMS/TMS/DownloadCandidateAttendence?BatchId=" + ABatchId + "&SubscriberId=" + userDetails.SubscriberId + "&TrainingId=" + training.TrainingId + "'>Download Analytics</a></td></tr></table></div><br /><br /><script>$('#fixTable_" + training.TrainingId + "').tableHeadFixer({'head' : false, 'left' : 1});</script>";
            }
            return Json(htmlContent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadCandidateAttendence(string BatchId, string SubscriberId, string TrainingId)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Views/Report/CandidateTrainingWiseAttendence.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@BatchId", BatchId));
            cmd.Parameters.Add(new SqlParameter("@TrainingId", TrainingId));
            cmd.Connection = thisConnection;
            string MyDataSource1 = "USP_CandidateTrainingWiseAttendenceDetails";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];
            if (DataSet1.Tables[0].Rows.Count == 0)
            {
                @TempData["NoRecordFound"] = true;
            }
            else
            {
                ReportParameter[] parms = new ReportParameter[2];
                parms[0] = new ReportParameter("BatchId", BatchId.ToString());
                parms[1] = new ReportParameter("TrainingId", TrainingId);
                rptViewer.LocalReport.SetParameters(parms);
                rptViewer.LocalReport.DataSources.Add(reportDataSource);
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.SizeToReportContent = true;
                rptViewer.ZoomMode = ZoomMode.PageWidth;
                rptViewer.Width = Unit.Percentage(99);
                rptViewer.Height = Unit.Pixel(1000);
                var reList = rptViewer.LocalReport.ListRenderingExtensions();
                string mimeType = string.Empty;
                string encoding = string.Empty;
                rptViewer.LocalReport.Refresh();
                //bool excxel = rptViewer.LocalReport.ListRenderingExtensions().ToList().Find(x => x.Name.Equals("EXCELOPENXML", StringComparison.CurrentCultureIgnoreCase)).Visible;
                //byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                //string adminrpt = "Adminreport";
                Response.AddHeader("content-disposition", "attachment; filename=CandidateAttendence.pdf");
                Response.BinaryWrite(bytes); // create the file
                Response.Flush();
                //return RedirectToAction("AdminReport", "Report");    
                //return View(rptViewer);
            }
            return View(rptViewer);
        }

        public ActionResult DownloadCandidateAttendenceExcel(string BatchId, string SubscriberId, string TrainingId)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Views/Report/CandidateTrainingWiseAttendence.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@BatchId", BatchId));
            cmd.Parameters.Add(new SqlParameter("@TrainingId", TrainingId));
            cmd.Connection = thisConnection;
            string MyDataSource1 = "USP_CandidateTrainingWiseAttendenceDetails";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];
            if (DataSet1.Tables[0].Rows.Count == 0)
            {
                @TempData["NoRecordFound"] = true;
            }
            else
            {
                ReportParameter[] parms = new ReportParameter[2];
                parms[0] = new ReportParameter("BatchId", BatchId.ToString());
                parms[1] = new ReportParameter("TrainingId", TrainingId);
                rptViewer.LocalReport.SetParameters(parms);
                rptViewer.LocalReport.DataSources.Add(reportDataSource);
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.SizeToReportContent = true;
                rptViewer.ZoomMode = ZoomMode.PageWidth;
                rptViewer.Width = Unit.Percentage(99);
                rptViewer.Height = Unit.Pixel(1000);
                var reList = rptViewer.LocalReport.ListRenderingExtensions();
                string mimeType = string.Empty;
                string encoding = string.Empty;
                rptViewer.LocalReport.Refresh();
                //bool excxel = rptViewer.LocalReport.ListRenderingExtensions().ToList().Find(x => x.Name.Equals("EXCELOPENXML", StringComparison.CurrentCultureIgnoreCase)).Visible;
                //byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                byte[] bytes = rptViewer.LocalReport.Render("Excel", null);
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                //string adminrpt = "Adminreport";
                Response.AddHeader("content-disposition", "attachment; filename='" + "CandidateAttendance" + ".xls");
                Response.BinaryWrite(bytes);
                Response.Flush();
                return View();

            }
            return View(rptViewer);
        }

        public ActionResult UnassignedCourse(string UID, string CC, string BID)
        {
            if (BID != "undefined")
            {
                Int64 BatchId = Convert.ToInt64(BID);
                CandidateCourseDetails ccd = db.CandidateCourseDetails.Where(c => c.UserId == UID && c.BatchId == BatchId).FirstOrDefault();
                if (ccd != null)
                {
                    db.CandidateCourseDetails.Remove(ccd);
                    db.SaveChanges();
                }

                return RedirectToAction("CourseDetail", "TMS", new { area = "TMS", CourseCode = CC });
            }
            else
            {
                var course = UID.Split(',');
                UID = course[0];
                CC = course[1];
                string BI = course[2];
                Int64 BatchId = Convert.ToInt64(BI);
                CandidateCourseDetails ccd = db.CandidateCourseDetails.Where(c => c.UserId == UID && c.BatchId == BatchId).FirstOrDefault();
                if (ccd != null)
                {
                    db.CandidateCourseDetails.Remove(ccd);
                    db.SaveChanges();
                }

                return RedirectToAction("CourseDetail", "TMS", new { area = "TMS", CourseCode = CC });
            }
        }

        public ActionResult RemoveReplies(string CC, Int64 RId)
        {
            ReviewReply review = db.ReviewReply.Find(RId);
            if (review != null)
            {
                db.ReviewReply.Remove(review);
                db.SaveChanges();
            }
            return RedirectToAction("CourseDetail", "TMS", new { area = "TMS", CourseCode = CC });
        }

        public ActionResult RemoveComment(string CC, Int64 CId)
        {
            lms.RemoveComments(CId);
            return RedirectToAction("CourseDetail", "TMS", new { area = "TMS", CourseCode = CC });
        }

        public ActionResult Lectures(string LectureId)
        {

            var lectureDetails = from d in db.LectureContentUpload
                                 join l in db.LectureMaster
                                on d.LectureId equals l.LectureId
                                 select new { d, l };

            return View(lectureDetails);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult CourseMasters(string useraction = "Add", string CourseCode = null)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetails.SubscriberId;
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;

            ViewBag.SubscriberId = userDetails.SubscriberId;
            ViewBag.IsLMS = "True";
            CourseMaster courseMaster = new CourseMaster();

            ViewBag.UserAction = useraction;
            List<CorporateProfile> corporateProfiles = db.CorporateProfile.Where(p => p.SubscriberId == userDetails.SubscriberId).ToList().FindAll(p => p.DepartmentId == "CLI" || (p.DepartmentId == "ADI"));
            if (useraction == "Add")
            {
                // Binding dropdowns 
                PopulateClient(userDetails.SubscriberId);
                //ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => cp.Name), "CorporateId", "Name");
                PopulateDuration();
                PopulateContentVisiblity();
                PopulateCurrency();
                PopulateCategory();
                ViewBag.Accommodation = 0;
                ViewBag.InstallmentInterest = 0;
                ViewBag.Transport = 0;
                ViewBag.Others = 0;
                ViewBag.Discount = 0;
            }
            else
            {
                var courseDetail = admin.GetCourseMasterDetails(CourseCode);
                ViewBag.CourseCode = courseDetail.CourseCode;
                ViewBag.CourseName = courseDetail.CourseName;
                ViewBag.SubscriberId = courseDetail.SubscriberId;
                ViewBag.CourseFee = courseDetail.CourseFee;
                ViewBag.CourseLateFee = courseDetail.CourseLateFee;
                ViewBag.Accommodation = courseDetail.Accommodation;
                ViewBag.Transport = courseDetail.Transport;
                ViewBag.Others = courseDetail.Others;
                ViewBag.InstallmentInterest = courseDetail.InstallmentInterest;
                ViewBag.Discount = courseDetail.Discount;

                // Binding dropdows 
                PopulateClient(userDetails.SubscriberId, courseDetail.CorporateId);
                //ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => cp.Name), "CorporateId", "Name", courseDetail.CorporateId);
                PopulateDuration(courseDetail.CourseDuration);
                ViewBag.CourseDescription = courseDetail.CourseDescription;
                PopulateCurrency(courseDetail.Currency);
                PopulateContentVisiblity(courseDetail.ContentVisiblity);
                PopulateCategory(courseDetail.CategoryId);
                ViewBag.CountLikes = courseDetail.CountLikes;
            }
            //ViewData["Content"] = (from i in userContext.CourseAttachment.Where(i => i.CourseCode == CourseCode) select i).FirstOrDefault();
            List<CourseMasterView> courseMasters = admin.GetCourseMasters(userDetails.SubscriberId);

            return View(courseMasters);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult CourseMasters(CourseMasterwithAddtionalView courseMaster, string fileName)
        {
            if (string.IsNullOrEmpty(courseMaster.CourseCode) || courseMaster.CourseCode == "0")
                courseMaster.CourseCode = admin.GenerateCourseCode();

            CourseMaster CourseMasterDetails = new CourseMaster();
            CourseMasterDetails.CategoryId = 1;
            CourseMasterDetails.ContentVisiblity = 1;
            CourseMasterDetails.CorporateId = courseMaster.CorporateId;
            CourseMasterDetails.CountLikes = courseMaster.CountLikes;
            CourseMasterDetails.CourseCode = courseMaster.CourseCode;
            CourseMasterDetails.CourseDescription = courseMaster.CourseDescription;
            CourseMasterDetails.CourseDuration = courseMaster.CourseDuration;
            CourseMasterDetails.CourseFee = courseMaster.CourseFee;
            CourseMasterDetails.CourseLateFee = courseMaster.CourseLateFee;
            CourseMasterDetails.CourseName = courseMaster.CourseName;
            CourseMasterDetails.Currency = courseMaster.Currency;
            CourseMasterDetails.LMSCourseCode = courseMaster.LMSCourseCode;
            CourseMasterDetails.SubscriberId = courseMaster.SubscriberId;

            bool result = admin.AddCourseMasters(CourseMasterDetails);
            if (result)
            {
                AdditionalCourseFee AdditionalCourseFee = new AdditionalCourseFee();
                AdditionalCourseFee.Accommodation = courseMaster.Accommodation;
                AdditionalCourseFee.CorporateId = courseMaster.CorporateId;
                AdditionalCourseFee.CourseCode = courseMaster.CourseCode;
                AdditionalCourseFee.Discount = courseMaster.Discount;
                AdditionalCourseFee.Others = courseMaster.Others;
                AdditionalCourseFee.InstallmentInterest = courseMaster.InstallmentInterest;
                AdditionalCourseFee.Transport = courseMaster.Transport;

                bool res = admin.AddCourseAddtionalFees(AdditionalCourseFee);
            }

            //if (!string.IsNullOrEmpty(courseMaster.CourseCode))
            //{
            //    foreach (string file in Request.Files)
            //    {
            //        HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
            //        admin.uploadCourseFile(courseMaster.CourseCode, attachment);
            //    }
            //}
            return Json(result, JsonRequestBehavior.AllowGet);//RedirectToAction("CourseMasters", new { area = "TMS" });
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult CourseBatch(DateTime? FromDate, DateTime? ToDate, DateTime? FromTime, DateTime? ToTime, DateTime? AvailableTillDate, string CourseCode, bool status = false, long batchId = 0, string UserAction = "Add", bool ContentAvailability = false)
        {
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["UserProfile"] = userDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.UserAction = UserAction;
            CourseBatchViewModel courseBatch = new CourseBatchViewModel();
            List<CourseBatchViewModel> courseBatches = tms.GetCourseBatches(userDetail.SubscriberId);
            //if (CourseCode != null)
            //{
            //    ViewData["CourseBatches"] = courseBatches.Where(c => c.CourseCode == CourseCode).OrderByDescending(d => d.BatchId).AsEnumerable();
            //}
            //else
            //{
            //    ViewData["CourseBatches"] = courseBatches.OrderByDescending(d => d.BatchId).AsEnumerable();
            //}

            //ViewData["Content"] = (from i in db.CourseAttachment.Where(i => i.CourseCode == CourseCode) select i).FirstOrDefault();
            PopulateWarden(userDetail.SubscriberId);

            //Populate drop down
            List<CorporateProfile> corporateProfiles = db.CorporateProfile.Where(p => p.SubscriberId == userDetail.SubscriberId).ToList().FindAll(p => p.DepartmentId == "CLI").FindAll(p => p.CorporateId == "CorporateId");
            ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => cp.Name), "CorporateId", "Name");
            // Binding dropdows 
            PopulateCAvailability();
            PopulateWarden(userDetail.SubscriberId, courseBatch.WardenId);
            PopulateCountry();
            PopulateState();
            PopulateCity();
            if (CourseCode != null)
            {
                ViewBag.CourseCode = new SelectList(admin.GetCourseMasters(userDetail.SubscriberId).OrderBy(cm => cm.CourseCode), "CourseCode", "CourseName", courseBatch.CourseCode);
            }
            else
            {
                ViewBag.CourseCode = new SelectList(admin.GetCourseMasters(userDetail.SubscriberId).OrderBy(cm => cm.CourseCode), "CourseCode", "CourseName");
            }
            if (UserAction == "Edit")
            {
                courseBatch = courseBatches.Where(i => i.BatchId == batchId).FirstOrDefault();
                PopulateCountry(courseBatch.CountryId);
                PopulateState(courseBatch.CountryId, courseBatch.StateId);
                PopulateCity(courseBatch.StateId, courseBatch.CityId);
                ViewBag.BatchId = batchId;
                PopulateWarden(userDetail.SubscriberId, courseBatch.WardenId);
                ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => cp.Name), "CorporateId", "Name", courseBatch.CorporateId);
                ViewBag.CourseCode = new SelectList(admin.GetCourseMasters(userDetail.SubscriberId).OrderBy(cm => cm.CourseCode), "CourseCode", "CourseCode", courseBatch.CourseCode);
                PopulateCAvailability(ContentAvailability);
                var fromDate = courseBatch.FromDate.ToString("dd-MM-yyyy");
                var toDate = courseBatch.ToDate.ToString("dd-MM-yyyy");
                FromTime = courseBatch.FromTime;
                var fromTime = FromTime;
                ToTime = courseBatch.ToTime;
                var toTime = ToTime;
                var schedule = fromDate + " " + fromTime.Value.ToString("hh:mm:ss tt") + " " + "-" + " " + toDate + " " + toTime.Value.ToString("hh:mm:ss tt");
                ViewBag.Schedule = schedule;
                AvailableTillDate = courseBatch.AvailableTillDate;
                if (courseBatch.AvailableTillDate != null)
                {
                    ViewBag.AvailableTillDate = courseBatch.AvailableTillDate.Value.ToString("dd-MM-yyyy");
                }
                return View(courseBatch);
            }
            ViewBag.UserName = userDetail.Name;
            ViewBag.UserRole = userDetail.Role;
            ViewBag.Action = UserAction;
            courseBatch.FromDate = DateTime.Today;
            courseBatch.ToDate = DateTime.Today;
            courseBatch.AvailableTillDate = DateTime.Today;
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult CourseBatch(CourseBatch courseBatch, string Schedule, string AvailableTillDate, Int64 batchId = 0)
        {
            string[] strSchedule = Schedule.Split('-');
            DateTime frmdate = DateTime.ParseExact(strSchedule[0].Trim(), "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);

            courseBatch.FromDate = frmdate.Date;
            courseBatch.FromTime = Convert.ToDateTime(frmdate.ToShortTimeString());

            DateTime todate = DateTime.ParseExact(strSchedule[1].Trim(), "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);
            courseBatch.ToDate = todate.Date;
            courseBatch.ToTime = Convert.ToDateTime(todate.ToShortTimeString());

            CourseMaster courseMaster = new CourseMaster();
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            courseMaster.SubscriberId = userDetails.SubscriberId;
            //var availableTillDate = Convert.ToDateTime(AvailableTillDate);

            courseBatch.AvailableTillDate = null;
            if (!String.IsNullOrEmpty(AvailableTillDate))
            {
                courseBatch.AvailableTillDate = DateTime.ParseExact(AvailableTillDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            bool result = tms.AddCourseBatch(courseBatch);

            //return RedirectToAction("CourseBatch", new { area = "TMS", status = result });
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult CandidateCourseDetails(CandidateCourseDetails candidateCourseDetail, string useraction = "Add")
        {
            var candidateCourseDetails = tms.GetCandidateCourseDetails(candidateCourseDetail.UserId);
            ViewData["CandidateCourseDetails"] = candidateCourseDetails.AsEnumerable();
            return View(candidateCourseDetails);
        }

        [HttpPost]
        public ActionResult CandidateCourseDetails(CandidateCourseDetails candidateCourseDetail)
        {
            tms.AddCandidateCourseDetails(candidateCourseDetail);
            return RedirectToAction("CandidateCourseDetails", "TMS", new { area = "TMS" });
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult TrainingSchedule(DateTime? FromDate, DateTime? ToDate, DateTime? FromTime, DateTime? ToTime, DateTime? AvailableTillDate, string TrainingId, string CourseCode, string Course, string TaskId, string UserAction = "Add", bool status = false, Int64 BatchId = 0, long batchId = 0, bool ContentAvailability = false)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetail.SubscriberId;
            CourseBatchViewModel courseBatch = new CourseBatchViewModel();
            ViewData["UserProfile"] = userDetail;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewBag.Result = "Failed";
            ViewBag.UserAction = UserAction;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;

            //if (TrainingId != null)
            //{
            ViewData["TrainingAssessment"] = tms.GetTrainingAssessments(TrainingId);
            if (status == true)
            {
                ViewBag.result = "Succeed";
            }
            if (!string.IsNullOrEmpty(Course))
            {
                ViewBag.Course = Course;
            }
            //}
            if (UserAction == "Edit")
            {
                var myTraining = tms.GetTrainingSchedule(userDetail.SubscriberId, TrainingId).FirstOrDefault();
                //PopulateCountry(myTraining.CountryId);
                //PopulateState(myTraining.CountryId, myTraining.StateId);
                //PopulateCity(myTraining.StateId, myTraining.CityId);              
                PopulateTasklist(userDetail.SubscriberId, myTraining.TaskId);
                PopulateCourse(userDetail.SubscriberId, myTraining.CourseCode);
                PopulateTrainer(userDetail.SubscriberId, myTraining.TrainerId);
                PopulateBatchByCourseForDuration(userDetail.SubscriberId, myTraining.CourseCode, myTraining.BatchId, true);
                PopulateAssessment(userDetail.SubscriberId);
                PopulateMentorTrainer(userDetail.SubscriberId, myTraining.TrainerMentorId);
                if (myTraining.OtherTrainerId != null)
                {
                    var OtherTrainerId = myTraining.OtherTrainerId;
                    string[] OtherTrainer = OtherTrainerId.Split(',');
                    //ViewBag.OtherTrainerId = new MultiSelectList(OtherTrainer, "UserId", "Name");

                    PopulateOtherTrainer(userDetail.SubscriberId, OtherTrainer);
                }
                else
                {
                    PopulateOtherTrainer(userDetail.SubscriberId);
                }
                ViewData["Content"] = (from i in userContext.TrainingScheduleAttachment.Where(i => i.TrainingId == TrainingId) select i).FirstOrDefault();
                return View(myTraining);
            }
            else
            {
                if (CourseCode != null)
                {
                    ViewBag.CourseCode = new SelectList(admin.GetCourseMasters(userDetail.SubscriberId).OrderBy(cm => cm.CourseCode), "CourseCode", "CourseName", courseBatch.CourseCode);
                }
                else
                {
                    PopulateCourse(userDetail.SubscriberId);
                }
                if (BatchId > 0)
                {
                    PopulateBatchByCourseForDuration(userDetail.SubscriberId, CourseCode, BatchId, true);

                }
                else
                {
                    PopulateBatchByCourseForDuration(userDetail.SubscriberId, CourseCode, null, true);
                }
                PopulateTrainer(userDetail.SubscriberId);
                if (TaskId != null)
                {
                    PopulateTasklist(userDetail.SubscriberId, TaskId);
                }
                else
                {
                    PopulateTasklist(userDetail.SubscriberId);
                }
                if (BatchId > 0)
                {
                    var batches = db.CourseBatch.Where(c => c.BatchId == BatchId).FirstOrDefault();
                    PopulateCountry(batches.CountryId);
                    PopulateState(batches.CountryId, batches.StateId);
                    PopulateCity(batches.StateId, batches.CityId);
                }
                else
                {
                    PopulateCountry();
                    PopulateState();
                    PopulateCity();
                }
                PopulateAssessment(userDetail.SubscriberId);
                PopulateOtherTrainer(userDetail.SubscriberId);
                PopulateMentorTrainer(userDetail.SubscriberId);
                //ViewBag.OtherTrainerId = new MultiSelectList(generic.GetSubscriberWiseList(userDetail.SubscriberId).Where(e => e.DepartmentId == "ADI" || e.DepartmentId == "FAC" || e.DepartmentId == "VFA"), "UserId", "Name");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> TrainingSchedule(string TrainingId, string SubjectLine, string Description, string UserAction, string TrainerId, string CountryId,
                                             string Address, string Status, string TaskId, DateTime? CreatedOn,
                                             string SelCity, string SelState, string StateId, string SelBatch, string CreatedBy, string[] OtherTrainerId, string UpdatedBy, DateTime? UpdatedOn,
                                             string CityId, string[] Assessment, Int64[] Weightage, Int64[] AssessmentId,
                                             string[] PublicationId, string[] PTitle, string[] StartDate, string[] EndDate,
                                             string[] StartTime, string[] EndTime, string TrainerMentor, Int32 BatchId = 0)
        {

            string UserId = User.Identity.GetUserId();

            var batches = db.CourseBatch.Where(c => c.BatchId == BatchId).FirstOrDefault();
            if (string.IsNullOrEmpty(TrainingId))
            {
                TrainingId = admin.GetTrainingId();
                Status = "Assigned";
            }
            if (string.IsNullOrEmpty(CreatedBy))
            {
                CreatedBy = UserId;
            }
            if (CreatedOn == null)
            {
                CreatedOn = DateTime.Now;
            }
            //string body = " has scheduled & assigned you a Training on ";
            //bool Nstatus = false;
            string OtherTrainer = null;
            if (OtherTrainerId != null)
            {
                OtherTrainer = string.Join(",", OtherTrainerId);
            }
            if (UserAction == "Edit")
            {
                var trainingDetails = db.TrainingSchedule.Find(TrainingId);
                if (trainingDetails.Status == "Rejected" && trainingDetails.TrainerId != TrainerId)
                {
                    Status = "Assigned";
                }
            }
            var result = tms.AddTrainingSchedule(TrainingId, BatchId, SubjectLine, Description, TrainerId, OtherTrainer, batches.CountryId, batches.StateId, batches.CityId, Address, Status, TaskId, CreatedOn, CreatedBy, DateTime.Now, UserId, TrainerMentor);
            if (!string.IsNullOrEmpty(TrainingId))
            {
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
                    tms.uploadFile(TrainingId, attachment);
                }
            }

            if (result == true && !string.IsNullOrEmpty(Assessment[0]))
            {
                int i = 0;
                string publicationId = "";
                string pTitle = "";
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                DateTime startTime = DateTime.Now;
                DateTime endTime = DateTime.Now;

                List<AssessmentTrainingView> trainAsst = new List<AssessmentTrainingView>();

                foreach (var assess in Assessment)
                {
                    if (PublicationId != null)
                    {
                        publicationId = PublicationId[i];
                        pTitle = PTitle[0];
                        if (publicationId != "0")
                        {
                            startDate = Convert.ToDateTime(StartDate[i]); //DateTime.ParseExact(StartDate[i], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                            endDate = Convert.ToDateTime(EndDate[i]); //DateTime.ParseExact(EndDate[i], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                            startTime = Convert.ToDateTime(StartTime[i]);// DateTime.ParseExact(StartTime[i], "hh:mm tt", CultureInfo.InvariantCulture);
                            endTime = Convert.ToDateTime(EndTime[i]);// DateTime.ParseExact(EndTime[i], "hh:mm tt", CultureInfo.InvariantCulture);
                        }
                    }
                    var res = tms.AddTrainingAssessment(AssessmentId[i], assess, TrainingId, Weightage[i], publicationId,
                                                        pTitle, startDate, endDate, startTime, endTime);

                    if (publicationId != "0")
                    {
                        trainAsst.Add(new AssessmentTrainingView()
                        {
                            AssessmentId = 0,
                            Assessment = assess,
                            TrainingId = TrainingId,
                            PublicationId = publicationId,
                            StartDate = startDate,
                            EndDate = endDate,
                            StartTime = startTime,
                            EndTime = endTime
                        });
                    }
                    i++;
                }
                await EnrollForAssessment(trainAsst);

            }
            //admin.AddNotification(TrainerId, UserId, body, "Training", TrainingId, Nstatus, DateTime.Now);
            return RedirectToAction("CourseDetail", "TMS", new { area = "TMS", status = result, CourseCode = batches.CourseCode });
        }


        [HttpGet]
        //[Authorize(Roles = "Admin,Employee,Client")]
        public ActionResult MyTraining(string TId, string Id, string ClientId, string TrainerId, string Status, string StatusforUpdate, string CourseCode, string TrainingId, string CityId, string TrainerMentorId, int? page, string UserAction = "Add", Int64 BatchId = 0, int PageSize = 10)
        {
            UserViewModel userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["UserProfile"] = userDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;

            var EmpDetails = ems.GetEmployeeBasicDetails(userDetail.UserId).FirstOrDefault();
            ViewData["EmpDetails"] = EmpDetails;
            //If Client has team members with all rights
            if (userDetail.CorporateId != null && userDetail.CorporateId != userDetail.SubscriberId)
            {
                userDetail.UserId = userDetail.CorporateId;
            }
            var attach = admin.GetTrainingAttachments(TrainingId);
            ViewData["Attachment"] = attach;
            ViewData["FinalAttach"] = tms.GetTrainingFinalAttachments(TrainingId);
            //ViewBag.UserId = UserId;

            PopulateTrainer(userDetail.UserId);
            PopulateCourse(userDetail.SubscriberId);
            PopulateMentorTrainer(userDetail.SubscriberId);
            if (userDetail.DepartmentId == "CLI")
            {
                if (userDetail.SubscriberId != userDetail.CorporateId)
                {
                    PopulateCourseForMyTraining(userDetail.SubscriberId, userDetail.CorporateId);
                }
                else
                {
                    PopulateCourseForMyTraining(userDetail.SubscriberId, userDetail.UserId);
                }
            }
            else
            {
                PopulateCourseForMyTraining(userDetail.SubscriberId);
            }
            PopulateBatchByCourse(userDetail.SubscriberId, CourseCode);
            PopulateClient(userDetail.SubscriberId);
            PopulateCityForTraining();
            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            ViewBag.Trainer = TrainerId;
            ViewBag.Course = CourseCode;
            ViewBag.Batch = BatchId;
            ViewBag.TrainingStatus = Status;
            ViewBag.TrainingClientId = ClientId;
            ViewBag.TrainingCityId = CityId;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            int cityId = 0;
            if (!string.IsNullOrEmpty(CityId))
            {
                if (CityId != "undefined")
                {
                    cityId = Convert.ToInt32(CityId);
                }
            }

            //For Update or Delete The Training
            if (Id != null && StatusforUpdate != null)
            {
                tms.UpdateStatus(Id, StatusforUpdate);
            }
            if (UserAction == "Delete" && TrainingId != null)
            {
                var trainingattach = admin.GetTrainingAttachments(TrainingId).FirstOrDefault();
                if (trainingattach != null)
                {
                    tms.RemoveTrainingAttach(trainingattach.FileId);
                }
                tms.RemoveTraining(TrainingId);
                return RedirectToAction("MyTraining", "TMS", new { area = "TMS" });
            }


            //If user is client
            if (userDetail.DepartmentId == "CLI")
            {
                if (userDetail.CorporateId != null && userDetail.CorporateId != userDetail.SubscriberId)
                {
                    var myClientTraining = admin.GetTrainingSchedulesForClients(userDetail.CorporateId);
                    //if filtered only on the basis of Status
                    if (!string.IsNullOrEmpty(Status))
                    {
                        myClientTraining = myClientTraining.Where(c => c.Status == Status).ToList();
                        ViewBag.Status = Status;
                    }
                    //if filtered only on the basis of CourseCode
                    if (!string.IsNullOrEmpty(CourseCode))
                    {
                        myClientTraining = myClientTraining.Where(c => c.CourseCode == CourseCode).ToList();
                        PopulateCourseForMyTraining(userDetail.SubscriberId, userDetail.CorporateId);
                    }
                    //if filtered only on the basis of BatchId
                    if (BatchId > 0)
                    {
                        myClientTraining = myClientTraining.Where(c => c.BatchId == BatchId).ToList();
                        PopulateBatchByCourse(userDetail.SubscriberId, CourseCode, BatchId);
                    }
                    //if filtered only on the basis of CityId
                    if (cityId > 0)
                    {
                        myClientTraining = myClientTraining.Where(c => c.CityId == cityId).ToList();
                        PopulateCityForTraining(cityId);
                    }
                    if (!string.IsNullOrEmpty(TrainerMentorId))
                    {
                        myClientTraining = myClientTraining.Where(c => c.TrainerMentorId == TrainerMentorId).ToList();
                        PopulateMentorTrainer(userDetail.SubscriberId, TrainerMentorId);
                    }

                    ViewBag.TotalCount = myClientTraining.Count();
                    return View(myClientTraining.OrderByDescending(c => c.CreatedOn).ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    var myClientTraining = admin.GetTrainingSchedulesForClients(userDetail.UserId);
                    //if filtered only on the basis of Status
                    if (!string.IsNullOrEmpty(Status))
                    {
                        myClientTraining = myClientTraining.Where(c => c.Status == Status).ToList();
                        ViewBag.Status = Status;
                    }
                    //if filtered only on the basis of CourseCode
                    if (!string.IsNullOrEmpty(CourseCode))
                    {
                        myClientTraining = myClientTraining.Where(c => c.CourseCode == CourseCode).ToList();
                        PopulateCourseForMyTraining(userDetail.SubscriberId, userDetail.UserId);
                    }
                    //if filtered only on the basis of BatchId
                    if (BatchId > 0)
                    {
                        myClientTraining = myClientTraining.Where(c => c.BatchId == BatchId).ToList();
                        PopulateBatchByCourse(userDetail.SubscriberId, CourseCode, BatchId);
                    }
                    //if filtered only on the basis of CityId
                    if (cityId > 0)
                    {
                        myClientTraining = myClientTraining.Where(c => c.CityId == cityId).ToList();
                        PopulateCityForTraining(cityId);
                    }
                    if (!string.IsNullOrEmpty(TrainerMentorId))
                    {
                        myClientTraining = myClientTraining.Where(c => c.TrainerMentorId == TrainerMentorId).ToList();
                        PopulateMentorTrainer(userDetail.SubscriberId, TrainerMentorId);
                    }
                    ViewBag.TotalCount = myClientTraining.Count();
                    return View(myClientTraining.OrderByDescending(c => c.CreatedOn).ToPagedList(pageNumber, pageSize));
                }


            }
            //If User is Admin or Academics
            else if (userDetail.UserId == userDetail.SubscriberId || userDetail.DepartmentId == "ADI" || userDetail.DepartmentId == "ACD" || userDetail.DepartmentId == "FIN")
            {
                var myTraining = tms.GetTrainingSchedule(userDetail.SubscriberId, "NA");

                if (!string.IsNullOrEmpty(TrainerId) || !string.IsNullOrEmpty(Status) || !string.IsNullOrEmpty(ClientId) || (!string.IsNullOrEmpty(CourseCode)) || (!string.IsNullOrEmpty(TrainerMentorId)) || BatchId > 0 || cityId > 0)
                {
                    //if filtered only on the basis of TrainerId
                    if (!string.IsNullOrEmpty(TrainerId))
                    {
                        myTraining = myTraining.Where(c => c.TrainerId == TrainerId).ToList();
                        PopulateTrainer(userDetail.UserId, TrainerId);
                    }
                    if (!string.IsNullOrEmpty(Status))
                    {
                        myTraining = myTraining.Where(c => c.Status == Status).ToList();
                    }
                    if (!string.IsNullOrEmpty(ClientId))
                    {
                        myTraining = myTraining.Where(c => c.CorporateId == ClientId).ToList();
                        PopulateClient(userDetail.SubscriberId, ClientId);
                    }
                    if (!string.IsNullOrEmpty(CourseCode))
                    {
                        myTraining = myTraining.Where(c => c.CourseCode == CourseCode).ToList();
                        PopulateCourse(userDetail.SubscriberId, CourseCode);

                    }
                    if (BatchId > 0)
                    {
                        myTraining = myTraining.Where(c => c.BatchId == BatchId).ToList();
                        PopulateBatchByCourse(userDetail.SubscriberId, CourseCode, BatchId);
                    }
                    if (cityId > 0)
                    {
                        myTraining = myTraining.Where(c => c.CityId == cityId).ToList();
                        PopulateCityForTraining(cityId);
                    }
                    if (!string.IsNullOrEmpty(TrainerMentorId))
                    {
                        myTraining = myTraining.Where(c => c.TrainerMentorId == TrainerMentorId).ToList();
                        PopulateMentorTrainer(userDetail.SubscriberId, TrainerMentorId);
                    }
                }
                ViewBag.TotalCount = myTraining.Count();
                return View(myTraining.OrderByDescending(c => c.CreatedOn).ToPagedList(pageNumber, pageSize));
            }
            //if User is Employee
            else
            {
                List<TrainingScheduleView> myTraining = tms.GetTrainingSchedule(userDetail.UserId, "NA").ToList();
                List<TrainingScheduleView> training = new List<TrainingScheduleView>();
                foreach (TrainingScheduleView other in myTraining)
                {
                    if (other.OtherTrainerId != null)
                    {
                        string[] othert = other.OtherTrainerId.Split(',');
                        foreach (var ot in othert)
                        {
                            if (ot == TrainerId)
                            {

                                training.Add(other);
                            }
                        }
                    }

                }

                if ((!string.IsNullOrEmpty(Status)) || (!string.IsNullOrEmpty(CourseCode)) || (!string.IsNullOrEmpty(TrainerMentorId)) || BatchId > 0)//
                {
                    if (!string.IsNullOrEmpty(Status))
                    {
                        myTraining = myTraining.Where(c => c.Status == Status).ToList();
                        ViewBag.Status = Status;
                    }

                    if (!string.IsNullOrEmpty(CourseCode))
                    {
                        myTraining = myTraining.Where(c => c.CourseCode == CourseCode).ToList();
                        PopulateCourse(userDetail.SubscriberId, CourseCode);
                    }

                    if (BatchId > 0)
                    {
                        myTraining = myTraining.Where(c => c.BatchId == BatchId).ToList();
                        PopulateBatchByCourse(userDetail.SubscriberId, CourseCode, BatchId);
                    }
                    if (userDetail.DepartmentId == "FAC" && userDetail.ManagerLevel == true)
                    {
                        if (!string.IsNullOrEmpty(TrainerMentorId))
                        {
                            myTraining = myTraining.Where(c => c.TrainerMentorId == TrainerMentorId).ToList();
                            PopulateMentorTrainer(userDetail.SubscriberId, TrainerMentorId);
                        }
                    }
                }

                var list = myTraining.Concat(training).ToList();
                ViewBag.TotalCount = list.Count();
                return View(list.OrderByDescending(c => c.TrainingId).ToPagedList(pageNumber, pageSize));
            }
        }

        //[Authorize(Roles = "Admin,Employee,Client")]
        public ActionResult TrainingDetails(string Id, string Status)
        {
            UserViewModel userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetail.UserId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            //If Client has team members with all rights
            if (userDetail.CorporateId != null && userDetail.CorporateId != userDetail.SubscriberId)
            {
                userDetail.UserId = userDetail.CorporateId;
            }
            ViewData["Attachment"] = admin.GetTrainingAttachments(Id);
            ViewData["FinalAttach"] = tms.GetTrainingFinalAttachments(Id);
            ViewData["TrainingAssessment"] = tms.GetTrainingAssessments(Id);
            var training = tms.GetTrainingScheduleById(userDetail.UserId, Id);
            CourseBatchViewModel courseBatches = tms.GetCourseBatches(userDetail.SubscriberId, training.BatchId).FirstOrDefault();
            ViewData["CourseBatches"] = courseBatches;
            ViewBag.UserId = userDetail.UserId;

            if (Id != null && Status != null)
            {
                bool status = tms.UpdateStatus(Id, Status);
                if (status == true)
                {
                    ViewBag.Result = "Succeeded";
                }
            }
            return View(training);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Employee,Client")]
        public ActionResult SendNotification(string TrainingId, string CourseCode)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId);
            //Sending SMS & Email To Trainer on Scheduling Training
            var Training = db.TrainingSchedule.Find(TrainingId);

            var BatchCandidates = admin.GetBatchCandidates(CourseCode);
            BatchCandidates = BatchCandidates.Where(c => c.BatchId == Training.BatchId).ToList();

            var TrainerDetail = generic.GetUserDetail(Training.TrainerId);
            var subscriberDetail = cms.GetCorporateProfile(userDetail.SubscriberId).FirstOrDefault();

            string mobile = admin.GetUserRegistrationDetails(Training.TrainerId).PhoneNumber;
            string email = admin.GetUserRegistrationDetails(Training.TrainerId).Email;
            if (!string.IsNullOrEmpty(mobile))
            {
                string message1 = "Dear Shri/Smt  \r\n\r\n Welcome to the Training program for " + Training.SubjectLine + "( " + TrainingId + " )" + " to be held from " + BatchCandidates.FirstOrDefault().FromDate.ToString("dd-MMM-yyyy") + " To " + BatchCandidates.FirstOrDefault().ToDate.ToString("dd-MMM-yyyy") + " At " + Training.Address + ".  \r\n You are requested to report at the venue one day before the commencement of the training. Please carry your AADHAR card, PAN card and self attested copies of both.  \r\n \r\n All the best! /n Training Team,  \r\n India Post payments Bank";
                //string message1 = "Dear Sir/Madam <br/> A Training " + Training.SubjectLine + "( " + TrainingId + " )" + " has been assigned to you by " + generic.GetUserDetail(userDetail.SubscriberId).Name + " At " + Training.Address + " From " + BatchCandidates.FirstOrDefault().FromDate.ToString("dd-MM-yyyy") + " To " + BatchCandidates.FirstOrDefault().ToDate.ToString("dd-MM-yyyy");
                // generic.sendSMSMessage(message1, mobile);
                generic.sendSMS(message1, mobile);
            }
            //if (!string.IsNullOrEmpty(email))
            //{
            //    string callbackUrl = await SendTrainerEmail(Training.SubjectLine, TrainingId, subscriberDetail.Name, Training.TrainerId, "Training Schedule", userDetail.PhoneNumber, userDetail.Name);
            //}

            //Sending SMS & Email to Candidate while assigning to Batch
            if (BatchCandidates.Count > 0)
            {
                foreach (var candidate in BatchCandidates)
                {
                    if (candidate.PhoneNumber != null)
                    {
                        string candidatemobile = candidate.PhoneNumber;
                        //string message = "Dear " + candidate.Name + ", <br /> you have been Nominated " + Training.SubjectLine + "( " + TrainingId + " )" + " By " + generic.GetUserDetail(userDetail.SubscriberId).Name + " From " + candidate.FromDate.ToString("dd-MM-yyyy") + " To " + candidate.ToDate.ToString("dd-MM-yyyy") + " Timings " + "From " + candidate.FromTime + " To " + candidate.ToTime + " At " + Training.Address;
                        string message = "Dear Shri/Smt " + candidate.Name + ", \r\n\r\n Welcome to the Training program for " + Training.SubjectLine + "( " + TrainingId + " )" + " to be held from " + BatchCandidates.FirstOrDefault().FromDate.ToString("dd-MMM-yyyy") + " To " + BatchCandidates.FirstOrDefault().ToDate.ToString("dd-MMM-yyyy") + " At " + Training.Address + ". \r\n You are requested to report at the venue one day before the commencement of the training. Please carry your AADHAR card, PAN card and self attested copies of both. \r\n\r\n All the best! \r\n Training Team, \r\n India Post payments Bank";
                        //generic.sendSMSMessage(message, candidatemobile);
                        generic.sendSMS(message, candidatemobile);
                    }
                    //if (candidate.Email != null)
                    //{
                    //    string callbackUrl = await SendCandidateEmail(Training.SubjectLine, TrainingId, subscriberDetail.Name, candidate.UserId, "Training Schedule", candidate.PhoneNumber, candidate.Name, candidate.BatchName, candidate.FromDate, candidate.ToDate, candidate.FromTime, candidate.ToTime);
                    //}
                }
            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Employee,Client")]
        public async Task<ActionResult> SendAssessmentNotification(string TrainingId, Int64 AssessmentId, string CourseCode)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId);
            //Sending SMS & Email To Trainer on Scheduling Training
            var Training = db.TrainingSchedule.Find(TrainingId);
            var Assessment = db.TrainingAssessment.Find(AssessmentId);
            var BatchCandidates = admin.GetBatchCandidates(CourseCode);
            BatchCandidates = BatchCandidates.Where(c => c.BatchId == Training.BatchId).ToList();

            var TrainerDetail = generic.GetUserDetail(Training.TrainerId);
            var subscriberDetail = cms.GetCorporateProfile(userDetail.SubscriberId).FirstOrDefault();

            string mobile = admin.GetUserRegistrationDetails(Training.TrainerId).PhoneNumber;
            string email = admin.GetUserRegistrationDetails(Training.TrainerId).Email;
            if (!string.IsNullOrEmpty(mobile))
            {
                //string message1 = "A Training " + Training.SubjectLine + "(" + TrainingId + ")" + " has been assigned to you by " + generic.GetUserDetail(userDetail.SubscriberId).Name; //eg "message hello ";                
                string message1 = "Dear Sir/Madam <br/> A Assessment " + Assessment.Assessment + " has been scheduled for Training" + Training.SubjectLine + "(" + TrainingId + ")" + " From " + Assessment.StartDate + "-" + Assessment.StartTime.ToString("dd-MM-yyyy") + " To " + Assessment.EndDate + "-" + Assessment.EndTime.ToString("dd-MM-yyyy");
                // generic.sendSMSMessage(message1, mobile);
                generic.sendSMS(message1, mobile);
            }
            if (!string.IsNullOrEmpty(email))
            {
                string callbackUrl = await SendTrainerEmailforAssessment(Training.SubjectLine, TrainingId, subscriberDetail.Name, Training.TrainerId, "Training Schedule", userDetail.PhoneNumber, userDetail.Name, Assessment.Assessment, Assessment.StartDate, Assessment.StartTime, Assessment.EndDate, Assessment.EndTime);
            }

            //Sending SMS & Email to Candidate while assigning to Batch
            if (BatchCandidates.Count > 0)
            {
                foreach (var candidate in BatchCandidates)
                {
                    if (candidate.PhoneNumber != null)
                    {
                        string candidatemobile = candidate.PhoneNumber;
                        // string message = "Hello" + candidate.Name + ",  you are Assigned to Training  " + Training.SubjectLine + "(" + TrainingId + ")" + "From " + candidate.FromDate.ToString("dd-MM-yyyy") + " To " + candidate.ToDate.ToString("dd-MM-yyyy") + " Timings " + "From " + candidate.FromTime + " To " + candidate.ToTime;
                        string message = "Dear Sir/Madam <br/> A Assessment " + Assessment.Assessment + " has been scheduled for Training" + Training.SubjectLine + "(" + TrainingId + ")" + " From " + Assessment.StartDate + "-" + Assessment.StartTime.ToString("dd-MM-yyyy") + " To " + Assessment.EndDate + "-" + Assessment.EndTime.ToString("dd-MM-yyyy");
                        //generic.sendSMSMessage(message, candidatemobile);
                        generic.sendSMS(message, candidatemobile);
                    }
                    if (candidate.Email != null)
                    {
                        string callbackUrl = await SendCandidateEmailforAssessment(Training.SubjectLine, TrainingId, subscriberDetail.Name, candidate.UserId, "Training Schedule", candidate.PhoneNumber, candidate.Name, candidate.BatchName, candidate.FromDate, candidate.ToDate, candidate.FromTime, candidate.ToTime, Assessment.Assessment, Assessment.StartDate, Assessment.StartTime, Assessment.EndDate, Assessment.EndTime);
                    }
                }
            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult TrainerPlanner(DateTime? FromDate, DateTime? ToDate, DateTime? FromTime, DateTime? ToTime, string ViewTypes, bool status = false, long plannerId = 0, string useraction = "add")
        {
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            TrainerPlannerView trainerPlan = new TrainerPlannerView();

            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["UserProfile"] = userDetail;
            var empDetail = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["EmpDetails"] = empDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.UserId = UserId;
            ViewBag.UserRole = userDetail.Role;
            ViewBag.PlannerId = plannerId;
            ViewBag.FromDate = DateTime.Now;
            ViewBag.ToDate = DateTime.Now;
            ViewBag.Action = useraction;
            var employeeDetails = db.EmpJoiningDetail.Where(c => c.UserId == UserId).FirstOrDefault();
            Int16 SchemeId = 0;
            if (userDetail.DepartmentId == "ADI")
            {
                SchemeId = 4;
                PopulateEngagementType(userDetail.SubscriberId, SchemeId);
            }
            else
            {
                if (employeeDetails != null)
                    SchemeId = employeeDetails.SchemeId;
                PopulateEngagementType(userDetail.SubscriberId, SchemeId, empDetail.Gender);
            }


            List<Global.ShiftTime> shifts = Global.GetShiftTimeList();
            ViewBag.StartShiftTimings = new SelectList(shifts, "Time", "Time", shifts.FirstOrDefault().Time);
            ViewBag.EndShiftTimings = new SelectList(shifts, "Time", "Time", shifts.LastOrDefault().Time);
            List<TrainerPlannerView> trainerPlanner = tms.GetTrainerPlaner(UserId);
            ViewData["TrainerPlanner"] = trainerPlanner.AsEnumerable();
            if (useraction == "Cancel" && plannerId > 0)
            {
                var result = tms.EngagementApprovalStatus(3, userDetail.UserId, DateTime.Now, plannerId);
                PopulateEngagementType(userDetail.SubscriberId, employeeDetails.SchemeId);
                ViewBag.Deleted = "Deleted";
                return View();
            }
            //var tourid = db.EmployeeTour.Where(x => x.UserId == UserId).ToList();
            //ViewBag.Tour = 0;
            //if (tourid.Count() > 0)
            //{
            //    ViewBag.Tour = tourid.Count;
            //}

            if (useraction == "Edit")
            {
                trainerPlan = trainerPlanner.Where(i => i.PlannerId == plannerId).FirstOrDefault();
                ViewBag.StartShiftTimings = new SelectList(shifts, "Time", "Time", trainerPlan.FromTime.TimeOfDay);
                ViewBag.EndShiftTimings = new SelectList(shifts, "Time", "Time", trainerPlan.ToTime.TimeOfDay);
                FromDate = trainerPlan.FromDate;
                var fromDate = FromDate.Value.ToString("dd-MM-yyyy");
                ToDate = trainerPlan.ToDate;
                var toDate = ToDate.Value.ToString("dd-MM-yyyy");
                FromTime = trainerPlan.FromTime;
                var fromTime = FromTime;
                ToTime = trainerPlan.ToTime;
                var toTime = ToTime;
                var schedule = fromDate + " " + fromTime.Value.ToString("hh:mm:ss tt") + " " + "-" + " " + toDate + " " + toTime.Value.ToString("hh:mm:ss tt");
                ViewBag.Schedule = schedule;
                PopulateEngagementType(userDetail.SubscriberId, employeeDetails.SchemeId, empDetail.Gender, trainerPlan.EngagementTypeId);

                ViewData["Content"] = (from i in userContext.TrainerPlannerAttachment.Where(i => i.PlannerId == plannerId) select i).FirstOrDefault();
            }
            if (trainerPlan.FromDate.ToShortDateString().Equals("1/1/0001"))
            {
                trainerPlan.FromDate = DateTime.Today;
            }
            if (trainerPlan.ToDate.ToShortDateString().Equals("1/1/0001"))
            {
                trainerPlan.ToDate = DateTime.Today;
            }
            return View(trainerPlan);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Employee")]
        public async Task<ActionResult> TrainerPlanner(TrainerPlanner trainerPlan, HttpPostedFileBase uploadPhoto, string Schedule)
        {
            //Make an entry into Trainer Planner
            string userId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(userId);
            trainerPlan.TrainerId = userId;
            trainerPlan.IsApproved = 0;

            string[] strSchedule = Schedule.Split('-');
            DateTime frmdate = DateTime.ParseExact(strSchedule[0].Trim(), "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);

            trainerPlan.FromDate = frmdate;
            trainerPlan.FromTime = Convert.ToDateTime(frmdate.ToShortTimeString());

            DateTime todate = DateTime.ParseExact(strSchedule[1].Trim(), "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);
            trainerPlan.ToDate = todate;
            trainerPlan.ToTime = Convert.ToDateTime(todate.ToShortTimeString());
            var employeeDetails = db.EmpJoiningDetail.Where(c => c.UserId == userId).FirstOrDefault();
            if (employeeDetails != null)
            {
                trainerPlan.SchemeId = employeeDetails.SchemeId;
            }
            //trainerPlan.FromTime = Convert.ToDateTime(startShiftTimings);
            //trainerPlan.ToTime = Convert.ToDateTime(endShiftTimings);
            bool result = tms.AddTrainerPlan(trainerPlan);
            if (result && uploadPhoto != null)
            {
                if (trainerPlan.PlannerId == 0)
                    trainerPlan.PlannerId = db.TrainerPlanner.OrderByDescending(t => t.PlannerId).FirstOrDefault().PlannerId;
                //foreach (string file in Request.Files)
                //{
                //    HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
                tms.uploadLeaveAttachment(trainerPlan.PlannerId, uploadPhoto);
                //}

            }



            if (result == true)
            {
                EngagementTypeMaster EngagementType = db.EngagementTypeMaster.Find(trainerPlan.EngagementTypeId);
                if (userDetail.Role != "Admin")
                {
                    string callbackUrl = await SendApplicationLeaveEmailTokenAsync(userDetail.ReportingAuthority, userDetail.ReportingAuthorityname, userDetail.Name, trainerPlan.Remarks, EngagementType.EngagementType, trainerPlan.FromDate, trainerPlan.ToDate);
                    admin.AddNotification(userDetail.ReportingAuthority, userDetail.UserId, "You received Leave application from " + userDetail.Name + ".", "Engagement", trainerPlan.PlannerId.ToString(), false, DateTime.Now);
                }
            }
            return RedirectToAction("TrainerPlanner", "TMS", new { area = "TMS", status = result });
        }

        ////[Authorize(Roles = "Admin,Employee")]
        //public ActionResult RemovePlan(long Id)
        //{
        //    bool result = tms.DeleteTrainerPlan(Id);
        //    return RedirectToAction("TrainerPlanner", "TMS");
        //}


        /// <summary>
        ///Created By : Ajay kumar Choudhary
        ///Created On : 22-05-2017
        ///For Creating Holiday Calendar
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult Holiday(DateTime? FromDate, DateTime? ToDate, bool status = false, long HolidayId = 0, string useraction = "add")
        {
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            Holiday holiday = new Holiday();

            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["UserProfile"] = userDetail;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;

            ViewBag.UserId = UserId;
            ViewBag.UserRole = userDetail.Role;
            ViewBag.HolidayId = HolidayId;
            ViewBag.FromDate = DateTime.Now;
            ViewBag.ToDate = DateTime.Now;
            ViewBag.Action = useraction;
            List<Global.ShiftTime> shifts = Global.GetShiftTimeList();
            ViewBag.StartShiftTimings = new SelectList(shifts, "Time", "Time", shifts.FirstOrDefault().Time);
            ViewBag.EndShiftTimings = new SelectList(shifts, "Time", "Time", shifts.LastOrDefault().Time);
            List<Holiday> holidays = tms.GetHolidays(userDetail.SubscriberId);
            ViewData["HolidayPlanner"] = holidays.AsEnumerable();
            if (useraction == "Edit")
            {
                holiday = holidays.Where(i => i.HolidayId == HolidayId).FirstOrDefault();
                FromDate = holiday.FromDate;
                var fromDate = FromDate.Value.ToString("dd-MM-yyyy");
                ToDate = holiday.ToDate;
                var toDate = ToDate.Value.ToString("dd-MM-yyyy");
                var schedule = fromDate + "-" + " " + toDate;
                ViewBag.Schedule = schedule;
            }
            if (useraction == "Delete" && HolidayId != 0)
            {
                tms.RemoveHolidays(HolidayId);
                return RedirectToAction("Holiday", "TMS", new { area = "TMS" });
            }
            if (holiday.FromDate.ToShortDateString().Equals("1/1/0001"))
            {
                holiday.FromDate = DateTime.Today;
            }
            if (holiday.ToDate.ToShortDateString().Equals("1/1/0001"))
            {
                holiday.ToDate = DateTime.Today;
            }
            return View(holiday);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult Holiday(Holiday holiday, string Schedule)
        {
            //Make an entry for Holiday
            string userId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(userId);
            holiday.CorporateId = userId;

            string[] strSchedule = Schedule.Split('-');
            DateTime frmdate = DateTime.ParseExact(strSchedule[0].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            holiday.FromDate = frmdate;

            DateTime todate = DateTime.ParseExact(strSchedule[1].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            holiday.ToDate = todate;

            bool result = tms.AddHoliday(holiday);

            return RedirectToAction("Holiday", new { area = "TMS", status = result });
        }

        [HttpGet]
        public ActionResult UploadAttachment(string TrainingId)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.TrainingId = TrainingId;
            return View();
        }

        [HttpPost]
        public ActionResult UploadAttachment(string TrainingId, string fileName, string Status = "Completed")
        {
            if (!string.IsNullOrEmpty(TrainingId))
            {
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
                    tms.uploadTrainingFile(TrainingId, attachment);
                }
            }
            if (TrainingId != null && Status != null)
            {
                tms.UpdateStatus(TrainingId, Status);
            }

            return RedirectToAction("MyTraining", "TMS");
        }

        [HttpGet]
        public ActionResult TrainingEvaluation(string Id, Int64 Assessment = 0)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel UserDetails = generic.GetUserDetail(UserId);
            ViewData["userprofile"] = UserDetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.TrainingId = Id;
            //var weightage = db.TrainingAssessment.Where(c => c.TrainingId == Id).ToList();
            //ViewData["weightage"] = weightage;
            ViewBag.AssessId = Assessment;

            if (Assessment > 0)
            {
                var TrainingAssessments = tms.GetTrainingAssessments(Id).Where(c => c.AssessmentId == Assessment);
                var candidate = student.GetSubscriberWiseCandidateList(UserDetails.SubscriberId, TrainingAssessments.FirstOrDefault().BatchId).OrderBy(c => c.Name).ToList();
                ViewBag.CandidateCount = candidate.Count;
                ViewData["Candidate"] = candidate;
                PopulateMarksList();
                var Assessments = tms.GetAssessmentEvaluation(Id).ToList();
                ViewData["Assessments"] = Assessments.Where(c => c.AssessmentId == Assessment);
                return View(TrainingAssessments);
            }
            else
            {
                var TrainingAssessments = tms.GetTrainingAssessments(Id);
                var candidate = student.GetSubscriberWiseCandidateList(UserDetails.SubscriberId, TrainingAssessments.FirstOrDefault().BatchId).OrderBy(c => c.Name).ToList();
                ViewBag.CandidateCount = candidate.Count;
                ViewData["Candidate"] = candidate;
                PopulateMarksList();
                //var Assessments = db.AssessmentEvaluation.Where(a => a.TrainingId == Id).ToList();
                var Assessments = tms.GetAssessmentEvaluation(Id).ToList();
                ViewData["Assessments"] = Assessments;
                return View(TrainingAssessments);
            }
        }

        [HttpPost]
        public ActionResult TrainingEvaluation(string TrainingId, string[] UserId, Int64[] AssessmentId, float[] Marks)
        {
            string UpdatedBy = User.Identity.GetUserId();

            int i = 0;
            foreach (var mark in Marks)
            {
                var result = tms.AddEvaluationMarks(AssessmentId[i], UserId[i], TrainingId, mark, DateTime.Now, UpdatedBy);
                i++;
            }

            return RedirectToAction("TrainingEvaluation", "TMS", new { area = "TMS" });
        }

        [HttpPost]
        public async Task<ActionResult> CourseIntegrateToLMS(string WikiCourseCode, string CourseCode)
        {
            bool result = false;
            string UserId = User.Identity.GetUserId();
            result = await admin.IntegrateLMSCourse(CourseCode, WikiCourseCode, UserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void PopulateBatch(string SubscriberId, object selectedValue = null)
        {

            var query = tms.GetCourseBatches(SubscriberId, 0);
            SelectList BatchId = new SelectList(query, "BatchId", "BatchName", selectedValue);
            ViewBag.BatchId = BatchId;
        }

        private void PopulateCurrency(Object selectedCurrency = null)
        {
            var query = generic.GetCurrency();
            SelectList CurrencyList = new SelectList(query, "Currency", "Currency", selectedCurrency);
            ViewBag.Currency = CurrencyList;
        }

        private void PopulateContentVisiblity(object selectedValue = null)
        {
            var ContentVisiblityList = Global.GetContentVisiblity();
            ViewBag.ContentVisiblity = new SelectList(ContentVisiblityList, "ContentVisiblity", "ContentVisiblityName", selectedValue);
        }

        private void PopulateTrainer(string SubscriberId, object selectedValue = null)
        {
            EMSManager emsMgr = new EMSManager();
            var TrainerId = emsMgr.GetSubscriberWiseEmployeeList(SubscriberId).Where(e => e.DepartmentId == "ADI" || e.DepartmentId == "FAC" || e.DepartmentId == "VFA").ToList();
            ViewBag.TrainerId = new SelectList(TrainerId, "UserId", "Name", selectedValue);
        }

        private void PopulateClient(string SubscriberId, object selectedOrderType = null)
        {
            var query = generic.GetSubscriberWiseClientListBulkUpload(SubscriberId, false);
            SelectList OrderTypes = new SelectList(query, "CorporateId", "Name", selectedOrderType);
            ViewBag.CorporateId = OrderTypes;
        }

        private void PopulateWarden(string SubscriberId, object selectedValue = null)
        {
            EMSManager emsMgr = new EMSManager();
            var WardenId = emsMgr.GetSubscriberWiseEmployeeList(SubscriberId).Where(e => e.DepartmentId == "ADI" || e.DepartmentId == "WAR").ToList();
            ViewBag.WardenId = new SelectList(WardenId, "UserId", "Name", selectedValue);
        }

        private void PopulateTrainings(Int64 BatchId, object selectedValue = null)
        {
            var TrainingId = db.TrainingSchedule.Where(train => train.BatchId == BatchId).ToList();
            ViewBag.TrainingId = new SelectList(TrainingId, "TrainingId", "SubjectLine", selectedValue);
            ViewBag.ETrainingId = new SelectList(TrainingId, "TrainingId", "SubjectLine", selectedValue);
            ViewBag.RTrainingId = new SelectList(TrainingId, "TrainingId", "SubjectLine", selectedValue);
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

        private void PopulateCity(int StateId = 0, object selectedCity = null)
        {
            var query = generic.GetCity(StateId);
            SelectList Cities = new SelectList(query, "CityId", "City", selectedCity);
            ViewBag.CityId = Cities;
        }

        private void PopulateCityForTraining(object selectedCity = null)
        {
            string userId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(userId);
            if (userDetail.DepartmentId == "CLI")
            {
                var query = admin.GetTrainingSchedulesCitiesForClients(userId);
                SelectList Cities = new SelectList(query, "CityId", "City", selectedCity);
                ViewBag.CityId = Cities;
            }
            else
            {
                var query = admin.GetTrainingSchedulesForCities(userDetail.SubscriberId);
                SelectList Cities = new SelectList(query, "CityId", "City", selectedCity);
                ViewBag.CityId = Cities;
            }

        }


        private void PopulateCAvailability(object selectedValue = null)
        {
            var ContentAvailabilityList = Global.GetContentAvailability();
            ViewBag.ContentAvailability = new SelectList(ContentAvailabilityList, "ContentAvailability", "AvaibilityName", selectedValue);
        }

        private void PopulateCourse(string SubscriberId, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetCourseDetails(SubscriberId);
            SelectList CourseCode = new SelectList(query, "CourseCode", "CourseName", selectedValue);
            ViewBag.CourseCode = CourseCode;
        }
        private void PopulateCourseForMyTraining(string SubscriberId, string CorporateId = null, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetCourseDetails(SubscriberId);
            if (CorporateId != null)
            {
                var query1 = query.Where(c => c.CorporateId == CorporateId).ToList();
                SelectList CourseCode = new SelectList(query1, "CourseCode", "CourseName", selectedValue);
                ViewBag.CourseCode = CourseCode;
            }
            else
            {
                SelectList CourseCode = new SelectList(query, "CourseCode", "CourseName", selectedValue);
                ViewBag.CourseCode = CourseCode;
            }
        }

        private void PopulateBatchByCourse(string SubscriberId = null, string CourseCode = null, object selectedValue = null, bool DateFilter = false)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetBatches(SubscriberId, CourseCode);


            if (DateFilter)
                query = query.Where(b => b.ToDate >= DateTime.UtcNow.Date).ToList();

            SelectList BatchId = new SelectList(query, "BatchId", "BatchName", selectedValue);
            ViewBag.BatchId = BatchId;
            ViewBag.ABatchId = BatchId;
            ViewBag.EBatchId = BatchId;
            ViewBag.RBatchId = BatchId;
        }

        private void PopulateBatchByCourseForDuration(string SubscriberId = null, string CourseCode = null, object selectedValue = null, bool DateFilter = false)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetBatches(SubscriberId, CourseCode);
            if (DateFilter)
                query = query.Where(b => b.ToDate >= DateTime.UtcNow.Date).ToList();

            SelectList BatchId = new SelectList(query, "BatchId", "BatchNameDuration", selectedValue);
            ViewBag.BatchId = BatchId;
            ViewBag.ABatchId = BatchId;
            ViewBag.EBatchId = BatchId;
            ViewBag.RBatchId = BatchId;
        }

        private void PopulateDuration(object selectedValue = null)
        {
            var query = Global.GetDuration();
            SelectList CourseDurations = new SelectList(query, "Duration", "DurationName", selectedValue);
            ViewBag.CourseDuration = CourseDurations;
        }

        private void PopulateEngagementType(string SubscriberId, Int16 SchemeId, string Gender = "TR", object selectedValue = null)
        {
            var query = tms.GetEngagementType(SubscriberId);
            if (Gender == "MA")
                query = query.Where(q => q.SchemeId == SchemeId && q.LeaveTypeId != "ML").ToList();
            else if (Gender == "FE")
                query = query.Where(q => q.SchemeId == SchemeId && q.LeaveTypeId != "PL").ToList();
            else
                query = query.Where(q => q.SchemeId == SchemeId && q.LeaveTypeId != "ML" && q.LeaveTypeId != "PL").ToList();

            SelectList Engagement = new SelectList(query, "EngagementTypeId", "EngagementType", selectedValue);
            ViewBag.EngagementTypeId = Engagement;
        }

        [HttpPost]
        public ActionResult GetState(string CountryId)
        {
            int countryId;
            List<SelectListItem> StateId = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(CountryId))
            {
                countryId = Convert.ToInt32(CountryId);
                List<StatesMaster> States = db.StatesMaster.Where(x => x.CountryId == countryId).ToList();
                States.ForEach(x =>
                {
                    StateId.Add(new SelectListItem { Text = x.State, Value = x.StateId.ToString() });
                });
            }
            return Json(StateId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetBranch(string StateId)
        {
            int stateId;
            List<SelectListItem> CityId = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(StateId))
            {
                stateId = Convert.ToInt32(StateId);
                List<CityMaster> Cities = db.CityMaster.Where(x => x.StateId == stateId).ToList();
                Cities.ForEach(x =>
                {
                    CityId.Add(new SelectListItem { Text = x.City, Value = x.CityId.ToString() });
                });
            }
            return Json(CityId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TrainerAvailability(string TrainerId, string BatchId)
        {
            bool status = true;
            int batchId = 0;
            if (!string.IsNullOrEmpty(BatchId))
                batchId = Convert.ToInt32(BatchId);
            var Trainer = tms.GetTrainerAvailability(TrainerId, batchId);
            if (Trainer.Count > 0)
            {
                status = false;
            }
            else
            {
                status = true;
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TrainingForBatch(Int64? BatchId)
        {
            BatchId = BatchId ?? 0;
            var TrainingId = db.TrainingSchedule.Where(train => train.BatchId == BatchId).ToList();
            ViewBag.TrainingId = new SelectList(TrainingId, "TrainingId", "SubjectLine");
            return Json(TrainingId, JsonRequestBehavior.AllowGet);
        }

        private void PopulateTasklist(string SubscriberId, object selectedValue = null)
        {
            Generic generic = new Generic();
            var TaskId = generic.GetTask().Where(c => c.SubscriberId == SubscriberId || c.TaskId == String.Empty).ToList();
            ViewBag.TaskId = new SelectList(TaskId, "TaskId", "TaskId", selectedValue);
        }

        [HttpPost]
        public ActionResult GetBatch(string CourseCode)
        {
            List<SelectListItem> BatchId = new List<SelectListItem>();
            UserViewModel userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            string SubscriberId = userDetail.SubscriberId;
            if (!string.IsNullOrEmpty(CourseCode))
            {
                List<CourseBatch> Batches = (from b in userContext.CourseBatch
                                             join c in userContext.CourseMaster
                                             on b.CourseCode equals c.CourseCode
                                             where b.CourseCode == CourseCode && c.SubscriberId == SubscriberId
                                             && b.ToDate >= DateTime.UtcNow
                                             select b).ToList();
                Batches.ForEach(x =>
                {
                    BatchId.Add(new SelectListItem { Text = x.BatchName, Value = x.BatchId.ToString() });
                });
            }
            return Json(BatchId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> ResendToken(string UserId, string UserName, string Employeename, string Reason, string EngagementType, DateTime? From, DateTime? To)
        {
            string callbackUrl = await SendApplicationLeaveEmailTokenAsync(UserId, UserName, Employeename, Reason, EngagementType, From, To);
            return Json(callbackUrl, JsonRequestBehavior.AllowGet);
        }

        private async Task<string> SendApplicationLeaveEmailTokenAsync(string UserId, string UserName, string Employeename, string Reason, string EngagementType, DateTime? From, DateTime? To)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(UserId);
            var callbackUrl = Url.Action("Trainers", "Calendar",
               new { area = "CMS", userId = UserId, code = code }, protocol: Request.Url.Scheme);
            var msgBody = "Hello  " + UserName + ", <br/> <br/>" +
                " You received Leave application from " + Employeename + "  ." +
                "<br/><br/><b>Reason:</b> " + Reason +
                "<br/><br/><b>Dates:</b> " + From + "-" + To +
                 "<br/><br/>Thanks & Regards" +
                "<br/>RECKONN";

            await UserManager.SendEmailAsync(UserId, EngagementType, msgBody);

            return callbackUrl;
        }

        private void PopulateMarksList(Object selectName = null)
        {
            var MarksfullList = Global.GetMarks();
            SelectList Marks = new SelectList(MarksfullList, "Marks", "Marks", selectName);
            ViewBag.Marks = Marks;
            ViewData["Marks"] = MarksfullList;
        }

        public void PopulateWikipianCourse(string SubscriberId, object selectedValue = null)
        {
            try
            {
                using (WebClient webClient = new System.Net.WebClient())
                {
                    var url = Global.WikipianUrl() + "api/value/GetCourses?SubscriberId=" + SubscriberId;
                    var myString = webClient.DownloadString(url);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    CourseMasterViewModel[] myData = js.Deserialize<CourseMasterViewModel[]>(myString);
                    IEnumerable<CourseMasterViewModel> courses = myData;
                    SelectList CourseCode = new SelectList(courses, "CourseCode", "CourseName", selectedValue);
                    ViewBag.WikiCourseCode = CourseCode;
                }
            }
            catch (Exception)
            {

            }
        }

        private void PopulateOtherTrainer(string SubscriberId, string[] selectedtrainer = null)
        {
            //First way to populate dropdownlist
            //ViewBag.OtherTrainerId = new MultiSelectList(generic.GetSubscriberWiseList(SubscriberId).Where(e => e.DepartmentId == "ADI" || e.DepartmentId == "FAC" || e.DepartmentId == "VFA"), "UserId", "Name");
            var query = generic.GetSubscriberWiseList(SubscriberId).Where(e => e.DepartmentId == "ADI" || e.DepartmentId == "FAC" || e.DepartmentId == "VFA");
            MultiSelectList OtherTrainerId = new MultiSelectList(query, "UserId", "Name", selectedtrainer);
            ViewBag.OtherTrainerId = OtherTrainerId;

        }


        /// <summary>
        /// Created By Kulesh on 31-Jul-2017
        /// </summary>
        /// <param name="CategoryId"></param>
        private void PopulateCategory(Int64 CategoryId = 0)
        {
            var categories = db.Category.ToList();
            SelectList Categories = new SelectList(categories, "CategoryId", "CategoryName", CategoryId);
            ViewBag.CategoryId = Categories;
        }

        private void PopulateAssessment(string SubscriberId)
        {
            List<PublicationView> asstCat = new List<PublicationView>();
            using (WebClient webClient = new System.Net.WebClient())
            {
                try
                {
                    string URL = Global.PreloreUrl() + "/Api/Value/GetAssessment?UserId=" + SubscriberId;
                    var json = webClient.DownloadString(URL);
                    ViewBag.OriginalData = Convert.ToString(json);

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    PublicationView[] myData = js.Deserialize<PublicationView[]>(json);
                    asstCat = myData.ToList();
                }
                catch
                {

                }
                ViewData["Assessments"] = asstCat;
            }
        }

        public async Task<bool> EnrollForAssessment(List<AssessmentTrainingView> AsstTraining)
        {
            bool Result = false;
            if (AsstTraining.Count() > 0)
            {

                string UserId = User.Identity.GetUserId();
                string TrainingId = AsstTraining[0].TrainingId;
                Int64 BatchId = db.TrainingSchedule.Where(t => t.TrainingId == TrainingId).FirstOrDefault().BatchId;
                UserViewModel userDetails = generic.GetUserDetail(UserId);
                var candidateDetails = student.GetSubscriberWiseCandidateList(userDetails.SubscriberId, BatchId).OrderBy(c => c.Name).ToList();
                List<CandidateRegisterView> candiReg = new List<CandidateRegisterView>();

                foreach (var Ast in AsstTraining)
                {
                    foreach (var candi in candidateDetails)
                    {
                        candiReg.Add(new CandidateRegisterView()
                        {
                            CandidateId = candi.UserId,
                            Name = candi.Name,
                            Email = candi.Email,
                            PhoneNumber = candi.PhoneNumber,
                            UserId = UserId,
                            Redirectionurl = Global.WebsiteUrl() + "/Home/Index",
                            PublicationId = Ast.PublicationId,
                            Password = candi.PCode,
                            Category = !string.IsNullOrEmpty(candi.BatchName) ? candi.BatchName : Convert.ToString(BatchId),
                            TrainingId = Ast.TrainingId,
                            StartDate = Ast.StartDate,
                            EndDate = Ast.EndDate,
                            StartTime = Ast.StartTime,
                            EndTime = Ast.EndTime,
                        });
                    }
                }

                string apiUrl = Global.PreloreUrl() + "/Api/Value/PostRegisterCandidate";
                HttpResponseMessage responsePostMethod = new HttpResponseMessage();
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    responsePostMethod = await client.PostAsJsonAsync(apiUrl, candiReg);
                }
            }
            return Result;
        }

        public ActionResult DownloadAssessmentSummary(Int64 BatchId)
        {
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/rptAssessmentSumm1.rdlc";

            var UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);

            List<CourseBatchViewModel> corsebatch = tms.GetCourseBatches(userDetails.CorporateId, BatchId);

            List<TrainingScheduleView> SchedulesBatch = admin.GetTrainingSchedulesBatch(BatchId);

            DataTable DtAssessment = admin.GetAssessmentReport(BatchId);

            ReportParameter[] parms = new ReportParameter[1];
            parms[0] = new ReportParameter("BatchId", BatchId.ToString());
            rptViewer.LocalReport.SetParameters(parms);

            //ReportParameter[] parms1 = new ReportParameter[1];
            //parms1[0] = new ReportParameter("TrainingId", TrainingId);
            //rptViewer.LocalReport.SetParameters(parms1);

            ReportDataSource reportDataSource1 = new ReportDataSource();
            reportDataSource1.Name = "dtAssessments";
            reportDataSource1.Value = DtAssessment;

            ReportDataSource reportDataSource2 = new ReportDataSource();
            reportDataSource2.Name = "DtCourseBatches";
            reportDataSource2.Value = corsebatch;

            ReportDataSource reportDataSource3 = new ReportDataSource();
            reportDataSource3.Name = "DtTrainings";
            reportDataSource3.Value = SchedulesBatch;

            rptViewer.LocalReport.DataSources.Add(reportDataSource1);
            rptViewer.LocalReport.DataSources.Add(reportDataSource2);
            rptViewer.LocalReport.DataSources.Add(reportDataSource3);

            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=BatchAssessmentSummary.pdf");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            return View();
        }

        public ActionResult DownloadTrainingAssessmentSummary(Int64 BatchId, string TrainingId)
        {
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/TrainingAssessmentSumm.rdlc";

            var UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);

            List<CourseBatchViewModel> corsebatch = tms.GetCourseBatches(userDetails.CorporateId, BatchId);

            List<TrainingScheduleView> SchedulesBatch = admin.GetTrainingSchedulesBatch(BatchId)
                                                        .Where(t => t.TrainingId == TrainingId).ToList();

            DataTable DtAssessment = admin.GetAssessmentReport(BatchId, TrainingId);

            List<CandidateAttendanceView> Attendance = admin.GetAttendence(TrainingId);

            ReportDataSource reportDataSource1 = new ReportDataSource();
            reportDataSource1.Name = "DtAssessments";
            reportDataSource1.Value = DtAssessment;

            ReportDataSource reportDataSource2 = new ReportDataSource();
            reportDataSource2.Name = "DtCourseBatches";
            reportDataSource2.Value = corsebatch;

            ReportDataSource reportDataSource3 = new ReportDataSource();
            reportDataSource3.Name = "DtTrainings";
            reportDataSource3.Value = SchedulesBatch;

            ReportDataSource reportDataSource4 = new ReportDataSource();
            reportDataSource4.Name = "DtAttendance";
            reportDataSource4.Value = Attendance;

            ReportParameter[] parms = new ReportParameter[3];
            parms[0] = new ReportParameter("BatchId", BatchId.ToString());
            parms[1] = new ReportParameter("TrainingId", TrainingId);
            parms[2] = new ReportParameter("CorporateId", userDetails.CorporateId);
            rptViewer.LocalReport.SetParameters(parms);

            rptViewer.LocalReport.DataSources.Add(reportDataSource1);
            rptViewer.LocalReport.DataSources.Add(reportDataSource2);
            rptViewer.LocalReport.DataSources.Add(reportDataSource3);
            rptViewer.LocalReport.DataSources.Add(reportDataSource4);


            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=TrainingAssessmentSummary.pdf");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            return View();
        }

        public ActionResult DownloadTrainingAssessmentSummaryExcel(Int64 BatchId, string TrainingId)
        {
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/TrainingAssessmentSumm.rdlc";

            var UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);

            List<CourseBatchViewModel> corsebatch = tms.GetCourseBatches(userDetails.CorporateId, BatchId);

            List<TrainingScheduleView> SchedulesBatch = admin.GetTrainingSchedulesBatch(BatchId)
                                                        .Where(t => t.TrainingId == TrainingId).ToList();

            DataTable DtAssessment = admin.GetAssessmentReport(BatchId, TrainingId);

            List<CandidateAttendanceView> Attendance = admin.GetAttendence(TrainingId);

            ReportDataSource reportDataSource1 = new ReportDataSource();
            reportDataSource1.Name = "DtAssessments";
            reportDataSource1.Value = DtAssessment;

            ReportDataSource reportDataSource2 = new ReportDataSource();
            reportDataSource2.Name = "DtCourseBatches";
            reportDataSource2.Value = corsebatch;

            ReportDataSource reportDataSource3 = new ReportDataSource();
            reportDataSource3.Name = "DtTrainings";
            reportDataSource3.Value = SchedulesBatch;

            ReportDataSource reportDataSource4 = new ReportDataSource();
            reportDataSource4.Name = "DtAttendance";
            reportDataSource4.Value = Attendance;

            ReportParameter[] parms = new ReportParameter[3];
            parms[0] = new ReportParameter("BatchId", BatchId.ToString());
            parms[1] = new ReportParameter("TrainingId", TrainingId);
            parms[2] = new ReportParameter("CorporateId", userDetails.CorporateId);
            rptViewer.LocalReport.SetParameters(parms);

            rptViewer.LocalReport.DataSources.Add(reportDataSource1);
            rptViewer.LocalReport.DataSources.Add(reportDataSource2);
            rptViewer.LocalReport.DataSources.Add(reportDataSource3);
            rptViewer.LocalReport.DataSources.Add(reportDataSource4);

            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            byte[] bytes = rptViewer.LocalReport.Render("Excel", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            //string adminrpt = "Adminreport";
            Response.AddHeader("content-disposition", "attachment; filename='" + "Training_Summary" + ".xls");
            Response.BinaryWrite(bytes);
            Response.Flush();
            return View();
        }

        public ActionResult DownloadOfflineTrainingAssessmentExcel(string TrainingId, Int64 AssessmentId = 0)
        {
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/OfflineAssessmentResult.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@AssessmentId", AssessmentId));
            cmd.Connection = thisConnection;

            string MyDataSource1 = "USP_GetAssessmentByAssessmentId";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Parameters.Add(new SqlParameter("@AssessmentId", AssessmentId));
            cmd2.Connection = thisConnection;

            ReportParameter[] parms = new ReportParameter[2];
            parms[0] = new ReportParameter("TrainingId", TrainingId);
            parms[1] = new ReportParameter("AssessmentId", AssessmentId.ToString());
            rptViewer.LocalReport.SetParameters(parms);

            rptViewer.LocalReport.DataSources.Add(reportDataSource);
            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            string extension = string.Empty;
            byte[] bytes = rptViewer.LocalReport.Render("Excel", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            //string adminrpt = "Adminreport";
            Response.AddHeader("content-disposition", "attachment; filename='" + "Training_Summary" + ".xls");
            Response.BinaryWrite(bytes);
            Response.Flush();
            return View();
        }

        public ActionResult DownloadOfflineTrainingAssessmentPDF(string TrainingId, Int64 AssessmentId = 0)
        {
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/OfflineAssessmentResult.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@AssessmentId", AssessmentId));
            cmd.Connection = thisConnection;

            string MyDataSource1 = "USP_GetAssessmentByAssessmentId";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Parameters.Add(new SqlParameter("@AssessmentId", AssessmentId));
            cmd2.Connection = thisConnection;

            ReportParameter[] parms = new ReportParameter[2];
            parms[0] = new ReportParameter("TrainingId", TrainingId);
            parms[1] = new ReportParameter("AssessmentId", AssessmentId.ToString());
            rptViewer.LocalReport.SetParameters(parms);

            rptViewer.LocalReport.DataSources.Add(reportDataSource);
            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            string extension = string.Empty;
            byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=OfflineAssessmentEvaluationReport.pdf");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            return View();
        }

        //method for dowanload assessment evaluation report
        [HttpGet]
        public ActionResult DownloadAssessmentEvaluation(string TrainingId = "")
        {

            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Views/Report/AssessmentEvaluationReport.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@TrainingId", TrainingId));
            cmd.Connection = thisConnection;

            string MyDataSource1 = "USP_GetAssessmentEvaluation";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Parameters.Add(new SqlParameter("@TrainingId", TrainingId));
            cmd2.Connection = thisConnection;
            string MyDataSource2 = "USP_GetTrainingAssessments";
            cmd2.CommandText = string.Format(MyDataSource2);
            cmd2.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN2 = new SqlDataAdapter(cmd2);
            System.Data.DataSet DataSet2 = new System.Data.DataSet();
            daN2.Fill(DataSet2);
            ReportDataSource reportDataSource2 = new ReportDataSource();
            reportDataSource2.Name = "DataSet2";
            reportDataSource2.Value = DataSet2.Tables[0];

            ReportParameter[] parms = new ReportParameter[1];
            parms[0] = new ReportParameter("TrainingId", TrainingId);
            rptViewer.LocalReport.SetParameters(parms);

            rptViewer.LocalReport.DataSources.Add(reportDataSource);
            rptViewer.LocalReport.DataSources.Add(reportDataSource2);
            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            string extension = string.Empty;
            byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=AssessmentEvaluationReport.pdf");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            return View();
        }

        [HttpGet]
        public ActionResult DownloadAssessmentEvaluationExcel(string TrainingId = "")
        {

            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Views/Report/AssessmentEvaluationReport.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@TrainingId", TrainingId));
            cmd.Connection = thisConnection;

            string MyDataSource1 = "USP_GetAssessmentEvaluation";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Parameters.Add(new SqlParameter("@TrainingId", TrainingId));
            cmd2.Connection = thisConnection;
            string MyDataSource2 = "USP_GetTrainingAssessments";
            cmd2.CommandText = string.Format(MyDataSource2);
            cmd2.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN2 = new SqlDataAdapter(cmd2);
            System.Data.DataSet DataSet2 = new System.Data.DataSet();
            daN2.Fill(DataSet2);
            ReportDataSource reportDataSource2 = new ReportDataSource();
            reportDataSource2.Name = "DataSet2";
            reportDataSource2.Value = DataSet2.Tables[0];

            ReportParameter[] parms = new ReportParameter[1];
            parms[0] = new ReportParameter("TrainingId", TrainingId);
            rptViewer.LocalReport.SetParameters(parms);

            rptViewer.LocalReport.DataSources.Add(reportDataSource);
            rptViewer.LocalReport.DataSources.Add(reportDataSource2);
            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            string extension = string.Empty;
            byte[] bytes = rptViewer.LocalReport.Render("Excel", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            //string adminrpt = "Adminreport";
            Response.AddHeader("content-disposition", "attachment; filename='" + "Assessment_Evaluation" + ".xls");
            Response.BinaryWrite(bytes);
            Response.Flush();
            return View();
        }

        public ActionResult DownloadAssessmentMarks(string TrainingId = "", string CandidateId = "")
        {
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Views/Report/AssessmentMarksheet.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@TrainingId", TrainingId));
            cmd.Parameters.Add(new SqlParameter("@UserId", CandidateId));
            cmd.Connection = thisConnection;
            string MyDataSource1 = "USP_GetCandidateAssesmentMarks";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];
            ReportParameter[] parms = new ReportParameter[2];
            parms[0] = new ReportParameter("UserId", CandidateId);
            parms[1] = new ReportParameter("TrainingId", TrainingId);
            rptViewer.LocalReport.SetParameters(parms);
            rptViewer.LocalReport.DataSources.Add(reportDataSource);
            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            string extension = string.Empty;
            byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=AssessmentMarks.pdf");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            return View();
        }
        //end
        //for export candidacte course certificate
        public ActionResult DownloadCandidateCertificate(string Id = "", string CourseCode = "")
        {
            var UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            if (userDetails.Email == "ip@nibf.in")
            {
                ReportViewer rptViewer = new ReportViewer();
                rptViewer.LocalReport.ReportPath = "Views/Report/IPPBCertificate.rdlc";
                string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection thisConnection = new SqlConnection(thisConnectionString);
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Add(new SqlParameter("@UserId", Id));
                cmd.Parameters.Add(new SqlParameter("@CourseCode", CourseCode));
                cmd.Connection = thisConnection;
                string MyDataSource1 = "USP_GetCandidateCertificate";
                cmd.CommandText = string.Format(MyDataSource1);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter daN = new SqlDataAdapter(cmd);
                System.Data.DataSet DataSet1 = new System.Data.DataSet();
                daN.Fill(DataSet1);
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1";
                reportDataSource.Value = DataSet1.Tables[0];
                ReportParameter[] parms = new ReportParameter[2];
                parms[0] = new ReportParameter("UserId", Id);
                parms[1] = new ReportParameter("CourseCode", CourseCode);
                rptViewer.LocalReport.SetParameters(parms);
                rptViewer.LocalReport.DataSources.Add(reportDataSource);
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.SizeToReportContent = true;
                rptViewer.ZoomMode = ZoomMode.PageWidth;
                rptViewer.Width = Unit.Percentage(99);
                rptViewer.Height = Unit.Pixel(1000);
                var reList = rptViewer.LocalReport.ListRenderingExtensions();
                string mimeType = string.Empty;
                string encoding = string.Empty;
                rptViewer.LocalReport.Refresh();
                string extension = string.Empty;
                byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition", "attachment; filename=Certificate.pdf");
                Response.BinaryWrite(bytes); // create the file
                Response.Flush();
                return View();
            }
            else
            {

                ReportViewer rptViewer = new ReportViewer();
                if (CourseCode == "CC201805040001")
                {
                    rptViewer.LocalReport.ReportPath = "Views/Report/EmployerCertificates.rdlc";
                }
                else
                {
                    rptViewer.LocalReport.ReportPath = "Views/Report/Certificate.rdlc";
                }
                string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection thisConnection = new SqlConnection(thisConnectionString);
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Add(new SqlParameter("@UserId", Id));
                cmd.Parameters.Add(new SqlParameter("@CourseCode", CourseCode));
                cmd.Connection = thisConnection;
                string MyDataSource1 = "USP_GetCandidateCertificate";
                cmd.CommandText = string.Format(MyDataSource1);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter daN = new SqlDataAdapter(cmd);
                System.Data.DataSet DataSet1 = new System.Data.DataSet();
                daN.Fill(DataSet1);
                string CName = DataSet1.Tables[0].Rows[0][1].ToString();
                CName = Regex.Replace(CName, @"\s", "");
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1";
                reportDataSource.Value = DataSet1.Tables[0];
                //DATASET2
                SqlCommand cmd2 = new SqlCommand();
                cmd2.Parameters.Add(new SqlParameter("@CorporateId", userDetails.CorporateId));
                cmd2.Connection = thisConnection;
                string MyDataSource2 = "USP_GetCompanyProfiles";
                cmd2.CommandText = string.Format(MyDataSource2);
                cmd2.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter daN2 = new SqlDataAdapter(cmd2);
                System.Data.DataSet DataSet2 = new System.Data.DataSet();
                daN2.Fill(DataSet2);
                ReportDataSource reportDataSource2 = new ReportDataSource();
                reportDataSource2.Name = "DataSet2";
                reportDataSource2.Value = DataSet2.Tables[0];

                ReportParameter[] parms = new ReportParameter[3];
                parms[0] = new ReportParameter("UserId", Id);
                parms[1] = new ReportParameter("CorporateId", userDetails.CorporateId);
                parms[2] = new ReportParameter("CourseCode", CourseCode);
                rptViewer.LocalReport.SetParameters(parms);
                rptViewer.LocalReport.DataSources.Add(reportDataSource);
                rptViewer.LocalReport.DataSources.Add(reportDataSource2);
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.SizeToReportContent = true;
                rptViewer.ZoomMode = ZoomMode.PageWidth;
                rptViewer.Width = Unit.Percentage(99);
                rptViewer.Height = Unit.Pixel(1000);
                var reList = rptViewer.LocalReport.ListRenderingExtensions();
                string mimeType = string.Empty;
                string encoding = string.Empty;
                rptViewer.LocalReport.Refresh();
                string extension = string.Empty;
                byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition", "attachment; filename=" + CName + ".pdf");
                Response.BinaryWrite(bytes); // create the file
                Response.Flush();
                return View();
            }

        }
        public ActionResult LoginToPrelore()
        {
            var UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            var CP = db.CorporateProfile.Where(c => c.CorporateId == userDetails.SubscriberId).FirstOrDefault();
            AJSolutions.Controllers.AccountController acc = new AJSolutions.Controllers.AccountController();
            if (!string.IsNullOrEmpty(CP.Pcode))
            {
                var Email = acc.Encrypt(userDetails.Email);
                var Pass = CP.Pcode;
                return Redirect(Global.PreloreUrl() + "Admin/Dashboard/UserSignIn?U=" + Url.Encode(Email +
                                        "&" + Pass));
            }
            return RedirectToAction("Index", "Dashboard", new { area = "CMS" });
        }

        [HttpGet]
        public ActionResult CandidateReport(string Id, string PublicationId, string CandidateId) /// Id is used for TrainingId
        {
            ReportViewer rptViewer = new ReportViewer();
            var SubscriberId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(SubscriberId);

            rptViewer.LocalReport.ReportPath = "Report/Result.rdlc";

            var Candidate = generic.GetUserDetail(CandidateId);
            List<ExamDetailView> examDetail = GetCandidateExamDetail(Id, CandidateId);
            var companyProfile = cms.GetCompanyProfile(userdetails.CorporateId);

            List<CandidatePublicationView> CandPubList = GetCandidatePublication(Id, CandidateId);

            List<CandidateResultView> CandidateResult = GetCandidateResultView(Id, PublicationId, CandidateId);

            var CandidateReport = GetCandidateReport(Id, PublicationId, CandidateId);
            List<PublicationView> PubList = GetPublication(PublicationId);
            List<CandidateAttemptCountsView> QuestionCountsList = GetQuestionAttemptCounts(Id, PublicationId, CandidateId);

            var companyProf = new List<CompanyProfile> { companyProfile };
            var CandidateList = new List<UserViewModel> { Candidate };

            double OpScore = 0;
            for (int i = 0; i < CandidateResult.Count(); i++)
            {
                OpScore = OpScore + CandidateResult[i].Score;
            }

            ReportDataSource reportDataSource1 = new ReportDataSource();
            reportDataSource1.Name = "CandidateProfile";
            reportDataSource1.Value = CandidateList;

            ReportDataSource reportDataSource2 = new ReportDataSource();
            reportDataSource2.Name = "CandidateReport";
            reportDataSource2.Value = CandidateReport;

            ReportDataSource reportDataSource3 = new ReportDataSource();
            reportDataSource3.Name = "CandidateResult";
            reportDataSource3.Value = CandidateResult;

            ReportDataSource reportDataSource4 = new ReportDataSource();
            reportDataSource4.Name = "DsCandidateEnroll";
            reportDataSource4.Value = CandPubList;

            ReportDataSource reportDataSource5 = new ReportDataSource();
            reportDataSource5.Name = "PublicationsView";
            reportDataSource5.Value = PubList;

            ReportDataSource reportDataSource6 = new ReportDataSource();
            reportDataSource6.Name = "QuestionCounts";
            reportDataSource6.Value = QuestionCountsList;

            ReportDataSource reportDataSource7 = new ReportDataSource();
            reportDataSource7.Name = "CompanyProfile";
            reportDataSource7.Value = companyProf;

            ReportDataSource reportDataSource8 = new ReportDataSource();
            reportDataSource8.Name = "ExamDetail";
            reportDataSource8.Value = examDetail;

            ReportParameter[] parms = new ReportParameter[1];
            parms[0] = new ReportParameter("OpScore", (Math.Round(OpScore, 0)).ToString());
            rptViewer.LocalReport.SetParameters(parms); //NotAttempted

            var NotAttempted = 0;
            if (QuestionCountsList.Count > 0)
            {
                NotAttempted = QuestionCountsList[1].Value;
            }

            ReportParameter[] parms1 = new ReportParameter[1];
            parms1[0] = new ReportParameter("NotAttempted", NotAttempted.ToString());
            rptViewer.LocalReport.SetParameters(parms1);

            rptViewer.LocalReport.DataSources.Add(reportDataSource1);
            rptViewer.LocalReport.DataSources.Add(reportDataSource2);
            rptViewer.LocalReport.DataSources.Add(reportDataSource3);
            rptViewer.LocalReport.DataSources.Add(reportDataSource4);
            rptViewer.LocalReport.DataSources.Add(reportDataSource5);
            rptViewer.LocalReport.DataSources.Add(reportDataSource6);
            rptViewer.LocalReport.DataSources.Add(reportDataSource7);
            rptViewer.LocalReport.DataSources.Add(reportDataSource8);

            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=Result.pdf");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            return View();

        }

        public ActionResult DownloadCandidateCredentials(Int64 BatchId = 0)
        {
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/CandidateCredentials.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@BatchId", BatchId));
            cmd.Connection = thisConnection;
            string MyDataSource1 = "USP_GetCandidateCredentials";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DsCandidateDetails = new System.Data.DataSet();
            daN.Fill(DsCandidateDetails);
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DsCandidateDetails";
            reportDataSource.Value = DsCandidateDetails.Tables[0];
            ReportParameter[] parms = new ReportParameter[1];
            parms[0] = new ReportParameter("BatchId", BatchId.ToString());
            rptViewer.LocalReport.SetParameters(parms);
            rptViewer.LocalReport.DataSources.Add(reportDataSource);
            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            string extension = string.Empty;
            byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=CandidateCredentials.pdf");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            return View();
        }

        public ActionResult DownloadCandidateCredentialsExcel(Int64 BatchId = 0)
        {
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/CandidateCredentials.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@BatchId", BatchId));
            cmd.Connection = thisConnection;
            string MyDataSource1 = "USP_GetCandidateCredentials";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DsCandidateDetails = new System.Data.DataSet();
            daN.Fill(DsCandidateDetails);
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DsCandidateDetails";
            reportDataSource.Value = DsCandidateDetails.Tables[0];
            ReportParameter[] parms = new ReportParameter[1];
            parms[0] = new ReportParameter("BatchId", BatchId.ToString());
            rptViewer.LocalReport.SetParameters(parms);
            rptViewer.LocalReport.DataSources.Add(reportDataSource);
            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            string extension = string.Empty;
            byte[] bytes = rptViewer.LocalReport.Render("Excel", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            //string adminrpt = "Adminreport";
            Response.AddHeader("content-disposition", "attachment; filename='" + "Candidate_Credentials" + ".xls");
            Response.BinaryWrite(bytes);
            Response.Flush();
            return View();


        }


        public ActionResult CandidateAssessmentSummaryReport(string PublicationId, Int64 BatchId, string TrainingId)
        {

            var CandidatesData = tms.GetCandidateCredentials(BatchId);
            var CandidateResult = GetCandidateResultSummary(PublicationId, BatchId);

            var CandidateFinalResult = (from cd in CandidatesData
                                        join cr in CandidateResult
                                        on cd.UserId equals cr.CandidateId
                                        select new
                                        {
                                            cd.UserId,
                                            cd.UserName,
                                            cd.RegistrationId,
                                            cd.Name,
                                            cr.AssessmentId,
                                            cr.PublicationId,
                                            cr.Title,
                                            cr.Status,
                                            cr.TotalQuestions,
                                            cr.TotalAttemptedQues,
                                            cr.NoOfCorrectAns,
                                            cr.Totalmark,
                                            cr.Percentage,
                                            cr.Obtained,
                                            cr.ExamDate
                                        }).ToList();


            var StatusCount = GetStatusWiseResponseCount(PublicationId, TrainingId);


            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/CandidateAssessmentResult.rdlc";


            ReportDataSource reportDataSource1 = new ReportDataSource();
            reportDataSource1.Name = "CandidateSummary";
            reportDataSource1.Value = CandidateFinalResult;

            ReportDataSource reportDataSource2 = new ReportDataSource();
            reportDataSource2.Name = "StatusCount";
            reportDataSource2.Value = StatusCount;



            rptViewer.LocalReport.DataSources.Add(reportDataSource1);
            rptViewer.LocalReport.DataSources.Add(reportDataSource2);


            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=CandidateAssessmentSummaryReport.pdf");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();

            return View();
        }

        public ActionResult CandidateAssessmentExcelReport(string PublicationId, Int64 BatchId, string TrainingId)
        {


            var CandidatesData = tms.GetCandidateCredentials(BatchId);
            var CandidateResult = GetCandidateResultSummary(PublicationId, BatchId);

            var CandidateFinalResult = (from cd in CandidatesData
                                        join cr in CandidateResult
                                        on cd.UserId equals cr.CandidateId
                                        select new
                                        {
                                            UserName = cd.UserName,
                                            RegistrationNo = cd.RegistrationId,
                                            Name = cd.Name,
                                            TotalQuestions = cr.TotalQuestions,
                                            Attempted = cr.TotalAttemptedQues,
                                            Score = cr.NoOfCorrectAns,
                                            Percentage = cr.Percentage,
                                            Status = cr.Status
                                        }).ToList();


            var StatusCount = GetStatusWiseResponseCount(PublicationId, TrainingId);
            ExportToExcel ExportToXls = new ExportToExcel();

            DataTable tblPubResult = new DataTable();
            tblPubResult = generic.LINQResultToDataTable(CandidateFinalResult);

            ExportToXls.ExporttoExcel(tblPubResult, "Publication Summary Report ");
            return View();
        }



        public List<ExamDetailView> GetCandidateExamDetail(string ID, string CandidateId)
        {
            List<ExamDetailView> candidateExam = new List<ExamDetailView>();
            using (WebClient webClient = new System.Net.WebClient())
            {
                try
                {
                    string URL = Global.PreloreUrl() + "/Api/Value/GetCandidateExamDetail?ID=" + ID + "&CandidateId=" + CandidateId;
                    var json = webClient.DownloadString(URL);
                    ViewBag.OriginalData = Convert.ToString(json);

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    ExamDetailView[] myData = js.Deserialize<ExamDetailView[]>(json);
                    candidateExam = myData.ToList();
                }
                catch
                {

                }
            }
            return candidateExam;
        }

        public List<CandidateResultView> GetCandidateResultView(string ID, string PublicationId, string CandidateId)
        {
            List<CandidateResultView> candidateResult = new List<CandidateResultView>();
            using (WebClient webClient = new System.Net.WebClient())
            {
                try
                {
                    string URL = Global.PreloreUrl() + "/Api/Value/GetCandidateResultView?ID=" + ID + "&PublicationId=" + PublicationId + "&CandidateId=" + CandidateId;
                    var json = webClient.DownloadString(URL);
                    ViewBag.OriginalData = Convert.ToString(json);

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    CandidateResultView[] myData = js.Deserialize<CandidateResultView[]>(json);
                    candidateResult = myData.ToList();
                }
                catch
                {

                }
            }
            return candidateResult;
        }

        public List<CandidateReportView> GetCandidateReport(string ID, string PublicationId, string CandidateId)
        {
            List<CandidateReportView> CandidateRpt = new List<CandidateReportView>();
            using (WebClient webClient = new System.Net.WebClient())
            {
                try
                {
                    string URL = Global.PreloreUrl() + "/Api/Value/GetCandidateReport?ID=" + ID + "&PublicationId=" + PublicationId + "&CandidateId=" + CandidateId;
                    var json = webClient.DownloadString(URL);
                    ViewBag.OriginalData = Convert.ToString(json);

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    CandidateReportView[] myData = js.Deserialize<CandidateReportView[]>(json);
                    CandidateRpt = myData.ToList();
                }
                catch
                {

                }
            }
            return CandidateRpt;
        }

        public List<PublicationView> GetPublication(string PublicationId)
        {
            List<PublicationView> publication = new List<PublicationView>();
            using (WebClient webClient = new System.Net.WebClient())
            {
                try
                {
                    string URL = Global.PreloreUrl() + "/Api/Value/GetPublication?PublicationId=" + PublicationId;
                    var json = webClient.DownloadString(URL);
                    ViewBag.OriginalData = Convert.ToString(json);

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    PublicationView[] myData = js.Deserialize<PublicationView[]>(json);
                    publication = myData.ToList();
                }
                catch
                {

                }
            }
            return publication;
        }

        public List<CandidateAttemptCountsView> GetQuestionAttemptCounts(string ID, string PublicationId, string CandidateId)
        {
            List<CandidateAttemptCountsView> QuestionAttempt = new List<CandidateAttemptCountsView>();
            using (WebClient webClient = new System.Net.WebClient())
            {
                try
                {
                    string URL = Global.PreloreUrl() + "/Api/Value/GetQuestionAttemptCounts?ID=" + ID + "&PublicationId=" + PublicationId + "&CandidateId=" + CandidateId;
                    var json = webClient.DownloadString(URL);
                    ViewBag.OriginalData = Convert.ToString(json);

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    CandidateAttemptCountsView[] myData = js.Deserialize<CandidateAttemptCountsView[]>(json);
                    QuestionAttempt = myData.ToList();
                }
                catch
                { }
            }
            return QuestionAttempt;
        }

        public List<CandidatePublicationView> GetCandidatePublication(string ID, string CandidateId)
        {
            List<CandidatePublicationView> CandidatePub = new List<CandidatePublicationView>();
            using (WebClient webClient = new System.Net.WebClient())
            {
                try
                {
                    string URL = Global.PreloreUrl() + "/Api/Value/GetCandidatePublication?ID=" + ID + "&CandidateId=" + CandidateId;
                    var json = webClient.DownloadString(URL);
                    ViewBag.OriginalData = Convert.ToString(json);

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    CandidatePublicationView[] myData = js.Deserialize<CandidatePublicationView[]>(json);
                    CandidatePub = myData.ToList();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            return CandidatePub;
        }

        #region "Assesment Report"

        /// <summary>
        /// All Prelore handshake done by vishnu
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="BatchId"></param>
        /// <returns></returns>
        public List<ResultsView> GetCandidateResultSummary(string PublicationId, long BatchId)
        {
            List<ResultsView> CandidateResult = new List<ResultsView>();
            using (WebClient webClient = new System.Net.WebClient())
            {
                try
                {
                    string URL = Global.PreloreUrl() + "/Api/Value/GetPublicationwiseCandidateResultSummaryDetail?PublicationId=" + PublicationId;
                    var json = webClient.DownloadString(URL);
                    ViewBag.OriginalData = Convert.ToString(json);

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    ResultsView[] myData = js.Deserialize<ResultsView[]>(json);
                    CandidateResult = myData.ToList();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                return CandidateResult;
            }

        }

        public List<ResponceCountByStatus> GetStatusWiseResponseCount(string PublicationId, string TrainingId)
        {
            List<ResponceCountByStatus> ResponseCount = new List<ResponceCountByStatus>();
            using (WebClient webClient = new System.Net.WebClient())
            {
                try
                {
                    string URL = Global.PreloreUrl() + "/Api/Value/GetResponseCountByStatus?PublicationId=" + PublicationId + "&TrainingId=" + TrainingId;
                    var json = webClient.DownloadString(URL);
                    ViewBag.OriginalData = Convert.ToString(json);

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    ResponceCountByStatus[] myData = js.Deserialize<ResponceCountByStatus[]>(json);
                    ResponseCount = myData.ToList();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                return ResponseCount;
            }

        }

        #endregion

        public ActionResult MyAssessments(string Id = "NA", Int64 BatchId = 0)
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewBag.CourseCode = Id;
            ViewBag.UserId = UserId;
            var Course = db.CourseMaster.Where(c => c.CourseCode == Id).FirstOrDefault();
            ViewBag.CourseName = Course.CourseName;
            List<TrainingAssessmentView> Assts = tms.GetCandidateAssessment(UserId, Id);
            List<TrainingAssessmentView> training = Assts;
            if (BatchId != 0)
            {
                training = Assts = Assts.Where(a => a.BatchId == BatchId).ToList();
            }
            ViewBag.BatchId = BatchId;

            ViewData["Training"] = training;

            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

            DateTime checkindate = indianTime.Date;
            TimeSpan CheckInTime = indianTime.TimeOfDay;

            ViewBag.CurrentDate = indianTime.Date;
            ViewBag.CurrentTime = indianTime.TimeOfDay;
            return View(Assts);
        }

        public ActionResult CandidateLoginToPrelore(string ID, string PublicationId)
        {
            var UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            var CP = db.UserProfile.Where(c => c.UserId == userDetails.UserId).FirstOrDefault();
            AJSolutions.Controllers.AccountController acc = new AJSolutions.Controllers.AccountController();
            if (!string.IsNullOrEmpty(CP.PCode))
            {
                var userId = acc.Encrypt(UserId);
                var Pass = acc.Encrypt(CP.PCode);
                var publicationId = acc.Encrypt(PublicationId);
                var id = acc.Encrypt(ID);
                return Redirect(Global.PreloreUrl() + "/Candidate/UserSignIn?U=" + Url.Encode(userId +
                                        "&" + Pass + "&" + publicationId + "&" + id));
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Candidate" });
        }

        private void PopulatePaging(object selectedValue = null)
        {
            var PageList = generic.GetPaging();
            ViewBag.PageSize = new SelectList(PageList, "PageSize", "PageSize", selectedValue);
        }

        [HttpPost]
        public ActionResult GetEvaluationPerBatchNTraining(string TrainingId)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel UserDetails = generic.GetUserDetail(UserId);
            var TrainingAssessments = tms.GetTrainingAssessments(TrainingId);
            var candidate = student.GetSubscriberWiseCandidateList(UserDetails.SubscriberId, TrainingAssessments.FirstOrDefault().BatchId).OrderBy(c => c.Name).ToList();
            if (candidate.Count() > 0)
            {
                foreach (var item in candidate)
                {
                    if (item.RegistrationId != "-")
                    {
                        item.Name = item.Name + '(' + item.RegistrationId + ')';
                    }
                    else
                    {
                        item.Name = item.Name;
                    }
                }
            }
            var Assessments = tms.GetAssessmentEvaluation(TrainingId).ToList();
            return Json(new { CandidateCount = candidate.Count, TrainingAssessments = TrainingAssessments, candidate = candidate, Assessments = Assessments }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetEngagementTypeCount(Int64 EngagementTypeId)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel UserDetails = generic.GetUserDetail(UserId);
            string leaveyear = db.CompanySetting.Where(c => c.CorporateId == UserDetails.SubscriberId).FirstOrDefault().CalendarYear;
            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            if (leaveyear == "Mar-Apr" && Month <= 3)
                Year = Year - 1;
            short SchemeId = userContext.EmpJoiningDetail.Where(e => e.UserId == UserDetails.UserId).FirstOrDefault().SchemeId;

            List<EmployeeLeaveSummariesViewModel> empLeaveRecords = ems.GetEmployeeLeaveSummary(UserDetails.SubscriberId, Year, SchemeId, UserDetails.DepartmentId, UserDetails.UserId);
            EmloyeeLeaves Leaves = new EmloyeeLeaves();

            empLeaveRecords = empLeaveRecords.Where(l => l.EngagementTypeId == EngagementTypeId).ToList();

            if (empLeaveRecords.Count > 0)
            {
                Leaves.OutstandingLeaves = empLeaveRecords.FirstOrDefault().LeaveLimit - empLeaveRecords.FirstOrDefault().EngagementCount;
                Leaves.TotalLeaves = empLeaveRecords.FirstOrDefault().LeaveLimit;
            }
            else
            {
                Leaves.OutstandingLeaves = 0;
                Leaves.TotalLeaves = 0;
            }
            //if (UserDetails.Role == "Employee")
            //{
            //    var EmpDetails = db.EmpJoiningDetail.Where(c => c.UserId == UserId).FirstOrDefault();
            //    if (EmpDetails != null)
            //    {
            //        var EmpScheme = EmpDetails.SchemeId;
            //        var CorporateEngagements = db.EngagementTypeMaster.Where(c => c.CorporateId == UserDetails.SubscriberId).ToList();
            //        var selectedengagement = CorporateEngagements.Where(c => c.EngagementTypeId == EngagementTypeId && c.SchemeId == EmpScheme).FirstOrDefault();
            //        int totalLeaves = selectedengagement.LeaveLimit;
            //        EmloyeeLeaves Leaves = new EmloyeeLeaves();
            //        Leaves.TotalLeaves = totalLeaves;
            //        //var remaining = db.TrainerPlannerSummary.Where(c => c.TrainerId == UserId && c.EngagementTypeId == EngagementTypeId && c.SchemeId == EmpScheme).FirstOrDefault();
            //        var remainingleaves = tms.GetTrainerLeaveCalculation(UserId).ToList();
            //        var remaining = remainingleaves.Where(c => c.SchemeId == EmpScheme && c.EngagementTypeId == EngagementTypeId).FirstOrDefault();
            //        if (remaining == null)
            //        {
            //            Leaves.OutstandingLeaves = totalLeaves;
            //            return Json(Leaves, JsonRequestBehavior.AllowGet);
            //        }
            //        else
            //        {
            //            Leaves.OutstandingLeaves = (totalLeaves - remaining.TotalDays);
            //            return Json(Leaves, JsonRequestBehavior.AllowGet);
            //        }

            //    }
            //    else
            //    {
            //        return Json(0, JsonRequestBehavior.AllowGet);
            //    }
            //}
            //else
            //{
            //    return Json("NA", JsonRequestBehavior.AllowGet);
            //}
            return Json(Leaves, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadScoreTemplate(string Id, Int64 Assessment = 0)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel UserDetails = generic.GetUserDetail(UserId);
            ViewData["userprofile"] = UserDetails;
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=BulkUpload.csv");
            Response.Charset = "";
            Response.ContentType = "application/text";
            StringBuilder sb = new StringBuilder();
            if (UserDetails.Role == "Employee")
            {
                var TrainingAssessmentsrec = tms.GetTrainingAssessments(Id).Where(c => c.AssessmentId == Assessment);
                var candidate = student.GetSubscriberWiseCandidateList(UserDetails.SubscriberId, TrainingAssessmentsrec.FirstOrDefault().BatchId).OrderBy(c => c.Name).ToList();
                var Assessments = tms.GetAssessmentEvaluation(Id).Where(c => c.AssessmentId == Assessment).ToList();
                sb.Append("CandidateName" + ',');
                sb.Append("CandidateId" + ',');
                sb.Append(TrainingAssessmentsrec.FirstOrDefault().Assessment + (TrainingAssessmentsrec.FirstOrDefault().Weightage + "%") + ',');
                sb.Length--;
                //append new line
                sb.Append("\r\n");
                foreach (var itemRow in candidate)
                {
                    sb.Append(itemRow.Name + ',');
                    sb.Append(itemRow.UserName + ',');
                    if (Assessments.Where(c => c.UserId == itemRow.UserId && c.Percentage > 0).Count() > 0)
                    {
                        var assessmentmarks = Assessments.Where(c => c.UserId == itemRow.UserId).FirstOrDefault().Percentage;
                        sb.Append(assessmentmarks.ToString() + ',');
                    }
                    else
                    {
                        sb.Append("" + ',');
                    }
                    sb.Length--;
                    sb.Append("\r\n");
                }
            }
            else
            {
                var TrainingAssessment = tms.GetTrainingAssessments(Id);
                var candidate = student.GetSubscriberWiseCandidateList(UserDetails.SubscriberId, TrainingAssessment.FirstOrDefault().BatchId).OrderBy(c => c.Name).ToList();
                var Assessments = tms.GetAssessmentEvaluation(Id).ToList();
                sb.Append("CandidateName" + ',');
                sb.Append("CandidateId" + ',');
                foreach (var itemcol in TrainingAssessment)
                {
                    sb.Append(itemcol.Assessment + (itemcol.Weightage + "%") + ',');
                }
                sb.Length--;
                //append new line
                sb.Append("\r\n");
                foreach (var itemRow in candidate)
                {
                    sb.Append(itemRow.Name + ',');
                    sb.Append(itemRow.UserName + ',');
                    foreach (var itemcol in TrainingAssessment)
                    {
                        if (Assessments.Where(c => c.UserId == itemRow.UserId && c.AssessmentId == itemcol.AssessmentId && c.Percentage > 0).Count() > 0)
                        {
                            var assessmentmarks = Assessments.Where(c => c.UserId == itemRow.UserId && c.AssessmentId == itemcol.AssessmentId).FirstOrDefault().Percentage;
                            sb.Append(assessmentmarks.ToString() + ',');
                        }
                        else
                        {
                            sb.Append("" + ',');
                        }
                    }
                    sb.Append("" + ',');
                    sb.Length--;
                    sb.Append("\r\n");
                }
            }
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
            return View();
        }

        [HttpGet]
        public ActionResult ScoreBulkUpload(string Id, Int64 Assessment = 0)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel UserDetails = generic.GetUserDetail(UserId);
            ViewData["userprofile"] = UserDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            var TrainingAssessments = tms.GetTrainingAssessments(Id);
            ViewBag.TrainingId = Id;
            ViewBag.AssessId = Assessment;
            return View(TrainingAssessments);
        }

        [HttpPost]
        public ActionResult ScoreBulkUpload(string Id, HttpPostedFileBase FileUpload, Int64 Assessment = 0)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            if (FileUpload != null)
            {
                if (FileUpload.ContentLength > 0)
                {
                    try
                    {
                        string result = ReadFileSaveInDB(FileUpload, Id, UserId, Assessment);
                        ViewBag.result = result;
                        ViewBag.SuccessCount = SuccessCount;
                        ViewBag.FailureCount = FailureCount;
                        ViewBag.Download = "Yes";
                        string filePath = Server.MapPath(Url.Content("~/Content/Result.csv"));
                        AddToCSV(dt, filePath);
                    }
                    catch (Exception ex)
                    {
                        //Catch errors
                        ViewData["Feedback"] = ex.Message;
                    }
                }
                else
                {
                    ViewData["Feedback"] = "Please select a file";
                }
            }
            ViewData["userprofile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            var TrainingAssessments = tms.GetTrainingAssessments(Id);
            ViewBag.TrainingId = Id;
            ViewBag.AssessId = Assessment;
            return View("ScoreBulkUpload", ViewData["Feedback"]);
        }

        public string ReadFileSaveInDB(HttpPostedFileBase myFile, string Id, string UserId, Int64 Assessment = 0)
        {
            UserViewModel UserDetails = generic.GetUserDetail(UserId);
            StreamReader reader = new StreamReader(myFile.InputStream, Encoding.GetEncoding(1252));
            int i = 0;
            dt.Columns.Add(new DataColumn("CandidateName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("CandidateId", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Assessment", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Marks", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Error", Type.GetType("System.String")));

            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                for (int x = 0; x < values.Length; x++)
                {
                    values[x] = values[x].TrimStart(' ', '"');
                    values[x] = values[x].TrimEnd('"');
                }
                if (i > 0)
                {
                    int j = 2;
                    if (UserDetails.Role == "Employee")
                    {
                        var TrainingAssessmentsrec = tms.GetTrainingAssessments(Id).Where(c => c.AssessmentId == Assessment);
                        foreach (var itemMarks in TrainingAssessmentsrec)
                        {
                            var result = AddCandiateToDB(values[0], values[1], values[j], Id, UserId, itemMarks.AssessmentId);
                        }
                    }
                    else
                    {
                        var TrainingAssessmentsrec = tms.GetTrainingAssessments(Id);
                        foreach (var itemMarks in TrainingAssessmentsrec)
                        {
                            var result = AddCandiateToDB(values[0], values[1], values[j], Id, UserId, itemMarks.AssessmentId);
                            j++;
                        }
                    }

                }
                i++;
            }
            return "Success";
        }

        public string AddCandiateToDB(string Name, string UserName, string Marks, string Id, string UserId, Int64 AssessmentId = 0)
        {
            var assesment = tms.GetTrainingAssessments(Id).Where(x => x.AssessmentId == AssessmentId).FirstOrDefault().Assessment;
            try
            {
                UserViewModel UserDetails = generic.GetUserDetail(UserId);
                var TrainingAssessment = tms.GetTrainingAssessments(Id);
                var candidate = student.GetSubscriberWiseCandidateList(UserDetails.SubscriberId, TrainingAssessment.FirstOrDefault().BatchId).OrderBy(c => c.Name).ToList();
                var CandidateUserId = candidate.Where(c => c.UserName == UserName).FirstOrDefault().UserId;
                bool res = false;
                if (Marks == "")
                {
                    Marks = "0";
                }
                float value = Convert.ToSingle(Marks);
                res = tms.AddEvaluationMarks(AssessmentId, CandidateUserId, Id, value, DateTime.Now, UserId);

                if (res)
                {
                    SuccessCount++;

                    Object[] data = new Object[5];
                    data[0] = Name;
                    data[1] = UserName;
                    data[2] = assesment;
                    data[3] = value;
                    data[4] = "Upload Suucessfully";
                    dt.Rows.Add(data);
                }
                else
                {
                    SuccessCount++;
                    Object[] data = new Object[5];
                    data[0] = Name;
                    data[1] = UserName;
                    data[2] = assesment;
                    data[3] = value;
                    data[4] = "Upload Failed";
                    dt.Rows.Add(data);
                }
            }
            catch
            {
                FailureCount++;
                Object[] data = new Object[5];
                data[0] = Name;
                data[1] = UserName;
                data[2] = assesment;
                data[3] = Marks;
                data[4] = "Upload Failed";
                dt.Rows.Add(data);
            }
            return "Result";
        }

        public void AddToCSV(DataTable dtDataTable, string filePath)
        {
            StreamWriter sw = new StreamWriter(filePath, false);
            //headers  
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        public ActionResult DownoladFile()
        {
            string path = VirtualPathUtility.ToAbsolute("~/Content/Result.csv");
            return File(path, "text/csv", "Result.csv");
        }

        [HttpGet]
        public ActionResult CourseFeeSetting(string UserName, string CourseCode, string BatchName, Int16 InstallmentId = 1, string savestatus = "NA")
        {
            string UserId = User.Identity.GetUserId();
            CandidateViewModel candidate = new CandidateViewModel();
            UserViewModel UserDetails = generic.GetUserDetail(UserId);
            ViewData["userprofile"] = UserDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewBag.Status = savestatus;
            string userstatus = "";
            if (!string.IsNullOrEmpty(UserName))
            {
                using (WebClient webClient = new System.Net.WebClient())
                {
                    //var url = "http://jedev.azurewebsites.net/api/CorporateApi/GetJobSeekerProfileByJEID?JEID=" + UserName;
                    var url = Global.JobEnablerUrl() + "api/CorporateApi/GetJobSeekerProfileByJEID?JEID=" + UserName;
                    var myString = webClient.DownloadString(url);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    JSBasicDetailViewModel myData = js.Deserialize<JSBasicDetailViewModel>(myString);
                    myData = js.Deserialize<JSBasicDetailViewModel>(myString);
                    if (myData != null)
                    {
                        ViewBag.BatchId = BatchName;
                        ViewBag.CourseCode = CourseCode;
                        var Index = myData.PhoneNumber.IndexOf("-");
                        string ext = myData.PhoneNumber.Substring(Index + 1, myData.PhoneNumber.Length - (Index + 1));

                        var loginPhoneNumber = admin.GetLoginDetails(ext);
                        var loginEmail = admin.GetLoginDetails(myData.Email);
                        if (loginPhoneNumber == null && loginEmail == null)
                        {
                            candidate.PhoneNumber = ext;
                            candidate.Name = myData.FirstName + " " + myData.LastName;
                            candidate.Email = myData.Email;
                            candidate.RegistrationId = myData.JEId;
                            ViewBag.ReferenceId = myData.ReferenceId;
                            ViewBag.UserName = UserName;
                            PopulateCourse(UserDetails.SubscriberId);
                            PopulateBatchByCourse();
                            PopulatePaymentModeType();
                            if (CourseCode != null)
                            {
                                candidate.CourseCode = CourseCode;
                                Int64 BatchId = Convert.ToInt64(BatchName);
                                candidate.BatchId = BatchId;
                                PopulateBatchByCourse(UserDetails.SubscriberId, CourseCode, BatchName);
                                var details = student.GetInstallmentDetails(myData.Id, BatchId);
                                if (details != null)
                                {
                                    ViewBag.InstallmentId = details.InstallmentId;
                                }
                                ViewData["InstallmentDetail"] = details;
                                if (details != null)
                                {
                                    PopulateInstallments(details.InstallmentId);
                                    PopulatePaymentModeType();
                                }
                                else
                                {
                                    PopulateInstallments();
                                }
                                var course = tms.GetCourseCompleteDetails(UserDetails.SubscriberId).Where(x => x.CourseCode == CourseCode).FirstOrDefault();
                                candidate.CourseFee = course.CourseFee;
                                ViewData["CourseDetail"] = course;
                            }
                            return View(candidate);
                        }
                        else
                        {
                            userstatus = "Email already Exists";
                            PopulateCourse(UserDetails.SubscriberId);
                            PopulateBatchByCourse();
                            PopulatePaymentModeType();
                            return Json(userstatus, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        userstatus = "Unsuccess";
                        PopulateCourse(UserDetails.SubscriberId);
                        PopulateBatchByCourse();
                        PopulatePaymentModeType();
                        return Json(userstatus, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                PopulateCourse(UserDetails.SubscriberId);
                PopulateBatchByCourse();
                PopulatePaymentModeType();
                return View(candidate);
            }

        }

        [HttpPost]
        public async Task<ActionResult> CourseFeeSetting(string Name, string Email, string PhoneNumber,
                                                        string RegistrationId, string ReferenceId, string UserName,
                                                        string CourseCode, string BatchName, double TotalAmt,
                                                        float CourseFee, DateTime? PaymentDate,
                                                        string BankName, string ReferenceNumber, string Remarks,
                                                        string InstallmentAmount, string InstallmentNumber,
                                                        bool Accommodation = false, bool Transport = false, bool Others = false,
                                                        bool InstallmentInterest = false, bool Discount = false,
                                                        short PaymentModeId = 1, Int16 Installments = 1)
        {
            string Status = "";
            string UId = null;
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            string SubscriberId = userDetails.SubscriberId;
            var subscriberDetail = cms.GetCorporateProfile(SubscriberId).FirstOrDefault();
            double TotalFeePaid = 0.0;
            if (String.IsNullOrEmpty(UId))
            {
                string userName = admin.GenerateUserName();
                var user = new ApplicationUser { UserName = userName, Email = Email, PhoneNumber = PhoneNumber, EmailConfirmed = true };
                var result = await UserManager.CreateAsync(user, "changeme");
                if (result.Succeeded)
                {
                    string ModuleAccess = "SMS";
                    string RoleId = "Candidate";
                    string Department = "CAN";
                    var status = UserManager.AddToRole(user.Id, RoleId);
                    Int64 BatchId = Convert.ToInt64(BatchName);
                    if (status.Succeeded)
                    {
                        string UpdatedBy = User.Identity.GetUserId();
                        Status = "Succeeded";
                        bool registration = false;
                        registration = admin.UserRegistration(user.Id, Name, DateTime.UtcNow, ModuleAccess,
                                                                Department, RoleId, SubscriberId, false, UpdatedBy,
                                                                DateTime.UtcNow, UpdatedBy, RegistrationId, "", null,
                                                                null, null, null, null, userDetails.CorporateId,
                                                                null, "", "", "Student", ReferenceId);

                        var res = cms.UpdatePassword(user.Id, "changeme");

                        string callbackUrl = await SendEmailConfirmationTokenAsync(subscriberDetail.Name, user.Id, "Account activation", userName, PhoneNumber, Name);
                        if (registration)
                        {
                            if (user.Id != null && BatchId != 0)
                            {
                                admin.AddCandidateCourse(user.Id, BatchId, 1);
                            }
                            bool resultInstallment = student.AddInstallments(user.Id, Convert.ToInt16(InstallmentNumber), CourseCode, BatchId,
                                                                             DateTime.UtcNow, UpdatedBy, TotalAmt, Accommodation, Transport,
                                                                             Others, InstallmentInterest, Discount);

                            if (resultInstallment)
                            {
                                string strReturnUrl = Global.WebsiteUrl() + "Candidate/Candidate/PaymentConfirmation";
                                string transactionId = student.GetTransactionId(PaymentModeId);
                                double RemainingAmount = 0.0;
                                double conveyancefee = 0.0;
                                var details = student.GetCandidateWiseCourseDetail(user.Id, true, BatchId).FirstOrDefault();
                                float installment = Convert.ToSingle(InstallmentAmount);
                                if (TotalFeePaid == 0.0)
                                {
                                    RemainingAmount = (details.TotalFeeAmount - installment);
                                }
                                else
                                {
                                    RemainingAmount = (details.TotalFeeAmount - (TotalFeePaid + installment));
                                }
                                if (db.PaymentModeMaster.Find(PaymentModeId).PaymentMode.ToUpper() == "ONLINE")
                                {
                                    conveyancefee = (Convert.ToDouble(installment) * 3) / 100;
                                    var newconveyancefee = Convert.ToDouble(installment) + conveyancefee;
                                    string CandidateConeyanceFee = Convert.ToString(newconveyancefee);
                                    string OnlineResult = generic.PaymentRequest(transactionId, Name, CandidateConeyanceFee, Email, PhoneNumber, Remarks, strReturnUrl);

                                    if (!OnlineResult.ToUpper().StartsWith("ERROR"))
                                    {

                                        bool addresult = student.AddCandidateFeeDetails(transactionId, user.Id,
                                                        Convert.ToSingle(InstallmentAmount), CourseCode, BatchId,
                                                        DateTime.UtcNow, PaymentDate, PaymentModeId, BankName,
                                                         ReferenceNumber, "Initiate", null, Remarks, null, Installments,
                                                         Convert.ToInt16(InstallmentNumber), Convert.ToSingle(RemainingAmount), null, conveyancefee);
                                        if (addresult)
                                            return Json(OnlineResult, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        Status = "UnSuccess";
                                        return Json(Status, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    bool cashresults = student.AddCandidateFeeDetails(transactionId, user.Id, Convert.ToSingle(InstallmentAmount),
                                                       CourseCode, BatchId, DateTime.UtcNow, PaymentDate, PaymentModeId,
                                                       BankName, ReferenceNumber, "Approved", null, Remarks, null,
                                                       Convert.ToInt16(InstallmentNumber), Installments,
                                                       Convert.ToSingle(RemainingAmount), userDetails.UserId, conveyancefee);
                                    if (cashresults)
                                    {
                                        Status = "Succeeded";
                                        return Json(transactionId, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        Status = "UnSuccess";
                                        return Json(Status, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            else
                            {
                                Status = "UnSuccess";
                                return Json(Status, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }

            return RedirectToAction("CourseFeeSetting", "TMS");
        }

        private void PopulateInstallments(object selectedValue = null)
        {
            var InstallmentList = db.InstallmentMaster.ToList();
            SelectList Installments = new SelectList(InstallmentList, "InstallmentId", "Installment", selectedValue);
            ViewBag.Installments = Installments;
        }

        private void PopulatePaymentModeType(object selectedPaymentModeType = null)
        {
            EMSManager ems = new EMSManager();
            var query = ems.PaymentModeTypeList();
            SelectList PaymentModeId = new SelectList(query, "PaymentModeId", "PaymentMode", selectedPaymentModeType);
            ViewBag.PaymentModeId = PaymentModeId;
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
            await UserManager.SendEmailAsync(userID, subject, msgBody);

            return callbackUrl;
        }

        private async Task<string> SendTrainerEmail(string SubjectLine, string TrainingId, string SubScriberName, string userID, string subject, string phoneNumber, string Name = "User")
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { area = "", userId = userID, code = code }, protocol: Request.Url.Scheme);
            var msgBody = "Hello " + Name + ", <br/> <br/>" +
                 "A Training " + SubjectLine + "(" + TrainingId + ")" + " has been assigned to you by " + SubScriberName + ";" +
            "<br/><br/>RECKONN";
            await UserManager.SendEmailAsync(userID, subject, msgBody);

            return callbackUrl;
        }

        private async Task<string> SendCandidateEmail(string SubjectLine, string TrainingId, string SubScriberName, string userID, string subject, string phoneNumber, string Name, string BatchName, DateTime FromDate, DateTime ToDate, DateTime FromTime, DateTime ToTime)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { area = "", userId = userID, code = code }, protocol: Request.Url.Scheme);
            var msgBody = "Hello " + Name + ", <br/> <br/>" +
                 "you are Assigned to Batch  " + BatchName + "From " + FromDate + " To " + ToDate + " Timings " + "From " + FromTime + " To " + ToTime + ";" +
            "<br/><br/>RECKONN";
            await UserManager.SendEmailAsync(userID, subject, msgBody);

            return callbackUrl;
        }

        private async Task<string> SendTrainerEmailforAssessment(string SubjectLine, string TrainingId, string SubScriberName, string userID, string subject, string phoneNumber, string Name, string AssessmentName, DateTime StartDate, DateTime StartTime, DateTime EndDate, DateTime EndTime)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { area = "", userId = userID, code = code }, protocol: Request.Url.Scheme);
            var msgBody = "Dear Sir/Madam <br/> A Assessment " + AssessmentName + " has been scheduled for Training" + SubScriberName + "(" + TrainingId + ")" + " From " + StartDate + "-" + StartTime.ToString("dd-MM-yyyy") + " To " + EndDate + "-" + EndTime.ToString("dd-MM-yyyy") +
            "<br/><br/>RECKONN";
            await UserManager.SendEmailAsync(userID, subject, msgBody);

            return callbackUrl;
        }

        private async Task<string> SendCandidateEmailforAssessment(string SubjectLine, string TrainingId, string SubScriberName, string userID, string subject, string phoneNumber, string Name, string BatchName, DateTime FromDate, DateTime ToDate, DateTime FromTime, DateTime ToTime, string AssessmentName, DateTime StartDate, DateTime StartTime, DateTime EndDate, DateTime EndTime)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { area = "", userId = userID, code = code }, protocol: Request.Url.Scheme);
            var msgBody = "Dear Sir/Madam <br/> A Assessment " + AssessmentName + " has been scheduled for Training" + SubScriberName + "(" + TrainingId + ")" + " From " + StartDate + "-" + StartTime.ToString("dd-MM-yyyy") + " To " + EndDate + "-" + EndTime.ToString("dd-MM-yyyy") +
            "<br/><br/>RECKONN";
            await UserManager.SendEmailAsync(userID, subject, msgBody);

            return callbackUrl;
        }


        public ActionResult DownloadCandidateReceipt(string TransactionId = "")
        {
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/Receipt.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@TransactionId", TransactionId));
            cmd.Connection = thisConnection;
            string MyDataSource1 = "USP_GetCandidateFeeReceipt";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];
            ReportParameter[] parms = new ReportParameter[1];
            parms[0] = new ReportParameter("TransactionId", TransactionId);

            rptViewer.LocalReport.SetParameters(parms);
            rptViewer.LocalReport.DataSources.Add(reportDataSource);
            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=CandidateFeeReceipt.pdf");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            return View();
        }

        public ActionResult DownloadCandidateRecordBatchWise(string CourseCode, Int64 BatchId = 0)
        {
            var UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            CandidateViewModel Details = student.GetSubscriberWiseCandidateList(userDetails.SubscriberId, BatchId).FirstOrDefault();
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Views/Report/CandidateRecordBatchWise.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@SubscriberId", userDetails.SubscriberId));
            cmd.Parameters.Add(new SqlParameter("@CourseCode", CourseCode));
            cmd.Parameters.Add(new SqlParameter("@BatchId", BatchId));
            cmd.Connection = thisConnection;
            string MyDataSource1 = "USP_GetDrumpSubsciberWiseCandidateList";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];

            ReportParameter[] parms = new ReportParameter[3];
            parms[0] = new ReportParameter("SubscriberId", userDetails.SubscriberId.ToString());
            parms[1] = new ReportParameter("CourseCode", CourseCode.ToString());
            parms[2] = new ReportParameter("BatchId", BatchId.ToString());
            rptViewer.LocalReport.SetParameters(parms);
            rptViewer.LocalReport.DataSources.Add(reportDataSource);
            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            string extension = string.Empty;
            byte[] bytes = rptViewer.LocalReport.Render("Excel", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            //string adminrpt = "Adminreport";
            Response.AddHeader("content-disposition", "attachment; filename='" + Details.BatchName + ".xls");
            Response.BinaryWrite(bytes);
            Response.Flush();
            return View();

        }

        private void PopulateMentorTrainer(string SubscriberId, object selectedValue = null)
        {
            EMSManager emsMgr = new EMSManager();
            var TrainerMentor = emsMgr.GetSubscriberWiseEmployeeList(SubscriberId).Where(e => e.ManagerLevel == true && e.DepartmentId == "FAC").ToList();// || e.DepartmentId == "ADI")
            ViewBag.TrainerMentor = new SelectList(TrainerMentor, "UserId", "Name", selectedValue);
        }

        [HttpPost]
        public ActionResult GetCity(string StateId)
        {
            int stateId;
            List<SelectListItem> CityId = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(StateId))
            {
                stateId = Convert.ToInt32(StateId);
                List<CityMaster> Cities = db.CityMaster.Where(x => x.StateId == stateId).ToList();
                Cities.ForEach(x =>
                {
                    CityId.Add(new SelectListItem { Text = x.City, Value = x.CityId.ToString() });
                });
            }
            return Json(CityId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Employee,Client")]
        public ActionResult MentorTrainings(string TId, string Id, string ClientId, string TrainerId, string Status, string StatusforUpdate, string CourseCode, string TrainingId, string CityId, string TrainerMentorId, int? page, string UserAction = "Add", Int64 BatchId = 0, int PageSize = 10)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["UserProfile"] = userDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;

            var EmpDetails = ems.GetEmployeeBasicDetails(userDetail.UserId).FirstOrDefault();
            ViewData["EmpDetails"] = EmpDetails;
            //If Client has team members with all rights
            if (userDetail.CorporateId != null && userDetail.CorporateId != userDetail.SubscriberId)
            {
                userDetail.UserId = userDetail.CorporateId;
            }
            var attach = admin.GetTrainingAttachments(TrainingId);
            ViewData["Attachment"] = attach;
            ViewData["FinalAttach"] = tms.GetTrainingFinalAttachments(TrainingId);
            //ViewBag.UserId = UserId;

            PopulateTrainer(userDetail.UserId);
            PopulateCourse(userDetail.SubscriberId);
            PopulateMentorTrainer(userDetail.SubscriberId);
            if (userDetail.DepartmentId == "CLI")
            {
                if (userDetail.SubscriberId != userDetail.CorporateId)
                {
                    PopulateCourseForMyTraining(userDetail.SubscriberId, userDetail.CorporateId);
                }
                else
                {
                    PopulateCourseForMyTraining(userDetail.SubscriberId, userDetail.UserId);
                }
            }
            else
            {
                PopulateCourseForMyTraining(userDetail.SubscriberId);
            }
            PopulateBatchByCourse(userDetail.SubscriberId, CourseCode);
            PopulateClient(userDetail.SubscriberId);
            PopulateCityForTraining();
            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            ViewBag.Trainer = TrainerId;
            ViewBag.Course = CourseCode;
            ViewBag.Batch = BatchId;
            ViewBag.TrainingStatus = Status;
            ViewBag.TrainingClientId = ClientId;
            ViewBag.TrainingCityId = CityId;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            int cityId = 0;
            if (!string.IsNullOrEmpty(CityId))
            {
                if (CityId != "undefined")
                {
                    cityId = Convert.ToInt32(CityId);
                }
            }
            //if (userDetail.DepartmentId == "CLI")
            //{
            //    if (userDetail.CorporateId != null && userDetail.CorporateId != userDetail.SubscriberId)
            //    {
            //        var myClientTraining = admin.GetTrainingSchedulesForClients(userDetail.CorporateId).Where(c => c.TrainerMentorId == UserId);
            //        //if filtered only on the basis of Status
            //        if (!string.IsNullOrEmpty(Status))
            //        {
            //            myClientTraining = myClientTraining.Where(c => c.Status == Status).ToList();
            //            ViewBag.Status = Status;
            //        }
            //        //if filtered only on the basis of CourseCode
            //        if (!string.IsNullOrEmpty(CourseCode))
            //        {
            //            myClientTraining = myClientTraining.Where(c => c.CourseCode == CourseCode).ToList();
            //            PopulateCourseForMyTraining(userDetail.SubscriberId, userDetail.CorporateId);
            //        }
            //        //if filtered only on the basis of BatchId
            //        if (BatchId > 0)
            //        {
            //            myClientTraining = myClientTraining.Where(c => c.BatchId == BatchId).ToList();
            //            PopulateBatchByCourse(userDetail.SubscriberId, CourseCode, BatchId);
            //        }
            //        //if filtered only on the basis of CityId
            //        if (cityId > 0)
            //        {
            //            myClientTraining = myClientTraining.Where(c => c.CityId == cityId).ToList();
            //            PopulateCityForTraining(cityId);
            //        }
            //        if (!string.IsNullOrEmpty(TrainerMentorId))
            //        {
            //            myClientTraining = myClientTraining.Where(c => c.TrainerMentorId == TrainerMentorId).ToList();
            //            PopulateMentorTrainer(userDetail.SubscriberId, TrainerMentorId);
            //        }

            //        ViewBag.TotalCount = myClientTraining.Count();
            //        return View(myClientTraining.OrderByDescending(c => c.CreatedOn).ToPagedList(pageNumber, pageSize));
            //    }
            //    else
            //    {
            //        var myClientTraining = admin.GetTrainingSchedulesForClients(userDetail.SubscriberId).Where(c => c.TrainerMentorId == UserId);
            //        //if filtered only on the basis of Status
            //        if (!string.IsNullOrEmpty(Status))
            //        {
            //            myClientTraining = myClientTraining.Where(c => c.Status == Status).ToList();
            //            ViewBag.Status = Status;
            //        }
            //        //if filtered only on the basis of CourseCode
            //        if (!string.IsNullOrEmpty(CourseCode))
            //        {
            //            myClientTraining = myClientTraining.Where(c => c.CourseCode == CourseCode).ToList();
            //            PopulateCourseForMyTraining(userDetail.SubscriberId, userDetail.UserId);
            //        }
            //        //if filtered only on the basis of BatchId
            //        if (BatchId > 0)
            //        {
            //            myClientTraining = myClientTraining.Where(c => c.BatchId == BatchId).ToList();
            //            PopulateBatchByCourse(userDetail.SubscriberId, CourseCode, BatchId);
            //        }
            //        //if filtered only on the basis of CityId
            //        if (cityId > 0)
            //        {
            //            myClientTraining = myClientTraining.Where(c => c.CityId == cityId).ToList();
            //            PopulateCityForTraining(cityId);
            //        }
            //        ViewBag.TotalCount = myClientTraining.Count();
            //        return View(myClientTraining.OrderByDescending(c => c.CreatedOn).ToPagedList(pageNumber, pageSize));
            //    }
            //}
            ////If User is Admin
            //else if (userDetail.UserId == userDetail.SubscriberId || userDetail.DepartmentId == "ADI" || userDetail.DepartmentId == "ACD" || userDetail.DepartmentId == "FIN")
            //{
            //    var myTraining = tms.GetTrainingSchedule(userDetail.SubscriberId, "NA").Where(c => c.TrainerMentorId == userDetail.SubscriberId).ToList();
            //    if (!string.IsNullOrEmpty(TrainerId))
            //    {
            //        myTraining = myTraining.Where(c => c.TrainerId == TrainerId).ToList();
            //        PopulateTrainer(userDetail.UserId, TrainerId);
            //    }
            //    if (!string.IsNullOrEmpty(Status))
            //    {
            //        myTraining = myTraining.Where(c => c.Status == Status).ToList();

            //    }
            //    if (!string.IsNullOrEmpty(ClientId))
            //    {
            //        myTraining = myTraining.Where(c => c.CorporateId == ClientId).ToList();
            //        PopulateClient(userDetail.SubscriberId, ClientId);
            //    }
            //    if (!string.IsNullOrEmpty(CourseCode))
            //    {
            //        myTraining = myTraining.Where(c => c.CourseCode == CourseCode).ToList();
            //        PopulateCourse(userDetail.SubscriberId, CourseCode);

            //    }
            //    if (BatchId > 0)
            //    {
            //        myTraining = myTraining.Where(c => c.BatchId == BatchId).ToList();
            //        PopulateBatchByCourse(userDetail.SubscriberId, CourseCode, BatchId);
            //    }
            //    if (cityId > 0)
            //    {
            //        myTraining = myTraining.Where(c => c.CityId == cityId).ToList();
            //        PopulateCityForTraining(cityId);
            //    }
            //    ViewBag.TotalCount = myTraining.Count();
            //    return View(myTraining.OrderByDescending(c => c.CreatedOn).ToPagedList(pageNumber, pageSize));
            //}
            //else
            //{
            List<TrainingScheduleView> myTraining = tms.GetTrainingSchedule(userDetail.SubscriberId, "NA").Where(c => c.TrainerMentorId == UserId).ToList();
            List<TrainingScheduleView> training = new List<TrainingScheduleView>();
            foreach (TrainingScheduleView other in myTraining)
            {
                if (other.OtherTrainerId != null)
                {
                    string[] othert = other.OtherTrainerId.Split(',');
                    foreach (var ot in othert)
                    {
                        if (ot == TrainerId)
                        {

                            training.Add(other);
                        }
                    }
                }
            }
            if ((!string.IsNullOrEmpty(Status)) || (!string.IsNullOrEmpty(CourseCode)) || BatchId > 0)//
            {
                if (!string.IsNullOrEmpty(Status))
                {
                    myTraining = myTraining.Where(c => c.Status == Status).ToList();
                    ViewBag.Status = Status;
                }

                if (!string.IsNullOrEmpty(CourseCode))
                {
                    myTraining = myTraining.Where(c => c.CourseCode == CourseCode).ToList();
                    PopulateCourse(userDetail.SubscriberId, CourseCode);
                }

                if (BatchId > 0)
                {
                    myTraining = myTraining.Where(c => c.BatchId == BatchId).ToList();
                    PopulateBatchByCourse(userDetail.SubscriberId, CourseCode, BatchId);
                }
            }
            var list = myTraining.Concat(training).ToList();
            ViewBag.TotalCount = list.Count();
            return View(list.OrderByDescending(c => c.TrainingId).ToPagedList(pageNumber, pageSize));
            //}
        }
        [HttpGet]
        public ActionResult MyCertificate(string Id)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["UserProfile"] = userDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var certificate = tms.GetCandidateCourseDetailsName(UserId, Id);
            ViewBag.Name = certificate.Name;
            ViewBag.CourseName = certificate.CourseName;
            ViewBag.CourseCode = Id;
            return View();
        }
    }
}