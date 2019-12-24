using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AJSolutions.Models;


namespace AJSolutions.DAL
{

    public class HMSManager
    {
        UserDBContext db = new UserDBContext();
        public bool AddCheckIn(string UserId, DateTime? CheckInDate, Int64 BatchId, string UpdatedBy, DateTime UpdatedOn)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var checkindate = new SqlParameter("@CheckInDate", CheckInDate);
                    var batchId = new SqlParameter("@BatchId", BatchId);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    int i = context.Database.ExecuteSqlCommand("USP_AddCheckInCheckOut @UserId,@CheckInDate, @BatchId, @UpdatedBy, @UpdatedOn", userid, checkindate, batchId, updatedBy, updatedOn);
                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return res;
        }
        //For CheckOut
        public bool AddCheckOut(string UserId, DateTime? CheckOutDate, Int64 BatchId, string UpdatedBy, DateTime UpdatedOn)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var checkoutdate = new SqlParameter("@CheckOutDate", CheckOutDate);
                    var batchId = new SqlParameter("@BatchId", BatchId);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateCheckOut @UserId,@CheckOutDate, @BatchId, @UpdatedBy, @UpdatedOn", userid, checkoutdate, batchId, updatedBy, updatedOn);
                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return res;
        }

        public string CInCOutInBulk(Int64 BatchId, string UserId, DateTime CheckInDate, DateTime? CheckOutDate, string UpdatedBy)
        {
            string res = "Failed:";

            try
            {
                using (var context = new UserDBContext())
                {
                    var batchId = new SqlParameter("@BatchId", BatchId);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var checkInDate = new SqlParameter("@CheckInDate", CheckInDate);
                    var checkOutDate = new SqlParameter("@CheckOutDate", CheckOutDate == null ? DBNull.Value : (object)CheckOutDate);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var updatedOn = new SqlParameter("@UpdatedOn", DateTime.UtcNow);
                    int i = context.Database.ExecuteSqlCommand("USP_UpdateCInCOutOfCandidate @BatchId,@UserId,@CheckInDate,@CheckOutDate,@UpdatedBy,@UpdatedOn"
                                                                , batchId, userId, checkInDate, checkOutDate, updatedBy, updatedOn);
                    if (i == 1)
                        res = "Succeed:Data saved successfully";
                }
            }
            catch (Exception ex)
            {
                res = res + ex.ToString();
            }

            return res;
        }

        public bool AddCandidateAttendances(Int64 AttendenceId, string TrainingId, string UserId, DateTime? AttendenceDate, string IsPresent, string Remarks, string Sessions)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var attendenceId = new SqlParameter("@AttendenceId", AttendenceId);
                    var trainingId = new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var attendancedate = new SqlParameter("@AttendenceDate", AttendenceDate ?? (object)DBNull.Value);
                    var ispresent = new SqlParameter("@IsPresent", string.IsNullOrEmpty(IsPresent) ? DBNull.Value : (object)IsPresent);
                    var remarks = new SqlParameter("@Remarks", string.IsNullOrEmpty(Remarks) ? DBNull.Value : (object)Remarks);
                    var sessions = new SqlParameter("@Sessions", string.IsNullOrEmpty(Sessions) ? DBNull.Value : (object)Sessions);
                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateAttendances @AttendenceId, @TrainingId ,@UserId ,@AttendenceDate ,@IsPresent, @Remarks, @Sessions",
                                                                                          attendenceId, trainingId, userId, attendancedate, ispresent, remarks, sessions);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return res;
        }

        public bool AddTrainerComments(string TrainingId, DateTime? CommentDate, string Comment)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var trainingId = new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId);
                    var commentDate = new SqlParameter("@CommentDate", CommentDate);
                    var comment = new SqlParameter("@Comment", Comment);
                    int i = context.Database.ExecuteSqlCommand("USP_AddTrainerComments  @TrainingId ,@CommentDate, @Comment",
                                                                 trainingId, commentDate, comment);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return res;
        }

        //list for candidate attendance

        public List<CandidateAttendance> GetCandidateAttendance(string TrainingId)
        {

            var studentattendance = new List<CandidateAttendance>();
            using (var db = new UserDBContext())
            {
                studentattendance = db.Database.SqlQuery<CandidateAttendance>("EXEC USP_GetCandidateAttendances @TrainingId",

                            new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId)
                            ).ToList();
            }
            return studentattendance;
        }

        public List<CandidateAttendanceView> GetCandidateAttendancelist(string TrainingId)
        {

            var studentattendance = new List<CandidateAttendanceView>();
            using (var db = new UserDBContext())
            {
                studentattendance = db.Database.SqlQuery<CandidateAttendanceView>("EXEC USP_GetCandidateAttendances @TrainingId",

                            new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId)
                            ).ToList();
            }
            return studentattendance;
        }

        //list for comment 

        public List<TrainerComments> GetTrainerComments(string TrainerId)
        {
            var trainercomment = new List<TrainerComments>();
            using (var db = new UserDBContext())
            {
                trainercomment = db.Database.SqlQuery<TrainerComments>("EXEC USP_GetTrainerComments @TrainerId",
                    new SqlParameter("TrainerId", string.IsNullOrEmpty(TrainerId) ? DBNull.Value : (object)TrainerId)
                    ).ToList();
            }
            return trainercomment;
        }


        public List<CandidateCourseDetailsView> GetBatchWiseStudent(string CourseCode, Int64 BatchtId)
        {

            var BatchStudent = new List<CandidateCourseDetailsView>();

            using (var db = new UserDBContext())
            {
                var batchtid = new SqlParameter("@BatchId", BatchtId);
                var coursecode = new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode);
                BatchStudent = db.Database
                          .SqlQuery<CandidateCourseDetailsView>("exec USP_BatchWiseStudent @CourseCode, @BatchId", coursecode, batchtid).ToList();
            }

            return BatchStudent;
        }
        public List<CheckInCheckOutView> GetCandidateCheckIn(string SubscriberId)
        {

            var CheckedInCandidate = new List<CheckInCheckOutView>();

            using (var db = new UserDBContext())
            {
                var subscriberid = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                CheckedInCandidate = db.Database
                          .SqlQuery<CheckInCheckOutView>("exec USP_GetCheckIn @SubscriberId ", subscriberid).ToList();
            }

            return CheckedInCandidate;
        }

        //BatchWise CheckIn Details
        public List<CheckInCheckOutView> GetBatchWiseCandidateCheckIn(string CourseCode, Int64 BatchId)
        {

            var CheckedInCandidate = new List<CheckInCheckOutView>();
            var batchtid = new SqlParameter("@BatchId", BatchId);
            var coursecode = new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode);
            using (var db = new UserDBContext())
            {

                CheckedInCandidate = db.Database
                          .SqlQuery<CheckInCheckOutView>("exec USP_GetBatchWiseCheckIn @CourseCode, @BatchId", coursecode, batchtid).ToList();
            }

            return CheckedInCandidate;
        }

    }
}