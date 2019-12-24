using AJSolutions.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace AJSolutions.Areas.CMS.Models
{
    public class CorporateViewModel
    {
        [Key]
        [StringLength(128)]
        public string CorporateId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [StringLength(64)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(16)]
        [RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        public string PhoneNumber { get; set; }

        //[RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
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

        [StringLength(64)]
        public string Department { get; set; }

        [StringLength(128)]
        public string RoleId { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        [DefaultValue(false)]
        public bool Deactivated { get; set; }

        public virtual ICollection<AdminLogoFile> AdminLogoFile { get; set; }
    }

    public partial class AdminLogoFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class JobOrderViewModel
    {
        [StringLength(16)]
        public string JobOrderNumber { get; set; }

        [StringLength(128)]
        public string ClientId { get; set; }

        [StringLength(512)]
        public string Subject { get; set; }

        [StringLength(128)]
        public string FunctionalPosition { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        [StringLength(512)]
        public string Conditions { get; set; }

        public DateTime JOPostedOn { get; set; }

        public DateTime? StartDate { get; set; }

        public int Duration { get; set; }

        [StringLength(128)]
        public string SalaryRange { get; set; }

        [StringLength(128)]
        public string ExpRange { get; set; }

        [StringLength(128)]
        public string Industry { get; set; }

        [DefaultValue(false)]
        public bool Feedback { get; set; }

        [DefaultValue(false)]
        public bool Attendance { get; set; }

        [DefaultValue(false)]
        public bool Accomodation { get; set; }

        public int JobOrderTypeId { get; set; }

        [StringLength(128)]
        public string JobOrderType { get; set; }

        [StringLength(16)]
        public string JobOrderStatus { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        public float TotalCost { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string ClientName { get; set; }

        [StringLength(128)]
        public string SubscriberName { get; set; }

        public string TotalInv { get; set; }

        public string TotalTask { get; set; }

        public string InvNo { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }
    }

    public class CorporateBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 BranchId { get; set; }

        [StringLength(256)]
        public string BranchName { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }
    }

}