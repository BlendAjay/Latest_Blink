using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AJSolutions.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.Models;
using System.Data.Entity;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;

namespace AJSolutions.Areas.CMS.Controllers
{
    public class TransactionsController : Controller
    {
        Student student = new Student();
        Generic generic = new Generic();
        AdminManager admin = new AdminManager();
        EMSManager ems = new EMSManager();
        UserDBContext db = new UserDBContext();
        CMSManager cms = new CMSManager();

        [HttpGet]
        // GET: CMS/Transactions
        //[Authorize(Roles = "Admin, Employee")]
        public ActionResult Candidate(string TransactionId, string Status)
        {

            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetail.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            if (TransactionId != null && !string.IsNullOrEmpty(Status))
            {
                FeeDetails feedetail = db.FeeDetails.Find(TransactionId);

                if (feedetail != null)
                {
                    if (Status == "Approve")
                    {
                        feedetail.Status = "Approved";
                    }
                    else
                    {
                        feedetail.Status = "Failed";
                    }
                    feedetail.ApprovedBy = User.Identity.GetUserId();
                    feedetail.ApprovalDate = DateTime.UtcNow;
                    db.Entry(feedetail).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Candidate", "Transactions", new { area = "CMS" });
                }
            }
            var detials = student.GetCandidatePaymentTransaction(userDetail.SubscriberId).Where(c => c.Status != "Failed");
            return View(detials);
        }

        //[HttpPost]
        ////[Authorize(Roles = "Admin")]
        //public ActionResult Candidate(string TransactionId)
        //{
        //    FeeDetails feedetail = db.FeeDetails.Find(TransactionId);

        //    if (feedetail != null)
        //    {
        //        feedetail.Status = "Approved";
        //        feedetail.ApprovedBy = User.Identity.GetUserId();
        //        feedetail.ApprovalDate = DateTime.UtcNow;
        //        db.Entry(feedetail).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("Candidate", "Transactions", new { area = "CMS" });
        //}

        [HttpGet]
        //[Authorize(Roles = "Admin, Employee")]
        public ActionResult CandidateFeeDetails(string CourseCode, Int64 BatchId = 0)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetail.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            PopulateCourse(userDetail.SubscriberId, CourseCode);
            ViewBag.Batch = 0;
            ViewBag.Course = null;
            PopulateBatchByCourse(userDetail.SubscriberId, CourseCode, BatchId);
            if (CourseCode != null && BatchId > 0)
            {
                var Details = student.GetSubscriberWiseCandidateList(userDetail.SubscriberId, BatchId).ToList();
                ViewBag.Course = CourseCode;
                ViewBag.Batch = BatchId;
                return View(Details);
            }
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin, Employee")]
        public ActionResult Payment(string CandidateId, string CourseCode, Int64 BatchId = 0, bool Initiated = false)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetail.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            PopulatePaymentModeType();
            var details = student.GetCandidateWiseCourseDetail(CandidateId, true, BatchId).FirstOrDefault();
            ViewData["BatchDetails"] = details;
            if (Initiated == true)
            {
                ViewBag.Initiated = "Initiated";
            }
            if (CandidateId != null)
            {
                if (details.TotalFeeAmount > 0)
                {
                    ViewBag.InstallmentAmount = (details.TotalFeeAmount / details.InstallmentId);
                }
                else
                {
                    ViewBag.InstallmentAmount = (details.TotalFees / details.InstallmentId);
                }
                ViewBag.CandidateId = CandidateId;
            }
            if (CandidateId != null && userDetail.Role != "Admin")
            {
                ViewBag.OnlineCharges = (Convert.ToSingle(ViewBag.InstallmentAmount) * 3 / 100);
            }
            var feedetails = student.GetCandidateFeeDetails(CandidateId, BatchId).Where(c => c.Status != "Failed").ToList();
            if (feedetails.Count > 0)
            {
                foreach (var item in feedetails)
                {
                    if (item.InstallmentNumber == 1 && (item.Status == "Approved" || item.Status == "Succeeded"))
                    {
                        ViewBag.InstallmentNumber = 2;
                    }
                    if (item.InstallmentNumber == 2 && (item.Status == "Approved" || item.Status == "Succeeded"))
                    {
                        ViewBag.InstallmentNumber = 3;
                    }
                    if (item.InstallmentNumber == 3 && (item.Status == "Approved" || item.Status == "Succeeded"))
                    {
                        ViewBag.InstallmentNumber = 4;
                    }
                    if (item.InstallmentNumber == 4 && (item.Status == "Approved" || item.Status == "Succeeded"))
                    {
                        ViewBag.InstallmentNumber = 5;
                    }
                }
            }
            else
            {
                ViewBag.InstallmentNumber = 1;
            }

            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin, Employee")]
        public ActionResult Payment(string CandidateId, Int64 BatchId, string CourseCode, double TotalFeeAmount, float CourseFee, DateTime? PaymentDate, string BankName, string ReferenceNumber, string Remarks, string InstallmentAmount, string InstallmentNumber, short PaymentModeId = 1, double TotalFeePaid = 0.0, float InstallmentAmountWithoutCharges = 0)
        {
            string Result = string.Empty;
            try
            {
                string strReturnUrl = Global.WebsiteUrl() + "Candidate/Candidate/PaymentConfirmation";
                var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
                var userPrimaryDetail = student.GetSubscriberWiseCandidateList(userDetail.SubscriberId,BatchId).Where(s => s.UserId == CandidateId).FirstOrDefault();
                string transactionId = student.GetTransactionId(PaymentModeId);
                var Details = db.InstallmentDetails.Where(c => c.UserId == CandidateId && c.CourseCode == CourseCode).FirstOrDefault();
                double conveyancefee = 0.0;
                double RemainingAmount = 0.0;
                float installment = Convert.ToSingle(InstallmentAmount);
                if (TotalFeePaid == 0.0)
                {
                    if (db.PaymentModeMaster.Find(PaymentModeId).PaymentMode.ToUpper() == "ONLINE")
                    {
                        RemainingAmount = (TotalFeeAmount - InstallmentAmountWithoutCharges);
                    }
                    else
                    {
                        RemainingAmount = (TotalFeeAmount - installment);
                    }
                }
                else
                {
                    if (db.PaymentModeMaster.Find(PaymentModeId).PaymentMode.ToUpper() == "ONLINE")
                    {
                        RemainingAmount = (TotalFeeAmount - (TotalFeePaid + InstallmentAmountWithoutCharges));
                    }
                    else
                    {
                        RemainingAmount = (TotalFeeAmount - (TotalFeePaid + installment));
                    }

                }
                if (db.PaymentModeMaster.Find(PaymentModeId).PaymentMode.ToUpper() == "ONLINE")
                {
                    conveyancefee = (Convert.ToDouble(installment) * 3) / 100;
                    var newconveyancefee = Convert.ToDouble(installment) + conveyancefee;
                    string CandidateConeyanceFee = Convert.ToString(newconveyancefee);
                    Result = generic.PaymentRequest(transactionId, userDetail.Name, CandidateConeyanceFee, userPrimaryDetail.Email, userPrimaryDetail.PhoneNumber, Remarks, strReturnUrl);

                    if (!Result.ToUpper().StartsWith("ERROR"))
                    {
                        bool result = student.AddCandidateFeeDetails(transactionId, CandidateId, installment, CourseCode, BatchId, DateTime.UtcNow, DateTime.Now, PaymentModeId, BankName, ReferenceNumber, "Initiate", null, Remarks, null, Convert.ToInt16(InstallmentNumber), Convert.ToInt16(InstallmentNumber), Convert.ToSingle(RemainingAmount), null, conveyancefee);
                        if (result)
                            return Redirect(Url.Content(Global.WebsiteUrl() + "Request.aspx?Id=" + Result));
                    }
                    return RedirectToAction("Payment", "Transactions", new { Area = "CMS", CourseCode = CourseCode, BatchId = BatchId, res = Result });
                }
                else
                {
                    bool cashresults = student.AddCandidateFeeDetails(transactionId, CandidateId, installment, CourseCode, BatchId, DateTime.UtcNow, PaymentDate, PaymentModeId, BankName, ReferenceNumber, "Approved", null, Remarks, null, Convert.ToInt16(InstallmentNumber), Convert.ToInt16(InstallmentNumber), Convert.ToSingle(RemainingAmount), userDetail.UserId, conveyancefee);
                    if (cashresults)
                        return RedirectToAction("PaymentConfirmation", "Candidate", new { Area = "Candidate", PMId = PaymentModeId, TId = transactionId });
                }
                //else
                //{
                //    bool results = student.AddCandidateFeeDetails(transactionId, CandidateId, Convert.ToSingle(InstallmentAmount), CourseCode, BatchId, DateTime.UtcNow, PaymentDate, PaymentModeId, BankName, ReferenceNumber, "Initiate", null, Remarks, null, Details.InstallmentId, Convert.ToInt16(InstallmentNumber), Convert.ToSingle(RemainingAmount), null);
                //    if (results)
                //        return RedirectToAction("Payment", "Transactions", new { Area = "CMS", CandidateId = CandidateId, CourseCode = CourseCode, BatchId = BatchId, Initiated = results });
                //}
            }
            catch (Exception ex)
            {
                Result = ex.ToString();
            }
            return RedirectToAction("Payment", "Transactions", new { Area = "CMS", BatchId = BatchId, res = Result });
        }

        //[HttpPost]
        ////[Authorize(Roles = "Admin")]
        //public ActionResult CandidateFeeDetails()
        //{

        //    return RedirectToAction("CandidateFeeDetails", "Transactions", new { area = "CMS" });
        //}

        [HttpGet]
        public ActionResult InstallmentDetails(string UserId, string CourseCode, Int64 BatchId = 0)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewBag.UserId = UserId;
            ViewBag.CourseCode = CourseCode;
            ViewBag.BatchId = Convert.ToString(BatchId);
            var courseDetail = admin.GetCourseMasterDetails(CourseCode);
            var detials = student.GetInstallmentDetails(UserId, BatchId);
            if (detials != null)
            {
                PopulateInstallments(detials.InstallmentId);
                return View(detials);
            }
            else
            {
                PopulateInstallments();
                return View(courseDetail);
            }
        }

        [HttpPost]
        public ActionResult InstallmentDetails(string UserId, string CourseCode, string BatchId, double TotalAmt, bool Accommodation = false, bool Transport = false, bool Others = false, bool InstallmentInterest = false, bool Discount = false, Int16 Installments = 1)
        {
            string UpdatedBy = User.Identity.GetUserId();
            bool result = student.AddInstallments(UserId, Installments, CourseCode, Convert.ToInt64(BatchId), DateTime.UtcNow, UpdatedBy, TotalAmt, Accommodation, Transport, Others, InstallmentInterest, Discount);
            return RedirectToAction("CandidateFeeDetails", "Transactions", new { area = "CMS", CourseCode = CourseCode, BatchId = Convert.ToInt64(BatchId) });
        }

        [HttpGet]
        public ActionResult PaymentDetails(string CandidateId, string CourseCode, Int64 BatchId = 0)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            var Details = student.GetSubscriberWiseCandidateList(userDetail.SubscriberId,BatchId).Where(b =>  b.UserId == CandidateId).FirstOrDefault();
            //var TotalFee = (Details.CourseFee / Details.InstallmentId);
            //ViewData["Details"] = TotalFee;
            var feedetails = student.GetCandidateFeeDetails(CandidateId, BatchId).ToList();
            if (feedetails.Count > 0)
            {
                feedetails = feedetails.Where(c => c.Status != "Failed").ToList();
                if (feedetails.Count > 0)
                {
                    var LastIntallments = feedetails.OrderByDescending(c => c.InstallmentNumber).FirstOrDefault().InstallmentNumber;
                    ViewBag.LastIntallment = LastIntallments;
                }
                else
                {
                    ViewBag.LastIntallment = 0;
                }
            }
            ViewData["feedetails"] = feedetails.Where(c => c.Status == "Approved" || c.Status == "Succeeded" || c.Status == "Initiate").ToList();
            return View(Details);
        }

        private void PopulateCourse(string SubscriberId, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetCourseDetails(SubscriberId);
            SelectList CourseCode = new SelectList(query, "CourseCode", "CourseName", selectedValue);
            ViewBag.CourseCode = CourseCode;
        }

        private void PopulateBatchByCourse(string SubscriberId = null, string CourseCode = null, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetBatches(SubscriberId, CourseCode).ToList();
            SelectList BatchId = new SelectList(query, "BatchId", "BatchName", selectedValue);
            ViewBag.BatchId = BatchId;
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

        public ActionResult DownloadCandidateFeeRecord(string CourseCode = "", Int64 BatchId = 0)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/CandidateFeesStatusReport.rdlc";

            var UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);

            List<AJSolutions.Areas.Candidate.Models.CandidateViewModel> DataSet1 = student.GetSubscriberWiseCandidateList(userDetail.SubscriberId, BatchId).ToList();

            ReportDataSource reportDataSource1 = new ReportDataSource();
            reportDataSource1.Name = "DataSet1";
            reportDataSource1.Value = DataSet1;

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
            Response.AddHeader("content-disposition", "attachment; filename=CandidateFeeRecord.pdf");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            return View();

            //ReportViewer rptViewer = new ReportViewer();
            //rptViewer.LocalReport.ReportPath = "Report/CandidateFeesStatusReport.rdlc";
            //string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            //SqlCommand cmd = new SqlCommand();
            //cmd.Parameters.Add(new SqlParameter("@CourseCode", CourseCode));
            //cmd.Parameters.Add(new SqlParameter("@BatchId", BatchId));
            //cmd.Connection = thisConnection;
            //string MyDataSource1 = "USP_GetCandidateFeeDetail";
            ////MyDataSource1 = MyDataSource1.OrderByDescending(x=>x.)
            //cmd.CommandText = string.Format(MyDataSource1);
            //cmd.CommandType = CommandType.StoredProcedure;
            //SqlDataAdapter daN = new SqlDataAdapter(cmd);
            //System.Data.DataSet DataSet1 = new System.Data.DataSet();
            //daN.Fill(DataSet1);
            //ReportDataSource reportDataSource = new ReportDataSource();
            //reportDataSource.Name = "DataSet1";
            //reportDataSource.Value = DataSet1.Tables[0];
            //rptViewer.LocalReport.DataSources.Add(reportDataSource);

            //rptViewer.ProcessingMode = ProcessingMode.Local;
            //rptViewer.SizeToReportContent = true;
            //rptViewer.ZoomMode = ZoomMode.PageWidth;
            //rptViewer.Width = Unit.Percentage(99);
            //rptViewer.Height = Unit.Pixel(1000);
            //var reList = rptViewer.LocalReport.ListRenderingExtensions();
            //string mimeType = string.Empty;
            //string encoding = string.Empty;
            //rptViewer.LocalReport.Refresh();
            //string extension = string.Empty;
            //byte[] bytes = rptViewer.LocalReport.Render("PDF", null);

            //Response.Buffer = true;
            //Response.Clear();
            //Response.ContentType = mimeType;
            //Response.AddHeader("content-disposition", "attachment; filename=CandidateFeeRecord.pdf");
            //Response.BinaryWrite(bytes); // create the file
            //Response.Flush();
            //return View();
        }

    }
}