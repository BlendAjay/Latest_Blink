using AJSolutions.Areas.Admin.Models;
using AJSolutions.DAL;
using AJSolutions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.Areas.CMS.Models;
using System.Globalization;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Data;
using System.Windows.Forms;
using ClosedXML;
using System.Collections;
using Microsoft.Reporting.WebForms;
using System.Data.SqlClient;
using System.Configuration;

namespace AJSolutions.Areas.CMS.Controllers
{
    public class UserController : Controller
    {
        AdminManager admin = new AdminManager();
        CMSManager cmsMgr = new CMSManager();
        EMSManager emsMgr = new EMSManager();
        Generic generic = new Generic();
        Student student = new Student();
        TMSManager tms = new TMSManager();
        UserDBContext userContext = new UserDBContext();
        DataTable dtCustmer = new DataTable();



        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

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

        [HttpGet]
        //////[Authorize(Roles = "Client")]
        public ActionResult AddChannelPartner(string CorporateId)
        {
            string Id = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(Id);
            ViewData["UserProfile"] = generic.GetUserDetail(Id);
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(Id).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            //var MyChannels = userContext.CorporateProfile.Where(c => c.SubscriberId == Id);
            //ViewData["MyChannels"] = MyChannels;
            //var Channel = MyChannels.Where(c => c.CorporateId == CorporateId);
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddChannelPartner(string Name, string Email, string PhoneNumber)
        {
            string Id = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(Id);
            string userName = admin.GenerateUserName();
            var user = new ApplicationUser { UserName = userName, Email = Email, PhoneNumber = PhoneNumber, EmailConfirmed = true };

            var result = await UserManager.CreateAsync(user, "changeme");
            if (result.Succeeded)
            {
                // Assign user to Role
                var status = UserManager.AddToRole(user.Id, "Client");
                if (status.Succeeded)
                {

                    string ModuleId = "CMS";
                    string RoleId = "Client";
                    string DepartmentId = "CPT";
                    DateTime RegisteredOn = DateTime.UtcNow;

                    var res = cmsMgr.AddChannelPartner(user.Id, Name, RegisteredOn, ModuleId, DepartmentId, RoleId, userDetails.UserId, userDetails.SubscriberId, DateTime.UtcNow, userDetails.UserId);
                    if (res)
                    {
                        string callbackUrl = await SendEmailConfirmationTokenAsync(userDetails.Name, user.Id, "Account activation", userName, PhoneNumber, Name);
                    }
                }
            }

            return RedirectToAction("Notifications", "UserNotification", new { area = "CMS" });
        }

        [HttpGet]
        ////[Authorize(Roles = "Client")]
        public ActionResult MyChannelPartner(string CorporateId, string sortOrder, string UserName, string Name, string EmailId, int? page, int PageSize = 10)
        {
            string Id = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(Id);
            ViewData["UserProfile"] = generic.GetUserDetail(Id);
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(Id).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            var MyChannels = cmsMgr.GetSubscriberWiseClientList(userDetails.SubscriberId).Where(c => c.ReferenceId == Id);
            ViewBag.ClCount = MyChannels == null ? 0 : MyChannels.Count();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.UserNameSortParam = sortOrder == "UserName" ? "UserName_desc" : "UserName";
            ViewBag.NameSortParam = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.EmailSortParam = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewBag.RegisteredOnSortParam = sortOrder == "RegisteredOn" ? "RegisteredOn_desc" : "RegisteredOn";
            ViewBag.LastLoggedInSortParam = sortOrder == "LastLoggedIn" ? "LastLoggedIn_desc" : "LastLoggedIn";
            ViewBag.CompanyNameSortParam = sortOrder == "CompanyName" ? "CompanyName_desc" : "CompanyName";

            ViewBag.UserName = UserName;
            ViewBag.Name = Name;
            ViewBag.EmailId = EmailId;
            ViewBag.Page = page;

            //Apply filter
            if (!String.IsNullOrEmpty(EmailId))
            {
                MyChannels = MyChannels.Where(s => (!String.IsNullOrEmpty(s.Email) && s.Email.Contains(EmailId))).ToList();
            }
            if (!String.IsNullOrEmpty(Name))
            {
                MyChannels = MyChannels.Where(s => s.Name.ToLower().Contains(Name.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(UserName))
            {
                MyChannels = MyChannels.Where(s => s.UserName.ToUpper().Contains(UserName.ToUpper())).ToList();
            }
            //Apply sorting
            if (MyChannels.Count() > 0)
            {
                switch (sortOrder)
                {
                    case "UserName":
                        MyChannels = MyChannels.OrderBy(c => c.UserName).ToList();
                        break;
                    case "Name":
                        MyChannels = MyChannels.OrderBy(c => c.Name).ToList();
                        break;
                    case "Name_desc":
                        MyChannels = MyChannels.OrderByDescending(c => c.Name).ToList();
                        break;
                    case "Email":
                        MyChannels = MyChannels.OrderBy(c => c.Email).ToList();
                        break;
                    case "Email_desc":
                        MyChannels = MyChannels.OrderByDescending(c => c.Email).ToList();
                        break;
                    case "RegisteredOn":
                        MyChannels = MyChannels.OrderBy(c => c.RegisteredOn).ToList();
                        break;
                    case "RegisteredOn_desc":
                        MyChannels = MyChannels.OrderByDescending(c => c.RegisteredOn).ToList();
                        break;
                    case "LastLoggedIn":
                        MyChannels = MyChannels.OrderBy(c => c.LastLogin).ToList();
                        break;
                    case "LastLoggedIn_desc":
                        MyChannels = MyChannels.OrderByDescending(c => c.LastLogin).ToList();
                        break;
                    default:
                        MyChannels = MyChannels.OrderBy(c => c.Name).ToList();
                        break;
                }
            }


            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);

            return View(MyChannels.ToPagedList(pageNumber, pageSize));

        }

        // GET: CMS/AddUser
        ////[Authorize(Roles = "Admin,Employee")]
        public ActionResult Add(string Id, string savestatus, string Uid, string CId, string UserAction, int Batch = 0)
        {
            ViewBag.Id = Id;
            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(UserId);
            ViewBag.UserAction = UserAction;
            ViewBag.UserId = userDetails.SubscriberId;
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();

            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;

            ViewBag.status = savestatus;
            ViewData["MyProfile"] = cmsMgr.GetCorporateProfile(Uid).FirstOrDefault();
            ViewData["ComapanyProfile"] = cmsMgr.GetCompanyProfileList(Uid).FirstOrDefault();
            ViewBag.Uid = Uid;
            ViewBag.Batch = Batch;
            ViewData["IppbBranch"] = (from i in userContext.BranchMaster select i).AsEnumerable();
            List<CorporateProfile> corporateProfiles = userContext.CorporateProfile.Where(p => p.SubscriberId == userDetails.SubscriberId).ToList().FindAll(p => p.DepartmentId == "CLI" || (p.DepartmentId == "ADI" && p.CorporateId == userDetails.SubscriberId));

            if (Id == "TPU")
            {
                PopulateDepartment("CMS");
                PopulateNationality();
                PopulateCompanyType();
                PopulateCompanySize();
            }
            else if (Id == "EMP")
            {
                PopulateDepartment("EMS");
                PopulateManager(userDetails.SubscriberId);
                PopulateGrade();
                PopulateEmployementStatus();
                var details = admin.GetUserRegistrationDetails(Uid);
                PopulateDepartment("EMS", null, details.DepartmentId);
            }
            else if (Id == "CND")
            {
                PopulateDepartment("SMS");
                //PopulateCourse(UserId);
                //PopulateBatch();
                PopulateGenderStatus();

                if (!string.IsNullOrEmpty(CId))
                {
                    ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => cp.Name), "CorporateId", "Name", CId);
                }
                else
                {
                    ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => cp.Name), "CorporateId", "Name");
                }

            }
            else if (Id == "VND")
            {
                PopulatethirdPartyDepartment("CMS");
            }
            if (Uid != null)
            {
                if (Id == "EMP")
                {
                    var details = admin.GetUserRegistrationDetails(Uid);
                    PopulateDepartment("EMS", null, details.DepartmentId);
                    PopulateManager(userDetails.SubscriberId, details.ReportingAuthority);
                    PopulateGrade();
                    PopulateEmployementStatus();
                    return View(details);
                }
                else if (Id == "VND")
                {
                    var details = admin.GetTPDetails(Uid);
                    PopulatethirdPartyDepartment("CMS", null, details.DepartmentId);
                    PopulateManager(userDetails.SubscriberId);
                    return View(details);
                }
                else if (Id == "ADI")
                {
                    var details = admin.GetTPDetails(Uid);
                    return View(details);
                }
                else if (Id == "CND")
                {
                    var details = admin.GetCandidateDetails(Uid);
                    PopulateDepartment("SMS", null, details.DepartmentId);
                    //PopulateCourse(userDetails.SubscriberId, details.CourseCode);
                    //PopulateBatch(userDetails.SubscriberId, details.CourseCode, details.BatchId);
                    PopulateGenderStatus(details.Gender);
                    ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => cp.Name), "CorporateId", "Name", details.CorporateId);
                    return View(details);
                }
                else if (Id == "TPU")
                {
                    var details = admin.GetClientDetails(Uid);
                    if (details.DepartmentId != null)
                    {
                        PopulateDepartment("CMS", null, details.DepartmentId);
                    }
                    PopulateNationality(details.Nationality);
                    PopulateCompanyType(details.CompanyType);
                    PopulateCompanySize(details.CompanySize);
                    return View(details);
                }
            }
            return View();
        }

        [HttpPost]
        ////[Authorize(Roles = "Admin,Employee")]
        public async Task<ActionResult> Add(string UserId, string Uid, string Name, string Email, string PhoneNumber, string DepartmentId,
                                            string Mod, string Name1, string AlternateContact, string CompanyName, string CorporateId, string CompanySize,
                                            string AlternateEmail, string Nationality, string CompanyType, string Website, string RegistrationId, string Source,
                                            string ReportingAuthority, DateTime? UpdatedOn, string UpdatedBy, string Designation, string Branch, string BranchCategory, string TrainingType,
                                            string Region, string BranchCode, string BranchState, string ClientId, string Gender, string Status, DateTime? dateofJoining = null,
                                            string ProbationPeriod = "0", string GradeID = "0", DateTime? DateofConfirmation = null, int Batch = 0, bool ManagerLevel = false, string TrackerId = null, string FacilityId = null, string Accesspoint = null)
        {
            string savestatus = "";

            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());

            string SubscriberId = userDetails.SubscriberId;

            var subscriberDetail = cmsMgr.GetCorporateProfile(SubscriberId).FirstOrDefault();

            if (String.IsNullOrEmpty(Uid))
            {
                //User Add Mode
                string userName = admin.GenerateUserName();
                var user = new ApplicationUser { UserName = userName, Email = Email, PhoneNumber = PhoneNumber, EmailConfirmed = true };

                var result = await UserManager.CreateAsync(user, "changeme");

                if (result.Succeeded)
                {
                    string ModuleAccess = "SMS";
                    string RoleId = "Candidate";
                    string Department = "CAN";
                    if (Mod == "TPU" || Mod == "VND")
                    {
                        ModuleAccess = "CMS";
                        RoleId = "Client";
                        Department = DepartmentId;
                    }
                    else if (Mod == "EMP")
                    {
                        ModuleAccess = "EMS";
                        RoleId = "Employee";
                        Department = DepartmentId;
                        var managerLevel = ManagerLevel;
                        var reportingAuthority = ReportingAuthority;
                    }
                    else if (Mod == "ADI")
                    {
                        ModuleAccess = "CMS";
                        RoleId = "Admin";
                        Department = "ADI";
                    }

                    // Assign user to Role
                    var status = UserManager.AddToRole(user.Id, RoleId);
                    if (status.Succeeded)
                    {
                        UpdatedBy = User.Identity.GetUserId();
                        savestatus = "Succeeded";
                        if (!string.IsNullOrEmpty(TrainingType))
                        {
                            RegistrationId = admin.GetSIUserId(Branch, TrainingType);
                        }

                        admin.UserRegistration(user.Id, Name, DateTime.UtcNow, ModuleAccess, Department, RoleId, SubscriberId,
                            ManagerLevel, ReportingAuthority, DateTime.UtcNow, UpdatedBy, RegistrationId, null, Branch,
                            BranchCategory, Region, BranchCode, BranchState, CorporateId, Gender, "", Source, Designation, null, TrackerId, FacilityId, Accesspoint);
                        //end
                        if (RoleId.ToUpper() == "CLIENT")
                        {
                            UpdatedBy = User.Identity.GetUserId();
                            cmsMgr.AddCorporateProfile(user.Id, Name, AlternateContact, AlternateEmail, Nationality, DepartmentId, SubscriberId, DateTime.UtcNow, UpdatedBy);
                            cmsMgr.AddCompanyProfile(user.Id, CompanyName, CompanyType, CompanySize, Website, DateTime.UtcNow, UpdatedBy, null, null, null, null, null);
                        }
                        else if (RoleId.ToUpper() == "EMPLOYEE")
                            emsMgr.AddProfileTypeDetails(0, user.Id, "Default");

                        else if (RoleId.ToUpper() == "CANDIDATE")
                        {
                            var res = cmsMgr.UpdatePassword(user.Id, "changeme");
                        }
                        if (user.Id != null && Batch != 0)
                        {
                            admin.AddCandidateCourse(user.Id, Batch, 1);
                        }
                        if (!string.IsNullOrEmpty(Email))
                        {
                            string callbackUrl = await SendEmailConfirmationTokenAsync(subscriberDetail.Name, user.Id, "Account activation", userName, PhoneNumber, Name);
                        }
                    }
                }
            }
            else
            {
                //User Edit Mode
                var regUser = UserManager.FindById(Uid);
                if ((String.IsNullOrEmpty(regUser.Email) || (!String.IsNullOrEmpty(regUser.Email) && regUser.Email != Email.Trim())) ||
                    (String.IsNullOrEmpty(regUser.PhoneNumber) || (!String.IsNullOrEmpty(regUser.PhoneNumber) && regUser.PhoneNumber != PhoneNumber.Trim())))
                {
                    bool result = admin.UpdateUserEmailPhone(regUser.UserName, Email, PhoneNumber, true);
                }

                //string ModuleAccess = "SMS";
                //string RoleId = "Candidate";
                string Department = "CAN";
                if (Mod == "EMP")
                {
                    //RoleId = "Employee";
                    Department = DepartmentId;
                    var managerLevel = ManagerLevel;
                    var reportingAuthority = ReportingAuthority;
                    UpdatedBy = User.Identity.GetUserId();
                    //changes preeti singh 26/8/17
                    var update = admin.UpdateUserRegistration(Uid, Name, Department, ManagerLevel, ReportingAuthority, DateTime.UtcNow, UpdatedBy, Designation);
                    //end
                    savestatus = "EditSucceeded";
                }
                else if (Mod == "TPU")
                {
                    //ModuleAccess = "CMS";
                    //RoleId = "Client";
                    Department = DepartmentId;
                    UpdatedBy = User.Identity.GetUserId();
                    var update = admin.UpdateThirdParty(Uid, Name, Department, DateTime.UtcNow, UpdatedBy);
                    cmsMgr.AddCorporateProfile(Uid, Name, AlternateContact, AlternateEmail, Nationality, DepartmentId, SubscriberId, DateTime.UtcNow, UpdatedBy);
                    cmsMgr.AddCompanyProfile(Uid, CompanyName, CompanyType, CompanySize, Website, DateTime.UtcNow, UpdatedBy, null, null, null, null, null);
                    savestatus = "EditSucceeded";
                }

                else if (Mod == "CND")
                {
                    UpdatedBy = User.Identity.GetUserId();
                    var update = admin.UpdateCandidate(Uid, Name, RegistrationId, DateTime.UtcNow, UpdatedBy, Designation, Branch, BranchCategory, Region, BranchCode, BranchState, CorporateId, Gender);
                    savestatus = "EditSucceeded";
                    //int batchId = 0;
                    //if (!string.IsNullOrEmpty(BatchId))
                    //    batchId = Convert.ToInt32(BatchId);
                    //admin.AddCandidateCourse(Uid, batchId, 1);
                }
                else if (Mod == "VND")
                {
                    Department = DepartmentId;
                    UpdatedBy = User.Identity.GetUserId();
                    var update = admin.UpdateThirdParty(Uid, Name, Department, DateTime.UtcNow, UpdatedBy);
                    savestatus = "EditSucceeded";
                }

            }
            //PopulateRoleAccessLevel();
            if (userDetails.SubscriberId == userDetails.UserId)
            {
                return RedirectToAction("Add", "User", new { area = "CMS", Id = Mod, savestatus });
            }
            else
            {
                return RedirectToAction("Index", "DashBoard", new { area = "EMS", Status = true });
            }
            //return Json(savestatus, JsonRequestBehavior.AllowGet);
        }

        ////[Authorize(Roles = "Admin,Employee")]
        public ActionResult Employees(string sortOrder, string UserName, string Department, string Name, string EmailId, string Designation, string UName, string ReportingAuthority, int? page, int PageSize = 10)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            var empdetails = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["EmpDetails"] = empdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            var EmployeeList = emsMgr.GetSubscriberWiseEmployeeList(userdetails.SubscriberId).Where(c => c.DepartmentId != "ADI").ToList();
            ViewBag.UserId = userdetails.SubscriberId;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.UserNameSortParam = sortOrder == "UserName" ? "UserName_desc" : "UserName";
            ViewBag.NameSortParam = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.EmailSortParam = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewBag.RegisteredOnSortParam = sortOrder == "RegisteredOn" ? "RegisteredOn_desc" : "RegisteredOn";
            ViewBag.LastLoggedInSortParam = sortOrder == "LastLoggedIn" ? "LastLoggedIn_desc" : "LastLoggedIn";
            ViewBag.DeginationSortParam = sortOrder == "Designation" ? "Designation_desc" : "Designation";
            ViewBag.DepartmentSortParam = sortOrder == "Department" ? "Department_desc" : "Department";
            ViewBag.ReportingAuthoritySortParam = sortOrder == "ReportingAuthority" ? "ReportingAuthority_desc" : "ReportingAuthority";

            ViewBag.UserName = UserName;
            ViewBag.Name = Name;
            ViewBag.EmailId = EmailId;
            ViewBag.Designation = Designation;
            ViewBag.Department = Department;
            ViewBag.UName = UName;
            ViewBag.Page = page;



            ViewBag.EmCount = EmployeeList == null ? 0 : EmployeeList.Count();

            if (!String.IsNullOrEmpty(EmailId))
            {
                EmployeeList = EmployeeList.Where(s => (!String.IsNullOrEmpty(s.Email) && s.Email.Contains(EmailId))).ToList();
            }
            if (!String.IsNullOrEmpty(Name))
            {
                EmployeeList = EmployeeList.Where(s => s.Name.ToLower().Contains(Name.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(UserName))
            {
                EmployeeList = EmployeeList.Where(s => s.UserName.ToUpper().Contains(UserName.ToUpper())).ToList();
            }


            if (EmployeeList.Count != 0)
            {
                switch (sortOrder)
                {
                    case "UserName":
                        EmployeeList = EmployeeList.OrderBy(c => c.UserName).ToList();
                        break;
                    case "Name":
                        EmployeeList = EmployeeList.OrderBy(c => c.Name).ToList();
                        break;
                    case "Name_desc":
                        EmployeeList = EmployeeList.OrderByDescending(c => c.Name).ToList();
                        break;
                    case "Email":
                        EmployeeList = EmployeeList.OrderBy(c => c.Email).ToList();
                        break;
                    case "Email_desc":
                        EmployeeList = EmployeeList.OrderByDescending(c => c.Email).ToList();
                        break;
                    case "RegisteredOn":
                        EmployeeList = EmployeeList.OrderBy(c => c.RegisteredOn).ToList();
                        break;
                    case "RegisteredOn_desc":
                        EmployeeList = EmployeeList.OrderByDescending(c => c.RegisteredOn).ToList();
                        break;
                    case "LastLoggedIn":
                        EmployeeList = EmployeeList.OrderBy(c => c.LastLogin).ToList();
                        break;
                    case "LastLoggedIn_desc":
                        EmployeeList = EmployeeList.OrderByDescending(c => c.LastLogin).ToList();
                        break;
                    //case "Designation":
                    //    EmployeeList = EmployeeList.OrderBy(c => c.Designation).ToList();
                    //    break;
                    //case "Designation_desc":
                    //    EmployeeList = EmployeeList.OrderByDescending(c => c.Designation).ToList();
                    //    break;
                    case "Department":
                        EmployeeList = EmployeeList.OrderBy(c => c.Department).ToList();
                        break;
                    case "Department_desc":
                        EmployeeList = EmployeeList.OrderByDescending(c => c.Department).ToList();
                        break;
                    case "ReportingAuthority":
                        EmployeeList = EmployeeList.OrderBy(c => c.ReportingAuthority).ToList();
                        break;
                    case "ReportingAuthority_desc":
                        EmployeeList = EmployeeList.OrderByDescending(c => c.ReportingAuthority).ToList();
                        break;
                    default:
                        EmployeeList = EmployeeList.OrderBy(c => c.Name).ToList();
                        break;
                }
            }

            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);

            return View(EmployeeList.ToPagedList(pageNumber, pageSize));
            //return View(EmployeeList);
        }

        [HttpGet]
        ////[Authorize(Roles = "Admin,Employee")]
        public ActionResult Candidates(string sortOrder, string UserName, string Name, string EmailId, string ReferenceId, string CorporateId, int? page, int PageSize = 10)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            List<CorporateProfile> corporateProfiles = userContext.CorporateProfile.Where(p => p.SubscriberId == userdetails.SubscriberId).ToList().FindAll(p => p.DepartmentId == "CLI" || (p.DepartmentId == "ADI" && p.CorporateId == userdetails.SubscriberId));
            ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => cp.Name), "CorporateId", "Name");
            var CandidateList = student.GetSubscriberWiseDistinctCandidateList(userdetails.SubscriberId);
            PopulateNIBFReference(userdetails.SubscriberId);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.UserNameSortParam = sortOrder == "UserName" ? "UserName_desc" : "UserName";
            ViewBag.NameSortParam = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.EmailSortParam = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewBag.RegisteredOnSortParam = sortOrder == "RegisteredOn" ? "RegisteredOn_desc" : "RegisteredOn";
            ViewBag.LastLoggedInSortParam = sortOrder == "LastLoggedIn" ? "LastLoggedIn_desc" : "LastLoggedIn";
            ViewBag.CorporateSortParam = sortOrder == "Corporate" ? "Corporate_desc" : "Corporate";

            ViewBag.UserName = UserName;
            ViewBag.Name = Name;
            ViewBag.EmailId = EmailId;
            ViewBag.CorporateIdFilter = CorporateId;
            ViewBag.Page = page;
            ViewBag.ReferenceId = ReferenceId;
            ViewBag.CCount = CandidateList == null ? 0 : CandidateList.Count();

            //Apply filter
            if (!String.IsNullOrEmpty(CorporateId))
            {
                CandidateList = CandidateList.Where(s => s.CorporateId != null).ToList();
                CandidateList = CandidateList.Where(s => s.CorporateId.Contains(CorporateId)).ToList();
                ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => cp.Name), "CorporateId", "Name", CorporateId);
            }
            else
            {
                ViewBag.CorporateId = new SelectList(corporateProfiles.OrderBy(cp => cp.Name), "CorporateId", "Name");
            }

            if (!String.IsNullOrEmpty(EmailId))
            {
                CandidateList = CandidateList.Where(s => (!String.IsNullOrEmpty(s.Email) && s.Email.Contains(EmailId))).ToList();
            }
            if (!String.IsNullOrEmpty(Name))
            {
                CandidateList = CandidateList.Where(s => s.Name.ToLower().Contains(Name.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(UserName))
            {
                CandidateList = CandidateList.Where(s => s.UserName.ToUpper().Contains(UserName.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(ReferenceId))
            {
                CandidateList = CandidateList.Where(s => s.ReferenceId == ReferenceId).ToList();
            }

            //Apply sorting
            if (CandidateList.Count != 0)
            {
                switch (sortOrder)
                {
                    case "UserName":
                        CandidateList = CandidateList.OrderBy(c => c.UserName).ToList();
                        break;
                    case "Name":
                        CandidateList = CandidateList.OrderBy(c => c.Name).ToList();
                        break;
                    case "Name_desc":
                        CandidateList = CandidateList.OrderByDescending(c => c.Name).ToList();
                        break;
                    case "Email":
                        CandidateList = CandidateList.OrderBy(c => c.Email).ToList();
                        break;
                    case "Email_desc":
                        CandidateList = CandidateList.OrderByDescending(c => c.Email).ToList();
                        break;
                    case "RegisteredOn":
                        CandidateList = CandidateList.OrderBy(c => c.RegisteredOn).ToList();
                        break;
                    case "RegisteredOn_desc":
                        CandidateList = CandidateList.OrderByDescending(c => c.RegisteredOn).ToList();
                        break;
                    case "LastLoggedIn":
                        CandidateList = CandidateList.OrderBy(c => c.LastLogin).ToList();
                        break;
                    case "LastLoggedIn_desc":
                        CandidateList = CandidateList.OrderByDescending(c => c.LastLogin).ToList();
                        break;
                    case "Corporate":
                        CandidateList = CandidateList.OrderBy(c => c.CorporateName).ToList();
                        break;
                    case "Corporate_desc":
                        CandidateList = CandidateList.OrderByDescending(c => c.CorporateName).ToList();
                        break;
                    default:
                        CandidateList = CandidateList.OrderBy(c => c.Name).ToList();
                        break;
                }
            }

            //Apply paging
            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);

            return View(CandidateList.ToPagedList(pageNumber, pageSize));
            //return View(CandidateList);
        }

        [HttpPost]
        ////[Authorize(Roles = "Admin,Employee")]
        [ActionName("Candidates")]
        public ActionResult CandidatesPost(string sortOrder, string UserName, string Name, string EmailId, string Reference, string CorporateId, int? page, int PageSize = 10)
        {
            return RedirectToAction("Candidates", "User", new { area = "CMS", sortOrder = sortOrder, UserName = UserName, Name = Name, EmailId = EmailId, ReferenceId = Reference, CorporateId = CorporateId, page = page, PageSize = PageSize });
        }

        ////[Authorize(Roles = "Admin")]
        public ActionResult Clients(string sortOrder, string UserName, string Name, string EmailId, string CompanyName, int? page, int PageSize = 10)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            var Clients = cmsMgr.GetSubscriberWiseClientList(userdetails.SubscriberId).Where(c => c.DepartmentId == "CLI").ToList();
            string Link = Global.WebsiteUrl();
            ViewBag.Link = Link;
            ViewBag.ClCount = Clients == null ? 0 : Clients.Count();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.UserNameSortParam = sortOrder == "UserName" ? "UserName_desc" : "UserName";
            ViewBag.NameSortParam = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.EmailSortParam = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewBag.RegisteredOnSortParam = sortOrder == "RegisteredOn" ? "RegisteredOn_desc" : "RegisteredOn";
            ViewBag.LastLoggedInSortParam = sortOrder == "LastLoggedIn" ? "LastLoggedIn_desc" : "LastLoggedIn";
            ViewBag.CompanyNameSortParam = sortOrder == "CompanyName" ? "CompanyName_desc" : "CompanyName";

            ViewBag.UserName = UserName;
            ViewBag.Name = Name;
            ViewBag.EmailId = EmailId;
            ViewBag.CompanyName = CompanyName;
            ViewBag.Page = page;

            //Apply filter
            if (!String.IsNullOrEmpty(EmailId))
            {
                Clients = Clients.Where(s => (!String.IsNullOrEmpty(s.Email) && s.Email.Contains(EmailId))).ToList();
            }
            if (!String.IsNullOrEmpty(Name))
            {
                Clients = Clients.Where(s => s.Name.ToLower().Contains(Name.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(UserName))
            {
                Clients = Clients.Where(s => s.UserName.ToUpper().Contains(UserName.ToUpper())).ToList();
            }
            if (!String.IsNullOrEmpty(CompanyName))
            {
                Clients = Clients.Where(s => s.CompanyName.ToUpper().Contains(CompanyName.ToUpper())).ToList();
            }
            //Apply sorting
            if (Clients.Count != 0)
            {
                switch (sortOrder)
                {
                    case "UserName":
                        Clients = Clients.OrderBy(c => c.UserName).ToList();
                        break;
                    case "Name":
                        Clients = Clients.OrderBy(c => c.Name).ToList();
                        break;
                    case "Name_desc":
                        Clients = Clients.OrderByDescending(c => c.Name).ToList();
                        break;
                    case "Email":
                        Clients = Clients.OrderBy(c => c.Email).ToList();
                        break;
                    case "Email_desc":
                        Clients = Clients.OrderByDescending(c => c.Email).ToList();
                        break;
                    case "RegisteredOn":
                        Clients = Clients.OrderBy(c => c.RegisteredOn).ToList();
                        break;
                    case "RegisteredOn_desc":
                        Clients = Clients.OrderByDescending(c => c.RegisteredOn).ToList();
                        break;
                    case "LastLoggedIn":
                        Clients = Clients.OrderBy(c => c.LastLogin).ToList();
                        break;
                    case "LastLoggedIn_desc":
                        Clients = Clients.OrderByDescending(c => c.LastLogin).ToList();
                        break;
                    case "CompanyName":
                        Clients = Clients.OrderBy(c => c.CompanyName).ToList();
                        break;
                    case "CompanyName_desc":
                        Clients = Clients.OrderByDescending(c => c.CompanyName).ToList();
                        break;
                    default:
                        Clients = Clients.OrderBy(c => c.Name).ToList();
                        break;
                }
            }


            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);

            return View(Clients.ToPagedList(pageNumber, pageSize));

            //return View(Clients);
        }


        //[Authorize(Roles = "Admin")]
        public ActionResult ThirdParty(string sortOrder, string UserName, string Name, string EmailId, string CompanyName, string Department, int? page, int PageSize = 10)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewBag.UserId = userdetails.SubscriberId;
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            var VendorList = cmsMgr.GetSubscriberWiseClientList(userdetails.SubscriberId).Where(c => c.DepartmentId != "CLI" && c.DepartmentId != "ADI").ToList();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.UserNameSortParam = sortOrder == "UserName" ? "UserName_desc" : "UserName";
            ViewBag.NameSortParam = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.EmailSortParam = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewBag.RegisteredOnSortParam = sortOrder == "RegisteredOn" ? "RegisteredOn_desc" : "RegisteredOn";
            ViewBag.LastLoggedInSortParam = sortOrder == "LastLoggedIn" ? "LastLoggedIn_desc" : "LastLoggedIn";
            ViewBag.CompanyNameSortParam = sortOrder == "CompanyName" ? "CompanyName_desc" : "CompanyName";
            ViewBag.DepartmentSortParam = sortOrder == "Department" ? "Department_desc" : "Department";

            ViewBag.UserName = UserName;
            ViewBag.Name = Name;
            ViewBag.EmailId = EmailId;
            ViewBag.CompanyName = CompanyName;
            ViewBag.Department = Department;
            ViewBag.Page = page;

            @ViewBag.TPCount = VendorList == null ? 0 : VendorList.Count();

            //Apply filter
            if (!String.IsNullOrEmpty(EmailId))
            {
                VendorList = VendorList.Where(s => (!String.IsNullOrEmpty(s.Email) && s.Email.Contains(EmailId))).ToList();
            }
            if (!String.IsNullOrEmpty(Name))
            {
                VendorList = VendorList.Where(s => s.Name.ToLower().Contains(Name.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(UserName))
            {
                VendorList = VendorList.Where(s => s.UserName.ToUpper().Contains(UserName.ToUpper())).ToList();
            }
            if (!String.IsNullOrEmpty(CompanyName))
            {
                VendorList = VendorList.Where(s => s.CompanyName.ToUpper().Contains(CompanyName.ToUpper())).ToList();
            }
            if (!String.IsNullOrEmpty(Department))
            {
                VendorList = VendorList.Where(s => s.Department.ToUpper().Contains(Department.ToUpper())).ToList();
            }
            //Apply sorting
            if (VendorList.Count != 0)
            {
                switch (sortOrder)
                {
                    case "UserName":
                        VendorList = VendorList.OrderBy(c => c.UserName).ToList();
                        break;
                    case "Name":
                        VendorList = VendorList.OrderBy(c => c.Name).ToList();
                        break;
                    case "Name_desc":
                        VendorList = VendorList.OrderByDescending(c => c.Name).ToList();
                        break;
                    case "Email":
                        VendorList = VendorList.OrderBy(c => c.Email).ToList();
                        break;
                    case "Email_desc":
                        VendorList = VendorList.OrderByDescending(c => c.Email).ToList();
                        break;
                    case "RegisteredOn":
                        VendorList = VendorList.OrderBy(c => c.RegisteredOn).ToList();
                        break;
                    case "RegisteredOn_desc":
                        VendorList = VendorList.OrderByDescending(c => c.RegisteredOn).ToList();
                        break;
                    case "LastLoggedIn":
                        VendorList = VendorList.OrderBy(c => c.LastLogin).ToList();
                        break;
                    case "LastLoggedIn_desc":
                        VendorList = VendorList.OrderByDescending(c => c.LastLogin).ToList();
                        break;
                    case "CompanyName":
                        VendorList = VendorList.OrderBy(c => c.CompanyName).ToList();
                        break;
                    case "CompanyName_desc":
                        VendorList = VendorList.OrderByDescending(c => c.CompanyName).ToList();
                        break;
                    case "Department":
                        VendorList = VendorList.OrderBy(c => c.Department).ToList();
                        break;
                    case "Department_desc":
                        VendorList = VendorList.OrderByDescending(c => c.Department).ToList();
                        break;
                    default:
                        VendorList = VendorList.OrderBy(c => c.Name).ToList();
                        break;
                }
            }

            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);

            return View(VendorList.ToPagedList(pageNumber, pageSize));

            //return View(VendorList);
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult CoAdmin(int? page, int PageSize = 10)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            var EmployeeList = cmsMgr.GetSubscriberWiseCoAdminList(userdetails.SubscriberId).Where(c => c.DepartmentId == "ADI").ToList();

            PopulatePaging(PageSize);

            ViewBag.Paging = PageSize;

            int pageSize = PageSize;
            int pageNumber = (page ?? 1);

            return View(EmployeeList.ToPagedList(pageNumber, pageSize));
            //return View(EmployeeList);
        }

        public ActionResult CoAdminProfile(string CorporateId)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewData["Profile"] = userContext.CorporateProfile.Find(CorporateId);
            ViewData["Address"] = userContext.Address.Where(u => u.CorporateId == CorporateId).ToList();

            return View(generic.GetUserDetail(CorporateId));
        }

        [HttpPost]
        public ActionResult GetBatch(string CourseCode)
        {
            List<SelectListItem> BatchId = new List<SelectListItem>();
            string SubscriberId = User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(CourseCode))
            {
                List<CourseBatch> Batches = (from b in userContext.CourseBatch
                                             join c in userContext.CourseMaster
                                             on b.CourseCode equals c.CourseCode
                                             where b.CourseCode == CourseCode && c.SubscriberId == SubscriberId
                                             select b).ToList();
                Batches.ForEach(x =>
                {
                    BatchId.Add(new SelectListItem { Text = x.BatchName, Value = x.BatchId.ToString() });
                });
            }
            return Json(BatchId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult MeetingMinutes(string MeetingHost, bool data = false)
        {

            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.Result = "Failed";
            if (data == true)
            {
                ViewBag.Result = "Succeeded";
            }
            //ViewBag.Participants = new MultiSelectList(generic.GetSubscriberWiseList(userdetails.SubscriberId), "UserId", "NameWithId");
            PopulateAdminMember(userdetails.SubscriberId);
            var meeting = cmsMgr.GetMeetingMinutes(MeetingHost);

            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult MeetingMinutes(DateTime? MMDate, string MeetingSubject, string MeetingDate, string MeetingRemarks, string MeetingHost, string Participants, string SubscriberId, string InternalRemarks, string Location, Int64 MeetingId = 0)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            PopulateAdminMember(userdetails.UserId);
            //var meetingDate = Convert.ToDateTime(MeetingDate);
            MMDate = null;
            if (!String.IsNullOrEmpty(MeetingDate))
            {
                MMDate = DateTime.ParseExact(MeetingDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            bool result = cmsMgr.AddMeetingMinutes(MeetingId, MeetingSubject, MMDate, MeetingRemarks, userdetails.UserId, Participants, userdetails.SubscriberId, InternalRemarks, DateTime.UtcNow, userdetails.UserId, Location);

            return RedirectToAction("MeetingMinutes", "User", new { Area = "CMS", data = result });
        }

        public ActionResult MeetingMinutesDetails(string MeetingHost, bool data = false, Int64 Id = 0)
        {
            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(UserId).FirstOrDefault();

            var details = cmsMgr.GetMeetingMinutes(null, Id).FirstOrDefault();
            return View(details);
        }

        public ActionResult MeetingMinutesListing(string MeetingHost, bool data = false)
        {
            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewData["Meeting"] = cmsMgr.GetMeetingMinutes(UserId).OrderByDescending(c => c.MeetingDate).ToList();

            return View();
        }

        public ActionResult Co_Admin(string SubscriberId)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            var coAdminList = admin.GetCoAdminList(userdetails.SubscriberId).Where(c => c.Department == "Admin" && c.SubscriberId != c.CorporateId).ToList();
            if (coAdminList.Count() > 0)
            {
                coAdminList = coAdminList.OrderBy(l => l.Name).ToList();
            }
            return View(coAdminList);
        }

        [HttpPost]
        public ActionResult EmailPhoneExist(string Email, string PhoneNumber, string Uid)
        {
            LoginViewModel login = null;
            if (!String.IsNullOrEmpty(Email))
            {
                login = admin.GetLoginDetails(Email);
                if (login != null)
                {
                    if (!String.IsNullOrEmpty(Uid))
                    {
                        var user = UserManager.FindById(Uid);
                        if (user.Email == Email)
                        {
                            return Json(null, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("Email already exist for a different user.", JsonRequestBehavior.AllowGet);
                        }
                    }

                    return Json("Email already exist.", JsonRequestBehavior.AllowGet);
                }
            }

            if (!String.IsNullOrEmpty(PhoneNumber))
            {
                login = admin.GetLoginDetails(PhoneNumber);
                if (login != null)
                {
                    if (!String.IsNullOrEmpty(Uid))
                    {
                        var user = UserManager.FindById(Uid);
                        if (user.PhoneNumber == PhoneNumber)
                        {
                            return Json(null, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("Phone Number already exist for a different user.", JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json("Phone Number already exist.", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ActivateDeactivateUser(string UId, string action, string Role)
        {
            if (!String.IsNullOrEmpty(action))
            {
                return Json(cmsMgr.ActivateDeactivateUser(UId, action, Role), JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GeneratePassword(string UserId, string Name, string oldPassword)
        {
            string newPassword = admin.GeneratePassword(Name);

            if (oldPassword == newPassword)
            {
                return Json(newPassword, JsonRequestBehavior.AllowGet);
            }

            var result = await UserManager.ChangePasswordAsync(UserId, oldPassword, newPassword);
            if (result.Succeeded)
            {
                //Save password to db
                var res = cmsMgr.UpdatePassword(UserId, newPassword);
                return Json(res ? newPassword : null, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SendEmailWithNewPwd(string UserId, string Email, string newPwd, string Name)
        {
            var msgBody = "Dear USER (" + Name + "), <br/> <br/> Greetings from RECKONN! <br/> <br/> Your password has been reset. Your new password is : " + newPwd + "<br/> <br/> " +
                           "Please login with your new password. <br><br>RECKONN ";
            await UserManager.SendEmailAsync(UserId, "Reset Password", msgBody);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// created by:preeti singh
        /// date:21/08/2017
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEmployee(string Uid, string UserAction, string ManagerLevel, string PhysicallyChallenged, string Emplanelled)
        {
            int year = DateTime.Now.Year;
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            var empdetails = emsMgr.GetEmployeeBasicDetails(userDetail.UserId).FirstOrDefault();
            ViewData["EmpDetails"] = empdetails;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            ViewBag.Uid = Uid;
            ViewBag.UserAction = UserAction;
            EmployeeView emp = new EmployeeView();

            ViewData["EngagementType"] = userContext.EngagementTypeMaster.ToList();
            var companySetting = userContext.CompanySetting.Where(c => c.CorporateId == userDetail.SubscriberId).FirstOrDefault();
            if (companySetting != null)
            {
                emp.ProbationPeriod = Convert.ToInt16(companySetting.ProbationPeriod);

                if (companySetting.CalendarYear == "Apr-Mar" && DateTime.Now.Month <= 3)
                {
                    year = year - 1;
                }
            }
            ViewBag.Year = year;
            if (Uid != null)
            {
                emp = admin.GetsEmployeeDetail(Uid);
                if (emp != null)
                {
                    PopulateGenderStatus(emp.Gender);
                    PopulateMaritalStatus(emp.MaritalStatus);
                    PopulateNationality(emp.Nationality);
                    PopulateDesignation(emp.DesignationId);
                    PopulateGrade(emp.GradeId);
                    PopulateDepartment("EMS", emp.DepartmentId, emp.DepartmentId);
                    PopulateStatusMaster(emp.StatusId);
                    PopulateManager(userDetail.SubscriberId, emp.ReportingAuthority);
                    //PopulateSchema(emp.SchemeId);
                    PopulateShift(userDetail.SubscriberId, emp.ShiftId);
                    PopulateMonth(emp.PayoutMonth);
                    PopulateBranches(userDetail.SubscriberId, emp.BranchId);
                    ViewBag.DOB = emp.DOB != null ? Convert.ToDateTime(emp.DOB).ToString("dd-MM-yyyy") : "";
                    ViewBag.MarriageDate = emp.MarriageDate != null ? Convert.ToDateTime(emp.MarriageDate).ToString("dd-MM-yyyy") : "";
                    ViewBag.JoiningDate = emp.JoiningDate != null ? Convert.ToDateTime(emp.JoiningDate).ToString("dd-MM-yyyy") : "";
                    ViewBag.ConfirmationDate = emp.ConfirmationDate != null ? Convert.ToDateTime(emp.ConfirmationDate).ToString("dd-MM-yyyy") : "";
                    ViewBag.ManagerLevel = Convert.ToBoolean(emp.ManagerLevel);
                    ViewBag.PhysicallyChallenged = Convert.ToBoolean(emp.PhysicallyChallenged);
                    ViewBag.Emplanelled = Convert.ToBoolean(emp.Emplanelled);

                    //  var employeeSalaryDetails = userContext.EmployeeSalary.Where(s => s.UserId == Uid).ToList();

                    //ViewBag.EngagementData = "NA";

                    ViewData["EmpEngagement"] = emsMgr.GetEmployeeLeaveSummary(userDetail.SubscriberId, year, emp.SchemeId, null, Uid);

                }

            }
            else
            {
                PopulateGenderStatus();
                PopulateMaritalStatus();
                PopulateNationality();
                PopulateDesignation();
                PopulateGrade();
                PopulateDepartment("EMS");
                PopulateStatusMaster();
                PopulateManager(userDetail.SubscriberId);
                //PopulateSchema();
                PopulateShift(userDetail.SubscriberId);
                PopulateMonth();
                PopulateBranches(userDetail.SubscriberId);
                ViewBag.ManagerLevel = Convert.ToBoolean(ManagerLevel);
                ViewBag.PhysicallyChallenged = Convert.ToBoolean(PhysicallyChallenged);
                ViewBag.Emplanelled = Convert.ToBoolean(Emplanelled);
            }
            return View(emp);
        }


        [HttpPost]
        public async Task<ActionResult> AddEmployee(EmployeeView emp, string Uid, string UserAction, string[] EngagementTypeId, string[] EngagementMaxLimit, string[] SalaryHeadItems)
        {
            string UserId = User.Identity.GetUserId();
            var userDetail = generic.GetUserDetail(UserId);
            var subscriberDetail = cmsMgr.GetCorporateProfile(UserId).FirstOrDefault();
            var empdetails = emsMgr.GetEmployeeBasicDetails(userDetail.UserId).FirstOrDefault();
            ViewData["EmpDetails"] = empdetails;
            //var scheme = emp.SchemeId;

            if (UserAction == "Edit")
            {
                emp.UserId = Uid;
                bool res = admin.AddEmployee(emp, userDetail.SubscriberId, UserId);
                if (res == true)
                {
                    emp.UserId = Uid;
                    bool jres = false;
                    EmpJoiningDetail empjoining = new EmpJoiningDetail();
                    empjoining.JoiningId = emp.JoiningId;
                    empjoining.UserId = emp.UserId;
                    empjoining.ConfirmationDate = emp.ConfirmationDate;
                    empjoining.JoiningDate = emp.JoiningDate;
                    empjoining.StatusId = emp.StatusId;
                    empjoining.ProbationPeriod = emp.ProbationPeriod;
                    empjoining.NoticePeriod = emp.NoticePeriod;
                    empjoining.GradeId = emp.GradeId;
                    //     empjoining.WorkLocation = emp.WorkLocation;
                    empjoining.BranchId = emp.BranchId;
                    empjoining.SchemeId = emp.SchemeId;
                    empjoining.ShiftId = emp.ShiftId;
                    jres = admin.EmployeeJoining(empjoining);
                }
            }
            else
            {
                string RoleId = "Employee";
                //  string savestatus = "";
                string UpdatedBy = "";
                // if (employee.UserId == null) { 
                var EmailExists = admin.GetLoginDetails(emp.Email);

                if (EmailExists != null)
                {
                    return Json("EmailExists", JsonRequestBehavior.AllowGet);
                }

                //Commented dur to IPPB Requirement
                //var ContactExists = admin.GetLoginDetails(emp.AlternateContact);
                //if (ContactExists != null)
                //{
                //    return Json("ContactExists", JsonRequestBehavior.AllowGet);
                //}

                string userName = admin.GenerateUserName();
                var UserExists = admin.GetLoginDetails(userName);
                if (UserExists != null)
                {
                    return Json("UserExists", JsonRequestBehavior.AllowGet);
                }

                var user = new ApplicationUser { UserName = userName, Email = emp.Email, PhoneNumber = emp.PhoneNumber, EmailConfirmed = true };
                var result = await UserManager.CreateAsync(user, "changeme");
                var status = UserManager.AddToRole(user.Id, RoleId);
                emp.UserId = user.Id;
                emp.Emplanelled = false;
                if (status.Succeeded)
                {
                    UpdatedBy = User.Identity.GetUserId();
                    // savestatus = "Succeeded";
                    bool res = admin.AddEmployee(emp, userDetail.SubscriberId, UpdatedBy);
                    if (RoleId.ToUpper() == "EMPLOYEE")
                        emsMgr.AddProfileTypeDetails(0, user.Id, "Default");

                    //Sending Email To User
                    //string callbackUrl = await SendEmailConfirmationTokenAsync(subscriberDetail.Name, user.Id, "Account activation", userName, emp.AlternateContact, emp.Name);

                    //Sending SMS To User
                    //string mobile = emp.AlternateContact;
                    //string message1 = "Hello" + emp.Name + ",  you are added as a Employee in RECKONN by " + generic.GetUserDetail(userDetail.SubscriberId).Name;              
                    //generic.sendSMSMessage(message1, mobile);


                    if (res == true)
                    {
                        emp.UserId = user.Id;
                        bool jres = false;
                        EmpJoiningDetail empjoining = new EmpJoiningDetail();
                        empjoining.JoiningId = emp.JoiningId;
                        empjoining.UserId = emp.UserId;
                        if (empjoining.ConfirmationDate != null)
                        {
                            empjoining.ConfirmationDate = Convert.ToDateTime(emp.ConfirmationDate.ToString());
                        }
                        empjoining.ConfirmationDate = emp.ConfirmationDate;
                        empjoining.JoiningDate = Convert.ToDateTime(emp.JoiningDate);
                        empjoining.StatusId = emp.StatusId;
                        empjoining.ProbationPeriod = emp.ProbationPeriod;
                        empjoining.NoticePeriod = emp.NoticePeriod;
                        empjoining.GradeId = emp.GradeId;
                        empjoining.BranchId = emp.BranchId;
                        empjoining.SchemeId = emp.SchemeId;
                        empjoining.ShiftId = emp.ShiftId;
                        jres = admin.EmployeeJoining(empjoining);
                    }
                    else
                    {
                        return Json("Unsucceeded", JsonRequestBehavior.AllowGet);
                    }
                }
                //}
                //else
                //{
                //    //User Edit Mode
                //    var regUser = UserManager.FindById();
                //    if ((String.IsNullOrEmpty(regUser.Email) || (!String.IsNullOrEmpty(regUser.Email) && regUser.Email != Email.Trim())) ||
                //        (String.IsNullOrEmpty(regUser.PhoneNumber) || (!String.IsNullOrEmpty(regUser.PhoneNumber) && regUser.PhoneNumber != PhoneNumber.Trim())))
                //    {
                //        bool result = admin.UpdateUserEmailPhone(regUser.UserName, Email, PhoneNumber, true);
                //    }

                //    string ModuleAccess = "SMS";
                //    string RoleId = "Candidate";
                //    string Department = "CAN";
                //    if (Mod == "EMP")
                //    {
                //        RoleId = "Employee";
                //        Department = DepartmentId;
                //        var managerLevel = ManagerLevel;
                //        var reportingAuthority = ReportingAuthority;
                //        UpdatedBy = User.Identity.GetUserId();
                //        var update = admin.UpdateUserRegistration(Uid, Name, Department, ManagerLevel, ReportingAuthority, DateTime.UtcNow, UpdatedBy, Status, dateofJoining, ProbationPeriod, GradeID, DateofConfirmation, Designation);
                //        savestatus = "EditSucceeded";
                //    }

                //}
                ////PopulateRoleAccessLevel();
                //return RedirectToAction("Add", "User", new { area = "CMS", Id = Mod, savestatus });

                return RedirectToAction("AddEmployee", "User");
            }
            return RedirectToAction("AddEmployee", "User");
        }

        //created by vikas pandey
        //24/11/2017
        //for CompanySettings
        public ActionResult CompanySetting()
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            string corporateId = userDetail.CorporateId;
            PopulateCurrency();
            PopulateCalendarYear();
            PopulatePayrollProcessDate();
            PopulatePayrollperweekday();
            var setting = userContext.CompanySetting.Where(q => q.CorporateId == corporateId).FirstOrDefault();
            if (setting != null)
            {
                ViewBag.SettingId = setting.SettingId;
                PopulateCurrency(setting.DefaultCurrency);
                PopulateCalendarYear(setting.CalendarYear);
                PopulatePayrollProcessDate(setting.AutoProcessPayrollDate);
                PopulatePayrollperweekday(setting.WorkingDayPerWeek);
            }

            return View(setting);
        }

        [HttpPost]
        public ActionResult CompanySetting(Int16 WorkingDayPerWeek, Int32 ProbationPeriods, string DefaultCurrency, string CalendarYear,
        bool PayslipPasswordProtaction, bool AutoeanblePayrollProcess, bool CompanyAttendance, bool LeavesCalculationCriteria, Int64 SettingId = 0, Int16 AutoProcessPayrollDate = 32)
        {
            string CorporateId = User.Identity.GetUserId();
            admin.AddCompanySetting(SettingId, CorporateId, WorkingDayPerWeek, ProbationPeriods, DefaultCurrency, CalendarYear, PayslipPasswordProtaction, AutoeanblePayrollProcess, AutoProcessPayrollDate, CompanyAttendance, LeavesCalculationCriteria);
            return RedirectToAction("CompanySetting", "User", new { area = "CMS" });
        }
        [HttpPost]
        public ActionResult GetPayrollHeads(string SubscriberId, float MonthlyCTC)
        {
            var PayrollHead = userContext.CorporatePayrollHead.Where(c => c.CorporateId == SubscriberId && c.PayrollHeadName != "TDS" && c.PayrollHeadName != "LoP").ToList();

            List<EmployeeSalaryHeadView> empSalary = new List<EmployeeSalaryHeadView>();
            if (PayrollHead.Count > 0)
            {
                //Calculation of Basic based on monthly CTC
                var CorporateBasichead = PayrollHead.Where(c => c.PayrollHeadID == Global.BasicHeadId()).FirstOrDefault();
                float basic = 0;

                basic = ((MonthlyCTC * CorporateBasichead.PayrollPercent) / 100);

                empSalary.Add(new EmployeeSalaryHeadView { CorporatePayrollHeadID = CorporateBasichead.CorporatePayrollHeadID, PayrollHeadName = CorporateBasichead.PayrollHeadName, Amount = basic });

                float total = basic;
                foreach (var item in PayrollHead.Where(c => c.PayrollHeadID != Global.BasicHeadId() && c.PayrollHeadID != Global.SpecialAllowancesHeadId()).ToList())
                {
                    float amount = 0;
                    float FirstAmount = 0;
                    if (item.PayrollHeadID == Global.BasicHeadId())
                        FirstAmount = basic;
                    else
                        FirstAmount = (basic * item.PayrollPercent) / 100;

                    if (item.PayrollPercent == 0)
                    {
                        amount = item.MaxLimit;
                    }
                    else if (item.PayrollPercent > 0 && item.MaxLimit > 0 && FirstAmount > item.MaxLimit)
                    {
                        amount = item.MaxLimit;
                    }
                    else
                    {
                        amount = FirstAmount;
                    }
                    total = total + amount;
                    empSalary.Add(new EmployeeSalaryHeadView { CorporatePayrollHeadID = item.CorporatePayrollHeadID, PayrollHeadName = item.PayrollHeadName, Amount = amount });
                }
                //Calculation of Special Allowances

                var SpecialHead = PayrollHead.Where(c => c.PayrollHeadID == Global.SpecialAllowancesHeadId()).FirstOrDefault();
                float splAllowances = MonthlyCTC - total;

                empSalary.Add(new EmployeeSalaryHeadView { CorporatePayrollHeadID = SpecialHead.CorporatePayrollHeadID, PayrollHeadName = SpecialHead.PayrollHeadName, Amount = splAllowances });
            }

            return Json(empSalary, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetPayrollHeadsBasedOnBasic(string SubscriberId, float Basic, float SpecialAmt)
        {
            var PayrollHead = userContext.CorporatePayrollHead.Where(c => c.CorporateId == SubscriberId && c.PayrollHeadName != "TDS" && c.PayrollHeadName != "LoP").ToList();


            List<EmployeeSalaryHeadView> empSalary = new List<EmployeeSalaryHeadView>();
            if (PayrollHead.Count > 0)
            {
                var CorporateBasichead = PayrollHead.Where(c => c.PayrollHeadID == Global.BasicHeadId()).FirstOrDefault();
                float basic = 0;

                basic = Basic;

                float CTC = basic;

                empSalary.Add(new EmployeeSalaryHeadView { CorporatePayrollHeadID = CorporateBasichead.CorporatePayrollHeadID, PayrollHeadName = CorporateBasichead.PayrollHeadName, Amount = basic });

                foreach (var item in PayrollHead.Where(c => c.PayrollHeadID != Global.BasicHeadId() && c.PayrollHeadID != Global.SpecialAllowancesHeadId()).ToList())
                {
                    float amount = 0;
                    float FirstAmount = 0;
                    if (item.PayrollHeadID == Global.BasicHeadId())
                        FirstAmount = basic;
                    else
                        FirstAmount = (basic * item.PayrollPercent) / 100;

                    if (item.PayrollPercent == 0)
                    {
                        amount = item.MaxLimit;
                    }
                    else if (item.PayrollPercent > 0 && item.MaxLimit > 0 && FirstAmount > item.MaxLimit)
                    {
                        amount = item.MaxLimit;
                    }
                    else
                    {
                        amount = FirstAmount;
                    }
                    CTC = CTC + amount;
                    empSalary.Add(new EmployeeSalaryHeadView { CorporatePayrollHeadID = item.CorporatePayrollHeadID, PayrollHeadName = item.PayrollHeadName, Amount = amount });
                }

                //Calculation of Special Allowances

                var SpecialHead = PayrollHead.Where(c => c.PayrollHeadID == Global.SpecialAllowancesHeadId()).FirstOrDefault();

                empSalary.Add(new EmployeeSalaryHeadView { CorporatePayrollHeadID = SpecialHead.CorporatePayrollHeadID, PayrollHeadName = SpecialHead.PayrollHeadName, Amount = SpecialAmt });


                CTC = CTC + SpecialAmt;
                empSalary.Add(new EmployeeSalaryHeadView { CorporatePayrollHeadID = 0, PayrollHeadName = "MonthlyCTC", Amount = CTC });
            }



            return Json(empSalary, JsonRequestBehavior.AllowGet);
        }

        private void PopulateCourse(string SubscriberId, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetCourseDetails(SubscriberId);
            SelectList CourseCode = new SelectList(query, "CourseCode", "CourseName", selectedValue);
            ViewBag.CourseCode = CourseCode;
        }

        private void PopulateBatch(string SubscriberId = null, string CourseCode = null, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetBatches(SubscriberId, CourseCode);
            SelectList BatchId = new SelectList(query, "BatchId", "BatchName", selectedValue);
            ViewBag.BatchId = BatchId;
        }

        private void PopulateDepartment(string ModuleId, string DepartmentId = null, object selectedDepartment = null)
        {
            Generic generic = new Generic();
            var query = generic.GetModuleWiseDepartments(ModuleId);
            if (ModuleId == "CMS")
                if (!string.IsNullOrEmpty(DepartmentId))
                {
                    query = query.Where(q => q.DepartmentId == DepartmentId).ToList();
                }
                else
                    query = query.Where(q => q.DepartmentId == "CLI").ToList();

            SelectList Departments;
            if (!string.IsNullOrEmpty(DepartmentId))
                Departments = new SelectList(query, "DepartmentId", "Department", selectedDepartment);
            else
                Departments = new SelectList(query, "DepartmentId", "RoleDepartment", selectedDepartment);

            ViewBag.DepartmentId = Departments;
        }

        private void PopulatethirdPartyDepartment(string ModuleId, string DepartmentId = null, object selectedDepartment = null)
        {
            Generic generic = new Generic();
            var query = generic.GetModuleWiseDepartments(ModuleId);
            if (ModuleId == "CMS")
                query = query.Where(q => q.DepartmentId != "CLI" && q.DepartmentId != "CPT").ToList();

            SelectList Departments;
            if (!string.IsNullOrEmpty(DepartmentId))
                Departments = new SelectList(query, "DepartmentId", "Department", selectedDepartment);
            else
                Departments = new SelectList(query, "DepartmentId", "Department", selectedDepartment);

            ViewBag.DepartmentId = Departments;
        }

        private void PopulateManager(string SubscriberId, object selectedValue = null)
        {
            EMSManager emsMgr = new EMSManager();
            var ReportingAuthority = emsMgr.GetSubscriberWiseEmployeeList(SubscriberId).Where(e => e.ManagerLevel == true).ToList();
            ViewBag.ReportingAuthority = new SelectList(ReportingAuthority, "UserId", "Name", selectedValue);
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
        //Reset pasword by admin
        //by:vikas pandey
        //19-may-2018
        public ActionResult ResetPassword(string status = null)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            if (status != null)
            {
                ViewBag.status = status;
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(string UserName, string NewPassword, string ConfirmPassword)
        {

            if (ModelState.IsValid)
            {

                if (NewPassword == ConfirmPassword)
                {

                    var UserId = cmsMgr.GetUserIdByReckonnId(UserName);
                    var removePassword = UserManager.RemovePassword(UserId.Id);
                    var result = await UserManager.AddPasswordAsync(UserId.Id, NewPassword);
                    if (result.Succeeded)
                    {
                        var user = await UserManager.FindByIdAsync(UserId.Id);

                        //if (user != null)
                        //{
                        //    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        //}
                        return Json("Password Changed SUCCESSFULLY.", JsonRequestBehavior.AllowGet);

                    }


                    //AddErrors(result);
                }
                else
                {
                    return Json("New Password & Confirm Password Not Matched.", JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction("ResetPassword", "User", new { area = "CMS" });

        }
        [HttpPost]
        public ActionResult UserExist(string UserName)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            var UserId = cmsMgr.GetUserIdByReckonnId(UserName);


            if (UserId != null)
            {
                var UserNameDetail = generic.GetUserDetail(UserId.Id);
                if (UserNameDetail.SubscriberId == userDetail.UserId)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Reckonn ID is not exist.", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Invalid UserName.", JsonRequestBehavior.AllowGet);
            }
        }
        private void PopulatePaging(object selectedValue = null)
        {
            var PageList = generic.GetPaging();
            ViewBag.PageSize = new SelectList(PageList, "PageSize", "PageSize", selectedValue);
        }
        private void PopulateNationality(object selectedNationality = null)
        {
            var nationalStatusQuery = generic.GetNationalityList();
            SelectList Nlist = new SelectList(nationalStatusQuery, "Nationality", "Nationality", selectedNationality);
            ViewBag.Nationality = Nlist;
        }
        private void PopulateCompanyType(object selectedValue = null)
        {
            var CompanyTypeList = generic.GetCompanyType();
            ViewBag.CompanyType = new SelectList(CompanyTypeList, "CType", "CType", selectedValue);
        }

        private void PopulateCompanySize(object selectedValue = null)
        {
            var CompanySizeList = generic.GetCompanySize();
            ViewBag.CompanySize = new SelectList(CompanySizeList, "CompanySize", "CompanySize", selectedValue);
        }

        private void PopulateAdminMember(string SubscriberId, object selectedOrderType = null)
        {
            var query = generic.GetSubscriberWiseList(SubscriberId);
            SelectList Participants = new SelectList(query, "UserId", "NameWithId", selectedOrderType);
            ViewBag.Participants = Participants;
        }

        private void PopulateGenderStatus(object selectedValue = null)
        {
            var GenderList = Global.GetGenderList();
            SelectList Gender = new SelectList(GenderList, "Genderid", "Gender", selectedValue);
            ViewBag.Gender = Gender;
        }
        ///Updated by : Achal Jha
        ///updated On : 01/06/2017
        ///Reason     : Payroll


        private void PopulateEmployementStatus(object selectedValue = null)
        {
            var EmployementStatus = Global.EmployeementStatus();
            SelectList EmployementStat = new SelectList(EmployementStatus, "Status", "Status", selectedValue);
            ViewBag.Status = EmployementStat;
        }
        /// <summary>
        /// created by:preeti singh
        /// date:21/08/2017
        /// </summary>
        /// <param name="selectedValue"></param>
        private void PopulateMaritalStatus(object selectedValue = null)
        {
            var MaritalList = Global.GetMaritalList();
            SelectList MaritalStatus = new SelectList(MaritalList, "MaritalStatus", "MaritalStatus", selectedValue);
            ViewBag.MaritalStatus = MaritalStatus;
        }
        //private void PopulateDesignation(string CorporateId)
        //{
        //    var designation = userContext.Designation.Where(a => a.CorporateId == CorporateId);
        //    SelectList Designation = new SelectList(designation, "DesignationId", "DesignationName");
        //    ViewBag.Designation = Designation;
        //}

        private void PopulateDesignation(object selectedvalue = null)
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            var query = tms.GetDesignation(userDetails.SubscriberId);
            SelectList DesignationList = new SelectList(query, "DesignationId", "DesignationName", selectedvalue);
            ViewBag.DesignationId = DesignationList;
        }

        private void PopulateGrade(object selectedValue = null)
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            var GradeList = admin.GetGrade(userDetails.SubscriberId);
            SelectList EmployementGrade = new SelectList(GradeList, "GradeId", "GradeName", selectedValue);
            ViewBag.GradeId = EmployementGrade;
        }

        private void PopulateStatusMaster(object selectedvalue = null)
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            var query = tms.GetStatusMaster(userDetails.SubscriberId);
            SelectList StatusMasterList = new SelectList(query, "StatusId", "StatusName", selectedvalue);
            ViewBag.StatusId = StatusMasterList;
        }

        private void PopulateSchema(object selectedvalue = null, Int16 SchemeId = 0)
        {
            var query = tms.GetSchemaList(SchemeId);
            SelectList SchemaList = new SelectList(query, "SchemeId", "SchemeName", selectedvalue);
            ViewBag.SchemeId = SchemaList;
        }

        private void PopulateShift(string UserId, object selectedvalue = null, Int64 ShiftId = 0)
        {
            var query = tms.GetShiftDetailList(UserId, ShiftId);
            SelectList shiftList = new SelectList(query, "ShiftId", "Shift", selectedvalue);
            ViewBag.ShiftId = shiftList;
        }

        private void PopulateCurrency(Object selectedCurrency = null)
        {
            var query = generic.GetCurrency();
            SelectList CurrencyList = new SelectList(query, "Currency", "Currency", selectedCurrency);
            ViewBag.Currency = CurrencyList;
        }
        //created by vikas pandey
        //24/11/2017
        private void PopulateCalendarYear(object selectedvalue = null)
        {
            var calyear = Global.GetCalendarYear();
            SelectList Callist = new SelectList(calyear, "CalendarYear", "CalendarYear", selectedvalue);
            ViewBag.CalendarYear = Callist;

        }
        //created by vikas pandey
        //24/11/2017
        private void PopulatePayrollProcessDate(object selectedvalue = null)
        {
            var Payrolldate = Global.GetPayrollProcessDate();
            SelectList Paydatelist = new SelectList(Payrolldate, "PayrollDate", "PayrollDatetext", selectedvalue);
            ViewBag.Payrolldate = Paydatelist;

        }

        private void PopulatePayrollperweekday(object selectedvalue = null)
        {
            var perweekday = Global.GetPayrollPerweekDay();
            SelectList perweekdaylist = new SelectList(perweekday, "PayrollworkDay", "PayrollworkDay", selectedvalue);
            ViewBag.perweekdaylist = perweekdaylist;

        }

        private void PopulateMonth(object selectedvalue = null)
        {
            Generic generic = new Generic();
            var MonthList = generic.GetMonths().ToList();
            ViewBag.MonthList = new SelectList(MonthList, "MonthId", "MonthName", selectedvalue);
        }


        private void PopulateNIBFReference(string SubscriberId, object selectedOrderType = null)
        {
            var query = generic.GetSubscriberWiseList(SubscriberId);
            query = query.Where(c => c.DepartmentId == "SRQ" || c.DepartmentId == "BPT").ToList();
            SelectList Reference = new SelectList(query, "UserId", "NameWithId", selectedOrderType);
            ViewBag.Reference = Reference;
        }

        public ActionResult GetEngagement(Int64 SchemeId, string Gender)
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            var EngagementData = userContext.EngagementTypeMaster.Where(c => c.CorporateId == userDetails.SubscriberId
                                 && c.SchemeId == SchemeId && c.LeaveTypeCategory == "D"
                                 && c.LeaveTypeId != "LW" && c.LeaveTypeId != "BT").ToList();
            if (Gender == "MA")
                EngagementData = EngagementData.Where(e => e.LeaveTypeId != "ML").ToList();
            else if (Gender == "FE")
                EngagementData = EngagementData.Where(e => e.LeaveTypeId != "PL").ToList();
            else
                EngagementData = EngagementData.Where(e => e.LeaveTypeId != "PL" && e.LeaveTypeId != "ML").ToList();

            if (EngagementData.Count > 0)
            {
                return Json(EngagementData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string NOEngagementData = "No Engagement Data Found";
                return Json(NOEngagementData, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetEmployeeEngagementLimit(Int16 SchemeId, Int32 year, string UId)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            var EngagementData = emsMgr.GetEmployeeLeaveSummary(userDetail.SubscriberId, year, SchemeId, null, UId);

            if (EngagementData.Count > 0)
            {
                return Json(EngagementData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string NOEngagementData = "No Engagement Data Found";
                return Json(NOEngagementData, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult ConfirmEmployee(string UserId)
        {
            PopulateConfirmationtype();
            var EmployeeList = emsMgr.GetNotifiedEmployee(UserId);
            var confirmation = userContext.EmployeeConfirmation.Where(c => c.UserId == UserId).FirstOrDefault();
            if (confirmation != null)
            {
                ViewBag.isconfirm = true;
            }
            return View(EmployeeList);

        }
        [HttpPost]
        public ActionResult ConfirmEmployee(string UserId, string Remark = null, Int16 Days = 0, Int16 Status = 0)
        {
            string UseApprovedBy = User.Identity.GetUserId();
            bool result = admin.AddEmployeeConfirmation(UserId, UseApprovedBy, Status, Remark);
            if (result == true && Status == 1)
            {
                var query = (from q in userContext.EmpJoiningDetail
                             where q.UserId == UserId
                             select q).FirstOrDefault();
                query.SchemeId = 4;
                userContext.SaveChanges();
            }
            else if (result == true && Status == 3)
            {
                var query = (from q in userContext.EmpJoiningDetail
                             where q.UserId == UserId
                             select q).FirstOrDefault();
                var extendeddate = query.ConfirmationDate.Value.AddDays(Days);
                query.ConfirmationDate = extendeddate;
                userContext.SaveChanges();
            }
            return RedirectToAction("Notifications", "UserNotification", new { area = "" });
        }
        public ActionResult DownloadOfferletter(string SubscriberId, string UId = null)
        {

            string UserId = User.Identity.GetUserId();
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Views/Report/Offerlatter.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            if (string.IsNullOrEmpty(UId))
            {
                cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@UserId", UId));
            }
            cmd.Parameters.Add(new SqlParameter("@SubscriberId", SubscriberId));
            cmd.Connection = thisConnection;

            string MyDataSource1 = "USP_GetEmpConfirmation";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];
            ReportParameter[] parms = new ReportParameter[2];
            parms[0] = new ReportParameter("UserId", UserId);
            parms[1] = new ReportParameter("SubscriberId", SubscriberId);
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
            Response.AddHeader("content-disposition", "attachment; filename=ConfirmationLetter.pdf");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            return View();

        }
        private void PopulateConfirmationtype(object selectedvalue = null)
        {
            var operation = Global.GetConfrimationOperations().ToList();
            ViewBag.ConfirmationOperation = new SelectList(operation, "Status", "Operation", selectedvalue);
        }

        private void PopulateBranches(string CorporateId, object selectedvalue = null)
        {
            var operation = userContext.CorporateBranch.Where(c => c.CorporateId == CorporateId).ToList();
            ViewBag.BranchId = new SelectList(operation, "BranchId", "BranchName", selectedvalue);
        }
    }
}