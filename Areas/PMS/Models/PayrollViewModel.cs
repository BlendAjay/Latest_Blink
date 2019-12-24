
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AJSolutions.Areas.PMS.Models
{
    public class EmployeeMonthlySalary
    {
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(32)]
        public string EmployeeId { get; set; }

        [StringLength(3)]
        public string DepartmentId { get; set; }

        [StringLength(64)]
        public string Department { get; set; }
       
        public Int64 DesignationId { get; set; }

        [StringLength(64)]
        public string DesignationName { get; set; }

        public DateTime? JoiningDate { get; set; }

        public Int64 EmployeeMonthlySalaryPayoutId { get; set; }

        public Int64 ESID { get; set; }

        public float GrossCTC { get; set; }

        public float NetCTC { get; set; }

        public int WorkingDays { get; set; }

        public int LWP { get; set; }

        public int TotalLeaves { get; set; }

        public Int16 PayoutMonth { get; set; }

        public int PayoutYear { get; set; }

        public bool Freeze { get; set; }

    }

    public class EmployeeMonthlySalaryDetail
    {
        [StringLength(128)]
        public string UserId { get; set; }

        public Int64 EmployeeMonthlySalaryPayoutId { get; set; }

        public Int64 CorporatePayrollHeadId { get; set; }

        [StringLength(64)]
        public String  PayrollHeadName { get; set; }

        [StringLength(16)]
        public String  PayrollCategory { get; set; }

        public Int64 EmployeeMonthlySalaryHeadId { get; set; }

        public float Amount { get; set; }

    }

    public class EmployeeSalaryProcessedDetail
    {
      
        public DateTime ProcessedDate { get; set; }

        public int NoOfEmployee { get; set; }

        
    }

   
}