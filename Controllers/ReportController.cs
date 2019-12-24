using AJSolutions.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.DAL;
using System.Reflection;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Web.UI;
using Microsoft.SqlServer;
using System.Data.OleDb;
using System.Globalization;
using System.Web.Script.Serialization;
using PagedList;


namespace AJSolutions.Controllers
{
    public class ReportController : Controller
    {
        AdminManager admin = new AdminManager();
        Generic generic = new Generic();
        CMSManager cmsMgr = new CMSManager();
        EMSManager emsMgr = new EMSManager();
        UserDBContext db = new UserDBContext();
        ReportInfo rpt = new Models.ReportInfo();
        ReportManager rm = new ReportManager();
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
        // GET: Report
        public ActionResult Index(string CourseCode, string ClientId, int? page, int PageSize = 10)
        {
            ReportManager rp = new ReportManager();

            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(UserId);
            string SubscriberId = userDetails.SubscriberId;

            PopulateCourse(SubscriberId);
            PopulateCorporates(SubscriberId, null);

            var CourseSummary = rp.GetCourseSummary(SubscriberId, CourseCode, ClientId);

            ViewBag.Page = page;
            PopulatePaging(PageSize);

            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);

            return View(CourseSummary.ToPagedList(pageNumber, pageSize));


        }

        private void PopulateCourse(string SubscriberId, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetCourseDetails(SubscriberId);
            SelectList CourseCode = new SelectList(query, "CourseCode", "CourseName", selectedValue);
            ViewBag.CourseCode = CourseCode;
        }

        private void PopulateCorporates(string SubscriberId, object selectedvalue = null)
        {
            var query = generic.GetSubscriberWiseClientListBulkUpload(SubscriberId, false);
            SelectList clientlist = new SelectList(query, "CorporateId", "Name", selectedvalue);
            ViewBag.Client = clientlist;
        }

        private void PopulatePaging(object selectedValue = null)
        {
            var PageList = generic.GetPaging();
            ViewBag.PageSize = new SelectList(PageList, "PageSize", "PageSize", selectedValue);
        }

        //public ActionResult InvoiceReportTemplate(string ReportName, string ReportDescription, int Width, int Height)
        //[Authorize(Roles = "Admin,Client,Employee")]
        public ActionResult InvoiceReportTemplate(string Id)
        {
            string UserId = User.Identity.GetUserId();
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            string InvoiceNumber = Id;
            ViewData["InvoiceNo"] = InvoiceNumber;
            //ViewBag.ReportViewer = rptViewer;
            return View();
        }
        //[Authorize(Roles = "Admin,Client,Employee")]
        public ActionResult DownloadInvoice(string InvoiceNumber)
        {
            string UserId = User.Identity.GetUserId();
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/Invoice1.rdlc"; /// Server.MapPath("Invoice1.rdlc");
            string SubscriberId = " ";
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@SubscriberId", SubscriberId));
            cmd.Parameters.Add(new SqlParameter("@InvoiceNumber", InvoiceNumber));
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            cmd.Connection = thisConnection;
            string MyDataSource1 = "USP_GetInvoiceDetails";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            System.Data.DataSet MyDataSet1 = new System.Data.DataSet();
            da.Fill(MyDataSet1);
            SqlCommand cmd1 = new SqlCommand("select * from invoiceitems where invoiceNumber=@InvoiceNumber", thisConnection);
            cmd1.Parameters.Add(new SqlParameter("@InvoiceNumber", InvoiceNumber));
            cmd1.CommandType = CommandType.Text;
            SqlDataAdapter daN = new SqlDataAdapter(cmd1);
            System.Data.DataSet MyDataSet2 = new System.Data.DataSet();
            daN.Fill(MyDataSet2);
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "MyDataSet";
            reportDataSource.Value = MyDataSet1.Tables[0];
            ReportDataSource reportDataSource1 = new ReportDataSource();
            reportDataSource1.Name = "InvoiceDetails";
            reportDataSource1.Value = MyDataSet2.Tables[0];
            ReportParameter[] parms = new ReportParameter[2];
            parms[0] = new ReportParameter("SubscriberId", SubscriberId);
            parms[1] = new ReportParameter("InvoiceNumber", InvoiceNumber);
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
            bool excxel = rptViewer.LocalReport.ListRenderingExtensions().ToList().Find(x => x.Name.Equals("EXCELOPENXML", StringComparison.CurrentCultureIgnoreCase)).Visible;
            //byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
            byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=" + InvoiceNumber + "." + "pdf");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            return View(rptViewer);
        }
        public FileResult File()
        {
            ReportViewer rv = new Microsoft.Reporting.WebForms.ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.ShowParameterPrompts = true;
            rv.LocalReport.ReportPath = "bin/Invoice1.rdlc"; // Server.MapPath("Invoice1.rdlc");
            rv.LocalReport.Refresh();
            byte[] streamBytes = null;
            string mimeType = "";
            string encoding = "";
            string filenameExtension = "";
            string[] streamids = null;
            Warning[] warnings = null;
            streamBytes = rv.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            return File("Invoice.pdf", mimeType);
        }
        public ActionResult ASPXView()
        {
            return View();
        }
        public ActionResult ASPXUserControl()
        {
            return View();
        }
        public ActionResult GetBatch(string CourseCode)
        {
            List<SelectListItem> BatchId = new List<SelectListItem>();
            string SubscriberId = User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(CourseCode))
            {
                List<CourseBatch> Batches = (from b in db.CourseBatch
                                             join c in db.CourseMaster
                                             on b.CourseCode equals c.CourseCode
                                             where b.CourseCode == CourseCode && c.SubscriberId == SubscriberId
                                             select b).ToList();
                Batches.ForEach(x =>
                {
                    BatchId.Add(new SelectListItem { Text = x.BatchName, Value = x.BatchId.ToString() });
                });
            }
            return Json(BatchId, JsonRequestBehavior.AllowGet);
            //string UserId = User.Identity.GetUserId();
            //System.Collections.Generic.List<AJSolutions.Models.CourseBatch> LstBatchIdEmpTra = new System.Collections.Generic.List<AJSolutions.Models.CourseBatch>();
            //LstBatchIdEmpTra = PopulateBatchByCourseEmpTra(UserId, CourseCode) as List<string>;
            //PopulateBatchByCourseEmpTra(UserId, CourseCode);

            //return Json(LstBatchIdEmpTra, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult AdminReport(string SubscriberId, string ddlRolelist, string ddlTypelist)
        {
            string UserId = User.Identity.GetUserId();

            ReportInfo rptinfo = new ReportInfo();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            Rolelist();
            // Typelist();
            string[] JOClient = TempData["JOClientPopulate"] as string[];
            string[] ClentStatus = TempData["ClientStatusPopulate"] as string[];
            if (TempData["JOClientPopulate"] != null)
                PopulateStatus(JOClient);
            else
                PopulateStatus();
            if (TempData["ClientStatusPopulate"] != null)
                PopulateClient(userDetails.SubscriberId, ClentStatus);
            else if (ddlRolelist != "Emp" && ddlTypelist != "Tsk")
            {
                PopulateClient(userDetails.SubscriberId);
            }
            else
            {
                PopulateEmployeeTask(userDetails.SubscriberId);
            }
            PopulateOrderType(userDetails.SubscriberId);
            JobOrderAction();
            //if (ddlRolelist == "Emp" && ddlTypelist == "Tra")
            //    EmpTraAction();
            //else if (ddlRolelist == "Can" && ddlTypelist == "Pro")
            //    CanProAction();
            //EmpTrainAction();
            TypelistAll(ddlRolelist);
            var Typelist1 = new SelectList(new[] 
                {
                    new { ID = "", Name = "" },
                    new { ID = "StrBet", Name = "Scheduled Between" },
                    new { ID = "ComBet", Name = "Completed Between" },                                       
                },
               "ID", "Name", null);
            ViewData["EmpTraAction"] = Typelist1;
            PopulateCourseForEmpTra(userDetails.SubscriberId);
            PopulateBatchByCourseEmpTra(userDetails.SubscriberId);
            PopulateEmployeeFaculty(userDetails.SubscriberId);
            EmployeeTaskStatus();
            ViewData["ClientProfileAdmin"] = TempData["ClientProfileAdmin"];
            ViewBag.ClientProfileAdmin = ViewData["ClientProfileAdmin"];
            ViewData["ClientProfileAdminRequiredField"] = TempData["ClientProfileAdminRequiredField"];
            ViewBag.ClientProfileRequiredFied = TempData["ClientProfileAdminRequiredField"];
            ViewData["ClientJobOrderAdminRequiredField"] = TempData["ClientJobOrderAdminRequiredField"];
            ViewBag.ClientJobOrderAdminRequiredField = TempData["ClientJobOrderAdminRequiredField"];
            ViewData["ClientJobOrderAdmin"] = TempData["ClientJobOrderAdmin"];
            ViewBag.EmpTsk = ViewData["EmpTsk"];
            ViewBag.SubscriberID = userDetails.SubscriberId;
            Models.CandidateDetails candidatedet = new CandidateDetails();
            return View();
        }

        public ActionResult Rolelist()
        {
            var Rolelist = new SelectList(new[] 
            {
                   
                    new { ID = "Cli", Name = "Client" },
                    //new { ID = "Stu", Name = "Student" },
                    new { ID = "Emp", Name = "Employee" },
                    //new { ID = "OTP", Name = "Third Party" },
                    new { ID = "Can", Name = "Candidate" },
             },
                "ID", "Name", 1);
            ViewData["Rolelist"] = Rolelist;
            return View();
        }

        public ActionResult Typelist(string Role)
        {
            if (Role == "Cli")
            {
                var Typelist = new SelectList(new[] 
            {
                 
                    new { ID = "Pro", Name = "Profile" },
                    new { ID = "Jor", Name = "Job Orders" },                   
             },
                    "ID", "Name", 1);
                ViewData["Typelist"] = Typelist;
            }
            else if (Role == "Emp")
            {
                var Typelist = new SelectList(new[] 
            {
                 
                    new { ID = "Pro", Name = "Profile" },
                    //new { ID = "Jor", Name = "Job Orders" },
                    //new { ID = "Lev", Name = "Leaves" },
                    new { ID = "Tsk", Name = "Tasks" },
                    new { ID = "Tra", Name = "Training" },
                    //new { ID = "Cof", Name = "Course Fee" },
             },
                    "ID", "Name", 1);
                ViewData["Typelist"] = Typelist;
            }
            else if (Role == "Can")
            {
                var Typelist = new SelectList(new[] 
            {
                 
                    new { ID = "Pro", Name = "Profile" },
                    //new { ID = "Jor", Name = "Job Orders" },
                    //new { ID = "Lev", Name = "Leaves" },
                    //new { ID = "Tsk", Name = "Tasks" },
                    //new { ID = "Tra", Name = "Training" },
                    new { ID = "Cof", Name = "Course Fee" },
             },
                    "ID", "Name", 1);
                ViewData["Typelist"] = Typelist;
            }
            else
            {
                var Typelist = new SelectList(new[] 
                    {                 
                    new { ID = "", Name = "" },
                    //new { ID = "Jor", Name = "Job Orders" },
                    //new { ID = "Lev", Name = "Leaves" },
                    //new { ID = "Tsk", Name = "Tasks" },
                    //new { ID = "Tra", Name = "Training" },
                    //new { ID = "Cof", Name = "Course Fee" },
                    },
                    "ID", "Name", 1);
                ViewData["Typelist"] = Typelist;
            }
            return Json(ViewData["Typelist"], JsonRequestBehavior.AllowGet);
        }

        public ActionResult TypelistAll(string Role)
        {
            var Typelist = new SelectList(new[] 
            {
                    new { ID = "", Name = "" },
                    //new { ID = "Pro", Name = "Profile" },
                    //new { ID = "Jor", Name = "Job Orders" },
                    //new { ID = "Lev", Name = "Leaves" },
                    //new { ID = "Tsk", Name = "Tasks" },
                    //new { ID = "Tra", Name = "Training" },
                    //new { ID = "Cof", Name = "Course Fee" },
             },
                  "ID", "Name", 1);
            ViewData["Typelist"] = Typelist;
            return View();
        }

        public ActionResult JobOrderAction()
        {
            var Typelist = new SelectList(new[] 
                {
                    new { ID = "", Name = "" },
                    new { ID = "CreBet", Name = "Created Between" },
                    new { ID = "ComBet", Name = "Completed Between" },                                       
                },
                "ID", "Name", 1);
            ViewData["JOAction"] = Typelist;
            return View();
        }

        public ActionResult EmpTrainAction(string selectedvalue = null)
        {
            var Typelist = new SelectList(new[] 
                {
                    new { ID = "", Name = "" },
                    new { ID = "StrBet", Name = "Scheduled Between" },
                    new { ID = "ComBet", Name = "Completed Between" },                                       
                },
                "ID", "Name", selectedvalue);
            ViewData["EmpTraAction"] = Typelist;
            return Json(Typelist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CanProAction(string selectedvalue = null)
        {
            var Typelist = new SelectList(new[] 
                {
                    new { ID = "", Name = "" },                    
                    new { ID = "RegBet", Name = "Registered Between" },                                       
                },
                "ID", "Name", selectedvalue);
            ViewData["EmpTraAction"] = Typelist;
            return Json(Typelist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmployeeTaskStatus()
        {
            var Typelist = new SelectList(new[] 
                {
                    new { ID = "", Name = "All" },
                    new { ID = "Unaccepted", Name = "Unaccepted" },
                    new { ID = "Assigned", Name = "Assigned" }, 
                    new { ID = "Inprogress", Name = "Inprogress" }, 
                    new { ID = "Rejected", Name = "Rejected" }, 
                    new { ID = "Completed", Name = "Completed" }, 
                    new { ID = "Discarded", Name = "Discarded" }, 
                },
                "ID", "Name", "0");
            ViewData["EmpTraStatus"] = Typelist;
            return View();
        }
        [HttpGet]
        public ActionResult PopUpReport()
        {
            return View();
        }

        public ActionResult AdminGetReport()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AdminReport(string ddlRolelist, string ddlTypelist, string ChkchkName, string ChkRegisOn, string ChkLastLoggedOn, string ChkGender, string ChkCompName, string ChkCompType, string ChkCompSize,
           string ChkCompWeb, string ChkPerAddress, string ChkPreAddress, string ChkCompAddress, string ChkCoressAddress, string ChkBank, string StartDate, string EndDate,
           string[] JOClient, string[] JOStatus, string JOType, string JOAction, DateTime? JOStartDate, DateTime? JOEndDate, string ChkJOId, string ChkJOTitle, string ChkJOCostAmount,
           string ChkJODescription, string ChkJOInvoice, string ChkInvoiceStatus, string btnDownloadReport, string btnDownloadJobOrderReport, string LstEmpCourseCode,
           string LstBatchIdEmpTra, string LstEmpTraining, string EmpTrainingStatus, string EmpTraAction, string EmpTraStartDate, string EmpTraEndDate, string EmpTraScheDuleID,
           string EmpTraScheDuleName, string EmpTraScheVenue, string EmpTraScheCity, string EmpTraScheState, string EmpTraScheCountry, string EmpTraScheNoOfCandidate,
            string btnDownloadEmpTraReport, string EmpTraAccomodation, string EmpTraAttendance, string EmpTraEmailID, string EmpTraMobile, string EmpTraAlternateEmail, string EmpTraAlternateMobile,
            string EmpTraGender, string EmpTraRegistrationId, string EmpTraDesignation, string EmpTraRegion, string EmpTraBranchName, string EmpTraBranchCategory, string EmpTraBranchState,
            string EmpTraBranchCode, string EmpTraBatchDuration, string EmpTraBatchTiming, string CanProName, string CanProUserId, string CanProPassword, string CanCourseFee, string CanCourseAmountPaid, string CanCourseBalanceDue,
            string ddlEmpTraAction, string btnDownloadEmpTraGraphicalReport, string[] Emplyee1, string ChkClientProfileEmail, string ChkClientProfilePhone,
            string ChkClientProfileAlternateEmail, string ChkClientProfileAlternateContact, string ChkClientProfileNationality, string ChkRole, string ChkReportingAuthority,
            string chkCanProEmail, string chkCanProCorporate)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            Rolelist();
            Typelist(ddlRolelist);
            if (Emplyee1 != null)
            {
                JOClient = Emplyee1;
            }
            if (TempData["ClientStatusPopulate"] != null)
                PopulateClient(userDetails.SubscriberId);
            else if (ddlRolelist != "Emp" && ddlTypelist != "Tsk")
            {
                PopulateClient(userDetails.SubscriberId);
            }
            else
            {
                PopulateEmployeeTask(userDetails.SubscriberId);
            }
            PopulateStatus(JOStatus);
            PopulateOrderType(userDetails.SubscriberId);
            JobOrderAction();
            if (ddlRolelist == "Emp" && ddlTypelist == "Tra")
            {
                EmpTrainAction(EmpTraAction);
            }
            else if (ddlRolelist == "Can" && ddlTypelist == "Pro")
                CanProAction(EmpTraAction);
            else if (ddlRolelist == "Can" && ddlTypelist == "Cof")
                CanProAction(EmpTraAction);
            else
            {
                EmpTrainAction(EmpTraAction);
            }
            PopulateCourseForEmpTra(userDetails.SubscriberId);
            if (LstEmpCourseCode != null && LstBatchIdEmpTra != null)
                PopulateBatchByCourseEmpTra(userDetails.SubscriberId, LstEmpCourseCode, LstBatchIdEmpTra);
            else if (LstEmpCourseCode != null)
                PopulateBatchByCourseEmpTra(userDetails.SubscriberId, LstEmpCourseCode);
            else
                PopulateBatchByCourseEmpTra(userDetails.SubscriberId);
            PopulateEmployeeFaculty(userDetails.SubscriberId);
            EmployeeTaskStatus();
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            Models.CandidateDetails candidatedet = new CandidateDetails();
            DataTable clientProfileDs = new DataTable();
            if (JOType == "")
            {
                JOType = "0";
            }
            if (ddlRolelist != string.Empty && ddlTypelist != string.Empty)
            {
                string StrJobOrderStatus = "";
                string strJOClient = "";
                if (JOStatus != null)
                {
                    for (int i = 0; i < JOStatus.Length; i++)
                    {
                        StrJobOrderStatus = StrJobOrderStatus + JOStatus[i].ToString() + ",";
                    }
                    StrJobOrderStatus = StrJobOrderStatus.TrimEnd(',');
                }
                if (JOClient != null)
                {
                    for (int i = 0; i < JOClient.Length; i++)
                    {
                        strJOClient = strJOClient + JOClient[i].ToString() + ",";
                    }
                    strJOClient = strJOClient.TrimEnd(',');
                }
                int TotalDays = 0;
                if (JOStartDate != null && JOEndDate != null)
                {
                    DateTime sDate = Convert.ToDateTime(JOStartDate);
                    DateTime eDate = Convert.ToDateTime(JOEndDate);
                    TotalDays = Convert.ToInt32((eDate - sDate).TotalDays);
                }
                if (ddlRolelist == "Cli" && ddlTypelist == "Pro")
                {
                    if (ChkchkName == "true" || ChkBank == "true" || ChkCompAddress == "true" || ChkCoressAddress == "true" || ChkPerAddress == "true" || ChkPreAddress == "true" || ChkRegisOn == "true" || ChkLastLoggedOn == "true" || ChkCompName == "true" || ChkCompSize == "true" || ChkCompType == "true" || ChkCompWeb == "true" || ChkClientProfileEmail == "true" || ChkClientProfilePhone == "true" || ChkClientProfileAlternateEmail == "true" || ChkClientProfileAlternateContact == "true" || ChkClientProfileNationality == "true")
                    {
                        ReportViewer rptViewer = new ReportViewer();
                        rptViewer.LocalReport.ReportPath = "Views/Report/UserProfileAdmin.rdlc"; /// Server.MapPath("Invoice1.rdlc");            
                        string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                        SqlConnection thisConnection = new SqlConnection(thisConnectionString);
                        SqlCommand cmd = new SqlCommand();
                        cmd.Parameters.Add(new SqlParameter("@SubscriberId", userDetails.SubscriberId));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", EndDate));
                        cmd.Connection = thisConnection;
                        string MyDataSource1 = "USP_GetClientDetailsDateWise";
                        cmd.CommandText = string.Format(MyDataSource1);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter daN = new SqlDataAdapter(cmd);
                        System.Data.DataSet DataSet1 = new System.Data.DataSet();
                        daN.Fill(DataSet1);
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet1";
                        reportDataSource.Value = DataSet1.Tables[0];
                        string[] ClientProfileRequiredFied = { ChkchkName, ChkBank, ChkCompAddress, ChkCoressAddress, ChkPerAddress, ChkPreAddress, ChkRegisOn, ChkLastLoggedOn, ChkCompName, ChkCompSize, ChkCompType, ChkCompWeb, ChkClientProfileEmail, ChkClientProfilePhone, ChkClientProfileAlternateEmail, ChkClientProfileAlternateContact, ChkClientProfileNationality };
                        SelectList clientProfileRequiredFied = new SelectList(ClientProfileRequiredFied);
                        ViewBag.ClientProfileRequiredFied = clientProfileRequiredFied;
                        //ViewBag.ClientProfileRequiredFields = ClientProfileRequiredFied;
                        TempData["ClientProfileAdminRequiredField"] = clientProfileRequiredFied.ToList();
                        TempData["ClientProfileAdmin"] = DataSet1.Tables[0].AsEnumerable();
                        List<DataRow> list = DataSet1.Tables[0].AsEnumerable().ToList();
                        List<string[]> results = DataSet1.Tables[0].Select().Select(dr => dr.ItemArray.Select(x => x.ToString()).ToArray()).ToList();
                        ViewBag.StartDate = StartDate;
                        ViewBag.EndDate = EndDate;
                        ViewData["ClientProfileAdmin"] = TempData["ClientProfileAdmin"];
                        ViewBag.ClientProfileAdmin = ViewData["ClientProfileAdmin"];
                        clientProfileDs = DataSet1.Tables[0];
                        //JavaScriptSerializer serializer = new JavaScriptSerializer();
                        //ViewData["JsonRegionList"] = list;
                        if (DataSet1.Tables[0].Rows.Count == 0)
                        {
                            @TempData["NoRecordFound"] = true;
                        }
                        if (btnDownloadReport != null)
                        {
                            ReportParameter[] parms = new ReportParameter[20];
                            parms[0] = new ReportParameter("SubscriberID", userDetails.SubscriberId);
                            parms[1] = new ReportParameter("StartDate", StartDate);
                            parms[2] = new ReportParameter("EndDate", EndDate);
                            //if(Name=="1")
                            parms[3] = new ReportParameter("Name", ChkchkName);
                            //if (RegisteredOn == "1")
                            parms[4] = new ReportParameter("Registeredon", ChkRegisOn);
                            //if (LastLogin == "1")
                            parms[5] = new ReportParameter("LastLogin", ChkLastLoggedOn);
                            //if (CompanyName == "1")
                            parms[6] = new ReportParameter("CompanyName", ChkCompName);
                            //if (CompanyType == "1")
                            parms[7] = new ReportParameter("CompanyType", ChkCompType);
                            //if (CompSize == "1")
                            parms[8] = new ReportParameter("CompanySize", ChkCompSize);
                            //if (Website == "1")
                            parms[9] = new ReportParameter("Website", ChkCompWeb);
                            //if (PermanenetAddress == "1")
                            parms[10] = new ReportParameter("PermanenetAddress", ChkPerAddress);
                            //if (PresentAddress == "1")
                            parms[11] = new ReportParameter("PresentAddress", ChkPreAddress);
                            //if (CompanyAddress == "1")
                            parms[12] = new ReportParameter("CompanyAddress", ChkCompAddress);
                            //if (CorrespondenceAddress == "1")
                            parms[13] = new ReportParameter("CorrespondenceAddress", ChkCoressAddress);
                            //if (Bank == "1")
                            parms[14] = new ReportParameter("Bank", ChkBank);
                            parms[15] = new ReportParameter("Email", ChkClientProfileEmail);
                            parms[16] = new ReportParameter("Phone", ChkClientProfilePhone);
                            parms[17] = new ReportParameter("AlternateEmail", ChkClientProfileAlternateEmail);
                            parms[18] = new ReportParameter("AlternateContact", ChkClientProfileAlternateContact);
                            parms[19] = new ReportParameter("Nationality", ChkClientProfileNationality);
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
                            Response.AddHeader("content-disposition", "attachment; filename=ClientProfile.pdf");
                            Response.BinaryWrite(bytes); // create the file
                            Response.Flush();
                        }
                    }
                    else
                    {
                        @TempData["ClientUserProfileRequired"] = true;
                    }
                    //return Json(clientProfileDs.Rows, JsonRequestBehavior.AllowGet);
                    //return Json(ViewData["JsonRegionList"], JsonRequestBehavior.AllowGet);  
                    return View();
                    //return RedirectToAction("PopUpReport", "Report", new{ }); 
                }
                else if (ddlRolelist == "Cli" && ddlTypelist == "Jor")
                {
                    if (JOAction != "")
                    {
                        if (ChkJOId == "true" || ChkJOTitle == "true" || ChkJOCostAmount == "true" || ChkJOInvoice == "true" || ChkJODescription == "true" || ChkInvoiceStatus == "true")
                        {
                            //ViewData["UserProfile"] = generic.GetUserDetail(UserId);
                            ReportViewer rptViewer = new ReportViewer();
                            rptViewer.LocalReport.ReportPath = "Views/Report/ClientJobOrderAdmin.rdlc"; /// Server.MapPath("Invoice1.rdlc");            
                            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
                            SqlCommand cmd = new SqlCommand();
                            if (strJOClient == "")
                            {
                                strJOClient = null;
                            }
                            if (StrJobOrderStatus == "")
                            {
                                StrJobOrderStatus = null;
                            }
                            cmd.Parameters.Add(new SqlParameter("@ClientId", strJOClient));
                            cmd.Parameters.Add(new SqlParameter("@JobOrderStatus", StrJobOrderStatus));
                            cmd.Parameters.Add(new SqlParameter("@JobOrderType", JOType));
                            cmd.Parameters.Add(new SqlParameter("@StartDate", JOStartDate));
                            cmd.Parameters.Add(new SqlParameter("@EndDate", JOEndDate));
                            cmd.Parameters.Add(new SqlParameter("@Duration", TotalDays));
                            cmd.Parameters.Add(new SqlParameter("@JOAction", JOAction));
                            cmd.Parameters.Add(new SqlParameter("@SubscriberID", userDetails.SubscriberId));
                            cmd.Connection = thisConnection;
                            string MyDataSource1 = "USP_GetClientJobOrder";
                            cmd.CommandText = string.Format(MyDataSource1);
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter daN = new SqlDataAdapter(cmd);
                            System.Data.DataSet DataSet1 = new System.Data.DataSet();
                            daN.Fill(DataSet1);
                            ReportDataSource reportDataSource = new ReportDataSource();
                            reportDataSource.Name = "MyDataSet";
                            reportDataSource.Value = DataSet1.Tables[0];
                            string[] ClientJobOrderAdminRequiredField = { ChkJOId, ChkJOTitle, ChkJOCostAmount, ChkJOInvoice, ChkJODescription, ChkInvoiceStatus };
                            SelectList clientJobOrderAdminRequiredField = new SelectList(ClientJobOrderAdminRequiredField);
                            ViewBag.ClientJobOrderAdminRequiredField = clientJobOrderAdminRequiredField;
                            //ViewBag.ClientProfileRequiredFields = ClientProfileRequiredFied;
                            TempData["ClientJobOrderAdminRequiredField"] = clientJobOrderAdminRequiredField.ToList();
                            TempData["ClientJobOrderAdmin"] = DataSet1.Tables[0].AsEnumerable();
                            ViewData["ClientJobOrderAdmin"] = TempData["ClientJobOrderAdmin"];
                            ViewBag.clientJobOrderAdmin = ViewData["ClientJobOrderAdmin"];
                            ViewBag.joborderDtartDate = JOStartDate;
                            ViewBag.joborderEndDate = JOEndDate;
                            ViewBag.jobClientID = JOClient;
                            ViewBag.strJobOrderStatus = JOStatus;
                            cmd.Parameters.Clear();
                            //ViewData["JoAdmin"] = rm.GetJobOrders(JOStartDate,JOEndDate, strJOClient, StrJobOrderStatus,Convert.ToInt32(JOType), TotalDays, JOAction, UserId);
                            //PopulateClient(UserId, JOClient);
                            //PopulateStatus(JOStatus);
                            //ViewBag.ClientList = JOClient;
                            var ClientJobOrderAdmin = DataSet1.Tables[0].AsEnumerable();
                            TempData["JOClientPopulate"] = JOClient;
                            TempData["ClientStatusPopulate"] = JOStatus;
                            if (DataSet1.Tables[0].Rows.Count == 0)
                            {
                                @TempData["NoRecordFound"] = true;
                            }
                            if (btnDownloadJobOrderReport != null)
                            {
                                ReportParameter[] parms = new ReportParameter[14];
                                parms[0] = new ReportParameter("ClientId", strJOClient);
                                parms[1] = new ReportParameter("JobOrderStatus", StrJobOrderStatus);
                                parms[2] = new ReportParameter("JobOrderType", JOType.ToString());
                                //if(Name=="1")
                                parms[3] = new ReportParameter("StartDate", JOStartDate.ToString());
                                //if (RegisteredOn == "1")
                                parms[4] = new ReportParameter("Duration", TotalDays.ToString());
                                //if (LastLogin == "1")
                                parms[5] = new ReportParameter("JOId", ChkJOId);
                                parms[6] = new ReportParameter("JOTitle", ChkJOTitle);
                                //if (CompanyName == "1")
                                parms[7] = new ReportParameter("JOCosting", ChkJOCostAmount);
                                //if (CompanyType == "1")
                                parms[8] = new ReportParameter("JoInvoiceNo", ChkJOInvoice);
                                //if (CompSize == "1")
                                parms[9] = new ReportParameter("JODescription", ChkJODescription);
                                //if (Website == "1")              
                                parms[10] = new ReportParameter("EndDate", JOEndDate.ToString());
                                parms[11] = new ReportParameter("JOAction", JOAction);
                                parms[12] = new ReportParameter("SubscriberID", userDetails.SubscriberId);
                                parms[13] = new ReportParameter("JOInvoiceStatus", ChkInvoiceStatus);
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
                                Response.AddHeader("content-disposition", "attachment; filename=JobOrderClient.pdf");
                                Response.BinaryWrite(bytes); // create the file
                                Response.Flush();
                                //return RedirectToAction("AdminReport", "Report");    
                                //return View(rptViewer);                                
                            }
                        }
                        else
                        {
                            @TempData["ClientJobOrderRequiredFields"] = true;
                        }
                        //return RedirectToAction("AdminReport", "Report"); 
                        return View();
                        //return Json(ViewBag.clientJobOrderAdmin, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        @TempData["ClientJobOrderRequired"] = true;
                    }
                }
                else if (ddlRolelist == "Emp" && ddlTypelist == "Pro")
                {
                    if (ChkchkName == "true" || ChkCompAddress == "true" || ChkCoressAddress == "true" || ChkPerAddress == "true" || ChkPreAddress == "true" || ChkRegisOn == "true" || ChkLastLoggedOn == "true" || ChkClientProfileEmail == "true" || ChkClientProfilePhone == "true" || ChkRole == "true" || ChkReportingAuthority == "true")
                    {
                        ReportViewer rptViewer = new ReportViewer();
                        rptViewer.LocalReport.ReportPath = "Views/Report/EmpProfileAdmin.rdlc"; /// Server.MapPath("Invoice1.rdlc");            
                        string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                        SqlConnection thisConnection = new SqlConnection(thisConnectionString);
                        SqlCommand cmd = new SqlCommand();
                        cmd.Parameters.Add(new SqlParameter("@SubscriberId", userDetails.SubscriberId));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", EndDate));
                        cmd.Connection = thisConnection;
                        string MyDataSource1 = "USP_GetEmployeeProfileDetails";
                        cmd.CommandText = string.Format(MyDataSource1);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter daN = new SqlDataAdapter(cmd);
                        System.Data.DataSet DataSet1 = new System.Data.DataSet();
                        daN.Fill(DataSet1);
                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "DataSet1";
                        reportDataSource.Value = DataSet1.Tables[0];
                        string[] ClientProfileRequiredFied = { ChkchkName, ChkRegisOn, ChkLastLoggedOn, ChkPerAddress, ChkPreAddress, ChkCompAddress, ChkCoressAddress, ChkClientProfileEmail, ChkClientProfilePhone, ChkRole, ChkReportingAuthority };
                        SelectList EmpProfileRequiredFied = new SelectList(ClientProfileRequiredFied);
                        ViewBag.EmpProfileRequiredField = EmpProfileRequiredFied;
                        //ViewBag.ClientProfileRequiredFields = ClientProfileRequiredFied;
                        TempData["EmpProfileAdminRequiredField"] = EmpProfileRequiredFied.ToList();
                        TempData["EmpProfileAdmin"] = DataSet1.Tables[0].AsEnumerable();
                        List<DataRow> list = DataSet1.Tables[0].AsEnumerable().ToList();
                        List<string[]> results = DataSet1.Tables[0].Select().Select(dr => dr.ItemArray.Select(x => x.ToString()).ToArray()).ToList();
                        ViewBag.StartDate = StartDate;
                        ViewBag.EndDate = EndDate;
                        ViewData["EmpProfileAdmin"] = TempData["EmpProfileAdmin"];
                        ViewBag.EmpProfileAdmin = ViewData["EmpProfileAdmin"];
                        if (DataSet1.Tables[0].Rows.Count == 0)
                        {
                            @TempData["NoRecordFound"] = true;
                        }
                        if (btnDownloadReport != null)
                        {
                            ReportParameter[] parms = new ReportParameter[14];
                            parms[0] = new ReportParameter("SubscriberID", userDetails.SubscriberId);
                            parms[1] = new ReportParameter("StartDate", StartDate);
                            parms[2] = new ReportParameter("EndDate", EndDate);
                            //if(Name=="1")
                            parms[3] = new ReportParameter("Name", ChkchkName);
                            //if (RegisteredOn == "1")
                            parms[4] = new ReportParameter("Registeredon", ChkRegisOn);
                            //if (LastLogin == "1")
                            parms[5] = new ReportParameter("LastLogin", ChkLastLoggedOn);
                            //if (PermanenetAddress == "1")
                            parms[6] = new ReportParameter("PermanenetAddress", ChkPerAddress);
                            //if (PresentAddress == "1")
                            parms[7] = new ReportParameter("PresentAddress", ChkPreAddress);
                            //if (CompanyAddress == "1")
                            parms[8] = new ReportParameter("CompanyAddress", ChkCompAddress);
                            //if (CorrespondenceAddress == "1")
                            parms[9] = new ReportParameter("CorrespondenceAddress", ChkCoressAddress);
                            //, ChkClientProfileEmail, ChkClientProfilePhone, ChkRole, ChkReportingAuthority
                            parms[10] = new ReportParameter("Email", ChkClientProfileEmail);
                            parms[11] = new ReportParameter("Phone", ChkClientProfilePhone);
                            parms[12] = new ReportParameter("Role", ChkRole);
                            parms[13] = new ReportParameter("Authority", ChkReportingAuthority);
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
                            Response.AddHeader("content-disposition", "attachment; filename=EmployeeProfile.pdf");
                            Response.BinaryWrite(bytes); // create the file
                            Response.Flush();
                            //return RedirectToAction("AdminReport", "Report");                             
                        }
                    }
                    else
                    {
                        @TempData["ClientUserProfileRequired"] = true;
                    }
                    return View();
                }
                else if (ddlRolelist == "Emp" && ddlTypelist == "Tsk")
                {
                    if (JOAction != "")
                    {
                        if (ChkJOId == "true" || ChkJOTitle == "true" || ChkJOCostAmount == "true" || ChkJOInvoice == "true" || ChkJODescription == "true" || ChkInvoiceStatus == "true")
                        {
                            //ViewData["UserProfile"] = generic.GetUserDetail(UserId);
                            ReportViewer rptViewer = new ReportViewer();
                            rptViewer.LocalReport.ReportPath = "Views/Report/EmployeeTasks.rdlc"; /// Server.MapPath("Invoice1.rdlc");            
                            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.Add(new SqlParameter("@StartDate", JOStartDate));
                            cmd.Parameters.Add(new SqlParameter("@EndDate", JOEndDate));
                            if (strJOClient == "")
                            {
                                strJOClient = null;
                            }
                            cmd.Parameters.Add(new SqlParameter("@ClientId", strJOClient));
                            cmd.Parameters.Add(new SqlParameter("@TaskStatus", StrJobOrderStatus));
                            if (JOType == "0")
                            {
                                JOType = null;
                            }
                            cmd.Parameters.Add(new SqlParameter("@TaskTypeId", JOType));
                            cmd.Parameters.Add(new SqlParameter("@JOAction", JOAction));
                            cmd.Parameters.Add(new SqlParameter("@SubscriberID", userDetails.SubscriberId));
                            cmd.Connection = thisConnection;
                            string MyDataSource1 = "USP_GetEmployeeTasks";
                            cmd.CommandText = string.Format(MyDataSource1);
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter daN = new SqlDataAdapter(cmd);
                            System.Data.DataSet DataSet1 = new System.Data.DataSet();
                            daN.Fill(DataSet1);
                            ReportDataSource reportDataSource = new ReportDataSource();
                            reportDataSource.Name = "MyDataSet";
                            reportDataSource.Value = DataSet1.Tables[0];
                            string[] ClientJobOrderAdminRequiredField = { ChkJOId, ChkJOTitle, ChkJOCostAmount, ChkJOInvoice, ChkJODescription, ChkInvoiceStatus };
                            SelectList clientJobOrderAdminRequiredField = new SelectList(ClientJobOrderAdminRequiredField);
                            ViewBag.ClientJobOrderAdminRequiredField = clientJobOrderAdminRequiredField;
                            //ViewBag.ClientProfileRequiredFields = ClientProfileRequiredFied;
                            TempData["ClientJobOrderAdminRequiredField"] = clientJobOrderAdminRequiredField.ToList();
                            TempData["ClientJobOrderAdmin"] = DataSet1.Tables[0].AsEnumerable();
                            ViewData["ClientJobOrderAdmin"] = TempData["ClientJobOrderAdmin"];
                            ViewBag.clientJobOrderAdmin = ViewData["ClientJobOrderAdmin"];
                            string dt = null, dt1 = null;
                            if (JOStartDate != null)
                                dt = Convert.ToDateTime(JOStartDate).ToString("yyyy/MM/dd");
                            //DateTime dt = DateTime.ParseExact(JOStartDate.ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                            //string s = dt.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                            if (JOEndDate != null)
                                dt1 = Convert.ToDateTime(JOEndDate).ToString("yyyy/MM/dd");
                            ViewBag.joborderDtartDate = dt;
                            ViewBag.joborderEndDate = dt1;
                            ViewBag.jobClientID = JOClient;
                            ViewBag.strJobOrderStatus = JOStatus;
                            cmd.Parameters.Clear();
                            TempData["EmpTsk"] = DataSet1.Tables[0].AsEnumerable();
                            ViewData["EmpTsk"] = TempData["EmpTsk"];
                            ViewBag.EmpTsk = ViewData["EmpTsk"];
                            if (DataSet1.Tables[0].Rows.Count == 0)
                            {
                                @TempData["NoRecordFound"] = true;
                            }
                            //ViewData["JoAdmin"] = rm.GetJobOrders(JOStartDate,JOEndDate, strJOClient, StrJobOrderStatus,Convert.ToInt32(JOType), TotalDays, JOAction, UserId);
                            //PopulateClient(UserId, JOClient);
                            //PopulateStatus(JOStatus);
                            //ViewBag.ClientList = JOClient;
                            var ClientJobOrderAdmin = DataSet1.Tables[0].AsEnumerable();
                            TempData["JOClientPopulate"] = JOClient;
                            TempData["ClientStatusPopulate"] = JOStatus;
                            if (btnDownloadJobOrderReport != null)
                            {
                                ReportParameter[] parms = new ReportParameter[13];
                                parms[0] = new ReportParameter("StartDate", JOStartDate.ToString());
                                parms[1] = new ReportParameter("EndDate", JOStartDate.ToString());
                                parms[2] = new ReportParameter("ClientId", strJOClient);
                                parms[3] = new ReportParameter("TaskStatus", StrJobOrderStatus);
                                parms[4] = new ReportParameter("TaskTypeId", JOType);
                                parms[5] = new ReportParameter("JOAction", JOAction);
                                parms[6] = new ReportParameter("SubscriberID", userDetails.SubscriberId);
                                parms[7] = new ReportParameter("JOId", ChkJOId);
                                parms[8] = new ReportParameter("JOTitle", ChkJOTitle);
                                parms[9] = new ReportParameter("JOCosting", ChkJOCostAmount);
                                parms[10] = new ReportParameter("JoInvoiceNo", ChkJOInvoice);
                                parms[11] = new ReportParameter("JODescription", ChkJODescription);
                                parms[12] = new ReportParameter("JOInvoiceStatus", ChkInvoiceStatus);
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
                                Response.AddHeader("content-disposition", "attachment; filename=EmployeeTasks.pdf");
                                Response.BinaryWrite(bytes); // create the file
                                Response.Flush();
                                //return RedirectToAction("AdminReport", "Report");    
                                //return View(rptViewer);
                            }
                        }
                        else
                        {
                            @TempData["ClientJobOrderRequiredFields"] = true;
                        }
                        //return RedirectToAction("AdminReport", "Report");

                        return View();
                        //return Json(ViewBag.clientJobOrderAdmin, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        @TempData["ClientJobOrderRequired"] = true;
                    }
                }
                else if (ddlRolelist == "Emp" && ddlTypelist == "Tra")
                {
                    if (EmpTraScheDuleID == "true" || EmpTraScheDuleName == "true" || EmpTraScheVenue == "true" || EmpTraScheCity == "true" || EmpTraScheState == "true" || EmpTraScheCountry == "true" || EmpTraScheNoOfCandidate == "true")
                    {
                        if (EmpTraAction != "")
                        {
                            //string EmpTraEmailID, string EmpTraMobile, string EmpTraAlternateEmail, string EmpTraAlternateMobile,
                            // string EmpTraGender, string EmpTraRegistrationId, string EmpTraDesignation, string EmpTraRegion, string EmpTraBranchName, string EmpTraBranchCategory, string EmpTraBranchState,
                            // string EmpTraBranchCode
                            ReportViewer rptViewer = new ReportViewer();
                            rptViewer.LocalReport.ReportPath = "Views/Report/EmployeeTrainingDetailedReport.rdlc"; /// Server.MapPath("Invoice1.rdlc");    //EmployeeTrainingDetailedReport.rdlc 
                            //rptViewer.LocalReport.ReportPath = "Report/EmployeeTrainingDetailedReport.rdlc";
                            ReportViewer rptViewer1 = new ReportViewer();
                            rptViewer1.LocalReport.ReportPath = "Views/Report/GraphicalAllTrainingCandidateCount.rdlc";
                            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
                            SqlCommand cmd = new SqlCommand();
                            if (LstEmpCourseCode == "")
                                LstEmpCourseCode = null;
                            cmd.Parameters.Add(new SqlParameter("@CourseCode", LstEmpCourseCode));
                            if (LstBatchIdEmpTra == "")
                                LstBatchIdEmpTra = null;
                            cmd.Parameters.Add(new SqlParameter("@BatchId", LstBatchIdEmpTra));
                            if (LstEmpTraining == "")
                                LstEmpTraining = null;
                            cmd.Parameters.Add(new SqlParameter("@TrainerID", LstEmpTraining));
                            if (EmpTrainingStatus == "")
                                EmpTrainingStatus = null;
                            cmd.Parameters.Add(new SqlParameter("@Status", EmpTrainingStatus));
                            if (EmpTraStartDate == "")
                                EmpTraStartDate = null;
                            cmd.Parameters.Add(new SqlParameter("@DateFrom", EmpTraStartDate));
                            if (EmpTraEndDate == "")
                                EmpTraEndDate = null;
                            cmd.Parameters.Add(new SqlParameter("@DateTo", EmpTraEndDate));
                            cmd.Parameters.Add(new SqlParameter("@Action", EmpTraAction));
                            cmd.Parameters.Add(new SqlParameter("@SubscriberID", userDetails.SubscriberId.Trim()));
                            cmd.Connection = thisConnection;
                            string MyDataSource1 = "USP_GetEmployeeTrainings";
                            cmd.CommandText = string.Format(MyDataSource1);
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter daN = new SqlDataAdapter(cmd);
                            System.Data.DataSet DataSet1 = new System.Data.DataSet();
                            daN.Fill(DataSet1);
                            SqlCommand cmd1 = new SqlCommand();
                            if (LstEmpCourseCode == "")
                                LstEmpCourseCode = null;
                            cmd1.Parameters.Add(new SqlParameter("@CourseCode", LstEmpCourseCode));
                            if (LstBatchIdEmpTra == "")
                                LstBatchIdEmpTra = null;
                            cmd1.Parameters.Add(new SqlParameter("@BatchId", LstBatchIdEmpTra));
                            cmd1.Parameters.Add(new SqlParameter("@SubscriberID", userDetails.SubscriberId.Trim()));
                            cmd1.Connection = thisConnection;
                            string MyDataSource2 = "USP_CandidateCInCOutWithAttendenceDetails";
                            cmd1.CommandText = string.Format(MyDataSource2);
                            cmd1.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter daN1 = new SqlDataAdapter(cmd1);
                            System.Data.DataSet DataSet2 = new System.Data.DataSet();
                            daN1.Fill(DataSet2);
                            ReportDataSource reportDataSource = new ReportDataSource();
                            reportDataSource.Name = "DataSet1";
                            reportDataSource.Value = DataSet1.Tables[0];
                            //ReportDataSource reportDataSource1 = new ReportDataSource();
                            //reportDataSource1.Name = "InvoiceDetails";
                            //reportDataSource1.Value = MyDataSet2.Tables[0];
                            ReportDataSource reportDataSource1 = new ReportDataSource();
                            reportDataSource1.Name = "DataSet2";
                            reportDataSource1.Value = DataSet2.Tables[0];
                            string[] EmployeeTrainingRequiredFied = { EmpTraScheDuleID, EmpTraScheDuleName, EmpTraScheVenue, EmpTraScheCity, EmpTraScheState, EmpTraScheCountry, EmpTraScheNoOfCandidate,
                            LstEmpCourseCode, LstBatchIdEmpTra, LstEmpTraining, EmpTrainingStatus, EmpTraAccomodation,EmpTraAttendance ,EmpTraEmailID, EmpTraMobile, EmpTraAlternateEmail, 
                            EmpTraAlternateMobile,EmpTraGender, EmpTraRegistrationId, EmpTraDesignation, EmpTraRegion, EmpTraBranchName, EmpTraBranchCategory, EmpTraBranchState,
                            EmpTraBranchCode,EmpTraBatchDuration,EmpTraBatchTiming};
                            SelectList employeeTrainingRequiredFied = new SelectList(EmployeeTrainingRequiredFied);
                            ViewBag.EmployeeTrainingRequiredFied = employeeTrainingRequiredFied;
                            //ViewBag.ClientProfileRequiredFields = ClientProfileRequiredFied;
                            TempData["EmployeeTrainingRequiredFied"] = employeeTrainingRequiredFied.ToList();
                            TempData["EmpTraAdmin"] = DataSet1.Tables[0].AsEnumerable();
                            List<DataRow> list = DataSet1.Tables[0].AsEnumerable().ToList();
                            List<string[]> results = DataSet1.Tables[0].Select().Select(dr => dr.ItemArray.Select(x => x.ToString()).ToArray()).ToList();
                            ViewBag.EmpTraStartDate = EmpTraStartDate;
                            ViewBag.EmpTraEndDate = EmpTraEndDate;
                            ViewData["EmpTraAdmin"] = TempData["EmpTraAdmin"];
                            ViewBag.EmpTraAdmin = ViewData["EmpTraAdmin"];
                            if (DataSet1.Tables[0].Rows.Count == 0)
                            {
                                @TempData["NoRecordFound"] = true;
                            }
                            if (btnDownloadEmpTraReport != null)
                            {
                                ReportParameter[] parms = new ReportParameter[31];
                                parms[0] = new ReportParameter("CourseCode", LstEmpCourseCode);
                                parms[1] = new ReportParameter("BatchId", LstBatchIdEmpTra);
                                parms[2] = new ReportParameter("TrainerID", LstEmpTraining);
                                //if(Name=="1")
                                parms[3] = new ReportParameter("Status", EmpTrainingStatus);
                                //if (RegisteredOn == "1")
                                parms[4] = new ReportParameter("DateFrom", EmpTraStartDate);
                                //if (LastLogin == "1")
                                parms[5] = new ReportParameter("DateTo", EmpTraEndDate);
                                //if (CompanyName == "1")
                                parms[6] = new ReportParameter("Action", EmpTraAction);
                                //if (CompanyType == "1")
                                parms[7] = new ReportParameter("TrainingId", EmpTraScheDuleID);
                                //if (CompSize == "1")
                                parms[8] = new ReportParameter("TrainingTitle", EmpTraScheDuleName);
                                //if (Website == "1")
                                parms[9] = new ReportParameter("Venue", EmpTraScheVenue);
                                //if (PermanenetAddress == "1")
                                parms[10] = new ReportParameter("City", EmpTraScheCity);
                                //if (PresentAddress == "1")
                                parms[11] = new ReportParameter("State", EmpTraScheState);
                                //if (CompanyAddress == "1")
                                parms[12] = new ReportParameter("Country", EmpTraScheCountry);
                                //if (CorrespondenceAddress == "1")
                                parms[13] = new ReportParameter("TotalNoOfCandidate", EmpTraScheNoOfCandidate);
                                parms[14] = new ReportParameter("SubscriberID", userDetails.SubscriberId.Trim());
                                parms[15] = new ReportParameter("Accomodation", EmpTraAccomodation);
                                parms[16] = new ReportParameter("Attendance", EmpTraAttendance);
                                parms[17] = new ReportParameter("Email", EmpTraEmailID);
                                parms[18] = new ReportParameter("Mobile", EmpTraMobile);
                                parms[19] = new ReportParameter("AlternateEmail", EmpTraAlternateEmail);
                                parms[20] = new ReportParameter("AlternateMobile", EmpTraAlternateMobile);
                                parms[21] = new ReportParameter("Gender", EmpTraGender);
                                parms[22] = new ReportParameter("RegisID", EmpTraRegistrationId);
                                parms[23] = new ReportParameter("Degis", EmpTraDesignation);
                                parms[24] = new ReportParameter("Region", EmpTraRegion);
                                parms[25] = new ReportParameter("BranchName", EmpTraBranchName);
                                parms[26] = new ReportParameter("BranchCategory", EmpTraBranchCategory);
                                parms[27] = new ReportParameter("BranchState", EmpTraBranchState);
                                parms[28] = new ReportParameter("BranchCode", EmpTraBranchCode);
                                parms[29] = new ReportParameter("BatchDuration", EmpTraBatchDuration);
                                parms[30] = new ReportParameter("BatchTiming", EmpTraBatchTiming);
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
                                Response.AddHeader("content-disposition", "attachment; filename=EmployeeTrainings.pdf");
                                Response.BinaryWrite(bytes); // create the file
                                Response.Flush();
                            }
                            if (btnDownloadEmpTraGraphicalReport != null)
                            {
                                ReportParameter[] parms = new ReportParameter[8];
                                parms[0] = new ReportParameter("CourseCode", LstEmpCourseCode);
                                parms[1] = new ReportParameter("BatchId", LstBatchIdEmpTra);
                                parms[2] = new ReportParameter("TrainerID", LstEmpTraining);
                                //if(Name=="1")
                                parms[3] = new ReportParameter("Status", EmpTrainingStatus);
                                //if (RegisteredOn == "1")
                                parms[4] = new ReportParameter("DateFrom", EmpTraStartDate);
                                //if (LastLogin == "1")
                                parms[5] = new ReportParameter("DateTo", EmpTraEndDate);
                                //if (CompanyName == "1")
                                parms[6] = new ReportParameter("Action", EmpTraAction);
                                //if (CompanyType == "1")                                
                                parms[7] = new ReportParameter("SubscriberID", userDetails.SubscriberId.Trim());
                                rptViewer1.LocalReport.SetParameters(parms);
                                rptViewer1.LocalReport.DataSources.Add(reportDataSource);
                                rptViewer1.ProcessingMode = ProcessingMode.Local;
                                rptViewer1.SizeToReportContent = true;
                                rptViewer1.ZoomMode = ZoomMode.PageWidth;
                                rptViewer1.Width = Unit.Percentage(99);
                                rptViewer1.Height = Unit.Pixel(1000);
                                var reList = rptViewer1.LocalReport.ListRenderingExtensions();
                                string mimeType = string.Empty;
                                string encoding = string.Empty;
                                rptViewer1.LocalReport.Refresh();
                                byte[] bytes = rptViewer1.LocalReport.Render("PDF", null);
                                Response.Buffer = true;
                                Response.Clear();
                                Response.ContentType = mimeType;
                                Response.AddHeader("content-disposition", "attachment; filename=EmployeeTrainingsGraph.pdf");
                                Response.BinaryWrite(bytes); // create the file
                                Response.Flush();
                            }
                        }
                        else
                        {
                            @TempData["ClientJobOrderRequired"] = true;
                        }
                    }
                    else
                    {
                        @TempData["ClientUserProfileRequired"] = true;
                    }
                    return View();

                }
                if (ddlRolelist == "Can" && ddlTypelist == "Pro")
                {
                    if (CanProName == "true" || CanProUserId == "true" || CanProPassword == "true" || EmpTraEmailID == "true" || EmpTraMobile == "true" || EmpTraAlternateEmail == "true" ||
                        EmpTraAlternateMobile == "true" || EmpTraGender == "true" || EmpTraRegistrationId == "true" || EmpTraDesignation == "true" || EmpTraRegion == "true" ||
                        EmpTraBranchName == "true" || EmpTraBranchCategory == "true" || EmpTraBranchState == "true" || EmpTraBranchCode == "true" || chkCanProEmail == "true" || chkCanProCorporate == "true")
                    {
                        if (EmpTraAction != "")
                        {
                            ReportViewer rptViewer = new ReportViewer();
                            rptViewer.LocalReport.ReportPath = "Views/Report/CandidateProfile.rdlc"; /// Server.MapPath("Invoice1.rdlc");            
                            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.Add(new SqlParameter("@SubscriberId", userDetails.SubscriberId));
                            if (EmpTraStartDate == "" || EmpTraStartDate == string.Empty)
                                EmpTraStartDate = null;
                            cmd.Parameters.Add(new SqlParameter("@DateFrom", EmpTraStartDate));
                            if (EmpTraEndDate == "" || EmpTraEndDate == string.Empty)
                                EmpTraEndDate = null;
                            cmd.Parameters.Add(new SqlParameter("@DateTo", EmpTraEndDate));
                            cmd.Connection = thisConnection;
                            string MyDataSource1 = "USP_CandidateProfile";
                            cmd.CommandText = string.Format(MyDataSource1);
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter daN = new SqlDataAdapter(cmd);
                            System.Data.DataSet DataSet1 = new System.Data.DataSet();
                            daN.Fill(DataSet1);
                            ReportDataSource reportDataSource = new ReportDataSource();
                            reportDataSource.Name = "DataSet1";
                            reportDataSource.Value = DataSet1.Tables[0];
                            string[] CandidateProfileAdminRequiredField = { CanProName , CanProUserId , CanProPassword , EmpTraEmailID , EmpTraMobile , EmpTraAlternateEmail , EmpTraAlternateMobile , 
                            EmpTraGender , EmpTraRegistrationId , EmpTraDesignation , EmpTraRegion , EmpTraBranchName , EmpTraBranchCategory , EmpTraBranchState , EmpTraBranchCode, chkCanProEmail, chkCanProCorporate };
                            SelectList CandidateProfileRequiredField = new SelectList(CandidateProfileAdminRequiredField);
                            ViewBag.CandidateProfileRequiredField = CandidateProfileRequiredField;
                            //ViewBag.ClientProfileRequiredFields = ClientProfileRequiredFied;
                            TempData["CandidateProfileRequiredField"] = CandidateProfileRequiredField.ToList();
                            TempData["CandidateProfileAdmin"] = DataSet1.Tables[0].AsEnumerable();
                            ViewData["CandidateProfileAdmin"] = TempData["CandidateProfileAdmin"];
                            ViewBag.candidateProfileAdmin = ViewData["CandidateProfileAdmin"];
                            ViewBag.EmpTraStartDate = EmpTraStartDate;
                            ViewBag.EmpTraEndDate = EmpTraEndDate;
                            cmd.Parameters.Clear();
                            TempData["CanPro"] = DataSet1.Tables[0].AsEnumerable();
                            ViewData["CanPro"] = TempData["CanPro"];
                            ViewBag.CandidateProfile = ViewData["CanPro"];
                            if (DataSet1.Tables[0].Rows.Count == 0)
                            {
                                @TempData["NoRecordFound"] = true;
                            }
                            if (btnDownloadEmpTraReport != null)
                            {
                                ReportParameter[] parms = new ReportParameter[19];
                                parms[0] = new ReportParameter("SubscriberId", userDetails.SubscriberId);
                                parms[1] = new ReportParameter("UserName", CanProUserId);
                                parms[2] = new ReportParameter("Name", CanProName);
                                parms[3] = new ReportParameter("Email", chkCanProEmail);
                                parms[4] = new ReportParameter("PhoneNumber", EmpTraMobile);
                                parms[5] = new ReportParameter("AlternateEmail", EmpTraAlternateEmail);
                                parms[6] = new ReportParameter("AlternateContact", EmpTraAlternateMobile);
                                parms[7] = new ReportParameter("Gender", EmpTraGender);
                                parms[8] = new ReportParameter("RegistrationId", EmpTraRegistrationId);
                                parms[9] = new ReportParameter("Designation", EmpTraDesignation);
                                parms[10] = new ReportParameter("Region", EmpTraRegion);
                                parms[11] = new ReportParameter("Branch", EmpTraBranchName);
                                parms[12] = new ReportParameter("BranchCategory", EmpTraBranchCategory);
                                parms[13] = new ReportParameter("BranchState", EmpTraBranchState);
                                parms[14] = new ReportParameter("BranchCode", EmpTraBranchCode);
                                parms[15] = new ReportParameter("Password", CanProPassword);
                                parms[16] = new ReportParameter("DateFrom", EmpTraStartDate);
                                parms[17] = new ReportParameter("DateTo", EmpTraEndDate);
                                parms[18] = new ReportParameter("Corporate", chkCanProCorporate);
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
                                Response.AddHeader("content-disposition", "attachment; filename=CandidateProfile.pdf");
                                Response.BinaryWrite(bytes); // create the file
                                Response.Flush();
                                //return RedirectToAction("AdminReport", "Report");    
                                //return View(rptViewer);
                            }
                        }
                        else
                        {
                            @TempData["ClientJobOrderRequired"] = true;
                        }
                    }
                    else
                    {
                        @TempData["ClientUserProfileRequired"] = true;
                    }
                }
                if (ddlRolelist == "Can" && ddlTypelist == "Cof")
                {
                    //if( LstEmpCourseCode!="" && LstBatchIdEmpTra!="")
                    //{
                    if (CanCourseFee == "true" || CanCourseAmountPaid == "true" || CanCourseBalanceDue == "true")
                    {
                        if (EmpTraAction != "")
                        {
                            ReportViewer rptViewer = new ReportViewer();
                            rptViewer.LocalReport.ReportPath = "Views/Report/CandidateCourseFee.rdlc"; /// Server.MapPath("Invoice1.rdlc");            
                            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.Add(new SqlParameter("@SubscriberId", userDetails.SubscriberId));
                            if (EmpTraStartDate == "" || EmpTraStartDate == string.Empty)
                                EmpTraStartDate = null;
                            cmd.Parameters.Add(new SqlParameter("@DateFrom", EmpTraStartDate));
                            if (EmpTraEndDate == "" || EmpTraEndDate == string.Empty)
                                EmpTraEndDate = null;
                            cmd.Parameters.Add(new SqlParameter("@DateTo", EmpTraEndDate));
                            if (LstBatchIdEmpTra == "")
                            {
                                LstBatchIdEmpTra = null;
                            }
                            cmd.Parameters.Add(new SqlParameter("@BatchId", LstBatchIdEmpTra));
                            if (LstEmpCourseCode == "")
                            {
                                LstEmpCourseCode = null;
                            }
                            cmd.Parameters.Add(new SqlParameter("@CourseCode", LstEmpCourseCode));
                            cmd.Connection = thisConnection;
                            string MyDataSource1 = "USP_CandidateCourseFee";
                            cmd.CommandText = string.Format(MyDataSource1);
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter daN = new SqlDataAdapter(cmd);
                            System.Data.DataSet DataSet1 = new System.Data.DataSet();
                            daN.Fill(DataSet1);
                            ReportDataSource reportDataSource = new ReportDataSource();
                            reportDataSource.Name = "DataSet1";
                            reportDataSource.Value = DataSet1.Tables[0];
                            string[] CandidateCourseFeeAdminRequiredField = { CanCourseFee, CanCourseAmountPaid, CanCourseBalanceDue };
                            SelectList CandidateCourseFeeRequiredField = new SelectList(CandidateCourseFeeAdminRequiredField);
                            ViewBag.CandidateCourseFeeRequiredField = CandidateCourseFeeRequiredField;
                            //ViewBag.ClientProfileRequiredFields = ClientProfileRequiredFied;
                            TempData["CandidateCourseFeeRequiredField"] = CandidateCourseFeeRequiredField.ToList();
                            TempData["CandidateCourseFeeAdmin"] = DataSet1.Tables[0].AsEnumerable();
                            ViewData["CandidateCourseFeeAdmin"] = TempData["CandidateCourseFeeAdmin"];
                            ViewBag.candidateCourseFeeAdmin = ViewData["CandidateCourseFeeAdmin"];
                            ViewBag.EmpTraStartDate = EmpTraStartDate;
                            ViewBag.EmpTraEndDate = EmpTraEndDate;
                            cmd.Parameters.Clear();
                            TempData["CanCourFee"] = DataSet1.Tables[0].AsEnumerable();
                            ViewData["CanCourFee"] = TempData["CanCourFee"];
                            ViewBag.CourseFee = ViewData["CanCourFee"];
                            if (DataSet1.Tables[0].Rows.Count == 0)
                            {
                                @TempData["NoRecordFound"] = true;
                            }
                            if (btnDownloadEmpTraReport != null)
                            {
                                ReportParameter[] parms = new ReportParameter[8];
                                parms[0] = new ReportParameter("SubscriberId", userDetails.SubscriberId);
                                parms[1] = new ReportParameter("DateFrom", EmpTraStartDate);
                                parms[2] = new ReportParameter("DateTo", EmpTraEndDate);
                                parms[3] = new ReportParameter("CourseFee", CanCourseFee);
                                parms[4] = new ReportParameter("AmountPaid", CanCourseAmountPaid);
                                parms[5] = new ReportParameter("BalanceDue", CanCourseBalanceDue);
                                parms[6] = new ReportParameter("BatchId", LstBatchIdEmpTra);
                                parms[7] = new ReportParameter("CourseCode", LstEmpCourseCode);
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
                                Response.AddHeader("content-disposition", "attachment; filename=CandidateCourseFee.pdf");
                                Response.BinaryWrite(bytes); // create the file
                                Response.Flush();
                                //return RedirectToAction("AdminReport", "Report");    
                                //return View(rptViewer);
                            }
                        }
                        else
                        {
                            @TempData["ClientJobOrderRequired"] = true;
                        }
                    }
                    else
                    {
                        @TempData["ClientUserProfileRequired"] = true;
                    }
                    return View();
                    //}
                    //else
                    //{
                    //    @TempData["CourseBatchMendatory"] = true;
                    //}                    
                }
            }
            else
            {
                @TempData["RoleTypeRequired"] = true;
            }
            return View();
        }
        [HttpPost]
        public ActionResult GetClientJobOrders()
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            System.Collections.Generic.List<AJSolutions.Models.ClientViewModel> JOClient = new System.Collections.Generic.List<AJSolutions.Models.ClientViewModel>();
            JOClient = generic.GetSubscriberWiseClientList(userDetails.SubscriberId);
            return Json(JOClient, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetEmpDetails()
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            System.Collections.Generic.List<AJSolutions.Models.MambersView> JOClient = new System.Collections.Generic.List<AJSolutions.Models.MambersView>();
            JOClient = generic.GetEmployee(userDetails.SubscriberId);
            return Json(JOClient, JsonRequestBehavior.AllowGet);
        }
        private void PopulateClient(string SubscriberId, object[] selectedValue = null)
        {
            Generic generic = new Generic();
            var Client = generic.GetSubscriberWiseClientList(SubscriberId).ToList();
            ViewBag.JOClient = new MultiSelectList(Client, "CorporateId", "Name", selectedValue);
        }
        private void PopulateEmployeeTask(string SubscriberId)
        {
            Generic generic = new Generic();
            System.Collections.Generic.List<AJSolutions.Models.MambersView> JOClient = new System.Collections.Generic.List<AJSolutions.Models.MambersView>();
            JOClient = generic.GetEmployee(SubscriberId);
            ViewBag.JOClient = new MultiSelectList(JOClient, "UserId", "Name");
        }
        private void PopulateStatus(object[] selectedStatus = null)
        {
            var query = Global.GetStatusList();
            MultiSelectList StatusList = new MultiSelectList(query, "StatusType", "StatusType", selectedStatus);
            ViewBag.JOStatus = StatusList;
        }
        private void PopulateOrderType(string SubscriberId, object selectedOrderType = null)
        {

            var query = generic.GetJobOrderType(SubscriberId);
            SelectList OrderTypes = new SelectList(query, "JobOrderTypeId", "JobOrderType", selectedOrderType);
            ViewBag.JobOrderTypeId = OrderTypes;
        }
        private void PopulateEmployeeFaculty(string SubscriberId, object[] selectedValue = null)
        {
            Generic generic = new Generic();
            var LstEmpTraining = GetEmployeeFaculty(SubscriberId).ToList();
            ViewBag.LstEmpTraining = new MultiSelectList(LstEmpTraining, "USerID", "Name", selectedValue);
        }
        public List<EmployeeBasicDetails> GetEmployeeFaculty(string SubscriberId)
        {
            var Employee = new List<EmployeeBasicDetails>();
            using (var db = new UserDBContext())
            {
                Employee = db.Database
                          .SqlQuery<EmployeeBasicDetails>("EXEC USP_GetEmployeeFaculty @SubscriberId",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }
            return Employee;
        }
        private void PopulateEmployeeStatus(object[] selectedStatus = null)
        {
            var query = Global.GetStatusList();
            MultiSelectList EmpTrainingStatus = new MultiSelectList(query, "StatusType", "StatusType", selectedStatus);
            ViewBag.EmpTrainingStatus = EmpTrainingStatus;
        }
        private void PopulateCourseForEmpTra(string SubscriberId, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetCourseDetails(SubscriberId);
            SelectList LstEmpCourseCode = new SelectList(query, "CourseCode", "CourseName", selectedValue);
            ViewBag.LstEmpCourseCode = LstEmpCourseCode;
        }
        private void PopulateBatchByCourseEmpTra(string SubscriberId = null, string CourseCode = null, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetBatches(SubscriberId, CourseCode);
            SelectList LstBatchIdEmpTra = new SelectList(query, "BatchId", "BatchName", selectedValue);
            ViewBag.LstBatchIdEmpTra = LstBatchIdEmpTra;
        }
        [HttpGet]
        public ActionResult CandidateDetails(string SubscriberId, string Accomodation, string Attendence, string Course, int Batch, string EmailID,
            string Mobile, string alternateEmail, string alternateMobile, string Gender, string RegistrationID, string Designation, string Region, string BranchName,
            string BranchCategory, string BranchState, string BranchCode, string TrainingTitle, string TrainingId)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            ViewBag.Accomodation = Accomodation;
            ViewBag.Attendence = Attendence;
            ViewBag.Course = Course;
            ViewBag.Batch = Batch;
            ViewBag.EmailID = EmailID;
            ViewBag.Mobile = Mobile;
            ViewBag.alternateEmail = alternateEmail;
            ViewBag.alternateMobile = alternateMobile;
            ViewBag.Gender = Gender;
            ViewBag.RegistrationID = RegistrationID;
            ViewBag.Designation = Designation;
            ViewBag.Region = Region;
            ViewBag.BranchName = BranchName;
            ViewBag.BranchCategory = BranchCategory;
            ViewBag.BranchState = BranchState;
            ViewBag.BranchCode = BranchCode;
            ViewBag.TrainingTitle = TrainingTitle;
            ViewBag.TrainingId = TrainingId;
            ViewBag.candidateCheckInCheckOut = GetCandidateCheckInCheckOut(Course, Batch);
            return View();
        }
        public List<CandidateCheckInCheckOut> GetCandidateCheckInCheckOut(string CourseCode, int BatchId)
        {
            var Candidate = new List<CandidateCheckInCheckOut>();
            using (var db = new UserDBContext())
            {
                Candidate = db.Database
                          .SqlQuery<CandidateCheckInCheckOut>("EXEC USP_CandidateCheckINCheckOutDetails @CourseCode,@BatchId",
                            new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode),
                            new SqlParameter("@BatchId", BatchId)).ToList();
            }
            return Candidate;
        }
        [HttpPost]
        public ActionResult CandidateDetails(string SubscriberId, string Accomodation, string Attendence, string EmailID,
           string Mobile, string alternateEmail, string alternateMobile, string Gender, string RegistrationID, string Designation, string Region, string BranchName,
           string BranchCategory, string BranchState, string BranchCode, string TrainingTitle, string TrainingId, string btnDownloadCanAttendenceAnalyticalReport, string course, int batch, string btnDownloadCanCinCoutAnalyticalReport)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Views/Report/GraphicalTrainingWiseCandidateDetails.rdlc"; /// Server.MapPath("Invoice1.rdlc");                                                                                                            /// 
            ReportViewer rptViewer1 = new ReportViewer();
            rptViewer1.LocalReport.ReportPath = "Views/Report/GraphicalTrainingWiseCandidateAccomodationDetails.rdlc"; /// Server.MapPath("Invoice1.rdlc");           
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@CourseCode", course));
            cmd.Parameters.Add(new SqlParameter("@BatchId", batch));
            cmd.Connection = thisConnection;
            string MyDataSource1 = "USP_CandidateCheckINCheckOutDetails";
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
            if (btnDownloadCanAttendenceAnalyticalReport != null)
            {
                ReportParameter[] parms = new ReportParameter[4];
                parms[0] = new ReportParameter("CourseCode", course);
                parms[1] = new ReportParameter("BatchId", batch.ToString());
                parms[2] = new ReportParameter("TrainingId", TrainingId);
                parms[3] = new ReportParameter("TrainingName", TrainingTitle);
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
            else if (btnDownloadCanCinCoutAnalyticalReport != null)
            {
                ReportParameter[] parms = new ReportParameter[4];
                parms[0] = new ReportParameter("CourseCode", course);
                parms[1] = new ReportParameter("BatchId", batch.ToString());
                parms[2] = new ReportParameter("TrainingID", TrainingId);
                parms[3] = new ReportParameter("TrainingName", TrainingTitle);
                rptViewer1.LocalReport.SetParameters(parms);
                rptViewer1.LocalReport.DataSources.Add(reportDataSource);
                rptViewer1.ProcessingMode = ProcessingMode.Local;
                rptViewer1.SizeToReportContent = true;
                rptViewer1.ZoomMode = ZoomMode.PageWidth;
                rptViewer1.Width = Unit.Percentage(99);
                rptViewer1.Height = Unit.Pixel(1000);
                var reList = rptViewer1.LocalReport.ListRenderingExtensions();
                string mimeType = string.Empty;
                string encoding = string.Empty;
                rptViewer1.LocalReport.Refresh();
                byte[] bytes = rptViewer1.LocalReport.Render("PDF", null);
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition", "attachment; filename=CandidateCheckInCheckOut.pdf");
                Response.BinaryWrite(bytes);
                Response.Flush();
            }
            return View();
        }
        [HttpGet]
        public ActionResult CandidateDetailsDateWise(string UserId, string UserName)
        {
            string SubscriberId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(SubscriberId);
            ViewData["UserProfile"] = generic.GetUserDetail(SubscriberId);
            ViewBag.UserId = UserId;
            ViewBag.UserName = UserName;
            ViewBag.candidateAttendenceDetailsDateWise = GetCandidateAttendenceDetailsDateWise(UserId);
            return View();
        }
        public List<CandidateAttendenceDetailsDateWise> GetCandidateAttendenceDetailsDateWise(string UserId)
        {
            var CandidateAttendance = new List<CandidateAttendenceDetailsDateWise>();
            using (var db = new UserDBContext())
            {
                CandidateAttendance = db.Database
                          .SqlQuery<CandidateAttendenceDetailsDateWise>("EXEC USP_CandidateAttendenceDetailsDateWise @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }
            return CandidateAttendance;
        }

        public ActionResult Attendance()
        {
            return View();
        }

    }



}