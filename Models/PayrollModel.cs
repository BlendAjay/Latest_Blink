using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace AJSolutions.Models
{
    public class LetterLogoAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        public Int64 TemplateId { get; set; }

        [ForeignKey("TemplateId")]
        public virtual CorporateTemplate CorporateTemplate { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class CorporateTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TemplateId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [AllowHtml]
        [StringLength(2048)]
        public string Header { get; set; }

        [Required]
        [AllowHtml]
        public string Content { get; set; }

        [AllowHtml]
        [StringLength(2048)]
        public string Footer { get; set; }

        public Int64 LetterTypeId { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        public Boolean SameAsCompanyLogo { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

        [ForeignKey("LetterTypeId")]
        public virtual LetterType LetterType { get; set; }

    }

    public class CorporateLetter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 LetterId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        public Int64 TemplateId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public DateTime? HeaderDate { get; set; }

        public DateTime? FooterDate { get; set; }

        [StringLength(64)]
        public string Status { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(128)]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Int64 LetterTypeId { get; set; }

        [StringLength(64)]
        public string ReferenceNo { get; set; }

        [ForeignKey("TemplateId")]
        public virtual CorporateTemplate CorporateTemplate { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

        [ForeignKey("LetterTypeId")]
        public virtual LetterType LetterType { get; set; }
    }

    //Created by : Anamika Pandey
    //Created on : 11-08-2017


    //public class SalaryComponent
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    //    public Int32 ComponentId { get; set; }

    //    [StringLength(64)]
    //    public string ComponentName { get; set; }

    //    public float CompPercentage { get; set; }

    //    public float MaxValue { get; set; }

    //    public Int16 ComponentType { get; set; }

    //    [StringLength(128)]
    //    public string CorporateId { get; set; }

    //    [ForeignKey("CorporateId")]
    //    public virtual CorporateProfile CorporateProfile { get; set; }
    //}

    public class ClaimCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 CategoryId { get; set; }

        [StringLength(128)]
        public string CategoryName { get; set; }

        public Int16 CategoryType { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class AssetStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 AssetStatusId { get; set; }

        [StringLength(32)]
        public string AssetStatusName { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class HoldSalaryReason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 ReasonId { get; set; }

        [StringLength(64)]
        public string HoldReason { get; set; }

        [StringLength(128)]
        public string Description { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    //public class HoldSalaryPayoutReason
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    //    public Int32 ReasonId { get; set; }

    //    [StringLength(64)]
    //    public string ReasonType { get; set; }

    //    [StringLength(128)]
    //    public string Description { get; set; }

    //    [StringLength(128)]
    //    public string CorporateId { get; set; }

    //    [ForeignKey("CorporateId")]
    //    public virtual CorporateProfile CorporateProfile { get; set; }
    //}

    //public class ReleaseSalaryReason
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    //    public Int32 ReasonId { get; set; }

    //    [StringLength(64)]
    //    public string ReasonType { get; set; }

    //    [StringLength(128)]
    //    public string Description { get; set; }

    //    [StringLength(128)]
    //    public string CorporateId { get; set; }

    //    [ForeignKey("CorporateId")]
    //    public virtual CorporateProfile CorporateProfile { get; set; }
    //}

    public class LeavingReason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 ReasonId { get; set; }

        [StringLength(64)]
        public string Reason { get; set; }

        [StringLength(128)]
        public string Description { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class Relation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int32 RelationId { get; set; }

        [StringLength(64)]
        public string RelationType { get; set; }

    }


    //public class AttendanceScheme
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    //    public Int32 AttendanceTypeId { get; set; }

    //    [StringLength(64)]
    //    public string AttendanceTypeName { get; set; }

    //    [StringLength(128)]
    //    public string CorporateId { get; set; }


    public class PayRollLookUps
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 LookUpsId { get; set; }

        [StringLength(64)]
        public string LookUpsName { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class LetterFieldNameViewModel
    {
        public string Coloum { get; set; }
    }

    public class CorporateTemplateView
    {
        public Int64 ContentId { get; set; }

        public Int64 LetterId { get; set; }

        public string Name { get; set; }

        public Int64 TemplateId { get; set; }

        public string Content { get; set; }

        public string CorporateId { get; set; }

        public Int64 LetterTypeId { get; set; }
    }

    public class AssetGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int32 AssetGroupId { get; set; }

        [StringLength(64)]
        public string AssetGroupName { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

    }

    public class AssetType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int32 AssteTypeId { get; set; }

        [StringLength(64)]
        public string AssetTypeName { get; set; }

        public Int32 AssetGroupId { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("AssetGroupId")]
        public virtual AssetGroup AssetGroup { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class EmpDetailPlaceholderView
    {
        public string Name { get; set; }

        public string Gender { get; set; }

        public string FatherName { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string ConfirmationDate { get; set; }

        public string JoiningDate { get; set; }

        public string DOB { get; set; }

        public string PostalCode { get; set; }

        public string ProbationPeriod { get; set; }

        public string StatusName { get; set; }

        public string AlternateContact { get; set; }

        public string AlternateEmail { get; set; }

        public string DesignationName { get; set; }

        public string WorkLocation { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string ReferenceNo { get; set; }

        public string CompanyAddressLine1 { get; set; }

        public string CompanyAddressLine2 { get; set; }

        public string CompanyPostalCode { get; set; }
    }

    public class LetterDesignView
    {
        public Int64 TemplateId { get; set; }

        public Int64 LetterTypeId { get; set; }

        [AllowHtml]
        [StringLength(2048)]
        public string Header { get; set; }

        public Boolean SameAsCompanyLogo { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public string CorporateId { get; set; }

        [AllowHtml]
        [StringLength(2048)]
        public string Footer { get; set; }

        public Int64? FileId { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public string Name { get; set; }

    }

    public class LetterType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 LetterTypeId { get; set; }

        [StringLength(64)]
        public string LetterTypeName { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class SendLetter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public Int64 LetterTypeId { get; set; }

        public DateTime IssuingDate { get; set; }

        [ForeignKey("LetterTypeId")]
        public virtual LetterType LetterType { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class SendLetterView
    {
        public Int64 FileId { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public string CorporateId { get; set; }

        public string UserId { get; set; }

        public Int64 LetterTypeId { get; set; }

        public DateTime IssuingDate { get; set; }

        public string Name { get; set; }

        public string LetterTypename { get; set; }
    }

    public class BiometricCheckInCheckOut
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 BiometricId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CheckInDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? CheckInTime { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CheckOutDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? CheckOutTime { get; set; }

        [StringLength(128)]
        public string IpAddress { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        public Int64 ShiftId { get; set; }

        [ForeignKey("ShiftId")]
        public virtual ShiftMaster ShiftMaster { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }
    }

    public class BiometricCheckInCheckOutview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 BiometricId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public string Name { get; set; }

        public string EmployeeId { get; set; }

        public string DepartmentId { get; set; }

        public bool Deactivated { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CheckInDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? CheckInTime { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CheckOutDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? CheckOutTime { get; set; }

        [StringLength(128)]
        public string IpAddress { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        public Int64 ShiftId { get; set; }

        [ForeignKey("ShiftId")]
        public virtual ShiftMaster ShiftMaster { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }
    }

    public class BranchLocationView
    {
        public Int64 IpId { get; set; }

        public Int64 BranchId { get; set; }

        public string BranchName { get; set; }

        public Int16 Authenticate { get; set; }

        public string IPAddressFrom { get; set; }

        public string IPAddressTo { get; set; }

        public float LatitudeFrom { get; set; }

        public float LatitudeTo { get; set; }

        public float LongitudeFrom { get; set; }

        public float LongitudeTo { get; set; }

        public string UserId { get; set; }
    }

    public class IpMasters
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 IpId { get; set; }

        [DefaultValue(0)]
        public Int64 BranchId { get; set; }

        [DefaultValue(0)]
        public Int16 Authenticate { get; set; }

        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$")]
        public string IPAddressFrom { get; set; }

        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$")]
        public string IPAddressTo { get; set; }

        [DefaultValue(0.0)]
        public float LatitudeFrom { get; set; }

        [DefaultValue(0.0)]
        public float LatitudeTo { get; set; }

        [DefaultValue(0.0)]
        public float LongitudeFrom { get; set; }

        [DefaultValue(0.0)]
        public float LongitudeTo { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }
    }

    public class EmployeeBiometricView
    {
        [Key]
        [StringLength(128)]
        public string CorporateId { get; set; }

        public string Name { get; set; }

        public string CompanyName { get; set; }

        public string Website { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public int TotalEmployee { get; set; }

        public List<BiometricCheckInCheckOutview> BiometricCheckInCheckOutview { get; set; }

    }

    public class PayrollHeads
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int16 PayrollHeadID { get; set; }

        [StringLength(128)]
        public string HeadName { get; set; }

        public float DefaultPercentage { get; set; }

        [StringLength(10)]
        public string Category { get; set; }

        public float MaxLimit { get; set; }

        [StringLength(10)]
        public string Period { get; set; }

        [DefaultValue(false)]
        public bool TaxExemption { get; set; }

        [DefaultValue(false)]
        public bool AvailableByDefault { get; set; }

    }

    public class CorporatePayrollHead
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 CorporatePayrollHeadID { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        public Int16 PayrollHeadID { get; set; }

        [StringLength(128)]
        public string PayrollHeadName { get; set; }

        public float PayrollPercent { get; set; }

        [StringLength(10)]
        public string PayrollCategory { get; set; }

        [DefaultValue(false)]
        public float MaxLimit { get; set; }

        [StringLength(10)]
        public string Period { get; set; }

        [DefaultValue(false)]
        public bool TaxExemption { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class CorporatePayrollHeadview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 CorporatePayrollHeadID { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        public float DefaultPercentage { get; set; }

        public bool AvailableByDefault { get; set; }

        public Int16 PayrollHeadID { get; set; }

        [StringLength(128)]
        public string PayrollHeadName { get; set; }

        public float PayrollPercent { get; set; }

        [StringLength(10)]
        public string PayrollCategory { get; set; }

        [DefaultValue(false)]
        public float MaxLimit { get; set; }

        [StringLength(10)]
        public string Period { get; set; }

        [DefaultValue(false)]
        public bool TaxExemption { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class CorporatePayrollSettings
    {
        [Key]
        [StringLength(128)]
        public string CorporateId { get; set; }

        [DefaultValue(false)]
        public bool PayslipPasswordProtection { get; set; }

        [DefaultValue(false)]
        public bool AutoEnablePayrollProcess { get; set; }

        public Int16 AutoProcessDate { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class EmployeeSalary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 ESID { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public float MonthlyCTC { get; set; }

        public float AnnualCTC { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        [StringLength(256)]
        public string Remarks { get; set; }

        public int PayoutYear { get; set; }

        public Int16 PayoutMonth { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }
    }

    public class EmployeeSalaryHeads
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 EmployeeSalaryHeadId { get; set; }

        public Int64 ESID { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public Int64 CorporatePayrollHeadID { get; set; }

        public float Amount { get; set; }

        [ForeignKey("ESID")]
        public virtual EmployeeSalary EmployeeSalary { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

        [ForeignKey("CorporatePayrollHeadID")]
        public virtual CorporatePayrollHead CorporatePayrollHead { get; set; }
    }

    public class EmployeeSalaryHeadsView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 EmployeeSalaryHeadId { get; set; }

        public Int64 ESID { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public Int64 CorporatePayrollHeadID { get; set; }

        public float Amount { get; set; }

        [StringLength(128)]
        public string PayrollHeadName { get; set; }

        public float PayrollPercent { get; set; }

        [StringLength(10)]
        public string PayrollCategory { get; set; }

        [DefaultValue(false)]
        public float MaxLimit { get; set; }

        [StringLength(10)]
        public string Period { get; set; }

        [DefaultValue(false)]
        public bool TaxExemption { get; set; }

        public DateTime? EffectiveFrom { get; set; }
    }

    public class EmployeeMonthlySalaryPayout
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 EmployeeMonthlySalaryPayoutId { get; set; }

        public Int64 ESID { get; set; }

        public float GrossCTC { get; set; }

        public float NetCTC { get; set; }

        public int WorkingDays { get; set; }

        public int LWP { get; set; }

        public int TotalLeaves { get; set; }

        public int PayoutYear { get; set; }

        public Int16 PayoutMonth { get; set; }

        [DefaultValue(false)]
        public bool Freeze { get; set; }

        [StringLength(256)]
        public string Comments { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        [ForeignKey("ESID")]
        public virtual EmployeeSalary EmployeeSalary { get; set; }
    }

    public class EmployeeMonthlySalaryHeads
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 EmployeeMonthlySalaryHeadId { get; set; }

        public Int64 EmployeeMonthlySalaryPayoutId { get; set; }

        public Int64 CorporatePayrollHeadID { get; set; }

        public float Amount { get; set; }

        [ForeignKey("EmployeeMonthlySalaryPayoutId")]
        public virtual EmployeeMonthlySalaryPayout EmployeeMonthlySalaryPayout { get; set; }
    }

    public class IncomeTaxSlab
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 IncomeTaxSlabId { get; set; }

        public float IncomeTaxSlabFrom { get; set; }

        public float IncomeTaxSlabTo { get; set; }

        public Int16 IncomeTaxRate { get; set; }

        public Int16 Educationcess { get; set; }

        public Int16 SecondaryAndHigherEducationCess { get; set; }
    }

    public class CompanyDetailsView
    {
        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(64)]
        public string CompanyName { get; set; }

        [StringLength(32)]
        public string CompanyType { get; set; }

        [StringLength(32)]
        public string CompanySize { get; set; }

        [StringLength(64)]
        [RegularExpression(@"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?)", ErrorMessage = "Invalid Website name")]
        public string Website { get; set; }

        [StringLength(128)]
        public string ProvidentFund { get; set; }

        [StringLength(16)]
        public string PANCardNo { get; set; }

        [StringLength(16)]
        public string TaxDeductionAccNo { get; set; }

        [StringLength(64)]
        public string EmployeeStateInsurance { get; set; }

        [StringLength(16)]
        public string GSTTax { get; set; }

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

        [StringLength(128)]
        public string Name { get; set; }

        //[RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        [StringLength(16)]
        public string AlternateContact { get; set; }

        [StringLength(64)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string AlternateEmail { get; set; }
    }

    public class EmployeeSalaryHeadView
    {
        public Int64 CorporatePayrollHeadID { get; set; }

        public string PayrollHeadName { get; set; }

        public float Amount { get; set; }
    }

    //Modal for Androied DeviceId & Name
    //Created By : Vikas Pandey
    //Created On : 18-08-2018
    public class DeviceDetail
    {
        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(64)]
        public string DeviceId { get; set; }

        [StringLength(64)]
        public string DeviceName { get; set; }

        [StringLength(256)]
        public string TokenId { get; set; }

    }
}
