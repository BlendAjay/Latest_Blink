using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AJSolutions.Models
{
    public class UserViewModel
    {
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        public Boolean EmailConfirmed { get; set; }

        public Boolean ManagerLevel { get; set; }

        [StringLength(16)]
        public string PhoneNumber { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string Role { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string SubscriberName { get; set; }

        [StringLength(128)]
        public string ReportingAuthority { get; set; }

        [StringLength(128)]
        public string ReportingAuthorityname { get; set; }

        [StringLength(128)]
        public string DepartmentId { get; set; }

        [StringLength(128)]
        public string Department { get; set; }

        [StringLength(32)]
        public string Nationality { get; set; }

        [StringLength(128)]
        public string AlternateEmail { get; set; }

        [StringLength(16)]
        public string AlternateContact { get; set; }

        [DefaultValue(false)]
        public bool IsProfileCompleted { get; set; }

        [DefaultValue(false)]
        public bool IsCompanyProfileCompleted { get; set; }

        [DefaultValue(false)]
        public bool IsAddressCompleted { get; set; }

        [DefaultValue(false)]
        public bool IsBankDetailCompleted { get; set; }

        [DefaultValue(false)]
        public bool IsEducationCompleted { get; set; }

        [DefaultValue(false)]
        public bool IsExperienceCompleted { get; set; }

        [DefaultValue(false)]
        public bool IsSocialDetailCompleted { get; set; }

        [DefaultValue(false)]
        public bool IsIdentificationCompleted { get; set; }

        [DefaultValue(false)]
        public bool IsSkillCompleted { get; set; }

        public bool CompanyAttendance { get; set; }

        [StringLength(128)]
        public string ReferenceId { get; set; }

        public string UserName { get; set; }
    }
}