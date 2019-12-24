
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AJSolutions.Areas.PMS.Models
{
    public class PayrollLeavsSettingViewModel
    {
        [StringLength(128)]
        public string SubscriberId { get; set; }

        public Int16 MaxId { get; set; }

        [StringLength(128)]
        public string LeaveName { get; set; }

        public Int16 LeaveId { get; set; }

        public Int16 NoofDays { get; set; }

        public Int16 SalarycalculationOn { get; set; }

        public Int16 HolidayInSalary { get; set; }
    }


    public class EmployeeLeaveSummariesViewModel
    {
        [StringLength(128)]
        public string UserId { get; set; }

        public Int16 SchemeId { get; set; }

        public Int64 EngagementTypeId { get; set; }
       
        public int LeaveYear { get; set; }

        [StringLength(128)]
        public string  Name { get; set; }

        [StringLength(3)]
        public string DepartmentId { get; set; }

        [StringLength(128)]
        public string Department { get; set; }

        [StringLength(128)]
        public string EngagementType { get; set; }

        public float EngagementCount { get; set; }

        public float LeaveLimit { get; set; }
       
    }


}