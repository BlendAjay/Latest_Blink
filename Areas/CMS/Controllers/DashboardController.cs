using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.DAL;
using AJSolutions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using AJSolutions.Areas.CMS.Models;
namespace AJSolutions.Areas.CMS.Controllers
{
    //[Authorize]
    public class DashboardController : Controller
    {

        CMSManager cms = new CMSManager();
        Student student = new Student();
        TMSManager tms = new TMSManager();
        Generic generic = new Generic();
        UserDBContext udb = new UserDBContext();
        AdminManager admin = new AdminManager();
        EMSManager ems = new EMSManager();
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

        // GET: CMS/Dashboard
        //[Authorize(Roles = "Admin")]
        public ActionResult Index(bool IsTrail = false)
        {

            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewBag.UserId = userdetails.UserId;
            //var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            //ViewData["plandetail"] = plandetail;
            var myTraining = tms.GetTrainingSchedule(userdetails.SubscriberId, "NA");
            ViewData["myTraining"] = myTraining.ToList();
            //ViewData["myTraining"] = myTraining.Where(c => c.Status == "Inprogress").Take(4).ToList();
            //var details = student.GetCandidatePaymentTransaction(userdetails.SubscriberId).Where(c => c.Status != "Initiate");
            //ViewData["FeeDetails"] = details.Take(4).ToList();
            //var userplan = admin.GetUserplanDetails(userdetails.SubscriberId).OrderByDescending(c => c.UserPlanId).FirstOrDefault();
            //if (userplan == null || userplan.PlanEndDate < DateTime.UtcNow || userplan.Status != "Succeeded")
            //{
            //    return RedirectToAction("Plan", "Plan_Pricing", new { area = "Admin" });
            //}

            var getCount = admin.GetCountEntry(userdetails.SubscriberId);
            ViewBag.NotificationCount = admin.SPCountNotification(userdetails.SubscriberId).TOTALNOTIFICATION;
            ViewBag.CourseCount = admin.SPCountCourse(userdetails.SubscriberId).TOTALCOURSE;

            DateTime WeekFirstdate = DateTime.Now.Date.AddDays(0 * (Int32)DateTime.Now.DayOfWeek);
            DateTime LastWeekFirstdate = WeekFirstdate.Date.AddDays(-7 * (Int32)DateTime.Now.DayOfWeek);
            DateTime LastWeekLastdate = WeekFirstdate.Date.AddDays(-2 * (Int32)DateTime.Now.DayOfWeek);

            var TotalTransaction = student.GetCandidatePaymentTransaction(userdetails.SubscriberId).Where(c => c.Status == "Approved" && c.Status == "Succeeded");
            TotalTransaction = TotalTransaction.Where(c => c.PaymentDate >= WeekFirstdate && c.PaymentDate <= DateTime.Today).ToList();
            float CurrentWeektotal = TotalTransaction.Sum(c => c.FeePaid);
            ViewBag.WeekTransaction = CurrentWeektotal;

            var LastWeekTotalTransaction = student.GetCandidatePaymentTransaction(userdetails.SubscriberId).Where(c => c.Status == "Approved" && c.Status == "Succeeded");
            LastWeekTotalTransaction = LastWeekTotalTransaction.Where(c => c.PaymentDate >= LastWeekFirstdate && c.PaymentDate <= LastWeekLastdate).ToList();
            float LastWeektotal = LastWeekTotalTransaction.Sum(c => c.FeePaid);
            ViewBag.LastWeekTransaction = LastWeektotal;

            float avarage = (CurrentWeektotal * 100) / LastWeektotal;
            ViewBag.Average = Convert.ToString(avarage);
            return View(getCount);
            //return View();
        }

        //[Authorize]
        //[Authorize(Roles = "Client")]
        public ActionResult Client()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            //If Client has team members with all rights
            if (userdetails.CorporateId != null && userdetails.CorporateId != userdetails.SubscriberId)
            {
                userdetails.UserId = userdetails.CorporateId;
            }
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            //var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            //ViewData["plandetail"] = plandetail;
            var getCount = admin.GetRecordCountForClint(userdetails.UserId);
            ViewBag.NotificationCount = admin.SPCountNotification(userdetails.UserId).TOTALNOTIFICATION;
            ViewData["JobOrderStatus"] = cms.GetJobOrderStatusCount(userdetails.UserId);
            ViewData["InvoiceStatus"] = cms.GetInvoicetatusCount(userdetails.UserId);
            ViewData["TrainingStatus"] = cms.GetTrainingstatusCount(userdetails.UserId);

            return View(getCount);

        }

        //[Authorize(Roles = "Client")]
        public ActionResult Partner()
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var UserDetails = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["EmpDetails"] = UserDetails;
            //var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            //ViewData["plandetail"] = plandetail;
            ViewData["EmpInvoiceStatus"] = cms.GetEMPInvoicetatusCount(UserId);
            ViewData["TaskStatus"] = cms.GetTaskCount(UserId);
            ViewData["TrainingStatus"] = cms.GetTrainingCount(UserId);


            return View(admin.GetUserwiseTasksInvoicesAndTrainigsCount(UserId));
        }


        [HttpGet]
        //[Authorize(Roles = "Admin,Client")]
        public ActionResult MyProfile(bool status = false)
        {

            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }

            string UserId = User.Identity.GetUserId();
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            //var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            //ViewData["plandetail"] = plandetail;
            var corporateprofiles = cms.GetCorporateProfile(UserId).FirstOrDefault();
            if (corporateprofiles != null)
            {
                PopulateNationality(corporateprofiles.Nationality);
            }
            else
            {
                PopulateNationality();
            }
            return View(corporateprofiles);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Client")]
        public ActionResult MyProfile(CorporateProfile MyProfile)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewBag.UserId = userdetails.SubscriberId;
            //If Client has team members with all rights
            if (userdetails.CorporateId != null && userdetails.CorporateId != userdetails.SubscriberId)
            {
                var TeamDetails = cms.GetTeamMember(userdetails.CorporateId, User.Identity.GetUserId()).FirstOrDefault();
                bool result = cms.AddTeamMember(User.Identity.GetUserId(), userdetails.CorporateId, userdetails.SubscriberId, MyProfile.Name, MyProfile.AlternateEmail, MyProfile.AlternateContact, TeamDetails.EmpRoleId, TeamDetails.Designation, DateTime.UtcNow, User.Identity.GetUserId());
                return RedirectToAction("MyProfile", "Dashboard", new { area = "CMS", status = result });
            }
            else
            {
                bool result = cms.AddCorporateProfile(userdetails.UserId, MyProfile.Name, MyProfile.AlternateContact, MyProfile.AlternateEmail, MyProfile.Nationality, MyProfile.DepartmentId, MyProfile.SubscriberId, DateTime.UtcNow, userdetails.UserId);

                if (!string.IsNullOrEmpty(MyProfile.CorporateId))
                {
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
                        cms.uploadLogo(MyProfile.CorporateId, attachment);
                    }
                }
                return RedirectToAction("MyProfile", "Dashboard", new { area = "CMS", status = result });
            }
        }
        /// <summary>
        /// Updated by : Achal Jha
        /// Updated on : 17-05-2017
        /// Reason : Role : Employee for getting Employee Bank Details
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        /// ////////////
        [HttpGet]
        //[Authorize(Roles = "Admin,Client,Employee")]
        public ActionResult BankDetails(bool status = false)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userdetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            //var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            //ViewData["plandetail"] = plandetail;

            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            //if (userdetails.DepartmentId == "BPT")
            //{
            //    var bankdetails = cms.GetBankDetails(UserId);
            //    return View(bankdetails);
            //}
            //else if (userdetails.Role != "Admin")
            //{
            var bankdetails = cms.GetBankDetails(UserId);
            var empdetails = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["EmpDetails"] = empdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            return View(bankdetails);
            //}
            //else
            //{
            //    var bankdetails = cms.GetBankDetails(userdetails.SubscriberId);
            //    return View(bankdetails);
            //}
        }
        [HttpPost]
        //[Authorize(Roles = "Admin,Client,Employee")]
        public ActionResult BankDetails(BankDetails BankDetails, string BankName, string AccountNumber, string AccountOwner, string IfscCode, string BranchCode, string BranchAddress, string ContactNumber)
        {
            string CorporateId = User.Identity.GetUserId();
            bool result = cms.AddBankDetails(CorporateId, BankName, AccountNumber, AccountOwner, IfscCode, BranchCode, BranchAddress, ContactNumber);

            return RedirectToAction("BankDetails", "Dashboard", new { area = "CMS", status = result });
        }

        ///////////////
        [HttpGet]
        //[Authorize(Roles = "Admin,Client")]
        public ActionResult CompanyProfile(string Id, bool status = false)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewBag.UserId = userdetails.SubscriberId;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();

            //var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            //ViewData["plandetail"] = plandetail;

            ViewBag.Id = Id;
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }

            var companyprofiles = cms.GetCompanyProfile(userdetails.UserId);
            ViewData["AddressList"] = cms.GetAddressDetails(userdetails.SubscriberId);
            if (companyprofiles != null)
            {
                PopulateCompanyType(companyprofiles.CompanyType);
                PopulateCompanySize(companyprofiles.CompanySize);
            }
            else
            {
                PopulateCompanyType();
                PopulateCompanySize();
            }
            return View(companyprofiles);

        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Client")]
        public ActionResult CompanyProfile(CompanyProfile CompanyProfile, string CompanyName, string CompanyType, string CompanySize, string Website
                                           , string PANCardNo, string TaxDeductionAccNo, string GSTTax, string ProvidentFund = "0", string EmployeeStateInsurance = "0")
        {
            string CorporateId = User.Identity.GetUserId();
            bool result = cms.AddCompanyProfile(CorporateId, CompanyName, CompanyType, CompanySize, Website, DateTime.UtcNow, CorporateId,
                                                ProvidentFund, PANCardNo, TaxDeductionAccNo, EmployeeStateInsurance, GSTTax);

            return RedirectToAction("CompanyProfile", "Dashboard", new { area = "CMS", status = result });
        }


        //Address Details
        [HttpGet]
        //[Authorize(Roles = "Admin,Client")]
        public ActionResult AddressDetails(string result, string AddressType = "", string UserAction = "Add")
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userdetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            //var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            //ViewData["plandetail"] = plandetail;
            if (result == "Deleted")
            {
                ViewBag.Data = "Deleted";
            }

            ViewBag.UserAction = UserAction;

            var addressdetails = cms.GetAddressDetails(UserId);
            ViewData["AddressList"] = addressdetails;

            var address = addressdetails.Where(a => a.AddressType == AddressType).FirstOrDefault();

            if (address != null)
            {
                if (userdetails.SubscriberId == userdetails.UserId || userdetails.DepartmentId == "BPT")
                {
                    PopulateAddressType(address.AddressType);
                }
                else
                {
                    PopulateCoAdminAddressType(address.AddressType);
                }
                PopulateCountry(address.CountryId);
                PopulateState(address.CountryId, address.StateId);
                PopulateCity(address.StateId, address.CityId);
            }
            else
            {
                if (userdetails.SubscriberId == userdetails.UserId || userdetails.DepartmentId == "BPT")
                {
                    PopulateAddressType();
                }
                else
                {
                    PopulateCoAdminAddressType();
                }
                PopulateCountry();
                PopulateState();
                PopulateCity();
            }
            return View(address);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Client")]
        public ActionResult AddressDetails(string AddressType, string AddressLine1, string AddressLine2, string CityId, string StateId, string PostalCode, string CountryId, string FaxNo)
        {
            string CorporateId = User.Identity.GetUserId();
            int cityId = 0, stateId = 0, countryId = 0;
            if (!string.IsNullOrEmpty(CityId))
                cityId = Convert.ToInt32(CityId);
            if (!string.IsNullOrEmpty(StateId))
                stateId = Convert.ToInt32(StateId);
            if (!string.IsNullOrEmpty(CountryId))
                countryId = Convert.ToInt32(CountryId);

            cms.AddAddressDetails(CorporateId, AddressType, AddressLine1, AddressLine2, cityId, stateId, PostalCode, countryId, FaxNo);

            return RedirectToAction("AddressDetails", "Dashboard", new { area = "CMS" });
        }


        //[Authorize]
        public ActionResult RemoveAddressDetails(string AT)
        {
            string CorporateId = User.Identity.GetUserId();

            bool result = cms.RemoveAddressDetails(CorporateId, AT);

            return RedirectToAction("AddressDetails", "Dashboard", new { area = "CMS", result = "Deleted" });
        }
        // Task Master
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult TaskMaster()
        {
            PopulateAddressType("CO");
            string UserId = User.Identity.GetUserId();
            UserViewModel userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            //var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            //ViewData["plandetail"] = plandetail;
            var addressdetails = cms.GetAddressDetails(UserId);
            return View(addressdetails);
        }

        public ActionResult ClientDetails(string CorporateId)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            //var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            //ViewData["plandetail"] = plandetail;
            ViewBag.UserRole = GetUserRole(userdetails.SubscriberId);
            ViewData["Address"] = cms.GetAddressDetails(CorporateId);
            // ViewData["Address"] = udb.Address.Where(u => u.CorporateId == CorporateId).ToList();
            ViewData["Company"] = udb.CompanyProfile.Find(CorporateId);
            return View(generic.GetUserDetail(CorporateId));
        }

        public ActionResult ThirdPatyDetails(string CorporateId)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            //var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            //ViewData["plandetail"] = plandetail;
            ViewData["Address"] = cms.GetAddressDetails(CorporateId);
            ViewData["Company"] = udb.CompanyProfile.Find(CorporateId);
            return View(generic.GetUserDetail(CorporateId));
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult Branches()
        {

            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();

            var branch = udb.CorporateBranch.Where(c => c.CorporateId == userdetails.SubscriberId).ToList();
            ViewData["BranchList"] = branch;
            return View();
        }

        public ActionResult Branches(string BranchName, string CorporateId)
        {

            cms.AddCorporateBranch(BranchName, CorporateId);

            return RedirectToAction("Branches", "Dashboard", new { area = "CMS" });
        }



        private void PopulateAddressType(object selectedValue = null)
        {
            var AddressTypeList = Global.GetAddressType();
            ViewBag.addresstype = new SelectList(AddressTypeList, "addresstypeid", "addresstype", selectedValue);
        }

        private void PopulateCoAdminAddressType(object selectedValue = null)
        {
            var AddressTypeList = Generic.GetCoAdminAddressType();
            ViewBag.addresstype = new SelectList(AddressTypeList, "addresstypeid", "addresstype", selectedValue);
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

        [HttpPost]
        public ActionResult GetState(string CountryId)
        {
            int countryId;
            List<SelectListItem> StateId = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(CountryId))
            {
                countryId = Convert.ToInt32(CountryId);
                List<StatesMaster> States = udb.StatesMaster.Where(x => x.CountryId == countryId).ToList();
                States.ForEach(x =>
                {
                    StateId.Add(new SelectListItem { Text = x.State, Value = x.StateId.ToString() });
                });
            }
            return Json(StateId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCity(string StateId)
        {
            int stateId;
            List<SelectListItem> CityId = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(StateId))
            {
                stateId = Convert.ToInt32(StateId);
                List<CityMaster> Cities = udb.CityMaster.Where(x => x.StateId == stateId).ToList();
                Cities.ForEach(x =>
                {
                    CityId.Add(new SelectListItem { Text = x.City, Value = x.CityId.ToString() });
                });
            }
            return Json(CityId, JsonRequestBehavior.AllowGet);
        }

        private void PopulateNationality(object selectedNationality = null)
        {
            var nationalStatusQuery = generic.GetNationalityList();
            SelectList Nlist = new SelectList(nationalStatusQuery, "Nationality", "Nationality", selectedNationality);
            ViewBag.Nationality = Nlist;
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

        private string GetUserRole(string UserId)
        {
            if (!String.IsNullOrEmpty(UserId))
            {
                if (UserManager.IsInRole(UserId, "Admin"))
                    return "Admin";
                else if (UserManager.IsInRole(UserId, "Client"))
                    return "Client";
                else if (UserManager.IsInRole(UserId, "Employee"))
                    return "Employee";
            }
            return string.Empty;
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

        //
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