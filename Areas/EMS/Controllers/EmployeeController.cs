using AJSolutions.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AJSolutions.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Globalization;

namespace AJSolutions.Areas.EMS.Controllers
{
    public class EmployeeController : Controller
    {
        EMSManager ems = new EMSManager();
        CMSManager cms = new CMSManager();
        Generic generic = new Generic();
        UserDBContext udb = new UserDBContext();
        Student student = new Student();
        AdminManager admin = new AdminManager();
        private UserDBContext db = new UserDBContext();
        // GET: eMS/Employee
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Employee")]
        public ActionResult BasicDetails(DateTime? DOB, bool status = false)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            ViewData["Reporting"] = ems.GetSubscriberWiseEmployeeList(userdetails.SubscriberId).Where(r => r.UserId == UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            var empdetails = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["EmpDetails"] = empdetails;
            if (empdetails != null)
            {
                PopulateMaritalStatus(empdetails.MaritalStatus);
                PopulateGenderStatus(empdetails.Gender);
                PopulateNationality(empdetails.Nationality);
                if (empdetails.DOB != null)
                {
                    ViewBag.dob = empdetails.DOB.Value.ToString("dd-MM-yyyy");
                }
                //PopulateEmployementStatus(empdetails.Status);
                //PopulateGrade(empdetails.GradeId);
                //if (empdetails.DOB != null)
                //{
                //    ViewBag.dob = empdetails.DOB.Value.ToString("dd-MM-yyyy");
                //}
                //if (empdetails.DateofJoining != null)
                //{
                //    ViewBag.doj = empdetails.DateofJoining.Value.ToString("dd-MM-yyyy");
                //}
                //if (empdetails.DateofConfirmation != null)
                //{
                //    ViewBag.doc = empdetails.DateofConfirmation.Value.ToString("dd-MM-yyyy");
                //}
            }
            else
            {
                PopulateMaritalStatus();
                PopulateGenderStatus();
                PopulateNationality();
                ViewBag.dob = DOB.Value.ToString("dd-MM-yyyy");
                PopulateEmployementStatus();
                //PopulateGrade();
            }
            ///Achal Jha For Payroll Date : 17-05-2017 Reason : For PAyroll

            return View(empdetails);
        }

        [HttpPost]
        //[Authorize(Roles = "Employee")]
        public ActionResult BasicDetails(EmployeeBasicDetails BasicDetails, DateTime? DateofBirth, string Name, string DOB, string Gender, string MaritalStatus, string AlternateContact, string EmployeeId, string AlternateEmail, string Nationality, string fatherName, string spouseName, bool Emplanelled = false)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userdetails = generic.GetUserDetail(UserId);
            //var dob = Convert.ToDateTime(DOB);
            DateofBirth = null;
            if (!String.IsNullOrEmpty(DOB))
            {
                DateofBirth = DateTime.ParseExact(DOB, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            var empdetails = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            bool result = ems.AddEmployeeBasicDetails(Name, DateofBirth, UserId, userdetails.SubscriberId, Gender, MaritalStatus, AlternateContact, AlternateEmail, Nationality, EmployeeId, Emplanelled, userdetails.DepartmentId, fatherName, spouseName);
            //empdetails.Status, empdetails.DateofJoining, empdetails.ProbationPeriod, empdetails.GradeId, empdetails.DateofConfirmation
            PopulateMaritalStatus(BasicDetails.MaritalStatus);
            PopulateGenderStatus(BasicDetails.Gender);
            PopulateNationality(BasicDetails.Nationality);
            return RedirectToAction("BasicDetails", "Employee", new { area = "EMS", status = result });
        }

        [HttpGet]
        //[Authorize(Roles = "Employee")]
        public ActionResult EducationalDetails(string EducationLevel, string Degree, string Specialization, string University, string Institution, string YearOfPassing, string Percentage, bool status = false, string useraction = "Add")
        {

            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }

            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewBag.Degree = Degree;
            ViewBag.Specialization = Specialization;
            ViewBag.University = University;
            ViewBag.Institution = Institution;
            ViewBag.YearOfPassing = YearOfPassing;
            ViewBag.Percentage = Percentage;
            ViewBag.EduLevelId = EducationLevel;
            ViewBag.Action = useraction;
            PopulateEducationalLevel(EducationLevel);
            PopulateYear(YearOfPassing);

            var empedudetails = ems.GetEmployeeEducationalDetails(UserId);
            return View(empedudetails);
        }

        [HttpPost]
        //[Authorize(Roles = "Employee")]
        public ActionResult EducationalDetails(EmpEducationView EducationDetails, string EduLevelId = "0")
        {
            if (ModelState.IsValid)
            {
                string UserId = User.Identity.GetUserId();

                if (EducationDetails.EducationLevel == 0)
                    EducationDetails.EducationLevel = Convert.ToInt16(EduLevelId);

                bool result = ems.AddEmployeeEducationalDetails(UserId, EducationDetails.EducationLevel, EducationDetails.Degree, EducationDetails.Specialization, EducationDetails.University, EducationDetails.Institution, EducationDetails.YearOfPassing, EducationDetails.Percentage);
                return RedirectToAction("EducationalDetails", "Employee", new { area = "EMS", status = result });
            }

            PopulateEducationalLevel(EducationDetails.EducationLevel);
            PopulateYear(EducationDetails.YearOfPassing);
            return View();

        }

        //[Authorize(Roles = "Employee")]
        public ActionResult RemoveEducationDetails(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                bool result = ems.DeleteEmployeeEducationDetail(User.Identity.GetUserId(), Convert.ToInt16(Id));
            }
            return RedirectToAction("EducationalDetails", "Employee");
        }

        [HttpGet]
        //[Authorize(Roles = "Employee")]
        public ActionResult ExperienceDetails(string ProfileName, string result, string useraction = "Add", bool status = false, long ProfileId = 0, Int64 ExperienceId = 0)
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();

            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }

            if (result == "Deleted")
            {
                ViewBag.Data = "Deleted";
            }
            if (ProfileId == 0 && !string.IsNullOrEmpty(ProfileName))
                ProfileId = (from p in udb.UserProfileTypeDetails.Where(p => p.UserId == UserId && p.ProfileName == ProfileName) select p).FirstOrDefault().ProfileId;

            ViewBag.ProfileId = ProfileId;
            PopulateProfileType(UserId, ProfileId);
            ViewData["Profile"] = ems.GetProfileTypeDetails(UserId);
            var Empexperiencedetails = ems.GetEmployeeExperienceDetails(UserId);
            ViewData["Empexperiencedetails"] = Empexperiencedetails;
            var Experience = Empexperiencedetails.Where(i => i.ExperienceId == ExperienceId).FirstOrDefault();
            if (Experience != null)
            {
                if (Experience.JoiningDate != null)
                {
                    ViewBag.JoiningDate = Experience.JoiningDate.ToString("dd-MM-yyyy");
                }
                if (Experience.LeavingDate != null)
                {
                    ViewBag.LeavingDate = Experience.LeavingDate.Value.ToString("dd-MM-yyyy");
                }
            }
            return View(Experience);
        }

        [HttpPost]
        //[Authorize(Roles = "Employee")]
        public ActionResult ExperienceDetails(string ComapanyName, string WorkLocation, string LatestDesignation, string JoiningDate, string LeavingDate, bool WorkingStatus, int ProfileId, Int64 ExperienceId = 0)
        {
            string UserId = User.Identity.GetUserId();

            DateTime Jdate = DateTime.ParseExact(JoiningDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime? Ldate = null;
            if (!String.IsNullOrEmpty(LeavingDate))
            {
                Ldate = DateTime.ParseExact(LeavingDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            bool result = ems.AddEmployeeExperienceDetails(ExperienceId, UserId, ComapanyName, WorkLocation, LatestDesignation, Jdate, Ldate, WorkingStatus, ProfileId);

            return RedirectToAction("ExperienceDetails", "Employee", new { area = "EMS", status = result });
        }

        public ActionResult EmployeeProfile(string UserId)
        {

            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();

            ViewData["Profile"] = db.EmployeeBasicDetails.Find(UserId);
            //ViewData["Address"] = db.EmpAddressDetails.Where(u => u.UserId == UserId).ToList();
            var addressdetails = ems.GetAddressDetails(UserId);
            ViewData["AddressList"] = addressdetails;
            ViewData["Education"] = db.EmpEducationalDetails.Where(u => u.UserId == UserId).ToList();
            ViewData["Experience"] = db.EmpExperienceDetails.Where(u => u.UserId == UserId).ToList();
            ViewData["Skill"] = db.EmpSkillDetails.Where(u => u.UserId == UserId).ToList();
            ViewData["Social"] = db.EmpSocialDetails.Find(UserId);
            ViewData["Reporting"] = ems.GetSubscriberWiseEmployeeList(userdetails.SubscriberId).Where(r => r.UserId == UserId).FirstOrDefault();
            ViewData["Language"] = student.GetLanguages(UserId);
            ViewData["Cretifications"] = db.Certification.Where(c => c.UserId == UserId).ToList();
            return View(generic.GetUserDetail(UserId));
        }

        //[Authorize(Roles = "Employee")]
        public ActionResult RemoveExperience(int ExperienceId)
        {

            bool result = ems.DeleteExperience(ExperienceId);
            return RedirectToAction("ExperienceDetails", "Employee", new { result = "Deleted" });
        }

        [HttpGet]
        //[Authorize(Roles = "Employee")]
        public ActionResult SkillDetails(string ProfileName, string SkillName, string result, bool status = false, string useraction = "Add", long ProfileId = 0, short YearofExperience = 0)
        {

            ViewBag.SkillName = SkillName;
            ViewBag.Action = useraction;
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            if (result == "Deleted")
            {
                ViewBag.Data = "Deleted";
            }
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();

            if (ProfileId == 0 && !string.IsNullOrEmpty(ProfileName))
                ProfileId = (from p in udb.UserProfileTypeDetails.Where(p => p.UserId == UserId && p.ProfileName == ProfileName) select p).FirstOrDefault().ProfileId;
            PopulateProfileType(UserId, ProfileId);
            ViewBag.ProfileTypeId = ProfileId;

            ViewData["Profile"] = ems.GetProfileTypeDetails(UserId);
            ViewBag.UserName = ems.GetUserName(UserId);
            var EmpSkilldetails = ems.GetEmployeeSkillDetails(UserId);
            ViewData["EmpSkills"] = EmpSkilldetails;
            var employeeSkill = EmpSkilldetails.Where(s => s.ProfileId == ProfileId && s.SkillName == SkillName).FirstOrDefault();
            if (employeeSkill != null)
            {
                PopulateYearOfExp(employeeSkill.YearofExperience);
            }
            else
            {
                PopulateYearOfExp();
            }
            return View(employeeSkill);
        }

        [HttpPost]
        //[Authorize(Roles = "Employee")]
        public ActionResult SkillDetails(EmpSkillView SkillDetails, string ProfileTypeId)
        {
            string UserId = User.Identity.GetUserId();
            if (SkillDetails.ProfileId == 0)
                SkillDetails.ProfileId = Convert.ToInt64(ProfileTypeId);
            bool result = ems.AddEmployeeSkillDetails(UserId, SkillDetails.SkillName, SkillDetails.ProfileId, SkillDetails.YearofExperience);

            return RedirectToAction("SkillDetails", "Employee", new { area = "EMS", status = result });
        }

        //[Authorize(Roles = "Employee")]
        public ActionResult RemoveSkills(string SN, string PId)
        {
            Int64 pId = 0;
            if (!string.IsNullOrEmpty(PId))
                pId = Convert.ToInt64(PId);

            bool result = ems.DeleteEmployeeSkills(User.Identity.GetUserId(), SN, pId);
            return RedirectToAction("SkillDetails", "Employee", new { area = "EMS", result = "Deleted" });
        }

        [HttpGet]
        //[Authorize(Roles = "Employee")]
        public ActionResult IdentificationDetails(string result, DateTime? IssuingDate, DateTime? ValidTill, bool status = false, short IdentificationTypeId = 0, string UserAction = "Add")
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();

            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            if (result == "Deleted")
            {
                ViewBag.data = "Deleted";
            }
            ViewBag.IdentityType = IdentificationTypeId;
            ViewBag.Action = UserAction;

            PopulateIdentificationType(IdentificationTypeId);
            var IdentificationList = ems.GetEmployeeIdentificationDetails(UserId);
            ViewData["IdentificationList"] = IdentificationList;
            var identifications = IdentificationList.Where(i => i.IdType == IdentificationTypeId).FirstOrDefault();
            if (identifications != null)
            {
                if (identifications.IssuingDate != null)
                {
                    IssuingDate = identifications.IssuingDate;
                    ViewBag.issuingDate = IssuingDate.Value.ToString("dd-MM-yyyy");
                }
                if (identifications.ValidTill != null)
                {
                    ValidTill = identifications.ValidTill;
                    ViewBag.validTill = ValidTill.Value.ToString("dd-MM-yyyy");
                }

                return View(identifications);
            }
            return View();
        }


        [HttpPost]
        //[Authorize(Roles = "Employee")]
        public ActionResult IdentificationDetails(string IdNumber, string IssuingDate, string ValidTill, short IdentificationTypeId = 0, string IdentityType = "0")
        {
            string UserId = User.Identity.GetUserId();

            if (IdentificationTypeId == 0)
                IdentificationTypeId = Convert.ToInt16(IdentityType);

            //var issuingDate = Convert.ToDateTime(IssuingDate);
            //var validTill = Convert.ToDateTime(ValidTill);

            DateTime? issuingDate = null;
            if (!String.IsNullOrEmpty(IssuingDate))
            {
                issuingDate = DateTime.ParseExact(IssuingDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            }

            DateTime? validTill = null;
            if (!String.IsNullOrEmpty(ValidTill))
            {
                validTill = DateTime.ParseExact(ValidTill, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            }
            bool result = ems.AddEmployeeIdentificationDetails(UserId, IdentificationTypeId, IdNumber, issuingDate, validTill);

            return RedirectToAction("IdentificationDetails", "Employee", new { area = "EMS", status = result });

        }

        //[Authorize(Roles = "Employee")]
        public ActionResult RemoveIdentificationDetails(string Id)
        {
            Int16 identificationType = 0;
            if (!string.IsNullOrEmpty(Id))
            {
                identificationType = Convert.ToInt16(Id);

                bool result = ems.DeleteEmployeeIndentificationDetails(User.Identity.GetUserId(), identificationType);
                return RedirectToAction("IdentificationDetails", "Employee", new { area = "EMS", result = "Deleted" });
            }
            return RedirectToAction("IdentificationDetails", "Employee", new { area = "EMS" });
        }

        [HttpGet]
        //[Authorize(Roles = "Employee")]
        public ActionResult SocialDetails(bool status = false)
        {
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();

            var Empsocialdetails = ems.GetEmployeeSocialDetails(UserId);
            return View(Empsocialdetails);
        }

        [HttpPost]
        //[Authorize(Roles = "Employee")]
        public ActionResult SocialDetails(EmpSocialDetails SocialDetails)
        {
            if (ModelState.IsValid)
            {
                bool result = false;
                if (!string.IsNullOrEmpty(SocialDetails.LinkedIn) || !string.IsNullOrEmpty(SocialDetails.Facebook) || !string.IsNullOrEmpty(SocialDetails.Skypeid) || !string.IsNullOrEmpty(SocialDetails.GooglePlus))
                {
                    string UserId = User.Identity.GetUserId();
                    result = ems.AddEmployeeSocailDetails(UserId, SocialDetails.LinkedIn, SocialDetails.Facebook, SocialDetails.Skypeid, SocialDetails.GooglePlus);
                }
                return RedirectToAction("SocialDetails", "Employee", new { area = "EMS", status = result });
            }
            return RedirectToAction("SocialDetails", "Employee", new { area = "EMS" });
        }

        [HttpGet]
        //[Authorize(Roles = "Employee")]
        public ActionResult ProfileType(string p, string ProfileName, bool status = false, string useraction = "Add", int ProfileId = 0)
        {
            ViewBag.Page = p;
            ViewBag.ProfileName = ProfileName;
            ViewBag.ProfileId = ProfileId;
            ViewBag.Action = useraction;
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }

            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();

            var ProfileTypeDetails = ems.GetProfileTypeDetails(UserId);
            return View(ProfileTypeDetails);
        }

        [HttpPost]
        //[Authorize(Roles = "Employee")]
        public ActionResult ProfileType(string PreviousPage, string UserId, string ProfileName, string useraction, int ProfileId = 0)
        {
            UserId = User.Identity.GetUserId();
            bool result = ems.AddProfileTypeDetails(ProfileId, UserId, ProfileName);

            if (PreviousPage == "exp")
                return RedirectToAction("ExperienceDetails", "Employee", new { area = "EMS", ProfileName = ProfileName });
            else if (PreviousPage == "skill")
                return RedirectToAction("SkillDetails", "Employee", new { area = "EMS", ProfileName = ProfileName });
            else
                return RedirectToAction("ProfileType", "Employee", new { area = "EMS", status = result });
        }

        //[Authorize(Roles = "Employee")]
        public ActionResult RemoveProfileType(string Id)
        {
            bool result = ems.DeleteProfileDetails(User.Identity.GetUserId(), Id);
            return RedirectToAction("ProfileType", "Employee", new { area = "EMS" });
        }

        //Address Details
        [HttpGet]
        //[Authorize(Roles = "Employee")]
        public ActionResult AddressDetails(string AddressType = "", string UserAction = "Add")
        {
            ViewBag.UserAction = UserAction;
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var addressdetails = ems.GetAddressDetails(UserId);
            ViewData["AddressList"] = addressdetails;
            var address = addressdetails.Where(a => a.AddressType == AddressType).FirstOrDefault();
            if (address != null)
            {
                PopulateAddressType(address.AddressType);
                PopulateCountry(address.CountryId);
                PopulateState(address.CountryId, address.StateId);
                PopulateCity(address.StateId, address.CityId);
            }
            else
            {
                PopulateAddressType();
                PopulateCountry();
                PopulateState();
                PopulateCity();
            }
            return View(address);
        }

        [HttpPost]
        //[Authorize(Roles = "Employee")]
        public ActionResult AddressDetails(string AddressType, string AddressLine1, string AddressLine2, string CityId, string StateId, string PostalCode, string CountryId)
        {
            string UserId = User.Identity.GetUserId();
            int cityId = 0, stateId = 0, countryId = 0;
            if (!string.IsNullOrEmpty(CityId))
                cityId = Convert.ToInt32(CityId);
            if (!string.IsNullOrEmpty(StateId))
                stateId = Convert.ToInt32(StateId);
            if (!string.IsNullOrEmpty(CountryId))
                countryId = Convert.ToInt32(CountryId);

            ems.AddEmpAddressDetails(UserId, AddressType, AddressLine1, AddressLine2, cityId, stateId, PostalCode, countryId);

            return RedirectToAction("AddressDetails", "Employee", new { area = "EMS" });
        }


        [Authorize]
        public ActionResult RemoveAddressDetails(string AT)
        {
            string UserId = User.Identity.GetUserId();

            bool result = ems.RemoveAddressDetails(UserId, AT);

            return RedirectToAction("AddressDetails", "Employee", new { area = "EMS" });
        }

        ///Summary
        ///Created By: Ajay Kumar Choudhary
        ///Created on: 03-06-2017
        ///For: Languages  
        [HttpGet]
        //[Authorize(Roles = "Employee")]
        public ActionResult Languages(bool status = false, Int32 LanguageId = 0, Int64 UserLanguageId = 0, string UserAction = "Add")
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            PopulateLanguage();
            if (UserAction == "Delete" && UserLanguageId > 0)
            {
                student.DeleteLanguage(UserLanguageId);
                PopulateLanguage();
                ViewBag.Data = "Succeeded";
                return View();
            }

            var language = student.GetLanguages(UserId);
            ViewData["Language"] = language;
            var languages = language.Where(i => i.UserLanguageId == UserLanguageId).FirstOrDefault();
            if (languages != null)
            {
                PopulateLanguage(languages.LanguageId);
            }
            return View(languages);
        }

        [HttpPost]
        //[Authorize(Roles = "Employee")]
        public ActionResult Languages(string UserId, Int32 LanguageId, bool ReadLanguage = false, bool Write = false, bool Speak = false, Int64 UserLanguageId = 0)
        {
            UserId = User.Identity.GetUserId();
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);

            student.AddLaguages(UserLanguageId, UserId, LanguageId, ReadLanguage, Write, Speak);
            return RedirectToAction("Languages", "Employee", new { Area = "EMS" });
        }
        //END

        //By: Ajay Kumar Choudhary
        //On: 20-07-2017
        //For:Certifications
        //Start
        [HttpGet]
        //[Authorize(Roles = "Employee")]
        public ActionResult Certifications(string UserAction, Int64 CertificationId = 0, bool Data = false)
        {
            string UserId = User.Identity.GetUserId();
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            if (Data == true)
            {
                ViewBag.Data = "Succeed";
            }
            if (UserAction == "Delete" && CertificationId > 0)
            {
                ems.DeleteCertificate(CertificationId);
                PopulateYear();
                ViewBag.Result = "Deleted";
                return View();
            }
            PopulateYear();
            var Certificates = db.Certification.Where(c => c.UserId == UserId).ToList();
            ViewData["Cretifications"] = Certificates;
            var SelectedCertificate = Certificates.Where(c => c.CertificationId == CertificationId).FirstOrDefault();
            if (SelectedCertificate != null)
            {
                PopulateYear(SelectedCertificate.YearOfPassing);
            }
            return View(SelectedCertificate);
        }

        [HttpPost]
        public ActionResult Certifications(string UserId, string Certificate, string Specialization, string Institution, string YearOfPassing, string Percentage, Int64 CertificationId = 0)
        {
            var UserDetails = generic.GetUserDetail(User.Identity.GetUserId());
            bool result = ems.AddCertification(CertificationId, UserDetails.UserId, Certificate, Specialization, Institution, YearOfPassing, Percentage);
            return RedirectToAction("Certifications", "Employee", new { area = "EMS", Data = result });
        }
        //END

        private void PopulateMaritalStatus(object selectedValue = null)
        {
            var MaritalList = Global.GetMaritalList();
            SelectList MaritalStatus = new SelectList(MaritalList, "MaritalStatus", "MaritalStatus", selectedValue);
            ViewBag.MaritalStatus = MaritalStatus;
        }

        private void PopulateGenderStatus(object selectedValue = null)
        {
            var GenderList = Global.GetGenderList();
            SelectList Gender = new SelectList(GenderList, "Genderid", "Gender", selectedValue);
            ViewBag.Gender = Gender;
        }

        private void PopulateProfileType(string UserId, object selectedProfileType = null)
        {
            var query = ems.GetProfileTypeList(UserId);
            SelectList ProfileTypeList = new SelectList(query, "ProfileId", "ProfileName", selectedProfileType);
            ViewBag.ProfileId = ProfileTypeList;
        }

        private void PopulateNationality(object selectedNationality = null)
        {
            var nationalStatusQuery = generic.GetNationalityList();
            SelectList Nlist = new SelectList(nationalStatusQuery, "Nationality", "Nationality", selectedNationality);
            ViewBag.Nationality = Nlist;
        }

        private void PopulateYearOfExp(object selectedProfileType = null)
        {
            var query = Global.GetYearOfExperience();
            SelectList ExpList = new SelectList(query, "Value", "Text", selectedProfileType);
            ViewBag.YearofExperience = ExpList;
        }

        private void PopulateEducationalLevel(object selectedEducationLevel = null)
        {
            var query = ems.GetEducationLevelList();
            SelectList EducationLevelList = new SelectList(query, "EducationLevelId", "EducationLevelName", selectedEducationLevel);
            ViewBag.EducationLevel = EducationLevelList;
        }
        private void PopulateIdentificationType(object selectedIdentificationType = null)
        {
            EMSManager ems = new EMSManager();
            var query = ems.GetIdentificationTypeList();
            SelectList IdentificationTypes = new SelectList(query, "IdentificationTypeId", "IdentificationTypeName", selectedIdentificationType);
            ViewBag.IdentificationTypeId = IdentificationTypes;
        }

        private void PopulateYear(object selectedYear = null)
        {
            var query = generic.GetYear();
            SelectList YearList = new SelectList(query, "Year", "Year", selectedYear);
            ViewBag.YearOfPassing = YearList;
        }

        public void PopulateAddressType(object selectedValue = null)
        {
            var AddressTypeList = Global.GetAddressType();
            ViewBag.addresstype = new SelectList(AddressTypeList, "addresstypeid", "addresstype", selectedValue);
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
        //For Engegmnttype master

        [HttpGet]
        public ActionResult EngagementType(Int64 EngagementTypeId = 0, Int16 SchemeId = 0, string UserAction = "Delete")
        {
            populateLeaveLimit();
            PopulateLeaveScheme();
            PopulateYearEndAction();
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var engagementtype = ems.GetEngagementLeaveUnion(userDetail.SubscriberId);
            ViewData["Scheme"] = db.LeaveSchemeMaster.ToList();
            ViewData["EngagementType"] = engagementtype;

            if (UserAction == "Delete" && EngagementTypeId > 0)
            {
                ems.DeleteEngagement(EngagementTypeId);
                ViewBag.Result = "Deleted";
                return View();
            }

            return View(udb.EngagementTypeMaster.Find(EngagementTypeId));
        }

        [HttpPost]
        public ActionResult EngagementType(string LeaveTypeId, string LeaveTypeName, string ShortName, string LeaveTypeCategory, string EffectiveFrom, string YearEndAction, Int16 MaxLimit = 0, Int64 EngagementTypeId = 0, Int16 SchemeId = 0, int LeaveLimit = 0)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserDetail.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            string CorporateId = UserDetail.SubscriberId;

            DateTime EffectiveDate = DateTime.Now;
            if (!String.IsNullOrEmpty(EffectiveFrom))
            {
                EffectiveDate = DateTime.ParseExact(EffectiveFrom, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            bool result = ems.AddEngagementTypeMaster(EngagementTypeId, LeaveTypeName, CorporateId, ShortName, SchemeId, LeaveTypeId, LeaveTypeCategory, LeaveLimit, EffectiveDate, YearEndAction, MaxLimit);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEngagementType(Int64 EngagementTypeId = 0, Int16 SchemeId = 0, string UserAction = "Add")
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            var engagementDetail = udb.EngagementTypeMaster.Find(EngagementTypeId);
            ViewBag.SchemeId = SchemeId;
            ViewBag.EngagementTypeId = EngagementTypeId;
            
            if (UserAction == "Add")
            {
                populateLeaveLimit();
                populateMaxLeaveLimit();
                PopulateYearEndAction();
                populateLeaveTypeCategory();
            }
            else
            {
                ViewBag.LeaveTypeId = engagementDetail.LeaveTypeId;
                ViewBag.EffectiveFrom = engagementDetail.EffectiveFrom.ToString("dd-MM-yyyy");
                populateLeaveLimit(engagementDetail.LeaveLimit);
                PopulateYearEndAction(engagementDetail.YearEndAction);
                populateLeaveTypeCategory(engagementDetail.LeaveTypeCategory);
                populateMaxLeaveLimit(engagementDetail.MaxLimit, engagementDetail.LeaveLimit);
            }
            return View(engagementDetail);
        }
        //get method for EngagementType

        public ActionResult RemoveEngagementType(Int64 Id)
        {
            var removeItem = udb.EngagementTypeMaster.Find(Id);

            if (removeItem != null)
            {
                udb.EngagementTypeMaster.Remove(removeItem);
                udb.SaveChanges();
            }
            return RedirectToAction("EngagementType", "Employee");
        }
        [HttpPost]
        public ActionResult Maxlimit(int Limit)
        {

            List<SelectListItem> Limitmax = new List<SelectListItem>();

            for (int i = 0; i <= Limit; i++)
            {
                Limitmax.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            return Json(Limitmax, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///Created by : Achal Jha
        ///Created on : 17-05-2017
        ///Reason :     For Payroll
        /// </summary>gradeID
        /// <returns></returns>
        ///////////////////////////////////////////
        private void PopulateEmployementStatus(object selectedValue = null)
        {
            var EmployementStatus = Global.EmployeementStatus();
            SelectList EmployementStat = new SelectList(EmployementStatus, "Status", "Status", selectedValue);
            ViewBag.Status = EmployementStat;
        }

        private void PopulateLanguage(object selectedValue = null)
        {
            var LanguageList = student.GetLanguageList();
            SelectList Language = new SelectList(LanguageList, "LanguageId", "Language", selectedValue);
            ViewBag.LanguageId = Language;
        }
        private void populateLeaveLimit(object selectedValue = null)
        {
            var LeaveLimit = Global.LeaveLimitL();
            SelectList limit = new SelectList(LeaveLimit, "limit", "limit", selectedValue);
            ViewBag.limit = limit;
        }

        private void populateMaxLeaveLimit(object selectedValue = null,object limitvalue=null)
        {
            var MaxLeaveLimit = Global.LeaveLimitL().Where(a => a.limit <=Convert.ToInt16(limitvalue));
            SelectList Maxlimit = new SelectList(MaxLeaveLimit, "limit", "limit", selectedValue);
            ViewBag.Maxlimit = Maxlimit;
        }

        private void populateLeaveTypeCategory(object selectedValue = null)
        {
            var LeaveTypeCategory = Global.GetLeaveTypeCategory();
            SelectList LeaveType = new SelectList(LeaveTypeCategory, "LeaveTypeCategoryId", "LeaveTypeCategoryListName", selectedValue);
            ViewBag.LeaveType = LeaveType;
        }

        //created by : vikas pandey
        //17/11/2017
        private void PopulateLeaveScheme(object selectedValue = null)
        {
            var query = student.GetScheme();
            SelectList SchemeId = new SelectList(query, "SchemeId", "SchemeName", selectedValue);
            ViewBag.SchemeId = SchemeId;
        }
        private void PopulateYearEndAction(object selectedValue = null)
        {
            var yearandaction = Global.GetYearEndAction();
            SelectList yearendactions = new SelectList(yearandaction, "Yearshortname", "Yearaction", selectedValue);
            ViewBag.YearendActions = yearendactions;
        }

    }
}