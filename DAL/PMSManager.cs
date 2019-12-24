using AJSolutions.Areas.PMS.Models;
using AJSolutions.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AJSolutions.DAL
{
    public class PMSManager
    {

        UserDBContext db = new UserDBContext();
        public bool ProcessSalary(string SubscriberId, string DepartmentId, string UserId, Int16 PayoutMonth, string UpdatedBy, Int32 PayoutYear)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                    var departmentId = new SqlParameter("@DepartmentId", string.IsNullOrEmpty(DepartmentId) ? DBNull.Value : (object)DepartmentId);
                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var payoutMonth = new SqlParameter("@PayoutMonth", PayoutMonth);
                    var payoutYear = new SqlParameter("@PayoutYear", PayoutYear);
                    int i = context.Database.ExecuteSqlCommand("USP_ProcessEmployeeSalary @SubscriberId,  @DepartmentId,@UserId,@UpdatedBy, @PayoutMonth, @PayoutYear"
                        , subscriberId, departmentId, userid, updatedBy, payoutMonth, payoutYear);
                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return res;
        }

        public bool FreezeSalary(string SubscriberId, string DepartmentId, string UserId, Int16 PayoutMonth, string UpdatedBy, Int32 PayoutYear)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                    var departmentId = new SqlParameter("@DepartmentId", string.IsNullOrEmpty(DepartmentId) ? DBNull.Value : (object)DepartmentId);
                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var payoutMonth = new SqlParameter("@PayoutMonth", PayoutMonth);
                    var payoutYear = new SqlParameter("@PayoutYear", PayoutYear);
                    int i = context.Database.ExecuteSqlCommand("USP_FreezeProceessedSalary @SubscriberId,  @DepartmentId,@UserId,@UpdatedBy, @PayoutMonth, @PayoutYear"
                        , subscriberId, departmentId, userid, updatedBy, payoutMonth, payoutYear);
                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return res;
        }

        public List<EmployeeSalaryProcessedDetail> ProcessedSalaryStatement(Int16 PayoutMonth, Int32 PayoutYear)
        {
            var empSalary = new List<EmployeeSalaryProcessedDetail>();


            using (var context = new UserDBContext())
            {
                var payoutMonth = new SqlParameter("@PayoutMonth", PayoutMonth);
                var payoutYear = new SqlParameter("@PayoutYear", PayoutYear);
                empSalary = db.Database.SqlQuery<EmployeeSalaryProcessedDetail>("EXEC USP_GetMonthlySalaryStatement @PayoutMonth, @PayoutYear "
                    , payoutMonth, payoutYear).ToList();

            }


            return empSalary;
        }

        public List<EmployeeMonthlySalary> GetEmployeeMonthlySalary(string SubscriberId, string DepartmentId, string UserId, Int16 PayoutMonth, Int32 PayoutYear)
        {

            var empSalary = new List<EmployeeMonthlySalary>();

            using (var db = new UserDBContext())
            {
                empSalary = db.Database.SqlQuery<EmployeeMonthlySalary>("EXEC USP_GETEMPLOYEEMONTHLYSALARY @SubscriberId , @PayoutMonth, @PayoutYear , @DepartmentId , @EmployeeId"
                                                                         , new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)
                                                                         , new SqlParameter("@PayoutMonth", PayoutMonth)
                                                                         , new SqlParameter("@PayoutYear", PayoutYear)
                                                                         , new SqlParameter("@DepartmentId", string.IsNullOrEmpty(DepartmentId) ? DBNull.Value : (object)DepartmentId)
                                                                         , new SqlParameter("@EmployeeId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                                                                         ).ToList();
            }

            return empSalary;
        }

        public EmployeeMonthlySalary GetEmployeeMonthlySalary(Int64 PayoutId)
        {

            var empSalary = new EmployeeMonthlySalary();

            using (var db = new UserDBContext())
            {
                empSalary = db.Database.SqlQuery<EmployeeMonthlySalary>("EXEC USP_GetIndividualEmployeeMonthlySalary  @PayoutId"
                                                                         , new SqlParameter("@PayoutId", PayoutId)
                                                                         ).FirstOrDefault();
            }

            return empSalary;
        }

        public List<EmployeeMonthlySalaryDetail> GetEmployeeMonthlySalaryDetails(string SubscriberId, string DepartmentId, string UserId, Int16 PayoutMonth, Int32 PayoutYear)
        {

            var empSalaryDetails = new List<EmployeeMonthlySalaryDetail>();

            using (var db = new UserDBContext())
            {
                empSalaryDetails = db.Database.SqlQuery<EmployeeMonthlySalaryDetail>("EXEC USP_GetEmployeeMonthlySalaryDetails @SubscriberId , @PayoutMonth, @PayoutYear , @DepartmentId , @EmployeeId"
                                                                         , new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)
                                                                         , new SqlParameter("@PayoutMonth", PayoutMonth)
                                                                         , new SqlParameter("@PayoutYear", PayoutYear)
                                                                         , new SqlParameter("@DepartmentId", string.IsNullOrEmpty(DepartmentId) ? DBNull.Value : (object)DepartmentId)
                                                                         , new SqlParameter("@EmployeeId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                                                                         ).ToList();
            }

            return empSalaryDetails;
        }

        public void UpdateSalaryHead(Int64 headId, float amount)
        {
            EmployeeMonthlySalaryHeads emp = db.EmployeeMonthlySalaryHeads.Find(headId);

            if (emp != null)
            {
                emp.Amount = amount;
                db.Entry(emp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void UpdateSalary(Int64 PayoutId, Int16 LWP, float NetSalary)
        {
            EmployeeMonthlySalaryPayout payout = db.EmployeeMonthlySalaryPayout.Find(PayoutId);

            if (payout != null)
            {
                payout.LWP = LWP;
                payout.NetCTC = NetSalary;
                db.Entry(payout).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        //method for add User Androied Device Information
        //Created By: Vikas Pandey
        //Created On : 18/08/2018
        public bool AddDeviceInfromation(string UserId, string AndroidId, string DeviceName)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var androidId = new SqlParameter("@AndroidId", string.IsNullOrEmpty(AndroidId) ? DBNull.Value : (object)AndroidId);
                    var deviceName = new SqlParameter("@DeviceName", string.IsNullOrEmpty(AndroidId) ? DBNull.Value : (object)DeviceName);
                    int i = context.Database.ExecuteSqlCommand("USP_UserDeviceInformation @UserId,@AndroidId, @DeviceName"
                        , userid, androidId, deviceName);
                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return res;
        }

    }
}