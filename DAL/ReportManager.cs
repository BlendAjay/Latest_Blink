using AJSolutions.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.SqlServer;
using System.Data.OleDb;

namespace AJSolutions.DAL
{
    public class ReportManager
    {
        public List<CandidateAttendanceView> GetStudentAttendence(string batchId, DateTime currentDate)
        {
            var candidatelst = new List<CandidateAttendanceView>();
            using (var db = new UserDBContext())
            {
                //candidatelst = db.Database
                //          .SqlQuery<CandidateAttendanceView>("Select getdate() as TodayDate,CM.CourseCode,CM.CourseName,CB.BatchId,CB.BatchName,CA.UserID,Up.Name,'' as Attendence,"
                //                                         + "'' as Remarks,CA.AttendenceId,CA.TrainingId,CA.AttendenceDate,'' as IsPresent,CA.Remarks "
                //                                         +" from CandidateAttendances CA inner join trainingSchedules TS on CA.TrainingId=TS.TrainingId inner join "
                //                                         +" coursebatches CB on TS.BatchId=CB.BatchId  inner join UserProfiles UP on CA.UserID=UP.UserID inner join CourseMasters "
                //                                         + " CM on CB.CourseCode=CM.CourseCode where CB.BatchID=@batchId",
                //          new SqlParameter("@batchId", string.IsNullOrEmpty(batchId) ? DBNull.Value : (object)batchId)
                //          ).ToList();
                candidatelst = db.Database
                         .SqlQuery<CandidateAttendanceView>("SELECT "
                         + "     cast('" + currentDate + "' as DateTime)  as AttendenceDate,"
                         + "	UP.UserId "
                         + ",	UP.Name "
                         + ",	U.Email "
                         + ",	U.PhoneNumber "
                        + ",	UH.RegisteredOn "
                        + ",	UH.LastLogin "
                        + ",	UP.SubscriberId "
                        + ",	CM.CourseCode "
                        + ",	CM.CourseName "
                        + ",	ISNULL(CB.BatchId, 0) AS BatchId "
                        + ",	CB.BatchName "
                        + ",    CB.AttendenceNeeded "
                        + ",     TS.TrainingID"
                        + ",	CB.FromDate AS CourseStartDate "
                        + ",	CB.ToDate AS CourseEndDate "
                        + ",	ISNULL(CM.CourseFee,0) AS CourseFee "
                        + ",	ISNULL(IM.InstallmentId,0) AS InstallmentId "
                        + ",	IM.Installment "
                        + ", 	(SELECT ISNULL(SUM(FeePaid),0) FROM FEEDETAILS	WHERE UserId = UP.UserId AND CourseCode = CB.CourseCode) AS PaidAmount "
                        + ", 	ISNULL(CAST(CIO.CheckInDate AS NVARCHAR),'') AS  CheckInDate "
                        + ", 	ISNULL(CAST(CIO.CheckOutDate AS NVARCHAR),'') AS  CheckOutDate "
                        + ",	ISNULL(CAST(DATEDIFF(d, CheckInDate, CheckOutDate) AS NVARCHAR),'')AS TotalDays  "
                        + " FROM "
                        + " USERPROFILES UP "
                        + " JOIN	ASPNETUSERS	U "
                        + "	ON	UP.UserId	=	U.Id "
                        + " LEFT OUTER	JOIN	USERHISTORIES UH "
                        + " 	ON	UP.UserId	= 	UH.UserId "
                        + " LEFT OUTER JOIN		CandidateCourseDetails CCD "
                        + " 	ON	UP.UserId	= CCD.UserId "
                        + " LEFT OUTER JOIN		CourseBatches CB "
                        + "	ON	CCD.BatchId	= CB.BatchId "
                        + " LEFT OUTER JOIN		CourseMasters CM "
                        + "	ON	CB.CourseCode	= CM.CourseCode "
                        + " LEFT OUTER JOIN		InstallmentMasters IM "
                        + "	ON	CCD.InstallmentId	=	IM.InstallmentId "
                        + " LEFT OUTER JOIN		CheckInCheckOuts CIO "
                        + "	ON	UP.UserId	=	CIO.UserId "
                        + " LEFT OUTER JOIN TrainingSchedules TS "
                        + " ON CB.BatchID=TS.BATCHID where CB.batchid=@batchId",
                         new SqlParameter("@batchId", string.IsNullOrEmpty(batchId) ? DBNull.Value : (object)batchId)
                         ).ToList();
            }
            return candidatelst;
        }
        public bool InsertStudentAttendence(string TrainingId, string UserID, DateTime AttendenceDate, bool IsPresent, string Remarks)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var trainingId = new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId);
                    var userID = new SqlParameter("@UserID", string.IsNullOrEmpty(UserID) ? DBNull.Value : (object)UserID);
                    var attendenceDate = new SqlParameter("@attendenceDate", AttendenceDate);
                    var isPresent = new SqlParameter("@IsPresent", IsPresent);
                    var remarks = new SqlParameter("@Remarks", string.IsNullOrEmpty(Remarks) ? DBNull.Value : (object)Remarks);
                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateAttendances @TrainingId,@UserId,@AttendenceDate,@IsPresent,@Remarks ", trainingId, userID, attendenceDate, isPresent, remarks);
                    if (i == 1)
                        res = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }
        public bool IsStudentAttendenceExists(string TrainingId, string UserID, DateTime AttendenceDate)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var totalCount = context.CandidateAttendance.Where(c => c.TrainingId == TrainingId && c.UserId == UserID && c.AttendenceDate == AttendenceDate).Count();
                    if (Convert.ToInt32(totalCount) > 0)
                        res = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }
        //public bool IsUserExists(string MobileNo, string emailAddress)
        //{
        //    bool res = false;
        //    try
        //    {
        //        using (var context = new UserDBContext())
        //        {
        //            var totalCount = context..Where(c=> c.).UserHistory Where(c => c. ).CandidateAttendance.Where(c => c.TrainingId == TrainingId && c.UserId == UserID && c.AttendenceDate == AttendenceDate).Count();
        //            if (Convert.ToInt32(totalCount) > 0)
        //                res = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return res;
        //}
        public bool InsertBulkUser(string ID, string SubscriberID, string PhoneNumber, string Email, string Role, string Name, string DepartmentName)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var id = new SqlParameter("@ID", string.IsNullOrEmpty(ID) ? DBNull.Value : (object)ID);
                    var subscriberId = new SqlParameter("@SubscriberID", string.IsNullOrEmpty(SubscriberID) ? DBNull.Value : (object)SubscriberID);
                    var phoneNumber = new SqlParameter("@PhoneNumber", string.IsNullOrEmpty(PhoneNumber) ? DBNull.Value : (object)PhoneNumber);
                    var email = new SqlParameter("@Email", string.IsNullOrEmpty(Email) ? DBNull.Value : (object)Email);
                    var role = new SqlParameter("@Role", string.IsNullOrEmpty(Role) ? DBNull.Value : (object)Role);
                    var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
                    var departmentname = new SqlParameter("@DepartmentName", string.IsNullOrEmpty(DepartmentName) ? DBNull.Value : (object)DepartmentName);
                    int i = context.Database.ExecuteSqlCommand("USP_BulkUserRegister @ID,@SubscriberID,@PhoneNumber,@Email,@Role,@Name,@DepartmentName", id, subscriberId, phoneNumber, email, role, name, departmentname);
                    if (i != 0)
                        res = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }
        public List<ClientJobOrderAdmin> GetJobOrders(DateTime? StartDate, DateTime? EndDate, string ClientId = null, string JobOrderStatus = null, int JobOrderType = 0, int Duration = 0, string JOAction = null, string SubscriberID = null)
        {
            var JobOrder = new List<ClientJobOrderAdmin>();

            using (var db = new UserDBContext())
            {

                var startDate = new SqlParameter("@StartDate", string.IsNullOrEmpty(StartDate.ToString()) ? DBNull.Value : (object)StartDate);
                var endDate = new SqlParameter("@EndDate", string.IsNullOrEmpty(EndDate.ToString()) ? DBNull.Value : (object)EndDate);
                var clientId = new SqlParameter("@ClientId", string.IsNullOrEmpty(ClientId) ? DBNull.Value : (object)ClientId);
                var jobOrderStatus = new SqlParameter("@JobOrderStatus", JobOrderStatus);
                var jobOrderType = new SqlParameter("@JobOrderType", JobOrderType);
                var duration = new SqlParameter("@Duration", Duration);
                var jOAction = new SqlParameter("@JOAction", string.IsNullOrEmpty(JOAction) ? DBNull.Value : (object)JOAction);
                var subscriberID = new SqlParameter("@SubscriberID", string.IsNullOrEmpty(SubscriberID) ? DBNull.Value : (object)SubscriberID);
                JobOrder = db.Database.SqlQuery<ClientJobOrderAdmin>("EXEC USP_GetClientJobOrder @StartDate,@EndDate,@ClientId, @JobOrderStatus,@JobOrderType,@Duration,@JOAction,@SubscriberID", startDate, endDate, clientId, jobOrderStatus, jobOrderType, duration, jOAction, subscriberID).ToList();

            }
            return JobOrder;
        }

        /// <summary>
        /// Function to get course wise summary like no of training schedule, # of candidate, status of training etc
        /// </summary>
        /// <param name="CourseId"></param>
        /// <param name="CorporateId"></param>
        /// <returns></returns>
        public List<CourseSummaryViewModel> GetCourseSummary(string SubscriberId, string CourseId, string CorporateId)
        {

            var CourseDetail = new List<CourseSummaryViewModel>();

            using (var db = new UserDBContext())
            {
                var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);

                var courseId = new SqlParameter("@CourseId", string.IsNullOrEmpty(CourseId) ? DBNull.Value : (object)CourseId);

                var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);

                CourseDetail = db.Database.SqlQuery<CourseSummaryViewModel>("EXEC USP_CourseWiseSummary @SubscriberId, @CourseId, @CorporateId", subscriberId, courseId, corporateId).ToList();

            }
            return CourseDetail;

        }
    }

}