using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AJSolutions.Models
{

    public class CourseBatch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 BatchId { get; set; }

        [Required]
        [StringLength(16)]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(128)]
        public string BatchName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime FromTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime ToTime { get; set; }

        [DefaultValue(false)]
        public bool IsDailyAttendence { get; set; }

        [DefaultValue(false)]
        public bool IsFeedbackRequired { get; set; }

        [StringLength(1024)]
        public string FeedbackLink { get; set; }

        [StringLength(1024)]
        public string ContentLink { get; set; }

        [DefaultValue(false)]
        public bool ContentAvailability { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AvailableTillDate { get; set; }

        [ForeignKey("CourseCode")]
        public virtual CourseMaster CourseMaster { get; set; }

        [DefaultValue(false)]
        public bool AccomondationNeeded { get; set; }

        [DefaultValue(false)]
        public bool AttendenceNeeded { get; set; }

        [StringLength(128)]
        public string WardenId { get; set; }

        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }
    }

    public class CandidateCourseDetails
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int64 BatchId { get; set; }

        public Int16 InstallmentId { get; set; }

        [DefaultValue(false)]
        public bool Like { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        [ForeignKey("BatchId")]
        public virtual CourseBatch CourseBatch { get; set; }

        [ForeignKey("InstallmentId")]
        public virtual InstallmentMaster InstallmentMaster { get; set; }

    }

    public class CandidateCourseDetailsView
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        public Int64 BatchId { get; set; }

        public Int16 InstallmentId { get; set; }

        [DefaultValue(false)]
        public bool Like { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        [ForeignKey("BatchId")]
        [StringLength(128)]
        public virtual CourseBatch CourseBatch { get; set; }

        [StringLength(16)]
        public string UserName { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        public DateTime? CheckInDate { get; set; }

        public string BatchName { get; set; }

        [ForeignKey("InstallmentId")]
        public virtual InstallmentMaster InstallmentMaster { get; set; }

        public string CourseCode { get; set; }

        public string CourseName { get; set; }
    }

    public class FeeDetails
    {
        [Key]
        [StringLength(32)]
        public string TransactionId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(16)]
        public string CourseCode { get; set; }

        public Int64 BatchId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PaymentDate { get; set; }

        public float FeePaid { get; set; }

        public float ConveyanceFee { get; set; }

        public float RemainingAmount { get; set; }

        [StringLength(64)]
        public string ReferenceNumber { get; set; }

        public Int16 PaymentModeId { get; set; }

        [StringLength(64)]
        public string BankName { get; set; }

        [StringLength(32)]
        public string Status { get; set; }

        [StringLength(16)]
        public string BankCode { get; set; }

        [StringLength(256)]
        public string Remarks { get; set; }

        public string PGComment { get; set; }

        [StringLength(128)]
        public string ApprovedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public Int16 TotalInstallment { get; set; }

        public Int16 InstallmentNumber { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        [ForeignKey("CourseCode")]
        public virtual CourseMaster CourseMaster { get; set; }

        [ForeignKey("PaymentModeId")]
        public virtual PaymentModeMaster PaymentModeMaster { get; set; }
    }

    public class FeeDetailsView
    {
        [Key]
        [StringLength(32)]
        public string TransactionId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(16)]
        public string CourseCode { get; set; }

        public Int64 BatchId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PaymentDate { get; set; }

        public float FeePaid { get; set; }

        public double TotalFees { get; set; }

        public double TotalFeeAmount { get; set; }

        [StringLength(64)]
        public string ReferenceNumber { get; set; }

        public Int16 PaymentModeId { get; set; }

        [StringLength(64)]
        public string BankName { get; set; }

        [StringLength(32)]
        public string Status { get; set; }

        [StringLength(16)]
        public string BankCode { get; set; }

        [StringLength(256)]
        public string Remarks { get; set; }

        public string PGComment { get; set; }

        public string ApprovedByName { get; set; }

        public string PaymentMode { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        public double CourseFee { get; set; }

        public double TotalFeePaid { get; set; }

        public string CourseName { get; set; }

        public string BatchName { get; set; }

        public string CompanyName { get; set; }

        public string Installment { get; set; }

        public float RemainingAmount { get; set; }

        [StringLength(128)]
        public string ApprovedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public Int16 TotalInstallment { get; set; }

        public Int16 InstallmentNumber { get; set; }

        public float ConveyanceFee { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        [ForeignKey("CourseCode")]
        public virtual CourseMaster CourseMaster { get; set; }

        [ForeignKey("PaymentModeId")]
        public virtual PaymentModeMaster PaymentModeMaster { get; set; }
    }

    /// <summary>
    /// By: Ajay Kumar Choudhary
    /// on: 17-07-2017
    /// For: Adding Assessment in Training Schedule
    /// </summary>
    //Start
    public class TrainingAssessment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 AssessmentId { get; set; }

        [StringLength(128)]
        public string Assessment { get; set; }

        public Int64 Weightage { get; set; }

        [StringLength(16)]
        public string TrainingId { get; set; }

        [StringLength(128)]
        public string PublicationId { get; set; }

        [StringLength(256)]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; }

        [ForeignKey("TrainingId")]
        public virtual TrainingSchedule TrainingSchedule { get; set; }
    }

    public class TrainingAssessmentView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 AssessmentId { get; set; }

        [StringLength(128)]
        public string Assessment { get; set; }

        [DefaultValue(0)]
        public Int64 Weightage { get; set; }

        [StringLength(16)]
        public string TrainingId { get; set; }

        public string TrainerId { get; set; }

        public Int64 BatchId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Name { get; set; }

        public string SubjectLine { get; set; }

        public string PublicationId { get; set; }

        public string Title { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; }

        [DefaultValue(0)]
        public float Percentage { get; set; }
    }

    public class AssessmentEvaluation
    {
        [Key]
        [Column(Order = 0)]
        public Int64 AssessmentId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(16)]
        public string TrainingId { get; set; }

        public float Percentage { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        [ForeignKey("AssessmentId")]
        public virtual TrainingAssessment TrainingAssessment { get; set; }
    }

    public class AssessmentEvaluationView
    {
        [Key]
        [Column(Order = 0)]
        public Int64 AssessmentId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(16)]
        public string TrainingId { get; set; }

        public float Percentage { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        public Int64 Weightage { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        [ForeignKey("AssessmentId")]
        public virtual TrainingAssessment TrainingAssessment { get; set; }
    }
    //End

    public class TrainingSchedule
    {
        [Key]
        [StringLength(16)]
        public string TrainingId { get; set; }

        public Int64 BatchId { get; set; }

        [Required]
        [StringLength(512)]
        public string SubjectLine { get; set; }

        [Required]
        public string Description { get; set; }

        [StringLength(128)]
        public string TrainerId { get; set; }

        public string OtherTrainerId { get; set; }

        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        [StringLength(512)]
        public string Address { get; set; }

        [StringLength(16)]
        public string Status { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedOn { get; set; }

        [StringLength(32)]
        public string TaskId { get; set; }

        [StringLength(128)]
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [StringLength(128)]
        public string TrainerMentor { get; set; }

        [ForeignKey("BatchId")]
        public virtual CourseBatch CourseBatch { get; set; }

        [ForeignKey("CountryId")]
        public virtual CountryMaster CountryMaster { get; set; }

        [ForeignKey("StateId")]
        public virtual StatesMaster StateMaster { get; set; }

        [ForeignKey("CityId")]
        public virtual CityMaster CityMaster { get; set; }

    }

    //public class TrainingStatus
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    //    public Int64 StatusId { get; set; }

    //    [StringLength(16)]
    //    public string TrainingId { get; set; }

    //    [StringLength(16)]
    //    public string StatusFrom { get; set; }

    //    [StringLength(16)]
    //    public string StatusTo { get; set; }

    //    [StringLength(128)]
    //    public string Updatedby { get; set; }

    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //    public DateTime UpdatedOn { get; set; }

    //}

    //public class TaskTrainingCount
    //{
    //    public Int32 TotalTrainings { get; set; }
    //}

    public class TrainerPlanner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 PlannerId { get; set; }

        [Required]
        [StringLength(128)]
        public string TrainerId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime FromTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime ToTime { get; set; }

        [StringLength(512)]
        public string Remarks { get; set; }

        public Int64 EngagementTypeId { get; set; }

        [ForeignKey("EngagementTypeId")]
        public virtual EngagementTypeMaster EngagementTypeMaster { get; set; }

        [DefaultValue(false)]
        public Boolean HalfDay { get; set; }

        public Int64 IsApproved { get; set; }

        [StringLength(128)]
        public string ApprovedBy { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? ApprovalDate { get; set; }

        [DefaultValue(0)]
        public Int16 SchemeId { get; set; }

    }

    public partial class TrainerPlannerAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        public Int64 PlannerId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [ForeignKey("PlannerId")]
        public virtual TrainerPlanner TrainerPlanner { get; set; }
    }

    //Trainner Planner ViewModel
    public class TrainerPlannerView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 PlannerId { get; set; }

        [Required]
        [StringLength(128)]
        public string TrainerId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime FromTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime ToTime { get; set; }

        [StringLength(512)]
        public string Remarks { get; set; }

        public string Reson { get; set; }
        public Int64 EngagementTypeId { get; set; }

        public string EngagementType { get; set; }

        public string Name { get; set; }

        [ForeignKey("EngagementTypeId")]
        public virtual EngagementTypeMaster EngagementTypeMaster { get; set; }

        [DefaultValue(false)]
        public Boolean HalfDay { get; set; }

        public Int64 IsApproved { get; set; }

        [StringLength(128)]
        public string ApprovedBy { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? ApprovalDate { get; set; }

        public string ApprovedByName { get; set; }

        public string LeaveTypeCategory { get; set; }

        public string EmployeeName { get; set; }

        public int TotalDays { get; set; }

        public Int16 SchemeId { get; set; }

        public float LeaveLimit { get; set; }

        public Int64 TourId { get; set; }
    }

    /// <summary>
    /// CreatedBy: Ajay Kumar Choudhary
    /// CreatedOn: 22-5-2017
    /// </summary>
    public class Holiday
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 HolidayId { get; set; }

        [Required]
        [StringLength(128)]
        public string CorporateId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        [StringLength(512)]
        public string Remarks { get; set; }

        [StringLength(32)]
        public string HolidayType { get; set; }

    }

    public class CandidateAttendanceView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 AttendenceId { get; set; }

        [Required]
        [StringLength(16)]
        public string TrainingId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }


        [StringLength(256)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AttendenceDate { get; set; }

        public string IsPresent { get; set; }

        [StringLength(256)]
        public string Remarks { get; set; }

        public string Attendence { get; set; }


    }

    public class CandidateTraining
    {
        public string TrainingId { get; set; }

        public string TrainingName { get; set; }

        public Int64 BatchId { get; set; }

        public string BatchName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        public string TrainerId { get; set; }

        public string TrainerName { get; set; }

        public string Status { get; set; }

        public int TotalStudent { get; set; }

        public List<CandidateAttendanceView> CandidateList { get; set; }
    }

    public class CandidateAttendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 AttendenceId { get; set; }

        [Required]
        [StringLength(16)]
        public string TrainingId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AttendenceDate { get; set; }

        [Required]
        [StringLength(1)]
        public string IsPresent { get; set; }

        [StringLength(256)]
        public string Remarks { get; set; }

        [Required]
        [StringLength(16)]
        public string Sessions { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        [ForeignKey("TrainingId")]
        public virtual TrainingSchedule TrainingSchedule { get; set; }

    }

    public class TrainerComments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 CommentId { get; set; }

        [Required]
        [StringLength(16)]
        public string TrainingId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CommentDate { get; set; }

        [StringLength(256)]
        public string Comment { get; set; }

        [ForeignKey("TrainingId")]
        public virtual TrainingSchedule TrainingSchedule { get; set; }
    }

    public class TrainingScheduleCityView
    {
        public int CityId { get; set; }
        public string City { get; set; }
    }

    public class TrainingScheduleView
    {
        [Key]
        [StringLength(16)]
        public string TrainingId { get; set; }

        public Int64 BatchId { get; set; }

        public string CLientName { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        [StringLength(256)]
        public string CourseName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        [Required]
        [StringLength(512)]
        public string SubjectLine { get; set; }

        [Required]
        public string Description { get; set; }

        [StringLength(128)]
        public string TrainerId { get; set; }

        public string CorporateId { get; set; }

        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        [StringLength(512)]
        public string Address { get; set; }

        [StringLength(16)]
        public string Status { get; set; }

        [StringLength(256)]
        public string BatchName { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        public string DepartmentId { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public int TotalPresent { get; set; }

        public int TotalAbsent { get; set; }

        public int TotalStudent { get; set; }

        [StringLength(128)]
        public string CreatedBy { get; set; }

        [StringLength(128)]
        public string CreatedName { get; set; }

        public string OtherTrainerId { get; set; }

        public string TrainerName { get; set; }

        public int TrainersCount { get; set; }

        [DefaultValue(false)]
        public bool AttendenceNeeded { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedOn { get; set; }

        [StringLength(32)]
        public string TaskId { get; set; }

        public string TrainerMentor { get; set; }

        public string TrainerMentorId { get; set; }

        [StringLength(1024)]
        public string FeedbackLink { get; set; }

        [StringLength(1024)]
        public string ContentLink { get; set; }

        public bool AccomondationNeeded { get; set; }
    }

    public partial class TrainingScheduleAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(16)]
        public string TrainingId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [ForeignKey("TrainingId")]
        public virtual TrainingSchedule TrainingSchedule { get; set; }
    }

    public partial class TrainingScheduleFinalAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(16)]
        public string TrainingId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [ForeignKey("TrainingId")]
        public virtual TrainingSchedule TrainingSchedule { get; set; }
    }

    public class CourseBatchViewModel
    {
        [Key]
        public Int64 BatchId { get; set; }

        public string BatchNameDuration { get; set; }

        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        [Required]
        [StringLength(16)]
        public string CourseCode { get; set; }

        [StringLength(32)]
        public string CourseName { get; set; }

        [Required]
        [StringLength(128)]
        public string BatchName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime FromTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime ToTime { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [DefaultValue(false)]
        public bool IsDailyAttendence { get; set; }

        [DefaultValue(false)]
        public bool IsFeedbackRequired { get; set; }

        [StringLength(1024)]
        public string FeedbackLink { get; set; }

        [StringLength(1024)]
        public string ContentLink { get; set; }

        public double CourseFee { get; set; }

        public double TotalFees { get; set; }

        public double TotalFeeAmount { get; set; }

        public Int16 InstallmentId { get; set; }

        public bool Like { get; set; }

        [StringLength(32)]
        public string Installment { get; set; }

        [DefaultValue(false)]
        public bool ContentAvailability { get; set; }

        [DefaultValue(false)]
        public bool AccomondationNeeded { get; set; }

        [DefaultValue(false)]
        public bool AttendenceNeeded { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AvailableTillDate { get; set; }

        public double TotalFeePaid { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fdate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Tdate { get; set; }

        [ForeignKey("CourseCode")]
        public virtual CourseMaster CourseMaster { get; set; }

        public virtual CorporateProfile CorporateProfile { get; set; }

        [StringLength(128)]
        public string WardenId { get; set; }

        public int CandidateCount { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string LMSCourseCode { get; set; }

        public string LastInstallmentStatus { get; set; }
        //public int TotalTopics { get; set; }

        //public int TotalLecture { get; set; }
    }

    public class CheckInCheckOut
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 StudentId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public Int64 BatchId { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime CheckInDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? CheckOutDate { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        [ForeignKey("BatchId")]
        public virtual CourseBatch CourseBatch { get; set; }
    }

    public class CheckInCheckOutView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 StudentId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        public string Name { get; set; }
        [StringLength(128)]

        public string CourseCode { get; set; }

        public string CourseName { get; set; }

        public string BatchName { get; set; }

        public Int64 BatchId { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime CheckInDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? CheckOutDate { get; set; }

        [StringLength(128)]
        public string WardenId { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }
    }

    public class CandidateCredentialsView
    {
        public string Name { get; set; }

        public string UserName { get; set; }

        public string RegistrationId { get; set; }

        public string Password { get; set; }

        public string UserId { get; set; }
    }

    public class EmloyeeLeaves
    {
        public float TotalLeaves { get; set; }

        public float OutstandingLeaves { get; set; }
    }

    public class EmployeeTour
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TourId { get; set; }

        public Int64 PlannerId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string Purpose { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime TourFromDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime TourToDate { get; set; }

        [StringLength(256)]
        public string Location { get; set; }

        [StringLength(1024)]
        public string Observations { get; set; }

        [StringLength(1024)]
        public string Conclusion { get; set; }

        [StringLength(16)]
        public string ContactNumber { get; set; }

    }

    public class TourAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        public Int64 TourId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [ForeignKey("TourId")]
        public virtual EmployeeTour EmployeeTour { get; set; }

    }

    public class EmployeeTourView
    {
        public Int64 TourId { get; set; }

        public Int64 PlannerId { get; set; }

        public string UserId { get; set; }

        public string Purpose { get; set; }

        public DateTime TourFromDate { get; set; }

        public DateTime TourToDate { get; set; }

        public string Location { get; set; }

        public string Observations { get; set; }

        public string Conclusion { get; set; }

        public string ContactNumber { get; set; }

        public Int64 FileId { get; set; }
    }

    public class TrackerReport
    {
        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        public Int32 Phase { get; set; }

        [StringLength(64)]
        public string Answer1 { get; set; }
        [StringLength(64)]
        public string Answer2 { get; set; }
        [StringLength(64)]
        public string Answer3 { get; set; }
        [StringLength(64)]
        public string Answer4 { get; set; }
        [StringLength(64)]
        public string Answer5 { get; set; }
        [StringLength(64)]
        public string Answer6 { get; set; }
        [StringLength(64)]
        public string Answer7 { get; set; }
        [StringLength(64)]
        public string Answer8 { get; set; }
        [StringLength(64)]
        public string Answer9 { get; set; }
        [StringLength(64)]
        public string Answer10 { get; set; }
        [StringLength(64)]
        public string Answer11 { get; set; }
        [StringLength(64)]
        public string Answer12 { get; set; }
        [StringLength(64)]
        public string Answer13 { get; set; }
        [StringLength(64)]
        public string Answer14 { get; set; }
        [StringLength(64)]
        public string Answer15 { get; set; }
        [StringLength(64)]
        public string Answer16 { get; set; }
        [StringLength(64)]
        public string Answer17 { get; set; }
        [StringLength(64)]
        public string Answer18 { get; set; }
        [StringLength(64)]
        public string Answer19 { get; set; }
        [StringLength(64)]
        public string Answer20 { get; set; }
        [StringLength(64)]
        public string Answer21 { get; set; }
        [StringLength(64)]
        public string Answer22 { get; set; }
        [StringLength(64)]
        public string Answer23 { get; set; }
        [StringLength(64)]
        public string Answer24 { get; set; }
        [StringLength(64)]
        public string Answer25 { get; set; }
        [StringLength(64)]
        public string Answer26 { get; set; }
        [StringLength(64)]
        public string Answer27 { get; set; }
        [StringLength(64)]
        public string Answer28 { get; set; }
        [StringLength(64)]
        public string Answer29 { get; set; }
        [StringLength(64)]
        public string Answer30 { get; set; }
        [StringLength(64)]
        public string Answer31 { get; set; }
        [StringLength(64)]
        public string Answer32 { get; set; }
        [StringLength(64)]
        public string Answer33 { get; set; }
        [StringLength(64)]
        public string Answer34 { get; set; }
        [StringLength(64)]
        public string Answer35 { get; set; }
        [StringLength(64)]
        public string Answer36 { get; set; }
        [StringLength(64)]
        public string Answer37 { get; set; }
        [StringLength(64)]
        public string Answer38 { get; set; }
        [StringLength(64)]
        public string Answer39 { get; set; }
        [StringLength(64)]
        public string Answer40 { get; set; }
        [StringLength(64)]
        public string Answer41 { get; set; }
        [StringLength(64)]
        public string Answer42 { get; set; }
        [StringLength(64)]
        public string Answer43 { get; set; }
        [StringLength(64)]
        public string Answer44 { get; set; }
        [StringLength(64)]
        public string Answer45 { get; set; }
        [StringLength(64)]
        public string Answer46 { get; set; }
        [StringLength(64)]
        public string Answer47 { get; set; }
        [StringLength(64)]
        public string Answer48 { get; set; }
        [StringLength(64)]
        public string Answer49 { get; set; }
        [StringLength(64)]
        public string Answer50 { get; set; }
        [StringLength(64)]
        public string Answer51 { get; set; }
        [StringLength(64)]
        public string Answer52 { get; set; }
        [StringLength(64)]
        public string Answer54 { get; set; }
        [StringLength(64)]
        public string Answer55 { get; set; }
        [StringLength(64)]
        public string Answer56 { get; set; }
        [StringLength(64)]
        public string Answer57 { get; set; }
        [StringLength(64)]
        public string Answer58 { get; set; }
        [StringLength(64)]
        public string Answer59 { get; set; }
        [StringLength(64)]
        public string Answer60 { get; set; }
        [StringLength(64)]
        public string Answer61 { get; set; }
        [StringLength(64)]
        public string Answer62 { get; set; }
        [StringLength(64)]
        public string Answer63 { get; set; }
        [StringLength(64)]
        public string Answer64 { get; set; }
        [StringLength(64)]
        public string Answer65 { get; set; }
        [StringLength(64)]
        public string Answer66 { get; set; }
        [StringLength(64)]
        public string Answer67 { get; set; }
        [StringLength(64)]
        public string Answer68 { get; set; }
        [StringLength(64)]
        public string Answer69 { get; set; }
        [StringLength(64)]
        public string Answer70 { get; set; }
        [StringLength(64)]
        public string Answer71 { get; set; }
        [StringLength(64)]
        public string Answer72 { get; set; }
        [StringLength(64)]
        public string Answer73 { get; set; }
        [StringLength(64)]
        public string Answer74 { get; set; }
        [StringLength(64)]
        public string Answer75 { get; set; }
        [StringLength(64)]
        public string Answer76 { get; set; }
        [StringLength(64)]
        public string Answer77 { get; set; }
        [StringLength(64)]
        public string Answer78 { get; set; }
        [StringLength(64)]
        public string Answer79 { get; set; }
        [StringLength(64)]
        public string Answer80 { get; set; }
        [StringLength(64)]
        public string Answer81 { get; set; }
        [StringLength(64)]
        public string Answer82 { get; set; }
        [StringLength(64)]
        public string Answer83 { get; set; }
        [StringLength(64)]
        public string Answer84 { get; set; }
        [StringLength(64)]
        public string Answer85 { get; set; }
        [StringLength(64)]
        public string Answer86 { get; set; }
        [StringLength(64)]
        public string Answer87 { get; set; }
        [StringLength(64)]
        public string Answer88 { get; set; }
        [StringLength(64)]
        public string Answer89 { get; set; }
        [StringLength(64)]
        public string Answer90 { get; set; }
        [StringLength(64)]
        public string Answer91 { get; set; }
        [StringLength(64)]
        public string Answer92 { get; set; }
        [StringLength(64)]
        public string Answer93 { get; set; }
        [StringLength(64)]
        public string Answer94 { get; set; }
        [StringLength(64)]
        public string Answer95 { get; set; }

        [StringLength(64)]
        public string Answer96 { get; set; }

        [StringLength(64)]
        public string Answer97 { get; set; }

        public DateTime? FeedbackDate { get; set; }

        public DateTime UpdatedOn { get; set; }
    }

    public class TrackerReportView
    {
        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        public string Name { get; set; }

        public Int32 Phase { get; set; }

        [StringLength(64)]
        public string Answer1 { get; set; }
        [StringLength(64)]
        public string Answer2 { get; set; }
        [StringLength(64)]
        public string Answer3 { get; set; }
        [StringLength(64)]
        public string Answer4 { get; set; }
        [StringLength(64)]
        public string Answer5 { get; set; }
        [StringLength(64)]
        public string Answer6 { get; set; }
        [StringLength(64)]
        public string Answer7 { get; set; }
        [StringLength(64)]
        public string Answer8 { get; set; }
        [StringLength(64)]
        public string Answer9 { get; set; }
        [StringLength(64)]
        public string Answer10 { get; set; }
        [StringLength(64)]
        public string Answer11 { get; set; }
        [StringLength(64)]
        public string Answer12 { get; set; }
        [StringLength(64)]
        public string Answer13 { get; set; }
        [StringLength(64)]
        public string Answer14 { get; set; }
        [StringLength(64)]
        public string Answer15 { get; set; }
        [StringLength(64)]
        public string Answer16 { get; set; }
        [StringLength(64)]
        public string Answer17 { get; set; }
        [StringLength(64)]
        public string Answer18 { get; set; }
        [StringLength(64)]
        public string Answer19 { get; set; }
        [StringLength(64)]
        public string Answer20 { get; set; }
        [StringLength(64)]
        public string Answer21 { get; set; }
        [StringLength(64)]
        public string Answer22 { get; set; }
        [StringLength(64)]
        public string Answer23 { get; set; }
        [StringLength(64)]
        public string Answer24 { get; set; }
        [StringLength(64)]
        public string Answer25 { get; set; }
        [StringLength(64)]
        public string Answer26 { get; set; }
        [StringLength(64)]
        public string Answer27 { get; set; }
        [StringLength(64)]
        public string Answer28 { get; set; }
        [StringLength(64)]
        public string Answer29 { get; set; }
        [StringLength(64)]
        public string Answer30 { get; set; }
        [StringLength(64)]
        public string Answer31 { get; set; }
        [StringLength(64)]
        public string Answer32 { get; set; }
        [StringLength(64)]
        public string Answer33 { get; set; }
        [StringLength(64)]
        public string Answer34 { get; set; }
        [StringLength(64)]
        public string Answer35 { get; set; }
        [StringLength(64)]
        public string Answer36 { get; set; }
        [StringLength(64)]
        public string Answer37 { get; set; }
        [StringLength(64)]
        public string Answer38 { get; set; }
        [StringLength(64)]
        public string Answer39 { get; set; }
        [StringLength(64)]
        public string Answer40 { get; set; }
        [StringLength(64)]
        public string Answer41 { get; set; }
        [StringLength(64)]
        public string Answer42 { get; set; }
        [StringLength(64)]
        public string Answer43 { get; set; }
        [StringLength(64)]
        public string Answer44 { get; set; }
        [StringLength(64)]
        public string Answer45 { get; set; }
        [StringLength(64)]
        public string Answer46 { get; set; }
        [StringLength(64)]
        public string Answer47 { get; set; }
        [StringLength(64)]
        public string Answer48 { get; set; }
        [StringLength(64)]
        public string Answer49 { get; set; }
        [StringLength(64)]
        public string Answer50 { get; set; }
        [StringLength(64)]
        public string Answer51 { get; set; }
        [StringLength(64)]
        public string Answer52 { get; set; }
        [StringLength(64)]
        public string Answer54 { get; set; }
        [StringLength(64)]
        public string Answer55 { get; set; }
        [StringLength(512)]
        public string Answer56 { get; set; }
        [StringLength(64)]
        public string Answer57 { get; set; }
        [StringLength(64)]
        public string Answer58 { get; set; }
        [StringLength(64)]
        public string Answer59 { get; set; }
        [StringLength(64)]
        public string Answer60 { get; set; }
        [StringLength(64)]
        public string Answer61 { get; set; }
        [StringLength(64)]
        public string Answer62 { get; set; }
        [StringLength(64)]
        public string Answer63 { get; set; }
        [StringLength(64)]
        public string Answer64 { get; set; }
        [StringLength(64)]
        public string Answer65 { get; set; }
        [StringLength(64)]
        public string Answer66 { get; set; }
        [StringLength(64)]
        public string Answer67 { get; set; }
        [StringLength(64)]
        public string Answer68 { get; set; }
        [StringLength(64)]
        public string Answer69 { get; set; }
        [StringLength(64)]
        public string Answer70 { get; set; }
        [StringLength(64)]
        public string Answer71 { get; set; }
        [StringLength(64)]
        public string Answer72 { get; set; }
        [StringLength(64)]
        public string Answer73 { get; set; }
        [StringLength(64)]
        public string Answer74 { get; set; }
        [StringLength(64)]
        public string Answer75 { get; set; }
        [StringLength(64)]
        public string Answer76 { get; set; }
        [StringLength(64)]
        public string Answer77 { get; set; }
        [StringLength(64)]
        public string Answer78 { get; set; }
        [StringLength(64)]
        public string Answer79 { get; set; }
        [StringLength(64)]
        public string Answer80 { get; set; }
        [StringLength(64)]
        public string Answer81 { get; set; }
        [StringLength(64)]
        public string Answer82 { get; set; }
        [StringLength(64)]
        public string Answer83 { get; set; }
        [StringLength(64)]
        public string Answer84 { get; set; }
        [StringLength(64)]
        public string Answer85 { get; set; }
        [StringLength(64)]
        public string Answer86 { get; set; }
        [StringLength(64)]
        public string Answer87 { get; set; }
        [StringLength(64)]
        public string Answer88 { get; set; }
        [StringLength(64)]
        public string Answer89 { get; set; }
        [StringLength(64)]
        public string Answer90 { get; set; }
        [StringLength(64)]
        public string Answer91 { get; set; }
        [StringLength(64)]
        public string Answer92 { get; set; }
        [StringLength(64)]
        public string Answer93 { get; set; }
        [StringLength(64)]
        public string Answer94 { get; set; }
        [StringLength(64)]
        public string Answer95 { get; set; }

        [StringLength(64)]
        public string Answer96 { get; set; }

        [StringLength(64)]
        public string Answer97 { get; set; }
        public DateTime? CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }

    public class TrackerReportTotalView
    {
        public int Answer4Total { get; set; }
        public int Answer5Total { get; set; }
        public int Answer6Total { get; set; }
        public int Answer7Total { get; set; }
        public int Answer46Total { get; set; }
        public int Answer47Total { get; set; }
        public int Answer50Total { get; set; }
        public int Answer51Total { get; set; }
        public int Answer68Total { get; set; }
        public int Answer54Total { get; set; }
        public int Answer55Total { get; set; }
        public int Answer56Total { get; set; }
        public int Answer57Total { get; set; }
        public int Answer58Total { get; set; }
        public int Answer59Total { get; set; }
        public int Answer70Total { get; set; }
        public int Answer71Total { get; set; }
        public int Answer72Total { get; set; }
        public int Answer73Total { get; set; }
        public int Answer74Total { get; set; }
        public int Answer75Total { get; set; }
        public int Answer82Total { get; set; }
        public int Answer49Total { get; set; }
        public int Answer61Total { get; set; }
        public int Answer77Total { get; set; }
        public int Answer40Total { get; set; }

        public int Answer38Total { get; set; }
        public int Answer42Total { get; set; }
        public int Answer44Total { get; set; }
        public int Answer76Total { get; set; }
        public int Answer60Total { get; set; }
        public string Circle { get; set; }

    }

    public class TrackerReportBatchWise
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TrackerNumber { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public Int32 Batch { get; set; }

        public Int32 Phase { get; set; }

        [StringLength(64)]
        public string Answer1 { get; set; }
        [StringLength(64)]
        public string Answer2 { get; set; }
        [StringLength(64)]
        public string Answer3 { get; set; }
        [StringLength(64)]
        public string Answer4 { get; set; }
        [StringLength(64)]
        public string Answer5 { get; set; }
        [StringLength(64)]
        public string Answer6 { get; set; }
        [StringLength(64)]
        public string Answer7 { get; set; }
        [StringLength(64)]
        public string Answer8 { get; set; }
        [StringLength(64)]
        public string Answer9 { get; set; }
        [StringLength(64)]
        public string Answer10 { get; set; }
        [StringLength(64)]
        public string Answer11 { get; set; }
        [StringLength(64)]
        public string Answer12 { get; set; }
        [StringLength(64)]
        public string Answer13 { get; set; }
        [StringLength(64)]
        public string Answer14 { get; set; }
        [StringLength(64)]
        public string Answer15 { get; set; }
        [StringLength(64)]
        public string Answer16 { get; set; }
        [StringLength(64)]
        public string Answer17 { get; set; }
        [StringLength(64)]
        public string Answer18 { get; set; }
        [StringLength(64)]
        public string Answer19 { get; set; }
        [StringLength(64)]
        public string Answer20 { get; set; }
        [StringLength(64)]
        public string Answer21 { get; set; }
        [StringLength(64)]
        public string Answer22 { get; set; }
        [StringLength(64)]
        public string Answer23 { get; set; }
        [StringLength(64)]
        public string Answer24 { get; set; }
        [StringLength(64)]
        public string Answer25 { get; set; }
        [StringLength(64)]
        public string Answer26 { get; set; }
        [StringLength(64)]
        public string Answer27 { get; set; }
        [StringLength(64)]
        public string Answer28 { get; set; }
        [StringLength(64)]
        public string Answer29 { get; set; }
        [StringLength(64)]
        public string Answer30 { get; set; }
        [StringLength(64)]
        public string Answer31 { get; set; }
        [StringLength(64)]
        public string Answer32 { get; set; }
        [StringLength(64)]
        public string Answer33 { get; set; }
        [StringLength(64)]
        public string Answer34 { get; set; }
        [StringLength(64)]
        public string Answer35 { get; set; }
        [StringLength(64)]
        public string Answer36 { get; set; }
        [StringLength(64)]
        public string Answer37 { get; set; }
        [StringLength(64)]
        public string Answer38 { get; set; }
        [StringLength(64)]
        public string Answer39 { get; set; }
        [StringLength(64)]
        public string Answer40 { get; set; }
        [StringLength(64)]
        public string Answer41 { get; set; }
        [StringLength(64)]
        public string Answer42 { get; set; }
        [StringLength(64)]
        public string Answer43 { get; set; }
        [StringLength(64)]
        public string Answer44 { get; set; }
        [StringLength(64)]
        public string Answer45 { get; set; }
        [StringLength(64)]
        public string Answer46 { get; set; }
        [StringLength(64)]
        public string Answer47 { get; set; }
        [StringLength(64)]
        public string Answer48 { get; set; }
        [StringLength(64)]
        public string Answer49 { get; set; }
        [StringLength(64)]
        public string Answer50 { get; set; }
        [StringLength(64)]
        public string Answer51 { get; set; }
        [StringLength(64)]
        public string Answer52 { get; set; }
        [StringLength(64)]
        public string Answer54 { get; set; }
        [StringLength(64)]
        public string Answer55 { get; set; }
        [StringLength(64)]
        public string Answer56 { get; set; }
        [StringLength(64)]
        public string Answer57 { get; set; }
        [StringLength(64)]
        public string Answer58 { get; set; }
        [StringLength(64)]
        public string Answer59 { get; set; }
        [StringLength(64)]
        public string Answer60 { get; set; }
        [StringLength(64)]
        public string Answer61 { get; set; }
        [StringLength(64)]
        public string Answer62 { get; set; }
        [StringLength(64)]
        public string Answer63 { get; set; }
        [StringLength(64)]
        public string Answer64 { get; set; }
        [StringLength(64)]
        public string Answer65 { get; set; }
        [StringLength(64)]
        public string Answer66 { get; set; }
        [StringLength(64)]
        public string Answer67 { get; set; }
        [StringLength(64)]
        public string Answer68 { get; set; }
        [StringLength(64)]
        public string Answer69 { get; set; }
        [StringLength(64)]
        public string Answer70 { get; set; }
        [StringLength(64)]
        public string Answer71 { get; set; }
        [StringLength(64)]
        public string Answer72 { get; set; }
        [StringLength(64)]
        public string Answer73 { get; set; }
        [StringLength(64)]
        public string Answer74 { get; set; }
        [StringLength(64)]
        public string Answer75 { get; set; }
        [StringLength(64)]
        public string Answer76 { get; set; }
        [StringLength(64)]
        public string Answer77 { get; set; }
        [StringLength(64)]
        public string Answer78 { get; set; }
        [StringLength(64)]
        public string Answer79 { get; set; }
        [StringLength(64)]
        public string Answer80 { get; set; }
        [StringLength(64)]
        public string Answer81 { get; set; }
        [StringLength(64)]
        public string Answer82 { get; set; }
        [StringLength(64)]
        public string Answer83 { get; set; }
        [StringLength(64)]
        public string Answer84 { get; set; }
        [StringLength(64)]
        public string Answer85 { get; set; }
        [StringLength(64)]
        public string Answer86 { get; set; }
        [StringLength(64)]
        public string Answer87 { get; set; }
        [StringLength(64)]
        public string Answer88 { get; set; }
        [StringLength(64)]
        public string Answer89 { get; set; }
        [StringLength(64)]
        public string Answer90 { get; set; }
        [StringLength(64)]
        public string Answer91 { get; set; }
        [StringLength(64)]
        public string Answer92 { get; set; }
        [StringLength(64)]
        public string Answer93 { get; set; }
        [StringLength(64)]
        public string Answer94 { get; set; }

        [StringLength(64)]
        public string Answer95 { get; set; }

        [StringLength(64)]
        public string Answer96 { get; set; }

        [StringLength(64)]
        public string Answer97 { get; set; }

        public DateTime? FeedbackDate { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(16)]
        public string IPPBOfficerMobile { get; set; }

        [StringLength(12)]
        public string TrainingType { get; set; }
    }

    public class TrackerReportBatchWiseView
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int32 Batch { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TrackerNumber { get; set; }

        public Int32 Phase { get; set; }

        [StringLength(64)]
        public string Answer1 { get; set; }
        [StringLength(64)]
        public string Answer2 { get; set; }
        [StringLength(64)]
        public string Answer3 { get; set; }
        [StringLength(64)]
        public string Answer4 { get; set; }
        [StringLength(64)]
        public string Answer5 { get; set; }
        [StringLength(64)]
        public string Answer6 { get; set; }
        [StringLength(64)]
        public string Answer7 { get; set; }
        [StringLength(64)]
        public string Answer8 { get; set; }
        [StringLength(64)]
        public string Answer9 { get; set; }
        [StringLength(64)]
        public string Answer10 { get; set; }
        [StringLength(64)]
        public string Answer11 { get; set; }
        [StringLength(64)]
        public string Answer12 { get; set; }
        [StringLength(64)]
        public string Answer13 { get; set; }
        [StringLength(64)]
        public string Answer14 { get; set; }
        [StringLength(64)]
        public string Answer15 { get; set; }
        [StringLength(64)]
        public string Answer16 { get; set; }
        [StringLength(64)]
        public string Answer17 { get; set; }
        [StringLength(64)]
        public string Answer18 { get; set; }
        [StringLength(64)]
        public string Answer19 { get; set; }
        [StringLength(64)]
        public string Answer20 { get; set; }
        [StringLength(64)]
        public string Answer21 { get; set; }
        [StringLength(64)]
        public string Answer22 { get; set; }
        [StringLength(64)]
        public string Answer23 { get; set; }
        [StringLength(64)]
        public string Answer24 { get; set; }
        [StringLength(64)]
        public string Answer25 { get; set; }
        [StringLength(64)]
        public string Answer26 { get; set; }
        [StringLength(64)]
        public string Answer27 { get; set; }
        [StringLength(64)]
        public string Answer28 { get; set; }
        [StringLength(64)]
        public string Answer29 { get; set; }
        [StringLength(64)]
        public string Answer30 { get; set; }
        [StringLength(64)]
        public string Answer31 { get; set; }
        [StringLength(64)]
        public string Answer32 { get; set; }
        [StringLength(64)]
        public string Answer33 { get; set; }
        [StringLength(64)]
        public string Answer34 { get; set; }
        [StringLength(64)]
        public string Answer35 { get; set; }
        [StringLength(64)]
        public string Answer36 { get; set; }
        [StringLength(64)]
        public string Answer37 { get; set; }
        [StringLength(64)]
        public string Answer38 { get; set; }
        [StringLength(64)]
        public string Answer39 { get; set; }
        [StringLength(64)]
        public string Answer40 { get; set; }
        [StringLength(64)]
        public string Answer41 { get; set; }
        [StringLength(64)]
        public string Answer42 { get; set; }
        [StringLength(64)]
        public string Answer43 { get; set; }
        [StringLength(64)]
        public string Answer44 { get; set; }
        [StringLength(64)]
        public string Answer45 { get; set; }
        [StringLength(64)]
        public string Answer46 { get; set; }
        [StringLength(64)]
        public string Answer47 { get; set; }
        [StringLength(64)]
        public string Answer48 { get; set; }
        [StringLength(64)]
        public string Answer49 { get; set; }
        [StringLength(64)]
        public string Answer50 { get; set; }
        [StringLength(64)]
        public string Answer51 { get; set; }
        [StringLength(64)]
        public string Answer52 { get; set; }
        [StringLength(64)]
        public string Answer54 { get; set; }
        [StringLength(64)]
        public string Answer55 { get; set; }
        [StringLength(64)]
        public string Answer56 { get; set; }
        [StringLength(64)]
        public string Answer57 { get; set; }
        [StringLength(64)]
        public string Answer58 { get; set; }
        [StringLength(64)]
        public string Answer59 { get; set; }
        [StringLength(64)]
        public string Answer60 { get; set; }
        [StringLength(64)]
        public string Answer61 { get; set; }
        [StringLength(64)]
        public string Answer62 { get; set; }
        [StringLength(64)]
        public string Answer63 { get; set; }
        [StringLength(64)]
        public string Answer64 { get; set; }
        [StringLength(64)]
        public string Answer65 { get; set; }
        [StringLength(64)]
        public string Answer66 { get; set; }
        [StringLength(64)]
        public string Answer67 { get; set; }
        [StringLength(64)]
        public string Answer68 { get; set; }
        [StringLength(64)]
        public string Answer69 { get; set; }
        [StringLength(64)]
        public string Answer70 { get; set; }
        [StringLength(64)]
        public string Answer71 { get; set; }
        [StringLength(64)]
        public string Answer72 { get; set; }
        [StringLength(64)]
        public string Answer73 { get; set; }
        [StringLength(64)]
        public string Answer74 { get; set; }
        [StringLength(64)]
        public string Answer75 { get; set; }
        [StringLength(64)]
        public string Answer76 { get; set; }
        [StringLength(64)]
        public string Answer77 { get; set; }
        [StringLength(64)]
        public string Answer78 { get; set; }
        [StringLength(64)]
        public string Answer79 { get; set; }
        [StringLength(64)]
        public string Answer80 { get; set; }
        [StringLength(64)]
        public string Answer81 { get; set; }
        [StringLength(64)]
        public string Answer82 { get; set; }
        [StringLength(64)]
        public string Answer83 { get; set; }
        [StringLength(64)]
        public string Answer84 { get; set; }
        [StringLength(64)]
        public string Answer85 { get; set; }
        [StringLength(64)]
        public string Answer86 { get; set; }
        [StringLength(64)]
        public string Answer87 { get; set; }
        [StringLength(64)]
        public string Answer88 { get; set; }
        [StringLength(64)]
        public string Answer89 { get; set; }
        [StringLength(64)]
        public string Answer90 { get; set; }
        [StringLength(64)]
        public string Answer91 { get; set; }
        [StringLength(64)]
        public string Answer92 { get; set; }
        [StringLength(64)]
        public string Answer93 { get; set; }
        [StringLength(64)]
        public string Answer94 { get; set; }

        public DateTime? FeedbackDate { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(16)]
        public string IPPBOfficerMobile { get; set; }

        [StringLength(12)]
        public string TrainingType { get; set; }
        public string Name { get; set; }

    }

    public class TrackerReportTotalBatchWiseView
    {
        public int Answer4Total { get; set; }
        public int Answer5Total { get; set; }
        public int Answer6Total { get; set; }
        public int Answer7Total { get; set; }
        public int Answer46Total { get; set; }
        public int Answer47Total { get; set; }
        public int Answer50Total { get; set; }
        public int Answer51Total { get; set; }
        public int Answer68Total { get; set; }
        public int Answer54Total { get; set; }
        public int Answer55Total { get; set; }
        public int Answer56Total { get; set; }
        public int Answer57Total { get; set; }
        public int Answer58Total { get; set; }
        public int Answer59Total { get; set; }
        public int Answer70Total { get; set; }
        public int Answer71Total { get; set; }
        public int Answer72Total { get; set; }
        public int Answer73Total { get; set; }
        public int Answer74Total { get; set; }
        public int Answer75Total { get; set; }
        public int Answer82Total { get; set; }
        public int Answer49Total { get; set; }
        public int Answer61Total { get; set; }
        public int Answer77Total { get; set; }
        public int Answer40Total { get; set; }
        public string Circle { get; set; }

    }

    public class GetMTTrainingOrderCount
    {
        public int TotalNominated { get; set; }
        public int TotalTraineeAttended { get; set; }
        public int CertifiedTrainee { get; set; }
        public int YetToCertified { get; set; }
    }

    public class EndUserAttendedCount
    {
        public int TotalTraineeAttended { get; set; }
        public int TotalPATraineeAttended { get; set; }
        public int TotalGDSTraineeAttended { get; set; }
        public string Circle { get; set; }
    }

    public class EndUserNominatedCount
    {
        public int TotalTraineeNominated { get; set; }
        public int TotalPATraineeNominated { get; set; }
        public int TotalGDSTraineeNominated { get; set; }
        public string Circle { get; set; }
    }


    public class EndUserCertifiedCount
    {
        public int TotalTraineeCertified { get; set; }
        public int TotalPACertified { get; set; }
        public int TotalGDSCertified { get; set; }
        public string Circle { get; set; }
    }

    public class GetMTTrainingOrderAttendedExport
    {
        public string Name { get; set; }
        public string TraineeReckonnId { get; set; }
        public string PhoneNumber { get; set; }
        public string Branch { get; set; }
        public string Circle { get; set; }
        public string SIUSERID { get; set; }
        public string Designation { get; set; }
        public string Gender { get; set; }
        public string TraineeAddedOn { get; set; }
        public string BatchId { get; set; }
        public string BatchName { get; set; }
        public string TrainingId { get; set; }
        public string TrainerName { get; set; }
        public string TrainerReckonnId { get; set; }
        public string TrainerPhone { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class EndUserAttendedCountTrend
    {
        public int TotalTraineeAttended { get; set; }
        public int TotalPATraineeAttended { get; set; }
        public int TotalGDSTraineeAttended { get; set; }
        public Int32 UpdatedON { get; set; }
    }

    public class EndUserNominatedCountTrend
    {
        public int TotalTraineeNominated { get; set; }
        public int TotalPATraineeNominated { get; set; }
        public int TotalGDSTraineeNominated { get; set; }

        public Int32 UpdatedON { get; set; }
    }
    public class EndUserTrendGraphDetail
    {
        //for nominated trend graph
        public Int64 TotalNominatedTrand { get; set; }
        public Int64 TotalPATraineeNominatedTrend { get; set; }
        public Int64 TotalGDSTraineeNominatedTrend { get; set; }

        //for attended trend graph
        public Int64 TotalTraineeAttendedTrend { get; set; }
        public Int64 TotalPATraineeAttendedTrend { get; set; }
        public Int64 TotalGDSTraineeAttendedTrend { get; set; }



        //for certified trend graph
        public Int64 TotalTraineeCertifiedTrend { get; set; }
        public Int64 TotalPACertifiedTrend { get; set; }
        public Int64 TotalGDSCertifiedTrend { get; set; }

        public Int64 UpdatedON { get; set; }
    }

    public class EndUserCertifiedCountTrend
    {
        public int TotalTraineeCertified { get; set; }
        public int TotalPACertified { get; set; }
        public int TotalGDSCertified { get; set; }
        public Int32 UpdatedON { get; set; }
    }

    public class MultiTrendTotalCounts
    {
        public int TotalTraineeCertified { get; set; }
        public int TotalTraineeNominated { get; set; }
        public int TotalTraineeAttended { get; set; }
        public string Circle { get; set; }
    }

    public class PAMultiTrendTotalCounts
    {
        public int PATraineeCertified { get; set; }
        public int PATraineeNominated { get; set; }
        public int PATraineeAttended { get; set; }
        public string Circle { get; set; }
    }


    public class GDSMultiTrendTotalCounts
    {
        public int GDSTraineeCertified { get; set; }
        public int GDSTraineeNominated { get; set; }
        public int GDSTraineeAttended { get; set; }
        public string Circle { get; set; }
    }


    public class UserIdView
    {
        public string Id { get; set; }
    }
    public class EndUserCounts
    {
        public int TotalTraineeAttended { get; set; }
        public int TotalPATraineeAttended { get; set; }
        public int TotalGDSTraineeAttended { get; set; }
        public int TotalTraineeNominated { get; set; }
        public int TotalPATraineeNominated { get; set; }
        public int TotalGDSTraineeNominated { get; set; }
        public int TotalTraineeCertified { get; set; }
        public int TotalPACertified { get; set; }
        public int TotalGDSCertified { get; set; }
        public string Circle { get; set; }
        public int CircleId { get; set; }
    }

}