using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.DAL;
using AJSolutions.Models;
using System.IO;
using System.Data;
using AJSolutions.Areas.Candidate.Models;
using System.Threading.Tasks;
using PagedList;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace AJSolutions.Areas.CMS.Controllers
{
    public class CandidateController : Controller
    {
        int SuccessCount = 0;
        int FailureCount = 0;
        DataTable resultedTable = new DataTable();
        DataTable CandidateResult = new DataTable();
        //BulkUploading service = new BulkUploading();
        Generic generic = new Generic();
        AdminManager admMgr = new AdminManager();
        Student student = new Student();
        TMSManager tms = new TMSManager();
        UserDBContext db = new UserDBContext();
        HMSManager hostalmgr = new HMSManager();
        EMSManager ems = new EMSManager();
        CMSManager cms = new CMSManager();
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

        // GET: CMS/Candidate
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult BulkUpload()
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            ViewData["UserProfile"] = userDetails;
            var plandetail = admMgr.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            PopulateClient(userDetails.SubscriberId, null);
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult BulkUpload(string CorporateId, HttpPostedFileBase FileUpload)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            PopulateClient(userDetails.SubscriberId, CorporateId);

            if (FileUpload != null)
            {
                //check we have a file
                if (FileUpload.ContentLength > 0)
                {
                    //Try and upload
                    try
                    {
                        if (string.IsNullOrEmpty(CorporateId))
                            CorporateId = userDetails.SubscriberId;

                        string result = ReadFileSaveInDB(FileUpload, userDetails.SubscriberId, CorporateId, UserId);
                        ViewBag.result = result;
                        ViewBag.SuccessCount = SuccessCount;
                        ViewBag.FailureCount = FailureCount;
                        if (FailureCount != 0)
                        {
                            ViewBag.Download = "Yes";
                        }
                        string filePath = Server.MapPath(Url.Content("~/Content/Result.csv"));
                        ToCSV(resultedTable, filePath);
                    }
                    catch (Exception ex)
                    {
                        //Catch errors
                        ViewData["Feedback"] = ex.Message;
                    }
                }
                else
                {
                    //Catch errors
                    ViewData["Feedback"] = "Please select a file";
                }
            }

            return View("BulkUpload", ViewData["Feedback"]);

        }

        #region "Supporting Function of Candidate Bulk upload"

        [HttpGet]
        public ActionResult DownoladSample()
        {
            string path = VirtualPathUtility.ToAbsolute("~/Content/Sample.csv");
            return File(path, "text/csv", "SampleTemplate.csv");
        }

        [HttpGet]
        public ActionResult Download()
        {
            string path = Path.Combine(Server.MapPath("~/Content/Result.csv"));
            return File(path, "text/csv", "result.csv");
        }

        public void ToCSV(DataTable dtDataTable, string filePath)
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

        public string ReadFileSaveInDB(HttpPostedFileBase myFile, string SubscriberId, string CorporateId, string UpdatedBy)
        {
            var reader = new StreamReader(myFile.InputStream, Encoding.GetEncoding(1252));
            //var reader = new StreamReader(myFile.InputStream);
            int i = 0;
            resultedTable.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            resultedTable.Columns.Add(new DataColumn("Password", Type.GetType("System.String")));
            resultedTable.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            resultedTable.Columns.Add(new DataColumn("Email", Type.GetType("System.String")));
            resultedTable.Columns.Add(new DataColumn("Mobile", Type.GetType("System.String")));
            resultedTable.Columns.Add(new DataColumn("Success", Type.GetType("System.String")));
            resultedTable.Columns.Add(new DataColumn("Error 1", Type.GetType("System.String")));
            resultedTable.Columns.Add(new DataColumn("Error 2", Type.GetType("System.String")));
            resultedTable.Columns.Add(new DataColumn("Error 3", Type.GetType("System.String")));
            resultedTable.Columns.Add(new DataColumn("Error 4", Type.GetType("System.String")));

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

                if (values.Length != 12)
                {
                    return "Failure : Number of Column doesn't match with template column";
                }

                if (i != 0)
                {
                    var result = AddUserToDB(values[1], values[2], values[3], values[4], values[5], values[6], values[7], values[8], values[9], values[10], values[11], SubscriberId, CorporateId, UpdatedBy);
                }
                i++;
            }
            return "Success";
        }

        public ActionResult AddUserToDB(string Name, string Email, string Mobile, string Gender, string EmployeeCode, string Designation, string BranchCode, string BranchName, string BranchCategory, string State, string Region, string SubscriberId, string CorporateId, string UpdatedBy)
        {
            try
            {
                bool res = false;
                if (!admMgr.GetUserExists(Email, Mobile, EmployeeCode))
                {
                    string userName = admMgr.GenerateUserName();
                    string passcode = admMgr.GeneratePassword(Name);

                    var user = new ApplicationUser { UserName = userName, Email = Email, PhoneNumber = Mobile, EmailConfirmed = true };
                    var result = UserManager.Create(user, passcode);
                    if (result.Succeeded)
                    {

                        string ModuleAccess = "SMS";
                        string RoleId = "Candidate";
                        string Department = "CAN";

                        var status = UserManager.AddToRole(user.Id, RoleId);
                        if (status.Succeeded)
                        {
                            //res = admMgr.UserRegistration(user.Id, Name, DateTime.UtcNow, ModuleAccess, Department, RoleId, SubscriberId, false, SubscriberId, DateTime.UtcNow,
                            //                             UpdatedBy, EmployeeCode, BranchName, BranchCategory, Region, BranchCode, State, CorporateId, Gender, passcode, "", "", null, "", "", null, Designation);
                            res = admMgr.UserRegistration(user.Id, Name, DateTime.UtcNow, ModuleAccess, Department, RoleId, SubscriberId, false, SubscriberId, DateTime.UtcNow,
                                                         UpdatedBy, EmployeeCode, null, BranchName, BranchCategory, Region, BranchCode, State, CorporateId, Gender, passcode, "", Designation);

                        }


                        if (res)
                            SuccessCount++;

                        Object[] data = new Object[10];
                        data[0] = userName;
                        data[1] = passcode;
                        data[2] = Name;
                        data[3] = Email;
                        data[4] = Mobile;
                        data[5] = result.Succeeded;
                        int i = 6;
                        foreach (string err in result.Errors)
                        {
                            data[i] = err;
                            i++;
                        }
                        resultedTable.Rows.Add(data);
                    }
                    else
                    {
                        FailureCount++;
                        Object[] data = new Object[10];
                        data[2] = Name;
                        data[3] = Email;
                        data[4] = Mobile;
                        data[5] = result.Succeeded;
                        int i = 6;
                        foreach (string err in result.Errors)
                        {
                            data[i] = err;
                            i++;
                        }
                        resultedTable.Rows.Add(data);

                    }

                    //if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Mobile))
                    //{
                    //    string callbackUrl = await SendEmailConfirmationTokenAsync("hello", user.Id, "Account activation", userName, Mobile, passcode, Name);
                    //}
                }
                else
                {
                    FailureCount++;
                    Object[] data = new Object[10];
                    data[2] = Name;
                    data[3] = Email;
                    data[4] = Mobile;
                    data[5] = "Failure";
                    data[6] = "Email or Phone Number or Employee Code already exists";
                    resultedTable.Rows.Add(data);
                }
            }
            catch (Exception ex)
            {
                FailureCount++;
                Object[] data = new Object[10];
                data[2] = Name;
                data[3] = Email;
                data[4] = Mobile;
                data[5] = "Failed";
                data[6] = ex.Message;
                resultedTable.Rows.Add(data);
            }
            return Json("result", JsonRequestBehavior.AllowGet);
        }

        private void PopulateClient(string SubscriberId, object selectedOrderType = null)
        {
            var query = generic.GetSubscriberWiseClientListBulkUpload(SubscriberId, false);
            SelectList OrderTypes = new SelectList(query, "CorporateId", "Name", selectedOrderType);
            ViewBag.CorporateId = OrderTypes;
        }

        #endregion

        [HttpGet]
        public ActionResult CheckInCheckOutBulkUpload(string CourseCode = null, Int64 BatchId = 0)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetails.SubscriberId;
            CourseBatchViewModel courseBatch = new CourseBatchViewModel();
            ViewData["UserProfile"] = userDetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admMgr.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            PopulateCourse(userDetails.SubscriberId, CourseCode);

            PopulateBatchByCourse(userDetails.SubscriberId, CourseCode, BatchId, false);
            //List<CandidateCourseDetailsView> Details = hostalmgr.GetBatchWiseStudent(CourseCode, BatchId);
            return View();
        }

        [HttpPost]
        public ActionResult CheckInCheckOutBulkUpload(string CourseCode, HttpPostedFileBase csvFile, Int64 SelBatch = 0)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;

            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            PopulateCourse(userDetails.SubscriberId, CourseCode);

            PopulateBatchByCourse(userDetails.SubscriberId, CourseCode, SelBatch, false);


            if (csvFile != null)
            {
                //check we have a file
                if (csvFile.ContentLength > 0)
                {
                    //Try and upload
                    try
                    {
                        string result = ReadCandidatesFileSaveInDB(csvFile, CourseCode, SelBatch);
                        ViewBag.result = result;
                        ViewBag.SuccessCount = SuccessCount;
                        ViewBag.FailureCount = FailureCount;
                        //if (FailureCount != 0)
                        //{
                        ViewBag.Download = "Yes";
                        //}
                        string filePath = Server.MapPath(Url.Content("~/Content/CInCOutResult.csv"));
                        ToCSV(CandidateResult, filePath);
                    }
                    catch (Exception ex)
                    {
                        //Catch errors
                        ViewData["Feedback"] = ex.Message;
                    }
                }
                else
                {
                    //Catch errors
                    ViewData["Feedback"] = "Please select a file";
                }
            }

            return View("CheckInCheckOutBulkUpload", ViewData["Feedback"]);

        }



        #region "Supporting fuction of candidate checkin checkout bulk upload"

        //Download candidate list for CIn & COut
        [HttpGet]
        public ActionResult DownloadCandidateList()
        {
            // GetBatchWiseCandidateToCSV(CourseCode, BatchId);        
            string path = Path.Combine(Server.MapPath("~/Content/CandidateList.csv"));
            return File(path, "text/csv", "CandidateList.csv");
        }


        public void GetBatchWiseCandidateToCSV(string CourseCode, Int64 BatchId)
        {
            DataTable candidateTable = new DataTable();

            List<CandidateCourseDetailsView> Details = hostalmgr.GetBatchWiseStudent(CourseCode, BatchId);

            if (Details != null)
            {
                if (Details.Count > 0)
                {

                    candidateTable.Columns.Add(new DataColumn("Course Code", Type.GetType("System.String")));
                    candidateTable.Columns.Add(new DataColumn("Batch Id", Type.GetType("System.Int64")));
                    candidateTable.Columns.Add(new DataColumn("Candidate Id", Type.GetType("System.String")));
                    candidateTable.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
                    candidateTable.Columns.Add(new DataColumn("CheckIn Date (MM/DD/YYYY HH:MM)", Type.GetType("System.String")));
                    candidateTable.Columns.Add(new DataColumn("CheckOut Date (MM/DD/YYYY HH:MM)", Type.GetType("System.String")));


                    foreach (CandidateCourseDetailsView candidate in Details)
                    {
                        Object[] data = new Object[6];
                        data[0] = CourseCode;
                        data[1] = BatchId;
                        data[2] = candidate.UserName;
                        data[3] = candidate.Name;
                        candidateTable.Rows.Add(data);
                    }

                    string filePath = Server.MapPath(Url.Content("~/Content/CandidateList.csv"));
                    ToCSV(candidateTable, filePath);
                }
                else
                {
                    candidateTable.Columns.Add(new DataColumn("Message", Type.GetType("System.String")));

                    Object[] data1 = new Object[1];
                    data1[0] = "No Record Found for selected Course and Batch";
                    candidateTable.Rows.Add(data1);
                    string filePath1 = Server.MapPath(Url.Content("~/Content/CandidateList.csv"));
                    ToCSV(candidateTable, filePath1);
                }
            }
            else
            {
                candidateTable.Columns.Add(new DataColumn("Message", Type.GetType("System.String")));
                Object[] data1 = new Object[1];
                data1[0] = "No Record Found for selected Course and Batch";
                candidateTable.Rows.Add(data1);
                string filePath1 = Server.MapPath(Url.Content("~/Content/CandidateList.csv"));
                ToCSV(candidateTable, filePath1);
            }
        }

        [HttpPost]
        public ActionResult GetBatch(string CourseCode)
        {
            List<SelectListItem> BatchId = new List<SelectListItem>();
            UserViewModel userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            string SubscriberId = userDetail.SubscriberId;
            if (!string.IsNullOrEmpty(CourseCode))
            {
                List<CourseBatch> Batches = (from b in db.CourseBatch
                                             join c in db.CourseMaster
                                             on b.CourseCode equals c.CourseCode
                                             where b.CourseCode == CourseCode && c.SubscriberId == SubscriberId
                                             //&& b.ToDate >= DateTime.UtcNow
                                             select b).ToList();
                Batches.ForEach(x =>
                {
                    BatchId.Add(new SelectListItem { Text = x.BatchName, Value = x.BatchId.ToString() });
                });
            }
            return Json(BatchId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetBatchWiseStudent(string CourseCode, Int64 BatchId)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);

            ViewData["UserProfile"] = userDetails;
            List<CandidateViewModel> Details = student.GetSubscriberWiseCandidateList(userDetails.SubscriberId,BatchId).ToList();
            //if (BatchId != 0)
            //{
            //    Details = Details.Where(c => c.BatchId == BatchId).ToList();
            //}
            return Json(Details, JsonRequestBehavior.AllowGet);
        }

        private void PopulateCourse(string SubscriberId, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetCourseBatches(SubscriberId);

            SelectList CourseCode = new SelectList(query.Select(c => new { c.CourseCode, c.CourseName }).Distinct(), "CourseCode", "CourseName", selectedValue);
            ViewBag.CourseCode = CourseCode;


            //var query = tms.GetCourseDetails(SubscriberId).OrderBy(c => c.CourseName).ToList();
            //SelectList CourseCode = new SelectList(query, "CourseCode", "CourseName", selectedValue);
            //ViewBag.CourseCode = CourseCode;
        }

        private void PopulateBatchByCourse(string SubscriberId = null, string CourseCode = null, object selectedValue = null, bool DateFilter = false)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetBatches(SubscriberId, CourseCode);
            if (DateFilter)
                query = query.Where(b => b.ToDate >= DateTime.UtcNow).ToList();

            SelectList BatchId = new SelectList(query, "BatchId", "BatchName", selectedValue);
            ViewBag.BatchId = BatchId;
        }

        public string ReadCandidatesFileSaveInDB(HttpPostedFileBase myFile, string Course, Int64 BatchId)
        {

            try
            {
                var reader = new StreamReader(myFile.InputStream);
                int i = 0;
                CandidateResult.Columns.Add(new DataColumn("Course Code", Type.GetType("System.String")));
                CandidateResult.Columns.Add(new DataColumn("Batch Id", Type.GetType("System.Int64")));
                CandidateResult.Columns.Add(new DataColumn("Candidate Id", Type.GetType("System.String")));
                CandidateResult.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
                CandidateResult.Columns.Add(new DataColumn("CheckIn Date", Type.GetType("System.String")));
                CandidateResult.Columns.Add(new DataColumn("CheckOut Date", Type.GetType("System.String")));
                CandidateResult.Columns.Add(new DataColumn("Success", Type.GetType("System.String")));
                CandidateResult.Columns.Add(new DataColumn("Message", Type.GetType("System.String")));


                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    if (values.Length != 6)
                    {
                        return "Failure : Number of Column doesn't match with template column";
                    }

                    if (i != 0)
                    {

                        if (!string.IsNullOrEmpty(values[4]) && BatchId == Convert.ToInt64(values[1]))
                        {
                            //DateTime checkInDate = DateTime.ParseExact(values[4], "MM/dd/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture);
                            DateTime checkInDate = Convert.ToDateTime(values[4]);

                            //DateTime? checkOutDate = null;
                            //if (!String.IsNullOrEmpty(values[5]))
                            //{
                            //    checkOutDate = DateTime.ParseExact(values[5], "MM/dd/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture);
                            //}
                            // DateTime checkOutDate = Convert.ToDateTime(values[5]);
                            DateTime? checkOutDate = null;


                            if (!string.IsNullOrEmpty(values[5]))
                                checkOutDate = Convert.ToDateTime(values[5]);

                            CInCOutToDB(Course, BatchId, values[2], values[3], checkInDate, checkOutDate);
                        }
                        else
                        {
                            //return "Failure Batch doesn,t match with the selected batch or checkin is null or empty";
                            FailureCount++;
                            Object[] data = new Object[8];
                            data[0] = Course;
                            data[1] = BatchId;
                            data[2] = values[2];
                            data[3] = values[3];
                            data[4] = values[4];
                            data[5] = values[5];
                            data[6] = "Failure";
                            data[7] = "Failed because batch doesn,t match with the selected batch or checkin date is null or empty";
                            CandidateResult.Rows.Add(data);
                        }
                    }
                    i++;
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public void CInCOutToDB(string Course, Int64 BatchId, string UserName, string Name, DateTime CInDate, DateTime? COutDate)
        {
            try
            {
                string res;

                var Users = UserManager.FindByName(UserName);
                res = hostalmgr.CInCOutInBulk(BatchId, Users.Id, CInDate, COutDate, User.Identity.GetUserId());

                string[] Result = res.Split(':');
                if (Result[0] == "Succeed")
                {
                    SuccessCount++;
                }
                else
                {
                    FailureCount++;
                }
                Object[] data = new Object[8];
                data[0] = Course;
                data[1] = BatchId;
                data[2] = UserName;
                data[3] = Name;
                data[4] = CInDate;
                data[5] = COutDate;
                data[6] = Result[0];
                data[7] = Result[1];
                CandidateResult.Rows.Add(data);

            }
            catch (Exception ex)
            {
                FailureCount++;
                Object[] data = new Object[8];
                data[0] = Course;
                data[1] = BatchId;
                data[2] = UserName;
                data[3] = Name;
                data[4] = CInDate;
                data[5] = COutDate;
                data[6] = "Failed";
                data[7] = ex.Message;
                CandidateResult.Rows.Add(data);
            }

        }

        [HttpGet]
        public ActionResult CheckInCheckOutDownload()
        {
            string path = Path.Combine(Server.MapPath("~/Content/CInCOutResult.csv"));
            return File(path, "text/csv", "result.csv");
        }
        #endregion

        [HttpGet]
        public ActionResult DownloadCandidateLeads()
        {
            string path = VirtualPathUtility.ToAbsolute("~/Content/CandidateLeadSample.csv");
            return File(path, "text/csv", "SampleTemplate.csv");
        }

        [HttpGet]
        public ActionResult CandidateLeadResultDownload()
        {
            string path = Path.Combine(Server.MapPath("~/Content/CandidateLeadResult.csv"));
            return File(path, "text/csv", "result.csv");
        }

        [HttpGet]
        public ActionResult CandidateLeadUpload()
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admMgr.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            return View();
        }


        [HttpPost]
        public ActionResult CandidateLeadUpload(HttpPostedFileBase FileUpload, string UserId, string UpdatedBy, DateTime? UpdatedOn, DateTime? SubmittedOn, string SubmittedBy, string Comments, string SubcsriberId, Int16 Status = 0)
        {
            string Id = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(Id);
            ViewData["UserProfile"] = userDetails;

            string ReferenceId = userDetails.ReferenceId;

            if (ReferenceId == null)
                ReferenceId = userDetails.UserId;
            if (FileUpload != null)
            {
                //check we have a file
                if (FileUpload.ContentLength > 0)
                {
                    //Try and upload
                    try
                    {

                        string result = ReadCandidateLeadFileSaveInDB(FileUpload, UserId, Id, DateTime.UtcNow, DateTime.UtcNow, Id, Comments, userDetails.SubscriberId, ReferenceId, Status);
                        ViewBag.result = result;
                        ViewBag.SuccessCount = SuccessCount;
                        ViewBag.FailureCount = FailureCount;
                        if (FailureCount != 0)
                        {
                            ViewBag.Download = "Yes";
                        }
                        string filePath = Server.MapPath(Url.Content("~/Content/CandidateLeadResult.csv"));
                        ToCSV(CandidateResult, filePath);
                    }
                    catch (Exception ex)
                    {
                        //Catch errors
                        ViewData["Feedback"] = ex.Message;
                    }
                }
                else
                {
                    //Catch errors
                    ViewData["Feedback"] = "Please select a file";
                }
            }

            return View("CandidateLeadUpload", ViewData["Feedback"]);

        }

        public string ReadCandidateLeadFileSaveInDB(HttpPostedFileBase myFile, string UserId, string UpdatedBy, DateTime? UpdatedOn, DateTime? SubmittedOn, string SubmittedBy, string Comments, string SubscriberId, string ReferenceId, short Status = 0, Int64 LeadId = 0)
        {
            var reader = new StreamReader(myFile.InputStream, Encoding.GetEncoding(1252));
            //var reader = new StreamReader(myFile.InputStream);
            int i = 0;
            CandidateResult.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            CandidateResult.Columns.Add(new DataColumn("Email Id", Type.GetType("System.String")));
            CandidateResult.Columns.Add(new DataColumn("Phone Number", Type.GetType("System.String")));
            CandidateResult.Columns.Add(new DataColumn("Success", Type.GetType("System.String")));
            CandidateResult.Columns.Add(new DataColumn("Message", Type.GetType("System.String")));

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


                if (values.Length != 3)
                {
                    return "Failure : Number of Column doesn't match with template column";
                }

                if (i != 0)
                {
                    AddCandidateLeadToDB(LeadId, values[0], values[1], values[2], UserId, Status, Comments, DateTime.UtcNow, UpdatedBy, DateTime.UtcNow, SubmittedBy, SubscriberId, ReferenceId);
                }
                i++;
            }
            return "Success";
        }


        public void AddCandidateLeadToDB(Int64 LeadId, string Name, string EmailId, string PhoneNumber, string UserId, Int16 Status, string Comments, DateTime? UpdatedOn, string UpdatedBy, DateTime? SubmittedOn, string SubmittedBy, string SubscriberId, string ReferenceId)
        {
            try
            {
                string res;
                if (!admMgr.GetUserExists(EmailId, PhoneNumber))
                {
                    //var Users = UserManager.FindByName(UserName);
                    res = cms.AddCandidateLeads(LeadId, Name, EmailId, PhoneNumber, UserId, Status, Comments, DateTime.UtcNow, UpdatedBy, DateTime.UtcNow, SubmittedBy, SubscriberId, ReferenceId);

                    string[] Result = res.Split(':');
                    if (Result[0] == "Succeed")
                    {
                        SuccessCount++;
                    }
                    else
                    {
                        FailureCount++;
                    }
                    Object[] data = new Object[5];
                    data[0] = Name;
                    data[1] = EmailId;
                    data[2] = PhoneNumber;
                    data[3] = Result[0];
                    data[4] = Result[1];
                    CandidateResult.Rows.Add(data);

                }
                else
                {
                    FailureCount++;
                    Object[] data = new Object[5];
                    data[0] = Name;
                    data[1] = EmailId;
                    data[2] = PhoneNumber;
                    data[3] = "Failure";
                    data[4] = "Email or Phone Number already exists";
                    CandidateResult.Rows.Add(data);
                }
            }
            catch (Exception ex)
            {
                FailureCount++;
                Object[] data = new Object[5];
                data[0] = Name;
                data[1] = EmailId;
                data[2] = PhoneNumber;
                data[3] = "Failed";
                data[4] = ex.Message;
                CandidateResult.Rows.Add(data);
            }

        }

        public ActionResult CandidateLeads(string sortOrder, string UserName, string Name, string EmailId, string PhoneNumber, DateTime? SubmittedOn, string Source, string AllCategory, string Gender, int? page, Int16 Status = 0, int PageSize = 10)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admMgr.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            var GenderList = Global.GetGenderList();
            ViewBag.Gender = new SelectList(GenderList, "Genderid", "Gender");
            //ViewBag.Gender = Gender;

            var CategoryList = Global.GETAllCategory();
            ViewBag.AllCategory = new SelectList(CategoryList, "Category", "Category");
            //ViewBag.AllCategory = AllCategory;

            var Leads = cms.GetCandidateLeads(UserId).ToList();
            var myleads = db.CandidateLeads.Where(c => c.ReferenceId == UserId).ToList();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.UserNameSortParam = sortOrder == "UserName" ? "UserName_desc" : "UserName";
            ViewBag.NameSortParam = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.EmailSortParam = sortOrder == "EmailId" ? "EmailId_desc" : "EmailId";
            ViewBag.PhoneNumberSortParam = sortOrder == "PhoneNumber" ? "PhoneNumber_desc" : "PhoneNumber";
            ViewBag.SubmittedOnSortParam = sortOrder == "SubmittedOn" ? "SubmittedOn_desc" : "SubmittedOn";
            ViewBag.StatusSortParam = sortOrder == "Status" ? "Status_desc" : "Status";
            ViewBag.SourceSortParam = sortOrder == "Source" ? "Source_desc" : "Source";

            ViewBag.PhoneNumber = PhoneNumber;
            ViewBag.Name = Name;
            var Category = AllCategory;
            ViewBag.SelectedGender = Gender;
            ViewBag.EmailId = EmailId;
            ViewBag.Page = page;

            //Apply filter
            if (!String.IsNullOrEmpty(EmailId))
            {
                Leads = Leads.Where(s => (!String.IsNullOrEmpty(s.EmailId) && s.EmailId.Contains(EmailId))).ToList();
            }
            if (!String.IsNullOrEmpty(Name))
            {
                Leads = Leads.Where(s => s.Name.ToLower().Contains(Name.ToLower())).ToList();
            }

            if (!String.IsNullOrEmpty(Gender))
            {
                Leads = Leads.Where(s => (!String.IsNullOrEmpty(s.Gender) && s.Gender.Contains(Gender))).ToList();
            }

            if (!String.IsNullOrEmpty(Category))
            {
                Leads = Leads.Where(s => (!String.IsNullOrEmpty(s.Category) && s.Category.Contains(Category))).ToList();
            }
            if (!String.IsNullOrEmpty(UserName))
            {
                Leads = Leads.Where(s => s.UserName.ToUpper().Contains(UserName.ToUpper())).ToList();
            }
            if (!String.IsNullOrEmpty(PhoneNumber))
            {
                Leads = Leads.Where(s => s.PhoneNumber.ToUpper().Contains(PhoneNumber.ToUpper())).ToList();
            }

            //Apply sorting
            if (Leads.Count != 0)
            {
                switch (sortOrder)
                {
                    case "UserName":
                        Leads = Leads.OrderBy(c => c.UserName).ToList();
                        break;
                    case "UserName_desc":
                        Leads = Leads.OrderBy(c => c.UserName).ToList();
                        break;
                    case "Name":
                        Leads = Leads.OrderBy(c => c.Name).ToList();
                        break;
                    case "Name_desc":
                        Leads = Leads.OrderByDescending(c => c.Name).ToList();
                        break;
                    case "EmailId":
                        Leads = Leads.OrderBy(c => c.EmailId).ToList();
                        break;
                    case "EmailId_desc":
                        Leads = Leads.OrderByDescending(c => c.EmailId).ToList();
                        break;
                    case "Phone":
                        Leads = Leads.OrderBy(c => c.PhoneNumber).ToList();
                        break;
                    case "Phone_desc":
                        Leads = Leads.OrderByDescending(c => c.PhoneNumber).ToList();
                        break;
                    case "SubmittedOn":
                        Leads = Leads.OrderBy(c => c.SubmittedOn).ToList();
                        break;
                    case "SubmittedOn_desc":
                        Leads = Leads.OrderByDescending(c => c.SubmittedOn).ToList();
                        break;
                    case "Status":
                        Leads = Leads.OrderBy(c => c.Status).ToList();
                        break;
                    case "Status_desc":
                        Leads = Leads.OrderByDescending(c => c.Status).ToList();
                        break;
                    case "Source":
                        Leads = Leads.OrderBy(c => c.SubmittedByName).ToList();
                        break;
                    case "Source_desc":
                        Leads = Leads.OrderByDescending(c => c.SubmittedByName).ToList();
                        break;
                    default:
                        Leads = Leads.OrderBy(c => c.Name).ToList();
                        break;
                }
            }
            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(Leads.ToPagedList(pageNumber, pageSize));

        }

        //[Authorize(Roles = "Admin")]
        public ActionResult ApproveCandidate(string Id, string savestatus, string UserAction, Int64 Lead = 0)
        {
            ViewBag.Id = Id;
            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetails.SubscriberId;
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            ViewBag.status = savestatus;
            var Leads = cms.GetCandidateLeadsToApprove(Lead);
            //ViewData["Leads"] = Leads;
            List<CorporateProfile> corporateProfiles = db.CorporateProfile.Where(p => p.SubscriberId == userDetails.SubscriberId).ToList().FindAll(p => p.DepartmentId == "CLI" || (p.DepartmentId == "ADI" && p.CorporateId == userDetails.SubscriberId));
            PopulateGenderStatus();
            if (Leads != null)
            {
                ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => cp.Name), "CorporateId", "Name", Leads.SubmittedBy);
                //ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => Leads.SubmittedBy), "CorporateId", "Name");
            }
            else
            {
                ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => cp.Name), "CorporateId", "Name");
            }
            return View(Leads);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> ApproveCandidate(string UserId, string Name, string Emailid, string PhoneNumber, string DepartmentId, string EmployeeCode, string BranchName,
                                            string Mod, string Name1, string AlternateContact, string AlternateEmail, string Nationality, string RegistrationId, string SubmittedBy,
                                             DateTime? UpdatedOn, string UpdatedBy, string Branch, string BranchCategory, string Designation, string CorporateId, string Source,
                                            string State, string passcode, string Region, string BranchCode, string BranchState, string ClientId, string Gender, string Comments, Int16 Status = 0, Int64 LeadId = 0)
        {
            string savestatus = "";

            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());

            string SubscriberId = userDetails.SubscriberId;

            var subscriberDetail = cms.GetCorporateProfile(SubscriberId).FirstOrDefault();

            if (String.IsNullOrEmpty(UserId))
            {
                //User Add Mode
                string userName = admMgr.GenerateUserName();
                var user = new ApplicationUser { UserName = userName, Email = Emailid, PhoneNumber = PhoneNumber, EmailConfirmed = true };

                var result = await UserManager.CreateAsync(user, "changeme");
                if (result.Succeeded)
                {

                    string ModuleAccess = "SMS";
                    string RoleId = "Candidate";
                    string Department = "CAN";

                    var status = UserManager.AddToRole(user.Id, RoleId);
                    var Leads = cms.GetCandidateLeadsToApprove(LeadId);
                    if (status.Succeeded)
                    {
                        UpdatedBy = User.Identity.GetUserId();
                        savestatus = "Succeeded";
                        admMgr.UserRegistration(user.Id, Name, DateTime.UtcNow, ModuleAccess, Department, RoleId, SubscriberId, false, SubscriberId, DateTime.UtcNow, UpdatedBy, EmployeeCode, null, Designation, BranchName, BranchCategory, Region, BranchCode, State, CorporateId, "", passcode, Leads.SubmittedBy);
                    }
                    if (user.Id != null)
                    {
                        Status = 1;
                        cms.AddCandidateLeads(LeadId, Leads.Name, Leads.EmailId, Leads.PhoneNumber, user.Id, Status, Leads.Comments, DateTime.UtcNow, userDetails.UserId, Leads.SubmittedOn, Leads.SubmittedBy, Leads.SubscriberId, Leads.ReferenceId);
                    }
                    if (RoleId.ToUpper() == "CANDIDATE")
                    {
                        var res = cms.UpdatePassword(user.Id, "changeme");
                    }
                    string callbackUrl = await SendEmailConfirmationTokenAsync(subscriberDetail.Name, user.Id, "Account activation", userName, PhoneNumber, Name);
                }
            }
            return RedirectToAction("CandidateLeads", "Candidate", new { area = "CMS", Id = Mod, savestatus });
        }

        [HttpPost]
        public async Task<ActionResult> SendEmailWithNewPwd(string UserId, string Email, string newPwd, string Name)
        {
            var msgBody = "Dear USER (" + Name + "), <br/> <br/> Greetings from RECKONN! <br/> <br/> Your password has been reset. Your new password is : " + newPwd + "<br/> <br/> " +
                           "Please login with your new password. <br><br>RECKONN ";
            await UserManager.SendEmailAsync(UserId, "Reset Password", msgBody);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RejectCandidate(Int16 Status = 0, Int64 LeadId = 0)
        {
            string id = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            Status = 2;
            var Leads = cms.GetCandidateLeadsToApprove(LeadId);
            var result = cms.AddCandidateLeads(LeadId, Leads.Name, Leads.EmailId, Leads.PhoneNumber, Leads.UserId, Status, Leads.Comments, DateTime.UtcNow, id, Leads.SubmittedOn, Leads.SubmittedBy, Leads.SubscriberId, Leads.ReferenceId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string SubScriber, string userID, string subject, string userName, string phoneNumber, string passcode, string Name = "User")
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { area = "", userId = userID, code = code }, protocol: Request.Url.Scheme);
            var msgBody = "Dear " + Name + " <br/> <br/>" + SubScriber +
                " has added you as their USER in RECKONN. Your User Name is " + userName + " and Phone Number is " + phoneNumber + "." +
                "<br><br> <a href='" + callbackUrl + "' > CLICK HERE</a> to Verify your email." +
                "<br/><br/>You can login to your account using the password" + passcode + " ." +
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

        //By:  Ajay Kumar Choudhary
        //On:  19/07/2017
        //For: Adding Candidate leads Details
        //Start
        [HttpGet]
        public ActionResult LeadDetails(string UserAction, DateTime? DOB, bool data = false, Int64 LeadId = 0)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            var plandetail = admMgr.GetUserplanDetails(UserDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.Result = "Failed";
            if (data == true)
            {
                ViewBag.Result = "Succeeded";
            }
            if (UserAction == "Delete" && LeadId > 0)
            {
                cms.DeleteCandidateLeads(LeadId);
                PopulateMaritalStatus();
                PopulateGenderStatus();
                PopulateCategoryLists();
                PopulatePoverty();
                PopulateQualification();
                PopulateCountry();
                PopulateState();
                PopulateCity();
                PopulateIdentification();
                ViewBag.Data = "Success";
                return RedirectToAction("CandidateLeads", "Candidate", new { area = "CMS" });
            }
            var Leads = cms.GetCandidateLeadsToApprove(LeadId);
            if (Leads != null)
            {
                PopulateMaritalStatus(Leads.MaritalStatus);
                PopulateGenderStatus(Leads.Gender);
                PopulateCategoryLists(Leads.Category);
                PopulatePoverty(Leads.BelowPoverty);
                PopulateQualification(Leads.Qualification);
                PopulateIdentification(Leads.IdName);
                if (Leads.DOB != null)
                    ViewBag.DOB = Leads.DOB.Value.ToString("dd-MM-yyyy");
            }
            else
            {
                PopulateMaritalStatus();
                PopulateGenderStatus();
                PopulateCategoryLists();
                PopulatePoverty();
                PopulateQualification();
                PopulateCountry();
                PopulateState();
                PopulateCity();
                PopulateIdentification();
            }
            return View(Leads);
        }

        [HttpPost]
        public ActionResult LeadDetails(string UserId, string Name, string PhoneNumber, string EmailId, string FatherName, string FatherOccupation, string FamilyIncome, string BelowPoverty, string MotherName,
                                        string Gender, string DOBirth, string AllCategory, string Religion, string MaritalStatus, string Qualifications, string IdName, string MotherOccupation,
                                         string Comments, DateTime? UpdatedOn, string UpdatedBy, DateTime? SubmittedOn, string SubmittedBy, string MediumOfEducation, string IdNumber,
                                        string SubscriberId, bool DifferentlyAbled = false, bool Relocate = false, Int64 LeadId = 0, Int16 Status = 0, Int16 RelocateId = 0)
        {
            string Id = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(Id);

            DateTime? DOB = null;
            if (!string.IsNullOrEmpty(DOBirth))
            {
                DOB = DateTime.ParseExact(DOBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }



            string ReferenceId = UserDetail.ReferenceId;

            if (ReferenceId == null)
                ReferenceId = UserDetail.UserId;

            var result = cms.AddCandidateLeadsDetails(LeadId, UserId, Name, PhoneNumber, EmailId, FatherName, FatherOccupation, MotherName, MotherOccupation
                                            , IdName, IdNumber, Gender, DOB, AllCategory, Religion, DifferentlyAbled, MaritalStatus, MediumOfEducation
                                            , Relocate, BelowPoverty, FamilyIncome, Qualifications, Status, Comments,
                                            DateTime.UtcNow, UserDetail.UserId, DateTime.UtcNow, UserDetail.UserId, ReferenceId, UserDetail.SubscriberId, RelocateId);

            return RedirectToAction("LeadDetails", "Candidate", new { Area = "CMS" });
        }

        [HttpGet]
        public ActionResult CandidateLeadDetails(long LeadId = 0)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            var plandetail = admMgr.GetUserplanDetails(UserDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            var Details = cms.GetCandidateLeadsToApprove(LeadId);
            return View(Details);
        }

        //By:  Ajay Kumar Choudhary
        //On:  19/07/2017
        //For: Adding Candidate leads Details
        //Start
        [HttpGet]
        public ActionResult DduGkyLeads(string UserAction, DateTime? DOB, bool data = false, long LeadId = 0)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            ViewBag.Result = "Failed";
            if (data == true)
            {
                ViewBag.Result = "Succeeded";
            }
            if (UserAction == "Delete" && LeadId > 0)
            {
                cms.DeleteCandidateLeads(LeadId);
                PopulateMaritalStatus();
                PopulateGenderStatus();
                PopulateCategoryLists();
                PopulatePoverty();
                PopulateQualification();
                PopulateCountry();
                PopulateState();
                PopulateCity();
                PopulateIdentification();
                PopulateReligion();
                PopulateRelocate();
                ViewBag.Data = "Success";
                return RedirectToAction("CandidateLeads", "Candidate", new { area = "CMS" });
            }
            var Leads = cms.GetCandidateLeadsToApprove(LeadId);
            if (Leads != null)
            {
                PopulateMaritalStatus(Leads.MaritalStatus);
                PopulateGenderStatus(Leads.Gender);
                PopulateCategoryLists(Leads.Category);
                PopulatePoverty(Leads.BelowPoverty);
                PopulateQualification(Leads.Qualification);
                PopulateIdentification(Leads.IdName);
                PopulateReligion(Leads.Religion);
                PopulateRelocate(Leads.RelocateId);
                if (Leads.DOB != null)
                    ViewBag.DOB = Leads.DOB.Value.ToString("dd-MM-yyyy");
            }
            else
            {
                PopulateMaritalStatus();
                PopulateGenderStatus();
                PopulateCategoryLists();
                PopulatePoverty();
                PopulateQualification();
                PopulateCountry();
                PopulateState();
                PopulateCity();
                PopulateReligion();
                PopulateIdentification();
                PopulateRelocate();
            }
            return View(Leads);
        }

        [HttpPost]
        public ActionResult DduGkyLeads(string UserId, string Name, string PhoneNumber, string EmailId, string FatherName, string FatherOccupation, string FamilyIncome, string BelowPoverty, string MotherName,
                                        string Gender, string DOBirth, string AllCategory, string Religion, string MaritalStatus, string Qualifications, string IdName, string MotherOccupation,
                                         string Comments, DateTime? UpdatedOn, string UpdatedBy, DateTime? SubmittedOn, string SubmittedBy, string MediumOfEducation, string IdNumber,
                                        string SubscriberId, bool DifferentlyAbled = false, bool Relocate = false, long LeadId = 0, Int16 Status = 0, Int16 RelocateId = 0)
        {
            string Id = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(Id);

            DateTime? DOB = null;
            if (!string.IsNullOrEmpty(DOBirth))
            {
                DOB = DateTime.ParseExact(DOBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }


            string ReferenceId = UserDetail.ReferenceId;

            if (ReferenceId == null)
                ReferenceId = UserDetail.UserId;

            var result = cms.AddCandidateLeadsDetails(LeadId, UserId, Name, PhoneNumber, EmailId, FatherName, FatherOccupation, MotherName, MotherOccupation
                                            , IdName, IdNumber, Gender, DOB, AllCategory, Religion, DifferentlyAbled, MaritalStatus, MediumOfEducation
                                            , Relocate, BelowPoverty, FamilyIncome, Qualifications, Status, Comments, DateTime.UtcNow, UserDetail.UserId, DateTime.UtcNow, UserDetail.UserId, ReferenceId, UserDetail.SubscriberId, RelocateId);

            return RedirectToAction("LeadDetails", "Candidate", new { Area = "CMS" });
        }

        //END

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


        private void PopulateGenderStatus(object selectedValue = null)
        {
            var GenderList = Global.GetGenderList();
            SelectList Gender = new SelectList(GenderList, "Genderid", "Gender", selectedValue);
            ViewBag.Gender = Gender;
        }

        private void PopulatePaging(object selectedValue = null)
        {
            var PageList = generic.GetPaging();
            ViewBag.PageSize = new SelectList(PageList, "PageSize", "PageSize", selectedValue);
        }

        private void PopulateMaritalStatus(object selectedValue = null)
        {
            var MaritalList = Global.GetMaritalList();
            SelectList MaritalStatus = new SelectList(MaritalList, "MaritalStatus", "MaritalStatus", selectedValue);
            ViewBag.MaritalStatus = MaritalStatus;
        }

        private void PopulateCategoryLists(object selectedValue = null)
        {
            var CategoryList = Global.GETAllCategory();
            SelectList AllCategory = new SelectList(CategoryList, "Category", "Category", selectedValue);
            ViewBag.AllCategory = AllCategory;
        }

        private void PopulateIdentification(object selectedValue = null)
        {
            var IdentificationList = Global.GETAllIdentification();
            SelectList AllIdentification = new SelectList(IdentificationList, "Identification", "Identification", selectedValue);
            ViewBag.AllIdentification = AllIdentification;
        }


        private void PopulateQualification(object selectedValue = null)
        {
            var QualificationList = Global.GetQualification();
            SelectList Qualifications = new SelectList(QualificationList, "Qualification", "Qualification", selectedValue);
            ViewBag.Qualifications = Qualifications;
        }

        private void PopulatePoverty(object selectedValue = null)
        {
            var OptionList = Global.GetOption();
            SelectList option = new SelectList(OptionList, "options", "options", selectedValue);
            ViewBag.option = option;
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

        private void PopulateReligion(object selectedValue = null)
        {
            var ReligionList = generic.GetReligions();
            SelectList Religion = new SelectList(ReligionList, "Religion", "Religion", selectedValue);
            ViewBag.Religion = Religion;
        }

        private void PopulateRelocate(object selectedValue = null)
        {
            var RelocateList = generic.GetRelocateList();
            SelectList Relocate = new SelectList(RelocateList, "RelocateId", "Relocate", selectedValue);
            ViewBag.Relocate = Relocate;
        }
    }
}