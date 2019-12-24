using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AJSolutions.Areas.Candidate.Models;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using AJSolutions.Models;

namespace AJSolutions.DAL
{
    public class Student
    {
        UserDBContext db = new UserDBContext();


        public bool AddCandidateGeneralDetails(string Name, DateTime? DOB, string UserId, string SubscriberId, string Gender, string MaritalStatus, string AlternateContact, string AlternateEmail, string Nationality, string DepartmentId, DateTime UpdatedOn, string UpdatedBy)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
                    var dob = new SqlParameter("@DOB", DOB);
                    var gender = new SqlParameter("@Gender", string.IsNullOrEmpty(Gender) ? DBNull.Value : (object)Gender);
                    var maritalstatus = new SqlParameter("@MaritalStatus", string.IsNullOrEmpty(MaritalStatus) ? DBNull.Value : (object)MaritalStatus);
                    var alternateContact = new SqlParameter("@AlternateContact", string.IsNullOrEmpty(AlternateContact) ? DBNull.Value : (object)AlternateContact);
                    var alternateEmail = new SqlParameter("@AlternateEmail", string.IsNullOrEmpty(AlternateEmail) ? DBNull.Value : (object)AlternateEmail);
                    var nationality = new SqlParameter("@Nationality", string.IsNullOrEmpty(Nationality) ? DBNull.Value : (object)Nationality);
                    var subscriberid = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                    var departmentId = new SqlParameter("@DepartmentId", string.IsNullOrEmpty(DepartmentId) ? DBNull.Value : (object)DepartmentId);
                    var updatedOn = new SqlParameter("@UpdatedOn", (object)UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateGeneralDetails @UserId, @Name, @DOB, @Gender, @MaritalStatus, @AlternateContact, @AlternateEmail,  @Nationality, @SubscriberId, @DepartmentId, @UpdatedOn, @UpdatedBy", userid, name, dob, gender, maritalstatus, alternateContact, alternateEmail, nationality, subscriberid, departmentId, updatedOn, updatedBy);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }

        //GeneralDetails 
        public UserProfile GetCandidateGeneralDetails(string UserId)
        {
            var details = new UserProfile();
            using (var db = new UserDBContext())
            {
                details = db.Database
                         .SqlQuery<UserProfile>("exec USP_GetCandidateGeneralDetails @UserId",
                         new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).FirstOrDefault();
            }

            return details;
        }

        //Family Details
        public bool AddCandidateFamilyDetails(string UserId, string FatherName, string FatherOccupation, string FatherContact, string MotherName, string MotherOccupation, string MotherContact, string SpouseName, string SpouseContact, string SpouseOccupation, string BloodGroup, string FamilyIncome)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var fathername = new SqlParameter("@FatherName", string.IsNullOrEmpty(FatherName) ? DBNull.Value : (object)FatherName);
                    var fatheroccupation = new SqlParameter("@FatherOccupation", string.IsNullOrEmpty(FatherOccupation) ? DBNull.Value : (object)FatherOccupation);
                    var fathercontact = new SqlParameter("@FatherContact", string.IsNullOrEmpty(FatherContact) ? DBNull.Value : (object)FatherContact);
                    var mothername = new SqlParameter("@MotherName", string.IsNullOrEmpty(MotherName) ? DBNull.Value : (object)MotherName);
                    var motheroccupation = new SqlParameter("@MotherOccupation", string.IsNullOrEmpty(MotherOccupation) ? DBNull.Value : (object)MotherOccupation);
                    var mothercontact = new SqlParameter("@MotherContact", string.IsNullOrEmpty(MotherContact) ? DBNull.Value : (object)MotherContact);
                    var spousename = new SqlParameter("@SpouseName", string.IsNullOrEmpty(SpouseName) ? DBNull.Value : (object)SpouseName);
                    var spousecontact = new SqlParameter("@SpouseContact", string.IsNullOrEmpty(SpouseContact) ? DBNull.Value : (object)SpouseContact);
                    var spouseoccupation = new SqlParameter("@SpouseOccupation", string.IsNullOrEmpty(SpouseOccupation) ? DBNull.Value : (object)SpouseOccupation);
                    var bloodgroup = new SqlParameter("@BloodGroup", string.IsNullOrEmpty(BloodGroup) ? DBNull.Value : (object)BloodGroup);
                    var familyincome = new SqlParameter("@FamilyIncome", string.IsNullOrEmpty(FamilyIncome) ? DBNull.Value : (object)FamilyIncome);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateFamilyDetails @UserId, @FatherName,  @FatherOccupation, @FatherContact, @MotherName, @MotherOccupation, @MotherContact, @SpouseName, @SpouseContact, @SpouseOccupation, @BloodGroup, @FamilyIncome",
                        userid, fathername, fatheroccupation, fathercontact, mothername, motheroccupation, mothercontact, spousename, spousecontact, spouseoccupation, bloodgroup, familyincome);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }


        public UserFamilyDetails GetCandidateFamilyDetails(string UserId)
        {
            var familydetails = new UserFamilyDetails();
            using (var db = new UserDBContext())
            {
                familydetails = db.Database
                         .SqlQuery<UserFamilyDetails>("exec USP_GetCandidateFamilyDetails @UserId",
                          new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).FirstOrDefault();
            }

            return familydetails;
        }

        //Educational Detials
        public bool AddCandidateEducationalDetails(string UserId, short EducationLevel, string Degree, string Specialization, string University, string Institution, string YearOfPassing, string Percentage)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var educationlevel = new SqlParameter("@EducationLevel", EducationLevel);
                    var degree = new SqlParameter("@Degree", string.IsNullOrEmpty(Degree) ? DBNull.Value : (object)Degree);
                    var specialization = new SqlParameter("@Specialization", string.IsNullOrEmpty(Specialization) ? DBNull.Value : (object)Specialization);
                    var university = new SqlParameter("@University", string.IsNullOrEmpty(University) ? DBNull.Value : (object)University);
                    var institution = new SqlParameter("@Institution", string.IsNullOrEmpty(Institution) ? DBNull.Value : (object)Institution);
                    var yearofpassing = new SqlParameter("@YearOfPassing", string.IsNullOrEmpty(YearOfPassing) ? DBNull.Value : (object)YearOfPassing);
                    var percentage = new SqlParameter("@Percentage", string.IsNullOrEmpty(Percentage) ? DBNull.Value : (object)Percentage);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateEducationalDetails @UserId, @EducationLevel, @Degree, @Specialization, @University, @Institution, @YearOfPassing, @Percentage", userid, educationlevel, degree, specialization, university, institution, yearofpassing, percentage);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }


        public List<UserEduactionView> GetCandidateEducationalDetails(string UserId)
        {
            var Educationaldetails = new List<UserEduactionView>();
            using (var db = new UserDBContext())
            {
                Educationaldetails = db.Database
                         .SqlQuery<UserEduactionView>("exec USP_GetCandidateEducationalDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return Educationaldetails;
        }

        public bool DeleteEducationDetails(string UserId, short EducationLevel)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var educationLevel = new SqlParameter("@EducationLevel", EducationLevel);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteCandidateEducationalDetails  @UserId, @EducationLevel", userId, educationLevel);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }

        //Experience Details
        public bool AddCandidateExperienceDetails(Int64 ExperienceId, string UserId, string ComapanyName, string WorkLocation, string LatestDesignation, DateTime JoiningDate, DateTime? LeavingDate, bool WorkingStatus)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var experienceid = new SqlParameter("@ExperienceId", ExperienceId);
                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var comapanyname = new SqlParameter("@ComapanyName", string.IsNullOrEmpty(ComapanyName) ? DBNull.Value : (object)ComapanyName);
                    var worklocation = new SqlParameter("@WorkLocation", string.IsNullOrEmpty(WorkLocation) ? DBNull.Value : (object)WorkLocation);
                    var latestDesignation = new SqlParameter("@LatestDesignation", string.IsNullOrEmpty(LatestDesignation) ? DBNull.Value : (object)LatestDesignation);
                    var joiningdate = new SqlParameter("@JoiningDate", JoiningDate);
                    var leavingdate = new SqlParameter("@LeavingDate", DBNull.Value);
                    if (LeavingDate != null)
                        leavingdate = new SqlParameter("@LeavingDate", LeavingDate);
                    var workingstatus = new SqlParameter("@WorkingStatus", WorkingStatus);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateExperienceDetails @ExperienceId, @UserId, @ComapanyName, @WorkLocation,@LatestDesignation, @JoiningDate, @LeavingDate, @WorkingStatus", experienceid, userid, comapanyname, worklocation, latestDesignation, joiningdate, leavingdate, workingstatus);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }


        public List<UserExperienceDetails> GetCandidateExperienceDetails(string UserId)
        {
            var experiencedetails = new List<UserExperienceDetails>();
            using (var db = new UserDBContext())
            {
                experiencedetails = db.Database
                         .SqlQuery<UserExperienceDetails>("exec USP_GetCandidateExperienceDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return experiencedetails;
        }

        public bool DeleteExperienceDetails(int ExperienceId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var experienceid = new SqlParameter("@ExperienceId", ExperienceId);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteCandidateExperience  @ExperienceId", experienceid);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }

        //Identification Details
        public bool AddCandidateIdentificationDetails(string UserId, short IdentificationTypeId, string IdNumber, DateTime? IssuingDate, DateTime? ValidTill)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var identificationtypeid = new SqlParameter("@IdType", IdentificationTypeId);
                    var idnumber = new SqlParameter("@IdNumber", string.IsNullOrEmpty(IdNumber) ? DBNull.Value : (object)IdNumber);
                    var issuingdate = new SqlParameter("@IssuingDate", IssuingDate);

                    var validtill = new SqlParameter("@ValidTill", DBNull.Value);
                    if (ValidTill != null)
                        validtill = new SqlParameter("@ValidTill", ValidTill);
                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateIdentificationDetails @UserId, @IdType, @IdNumber, @IssuingDate, @ValidTill", userid, identificationtypeid, idnumber, issuingdate, validtill);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }


        public List<UserIdentificationDetailsView> GetCandidateIdentificationDetails(string UserId)
        {
            var identificationdetails = new List<UserIdentificationDetailsView>();
            using (var db = new UserDBContext())
            {
                identificationdetails = db.Database
                         .SqlQuery<UserIdentificationDetailsView>("exec USP_GetCandidateIdentificationDetails @UserId",
                           new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return identificationdetails;
        }

        public bool DeleteCandidateIndentificationDetails(string UserId, Int16 IdType)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);


                    var idType = new SqlParameter("@IdType", IdType);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteCandidateIndentificationDetails  @UserId,@IdType", userId, idType);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }

        //Social Details
        public bool AddCandidateSocailDetails(string UserId, string LinkedIn, string Facebook, string Skypeid, string GooglePlus)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var linkedin = new SqlParameter("@LinkedIn", string.IsNullOrEmpty(LinkedIn) ? DBNull.Value : (object)LinkedIn);
                    var facebook = new SqlParameter("@Facebook", string.IsNullOrEmpty(Facebook) ? DBNull.Value : (object)Facebook);
                    var skypeid = new SqlParameter("@Skypeid", string.IsNullOrEmpty(Skypeid) ? DBNull.Value : (object)Skypeid);
                    var googleplus = new SqlParameter("@GooglePlus", string.IsNullOrEmpty(GooglePlus) ? DBNull.Value : (object)GooglePlus);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateSocialDetails @UserId, @LinkedIn, @Facebook, @Skypeid, @GooglePlus", userid, linkedin, facebook, skypeid, googleplus);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }


        public UserSocialDetails GetCandidateSocialDetails(string UserId)
        {
            var socialdetails = new UserSocialDetails();
            using (var db = new UserDBContext())
            {
                socialdetails = db.Database
                         .SqlQuery<UserSocialDetails>("exec USP_GetCandidateSocialDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).FirstOrDefault();
            }

            return socialdetails;
        }


        //Vehicle Details
        public bool AddCandidateVehicleDetails(string UserId, string VehicleType, string VehicleNumber, string VehicleOwner, string DrivingLicence)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var vehicletype = new SqlParameter("@VehicleType", string.IsNullOrEmpty(VehicleType) ? DBNull.Value : (object)VehicleType);
                    var vehiclenumber = new SqlParameter("@VehicleNumber", string.IsNullOrEmpty(VehicleNumber) ? DBNull.Value : (object)VehicleNumber);
                    var vehicleowner = new SqlParameter("@VehicleOwner", string.IsNullOrEmpty(VehicleOwner) ? DBNull.Value : (object)VehicleOwner);
                    var drivinglicence = new SqlParameter("@DrivingLicence", string.IsNullOrEmpty(DrivingLicence) ? DBNull.Value : (object)DrivingLicence);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateVehicleDetails @UserId, @VehicleType, @VehicleNumber, @VehicleOwner, @DrivingLicence", userid, vehicletype, vehiclenumber, vehicleowner, drivinglicence);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }


        public UserVehicleDetails GetCandidateVehicleDetails(string UserId)
        {
            var vehicledetails = new UserVehicleDetails();
            using (var db = new UserDBContext())
            {
                vehicledetails = db.Database
                         .SqlQuery<UserVehicleDetails>("exec USP_GetCandidateVehicleDetails @UserId",
                           new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).FirstOrDefault();
            }

            return vehicledetails;
        }


        //Skill Details
        public bool AddCandidateSkillDetails(string UserId, string SkillName, short YearofExperience)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var skillName = new SqlParameter("@SkillName", string.IsNullOrEmpty(SkillName) ? DBNull.Value : (object)SkillName);
                    var yearofExperience = new SqlParameter("@YearofExperience", YearofExperience);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateSkillDetails @UserId, @SkillName, @YearofExperience", userid, skillName, yearofExperience);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }


        public List<UserSkillDetails> GetCandidateSkillDetails(string UserId)
        {
            var skillDetails = new List<UserSkillDetails>();
            using (var db = new UserDBContext())
            {
                skillDetails = db.Database
                         .SqlQuery<UserSkillDetails>("exec USP_GetCandidateSkillDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return skillDetails;
        }


        public bool DeleteSkillDetails(string UserId, string SkillName)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var skillName = new SqlParameter("@SkillName", string.IsNullOrEmpty(SkillName) ? DBNull.Value : (object)SkillName);

                    int i = context.Database.ExecuteSqlCommand("DeleteCandidateSkillDetails  @UserId, @SkillName", userId, skillName);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }

        //Fee Details
        public bool AddCandidateFeeDetails(string TransactionId, string UserId, float FeePaid, string CourseCode, Int64 BatchId, DateTime TransactionDate, DateTime? PaidOn, short PaymentMode, string BankName = null, string ReferenceNumber = null, string Status = null, string BankCode = null, string Remarks = null, string PGComment = null, Int16 TotalInstallment = 0, Int16 InstallmentNumber = 0, float RemainingAmount = 0, string ApprovedBy = null, double Conveyancefee = 0)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var userid = new SqlParameter("@UserId", (object)UserId);
                    var feePaid = new SqlParameter("@FeePaid", FeePaid);
                    var referenceNumber = new SqlParameter("@ReferenceNumber", string.IsNullOrEmpty(ReferenceNumber) ? DBNull.Value : (object)ReferenceNumber);
                    var bankName = new SqlParameter("@BankName", string.IsNullOrEmpty(BankName) ? DBNull.Value : (object)BankName);
                    var transactionId = new SqlParameter("@TransactionId", TransactionId);
                    var courseCode = new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode);
                    var batchId = new SqlParameter("@BatchId", BatchId);
                    var transactionDate = new SqlParameter("@TransactionDate", TransactionDate);

                    var paymentDate = new SqlParameter("@PaymentDate", TransactionDate);
                    if (PaidOn != null)
                        paymentDate = new SqlParameter("@PaymentDate", PaidOn);

                    var paymentMode = new SqlParameter("@PaymentMode", PaymentMode);

                    var status = new SqlParameter("@Status", string.IsNullOrEmpty(Status) ? DBNull.Value : (object)Status);
                    var bankCode = new SqlParameter("@BankCode", string.IsNullOrEmpty(BankCode) ? DBNull.Value : (object)BankCode);
                    var remarks = new SqlParameter("@Remarks", string.IsNullOrEmpty(Remarks) ? DBNull.Value : (object)Remarks);
                    var pGComment = new SqlParameter("@PGComment", string.IsNullOrEmpty(PGComment) ? DBNull.Value : (object)PGComment);
                    var totalInstallment = new SqlParameter("@TotalInstallment", TotalInstallment);
                    var installmentNumber = new SqlParameter("@InstallmentNumber", InstallmentNumber);
                    var remainingAmount = new SqlParameter("RemainingAmount", RemainingAmount);
                    var approvedBy = new SqlParameter("@ApprovedBy", string.IsNullOrEmpty(ApprovedBy) ? DBNull.Value : (object)ApprovedBy);
                    var conveyancefee = new SqlParameter("@Conveyancefee", Conveyancefee);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateFeeDetails @UserId, @FeePaid, @ReferenceNumber, @BankName, @TransactionId, @CourseCode, @BatchId, @TransactionDate, @PaymentDate, @PaymentMode, @Status, @BankCode, @Remarks, @PGComment, @TotalInstallment, @InstallmentNumber, @RemainingAmount, @ApprovedBy,@Conveyancefee",
                                                                userid, feePaid, referenceNumber, bankName, transactionId, courseCode, batchId, transactionDate, paymentDate, paymentMode, status, bankCode, remarks, pGComment, totalInstallment, installmentNumber, remainingAmount, approvedBy, conveyancefee);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }


        public string GetTransactionId(short PaymentModeId)
        {
            string year = Convert.ToString(DateTime.UtcNow.Year).Substring(2);
            string quarter = "Q1";
            int month = DateTime.UtcNow.Month;
            if (month > 3 && month <= 6)
            {
                quarter = "Q2";
            }
            else if (month > 6 && month <= 9)
            {
                quarter = "Q3";
            }
            else if (month > 9 && month <= 12)
            {
                quarter = "Q4";
            }

            string paymentMode = Convert.ToString(PaymentModeId);

            string transactionId = "T" + paymentMode + year + quarter + "000001";

            var transactions = from s in db.FeeDetails.Where(s => s.TransactionId.Substring(0, 6) == "T" + paymentMode + year + quarter)
                               orderby s.TransactionId descending
                               select s.TransactionId;

            var transaction = transactions.FirstOrDefault();

            if (transaction != null)
            {
                string transactionPartialId = transaction.Substring(6);
                int lastVal = Convert.ToInt32(transactionPartialId);
                lastVal = lastVal + 1;
                string suffix = string.Empty;

                for (int i = Convert.ToString(lastVal).Length; i < 6; i++)
                {
                    suffix = suffix + "0";
                }

                transactionId = transaction.Substring(0, 6) + suffix + Convert.ToString(lastVal);
            }
            return transactionId;
        }

        public List<FeeDetailsView> GetCandidateFeeDetails(string UserId, Int64 BatchId)
        {
            var feeDetails = new List<FeeDetailsView>();
            using (var db = new UserDBContext())
            {
                feeDetails = db.Database
                         .SqlQuery<FeeDetailsView>("exec USP_GetCandidateFeeDetails @UserId, @BatchId",
                           new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId),
                           new SqlParameter("@BatchId", BatchId)).ToList();
            }

            return feeDetails;
        }



        //Course Details
        public bool AddCandidateCourseDetails(string UserId, string CourseCode, DateTime CourseStartdate, DateTime CourseEndDate, float TotalFee, float Discount, float RemainingFee)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var courseCode = new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode);
                    var courseStartdate = new SqlParameter("@CourseStartdate", CourseStartdate);
                    var courseEndDate = new SqlParameter("@CourseEndDate", CourseEndDate);
                    var totalFee = new SqlParameter("@TotalFee", TotalFee);
                    var discount = new SqlParameter("@Discount", Discount);
                    var remainingFee = new SqlParameter("@RemainingFee", RemainingFee);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateCourseDetails @UserId, @CourseCode, @CourseStartdate, @CourseEndDate, @TotalFee, @Discount, @RemainingFee",
                                                                userid, courseCode, courseStartdate, courseEndDate, totalFee, discount, remainingFee);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }


        public List<CourseView> GetCandidateCourseDetails(string UserId)
        {
            var courseDetails = new List<CourseView>();
            using (var db = new UserDBContext())
            {
                courseDetails = db.Database
                         .SqlQuery<CourseView>("exec USP_GetCandidateCourseDetails @UserId",
                           new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return courseDetails;
        }


        public List<CourseMaster> GetCourseMastereList()
        {
            List<CourseMaster> CourseMasterList = db.CourseMaster.ToList();


            //CourseMaster t5default = new CourseMaster { CourseName = String.Empty };
            //CourseMasterList.Add(t5default);


            CourseMasterList = CourseMasterList.OrderBy(s => s.CourseName).ToList();
            return CourseMasterList;
        }

        public List<CourseMaster> GetCourseDetail(string UserId)
        {
            List<CourseMaster> CourseList = new List<CourseMaster>();

            using (var db = new UserDBContext())
            {
                CourseList = db.Database
                         .SqlQuery<CourseMaster>("exec USP_GetCourse @UserId",
                           new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return CourseList;
        }


        public List<CandidateViewModel> GetSubscriberWiseCandidateList(string SubscriberId, Int64 BatchId = 0)
        {
            List<CandidateViewModel> CandidateList = new List<CandidateViewModel>();

            using (var db = new UserDBContext())
            {
                CandidateList = db.Database
                         .SqlQuery<CandidateViewModel>("exec USP_GetSubsciberWiseCandidateList @SubscriberId, @BatchId",
                           new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId),
                            new SqlParameter("@BatchId", (object)BatchId)).ToList();
            }

            return CandidateList;
        }

        public List<CandidateViewModel> GetSubscriberWiseDistinctCandidateList(string SubscriberId)
        {
            List<CandidateViewModel> CandidateList = new List<CandidateViewModel>();

            using (var db = new UserDBContext())
            {
                CandidateList = db.Database
                         .SqlQuery<CandidateViewModel>("exec USP_GetSubsciberWiseDistictCandidateList @SubscriberId",
                           new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            return CandidateList;
        }



        public List<CorporateCandidateViewModel> GetCandidateListForAssignCourse(string SubscriberId, string CorporateId, string CourseCode)
        {
            List<CorporateCandidateViewModel> CandidateList = new List<CorporateCandidateViewModel>();

            using (var db = new UserDBContext())
            {
                CandidateList = db.Database
                         .SqlQuery<CorporateCandidateViewModel>("exec USP_GetCandidateListForAssignCourse @SubscriberId,@CorporateId,@CourseCode",
                            new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)
                           , new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)
                           , new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode)).ToList();
            }

            return CandidateList;
        }


        public List<CandidateTransactionViewModel> GetCandidatePaymentTransaction(string SubscriberId)
        {
            var transactionDetails = new List<CandidateTransactionViewModel>();
            using (var db = new UserDBContext())
            {
                transactionDetails = db.Database
                         .SqlQuery<CandidateTransactionViewModel>("exec USP_CandidatePaymentTransaction @SubscriberId",
                           new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            return transactionDetails;
        }

        public string GetUserName(string UserId)
        {
            var mydetails = GetCandidateGeneralDetails(UserId);
            if (mydetails != null)
            {
                return mydetails.Name;
            }
            return string.Empty;
        }

        //Address Details
        public void AddAddressDetails(string UserId, string AddressType, string AddressLine1, string AddressLine2, int City, int State, string PostalCode, int Country)
        {
            try
            {
                using (var context = new UserDBContext())
                {

                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var addresstype = new SqlParameter("@AddressType", string.IsNullOrEmpty(AddressType) ? DBNull.Value : (object)AddressType);
                    var addressline1 = new SqlParameter("@AddressLine1", string.IsNullOrEmpty(AddressLine1) ? DBNull.Value : (object)AddressLine1);
                    var addressline2 = new SqlParameter("@AddressLine2", string.IsNullOrEmpty(AddressLine2) ? DBNull.Value : (object)AddressLine2);
                    var city = new SqlParameter("@City", City);
                    var state = new SqlParameter("@State", State);
                    var postalcode = new SqlParameter("@PostalCode", string.IsNullOrEmpty(PostalCode) ? DBNull.Value : (object)PostalCode);
                    var country = new SqlParameter("@Country", Country);


                    int i = context.Database.ExecuteSqlCommand("dbo.USP_AddCandidateAddressDetails @UserId, @AddressType, @AddressLine1, @AddressLine2, @City, @State, @PostalCode, @Country",
                                                                userId, addresstype, addressline1, addressline2, city, state, postalcode, country);

                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

        }

        //Address Details
        public List<UserAddressViewModel> GetAddressDetails(string UserId)
        {
            var AddressList = new List<UserAddressViewModel>();
            using (var db = new UserDBContext())
            {
                AddressList = db.Database
                         .SqlQuery<UserAddressViewModel>("exec USP_GetCandidateAddressDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return AddressList;
        }

        public UserAddressViewModel GetAddressDetail(string UserId)
        {
            var AddressList = new UserAddressViewModel();
            using (var db = new UserDBContext())
            {
                AddressList = db.Database
                         .SqlQuery<UserAddressViewModel>("exec USP_GetCandidateAddressDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).FirstOrDefault();
            }

            return AddressList;
        }

        public bool RemoveAddressDetails(string UserId, string AddressType)
        {
            bool result = false;
            try
            {
                using (var context = new UserDBContext())
                {

                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var addresstype = new SqlParameter("@AddressType", string.IsNullOrEmpty(AddressType) ? DBNull.Value : (object)AddressType);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteCandidateAddressDetails @UserId, @AddressType", userId, addresstype);

                    if (i > 0)
                        result = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return result;
        }

        //public bool AddBankDetails(string CorporateId, string BankName, string AccountNumber, string IfscCode, string BranchCode, string BranchAddress, string ContactNumber)
        //{
        //    bool res = false;

        //    try
        //    {
        //        using (var context = new UserDBContext())
        //        {
        //            var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
        //            var bankname = new SqlParameter("@BankName", string.IsNullOrEmpty(BankName) ? DBNull.Value : (object)BankName);
        //            var accoutnumber = new SqlParameter("@AccountNumber", string.IsNullOrEmpty(AccountNumber) ? DBNull.Value : (object)AccountNumber);
        //            var ifsccode = new SqlParameter("@IfscCode", string.IsNullOrEmpty(IfscCode) ? DBNull.Value : (object)IfscCode);
        //            var branchcode = new SqlParameter("@BranchCode", string.IsNullOrEmpty(BranchCode) ? DBNull.Value : (object)BranchCode);
        //            var branchaddress = new SqlParameter("@BranchAddress", string.IsNullOrEmpty(BranchAddress) ? DBNull.Value : (object)BranchAddress);
        //            var contactnumber = new SqlParameter("@ContactNumber", string.IsNullOrEmpty(ContactNumber) ? DBNull.Value : (object)ContactNumber);


        //            int i = context.Database.ExecuteSqlCommand("USP_ADDBANKDETAILS @CorporateId, @BankName, @AccountNumber, @IfscCode, @BranchCode, @BranchAddress, @ContactNumber", CorporateId, BankName, AccountNumber, IfscCode, BranchCode, BranchAddress, ContactNumber);

        //            if (i == 1)
        //                res = true;
        //        }
        //    }
        //    catch (RetryLimitExceededException /* dex */)
        //    {
        //        //Log the error (uncomment dex variable name and add a line here to write a log.
        //        //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
        //    }

        //    return res;
        //}


        //public BankDetails GetBankDetails(string CorporateId)
        //{
        //    var bankdetails = new BankDetails();
        //    using (var db = new UserDBContext())
        //    {
        //        bankdetails = db.Database
        //                 .SqlQuery<BankDetails>("exec USP_GetBANKDEATAILS @CorporateId",
        //                    new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId))).FirstOrDefault();
        //    }

        //    return bankdetails;
        //}



        //public bool AddCompanyProfile(string CorporateId, string CompanyName, string CompanyType, string CompanySize, string Website)
        //{
        //    bool res = false;
        //    try
        //    {
        //        using (var context = new UserDBContext())
        //        {
        //            var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
        //            var companyname = new SqlParameter("@CompanyName", string.IsNullOrEmpty(CompanyName) ? DBNull.Value : (object)CompanyName);
        //            var companytype = new SqlParameter("@CompanyType", string.IsNullOrEmpty(CompanyType) ? DBNull.Value : (object)CompanyType);
        //            var companysize = new SqlParameter("@CompanySize", string.IsNullOrEmpty(CompanySize) ? DBNull.Value : (object)CompanySize);
        //            var website = new SqlParameter("@Website", string.IsNullOrEmpty(Website) ? DBNull.Value : (object)Website);


        //            int i = context.Database.ExecuteSqlCommand("USP_AddCompanyProfiles @CorporateId, @CompanyName, @CompanyType, @CompanySize, @Website", CorporateId, CompanyName, CompanyType, CompanySize, Website);

        //            if (i == 1)
        //                res = true;
        //        }
        //    }
        //    catch (RetryLimitExceededException /* dex */)
        //    {
        //        //Log the error (uncomment dex variable name and add a line here to write a log.
        //        //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
        //    }

        //    return res;
        //}


        //public CompanyProfile GetCompanyProfile(string CorporateId)
        //{
        //    var companyprofiles = new CompanyProfile();
        //    using (var db = new UserDBContext())
        //    {
        //        companyprofiles = db.Database
        //                 .SqlQuery<CompanyProfile>("exec USP_GetCompanyProfiles @CorporateId",
        //                    new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId))).FirstOrDefault();
        //    }

        //    return companyprofiles;
        //}


        //public bool AddCorporateProfile(string CorporateId, string Name, string AlternateContact, string AlternateEmail, string Nationality, string DepartmentId, string SubscriberId)
        //{
        //    bool res = false;
        //    try
        //    {
        //        using (var context = new UserDBContext())
        //        {
        //            var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
        //            var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
        //            var alternatecontact = new SqlParameter("@AlternateContact", string.IsNullOrEmpty(AlternateContact) ? DBNull.Value : (object)AlternateContact);
        //            var alternateemail = new SqlParameter("@AlternateEmail", string.IsNullOrEmpty(AlternateEmail) ? DBNull.Value : (object)AlternateEmail);
        //            var nationality = new SqlParameter("@Nationality", string.IsNullOrEmpty(Nationality) ? DBNull.Value : (object)Nationality);
        //            var departmentid = new SqlParameter("@DepartmentId", string.IsNullOrEmpty(DepartmentId) ? DBNull.Value : (object)DepartmentId);
        //            var subscriberid = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);


        //            int i = context.Database.ExecuteSqlCommand("USP_AddCorporateProfiles @CorporateId, @Name, @AlternateContact, @AlternateEmail, @Nationality, @DepartmentId, @SubscriberId", CorporateId, Name, AlternateContact, AlternateEmail, Nationality, DepartmentId, SubscriberId);

        //            if (i == 1)
        //                res = true;
        //        }
        //    }
        //    catch (RetryLimitExceededException /* dex */)
        //    {
        //        //Log the error (uncomment dex variable name and add a line here to write a log.
        //        //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
        //    }

        //    return res;
        //}

        public List<CourseBatchViewModel> GetCandidateWiseCourseDetail(string UserId, bool IsCandidateView = true, Int64 BatchId = 0)
        {
            var CourseBatches = new List<CourseBatchViewModel>();
            using (var db = new UserDBContext())
            {
                CourseBatches = db.Database
                         .SqlQuery<CourseBatchViewModel>("exec USP_CandidateWiseCourseDetails @UserId, @IsCandidateView , @BatchId ",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                            , new SqlParameter("@IsCandidateView", (object)IsCandidateView)
                             , new SqlParameter("@BatchId", (object)BatchId)).ToList();
            }

            return CourseBatches;
        }

        public bool UpdateCandidateLikesforCourse(string UserId, string BatchId)
        {
            bool result = false;
            try
            {
                using (var context = new UserDBContext())
                {

                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var batchId = new SqlParameter("@BatchId", string.IsNullOrEmpty(BatchId) ? DBNull.Value : (object)BatchId);

                    int i = context.Database.ExecuteSqlCommand("USP_UPDATECOURSELIKE @UserId, @BatchId", userId, batchId);

                    if (i > 0)
                        result = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return result;
        }


        //Languages By: Ajay Kumar Choudhary
        public bool AddLaguages(Int64 UserLanguageId, string UserId, Int32 LanguageId, bool Read, bool Write, bool Speak)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var userLanguageId = new SqlParameter("@UserLanguageId", UserLanguageId);
                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var languageId = new SqlParameter("@LanguageId", LanguageId);
                    var read = new SqlParameter("@Read", Read);
                    var write = new SqlParameter("@Write", Write);
                    var speak = new SqlParameter("@Speak", Speak);


                    int i = context.Database.ExecuteSqlCommand("USP_Addlanguages @UserLanguageId, @UserId, @LanguageId, @Read, @Write, @Speak", userLanguageId, userid, languageId, read, write, speak);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }


        public List<LanguagesView> GetLanguages(string UserId)
        {
            var langugages = new List<LanguagesView>();
            using (var db = new UserDBContext())
            {
                langugages = db.Database
                         .SqlQuery<LanguagesView>("exec USP_GetLanguages @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return langugages;
        }

        public List<LanguageMaster> GetLanguageList()
        {
            var langugagelist = new List<LanguageMaster>();
            using (var db = new UserDBContext())
            {
                langugagelist = db.Database
                         .SqlQuery<LanguageMaster>("exec USP_GetLangaugeList").ToList();
            }

            return langugagelist;
        }

        public void DeleteLanguage(Int64 UserLanguageId)
        {
            var language = db.Languages.Find(UserLanguageId);
            if (language != null)
            {
                db.Languages.Remove(language);
                db.SaveChanges();
            }
        }
        //for scheme by vikas pandey ,17/11/2017
        public List<LeaveSchemeMaster> GetScheme()
        {
            List<LeaveSchemeMaster> leavescheme;
            using (var db = new UserDBContext())
            {

                leavescheme = db.Database.SqlQuery<LeaveSchemeMaster>("exec Usp_GetScheme").ToList();
            }
            return leavescheme;
        }

        //For Installment By: Ajay Kumar Choudhary
        public bool AddInstallments(string UserId, Int16 InstallmentId, string CourseCode, Int64 BatchId, DateTime UpdatedOn, string UpdatedBy, double TotalFeeAmount, bool Accommodation, bool Transport, bool Others, bool InstallmentInterest, bool Discount)
        {
            bool res = false;

            using (var context = new UserDBContext())
            {

                var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                var intallments = new SqlParameter("@InstallmentId", InstallmentId);
                var courseCode = new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode);
                var batchId = new SqlParameter("@BatchId", BatchId);
                var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                var totalFeeAmount = new SqlParameter("@TotalFeeAmount", TotalFeeAmount);
                var accommodation = new SqlParameter("@Accommodation", Accommodation);
                var transport = new SqlParameter("@Transport", Transport);
                var others = new SqlParameter("@Others", Others);
                var installmentInterest = new SqlParameter("@InstallmentInterest", InstallmentInterest);
                var discount = new SqlParameter("@Discount", Discount);

                int i = context.Database.ExecuteSqlCommand("USP_AddlInstallments @UserId, @InstallmentId, @CourseCode, @BatchId, @UpdatedOn, @UpdatedBy, @TotalFeeAmount, @Accommodation, @Transport, @Others, @InstallmentInterest, @Discount"
                                                            , userid, intallments, courseCode, batchId, updatedOn, updatedBy, totalFeeAmount, accommodation, transport, others, installmentInterest, discount);

                if (i == 1)
                    res = true;
            }

            return res;
        }

        public CourseMasterView GetInstallmentDetails(string UserId, Int64 BatchId = 0)
        {
            var fees = new CourseMasterView();
            using (var db = new UserDBContext())
            {
                fees = db.Database
                         .SqlQuery<CourseMasterView>("exec USP_CandidateFeeSetting @UserId, @BatchId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId),
                            new SqlParameter("@BatchId", BatchId)).FirstOrDefault();
            }

            return fees;
        }

    }

}