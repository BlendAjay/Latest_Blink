using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.DAL;
using AJSolutions.Areas.RMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AJSolutions.Models;
using PagedList;
using Microsoft.Reporting.WebForms;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using System.Globalization;

namespace AJSolutions.Areas.RMS.Controllers
{
    public class RMSController : Controller
    {
        Generic generic = new Generic();
        RMSManager rms = new RMSManager();
        EMSManager ems = new EMSManager();
        UserDBContext db = new UserDBContext();
        EMSManager emsMgr = new EMSManager();
        CMSManager cms = new CMSManager();
        AdminManager admin = new AdminManager();
        // GET: RMS/RMS

        //[Authorize(Roles = "Employee")]
        public ActionResult Index(string Id, bool status = true)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            PopulateFrequency();
            ViewData["BranchDetails"] = rms.GetTrainer(UserId).FirstOrDefault();
            var QuestionList = rms.GetQuestion(UserDetails.SubscriberId);
            return View(QuestionList);
        }

        [HttpPost]
        //[Authorize(Roles = "Employee")]
        public ActionResult Index(string[] QuestionId, string[] GapObserved, string[] SuggestiveMeasures, string Id, string FeedBackdate, Int64 TrainerAssignId = 0, Int64 FeedbackId = 0)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);

            DateTime FeedBackdt = DateTime.ParseExact(FeedBackdate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            for (int i = 0; i < QuestionId.Length; i++)
            {
                bool result = rms.AddFeedback(FeedbackId, TrainerAssignId, Convert.ToInt16(QuestionId[i]), GapObserved[i], SuggestiveMeasures[i], Id, FeedBackdt, DateTime.Now, UserDetails.SubscriberId);
            }
            return RedirectToAction("Index", "RMS", new { Area = "RMS" });
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult QuestionMasters(Int64 QuestionId = 0, bool status = false, string UserAction = "Add")
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            if (UserAction == "Delete" && QuestionId > 0)
            {
                rms.Deletequestion(QuestionId);
                PopulateFrequency();
                ViewBag.DeleteResult = "Deleted";
                return View();
            }

            var QuestionList = rms.GetQuestion(UserDetails.SubscriberId);
            ViewData["Questions"] = QuestionList;
            var Question = QuestionList.Where(i => i.QuestionId == QuestionId).FirstOrDefault();
            //if (UserAction == "Edit")
            //{
            //    PopulateFrequency(Question.Frequency);
            //}
            //else
            //{
            //    PopulateFrequency();
            //}
            return View(Question);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult QuestionMasters(QuestionMaster QuestionMaster)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;

            bool result = rms.AddQuestions(QuestionMaster.QuestionId, QuestionMaster.Question, QuestionMaster.Category, UserDetails.SubscriberId);

            return RedirectToAction("QuestionMasters", "RMS", new { Area = "RMS", status = result });

        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult Branch(string BranchCode, bool status = false, string UserAction = "Add")
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.Action = UserAction;
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            if (UserAction == "Delete" && BranchCode != null)
            {
                rms.DeleteBranch(BranchCode);
            }
            List<CorporateProfile> corporateProfiles = db.CorporateProfile.Where(p => p.SubscriberId == UserDetails.SubscriberId).ToList().FindAll(p => p.DepartmentId == "CLI" || (p.DepartmentId == "ADI"));
            var BranchList = rms.GetBranchDetails(UserDetails.SubscriberId);
            ViewData["Branch"] = BranchList;
            var BranchById = BranchList.Where(i => i.BranchCode == BranchCode).FirstOrDefault();

            ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => cp.Name), "CorporateId", "Name");

            return View(BranchById);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult Branch(BranchDetails BranchDetails)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;

            bool result = rms.AddBranch(BranchDetails.BranchCode, BranchDetails.BranchName, BranchDetails.BranchZone, BranchDetails.CorporateId, UserDetails.SubscriberId);

            return RedirectToAction("Branch", "RMS", new { Area = "RMS", status = result });
        }

        //[Authorize(Roles = "Admin,Client")]
        public ActionResult GetFeedBacks(string Name, string sortOrder, string TrainerId, string Frequency, int? page, int PageSize = 10)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            PopulateFrequency();
            PopulateTrainer();
            if (UserDetails.DepartmentId == "ADI")
            {
                var feedback = rms.GetFeedback(null, Frequency, null);
                ViewBag.CurrentSort = sortOrder;
                ViewBag.FeedBackdateSortParam = sortOrder == "FeedBackDate" ? "FeedBackDate_desc" : "FeedBackDate";
                ViewBag.NameSortParam = sortOrder == "Name" ? "Name_desc" : "Name";
                ViewBag.FrequencySortParam = sortOrder == "Frequency" ? "Frequency_desc" : "Frequency";
                ViewBag.SubmittedSortParam = sortOrder == "SubmittedOn" ? "SubmittedOn_desc" : "Submitted";

                ViewBag.Name = Name;
                ViewBag.Frequency = Frequency;
                ViewBag.Page = page;

                if (feedback.Count != 0)
                {
                    switch (sortOrder)
                    {
                        case "FeedBackDate":
                            feedback = feedback.OrderBy(c => c.FeedBackdate).ToList();
                            break;
                        case "FeedBackDate_desc":
                            feedback = feedback.OrderByDescending(c => c.FeedBackdate).ToList();
                            break;
                        case "Frequency":
                            feedback = feedback.OrderBy(c => c.Frequency).ToList();
                            break;
                        case "Frequency_desc":
                            feedback = feedback.OrderByDescending(c => c.Frequency).ToList();
                            break;
                        case "Name":
                            feedback = feedback.OrderBy(c => c.Name).ToList();
                            break;
                        case "Name_desc":
                            feedback = feedback.OrderByDescending(c => c.Name).ToList();
                            break;
                        case "SubmittedOn":
                            feedback = feedback.OrderBy(c => c.UpdatedOn).ToList();
                            break;
                        case "SubmittedOn_desc":
                            feedback = feedback.OrderByDescending(c => c.UpdatedOn).ToList();
                            break;
                        default:
                            feedback = feedback.OrderBy(c => c.Name).ToList();
                            break;
                    }
                }
                PopulatePaging(PageSize);
                ViewBag.Paging = PageSize;
                int pageSize = PageSize;
                int pageNumber = (page ?? 1);
                return View(feedback.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var feedback = rms.GetFeedback(null, Frequency, UserId);
                ViewBag.CurrentSort = sortOrder;
                ViewBag.FeedBackdateSortParam = sortOrder == "FeedBackDate" ? "FeedBackDate_desc" : "FeedBackDate";
                ViewBag.NameSortParam = sortOrder == "Name" ? "Name_desc" : "Name";
                ViewBag.FrequencySortParam = sortOrder == "Frequency" ? "Frequency_desc" : "Frequency";
                ViewBag.SubmittedSortParam = sortOrder == "SubmittedOn" ? "SubmittedOn_desc" : "Submitted";

                ViewBag.Name = Name;
                ViewBag.Frequency = Frequency;
                ViewBag.Page = page;

                if (feedback.Count != 0)
                {
                    switch (sortOrder)
                    {
                        case "FeedBackDate":
                            feedback = feedback.OrderBy(c => c.FeedBackdate).ToList();
                            break;
                        case "FeedBackDate_desc":
                            feedback = feedback.OrderByDescending(c => c.FeedBackdate).ToList();
                            break;
                        case "Frequency":
                            feedback = feedback.OrderBy(c => c.Frequency).ToList();
                            break;
                        case "Frequency_desc":
                            feedback = feedback.OrderByDescending(c => c.Frequency).ToList();
                            break;
                        case "Name":
                            feedback = feedback.OrderBy(c => c.Name).ToList();
                            break;
                        case "Name_desc":
                            feedback = feedback.OrderByDescending(c => c.Name).ToList();
                            break;
                        case "SubmittedOn":
                            feedback = feedback.OrderBy(c => c.UpdatedOn).ToList();
                            break;
                        case "SubmittedOn_desc":
                            feedback = feedback.OrderByDescending(c => c.UpdatedOn).ToList();
                            break;
                        default:
                            feedback = feedback.OrderBy(c => c.Name).ToList();
                            break;
                    }
                }
                PopulatePaging(PageSize);
                ViewBag.Paging = PageSize;
                int pageSize = PageSize;
                int pageNumber = (page ?? 1);
                return View(feedback.ToPagedList(pageNumber, pageSize));
            }


        }
        [HttpGet]
        //[Authorize(Roles = "Admin,Client")]
        public ActionResult GetFeedBackDetails(string TrainerId, string Frequency, Int64 TrainerAssignId = 0)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            PopulateFrequency();
            PopulateTrainer();

            ViewBag.Frequency = Frequency;
            ViewBag.TrainerAssignId = TrainerAssignId;
            var trainerfeedback = rms.GetFeedbackDetails(TrainerAssignId, Frequency).FirstOrDefault();
            ViewData["BranchDetails"] = trainerfeedback;
            if (trainerfeedback != null)
            {
                ViewBag.TrainerId = trainerfeedback.TrainerId;
                ViewBag.CorporateId = trainerfeedback.CorporateId;
            }
            var feedback = rms.GetFeedbackDetails(TrainerAssignId, Frequency);
            return View(feedback);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Client")]
        public ActionResult GetFeedBackDetails(string TrainerId, string Frequency, string BranchCode, string CorporateId, Int64 TrainerAssignId = 0)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;
            PopulateFrequency();
            PopulateTrainer();
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Views/Report/FeedbackByFaculty1.rdlc"; /// Server.MapPath("Invoice1.rdlc");            
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@TrainerAssignId", TrainerAssignId));
            cmd.Parameters.Add(new SqlParameter("@Frequency", Frequency));
            cmd.Connection = thisConnection;
            string MyDataSource1 = "USP_GetFeedBacks";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];
            SqlCommand cmd1 = new SqlCommand();
            //cmd1.Parameters.Add(new SqlParameter("@UserId", TrainerId));
            cmd1.Parameters.Add(new SqlParameter("@BranchCode", BranchCode));
            cmd1.Connection = thisConnection;
            string MyDataSource2 = "USP_GETBranchDetails";
            cmd1.CommandText = string.Format(MyDataSource2);
            cmd1.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN1 = new SqlDataAdapter(cmd1);
            System.Data.DataSet DataSet2 = new System.Data.DataSet();
            daN1.Fill(DataSet2);
            ReportDataSource reportDataSource1 = new ReportDataSource();
            reportDataSource1.Name = "DataSet2";
            reportDataSource1.Value = DataSet2.Tables[0];
            ReportParameter[] parms = new ReportParameter[4];
            parms[0] = new ReportParameter("TrainerId", TrainerId);
            parms[1] = new ReportParameter("Frequency", Frequency);
            parms[2] = new ReportParameter("CorporateId", CorporateId);
            parms[3] = new ReportParameter("BranchCode", BranchCode);
            rptViewer.LocalReport.SetParameters(parms);
            rptViewer.LocalReport.DataSources.Add(reportDataSource);
            rptViewer.LocalReport.DataSources.Add(reportDataSource1);
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
            Response.AddHeader("content-disposition", "attachment; filename=FeedBackbyTrainer.pdf");
            Response.BinaryWrite(bytes);
            Response.Flush();
            return View(rptViewer);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult AssignTrainer(Int64 TrainerAssignId = 0, bool status = false, string UserAction = "Add")
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.Result = "Failed";
            List<CorporateProfile> corporateProfiles = db.CorporateProfile.Where(p => p.SubscriberId == UserDetails.SubscriberId).ToList().FindAll(p => p.DepartmentId == "CLI" || (p.DepartmentId == "ADI"));
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            if (UserAction == "Delete" && TrainerAssignId > 0)
            {
                rms.DeleteTrainer(TrainerAssignId);
                //ViewBag.TrainerId = new SelectList(emsMgr.GetSubscriberWiseEmployeeList(UserDetails.SubscriberId).Where(e => e.DepartmentId == "ADI" || e.DepartmentId == "FAC" || e.DepartmentId == "VFA"), "UserId", "Name");
                PopulateEmployee(UserDetails.SubscriberId);
                PopulateBranch();
                return View();
            }
            PopulateEmployee(UserDetails.SubscriberId);
            PopulateBranch();
            var trainerList = rms.GetTrainer();
            ViewData["Trainer"] = trainerList;
            var trainer = trainerList.Where(i => i.TrainerAssignId == TrainerAssignId).FirstOrDefault();
            if (trainer != null)
            {
                PopulateBranch(trainer.BranchCode);
                PopulateEmployee(UserDetails.SubscriberId, trainer.TrainerId);
                if (trainer.DateOfJoining != null)
                    ViewBag.joiningdate = trainer.DateOfJoining.Value.ToString("dd-MM-yyyy");

                if (trainer.LeavingDate != null)
                    ViewBag.LeavingDate = trainer.LeavingDate.Value.ToString("dd-MM-yyyy");
            }
            return View(trainer);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult AssignTrainer(TrainerAssign TrainerAssign, string BranchCode, string TrainerId, string JoiningDate, string LeavingDate, Int64 TrainerAssignId = 0)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;
            if (!String.IsNullOrEmpty(JoiningDate))
            {
                TrainerAssign.DateOfJoining = DateTime.ParseExact(JoiningDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            if (!String.IsNullOrEmpty(LeavingDate))
            {
                TrainerAssign.LeavingDate = DateTime.ParseExact(LeavingDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            bool result = rms.AddTrainer(TrainerAssignId, BranchCode, TrainerId, TrainerAssign.DateOfJoining, TrainerAssign.LeavingDate);

            return RedirectToAction("AssignTrainer", "RMS", new { Area = "RMS", status = result });
        }


        private void PopulateFrequency(object selectedValue = null)
        {
            var Frequencylist = Global.GetFrequencyList();
            SelectList FrequencyStatus = new SelectList(Frequencylist, "FrequencyStatus", "FrequencyStatus", selectedValue);
            ViewBag.FrequencyStatus = FrequencyStatus;
        }

        private void PopulateTrainer(object selectedOrderType = null)
        {
            //var query = rms.GetEmployee();
            //SelectList TrainerId = new SelectList(query, "TrainerId", "EmployeeName", selectedOrderType);
            //ViewBag.TrainerId = TrainerId;
            var TrainerId = rms.GetEmployee().ToList();
            ViewBag.TrainerId = new SelectList(TrainerId, "TrainerId", "Name", selectedOrderType);
        }

        private void PopulateEmployee(string SubscriberId, object selectedValue = null)
        {
            EMSManager emsMgr = new EMSManager();
            var TrainerId = emsMgr.GetSubscriberWiseEmployeeList(SubscriberId).Where(e => e.DepartmentId == "ADI" || e.DepartmentId == "FAC" || e.DepartmentId == "VFA").ToList();
            ViewBag.TrainerId = new SelectList(TrainerId, "UserId", "Name", selectedValue);

        }


        private void PopulateBranch(object selectedOrderType = null)
        {
            var query = rms.GetBranchCode();
            SelectList BranchCode = new SelectList(query, "BranchCode", "BranchName", selectedOrderType);
            ViewBag.BranchCode = BranchCode;
        }

        private void PopulatePaging(object selectedValue = null)
        {
            var PageList = generic.GetPaging();
            ViewBag.PageSize = new SelectList(PageList, "PageSize", "PageSize", selectedValue);
        }
    }
}