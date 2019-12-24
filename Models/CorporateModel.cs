using AJSolutions.Areas.CMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AJSolutions.Models
{
    public class CorporateProfile
    {
        [Key]
        [StringLength(128)]
        public string CorporateId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        //[RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        [StringLength(16)]
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

        [StringLength(128)]
        public string ReferenceId { get; set; }

        public string Pcode { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMaster { get; set; }

        [DefaultValue(false)]
        public bool Deactivated { get; set; }

        [StringLength(128)]
        public string RegistrationId { get; set; }

        public virtual ICollection<AdminLogoFile> AdminLogoFile { get; set; }
    }

    public class Address
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(128)]
        public string CorporateId { get; set; }

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

        [StringLength(32)]
        public string FaxNo { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

        [ForeignKey("CityId")]
        public virtual CityMaster CityMaster { get; set; }

        [ForeignKey("StateId")]
        public virtual StatesMaster StateMaster { get; set; }

        [ForeignKey("CountryId")]
        public virtual CountryMaster CountryMaster { get; set; }
    }

    public class CompanyProfile
    {
        [Key]
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

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class BankDetails
    {

        [Key]
        [StringLength(128)]
        public string CorporateId { get; set; }

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
        [RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        public string ContactNumber { get; set; }

    }

    public class TaxMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TaxationId { get; set; }

        public string TaxName { get; set; }

        public float TaxationValue { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

    }

    public class JobOrderTypeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int JobOrderTypeId { get; set; }

        [StringLength(128)]
        public string JobOrderType { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class ItemTypeMasters
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 ItemTypeId { get; set; }

        [StringLength(128)]
        public string ItemTypeName { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

    }

    public class GenerateInvoice
    {
        [Key]
        [StringLength(32)]
        public string InvoiceNumber { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [Required]
        [StringLength(128)]
        public string InvoiceTo { get; set; }

        [StringLength(16)]
        public string ReferenceId { get; set; }

        public float Total { get; set; }

        public float AdditionalCost { get; set; }

        public float Deductions { get; set; }

        public float NetAmount { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime InvoiceDate { get; set; }

        [StringLength(256)]
        public string InvoiceSubject { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        [StringLength(16)]
        public string Status { get; set; }

        [DefaultValue(false)]
        public bool Acknowledge { get; set; }

        [StringLength(512)]
        public string EntryDescription { get; set; }

        [StringLength(512)]
        public string Remarks { get; set; }

        [StringLength(64)]
        public string ReferenceNumber { get; set; }

        public Int16 PaymentModeId { get; set; }

        [StringLength(64)]
        public string BankName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PaymentDate { get; set; }

        [StringLength(16)]
        public string BankCode { get; set; }

        public string PGComment { get; set; }

        [StringLength(512)]
        public string PayerRemarks { get; set; }

        [StringLength(128)]
        public string ApprovedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        [Required]
        [StringLength(128)]
        public string SubscriberId { get; set; }

        [ForeignKey("SubscriberId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

    }

    public class InvoiceTaxationDetails
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(32)]
        public string InvoiceNumber { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int64 TaxationId { get; set; }

        public float TaxactionAmount { get; set; }
    }

    public class InvoiceTaxationDetailsView
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(32)]
        public string InvoiceNumber { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int64 TaxationId { get; set; }

        public string TaxName { get; set; }

        public float TaxationValue { get; set; }

        public float TaxactionAmount { get; set; }
    }

    public class InvoiceItems
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(16)]
        public string InvoiceNumber { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int16 ItemId { get; set; }

        public Int64 ItemTypeId { get; set; }

        public string ItemDescription { get; set; }

        public int Unit { get; set; }

        public float UnitPrice { get; set; }

        [DefaultValue(0)]
        public int Duration { get; set; }

        [ForeignKey("InvoiceNumber")]
        public virtual GenerateInvoice GenerateInvoice { get; set; }

        //[ForeignKey("ItemTypeId")]
        //public virtual ItemTypeMasters ItemTypeMaster { get; set; }

    }

    public class InvoiceItemsView
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(16)]
        public string InvoiceNumber { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int16 ItemId { get; set; }

        public Int64 ItemTypeId { get; set; }

        [StringLength(128)]
        public string ItemTypeName { get; set; }

        public string ItemDescription { get; set; }

        public int Unit { get; set; }

        public float UnitPrice { get; set; }

        [DefaultValue(0)]
        public int Duration { get; set; }

    }

    public class CourseMaster
    {

        [Key]
        [StringLength(16)]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(128)]
        public string CourseName { get; set; }

        public Int64 CategoryId { get; set; }

        [StringLength(32)]
        public string CourseDuration { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        public double CourseFee { get; set; }

        public double CourseLateFee { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        [DefaultValue(false)]
        public bool DiscussionForum { get; set; }

        public Int16 ContentVisiblity { get; set; }

        [DefaultValue(0)]
        public int CountLikes { get; set; }

        [StringLength(1024)]
        public string CourseDescription { get; set; }

        [StringLength(16)]
        public string LMSCourseCode { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

    }

    public class AdditionalCourseFee
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(16)]
        public string CourseCode { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(128)]
        public string CorporateId { get; set; }

        public float Accommodation { get; set; }

        public float Transport { get; set; }

        public float Others { get; set; }

        public float Discount { get; set; }

        public float InstallmentInterest { get; set; }

        [ForeignKey("CourseCode")]
        public virtual CourseMaster CourseMaster { get; set; }

    }

    public class CourseMasterwithAddtionalView
    {
        [StringLength(16)]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(128)]
        public string CourseName { get; set; }

        public Int64 CategoryId { get; set; }

        [StringLength(32)]
        public string CourseDuration { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        public double CourseFee { get; set; }

        public double CourseLateFee { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        [DefaultValue(false)]
        public bool DiscussionForum { get; set; }

        public Int16 ContentVisiblity { get; set; }

        [DefaultValue(0)]
        public int CountLikes { get; set; }

        public int TotalBatches { get; set; }

        public int Totaltopics { get; set; }

        public int totallecture { get; set; }

        public string TrainingId { get; set; }

        [StringLength(1024)]
        public string CourseDescription { get; set; }

        [StringLength(16)]
        public string LMSCourseCode { get; set; }

        public float Accommodation { get; set; }

        public float Transport { get; set; }

        public float Others { get; set; }

        public float Discount { get; set; }

        public float InstallmentInterest { get; set; }



        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

        //[ForeignKey("TrainingId")]
        //public virtual TrainingScheduleView TrainingScheduleView { get; set; }

    }

    public partial class CourseAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [ForeignKey("CourseCode")]
        public virtual CourseMaster CourseMaster { get; set; }
    }

    public class TaskMaster
    {
        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string CreatedBy { get; set; }

        [Key]
        [StringLength(32)]
        public string TaskId { get; set; }

        public int TaskTypeId { get; set; }

        [StringLength(16)]
        public string JobOrderNumber { get; set; }

        [StringLength(512)]
        public string Subject { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        public int Duration { get; set; }

        [StringLength(512)]
        public string Venue { get; set; }

        [StringLength(32)]
        public string InvoiceFrequency { get; set; }

        [StringLength(32)]
        public string City { get; set; }

        [StringLength(32)]
        public string State { get; set; }

        [StringLength(32)]
        public string Country { get; set; }

        public short TaskStatus { get; set; }

        [StringLength(128)]
        public string AssignedTo { get; set; }

        public float TaskAmount { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        public DateTime? CreatedOn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }

        [ForeignKey("TaskTypeId")]
        public virtual JobOrderTypeMaster JobOrderTypeMaster { get; set; }

    }

    public class TaskItems
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(32)]
        public string TaskId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Int16 ItemId { get; set; }

        public Int64 ItemTypeId { get; set; }

        public string ItemDescription { get; set; }

        public int Unit { get; set; }

        public float UnitPrice { get; set; }

        [DefaultValue(0)]
        public int Duration { get; set; }

        [ForeignKey("TaskId")]
        public virtual TaskMaster TaskMaster { get; set; }

        //[ForeignKey("ItemTypeId")]
        //public virtual ItemTypeMasters ItemTypeMaster { get; set; }
    }

    public partial class TaskAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(32)]
        public string TaskId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [ForeignKey("TaskId")]
        public virtual TaskMaster TaskMaster { get; set; }
    }

    public partial class TaskFinalAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(32)]
        public string TaskId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [ForeignKey("TaskId")]
        public virtual TaskMaster TaskMaster { get; set; }
    }

    public class TaskItemsView
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(32)]
        public string TaskId { get; set; }

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

    public class CourseMasterView
    {
        [StringLength(16)]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(128)]
        public string CourseName { get; set; }

        public Int64 CategoryId { get; set; }

        public string CategoryName { get; set; }

        [StringLength(32)]
        public string CourseDuration { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        public double CourseFee { get; set; }

        public double CourseLateFee { get; set; }

        public double TotalFees { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        [DefaultValue(false)]
        public bool DiscussionForum { get; set; }

        public Int16 ContentVisiblity { get; set; }

        [DefaultValue(0)]
        public int CountLikes { get; set; }

        public int TotalBatches { get; set; }

        public int Totaltopics { get; set; }

        public int totallecture { get; set; }

        public string TrainingId { get; set; }

        [StringLength(1024)]
        public string CourseDescription { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string NewFileName { get; set; }

        [StringLength(16)]
        public string LMSCourseCode { get; set; }

        public float Accommodation { get; set; }

        public float Transport { get; set; }

        public float Others { get; set; }

        public float Discount { get; set; }

        public float InstallmentInterest { get; set; }

        public double TotalFeeAmount { get; set; }

        [DefaultValue(false)]
        public bool IsAccommodation { get; set; }

        [DefaultValue(false)]
        public bool IsTransport { get; set; }

        [DefaultValue(false)]
        public bool IsOthers { get; set; }

        [DefaultValue(false)]
        public bool IsDiscount { get; set; }

        [DefaultValue(false)]
        public bool IsInstallmentInterest { get; set; }

        public Int16 InstallmentId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

        //[ForeignKey("TrainingId")]
        //public virtual TrainingScheduleView TrainingScheduleView { get; set; }

    }

    public class feesettingview
    {
        [StringLength(16)]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(128)]
        public string CourseName { get; set; }

        public double CourseFee { get; set; }

        public double CourseLateFee { get; set; }

        public double TotalFees { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public float Accommodation { get; set; }

        public float Transport { get; set; }

        public float Others { get; set; }

        public float Discount { get; set; }

        public double TotalFeeAmount { get; set; }

        [DefaultValue(false)]
        public bool IsAccommodation { get; set; }

        [DefaultValue(false)]
        public bool IsTransport { get; set; }

        [DefaultValue(false)]
        public bool IsOthers { get; set; }

        [DefaultValue(false)]
        public bool IsDiscount { get; set; }

        public Int16 InstallmentId { get; set; }
    }


    public partial class CourseFileManagerView
    {
        public Int64 ContentFileId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        [StringLength(128)]
        public string ActualFileName { get; set; }

        [StringLength(128)]
        public string NewFileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }
    }

    public class TaskMasterView
    {

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string CreatedBy { get; set; }

        [Key]
        [StringLength(32)]
        public string TaskId { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        public int TaskTypeId { get; set; }

        [StringLength(16)]
        public string JobOrderNumber { get; set; }

        [StringLength(512)]
        public string Subject { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        public int Duration { get; set; }

        [StringLength(512)]
        public string Venue { get; set; }

        [StringLength(32)]
        public string InvoiceFrequency { get; set; }

        [StringLength(32)]
        public string City { get; set; }

        [StringLength(32)]
        public string State { get; set; }

        [StringLength(32)]
        public string Country { get; set; }

        public short TaskStatus { get; set; }

        [StringLength(128)]
        public string AssignedTo { get; set; }

        [StringLength(128)]
        public string TaskType { get; set; }

        [StringLength(128)]
        public string AssignedToName { get; set; }

        public float TaskAmount { get; set; }

        [StringLength(128)]
        public string CreatedByName { get; set; }

        public string TInv { get; set; }

        [StringLength(64)]
        public string CountryName { get; set; }

        [StringLength(64)]
        public string StateName { get; set; }

        [StringLength(64)]
        public string CityName { get; set; }

        [ForeignKey("TaskTypeId")]
        public virtual JobOrderTypeMaster JobOrderTypeMaster { get; set; }

        [StringLength(32)]
        public string InvoiceNumber { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CompletedOn { get; set; }

    }

    public class GenerateInvoiceView
    {
        [Key]
        [StringLength(32)]
        public string InvoiceNumber { get; set; }

        [Required]
        [StringLength(128)]
        public string SubscriberId { get; set; }

        [Required]
        [StringLength(128)]
        public string InvoiceTo { get; set; }

        [StringLength(128)]
        public string InvoiceToName { get; set; }

        [StringLength(64)]
        public string InvoiceToCompany { get; set; }

        [StringLength(64)]
        public string InvoiceToAddressLine1 { get; set; }

        [StringLength(64)]
        public string InvoiceToAddressLine2 { get; set; }

        [StringLength(64)]
        public string InvoiceToCity { get; set; }

        [StringLength(64)]
        public string InvoiceToState { get; set; }

        [StringLength(64)]
        public string InvoiceToCountry { get; set; }

        [StringLength(16)]
        public string InvoiceToPostalCode { get; set; }

        [StringLength(16)]
        public string ReferenceId { get; set; }

        public float Total { get; set; }

        [DefaultValue(0)]
        public float AdditionalCost { get; set; }

        [DefaultValue(0)]
        public float Deductions { get; set; }

        public float NetAmount { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime InvoiceDate { get; set; }

        [StringLength(256)]
        public string InvoiceSubject { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        [StringLength(16)]
        public string Status { get; set; }

        [DefaultValue(false)]
        public bool Acknowledge { get; set; }

        [StringLength(512)]
        public string EntryDescription { get; set; }

        [StringLength(512)]
        public string Remarks { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(64)]
        public string CompanyName { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(64)]
        public string AddressLine1 { get; set; }

        [StringLength(64)]
        public string AddressLine2 { get; set; }

        [StringLength(64)]
        public string City { get; set; }

        [StringLength(64)]
        public string State { get; set; }

        [StringLength(64)]
        public string Country { get; set; }

        [StringLength(16)]
        public string PostalCode { get; set; }

        [StringLength(64)]
        public string PayeeBankName { get; set; }

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
        [RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        public string ContactNumber { get; set; }

        [StringLength(64)]
        public string BankName { get; set; }

        [StringLength(64)]
        public string ReferenceNumber { get; set; }

        public Int16 PaymentModeId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PaymentDate { get; set; }

        [StringLength(16)]
        public string BankCode { get; set; }

        public string PGComment { get; set; }

        [StringLength(512)]
        public string PayerRemarks { get; set; }

        [StringLength(128)]
        public string ApprovedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public string InvoiceByName { get; set; }

        [ForeignKey("SubscriberId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

    }

    public class GetCountEntryView
    {
        public int TOTALJOBS { get; set; }

        public int TOTALINVOICE { get; set; }

        public int TOTALTASK { get; set; }

        public int TOTALSTUDENT { get; set; }

        public int TOTALCLINT { get; set; }

        public int TOTALEMPLOYEE { get; set; }

        public int TOTALVENDOR { get; set; }

        public int TOTALTRAINING { get; set; }

        public int TOTALCOURSE { get; set; }


    }

    public class GetJoborderStatusCountView
    {
        public int TotalAssigned { get; set; }

        public int TotalUnAssigned { get; set; }

        public int TotalCompleted { get; set; }

        public int TotalRejected { get; set; }
    }

    public class GetInvoiceStatusCountView
    {
        public int TotalPaid { get; set; }

        public int TotalUnPaid { get; set; }

        public int TotalSubmitted { get; set; }

        public int TotalAccepted { get; set; }

        public int TotalRejected { get; set; }

        public int TotalOnHold { get; set; }
    }

    public class GetTaskCountView
    {
        public int TotalNotApproved { get; set; }

        public int TotalNew { get; set; }

        public int TotalAssigned { get; set; }

        public int TotalInProgress { get; set; }

        public int TotalRejected { get; set; }

        public int TotalCompleted { get; set; }

        public int TotalAbonded { get; set; }
    }

    public class GetTrainingStatusCountView
    {
        public int TotalAssigned { get; set; }

        public int TotalInProgress { get; set; }

        public int TotalCompleted { get; set; }

        public int TotalCancelled { get; set; }

        public int TotalRejected { get; set; }
    }

    public partial class AddressViewModel
    {
        [StringLength(128)]
        public string CorporateId { get; set; }

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

        [StringLength(32)]
        public string FaxNo { get; set; }

        [StringLength(64)]
        public string Country { get; set; }

        [StringLength(64)]
        public string State { get; set; }

        [StringLength(64)]
        public string City { get; set; }

    }

    public class GetUserTaskInvoiceView
    {
        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        public int TotalTasks { get; set; }

        public int TotalInvoices { get; set; }

        public int TrainingScheduled { get; set; }

    }

    public class EngagementTypeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 EngagementTypeId { get; set; }

        [StringLength(64)]
        public string EngagementType { get; set; }

        [StringLength(8)]
        public string ShortName { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        public Int16 SchemeId { get; set; }

        //[StringLength(64)]
        //public string LeaveTypeName { get; set; }

        [StringLength(2)]
        public string LeaveTypeId { get; set; }

        [StringLength(2)]
        public string LeaveTypeCategory { get; set; }

        [DefaultValue(0)]
        public float LeaveLimit { get; set; }

        public DateTime EffectiveFrom { get; set; }

        //new field added date:21/11/2017 by vikas pandey
        [StringLength(1)]
        public string YearEndAction { get; set; }

        [DefaultValue(0)]
        public float MaxLimit { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

    }
    //engagementview modal created by vikas pandey 20/11/2017
    public class EngagementTypeMasterView
    {

        public Int64 EngagementTypeId { get; set; }


        public string EngagementType { get; set; }


        public string ShortName { get; set; }


        public string CorporateId { get; set; }

        public Int16 SchemeId { get; set; }

        //[StringLength(64)]
        //public string LeaveTypeName { get; set; }


        public string LeaveTypeId { get; set; }

        public string LeaveTypeCategory { get; set; }

        [DefaultValue(0)]
        public float LeaveLimit { get; set; }

        public DateTime EffectiveFrom { get; set; }

        [DefaultValue(0)]
        public float CountLeave { get; set; }

        //new field added date:21/11/2017 by vikas pandey

        public string YearEndAction { get; set; }

        [DefaultValue(0)]
        public float MaxLimit { get; set; }

        public string LeaveCategory { get; set; }


    }


    public class JobOrderProgressReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 JoReportId { get; set; }

        [StringLength(16)]
        public string JobOrderNumber { get; set; }

        [StringLength(128)]
        public string ReportedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReportedOn { get; set; }

        [StringLength(1024)]
        public string Comment { get; set; }

        [ForeignKey("JobOrderNumber")]
        public virtual JobOrder JobOrder { get; set; }

    }

    public class TaskProgressReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TaskReportId { get; set; }

        [StringLength(32)]
        public string TaskId { get; set; }

        [StringLength(128)]
        public string ReportedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReportedOn { get; set; }

        [StringLength(1024)]
        public string Comment { get; set; }

        [ForeignKey("TaskId")]
        public virtual TaskMaster TaskMaster { get; set; }

    }

    public partial class InVoiceAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(16)]
        public string ReferenceId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }
    }

    public class Category
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 CategoryId { get; set; }

        [StringLength(128)]
        public string CategoryName { get; set; }
    }

    public partial class AnalyticsViewModel
    {
        public Int64 BatchId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public double Total { get; set; }
    }
}