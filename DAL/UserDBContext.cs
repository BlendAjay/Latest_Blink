using System.Linq;
using System.Web;
using AJSolutions.Models;
using AJSolutions.Areas.LMS.Models;
using AJSolutions.Areas.Admin.Models;
using AJSolutions.Areas.RMS.Models;
using AJSolutions.Areas.CMS.Models;
using AJSolutions.Areas.TMSLite.Models;
using System.Data.Entity;

namespace AJSolutions.DAL
{
    public class UserDBContext : DbContext
    {
        public UserDBContext()
            : base("DefaultConnection")
        {
        }
        public virtual DbSet<CountryMaster> CountryMaster { get; set; }

        public virtual DbSet<StatesMaster> StatesMaster { get; set; }

        public virtual DbSet<CityMaster> CityMaster { get; set; }

        public virtual DbSet<DepartmentMaster> DepartmentMaster { get; set; }

        public virtual DbSet<ModuleMaster> ModuleMaster { get; set; }

        public virtual DbSet<ModuleRolesMapping> ModuleRolesMapping { get; set; }

        public virtual DbSet<UserModuleAccess> UserModuleAccess { get; set; }

        public virtual DbSet<InstallmentMaster> InstallmentMaster { get; set; }

        public virtual DbSet<PaymentModeMaster> PaymentModeMaster { get; set; }

        public virtual DbSet<AllCurrency> Currency { get; set; }

        public virtual DbSet<Nationalities> Nationalities { get; set; }

        public virtual DbSet<ShiftMaster> ShiftMaster { get; set; }

        public virtual DbSet<UserHistory> UserHistory { get; set; }

        public virtual DbSet<UserProfile> UserProfile { get; set; }

        public virtual DbSet<CorporateProfile> CorporateProfile { get; set; }

        public virtual DbSet<UserFamilyDetails> UserFamilyDetails { get; set; }

        public virtual DbSet<UserEduactionDetails> UserEduactionDetails { get; set; }

        public virtual DbSet<UserIdentificationDetails> UserIdentificationDetails { get; set; }

        public virtual DbSet<UserVehicleDetails> UserVehicleDetails { get; set; }

        public virtual DbSet<UserSocialDetails> UserSocialDetails { get; set; }

        public virtual DbSet<UserExperienceDetails> UserExperienceDetails { get; set; }

        public virtual DbSet<UserSkillDetails> UserSkillDetails { get; set; }

        public virtual DbSet<UserProfileTypeDetails> UserProfileTypeDetails { get; set; }

        public virtual DbSet<UserAddressDetails> UserAddressDetails { get; set; }

        public virtual DbSet<EmployeeBasicDetails> EmployeeBasicDetails { get; set; }

        public virtual DbSet<EmpEducationalDetails> EmpEducationalDetails { get; set; }

        public virtual DbSet<EmpExperienceDetails> EmpExperienceDetails { get; set; }

        public virtual DbSet<EmpIdentificationDetails> EmpIdentificationDetails { get; set; }

        public virtual DbSet<EmpSkillDetails> EmpSkillDetails { get; set; }

        public virtual DbSet<EmpSocialDetails> EmpSocialDetails { get; set; }

        public virtual DbSet<EmployeeBankDetails> EmployeeBankDetails { get; set; }

        public virtual DbSet<EmpAddressDetails> EmpAddressDetails { get; set; }

        public virtual DbSet<Address> Address { get; set; }

        public virtual DbSet<CompanyProfile> CompanyProfile { get; set; }

        public virtual DbSet<BankDetails> BankDetails { get; set; }

        public virtual DbSet<TaxMaster> TaxMaster { get; set; }

        public virtual DbSet<GenerateInvoice> GenerateInvoice { get; set; }

        public virtual DbSet<InvoiceTaxationDetails> InvoiceTaxationDetails { get; set; }

        public virtual DbSet<InvoiceItems> InvoiceItems { get; set; }

        public virtual DbSet<CourseMaster> CourseMaster { get; set; }

        public virtual DbSet<JobOrderTypeMaster> JobOrderTypeMaster { get; set; }

        public virtual DbSet<JobOrder> JobOrder { get; set; }

        public virtual DbSet<JobOrderItems> JobOrderItems { get; set; }

        public virtual DbSet<TaskMaster> TaskMaster { get; set; }

        public virtual DbSet<TaskItems> TaskItems { get; set; }

        public virtual DbSet<ProgrammeMaster> ProgrammeMaster { get; set; }

        public virtual DbSet<ProgrammePermissions> ProgrammePermissions { get; set; }

        public virtual DbSet<EducationLevelMaster> EducatioanLevelMaster { get; set; }

        public virtual DbSet<IdentificationTypeMaster> IdentificationTypeMaster { get; set; }

        public virtual DbSet<CourseBatch> CourseBatch { get; set; }

        public virtual DbSet<CandidateCourseDetails> CandidateCourseDetails { get; set; }

        public virtual DbSet<FeeDetails> FeeDetails { get; set; }

        public virtual DbSet<TrainingSchedule> TrainingSchedule { get; set; }

        public virtual DbSet<TrainerPlanner> TrainerPlanner { get; set; }

        public virtual DbSet<TrainerPlannerAttachment> TrainerPlannerAttachment { get; set; }

        public virtual DbSet<CandidateAttendance> CandidateAttendance { get; set; }

        public virtual DbSet<TrainerComments> TrainerComments { get; set; }

        public virtual DbSet<LectureMaster> LectureMaster { get; set; }

        public virtual DbSet<TopicLectures> TopicLectures { get; set; }

        public virtual DbSet<DiscussionForum> DiscussionForum { get; set; }

        public virtual DbSet<TopicMaster> TopicMaster { get; set; }

        public virtual DbSet<COURSETOPICS> COURSETOPICS { get; set; }

        public virtual DbSet<UserTopicStatus> UserTopicStatus { get; set; }

        public virtual DbSet<UserLectureStatus> UserLectureStatus { get; set; }

        public virtual DbSet<UserCourseSubscription> UserCourseSubscription { get; set; }

        public virtual DbSet<ReviewReply> ReviewReply { get; set; }

        public virtual DbSet<UserNotification> UserNotification { get; set; }

        public virtual DbSet<LectureContentUpload> LectureContentUpload { get; set; }

        public virtual DbSet<CheckInCheckOut> CheckInCheckOut { get; set; }

        public virtual DbSet<ItemTypeMasters> ItemTypeMasters { get; set; }

        public virtual DbSet<MeetingMinutes> MeetingMinutes { get; set; }

        public virtual DbSet<EngagementTypeMaster> EngagementTypeMaster { get; set; }

        public virtual DbSet<JobOrderProgressReport> JobOrderProgressReport { get; set; }

        public virtual DbSet<TaskProgressReport> TaskProgressReport { get; set; }

        public virtual DbSet<InVoiceAttachment> InVoiceAttachment { get; set; }

        public virtual DbSet<JobOrderAttachment> JobOrderAttachment { get; set; }

        public virtual DbSet<TaskAttachment> TaskAttachment { get; set; }

        public virtual DbSet<JOCommentsForum> JOCommentsForum { get; set; }

        public virtual DbSet<JOReplyForum> JOReplyForum { get; set; }

        public virtual DbSet<TaskCommentsForum> TaskCommentsForum { get; set; }

        public virtual DbSet<TaskReplyForum> TaskReplyForum { get; set; }

        public virtual DbSet<CourseAttachment> CourseAttachment { get; set; }

        public virtual DbSet<TrainingScheduleAttachment> TrainingScheduleAttachment { get; set; }

        public virtual DbSet<CandidateLeads> CandidateLeads { get; set; }

        //changes by Achal Jha 13-05-2017

        public virtual DbSet<EmployeeAssetsIssueDetails> EmployeeAssetsIssueDetails { get; set; }

        public virtual DbSet<EmployeePayrollDetails> EmployeePayrollDetails { get; set; }

        //public virtual DbSet<EmployeeLeaves> EmployeeLeaves { get; set; }

        public virtual DbSet<GradeMaster> GradeMaster { get; set; }

        public virtual DbSet<AssetsMaster> AssetsMaster { get; set; }

        public virtual DbSet<EmployeeLoanDetails> EmployeeLoanDetails { get; set; }


        //Createdby:- Ajay Kumar Choudhary Created on :- 18-05-2017
        public virtual DbSet<ClientTeamRoles> ClientTeamRoles { get; set; }

        public virtual DbSet<ClientTeamRights> ClientTeamRights { get; set; }

        public virtual DbSet<ClientTeamMemberProfile> ClientTeamMemberProfile { get; set; }

        public virtual DbSet<ClientTeamMemberRights> ClientTeamMemberRights { get; set; }

        public virtual DbSet<Holiday> Holiday { get; set; }

        //Start
        //Createdby Ajay Kumar Choudhary Created on :- 29-05-2017
        //Reason: For TrainerFeedback
        public virtual DbSet<QuestionMaster> QuestionMaster { get; set; }

        public virtual DbSet<BranchDetails> BranchDetails { get; set; }

        public virtual DbSet<FeedBack> FeedBack { get; set; }

        public virtual DbSet<TrainerAssign> TrainerAssign { get; set; }
        //END

        //Start
        //Created by Achal Jha Created on :- 29-05-2017
        //Reason : Payroll Details
        public virtual DbSet<PayrollHeads> PayrollHeads { get; set; }

        public virtual DbSet<PayrollLeavsSettings> PayrollLeavsSettings { get; set; }

        public virtual DbSet<PayrollHeadSettings> PayrollHeadSettings { get; set; }

        public virtual DbSet<EmployeePayroll> EmployeePayroll { get; set; }

        public virtual DbSet<EmployeePayrollSettings> EmployeePayrollSettings { get; set; }

        //End

        //Createdby Ajay Kumar Choudhary Created on :- 29-05-2017
        //Reason: For Laguages
        public virtual DbSet<Languages> Languages { get; set; }

        public virtual DbSet<AdminLogoFile> AdminLogoFile { get; set; }

        public virtual DbSet<TrainingScheduleFinalAttachment> TrainingScheduleFinalAttachment { get; set; }

        public virtual DbSet<JobOrderFinalAttachment> JobOrderFinalAttachment { get; set; }

        public virtual DbSet<TaskFinalAttachment> TaskFinalAttachment { get; set; }

        public virtual DbSet<TrainingAssessment> TrainingAssessment { get; set; }

        //public virtual DbSet<TrainingStatus> TrainingStatus { get; set; }

        public virtual DbSet<AssessmentEvaluation> AssessmentEvaluation { get; set; }

        public virtual DbSet<Certification> Certification { get; set; }


        //Createdby Ajay Kumar Choudhary Created on :- 29-05-2017
        //Reason: For Plan Pricing
        public virtual DbSet<Plan> Plan { get; set; }

        public virtual DbSet<PaymentTransaction> PaymentTransaction { get; set; }

        public virtual DbSet<UserPlan> UserPlan { get; set; }

        public virtual DbSet<Features> Features { get; set; }

        public virtual DbSet<PlanFeatures> PlanFeatures { get; set; }

        public virtual DbSet<PlanAddOns> PlanAddOns { get; set; }

        public virtual DbSet<UserPlanAddOns> UserPlanAddOns { get; set; }

        public virtual DbSet<InstallmentDetails> InstallmentDetails { get; set; }
        //END

        //BEGIN
        /// <summary>
        /// Created By: Kuleshwar Sahu On 31-Jul-2017
        /// Course category to manage category of course (course Type)
        /// </summary>
        public virtual DbSet<Category> Category { get; set; }
        //END


        //BEGIN
        /// <summary>
        /// Created By: Vikash Das on 04-08-2017
        /// Corporate Letter Format
        /// </summary>
        /// <param name="modelBuilder"></param>

        public virtual DbSet<LetterLogoAttachment> LetterLogoAttachment { get; set; }

        public virtual DbSet<CorporateTemplate> CorporateTemplate { get; set; }

        public virtual DbSet<CorporateLetter> CorporateLetter { get; set; }

        //END

        /// <summary>
        /// Created By:- Preeti Singh on 16-08-2017
        /// Employee joining related Detail
        /// </summary>

        public virtual DbSet<EmpJoiningDetail> EmpJoiningDetail { get; set; }

        public virtual DbSet<Designation> Designation { get; set; }

        public virtual DbSet<StatusMaster> StatusMaster { get; set; }

        public virtual DbSet<EmployeeImage> EmployeeImage { get; set; }

        public virtual DbSet<EmpDesignationHistory> EmpDesignationHistory { get; set; }

        /// <summary>
        /// created by Rahul Haldkar
        /// created on 18-08-2017
        /// Asset Masters
        /// </summary>
        /// <param name="modelBuilder"></param>
        public virtual DbSet<AssetGroup> AssetGroup { get; set; }

        public virtual DbSet<AssetType> AssetType { get; set; }

        /// <summary>
        /// Created By:- Anamika Pandey on 20-08-2017
        /// Asset status related master
        /// </summary>


        public virtual DbSet<AssetStatus> AssetStatus { get; set; }

        /// <summary>
        /// Created By:- Anamika Pandey on 21-08-2017
        /// Relation master for super admin
        /// </summary>

        public virtual DbSet<Relation> Relation { get; set; }

        /// <summary>
        /// Created By:- Anamika Pandey on 22-08-2017
        /// Leaving Reason master
        /// </summary>

        public virtual DbSet<LeavingReason> LeavingReason { get; set; }

        public virtual DbSet<ClaimCategory> ClaimCategory { get; set; }

        /// <summary>
        /// Created By:- Anamika Pandey on 23-08-2017
        /// HaldSalaryReason master
        /// </summary>

        public virtual DbSet<HoldSalaryReason> HoldSalaryReason { get; set; }

        public virtual DbSet<PayRollLookUps> PayRollLookUps { get; set; }

        /// <summary>
        /// Create By: Vikash Das
        /// </summary>
        public virtual DbSet<LetterType> LetterType { get; set; }

        public virtual DbSet<SendLetter> SendLetter { get; set; }

        /// <summary>
        /// By: Ajay Kumar Choudhary
        /// For: Biometric Record
        /// On: 06-11-2017
        /// </summary> 
        public virtual DbSet<BiometricCheckInCheckOut> BiometricCheckInCheckOut { get; set; }

        public virtual DbSet<IpMasters> IpMasters { get; set; }

        /// <summary>
        /// By: vikas pandey
        /// For: Leave scheme and leavetype
        /// On: 17-11-2017
        /// </summary>
        public virtual DbSet<LeaveSchemeMaster> LeaveSchemeMaster { get; set; }

        public virtual DbSet<LeaveType> LeaveType { get; set; }

        public virtual DbSet<TrainerPlannerSummary> TrainerPlannerSummary { get; set; }

        public virtual DbSet<CorporatePayrollSettings> CorporatePayrollSettings { get; set; }

        public virtual DbSet<CorporatePayrollHead> CorporatePayrollHead { get; set; }

        public virtual DbSet<EmployeeTour> EmployeeTour { get; set; }

        public virtual DbSet<TourAttachment> TourAttachment { get; set; }

        public virtual DbSet<EmployeeSalary> EmployeeSalary { get; set; }

        public virtual DbSet<EmployeeSalaryHeads> EmployeeSalaryHeads { get; set; }

        public virtual DbSet<AdditionalCourseFee> AdditionalCourseFee { get; set; }

        public virtual DbSet<EmployeeMonthlySalaryPayout> EmployeeMonthlySalaryPayout { get; set; }

        public virtual DbSet<EmployeeMonthlySalaryHeads> EmployeeMonthlySalaryHeads { get; set; }

        public virtual DbSet<IncomeTaxSlab> IncomeTaxSlab { get; set; }

        //created by vikas pandey 24/11/2017
        public virtual DbSet<CompanySetting> CompanySetting { get; set; }

        public virtual DbSet<Resignation> Resignation { get; set; }

        public virtual DbSet<TrackerReport> TrackerReport { get; set; }

        public virtual DbSet<TrackerReportBatchWise> TrackerReportBatchWise { get; set; }

        public virtual DbSet<InstructorLeadProfile> InstructorLeadProfile { get; set; }

        public virtual DbSet<InstructorAttachment> InstructorAttachment { get; set; }

        public virtual DbSet<QualificationMaster> QualificationMaster { get; set; }

        public virtual DbSet<DomainMaster> DomainMaster { get; set; }

        public virtual DbSet<SpecializationMaster> SpecializationMaster { get; set; }

        public virtual DbSet<ProjectMaster> ProjectMaster { get; set; }

        public virtual DbSet<OrganizationMaster> OrganizationMaster { get; set; }

        public virtual DbSet<HelpLineLayers> HelpLineLayers { get; set; }

        public virtual DbSet<HelpLineLayerDetails> HelpLineLayerDetails { get; set; }

        public virtual DbSet<HelpLineTracker> HelpLineTracker { get; set; }

        public virtual DbSet<CircleMaster> CircleMaster { get; set; }

        public virtual DbSet<BranchMaster> BranchMaster { get; set; }

        public virtual DbSet<OPSData> OPSData { get; set; }

        public virtual DbSet<CertifiedData> CertifiedData { get; set; }

        public virtual DbSet<EmployeeConfirmation> EmployeeConfirmation { get; set; }

        public virtual DbSet<SubscriberLogInHistory> SubscriberLogInHistory { get; set; }

        //TMS LITE

        public virtual DbSet<Course> Course { get; set; }

        public virtual DbSet<Subject> Subject { get; set; }

        public virtual DbSet<CourseSubject> CourseSubject { get; set; }

        public virtual DbSet<Batches> Batches { get; set; }

        public virtual DbSet<MTTrainingOrder> MTTrainingOrder { get; set; }

        public virtual DbSet<MTTrainingOrderTraineeDetails> MTTrainingOrderTraineeDetails { get; set; }

        public virtual DbSet<EndUserTrainingOrder> EndUserTrainingOrder { get; set; }

        public virtual DbSet<EnduUserTrainee> EnduUserTrainee { get; set; }

        public virtual DbSet<DeviceDetail> DeviceDetail { get; set; }

        public virtual DbSet<CorporateBranch> CorporateBranch { get; set; }

        public virtual DbSet<RegionMasters> RegionMasters { get; set; }

        public virtual DbSet<DivisionMasters> DivisionMasters { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
