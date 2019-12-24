using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AJSolutions.Models
{
    public class JobOrder
    {
        [Key]
        [StringLength(16)]
        public string JobOrderNumber { get; set; }

        [StringLength(128)]
        public string ClientId { get; set; }

        [StringLength(512)]
        public string Subject { get; set; }

        public int JobOrderTypeId { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        public float TotalCost { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        [StringLength(512)]
        public string Conditions { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime JOPostedOn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DefaultValue(0)]
        public int Duration { get; set; }

        [StringLength(128)]
        public string SalaryRange { get; set; }

        [StringLength(128)]
        public string ExpRange { get; set; }

        [StringLength(128)]
        public string Industry { get; set; }

        [DefaultValue(false)]
        public bool Feedback { get; set; }

        [StringLength(128)]
        public string FunctionalPosition { get; set; }

        [DefaultValue(false)]
        public bool Attendance { get; set; }

        [DefaultValue(false)]
        public bool Accomodation { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(16)]
        public string JobOrderStatus { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CompletedOn { get; set; }

        [ForeignKey("ClientId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

        [ForeignKey("JobOrderTypeId")]
        public virtual JobOrderTypeMaster JobOrderTypeMaster { get; set; }
    }

    public class JobOrderItems
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(16)]
        public string JobOrderNumber { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int16 ItemId { get; set; }

        public Int64 ItemTypeId { get; set; }

        public string ItemDescription { get; set; }

        public int Unit { get; set; }

        public float UnitPrice { get; set; }

        [DefaultValue(0)]
        public int Duration { get; set; }

        [ForeignKey("JobOrderNumber")]
        public virtual JobOrder JobOrder { get; set; }

        //[ForeignKey("ItemTypeId")]
        //public virtual ItemTypeMasters ItemTypeMaster { get; set; }

    }

    public partial class JobOrderAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(16)]
        public string JobOrderNumber { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [ForeignKey("JobOrderNumber")]
        public virtual JobOrder JobOrder { get; set; }
    }

    public partial class JobOrderFinalAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(16)]
        public string JobOrderNumber { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [ForeignKey("JobOrderNumber")]
        public virtual JobOrder JobOrder { get; set; }
    }


    public class JobOrderItemsView
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(16)]
        public string JobOrderNumber { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int16 ItemId { get; set; }

        public Int64 ItemTypeId { get; set; }

        [StringLength(128)]
        public string ItemTypeName { get; set; }

        [StringLength(128)]
        public string ItemDescription { get; set; }

        public int Unit { get; set; }

        public float UnitPrice { get; set; }

        [DefaultValue(0)]
        public int Duration { get; set; }

    }

    public class ClientViewModel
    {
        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please Enter Correct Email Address")]
        public string Email { get; set; }

        [StringLength(16)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? RegisteredOn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LastLogin { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string AlternateEmail { get; set; }

        [StringLength(16)]
        public string AlternateContact { get; set; }

        [StringLength(64)]
        public string CompanyName { get; set; }

        [StringLength(32)]
        public string CompanyType { get; set; }

        [StringLength(32)]
        public string CompanySize { get; set; }

        [StringLength(64)]

        public string Website { get; set; }

        [StringLength(3)]
        public string DepartmentId { get; set; }

        [StringLength(64)]
        public string Department { get; set; }

        [StringLength(128)]
        public string RegisterBy { get; set; }

        [StringLength(128)]
        public string UserName { get; set; }

        public bool EmailConfirmed { get; set; }

        [StringLength(128)]
        public string ReferenceId { get; set; }

        [DefaultValue(false)]
        public bool Deactivated { get; set; }
    }

    public class AdminViewModel
    {
        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please Enter Correct Email Address")]
        public string Email { get; set; }

        [StringLength(16)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? RegisteredOn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LastLogin { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string AlternateEmail { get; set; }

        [StringLength(16)]
        public string AlternateContact { get; set; }

        [StringLength(64)]
        public string CompanyName { get; set; }

        [StringLength(32)]
        public string CompanyType { get; set; }

        [StringLength(32)]
        public string CompanySize { get; set; }

        [StringLength(64)]

        public string Website { get; set; }

        [StringLength(3)]
        public string DepartmentId { get; set; }

        [StringLength(64)]
        public string Department { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        //[StringLength(64)]
        //public string Designation { get; set; }

        [StringLength(32)]
        public string EmployeeId { get; set; }

        [DefaultValue(false)]
        public bool Emplanelled { get; set; }

        public string NameWithId { get; set; }
    }

    #region "TEAM MEMBER"
    //Model Created by:- Ajay Kumar Choudhary Created On:- 17-5-2017

    public class ClientTeamRoles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int16 EmpRoleId { get; set; }

        [StringLength(32)]
        public string EmpRole { get; set; }

        [DefaultValue(true)]
        public Boolean Visibility { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

    }

    public class ClientTeamRights
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int16 EmpRightsId { get; set; }

        [StringLength(64)]
        public string Rights { get; set; }

        public Int16 GroupType { get; set; }

        [DefaultValue(true)]
        public Boolean Visibility { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

    }

    public class ClientTeamMemberProfile
    {
        [Key]
        [StringLength(128)]
        public string MemberId { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(64)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string EmailId { get; set; }

        [StringLength(16)]
        public string PhoneNumber { get; set; }

        public Int16 EmpRoleId { get; set; }

        [StringLength(128)]
        public string Designation { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

        [ForeignKey("EmpRoleId")]
        public virtual ClientTeamRoles ClientTeamRoles { get; set; }

        public IEnumerable<ClientTeamMemberRights> ClientTeamMemberRights { get; set; }
    }

    public class ClientTeamMemberRights
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int16 EmpRightsId { get; set; }

        [ForeignKey("EmpRightsId")]
        public virtual ClientTeamRights ClientTeamRights { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }
    }

    public class ClientTeamMemberProfileView
    {
        [Key]
        [StringLength(128)]
        public string MemberId { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(64)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string EmailId { get; set; }

        [StringLength(16)]
        public string PhoneNumber { get; set; }

        public Int16 EmpRoleId { get; set; }

        [StringLength(128)]
        public string Designation { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        public string EmpRole { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime? RegisteredOn { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

        [ForeignKey("EmpRoleId")]
        public virtual ClientTeamRoles ClientTeamRoles { get; set; }

        public IEnumerable<ClientTeamMemberRights> ClientTeamRights { get; set; }
    }

    public class ClientTeamMemberRightsView
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int16 EmpRightsId { get; set; }

        public string Rights { get; set; }

        [ForeignKey("EmpRightsId")]
        public virtual ClientTeamRights ClientTeamRights { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }
    }
    #endregion


    #region "Third Party Registration"
    public class PreloreAdminView
    {
        [Required]
        [StringLength(128)]
        public string SubscriberId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(16)]
        public string MobileNumber { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(32)]
        public string Password { get; set; }
    }

    #endregion
    /// <summary>
    /// Preparing for Master Trainer Training Order
    /// </summary>
    public class MTTrainingOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TrainingOrderId { get; set; }

        [StringLength(128)]
        public string TrainingWeek { get; set; }

        [StringLength(128)]
        public string Circle { get; set; }

        [StringLength(10)]
        public string TOFilledByContactNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }
    }

    public class MTTrainingOrderTraineeDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TraineeId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string EmpId { get; set; }

        [StringLength(128)]
        public string IppbBranch { get; set; }

        [StringLength(6)]
        public string Gender { get; set; }

        [StringLength(32)]
        public string Designation { get; set; }

        [StringLength(10)]
        public string MobileNumber { get; set; }

        [StringLength(16)]
        public string TrainingType { get; set; }

        [StringLength(128)]
        public string TrainingLocation { get; set; }

        [StringLength(32)]
        public string TrackerId { get; set; }

        public Int64 TrainingOrderId { get; set; }

        [ForeignKey("TrainingOrderId")]
        public virtual MTTrainingOrder MTTrainingOrder { get; set; }
    }

    public class MTTrainingOrderTraineeDetailsView
    {
        public Int64 TrainingOrderId { get; set; }

        [StringLength(128)]
        public string TrainingWeek { get; set; }

        [StringLength(128)]
        public string Circle { get; set; }

        [StringLength(10)]
        public string TOFilledByContactNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        public Int64 TraineeId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string EmpId { get; set; }

        [StringLength(128)]
        public string IppbBranch { get; set; }

        [StringLength(6)]
        public string Gender { get; set; }

        [StringLength(32)]
        public string Designation { get; set; }

        [StringLength(10)]
        public string MobileNumber { get; set; }

        [StringLength(16)]
        public string TrainingType { get; set; }

        [StringLength(128)]
        public string TrainingLocation { get; set; }

        [StringLength(32)]
        public string TrackerId { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }

        public string UpdatedByName { get; set; }

        public int TraineeCount { get; set; }
    }

    public class EndUserTrainingOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 EUTrainingOrderId { get; set; }

        [StringLength(32)]
        public string Wave { get; set; }

        [StringLength(32)]
        public string Circle { get; set; }

        [StringLength(10)]
        public string DOContact { get; set; }

        [StringLength(3)]
        public string TrainingType { get; set; }

        public int PACount { get; set; }

        public int SUFinacleCount { get; set; }

        [StringLength(128)]
        public string PAOfficeName { get; set; }


        public int PAState { get; set; }


        public int PACity { get; set; }

        [StringLength(128)]
        public string PADivision { get; set; }

        [StringLength(128)]
        public string PAMTName { get; set; }

        [StringLength(128)]
        public string PAMTDesignation { get; set; }

        [StringLength(10)]
        public string PAMTContact { get; set; }

        [StringLength(128)]
        public string PAMTEmail { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PATrainingStartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PATrainingEndDate { get; set; }

        public int GDSCount { get; set; }

        public int PostmenCount { get; set; }

        public int GDSSUCount { get; set; }

        [StringLength(128)]
        public string GDSOfficeName { get; set; }


        public int GDSState { get; set; }


        public int GDSCity { get; set; }

        [StringLength(128)]
        public string GDSDivision { get; set; }

        [DefaultValue(0)]
        public Int64? Region { get; set; }

        [StringLength(128)]
        public string GDSMTName { get; set; }

        [StringLength(128)]
        public string GDSMTDesignation { get; set; }

        [StringLength(10)]
        public string GDSMTContact { get; set; }

        [StringLength(128)]
        public string GDSMTEmail { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime GDSTrainingStartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime GDSTrainingEndDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [StringLength(16)]
        public string Status { get; set; }

        [StringLength(16)]
        public string MTReckonnId { get; set; }

        [StringLength(16)]
        public string BatchName { get; set; }

    }

    public class EnduUserTrainee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 EUTraineeId { get; set; }

        [StringLength(128)]
        public string TrackerId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(10)]
        public string PhoneNumber { get; set; }

        [StringLength(6)]
        public string Gender { get; set; }

        [StringLength(32)]
        public string EmpId { get; set; }

        [StringLength(128)]
        public string Designation { get; set; }

        [StringLength(128)]
        public string Accesspoint { get; set; }

        [StringLength(128)]
        public string FaciltyId { get; set; }

        [StringLength(128)]
        public string Branch { get; set; }

        public Int64 EUTrainingOrderId { get; set; }

        [ForeignKey("EUTrainingOrderId")]
        public virtual EndUserTrainingOrder EndUserTrainingOrder { get; set; }
    }
    public class EndUserTrainingOrderView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 EUTrainingOrderId { get; set; }

        [StringLength(32)]
        public string Wave { get; set; }

        [StringLength(32)]
        public string Circle { get; set; }

        [StringLength(128)]
        public string Branch { get; set; }

        [StringLength(10)]
        public string DOContact { get; set; }

        [StringLength(3)]
        public string TrainingType { get; set; }

        public int PACount { get; set; }

        public int SUFinacleCount { get; set; }

        [StringLength(128)]
        public string PAOfficeName { get; set; }

        public int PAState { get; set; }

        public string PAStateName { get; set; }

        public int PACity { get; set; }

        public string PACityName { get; set; }

        [StringLength(128)]
        public string PADivision { get; set; }

        [StringLength(128)]
        public string PAMTName { get; set; }

        [StringLength(128)]
        public string PAMTDesignation { get; set; }

        [StringLength(10)]
        public string PAMTContact { get; set; }

        [StringLength(128)]
        public string PAMTEmail { get; set; }

        public Int64 Region { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PATrainingStartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PATrainingEndDate { get; set; }

        public int GDSCount { get; set; }

        public int PostmenCount { get; set; }

        public int GDSSUCount { get; set; }

        [StringLength(128)]
        public string GDSOfficeName { get; set; }

        public int GDSState { get; set; }

        public string GDSStateName { get; set; }

        public int GDSCity { get; set; }

        public string GDSCityName { get; set; }

        [StringLength(128)]
        public string GDSDivision { get; set; }

        [StringLength(128)]
        public string GDSMTName { get; set; }

        [StringLength(128)]
        public string GDSMTDesignation { get; set; }

        [StringLength(10)]
        public string GDSMTContact { get; set; }

        [StringLength(128)]
        public string GDSMTEmail { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime GDSTrainingStartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime GDSTrainingEndDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }
        public Int64 EUTraineeId { get; set; }


        public string TrackerId { get; set; }


        public string Name { get; set; }


        public string PhoneNumber { get; set; }


        public string Gender { get; set; }


        public string EmpId { get; set; }


        public string Designation { get; set; }

        public int TotalTrainee { get; set; }

        [StringLength(128)]
        public string Accesspoint { get; set; }

        [StringLength(128)]
        public string FaciltyId { get; set; }

        public string Status { get; set; }

        [StringLength(16)]
        public string MTReckonnId { get; set; }

        [StringLength(16)]
        public string BatchName { get; set; }

        public string RegionName { get; set; }


    }

    public class EndUserTrainingOrderAdminView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 EUTrainingOrderId { get; set; }

        [StringLength(32)]
        public string Wave { get; set; }

        [StringLength(32)]
        public string Circle { get; set; }

        [StringLength(128)]
        public string Branch { get; set; }

        [StringLength(10)]
        public string DOContact { get; set; }

        [StringLength(3)]
        public string TrainingType { get; set; }

        public int PACount { get; set; }

        public int SUFinacleCount { get; set; }

        [StringLength(128)]
        public string PAOfficeName { get; set; }

        public int PAState { get; set; }

        public string PAStateName { get; set; }

        public int PACity { get; set; }

        public string PACityName { get; set; }

        [StringLength(128)]
        public string PADivision { get; set; }

        [StringLength(128)]
        public string PAMTName { get; set; }

        [StringLength(128)]
        public string PAMTDesignation { get; set; }

        [StringLength(10)]
        public string PAMTContact { get; set; }

        [StringLength(128)]
        public string PAMTEmail { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PATrainingStartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PATrainingEndDate { get; set; }

        public int GDSCount { get; set; }

        public int PostmenCount { get; set; }

        public int GDSSUCount { get; set; }

        [StringLength(128)]
        public string GDSOfficeName { get; set; }

        public int GDSState { get; set; }

        public string GDSStateName { get; set; }

        public int GDSCity { get; set; }

        public string GDSCityName { get; set; }

        [StringLength(128)]
        public string GDSDivision { get; set; }

        [StringLength(128)]
        public string GDSMTName { get; set; }

        [StringLength(128)]
        public string GDSMTDesignation { get; set; }

        [StringLength(10)]
        public string GDSMTContact { get; set; }

        [StringLength(128)]
        public string GDSMTEmail { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime GDSTrainingStartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime GDSTrainingEndDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }
        public Int64 EUTraineeId { get; set; }


        public string TrackerId { get; set; }


        public string Name { get; set; }


        public string PhoneNumber { get; set; }


        public string Gender { get; set; }


        public string EmpId { get; set; }


        public string Designation { get; set; }

        public int TotalTraineeNominated { get; set; }
        public int TotalTraineeAssigned { get; set; }

        [StringLength(128)]
        public string Accesspoint { get; set; }

        [StringLength(128)]
        public string FaciltyId { get; set; }

        public string Status { get; set; }

        [StringLength(16)]
        public string MTReckonnId { get; set; }

        [StringLength(16)]
        public string BatchName { get; set; }

    }

    public class RegionMasters
    {
        [Key]
        public Int64 RegionId { get; set; }

        [StringLength(128)]
        public string Region { get; set; }

        public Int64 CircleId { get; set; }

        [ForeignKey("CircleId")]
        public virtual CircleMaster CircleMaster { get; set; }
    }

    public class DivisionMasters
    {
        [Key]
        public Int64 DivisionId { get; set; }

        [StringLength(128)]
        public string Division { get; set; }

        public Int64 RegionId { get; set; }

        [ForeignKey("RegionId")]
        public virtual RegionMasters RegionMasters { get; set; }
    }
}