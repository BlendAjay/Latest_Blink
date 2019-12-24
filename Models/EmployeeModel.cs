using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace AJSolutions.Models
{
    public class EmployeeBasicDetails
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
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string AlternateEmail { get; set; }

        [StringLength(32)]
        public string Nationality { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        public Int64? DesignationId { get; set; }

        [StringLength(128)]
        public string EmployeeId { get; set; }

        [DefaultValue(false)]
        public bool Emplanelled { get; set; }

        [DefaultValue(false)]
        public bool ManagerLevel { get; set; }

        [StringLength(128)]
        public string ReportingAuthority { get; set; }

        [StringLength(3)]
        public string DepartmentId { get; set; }

        [StringLength(64)]
        public string FatherName { get; set; }

        [StringLength(64)]
        public string SpouseName { get; set; }

        [DefaultValue(false)]
        public bool Deactivated { get; set; }

        /// <summary>
        /// Model Changes Preeti
        /// </summary>
        [StringLength(128)]
        public string EmergencyContactName { get; set; }

        [StringLength(16)]
        public string EmergencyContactNumber { get; set; }

        [StringLength(8)]
        public string BloodGroup { get; set; }

        [DefaultValue(false)]
        public bool PhysicallyChallenged { get; set; }

        [StringLength(32)]
        public string Location { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? MarriageDate { get; set; }

        //--------------------//

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMaster { get; set; }

    }


    public class ShiftMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 ShiftId { get; set; }

        [StringLength(128)]
        public string Shift { get; set; }

        public string CorporateId { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? StartTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? EndTime { get; set; }
    }

    public class EmpEducationalDetails
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
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }
    }

    public class EmpExperienceDetails
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


        public Int64 ProfileId { get; set; }

        //Changes by Achal

        [StringLength(128)]
        public string NatureofDuties { get; set; }

        [StringLength(128)]
        public string LeavingReason { get; set; }

        //Changes by Achal

        [ForeignKey("ProfileId")]
        public virtual UserProfileTypeDetails UserProfileTypeDetails { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

    }

    public class EmpSkillDetails
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(64)]
        public string SkillName { get; set; }

        [Key]
        [Column(Order = 2)]
        public Int64 ProfileId { get; set; }

        public short YearofExperience { get; set; }

        [ForeignKey("ProfileId")]
        public virtual UserProfileTypeDetails UserProfileTypeDetails { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }


    }

    public class EmpIdentificationDetails
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

        [ForeignKey("IdType")]
        public virtual IdentificationTypeMaster IdentificationTypeMaster { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

    }

    public class EmpIdentificationDetailsView
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

    public class EmpSocialDetails
    {
        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        [RegularExpression(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", ErrorMessage = "Invalid LinkedIn link")]
        [StringLength(64)]
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
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }
    }

    public class UserProfileTypeDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 ProfileId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string ProfileName { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }
    }

    public class EmployeeBankDetails
    {

        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(64)]
        public string BankName { get; set; }

        [StringLength(32)]
        public string AccountNumber { get; set; }

        [StringLength(128)]
        public string AccountOwner { get; set; }

        [StringLength(32)]
        public string IfscCode { get; set; }

        [StringLength(16)]
        public string BranchCode { get; set; }

        [StringLength(128)]
        public string BranchAddress { get; set; }

        [StringLength(16)]
        public string ContactNumber { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

    }

    public class EmpAddressDetails
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
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

        [ForeignKey("CityId")]
        public virtual CityMaster CityMaster { get; set; }

        [ForeignKey("StateId")]
        public virtual StatesMaster StateMaster { get; set; }

        [ForeignKey("CountryId")]
        public virtual CountryMaster CountryMaster { get; set; }
    }

    public class EmpSkillView
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(64)]
        public string SkillName { get; set; }

        [StringLength(128)]
        public string ProfileName { get; set; }

        [Key]
        [Column(Order = 2)]
        public Int64 ProfileId { get; set; }

        public short YearofExperience { get; set; }

        [ForeignKey("ProfileId")]
        public virtual UserProfileTypeDetails UserProfileTypeDetails { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }


    }

    public class EmpExperienceView
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

    public class EmpEducationView
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
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }
    }

    public class MambersView
    {
        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(256)]
        public string UserName { get; set; }

        [StringLength(128)]
        public string RoleId { get; set; }

        public bool Deactivated { get; set; }

        [DefaultValue(false)]
        public bool ManagerLevel { get; set; }
    }

    public partial class EmpAddressViewModel
    {
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
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
    //Changed by : Achal Jha 
    //Chage on : 13-05-2017
    //Reason : For Payroll of Employee.
    public class EmployeeAssetsIssueDetails
    {
        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(64)]
        public string AssetsStatus { get; set; }

        [StringLength(128)]
        public string AssetsDetails { get; set; }

        [StringLength(16)]
        public string AssetsID { get; set; }

        [StringLength(512)]
        public string Remark { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime ValidTill { get; set; }

        public DateTime? ReturnOn { get; set; }

        [ForeignKey("AssetsID")]
        public virtual AssetsMaster AssetsMaster { get; set; }
    }


    //Achal Jha 13/05/2017

    public class GradeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 GradeId { get; set; }

        [StringLength(32)]
        public string GradeName { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class AssetsMaster
    {
        [Key]
        [StringLength(16)]
        public string AssetsID { get; set; }

        [StringLength(32)]
        public string AssetsType { get; set; }

        [StringLength(256)]
        public string AssetsDetails { get; set; }

        public float AssetsValue { get; set; }

    }

    public class EmployeeLoanDetails
    {
        [Key]
        [StringLength(16)]
        public string UserId { get; set; }

        [StringLength(32)]
        public string LoanType { get; set; }

        public float LoanAmount { get; set; }

        public DateTime DateofIssue { get; set; }

        //in month
        public int LoanPeriod { get; set; }

        public float Repayment { get; set; }
    }

    /// <summary>
    /// Created by Achal Jha
    /// Created on 29.05.2017
    /// Created for : Payroll
    /// </summary>

    public class PayrollLeavsSettings
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string SubscriberId { get; set; }

        //[Key]
        //[Column(Order = 1)]
        //public DateTime DateFrom { get; set; }

        [Key]
        [Column(Order = 2)]
        public int LeaveId { get; set; }

        [StringLength(32)]
        public string LeaveName { get; set; }

        public Int16 NoofDays { get; set; }

        //calculateon No of Workingdays /Total No of Days 
        public Int16 SalarycalculationOn { get; set; }

        //Holidays Include or Exclude Status
        public Int16 HolidayInSalary { get; set; }

        //[ForeignKey("SubscriberId")]
        //public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class PayrollHeadSettings
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string SubscriberId { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime DateFrom { get; set; }

        [Key]
        [Column(Order = 2)]
        public Int16 HeadId { get; set; }

        [StringLength(32)]
        public string HeadName { get; set; }

        public float Deduction { get; set; }

        public Int16 DeductionCriteria { get; set; }

        //[ForeignKey("HeadId")]
        //public virtual PayrollHeads PayrollHeads { get; set; }

        //[ForeignKey("SubscriberId")]
        //public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class EmployeePayrollSettings
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string SubscriberId { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime DateFrom { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(128)]
        public string UserId { get; set; }

        public Int16 HeadId { get; set; }

        public float Amount { get; set; }

        //[ForeignKey("HeadId")]
        //public virtual PayrollHeads PayrollHeads { get; set; }

        //[ForeignKey("UserId")]
        //public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

    }

    public class EmployeePayrollDetails
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string SubscriberId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        public Int16 Month { get; set; }

        [Key]
        [Column(Order = 3)]
        public int Year { get; set; }

        public Int16 HeadId { get; set; }

        public float Amount { get; set; }

        //[ForeignKey("SubscriberId")]
        //public virtual CorporateProfile CorporateProfile { get; set; }

        //[ForeignKey("UserId")]
        //public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }
    }

    public class EmployeePayroll
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string SubscriberId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        public Int16 Month { get; set; }

        [Key]
        [Column(Order = 3)]
        public int Year { get; set; }

        public Int16 TotalNoofDays { get; set; }

        public Int16 TotalNoofWorkingDays { get; set; }

        public float GrossAmount { get; set; }

        public float TotalDeductions { get; set; }

        public float NetAmount { get; set; }

        //[ForeignKey("SubscriberId")]
        //public virtual CorporateProfile CorporateProfile { get; set; }

        //[ForeignKey("UserId")]
        //public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }
    }

    public class Certification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 CertificationId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string Certificate { get; set; }

        [StringLength(64)]
        public string Specialization { get; set; }

        [StringLength(64)]
        public string Institution { get; set; }

        [StringLength(4)]
        public string YearOfPassing { get; set; }

        [StringLength(16)]
        public string Percentage { get; set; }
    }
    /// <summary>
    /// Created By :- Preeti Singh
    /// Created On :- 16-08-2017
    /// Model EmpJoining Detail
    /// </summary>
    public class EmpJoiningDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 JoiningId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? JoiningDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ConfirmationDate { get; set; }

        public Int16 StatusId { get; set; }

        public Int16 ProbationPeriod { get; set; }

        public Int16 NoticePeriod { get; set; }

        public Int64 GradeId { get; set; }

        [StringLength(32)]
        public string WorkLocation { get; set; }

        [DefaultValue(0)]
        public Int64 BranchId { get; set; }

        public Int64 ShiftId { get; set; }

        public Int16 SchemeId { get; set; }


        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

        [ForeignKey("GradeId")]
        public virtual GradeMaster GradeMaster { get; set; }

    }

    /// <summary>
    /// Created By :- Preeti Singh
    /// CreatedOn :- 16-08-2017
    /// </summary>
    public class EmpDesignationHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 EmpDesignationId { get; set; }

        public Int64 DesignationId { get; set; }


        [StringLength(128)]
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EffectiveDate { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

        [ForeignKey("DesignationId")]
        public virtual Designation Designation { get; set; }

    }

    /// <summary>
    /// Created By :- Preeti Singh
    /// CreatedOn :- 16-08-2017
    /// </summary>
    public class StatusMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int16 StatusId { get; set; }

        [StringLength(128)]
        public string StatusName { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

    }

    public class EmployeeImage
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

    }

    public class Designation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 DesignationId { get; set; }

        [StringLength(128)]
        public string DesignationName { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }


    }

    public class EmployeeView
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        public string Gender { get; set; }

        public string MaritalStatus { get; set; }

        public string AlternateContact { get; set; }

        public string AlternateEmail { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Nationality { get; set; }

        public string SubscriberId { get; set; }

        public Int64? DesignationId { get; set; }

        public string DesignationName { get; set; }

        public string EmployeeId { get; set; }

        public bool Emplanelled { get; set; }

        public bool ManagerLevel { get; set; }

        public string ReportingAuthority { get; set; }

        public string DepartmentId { get; set; }

        public string FatherName { get; set; }

        public string SpouseName { get; set; }

        public bool Deactivated { get; set; }

        public string EmergencyContactName { get; set; }

        public string EmergencyContactNumber { get; set; }

        public string BloodGroup { get; set; }

        public bool PhysicallyChallenged { get; set; }

        public string Location { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? MarriageDate { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public Int64 JoiningId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? JoiningDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ConfirmationDate { get; set; }

        public Int16 StatusId { get; set; }

        public string StatusName { get; set; }

        public Int16 ProbationPeriod { get; set; }

        public Int16 NoticePeriod { get; set; }

        public Int64 GradeId { get; set; }

        public string GradeName { get; set; }

        public string WorkLocation { get; set; }

        public Int64 BranchId { get; set; }

        public Int64 ShiftId { get; set; }

        public Int16 SchemeId { get; set; }

        public Int64 ESID { get; set; }

        public float MonthlyCTC { get; set; }

        public float AnnualCTC { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        [StringLength(256)]
        public string Remarks { get; set; }

        public int PayoutYear { get; set; }

        public Int16 PayoutMonth { get; set; }

        //Created BY VIKASH DAS
        public Int64 EngagementTypeId { get; set; }

        public Int16 MaxLimit { get; set; }

        public string EJoiningDate { get; set; }

        public string EConfirmationDate { get; set; }

    }

    public class TrainerPlannerSummary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 PlannerSummaryId { get; set; }

        [StringLength(128)]
        public string TrainerId { get; set; }

        public Int16 SchemeId { get; set; }

        public Int64 EngagementTypeId { get; set; }

        public float EngagementCount { get; set; }

        public Int32 LeaveYear { get; set; }

        [DefaultValue(0)]
        public float MaxLimit { get; set; }

        [ForeignKey("SchemeId")]
        public virtual LeaveSchemeMaster LeaveSchemeMaster { get; set; }

        [ForeignKey("EngagementTypeId")]
        public virtual EngagementTypeMaster EngagementTypeMaster { get; set; }
    }

    public class ExcelviewModel
    {
        public Int16 SchemeId { get; set; }

        public Int64 EngagementTypeId { get; set; }

        [StringLength(64)]
        public string EngagementType { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }
    }

    public class Resignation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 ResignationId { get; set; }

        public Int64 ReasonId { get; set; }

        public string RelievingReason { get; set; }

        public DateTime DateofResignation { get; set; }

        public DateTime LastWorkingDate { get; set; }

        public Int16 Status { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public DateTime? AprrovedOn { get; set; }

        [StringLength(128)]
        public string ApprovedBy { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

        [ForeignKey("ReasonId")]
        public virtual LeavingReason LeavingReason { get; set; }

    }

    public class ResignationViewModel
    {
        [Key]
        public Int64 ResignationId { get; set; }

        public Int64 ReasonId { get; set; }

        [StringLength(64)]
        public string Reason { get; set; }

        public string RelievingReason { get; set; }

        public DateTime DateofResignation { get; set; }

        public DateTime LastWorkingDate { get; set; }

        public Int16 Status { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public DateTime? AprrovedOn { get; set; }

        [StringLength(128)]
        public string ApprovedBy { get; set; }

        [StringLength(128)]
        public string ApprovedByName { get; set; }

        public string Name { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

        [ForeignKey("ReasonId")]
        public virtual LeavingReason LeavingReason { get; set; }

    }

    public class EmployeeRegistrationDetailViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string SubscriberId { get; set; }

    }

    enum ApprovalStatus
    {
        Applied = 1
    ,
        Approved = 2
            ,
        Disapproved = 3
            , Cancelled = 4
    }
    public class EmployeeConfirmation
    {

        [Key]
        [StringLength(32)]
        public string LetterId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public DateTime? AprrovedOn { get; set; }

        [StringLength(128)]
        public string ApprovedBy { get; set; }

        public Int16 Status { get; set; }

        [StringLength(1024)]
        public string Remarks { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

    }
    public class EmployeeConfirmationView
    {
        public string LetterId { get; set; }
        public string UserId { get; set; }
        public string JoininDate { get; set; }
        public string ConfirmationDate { get; set; }
        public string Name { get; set; }
        public string AprrovedOn { get; set; }
        public string ApprovedBy { get; set; }
        public string SubscriberId { get; set; }
        public string Remarks { get; set; }
        public Int16 Status { get; set; }

    }
    public class SubscriberLogInHistory
    {
        [Key]
        [StringLength(128)]
        public string SubscriberId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LastLogIn { get; set; }


    }
    public class EmpAttendanceAppView
    {
        public string Date { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
    }
}

