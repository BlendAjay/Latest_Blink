using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AJSolutions.Models
{

    public class DepartmentMaster
    {
        [Key]
        [StringLength(3)]
        public string DepartmentId { get; set; }

        [StringLength(64)]
        public string Department { get; set; }

        [StringLength(128)]
        public string RoleId { get; set; }

        public Boolean IsVisible { get; set; }

    }

    public class ModuleMaster
    {
        [Key]
        [StringLength(3)]
        public string ModuleId { get; set; }

        [StringLength(128)]
        public string Module { get; set; }

    }

    public class ProgrammeMaster
    {
        [Key]
        [StringLength(16)]
        public string ProgrammeId { get; set; }

        [StringLength(64)]
        public string ProgrammeName { get; set; }

        [StringLength(3)]
        public string ModuleId { get; set; }

        [ForeignKey("ModuleId")]
        public virtual ModuleMaster ModuleMaster { get; set; }
    }

    public class ModuleRolesMapping
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(3)]
        public string ModuleId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(128)]
        public string RoleId { get; set; }

        [ForeignKey("ModuleId")]
        public virtual ModuleMaster ModuleMaster { get; set; }

    }

    public class UserModuleAccess
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string ModuleId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CommencementDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? RetiredDate { get; set; }

        [ForeignKey("ModuleId")]
        public virtual ModuleMaster ModuleMaster { get; set; }

    }

    public class UserHistory
    {
        [Key]
        public string UserId { get; set; }

        public DateTime RegisteredOn { get; set; }

        public DateTime? LastLogin { get; set; }

        public string TimeZone { get; set; }
    }



    public class EducationLevelMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short EducationLevelId { get; set; }

        [StringLength(32)]
        public string EducationLevelName { get; set; }

    }

    public class IdentificationTypeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short IdentificationTypeId { get; set; }

        [StringLength(64)]
        public string IdentificationTypeName { get; set; }

    }

    public class InstallmentMaster
    {
        [Key]
        public Int16 InstallmentId { get; set; }

        [StringLength(128)]
        public string Installment { get; set; }
    }

    public class PaymentModeMaster
    {
        [Key]
        public Int16 PaymentModeId { get; set; }

        [StringLength(128)]
        public string PaymentMode { get; set; }
    }

    public partial class CountryMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CountryId { get; set; }

        [StringLength(6)]
        public string ShortName { get; set; }

        [StringLength(40)]
        public string Country { get; set; }

        public string RegionCode { get; set; }

    }

    public partial class StatesMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StateId { get; set; }

        [StringLength(32)]
        public string State { get; set; }

        public int CountryId { get; set; }

        public virtual CountryMaster CountryMaster { get; set; }

    }

    public partial class CityMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CityId { get; set; }

        [StringLength(64)]
        public string City { get; set; }

        public int StateId { get; set; }

        public virtual StatesMaster StatesMaster { get; set; }
    }



    public partial class Nationalities
    {
        [Key]
        [StringLength(48)]
        public string Nationality { get; set; }
    }

    public partial class AllCurrency
    {
        [Key]
        [StringLength(16)]
        public string Currency { get; set; }

    }

    public class JOCommentsForum
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 JOCommentId { get; set; }

        [StringLength(16)]
        public string JobOrderNumber { get; set; }

        [StringLength(512)]
        public string JOComment { get; set; }

        public DateTime CommentedOn { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [ForeignKey("JobOrderNumber")]
        public virtual JobOrder JobOrder { get; set; }
    }

    public class JOReplyForum
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 JOReplyId { get; set; }

        public Int64 JOCommentId { get; set; }

        [StringLength(512)]
        public string JOReply { get; set; }

        public DateTime RepliedOn { get; set; }

        public string UserId { get; set; }

        [ForeignKey("JOCommentId")]
        public virtual JOCommentsForum JOCommentsForum { get; set; }

    }

    public class TaskCommentsForum
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TaskCommentId { get; set; }

        [StringLength(32)]
        public string TaskId { get; set; }

        [StringLength(512)]
        public string TaskComment { get; set; }

        public DateTime CommentedOn { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [ForeignKey("TaskId")]
        public virtual TaskMaster TaskMaster { get; set; }
    }

    public class TaskReplyForum
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TaskReplyId { get; set; }

        public Int64 TaskCommentId { get; set; }

        [StringLength(512)]
        public string TaskReply { get; set; }

        public DateTime RepliedOn { get; set; }

        public string UserId { get; set; }

        [ForeignKey("TaskCommentId")]
        public virtual TaskCommentsForum TaskCommentsForum { get; set; }

    }

    public class JOCommentsForumView
    {
        public Int64 JOCommentId { get; set; }

        [StringLength(16)]
        public string JobOrderNumber { get; set; }

        [StringLength(512)]
        public string JOComment { get; set; }

        public DateTime CommentedOn { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public string UserName { get; set; }
    }

    public class JOReplyForumView
    {
        public Int64 JOReplyId { get; set; }

        public Int64 JOCommentId { get; set; }

        [StringLength(512)]
        public string JOReply { get; set; }

        public DateTime RepliedOn { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public string UserName { get; set; }

    }

    public class JOCommentsCount
    {
        public Int32 TotalJOComments { get; set; }
    }

    public class TaskCommentsForumView
    {
        public Int64 TaskCommentId { get; set; }

        [StringLength(32)]
        public string TaskId { get; set; }

        [StringLength(512)]
        public string TaskComment { get; set; }

        public DateTime CommentedOn { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public string UserName { get; set; }
    }

    public class TaskReplyForumView
    {
        public Int64 TaskReplyId { get; set; }

        public Int64 TaskCommentId { get; set; }

        [StringLength(512)]
        public string TaskReply { get; set; }

        public DateTime RepliedOn { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }
    }

    //By: Ajay Kumar Choudhary
    //On: 01-08-2017
    //For Plan & Pricing Features

    public partial class Plan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlanId { get; set; }

        [StringLength(64)]
        public string PlanName { get; set; }

        [StringLength(128)]
        public string ShortDescription { get; set; }

        public short PlanSequence { get; set; }

        public DateTime CommenceMentDate { get; set; }

        public DateTime? RetiredDate { get; set; }

        public double INRAmount { get; set; }

        public double INRDiscount { get; set; }

        public double OtherAmount { get; set; }

        public double OtherDiscount { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

    }

    public class PaymentTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TransactionId { get; set; }

        [StringLength(12)]
        public string UserPlanId { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(16)]
        public string ReferenceId { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        public double Amount { get; set; }

        [StringLength(512)]
        public string Comments { get; set; }

        [StringLength(128)]
        public string PayeeName { get; set; }

        [StringLength(128)]
        public string PayeeEmail { get; set; }

        [StringLength(16)]
        public string PayeePhoneNumber { get; set; }

        public DateTime PaymentDate { get; set; }

        [StringLength(16)]
        public string Status { get; set; }

        [StringLength(64)]
        public string TransactionReferenceNumber { get; set; }

        [StringLength(16)]
        public string BankCode { get; set; }

        [StringLength(2048)]
        public string PGComment { get; set; }

        [StringLength(50)]
        public string ClientTxnRefNo { get; set; }

        [StringLength(24)]
        public string TSPLTxnId { get; set; }

        [StringLength(24)]
        public string txn_status { get; set; }

        [StringLength(24)]
        public string txn_msg { get; set; }
    }

    public class UserPlan
    {
        [Key]
        [StringLength(12)]
        public string UserPlanId { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        public int PlanId { get; set; }

        public DateTime PlanStartDate { get; set; }

        public DateTime PlanEndDate { get; set; }

        public DateTime NextDueDate { get; set; }

        [StringLength(128)]
        public string Comments { get; set; }

        [StringLength(16)]
        public string PlanStatus { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [ForeignKey("PlanId")]
        public virtual Plan Plan { get; set; }

    }

    public class Features
    {
        [Key]
        public Int64 FeatureId { get; set; }

        [StringLength(128)]
        public string Feature { get; set; }

        public Int16 FeatureSequence { get; set; }

    }

    public class PlanFeatures
    {
        [Key]
        [Column(Order = 0)]
        public int PlanId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int64 FeatureId { get; set; }

        [ForeignKey("PlanId")]
        public virtual Plan Plan { get; set; }

        [ForeignKey("FeatureId")]
        public virtual Features Features { get; set; }
    }

    public class PlanFeaturesView
    {
        [Key]
        [Column(Order = 0)]
        public int PlanId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int64 FeatureId { get; set; }

        public string Feature { get; set; }

        public string PlanName { get; set; }
    }

    public partial class PlanAddOns
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AddOnId { get; set; }

        [StringLength(128)]
        public string AddOnName { get; set; }

        public double INRAmount { get; set; }

        public double INRDiscount { get; set; }

        public double OtherAmount { get; set; }

        public double OtherDiscount { get; set; }
    }

    public class UserPlanAddOns
    {
        [Key]
        [StringLength(12)]
        public string UserAdonId { get; set; }

        [StringLength(12)]
        public string UserPlanId { get; set; }

        public int AddOnId { get; set; }

        public DateTime AddonStartDate { get; set; }

        [StringLength(128)]
        public string Comments { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [ForeignKey("AddOnId")]
        public virtual PlanAddOns PlanAddOns { get; set; }

    }

    public class UserPlanDetailView
    {
        [Key]
        [StringLength(12)]
        public string UserPlanId { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        public int PlanId { get; set; }

        public DateTime PlanStartDate { get; set; }

        public DateTime PlanEndDate { get; set; }

        public DateTime NextDueDate { get; set; }

        [StringLength(128)]
        public string Comments { get; set; }

        [StringLength(16)]
        public string PlanStatus { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        public string PlanName { get; set; }

        public string UserAdonId { get; set; }

        public int AddOnId { get; set; }

        public string AddOnName { get; set; }

        public string Status { get; set; }

        public string ReferenceId { get; set; }

        public string TransactionReferenceNumber { get; set; }

        public string TSPLTxnId { get; set; }

    }

    //Model For LeaveSchemeMaster
    //created by : Vikas Pandey
    //17/11/2017
    public class LeaveSchemeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int16 SchemeId { get; set; }

        [StringLength(12)]
        public string SchemeName { get; set; }
    }

    //Model For LeaveType
    //Created By : Vikas Pandey
    //17/11/2017
    public class LeaveType
    {
        [Key]
        [StringLength(2)]
        public string LeaveTypeId { get; set; }

        [StringLength(64)]
        public string LeaveTypeName { get; set; }

        [StringLength(2)]
        public string LeaveTypeCategory { get; set; }
    }

    //}
    //public class TaskCommentsCount
    //{
    //    public Int32 TotalTaskComments { get; set; }
    //}

    //created by : vikas pandey
    //24/11/2017
    //for company settings
    public class CompanySetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 SettingId { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        public Int16 WorkingDayPerWeek { get; set; }

        public Int32 ProbationPeriod { get; set; }

        [StringLength(3)]
        public string DefaultCurrency { get; set; }

        [StringLength(7)]
        public string CalendarYear { get; set; }

        public bool PayslipPasswordProtection { get; set; }

        public bool AutoenablePayrollProcess { get; set; }

        public Int16 AutoProcessPayrollDate { get; set; }

        public bool CompanyAttendance { get; set; }

        public bool LeavesCalculationCriteria { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

    }
    public class ProgrammePermissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 ProgrammeAccessId { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(16)]
        public string ProgrammeId { get; set; }

        public bool ReadOnly { get; set; }

        public bool ReadWrite { get; set; }

        [ForeignKey("ProgrammeId")]
        public virtual ProgrammeMaster ProgrammeMaster { get; set; }
    }
}