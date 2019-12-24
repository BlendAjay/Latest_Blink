using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AJSolutions.Models;
using AJSolutions.Areas.Admin.Models;
using AJSolutions.Areas.EMS.Models;
using AJSolutions.Areas.Candidate.Models;
using System.Data;
using System.Data.Entity;
using System.Security.Cryptography;
using DotNetIntegrationKit;
using System.Globalization;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace AJSolutions.DAL
{
    public class AdminManager
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        private const string strMerchantCode = "L3470";
        private const string strSchemeCode = "NIBF";
        private const string strKEY = "5599569174MHYEJX"; //"1671766618YOTTIM";//
        private const string strIV = "4272145358TUOJWX"; //"8977398069VCCASM";//
        UserDBContext db = new UserDBContext();
        BlobManager blobManager = new BlobManager();
        Generic generic = new Generic();

        /// <summary>
        /// Adding Roles in Master
        /// </summary>
        /// <param name="RoleId">Ex: Admin</param>
        /// <param name="Role">Ex: Admin</param>
        /// <returns>It will return true if data added in database</returns>

        public bool AddRole(string RoleId, string Role)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var roleId = new SqlParameter("@RoleId", string.IsNullOrEmpty(RoleId) ? DBNull.Value : (object)RoleId);
                    var role = new SqlParameter("@Role", string.IsNullOrEmpty(Role) ? DBNull.Value : (object)Role);

                    int i = context.Database.ExecuteSqlCommand("USP_ADDROLES  @RoleId, @Role", roleId, role);

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

        /// <summary>
        /// Delete some existing Roles if it is not used anywhere in program
        /// </summary>
        /// <param name="RoleId">Passing role id to which roles need to delete</param>
        /// <returns>It will true if successfully deleted the records</returns>
        public bool DeleteRole(string RoleId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var roleId = new SqlParameter("@RoleId", string.IsNullOrEmpty(RoleId) ? DBNull.Value : (object)RoleId);

                    int i = context.Database.ExecuteSqlCommand("USP_DELETEROLES  @RoleId", roleId);

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
        public UserRegistrationViewModel GetUserDetailsWithPhone(string UserId = null)
        {

            var details = new UserRegistrationViewModel();

            using (var db = new UserDBContext())
            {

                details = db.Database
                          .SqlQuery<UserRegistrationViewModel>("exec Usp_GetUserBasicDetails @id",
                           new SqlParameter("@id", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).FirstOrDefault();
            }

            return details;
        }

        /// <summary>
        /// Mapping modules with their relevant Roles
        /// </summary>
        /// <param name="Module">Module Id need to mapped</param>
        /// <param name="Role"> Role ID for mapping with module</param>
        /// <returns>Return true if successfully mapped module with an role</returns>
        public bool AddModuleRole(string Module, string Role)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var module = new SqlParameter("@Module", string.IsNullOrEmpty(Module) ? DBNull.Value : (object)Module);
                    var role = new SqlParameter("@Role", string.IsNullOrEmpty(Role) ? DBNull.Value : (object)Role);

                    int i = context.Database.ExecuteSqlCommand("USP_AddModuleRole  @Module, @Role", module, role);

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

        /// <summary>
        /// Remove mapping if some wrong mapping is done or mapping not required
        /// </summary>
        /// <param name="Role">RoleId need to remove</param>
        /// <param name="Module">Module Id need to remove</param>
        /// <returns>Return true if successfully removed mapping</returns>
        public bool DeleteModuleRole(string Role, string Module)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var module = new SqlParameter("@Module", string.IsNullOrEmpty(Module) ? DBNull.Value : (object)Module);
                    var role = new SqlParameter("@Role", string.IsNullOrEmpty(Role) ? DBNull.Value : (object)Role);


                    int i = context.Database.ExecuteSqlCommand("USP_DELETEMODULEROLES  @Module, @Role", module, role);

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

        /// <summary>
        /// Add Job order type
        /// </summary>
        /// <param name="JobOrderType"></param>
        /// <returns></returns>
        public bool AddJobOrderType(string JobOrderType, string CorporateId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var jobOrderType = new SqlParameter("@JobOrderType", string.IsNullOrEmpty(JobOrderType) ? DBNull.Value : (object)JobOrderType);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddJobOrderType  @JobOrderType , @CorporateId", jobOrderType, corporateId);

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
        /// <summary>
        /// List of all module role combination if module id is null else it will list only related modules role mapping
        /// </summary>
        /// <param name="ModuleId">Module Id to filter which module roles is required</param>
        /// <returns>Return Module Role lists</returns>
        public List<ModuleRolesViewModel> GetAllModulesRoles(string ModuleId = null)
        {

            var ModuleRoles = new List<ModuleRolesViewModel>();

            using (var db = new UserDBContext())
            {

                ModuleRoles = db.Database
                          .SqlQuery<ModuleRolesViewModel>("exec USP_GETModuleRoles @ModuleId",
                           new SqlParameter("@ModuleId", string.IsNullOrEmpty(ModuleId) ? DBNull.Value : (object)ModuleId)).ToList();
            }

            return ModuleRoles;
        }

        public void AddUserProfileDetails(string UserId, string UserName, string UserAccessId)
        {
            UserProfile profile = new UserProfile();
            profile.UserId = UserId;
            profile.Name = UserName;
            // profile.AccessId = UserAccessId;

            db.UserProfile.Add(profile);
            db.SaveChanges();
        }

        public List<UserPrimaryDetailViewModel> GetUserDetails(string UserId = null)
        {

            var profile = new List<UserPrimaryDetailViewModel>();

            using (var db = new UserDBContext())
            {

                profile = db.Database
                          .SqlQuery<UserPrimaryDetailViewModel>("exec USP_UserLoginDetails @USERID",
                           new SqlParameter("@USERID", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return profile;
        }

        public bool GetUserExists(string Email = null, string Phone = null, string EmployeeCode = null)
        {

            int profile = 0;

            using (var db = new UserDBContext())
            {

                profile = db.Database
                          .SqlQuery<int>("exec USP_GetUserExist @Email,@Phone,@EmployeeCode",
                                                 new SqlParameter("@Email", string.IsNullOrEmpty(Email) ? DBNull.Value : (object)Email)
                                               , new SqlParameter("@Phone", string.IsNullOrEmpty(Phone) ? DBNull.Value : (object)Phone)
                                               , new SqlParameter("@EmployeeCode", string.IsNullOrEmpty(EmployeeCode) ? DBNull.Value : (object)EmployeeCode)
                                           ).FirstOrDefault();
            }

            if (profile > 0)
                return true;

            return false;
        }

        //By : Ajay Kumar Choudhary
        //On : 25-07-2017
        //Reason : To Get wheater employee exist or not
        public bool GetEmployeeExists(string Email = null, string Phone = null)
        {
            int profile = 0;
            using (var db = new UserDBContext())
            {

                profile = db.Database
                          .SqlQuery<int>("exec USP_GetEmployeeExist @Email,@Phone",
                                                 new SqlParameter("@Email", string.IsNullOrEmpty(Email) ? DBNull.Value : (object)Email)
                                               , new SqlParameter("@Phone", string.IsNullOrEmpty(Phone) ? DBNull.Value : (object)Phone)
                                           ).FirstOrDefault();
            }
            if (profile > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Update by : Achal Kumar Jha
        /// Updated on : 02-06-2017
        /// Reson : For Payroll
        /// </summary>       
        /// <param name="Status"></param>
        /// <param name="DateofJoining"></param>
        /// <param name="ProbationPeriod"></param>
        /// <param name="FatherName"></param>
        /// <param name="SpouseName"></param>
        /// <param name="Grade"></param>
        /// <param name="DateofConfirmation"></param>
        /// <param name="Designation"></param>
        /// <returns></returns>

        public bool UserRegistration(string UserId, string Name, DateTime RegisteredOn, string ModuleId, string DepartmentId, string RoleId, string SubscriberId,
                                     bool ManagerLevel, string ReportingAuthority, DateTime? UpdatedOn, string UpdatedBy, string RegistrationId, string Pcode = "",
                                    string Branch = "", string BranchCategory = "", string Region = "", string BranchCode = "",
                                     string BranchState = "", string ClientId = "", string Gender = "", string PassCode = "", string Source = ""
                                     , string Designation = "", string ReferenceId = "", string TrackerId = "", string Facilityid = "", string Accesspoint = "")
        {
            //try
            //{
            using (var context = new UserDBContext())
            {

                var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
                var registeredOn = new SqlParameter("@RegisteredOn", RegisteredOn);
                var moduleId = new SqlParameter("@ModuleId", string.IsNullOrEmpty(ModuleId) ? DBNull.Value : (object)ModuleId);
                var departmentId = new SqlParameter("@DepartmentId", string.IsNullOrEmpty(DepartmentId) ? DBNull.Value : (object)DepartmentId);
                var roleId = new SqlParameter("@RoleId", string.IsNullOrEmpty(RoleId) ? DBNull.Value : (object)RoleId);
                var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                var managerLevel = new SqlParameter("@ManagerLevel", ManagerLevel);
                var reportingAuthority = new SqlParameter("@ReportingAuthority", string.IsNullOrEmpty(ReportingAuthority) ? DBNull.Value : (object)ReportingAuthority);
                var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                var registrationid = new SqlParameter("@RegistrationId", string.IsNullOrEmpty(RegistrationId) ? DBNull.Value : (object)RegistrationId);
                var pcode = new SqlParameter("@Pcode", string.IsNullOrEmpty(Pcode) ? DBNull.Value : (object)Pcode);
                var designation = new SqlParameter("@Designation", string.IsNullOrEmpty(Designation) ? DBNull.Value : (object)Designation);
                var branch = new SqlParameter("@Branch", string.IsNullOrEmpty(Branch) ? DBNull.Value : (object)Branch);
                var branchCategory = new SqlParameter("@BranchCategory", string.IsNullOrEmpty(BranchCategory) ? DBNull.Value : (object)BranchCategory);
                var region = new SqlParameter("@Region", string.IsNullOrEmpty(Region) ? DBNull.Value : (object)Region);
                var branchCode = new SqlParameter("@BranchCode", string.IsNullOrEmpty(BranchCode) ? DBNull.Value : (object)BranchCode);
                var branchState = new SqlParameter("@BranchState", string.IsNullOrEmpty(BranchState) ? DBNull.Value : (object)BranchState);
                var clientId = new SqlParameter("@ClientId", string.IsNullOrEmpty(ClientId) ? DBNull.Value : (object)ClientId);
                var gender = new SqlParameter("@Gender", string.IsNullOrEmpty(Gender) ? DBNull.Value : (object)Gender);
                var password = new SqlParameter("@Password", string.IsNullOrEmpty(PassCode) ? DBNull.Value : (object)PassCode);
                var source = new SqlParameter("@Source", string.IsNullOrEmpty(Source) ? DBNull.Value : (object)Source);
                var referenceId = new SqlParameter("@ReferenceId", string.IsNullOrEmpty(ReferenceId) ? DBNull.Value : (object)ReferenceId);
                var trackerId = new SqlParameter("@TrackerId", string.IsNullOrEmpty(TrackerId) ? DBNull.Value : (object)TrackerId);
                var facilityId = new SqlParameter("@Facilityid", string.IsNullOrEmpty(Facilityid) ? DBNull.Value : (object)Facilityid);
                var accesspoint = new SqlParameter("@Accesspoint", string.IsNullOrEmpty(Accesspoint) ? DBNull.Value : (object)Accesspoint);
                //var status = new SqlParameter("@status", string.IsNullOrEmpty(Status) ? DBNull.Value : (object)Status);
                //var dateofJoining = new SqlParameter("@DateofJoining", string.IsNullOrEmpty(DateofJoining.ToString()) ? DBNull.Value : (object)DateofJoining);
                //var probationPeriod = new SqlParameter("@ProbationPeriod", string.IsNullOrEmpty(ProbationPeriod) ? DBNull.Value : (object)ProbationPeriod);
                //var grade = new SqlParameter("@GradeId", string.IsNullOrEmpty(Grade) ? DBNull.Value : (object)Grade);
                //var dateofConfirmation = new SqlParameter("@DateofConfirmation", string.IsNullOrEmpty(DateofConfirmation.ToString()) ? DBNull.Value : (object)DateofConfirmation);
                int i = context.Database.ExecuteSqlCommand("USP_UserRegistration  @UserId, @Name , @RegisteredOn, @ModuleId, @DepartmentId, " +
                                                           "@RoleId, @SubscriberId, @ManagerLevel, @ReportingAuthority, @UpdatedOn, @UpdatedBy, " +
                                                           "@RegistrationId, @Pcode, @Designation,@Branch,@BranchCategory,@Region,@BranchCode,@BranchState," +
                                                           "@ClientId,@Gender,@Password, @Source,@ReferenceId,@TrackerId,@Facilityid,@Accesspoint",
                                                           userId, name, registeredOn, moduleId, departmentId, roleId, subscriberId,
                                                           managerLevel, reportingAuthority, updatedOn, updatedBy, registrationid, pcode, designation,
                                                           branch, branchCategory, region, branchCode, branchState, clientId, gender, password,
                                                           source, referenceId, trackerId, facilityId, accesspoint);

                if (i > 0)
                    return true;
                else
                    return false;
            }
            //}
            //catch (RetryLimitExceededException /* dex */)
            //{
            //    //Log the error (uncomment dex variable name and add a line here to write a log.
            //    //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            //}
            //  return true;
        }

        /// <summary>
        /// Update by : Achal Kumar Jha
        /// Updated on : 02-06-2017
        /// Reson : For Payroll
        /// </summary>       
        /// <param name="Status"></param>
        /// <param name="DateofJoining"></param>
        /// <param name="ProbationPeriod"></param>
        /// <param name="FatherName"></param>
        /// <param name="SpouseName"></param>
        /// <param name="Grade"></param>
        /// <param name="DateofConfirmation"></param>
        /// <param name="Designation"></param>
        /// <returns></returns>
        /// 
        public bool UpdateUserRegistration(string UserId, string Name, string DepartmentId, bool ManagerLevel, string ReportingAuthority,
            DateTime? UpdatedOn, string UpdatedBy, string Status = "", DateTime? DateofJoining = null, string ProbationPeriod = "",
             string Grade = "", DateTime? DateofConfirmation = null, string Designation = "")
        {
            try
            {
                using (var context = new UserDBContext())
                {

                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
                    var departmentId = new SqlParameter("@DepartmentId", string.IsNullOrEmpty(DepartmentId) ? DBNull.Value : (object)DepartmentId);
                    var managerLevel = new SqlParameter("@ManagerLevel", ManagerLevel);
                    var reportingAuthority = new SqlParameter("@ReportingAuthority", string.IsNullOrEmpty(ReportingAuthority) ? DBNull.Value : (object)ReportingAuthority);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);

                    var status = new SqlParameter("@status", string.IsNullOrEmpty(Status) ? DBNull.Value : (object)Status);
                    var dateofJoining = new SqlParameter("@DateofJoining", string.IsNullOrEmpty(DateofJoining.ToString()) ? DBNull.Value : (object)DateofJoining);
                    var probationPeriod = new SqlParameter("@ProbationPeriod", string.IsNullOrEmpty(ProbationPeriod) ? DBNull.Value : (object)ProbationPeriod);
                    var grade = new SqlParameter("@GradeId", string.IsNullOrEmpty(Grade) ? DBNull.Value : (object)Grade);
                    var dateofConfirmation = new SqlParameter("@DateofConfirmation", string.IsNullOrEmpty(DateofConfirmation.ToString()) ? DBNull.Value : (object)DateofConfirmation);
                    var designation = new SqlParameter("@Designation", string.IsNullOrEmpty(Designation) ? DBNull.Value : (object)Designation);
                    int i = context.Database.ExecuteSqlCommand("USP_UpdateEmployee  @UserId, @Name, @DepartmentId, @ManagerLevel, @ReportingAuthority, @UpdatedOn, @UpdatedBy,@status,@DateofJoining,@ProbationPeriod,@DateofConfirmation,@GradeId,@Designation", userId, name, departmentId, managerLevel, reportingAuthority, updatedOn, updatedBy, status, dateofJoining, probationPeriod, dateofConfirmation, grade, designation);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return true;
        }

        public bool UpdateThirdParty(string CorporateId, string Name, string DepartmentId, DateTime? UpdatedOn, string UpdatedBy)
        {
            try
            {
                using (var context = new UserDBContext())
                {

                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
                    var departmentId = new SqlParameter("@DepartmentId", string.IsNullOrEmpty(DepartmentId) ? DBNull.Value : (object)DepartmentId);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    int i = context.Database.ExecuteSqlCommand("USP_UpdateThirdParty  @CorporateId, @Name, @DepartmentId, @UpdatedOn, @UpdatedBy", corporateId, name, departmentId, updatedOn, updatedBy);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return true;
        }

        public bool UpdateCandidate(string UserId, string Name, string RegistrationId, DateTime? UpdatedOn, string UpdatedBy, string Designation,
                                        string Branch, string BranchCategory, string Region, string BranchCode, string BranchState, string ClientId, string Gender)
        {
            try
            {
                using (var context = new UserDBContext())
                {

                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
                    var registrationId = new SqlParameter("@RegistrationId", string.IsNullOrEmpty(RegistrationId) ? DBNull.Value : (object)RegistrationId);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var designation = new SqlParameter("@Designation", string.IsNullOrEmpty(Designation) ? DBNull.Value : (object)Designation);
                    var branch = new SqlParameter("@Branch", string.IsNullOrEmpty(Branch) ? DBNull.Value : (object)Branch);
                    var branchCategory = new SqlParameter("@BranchCategory", string.IsNullOrEmpty(BranchCategory) ? DBNull.Value : (object)BranchCategory);
                    var region = new SqlParameter("@Region", string.IsNullOrEmpty(Region) ? DBNull.Value : (object)Region);
                    var branchCode = new SqlParameter("@BranchCode", string.IsNullOrEmpty(BranchCode) ? DBNull.Value : (object)BranchCode);
                    var branchState = new SqlParameter("@BranchState", string.IsNullOrEmpty(BranchState) ? DBNull.Value : (object)BranchState);
                    var clientId = new SqlParameter("@ClientId", string.IsNullOrEmpty(ClientId) ? DBNull.Value : (object)ClientId);
                    var gender = new SqlParameter("@Gender", string.IsNullOrEmpty(Gender) ? DBNull.Value : (object)Gender);
                    int i = context.Database.ExecuteSqlCommand("USP_UpdateCandidate  @UserId, @Name, @RegistrationId, @UpdatedOn, @UpdatedBy, @Designation,@Branch,@BranchCategory,@Region,@BranchCode,@BranchState,@ClientId,@Gender", userId, name, registrationId, updatedOn, updatedBy, designation, branch, branchCategory, region, branchCode, branchState, clientId, gender);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return true;
        }

        public UserRegistrationViewModel GetUserRegistrationDetails(string UserId = null)
        {

            var details = new UserRegistrationViewModel();

            using (var db = new UserDBContext())
            {

                details = db.Database
                          .SqlQuery<UserRegistrationViewModel>("exec USP_GetUserDetails @UserId",
                           new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).FirstOrDefault();
            }

            return details;
        }

        public UserRegistrationViewModel GetTPDetails(string CorporateId = null)
        {

            var details = new UserRegistrationViewModel();

            using (var db = new UserDBContext())
            {

                details = db.Database
                          .SqlQuery<UserRegistrationViewModel>("exec USP_GetThirdPartyDetails @CorporateId",
                           new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).FirstOrDefault();
            }

            return details;
        }

        public UserRegistrationViewModel GetCandidateDetails(string UserId = null)
        {

            var details = new UserRegistrationViewModel();

            using (var db = new UserDBContext())
            {

                details = db.Database
                          .SqlQuery<UserRegistrationViewModel>("exec USP_GetCandidateDetails @UserId",
                           new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).FirstOrDefault();
            }

            return details;
        }

        public UserRegistrationViewModel GetClientDetails(string CorporateId = null)
        {

            var details = new UserRegistrationViewModel();

            using (var db = new UserDBContext())
            {

                details = db.Database
                          .SqlQuery<UserRegistrationViewModel>("exec USP_GetClientDetails @CorporateId",
                           new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).FirstOrDefault();
            }

            return details;
        }

        public bool AddCandidateCourse(string UserId, Int64 BatchId, int InstallmentId)
        {
            try
            {
                using (var context = new UserDBContext())
                {

                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var batchId = new SqlParameter("@BatchId", BatchId);
                    var installmentId = new SqlParameter("@InstallmentId", InstallmentId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateCourseDetails  @UserId , @BatchId  , @InstallmentId", userId, batchId, installmentId);

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

        public UserModuleAccess GetUserModuleAccess(string UserId, DateTime LoginDate, string Role)
        {

            var profile = new UserModuleAccess();

            using (var db = new UserDBContext())
            {

                profile = db.Database
                          .SqlQuery<UserModuleAccess>("exec USP_GETUSERMODULEACCESS @LoginId, @LoginDate, @Role",
                           new SqlParameter("@LoginId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                           , new SqlParameter("@LoginDate", LoginDate)
                           , new SqlParameter("@Role", string.IsNullOrEmpty(Role) ? DBNull.Value : (object)Role)).FirstOrDefault();
            }

            return profile;
        }

        public bool AddEducationLevel(short EducationLevelId, string EducationLevelName)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var educationlevelid = new SqlParameter("@EducationLevelId", EducationLevelId);
                    var educationlevelname = new SqlParameter("@EducationLevelName", string.IsNullOrEmpty(EducationLevelName) ? DBNull.Value : (object)EducationLevelName);

                    int i = context.Database.ExecuteSqlCommand("USP_AddEducationLevel @EducationLevelId, @EducationLevelName", educationlevelid, educationlevelname);

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

        public List<EducationLevelMaster> GetEducationLevel()
        {

            var educationlevel = new List<EducationLevelMaster>();

            using (var db = new UserDBContext())
            {
                educationlevel = db.Database
                          .SqlQuery<EducationLevelMaster>("EXEC USP_GetEducationLevel").ToList();
            }

            return educationlevel;
        }

        public string GenerateCourseCode()
        {

            UserDBContext udbc = new UserDBContext();
            string month = Convert.ToString(DateTime.UtcNow.Month);
            if (month.Length == 1)
                month = "0" + month;
            string day = Convert.ToString(DateTime.UtcNow.Day);
            if (day.Length == 1)
                day = "0" + day;

            string currentDate = Convert.ToString(DateTime.UtcNow.Year) + month + day;


            string CourseId = "CC" + currentDate + "0001";

            var CourseCodes = from s in udbc.CourseMaster.Where(s => s.CourseCode.Substring(0, 10) == "CC" + currentDate)
                              orderby s.CourseCode descending
                              select s.CourseCode;

            var CourseCode = CourseCodes.FirstOrDefault();

            if (CourseCode != null)
            {
                string CourseCodePartialId = CourseCode.Substring(10);
                int lastVal = Convert.ToInt32(CourseCodePartialId);
                lastVal = lastVal + 1;
                string suffix = string.Empty;

                for (int i = Convert.ToString(lastVal).Length; i < 4; i++)
                {
                    suffix = suffix + "0";
                }

                CourseId = CourseCode.Substring(0, 10) + suffix + Convert.ToString(lastVal);
            }
            return CourseId;

        }

        public bool AddCourseMasters(CourseMaster courseMaster)
        {
            bool res = false;

            try
            {
                //if (string.IsNullOrEmpty(courseMaster.CourseCode) || courseMaster.CourseCode == "0")
                //    courseMaster.CourseCode = GenerateCourseCode();

                using (var context = new UserDBContext())
                {
                    var courseCode = new SqlParameter("@CourseCode", string.IsNullOrEmpty(courseMaster.CourseCode) ? DBNull.Value : (object)courseMaster.CourseCode);
                    var courseName = new SqlParameter("@CourseName", string.IsNullOrEmpty(courseMaster.CourseName) ? DBNull.Value : (object)courseMaster.CourseName);
                    var courseDuration = new SqlParameter("@CourseDuration", courseMaster.CourseDuration);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(courseMaster.SubscriberId) ? DBNull.Value : (object)courseMaster.SubscriberId);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(courseMaster.CorporateId) ? DBNull.Value : (object)courseMaster.CorporateId);
                    var courseFee = new SqlParameter("@CourseFee", courseMaster.CourseFee);
                    var courseLateFee = new SqlParameter("@CourseLateFee", courseMaster.CourseLateFee);
                    var currency = new SqlParameter("@Currency", string.IsNullOrEmpty(courseMaster.Currency) ? DBNull.Value : (object)courseMaster.Currency);
                    var discussionFrom = new SqlParameter("@DiscussionForum", courseMaster.DiscussionForum);
                    var contentvisibility = new SqlParameter("@ContentVisiblity", courseMaster.ContentVisiblity);
                    var clike = new SqlParameter("@CountLikes", courseMaster.CountLikes);
                    var coursedescription = new SqlParameter("@CourseDescription", string.IsNullOrEmpty(courseMaster.CourseDescription) ? DBNull.Value : (object)courseMaster.CourseDescription);
                    var categoryId = new SqlParameter("@CategoryId", courseMaster.CategoryId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCourseMasters  @CourseCode ,@CourseName ,@CourseDuration ,@SubscriberId ,@CorporateId, @CourseFee, @CourseLateFee, @Currency, @DiscussionForum, @ContentVisiblity, @CountLikes, @CourseDescription, @CategoryId",
                                                                courseCode, courseName, courseDuration, subscriberId, corporateId, courseFee, courseLateFee, currency, discussionFrom, contentvisibility, clike, coursedescription, categoryId);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                /*Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator."); */
            }

            return res;
        }


        public bool AddCourseAddtionalFees(AdditionalCourseFee AdditionalCourseFee)
        {
            bool res = false;

            try
            {
                //if (string.IsNullOrEmpty(courseMaster.CourseCode) || courseMaster.CourseCode == "0")
                //    courseMaster.CourseCode = GenerateCourseCode();

                using (var context = new UserDBContext())
                {
                    var courseCode = new SqlParameter("@CourseCode", string.IsNullOrEmpty(AdditionalCourseFee.CourseCode) ? DBNull.Value : (object)AdditionalCourseFee.CourseCode);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(AdditionalCourseFee.CorporateId) ? DBNull.Value : (object)AdditionalCourseFee.CorporateId);
                    var accommodation = new SqlParameter("@Accommodation", AdditionalCourseFee.Accommodation);
                    var discount = new SqlParameter("@Discount", AdditionalCourseFee.Discount);
                    var others = new SqlParameter("@Others", AdditionalCourseFee.Others);
                    var installmentInterest = new SqlParameter("@InstallmentInterest", AdditionalCourseFee.InstallmentInterest);
                    var transport = new SqlParameter("@Transport", AdditionalCourseFee.Transport);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCourseAddtionalFees  @CourseCode ,@CorporateId ,@Accommodation ,@Discount ,@Others, @InstallmentInterest, @Transport",
                                                                courseCode, corporateId, accommodation, discount, others, installmentInterest, transport);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                /*Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator."); */
            }

            return res;
        }

        public bool AddCourseAttachmentToDB(string CourseCode, string FileName, string ContentType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var courseCode = new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCourseAttachment @CourseCode, @FileName, @ContentType",
                        courseCode, fileName, contentType);

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

        public List<CourseAttachment> GetCourseAttachments(string CourseCode)
        {
            var courseAttachment = new List<CourseAttachment>();
            using (var db = new UserDBContext())
            {
                courseAttachment = db.Database.SqlQuery<CourseAttachment>("EXEC USP_AddCourseAttachment @CourseCode",
                    new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode)).ToList();
            }
            return courseAttachment;
        }

        public string uploadCourseFile(string CourseCode, HttpPostedFileBase upload)
        {
            string res = "Failure";
            if (upload != null && upload.ContentLength > 0)
            {

                var file = db.CourseAttachment.Where(d => d.CourseCode == CourseCode).FirstOrDefault();

                if (file != null)
                {
                    Int64 FileId = file.FileId;

                    blobManager.DeleteBlob(CourseCode.ToLower(), GetFileName(FileId).ToLower());
                    db.CourseAttachment.Remove(file);
                    db.SaveChanges();
                }


                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");


                bool added = AddCourseAttachmentToDB(CourseCode, imgFileName.ToLower(), upload.ContentType);

                if (added)
                {
                    blobManager.UploadBlob(CourseCode.ToLower(), ReplaceFileName(CourseCode).ToLower(), upload);

                }

                res = "Succeed";
            }


            return res;
        }

        public CourseMasterView GetCourseMasterDetails(string CourseCode)
        {
            var courseMaster = new CourseMasterView();
            using (var db = new UserDBContext())
            {
                courseMaster = db.Database
                          .SqlQuery<CourseMasterView>("EXEC USP_GetCourseMasterDetail @CourseCode",
                          new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode)).FirstOrDefault();
            }

            return courseMaster;
        }

        public List<CourseMasterView> GetCourseMasters(string SubscriberId, string CorporateId = "")
        {
            var courseMaster = new List<CourseMasterView>();
            using (var db = new UserDBContext())
            {
                courseMaster = db.Database
                          .SqlQuery<CourseMasterView>("EXEC USP_GetCourseMasters @SubscriberId, @CorporateId",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId),
                          new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
            }

            return courseMaster;
        }

        //public List<CourseMaster> GetCourseMasters(string SubscriberId)
        //{

        //    var courseMaster = new List<CourseMaster>();

        //    using (var db = new UserDBContext())
        //    {
        //        courseMaster = db.Database
        //                  .SqlQuery<CourseMaster>("EXEC USP_GetCourseMasters @SubscriberId", new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
        //    }

        //    return courseMaster;
        //}

        public string GetTaskId()
        {
            UserDBContext udbc = new UserDBContext();
            string year = Convert.ToString(DateTime.UtcNow.Year).Substring(2);
            string quarter = "T1";
            int month = DateTime.UtcNow.Month;
            if (month > 3 && month <= 6)
            {
                quarter = "T2";
            }
            else if (month > 6 && month <= 9)
            {
                quarter = "T3";
            }
            else if (month > 9 && month <= 12)
            {
                quarter = "T4";
            }

            string TaskId = "TA" + year + quarter + "000001";

            var Tasks = from s in udbc.TaskMaster.Where(s => s.TaskId.Substring(0, 6) == "TA" + year + quarter)
                        orderby s.TaskId descending
                        select s.TaskId;

            var Task = Tasks.FirstOrDefault();

            if (Task != null)
            {
                string TaskPartialId = Task.Substring(7);
                int lastVal = Convert.ToInt32(TaskPartialId);
                lastVal = lastVal + 1;
                string suffix = string.Empty;

                for (int i = Convert.ToString(lastVal).Length; i < 6; i++)
                {
                    suffix = suffix + "0";
                }

                TaskId = Task.Substring(0, 6) + suffix + Convert.ToString(lastVal);
            }
            return TaskId;
        }

        public bool AddTaskMasters(TaskMaster task, string[] ItemId, string[] ItemType, string[] ItemDescription, string[] Unit, string[] UnitPrice, string[] ItemDuration, string[] Actions, string DepartmentId)
        {
            bool res = false;

            try
            {
                DataTable TaskItem = new DataTable();
                TaskItem.Columns.Add("TaskId");
                TaskItem.Columns.Add("ItemId");
                TaskItem.Columns.Add("ItemTypeId");
                TaskItem.Columns.Add("ItemDescription");
                TaskItem.Columns.Add("Unit");
                TaskItem.Columns.Add("UnitPrice");
                TaskItem.Columns.Add("Duration");
                TaskItem.Columns.Add("Actions");


                string Tasknum = "";
                string itemDuration = "";
                int itemId = 0;


                if (task.TaskId == null)
                {
                    Tasknum = GetTaskId();
                }
                else
                {
                    Tasknum = task.TaskId;
                }
                if (ItemType != null)
                {
                    for (int i = 0; i < ItemType.Length; i++)
                    {
                        if (ItemType[i] != "0")
                        {
                            if (ItemType[i].ToString() != "- - - Select Item Type - - -")
                            {
                                if (ItemDuration[i].ToString() == "NA")
                                {
                                    itemDuration = "0";
                                }
                                else
                                {
                                    itemDuration = ItemDuration[i].ToString();
                                }

                                if (ItemId[i].ToString() == "0")
                                {
                                    itemId = 0;
                                }
                                else
                                {
                                    itemId = Convert.ToInt32(ItemId[i]);
                                }

                                TaskItem.Rows.Add(Tasknum, itemId, ItemType[i], ItemDescription[i],
                                                          Unit[i], UnitPrice[i], itemDuration, Actions[i]);


                            }
                        }
                    }
                }


                if (TaskItem.Rows.Count > 0 && DepartmentId != "ADI")
                {
                    task.TaskStatus = 0;
                }
                else
                {
                    task.TaskStatus = 1;
                }


                using (var context = new UserDBContext())
                {
                    var taskId = new SqlParameter("@TaskId", string.IsNullOrEmpty(Tasknum) ? DBNull.Value : (object)Tasknum);
                    var subject = new SqlParameter("@Subject", string.IsNullOrEmpty(task.Subject) ? DBNull.Value : (object)task.Subject);
                    var description = new SqlParameter("@Description", string.IsNullOrEmpty(task.Description) ? DBNull.Value : (object)task.Description);
                    var venue = new SqlParameter("@Venue", string.IsNullOrEmpty(task.Venue) ? DBNull.Value : (object)task.Venue);
                    var startDate = new SqlParameter("@StartDate", task.StartDate == null ? DBNull.Value : (object)task.StartDate);
                    var duration = new SqlParameter("@Duration", task.Duration);
                    var jobOrderNumber = new SqlParameter("@JobOrderNumber", string.IsNullOrEmpty(task.JobOrderNumber) ? DBNull.Value : (object)task.JobOrderNumber);
                    var city = new SqlParameter("@City", string.IsNullOrEmpty(task.City) ? DBNull.Value : (object)task.City);
                    var state = new SqlParameter("@State", string.IsNullOrEmpty(task.State) ? DBNull.Value : (object)task.State);
                    var country = new SqlParameter("@Country", string.IsNullOrEmpty(task.Country) ? DBNull.Value : (object)task.Country);
                    var taskStatus = new SqlParameter("@TaskStatus", task.TaskStatus);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(task.SubscriberId) ? DBNull.Value : (object)task.SubscriberId);
                    var createdBy = new SqlParameter("@CreatedBy", string.IsNullOrEmpty(task.CreatedBy) ? DBNull.Value : (object)task.CreatedBy);
                    var taskTypeId = new SqlParameter("@TaskTypeId", task.TaskTypeId);
                    var invoiceFrequency = new SqlParameter("@InvoiceFrequency", string.IsNullOrEmpty(task.InvoiceFrequency) ? DBNull.Value : (object)task.InvoiceFrequency);
                    var assignedTo = new SqlParameter("@AssignedTo", string.IsNullOrEmpty(task.AssignedTo) ? DBNull.Value : (object)task.AssignedTo);
                    var taskAmount = new SqlParameter("@TaskAmount", task.TaskAmount);
                    var currency = new SqlParameter("@Currency", string.IsNullOrEmpty(task.Currency) ? DBNull.Value : (object)task.Currency);
                    var updatedon = new SqlParameter("@UpdatedOn", DateTime.UtcNow);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(task.UpdatedBy) ? DBNull.Value : (object)task.UpdatedBy);
                    var createdOn = new SqlParameter("@CreatedOn", DateTime.UtcNow);
                    var taskItemsTb = new SqlParameter("@TaskItemsTb", SqlDbType.Structured);
                    taskItemsTb.TypeName = "dbo.tvpTASKITEM";
                    taskItemsTb.Value = TaskItem;

                    int i = context.Database.ExecuteSqlCommand("USP_AddTask @TaskId, @Subject, @Description, @Venue, @StartDate, @Duration, " +
                                                               " @JobOrderNumber, @City, @State, @Country, @TaskStatus, @SubscriberId, @CreatedBy, " +
                                                               "@TaskTypeId, @InvoiceFrequency, @AssignedTo,  @TaskAmount, @Currency, @UpdatedOn , @UpdatedBy, @CreatedOn, @TaskItemsTb",
                                                               taskId, subject, description, venue, startDate, duration, jobOrderNumber, city,
                                                               state, country, taskStatus, subscriberId, createdBy, taskTypeId, invoiceFrequency, assignedTo, taskAmount, currency, updatedon, updatedBy, createdOn, taskItemsTb);

                    if (i > 0)
                    {
                        res = true;
                        //if (task.JobOrderNumber != null)
                        //{
                        //    CMSManager cmsMgr = new CMSManager();
                        //    cmsMgr.UpdateJobStatus(task.SubscriberId, task.JobOrderNumber, "Inprogress", DateTime.Now, task.UpdatedBy, DateTime.Now);
                        //}

                    }

                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
            }

            return res;
        }

        public void CreateTaskItem(string TaskId, short ItemId, Int64 ItemTypeId, string Description, int Unit, float UnitPrice, int Duration)
        {
            TaskItems task = new TaskItems();

            task.TaskId = TaskId;
            task.ItemId = ItemId;
            task.ItemTypeId = ItemTypeId;
            task.ItemDescription = Description;
            task.Unit = Unit;
            task.UnitPrice = UnitPrice;
            task.Duration = Duration;
            db.TaskItems.Add(task);
            db.SaveChanges();
        }

        public List<TaskMasterView> GetTaskMasters(string SubscriberId, string TaskId = null)
        {

            var taskMasters = new List<TaskMasterView>();

            using (var db = new UserDBContext())
            {
                taskMasters = db.Database
                          .SqlQuery<TaskMasterView>("EXEC USP_GetTask  @SubscriberId, @TaskId",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId),
                          new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId)
                          ).ToList();
            }

            return taskMasters;
        }

        public List<TaskMasterView> GetTaskFORAssignedTo(string SubscriberId)
        {

            var taskMasters = new List<TaskMasterView>();

            using (var db = new UserDBContext())
            {
                taskMasters = db.Database
                          .SqlQuery<TaskMasterView>("EXEC USP_GetTaskFORAssignedTo @AssignedTo",
                          new SqlParameter("@AssignedTo", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)
                          ).ToList();
            }

            return taskMasters;
        }

        public bool UpdateTaskStatus(string TaskId, short TaskStatus, string AssignedTo)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var taskId = new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId);
                    var taskStatus = new SqlParameter("@TaskStatus", TaskStatus);
                    var assignedTo = new SqlParameter("@AssignedTo", string.IsNullOrEmpty(AssignedTo) ? DBNull.Value : (object)AssignedTo);

                    int i = context.Database.ExecuteSqlCommand("USP_UpdateTaskStatus @TaskId, @TaskStatus,  @AssignedTo",
                                                               taskId, taskStatus, assignedTo);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
            }

            return res;
        }

        public TaskMaster GetTask(string TaskId, string AssignedTo)
        {

            var task = new TaskMaster();

            using (var db = new UserDBContext())
            {
                task = db.Database
                          .SqlQuery<TaskMaster>("EXEC USP_GetTaskAssignedByTaskId @TaskId, @AssignedTo",
                           new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId),
                          new SqlParameter("@AssignedTo", string.IsNullOrEmpty(AssignedTo) ? DBNull.Value : (object)AssignedTo)).FirstOrDefault();
            }

            return task;
        }

        public List<TaskItemsView> GetTaskItems(string TaskId)
        {


            List<TaskItemsView> taskItems = (from t in db.TaskItems
                                             join i in db.ItemTypeMasters
                                                  on t.ItemTypeId equals i.ItemTypeId
                                             where t.TaskId == TaskId
                                             select new TaskItemsView
                                             {
                                                 TaskId = t.TaskId
                                                 ,
                                                 ItemId = t.ItemId
                                                 ,
                                                 ItemDescription = t.ItemDescription
                                                 ,
                                                 ItemTypeId = t.ItemTypeId
                                                 ,
                                                 Unit = t.Unit
                                                 ,
                                                 Duration = t.Duration
                                                 ,
                                                 UnitPrice = t.UnitPrice
                                                 ,
                                                 ItemTypeName = i.ItemTypeName
                                             }
                          ).ToList();

            //  List<TaskItems> taskItemList = db.TaskItems.Where(t => t.TaskId == TaskId).Select(t => t.ItemTypeMaster).ToList();
            return taskItems;

        }

        public TaskMasterView GetTaskDetails(string AssignedTo, string TaskId)
        {

            var task = new TaskMasterView();

            using (var db = new UserDBContext())
            {
                task = db.Database
                          .SqlQuery<TaskMasterView>("EXEC USP_GetTaskAssignedByTaskId @TaskId, @AssignedTo",
                           new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId),
                          new SqlParameter("@AssignedTo", string.IsNullOrEmpty(AssignedTo) ? DBNull.Value : (object)AssignedTo)).FirstOrDefault();
            }

            return task;
        }

        public string GetTrainingId()
        {
            UserDBContext udbc = new UserDBContext();
            string year = Convert.ToString(DateTime.UtcNow.Year).Substring(2);
            string quarter = "SCH1";
            int month = DateTime.UtcNow.Month;
            if (month > 3 && month <= 6)
            {
                quarter = "SCH2";
            }
            else if (month > 6 && month <= 9)
            {
                quarter = "SCH3";
            }
            else if (month > 9 && month <= 12)
            {
                quarter = "SCH4";
            }

            string TrainingId = "TR" + year + quarter + "000001";

            var Trainings = from s in udbc.TrainingSchedule.Where(s => s.TrainingId.Substring(0, 8) == "TR" + year + quarter)
                            orderby s.TrainingId descending
                            select s.TrainingId;

            var Training = Trainings.FirstOrDefault();

            if (Training != null)
            {
                string TrainingPartialId = Training.Substring(8);
                int lastVal = Convert.ToInt32(TrainingPartialId);
                lastVal = lastVal + 1;
                string suffix = string.Empty;

                for (int i = Convert.ToString(lastVal).Length; i < 6; i++)
                {
                    suffix = suffix + "0";
                }

                TrainingId = Training.Substring(0, 8) + suffix + Convert.ToString(lastVal);
            }
            return TrainingId;
        }

        public string GetInvoiceNumber()
        {
            UserDBContext udbc = new UserDBContext();
            string year = Convert.ToString(DateTime.UtcNow.Year).Substring(2);
            string quarter = "I1";
            int month = DateTime.UtcNow.Month;
            if (month > 3 && month <= 6)
            {
                quarter = "I2";
            }
            else if (month > 6 && month <= 9)
            {
                quarter = "I3";
            }
            else if (month > 9 && month <= 12)
            {
                quarter = "I4";
            }

            string InvoiceNumber = "IN" + year + quarter + "000001";

            var Invoices = from s in udbc.GenerateInvoice.Where(s => s.InvoiceNumber.Substring(0, 6) == "IN" + year + quarter)
                           orderby s.InvoiceNumber descending
                           select s.InvoiceNumber;

            var Invoice = Invoices.FirstOrDefault();

            if (Invoice != null)
            {
                string InvoicePartialId = Invoice.Substring(7);
                int lastVal = Convert.ToInt32(InvoicePartialId);
                lastVal = lastVal + 1;
                string suffix = string.Empty;

                for (int i = Convert.ToString(lastVal).Length; i < 6; i++)
                {
                    suffix = suffix + "0";
                }

                InvoiceNumber = Invoice.Substring(0, 6) + suffix + Convert.ToString(lastVal);
            }
            return InvoiceNumber;
        }


        public bool AddInvoice(GenerateInvoice GenerateInvoice, string[] ItemId, string[] ItemType, string[] ItemDescription, string[] Unit, string[] UnitPrice,
                                string[] ItemDuration, string[] Actions, Int64[] Taxation, string[] CalculatedTax, string[] ActionTax)
        {
            bool res = false;

            try
            {

                DataTable InvItem = new DataTable();
                InvItem.Columns.Add("InvoiceNumber");
                InvItem.Columns.Add("ItemId");
                InvItem.Columns.Add("ItemTypeId");
                InvItem.Columns.Add("ItemDescription");
                InvItem.Columns.Add("Unit");
                InvItem.Columns.Add("UnitPrice");
                InvItem.Columns.Add("Duration");
                InvItem.Columns.Add("Actions");


                string itemDuration = "";
                int itemId = 0;


                DataTable TaxItem = new DataTable();
                TaxItem.Columns.Add("InvoiceNumber");
                TaxItem.Columns.Add("Taxation");
                TaxItem.Columns.Add("CalculatedTax");
                TaxItem.Columns.Add("ActionTax");



                if (ItemType != null)
                {
                    for (int i = 0; i < ItemType.Length; i++)
                    {
                        if (ItemDuration[i].ToString() == "NA")
                        {
                            itemDuration = "0";
                        }
                        else
                        {
                            itemDuration = ItemDuration[i].ToString();
                        }

                        if (ItemId[i].ToString() == "0")
                        {
                            itemId = 0;
                        }
                        else
                        {
                            itemId = Convert.ToInt32(ItemId[i]);
                        }

                        InvItem.Rows.Add(GenerateInvoice.InvoiceNumber, itemId, ItemType[i], ItemDescription[i],
                                                  Unit[i], UnitPrice[i], itemDuration, Actions[i]);
                    }
                }

                if (Taxation != null)
                {
                    for (int j = 0; j < Taxation.Length; j++)
                    {
                        float calculatedTax = 0;
                        if (!string.IsNullOrEmpty(CalculatedTax[j]))
                            calculatedTax = Convert.ToSingle(CalculatedTax[j]);

                        TaxItem.Rows.Add(GenerateInvoice.InvoiceNumber, Taxation[j], calculatedTax, ActionTax[j]);
                    }
                }
                using (var context = new UserDBContext())
                {
                    var invoiceNumber = new SqlParameter("@InvoiceNumber", string.IsNullOrEmpty(GenerateInvoice.InvoiceNumber) ? DBNull.Value : (object)GenerateInvoice.InvoiceNumber);
                    var invoiceTo = new SqlParameter("@InvoiceTo", string.IsNullOrEmpty(GenerateInvoice.InvoiceTo) ? DBNull.Value : (object)GenerateInvoice.InvoiceTo);
                    var referenceId = new SqlParameter("@ReferenceId", string.IsNullOrEmpty(GenerateInvoice.ReferenceId) ? DBNull.Value : (object)GenerateInvoice.ReferenceId);
                    var total = new SqlParameter("@Total", GenerateInvoice.Total);
                    var additionalCost = new SqlParameter("@AdditionalCost", GenerateInvoice.AdditionalCost);
                    var deductions = new SqlParameter("@Deductions", GenerateInvoice.Deductions);
                    var netAmount = new SqlParameter("@NetAmount", GenerateInvoice.NetAmount);
                    var invoiceDate = new SqlParameter("@InvoiceDate", DateTime.UtcNow);
                    var invoiceSubject = new SqlParameter("@InvoiceSubject", string.IsNullOrEmpty(GenerateInvoice.InvoiceSubject) ? DBNull.Value : (object)GenerateInvoice.InvoiceSubject);
                    var currency = new SqlParameter("@Currency", string.IsNullOrEmpty(GenerateInvoice.Currency) ? DBNull.Value : (object)GenerateInvoice.Currency);
                    var status = new SqlParameter("@Status", string.IsNullOrEmpty(GenerateInvoice.Status) ? DBNull.Value : (object)GenerateInvoice.Status);
                    var acknowledge = new SqlParameter("@Acknowledge", GenerateInvoice.Acknowledge);
                    var entryDescription = new SqlParameter("@EntryDescription", string.IsNullOrEmpty(GenerateInvoice.EntryDescription) ? DBNull.Value : (object)GenerateInvoice.EntryDescription);
                    var remarks = new SqlParameter("@Remarks", string.IsNullOrEmpty(GenerateInvoice.Remarks) ? DBNull.Value : (object)GenerateInvoice.Remarks);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(GenerateInvoice.SubscriberId) ? DBNull.Value : (object)GenerateInvoice.SubscriberId);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(GenerateInvoice.CorporateId) ? DBNull.Value : (object)GenerateInvoice.CorporateId);
                    var paymentModeId = new SqlParameter("@PaymentModeId", GenerateInvoice.PaymentModeId);

                    var invoiceItemsTb = new SqlParameter("@InvoiceItemsTb", SqlDbType.Structured);
                    invoiceItemsTb.TypeName = "dbo.tvpInvoiceItem";
                    invoiceItemsTb.Value = InvItem;
                    var invoiceTaxTb = new SqlParameter("@InvoiceTaxTb", SqlDbType.Structured);
                    invoiceTaxTb.TypeName = "dbo.tvpTaxItem";
                    invoiceTaxTb.Value = TaxItem;

                    int i = context.Database.ExecuteSqlCommand("USP_AddInvoice  @InvoiceNumber, @InvoiceTo, @ReferenceId,  @Total, @AdditionalCost, " +
                                                               "@Deductions, @NetAmount, @InvoiceDate, @InvoiceSubject, @Currency, @Status, @Acknowledge, @EntryDescription, " +
                                                               "@Remarks, @SubscriberId, @CorporateId, @PaymentModeId, @InvoiceItemsTb, @InvoiceTaxTb",
                                                                invoiceNumber, invoiceTo, referenceId, total, additionalCost, deductions, netAmount, invoiceDate,
                                                                invoiceSubject, currency, status, acknowledge, entryDescription, remarks, subscriberId, corporateId, paymentModeId, invoiceItemsTb, invoiceTaxTb);

                    if (i > 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                /*Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator."); */
            }

            return res;
        }

        public void CreateInvoiceItems(string InvoiceNumber, short ItemId, long ItemType, string Description, int Unit, float UnitPrice, int Duration)
        {
            InvoiceItems JOItem = new InvoiceItems();

            JOItem.InvoiceNumber = InvoiceNumber;
            JOItem.ItemId = ItemId;
            JOItem.ItemTypeId = ItemType;
            JOItem.ItemDescription = Description;
            JOItem.Unit = Unit;
            JOItem.UnitPrice = UnitPrice;
            JOItem.Duration = Duration;
            db.InvoiceItems.Add(JOItem);
            db.SaveChanges();
        }

        public void AddInvoiceTaxes(string InvoiceNumber, Int64 TaxationId, float TaxationAmount)
        {
            InvoiceTaxationDetails InvoiceTaxes = new InvoiceTaxationDetails();

            InvoiceTaxes.InvoiceNumber = InvoiceNumber;
            InvoiceTaxes.TaxationId = TaxationId;
            InvoiceTaxes.TaxactionAmount = TaxationAmount;
            db.InvoiceTaxationDetails.Add(InvoiceTaxes);
            db.SaveChanges();
        }

        public void CreateTaskItems(string TaskID, short ItemId, Int64 ItemTypeId, string Description, int Unit, float UnitPrice, int Duration)
        {
            TaskItems TItem = new TaskItems();
            TItem.TaskId = TaskID;
            TItem.ItemId = ItemId;
            TItem.ItemTypeId = ItemTypeId;
            TItem.ItemDescription = Description;
            TItem.Unit = Unit;
            TItem.UnitPrice = UnitPrice;
            TItem.Duration = Duration;
            db.TaskItems.Add(TItem);
            db.SaveChanges();
        }

        public List<GenerateInvoiceView> GETInvoiceFor(string ReferenceId)
        {

            var INVlist = new List<GenerateInvoiceView>();

            using (var db = new UserDBContext())
            {

                INVlist = db.Database
                          .SqlQuery<GenerateInvoiceView>("EXEC USP_GETInvoiceFor @ReferenceId",
                           new SqlParameter("@ReferenceId", string.IsNullOrEmpty(ReferenceId) ? DBNull.Value : (object)ReferenceId)).ToList();
            }

            return INVlist;
        }

        public List<GenerateInvoiceView> GetInvoice(string UserId, string Tag, string Status, string CorporateId)
        {

            if (Tag == "Incoming")
            {
                Tag = "TA";
            }
            else if (Tag == "Outgoing")
            {
                Tag = "JO";
            }
            else if (Tag == "Any")
            {
                Tag = "ANY";
            }

            var invoice = new List<GenerateInvoiceView>();

            using (var db = new UserDBContext())
            {
                invoice = db.Database
                          .SqlQuery<GenerateInvoiceView>("EXEC USP_GetInvoice @UserId, @Tag , @Status ,@CorporateId",
                          new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId),
                          new SqlParameter("@Tag", string.IsNullOrEmpty(Tag) ? DBNull.Value : (object)Tag),
                          new SqlParameter("@Status", string.IsNullOrEmpty(Status) ? DBNull.Value : (object)Status),
                          new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)
                          ).ToList();
            }

            return invoice;
        }

        public GenerateInvoiceView GetInvoiceDetails(string SubscriberId, string InvoiceNumber)
        {

            var Count = new GenerateInvoiceView();

            using (var db = new UserDBContext())
            {
                Count = db.Database
                          .SqlQuery<GenerateInvoiceView>("EXEC USP_GetInvoiceDetails @SubscriberId, @InvoiceNumber ",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId),
                          new SqlParameter("@InvoiceNumber", string.IsNullOrEmpty(InvoiceNumber) ? DBNull.Value : (object)InvoiceNumber)
                          ).FirstOrDefault();
            }

            return Count;
        }

        public List<InvoiceItemsView> GetInvoiceItems(string InvoiceNumber)
        {

            var invoiceItems = new List<InvoiceItemsView>();

            using (var db = new UserDBContext())
            {

                invoiceItems = db.Database
                          .SqlQuery<InvoiceItemsView>("EXEC GetInvoiceItems @InvoiceNumber",
                          new SqlParameter("@InvoiceNumber", string.IsNullOrEmpty(InvoiceNumber) ? DBNull.Value : (object)InvoiceNumber)).ToList();
            }

            return invoiceItems;
        }

        public List<InvoiceTaxationDetailsView> GetInvoiceTaxes(string InvoiceNumber)
        {

            var invoiceTaxes = new List<InvoiceTaxationDetailsView>();

            using (var db = new UserDBContext())
            {

                invoiceTaxes = db.Database
                          .SqlQuery<InvoiceTaxationDetailsView>("EXEC GetInvoiceTaxes @InvoiceNumber",
                          new SqlParameter("@InvoiceNumber", string.IsNullOrEmpty(InvoiceNumber) ? DBNull.Value : (object)InvoiceNumber)).ToList();
            }

            return invoiceTaxes;
        }

        public bool UpdateInvoiceStatus(string InvoiceNumber, string ReferenceId, string Status)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var invoiceNumber = new SqlParameter("@InvoiceNumber", string.IsNullOrEmpty(InvoiceNumber) ? DBNull.Value : (object)InvoiceNumber);
                    var referenceId = new SqlParameter("@ReferenceId", string.IsNullOrEmpty(ReferenceId) ? DBNull.Value : (object)ReferenceId);
                    var status = new SqlParameter("@Status", string.IsNullOrEmpty(Status) ? DBNull.Value : (object)Status);

                    int i = context.Database.ExecuteSqlCommand("USP_UpdateInvoiceStatus @InvoiceNumber, @ReferenceId,  @Status",
                                                               invoiceNumber, referenceId, status);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
            }

            return res;
        }

        public bool UpdateInvoicePayment(string InvoiceNumber, string BankName, string ReferenceNumber, DateTime? PaymentDate, string PayerRemarks, Int16 PaymentModeId, string Status = "Onhold")
        {
            GenerateInvoice invoice = db.GenerateInvoice.Find(InvoiceNumber);
            if (invoice != null)
            {
                invoice.BankName = BankName;
                invoice.ReferenceNumber = ReferenceNumber;
                invoice.PaymentDate = PaymentDate;
                invoice.Status = Status;
                invoice.PayerRemarks = PayerRemarks;
                invoice.PaymentModeId = PaymentModeId;
                db.Entry(invoice).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public GetCountEntryView GetCountEntry(string SubscriberId)
        {

            var Count = new GetCountEntryView();

            using (var db = new UserDBContext())
            {
                Count = db.Database
                          .SqlQuery<GetCountEntryView>("EXEC USP_GetCountEntry @SubscriberId ",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)
                          ).FirstOrDefault();
            }

            return Count;
        }

        public GetUserTaskInvoiceView GetUserwiseTasksInvoicesAndTrainigsCount(string UserId)
        {

            var Count = new GetUserTaskInvoiceView();

            using (var db = new UserDBContext())
            {
                Count = db.Database
                          .SqlQuery<GetUserTaskInvoiceView>("EXEC USP_GetTasksInvoicesAndTrainingCount @UserId ",
                          new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                          ).FirstOrDefault();
            }

            return Count;
        }

        public InstallmentMaster GetInstallments()
        {
            var installments = new InstallmentMaster();

            using (var db = new UserDBContext())
            {
                installments = db.Database
                          .SqlQuery<InstallmentMaster>("EXEC USP_GetInstallments").FirstOrDefault();
            }

            return installments;
        }

        public List<PaymentModeMaster> GetPaymentModes()
        {
            var paymentModes = new List<PaymentModeMaster>();

            using (var db = new UserDBContext())
            {
                paymentModes = db.Database
                          .SqlQuery<PaymentModeMaster>("EXEC USP_GetPaymentModes").ToList();
            }

            return paymentModes;
        }

        public bool AddInstallment(string Installment)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var installment = new SqlParameter("@Installment", string.IsNullOrEmpty(Installment) ? DBNull.Value : (object)Installment);

                    int i = context.Database.ExecuteSqlCommand("USP_AddInstallment  @Installment", installment);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return res;
        }

        public bool AddPaymentMode(string PaymentMode, int PaymentModeId = 0)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var paymentMode = new SqlParameter("@PaymentMode", string.IsNullOrEmpty(PaymentMode) ? DBNull.Value : (object)PaymentMode);
                    var paymentModeId = new SqlParameter("@PaymentModeId", (object)PaymentModeId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddPaymentMode  @PaymentMode, @PaymentModeId", paymentMode, paymentModeId);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return res;
        }

        public GetCountEntryView GetRecordCountForClint(string ClientId)
        {

            var Count = new GetCountEntryView();

            using (var db = new UserDBContext())
            {
                Count = db.Database
                          .SqlQuery<GetCountEntryView>("EXEC USP_GetRecordCountForClint @ClientId ",
                           new SqlParameter("@ClientId", string.IsNullOrEmpty(ClientId) ? DBNull.Value : (object)ClientId)
                          ).FirstOrDefault();
            }

            return Count;
        }

        public bool AddIdentificationType(short IdentificationTypeId, string IdentificationTypeName)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var identificationtypeid = new SqlParameter("@IdentificationTypeId", IdentificationTypeId);
                    var identificationtypename = new SqlParameter("@IdentificationTypeName", string.IsNullOrEmpty(IdentificationTypeName) ? DBNull.Value : (object)IdentificationTypeName);

                    int i = context.Database.ExecuteSqlCommand("USP_AddIdentificationType @IdentificationTypeId, @IdentificationTypeName", identificationtypeid, identificationtypename);

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

        public bool AddNotification(string AssignedTo, string AssignedBy, string NotificationBody, string NotificationFor, string ReferenceId, Boolean Status, DateTime ViewTime)
        {
            bool res = false;


            try
            {
                using (var context = new UserDBContext())
                {
                    var assignedto = new SqlParameter("@AssingedTo", string.IsNullOrEmpty(AssignedTo) ? DBNull.Value : (object)AssignedTo);
                    var assignedby = new SqlParameter("@AssignedBy", string.IsNullOrEmpty(AssignedBy) ? DBNull.Value : (object)AssignedBy);
                    var notificationbody = new SqlParameter("@NotificationBody", string.IsNullOrEmpty(NotificationBody) ? DBNull.Value : (object)NotificationBody);
                    var notificationFor = new SqlParameter("@NotificationFor", string.IsNullOrEmpty(NotificationFor) ? DBNull.Value : (object)NotificationFor);
                    var referenceId = new SqlParameter("@ReferenceId", string.IsNullOrEmpty(ReferenceId) ? DBNull.Value : (object)ReferenceId);
                    var status = new SqlParameter("@Status", Status);
                    var viewtime = new SqlParameter("@ViewDate", ViewTime);


                    int i = context.Database.ExecuteSqlCommand("USPAddNotification @AssingedTo,@AssignedBy,@NotificationBody,@NotificationFor,@ReferenceId, @Status,@ViewDate", assignedto, assignedby, notificationbody, notificationFor, referenceId, status, viewtime);

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

        public bool UpdateNotification(Int64 NotificationId, DateTime ViewTime)
        {
            bool res = false;


            try
            {
                using (var context = new UserDBContext())
                {

                    var notificationid = new SqlParameter("@NotificationId", NotificationId);
                    var viewtime = new SqlParameter("@ViewTime", ViewTime);


                    int i = context.Database.ExecuteSqlCommand("USP_UpdateUserNotification @NotificationId,@ViewTime", notificationid, viewtime);

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

        public void RemoveNotification(Int64 NotificationId)
        {
            var notification = db.UserNotification.Find(NotificationId);
            if (notification != null)
            {
                db.UserNotification.Remove(notification);
                db.SaveChanges();
            }
        }

        public List<IdentificationTypeMaster> GetIdentificationType()
        {

            var identificationtype = new List<IdentificationTypeMaster>();

            using (var db = new UserDBContext())
            {
                identificationtype = db.Database
                          .SqlQuery<IdentificationTypeMaster>("EXEC USP_GetIdentificationType").ToList();
            }

            return identificationtype;
        }

        public List<UserNotificationView> GetUserNotification(string SubscriberId)
        {
            var notifications = new List<UserNotificationView>();
            using (var db = new UserDBContext())
            {

                var subscriberid = new SqlParameter("@AssingedTo", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);

                notifications = db.Database.SqlQuery<UserNotificationView>("EXEC USPUserNotifications @AssingedTo", subscriberid).ToList();
            }
            return notifications;

        }

        public GetCountNotification SPCountNotification(string UserId)
        {

            var Count = new GetCountNotification();

            using (var db = new UserDBContext())
            {
                Count = db.Database
                          .SqlQuery<GetCountNotification>("EXEC USP_CountNotification @AssingedTo ",
                          new SqlParameter("@AssingedTo", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                          ).FirstOrDefault();
            }

            return Count;
        }

        public GetCountNotification SPCountCourse(string UserId)
        {

            var Count = new GetCountNotification();

            using (var db = new UserDBContext())
            {
                Count = db.Database
                          .SqlQuery<GetCountNotification>("EXEC USP_CourseCount @SubscriberId ",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                          ).FirstOrDefault();
            }

            return Count;
        }

        public bool AddItemTypeMaster(Int64 ItemTypeId, string ItemTypeName, string CorporateId)
        {
            bool res = false;

            using (var context = new UserDBContext())
            {
                var itemid = new SqlParameter("@ItemTypeId", ItemTypeId);
                var itemname = new SqlParameter("@ItemTypeName", string.IsNullOrEmpty(ItemTypeName) ? DBNull.Value : (object)ItemTypeName);
                var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);

                int i = context.Database.ExecuteSqlCommand("USP_AddItemTypeMasters @ItemTypeId,@ItemTypeName,@CorporateId", itemid, itemname, corporateid);

                if (i == 1)
                    res = true;
            }


            return res;
        }
        //Created by vikas pandey
        //17/11/2017
        public bool AddLeaveSchemeType(int SchemeId, string SchemeName)
        {
            bool res = false;

            using (var context = new UserDBContext())
            {
                var schemeid = new SqlParameter("@SchemeId", SchemeId);
                var schemename = new SqlParameter("@SchemeName", string.IsNullOrEmpty(SchemeName) ? DBNull.Value : (object)SchemeName);
                int i = context.Database.ExecuteSqlCommand("USP_AddLeaveSchemeMasters @SchemeId,@SchemeName", schemeid, schemename);

                if (i == 1)
                    res = true;
            }


            return res;
        }
        //Created by vikas pandey
        //17/11/2017
        public bool AddLeaveType(string LeaveTypeId, string LeaveTypeName, string LeaveCategory)
        {
            bool res = false;

            using (var context = new UserDBContext())
            {
                var leavetypeid = new SqlParameter("@LeaveTypeId", string.IsNullOrEmpty(LeaveTypeId) ? DBNull.Value : (object)LeaveTypeId);
                var leavetypename = new SqlParameter("@LeaveTypeName", string.IsNullOrEmpty(LeaveTypeName) ? DBNull.Value : (object)LeaveTypeName);
                var leavetyepecategory = new SqlParameter("@LeaveTypeCategory", string.IsNullOrEmpty(LeaveCategory) ? DBNull.Value : (object)LeaveCategory);
                int i = context.Database.ExecuteSqlCommand("USP_LeaveTypeMaster @LeaveTypeId,@LeaveTypeName,@LeaveTypeCategory", leavetypeid, leavetypename, leavetyepecategory);

                if (i == 1)
                    res = true;
            }


            return res;
        }


        public List<ItemTypeMasters> GetItemTypeMaster()
        {
            var getitemtype = new List<ItemTypeMasters>();

            using (var db = new UserDBContext())
            {
                getitemtype = db.Database
                          .SqlQuery<ItemTypeMasters>("EXEC USP_GetItemTypeMasters").ToList();
            }

            return getitemtype;
        }

        public bool AddTaxMaster(Int64 TaxationId, string TaxName, float TaxationValue, string CorporateId)
        {
            bool res = false;

            using (var context = new UserDBContext())
            {
                var taxationid = new SqlParameter("@TaxationId", TaxationId);
                var taxname = new SqlParameter("@TaxName", string.IsNullOrEmpty(TaxName) ? DBNull.Value : (object)TaxName);
                var taxationvalue = new SqlParameter("@TaxationValue", TaxationValue);
                var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);

                int i = context.Database.ExecuteSqlCommand("USP_AddTaxMasters @TaxationId,@TaxName,@TaxationValue,@CorporateId", taxationid, taxname, taxationvalue, corporateid);

                if (i == 1)
                    res = true;
            }

            return res;
        }

        public List<TaxMaster> GetTaxMaster()
        {
            var getTax = new List<TaxMaster>();

            using (var db = new UserDBContext())
            {
                getTax = db.Database
                          .SqlQuery<TaxMaster>("EXEC USP_GetTaxMasters").ToList();
            }

            return getTax;
        }

        public GetAllTypeUserCount GetCountValue()
        {

            var Count = new GetAllTypeUserCount();

            using (var db = new UserDBContext())
            {
                Count = db.Database
                          .SqlQuery<GetAllTypeUserCount>("EXEC USP_GetCountValue").FirstOrDefault();
            }

            return Count;
        }

        public List<CandidateViewModel> GetAllCandidateList()
        {
            List<CandidateViewModel> CandidateList = new List<CandidateViewModel>();

            using (var db = new UserDBContext())
            {
                CandidateList = db.Database
                         .SqlQuery<CandidateViewModel>("exec USP_GetAllCandidateList").ToList();
            }

            return CandidateList;
        }

        public List<EmployeeViewModel> GetAllEmployeeList()
        {
            List<EmployeeViewModel> EmployeeList = new List<EmployeeViewModel>();

            using (var db = new UserDBContext())
            {
                EmployeeList = db.Database
                         .SqlQuery<EmployeeViewModel>("exec USP_GetAllEmployeeList").ToList();
            }

            return EmployeeList;
        }

        public List<ClientViewModel> GetAllClientList()
        {
            List<ClientViewModel> EmployeeList = new List<ClientViewModel>();

            using (var db = new UserDBContext())
            {
                EmployeeList = db.Database
                         .SqlQuery<ClientViewModel>("exec USP_GetAllVendorList").ToList();
            }

            return EmployeeList;
        }

        public List<ClientViewModel> GetCoAdminList(string SubscriberId)
        {
            List<ClientViewModel> CoAdminList = new List<ClientViewModel>();

            using (var db = new UserDBContext())
            {
                CoAdminList = db.Database
             .SqlQuery<ClientViewModel>("EXEC USP_GetSubscriberwiseCoAdmin @SubscriberId ",
                           new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            return CoAdminList;
        }

        public LoginViewModel GetLoginDetails(string LoginKey = null)
        {

            var user = new LoginViewModel();

            using (var db = new UserDBContext())
            {

                user = db.Database
                          .SqlQuery<LoginViewModel>("exec USP_GetLoginDetails @LoginKey",
                           new SqlParameter("@LoginKey", string.IsNullOrEmpty(LoginKey) ? DBNull.Value : (object)LoginKey)).FirstOrDefault();
            }

            return user;
        }

        public string GenerateUserName()
        {
            //string year = Convert.ToString(DateTime.UtcNow.Year).Substring(2);
            //string quarter = "R1";
            //int month = DateTime.UtcNow.Month;
            //if (month > 3 && month <= 6)
            //{
            //    quarter = "R2";
            //}
            //else if (month > 6 && month <= 9)
            //{
            //    quarter = "R3";
            //}
            //else if (month > 9 && month <= 12)
            //{
            //    quarter = "R4";
            //}
            //string userName = "REC" + year + quarter + "1";

            //var user = "";
            //using (var db = new UserDBContext())
            //{
            //    user = db.Database.SqlQuery<string>("exec USP_GetMaxUserName @Record",
            //         new SqlParameter("@Record", "REC" + year + quarter)).FirstOrDefault();
            //}

            //if (user != null)
            //{
            //    string suffix = user.Substring(7);
            //    string qr = user.Substring(5, 2);
            //    int sfx = 0;
            //    if (quarter != qr)
            //    {
            //        sfx = 1;
            //    }
            //    else
            //    {
            //        sfx = Convert.ToInt32(suffix) + 1;
            //    }
            //    userName = "REC" + year + quarter + sfx.ToString();
            //}



            //return userName;
            string userName;
            using (var db = new UserDBContext())
            {
                userName = db.Database.SqlQuery<string>("exec USP_GETUSERNAME").FirstOrDefault();
            }
            return userName;
        }

        public string GetSIUserId(string Branch, string TrainingType)
        {
            UserDBContext udbc = new UserDBContext();
            string type = "";
            if (TrainingType == "PA")
            {
                type = "F";
                var BranchDetails = udbc.BranchMaster.Where(c => c.Branch == Branch).FirstOrDefault();
                string BranchCode = BranchDetails.BranchCode;

                string SIUserId = BranchCode + type + "0001";

                var SIRegistrationId = from s in udbc.UserProfile.Where(s => s.RegistrationId.Substring(0, 5) == BranchCode + type)
                                       orderby s.RegistrationId descending
                                       select s.RegistrationId;

                var SIID = SIRegistrationId.FirstOrDefault();

                if (SIID != null)
                {
                    string SIIDPartialId = SIID.Substring(5);
                    int lastVal = Convert.ToInt32(SIIDPartialId);
                    lastVal = lastVal + 1;
                    string suffix = string.Empty;

                    for (int i = Convert.ToString(lastVal).Length; i < 4; i++)
                    {
                        suffix = suffix + "0";
                    }

                    SIUserId = SIID.Substring(0, 5) + suffix + Convert.ToString(lastVal);
                }
                return SIUserId;
            }
            else
            {
                type = "M";
                var BranchDetails = udbc.BranchMaster.Where(c => c.Branch == Branch).FirstOrDefault();
                string BranchCode = BranchDetails.BranchCode;

                string SIUserId = BranchCode + type + "3001";

                var SIRegistrationId = from s in udbc.UserProfile.Where(s => s.RegistrationId.Substring(0, 6) == BranchCode + type + "3")
                                       orderby s.RegistrationId descending
                                       select s.RegistrationId;

                var SIID = SIRegistrationId.FirstOrDefault();

                if (SIID != null)
                {
                    string SIIDPartialId = SIID.Substring(5);
                    int lastVal = Convert.ToInt32(SIIDPartialId);
                    lastVal = lastVal + 1;
                    string suffix = string.Empty;

                    for (int i = Convert.ToString(lastVal).Length; i < 4; i++)
                    {
                        suffix = suffix + "0";
                    }

                    SIUserId = SIID.Substring(0, 5) + suffix + Convert.ToString(lastVal);
                }
                return SIUserId;
            }

        }

        public string uploadFile(string TaskId, HttpPostedFileBase upload)
        {
            string res = "Failure";
            if (upload != null && upload.ContentLength > 0)
            {

                var file = db.TaskAttachment.Where(d => d.TaskId == TaskId).FirstOrDefault();

                if (file != null)
                {
                    Int64 FileId = file.FileId;

                    blobManager.DeleteBlob(TaskId.ToLower(), GetFileName(FileId).ToLower());
                    db.TaskAttachment.Remove(file);
                    db.SaveChanges();
                }


                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");


                bool added = AddTaskAttachmentToDB(TaskId, imgFileName.ToLower(), upload.ContentType);

                if (added)
                {
                    blobManager.UploadBlob(TaskId.ToLower(), ReplaceFileName(TaskId).ToLower(), upload);

                }

                res = "Succeed";
            }


            return res;
        }

        public string GetFileName(Int64 FileId)
        {
            string fileName = null;
            var image = db.TaskAttachment.Find(FileId);

            if (image != null)
            {
                fileName = "attachment/" + image.FileId + "/" + image.FileName;
            }
            return fileName;
        }

        public string GetEngagementFileName(Int64 FileId)
        {
            string fileName = null;
            var image = db.TrainerPlannerAttachment.Find(FileId);

            if (image != null)
            {
                fileName = "attachment/" + image.FileId + "/" + image.FileName;
            }
            return fileName;
        }

        public string ReplaceFileName(string TaskId)
        {
            string fileName = null;
            var image = db.TaskAttachment.Where(f => f.TaskId == TaskId);

            if (image != null)
            {
                if (image.Count() > 0)
                {
                    string imgFileName = image.FirstOrDefault().FileName;
                    imgFileName = imgFileName.Replace(' ', '_');
                    imgFileName = imgFileName.Replace("'", "_");


                    fileName = "attachment/" + image.FirstOrDefault().FileId + "/" + imgFileName;
                }
            }
            return fileName;
        }

        public bool AddTaskAttachmentToDB(string TaskId, string FileName, string ContentType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var taskId = new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);

                    int i = context.Database.ExecuteSqlCommand("USP_AddTaskAttachment @TaskId, @FileName, @ContentType",
                        taskId, fileName, contentType);

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

        public string uploadInvFile(string ReferenceId, HttpPostedFileBase upload)
        {
            string res = "Failure";
            if (upload != null && upload.ContentLength > 0)
            {

                var file = db.InVoiceAttachment.Where(d => d.ReferenceId == ReferenceId).FirstOrDefault();

                if (file != null)
                {
                    Int64 FileId = file.FileId;

                    blobManager.DeleteBlob(ReferenceId.ToLower(), GetFileInvName(FileId).ToLower());
                    db.InVoiceAttachment.Remove(file);
                    db.SaveChanges();
                }


                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");


                bool added = AddInvAttachmentToDB(ReferenceId, imgFileName.ToLower(), upload.ContentType);

                if (added)
                {
                    blobManager.UploadBlob(ReferenceId.ToLower(), ReplaceInvFileName(ReferenceId).ToLower(), upload);

                }

                res = "Succeed";
            }


            return res;
        }

        public string GetFileInvName(Int64 FileId)
        {
            string fileName = null;
            var image = db.InVoiceAttachment.Find(FileId);

            if (image != null)
            {
                fileName = "attachment/" + image.FileId + "/" + image.FileName;
            }
            return fileName;
        }

        public string GetTrainingscheduleFile(Int64 FileId)
        {
            string fileName = null;
            var image = db.TrainingScheduleAttachment.Find(FileId);

            if (image != null)
            {
                fileName = "attachment/" + image.FileId + "/" + image.FileName;
            }
            return fileName;
        }

        public string ReplaceInvFileName(string ReferenceId)
        {
            string fileName = null;
            var image = db.InVoiceAttachment.Where(f => f.ReferenceId == ReferenceId);

            if (image != null)
            {
                if (image.Count() > 0)
                {
                    string imgFileName = image.FirstOrDefault().FileName;
                    imgFileName = imgFileName.Replace(' ', '_');
                    imgFileName = imgFileName.Replace("'", "_");


                    fileName = "attachment/" + image.FirstOrDefault().FileId + "/" + imgFileName;
                }
            }
            return fileName;
        }

        public bool AddInvAttachmentToDB(string ReferenceId, string FileName, string ContentType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var referenceId = new SqlParameter("@ReferenceId", string.IsNullOrEmpty(ReferenceId) ? DBNull.Value : (object)ReferenceId);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);

                    int i = context.Database.ExecuteSqlCommand("USP_AddInvAttachment @ReferenceId, @FileName, @ContentType",
                        referenceId, fileName, contentType);

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

        public List<TaskAttachment> GetTaskAttachments(string TaskId)
        {
            var taskAttachment = new List<TaskAttachment>();
            try
            {
                using (var context = new UserDBContext())
                {
                    taskAttachment = db.Database
                            .SqlQuery<TaskAttachment>("EXEC USP_GetTaskAttachments @TaskId",
                             new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return taskAttachment;
        }

        public List<TrainingScheduleAttachment> GetTrainingAttachments(string TrainingId)
        {
            var trainingScheduleAttachment = new List<TrainingScheduleAttachment>();
            try
            {
                using (var context = new UserDBContext())
                {
                    trainingScheduleAttachment = db.Database
                            .SqlQuery<TrainingScheduleAttachment>("EXEC USP_GetTrainingAttachments @TrainingId",
                             new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return trainingScheduleAttachment;
        }

        public string GeneratePassword(string Name)
        {

            return Name.Substring(0, 1).Replace(' ', '5').ToUpper() + GetRandomSymbol(Name.Length) + Name.Length.ToString() + Name.Substring(1, 2).Replace(' ', '5').ToLower() + GetRandomSymbol(Name.Length + 1);
        }

        public string GetRandomSymbol(int count)
        {
            string random = "!";
            int remain = count % 2;
            if (remain == 0)
            {
                random = "!";
            }
            else if (remain == 1)
            {
                random = "@";
            }
            else if (remain == 2)
            {
                random = "$";

            }
            else if (remain == 3)
            {
                random = "*";
            }
            else if (remain == 4)
            {
                random = "?";
            }
            else if (remain == 5)
            {
                random = "!";
            }
            else if (remain == 6)
            {
                random = "%";
            }
            else if (remain == 7)
            {
                random = "#";
            }
            else if (remain == 8)
            {
                random = "1";
            }
            else if (remain == 8)
            {
                random = "1";
            }
            else
            {
                random = "=";
            }

            return random;
        }

        public bool IsUserDeactivated(string UserId, string roleId)
        {
            if (roleId.ToUpper() == "CANDIDATE")
            {
                var userProfile = db.UserProfile.Where(p => p.UserId == UserId).FirstOrDefault();
                if (userProfile != null && userProfile.Deactivated)
                {
                    return true;
                }
            }
            else if (roleId.ToUpper() == "EMPLOYEE")
            {
                var empProfile = db.EmployeeBasicDetails.Where(p => p.UserId == UserId).FirstOrDefault();
                if (empProfile != null && empProfile.Deactivated)
                {
                    return true;
                }
            }
            else if (roleId.ToUpper() == "CLIENT")
            {
                var corporateProfile = db.CorporateProfile.Where(p => p.CorporateId == UserId).FirstOrDefault();
                if (corporateProfile != null && corporateProfile.Deactivated)
                {
                    return true;
                }
            }
            else if (roleId.ToUpper() == "ADMINISTRATOR")
            {

            }
            return false;
        }

        public bool RemoveInvoice(string ReferenceId, string InvoiceNumber, string SubscriberId)
        {

            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    if (!string.IsNullOrEmpty(ReplaceInvFileName(ReferenceId)))
                        blobManager.DeleteBlob(ReferenceId.ToLower(), ReplaceInvFileName(ReferenceId).ToLower());

                    var referenceId = new SqlParameter("@ReferenceId", string.IsNullOrEmpty(ReferenceId) ? DBNull.Value : (object)ReferenceId);
                    var invoiceNumber = new SqlParameter("@InvoiceNumber", string.IsNullOrEmpty(InvoiceNumber) ? DBNull.Value : (object)InvoiceNumber);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteInvoices @ReferenceId,  @InvoiceNumber, @SubscriberId",
                        referenceId, invoiceNumber, subscriberId);

                    if (i > 0)
                    {
                        res = true;
                    }
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }

        public bool UpdateUserEmailPhone(string UserName, string Email, string PhoneNumber, bool EmailConfirmed)
        {
            bool res = false;
            using (var context = new UserDBContext())
            {
                var userName = new SqlParameter("@UserName", string.IsNullOrEmpty(UserName) ? DBNull.Value : (object)UserName);
                var email = new SqlParameter("@Email", string.IsNullOrEmpty(Email) ? DBNull.Value : (object)Email);
                var phoneNumber = new SqlParameter("@PhoneNumber", string.IsNullOrEmpty(PhoneNumber) ? DBNull.Value : (object)PhoneNumber);
                var emailConfirmed = new SqlParameter("@EmailConfirmed", (object)EmailConfirmed);

                int i = context.Database.ExecuteSqlCommand("USP_UpdateUserEmailPhone @UserName,@Email,@PhoneNumber,@EmailConfirmed", userName, email, phoneNumber, emailConfirmed);

                if (i == 1)
                    res = true;
            }
            return res;
        }

        //Start
        //Created By:- Ajay kumar Choudhary Created of:- 17-5-2017

        public bool AddEmproles(Int16 EmpRoleId, string EmpRole, bool Visibility, DateTime UpdatedOn, string UpdatedBy)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var empRoleId = new SqlParameter("@EmpRoleId", EmpRoleId);
                    var role = new SqlParameter("@EmpRole", string.IsNullOrEmpty(EmpRole) ? DBNull.Value : (object)EmpRole);
                    var visibility = new SqlParameter("@Visibility", Visibility);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);

                    int i = context.Database.ExecuteSqlCommand("USP_AddEmpRoles  @EmpRoleId, @EmpRole, @Visibility, @UpdatedOn, @UpdatedBy", empRoleId, role, visibility, updatedOn, updatedBy);

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
        ///

        public List<ClientTeamRoles> GetempRoles()
        {

            var EmpRoles = new List<ClientTeamRoles>();

            using (var db = new UserDBContext())
            {

                EmpRoles = db.Database
                          .SqlQuery<ClientTeamRoles>("exec USP_GETEmpRoles").ToList();
            }

            return EmpRoles;
        }

        public void Deleteroles(Int16 EmpRoleId)
        {
            var Roles = db.ClientTeamRoles.Find(EmpRoleId);
            if (Roles != null)
            {
                db.ClientTeamRoles.Remove(Roles);
                db.SaveChanges();
            }
        }

        public bool AddEmprights(Int16 EmpRightsId, string Rights, Int16 GroupType, bool Visibility, DateTime UpdatedOn, string UpdatedBy)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var empRightsId = new SqlParameter("@EmpRightsId", EmpRightsId);
                    var rights = new SqlParameter("@Rights", string.IsNullOrEmpty(Rights) ? DBNull.Value : (object)Rights);
                    var groupType = new SqlParameter("@GroupType", GroupType);
                    var visibility = new SqlParameter("@Visibility", Visibility);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);

                    int i = context.Database.ExecuteSqlCommand("USP_AddEmprigths  @EmpRightsId, @Rights, @GroupType,  @Visibility, @UpdatedOn, @UpdatedBy", empRightsId, rights, groupType, visibility, updatedOn, updatedBy);

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

        public List<ClientTeamRights> GetempRights()
        {

            var EmpRights = new List<ClientTeamRights>();

            using (var db = new UserDBContext())
            {

                EmpRights = db.Database
                          .SqlQuery<ClientTeamRights>("exec USP_GETEmpRightsId").ToList();
            }

            return EmpRights;
        }

        public void Deleterights(Int16 EmpRightsId)
        {
            var Rights = db.ClientTeamRights.Find(EmpRightsId);
            if (Rights != null)
            {
                db.ClientTeamRights.Remove(Rights);
                db.SaveChanges();
            }
        }

        //End

        /// <summary>
        /// Created by : Achal Kumar Jha
        /// Created on : 29-05-2017
        /// Created For :Payroll
        /// </summary>        
        /// <returns></returns>
        public List<PayrollHeads> GetPayrollHeadList()
        {
            var headList = new List<PayrollHeads>();
            try
            {
                using (var context = new UserDBContext())
                {
                    headList = db.Database
                        .SqlQuery<PayrollHeads>("exec USP_GetPayrollHeads").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return headList;
        }
        public bool AddPayrollHead(string HeadName, int HeadType = 0, int HeadId = 0)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var headName = new SqlParameter("@headName", string.IsNullOrEmpty(HeadName) ? DBNull.Value : (object)HeadName);
                    var headType = new SqlParameter("@HeadType", string.IsNullOrEmpty(HeadType.ToString()) ? DBNull.Value : (object)HeadType);
                    var headId = new SqlParameter("@headId", string.IsNullOrEmpty(HeadId.ToString()) ? DBNull.Value : (object)HeadId);
                    int i = context.Database.ExecuteSqlCommand("USP_AddPayrollHead  @headName, @HeadType,@headId", headName, headType, headId);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return res;
        }

        /// <summary>
        /// Created by : Achal Kumar Jha
        /// Created on : 30-05-2017
        /// Created For :Payroll Leavs Settings
        /// </summary>        
        /// <returns></returns>

        public List<PayrollLeavsSettings> GetPayrollLeavsList(string subscriberID)
        {
            var leaveList = new List<PayrollLeavsSettings>();
            try
            {
                using (var context = new UserDBContext())
                {
                    leaveList = db.Database
                             .SqlQuery<PayrollLeavsSettings>("EXEC USP_GetPayrollLeavsSettings @SubscriberId",
                              new SqlParameter("@SubscriberId", string.IsNullOrEmpty(subscriberID) ? DBNull.Value : (object)subscriberID)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return leaveList;
        }
        public bool AddPayrollLeavesTypeMaster(string SubscriberId, string LeaveName, Int16 NoofDays, Int16 SalarycalculationOn, Int16 LeaveId, Int16 HolidayInSalary = 0)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                    var leaveId = new SqlParameter("@LeaveId", string.IsNullOrEmpty(LeaveId.ToString()) ? DBNull.Value : (object)LeaveId);
                    var leaveName = new SqlParameter("@LeaveName", string.IsNullOrEmpty(LeaveName) ? DBNull.Value : (object)LeaveName);
                    var noofDays = new SqlParameter("@NoofDays", string.IsNullOrEmpty(NoofDays.ToString()) ? DBNull.Value : (object)NoofDays);
                    var salarycalculationOn = new SqlParameter("@SalarycalculationOn", string.IsNullOrEmpty(SalarycalculationOn.ToString()) ? DBNull.Value : (object)SalarycalculationOn);
                    var holidayInSalary = new SqlParameter("@HolidayInSalary", string.IsNullOrEmpty(HolidayInSalary.ToString()) ? DBNull.Value : (object)HolidayInSalary);
                    int i = context.Database.ExecuteSqlCommand("USP_AddPayrollLeavsSetting  @SubscriberId, @LeaveId,@LeaveName,@NoofDays,@SalarycalculationOn,@HolidayInSalary", subscriberId, leaveId, leaveName, noofDays, salarycalculationOn, holidayInSalary);
                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return res;
        }
        public AJSolutions.Areas.PMS.Models.PayrollLeavsSettingViewModel MaxLeaveId(string subscriberID)
        {
            var leaveList = new AJSolutions.Areas.PMS.Models.PayrollLeavsSettingViewModel();
            try
            {
                using (var context = new UserDBContext())
                {
                    leaveList = db.Database
                             .SqlQuery<AJSolutions.Areas.PMS.Models.PayrollLeavsSettingViewModel>("EXEC USP_GetMaxLeavesID @SubscriberId",
                              new SqlParameter("@SubscriberId", string.IsNullOrEmpty(subscriberID) ? DBNull.Value : (object)subscriberID)).FirstOrDefault();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return leaveList;
        }

        /// <summary>
        /// Created by : Achal Kumar Jha
        /// Created on : 31-05-2017
        /// Created For :Payroll Heads Settings
        /// </summary>        
        /// <returns></returns>
        public List<PayrollHeadSettings> GetPayrollHeadAdminList(string subscriberID)
        {
            var headList = new List<PayrollHeadSettings>();
            try
            {
                using (var context = new UserDBContext())
                {
                    headList = db.Database
                             .SqlQuery<PayrollHeadSettings>("EXEC USP_GetPayrollLeavsSettings @SubscriberId",
                              new SqlParameter("@SubscriberId", string.IsNullOrEmpty(subscriberID) ? DBNull.Value : (object)subscriberID)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return headList;
        }

        public bool AddPayrollHeadSettings(string SubscriberId, DateTime DateFrom, Int16 HeadId, string HeadName, float Deduction, Int16 DeductionCriteria = 0)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                    var dateFrom = new SqlParameter("@DateFrom", DateFrom);
                    var headId = new SqlParameter("@HeadId", string.IsNullOrEmpty(HeadId.ToString()) ? DBNull.Value : (object)HeadId);
                    var headName = new SqlParameter("@HeadName", string.IsNullOrEmpty(HeadName) ? DBNull.Value : (object)HeadName);
                    var deduction = new SqlParameter("@Deduction", string.IsNullOrEmpty(Deduction.ToString()) ? DBNull.Value : (object)Deduction);
                    var deductionCriteria = new SqlParameter("@DeductionCriteria", string.IsNullOrEmpty(DeductionCriteria.ToString()) ? DBNull.Value : (object)DeductionCriteria);

                    int i = context.Database.ExecuteSqlCommand("USP_AddPayrollHeadSettings  @SubscriberId, @DateFrom, @HeadId, @HeadName, @Deduction, @DeductionCriteria", subscriberId, dateFrom, headId, headName, deduction, deductionCriteria);
                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return res;
        }
        public List<PayrollHeadSettings> GetPayrollHeadSettingList(string subscriberID, DateTime? DateFrom = null, string HeadId = null)
        {
            var headList = new List<PayrollHeadSettings>();
            try
            {
                using (var context = new UserDBContext())
                {
                    if (DateFrom == null)
                    {
                        headList = db.Database
                                 .SqlQuery<PayrollHeadSettings>("EXEC USP_GetPayrollHeadsSettingList @SubscriberId",
                                  new SqlParameter("@SubscriberId", string.IsNullOrEmpty(subscriberID) ? DBNull.Value : (object)subscriberID)).ToList();
                    }
                    else
                    {
                        headList = db.Database
                                .SqlQuery<PayrollHeadSettings>("EXEC USP_GetPayrollHeadsSettingList @SubscriberId,@DateFrom,@HeadId",
                                 new SqlParameter("@SubscriberId", string.IsNullOrEmpty(subscriberID) ? DBNull.Value : (object)subscriberID),
                                 new SqlParameter("@DateFrom", string.IsNullOrEmpty(DateFrom.ToString()) ? DBNull.Value : (object)DateFrom),
                                 new SqlParameter("@HeadId", string.IsNullOrEmpty(HeadId.ToString()) ? DBNull.Value : (object)HeadId)).ToList();
                    }
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return headList;
        }
        public string GeneratePassword(int length)
        {
            return GetRandomAlphanumericString(length);
            //return Name.Substring(0, 1).Replace(' ', '_').ToUpper() + GetRandomSymbol(Name.Length) + Name.Length.ToString() + Name.Substring(1, 2).Replace(' ', '_').ToLower() + GetRandomSymbol(Name.Length + 3);
        }
        public static string GetRandomAlphanumericString(int length)
        {
            const string alphanumericCharacters =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789";
            return GetRandomString(length, alphanumericCharacters);
        }
        public static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");

            var bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }

        //By : Ajay Kumar Choudhary
        //On : 25-07-2017
        //Reason : For Plan & Pricing

        public bool AddPlan(int PlanId, string PlanName, string ShortDescription, short PlanSequence, DateTime CommenceMentDate, DateTime? RetiredDate, double INRAmount, double INRDiscount, double OtherAmount, double OtherDiscount, DateTime UpdatedOn, string UpdatedBy)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var planId = new SqlParameter("@PlanId", PlanId);
                    var planName = new SqlParameter("@PlanName", string.IsNullOrEmpty(PlanName) ? DBNull.Value : (object)PlanName);
                    var shortDescription = new SqlParameter("@ShortDescription", string.IsNullOrEmpty(ShortDescription) ? DBNull.Value : (object)ShortDescription);
                    var planSequence = new SqlParameter("@PlanSequence", PlanSequence);
                    var commencementDate = new SqlParameter("@CommenceMentDate", CommenceMentDate);
                    var retiredDate = new SqlParameter("@RetiredDate", string.IsNullOrEmpty(RetiredDate.ToString()) ? DBNull.Value : (object)RetiredDate);
                    var iNRAmount = new SqlParameter("@INRAmount", INRAmount);
                    var iNRDiscount = new SqlParameter("@INRDiscount", INRDiscount);
                    var otherAmount = new SqlParameter("@OtherAmount", OtherAmount);
                    var otherDiscount = new SqlParameter("@OtherDiscount", OtherDiscount);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", UpdatedBy);

                    int i = context.Database.ExecuteSqlCommand("USP_AddPlans @PlanId, @PlanName, @ShortDescription, @PlanSequence, @CommenceMentDate, @RetiredDate, @INRAmount, @INRDiscount, @OtherAmount, @OtherDiscount, @UpdatedOn, @UpdatedBy",
                                                                              planId, planName, shortDescription, planSequence, commencementDate, retiredDate, iNRAmount, iNRDiscount, otherAmount, otherDiscount, updatedOn, updatedBy);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            {
            }
            return res;
        }

        public void DeletePlan(int PlanId)
        {
            var plans = db.Plan.Find(PlanId);
            if (plans != null)
            {
                db.Plan.Remove(plans);
                db.SaveChanges();
            }
        }

        public bool AddPlanAddOns(int AddOnId, string AddOnName, double INRAmount, double INRDiscount, double OtherAmount, double OtherDiscount)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var addOnId = new SqlParameter("@AddOnId", AddOnId);
                    var addOnName = new SqlParameter("@AddOnName", string.IsNullOrEmpty(AddOnName) ? DBNull.Value : (object)AddOnName);
                    var iNRAmount = new SqlParameter("@INRAmount", INRAmount);
                    var iNRDiscount = new SqlParameter("@INRDiscount", INRDiscount);
                    var otherAmount = new SqlParameter("@OtherAmount", OtherAmount);
                    var otherDiscount = new SqlParameter("@OtherDiscount", OtherDiscount);

                    int i = context.Database.ExecuteSqlCommand("USP_AddPlanAddons @AddOnId, @AddOnName, @INRAmount, @INRDiscount, @OtherAmount, @OtherDiscount",
                                                                              addOnId, addOnName, iNRAmount, iNRDiscount, otherAmount, otherDiscount);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            {
            }
            return res;
        }

        public void DeletePlanAddons(int AddOnId)
        {
            var planaddons = db.PlanAddOns.Find(AddOnId);
            if (planaddons != null)
            {
                db.PlanAddOns.Remove(planaddons);
                db.SaveChanges();
            }
        }

        public bool AddFeatures(Int64 FeatureId, string Feature, short FeatureSequence)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var featureId = new SqlParameter("@FeatureId", FeatureId);
                    var feature = new SqlParameter("@Feature", string.IsNullOrEmpty(Feature) ? DBNull.Value : (object)Feature);
                    var featureSequence = new SqlParameter("@FeatureSequence", FeatureSequence);

                    int i = context.Database.ExecuteSqlCommand("USP_AddFeature @FeatureId, @Feature, @FeatureSequence", featureId, feature, featureSequence);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            {

            }
            return res;
        }

        public bool AddPlanFeatures(int PlanId, Int64 FeatureId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var planId = new SqlParameter("@PlanId", PlanId);
                    var featureId = new SqlParameter("@FeatureId", FeatureId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddPlanFeature @PlanId, @FeatureId", planId, featureId);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            {

            }
            return res;
        }

        public List<AssessmentTrainingView> GetBatchAssessments(Int64 BatchId)
        {
            List<AssessmentTrainingView> TrainAsst = new List<AssessmentTrainingView>();
            TrainAsst = db.Database
                               .SqlQuery<AssessmentTrainingView>("EXEC USP_GetBatchAssessments @BatchId",
                                new SqlParameter("@BatchId", BatchId)).ToList();


            return TrainAsst;
        }
        public List<PlanFeaturesView> GetPlanFeatures()
        {
            var planfeaturesList = new List<PlanFeaturesView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    planfeaturesList = db.Database.SqlQuery<PlanFeaturesView>("EXEC USP_GeTPlanFeatures").ToList();

                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return planfeaturesList;
        }

        public void DeleteFeatures(Int64 FeatureId)
        {
            var features = db.Features.Find(FeatureId);
            if (features != null)
            {
                db.Features.Remove(features);
                db.SaveChanges();
            }
        }

        public string GetReferenceId()
        {
            UserDBContext udbc = new UserDBContext();
            string year = Convert.ToString(DateTime.UtcNow.Year).Substring(2);
            string quarter = "R1";
            int month = DateTime.UtcNow.Month;
            if (month > 3 && month <= 6)
            {
                quarter = "R2";
            }
            else if (month > 6 && month <= 9)
            {
                quarter = "R3";
            }
            else if (month > 9 && month <= 12)
            {
                quarter = "R4";
            }

            string ReferenceId = "RA" + year + quarter + "000001";

            var References = from s in udbc.PaymentTransaction.Where(s => s.ReferenceId.Substring(0, 6) == "RA" + year + quarter)
                             orderby s.ReferenceId descending
                             select s.ReferenceId;

            var Reference = References.FirstOrDefault();

            if (Reference != null)
            {
                string ReferencePartialId = Reference.Substring(7);
                int lastVal = Convert.ToInt32(ReferencePartialId);
                lastVal = lastVal + 1;
                string suffix = string.Empty;

                for (int i = Convert.ToString(lastVal).Length; i < 6; i++)
                {
                    suffix = suffix + "0";
                }

                ReferenceId = Reference.Substring(0, 6) + suffix + Convert.ToString(lastVal);
            }
            return ReferenceId;
        }

        public bool AddRegistrationDetails(Int64 TransactionId, string UserPlanId, string CorporateId, string ReferenceId, string Currency, double Amount, string Comments, string PayeeName,
            string PayeeEmail, string PayeePhoneNumber, DateTime PaymentDate, string Status, string TransactionReferenceNumber, string BankCode, string PGComment,
            string ClientTxnRefNo, string TSPLTxnId, string txn_status, string txn_msg)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var transactionId = new SqlParameter("@TransactionId", TransactionId);
                    var userPlanId = new SqlParameter("@UserPlanId", string.IsNullOrEmpty(UserPlanId) ? DBNull.Value : (object)UserPlanId);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var referenceId = new SqlParameter("@ReferenceId", string.IsNullOrEmpty(ReferenceId) ? DBNull.Value : (object)ReferenceId);
                    var currency = new SqlParameter("@Currency", string.IsNullOrEmpty(Currency) ? DBNull.Value : (object)Currency);
                    var amount = new SqlParameter("@Amount", Amount);
                    var comments = new SqlParameter("@Comments", string.IsNullOrEmpty(Comments) ? DBNull.Value : (object)Comments);
                    var payeeName = new SqlParameter("@PayeeName", string.IsNullOrEmpty(PayeeName) ? DBNull.Value : (object)PayeeName);
                    var payeeEmail = new SqlParameter("@PayeeEmail", string.IsNullOrEmpty(PayeeEmail) ? DBNull.Value : (object)PayeeEmail);
                    var payeePhoneNumber = new SqlParameter("@PayeePhoneNumber", string.IsNullOrEmpty(PayeePhoneNumber) ? DBNull.Value : (object)PayeePhoneNumber);
                    var paymentDate = new SqlParameter("@PaymentDate", PaymentDate);
                    var status = new SqlParameter("@Status", string.IsNullOrEmpty(Status) ? DBNull.Value : (object)Status);
                    var transactionReferenceNumber = new SqlParameter("@TransactionReferenceNumber", string.IsNullOrEmpty(TransactionReferenceNumber) ? DBNull.Value : (object)TransactionReferenceNumber);
                    var bankCode = new SqlParameter("@BankCode", string.IsNullOrEmpty(BankCode) ? DBNull.Value : (object)BankCode);
                    var pGComment = new SqlParameter("@PGComment", string.IsNullOrEmpty(PGComment) ? DBNull.Value : (object)PGComment);
                    var clientTxnRefNo = new SqlParameter("@ClientTxnRefNo", string.IsNullOrEmpty(ClientTxnRefNo) ? DBNull.Value : (object)ClientTxnRefNo);
                    var tSPLTxnId = new SqlParameter("@TSPLTxnId", string.IsNullOrEmpty(TSPLTxnId) ? DBNull.Value : (object)TSPLTxnId);
                    var Txn_status = new SqlParameter("@txn_status", string.IsNullOrEmpty(txn_status) ? DBNull.Value : (object)txn_status);
                    var Txn_msg = new SqlParameter("@txn_msg", string.IsNullOrEmpty(txn_msg) ? DBNull.Value : (object)txn_msg);


                    int i = context.Database.ExecuteSqlCommand("USP_AddRegistrationDetails @TransactionId, @UserPlanId , @CorporateId , @ReferenceId , @Currency , @Amount , @Comments , @PayeeName , @PayeeEmail, @PayeePhoneNumber, @PaymentDate, @Status, @TransactionReferenceNumber, @BankCode, @PGComment, @ClientTxnRefNo, @TSPLTxnId, @txn_status, @txn_msg",
                                                                transactionId, userPlanId, corporateId, referenceId, currency, amount, comments, payeeName, payeeEmail, payeePhoneNumber, paymentDate, status, transactionReferenceNumber, bankCode, pGComment, @clientTxnRefNo, @tSPLTxnId, @Txn_status, @Txn_msg);

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

        #region Payment Gateway Functions




        public string PaymentRequest(string MerchantTxnRefNo, string PayeeName, string Amount, string EmailAddress, string MobileNumber, string Comment, string strReturnUrl)
        {

            string shoppingCartDetail = strSchemeCode + "_" + Amount + "_0.0";

            string result = PostRequest("T", strMerchantCode, MerchantTxnRefNo, PayeeName, Amount, Comment, "INR", strReturnUrl, shoppingCartDetail, DateTime.UtcNow, EmailAddress, MobileNumber, PayeeName, strKEY, strIV);

            return result;
        }

        public string PostRequest(string RequestType, string MerchantCode, string MerchantTxnRefNo, string PayeeName, string Amount, string Comments, string CurrencyCode, string ReturnUrl,
                                    string ShoppingCartDetails, DateTime PaymentDate, string Email, string MobileNumber, string CustomerName, string Key, string IV)
        {
            RequestURL objRequestURL = new RequestURL();

            String response = objRequestURL.SendRequest
                                           (
                                                  RequestType
                                                , MerchantCode
                                                , MerchantTxnRefNo
                                                , PayeeName
                                                , Amount
                                                , CurrencyCode
                                                , MerchantTxnRefNo
                                                , ReturnUrl
                                                , ""
                                                , ""
                                                , ShoppingCartDetails
                                                , PaymentDate.ToString("dd-MM-yyyy")
                                                , Email
                                                , MobileNumber
                                                , ""
                                                , PayeeName
                                                , ""
                                                , ""
                                                , Key
                                                , IV
                                                );

            return response;
            //else
            //{
            //    if (RequestType.ToUpper() == "T")
            //    {

            //        Response.Write("<form name='s1_2' id='s1_2' action='" + response + "' method='post'> ");

            //        Response.Write("<script type='text/javascript' language='javascript' >document.getElementById('s1_2').submit();");

            //        Response.Write("</script>");
            //        Response.Write("<script language='javascript' >");
            //        Response.Write("</script>");
            //        Response.Write("</form> ");

            //    }

        }







        #region Variable Declaration
        string strPG_TxnStatus = string.Empty,
        strPG_ClintTxnRefNo = string.Empty,
                strPG_TPSLTxnBankCode = string.Empty,
                strPG_TPSLTxnID = string.Empty,
                strPG_TxnAmount = string.Empty,
                strPG_TxnDateTime = string.Empty,
                strPG_TxnDate = string.Empty,
                strPG_TxnTime = string.Empty,
                strPG_TxnMsg = string.Empty;

        string[] strSplitDecryptedResponse;
        string[] strArrPG_TxnDateTime;

        #endregion


        public string ConfirmTransaction(string PGResponse)
        {
            RequestURL objRequestURL = new RequestURL();

            string strDecryptedVal = objRequestURL.VerifyPGResponse(PGResponse, strKEY, strIV);

            if (strDecryptedVal.StartsWith("ERROR"))
            {
                return strPG_ClintTxnRefNo + "|" + "Transaction Fail :: <br/>" + "Response :: <br/>" + strDecryptedVal;
            }
            else
            {
                strSplitDecryptedResponse = strDecryptedVal.Split('|');
                GetPGRespnseData(strSplitDecryptedResponse, strDecryptedVal);

                if (strPG_TxnStatus == "0300")
                {
                    return strPG_ClintTxnRefNo + "|" + "Transaction Success  " + strPG_TxnStatus + "|" + strPG_TxnMsg;
                }
                else
                {
                    return strPG_ClintTxnRefNo + "|" + "Transaction Fail :: <br/>" + "Response :: <br/>"
            + strDecryptedVal;
                }
            }
        }

        public void GetPGRespnseData(string[] parameters, string pgResponsedValue)
        {
            string[] strGetMerchantParamForCompare;
            for (int i = 0; i < parameters.Length; i++)
            {
                strGetMerchantParamForCompare = parameters[i].ToString().Split('=');
                if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "TXN_STATUS")
                {
                    strPG_TxnStatus = Convert.ToString(strGetMerchantParamForCompare[1]);
                }
                else if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "CLNT_TXN_REF")
                {
                    strPG_ClintTxnRefNo = Convert.ToString(strGetMerchantParamForCompare[1]);
                }
                else if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "TPSL_BANK_CD")
                {
                    strPG_TPSLTxnBankCode = Convert.ToString(strGetMerchantParamForCompare[1]);
                }
                else if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "TPSL_TXN_ID")
                {
                    strPG_TPSLTxnID = Convert.ToString(strGetMerchantParamForCompare[1]);
                }
                else if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "TXN_AMT")
                {
                    strPG_TxnAmount = Convert.ToString(strGetMerchantParamForCompare[1]);
                }
                else if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "TXN_MSG")
                {
                    strPG_TxnMsg = Convert.ToString(strGetMerchantParamForCompare[1]);
                }
                else if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "TPSL_TXN_TIME")
                {
                    strPG_TxnDateTime = Convert.ToString(strGetMerchantParamForCompare[1]);
                    strArrPG_TxnDateTime = strPG_TxnDateTime.Split(' ');
                    strPG_TxnDate = Convert.ToString(strArrPG_TxnDateTime[0]);
                    strPG_TxnTime = Convert.ToString(strArrPG_TxnDateTime[1]);
                }
            }

            Student student = new Student();

            DateTime? strPGTxnDate = null;
            if (!string.IsNullOrEmpty(strPG_TxnDateTime))
                strPGTxnDate = Convert.ToDateTime(strPG_TxnDateTime);

            var studentFee = db.PaymentTransaction.Where(c => c.ReferenceId == strPG_ClintTxnRefNo).FirstOrDefault();
            if (studentFee != null)
                //AddRegistrationDetails(strPG_ClintTxnRefNo, studentFee.CorporateId, Convert.ToSingle(strPG_TxnAmount), studentFee.CourseCode, studentFee.TransactionDate, strPGTxnDate, studentFee.PaymentModeId, studentFee.BankName, strPG_TPSLTxnID, "Succeeded", strPG_TPSLTxnBankCode, studentFee.Remarks, pgResponsedValue);

                if (strPG_TxnMsg == "success")
                {
                    AddRegistrationDetails(studentFee.TransactionId, studentFee.UserPlanId, studentFee.CorporateId, studentFee.ReferenceId, studentFee.Currency, Convert.ToSingle(strPG_TxnAmount), studentFee.Comments, studentFee.PayeeName, studentFee.PayeeEmail, studentFee.PayeePhoneNumber, studentFee.PaymentDate, "Succeeded", strPG_ClintTxnRefNo, strPG_TPSLTxnBankCode, pgResponsedValue, strPG_ClintTxnRefNo, strPG_TPSLTxnID, strPG_TxnStatus, strPG_TxnMsg);
                }
                else
                {
                    AddRegistrationDetails(studentFee.TransactionId, studentFee.UserPlanId, studentFee.CorporateId, studentFee.ReferenceId, studentFee.Currency, Convert.ToSingle(strPG_TxnAmount), studentFee.Comments, studentFee.PayeeName, studentFee.PayeeEmail, studentFee.PayeePhoneNumber, studentFee.PaymentDate, "Failed", strPG_ClintTxnRefNo, strPG_TPSLTxnBankCode, pgResponsedValue, strPG_ClintTxnRefNo, strPG_TPSLTxnID, strPG_TxnStatus, strPG_TxnMsg);
                }
        }

        #endregion

        //By : Rahul Newara
        //On : 09-08-2017
        //Reason : Batchwish training Summary Report

        public DataTable GetAssessmentReport(Int64 BatchId, string TrainingId = "")
        {
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@BatchId", BatchId));
            cmd.Parameters.Add(new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId));
            cmd.Connection = thisConnection;
            string MyDataSource = "USP_GetAssessmentReport";
            cmd.CommandText = string.Format(MyDataSource);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            DataTable Dt = new DataTable();
            daN.Fill(Dt);
            return Dt;
        }

        public DataTable GetCandidateFeeDetails(string UserId, Int64 BatchId)
        {
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId));
            cmd.Parameters.Add(new SqlParameter("@BatchId", BatchId));
            cmd.Connection = thisConnection;
            string MyDataSource = "USP_GetCandidateFeeDetails";

            cmd.CommandText = string.Format(MyDataSource);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            DataTable Dt = new DataTable();
            daN.Fill(Dt);
            return Dt;
        }

        public List<TrainingScheduleView> GetTrainingSchedulesBatch(Int64 BatchId)
        {
            var SchedulesBatch = new List<TrainingScheduleView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    SchedulesBatch = db.Database
                             .SqlQuery<TrainingScheduleView>("EXEC USP_GetTrainingSchedulesBatchWise @BatchId",
                              new SqlParameter("@BatchId", BatchId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return SchedulesBatch;
        }

        public List<TrainingScheduleView> GetTrainingSchedulesForClients(string CorporateId)
        {
            var SchedulesBatch = new List<TrainingScheduleView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    SchedulesBatch = db.Database
                             .SqlQuery<TrainingScheduleView>("EXEC USP_GetTrainingForClients @CorporateId",
                              new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return SchedulesBatch;
        }



        public List<TrainingScheduleCityView> GetTrainingSchedulesCitiesForClients(string CorporateId)
        {
            var SchedulesBatch = new List<TrainingScheduleCityView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    SchedulesBatch = db.Database
                             .SqlQuery<TrainingScheduleCityView>("EXEC USP_GetCitiesOfTrainingForClient @CorporateId",
                              new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return SchedulesBatch;
        }

        public List<TrainingScheduleCityView> GetTrainingSchedulesForCities(string SubscriberId)
        {
            var SchedulesBatch = new List<TrainingScheduleCityView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    SchedulesBatch = db.Database
                             .SqlQuery<TrainingScheduleCityView>("EXEC USP_GetCitiesOfTraining @SubscriberId",
                              new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return SchedulesBatch;
        }
        //public CourseMasterViewModel GetIntegratedCourse(string CourseCode)
        //{
        //    CourseMasterViewModel CourseMaster
        //}

        /// <summary>
        /// Create By: Vikash Das
        /// Created On: 10-08-2017
        /// 
        /// </summary>
        /// <returns></returns>
        public List<LetterFieldNameViewModel> LetterFieldName()
        {
            List<LetterFieldNameViewModel> PlaceholderDetail = new List<LetterFieldNameViewModel>();
            using (var db = new UserDBContext())
            {
                PlaceholderDetail = db.Database
             .SqlQuery<LetterFieldNameViewModel>("exec LetterFieldName").ToList();
            }
            return PlaceholderDetail;
        }

        /// <summary>
        /// CreatedBy : Vikash Das
        /// CreatedOn : 06-08-2017
        /// Purpose   : Add the letter Header
        /// </summary>
        /// <param name="HeaderTitle"></param>
        /// <param name="SameAsCompanyLogo"></param>
        /// <param name="CorporateId"></param>
        /// <returns></returns>
        //public bool AddLetterHeader(LetterHeader letterheader, string CorporateId)
        //{
        //    bool res = false;
        //    try
        //    {
        //        using (var context = new UserDBContext())
        //        {
        //            var headerId = new SqlParameter("@HeaderId", letterheader.HeaderId);
        //            var headerName = new SqlParameter("@HeaderName", string.IsNullOrEmpty(letterheader.HeaderName) ? DBNull.Value : (object)letterheader.HeaderName);
        //            var headerTitle = new SqlParameter("@HeaderTitle", string.IsNullOrEmpty(letterheader.HeaderTitle) ? DBNull.Value : (object)letterheader.HeaderTitle);
        //            var sameAsCompanyLogo = new SqlParameter("@SameAsCompanyLogo", letterheader.SameAsCompanyLogo);
        //            var corporateId = new SqlParameter("CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
        //            int i = context.Database.ExecuteSqlCommand("USP_AddLetterHeader @HeaderId,@HeaderName, @HeaderTitle, @SameAsCompanyLogo, @CorporateId ",
        //                            headerId, headerName, headerTitle, sameAsCompanyLogo, corporateId);
        //            if (i > 0)
        //                res = true;
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    return res;
        //}

        /// <summary>
        /// upload Document in Blob
        /// </summary>
        /// <param name="CorporateId"></param>
        /// <param name="ContentType"></param>
        /// <param name="FileName"></param>
        /// <param name="upload"></param>
        /// <param name="HeaderId"></param>
        /// <returns></returns>
        public string uploadLetterLogoAttachmentFile(string CorporateId, HttpPostedFileBase upload, Int64 TemplateId = 0)
        {
            string res = "Failure";
            if (upload != null && upload.ContentLength > 0)
            {
                if (TemplateId == 0)
                {
                    TemplateId = db.CorporateTemplate.Where(c => c.CorporateId == CorporateId).Max(c => c.TemplateId);
                }
                var file = db.LetterLogoAttachment.Where(d => d.TemplateId == TemplateId).FirstOrDefault();
                if (file != null)
                {
                    Int64 FileId = file.FileId;
                    blobManager.DeleteBlob(CorporateId.ToLower(), GetLogoFileName(FileId).ToLower());
                    db.LetterLogoAttachment.Remove(file);
                    db.SaveChanges();
                }
                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");
                bool added = AddDocument(upload.FileName.ToLower(), upload.ContentType, CorporateId, TemplateId);
                if (added)
                {
                    string fileName = ReplaceLetterFileName(CorporateId, TemplateId);
                    blobManager.UploadBlob(CorporateId.ToLower(), fileName.ToLower(), upload);
                }
                res = "Succeed";
            }
            return res;
        }

        /// <summary>
        /// CreateBy : Vikash Das
        /// CreatedOn : 11-08-2017
        /// Purpose :Get File Name For Letter Header
        /// </summary>
        /// <param name="FileId"></param>
        /// <returns></returns>
        public string GetLogoFileName(Int64 FileId)
        {
            string fileName = null;
            var png = db.LetterLogoAttachment.Find(FileId);

            png.FileName = png.FileName.Replace("'", "_");
            png.FileName = png.FileName.Replace(' ', '_');

            if (png != null)
            {
                fileName = "LetterLogoAttachment/" + png.FileId + "/" + png.FileName;
            } return fileName;
        }

        /// <summary>
        /// Add the LetterLogoAttachment
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="ContentType"></param>
        /// <param name="CorporateId"></param>
        /// <param name="HeaderId"></param>
        /// <returns></returns>
        public bool AddDocument(string FileName, string ContentType, string CorporateId, Int64 TemplateId = 0)
        {
            bool res = false;
            Int64 FileId = 0;
            try
            {
                using (var context = new UserDBContext())
                {
                    var fileId = new SqlParameter("@FileId", FileId);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var templateId = new SqlParameter("@TemplateId", TemplateId);
                    int i = context.Database.ExecuteSqlCommand("USP_AddLetterLogoAttachment @FileId, @FileName,@ContentType, @CorporateId, @TemplateId",
                                                                fileId, fileName, contentType, corporateId, templateId);
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

        //public bool AddLetterContent(LetterContent lettercontent, string CorporateId)
        //{
        //    bool res = false;
        //    try
        //    {
        //        using (var context = new UserDBContext())
        //        {
        //            var contentId = new SqlParameter("@ContentId", lettercontent.ContentId);
        //            var contentName = new SqlParameter("@ContentName", string.IsNullOrEmpty(lettercontent.ContentName) ? DBNull.Value : (object)lettercontent.ContentName);
        //            var content = new SqlParameter("@Content", string.IsNullOrEmpty(lettercontent.Content) ? DBNull.Value : (object)lettercontent.Content);
        //            var corporateId = new SqlParameter("CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
        //            int i = context.Database.ExecuteSqlCommand("USP_AddLetterContents @ContentId, @ContentName, @Content, @CorporateId ",
        //                            contentId, contentName, content, corporateId);
        //            if (i > 0)
        //                res = true;
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    return res;
        //}

        public List<LetterDesignView> GetLetterContent()
        {
            var letterContent = new List<LetterDesignView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    letterContent = db.Database.SqlQuery<LetterDesignView>("EXEC USP_GetLetterContent").ToList();
                    //new SqlParameter("@ContentId", ContentId)
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return letterContent;
        }

        public bool AddCorporateTemplate(CorporateTemplate corporatetemplate, string CorporateId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var templateId = new SqlParameter("@TemplateId", corporatetemplate.TemplateId);
                    var templateName = new SqlParameter("@Name", string.IsNullOrEmpty(corporatetemplate.Name) ? DBNull.Value : (object)corporatetemplate.Name);
                    var header = new SqlParameter("@Header", string.IsNullOrEmpty(corporatetemplate.Header) ? DBNull.Value : (object)corporatetemplate.Header);
                    var footer = new SqlParameter("@Footer", string.IsNullOrEmpty(corporatetemplate.Footer) ? DBNull.Value : (object)corporatetemplate.Footer);
                    var content = new SqlParameter("@Content", string.IsNullOrEmpty(corporatetemplate.Content) ? DBNull.Value : (object)corporatetemplate.Content);
                    var sameasCompanyLogo = new SqlParameter("@SameAsCompanyLogo", corporatetemplate.SameAsCompanyLogo);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var letterTypeId = new SqlParameter("@LetterTypeId", corporatetemplate.LetterTypeId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCorporateTemplate @TemplateId," +
                            " @Name, @Header, @Footer, @Content, @SameAsCompanyLogo, @CorporateId, @LetterTypeId ",
                            templateId, templateName, header, footer, content, sameasCompanyLogo, corporateId, letterTypeId);

                    if (i > 0)
                        res = true;
                }
            }
            catch
            { }
            return res;
        }

        public bool AddCorporateLetter(CorporateLetter cletter, string CorporateId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var letterId = new SqlParameter("@LetterId", cletter.LetterId);
                    var letterName = new SqlParameter("@Name", string.IsNullOrEmpty(cletter.Name) ? DBNull.Value : (object)cletter.Name);
                    var templateId = new SqlParameter("@TemplateId", cletter.TemplateId);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(cletter.UserId) ? DBNull.Value : (object)cletter.UserId);
                    var headerDate = new SqlParameter("@HeaderDate", DateTime.Now);
                    var footerDate = new SqlParameter("@FooterDate", DateTime.Now);
                    var status = new SqlParameter("@Status", "success");
                    var letterTypeId = new SqlParameter("@LetterTypeId", cletter.LetterTypeId);
                    var referenceNo = new SqlParameter("@ReferenceNo", string.IsNullOrEmpty(cletter.ReferenceNo) ? DBNull.Value : (object)cletter.ReferenceNo);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var createBy = new SqlParameter("@CreatedBy", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var createdOn = new SqlParameter("@CreatedOn", DateTime.Now);
                    int i = context.Database.ExecuteSqlCommand("USP_AddCorporateLetter @LetterId," +
                            " @Name, @TemplateId, @UserId, @HeaderDate, @FooterDate ," +
                            " @Status, @LetterTypeId, @ReferenceNo, @CorporateId, @CreatedBy ,@CreatedOn",
                             letterId, letterName, templateId, userId, headerDate, footerDate
                             , status, letterTypeId, referenceNo, corporateId, createBy, createdOn);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            {
                res = false;
            }
            return res;
        }

        //method for addemployee
        //created by:preeti singh
        //date:20/08/2017
        public bool AddEmployee(EmployeeView emp, string Id, string UpdatedBy)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(emp.UserId) ? DBNull.Value : (object)emp.UserId);
                    var employeeId = new SqlParameter("@EmployeeId", string.IsNullOrEmpty(emp.EmployeeId) ? DBNull.Value : (object)emp.EmployeeId);
                    var emplanelled = new SqlParameter("@Emplanelled", emp.Emplanelled);
                    var name = new SqlParameter("@Name", string.IsNullOrEmpty(emp.Name) ? DBNull.Value : (object)emp.Name);
                    var dob = new SqlParameter("@DOB", string.IsNullOrEmpty(emp.DOB.ToString()) ? DBNull.Value : (object)emp.DOB);
                    var gender = new SqlParameter("@Gender", string.IsNullOrEmpty(emp.Gender) ? DBNull.Value : (object)emp.Gender);
                    var maritalStatus = new SqlParameter("@MaritalStatus", string.IsNullOrEmpty(emp.MaritalStatus) ? DBNull.Value : (object)emp.MaritalStatus);
                    var alternateContact = new SqlParameter("@AlternateContact", string.IsNullOrEmpty(emp.AlternateContact) ? DBNull.Value : (object)emp.AlternateContact);
                    var alternateEmail = new SqlParameter("@AlternateEmail", string.IsNullOrEmpty(emp.AlternateEmail) ? DBNull.Value : (object)emp.AlternateEmail);
                    var nationality = new SqlParameter("@Nationality", string.IsNullOrEmpty(emp.Nationality) ? DBNull.Value : (object)emp.Nationality);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(Id) ? DBNull.Value : (object)Id);
                    var departmentId = new SqlParameter("@DepartmentId", string.IsNullOrEmpty(emp.DepartmentId) ? DBNull.Value : (object)emp.DepartmentId);
                    var managerLevel = new SqlParameter("@ManagerLevel", emp.ManagerLevel);
                    var reportingAuthority = new SqlParameter("@ReportingAuthority", string.IsNullOrEmpty(emp.ReportingAuthority) ? DBNull.Value : (object)emp.ReportingAuthority);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var updatedOn = new SqlParameter("@UpdatedOn", DateTime.UtcNow);
                    var deactivated = new SqlParameter("@Deactivated", emp.Deactivated);
                    var fatherName = new SqlParameter("@FatherName", string.IsNullOrEmpty(emp.FatherName) ? DBNull.Value : (object)emp.FatherName);
                    var spouseName = new SqlParameter("@SpouseName", string.IsNullOrEmpty(emp.SpouseName) ? DBNull.Value : (object)emp.SpouseName);
                    var emergencyContactName = new SqlParameter("@EmergencyContactName", string.IsNullOrEmpty(emp.EmergencyContactName) ? DBNull.Value : (object)emp.EmergencyContactName);
                    var emergencyContactNumber = new SqlParameter("@EmergencyContactNumber", string.IsNullOrEmpty(emp.EmergencyContactNumber) ? DBNull.Value : (object)emp.EmergencyContactNumber);
                    var bloodGroup = new SqlParameter("@BloodGroup", string.IsNullOrEmpty(emp.BloodGroup) ? DBNull.Value : (object)emp.BloodGroup);
                    var physicallyChallenged = new SqlParameter("@PhysicallyChallenged", emp.PhysicallyChallenged);
                    var location = new SqlParameter("@Location", string.IsNullOrEmpty(emp.Location) ? DBNull.Value : (object)emp.Location);
                    var marriageDate = new SqlParameter("@MarriageDate", string.IsNullOrEmpty(emp.MarriageDate.ToString()) ? DBNull.Value : (object)emp.MarriageDate);
                    var designationId = new SqlParameter("@DesignationId", string.IsNullOrEmpty(emp.DesignationId.ToString()) ? DBNull.Value : (object)emp.DesignationId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddEmployee @UserId, @EmployeeId,@Emplanelled, @Name, @DOB, " +
                        "@Gender, @MaritalStatus,@AlternateContact, @AlternateEmail,@Nationality,@SubscriberId,@DepartmentId," +
                        "@ManagerLevel,@ReportingAuthority, @UpdatedBy, @UpdatedOn, @Deactivated ,@FatherName,@SpouseName," +
                        "@EmergencyContactName,@EmergencyContactNumber, @BloodGroup,@PhysicallyChallenged,@Location,@MarriageDate,@DesignationId",
                        userId, employeeId, emplanelled, name, dob, gender, maritalStatus, alternateContact, alternateEmail, nationality, subscriberId, departmentId,
                        managerLevel, reportingAuthority, updatedBy, updatedOn, deactivated, fatherName, spouseName, emergencyContactName,
                        emergencyContactNumber, bloodGroup, physicallyChallenged, location, marriageDate, designationId);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            { }
            return res;
        }
        //end

        /// <summary>
        /// method for addjoiningdetails
        /// created by:preeti singh
        /// date:22/08/2017
        /// </summary>
        /// <param name="TrainingId"></param>
        /// <returns></returns>
        public bool EmployeeJoining(EmpJoiningDetail empjoining)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(empjoining.UserId) ? DBNull.Value : (object)empjoining.UserId);
                    var joiningId = new SqlParameter("@JoiningId", empjoining.JoiningId);
                    var joiningDate = new SqlParameter("@JoiningDate", empjoining.JoiningDate);
                    var confirmationDate = new SqlParameter("@ConfirmationDate", DBNull.Value);
                    if (empjoining.ConfirmationDate != null)
                        confirmationDate = new SqlParameter("@ConfirmationDate", empjoining.ConfirmationDate);

                    var probationPeriod = new SqlParameter("@ProbationPeriod", empjoining.ProbationPeriod);
                    var statusId = new SqlParameter("@StatusId", empjoining.StatusId);
                    var noticePeriod = new SqlParameter("@NoticePeriod", empjoining.NoticePeriod);
                    var gradeId = new SqlParameter("@GradeId", empjoining.GradeId);
                    var workLocation = new SqlParameter("@BranchId", empjoining.BranchId);
                    var shiftId = new SqlParameter("@ShiftId", empjoining.ShiftId);
                    var schemeId = new SqlParameter("@SchemeId", empjoining.SchemeId);

                    int i = context.Database.ExecuteSqlCommand("USP_EmpJoiningDetail @UserId, @JoiningId,@JoiningDate, @ConfirmationDate, @ProbationPeriod, " +
                        "@StatusId, @NoticePeriod,@GradeId, @BranchId,@ShiftId,@SchemeId",
                        userId, joiningId, joiningDate, confirmationDate, probationPeriod, statusId, noticePeriod, gradeId, workLocation, shiftId, schemeId);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            { }
            return res;
        }
        /// <summary>
        /// Developed by: Vishnu Kalihari on date: 18/04/2018
        /// Add / Update instructor lead data
        /// </summary>
        /// <param name="instructor">It is object should contain all instructor related parameters</param>
        /// <returns>True if data added successfully</returns>

        public bool AddTrainerLead(InstructorLeadProfileView instructor)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {

                    var instructorId = new SqlParameter("@InstructorId", string.IsNullOrEmpty(instructor.InstructorId) ? DBNull.Value : (object)instructor.InstructorId);
                    var Name = new SqlParameter("@Name", string.IsNullOrEmpty(instructor.Name) ? DBNull.Value : (object)instructor.Name);
                    var countryId = new SqlParameter("@CountryId", instructor.CountryId);
                    var stateId = new SqlParameter("@StateId", instructor.StateId);
                    var cityId = new SqlParameter("@CityId", instructor.CityId);

                    var zone = new SqlParameter("@Zone", string.IsNullOrEmpty(instructor.Zone) ? DBNull.Value : (object)instructor.Zone);

                    var qualification = new SqlParameter("@Qualification", string.IsNullOrEmpty(instructor.Qualification) ? DBNull.Value : (object)instructor.Qualification);

                    var experience = new SqlParameter("@Experience", instructor.Experience);

                    var domainExpertize = new SqlParameter("@DomainExpertize", string.IsNullOrEmpty(instructor.DomainExpertize) ? DBNull.Value : (object)instructor.DomainExpertize);

                    var organization = new SqlParameter("@Organization", string.IsNullOrEmpty(instructor.Organization) ? DBNull.Value : (object)instructor.Organization);

                    var languageKnown = new SqlParameter("@LanguageKnown", string.IsNullOrEmpty(instructor.LanguageKnown) ? DBNull.Value : (object)instructor.LanguageKnown);

                    var specialization = new SqlParameter("@Specialization", string.IsNullOrEmpty(instructor.Specialization) ? DBNull.Value : (object)instructor.Specialization);

                    var nibfProject = new SqlParameter("@NibfProject", string.IsNullOrEmpty(instructor.NibfProject) ? DBNull.Value : (object)instructor.NibfProject);

                    var trainingLocation = new SqlParameter("@TrainingLocation", string.IsNullOrEmpty(instructor.TrainingLocation) ? DBNull.Value : (object)instructor.TrainingLocation);

                    var remarks = new SqlParameter("@Remarks", string.IsNullOrEmpty(instructor.Remarks) ? DBNull.Value : (object)instructor.Remarks);

                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(instructor.SubscriberId) ? DBNull.Value : (object)instructor.SubscriberId);

                    var empanelled = new SqlParameter("@Empanelled", instructor.Empanelled);

                    var updatedOn = new SqlParameter("@UpdatedOn", DateTime.Now);

                    var UpdatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(instructor.UpdatedBy) ? DBNull.Value : (object)instructor.UpdatedBy);

                    int i = context.Database.ExecuteSqlCommand("USP_AddInstructorLead @InstructorId, @Name,@CountryId, @StateId, @CityId, " +
                        "@Zone, @Qualification,@Experience, @DomainExpertize,@Organization,@LanguageKnown, @Specialization, @NibfProject, @TrainingLocation,  " +
                        "@Remarks, @SubscriberId, @Empanelled, @UpdatedOn, @UpdatedBy ",
                        instructorId, Name, countryId, stateId, cityId, zone, qualification, experience, domainExpertize, organization, languageKnown, specialization,
                        nibfProject, trainingLocation, remarks, subscriberId, empanelled, updatedOn, UpdatedBy);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            { }
            return res;
        }

        public List<CandidateAttendanceView> GetAttendence(string TrainingId)
        {
            var SchedulesBatch = new List<CandidateAttendanceView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    SchedulesBatch = db.Database
                             .SqlQuery<CandidateAttendanceView>("EXEC USP_GetAttendence @TrainingId",
                              new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return SchedulesBatch;
        }

        /// <summary>
        /// CreateBy : Preeti Singh
        /// CreatedOn : 18-08-2017
        /// Purpose :Get Designation
        /// </summary>

        public List<Designation> GetDesignation(string CorporateId)
        {
            var designationList = new List<Designation>();
            try
            {
                using (var context = new UserDBContext())
                {
                    designationList = db.Database
                            .SqlQuery<Designation>("EXEC USP_GetDesignation @CorporateId",
                             new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return designationList;
        }

        public string GetUserplanId()
        {
            UserDBContext udbc = new UserDBContext();
            string year = Convert.ToString(DateTime.UtcNow.Year).Substring(2);
            string quarter = "U1";
            int month = DateTime.UtcNow.Month;
            if (month > 3 && month <= 6)
            {
                quarter = "U2";
            }
            else if (month > 6 && month <= 9)
            {
                quarter = "U3";
            }
            else if (month > 9 && month <= 12)
            {
                quarter = "U4";
            }

            string UserPlanId = "UP" + year + quarter + "000001";

            var UserPlans = from s in udbc.UserPlan.Where(s => s.UserPlanId.Substring(0, 6) == "UP" + year + quarter)
                            orderby s.UserPlanId descending
                            select s.UserPlanId;

            var UserPlan = UserPlans.FirstOrDefault();

            if (UserPlan != null)
            {
                string UserPlanPartialId = UserPlan.Substring(7);
                int lastVal = Convert.ToInt32(UserPlanPartialId);
                lastVal = lastVal + 1;
                string suffix = string.Empty;

                for (int i = Convert.ToString(lastVal).Length; i < 6; i++)
                {
                    suffix = suffix + "0";
                }

                UserPlanId = UserPlan.Substring(0, 6) + suffix + Convert.ToString(lastVal);
            }
            return UserPlanId;
        }

        public bool AddUserPlan(string UserPlanId, string CorporateId, int PlanId, DateTime PlanStartDate, DateTime PlanEndDate, DateTime NextDueDate, string Comments, string PlanStatus, DateTime UpdatedOn, string UpdatedBy)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userPlanId = new SqlParameter("@UserPlanId", string.IsNullOrEmpty(UserPlanId) ? DBNull.Value : (object)UserPlanId);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var planId = new SqlParameter("@PlanId", PlanId);
                    var planStartDate = new SqlParameter("@PlanStartDate", PlanStartDate);
                    var planEndDate = new SqlParameter("@PlanEndDate", PlanEndDate);
                    var nextDueDate = new SqlParameter("@NextDueDate", NextDueDate);
                    var comments = new SqlParameter("@Comments", string.IsNullOrEmpty(Comments) ? DBNull.Value : (object)Comments);
                    var planStatus = new SqlParameter("@PlanStatus", string.IsNullOrEmpty(PlanStatus) ? DBNull.Value : (object)PlanStatus);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);

                    int i = context.Database.ExecuteSqlCommand("USP_AddUserPlan @UserPlanId, @CorporateId, @PlanId, @PlanStartDate, @PlanEndDate, @NextDueDate, @Comments, @PlanStatus, @UpdatedOn, @UpdatedBy"
                                                                                , userPlanId, corporateId, planId, planStartDate, planEndDate, nextDueDate, comments, planStatus, updatedOn, updatedBy);

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

        public bool AddUserPlanAddons(string UserAdonId, string UserPlanId, int AddOnId, DateTime AddonStartDate, string Comments, DateTime UpdatedOn, string UpdatedBy)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userAdonId = new SqlParameter("@UserAdonId", string.IsNullOrEmpty(UserAdonId) ? DBNull.Value : (object)UserAdonId);
                    var userPlanId = new SqlParameter("@UserPlanId", string.IsNullOrEmpty(UserPlanId) ? DBNull.Value : (object)UserPlanId);
                    var addOnId = new SqlParameter("@AddOnId", AddOnId);
                    var addonStartDate = new SqlParameter("@AddonStartDate", AddonStartDate);
                    var comments = new SqlParameter("@Comments", string.IsNullOrEmpty(Comments) ? DBNull.Value : (object)Comments);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);

                    int i = context.Database.ExecuteSqlCommand("USP_AddUserPlanAddons @UserAdonId, @UserPlanId, @AddOnId, @AddonStartDate, @Comments, @UpdatedOn, @UpdatedBy"
                                                                              , userAdonId, userPlanId, addOnId, addonStartDate, comments, updatedOn, updatedBy);

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

        public string GetUserAddonsId()
        {
            UserDBContext udbc = new UserDBContext();
            string year = Convert.ToString(DateTime.UtcNow.Year).Substring(2);
            string quarter = "U1";
            int month = DateTime.UtcNow.Month;
            if (month > 3 && month <= 6)
            {
                quarter = "U2";
            }
            else if (month > 6 && month <= 9)
            {
                quarter = "U3";
            }
            else if (month > 9 && month <= 12)
            {
                quarter = "U4";
            }

            string UserAdonId = "UA" + year + quarter + "000001";

            var UserAddons = from s in udbc.UserPlanAddOns.Where(s => s.UserAdonId.Substring(0, 6) == "UA" + year + quarter)
                             orderby s.UserAdonId descending
                             select s.UserAdonId;

            var UserAddon = UserAddons.FirstOrDefault();

            if (UserAddon != null)
            {
                string UserAddonPartialId = UserAddon.Substring(7);
                int lastVal = Convert.ToInt32(UserAddonPartialId);
                lastVal = lastVal + 1;
                string suffix = string.Empty;

                for (int i = Convert.ToString(lastVal).Length; i < 6; i++)
                {
                    suffix = suffix + "0";
                }

                UserAdonId = UserAddon.Substring(0, 6) + suffix + Convert.ToString(lastVal);
            }
            return UserAdonId;
        }

        public List<UserPlanDetailView> GetUserplanDetails(string CorporateId)
        {
            var userPlans = new List<UserPlanDetailView>();
            using (var db = new UserDBContext())
            {
                userPlans = db.Database
                            .SqlQuery<UserPlanDetailView>("EXEC USP_GETUserPlanDetails @CorporateId",
                             new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();

            }
            return userPlans;
        }

        public List<StatusMaster> GetStatusMaster(string CorporateId)
        {
            var statusMasterList = new List<StatusMaster>();
            try
            {
                using (var context = new UserDBContext())
                {
                    statusMasterList = db.Database
                            .SqlQuery<StatusMaster>("EXEC USP_GetStatusMasters @CorporateId",
                             new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return statusMasterList;
        }

        /// <summary>
        /// CreateBy : Anamika Pandey
        /// CreatedOn : 20-08-2017
        /// Purpose :Add Asset Status
        /// </summary>

        public bool AddAssetStatus(Int64 AssetStatusId, string AssetStatusName, string CorporateId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var statusid = new SqlParameter("@AssetStatusId", AssetStatusId);
                    var statusname = new SqlParameter("@AssetStatusName", string.IsNullOrEmpty(AssetStatusName) ? DBNull.Value : (object)AssetStatusName);
                    var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddAssetStatus @AssetStatusId,@AssetStatusName,@CorporateId", statusid, statusname, corporateid);

                    if (i > 0)
                        res = true;
                }
            }
            catch
            {

            }

            return res;
        }

        public List<AssetStatus> GetAssetStatus(string CorporateId)
        {
            var getAssetStatuslist = new List<AssetStatus>();

            try
            {
                using (var db = new UserDBContext())
                {

                    getAssetStatuslist = db.Database
                            .SqlQuery<AssetStatus>("EXEC USP_GetAssetStatus @CorporateId",
                                 new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return getAssetStatuslist;
        }

        /// Created By: Rahul Haldakr
        /// Created On: 20-8-2017
        /// Purpose :Add Asset Group
        /// </summary>

        public bool AddAssetsGroup(AssetGroup assetGroup, string CorporateId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var assetGroupId = new SqlParameter("@AssetGroupId", assetGroup.AssetGroupId);
                    var assetGroupName = new SqlParameter("@AssetGroupName", string.IsNullOrEmpty(assetGroup.AssetGroupName) ? DBNull.Value : (object)assetGroup.AssetGroupName);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    int i = context.Database.ExecuteSqlCommand("USP_AddAssetsGroup @AssetGroupId,@AssetGroupName,@CorporateId",
                            assetGroupId, assetGroupName, corporateId);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            { }
            return res;
        }

        public List<EmpDetailPlaceholderView> GetplaceHolderValue(string UserId = null)
        {
            var placeholdervalue = new List<EmpDetailPlaceholderView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    placeholdervalue = db.Database
                            .SqlQuery<EmpDetailPlaceholderView>("EXEC LetterFieldValue @UserId",
                             new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return placeholdervalue;
        }

        /// <summary>
        /// CreateBy : Anamika Pandey
        /// CreatedOn : 21-08-2017
        /// Purpose :Add relation
        /// </summary>

        public bool AddRelation(Int32 RelationId, string RelationType)
        {
            bool res = false;

            using (var context = new UserDBContext())
            {
                var id = new SqlParameter("@RelationId", RelationId);
                var relationname = new SqlParameter("@RelationType", string.IsNullOrEmpty(RelationType) ? DBNull.Value : (object)RelationType);

                int i = context.Database.ExecuteSqlCommand("USP_AddRelation @RelationId,@RelationType", id, relationname);

                if (i == 1)
                    res = true;
            }


            return res;
        }

        public List<Relation> GetRelation(string CorporateId)
        {
            var getRelationList = new List<Relation>();

            try
            {
                using (var db = new UserDBContext())
                {

                    getRelationList = db.Database
                            .SqlQuery<Relation>("EXEC USP_GetRelation").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return getRelationList;
        }

        public bool AddAssetsType(AssetType assetType, string CorporateId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var assettypeId = new SqlParameter("@AssetTypeId", assetType.AssteTypeId);
                    var assetTypeName = new SqlParameter("@AssetTypeName", string.IsNullOrEmpty(assetType.AssetTypeName) ? DBNull.Value : (object)assetType.AssetTypeName);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var AssetGroupId = new SqlParameter("@AssetGroupId", assetType.AssetGroupId);
                    int i = context.Database.ExecuteSqlCommand("USP_AddAssetType @AssetTypeId,@AssetTypeName,@AssetGroupId,@CorporateId",
                            assettypeId, assetTypeName, AssetGroupId, corporateId);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            {

            }
            return res;
        }

        /// <summary>
        /// CreateBy : Anamika Pandey
        /// CreatedOn : 22-08-2017
        /// Purpose : Add Leaving Reason 
        /// </summary>

        public bool AddLeavingReason(Int64 ReasonId, string Reason, string Description, string CorporateId)
        {
            bool res = false;

            using (var context = new UserDBContext())
            {
                var Rid = new SqlParameter("@ReasonId", ReasonId);
                var Rname = new SqlParameter("@Reason", string.IsNullOrEmpty(Reason) ? DBNull.Value : (object)Reason);
                var Rdescription = new SqlParameter("@Description", string.IsNullOrEmpty(Description) ? DBNull.Value : (object)Description);
                var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);

                int i = context.Database.ExecuteSqlCommand("USP_AddLeavingReason @ReasonId,@Reason,@Description,@CorporateId", Rid, Rname, Rdescription, corporateid);

                if (i == 1)
                    res = true;
            }


            return res;
        }

        public List<LeavingReason> GetLeavingReason(string CorporateId)
        {
            var getLeavingReasonList = new List<LeavingReason>();

            try
            {
                using (var db = new UserDBContext())
                {

                    getLeavingReasonList = db.Database
                           .SqlQuery<LeavingReason>("EXEC USP_GetLeavingReason @CorporateId",
                             new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return getLeavingReasonList;
        }

        public List<GradeMaster> GetGrade(string CorporateId)
        {
            var gradeList = new List<GradeMaster>();

            try
            {
                using (var db = new UserDBContext())
                {

                    gradeList = db.Database
                            .SqlQuery<GradeMaster>("EXEC USP_GetGrade @CorporateId",
                             new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();

                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return gradeList;
        }

        public bool AddClaimCategory(Int64 CategoryId, string CategoryName, Int16 CategoryType, string CorporateId)
        {
            bool res = false;

            using (var context = new UserDBContext())
            {
                var cid = new SqlParameter("@CategoryId", CategoryId);
                var cname = new SqlParameter("@CategoryName", string.IsNullOrEmpty(CategoryName) ? DBNull.Value : (object)CategoryName);
                var ctype = new SqlParameter("@CategoryType", CategoryType);
                var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);

                int i = context.Database.ExecuteSqlCommand("USP_AddClaimCategory @CategoryId,@CategoryName,@CategoryType,@CorporateId", cid, cname, ctype, corporateid);

                if (i == 1)
                    res = true;
            }


            return res;
        }

        public List<ClaimCategory> GetClaimCategory(string CorporateId)
        {
            var getClaimCategoryList = new List<ClaimCategory>();

            try
            {
                using (var db = new UserDBContext())
                {

                    getClaimCategoryList = db.Database
                           .SqlQuery<ClaimCategory>("EXEC USP_GetClaimCategory @CorporateId",
                             new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return getClaimCategoryList;
        }

        /// <summary>
        /// CreateBy : Anamika Pandey
        /// CreatedOn : 23-08-2017
        /// Purpose : Add Hold Salary Reason 
        /// </summary>

        public bool AddHoldSalaryReason(Int64 ReasonId, string HoldReason, string Description, string CorporateId)
        {
            bool res = false;

            using (var context = new UserDBContext())
            {
                var Hid = new SqlParameter("@ReasonId", ReasonId);
                var Hname = new SqlParameter("@HoldReason", string.IsNullOrEmpty(HoldReason) ? DBNull.Value : (object)HoldReason);
                var Hdescription = new SqlParameter("@Description", string.IsNullOrEmpty(Description) ? DBNull.Value : (object)Description);
                var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);

                int i = context.Database.ExecuteSqlCommand("USP_AddHoldSalaryReason @ReasonId,@HoldReason,@Description,@CorporateId", Hid, Hname, Hdescription, corporateid);

                if (i > 0)
                    res = true;
            }


            return res;
        }

        public List<HoldSalaryReason> GetHoldSalaryReason(string CorporateId)
        {
            var getHoldReasonList = new List<HoldSalaryReason>();

            try
            {
                using (var db = new UserDBContext())
                {

                    getHoldReasonList = db.Database
                           .SqlQuery<HoldSalaryReason>("EXEC USP_GetHoldSalaryReason @CorporateId",
                             new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return getHoldReasonList;
        }

        /// <summary>
        /// CreateBy : Anamika Pandey
        /// CreatedOn : 24-08-2017
        /// Purpose : PayRollLookUps
        /// </summary>

        public bool AddPayRollLookUps(Int64 LookUpsId, string LookUpsName, string CorporateId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var Lid = new SqlParameter("@LookUpsId", LookUpsId);
                    var Lname = new SqlParameter("@LookUpsName", string.IsNullOrEmpty(LookUpsName) ? DBNull.Value : (object)LookUpsName);
                    var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddPayRollLookUps @LookUpsId,@LookUpsName,@CorporateId", Lid, Lname, corporateid);

                    if (i > 0)
                        res = true;
                }
            }
            catch
            {

            }

            return res;
        }

        public List<PayRollLookUps> GetPayRollLookUps(string CorporateId)
        {
            var getLookUplist = new List<PayRollLookUps>();

            try
            {
                using (var db = new UserDBContext())
                {

                    getLookUplist = db.Database
                            .SqlQuery<PayRollLookUps>("EXEC USP_GetPayRollLookUps @CorporateId",
                                 new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return getLookUplist;
        }

        public EmployeeView GetsEmployeeDetail(string UserId)
        {
            var employeedetail = new EmployeeView();

            try
            {
                using (var db = new UserDBContext())
                {
                    employeedetail = db.Database
                            .SqlQuery<EmployeeView>("EXEC USP_GetEmployeeDetails @UserId",
                                 new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).FirstOrDefault();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return employeedetail;
        }

        public string ReplaceLetterFileName(string CorporateId, Int64 TemplateId)
        {
            string fileName = null;
            var image = db.LetterLogoAttachment.Where(f => f.TemplateId == TemplateId);
            if (image != null)
            {
                string imgFileName = image.FirstOrDefault().FileName;
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");
                if (image.Count() > 0)
                    fileName = "letterlogoattachment/" + image.FirstOrDefault().FileId + "/" + imgFileName;
            }
            return fileName;
        }

        //public bool AddLetterFooter(LetterFooter letterfooter, string CorporateId)
        //{
        //    bool res = false;
        //    try
        //    {
        //        using (var context = new UserDBContext())
        //        {
        //            var footerId = new SqlParameter("@FooterId", letterfooter.FooterId);
        //            var footerName = new SqlParameter("@FooterName", string.IsNullOrEmpty(letterfooter.FooterName) ? DBNull.Value : (object)letterfooter.FooterName);
        //            var footerContent = new SqlParameter("@FooterContent", string.IsNullOrEmpty(letterfooter.FooterContent) ? DBNull.Value : (object)letterfooter.FooterContent);
        //            var addressLine1 = new SqlParameter("@AddressLine1", string.IsNullOrEmpty(letterfooter.AddressLine1) ? DBNull.Value : (object)letterfooter.AddressLine1);
        //            var addressLine2 = new SqlParameter("@AddressLine2", string.IsNullOrEmpty(letterfooter.AddressLine2) ? DBNull.Value : (object)letterfooter.AddressLine2);
        //            var cityId = new SqlParameter("@CityId", letterfooter.CityId ?? 0);
        //            var stateId = new SqlParameter("@StateId", letterfooter.StateId ?? 0);
        //            var postalCode = new SqlParameter("@PostalCode", string.IsNullOrEmpty(letterfooter.PostalCode) ? DBNull.Value : (object)letterfooter.PostalCode);
        //            var countryId = new SqlParameter("@CountryId", letterfooter.CountryId ?? 0);
        //            var signature = new SqlParameter("@Signature", string.IsNullOrEmpty(letterfooter.Signature) ? DBNull.Value : (object)letterfooter.Signature);
        //            var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);

        //            int i = context.Database.ExecuteSqlCommand("USP_AddLetterFooter @FooterId, @FooterName, @FooterContent, @AddressLine1," +
        //                                                        "@AddressLine2 ,@CityId, @StateId, @PostalCode ,@CountryId," +
        //                                                        "@Signature,  @CorporateId",
        //                                                        footerId, footerName, footerContent, addressLine1, addressLine2, cityId
        //                                                        , stateId, postalCode, countryId, signature, corporateId);
        //            if (i > 0)
        //                res = true;
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    return res;
        //}

        public bool AddLetterType(LetterType letterType, string CorporateId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var letterTypeId = new SqlParameter("@LetterTypeId", letterType.LetterTypeId);
                    var letterTypeName = new SqlParameter("@LetterTypeName", string.IsNullOrEmpty(letterType.LetterTypeName) ? DBNull.Value : (object)letterType.LetterTypeName);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddLetterType @LetterTypeId, @LetterTypeName, @CorporateId",
                                                                letterTypeId, letterTypeName, corporateId);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            {
            }
            return res;
        }

        public async Task<bool> IntegrateLMSCourse(string CourseCode, string LMSCourseCode, string UserId)
        {
            CourseMaster coursemaster = db.CourseMaster.Find(CourseCode);
            if (coursemaster != null)
            {
                coursemaster.LMSCourseCode = LMSCourseCode;
                db.Entry(coursemaster).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                bool result = await RegisterForCourse(GetBatchCandidates(CourseCode), LMSCourseCode, UserId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// To Get List of candidate assigned for any batch of given course that could be running batch or expired one
        /// </summary>
        /// <param name="CourseCode">Blink Course Code</param>
        /// <returns></returns>
        public List<CandidateViewModel> GetBatchCandidates(string CourseCode)
        {
            List<CandidateViewModel> users = new List<CandidateViewModel>();
            try
            {
                using (var db = new UserDBContext())
                {
                    users = db.Database.SqlQuery<CandidateViewModel>("EXEC USP_GetBatchCandidates @CourseCode",
                        new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode)).ToList();
                }
            }
            catch (Exception) { }
            return users;
        }

        /// <summary>
        /// Register Candidates to LMS Course so that they can be able to save there response
        /// </summary>
        /// <param name="candidates"> List Of Candidates with some details</param>
        /// <param name="CourseCode"> LMS Course Code</param>
        /// <param name="UserId"> Current User</param>
        /// <returns> Returns Boolean asynchronised value</returns>
        public async Task<bool> RegisterForCourse(List<CandidateViewModel> candidates, string CourseCode, string UserId)
        {
            bool Result = false;
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            List<UserRegView> userReg = new List<UserRegView>();
            foreach (var candi in candidates)
            {
                if (userReg.Count() == 0)
                {
                    userReg.Add(new UserRegView()
                    {
                        UserId = candi.UserId,
                        UserName = candi.UserName,
                        UserRole = "Candidate",
                        Name = candi.Name,
                        Email = candi.Email,
                        MobileNumber = candi.PhoneNumber,
                        Redirectionurl = "Blink.com",
                        SubscriberId = candi.SubscriberId,
                        Password = candi.PCode,
                        CourseCode = CourseCode
                    });

                }
                else
                {
                    if (userReg.Where(a => a.UserId == candi.UserId).FirstOrDefault() == null)
                    {
                        userReg.Add(new UserRegView()
                        {
                            UserId = candi.UserId,
                            UserName = candi.UserName,
                            UserRole = "Candidate",
                            Name = candi.Name,
                            Email = candi.Email,
                            MobileNumber = candi.PhoneNumber,
                            Redirectionurl = "Blink.com",
                            SubscriberId = candi.SubscriberId,
                            Password = candi.PCode,
                            CourseCode = CourseCode
                        });
                    }
                }
            }

            string apiUrl = Global.WikipianUrl() + "/Api/Value/PostBulkUserReg";
            HttpResponseMessage responsePostMethod = new HttpResponseMessage();
            var client = new HttpClient();
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            responsePostMethod = await client.PostAsJsonAsync(apiUrl, userReg);
            if (responsePostMethod.IsSuccessStatusCode)
            {
                Result = true;
            }
            return Result;
        }

        public string GenerateReferenceNo(string UserId = null, Int64 lettertypeId = 0)
        {
            string LetterTypeName = db.LetterType.Find(lettertypeId).LetterTypeName;
            string year = Convert.ToString(DateTime.UtcNow.Year).Substring(2);
            string monthname = DateTime.Now.ToString("MMM");
            var Gletter = db.CorporateLetter.Where(c => c.LetterTypeId == lettertypeId).ToList();
            var CompanyName = db.CompanyProfile.Find(UserId);
            string cName = "";
            if (CompanyName != null)
            {
                cName = CompanyName.CompanyName + "/";
            }

            var lettercount = 1;
            if (Gletter.Count > 0)
            {
                lettercount = Gletter.Where(c => c.CorporateId == UserId).Count();
                lettercount = lettercount + 1;
            }
            string abc = "";

            LetterTypeName = LetterTypeName.Replace(" ", "");
            LetterTypeName = LetterTypeName.Replace("Letter", "");
            cName = cName.Replace(" ", "");
            abc = LetterTypeName + "/" + cName + monthname + "-" + year + "/" + lettercount;

            return abc;
        }

        public List<LetterDesignView> GetLetterFooter()
        {
            var letterFooter = new List<LetterDesignView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    letterFooter = db.Database.SqlQuery<LetterDesignView>("EXEC USP_GetLetterFooter").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return letterFooter;
        }

        public List<LetterDesignView> GetLetterHeader()
        {
            var letterHeader = new List<LetterDesignView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    letterHeader = db.Database.SqlQuery<LetterDesignView>("EXEC USP_GetLetterHeader").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return letterHeader;
        }

        public string uploadSendLetter(string CorporateId, string UserId, string FileName, Int64 LetterTypeId, Byte[] upload)
        {
            string res = "Failure";
            Int64 FileId = 0;
            if (upload != null && upload.Length > 0)
            {
                var file = db.SendLetter.Where(d => d.LetterTypeId == LetterTypeId && d.CorporateId == CorporateId && d.UserId == UserId).FirstOrDefault();
                if (file != null)
                {
                    FileId = file.FileId;
                    if (LetterTypeId == file.LetterTypeId)
                    {
                        string fileName = "Letters/" + UserId + "/" + FileId + "/" + file.FileName;
                        blobManager.DeleteBlob(CorporateId.ToLower(), fileName.ToLower());
                        //db.SendLetter.Remove(file);
                        //db.SaveChanges();
                    }
                }
                string imgFileName = System.IO.Path.GetFileName(FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");
                bool added = AddDocument(FileName.ToLower(), "application/pdf", CorporateId, UserId, LetterTypeId, FileId);
                if (added)
                {
                    Int64 fileId = db.SendLetter.Where(c => c.LetterTypeId == LetterTypeId && c.UserId == UserId && c.CorporateId == CorporateId).FirstOrDefault().FileId;
                    string fileName = "Letters/" + UserId + "/" + fileId + "/" + imgFileName + ".pdf";
                    blobManager.UploadByteBlob(CorporateId.ToLower(), fileName.ToLower(), "application/pdf", upload);
                }
                res = "Succeed";
            }
            return res;
        }

        public bool AddDocument(string FileName, string ContentType, string CorporateId, string UserId, Int64 LetterTypeId = 0, Int64 FileId = 0)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var fileId = new SqlParameter("@FileId", FileId);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var letterTypeId = new SqlParameter("@LetterTypeId", LetterTypeId);
                    var issuingDate = new SqlParameter("@IssuingDate", DateTime.Now);

                    int i = context.Database.ExecuteSqlCommand("USP_AddSendLetter @FileId, @FileName,@ContentType, @CorporateId, @UserId, @LetterTypeId, @IssuingDate",
                                                                fileId, fileName, contentType, corporateId, userId, letterTypeId, issuingDate);
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

        public List<SendLetterView> GetIssueLetterList(string UserId)
        {
            var issueletterlist = new List<SendLetterView>();
            try
            {
                using (var db = new UserDBContext())
                {
                    issueletterlist = db.Database
                            .SqlQuery<SendLetterView>("EXEC USP_GetSendLetterList @UserId",
                                 new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
            }
            return issueletterlist;
        }

        public bool AddEmployeeCheckinCheckout(Int64 BiometricId, string UserId, DateTime? CheckInDate, TimeSpan? CheckInTime, DateTime? CheckOutDate, TimeSpan? CheckOutTime, string IpAddress, string SubscriberId, Int64 ShiftId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var biometricId = new SqlParameter("@BiometricId", BiometricId);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var checkInDate = new SqlParameter("@CheckInDate", CheckInDate);
                    var checkInTime = new SqlParameter("@CheckInTime", CheckInTime);
                    var checkOutDate = new SqlParameter("@CheckOutDate", DBNull.Value);
                    if (CheckOutDate != null)
                        checkOutDate = new SqlParameter("@CheckOutDate", CheckOutDate);

                    var checkOutTime = new SqlParameter("@CheckOutTime", DBNull.Value);
                    if (CheckOutDate != null)
                        checkOutTime = new SqlParameter("@CheckOutTime", CheckOutTime);

                    var ipAddress = new SqlParameter("@IpAddress", string.IsNullOrEmpty(IpAddress) ? DBNull.Value : (object)IpAddress);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                    var shiftId = new SqlParameter("@ShiftId", ShiftId);


                    int i = context.Database.ExecuteSqlCommand("USP_AddEmployeeCheckinCheckout @BiometricId, @UserId, @CheckInDate, @CheckInTime, @CheckOutDate, @CheckOutTime, @IpAddress, @SubscriberId, @ShiftId",
                                                                biometricId, userId, checkInDate, checkInTime, checkOutDate, checkOutTime, ipAddress, subscriberId, shiftId);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            {
            }
            return res;
        }

        public List<EmployeeBiometricView> GetEmployeewithDepartmentForBiometric(string CorporateId)
        {
            var Employee = new List<EmployeeBiometricView>();

            using (var db = new UserDBContext())
            {

                Employee = db.Database
                          .SqlQuery<EmployeeBiometricView>("exec USP_GetClientDetails  @CorporateId",
                          new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
            }

            if (Employee.Count() > 0)
            {
                foreach (var item in Employee)
                {
                    item.BiometricCheckInCheckOutview = GetBiometricData(CorporateId).ToList();
                }
            }
            return Employee;

        }

        public List<BiometricCheckInCheckOutview> GetBiometricData(string SubscriberId)
        {
            var biometricData = new List<BiometricCheckInCheckOutview>();
            try
            {
                using (var db = new UserDBContext())
                {
                    biometricData = db.Database
                            .SqlQuery<BiometricCheckInCheckOutview>("EXEC USP_GetBiometricData @SubscriberId",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
            }
            return biometricData;
        }

        public bool AddIpAddress(Int64 IpId, Int64 WorkLocation, Int16 Authenticate, string IPAddressFrom, string IpAddressTo, float LatitudeFrom, float LatitudeTo, float LongitudeFrom, float LongitudeTo, string UserId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var ipId = new SqlParameter("@IpId", IpId);
                    var workLocation = new SqlParameter("@WorkLocation", WorkLocation);
                    var authenticate = new SqlParameter("@Authenticate", Authenticate);
                    var iPAddressFrom = new SqlParameter("@IPAddressFrom", string.IsNullOrEmpty(IPAddressFrom) ? DBNull.Value : (object)IPAddressFrom);
                    var ipAddressTo = new SqlParameter("@IpAddressTo", string.IsNullOrEmpty(IpAddressTo) ? DBNull.Value : (object)IpAddressTo);
                    var latitudeFrom = new SqlParameter("@LatitudeFrom", LatitudeFrom);
                    var latitudeTo = new SqlParameter("@LatitudeTo", LatitudeTo);
                    var longitudeFrom = new SqlParameter("@LongitudeFrom", LongitudeFrom);
                    var longitudeTo = new SqlParameter("@LongitudeTo", LongitudeTo);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddIpAddress @IpId,@WorkLocation,@Authenticate, @IPAddressFrom, @IpAddressTo,"
                                                                + "@UserId,@LatitudeFrom,@LatitudeTo,@LongitudeFrom,@LongitudeTo ",
                                                                ipId, workLocation, authenticate, iPAddressFrom, ipAddressTo, userId,
                                                                latitudeFrom, latitudeTo, longitudeFrom, longitudeTo);
                    if (i > 0)
                        res = true;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return res;
        }


        public List<BranchLocationView> GetBranchLocationDetail(string SubscriberId)
        {
            var location = new List<BranchLocationView>();
            try
            {
                using (var db = new UserDBContext())
                {
                    location = db.Database
                            .SqlQuery<BranchLocationView>("EXEC USP_GetBranchLocationDetail @SubscriberId",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
            }
            return location;
        }

        public bool AddShifts(Int64 ShiftId, string Shift, string CorporateId, TimeSpan? StartTime, TimeSpan? EndTime)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var shiftId = new SqlParameter("@ShiftId", ShiftId);
                    var shift = new SqlParameter("@Shift", string.IsNullOrEmpty(Shift) ? DBNull.Value : (object)Shift);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);

                    var startTime = new SqlParameter("@StartTime", DBNull.Value);
                    if (StartTime != null)
                        startTime = new SqlParameter("@StartTime", StartTime);

                    var endTime = new SqlParameter("@EndTime", DBNull.Value);
                    if (EndTime != null)
                        endTime = new SqlParameter("@EndTime", EndTime);

                    int i = context.Database.ExecuteSqlCommand("USP_AddShifts @ShiftId, @Shift, @CorporateId, @StartTime, @EndTime",
                                                                shiftId, shift, corporateId, startTime, endTime);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            {
            }
            return res;
        }

        public void DeleteIp(Int64 IpId)
        {
            var ip = db.IpMasters.Find(IpId);
            if (ip != null)
            {
                db.IpMasters.Remove(ip);
                db.SaveChanges();
            }
        }

        public void DeleteShift(Int64 ShiftId)
        {
            var shift = db.ShiftMaster.Find(ShiftId);
            if (shift != null)
            {
                db.ShiftMaster.Remove(shift);
                db.SaveChanges();
            }
        }

        public List<ExcelviewModel> GetExcelColoumData(Int16 SchemeId = 0, string CorporateId = "")
        {
            var Excelcoloum = new List<ExcelviewModel>();
            try
            {
                using (var db = new UserDBContext())
                {
                    Excelcoloum = db.Database
                            .SqlQuery<ExcelviewModel>("EXEC USP_GetExcelLeaveScheme @SchemeId,@CorporateId",
                          new SqlParameter("@SchemeId", SchemeId),
                          new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
            }
            return Excelcoloum;
        }

        public bool AddEmployeeLeaveBulkUpload(string TrainerId, Int16 SchemeId, Int64 EngagementTypeId, float EngagementCount, int LeaveDate, float MaxLimit)
        {
            bool res = false;
            Int64 PlannerSummaryId = 0;
            //try
            //{
            using (var context = new UserDBContext())
            {
                var plannerSummaryId = new SqlParameter("@PlannerSummaryId", PlannerSummaryId);
                var trainerId = new SqlParameter("@TrainerId", string.IsNullOrEmpty(TrainerId) ? DBNull.Value : (object)TrainerId);
                var schemeId = new SqlParameter("@SchemeId", SchemeId);

                var engagementTypeId = new SqlParameter("@EngagementTypeId", EngagementTypeId);
                var engagementCount = new SqlParameter("@EngagementCount", EngagementCount);
                var leaveYear = new SqlParameter("@LeaveYear", LeaveDate);
                var maxlimit = new SqlParameter("@MaxLimit", MaxLimit);
                int i = context.Database.ExecuteSqlCommand("USP_AddEmployeeLeaveBulkUpload @PlannerSummaryId, @TrainerId, @SchemeId, @EngagementTypeId, @EngagementCount,@LeaveYear,@MaxLimit",
                                                            plannerSummaryId, trainerId, schemeId, engagementTypeId, engagementCount, leaveYear, maxlimit);
                if (i > 0)
                    res = true;
            }
            //}
            //catch
            //{
            //}
            return res;
        }

        public bool AddPayrollHeads(Int16 PayrollHeadID, string HeadName, float DefaultPercentage, string Category, float MaxLimit, string Period, bool TaxExemption, bool AvailableByDefault)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var payrollHeadID = new SqlParameter("@PayrollHeadID", PayrollHeadID);
                    var headName = new SqlParameter("@HeadName", string.IsNullOrEmpty(HeadName) ? DBNull.Value : (object)HeadName);
                    var defaultPercentage = new SqlParameter("@DefaultPercentage", DefaultPercentage);
                    var category = new SqlParameter("@Category", string.IsNullOrEmpty(Category) ? DBNull.Value : (object)Category);
                    var maxLimit = new SqlParameter("@MaxLimit", MaxLimit);
                    var period = new SqlParameter("@Period", string.IsNullOrEmpty(Period) ? DBNull.Value : (object)Period);
                    var taxExemption = new SqlParameter("@TaxExemption", TaxExemption);
                    var availableByDefault = new SqlParameter("@AvailableByDefault", AvailableByDefault);

                    int i = context.Database.ExecuteSqlCommand("USP_AddPayrollHeads @PayrollHeadID, @HeadName, @DefaultPercentage, @Category, @MaxLimit, @Period, @TaxExemption, @AvailableByDefault",
                                                                payrollHeadID, headName, defaultPercentage, category, maxLimit, period, taxExemption, availableByDefault);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            {
            }
            return res;
        }

        public void DeletePayrollHeads(Int16 PayrollHeadID)
        {
            var payrollHeads = db.PayrollHeads.Find(PayrollHeadID);
            if (payrollHeads != null)
            {
                db.PayrollHeads.Remove(payrollHeads);
                db.SaveChanges();
            }
        }

        public List<CorporatePayrollHeadview> GetAllPayrollHeads(string CorporateId)
        {
            var getheads = new List<CorporatePayrollHeadview>();

            using (var db = new UserDBContext())
            {
                getheads = db.Database
                          .SqlQuery<CorporatePayrollHeadview>("EXEC USP_GetCorporatePayrollHeads @CorporateId",
                 new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
            }

            return getheads;
        }

        public bool AddEmployeeTour(EmployeeTour employeeTour)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var tourId = new SqlParameter("@TourId", employeeTour.TourId);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(employeeTour.UserId) ? DBNull.Value : (object)employeeTour.UserId);
                    var purpose = new SqlParameter("@Purpose", string.IsNullOrEmpty(employeeTour.Purpose) ? DBNull.Value : (object)employeeTour.Purpose);
                    var tourFromDate = new SqlParameter("@TourFromDate", employeeTour.TourFromDate);
                    var tourToDate = new SqlParameter("@TourToDate", employeeTour.TourToDate);
                    var location = new SqlParameter("@Location", string.IsNullOrEmpty(employeeTour.Location) ? DBNull.Value : (object)employeeTour.Location);
                    var observations = new SqlParameter("@Observations", string.IsNullOrEmpty(employeeTour.Observations) ? DBNull.Value : (object)employeeTour.Observations);
                    var conclusion = new SqlParameter("@Conclusion", string.IsNullOrEmpty(employeeTour.Conclusion) ? DBNull.Value : (object)employeeTour.Conclusion);
                    var contactNumber = new SqlParameter("@ContactNumber", string.IsNullOrEmpty(employeeTour.ContactNumber) ? DBNull.Value : (object)employeeTour.ContactNumber);
                    var plannerId = new SqlParameter("@PlannerId", employeeTour.PlannerId);
                    int i = context.Database.ExecuteSqlCommand("USP_AddEmployeeTourRecord @TourId ,@UserId ,@Purpose," +
                                                                "@TourFromDate ,@TourToDate ,@Location ,@Observations," +
                                                                "@Conclusion, @ContactNumber,@plannerId", tourId, userId, purpose,
                                                                tourFromDate, tourToDate, location, observations
                                                                , conclusion, contactNumber, plannerId);

                    if (i > 0)
                        res = true;
                }
            }
            catch
            {

            }
            return res;
        }

        public string uploadTourFileAttachment(HttpPostedFileBase upload, string UserId, Int64 TourId = 0)
        {
            string res = "Failure";
            Int64 FileId = 0;
            if (upload != null && upload.ContentLength > 0)
            {
                var file = db.TourAttachment.Where(d => d.TourId == TourId).FirstOrDefault();
                if (file != null)
                {
                    FileId = file.FileId;
                    blobManager.DeleteBlob(TourId.ToString().ToLower(), GetFileName(FileId).ToLower());
                    db.TourAttachment.Remove(file);
                    db.SaveChanges();
                }
                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");
                bool added = AddTourAttachment(imgFileName.ToLower(), upload.ContentType, TourId, FileId);
                if (added)
                {
                    blobManager.UploadBlob(UserId.ToLower(), ReplaceTourFileName(TourId).ToLower(), upload);

                }
                res = "Succeed";
            }
            return res;
        }

        public bool AddTourAttachment(string FileName, string ContentType, Int64 TourId = 0, Int64 FileId = 0)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var fileId = new SqlParameter("@FileId", FileId);

                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);
                    var tourId = new SqlParameter("@TourId", TourId);
                    int i = context.Database.ExecuteSqlCommand("USP_AddEmployeeTourAttachment @FileId,@FileName, @ContentType, @TourId",
                        fileId, fileName, contentType, tourId);

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

        public string ReplaceTourFileName(Int64 TourId = 0)
        {
            string fileName = null;
            var image = db.TourAttachment.Where(c => c.TourId == TourId);

            if (image != null)
            {
                if (image.Count() > 0)
                {
                    string imgFileName = image.FirstOrDefault().FileName;
                    imgFileName = imgFileName.Replace(' ', '_');
                    imgFileName = imgFileName.Replace("'", "_");
                    fileName = "tourattachment/" + image.FirstOrDefault().FileId + "/" + imgFileName;
                }
            }
            return fileName;
        }
        //by vikas pandey 24/11/2017
        public bool AddCompanySetting(Int64 SettingId, string CorporateId, Int16 WorkingDayPerWeek, Int32 ProbationPeriods, string DefaultCurrency, string CalendarYear,
        bool PayslipPasswordProtaction, bool AutoeanblePayrollProcess, Int16 AutoProcessPayrollDate, bool CompanyAttendance, bool LeavesCalculationCriteria)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var settingid = new SqlParameter("@SettingId", SettingId);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var workingdayperweek = new SqlParameter("@WorkingDayPerWeek", WorkingDayPerWeek);
                    var probationperiods = new SqlParameter("@ProbationPeriod", ProbationPeriods);
                    var defaultcurrency = new SqlParameter("@DefaultCurrency", string.IsNullOrEmpty(DefaultCurrency) ? DBNull.Value : (object)DefaultCurrency);
                    var calendaryear = new SqlParameter("@CalendarYear", string.IsNullOrEmpty(CalendarYear) ? DBNull.Value : (object)CalendarYear);
                    var payslippasswordprotaction = new SqlParameter("@PaySlipPasswordProtaction", PayslipPasswordProtaction);
                    var autoenablepayrollprocess = new SqlParameter("@AutoEnablePayRollProcess", AutoeanblePayrollProcess);
                    var autoprocesspayrolldate = new SqlParameter("@AutoProcessPayRollDate", AutoProcessPayrollDate);
                    var companyattendance = new SqlParameter("@CompanyAttendance", CompanyAttendance);
                    var leavesCalculationCriteria = new SqlParameter("@LeavesCalculationCriteria", LeavesCalculationCriteria);
                    int i = context.Database.ExecuteSqlCommand("USP_AddCompanySetting  @SettingId,@CorporateId,@WorkingDayPerWeek,@ProbationPeriod,@DefaultCurrency ,@CalendarYear,@PaySlipPasswordProtaction ,@AutoEnablePayRollProcess,@AutoProcessPayRollDate,@CompanyAttendance, @LeavesCalculationCriteria"
                        , settingid, corporateId, workingdayperweek, probationperiods, defaultcurrency, calendaryear, payslippasswordprotaction, autoenablepayrollprocess, autoprocesspayrolldate, companyattendance, leavesCalculationCriteria);

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
        public bool AddCorporatePayrollHeads(Int64 CorporatePayrollHeadID, string CorporateId, Int16 PayrollHeadID, string PayrollHeadName, float PayrollPercent, string PayrollCategory, float MaxLimit, string Period, bool TaxExemption, DateTime? EffectiveFrom)
        {
            bool res = false;
            using (var context = new UserDBContext())
            {
                var corporatePayrollHeadID = new SqlParameter("@CorporatePayrollHeadID", CorporatePayrollHeadID);
                var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                var payrollHeadID = new SqlParameter("@PayrollHeadID", PayrollHeadID);
                var payrollHeadName = new SqlParameter("@PayrollHeadName", string.IsNullOrEmpty(PayrollHeadName) ? DBNull.Value : (object)PayrollHeadName);
                var payrollPercent = new SqlParameter("@PayrollPercent", PayrollPercent);
                var payrollCategory = new SqlParameter("@PayrollCategory", string.IsNullOrEmpty(PayrollCategory) ? DBNull.Value : (object)PayrollCategory);
                var maxLimit = new SqlParameter("@MaxLimit", MaxLimit);
                var period = new SqlParameter("@Period", string.IsNullOrEmpty(Period) ? DBNull.Value : (object)Period);
                var taxExemption = new SqlParameter("@TaxExemption", TaxExemption);
                var effectiveFrom = new SqlParameter("@EffectiveFrom", EffectiveFrom);

                int i = context.Database.ExecuteSqlCommand("USP_AddCorporatePayrollHeads @CorporatePayrollHeadID, @CorporateId, @PayrollHeadID, @PayrollHeadName, @PayrollPercent, @PayrollCategory, @MaxLimit, @Period, @TaxExemption, @EffectiveFrom",
                                                                                         corporatePayrollHeadID, corporateId, payrollHeadID, payrollHeadName, payrollPercent, payrollCategory, maxLimit, period, taxExemption, effectiveFrom);
                if (i > 0)
                    res = true;
            }

            return res;
        }

        public bool EmployeeSalary(EmployeeSalary employeeSalary)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var eSID = new SqlParameter("@ESID", employeeSalary.ESID);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(employeeSalary.UserId) ? DBNull.Value : (object)employeeSalary.UserId);
                    var monthlyCTC = new SqlParameter("@MonthlyCTC", employeeSalary.MonthlyCTC);
                    var annualCTC = new SqlParameter("@AnnualCTC", employeeSalary.AnnualCTC);
                    var remarks = new SqlParameter("@Remarks", string.IsNullOrEmpty(employeeSalary.Remarks) ? DBNull.Value : (object)employeeSalary.Remarks);
                    var payoutMonth = new SqlParameter("@PayoutMonth", employeeSalary.PayoutMonth);
                    var payoutYear = new SqlParameter("@PayoutYear", employeeSalary.PayoutYear);
                    var effectiveFrom = new SqlParameter("@EffectiveFrom", DBNull.Value);
                    if (employeeSalary.EffectiveFrom != null)
                        effectiveFrom = new SqlParameter("@EffectiveFrom", employeeSalary.EffectiveFrom);

                    int i = context.Database.ExecuteSqlCommand("USP_EmpSalaryDetail @ESID, @UserId, @MonthlyCTC, @AnnualCTC, @Remarks, @PayoutMonth, @PayoutYear, @EffectiveFrom",
                        eSID, userId, monthlyCTC, annualCTC, remarks, payoutMonth, payoutYear, effectiveFrom);
                    if (i > 0)
                        res = true;
                }
            }
            catch
            { }
            return res;
        }

        public bool AddEmployeeSalaryHeads(Int64 EmployeeSalaryHeadId, Int64 ESID, string UserId, Int64 CorporatePayrollHeadID, float Amount)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var employeeSalaryHeadId = new SqlParameter("@EmployeeSalaryHeadId", EmployeeSalaryHeadId);
                    var eSID = new SqlParameter("@ESID", ESID);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var corporatePayrollHeadID = new SqlParameter("@CorporatePayrollHeadID", CorporatePayrollHeadID);
                    var amount = new SqlParameter("@Amount", Amount);

                    int i = context.Database.ExecuteSqlCommand("USP_AddEmployeeSalaryHeads @EmployeeSalaryHeadId, @ESID, @UserId, @CorporatePayrollHeadID, @Amount",
                                                                                    employeeSalaryHeadId, eSID, userId, corporatePayrollHeadID, amount);

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

        public bool AddIncomeTaxSlabs(Int64 IncomeTaxSlabId, float IncomeTaxSlabFrom, float IncomeTaxSlabTo, Int16 IncomeTaxRate, Int16 Educationcess, Int16 SecondaryAndHigherEducationCess)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var incomeTaxSlabId = new SqlParameter("@IncomeTaxSlabId", IncomeTaxSlabId);
                    var incomeTaxSlabFrom = new SqlParameter("@IncomeTaxSlabFrom", IncomeTaxSlabFrom);
                    var incomeTaxSlabTo = new SqlParameter("@IncomeTaxSlabTo", IncomeTaxSlabTo);
                    var incomeTaxRate = new SqlParameter("@IncomeTaxRate", IncomeTaxRate);
                    var educationcess = new SqlParameter("@Educationcess", Educationcess);
                    var secondaryAndHigherEducationCess = new SqlParameter("@SecondaryAndHigherEducationCess", SecondaryAndHigherEducationCess);

                    int i = context.Database.ExecuteSqlCommand("USP_AddIncomeTaxSlabs @IncomeTaxSlabId, @IncomeTaxSlabFrom, @IncomeTaxSlabTo, @IncomeTaxRate, @Educationcess, @SecondaryAndHigherEducationCess",
                                                                                        incomeTaxSlabId, incomeTaxSlabFrom, incomeTaxSlabTo, incomeTaxRate, educationcess, secondaryAndHigherEducationCess);

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

        public void DeleteIncomeTaxSlab(Int64 IncomeTaxSlabId)
        {
            var SlabId = db.IncomeTaxSlab.Find(IncomeTaxSlabId);
            if (SlabId != null)
            {
                db.IncomeTaxSlab.Remove(SlabId);
                db.SaveChanges();
            }
        }

        public List<HelpLineTrackerViewModel> GetHelpLineData(string CorporateId = "", long Category = 0, long SubCategory = 0)
        {
            var Excelcoloum = new List<HelpLineTrackerViewModel>();
            try
            {
                using (var db = new UserDBContext())
                {
                    Excelcoloum = db.Database
                            .SqlQuery<HelpLineTrackerViewModel>("EXEC USP_HelpLineTracker @CorporateId,@Category,@SubCategory ",
                          new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)
                          , new SqlParameter("@Category", (object)Category)
                          , new SqlParameter("@SubCategory", (object)SubCategory)
                          ).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
            }
            return Excelcoloum;
        }

        public bool UpdateHelplineResponse(Int64 TrackerId, string Response, string ReplyBy)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var trackerId = new SqlParameter("@TrackerId", TrackerId);
                    var response = new SqlParameter("@Response", Response);
                    var replyBy = new SqlParameter("@ReplyBy", ReplyBy);
                    var repliedOn = new SqlParameter("@RepliedOn", DateTime.Now);


                    int i = context.Database.ExecuteSqlCommand("USP_UpdateHelpLineResponse @TrackerId, @Response, @ReplyBy, @RepliedOn",
                                                                                        trackerId, response, replyBy, repliedOn);

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


        public bool AddHelpLineTracker(HelpLineTracker HelpQuery)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(HelpQuery.SubscriberId) ? DBNull.Value : (object)HelpQuery.SubscriberId);
                    var userName = new SqlParameter("@UserName", string.IsNullOrEmpty(HelpQuery.UserName) ? DBNull.Value : (object)HelpQuery.UserName);
                    var name = new SqlParameter("@Name", string.IsNullOrEmpty(HelpQuery.Name) ? DBNull.Value : (object)HelpQuery.Name);
                    var phoneNumber = new SqlParameter("@PhoneNumber", string.IsNullOrEmpty(HelpQuery.PhoneNumber) ? DBNull.Value : (object)HelpQuery.PhoneNumber);
                    var email = new SqlParameter("@EmailId", string.IsNullOrEmpty(HelpQuery.EmailId) ? DBNull.Value : (object)HelpQuery.EmailId);
                    var category = new SqlParameter("@Category", HelpQuery.Category);
                    var subcategory = new SqlParameter("@SubCategory", HelpQuery.SubCategory);
                    var query = new SqlParameter("@Query", HelpQuery.Query);
                    var resolution = new SqlParameter("@Resolution", HelpQuery.Resolution);
                    var dynamicQuery = new SqlParameter("@DynamicQuery", string.IsNullOrEmpty(HelpQuery.DynamicQuery) ? DBNull.Value : (object)HelpQuery.DynamicQuery);
                    var replied = new SqlParameter("@Replied", HelpQuery.Replied);
                    var queriedOn = new SqlParameter("@QueriedOn", HelpQuery.QueriedOn);
                    var repliedOn = new SqlParameter("@RepliedOn", HelpQuery.RepliedOn);

                    int i = context.Database.ExecuteSqlCommand("USP_AddHelpLineTracker @SubscriberId, @UserName, @Name, @PhoneNumber, @EmailId, @Category, @SubCategory, @Query, @Resolution, @DynamicQuery, @Replied, @QueriedOn, @RepliedOn",
                                                                                        subscriberId, userName, name, phoneNumber, email, category, subcategory, query, resolution, dynamicQuery, replied, queriedOn, repliedOn);

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

        public List<EmployeeRegistrationDetailViewModel> GetEmployeeRegistrationDetails(string PhoneNumber = null)
        {

            var user = new List<EmployeeRegistrationDetailViewModel>();

            using (var db = new UserDBContext())
            {

                user = db.Database
                          .SqlQuery<EmployeeRegistrationDetailViewModel>("exec USP_GetEmployeeLoginDetails @PhoneNumber",
                           new SqlParameter("@PhoneNumber", string.IsNullOrEmpty(PhoneNumber) ? DBNull.Value : (object)PhoneNumber)).ToList();
            }

            return user;
        }
        //for add notification about the employeeconfirmation in daily login basis
        public bool AddConfirmationNotification()
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    int i = context.Database.ExecuteSqlCommand("USP_AddNotificationForEmpConfirmation");
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
        //for inserting confirmation of employee after confirm by admin
        public bool AddEmployeeConfirmation(string UserId, string ApprovedBy, Int16 Status, string Remark)
        {
            try
            {
                using (var context = new UserDBContext())
                {

                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var status = new SqlParameter("@Status", Status);
                    var remark = new SqlParameter("@Remark", string.IsNullOrEmpty(Remark) ? DBNull.Value : (object)Remark);
                    var approvedby = new SqlParameter("@AprrovedBy", string.IsNullOrEmpty(ApprovedBy) ? DBNull.Value : (object)ApprovedBy);
                    int i = context.Database.ExecuteSqlCommand("USP_AddEmployeeConfirmation  @UserId, @AprrovedBy,@Status,@Remark", userid, approvedby, status, remark);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return true;
        }
        public List<EmployeeConfirmationView> GetEmployeeConfirmationDetails(string UserId = null)
        {

            var details = new List<EmployeeConfirmationView>();

            using (var db = new UserDBContext())
            {

                details = db.Database
                          .SqlQuery<EmployeeConfirmationView>("exec USP_GetEmployeeConfirmationDetails @UserId",
                           new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return details;
        }
    }
}


