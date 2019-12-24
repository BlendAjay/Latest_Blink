using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AJSolutions.Models;
using Microsoft.AspNet.Identity;
using AJSolutions.DAL;
using Microsoft.Owin.Security;
using System.Globalization;

namespace AJSolutions.Areas.Candidate.Controllers
{
    public class CandidateController : Controller
    {
        Student student = new Student();
        EMSManager ems = new EMSManager();
        CMSManager cms = new CMSManager();
        Generic generic = new Generic();
        TMSManager tms = new TMSManager();
        AdminManager admin = new AdminManager();
        private UserDBContext db = new UserDBContext();
        // GET: Candidate/Candidate

        //[Authorize(Roles = "Candidate")]
        public ActionResult Index()
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            return View();
        }


        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public ActionResult GeneralDetails(DateTime? DOB, bool data = false)
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
            var mydetails = student.GetCandidateGeneralDetails(UserId);

            if (mydetails != null)
            {
                PopulateMaritalStatus(mydetails.MaritalStatus);
                PopulateGenderStatus(mydetails.Gender);
                PopulateNationality(mydetails.Nationality);
                if (mydetails.DOB != null)
                {
                    ViewBag.dob = mydetails.DOB.Value.ToString("dd-MM-yyyy");
                }
            }
            else
            {
                PopulateMaritalStatus();
                PopulateGenderStatus();
                PopulateNationality();
                ViewBag.dob = DOB.Value.ToString("dd-MM-yyyy");
            }
            return View(mydetails);
        }

        [HttpPost]
        //[Authorize(Roles = "Candidate")]
        public ActionResult GeneralDetails(UserProfile GeneralDetails, string DOBirth, string MaritalStatus, string Gender, string UserId, string SubscriberId, string Name, string AlternateContact, string AlternateEmail, string Nationality, string DepartmentId, DateTime? DOB)
        {
            UserId = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(UserId);
            DOB = null;
            if (!String.IsNullOrEmpty(DOBirth))
            {
                DOB = DateTime.ParseExact(DOBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            }
            bool result = student.AddCandidateGeneralDetails(Name, DOB, UserId, UserDetail.SubscriberId, Gender, MaritalStatus, AlternateContact, AlternateEmail, Nationality, UserDetail.Department, DateTime.Now, UserId);
            PopulateMaritalStatus(GeneralDetails.MaritalStatus);
            PopulateGenderStatus(GeneralDetails.Gender);
            PopulateNationality(GeneralDetails.Nationality);
            return RedirectToAction("GeneralDetails", "Candidate", new { Area = "Candidate", data = result });
        }

        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public ActionResult FamilyDetails(bool status = false)
        {

            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }

            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var familydetails = student.GetCandidateFamilyDetails(UserId);
            if (familydetails != null)
            {
                PopulateBloodGroup(familydetails.BloodGroup);
            }
            else
            {
                PopulateBloodGroup();
            }

            return View(familydetails);
        }

        [HttpPost]
        //[Authorize(Roles = "Candidate")]
        public ActionResult FamilyDetails(UserFamilyDetails FamilyDetails)
        {
            if (ModelState.IsValid)
            {
                bool result = false;
                if (!String.IsNullOrEmpty(FamilyDetails.FatherName) || !String.IsNullOrEmpty(FamilyDetails.FatherOccupation) || !String.IsNullOrEmpty(FamilyDetails.FatherContact) || !String.IsNullOrEmpty(FamilyDetails.MotherName) || !String.IsNullOrEmpty(FamilyDetails.MotherOccupation) || !String.IsNullOrEmpty(FamilyDetails.MotherContact) || !String.IsNullOrEmpty(FamilyDetails.SpouseName) || !String.IsNullOrEmpty(FamilyDetails.SpouseContact) || !String.IsNullOrEmpty(FamilyDetails.SpouseOccupation) || !String.IsNullOrEmpty(FamilyDetails.BloodGroup) || !String.IsNullOrEmpty(FamilyDetails.FamilyIncome))
                {
                    string UserId = User.Identity.GetUserId();
                    result = student.AddCandidateFamilyDetails(UserId, FamilyDetails.FatherName, FamilyDetails.FatherOccupation, FamilyDetails.FatherContact, FamilyDetails.MotherName, FamilyDetails.MotherOccupation, FamilyDetails.MotherContact, FamilyDetails.SpouseName, FamilyDetails.SpouseContact, FamilyDetails.SpouseOccupation, FamilyDetails.BloodGroup, FamilyDetails.FamilyIncome);
                }

                return RedirectToAction("FamilyDetails", "Candidate", new { Area = "Candidate", status = result });
            }
            PopulateBloodGroup(FamilyDetails.BloodGroup);
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public ActionResult EducationDetails(string Degree, string Specialization, string University, string Institution, string YearOfPassing, string Percentage, bool status = false, short EducationLevelId = 0, string useraction = "Add")
        {

            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewBag.Action = useraction;
            ViewBag.EduLevelId = EducationLevelId;
            PopulateEducationalLevel(EducationLevelId);
            ViewBag.Degree = Degree;
            ViewBag.Specialization = Specialization;
            ViewBag.University = University;
            ViewBag.Institution = Institution;
            ViewBag.YearOfPassing = YearOfPassing;
            ViewBag.Percentage = Percentage;

            PopulateYear(YearOfPassing);

            var educationaldetails = student.GetCandidateEducationalDetails(UserId);
            return View(educationaldetails);
        }

        [HttpPost]
        //[Authorize(Roles = "Candidate")]
        public ActionResult EducationDetails(UserEduactionDetails EducationDetails, short EducationLevelId = 0, string EduLevelId = "0")
        {
            bool result = false;
            string UserId = User.Identity.GetUserId();
            EducationDetails.UserId = UserId;


            if (EducationLevelId == 0)
                EducationLevelId = Convert.ToInt16(EduLevelId);

            result = student.AddCandidateEducationalDetails(UserId, EducationLevelId, EducationDetails.Degree, EducationDetails.Specialization, EducationDetails.University, EducationDetails.Institution, EducationDetails.YearOfPassing, EducationDetails.Percentage);

            return RedirectToAction("EducationDetails", "Candidate", new { Area = "Candidate", status = result });
        }

        //By: Ajay Kumar Choudhary
        //On: 21-07-2017
        //For:Certifications
        //Start
        [HttpGet]
        //[Authorize(Roles = "Candidate")]
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
            return RedirectToAction("Certifications", "Candidate", new { area = "Candidate", Data = result });
        }
        //END


        //[Authorize(Roles = "Candidate")]
        public ActionResult RemoveEducationDetails(short Id = 0)
        {

            bool result = student.DeleteEducationDetails(User.Identity.GetUserId(), Id);
            return RedirectToAction("EducationDetails", "Candidate", new { Area = "Candidate" });
        }

        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public ActionResult IdentificationDetails(DateTime? IssuingDate, DateTime? ValidTill, bool status = false, short IdentificationTypeId = 0, string UserAction = "Add")
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            ViewBag.IdentityType = IdentificationTypeId;
            ViewBag.Action = UserAction;
            PopulateIdentificationType(IdentificationTypeId);
            var IdentificationList = student.GetCandidateIdentificationDetails(UserId);
            ViewData["IdentificationType"] = IdentificationList;
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
        //[Authorize(Roles = "Candidate")]
        public ActionResult IdentificationDetails(string IdNumber, string IssuingDate, string ValidTill, short IdentificationTypeId = 0)
        {
            string UserId = User.Identity.GetUserId();


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
            bool result = student.AddCandidateIdentificationDetails(UserId, IdentificationTypeId, IdNumber, issuingDate, validTill);

            return RedirectToAction("IdentificationDetails", "Candidate", new { Area = "Candidate", status = result });
        }

        //[Authorize(Roles = "Candidate")]
        public ActionResult RemoveIdentificationDetails(string Id)
        {
            Int16 identificationType = 0;
            if (!string.IsNullOrEmpty(Id))
            {
                identificationType = Convert.ToInt16(Id);

                bool result = student.DeleteCandidateIndentificationDetails(User.Identity.GetUserId(), identificationType);
            }
            return RedirectToAction("IdentificationDetails", "Candidate", new { area = "Candidate" });
        }

        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public ActionResult VehicleDetails(bool status = false)
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }

            var vehicledetails = student.GetCandidateVehicleDetails(UserId);
            return View(vehicledetails);
        }

        [HttpPost]
        //[Authorize(Roles = "Candidate")]
        public ActionResult VehicleDetails(string UserId, string VehicleType, string VehicleNumber, string VehicleOwner, string DrivingLicence)
        {
            UserId = User.Identity.GetUserId();
            bool result = student.AddCandidateVehicleDetails(UserId, VehicleType, VehicleNumber, VehicleOwner, DrivingLicence);
            return RedirectToAction("VehicleDetails", "Candidate", new { Area = "Candidate", status = result });
        }


        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public ActionResult SocialDetails(bool status = false)
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();

            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }

            var socialdetails = student.GetCandidateSocialDetails(UserId);
            return View(socialdetails);
        }

        [HttpPost]
        //[Authorize(Roles = "Candidate")]
        public ActionResult SocialDetails(UserSocialDetails social, string UserId)
        {
            UserId = User.Identity.GetUserId();
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);

            student.AddCandidateSocailDetails(UserId, social.LinkedIn, social.Facebook, social.Skypeid, social.GooglePlus);
            return RedirectToAction("SocialDetails", "Candidate", new { Area = "Candidate" });
        }

        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public ActionResult ExperienceDetails(string ComapanyName, string WorkLocation, string LatestDesignation, string workingStatus, bool status = false, DateTime? JoiningDate = null, DateTime? LeavingDate = null, Int64 ExperienceId = 0)
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();

            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }

            var experiencedetails = student.GetCandidateExperienceDetails(UserId);
            ViewData["Empexperiencedetails"] = experiencedetails;
            var Experience = experiencedetails.Where(e => e.ExperienceId == ExperienceId).FirstOrDefault();
            if (Experience != null)
            {
                ViewBag.JoiningDate = Experience.JoiningDate.ToString("dd-MM-yyyy");

                if (Experience.LeavingDate != null)
                {
                    ViewBag.LeavingDate = Experience.LeavingDate.Value.ToString("dd-MM-yyyy");
                }
            }
            return View(Experience);
        }

        [HttpPost]
        //[Authorize(Roles = "Candidate")]
        public ActionResult ExperienceDetails(string ComapanyName, string WorkLocation, string LatestDesignation, string JoiningDate, string LeavingDate, string workingStatus, Int64 ExperienceId = 0)
        {

            string UserId = User.Identity.GetUserId();
            bool WorkingStatus = (workingStatus == null) ? false : Convert.ToBoolean(workingStatus);
            //var Jdate = Convert.ToDateTime(JoiningDate);
            //var Ldate = Convert.ToDateTime(LeavingDate);

            DateTime Jdate = DateTime.ParseExact(JoiningDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime? Ldate = null;
            if (!String.IsNullOrEmpty(LeavingDate))
            {
                Ldate = DateTime.ParseExact(LeavingDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            bool result = student.AddCandidateExperienceDetails(ExperienceId, UserId, ComapanyName, WorkLocation, LatestDesignation, Jdate, Ldate, WorkingStatus);

            return RedirectToAction("ExperienceDetails", "Candidate", new { Area = "Candidate", status = result });
        }

        //[Authorize(Roles = "Candidate")]
        public ActionResult RemoveExperienceDetails(int ExperienceId)
        {
            bool result = student.DeleteExperienceDetails(ExperienceId);
            return RedirectToAction("ExperienceDetails", "Candidate", new { Area = "Candidate" });
        }

        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public ActionResult SkillDetails(string SN, string YOE, string UserAction = "Add", bool status = false)
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();

            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            ViewBag.SkillName = SN;
            ViewBag.YearOfExp = YOE;
            ViewBag.UserAction = UserAction;
            var skillDetails = student.GetCandidateSkillDetails(UserId);
            return View(skillDetails);
        }

        [HttpPost]
        //[Authorize(Roles = "Candidate")]
        public ActionResult SkillDetails(string SkillName, string YearofExperience)
        {
            short yoe = 0;
            string UserId = User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(YearofExperience))
                yoe = Convert.ToInt16(YearofExperience);

            bool result = student.AddCandidateSkillDetails(UserId, SkillName, yoe);
            return RedirectToAction("SkillDetails", "Candidate", new { Area = "Candidate", status = result });
        }

        //[Authorize(Roles = "Candidate")]
        public ActionResult RemoveSkill(string Id)
        {

            bool result = student.DeleteSkillDetails(User.Identity.GetUserId(), Id);
            return RedirectToAction("SkillDetails", "Candidate", new { Area = "Candidate" });
        }


        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public ActionResult FeeDetails(Int64 BId, string InstallmentAmount, string res = null)
        {
            ViewBag.Status = res;
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewBag.InstallmentAmount = InstallmentAmount;
            ViewBag.OnlineCharges = (Convert.ToSingle(InstallmentAmount) * 3 / 100);
            PopulatePaymentModeType(1);
            var feedetails = student.GetCandidateFeeDetails(userdetails.UserId, BId).ToList();
            feedetails = feedetails.Where(c => c.Status != "Failed").ToList();
            if (feedetails.Count == 0)
            {
                ViewBag.InstallmentNumber = 1;
            }
            else
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
            ViewData["BatchDetails"] = student.GetCandidateWiseCourseDetail(UserId, true, BId).FirstOrDefault();

            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Candidate")]
        public ActionResult FeeDetails(Int64 BatchId, string CourseCode, float CourseFee, DateTime? CheckDate, string BankName, string ReferenceNumber, string Remarks, string InstallmentAmount, string InstallmentNumber, string OnlineCharges, short PaymentModeId = 1, double TotalFeePaid = 0.0)
        {
            string Result = string.Empty;
            try
            {
                string strReturnUrl = Global.WebsiteUrl() + "Candidate/Candidate/PaymentConfirmation";
                var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
                var userPrimaryDetail = student.GetSubscriberWiseCandidateList(userDetail.SubscriberId,BatchId).Where(s => s.UserId == userDetail.UserId).FirstOrDefault();
                string transactionId = student.GetTransactionId(PaymentModeId);
                //var Details = db.InstallmentDetails.Where(c => c.UserId == userDetail.UserId && c.BatchId == BatchId).FirstOrDefault();
                if (string.IsNullOrEmpty(InstallmentNumber))
                {
                    InstallmentNumber = "1";
                }
                //double RemainingAmount = (CourseFee - TotalFeePaid);
                double RemainingAmount = 0.0;
                if (TotalFeePaid == 0.0)
                {
                        RemainingAmount = (CourseFee - Convert.ToSingle(InstallmentAmount));
                }
                else
                {
                        RemainingAmount = (CourseFee - (TotalFeePaid + Convert.ToSingle(InstallmentAmount)));
                }

                float Amount = (Convert.ToSingle(InstallmentAmount) + Convert.ToSingle(OnlineCharges));
                if (db.PaymentModeMaster.Find(PaymentModeId).PaymentMode.ToUpper() == "ONLINE")
                {
                    Result = generic.PaymentRequest(transactionId, userDetail.Name, Convert.ToString(Amount), userPrimaryDetail.Email, userPrimaryDetail.PhoneNumber, Remarks, strReturnUrl);

                    if (!Result.ToUpper().StartsWith("ERROR"))
                    {
                        Amount = Convert.ToSingle(OnlineCharges);
                        bool result = student.AddCandidateFeeDetails(transactionId, userDetail.UserId, Convert.ToSingle(InstallmentAmount), CourseCode, BatchId, DateTime.UtcNow, CheckDate, PaymentModeId, BankName, ReferenceNumber, "Failed", null, Remarks, null, Convert.ToInt16(InstallmentNumber), Convert.ToInt16(InstallmentNumber), Convert.ToSingle(RemainingAmount),null,Amount);
                        if (result)
                            return Redirect(Url.Content(Global.WebsiteUrl() + "Request.aspx?Id=" + Result));
                    }

                    return RedirectToAction("FeeDetails", "Candidate", new { Area = "Candidate", BId = BatchId, res = Result });

                }

                //bool results = student.AddCandidateFeeDetails(transactionId, userDetail.UserId, CourseFee, CourseCode, BatchId, DateTime.UtcNow, CheckDate, PaymentModeId, BankName, ReferenceNumber, "Initiate", null, Remarks, null, Details.InstallmentId, Convert.ToInt16(InstallmentNumber), Convert.ToSingle(RemainingAmount));
                //if (results)
                //    return RedirectToAction("PaymentConfirmation", "Candidate", new { Area = "Candidate", PMId = PaymentModeId, TId = transactionId });

            }
            catch (Exception ex)
            {
                Result = ex.ToString();
            }
            return RedirectToAction("FeeDetails", "Candidate", new { Area = "Candidate", BId = BatchId, res = Result });
        }


        public ActionResult PaymentConfirmation(string TId, short PMId = 0)
        {
            if (PMId == 0)
            {
                string Result = string.Empty;

                var strPGResponse = Convert.ToString(System.Web.HttpContext.Current.Request["msg"]);
                if (strPGResponse != "" || strPGResponse != null)
                {
                    Generic generic = new Generic();
                    Result = generic.ConfirmTransaction(strPGResponse);

                    string[] status = Result.Split('|');
                    string clientRfNo = status[0];
                    string transactionstatus = status[1];

                    ViewBag.Result = transactionstatus;
                    if (!string.IsNullOrEmpty(clientRfNo))
                    {
                        if (transactionstatus == "Transaction Success  0300")
                        {
                            var userFeeDetail = db.FeeDetails.Find(clientRfNo);
                            var UserBatchDetails = student.GetCandidateWiseCourseDetail(userFeeDetail.UserId, true);
                            var userDetail = generic.GetUserDetail(userFeeDetail.UserId);
                            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
                            var userBatch = UserBatchDetails.Where(b => b.CourseCode == userFeeDetail.CourseCode).FirstOrDefault();
                            ViewData["UserBatchDetail"] = userBatch;
                            ViewData["Transactiondetails"] = student.GetCandidateFeeDetails(userFeeDetail.UserId, userBatch.BatchId).Where(c => c.TransactionId == clientRfNo).FirstOrDefault();
                            ViewData["userPrimaryDetail"] = student.GetSubscriberWiseCandidateList(userBatch.SubscriberId, userBatch.BatchId).Where(s => s.UserId == userFeeDetail.UserId).FirstOrDefault();
                            return View(userFeeDetail);
                        }
                        else
                        {
                            ViewBag.Message = "Transaction Failed";
                        }
                    }
                    else if (!string.IsNullOrEmpty(TId))
                    {
                        var userDetails = db.FeeDetails.Find(TId);
                        var UserBatchDetails = student.GetCandidateWiseCourseDetail(userDetails.UserId, true);
                        var userDetail = generic.GetUserDetail(userDetails.UserId);
                        ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
                        var userBatch = UserBatchDetails.Where(b => b.CourseCode == userDetails.CourseCode).FirstOrDefault();
                        ViewData["UserBatchDetail"] = userBatch;
                        ViewData["Transactiondetails"] = student.GetCandidateFeeDetails(userDetails.UserId, userBatch.BatchId).Where(c => c.TransactionId == TId).FirstOrDefault();
                        return View(userDetails);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(TId))
                {
                    var userDetails = db.FeeDetails.Find(TId);
                    var UserBatchDetails = student.GetCandidateWiseCourseDetail(userDetails.UserId, true);
                    var userBatch = UserBatchDetails.Where(b => b.CourseCode == userDetails.CourseCode).FirstOrDefault();
                    var SubscriberId = generic.GetUserDetail(User.Identity.GetUserId());
                    ViewData["CompanyLogo"] = cms.GetCompanyLogo(SubscriberId.SubscriberId).FirstOrDefault();
                    ViewData["UserBatchDetail"] = userBatch;
                    ViewData["Transactiondetails"] = student.GetCandidateFeeDetails(userDetails.UserId, userBatch.BatchId).Where(c => c.TransactionId == TId).FirstOrDefault();
                    return View(userDetails);
                }

            }
            return View();

            //    string Result = string.Empty;
            //    var strPGResponse = Convert.ToString(System.Web.HttpContext.Current.Request["msg"]);
            //    if (strPGResponse != "" || strPGResponse != null)
            //    {
            //        Generic generic = new Generic();
            //        Result = generic.ConfirmTransaction(strPGResponse);
            //        string[] status = Result.Split('|');
            //        string clientRfNo = status[0];
            //        string transactionstatus = status[1];
            //        ViewBag.Result = transactionstatus;
            //        if (!string.IsNullOrEmpty(clientRfNo))
            //        {
            //            var userFeeDetail = db.FeeDetails.Find(clientRfNo);
            //            var UserBatchDetails = student.GetCandidateWiseCourseDetail(userFeeDetail.UserId, true);
            //            var userBatch = UserBatchDetails.Where(b => b.CourseCode == userFeeDetail.CourseCode).FirstOrDefault();
            //            ViewData["UserBatchDetail"] = userBatch;
            //            ViewData["userPrimaryDetail"] = student.GetSubscriberWiseCandidateList(userBatch.SubscriberId).Where(s => s.UserId == userFeeDetail.UserId).FirstOrDefault();
            //            return View(userFeeDetail);
            //        }
            //        else if (!string.IsNullOrEmpty(TId))
            //        {
            //            var userDetails = db.FeeDetails.Find(TId);
            //            var UserBatchDetails = student.GetCandidateWiseCourseDetail(userDetails.UserId, true);
            //            var userBatch = UserBatchDetails.Where(b => b.CourseCode == userDetails.CourseCode).FirstOrDefault();
            //            ViewData["UserBatchDetail"] = userBatch;
            //            ViewData["userPrimaryDetail"] = student.GetSubscriberWiseCandidateList(userBatch.SubscriberId).Where(s => s.UserId == userDetails.UserId).FirstOrDefault();
            //            return View(userDetails);

            //        }
            //    }
            //    return View();

        }


        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public ActionResult CourseDetails(string Course, bool status = false, DateTime? CourseStartdate = null, DateTime? CourseEndDate = null, float TotalFee = 0, float Discount = 0, float RemainingFee = 0)
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewBag.Result = "Failed";
            if (status == true){
                ViewBag.Result = "Succeeded";
            }
            PopulateCourseMastereStatus(Course);
            ViewBag.CourseStartdate = CourseStartdate;
            ViewBag.CourseEndDate = CourseEndDate;
            ViewBag.TotalFee = TotalFee;
            ViewBag.Discount = Discount;
            ViewBag.RemainingFee = RemainingFee;
            var courseDetails = student.GetCandidateCourseDetails(UserId);
            return View(courseDetails);
        }

        [HttpPost]
        //[Authorize(Roles = "Candidate")]
        public ActionResult CourseDetails(string UserId, string Course, DateTime CourseStartdate, DateTime CourseEndDate, float TotalFee = 0, float Discount = 0, float RemainingFee = 0)
        {
            UserId = User.Identity.GetUserId();
            bool result = student.AddCandidateCourseDetails(UserId, Course, CourseStartdate, CourseEndDate, TotalFee, Discount, RemainingFee);
            return RedirectToAction("CourseDetails", "Candidate", new { Area = "Candidate", status = result });
        }


        //Address Details
        [HttpGet]
        //[Authorize(Roles = "Candidate")]
        public ActionResult AddressDetails(string AddressType = "", string UserAction = "Add")
        {
            ViewBag.UserAction = UserAction;
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();

            var addressdetails = student.GetAddressDetails(UserId);
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
        //[Authorize(Roles = "Candidate")]
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

            student.AddAddressDetails(UserId, AddressType, AddressLine1, AddressLine2, cityId, stateId, PostalCode, countryId);

            return RedirectToAction("AddressDetails", "Candidate", new { area = "Candidate" });
        }


        [Authorize]
        public ActionResult RemoveAddressDetails(string AT)
        {
            string UserId = User.Identity.GetUserId();

            bool result = student.RemoveAddressDetails(UserId, AT);

            return RedirectToAction("AddressDetails", "Candidate", new { area = "Candidate" });
        }

        public ActionResult CandidateProfile(string UserId)
        {
            string Id = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(Id);
            var Candidatedetails = generic.GetUserDetail(UserId);

            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewData["Profile"] = db.UserProfile.Find(UserId);
            //ViewData["Address"] = db.UserAddressDetails.Find(UserId);
            ViewData["Address"] = db.UserAddressDetails.Where(u => u.UserId == UserId).ToList();
            ViewData["Education"] = db.UserEduactionDetails.Where(u => u.UserId == UserId).ToList();
            ViewData["Experience"] = db.UserExperienceDetails.Where(u => u.UserId == UserId).ToList();
            ViewData["Skill"] = db.UserSkillDetails.Where(u => u.UserId == UserId).ToList();
            ViewData["Social"] = db.UserSocialDetails.Find(UserId);
            ViewData["Cretifications"] = db.Certification.Where(c => c.UserId == UserId).ToList();
            return View(Candidatedetails);
        }

        ///Summary
        ///Created By: Ajay Kumar Choudhary
        ///Created on: 03-06-2017
        ///For: Languages  
        [HttpGet]
        //[Authorize(Roles = "Candidate")]
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
        //[Authorize(Roles = "Candidate")]
        public ActionResult Languages(string UserId, Int32 LanguageId, bool Read, bool Write, bool Speak, Int64 UserLanguageId = 0)
        {
            UserId = User.Identity.GetUserId();
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);

            student.AddLaguages(UserLanguageId, UserId, LanguageId, Read, Write, Speak);
            return RedirectToAction("Languages", "Candidate", new { Area = "Candidate" });
        }
        //END


        private void PopulateMaritalStatus(object selectedValue = null)
        {
            var MaritalList = Global.GetMaritalList();
            SelectList MaritalStatus = new SelectList(MaritalList, "MaritalStatus", "MaritalStatus", selectedValue);
            ViewBag.MaritalStatus = MaritalStatus;
        }

        private void PopulateBloodGroup(object selectedBloodGroup = null)
        {
            var BloodGroupList = Global.GetBloodGroupList();
            SelectList BloodGroup = new SelectList(BloodGroupList, "BloodGroup", "BloodGroup", selectedBloodGroup);
            ViewBag.BloodGroup = BloodGroup;
        }

        private void PopulateGenderStatus(object selectedValue = null)
        {
            var GenderList = Global.GetGenderList();
            SelectList Gender = new SelectList(GenderList, "Genderid", "Gender", selectedValue);
            ViewBag.Gender = Gender;
        }

        private void PopulateEducationalLevel(object selectedEducationLevel = null)
        {
            var query = ems.GetEducationLevelList();
            SelectList EducationLevelList = new SelectList(query, "EducationLevelId", "EducationLevelName", selectedEducationLevel);
            ViewBag.EducationLevelList = EducationLevelList;
        }
        private void PopulateCourseMastereStatus(object selectedValue = null)
        {
            var CourseMastereList = student.GetCourseMastereList();
            ViewBag.Course = new SelectList(CourseMastereList, "CourseCode", "CourseName", selectedValue);
        }
        private void PopulateIdentificationType(object selectedIdentificationType = null)
        {
            var query = ems.GetIdentificationTypeList();
            SelectList IdentificationType = new SelectList(query, "IdentificationTypeId", "IdentificationTypeName", selectedIdentificationType);
            ViewBag.IdentificationTypeId = IdentificationType;
        }
        private void PopulatePaymentModeType(object selectedPaymentModeType = null)
        {
            EMSManager ems = new EMSManager();
            var query = ems.PaymentModeTypeList();
            SelectList PaymentModeId = new SelectList(query, "PaymentModeId", "PaymentMode", selectedPaymentModeType);
            ViewBag.PaymentModeId = PaymentModeId;
        }

        private void PopulateCourseDetail(string UserId, object selectedValue = null)
        {
            Student obj = new Student();
            var CourseDetail = obj.GetCourseDetail(UserId);
            ViewBag.CourseCode = new SelectList(CourseDetail, "CourseCode", "CourseName", selectedValue);
        }
        private void PopulateYear(object selectedYear = null)
        {
            var query = generic.GetYear();
            SelectList YearList = new SelectList(query, "Year", "Year", selectedYear);
            ViewBag.YearOfPassing = YearList;
        }

        private void PopulateNationality(object selectedNationality = null)
        {
            var nationalStatusQuery = generic.GetNationalityList();
            SelectList Nlist = new SelectList(nationalStatusQuery, "Nationality", "Nationality", selectedNationality);
            ViewBag.Nationality = Nlist;
        }

        private void PopulateAddressType(object selectedValue = null)
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

        //private void PopulateCity(int StateId = 0, object selectedCity = null)
        //{
        //    var query = generic.GetCity(StateId);
        //    SelectList Cities = new SelectList(query, "CityId", "City", selectedCity);
        //    ViewBag.CityId = Cities;
        //}
        private void PopulateCity(int StateId = 0, object selectedCity = null)
        {
            var query = generic.GetCity(StateId);
            SelectList Cities = new SelectList(query, "CityId", "City", selectedCity);
            ViewBag.CityId = Cities;
        }

        private void PopulateLanguage(object selectedValue = null)
        {
            var LanguageList = student.GetLanguageList();
            SelectList Language = new SelectList(LanguageList, "LanguageId", "Language", selectedValue);
            ViewBag.LanguageId = Language;
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

    }
}