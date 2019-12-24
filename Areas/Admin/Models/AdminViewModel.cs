using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using AJSolutions.Models;
using System.ComponentModel;


namespace AJSolutions.Areas.Admin.Models
{
    public partial class UserRegistrationViewModel
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        [Display(Name = "Mobile Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Department")]
        public string DepartmentId { get; set; }

        [DefaultValue(false)]
        public bool ManagerLevel { get; set; }

        [StringLength(128)]
        public string ReportingAuthority { get; set; }

        [StringLength(16)]
        public string RegistrationId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        [StringLength(32)]
        public string CourseName { get; set; }

        public Int64 BatchId { get; set; }

        [StringLength(128)]
        public string BatchName { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(16)]
        [RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        public string AlternateContact { get; set; }

        [StringLength(64)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string AlternateEmail { get; set; }

        [StringLength(32)]
        public string Nationality { get; set; }

        [StringLength(64)]
        public string CompanyName { get; set; }

        [StringLength(32)]
        public string CompanyType { get; set; }

        [StringLength(32)]
        public string CompanySize { get; set; }

        [StringLength(64)]
        [RegularExpression(@"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?)", ErrorMessage = "Invalid Website name")]
        public string Website { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public string UpdatedB { get; set; }

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

        [StringLength(2)]
        public string Gender { get; set; }

        [StringLength(16)]
        public string TrackerId { get; set; }

        [StringLength(16)]
        public string FacilityId { get; set; }

        [StringLength(16)]
        public string Accesspoint { get; set; }

        /// <summary>
        /// Update by : Achal Kumar Jha
        /// Updated on: 01/06/2017
        /// Reason    : Changes For Payroll
        /// </summary>

        [StringLength(64)]
        public string Status { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateofJoining { get; set; }

        public int ProbationPeriod { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateofConfirmation { get; set; }


        [DefaultValue(0)]
        public Int64 GradeId { get; set; }
    }

    public class UserPrimaryDetailViewModel
    {

        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^([0-9]+-)*[0-9]+$", ErrorMessage = "Invalid Mobile Number")]
        [Display(Name = "Mobile Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(128)]
        public string RoleId { get; set; }

        [Required]
        [StringLength(8)]
        public string Role { get; set; }

    }


    public class AddUserRoleViewModel
    {
        [Key]
        [StringLength(128)]
        public string RoleId { get; set; }

        [Required]
        [StringLength(256)]
        public string Role { get; set; }
    }

    public class ProgrammAccessViewModel
    {
        [Key]
        public Int64 ProgrammAccessId { get; set; }

        [StringLength(16)]
        public string ProgrammeId { get; set; }

        [StringLength(64)]
        public string ProgrammeName { get; set; }

        public bool ReadOnly { get; set; }

        public bool ReadWrite { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }


    }


    public class ModuleRolesViewModel
    {
        [Key]
        [StringLength(3)]
        public string ModuleId { get; set; }

        [StringLength(128)]
        public string Module { get; set; }

        [StringLength(128)]
        public string RoleId { get; set; }

        [StringLength(256)]
        public string Role { get; set; }

    }

    public partial class DepartmentModuleViewModel
    {
        [StringLength(3)]
        public string DepartmentId { get; set; }

        [StringLength(64)]
        public string Department { get; set; }

        [StringLength(128)]
        public string RoleId { get; set; }

        public Boolean IsVisible { get; set; }

        [StringLength(3)]
        public string ModuleId { get; set; }

        [StringLength(192)]
        public string RoleDepartment { get; set; }
    }

    public class MeetingMinutes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 MeetingId { get; set; }

        [StringLength(128)]
        public string MeetingSubject { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? MeetingDate { get; set; }

        public string MeetingRemarks { get; set; }

        [StringLength(128)]
        public string MeetingHost { get; set; }

        [StringLength(128)]
        public string Location { get; set; }

        public string Participants { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(1024)]
        public string InternalRemarks { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }
    }

    public class MeetingMinutesView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 MeetingId { get; set; }

        [StringLength(128)]
        public string MeetingSubject { get; set; }

        [StringLength(128)]
        public string Location { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? MeetingDate { get; set; }

        public string MeetingRemarks { get; set; }

        [StringLength(128)]
        public string MeetingHost { get; set; }

        public string Participants { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(1024)]
        public string InternalRemarks { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        public int ParticipantsCount { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedByName { get; set; }

        [StringLength(128)]
        public string UpdatedBy { get; set; }
    }

    public class GetAllTypeUserCount
    {
        public int TOTALADMIN { get; set; }

        public int TOTALCLIENT { get; set; }

        public int TOTALEMPLOYEE { get; set; }

        public int TOTALVENDOR { get; set; }

        public int TOTALSTUDENT { get; set; }


    }


    public class QualificationMaster
    {
        [Key]
        [StringLength(128)]
        public string Qualification { get; set; }
    }

    public class OrganizationMaster
    {
        [Key]
        [StringLength(128)]
        public string Organization { get; set; }
    }

    public class DomainMaster
    {
        [Key]
        [StringLength(128)]
        public string Domain { get; set; }
    }

    public class SpecializationMaster
    {
        [Key]
        [StringLength(128)]
        public string Specialization { get; set; }
    }

    public class ProjectMaster
    {
        [Key]
        [StringLength(128)]
        public string Project { get; set; }
    }
    //by vikas pandey
    //created on 03 -11-2018
    public class CheckinCheckoutdataAppView
    {
        public string UserId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

    }
}