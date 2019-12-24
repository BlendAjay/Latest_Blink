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
    public class EmployeeController : Controller
    {
        int SuccessCount = 0;
        int FailureCount = 0;
        DataTable resultedTable = new DataTable();
        DataTable BranchTable = new DataTable();
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

        // GET: CMS/Employee
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult EmployeeBulkUpload()
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["UserProfile"] = userDetails;
            var plandetail = admMgr.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            return View();
        }

        [HttpPost]
        public ActionResult EmployeeBulkUpload(string CorporateId, HttpPostedFileBase FileUpload)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;

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

                        string result = ReadFileSaveInDB(FileUpload, userDetails.SubscriberId, UserId);
                        ViewBag.result = result;
                        ViewBag.SuccessCount = SuccessCount;
                        ViewBag.FailureCount = FailureCount;
                        if (FailureCount != 0)
                        {
                            ViewBag.Download = "Yes";
                        }
                        string filePath = Server.MapPath(Url.Content("~/Content/EmployeeResult.csv"));
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

            return View("EmployeeBulkUpload", ViewData["Feedback"]);

        }

        #region "Supporting Function of Employee Bulk upload"

        [HttpGet]
        public ActionResult DownoladSample()
        {
            string path = VirtualPathUtility.ToAbsolute("~/Content/EmployeeSample.csv");
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

        public string ReadFileSaveInDB(HttpPostedFileBase myFile, string SubscriberId, string UpdatedBy)
        {
            DataTable resultedTable = new DataTable();
            var reader = new StreamReader(myFile.InputStream, Encoding.GetEncoding(1252));
            //var reader = new StreamReader(myFile.InputStream);
            int i = 0;
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

                if (values.Length != 5)
                {
                    return "Failure : Number of Column doesn't match with template column";
                }

                if (i != 0)
                {
                    var result = AddUserToDB(values[1], values[2], values[3], values[4], SubscriberId, UpdatedBy);
                }
                i++;
            }
            return "Success";
        }

        public string AddUserToDB(string Name, string Email, string Mobile, string Designation, string SubscriberId, string UpdatedBy)
        {
            try
            {
                bool res = false;
                //if (!admMgr.GetUserExists(Email, Mobile, ""))
                //{
                string userName = admMgr.GenerateUserName();
                var user = new ApplicationUser { UserName = userName, Email = Email, PhoneNumber = Mobile, EmailConfirmed = true };

                var result = UserManager.Create(user, "changeme");
                if (result.Succeeded)
                {

                    //string ModuleAccess = "EMS";
                    string RoleId = "Employee";
                    //string Department = "FAC";
                    //bool ManagerLevel = false;
                    var status = UserManager.AddToRole(user.Id, RoleId);
                    string UserId = User.Identity.GetUserId();
                    var userDetail = generic.GetUserDetail(UserId);
                    if (status.Succeeded)
                    {

                        // userId, employeeId, emplanelled, name, dob, gender, maritalStatus, alternateContact, alternateEmail, nationality, subscriberId, departmentId,
                        // managerLevel, reportingAuthority, updatedBy, updatedOn, deactivated, fatherName, spouseName, emergencyContactName,
                        // emergencyContactNumber, bloodGroup, physicallyChallenged, location, marriageDate, designationId)  

                        EmployeeView emp = new EmployeeView();
                        emp.UserId = user.Id;
                        emp.EmployeeId = "";
                        emp.Emplanelled = false;
                        emp.Name = Name;
                        emp.Gender = "";
                        emp.DOB = DateTime.Now;
                        emp.MaritalStatus = "";
                        emp.AlternateEmail = "";
                        emp.Nationality = "";
                        emp.DepartmentId = "FAC";
                        emp.ReportingAuthority = userDetail.SubscriberId;
                        emp.UpdatedOn = DateTime.Now;
                        emp.Deactivated = false;
                        emp.FatherName = "";
                        emp.SpouseName = "";
                        emp.EmergencyContactName = "";
                        emp.EmergencyContactNumber = "";
                        emp.BloodGroup = "";
                        emp.Location = "";
                        emp.PhysicallyChallenged = false;
                        emp.MarriageDate = DateTime.Now;
                        emp.DesignationId = 64;
                        emp.UpdatedBy = userDetail.SubscriberId;

                        res = admMgr.AddEmployee(emp, userDetail.SubscriberId, UserId);

                        var subscriberDetail = cms.GetCorporateProfile(SubscriberId).FirstOrDefault();
                        if (res)
                        {
                            SuccessCount++;
                            Object[] data = new Object[8];
                            data[0] = Name;
                            data[1] = Email;
                            data[2] = Mobile;
                            data[3] = result.Succeeded;

                            //await SendEmailConfirmationTokenAsync(subscriberDetail.Name, user.Id, "Account activation", userName, Mobile, Name);
                        }
                        else
                        {
                            Object[] data = new Object[8];
                            data[0] = Name;
                            data[1] = Email;
                            data[2] = Mobile;
                            data[3] = "Error";
                            resultedTable.Rows.Add(data);
                        }
                    }

                }
                else
                {
                    FailureCount++;
                    Object[] data = new Object[8];
                    data[0] = Name;
                    data[1] = Email;
                    data[2] = Mobile;
                    data[3] = result.Succeeded;
                    int i = 4;
                    foreach (string err in result.Errors)
                    {
                        data[i] = err;
                        i++;
                    }
                    resultedTable.Rows.Add(data);

                }
                //}
                //else
                //{
                //    FailureCount++;
                //    Object[] data = new Object[8];
                //    data[0] = Name;
                //    data[1] = Email;
                //    data[2] = Mobile;
                //    data[3] = "Failure";
                //    data[4] = "Email or Phone Number or Employee Code already exists";
                //    resultedTable.Rows.Add(data);
                //}
            }
            catch (Exception ex)
            {
                FailureCount++;
                Object[] data = new Object[8];
                data[0] = Name;
                data[1] = Email;
                data[2] = Mobile;
                data[3] = "Failed";
                data[4] = ex.Message;
                resultedTable.Rows.Add(data);
            }
            return "Result";
            //return Json("Result", JsonRequestBehavior.AllowGet);
        }

        //private async Task<string> SendEmailConfirmationTokenAsync(string SubScriber, string userID, string subject, string userName, string phoneNumber, string Name = "User")
        //{
        //    string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
        //    var callbackUrl = Url.Action("ConfirmEmail", "Account",
        //       new { area = "", userId = userID, code = code }, protocol: Request.Url.Scheme);
        //    var msgBody = "Dear " + Name + " <br/> <br/>" + SubScriber +
        //        " has added you as their USER in RECKONN. Your User Name is " + userName + " and Phone Number is " + phoneNumber + "." +
        //        "<br><br> <a href='" + callbackUrl + "' > CLICK HERE</a> to Verify your email." +
        //        "<br/><br/>You can login to your account using the password 'changeme'." +
        //    "<br/><br/>RECKONN";
        //    //   "<br/> Token will be valid for 48 hours. To regenerate token go to" + " <a href='http://www.jobenablers.com' target='_blank'>Login</a>" + " and put your credentials then it will regenerate your token.";

        //    //  msgBody = generic.AllEmailFormat(msgBody, callbackUrl, "Verify Now", "Dear", Name, "Compulsary", "Failure to verify your account within 15 days may lead to removal of your registration from our database.", "");

        //    await UserManager.SendEmailAsync(userID, subject, msgBody);

        //    return callbackUrl;
        //}

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


        #endregion

        //[HttpGet]
        //public ActionResult BranchBulkUpload()
        //{
        //    string UserId = User.Identity.GetUserId();
        //    UserViewModel userDetails = generic.GetUserDetail(UserId);
        //    ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
        //    ViewData["UserProfile"] = userDetails;
        //    PopulateClient(userDetails.SubscriberId, null);
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult BranchBulkUpload(string CorporateId, HttpPostedFileBase FileUpload)
        //{
        //    string UserId = User.Identity.GetUserId();
        //    UserViewModel userDetails = generic.GetUserDetail(UserId);
        //    ViewData["UserProfile"] = userDetails;
        //    PopulateClient(userDetails.SubscriberId, CorporateId);
        //    DataTable BranchTable = new DataTable();
        //    if (FileUpload != null)
        //    {
        //        //check we have a file
        //        if (FileUpload.ContentLength > 0)
        //        {
        //            //Try and upload
        //            try
        //            {
        //                if (string.IsNullOrEmpty(CorporateId))
        //                    CorporateId = userDetails.SubscriberId;

        //                string result = ReadFileSaveInDB(FileUpload, userDetails.SubscriberId, CorporateId, UserId);
        //                ViewBag.result = result;
        //                ViewBag.SuccessCount = SuccessCount;
        //                ViewBag.FailureCount = FailureCount;
        //                if (FailureCount != 0)
        //                {
        //                    ViewBag.Download = "Yes";
        //                }
        //                string filePath = Server.MapPath(Url.Content("~/Content/Result.csv"));
        //                ToBranchCSV(BranchTable, filePath);
        //            }
        //            catch (Exception ex)
        //            {
        //                //Catch errors
        //                ViewData["Feedback"] = ex.Message;
        //            }
        //        }
        //        else
        //        {
        //            //Catch errors
        //            ViewData["Feedback"] = "Please select a file";
        //        }
        //    }

        //    return View("BulkUpload", ViewData["Feedback"]);

        //}

        //#region "Supporting Function of Branch Bulk upload"

        //[HttpGet]
        //public ActionResult DownoladBranchSample()
        //{
        //    string path = VirtualPathUtility.ToAbsolute("~/Content/Sample.csv");
        //    return File(path, "text/csv", "SampleTemplate.csv");
        //}

        //[HttpGet]
        //public ActionResult DownloadBranch()
        //{
        //    string path = Path.Combine(Server.MapPath("~/Content/Result.csv"));
        //    return File(path, "text/csv", "result.csv");
        //}

        //public void ToBranchCSV(DataTable dtDataTable, string filePath)
        //{
        //    StreamWriter sw = new StreamWriter(filePath, false);
        //    //headers  
        //    for (int i = 0; i < dtDataTable.Columns.Count; i++)
        //    {
        //        sw.Write(dtDataTable.Columns[i]);
        //        if (i < dtDataTable.Columns.Count - 1)
        //        {
        //            sw.Write(",");
        //        }
        //    }
        //    sw.Write(sw.NewLine);
        //    foreach (DataRow dr in dtDataTable.Rows)
        //    {
        //        for (int i = 0; i < dtDataTable.Columns.Count; i++)
        //        {
        //            if (!Convert.IsDBNull(dr[i]))
        //            {
        //                string value = dr[i].ToString();
        //                if (value.Contains(','))
        //                {
        //                    value = String.Format("\"{0}\"", value);
        //                    sw.Write(value);
        //                }
        //                else
        //                {
        //                    sw.Write(dr[i].ToString());
        //                }
        //            }
        //            if (i < dtDataTable.Columns.Count - 1)
        //            {
        //                sw.Write(",");
        //            }
        //        }
        //        sw.Write(sw.NewLine);
        //    }
        //    sw.Close();
        //}

        //public string ReadFileSaveInDB(HttpPostedFileBase myFile, string SubscriberId, string CorporateId, string UpdatedBy)
        //{
        //    DataTable BranchTable = new DataTable();
        //    var reader = new StreamReader(myFile.InputStream, Encoding.GetEncoding(1252));
        //    //var reader = new StreamReader(myFile.InputStream);
        //    int i = 0;
        //    BranchTable.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
        //    BranchTable.Columns.Add(new DataColumn("Password", Type.GetType("System.String")));
        //    BranchTable.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
        //    BranchTable.Columns.Add(new DataColumn("Email", Type.GetType("System.String")));
        //    BranchTable.Columns.Add(new DataColumn("Mobile", Type.GetType("System.String")));
        //    BranchTable.Columns.Add(new DataColumn("Success", Type.GetType("System.String")));
        //    BranchTable.Columns.Add(new DataColumn("Error 1", Type.GetType("System.String")));
        //    BranchTable.Columns.Add(new DataColumn("Error 2", Type.GetType("System.String")));
        //    BranchTable.Columns.Add(new DataColumn("Error 3", Type.GetType("System.String")));
        //    BranchTable.Columns.Add(new DataColumn("Error 4", Type.GetType("System.String")));

        //    Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        //    while (!reader.EndOfStream)
        //    {
        //        var line = reader.ReadLine();
        //        var values = line.Split(',');

        //        for (int x = 0; x < values.Length; x++)
        //        {
        //            values[x] = values[x].TrimStart(' ', '"');
        //            values[x] = values[x].TrimEnd('"');
        //        }

        //        if (values.Length != 12)
        //        {
        //            return "Failure : Number of Column doesn't match with template column";
        //        }

        //        if (i != 0)
        //        {
        //            var result = AddUserToDB(values[1], values[2], values[3], values[4], values[5], values[6], values[7], values[8], values[9], values[10], values[11], SubscriberId, CorporateId, UpdatedBy);
        //        }
        //        i++;
        //    }
        //    return "Success";
        //}

        //public string AddUserToDB(string Name, string Email, string Mobile, string Gender, string EmployeeCode, string Designation, string BranchCode, string BranchName, string BranchCategory, string State, string Region, string SubscriberId, string CorporateId, string UpdatedBy)
        //{
        //    try
        //    {
        //        bool res = false;
        //        if (!admMgr.GetUserExists(Email, Mobile, EmployeeCode))
        //        {
        //            string userName = admMgr.GenerateUserName();
        //            string passcode = admMgr.GeneratePassword(Name);

        //            var user = new ApplicationUser { UserName = userName, Email = Email, PhoneNumber = Mobile, EmailConfirmed = true };

        //            var result = UserManager.Create(user, passcode);
        //            if (result.Succeeded)
        //            {

        //                string ModuleAccess = "SMS";
        //                string RoleId = "Candidate";
        //                string Department = "CAN";

        //                var status = UserManager.AddToRole(user.Id, RoleId);
        //                if (status.Succeeded)
        //                {
        //                    res = admMgr.UserRegistration(user.Id, Name, DateTime.UtcNow, ModuleAccess, Department, RoleId, SubscriberId, false, SubscriberId, DateTime.UtcNow,
        //                                                  UpdatedBy, EmployeeCode, BranchName, BranchCategory, Region, BranchCode, State, CorporateId, Gender, passcode, "", "", null, "", "", null, Designation);
        //                }
        //                if (res)
        //                    SuccessCount++;

        //                Object[] data = new Object[10];
        //                data[0] = userName;
        //                data[1] = passcode;
        //                data[2] = Name;
        //                data[3] = Email;
        //                data[4] = Mobile;
        //                data[5] = result.Succeeded;
        //                int i = 6;
        //                foreach (string err in result.Errors)
        //                {
        //                    data[i] = err;
        //                    i++;
        //                }
        //                BranchTable.Rows.Add(data);
        //            }
        //            else
        //            {
        //                FailureCount++;
        //                Object[] data = new Object[10];
        //                data[2] = Name;
        //                data[3] = Email;
        //                data[4] = Mobile;
        //                data[5] = result.Succeeded;
        //                int i = 6;
        //                foreach (string err in result.Errors)
        //                {
        //                    data[i] = err;
        //                    i++;
        //                }
        //                BranchTable.Rows.Add(data);

        //            }
        //        }
        //        else
        //        {
        //            FailureCount++;
        //            Object[] data = new Object[10];
        //            data[2] = Name;
        //            data[3] = Email;
        //            data[4] = Mobile;
        //            data[5] = "Failure";
        //            data[6] = "Email or Phone Number or Employee Code already exists";
        //            BranchTable.Rows.Add(data);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        FailureCount++;
        //        Object[] data = new Object[10];
        //        data[2] = Name;
        //        data[3] = Email;
        //        data[4] = Mobile;
        //        data[5] = "Failed";
        //        data[6] = ex.Message;
        //        BranchTable.Rows.Add(data);
        //    }
        //    return "Result";
        //}

        //private void PopulateClient(string SubscriberId, object selectedOrderType = null)
        //{
        //    var query = generic.GetSubscriberWiseClientListBulkUpload(SubscriberId, false);
        //    SelectList OrderTypes = new SelectList(query, "CorporateId", "Name", selectedOrderType);
        //    ViewBag.CorporateId = OrderTypes;
        //}

        //#endregion
    }
}