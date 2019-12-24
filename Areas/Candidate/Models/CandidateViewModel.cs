using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AJSolutions.Areas.Candidate.Models
{
    public class CandidateBasicDetails
    {
        [Key]
        [StringLength(128)]
        public string CandidateId { get; set; }

        [Required]
        [StringLength(64)]
        public string FirstName { get; set; }

        [StringLength(64)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(64)]
        public string LastName { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required]
        public bool Gender { get; set; }

        [StringLength(32)]
        public string MaritalStatus { get; set; }

        [StringLength(32)]
        public string Nationality { get; set; }

        [StringLength(32)]
        public string AlternateEmail { get; set; }

        [StringLength(32)]
        public string AlternateContact { get; set; }

        [StringLength(16)]
        public string RegistrationId { get; set; }
    }

    public partial class CorporateCandidateViewModel
    {
        public string UserId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        [StringLength(16)]
        public string PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; set; }

        [StringLength(256)]
        public string UserName { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(16)]
        public string RegistrationId { get; set; }

        [StringLength(128)]
        public string CorporateName { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

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

    }

    public partial class CandidateViewModel
    {
        public string UserId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        public string OtherId { get; set; }

        [StringLength(16)]
        public string PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; set; }

        public DateTime? LastLogin { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        [StringLength(32)]
        public string CourseName { get; set; }

        public Int64 BatchId { get; set; }

        public double TotalFeeAmount { get; set; }

        [StringLength(128)]
        public string BatchName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CourseStartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CourseEndDate { get; set; }

        public double CourseFee { get; set; }

        public double RemainingAmount { get; set; }

        public Int16 InstallmentId { get; set; }

        [StringLength(128)]
        public string Installment { get; set; }

        public double PaidAmount { get; set; }

        public string CheckInDate { get; set; }

        public string CheckOutDate { get; set; }

        public string TotalDays { get; set; }

        [StringLength(128)]
        public string RegisterBy { get; set; }

        [StringLength(256)]
        public string UserName { get; set; }

        [StringLength(16)]
        public string PCode { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(16)]
        public string RegistrationId { get; set; }

        [DefaultValue(false)]
        public bool Deactivated { get; set; }

        public bool AccomondationNeeded { get; set; }

        public int TrainingAvailable { get; set; }

        [StringLength(128)]
        public string CorporateName { get; set; }

        public string LastInstallmentStatus { get; set; }

        [StringLength(12)]
        public string ReferenceId { get; set; }

        public double TotalCourseFees { get; set; }

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

        public string Designation { get; set; }

        public string Region { get; set; }
    }

    public partial class CandidateCourseViewModel
    {
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        [StringLength(16)]
        public string PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; set; }

        public DateTime? LastLogin { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        [StringLength(32)]
        public string CourseName { get; set; }

        public Int64 BatchId { get; set; }

        [StringLength(128)]
        public string BatchName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CourseStartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CourseEndDate { get; set; }

        public double CourseFee { get; set; }

        public Int16 InstallmentId { get; set; }

        [StringLength(128)]
        public string Installment { get; set; }

    }

    public partial class CandidateTransactionViewModel
    {

        [StringLength(128)]
        public string UserId { get; set; }

        public string UserName { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        [StringLength(16)]
        public string PhoneNumber { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(32)]
        public string TransactionId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        [StringLength(32)]
        public string CourseName { get; set; }

        public double CourseFee { get; set; }

        public Int64 BatchId { get; set; }

        [StringLength(128)]
        public string BatchName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ToDate { get; set; }

        public Int16 PaymentModeId { get; set; }

        [StringLength(128)]
        public string PaymentMode { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? TransactionDate { get; set; }

        [StringLength(64)]
        public string BankName { get; set; }

        [StringLength(64)]
        public string ReferenceNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PaymentDate { get; set; }

        [StringLength(256)]
        public string Remarks { get; set; }

        public float FeePaid { get; set; }

        [StringLength(32)]
        public string Status { get; set; }
    }

    public class JSBasicDetailViewModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string JEId { get; set; }

        public string ReferenceId { get; set; }
    }



}