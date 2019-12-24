using AJSolutions.Areas.LMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AJSolutions.Models
{

    public class UserProfile
    {
        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        [StringLength(2)]
        public string Gender { get; set; }

        [StringLength(24)]
        public string MaritalStatus { get; set; }

        [StringLength(16)]
        [RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        public string AlternateContact { get; set; }

        [StringLength(64)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string AlternateEmail { get; set; }

        [StringLength(32)]
        public string Nationality { get; set; }

        [StringLength(3)]
        public string DepartmentId { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMaster { get; set; }

        [StringLength(16)]
        public string RegistrationId { get; set; }

        [StringLength(128)]
        public string Designation { get; set; }

        [StringLength(128)]
        public string Branch { get; set; }

        [StringLength(128)]
        public string BranchCategory { get; set; }

        [StringLength(128)]
        public string Region { get; set; }

        [StringLength(128)]
        public string BranchCode { get; set; }

        [StringLength(128)]
        public string BranchState { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(16)]
        public string PCode { get; set; }

        [DefaultValue(false)]
        public bool Deactivated { get; set; }

        [StringLength(128)]
        public string Source { get; set; }

        [StringLength(12)]
        public string ReferenceId { get; set; }

        [StringLength(16)]
        public string OtherId { get; set; }

        [StringLength(16)]
        public string TrackerId { get; set; }

        [StringLength(16)]
        public string FacilityId { get; set; }

        [StringLength(16)]
        public string Accesspoint { get; set; }
    }

    public class UserFamilyDetails
    {
        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(64)]
        public string FatherName { get; set; }

        [StringLength(32)]
        public string FatherOccupation { get; set; }

        [StringLength(16)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string FatherContact { get; set; }

        [StringLength(64)]
        public string MotherName { get; set; }

        [StringLength(32)]
        public string MotherOccupation { get; set; }

        [StringLength(16)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string MotherContact { get; set; }

        [StringLength(64)]
        public string SpouseName { get; set; }

        [StringLength(16)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string SpouseContact { get; set; }

        [StringLength(64)]
        public string SpouseOccupation { get; set; }

        [StringLength(8)]
        public string BloodGroup { get; set; }

        [StringLength(16)]
        public string FamilyIncome { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

    }

    public class UserEduactionDetails
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public short EducationLevel { get; set; }

        [StringLength(64)]
        public string Degree { get; set; }

        [StringLength(64)]
        public string Specialization { get; set; }

        [StringLength(64)]
        public string University { get; set; }

        [StringLength(64)]
        public string Institution { get; set; }

        [StringLength(4)]
        public string YearOfPassing { get; set; }

        [StringLength(16)]
        public string Percentage { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }
    }

    public class UserIdentificationDetails
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public short IdType { get; set; }

        [StringLength(64)]
        public string IdNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? IssuingDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ValidTill { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        [ForeignKey("IdType")]
        public virtual IdentificationTypeMaster IdentificationTypeMaster { get; set; }
    }

    public class UserVehicleDetails
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(16)]
        public string VehicleType { get; set; }

        [StringLength(16)]
        public string VehicleNumber { get; set; }

        [StringLength(32)]
        public string VehicleOwner { get; set; }

        [StringLength(16)]
        public string DrivingLicence { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }
    }

    public class UserSocialDetails
    {
        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(64)]
        [RegularExpression(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", ErrorMessage = "Invalid LinkedIn link")]
        public string LinkedIn { get; set; }

        [RegularExpression(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", ErrorMessage = "Invalid Facebook link")]
        [StringLength(64)]
        public string Facebook { get; set; }

        [StringLength(64)]
        public string Skypeid { get; set; }

        [RegularExpression(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", ErrorMessage = "Invalid Google Plus link")]
        [StringLength(64)]
        public string GooglePlus { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }
    }

    public class UserExperienceDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 ExperienceId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string ComapanyName { get; set; }

        [StringLength(64)]
        public string WorkLocation { get; set; }

        [StringLength(64)]
        public string LatestDesignation { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime JoiningDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LeavingDate { get; set; }

        [DefaultValue(false)]
        public bool WorkingStatus { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }
    }

    public class UserExperienceView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 ExperienceId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string ComapanyName { get; set; }

        [StringLength(64)]
        public string WorkLocation { get; set; }

        [StringLength(64)]
        public string LatestDesignation { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime JoiningDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LeavingDate { get; set; }

        [DefaultValue(false)]
        public bool WorkingStatus { get; set; }


        [StringLength(128)]
        public string ProfileName { get; set; }

        public Int64 ProfileId { get; set; }


        [ForeignKey("ProfileId")]
        public virtual UserProfileTypeDetails UserProfileTypeDetails { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }
    }

    public class UserSkillDetails
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(64)]
        public string SkillName { get; set; }

        public short YearofExperience { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }
    }

    public class UserAddressDetails
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(2)]
        public string AddressType { get; set; }

        [Required]
        [StringLength(64)]
        public string AddressLine1 { get; set; }

        [StringLength(64)]
        public string AddressLine2 { get; set; }

        public int CityId { get; set; }

        public int StateId { get; set; }

        [StringLength(16)]
        public string PostalCode { get; set; }

        public int CountryId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        [ForeignKey("CityId")]
        public virtual CityMaster CityMaster { get; set; }

        [ForeignKey("StateId")]
        public virtual StatesMaster StateMaster { get; set; }

        [ForeignKey("CountryId")]
        public virtual CountryMaster CountryMaster { get; set; }
    }


    public class UserEduactionView
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public short EducationLevel { get; set; }

        [StringLength(64)]
        public string Degree { get; set; }

        [StringLength(64)]
        public string Specialization { get; set; }

        [StringLength(64)]
        public string University { get; set; }

        [StringLength(64)]
        public string Institution { get; set; }

        [StringLength(4)]
        public string YearOfPassing { get; set; }

        [StringLength(16)]
        public string Percentage { get; set; }

        [StringLength(32)]
        public string EducationLevelName { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }
    }

    public class UserIdentificationDetailsView
    {

        [StringLength(128)]
        public string UserId { get; set; }

        public short IdType { get; set; }

        [StringLength(64)]
        public string IdNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? IssuingDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ValidTill { get; set; }

        [StringLength(64)]
        public string IdentificationTypeName { get; set; }

    }

    public class CourseView
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(16)]
        public string CourseCode { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CourseStartdate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CourseEndDate { get; set; }

        public double TotalFee { get; set; }

        public double Discount { get; set; }

        public double RemainingFee { get; set; }

        [StringLength(32)]
        public string CourseName { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        [ForeignKey("CourseCode")]
        public virtual CourseMaster Course { get; set; }
    }

    public partial class UserAddressViewModel
    {
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(2)]
        public string AddressType { get; set; }

        [Required]
        [StringLength(64)]
        public string AddressLine1 { get; set; }

        [StringLength(64)]
        public string AddressLine2 { get; set; }

        public int CityId { get; set; }

        public int StateId { get; set; }

        [StringLength(16)]
        public string PostalCode { get; set; }

        public int CountryId { get; set; }


        [StringLength(64)]
        public string Country { get; set; }

        [StringLength(64)]
        public string State { get; set; }

        [StringLength(64)]
        public string City { get; set; }

    }

    public class UserTopicStatus
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int64 CourseTopicId { get; set; }

        public Int16 Status { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        [ForeignKey("CourseTopicId")]
        public virtual COURSETOPICS COURSETOPICS { get; set; }
    }

    public class UserLectureStatus
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int64 TopicLectureId { get; set; }

        public Int16 Status { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }
    }

    public class UserCourseSubscription
    {
        [Key]
        [StringLength(16)]
        public string SubscriptionId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        public DateTime CourseStartDate { get; set; }

        public DateTime CourseEndDate { get; set; }

        [ForeignKey("CourseCode")]
        public virtual CourseMaster CourseMaster { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }
    }

    public class UserNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 NotificationId { get; set; }

        [StringLength(128)]
        public string AssingedTo { get; set; }

        [StringLength(128)]
        public string AssingedBy { get; set; }

        [StringLength(256)]
        public string NotificationBody { get; set; }

        [StringLength(32)]
        public string NotificationFor { get; set; }

        [StringLength(16)]
        public string ReferenceId { get; set; }

        [DefaultValue(false)]
        public bool Status { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ViewTime { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime NotificationDate { get; set; }
    }

    public class UserNotificationView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 NotificationId { get; set; }

        [StringLength(128)]
        public string AssingedTo { get; set; }

        [StringLength(128)]
        public string AssingedBy { get; set; }

        [StringLength(256)]
        public string NotificationBody { get; set; }

        [StringLength(32)]
        public string NotificationFor { get; set; }

        [StringLength(16)]
        public string ReferenceId { get; set; }

        [DefaultValue(false)]
        public bool Status { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ViewTime { get; set; }
    }

    public class GetCountNotification
    {
        public int TOTALNOTIFICATION { get; set; }
        public int TOTALCOURSE { get; set; }
    }

    public class CandidateLeads
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 LeadId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [StringLength(16)]
        [RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        public string PhoneNumber { get; set; }

        [StringLength(64)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string EmailId { get; set; }

        [StringLength(128)]
        public string FatherName { get; set; }

        [StringLength(128)]
        public string FatherOccupation { get; set; }

        [StringLength(128)]
        public string MotherName { get; set; }

        [StringLength(128)]
        public string MotherOccupation { get; set; }

        [StringLength(32)]
        public string IdName { get; set; }

        [StringLength(64)]
        public string IdNumber { get; set; }

        [StringLength(2)]
        public string Gender { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        [StringLength(3)]
        public string Category { get; set; }

        [StringLength(32)]
        public string Religion { get; set; }

        [DefaultValue(false)]
        public bool DifferentlyAbled { get; set; }

        [StringLength(24)]
        public string MaritalStatus { get; set; }

        [StringLength(32)]
        public string MediumOfEducation { get; set; }

        public bool Relocate { get; set; }

        public Int16 RelocateId { get; set; }

        [StringLength(3)]
        public string BelowPoverty { get; set; }

        [StringLength(16)]
        public string FamilyIncome { get; set; }

        [StringLength(16)]
        public string Qualification { get; set; }

        public Int16 Status { get; set; }

        [StringLength(512)]
        public string Comments { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        public DateTime SubmittedOn { get; set; }

        [StringLength(128)]
        public string SubmittedBy { get; set; }

        [StringLength(128)]
        public string ReferenceId { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string Address { get; set; }

    }

    public class CandidateLeadsListView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 LeadId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [StringLength(16)]
        [RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        public string PhoneNumber { get; set; }

        [StringLength(64)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string EmailId { get; set; }

        [StringLength(128)]
        public string FatherName { get; set; }

        [StringLength(128)]
        public string FatherOccupation { get; set; }

        [StringLength(128)]
        public string MotherName { get; set; }

        [StringLength(128)]
        public string MotherOccupation { get; set; }

        [StringLength(32)]
        public string IdName { get; set; }

        [StringLength(64)]
        public string IdNumber { get; set; }

        [StringLength(2)]
        public string Gender { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        [StringLength(3)]
        public string Category { get; set; }

        [StringLength(32)]
        public string Religion { get; set; }

        [DefaultValue(false)]
        public bool DifferentlyAbled { get; set; }

        [StringLength(24)]
        public string MaritalStatus { get; set; }

        [StringLength(32)]
        public string MediumOfEducation { get; set; }

        public bool Relocate { get; set; }

        public Int16 RelocateId { get; set; }

        [StringLength(3)]
        public string BelowPoverty { get; set; }

        [StringLength(16)]
        public string FamilyIncome { get; set; }

        [StringLength(16)]
        public string Qualification { get; set; }

        public Int16 Status { get; set; }

        [StringLength(512)]
        public string Comments { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        public DateTime SubmittedOn { get; set; }

        [StringLength(128)]
        public string SubmittedBy { get; set; }

        [StringLength(128)]
        public string ReferenceId { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string Address { get; set; }
        public string SubmittedByName { get; set; }
        public string UserName { get; set; }

    }

    public class CandidateLeadsView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 LeadId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [StringLength(16)]
        [RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        public string PhoneNumber { get; set; }

        [StringLength(64)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string EmailId { get; set; }

        [StringLength(128)]
        public string FatherName { get; set; }

        [StringLength(128)]
        public string FatherOccupation { get; set; }

        [StringLength(128)]
        public string MotherName { get; set; }

        [StringLength(128)]
        public string MotherOccupation { get; set; }

        [StringLength(32)]
        public string IdName { get; set; }

        [StringLength(64)]
        public string IdNumber { get; set; }

        [StringLength(2)]
        public string Gender { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        [StringLength(3)]
        public string Category { get; set; }

        [StringLength(32)]
        public string Religion { get; set; }

        [DefaultValue(false)]
        public bool DifferentlyAbled { get; set; }

        [StringLength(24)]
        public string MaritalStatus { get; set; }

        [StringLength(32)]
        public string MediumOfEducation { get; set; }

        public bool Relocate { get; set; }

        public Int16 RelocateId { get; set; }

        [StringLength(3)]
        public string BelowPoverty { get; set; }

        [StringLength(16)]
        public string FamilyIncome { get; set; }

        [StringLength(16)]
        public string Qualification { get; set; }

        public Int16 Status { get; set; }

        [StringLength(512)]
        public string Comments { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        public DateTime SubmittedOn { get; set; }

        [StringLength(128)]
        public string SubmittedBy { get; set; }

        [StringLength(128)]
        public string ReferenceId { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string Address { get; set; }

        public string SubmittedByName { get; set; }
    }

    ///Summary
    ///Created By: Ajay Kumar Choudhary
    ///Created on: 03-06-2017
    ///For: Languages   
    public class Languages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 UserLanguageId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public Int32 LanguageId { get; set; }

        [DefaultValue(false)]
        public bool Read { get; set; }

        [DefaultValue(false)]
        public bool Write { get; set; }

        [DefaultValue(false)]
        public bool Speak { get; set; }

        [ForeignKey("LanguageId")]
        public virtual LanguageMaster LanguageMaster { get; set; }
    }

    public class LanguageMaster
    {
        [Key]
        public Int32 LanguageId { get; set; }

        [StringLength(128)]
        public string Language { get; set; }
    }

    public class LanguagesView
    {
        public Int64 UserLanguageId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public string Language { get; set; }

        public Int32 LanguageId { get; set; }

        [DefaultValue(false)]
        public bool Read { get; set; }

        [DefaultValue(false)]
        public bool ReadLanguage { get; set; }

        [DefaultValue(false)]
        public bool Write { get; set; }

        [DefaultValue(false)]
        public bool Speak { get; set; }

        [ForeignKey("LanguageId")]
        public virtual LanguageMaster LanguageMaster { get; set; }
    }

    //Ajay Kumar Choudhary
    //For Candidate Installment Details
    public class InstallmentDetails
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int64 BatchId { get; set; }

        public Int16 InstallmentId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        public double TotalFeeAmount { get; set; }

        [DefaultValue(false)]
        public bool Accommodation { get; set; }

        [DefaultValue(false)]
        public bool Transport { get; set; }

        [DefaultValue(false)]
        public bool Others { get; set; }

        [DefaultValue(false)]
        public bool InstallmentInterest { get; set; }

        [DefaultValue(false)]
        public bool Discount { get; set; }

        [ForeignKey("CourseCode")]
        public virtual CourseMaster CourseMaster { get; set; }

        [ForeignKey("BatchId")]
        public virtual CourseBatch CourseBatch { get; set; }

        [ForeignKey("InstallmentId")]
        public virtual InstallmentMaster InstallmentMaster { get; set; }

    }

    public class InstructorLeadProfile
    {
        [Key]
        [StringLength(128)]
        public string InstructorId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(2)]
        public string Gender { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        [DefaultValue(91)]
        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        [StringLength(32)]
        public string Zone { get; set; }

        [StringLength(256)]
        public string Qualification { get; set; }

        public short Experience { get; set; }

        [StringLength(256)]
        public string DomainExpertize { get; set; }

        [StringLength(256)]
        public string Organization { get; set; }

        [StringLength(256)]
        public string LanguageKnown { get; set; }

        [StringLength(256)]
        public string Specialization { get; set; }

        [StringLength(256)]
        public string NibfProject { get; set; }

        public bool ReadyToReallocate { get; set; }

        [StringLength(256)]
        public string TrainingLocation { get; set; }

        [StringLength(256)]
        public string Remarks { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        public bool Empanelled { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }


    }

    public class InstructorLeadProfileView
    {
        [Key]
        [StringLength(128)]
        public string InstructorId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        public string IName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        [Display(Name = "Mobile Number")]
        public string PhoneNumber { get; set; }

        [StringLength(2)]
        public string Gender { get; set; }

        public DateTime? DOB { get; set; }

        [DefaultValue(91)]
        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        [StringLength(32)]
        public string Zone { get; set; }

        [StringLength(256)]
        public string Qualification { get; set; }

        public short Experience { get; set; }

        [StringLength(256)]
        public string DomainExpertize { get; set; }

        [StringLength(256)]
        public string Organization { get; set; }

        [StringLength(256)]
        public string LanguageKnown { get; set; }

        [StringLength(256)]
        public string Specialization { get; set; }

        [StringLength(256)]
        public string NibfProject { get; set; }

        public bool ReadyToReallocate { get; set; }

        [StringLength(256)]
        public string TrainingLocation { get; set; }

        [StringLength(256)]
        public string Remarks { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        public bool Empanelled { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public Int64 FileId { get; set; }


    }


    public class InstructorAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(128)]
        public string InstructorId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [ForeignKey("InstructorId")]
        public virtual InstructorLeadProfile InstructorLeadProfile { get; set; }
    }

    public class HelpLineLayers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 LayerId { get; set; }

        [StringLength(128)]
        public string LayerName { get; set; }

        public Int64 LayerParentId { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }
    }

    public class HelpLineLayerDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 LayerDetailsId { get; set; }

        public Int64 LayerId { get; set; }

        [StringLength(128)]
        public string LayerName { get; set; }

        public string LayerText { get; set; }

        public Int64 LayerDetailsParentId { get; set; }

        [DefaultValue(0)]
        public bool AutoReply { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [ForeignKey("LayerId")]
        public virtual HelpLineLayers HelpLineLayers { get; set; }
    }

    public class HelpLineTracker
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TrackerId { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string UserName { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [StringLength(16)]
        [RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        public string PhoneNumber { get; set; }

        [StringLength(64)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string EmailId { get; set; }

        public Int64 Category { get; set; }

        public Int64 SubCategory { get; set; }

        public Int64 Query { get; set; }

        public Int64 Resolution { get; set; }

        public string DynamicQuery { get; set; }

        public string OtherQueryResolution { get; set; }

        public bool Replied { get; set; }

        public string ReplyBy { get; set; }

        public DateTime QueriedOn { get; set; }

        public DateTime RepliedOn { get; set; }

    }

    public class HelpLineTrackerViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TrackerId { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string UserName { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [StringLength(16)]
        [RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        public string PhoneNumber { get; set; }

        [StringLength(64)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string EmailId { get; set; }

        public Int64 Category { get; set; }

        public string CategoryName { get; set; }

        public Int64 SubCategory { get; set; }

        public string SubCategoryName { get; set; }

        public Int64 Query { get; set; }

        public string Question { get; set; }

        public string DynamicQuery { get; set; }

        public Int64 Resolution { get; set; }

        public string Answer { get; set; }

        public string OtherQueryResolution { get; set; }

        public bool Replied { get; set; }

        public string ReplyBy { get; set; }

        public DateTime QueriedOn { get; set; }

        public DateTime RepliedOn { get; set; }

    }

    public class EditHelpLineTrackerViewModel
    {
        public Int64 TrackerId { get; set; }

        public string DynamicQuery { get; set; }

        public string OtherQueryResolution { get; set; }

        public string ReplyBy { get; set; }

        public DateTime RepliedOn { get; set; }
    }

    public class CircleMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 CircleId { get; set; }

        [StringLength(32)]
        public string Circle { get; set; }

        [StringLength(2)]
        public string CircleCode { get; set; }
    }

    public class BranchMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 BranchId { get; set; }

        [StringLength(32)]
        public string Branch { get; set; }

        [StringLength(4)]
        public string BranchCode { get; set; }

        public Int64 CircleId { get; set; }

        [ForeignKey("CircleId")]
        public virtual CircleMaster CircleMaster { get; set; }
    }

    public class OPSData
    {
        [Key]
        public Int64 CertifiedUsersId { get; set; }

        [StringLength(32)]
        public string CircleName { get; set; }

        [StringLength(32)]
        public string IPPBSOLID { get; set; }

        [StringLength(128)]
        public string IPPBSolName { get; set; }

        [StringLength(16)]
        public string FacilityID { get; set; }

        [StringLength(2)]
        public string FacilityType { get; set; }

        [StringLength(64)]
        public string EmpID { get; set; }

        [StringLength(128)]
        public string FullName { get; set; }

        [StringLength(16)]
        public string MobileNumber { get; set; }

        [StringLength(64)]
        public string Designation { get; set; }

        [StringLength(128)]
        public string EmailID { get; set; }

        [StringLength(64)]
        public string EmployeeDesignation { get; set; }

        [StringLength(64)]
        public string ReportingOfficerName { get; set; }

        [StringLength(64)]
        public string ReportingOfficerEmpID { get; set; }

        [StringLength(64)]
        public string ReportingOfficerDesignation { get; set; }

        [StringLength(64)]
        public string Reporting_ASP_IPO_Name { get; set; }

        [StringLength(64)]
        public string Reporting_ASP_IPO_EmpID { get; set; }

        [StringLength(16)]
        public string DOP_SOL_ID { get; set; }

        public bool IsCertified { get; set; }

        public bool IsAvailableLaunch { get; set; }

        public bool IsMobileDevice { get; set; }

        [StringLength(512)]
        public string Comments { get; set; }
    }

    public class CertifiedData
    {
        [Key]
        public Int64 CertifiedDataId { get; set; }

        [StringLength(16)]
        public string UserId { get; set; }

        [StringLength(16)]
        public string Circle { get; set; }

        [StringLength(128)]
        public string Branch { get; set; }

        [StringLength(16)]
        public string UserName { get; set; }

        [StringLength(16)]
        public string Mobile { get; set; }

        [StringLength(16)]
        public string TrainingType { get; set; }
    }

    public class OPSBranch
    {
        [StringLength(16)]
        public string CircleName { get; set; }

        [StringLength(128)]
        public string IPPBSolName { get; set; }
    }

    public class MDMDetailsView
    {
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        public string UserName { get; set; }

        public string RegistrationId { get; set; }

        public string PhoneNumber { get; set; }

        public string OtherId { get; set; }

    }
}