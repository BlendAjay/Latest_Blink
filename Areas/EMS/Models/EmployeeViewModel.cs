using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AJSolutions.Areas.EMS.Models
{
    public class EmployeeViewModel
    {      
        [StringLength(128)]
        public string UserId { get; set; }
       
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
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

        [StringLength(64)]
        public string DesignationName { get; set; }

        [StringLength(32)]
        public string EmployeeId { get; set; }

        [DefaultValue(false)]
        public bool Emplanelled { get; set; }

        [StringLength(3)]
        public string DepartmentId { get; set; }

        [StringLength(64)]
        public string Department { get; set; }

        [DefaultValue(false)]
        public bool ManagerLevel { get; set; }

        [StringLength(128)]
        public string ReportingAuthority { get; set; }
        [StringLength(128)]
        public string UName { get; set; }

        [StringLength(128)]
        public string RegisterBy { get; set; }

        [StringLength(256)]
        public string UserName { get; set; }

        //Changes by Achal Jha 13-05-2017
        [StringLength(64)]
        public string Status { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateofJoining { get; set; }

        public int ProbationPeriod { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateofConfirmation { get; set; }

        [StringLength(64)]
        public string FatherName { get; set; }

        [StringLength(64)]
        public string SpouseName { get; set; }

        public Int64 GradeId { get; set; }

        public Int16 StatusId { get; set; }

        public Int16 SchemeId { get; set; }

        [DefaultValue(false)]
        public bool Deactivated { get; set; }

        ///vikash das-----///
        public Int64? FileId { get; set; }

        ////-------------------------////
    }
}