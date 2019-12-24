using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AJSolutions.Models;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using AJSolutions.Areas.EMS.Models;
using AJSolutions.Areas.PMS.Models;

namespace AJSolutions.DAL
{

    public class EMSManager
    {
        UserDBContext udbc = new UserDBContext();
        /// <summary>
        /// Updated by : Achal Jha 
        /// Updation Date : 17-05-2017
        /// Reason : For Payroll
        /// </summary>        
        /// <param name="status"></param>
        /// <param name="dateofJoining"></param>
        /// <param name="probationPeriod"></param>
        /// <param name="fatherName"></param>
        /// <param name="spouseName"></param>
        /// <param name="grade"></param>
        /// <param name="dateofConfirmation"></param>
        /// <returns></returns>
        //, string Status, DateTime? DateofJoining, int ProbationPeriod, Int64 GradeId, DateTime? DateofConfirmation
        public bool AddEmployeeBasicDetails(string Name, DateTime? DOB, string UserId, string SubscriberId, string Gender, string MaritalStatus, string AlternateContact, string AlternateEmail, string Nationality, string EmployeeId, bool Emplanelled, string DepartmentId, string FatherName, string SpouseName)
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
                    //var designation = new SqlParameter("@Designation", string.IsNullOrEmpty(Designation) ? DBNull.Value : (object)Designation);
                    var employeeid = new SqlParameter("@EmployeeId", string.IsNullOrEmpty(EmployeeId) ? DBNull.Value : (object)EmployeeId);
                    var emplanelled = new SqlParameter("@Emplanelled", Emplanelled);
                    var departmentid = new SqlParameter("@DepartmentId", string.IsNullOrEmpty(DepartmentId) ? DBNull.Value : (object)DepartmentId);
                    //string grade,DateTime? dateofConfirmation
                    //var status = new SqlParameter("@Status", string.IsNullOrEmpty(Status) ? DBNull.Value : (object)Status);
                    //var dateofJoining = new SqlParameter("@DateofJoining", DBNull.Value);
                    //if (DateofJoining != null)
                    //    dateofJoining = new SqlParameter("@DateofJoining", DateofJoining);
                    //var probationPeriod = new SqlParameter("@ProbationPeriod", ProbationPeriod);
                    var fatherName = new SqlParameter("@FatherName", string.IsNullOrEmpty(FatherName) ? DBNull.Value : (object)FatherName);
                    var spouseName = new SqlParameter("@SpouseName", string.IsNullOrEmpty(SpouseName) ? DBNull.Value : (object)SpouseName);
                    //var grade = new SqlParameter("@GradeId", GradeId);
                    //var dateofConfirmation = new SqlParameter("@DateofConfirmation", string.IsNullOrEmpty(DateofConfirmation.ToString()) ? DBNull.Value : (object)DateofConfirmation);

                    int i = context.Database.ExecuteSqlCommand("USP_AddEmployeeBasicDetails @UserId, @Name, @DOB," +
                            "@Gender, @MaritalStatus, @AlternateContact, @AlternateEmail, @Nationality," +
                            " @SubscriberId, @EmployeeId, @Emplanelled, @DepartmentId," +
                            "@FatherName, @SpouseName"
                            , userid, name, dob, gender, maritalstatus, alternateContact,
                            alternateEmail, nationality, subscriberid, employeeid, emplanelled,
                            departmentid, fatherName, spouseName);
                    //status, dateofJoining, probationPeriod,  grade, dateofConfirmation
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

        public List<EmployeeBasicDetails> GetEmployeeBasicDetails(string UserId = null)
        {
            var empdetails = new List<EmployeeBasicDetails>();
            using (var db = new UserDBContext())
            {
                empdetails = db.Database
                         .SqlQuery<EmployeeBasicDetails>("exec USP_GetEmployeeBasicDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return empdetails;
        }

        //Educational Details
        public bool AddEmployeeEducationalDetails(string UserId, short EducationLevel, string Degree, string Specialization, string University, string Institution, string YearOfPassing, string Percentage)
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

                    int i = context.Database.ExecuteSqlCommand("USP_AddEmployeeEducationalDetails @UserId, @EducationLevel, @Degree, @Specialization, @University, @Institution, @YearOfPassing, @Percentage", userid, educationlevel, degree, specialization, university, institution, yearofpassing, percentage);

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


        public List<EmpEducationView> GetEmployeeEducationalDetails(string UserId)
        {
            var EmpEducationaldetails = new List<EmpEducationView>();
            using (var db = new UserDBContext())
            {
                EmpEducationaldetails = db.Database
                         .SqlQuery<EmpEducationView>("exec USP_GetEmployeeEducationalDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return EmpEducationaldetails;
        }

        public bool DeleteEmployeeEducationDetail(string UserId, short EducationLevel)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var educationLevel = new SqlParameter("@EducationLevel", EducationLevel);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteEmployeeEducationDetail  @UserId ,  @EducationLevel", userId, educationLevel);

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
        public bool AddEmployeeExperienceDetails(long ExperienceId, string UserId, string ComapanyName, string WorkLocation, string LatestDesignation, DateTime JoiningDate, DateTime? LeavingDate, bool WorkingStatus, int ProfileId)
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
                    var latestdesignation = new SqlParameter("@LatestDesignation", string.IsNullOrEmpty(LatestDesignation) ? DBNull.Value : (object)LatestDesignation);
                    var joiningdate = new SqlParameter("@JoiningDate", JoiningDate);
                    var leavingdate = new SqlParameter("@LeavingDate", DBNull.Value);
                    if (LeavingDate != null)
                        leavingdate = new SqlParameter("@LeavingDate", LeavingDate);

                    var workingstatus = new SqlParameter("@WorkingStatus", WorkingStatus);
                    var profileid = new SqlParameter("@ProfileId", ProfileId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddEmployeeExperienceDetails @ExperienceId, @UserId, @ComapanyName, @WorkLocation, @LatestDesignation, @JoiningDate, @LeavingDate, @WorkingStatus, @ProfileId", experienceid, userid, comapanyname, worklocation, latestdesignation, joiningdate, leavingdate, workingstatus, profileid);

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


        public List<EmpExperienceView> GetEmployeeExperienceDetails(string UserId = null)
        {
            var Empexperiencedetails = new List<EmpExperienceView>();
            using (var db = new UserDBContext())
            {
                Empexperiencedetails = db.Database
                         .SqlQuery<EmpExperienceView>("exec USP_GetEmployeeExperienceDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return Empexperiencedetails;
        }

        public bool DeleteExperience(int ExperienceId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var experienceid = new SqlParameter("@ExperienceId", ExperienceId);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteExperience  @ExperienceId", experienceid);

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

        //Skill Details
        public bool AddEmployeeSkillDetails(string UserId, string SkillName, long ProfileId, short YearofExperience)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var skillname = new SqlParameter("@SkillName", string.IsNullOrEmpty(SkillName) ? DBNull.Value : (object)SkillName);
                    var profileid = new SqlParameter("@ProfileId", ProfileId);
                    var yearofexperience = new SqlParameter("@YearofExperience", YearofExperience);

                    int i = context.Database.ExecuteSqlCommand("USP_AddEmployeeSkillDetails @UserId, @SkillName, @ProfileId, @YearofExperience", userid, skillname, profileid, yearofexperience);

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

        //skill details
        public List<EmpSkillView> GetEmployeeSkillDetails(string UserId)
        {
            var EmpSkilldetails = new List<EmpSkillView>();
            using (var db = new UserDBContext())
            {
                EmpSkilldetails = db.Database
                         .SqlQuery<EmpSkillView>("exec USP_GetEmployeeSkillDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return EmpSkilldetails;
        }

        public bool DeleteEmployeeSkills(string UserId, string SkillName, Int64 ProfileId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);

                    var skillName = new SqlParameter("@SkillName", string.IsNullOrEmpty(SkillName) ? DBNull.Value : (object)SkillName);

                    var profileid = new SqlParameter("@ProfileId", ProfileId);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteEmployeeSkillDetails  @UserId,@SkillName, @ProfileId", userId, skillName, profileid);

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
        public bool AddEmployeeIdentificationDetails(string UserId, short IdentificationTypeId, string IdNumber, DateTime? IssuingDate, DateTime? ValidTill)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var identificationtypeid = new SqlParameter("@IdType", IdentificationTypeId);
                    var idnumber = new SqlParameter("@IdNumber", string.IsNullOrEmpty(IdNumber) ? DBNull.Value : (object)IdNumber);
                    //var issuingdate = new SqlParameter("@IssuingDate", IssuingDate);
                    var issuingdate = new SqlParameter("@IssuingDate", DBNull.Value);
                    if (IssuingDate != null)
                        issuingdate = new SqlParameter("@IssuingDate", IssuingDate);

                    var validtill = new SqlParameter("@ValidTill", DBNull.Value);
                    if (ValidTill != null)
                        validtill = new SqlParameter("@ValidTill", ValidTill);

                    int i = context.Database.ExecuteSqlCommand("USP_AddEmployeeIdentificationDetails @UserId, @IdType, @IdNumber, @IssuingDate, @ValidTill", userid, identificationtypeid, idnumber, issuingdate, validtill);

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


        public List<EmpIdentificationDetailsView> GetEmployeeIdentificationDetails(string UserId)
        {
            var Empidentificationdetails = new List<EmpIdentificationDetailsView>();
            using (var db = new UserDBContext())
            {
                Empidentificationdetails = db.Database
                         .SqlQuery<EmpIdentificationDetailsView>("exec USP_GetEmployeeIdentificationDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return Empidentificationdetails;
        }


        public bool DeleteEmployeeIndentificationDetails(string UserId, Int16 IdType)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);


                    var idType = new SqlParameter("@IdType", IdType);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteEmployeeIndentificationDetails  @UserId,@IdType", userId, idType);

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
        public bool AddEmployeeSocailDetails(string UserId, string LinkedIn, string Facebook, string Skypeid, string GooglePlus)
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

                    int i = context.Database.ExecuteSqlCommand("USP_AddEmployeeSocialDetails @UserId, @LinkedIn, @Facebook, @Skypeid, @GooglePlus", userid, linkedin, facebook, skypeid, googleplus);

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


        public EmpSocialDetails GetEmployeeSocialDetails(string UserId)
        {
            var Empsocialdetails = new EmpSocialDetails();
            using (var db = new UserDBContext())
            {
                Empsocialdetails = db.Database
                         .SqlQuery<EmpSocialDetails>("exec USP_GetEmployeeSocialDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).FirstOrDefault();
            }

            return Empsocialdetails;
        }

        //profile type details
        public bool AddProfileTypeDetails(int ProfileId, string UserId, string ProfileName)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var profileid = new SqlParameter("@ProfileId", ProfileId);
                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var profilename = new SqlParameter("@ProfileName", string.IsNullOrEmpty(ProfileName) ? DBNull.Value : (object)ProfileName);

                    int i = context.Database.ExecuteSqlCommand("USP_AddEmployeeProfileTypeDetails @ProfileId, @UserId, @ProfileName", profileid, userid, profilename);

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

        //profile type details
        public List<UserProfileTypeDetails> GetProfileTypeDetails(string UserId)
        {
            var Empprofiletypedetails = new List<UserProfileTypeDetails>();
            using (var db = new UserDBContext())
            {
                Empprofiletypedetails = db.Database
                         .SqlQuery<UserProfileTypeDetails>("exec USP_GetEmployeeProfileTypeDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return Empprofiletypedetails;
        }


        public bool DeleteProfileDetails(string UserId, string ProfileId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);

                    var profileid = new SqlParameter("@ProfileId", string.IsNullOrEmpty(ProfileId) ? DBNull.Value : (object)ProfileId);

                    int i = context.Database.ExecuteSqlCommand("USP_DELETEPROFILESDETAILS  @UserId, @ProfileId", userId, profileid);

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

        public List<UserProfileTypeDetails> GetProfileTypeList(string UserId)
        {
            List<UserProfileTypeDetails> ProfileTypeList = (from p in udbc.UserProfileTypeDetails.Where(p => p.UserId == UserId) select p).ToList();

            //  UserProfileTypeDetails t5default = new UserProfileTypeDetails {ProfileId = 0, ProfileName = "Default" };
            //  ProfileTypeList.Add(t5default);

            ProfileTypeList = ProfileTypeList.OrderBy(s => s.ProfileId).ToList();
            return ProfileTypeList;
        }


        public List<EducationLevelMaster> GetEducationLevelList()
        {
            List<EducationLevelMaster> EducationLevelList = udbc.EducatioanLevelMaster.ToList();

            //EducationLevelMaster t5default = new EducationLevelMaster { EducationLevelName = String.Empty };
            //EducationLevelList.Add(t5default);

            EducationLevelList = EducationLevelList.OrderBy(s => s.EducationLevelName).ToList();
            return EducationLevelList;
        }

        public List<IdentificationTypeMaster> GetIdentificationTypeList()
        {
            List<IdentificationTypeMaster> IdentificationTypeList = udbc.IdentificationTypeMaster.ToList();

            //IdentificationTypeMaster t5default = new IdentificationTypeMaster { IdentificationTypeName = String.Empty };
            //IdentificationTypeList.Add(t5default);

            IdentificationTypeList = IdentificationTypeList.OrderBy(s => s.IdentificationTypeName).ToList();
            return IdentificationTypeList;
        }


        public List<PaymentModeMaster> PaymentModeTypeList()
        {
            List<PaymentModeMaster> PaymentModeList = udbc.PaymentModeMaster.ToList();

            //PaymentModeMaster t5default = new PaymentModeMaster { PaymentMode = String.Empty };
            //PaymentModeList.Add(t5default);

            PaymentModeList = PaymentModeList.OrderBy(s => s.PaymentMode).ToList();
            return PaymentModeList;
        }

        public List<EmployeeViewModel> GetSubscriberWiseEmployeeList(string SubscriberId)
        {
            List<EmployeeViewModel> EmployeeList = new List<EmployeeViewModel>();

            using (var db = new UserDBContext())
            {
                EmployeeList = db.Database
                         .SqlQuery<EmployeeViewModel>("exec USP_GetSubsciberWiseEmployeeList @SubscriberId",
                           new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            return EmployeeList;
        }

        public EmployeeView GetEmployeeSalaryDetails(string UserId)
        {
            EmployeeView EmployeeDetails = new EmployeeView();

            using (var db = new UserDBContext())
            {
                EmployeeDetails = db.Database
                         .SqlQuery<EmployeeView>("exec USP_GetEmployeeSalaryDetails @UserId",
                           new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).FirstOrDefault();
            }

            return EmployeeDetails;
        }

        public List<EmployeeSalaryHeadsView> GetEmployeesalaryheadsDetails(Int64 ESID)
        {
            List<EmployeeSalaryHeadsView> EmployeeList = new List<EmployeeSalaryHeadsView>();

            using (var db = new UserDBContext())
            {
                EmployeeList = db.Database
                         .SqlQuery<EmployeeSalaryHeadsView>("exec USP_GetEmployeeSalaryHeadsDetails @ESID",
                           new SqlParameter("@ESID", ESID)).ToList();
            }

            return EmployeeList;
        }

        public string GetUserName(string UserId)
        {
            var empDetails = GetEmployeeBasicDetails(UserId).FirstOrDefault();
            if (empDetails != null)
            {
                return empDetails.Name;
            }
            return string.Empty;
        }

        //Address Details
        public void AddEmpAddressDetails(string UserId, string AddressType, string AddressLine1, string AddressLine2, int City, int State, string PostalCode, int Country)
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


                    int i = context.Database.ExecuteSqlCommand("USP_AddEmpAddressDetails @UserId, @AddressType, @AddressLine1, @AddressLine2, @City, @State, @PostalCode, @Country",
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
        public List<EmpAddressViewModel> GetAddressDetails(string UserId)
        {
            var AddressList = new List<EmpAddressViewModel>();
            using (var db = new UserDBContext())
            {
                AddressList = db.Database
                         .SqlQuery<EmpAddressViewModel>("exec USP_GetEmpAddressDetails @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return AddressList;
        }

        public EmpAddressViewModel GetAddressDetail(string UserId)
        {
            var AddressList = new EmpAddressViewModel();
            using (var db = new UserDBContext())
            {
                AddressList = db.Database
                         .SqlQuery<EmpAddressViewModel>("exec USP_GetEmpAddressDetails @UserId",
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

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteEmpAddressDetails @UserId, @AddressType", userId, addresstype);

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
        //Birthday Calender
        public List<EmployeeBasicDetails> GetBirthday(string SubscriberId)
        {
            var employee = new List<EmployeeBasicDetails>();


            using (var db = new UserDBContext())
            {

                employee = db.Database.SqlQuery<EmployeeBasicDetails>("EXEC USP_GetBirthday @SubscriberId ",
                            new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            return employee;
        }
        //Method for geting engagement type masters
        public List<EngagementTypeMaster> GetEngagementTypeMaster(string SubscriberId)
        {
            var getengagementtype = new List<EngagementTypeMaster>();

            using (var db = new UserDBContext())
            {
                getengagementtype = db.Database
                          .SqlQuery<EngagementTypeMaster>("EXEC USP_GetEngagementTypeMaster @SubscriberId",
                new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            return getengagementtype;
        }
        //Method for adding engagement type masters
        public bool AddEngagementTypeMaster(Int64 EngagementTypeId, string LeaveTypeName, string CorporateId, string ShortName, Int16 SchemeId, string LeaveTypeId, string LeaveTypeCategory, int LeaveLimit, DateTime EffectiveFrom, string YearEndAction, Int16 MaxLimit)
        {
            bool res = false;

            using (var context = new UserDBContext())
            {
                var engagementtypeid = new SqlParameter("@EngagementTypeId", EngagementTypeId);
                var engagementtype = new SqlParameter("@EngagementType", string.IsNullOrEmpty(LeaveTypeName) ? DBNull.Value : (object)LeaveTypeName);
                var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                var shortname = new SqlParameter("@ShortName", string.IsNullOrEmpty(ShortName) ? DBNull.Value : (object)ShortName);
                var schemeid = new SqlParameter("@SchemeId", SchemeId);
                var leavetypeid = new SqlParameter("@LeaveTypeId", string.IsNullOrEmpty(LeaveTypeId) ? DBNull.Value : (object)LeaveTypeId);
                var leavetypecategory = new SqlParameter("@LeaveTypeCategory", string.IsNullOrEmpty(LeaveTypeCategory) ? DBNull.Value : (object)LeaveTypeCategory);
                var leavelimit = new SqlParameter("@LeaveLimit", LeaveLimit);
                var effectivefrom = new SqlParameter("@EffectiveFrom", EffectiveFrom);
                var yearendaction = new SqlParameter("@YearEndAction", string.IsNullOrEmpty(YearEndAction) ? DBNull.Value : (object)YearEndAction);
                var maxlimit = new SqlParameter("@MaxLimit", MaxLimit);

                int i = context.Database.ExecuteSqlCommand("USP_AddEngagementTypeMaster  @EngagementTypeId,@EngagementType,@CorporateId,@ShortName,@SchemeId,@LeaveTypeId,@LeaveTypeCategory,@LeaveLimit,@EffectiveFrom,@YearEndAction,@MaxLimit", engagementtypeid
                    , engagementtype, corporateid, shortname, schemeid, leavetypeid, leavetypecategory, leavelimit, effectivefrom, yearendaction, maxlimit);

                if (i == 1)
                    res = true;
            }


            return res;
        }
        //by vikas pandey 20/11/2017 for union leave type and engagementtypemaster
        public List<EngagementTypeMasterView> GetEngagementLeaveUnion(string SubscriberId)
        {
            var getengagementtype = new List<EngagementTypeMasterView>();

            using (var db = new UserDBContext())
            {
                getengagementtype = db.Database
                          .SqlQuery<EngagementTypeMasterView>("EXEC USP_GetEngagementType @CorporarteId",
                new SqlParameter("@CorporarteId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            return getengagementtype;
        }
        //Method for geting EmployeeEngagement 
        public List<TrainerPlannerView> GetEmployeeEngagement(string CorporateId, DateTime? StartDate, DateTime? EndDate)
        {
            var empangagement = new List<TrainerPlannerView>();

            using (var db = new UserDBContext())
            {
                empangagement = db.Database
                          .SqlQuery<TrainerPlannerView>("EXEC USP_GetEmployeeEngagement @CorporateId,@StartDate,@EndDate",
                          new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId),
                          new SqlParameter("@StartDate", StartDate == null ? DBNull.Value : (object)StartDate),
                          new SqlParameter("@EndDate", EndDate == null ? DBNull.Value : (object)EndDate)).ToList();
            }
            return empangagement;
        }

        public List<EmployeeLeaveSummariesViewModel> GetEmployeeLeaveSummary(string CorporateId, int LeaveYear, Int16 SchemeId, string DepartmentId, string EmployeeId)
        {
            var empangagement = new List<EmployeeLeaveSummariesViewModel>();

            using (var db = new UserDBContext())
            {
                empangagement = db.Database
                          .SqlQuery<EmployeeLeaveSummariesViewModel>("EXEC USP_GetEMPLeaveBalance @SubscriberId ,  @LeaveYear,  @SchemeId , @DepartmentId , @EmployeeId",
                                                                      new SqlParameter("@SubscriberId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId),
                                                                      new SqlParameter("@LeaveYear", LeaveYear),
                                                                      new SqlParameter("@SchemeId", SchemeId),
                                                                      new SqlParameter("@DepartmentId", string.IsNullOrEmpty(DepartmentId) ? DBNull.Value : (object)DepartmentId),
                                                                      new SqlParameter("@EmployeeId", string.IsNullOrEmpty(EmployeeId) ? DBNull.Value : (object)EmployeeId)
                                                                    ).ToList();
            }
            return empangagement;
        }

        public List<TaskCommentsForumView> GetTaskComments(string TaskId)
        {
            var TaskComments = new List<TaskCommentsForumView>();
            using (var db = new UserDBContext())
            {
                TaskComments = db.Database.SqlQuery<TaskCommentsForumView>("EXEC USP_GetTaskComments @TaskId",
                    new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId)).ToList();
            }
            return TaskComments;
        }

        public List<TaskReplyForumView> GetTaskReplies()
        {
            var TaskReplies = new List<TaskReplyForumView>();
            using (var db = new UserDBContext())
            {
                TaskReplies = db.Database.SqlQuery<TaskReplyForumView>("EXEC USP_GetTaskReplies").ToList();
            }
            return TaskReplies;
        }

        public bool AddTaskComments(string TaskId, string Comment, DateTime CommentedOn, string UserId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var taskId = new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId);
                    var comment = new SqlParameter("@Comment", string.IsNullOrEmpty(Comment) ? DBNull.Value : (object)Comment);
                    var commentedOn = new SqlParameter("@CommentedOn", CommentedOn);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddTaskComments @TaskId, @Comment, @CommentedOn, @UserId", taskId, comment, commentedOn, userId);

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

        public bool AddTaskReplies(Int64 TaskCommentId, string Reply, DateTime RepliedOn, string UserId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var taskCommentId = new SqlParameter("@TaskCommentId", TaskCommentId);
                    var reply = new SqlParameter("@Reply", string.IsNullOrEmpty(Reply) ? DBNull.Value : (object)Reply);
                    var repliedOn = new SqlParameter("@RepliedOn", RepliedOn);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddTaskReplies @TaskCommentId, @Reply, @RepliedOn, @UserId", taskCommentId, reply, repliedOn, userId);

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

        public int GetTaskCommentsCount(string TaskId)
        {

            int taskCommentsCount;

            using (var db = new UserDBContext())
            {
                taskCommentsCount = db.Database
                          .SqlQuery<int>("EXEC USP_GetTaskCommentsCount @TaskId ",
                          new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId)
                          ).FirstOrDefault();
            }

            return taskCommentsCount;
        }

        public void RemoveTaskComment(Int64 TaskCommentId)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var taskCommentId = new SqlParameter("@TaskCommentId", TaskCommentId);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteTaskComment @TaskCommentId", taskCommentId);
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
        }

        public void RemoveTaskReply(Int64 TaskReplyId)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var taskReplyId = new SqlParameter("@TaskReplyId", TaskReplyId);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteTaskReply @TaskReplyId", taskReplyId);
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
        }
        /// <summary>
        /// Created By : Achal Kumar Jha
        /// Created On : 18-05-2017
        /// Reason     : For Payroll
        /// </summary>
        /// <param name="GradeName"></param>
        /// <param name="CorporateId"></param>
        /// <returns></returns>

        public bool AddGrade(Int64 GradeId, string GradeName, string CorporateId)
        {
            bool res = false;
            using (var context = new UserDBContext())
            {
                var gradeid = new SqlParameter("@GradeId", GradeId);
                var gradename = new SqlParameter("@GradeName", string.IsNullOrEmpty(GradeName) ? DBNull.Value : (object)GradeName);
                var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);

                int i = context.Database.ExecuteSqlCommand("USP_AddGrade  @GradeId,@GradeName,@CorporateId", gradeid, gradename, corporateid);
                if (i == 1)
                    res = true;
            }
            return res;
        }

        //By: Ajay Kumar Choudhary
        //On: 20-07-2017
        //For:Certifications
        public bool AddCertification(Int64 CertificationId, string UserId, string Certificate, string Specialization, string Institution, string YearOfPassing, string Percentage)
        {
            bool res = false;
            using (var context = new UserDBContext())
            {
                var certificationId = new SqlParameter("@CertificationId", CertificationId);
                var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                var certificate = new SqlParameter("@Certificate", string.IsNullOrEmpty(Certificate) ? DBNull.Value : (object)Certificate);
                var specialization = new SqlParameter("@Specialization", string.IsNullOrEmpty(Specialization) ? DBNull.Value : (object)Certificate);
                var institution = new SqlParameter("@Institution", string.IsNullOrEmpty(Institution) ? DBNull.Value : (object)Institution);
                var yearOfPassing = new SqlParameter("@YearOfPassing", string.IsNullOrEmpty(YearOfPassing) ? DBNull.Value : (object)YearOfPassing);
                var percentage = new SqlParameter("@Percentage", string.IsNullOrEmpty(Percentage) ? DBNull.Value : (object)Percentage);
                int i = context.Database.ExecuteSqlCommand("USP_AddCertifications  @CertificationId,@UserId,@Certificate, @Specialization, @Institution, @YearOfPassing, @Percentage",
                                                                                    certificationId, userId, certificate, specialization, institution, yearOfPassing, percentage);
                if (i == 1)
                    res = true;
            }
            return res;
        }

        public List<Certification> GetCertification(string UserId)
        {
            var certificate = new List<Certification>();
            using (var db = new UserDBContext())
            {
                certificate = db.Database
                         .SqlQuery<Certification>("exec USP_GETCertification @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return certificate;
        }

        public void DeleteCertificate(Int64 CertificationId)
        {
            var certificate = udbc.Certification.Find(CertificationId);
            if (certificate != null)
            {
                udbc.Certification.Remove(certificate);
                udbc.SaveChanges();
            }
        }

        public void DeleteEngagement(Int64 EngagementTypeId)
        {
            var removeItem = udbc.EngagementTypeMaster.Find(EngagementTypeId);

            if (removeItem != null)
            {
                udbc.EngagementTypeMaster.Remove(removeItem);
                udbc.SaveChanges();
            }
        }

        /// <summary>
        /// Created By : Preeti Singh
        /// Created On : 18-08-2017
        /// Reason : add Designation
        /// </summary>

        public bool AddDesignation(string DesignationName, string CorporateId, string DesignationId)
        {
            bool res = false;
            using (var context = new UserDBContext())
            {
                var designationName = new SqlParameter("@DesignationName", string.IsNullOrEmpty(DesignationName) ? DBNull.Value : (object)DesignationName);
                var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                var designationId = new SqlParameter("@DesignationId", string.IsNullOrEmpty(DesignationId) ? DBNull.Value : (object)DesignationId);
                int i = context.Database.ExecuteSqlCommand("USP_AddDesignation  @DesignationName,@CorporateId,@DesignationId", designationName, corporateid, designationId);
                if (i == 1)
                    res = true;
            }
            return res;
        }

        public bool AddStatusMaster(string StatusName, string CorporateId, Int16 StatusId)
        {
            bool res = false;
            using (var context = new UserDBContext())
            {
                var statusName = new SqlParameter("@StatusName", string.IsNullOrEmpty(StatusName) ? DBNull.Value : (object)StatusName);
                var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                var statusId = new SqlParameter("@StatusId", StatusId);
                int i = context.Database.ExecuteSqlCommand("USP_AddStatusMasters  @StatusName,@CorporateId,@StatusId", statusName, corporateid, statusId);
                if (i == 1)
                    res = true;
            }
            return res;
        }

        public List<EmployeeViewModel> GetBulkUploadEmployeeDetail(string SubscriberId, Int16 SchemeId = 0)
        {
            List<EmployeeViewModel> EmpBulkList = new List<EmployeeViewModel>();
            using (var db = new UserDBContext())
            {
                EmpBulkList = db.Database
                         .SqlQuery<EmployeeViewModel>("exec USP_GetBulkUploadEmployeeDetail @SubscriberId, @SchemeId",
                           new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId),
                            new SqlParameter("@SchemeId", SchemeId)).ToList();
            }
            return EmpBulkList;
        }

        public List<EmployeeTourView> GetTourFile()
        {
            var employeetour = new List<EmployeeTourView>();
            using (var db = new UserDBContext())
            {
                employeetour = db.Database
                          .SqlQuery<EmployeeTourView>("EXEC USP_GetTourFile").ToList();
            }

            return employeetour;
        }
        //for Resignation 
        public bool Addresignation(ResignationViewModel reg)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var resignationId = new SqlParameter("@ResignationId", reg.ResignationId);
                    var releavinngreason = new SqlParameter("@RelievingReason", string.IsNullOrEmpty(reg.RelievingReason) ? DBNull.Value : (object)reg.RelievingReason);
                    var dateofresignation = new SqlParameter("@DateofResignation", reg.DateofResignation);
                    var lastworkingday = new SqlParameter("@LastWorkingDate", reg.LastWorkingDate);
                    var status = new SqlParameter("@Status", reg.Status);
                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(reg.UserId) ? DBNull.Value : (object)reg.UserId);
                    var approvedon = new SqlParameter("@AprrovedOn", DBNull.Value);
                    if (reg.AprrovedOn != null)
                        approvedon = new SqlParameter("@AprrovedOn", reg.AprrovedOn);

                    var approvedby = new SqlParameter("@ApprovedBy", string.IsNullOrEmpty(reg.ApprovedBy) ? DBNull.Value : (object)reg.ApprovedBy);
                    var reasonid = new SqlParameter("@ReasonId", reg.ReasonId);
                    int i = context.Database.ExecuteSqlCommand("USP_AddResignation  @ResignationId,@RelievingReason,@DateofResignation,@LastWorkingDate,@Status,@UserId,@AprrovedOn,@ApprovedBy,@ReasonId", resignationId, releavinngreason, dateofresignation, lastworkingday, status, userid, approvedon, approvedby, reasonid);
                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return true;
        }
        //geting resignation
        public List<ResignationViewModel> GetResignation(string UserId, string ViewType = "USER")
        {
            var resignations = new List<ResignationViewModel>();
            using (var db = new UserDBContext())
            {
                resignations = db.Database
                          .SqlQuery<ResignationViewModel>("EXEC USPGetResignations @UserId, @ViewType", new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                            , new SqlParameter("@ViewType", string.IsNullOrEmpty(ViewType) ? DBNull.Value : (object)ViewType)).ToList();
            }

            return resignations;
        }
        //without manager level
        public List<ResignationViewModel> GetEmployeeResignation()
        {
            var resignations = new List<ResignationViewModel>();
            using (var db = new UserDBContext())
            {
                resignations = db.Database
                          .SqlQuery<ResignationViewModel>("EXEC USP_GetEmployeeResignations").ToList();
            }

            return resignations;
        }
        public EmployeeView GetNotifiedEmployee(string UserId)
        {
            EmployeeView EmployeeList = new EmployeeView();

            using (var db = new UserDBContext())
            {
                EmployeeList = db.Database
                         .SqlQuery<EmployeeView>("exec USP_GetNotifiedEmployee @UserId",
                           new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).FirstOrDefault();
            }

            return EmployeeList;
        }

        public List<EmpAttendanceAppView> GetEmployeeAttendanceApp(string UserId = null, string SubscriberId = null, Int32 Month = 0, Int32 Year = 0)
        {
            var empattendance = new List<EmpAttendanceAppView>();
            using (var db = new UserDBContext())
            {
                empattendance = db.Database
                            .SqlQuery<EmpAttendanceAppView>("exec USP_GETEmployeeAttendanceForAPP @UserId,@SubscriberId,@MonthNumber,@YearNumber"
                            , new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                            , new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)
                            , new SqlParameter("@MonthNumber", Month), new SqlParameter("@YearNumber", Year)).ToList();
            }
            return empattendance;
        }


    }
}