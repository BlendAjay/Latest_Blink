using AJSolutions.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.Models;
using System.Threading.Tasks;
using PagedList;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Configuration;

namespace AJSolutions.Areas.CMS.Controllers
{
    public class CalendarController : Controller
    {
        TMSManager tms = new TMSManager();
        CMSManager cmsMgr = new CMSManager();
        EMSManager emsMgr = new EMSManager();
        Generic generic = new Generic();
        AdminManager admin = new AdminManager();
        DataTable dt = new DataTable();
        UserDBContext db = new UserDBContext();
        BlobManager blobManager = new BlobManager();
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

        // GET: CMS/Calendar
        [HttpGet]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult Index(bool status = false)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            List<TrainerPlannerView> trainerPlanner = tms.GetTrainerPlaner(userdetails.UserId);
            ViewData["Planner"] = trainerPlanner.AsEnumerable();

            return View();
        }

        /// <summary>
        ///Created By : Ajay kumar Choudhary
        ///Created On : 22-05-2017
        ///For Creating Holiday Calendar
        /// </summary>
        [HttpGet]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult HolidayCalendar(bool status = false)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            List<Holiday> holidays = tms.GetHolidays(userdetails.SubscriberId);
            ViewData["Holidays"] = holidays.AsEnumerable();

            return View();
        }

        //[HttpPost]
        ////[Authorize(Roles="Admin,Employee")]
        //public ActionResult Index()
        //{

        //    return View();
        //}

        // GET: CMS/Calendar
        [HttpGet]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult Trainers()
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            if (userdetails.Role == "Employee")
            {
                List<TrainerPlannerView> trainerPlanner = tms.GetTrainerPlanerView(userdetails.UserId, "Admin");
                ViewData["Planner"] = trainerPlanner.Where(t => t.TrainerId != userdetails.UserId).AsEnumerable();
            }
            else
            {
                List<TrainerPlannerView> trainerPlanner = tms.GetTrainerPlanerView(userdetails.SubscriberId, "Admin");
                ViewData["Planner"] = trainerPlanner.Where(t => t.TrainerId != userdetails.UserId).AsEnumerable();
            }
            return View();
        }
        [HttpPost]
        //[Authorize(Roles = "Admin,Employee")]
        public async Task<ActionResult> Trainers(string Leave, string PlannerId = "0", string IsApproved = "0")
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            //var subid = generic.GetSubscriberId(User.Identity.GetUserId());
            var result = tms.EngagementApprovalStatus(Convert.ToInt64(IsApproved), userdetails.UserId, DateTime.Now, Convert.ToInt64(PlannerId));
            if (result == true)
            {
                var Planner = tms.GetTrainerPlannerforPlannerId(Convert.ToInt64(PlannerId));
                if (IsApproved == "1")
                    Leave = "Approved";

                else
                    Leave = "DisApproved";


                string callbackUrl = await SendEmailConfirmationTokenAsync(Planner.TrainerId, Planner.EmployeeName, Leave, Planner.EngagementType, userdetails.Name);
            }
            return RedirectToAction("Trainers", "Calendar", new { area = "CMS" });

        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult EmployeeEngagement(string sortOrder, string Name, string ApprovedBy, DateTime? ApprovalDate, DateTime? ToDate, DateTime? FromDate, string EngagementType, int? page, Int64 IsApproved = 0, Int16 Status = 0, int PageSize = 10)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            
            List<TrainerPlannerView> trainerPlanner = tms.GetTrainerPlanerView(userdetails.SubscriberId, "Admin");
            var Planner = trainerPlanner.Where(t => t.TrainerId != userdetails.UserId).AsEnumerable();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.ApprovedBySortParam = sortOrder == "ApprovedBy" ? "ApprovedBy_desc" : "ApprovedBy";
            ViewBag.ApprovalDateSortParam = sortOrder == "ApprovalDate" ? "ApprovalDate_desc" : "ApprovalDate";
            ViewBag.ToDateSortParam = sortOrder == "ToDate" ? "ToDate_desc" : "ToDate";
            ViewBag.FromDateSortParam = sortOrder == "FromDate" ? "FromDate_desc" : "FromDate";
            ViewBag.EngagementTypeSortParam = sortOrder == "EngagementType" ? "EngagementType_desc" : "EngagementType";
            ViewBag.IsApprovedSortParam = sortOrder == "IsApproved" ? "IsApproved_desc" : "IsApproved";

            ViewBag.Name = Name;
            ViewBag.ApprovedBy = ApprovedBy;
            ViewBag.ApprovalDate = ApprovalDate;
            ViewBag.ToDate = ToDate;
            ViewBag.FromDate = FromDate;
            ViewBag.EngagementType = EngagementType;
            ViewBag.IsApproved = IsApproved;
            ViewBag.Page = page;

            //Apply filter
            if (!String.IsNullOrEmpty(Name))
            {
                Planner = Planner.Where(s => s.Name.ToLower().Contains(Name.ToLower())).ToList();
            }

            //Apply sorting
            if (Planner.Count() != 0)
            {
                switch (sortOrder)
                {
                    case "Name":
                        Planner = Planner.OrderBy(c => c.Name).ToList();
                        break;
                    case "Name_desc":
                        Planner = Planner.OrderByDescending(c => c.Name).ToList();
                        break;
                    case "ApprovedBy":
                        Planner = Planner.OrderBy(c => c.ApprovedBy).ToList();
                        break;
                    case "ApprovedBy_desc":
                        Planner = Planner.OrderByDescending(c => c.ApprovedBy).ToList();
                        break;
                    case "ApprovalDate":
                        Planner = Planner.OrderBy(c => c.ApprovalDate).ToList();
                        break;
                    case "ApprovalDate_desc":
                        Planner = Planner.OrderByDescending(c => c.ApprovalDate).ToList();
                        break;
                    case "ToDate":
                        Planner = Planner.OrderBy(c => c.ToDate).ToList();
                        break;
                    case "ToDate_desc":
                        Planner = Planner.OrderByDescending(c => c.ToDate).ToList();
                        break;
                    case "FromDate":
                        Planner = Planner.OrderBy(c => c.FromDate).ToList();
                        break;
                    case "FromDate_desc":
                        Planner = Planner.OrderByDescending(c => c.FromDate).ToList();
                        break;
                    case "EngagementType":
                        Planner = Planner.OrderBy(c => c.EngagementType).ToList();
                        break;
                    case "EngagementType_desc":
                        Planner = Planner.OrderByDescending(c => c.EngagementType).ToList();
                        break;
                    case "IsApproved":
                        Planner = Planner.OrderBy(c => c.IsApproved).ToList();
                        break;
                    case "IsApproved_desc":
                        Planner = Planner.OrderByDescending(c => c.IsApproved).ToList();
                        break;
                    default:
                        Planner = Planner.OrderBy(c => c.Name).ToList();
                        break;
                }
            }
            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(Planner.ToPagedList(pageNumber, pageSize));
        }

        //Get fro birthday
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult BirthDay()
        {

            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            List<EmployeeBasicDetails> employeebasicdetails = emsMgr.GetBirthday(userdetails.SubscriberId);
            ViewData["Birthday"] = employeebasicdetails.AsEnumerable();
            return View();
        }

        private string GetUserRole(string UserId)
        {
            if (!String.IsNullOrEmpty(UserId))
            {
                if (UserManager.IsInRole(UserId, "Admin"))
                    return "Admin";
                else if (UserManager.IsInRole(UserId, "Employee"))
                    return "Employee";
            }
            return string.Empty;
        }

        [HttpPost]
        public async Task<ActionResult> ResendToken(string UserId, string EmployeeName, string Leave, string EngagementType, string ApprovedBy)
        {
            string callbackUrl = await SendEmailConfirmationTokenAsync(UserId, EmployeeName, Leave, EngagementType, ApprovedBy);
            return Json(callbackUrl, JsonRequestBehavior.AllowGet);
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string UserId, string EmployeeName, string Leave, string EngagementType, string ApprovedBy)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(UserId);
            var callbackUrl = Url.Action("Trainers", "Calendar",
               new { area = "CMS", userId = UserId, code = code }, protocol: Request.Url.Scheme);
            var msgBody = "Dear " + EmployeeName + ", <br/> <br/>" +
                " Your application is " + Leave + " for " + EngagementType + " ." +
                "<br/><b>By :</b> " + ApprovedBy +
                "<br/><br/>Thanx & Regards" +
                "<br/>RECKONN";

            await UserManager.SendEmailAsync(UserId, EngagementType, msgBody);

            return callbackUrl;
        }

        private void PopulatePaging(object selectedValue = null)
        {
            var PageList = generic.GetPaging();
            ViewBag.PageSize = new SelectList(PageList, "PageSize", "PageSize", selectedValue);
        }

        [HttpGet]
        public ActionResult EmployeeLeaveBulkUploadRecord(string savestatus = "")
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            TrainerPlannerSummary trainerplannersummary = new TrainerPlannerSummary();
            ViewBag.Status = savestatus;
            if (userdetails.SubscriberId != null)
            {
                ViewData["Engagement"] = db.EngagementTypeMaster.Where(c => c.CorporateId == userdetails.SubscriberId).ToList();
            }
            PopulateSchema();
            return View(trainerplannersummary);
        }

        [HttpPost]
        public ActionResult EmployeeLeaveBulkUploadRecord(TrainerPlannerSummary plannersummary, HttpPostedFileBase FileUpload)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            if (FileUpload != null)
            {
                if (FileUpload.ContentLength > 0)
                {
                    try
                    {
                        string result = ReadFileSaveInDB(FileUpload, UserId, plannersummary.SchemeId);
                        ViewBag.result = result;
                        ViewBag.SuccessCount = SuccessCount;
                        ViewBag.FailureCount = FailureCount;
                        if (FailureCount != 0)
                        {
                            ViewBag.Download = "Yes";
                        }
                        string filePath = Server.MapPath(Url.Content("~/Content/LeaveBulkUpload.csv"));
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
            PopulateSchema();
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            return View("EmployeeLeaveBulkUploadRecord", ViewData["Feedback"]);
        }
        
        public ActionResult Engagement(Int16 SchemeId = 0)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            string result = "Success";
            if (userdetails.SubscriberId != null)
            {
                var data = db.EngagementTypeMaster.Where(c => c.CorporateId == userdetails.SubscriberId && c.SchemeId == SchemeId).FirstOrDefault();
                if (data != null)
                {
                    ViewBag.Result = result;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result = "Unsuccess";
                    return Json(result, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("EmployeeLeaveBulkUploadRecord", "Calendar", new { area = "CMS", savestatus = result });
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Download(Int16 SchemeId = 0)
        {
            string UserId = User.Identity.GetUserId();
            var emp = emsMgr.GetBulkUploadEmployeeDetail(UserId, SchemeId);
            var da = admin.GetExcelColoumData(SchemeId, UserId);
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Leave.csv");
            Response.Charset = "";
            Response.ContentType = "application/text";
            StringBuilder sb = new StringBuilder();
            //dt.Columns.Add(new DataColumn("EmpId", Type.GetType("System.String")));
            sb.Append("UserName" + ',');
            sb.Append("EmpId" + ',');
            sb.Append("EmpName" + ',');
            sb.Append("Designation" + ',');
            sb.Append("Department" + ',');
            foreach (var item in da)
            {
                sb.Append(item.EngagementType + ',');
            }
            sb.Length--;
            //append new line
            sb.Append("\r\n");
            foreach (var itemRow in emp)
            {
                //append new line
                sb.Append(itemRow.UserName + ',');
                sb.Append(itemRow.EmployeeId + ',');
                sb.Append(itemRow.Name + ',');
                sb.Append(itemRow.DesignationName + ',');
                sb.Append(itemRow.Department + ',');
                foreach (var itemDyCol in da)
                {
                    sb.Append("" + ',');
                }
                sb.Length--;
                sb.Append("\r\n");
            }
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
            return View();
        }

        public ActionResult DownoladFile()
        {
            string path = VirtualPathUtility.ToAbsolute("~/Content/LeaveBulkUpload.csv");
            return File(path, "text/csv", "LeaveBulkUpload.csv");
        }

        public string ReadFileSaveInDB(HttpPostedFileBase myFile, string UserId, Int16 SchemeId = 0)
        {
            StreamReader reader = new StreamReader(myFile.InputStream, Encoding.GetEncoding(1252));
            int i = 0;
            dt.Columns.Add(new DataColumn("EmployeeId", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("EmployeeName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("LeaveType", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Error", Type.GetType("System.String")));

            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            var emp = emsMgr.GetBulkUploadEmployeeDetail(UserId, SchemeId);
            var da = admin.GetExcelColoumData(SchemeId, UserId);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                for (int x = 0; x < values.Length; x++)
                {
                    values[x] = values[x].TrimStart(' ', '"');
                    values[x] = values[x].TrimEnd('"');
                }
                //if (values.Length >=0)
                //{
                //    return "Failure : Number of Column doesn't match with template column";
                //}

                if (i > 0)
                {
                    int j = 5;
                    foreach (ExcelviewModel leaveType in da)
                    {
                        
                        var result = AddCandiateToDB(values[0], values[2], leaveType.EngagementType, values[j], UserId, SchemeId);
                        j++;
                    }
                }
                i++;
            }
            return "Success";
        }

        public string AddCandiateToDB(string UserName, string EmpName, string EngagementType, string val, string UserId, Int16 SchemeId = 0)
        {
            try
            {
                var Engagement = db.EngagementTypeMaster.Where(c => c.CorporateId == UserId && c.EngagementType == EngagementType && c.SchemeId == SchemeId).FirstOrDefault();
                Int64 EngagementTypeId = Engagement.EngagementTypeId;
                float MaxLimit = Engagement.MaxLimit;
                string userId = User.Identity.GetUserId();
                bool res = false;
                if (val != "")
                {
                float value = Convert.ToSingle(val);
                int leaveYear = Convert.ToInt32(DateTime.Now.Year.ToString());

                    string loginId = User.Identity.GetUserId();
                    UserViewModel userDetails = generic.GetUserDetail(loginId);

                    var calendaryear = db.CompanySetting.Where(c => c.CorporateId == userDetails.SubscriberId).Select(c => c.CalendarYear).FirstOrDefault();

                    if (calendaryear == "Apr-Mar" && DateTime.Now.Month <= 3)
                        leaveYear = leaveYear - 1;

                    var TrainerPlanner = db.TrainerPlannerSummary.Where(c => c.TrainerId == UserId && c.SchemeId == SchemeId && c.EngagementTypeId == EngagementTypeId).ToList();
                    if (TrainerPlanner.Count > 0)
                    {
                        MaxLimit = TrainerPlanner.FirstOrDefault().MaxLimit;
                    }

                    res = admin.AddEmployeeLeaveBulkUpload(UserName, SchemeId, EngagementTypeId, value, leaveYear, MaxLimit);
          
                    if (res)
                    {
                        SuccessCount++;

                    Object[] data = new Object[4];
                    data[0] = UserName;
                    data[1] = EmpName;
                    data[2] = EngagementType;
                    data[3] = "Upload Suucessfully";
                    dt.Rows.Add(data);
                }
                else
                {
                    SuccessCount++;
                    Object[] data = new Object[4];
                    data[0] = UserName;
                    data[1] = EmpName;
                    data[2] = EngagementType;
                    data[3] = "Failed";
                    dt.Rows.Add(data);

                }
                }

            }
            catch
            {
                FailureCount++;
                Object[] data = new Object[3];
                data[0] = UserName;
                data[1] = EmpName;
                data[2] = EngagementType;
                data[3] = "Data Not Valid";
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

        [HttpGet]
        public ActionResult AddEmployeeTourRecord(Int64 PlannerId = 0, Int64 TourId = 0, Int64 FileId = 0, string savestatus = "NA")
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            if (userdetails.Role != "Admin")
            {
                ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            }
            ViewData["EmployeeTourData"] = db.EmployeeTour.Where(x => x.UserId == UserId).ToList();
            ViewBag.UserId = userdetails.UserId;
            ViewBag.Role = userdetails.Role;
            EmployeeTour empTour = new EmployeeTour();
            empTour.PlannerId = PlannerId;
            ViewBag.savestatus = savestatus;
            if (TourId != 0)
            {
                empTour = db.EmployeeTour.Where(c => c.TourId == TourId).FirstOrDefault();
                var schedule = empTour.TourFromDate + " " + empTour.TourFromDate.ToString("hh:mm:ss tt") + " " + "-" + " " + empTour.TourToDate + " " + empTour.TourToDate.ToString("hh:mm:ss tt");
                ViewBag.Schedule = schedule;
               
            }
            return View(empTour);
        }

        [HttpPost]
        public ActionResult AddEmployeeTourRecord(EmployeeTour employeetour, string Schedule, HttpPostedFileBase uploadPhoto)
        {
            bool result = false;
            string[] strSchedule = Schedule.Split('-');
            DateTime frmdate = DateTime.ParseExact(strSchedule[0].Trim(), "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);

            employeetour.TourFromDate = frmdate;
            employeetour.TourFromDate = Convert.ToDateTime(frmdate.ToShortTimeString());

            DateTime todate = DateTime.ParseExact(strSchedule[1].Trim(), "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);
            employeetour.TourToDate = todate;
            employeetour.TourToDate = Convert.ToDateTime(todate.ToShortTimeString());
            string Status = "NA";
            result = admin.AddEmployeeTour(employeetour);
            if (result == true)
            {
                Int64 TourId = db.EmployeeTour.Max(c => c.TourId);
                string res = admin.uploadTourFileAttachment(uploadPhoto, employeetour.UserId, TourId);
                Status = "Succeeded";
            }
            else
            {
                Status = "UnSucceeded";
            }
            return RedirectToAction("AddEmployeeTourRecord", "Calendar", new { area = "CMS", savestatus = Status });
        }

        public ActionResult TourDetails(Int64 TourId = 0, Int64 PlannerId = 0)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            EmployeeTour emptourdetails = db.EmployeeTour.Where(c => c.PlannerId == PlannerId && c.TourId == TourId).FirstOrDefault();
            var Filerecord = db.TourAttachment.Where(x => x.TourId == TourId).FirstOrDefault();
            ViewData["EmployeeTourData"] = Filerecord;
            return View(emptourdetails);
        }

        private void PopulateSchema(object selectedvalue = null, Int16 SchemeId = 0)
        {
            var query = tms.GetSchemaList(SchemeId);
            SelectList SchemaList = new SelectList(query, "SchemeId", "SchemeName", selectedvalue);
            ViewBag.SchemeId = SchemaList;
        }

    }
}