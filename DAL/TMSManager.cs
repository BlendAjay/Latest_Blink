using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AJSolutions.Areas.CMS.Models;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using AJSolutions.Models;
using System.Data.Entity;
using System.Web.Script.Serialization;
using System.Net;
using System.Data;

namespace AJSolutions.DAL
{
    public class TMSManager
    {
        UserDBContext udbc = new UserDBContext();
        Generic generic = new Generic();
        EMSManager ems = new EMSManager();
        HMSManager hms = new HMSManager();
        BlobManager blobManager = new BlobManager();

        public bool AddCourseBatch(CourseBatch courseBatch)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var courseCode = new SqlParameter("@CourseCode", string.IsNullOrEmpty(courseBatch.CourseCode) ? DBNull.Value : (object)courseBatch.CourseCode);
                    var batchName = new SqlParameter("@BatchName", string.IsNullOrEmpty(courseBatch.BatchName) ? DBNull.Value : (object)courseBatch.BatchName);
                    var fromDate = new SqlParameter("@FromDate", (object)courseBatch.FromDate);
                    var toDate = new SqlParameter("@ToDate", (object)courseBatch.ToDate);
                    var fromTime = new SqlParameter("@FromTime", (object)courseBatch.FromTime);
                    var toTime = new SqlParameter("@ToTime", (object)courseBatch.ToTime);
                    var isDailyAttendence = new SqlParameter("@IsDailyAttendence", (object)courseBatch.IsDailyAttendence);
                    var isFeedbackRequired = new SqlParameter("@IsFeedbackRequired", (object)courseBatch.IsFeedbackRequired);
                    var feedbackLink = new SqlParameter("@FeedbackLink", string.IsNullOrEmpty(courseBatch.FeedbackLink) ? DBNull.Value : (object)courseBatch.FeedbackLink);
                    var contentLink = new SqlParameter("@ContentLink", string.IsNullOrEmpty(courseBatch.ContentLink) ? DBNull.Value : (object)courseBatch.ContentLink);
                    var cavability = new SqlParameter("@ContentAvailability", courseBatch.ContentAvailability);
                    var tilldate = new SqlParameter("@AvailableTillDate", DBNull.Value);
                    if (courseBatch.AvailableTillDate != null)
                        tilldate = new SqlParameter("@AvailableTillDate", courseBatch.AvailableTillDate);
                    var accomondationNeeded = new SqlParameter("@AccomondationNeeded", courseBatch.AccomondationNeeded);
                    var attendenceNeeded = new SqlParameter("@AttendenceNeeded", courseBatch.AttendenceNeeded);
                    var wardenId = new SqlParameter("@WardenId", string.IsNullOrEmpty(courseBatch.WardenId) ? DBNull.Value : (object)courseBatch.WardenId);
                    var batchId = new SqlParameter("@BatchId", (object)courseBatch.BatchId);
                    var countryId = new SqlParameter("@CountryId", (object)courseBatch.CountryId);
                    var stateId = new SqlParameter("@StateId", (object)courseBatch.StateId);
                    var cityId = new SqlParameter("@CityId", (object)courseBatch.CityId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCourseBatch @CourseCode, @BatchName, @FromDate, @ToDate, @FromTime, @ToTime, @IsDailyAttendence, @IsFeedbackRequired, @FeedbackLink, @ContentLink, @ContentAvailability, @AvailableTillDate, @AccomondationNeeded, @AttendenceNeeded, @WardenId, @BatchId, @CountryId, @StateId, @CityId",
                                                                                    courseCode, batchName, fromDate, toDate, fromTime, toTime, isDailyAttendence, isFeedbackRequired, feedbackLink, contentLink, cavability, tilldate, accomondationNeeded, attendenceNeeded, wardenId, batchId, countryId, stateId, cityId);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            { }
            return true;
        }

        public List<CourseBatchViewModel> GetCourseBatches(string CorporateId = null, Int64 BatchId = 0)
        {
            var courseBatch = new List<CourseBatchViewModel>();

            using (var db = new UserDBContext())
            {
                courseBatch = db.Database.SqlQuery<CourseBatchViewModel>("EXEC USP_GetCourseBatch @CorporateId, @BatchId",
                            new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId),
                            new SqlParameter("@BatchId", (object)BatchId)).ToList();
            }
            if (courseBatch.Count > 0)
            {
                foreach (CourseBatchViewModel course in courseBatch)
                {
                    course.CorporateProfile = udbc.CorporateProfile.ToList().Find(p => p.CorporateId == course.CorporateId);
                }
            }

            return courseBatch;
        }

        public List<CandidateCredentialsView> GetCandidateCredentials(Int64 BatchId = 0)
        {
            var batches = new List<CandidateCredentialsView>();
            using (var db = new UserDBContext())
            {

                batches = db.Database.SqlQuery<CandidateCredentialsView>("EXEC USP_GetCandidateCredentials @BatchId",
                            new SqlParameter("@BatchId", BatchId)).ToList();
            }
            return batches;
        }

        public List<CourseBatchViewModel> GetBatches(string SubscriberId = null, string CourseCode = null)
        {
            var batches = new List<CourseBatchViewModel>();
            using (var db = new UserDBContext())
            {

                batches = db.Database.SqlQuery<CourseBatchViewModel>("EXEC USP_GetBatches @SubscriberId, @CourseCode",
                            new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId),
                            new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode)).ToList();
            }
            return batches;
        }

        public CourseBatchViewModel GetCBatches(string SubscriberId = null, string CourseCode = null)
        {
            var batches = new CourseBatchViewModel();

            using (var db = new UserDBContext())
            {

                batches = db.Database.SqlQuery<CourseBatchViewModel>("EXEC USP_GetBatches @SubscriberId, @CourseCode",
                            new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId),
                            new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode)).FirstOrDefault();

            }

            return batches;
        }

        public bool AddCandidateCourseDetails(CandidateCourseDetails candidateCourseDetail)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(candidateCourseDetail.UserId) ? DBNull.Value : (object)candidateCourseDetail.UserId);
                    var batchId = new SqlParameter("@BatchId", (object)candidateCourseDetail.BatchId);
                    var installmentId = new SqlParameter("@InstallmentId", (object)candidateCourseDetail.InstallmentId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateCourseDetails  @UserId , @BatchId , @InstallmentId", candidateCourseDetail.UserId, candidateCourseDetail.BatchId, candidateCourseDetail.InstallmentId);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return true;
        }

        public List<CandidateCourseDetails> GetCandidateCourseDetails(string UserId = null, long BatchId = 0)
        {
            var candidateCourseDetails = new List<CandidateCourseDetails>();


            using (var db = new UserDBContext())
            {

                candidateCourseDetails = db.Database.SqlQuery<CandidateCourseDetails>("EXEC USP_GetCandidateCourseDetails @UserId, @BatchId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                            , new SqlParameter("@BatchId", (object)BatchId)).ToList();
            }

            return candidateCourseDetails;
        }

        //public string GetTrainingId()
        //{
        //    UserDBContext udbc = new UserDBContext();
        //    string year = Convert.ToString(DateTime.UtcNow.Year).Substring(2);
        //    string quarter = "S1";
        //    int month = DateTime.UtcNow.Month;
        //    if (month > 3 && month <= 6)
        //    {
        //        quarter = "S2";
        //    }
        //    else if (month > 6 && month <= 9)
        //    {
        //        quarter = "S3";
        //    }
        //    else if (month > 9 && month <= 12)
        //    {
        //        quarter = "S4";
        //    }

        //    string TrainingId = "TS" + year + quarter + "000001";

        //    var TrainingSchedule = from s in udbc.TrainingSchedule.Where(s => s.TrainingId.Substring(0, 6) == "TS" + year + quarter)
        //                orderby s.TrainingId descending
        //                select s.TrainingId;

        //    var Training = TrainingSchedule.FirstOrDefault();

        //    if (Training != null)
        //    {
        //        string TrainingPartialId = Training.Substring(7);
        //        int lastVal = Convert.ToInt32(TrainingPartialId);
        //        lastVal = lastVal + 1;
        //        string suffix = string.Empty;

        //        for (int i = Convert.ToString(lastVal).Length; i < 6; i++)
        //        {
        //            suffix = suffix + "0";
        //        }

        //        TrainingId = Training.Substring(0, 6) + suffix + Convert.ToString(lastVal);
        //    }
        //    return TrainingId;
        //}

        public bool AddTrainingSchedule(string TrainingId, int BatchId, string SubjectLine, string Description,
                                        string TrainerId, string OtherTrainerId, int CountryId, int StateId, int CityId, string Address,
                                        string Status, String TaskId, DateTime? CreatedOn, string CreatedBy,
                                        DateTime? UpdatedOn, string UpdatedBy, string TrainerMentor)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var trainingId = new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId);
                    var batchId = new SqlParameter("@BatchId", BatchId);
                    var subjectLine = new SqlParameter("@SubjectLine", string.IsNullOrEmpty(SubjectLine) ? DBNull.Value : (object)SubjectLine);
                    var description = new SqlParameter("@Description", string.IsNullOrEmpty(Description) ? DBNull.Value : (object)Description);
                    var trainerId = new SqlParameter("@TrainerId", string.IsNullOrEmpty(TrainerId) ? DBNull.Value : (object)TrainerId);
                    var otherTrainerId = new SqlParameter("@OtherTrainerId", string.IsNullOrEmpty(OtherTrainerId) ? DBNull.Value : (object)OtherTrainerId);
                    var countryId = new SqlParameter("@CountryId", CountryId);
                    var stateId = new SqlParameter("@StateId", StateId);
                    var cityId = new SqlParameter("@CityId", CityId);
                    var address = new SqlParameter("@Address", string.IsNullOrEmpty(Address) ? DBNull.Value : (object)Address);
                    var status = new SqlParameter("@Status", string.IsNullOrEmpty(Status) ? DBNull.Value : (object)Status);
                    var taskId = new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId);
                    var createdon = new SqlParameter("@CreatedOn", DBNull.Value);
                    if (CreatedOn != null)
                    {
                        createdon = new SqlParameter("@CreatedOn", CreatedOn);
                    }
                    var createdBy = new SqlParameter("@CreatedBy", string.IsNullOrEmpty(CreatedBy) ? DBNull.Value : (object)CreatedBy);
                    var updatedOn = new SqlParameter("@UpdatedOn", DBNull.Value);
                    if (UpdatedOn != null)
                    {
                        updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    }
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var trainerMentor = new SqlParameter("TrainerMentor", string.IsNullOrEmpty(TrainerMentor) ? DBNull.Value : (object)TrainerMentor);

                    int i = context.Database.ExecuteSqlCommand("dbo.USP_AddTrainingSchedules  @TrainingId, @BatchId, @SubjectLine, @Description, @TrainerId, @OtherTrainerId, @CountryId, @StateId, @CityId, @Address, @Status, @TaskId, @CreatedOn, @CreatedBy, @UpdatedOn, @UpdatedBy, @TrainerMentor",
                                                                trainingId, batchId, subjectLine, description, trainerId, otherTrainerId, countryId, stateId, cityId, address, status, taskId, createdon, createdBy, updatedOn, updatedBy, trainerMentor);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return true;
        }


        public string uploadFile(string TrainingId, HttpPostedFileBase upload)
        {
            string res = "Failure";
            if (upload != null && upload.ContentLength > 0)
            {

                var file = udbc.TrainingScheduleAttachment.Where(d => d.TrainingId == TrainingId).FirstOrDefault();

                if (file != null)
                {
                    Int64 FileId = file.FileId;

                    blobManager.DeleteBlob(TrainingId.ToLower(), GetFileName(FileId).ToLower());
                    udbc.TrainingScheduleAttachment.Remove(file);
                    udbc.SaveChanges();
                }


                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");


                bool added = AddTrainingAttachmentToDB(TrainingId, imgFileName.ToLower(), upload.ContentType);

                if (added)
                {
                    blobManager.UploadBlob(TrainingId.ToLower(), ReplaceFileName(TrainingId).ToLower(), upload);

                }

                res = "Succeed";
            }


            return res;
        }

        public bool AddTrainingAssessment(Int64 AssessmentId, string Assessment, string TrainingId, Int64 Weightage,
                                           string PublicationId, string Title, DateTime StartDate,
                                           DateTime EndDate, DateTime StartTime, DateTime EndTime)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var assessmentId = new SqlParameter("@AssessmentId", AssessmentId);
                    var assessment = new SqlParameter("@Assessment", string.IsNullOrEmpty(Assessment) ? DBNull.Value : (object)Assessment);
                    var trainingId = new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId);
                    var weightage = new SqlParameter("@Weightage", Weightage);
                    var publicationId = new SqlParameter("@PublicationId", PublicationId);
                    var title = new SqlParameter("@Title", Title);
                    var startDate = new SqlParameter("@StartDate", StartDate);
                    var endDate = new SqlParameter("@EndDate", EndDate);
                    var startTime = new SqlParameter("@StartTime", StartTime);
                    var endTime = new SqlParameter("@EndTime", EndTime);

                    int i = context.Database.ExecuteSqlCommand("dbo.USP_AddTrainingAssessment  @AssessmentId, @Assessment, @TrainingId, " +
                                                               "@Weightage, @PublicationId, @Title, @StartDate, @EndDate, @StartTime, @EndTime",
                                                                assessmentId, assessment, trainingId, weightage, publicationId, title,
                                                                startDate, endDate, startTime, endTime);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return true;
        }
        //created by vikas pandey
        //created on - 09/10/2018
        //for marging two procedure in a single procedure
        public bool AddTrainingScheduleAssesment(string TrainingId, int BatchId, string SubjectLine, string Description,
                                       string TrainerId, string OtherTrainerId, int CountryId, int StateId, int CityId, string Address,
                                       string Status, String TaskId, DateTime? CreatedOn, string CreatedBy, DateTime? UpdatedOn, string UpdatedBy,
                                       string TrainerMentor, Int64 AssessmentId, string Assessment, Int64 Weightage,
                                       string PublicationId, string Title, DateTime StartDate, DateTime EndDate, DateTime StartTime, DateTime EndTime)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var trainingId = new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId);
                    var batchId = new SqlParameter("@BatchId", BatchId);
                    var subjectLine = new SqlParameter("@SubjectLine", string.IsNullOrEmpty(SubjectLine) ? DBNull.Value : (object)SubjectLine);
                    var description = new SqlParameter("@Description", string.IsNullOrEmpty(Description) ? DBNull.Value : (object)Description);
                    var trainerId = new SqlParameter("@TrainerId", string.IsNullOrEmpty(TrainerId) ? DBNull.Value : (object)TrainerId);
                    var otherTrainerId = new SqlParameter("@OtherTrainerId", string.IsNullOrEmpty(OtherTrainerId) ? DBNull.Value : (object)OtherTrainerId);
                    var countryId = new SqlParameter("@CountryId", CountryId);
                    var stateId = new SqlParameter("@StateId", StateId);
                    var cityId = new SqlParameter("@CityId", CityId);
                    var address = new SqlParameter("@Address", string.IsNullOrEmpty(Address) ? DBNull.Value : (object)Address);
                    var status = new SqlParameter("@Status", string.IsNullOrEmpty(Status) ? DBNull.Value : (object)Status);
                    var taskId = new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId);
                    var createdon = new SqlParameter("@CreatedOn", DBNull.Value);
                    if (CreatedOn != null)
                    {
                        createdon = new SqlParameter("@CreatedOn", CreatedOn);
                    }
                    var createdBy = new SqlParameter("@CreatedBy", string.IsNullOrEmpty(CreatedBy) ? DBNull.Value : (object)CreatedBy);
                    var updatedOn = new SqlParameter("@UpdatedOn", DBNull.Value);
                    if (UpdatedOn != null)
                    {
                        updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    }
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var trainerMentor = new SqlParameter("TrainerMentor", string.IsNullOrEmpty(TrainerMentor) ? DBNull.Value : (object)TrainerMentor);

                    //for assesment parameters
                    var assessmentId = new SqlParameter("@AssessmentId", AssessmentId);
                    var assessment = new SqlParameter("@Assessment", string.IsNullOrEmpty(Assessment) ? DBNull.Value : (object)Assessment);
                    var atrainingId = new SqlParameter("@ATrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId);
                    var weightage = new SqlParameter("@Weightage", Weightage);
                    var publicationId = new SqlParameter("@PublicationId", PublicationId);
                    var title = new SqlParameter("@Title", Title);
                    var startDate = new SqlParameter("@StartDate", StartDate);
                    var endDate = new SqlParameter("@EndDate", EndDate);
                    var startTime = new SqlParameter("@StartTime", StartTime);
                    var endTime = new SqlParameter("@EndTime", EndTime);
                    //end
                    int i = context.Database.ExecuteSqlCommand("USP_AddTrainingSchedulesAndAssessment  @TrainingId, @BatchId, @SubjectLine, @Description, @TrainerId, @OtherTrainerId, @CountryId, @StateId, @CityId, @Address, @Status, @TaskId, @CreatedOn, @CreatedBy, @UpdatedOn, @UpdatedBy, @TrainerMentor,@AssessmentId, @Assessment, @ATrainingId,@Weightage, @PublicationId, @Title, @StartDate, @EndDate, @StartTime, @EndTime",
                                                                 trainingId, batchId, subjectLine, description, trainerId, otherTrainerId, countryId, stateId, cityId, address, status, taskId, createdon, createdBy, updatedOn, updatedBy, trainerMentor, assessmentId, assessment, atrainingId, weightage, publicationId, title,
                                                                startDate, endDate, startTime, endTime);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return true;
        }
        public List<TrainingAssessmentView> GetTrainingAssessments(string TrainingId = null)
        {
            var trainingAssessments = new List<TrainingAssessmentView>();


            using (var db = new UserDBContext())
            {

                trainingAssessments = db.Database.SqlQuery<TrainingAssessmentView>("EXEC USP_GetTrainingAssessments @TrainingId",
                            new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId)).ToList();
            }

            return trainingAssessments;
        }


        public bool AddTrainingEvaluation(string UserId, Int64 AssessmentId, string TrainingId, DateTime UpdatedOn, string UpdatedBy)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var assessmentId = new SqlParameter("@AssessmentId", AssessmentId);
                    var trainingId = new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);

                    int i = context.Database.ExecuteSqlCommand("dbo.USP_AddTrainingEvaluation  @UserId, @AssessmentId, @TrainingId, @UpdatedOn, @UpdatedBy",
                                                                userId, assessmentId, trainingId, updatedOn, updatedBy);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return true;
        }

        public bool AddEvaluationMarks(Int64 AssessmentId, string UserId, string TrainingId, float Percentage, DateTime UpdatedOn, string UpdatedBy)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var assessmentId = new SqlParameter("AssessmentId", AssessmentId);
                    var userId = new SqlParameter("UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var trainingId = new SqlParameter("TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId);
                    var percentage = new SqlParameter("Percentage", Percentage);
                    var updatedOn = new SqlParameter("UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);

                    int i = context.Database.ExecuteSqlCommand("dbo.USP_AddEvaluationMarks @AssessmentId, @UserId, @TrainingId, @Percentage, @UpdatedOn, @UpdatedBy",
                        assessmentId, userId, trainingId, percentage, updatedOn, updatedBy);

                    if (i > 0)
                        return true;
                    else
                        return false;

                }
            }
            catch (RetryLimitExceededException)
            {

            }
            return true;
        }

        public List<AssessmentEvaluationView> GetAssessmentEvaluation(string TrainingId = null)
        {
            var evaluation = new List<AssessmentEvaluationView>();


            using (var db = new UserDBContext())
            {

                evaluation = db.Database.SqlQuery<AssessmentEvaluationView>("EXEC USP_GetAssessmentEvaluation @TrainingId",
                            new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId)).ToList();
            }

            return evaluation;
        }

        public string GetFileName(Int64 FileId)
        {
            string fileName = null;
            var image = udbc.TaskAttachment.Find(FileId);

            if (image != null)
            {
                fileName = "attachment/" + image.FileId + "/" + image.FileName;
            }
            return fileName;
        }


        public string ReplaceFileName(string TrainingId)
        {
            string fileName = null;
            var image = udbc.TrainingScheduleAttachment.Where(f => f.TrainingId == TrainingId);

            if (image != null)
            {
                if (image.Count() > 0)
                {
                    string imgFileName = image.FirstOrDefault().FileName;
                    imgFileName = imgFileName.Replace(' ', '_');
                    imgFileName = imgFileName.Replace("'", "_");


                    fileName = "attachment/" + image.FirstOrDefault().FileId + "/" + imgFileName;
                }
            }
            return fileName;
        }


        public string ReplaceLeaveAttachedFileName(Int64 plannerId)
        {
            string fileName = null;
            var image = udbc.TrainerPlannerAttachment.Where(f => f.PlannerId == plannerId);

            if (image != null)
            {
                if (image.Count() > 0)
                {
                    string imgFileName = image.FirstOrDefault().FileName;
                    imgFileName = imgFileName.Replace(' ', '_');
                    imgFileName = imgFileName.Replace("'", "_");


                    fileName = "attachment/" + image.FirstOrDefault().FileId + "/" + imgFileName;
                }
            }
            return fileName;
        }


        public bool AddTrainingAttachmentToDB(string TrainingId, string FileName, string ContentType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var trainingId = new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);

                    int i = context.Database.ExecuteSqlCommand("USP_AddTrainingAttachment @TrainingId, @FileName, @ContentType",
                        trainingId, fileName, contentType);

                    if (i == 1)
                        res = true;
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }

        public List<TrainingScheduleView> GetTrainingSchedule(string SubscriberId, string TrainingId)
        {
            var trainingScheduleView = new List<TrainingScheduleView>();
            using (var db = new UserDBContext())
            {

                trainingScheduleView = db.Database.SqlQuery<TrainingScheduleView>("EXEC USP_GetTrainingSchedules @SubscriberId, @TrainingId",
                            new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId),
                            new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId)

                            ).ToList();
            }

            return trainingScheduleView;

        }

        public void RemoveTrainingAttach(Int64 FileId)
        {
            var trainingattach = udbc.TrainingScheduleAttachment.Find(FileId);
            if (trainingattach != null)
            {
                udbc.TrainingScheduleAttachment.Remove(trainingattach);
                udbc.SaveChanges();
            }
        }

        public void RemoveTraining(string TrainingId)
        {
            var trainingassessment = udbc.TrainingAssessment.Where(c => c.TrainingId == TrainingId).ToList();
            if (trainingassessment.Count > 0)
            {
                foreach (var item in trainingassessment)
                {
                    udbc.TrainingAssessment.Remove(item);
                    udbc.SaveChanges();
                }
            }
            var training = udbc.TrainingSchedule.Find(TrainingId);
            if (training != null)
            {
                udbc.TrainingSchedule.Remove(training);
                udbc.SaveChanges();
            }
        }

        public void RemoveTrainingOrder(Int64 TrainingId)
        {
            var trainingorder = udbc.EndUserTrainingOrder.Find(TrainingId);
            var trainee = udbc.EnduUserTrainee.Where(c => c.EUTrainingOrderId == trainingorder.EUTrainingOrderId).ToList();
            if (trainee != null)
            {
                foreach (var item in trainee)
                {
                    var traineesId = udbc.EnduUserTrainee.Find(item.EUTraineeId);
                    udbc.EnduUserTrainee.Remove(traineesId);
                    udbc.SaveChanges();
                }
            }
            if (trainingorder != null)
            {
                udbc.EndUserTrainingOrder.Remove(trainingorder);
                udbc.SaveChanges();
            }
        }

        public int GetTaskTrainingCount(string TaskId)
        {

            int TaskTrainingCount;

            using (var db = new UserDBContext())
            {
                TaskTrainingCount = db.Database
                          .ExecuteSqlCommand("EXEC USP_GetTrainingScheduleCount @TaskId ",
                          new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId)
                          );
            }

            return TaskTrainingCount;
        }

        public TrainingScheduleView GetTrainingScheduleById(string SubscriberId, string TrainingId)
        {
            var trainingScheduleView = new TrainingScheduleView();
            using (var db = new UserDBContext())
            {
                trainingScheduleView = db.Database.SqlQuery<TrainingScheduleView>("EXEC USP_GetTrainingSchedules @SubscriberId, @TrainingId",
                            new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId),
                            new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId)
                            ).FirstOrDefault();
            }
            return trainingScheduleView;
        }

        //das
        public List<CandidateTraining> GetCandiateTraining(Int64 BatchId, string TrainingId = null)
        {
            var candidatetraining = new List<CandidateTraining>();
            using (var db = new UserDBContext())
            {
                candidatetraining = db.Database.SqlQuery<CandidateTraining>("EXEC USP_GetCandiateTraining @BatchId, @TrainingId",
                            new SqlParameter("@BatchId", BatchId),
                            new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId)).ToList();
            }

            if (candidatetraining.Count() > 0)
            {
                foreach (var item in candidatetraining)
                {
                    item.CandidateList = hms.GetCandidateAttendancelist(item.TrainingId).ToList();
                }
            }
            return candidatetraining;
        }

        public bool UpdateStatus(string TrainingId, string Status)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var trainingId = new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId);
                    var status = new SqlParameter("@Status", Status);

                    int i = context.Database.ExecuteSqlCommand("dbo.USP_UpdateTrainingStatus @TrainingId, @Status",
                                                               trainingId, status);
                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
            }

            return res;
        }

        public List<CourseMaster> GetCourseDetails(string CorporateId)
        {
            var courseMaster = new List<CourseMaster>();


            using (var db = new UserDBContext())
            {

                courseMaster = db.Database.SqlQuery<CourseMaster>("EXEC USP_GetCourseMasters @CorporateId",
                            new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
            }

            return courseMaster;
        }

        //getcourse for checkincheckout
        public List<CourseBatchViewModel> GetStudentBatchDetails(string CourseCode)
        {
            var courseMaster = new List<CourseBatchViewModel>();


            using (var db = new UserDBContext())
            {

                courseMaster = db.Database.SqlQuery<CourseBatchViewModel>("EXEC USP_GetStudentCourse @CourseCode ", new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode)).ToList();

            }

            return courseMaster;
        }

        public List<CourseBatchViewModel> GetStudentCourseDetails(string SubscriberId)
        {
            var course = new List<CourseBatchViewModel>();


            using (var db = new UserDBContext())
            {
                var subscriberid = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                course = db.Database.SqlQuery<CourseBatchViewModel>("EXEC USP_GetCourseView @SubscriberId ", subscriberid).ToList();


            }

            return course;
        }

        public bool AddTrainerPlan(TrainerPlanner TrainerPlanner)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var plannerId = new SqlParameter("@PlannerId", (object)TrainerPlanner.PlannerId);
                    var trainerId = new SqlParameter("@TrainerId", string.IsNullOrEmpty(TrainerPlanner.TrainerId) ? DBNull.Value : (object)TrainerPlanner.TrainerId);
                    var fromDate = new SqlParameter("@FromDate", (object)TrainerPlanner.FromDate);
                    var toDate = new SqlParameter("@ToDate", (object)TrainerPlanner.ToDate);
                    var fromTime = new SqlParameter("@FromTime", (object)TrainerPlanner.FromTime.ToString("hh:mm:ss tt"));
                    var toTime = new SqlParameter("@ToTime", (object)TrainerPlanner.ToTime.ToString("hh:mm:ss tt"));
                    var remarks = new SqlParameter("@Remarks", string.IsNullOrEmpty(TrainerPlanner.Remarks) ? DBNull.Value : (object)TrainerPlanner.Remarks);
                    var engagementtypeid = new SqlParameter("@EngagementTypeId", (object)TrainerPlanner.EngagementTypeId);
                    var isApproved = new SqlParameter("@IsApproved", TrainerPlanner.IsApproved);
                    var isHalfDay = new SqlParameter("IsHalfDay", TrainerPlanner.HalfDay);
                    var approvedBy = new SqlParameter("@ApprovedBy", string.IsNullOrEmpty(TrainerPlanner.ApprovedBy) ? DBNull.Value : (object)TrainerPlanner.ApprovedBy);
                    var approvalDate = new SqlParameter("@ApprovalDate", DBNull.Value);
                    if (TrainerPlanner.ApprovalDate != null)
                    {
                        approvalDate = new SqlParameter("@ApprovalDate", TrainerPlanner.ApprovalDate.Value);
                    }
                    var schemeId = new SqlParameter("@SchemeId", TrainerPlanner.SchemeId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddTrainerPlanners  @PlannerId , @TrainerId, @FromDate, @ToDate, @FromTime, @ToTime, @Remarks,@EngagementTypeId,@IsApproved,@IsHalfDay,@ApprovedBy,@ApprovalDate, @SchemeId", plannerId, trainerId, fromDate, toDate, fromTime, toTime, remarks, engagementtypeid, isApproved, isHalfDay, approvedBy, approvalDate, schemeId);


                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return true;
        }

        public string uploadLeaveAttachment(Int64 plannerId, HttpPostedFileBase upload)
        {
            string res = "Failure";
            if (upload != null && upload.ContentLength > 0)
            {

                using (var db = new UserDBContext())
                {
                    var file = db.TrainerPlannerAttachment.Where(d => d.PlannerId == plannerId).FirstOrDefault();

                    if (file != null)
                    {
                        Int64 FileId = file.FileId;

                        blobManager.DeleteBlob(plannerId.ToString(), GetFileName(FileId).ToLower());
                        db.TrainerPlannerAttachment.Remove(file);
                        db.SaveChanges();
                    }

                }
                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");


                bool added = AddLeaveAttachmentToDB(plannerId, imgFileName.ToLower(), upload.ContentType);

                if (added)
                {
                    blobManager.UploadBlob(plannerId.ToString(), ReplaceLeaveAttachedFileName(plannerId).ToLower(), upload);

                }

                res = "Succeed";
            }


            return res;
        }

        public bool AddLeaveAttachmentToDB(Int64 PlannerId, string FileName, string ContentType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var plannerId = new SqlParameter("@PlannerId", (object)PlannerId);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);

                    int i = context.Database.ExecuteSqlCommand("USP_AddLeaveAttachment @plannerId, @FileName, @ContentType",
                        plannerId, fileName, contentType);

                    if (i == 1)
                        res = true;
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }

        public List<TrainerPlannerView> GetTrainerPlaner(string UserId, string ViewType = "USER")
        {
            var trainerPlanner = new List<TrainerPlannerView>();


            using (var db = new UserDBContext())
            {

                trainerPlanner = db.Database.SqlQuery<TrainerPlannerView>("EXEC USP_GetTrainerPlanners @UserId, @ViewType",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                            , new SqlParameter("@ViewType", string.IsNullOrEmpty(ViewType) ? DBNull.Value : (object)ViewType)).ToList();
            }

            return trainerPlanner;
        }

        //Trainnerplannerviewmodel
        public List<TrainerPlannerView> GetTrainerPlanerView(string UserId, string ViewType = "USER")
        {
            var trainerPlanner = new List<TrainerPlannerView>();


            using (var db = new UserDBContext())
            {

                trainerPlanner = db.Database.SqlQuery<TrainerPlannerView>("EXEC USP_GetTrainerPlanners @UserId, @ViewType",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                            , new SqlParameter("@ViewType", string.IsNullOrEmpty(ViewType) ? DBNull.Value : (object)ViewType)).ToList();
            }

            return trainerPlanner;
        }

        //update trainerplanner
        public bool EngagementApprovalStatus(Int64 IsApproved, string ApprovedBy, DateTime? ApprovalDate, Int64 PlannerId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var isapproved = new SqlParameter("@IsApproved", IsApproved);
                    var approvedby = new SqlParameter("@ApprovedBy", string.IsNullOrEmpty(ApprovedBy) ? DBNull.Value : (object)ApprovedBy);
                    var approvaldate = new SqlParameter("@ApprovalDate", ApprovalDate);
                    var plnnerid = new SqlParameter("@PlannerId", PlannerId);
                    int i = context.Database.ExecuteSqlCommand("dbo.USP_UpdateTrainerPlanner @IsApproved, @ApprovedBy,@ApprovalDate,@PlannerId",
                                                               isapproved, approvedby, approvaldate, plnnerid);
                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
            }

            return res;
        }

        public TrainerPlannerView GetTrainerPlannerforPlannerId(Int64 PlannerId)
        {
            var trainerPlanner = new TrainerPlannerView();


            using (var db = new UserDBContext())
            {

                trainerPlanner = db.Database.SqlQuery<TrainerPlannerView>("EXEC USP_GetTrainerPlannersforPlannerId @PlannerId",
                            new SqlParameter("@PlannerId", PlannerId)).FirstOrDefault();
            }

            return trainerPlanner;
        }

        public bool DeleteTrainerPlan(long PlannerId)
        {
            bool result = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var plannerId = new SqlParameter("@PlannerId", (object)PlannerId);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteTrainingPlan @PlannerId", plannerId);

                    if (i == 1)
                        result = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return result;
        }

        public List<TrainerPlanner> GetTrainerAvailability(string TrainerId, int BatchId)
        {
            var TraineAvailability = new List<TrainerPlanner>();

            using (var db = new UserDBContext())
            {
                TraineAvailability = db.Database.SqlQuery<TrainerPlanner>("EXEC USP_GetTraineAvailability @TrainerId, @BatchId",
                            new SqlParameter("@TrainerId", string.IsNullOrEmpty(TrainerId) ? DBNull.Value : (object)TrainerId),
                            new SqlParameter("@BatchId", BatchId)
                            ).ToList();
            }

            return TraineAvailability;

        }

        public List<EngagementTypeMaster> GetEngagementType(string SubscriberId)
        {
            var engagementtype = new List<EngagementTypeMaster>();


            using (var db = new UserDBContext())
            {

                engagementtype = db.Database.SqlQuery<EngagementTypeMaster>("EXEC USP_GetEngagementTypeMaster @SubscriberId ",
                     new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();


            }

            return engagementtype;
        }

        /// <summary>
        /// Created by: Ajay Kumar Choudhary
        /// Created on: 22-5-2017
        /// </summary>
        public bool AddHoliday(Holiday holiday)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var holidayId = new SqlParameter("@HolidayId", (object)holiday.HolidayId);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(holiday.CorporateId) ? DBNull.Value : (object)holiday.CorporateId);
                    var fromDate = new SqlParameter("@FromDate", (object)holiday.FromDate);
                    var toDate = new SqlParameter("@ToDate", (object)holiday.ToDate);
                    var holidayType = new SqlParameter("@HolidayType", string.IsNullOrEmpty(holiday.HolidayType) ? DBNull.Value : (object)holiday.HolidayType);
                    var remarks = new SqlParameter("@Remarks", string.IsNullOrEmpty(holiday.Remarks) ? DBNull.Value : (object)holiday.Remarks);

                    int i = context.Database.ExecuteSqlCommand("USP_AddHolidays @HolidayId, @CorporateId, @FromDate, @ToDate, @HolidayType, @Remarks", holidayId, corporateId, fromDate, toDate, holidayType, remarks);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return true;
        }

        public List<Holiday> GetHolidays(string CorporateId)
        {
            var holidays = new List<Holiday>();


            using (var db = new UserDBContext())
            {

                holidays = db.Database.SqlQuery<Holiday>("EXEC USP_GetHolidays @CorporateId",
                            new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
            }

            return holidays;
        }

        public void RemoveHolidays(Int64 HolidayId)
        {
            var holiday = udbc.Holiday.Find(HolidayId);
            if (holiday != null)
            {
                udbc.Holiday.Remove(holiday);
                udbc.SaveChanges();
            }
        }

        public string uploadTrainingFile(string TrainingId, HttpPostedFileBase upload)
        {
            string res = "Failure";
            if (upload != null && upload.ContentLength > 0)
            {

                var file = udbc.TrainingScheduleFinalAttachment.Where(d => d.TrainingId == TrainingId).FirstOrDefault();

                if (file != null)
                {
                    Int64 FileId = file.FileId;

                    blobManager.DeleteBlob(TrainingId.ToLower(), GetTrainingFileName(FileId).ToLower());
                    udbc.TrainingScheduleFinalAttachment.Remove(file);
                    udbc.SaveChanges();
                }


                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");


                bool added = AddTrainingFinalAttachmentToDB(TrainingId, imgFileName.ToLower(), upload.ContentType);

                if (added)
                {
                    blobManager.UploadBlob(TrainingId.ToLower(), ReplaceFileNameFinalTraining(TrainingId).ToLower(), upload);

                }

                res = "Succeed";
            }


            return res;
        }

        public bool AddTrainingFinalAttachmentToDB(string TrainingId, string FileName, string ContentType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var trainingId = new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);

                    int i = context.Database.ExecuteSqlCommand("USP_AddTrainingFinalAttachment @TrainingId, @FileName, @ContentType",
                        trainingId, fileName, contentType);

                    if (i == 1)
                        res = true;
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }

        public List<TrainingScheduleFinalAttachment> GetTrainingFinalAttachments(string TrainingId)
        {
            var Attachment = new List<TrainingScheduleFinalAttachment>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Attachment = udbc.Database
                            .SqlQuery<TrainingScheduleFinalAttachment>("EXEC USP_GetTrainingFinalAttachments @TrainingId",
                             new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Attachment;
        }

        public string GetTrainingFileName(Int64 FileId)
        {
            string fileName = null;
            var image = udbc.TrainingScheduleFinalAttachment.Find(FileId);

            if (image != null)
            {
                fileName = "finalattachment/" + image.FileId + "/" + image.FileName;
            }
            return fileName;
        }

        public string ReplaceFileNameFinalTraining(string TrainingId)
        {
            string fileName = null;
            var image = udbc.TrainingScheduleFinalAttachment.Where(f => f.TrainingId == TrainingId);

            if (image != null)
            {
                if (image.Count() > 0)
                {
                    string imgFileName = image.FirstOrDefault().FileName;
                    imgFileName = imgFileName.Replace(' ', '_');
                    imgFileName = imgFileName.Replace("'", "_");


                    fileName = "finalattachment/" + image.FirstOrDefault().FileId + "/" + imgFileName;
                }
            }
            return fileName;
        }

        public string IsCourseIntegrated(string CourseCode)
        {
            string result = "NA";
            try
            {
                CourseMaster course = udbc.CourseMaster.Find(CourseCode);
                if (course != null)
                {
                    if (course.LMSCourseCode != null)
                        result = course.LMSCourseCode;
                }
                //using (WebClient webClient = new System.Net.WebClient())
                //{
                //    var url = Global.WikipianUrl() + "api/value/GetIsCourseIntegrated?RefCourseCode=" + CourseCode;
                //    var myString = webClient.DownloadString(url);
                //    JavaScriptSerializer js = new JavaScriptSerializer();
                //    bool myData = js.Deserialize<bool>(myString);
                //    result = myData;
                //}
            }
            catch (Exception) { }
            return result;
        }

        public CourseMasterViewModel GetLMSCourseDetails(string CourseCode)
        {
            CourseMasterViewModel coursedetails = new CourseMasterViewModel();
            try
            {
                using (WebClient webClient = new System.Net.WebClient())
                {
                    var url = Global.WikipianUrl() + "api/value/GetIntegratedCourseDetail?CourseCode=" + CourseCode;
                    var myString = webClient.DownloadString(url);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    CourseMasterViewModel myData = js.Deserialize<CourseMasterViewModel>(myString);
                    coursedetails = myData;
                }
            }
            catch (Exception)
            {

            }
            return coursedetails;
        }

        public List<CorporateTemplate> GetLetterTemplateDetail(string UserId = null, Int64 TemplateId = 0)
        {
            List<CorporateTemplate> templateId = new List<CorporateTemplate>();
            if (TemplateId == 0)
                templateId = udbc.CorporateTemplate.Where(c => c.CorporateId == UserId).OrderBy(c => c.Name).ToList();
            else
            {
                templateId = (from l in udbc.CorporateTemplate.Where(l => l.TemplateId == TemplateId) select l).OrderBy(c => c.Name).ToList();
            }
            return templateId;
        }

        public List<EmployeeBasicDetails> GetEmployeeDetailList(string UserId)
        {
            List<EmployeeBasicDetails> userId = new List<EmployeeBasicDetails>();
            if (UserId != null)
            {
                userId = udbc.EmployeeBasicDetails.Where(c => c.SubscriberId == UserId).OrderBy(c => c.Name).ToList();
            }
            else
            {
                userId = (from l in udbc.EmployeeBasicDetails.Where(l => l.UserId == UserId) select l).OrderBy(c => c.Name).ToList();
            }
            return userId;
        }

        public List<AssetGroup> GetAssetGroupDetail(string CorporateId, int AssetGroupId = 0)
        {
            List<AssetGroup> assetGroupId = new List<AssetGroup>();
            if (AssetGroupId == 0)
                assetGroupId = udbc.AssetGroup.Where(l => l.CorporateId == CorporateId).ToList();
            else
            {
                assetGroupId = (from l in udbc.AssetGroup.Where(l => l.CorporateId == CorporateId && l.AssetGroupId == AssetGroupId) select l).ToList();
            }
            return assetGroupId;
        }

        public List<Designation> GetDesignation(string CorporateId, int DesignationId = 0)
        {
            List<Designation> designationId = new List<Designation>();
            if (DesignationId == 0)
                designationId = udbc.Designation.Where(c => c.CorporateId == CorporateId).ToList();
            else
            {
                designationId = (from l in udbc.Designation.Where(l => l.CorporateId == CorporateId && l.DesignationId == DesignationId) select l).ToList();
            }
            designationId = designationId.OrderBy(c => c.DesignationName).ToList();
            return designationId;
        }

        public List<StatusMaster> GetStatusMaster(string CorporateId, int StatusId = 0)
        {
            List<StatusMaster> statusId = new List<StatusMaster>();
            if (StatusId == 0)
                statusId = udbc.StatusMaster.Where(c => c.CorporateId == CorporateId).ToList();
            else
            {
                statusId = (from l in udbc.StatusMaster.Where(l => l.CorporateId == CorporateId && l.StatusId == StatusId) select l).ToList();
            }
            return statusId;
        }

        /// <summary>
        /// Create By : Vikash Das
        /// Created On : 15-09-2017
        /// Purpose :
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<LetterType> GetLetterList(string UserId, Int64 LetterTypeId = 0)
        {
            List<LetterType> letterlist = new List<LetterType>();
            if (LetterTypeId == 0)
            {
                letterlist = udbc.LetterType.Where(c => c.CorporateId == UserId).OrderBy(c => c.LetterTypeName).ToList();
            }
            else
            {
                letterlist = (from l in udbc.LetterType.Where(l => l.CorporateId == UserId && l.LetterTypeId == LetterTypeId).OrderBy(c => c.LetterTypeName) select l).ToList();
            }
            return letterlist;
        }

        /// <summary>
        /// Create By : Rahul Newara
        /// Created On : 28-09-2017
        /// Purpose : get Canidate Assessment list
        /// </summary>
        /// 

        public List<TrainingAssessmentView> GetCandidateAssessment(string UserId, string CourseCode)
        {
            List<TrainingAssessmentView> TAV = new List<TrainingAssessmentView>();
            try
            {
                using (var udb = new UserDBContext())
                {
                    TAV = udb.Database.SqlQuery<TrainingAssessmentView>("EXEC USP_GetCandidateAssessment @UserId, @CourseCode",
                        new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId),
                        new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode)
                        ).ToList();
                }
            }
            catch (RetryLimitExceededException)
            {
            }
            return TAV;
        }
        public List<TrainingAssessmentView> GetCandidateAssessmentTraining(string UserId, string CourseCode)
        {
            List<TrainingAssessmentView> TAV = new List<TrainingAssessmentView>();
            try
            {
                using (var udb = new UserDBContext())
                {
                    TAV = udb.Database.SqlQuery<TrainingAssessmentView>("EXEC USP_GetCandidateAssessmentTraining @UserId, @CourseCode",
                        new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId),
                        new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode)
                        ).ToList();
                }
            }
            catch (RetryLimitExceededException)
            {
            }
            return TAV;
        }

        public List<TrainingAssessmentView> GetCandidateTrainingAssessments(string CandidateId)
        {
            List<TrainingAssessmentView> TAV = new List<TrainingAssessmentView>();
            try
            {
                using (var udb = new UserDBContext())
                {
                    TAV = udb.Database.SqlQuery<TrainingAssessmentView>("EXEC USP_GetCandidateTrainingAssessments @CandidateId",
                        new SqlParameter("@CandidateId", string.IsNullOrEmpty(CandidateId) ? DBNull.Value : (object)CandidateId)).ToList();
                }
            }
            catch (RetryLimitExceededException)
            {
            }
            return TAV;
        }

        public List<LeaveSchemeMaster> GetSchemaList(Int16 SchemeId = 0)
        {
            List<LeaveSchemeMaster> schemalist = new List<LeaveSchemeMaster>();
            if (SchemeId == 0)
                schemalist = udbc.LeaveSchemeMaster.OrderBy(c => c.SchemeName).ToList();
            else
                schemalist = (from l in udbc.LeaveSchemeMaster.Where(l => l.SchemeId == SchemeId).OrderBy(c => c.SchemeName) select l).ToList();
            return schemalist;
        }

        public List<ShiftMaster> GetShiftDetailList(string UserId, Int64 ShiftId = 0)
        {
            List<ShiftMaster> ShiftList = new List<ShiftMaster>();
            if (ShiftId == 0)
                ShiftList = udbc.ShiftMaster.Where(c => c.CorporateId == UserId).OrderBy(c => c.Shift).ToList();
            else
                ShiftList = (from l in udbc.ShiftMaster.Where(l => l.CorporateId == UserId && l.ShiftId == ShiftId).OrderBy(c => c.Shift) select l).ToList();
            return ShiftList;
        }

        public List<TrainerPlannerView> GetTrainerLeaveCalculation(string UserId)
        {
            var trainerPlanner = new List<TrainerPlannerView>();


            using (var db = new UserDBContext())
            {

                trainerPlanner = db.Database.SqlQuery<TrainerPlannerView>("EXEC USP_GetEmployeeLeaveCalculation @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return trainerPlanner;
        }

        public List<CourseMasterwithAddtionalView> GetCourseCompleteDetails(string CorporateId)
        {
            var courseMaster = new List<CourseMasterwithAddtionalView>();
            using (var db = new UserDBContext())
            {
                courseMaster = db.Database.SqlQuery<CourseMasterwithAddtionalView>("EXEC USP_GetCourseMasters @CorporateId",
                            new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
            }
            return courseMaster;
        }

        public List<AnalyticsViewModel> GetAnalyticsAssessment(string CourseCode)
        {
            var Analytics = new List<AnalyticsViewModel>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Analytics = udbc.Database
                            .SqlQuery<AnalyticsViewModel>("EXEC USP_GetAnalytics @CourseCode",
                             new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Analytics;
        }
        public bool AddTrackerReport(string UserId, Int32 Phase, DateTime FeedBackDate, DateTime UpdatedOn, string Answer1, string Answer2, string Answer3, string Answer4, string Answer5, string Answer6, string Answer7, string Answer8, string Answer9, string Answer10, string Answer11, string Answer12, string Answer13, string Answer14, string Answer15, string Answer16, string Answer17, string Answer18,
            string Answer19, string Answer20, string Answer21, string Answer22, string Answer23, string Answer24, string Answer25, string Answer26, string Answer27, string Answer28, string Answer29, string Answer30, string Answer31, string Answer32, string Answer33, string Answer34, string Answer35, string Answer36, string Answer37,
            string Answer38, string Answer39, string Answer40, string Answer41, string Answer42, string Answer43, string Answer44, string Answer45, string Answer46, string Answer47, string Answer48, string Answer49, string Answer50, string Answer51, string Answer52, string Answer54, string Answer55, string Answer56,
            string Answer57, string Answer58, string Answer59, string Answer60, string Answer61, string Answer62, string Answer63, string Answer64, string Answer65, string Answer66,
            string Answer67, string Answer68, string Answer69, string Answer70, string Answer71, string Answer72, string Answer73, string Answer74, string Answer75,
            string Answer76, string Answer77, string Answer78, string Answer79, string Answer80, string Answer81, string Answer82, string Answer83, string Answer84,
            string Answer85, string Answer86, string Answer87, string Answer88, string Answer89, string Answer90, string Answer91, string Answer92, string Answer93, string Answer94, string Answer95, string Answer96, string Answer97)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var phase = new SqlParameter("@Phase", Phase);
                    var feedBackDate = new SqlParameter("@FeedBackDate", FeedBackDate);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var answer1 = new SqlParameter("@Answer1", string.IsNullOrEmpty(Answer1) ? DBNull.Value : (object)Answer1);
                    var answer2 = new SqlParameter("@Answer2", string.IsNullOrEmpty(Answer2) ? DBNull.Value : (object)Answer2);
                    var answer3 = new SqlParameter("@Answer3", string.IsNullOrEmpty(Answer3) ? DBNull.Value : (object)Answer3);
                    var answer4 = new SqlParameter("@Answer4", string.IsNullOrEmpty(Answer4) ? DBNull.Value : (object)Answer4);
                    var answer5 = new SqlParameter("@Answer5", string.IsNullOrEmpty(Answer5) ? DBNull.Value : (object)Answer5);
                    var answer6 = new SqlParameter("@Answer6", string.IsNullOrEmpty(Answer6) ? DBNull.Value : (object)Answer6);
                    var answer7 = new SqlParameter("@Answer7", string.IsNullOrEmpty(Answer7) ? DBNull.Value : (object)Answer7);
                    var answer8 = new SqlParameter("@Answer8", string.IsNullOrEmpty(Answer8) ? DBNull.Value : (object)Answer8);
                    var answer9 = new SqlParameter("@Answer9", string.IsNullOrEmpty(Answer9) ? DBNull.Value : (object)Answer9);
                    var answer10 = new SqlParameter("@Answer10", string.IsNullOrEmpty(Answer10) ? DBNull.Value : (object)Answer10);
                    var answer11 = new SqlParameter("@Answer11", string.IsNullOrEmpty(Answer11) ? DBNull.Value : (object)Answer11);
                    var answer12 = new SqlParameter("@Answer12", string.IsNullOrEmpty(Answer12) ? DBNull.Value : (object)Answer12);
                    var answer13 = new SqlParameter("@Answer13", string.IsNullOrEmpty(Answer13) ? DBNull.Value : (object)Answer13);
                    var answer14 = new SqlParameter("@Answer14", string.IsNullOrEmpty(Answer14) ? DBNull.Value : (object)Answer14);
                    var answer15 = new SqlParameter("@Answer15", string.IsNullOrEmpty(Answer15) ? DBNull.Value : (object)Answer15);
                    var answer16 = new SqlParameter("@Answer16", string.IsNullOrEmpty(Answer16) ? DBNull.Value : (object)Answer16);
                    var answer17 = new SqlParameter("@Answer17", string.IsNullOrEmpty(Answer17) ? DBNull.Value : (object)Answer17);
                    var answer18 = new SqlParameter("@Answer18", string.IsNullOrEmpty(Answer18) ? DBNull.Value : (object)Answer18);
                    var answer19 = new SqlParameter("@Answer19", string.IsNullOrEmpty(Answer19) ? DBNull.Value : (object)Answer19);
                    var answer20 = new SqlParameter("@Answer20", string.IsNullOrEmpty(Answer20) ? DBNull.Value : (object)Answer20);
                    var answer21 = new SqlParameter("@Answer21", string.IsNullOrEmpty(Answer21) ? DBNull.Value : (object)Answer21);
                    var answer22 = new SqlParameter("@Answer22", string.IsNullOrEmpty(Answer22) ? DBNull.Value : (object)Answer22);
                    var answer23 = new SqlParameter("@Answer23", string.IsNullOrEmpty(Answer23) ? DBNull.Value : (object)Answer23);
                    var answer24 = new SqlParameter("@Answer24", string.IsNullOrEmpty(Answer24) ? DBNull.Value : (object)Answer24);
                    var answer25 = new SqlParameter("@Answer25", string.IsNullOrEmpty(Answer25) ? DBNull.Value : (object)Answer25);
                    var answer26 = new SqlParameter("@Answer26", string.IsNullOrEmpty(Answer26) ? DBNull.Value : (object)Answer26);
                    var answer27 = new SqlParameter("@Answer27", string.IsNullOrEmpty(Answer27) ? DBNull.Value : (object)Answer27);
                    var answer28 = new SqlParameter("@Answer28", string.IsNullOrEmpty(Answer28) ? DBNull.Value : (object)Answer28);
                    var answer29 = new SqlParameter("@Answer29", string.IsNullOrEmpty(Answer29) ? DBNull.Value : (object)Answer29);
                    var answer30 = new SqlParameter("@Answer30", string.IsNullOrEmpty(Answer30) ? DBNull.Value : (object)Answer30);
                    var answer31 = new SqlParameter("@Answer31", string.IsNullOrEmpty(Answer31) ? DBNull.Value : (object)Answer31);
                    var answer32 = new SqlParameter("@Answer32", string.IsNullOrEmpty(Answer32) ? DBNull.Value : (object)Answer32);
                    var answer33 = new SqlParameter("@Answer33", string.IsNullOrEmpty(Answer33) ? DBNull.Value : (object)Answer33);
                    var answer34 = new SqlParameter("@Answer34", string.IsNullOrEmpty(Answer34) ? DBNull.Value : (object)Answer34);
                    var answer35 = new SqlParameter("@Answer35", string.IsNullOrEmpty(Answer35) ? DBNull.Value : (object)Answer35);
                    var answer36 = new SqlParameter("@Answer36", string.IsNullOrEmpty(Answer36) ? DBNull.Value : (object)Answer36);
                    var answer37 = new SqlParameter("@Answer37", string.IsNullOrEmpty(Answer37) ? DBNull.Value : (object)Answer37);
                    var answer38 = new SqlParameter("@Answer38", string.IsNullOrEmpty(Answer38) ? DBNull.Value : (object)Answer38);
                    var answer39 = new SqlParameter("@Answer39", string.IsNullOrEmpty(Answer39) ? DBNull.Value : (object)Answer39);
                    var answer40 = new SqlParameter("@Answer40", string.IsNullOrEmpty(Answer40) ? DBNull.Value : (object)Answer40);
                    var answer41 = new SqlParameter("@Answer41", string.IsNullOrEmpty(Answer41) ? DBNull.Value : (object)Answer41);
                    var answer42 = new SqlParameter("@Answer42", string.IsNullOrEmpty(Answer42) ? DBNull.Value : (object)Answer42);
                    var answer43 = new SqlParameter("@Answer43", string.IsNullOrEmpty(Answer43) ? DBNull.Value : (object)Answer43);
                    var answer44 = new SqlParameter("@Answer44", string.IsNullOrEmpty(Answer44) ? DBNull.Value : (object)Answer44);
                    var answer45 = new SqlParameter("@Answer45", string.IsNullOrEmpty(Answer45) ? DBNull.Value : (object)Answer45);
                    var answer46 = new SqlParameter("@Answer46", string.IsNullOrEmpty(Answer46) ? DBNull.Value : (object)Answer46);
                    var answer47 = new SqlParameter("@Answer47", string.IsNullOrEmpty(Answer47) ? DBNull.Value : (object)Answer47);
                    var answer48 = new SqlParameter("@Answer48", string.IsNullOrEmpty(Answer48) ? DBNull.Value : (object)Answer48);
                    var answer49 = new SqlParameter("@Answer49", string.IsNullOrEmpty(Answer49) ? DBNull.Value : (object)Answer49);
                    var answer50 = new SqlParameter("@Answer50", string.IsNullOrEmpty(Answer50) ? DBNull.Value : (object)Answer50);
                    var answer51 = new SqlParameter("@Answer51", string.IsNullOrEmpty(Answer51) ? DBNull.Value : (object)Answer51);
                    var answer52 = new SqlParameter("@Answer52", string.IsNullOrEmpty(Answer52) ? DBNull.Value : (object)Answer52);
                    var answer54 = new SqlParameter("@Answer54", string.IsNullOrEmpty(Answer54) ? DBNull.Value : (object)Answer54);
                    var answer55 = new SqlParameter("@Answer55", string.IsNullOrEmpty(Answer55) ? DBNull.Value : (object)Answer55);
                    var answer56 = new SqlParameter("@Answer56", string.IsNullOrEmpty(Answer56) ? DBNull.Value : (object)Answer56);
                    var answer57 = new SqlParameter("@Answer57", string.IsNullOrEmpty(Answer57) ? DBNull.Value : (object)Answer57);
                    var answer58 = new SqlParameter("@Answer58", string.IsNullOrEmpty(Answer58) ? DBNull.Value : (object)Answer58);
                    var answer59 = new SqlParameter("@Answer59", string.IsNullOrEmpty(Answer59) ? DBNull.Value : (object)Answer59);
                    var answer60 = new SqlParameter("@Answer60", string.IsNullOrEmpty(Answer60) ? DBNull.Value : (object)Answer60);
                    var answer61 = new SqlParameter("@Answer61", string.IsNullOrEmpty(Answer61) ? DBNull.Value : (object)Answer61);
                    var answer62 = new SqlParameter("@Answer62", string.IsNullOrEmpty(Answer62) ? DBNull.Value : (object)Answer62);
                    var answer63 = new SqlParameter("@Answer63", string.IsNullOrEmpty(Answer63) ? DBNull.Value : (object)Answer63);
                    var answer64 = new SqlParameter("@Answer64", string.IsNullOrEmpty(Answer64) ? DBNull.Value : (object)Answer64);
                    var answer65 = new SqlParameter("@Answer65", string.IsNullOrEmpty(Answer65) ? DBNull.Value : (object)Answer65);
                    var answer66 = new SqlParameter("@Answer66", string.IsNullOrEmpty(Answer66) ? DBNull.Value : (object)Answer66);
                    var answer67 = new SqlParameter("@Answer67", string.IsNullOrEmpty(Answer67) ? DBNull.Value : (object)Answer67);
                    var answer68 = new SqlParameter("@Answer68", string.IsNullOrEmpty(Answer68) ? DBNull.Value : (object)Answer68);
                    var answer69 = new SqlParameter("@Answer69", string.IsNullOrEmpty(Answer69) ? DBNull.Value : (object)Answer69);
                    var answer70 = new SqlParameter("@Answer70", string.IsNullOrEmpty(Answer70) ? DBNull.Value : (object)Answer70);
                    var answer71 = new SqlParameter("@Answer71", string.IsNullOrEmpty(Answer71) ? DBNull.Value : (object)Answer71);
                    var answer72 = new SqlParameter("@Answer72", string.IsNullOrEmpty(Answer72) ? DBNull.Value : (object)Answer72);
                    var answer73 = new SqlParameter("@Answer73", string.IsNullOrEmpty(Answer73) ? DBNull.Value : (object)Answer73);
                    var answer74 = new SqlParameter("@Answer74", string.IsNullOrEmpty(Answer74) ? DBNull.Value : (object)Answer74);
                    var answer75 = new SqlParameter("@Answer75", string.IsNullOrEmpty(Answer75) ? DBNull.Value : (object)Answer75);
                    var answer76 = new SqlParameter("@Answer76", string.IsNullOrEmpty(Answer76) ? DBNull.Value : (object)Answer76);
                    var answer77 = new SqlParameter("@Answer77", string.IsNullOrEmpty(Answer77) ? DBNull.Value : (object)Answer77);
                    var answer78 = new SqlParameter("@Answer78", string.IsNullOrEmpty(Answer78) ? DBNull.Value : (object)Answer78);
                    var answer79 = new SqlParameter("@Answer79", string.IsNullOrEmpty(Answer79) ? DBNull.Value : (object)Answer79);
                    var answer80 = new SqlParameter("@Answer80", string.IsNullOrEmpty(Answer80) ? DBNull.Value : (object)Answer80);
                    var answer81 = new SqlParameter("@Answer81", string.IsNullOrEmpty(Answer81) ? DBNull.Value : (object)Answer81);
                    var answer82 = new SqlParameter("@Answer82", string.IsNullOrEmpty(Answer82) ? DBNull.Value : (object)Answer82);
                    var answer83 = new SqlParameter("@Answer83", string.IsNullOrEmpty(Answer83) ? DBNull.Value : (object)Answer83);
                    var answer84 = new SqlParameter("@Answer84", string.IsNullOrEmpty(Answer84) ? DBNull.Value : (object)Answer84);
                    var answer85 = new SqlParameter("@Answer85", string.IsNullOrEmpty(Answer85) ? DBNull.Value : (object)Answer85);
                    var answer86 = new SqlParameter("@Answer86", string.IsNullOrEmpty(Answer86) ? DBNull.Value : (object)Answer86);
                    var answer87 = new SqlParameter("@Answer87", string.IsNullOrEmpty(Answer87) ? DBNull.Value : (object)Answer87);
                    var answer88 = new SqlParameter("@Answer88", string.IsNullOrEmpty(Answer88) ? DBNull.Value : (object)Answer88);
                    var answer89 = new SqlParameter("@Answer89", string.IsNullOrEmpty(Answer89) ? DBNull.Value : (object)Answer89);
                    var answer90 = new SqlParameter("@Answer90", string.IsNullOrEmpty(Answer90) ? DBNull.Value : (object)Answer90);
                    var answer91 = new SqlParameter("@Answer91", string.IsNullOrEmpty(Answer91) ? DBNull.Value : (object)Answer91);
                    var answer92 = new SqlParameter("@Answer92", string.IsNullOrEmpty(Answer92) ? DBNull.Value : (object)Answer92);
                    var answer93 = new SqlParameter("@Answer93", string.IsNullOrEmpty(Answer93) ? DBNull.Value : (object)Answer93);
                    var answer94 = new SqlParameter("@Answer94", string.IsNullOrEmpty(Answer94) ? DBNull.Value : (object)Answer94);
                    var answer95 = new SqlParameter("@Answer95", string.IsNullOrEmpty(Answer94) ? DBNull.Value : (object)Answer94);
                    var answer96 = new SqlParameter("@Answer96", string.IsNullOrEmpty(Answer94) ? DBNull.Value : (object)Answer94);
                    var answer97 = new SqlParameter("@Answer97", string.IsNullOrEmpty(Answer94) ? DBNull.Value : (object)Answer94);

                    int i = context.Database.ExecuteSqlCommand("USP_AddTrackerReport @UserId, @Phase, @FeedbackDate," +
                        "@UpdatedOn,@Answer1,@Answer2,@Answer3,@Answer4,@Answer5,@Answer6,@Answer7,@Answer8,@Answer9,@Answer10,@Answer11,@Answer12," +
                        "@Answer13,@Answer14,@Answer15,@Answer16,@Answer17,@Answer18,@Answer19,@Answer20,@Answer21,@Answer22,@Answer23,@Answer24,@Answer25," +
                        "@Answer26,@Answer27,@Answer28,@Answer29,@Answer30,@Answer31,@Answer32,@Answer33,@Answer34,@Answer35,@Answer36,@Answer37,@Answer38,@Answer39," +
                        "@Answer40,@Answer41,@Answer42,@Answer43,@Answer44,@Answer45,@Answer46,@Answer47,@Answer48,@Answer49,@Answer50,@Answer51,@Answer52,@Answer54,@Answer55,@Answer56," +
                        "@Answer57, @Answer58, @Answer59, @Answer60, @Answer61, @Answer62, @Answer63, @Answer64, @Answer65, @Answer66," +
                        "@Answer67, @Answer68, @Answer69, @Answer70, @Answer71, @Answer72, @Answer73, @Answer74, @Answer75," +
                        "@Answer76, @Answer77, @Answer78, @Answer79, @Answer80, @Answer81, @Answer82, @Answer83, @Answer84," +
                        "@Answer85, @Answer86, @Answer87, @Answer88, @Answer89, @Answer90, @Answer91, @Answer92, @Answer93, @Answer94, @Answer95, @Answer96, @Answer97",
                        userId, phase, feedBackDate, updatedOn, answer1, answer2, answer3, answer4, answer5, answer6, answer7, answer8, answer9, answer10, answer11,
                        answer12, answer13, answer14, answer15, answer16, answer17, answer18, answer19, answer20, answer21, answer22, answer23, answer24, answer25, answer26, answer27,
                        answer28, answer29, answer30, answer31, answer32, answer33, answer34, answer35, answer36, answer37, answer38, answer39, answer40, answer41, answer42,
                        answer43, answer44, answer45, answer46, answer47, answer48, answer49, answer50, answer51, answer52, answer54, answer55, answer56,
                        answer57, answer58, answer59, answer60, answer61, answer62, answer63, answer64, answer65, answer66,
                        answer67, answer68, answer69, answer70, answer71, answer72, answer73, answer74, answer75,
                        answer76, answer77, answer78, answer79, answer80, answer81, answer82, answer83, answer84,
                        answer85, answer86, answer87, answer88, answer89, answer90, answer91, answer92, answer93, answer94, answer95, answer96, answer97);

                    if (i == 1)
                        res = true;
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }

        public List<TrackerReportView> GetTrackerReports()
        {
            var Report = new List<TrackerReportView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<TrackerReportView>("EXEC USP_GETTrackerReport").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        public List<TrackerReportTotalView> GetTrackerReportsTotal()
        {
            var Report = new List<TrackerReportTotalView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<TrackerReportTotalView>("EXEC USP_GetTrackerDetailCircleWise").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        public bool AddTrackerReportBatchWise(string UserId, Int32 Phase, DateTime FeedBackDate, DateTime UpdatedOn, string Answer1, string Answer2, string Answer3, string Answer4, string Answer5, string Answer6, string Answer7, string Answer8, string Answer9, string Answer10, string Answer11, string Answer12, string Answer13, string Answer14, string Answer15, string Answer16, string Answer17, string Answer18,
            string Answer19, string Answer20, string Answer21, string Answer22, string Answer23, string Answer24, string Answer25, string Answer26, string Answer27, string Answer28, string Answer29, string Answer30, string Answer31, string Answer32, string Answer33, string Answer34, string Answer35, string Answer36, string Answer37,
            string Answer38, string Answer39, string Answer40, string Answer41, string Answer42, string Answer43, string Answer44, string Answer45, string Answer46, string Answer47, string Answer48, string Answer49, string Answer50, string Answer51, string Answer52, string Answer54, string Answer55, string Answer56,
            string Answer57, string Answer58, string Answer59, string Answer60, string Answer61, string Answer62, string Answer63, string Answer64, string Answer65, string Answer66,
            string Answer67, string Answer68, string Answer69, string Answer70, string Answer71, string Answer72, string Answer73, string Answer74, string Answer75,
            string Answer76, string Answer77, string Answer78, string Answer79, string Answer80, string Answer81, string Answer82, string Answer83, string Answer84,
            string Answer85, string Answer86, string Answer87, string Answer88, string Answer89, string Answer90, string Answer91, string Answer92, string Answer93,
            string Answer94, string Answer95, string Answer96, string Answer97, Int32 Batch, Int64 TrackerNumber, string IPPBOfficerMobile, string TrainingType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var phase = new SqlParameter("@Phase", Phase);
                    var feedBackDate = new SqlParameter("@FeedBackDate", FeedBackDate);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var answer1 = new SqlParameter("@Answer1", string.IsNullOrEmpty(Answer1) ? DBNull.Value : (object)Answer1);
                    var answer2 = new SqlParameter("@Answer2", string.IsNullOrEmpty(Answer2) ? DBNull.Value : (object)Answer2);
                    var answer3 = new SqlParameter("@Answer3", string.IsNullOrEmpty(Answer3) ? DBNull.Value : (object)Answer3);
                    var answer4 = new SqlParameter("@Answer4", string.IsNullOrEmpty(Answer4) ? DBNull.Value : (object)Answer4);
                    var answer5 = new SqlParameter("@Answer5", string.IsNullOrEmpty(Answer5) ? DBNull.Value : (object)Answer5);
                    var answer6 = new SqlParameter("@Answer6", string.IsNullOrEmpty(Answer6) ? DBNull.Value : (object)Answer6);
                    var answer7 = new SqlParameter("@Answer7", string.IsNullOrEmpty(Answer7) ? DBNull.Value : (object)Answer7);
                    var answer8 = new SqlParameter("@Answer8", string.IsNullOrEmpty(Answer8) ? DBNull.Value : (object)Answer8);
                    var answer9 = new SqlParameter("@Answer9", string.IsNullOrEmpty(Answer9) ? DBNull.Value : (object)Answer9);
                    var answer10 = new SqlParameter("@Answer10", string.IsNullOrEmpty(Answer10) ? DBNull.Value : (object)Answer10);
                    var answer11 = new SqlParameter("@Answer11", string.IsNullOrEmpty(Answer11) ? DBNull.Value : (object)Answer11);
                    var answer12 = new SqlParameter("@Answer12", string.IsNullOrEmpty(Answer12) ? DBNull.Value : (object)Answer12);
                    var answer13 = new SqlParameter("@Answer13", string.IsNullOrEmpty(Answer13) ? DBNull.Value : (object)Answer13);
                    var answer14 = new SqlParameter("@Answer14", string.IsNullOrEmpty(Answer14) ? DBNull.Value : (object)Answer14);
                    var answer15 = new SqlParameter("@Answer15", string.IsNullOrEmpty(Answer15) ? DBNull.Value : (object)Answer15);
                    var answer16 = new SqlParameter("@Answer16", string.IsNullOrEmpty(Answer16) ? DBNull.Value : (object)Answer16);
                    var answer17 = new SqlParameter("@Answer17", string.IsNullOrEmpty(Answer17) ? DBNull.Value : (object)Answer17);
                    var answer18 = new SqlParameter("@Answer18", string.IsNullOrEmpty(Answer18) ? DBNull.Value : (object)Answer18);
                    var answer19 = new SqlParameter("@Answer19", string.IsNullOrEmpty(Answer19) ? DBNull.Value : (object)Answer19);
                    var answer20 = new SqlParameter("@Answer20", string.IsNullOrEmpty(Answer20) ? DBNull.Value : (object)Answer20);
                    var answer21 = new SqlParameter("@Answer21", string.IsNullOrEmpty(Answer21) ? DBNull.Value : (object)Answer21);
                    var answer22 = new SqlParameter("@Answer22", string.IsNullOrEmpty(Answer22) ? DBNull.Value : (object)Answer22);
                    var answer23 = new SqlParameter("@Answer23", string.IsNullOrEmpty(Answer23) ? DBNull.Value : (object)Answer23);
                    var answer24 = new SqlParameter("@Answer24", string.IsNullOrEmpty(Answer24) ? DBNull.Value : (object)Answer24);
                    var answer25 = new SqlParameter("@Answer25", string.IsNullOrEmpty(Answer25) ? DBNull.Value : (object)Answer25);
                    var answer26 = new SqlParameter("@Answer26", string.IsNullOrEmpty(Answer26) ? DBNull.Value : (object)Answer26);
                    var answer27 = new SqlParameter("@Answer27", string.IsNullOrEmpty(Answer27) ? DBNull.Value : (object)Answer27);
                    var answer28 = new SqlParameter("@Answer28", string.IsNullOrEmpty(Answer28) ? DBNull.Value : (object)Answer28);
                    var answer29 = new SqlParameter("@Answer29", string.IsNullOrEmpty(Answer29) ? DBNull.Value : (object)Answer29);
                    var answer30 = new SqlParameter("@Answer30", string.IsNullOrEmpty(Answer30) ? DBNull.Value : (object)Answer30);
                    var answer31 = new SqlParameter("@Answer31", string.IsNullOrEmpty(Answer31) ? DBNull.Value : (object)Answer31);
                    var answer32 = new SqlParameter("@Answer32", string.IsNullOrEmpty(Answer32) ? DBNull.Value : (object)Answer32);
                    var answer33 = new SqlParameter("@Answer33", string.IsNullOrEmpty(Answer33) ? DBNull.Value : (object)Answer33);
                    var answer34 = new SqlParameter("@Answer34", string.IsNullOrEmpty(Answer34) ? DBNull.Value : (object)Answer34);
                    var answer35 = new SqlParameter("@Answer35", string.IsNullOrEmpty(Answer35) ? DBNull.Value : (object)Answer35);
                    var answer36 = new SqlParameter("@Answer36", string.IsNullOrEmpty(Answer36) ? DBNull.Value : (object)Answer36);
                    var answer37 = new SqlParameter("@Answer37", string.IsNullOrEmpty(Answer37) ? DBNull.Value : (object)Answer37);
                    var answer38 = new SqlParameter("@Answer38", string.IsNullOrEmpty(Answer38) ? DBNull.Value : (object)Answer38);
                    var answer39 = new SqlParameter("@Answer39", string.IsNullOrEmpty(Answer39) ? DBNull.Value : (object)Answer39);
                    var answer40 = new SqlParameter("@Answer40", string.IsNullOrEmpty(Answer40) ? DBNull.Value : (object)Answer40);
                    var answer41 = new SqlParameter("@Answer41", string.IsNullOrEmpty(Answer41) ? DBNull.Value : (object)Answer41);
                    var answer42 = new SqlParameter("@Answer42", string.IsNullOrEmpty(Answer42) ? DBNull.Value : (object)Answer42);
                    var answer43 = new SqlParameter("@Answer43", string.IsNullOrEmpty(Answer43) ? DBNull.Value : (object)Answer43);
                    var answer44 = new SqlParameter("@Answer44", string.IsNullOrEmpty(Answer44) ? DBNull.Value : (object)Answer44);
                    var answer45 = new SqlParameter("@Answer45", string.IsNullOrEmpty(Answer45) ? DBNull.Value : (object)Answer45);
                    var answer46 = new SqlParameter("@Answer46", string.IsNullOrEmpty(Answer46) ? DBNull.Value : (object)Answer46);
                    var answer47 = new SqlParameter("@Answer47", string.IsNullOrEmpty(Answer47) ? DBNull.Value : (object)Answer47);
                    var answer48 = new SqlParameter("@Answer48", string.IsNullOrEmpty(Answer48) ? DBNull.Value : (object)Answer48);
                    var answer49 = new SqlParameter("@Answer49", string.IsNullOrEmpty(Answer49) ? DBNull.Value : (object)Answer49);
                    var answer50 = new SqlParameter("@Answer50", string.IsNullOrEmpty(Answer50) ? DBNull.Value : (object)Answer50);
                    var answer51 = new SqlParameter("@Answer51", string.IsNullOrEmpty(Answer51) ? DBNull.Value : (object)Answer51);
                    var answer52 = new SqlParameter("@Answer52", string.IsNullOrEmpty(Answer52) ? DBNull.Value : (object)Answer52);
                    var answer54 = new SqlParameter("@Answer54", string.IsNullOrEmpty(Answer54) ? DBNull.Value : (object)Answer54);
                    var answer55 = new SqlParameter("@Answer55", string.IsNullOrEmpty(Answer55) ? DBNull.Value : (object)Answer55);
                    var answer56 = new SqlParameter("@Answer56", string.IsNullOrEmpty(Answer56) ? DBNull.Value : (object)Answer56);
                    var answer57 = new SqlParameter("@Answer57", string.IsNullOrEmpty(Answer57) ? DBNull.Value : (object)Answer57);
                    var answer58 = new SqlParameter("@Answer58", string.IsNullOrEmpty(Answer58) ? DBNull.Value : (object)Answer58);
                    var answer59 = new SqlParameter("@Answer59", string.IsNullOrEmpty(Answer59) ? DBNull.Value : (object)Answer59);
                    var answer60 = new SqlParameter("@Answer60", string.IsNullOrEmpty(Answer60) ? DBNull.Value : (object)Answer60);
                    var answer61 = new SqlParameter("@Answer61", string.IsNullOrEmpty(Answer61) ? DBNull.Value : (object)Answer61);
                    var answer62 = new SqlParameter("@Answer62", string.IsNullOrEmpty(Answer62) ? DBNull.Value : (object)Answer62);
                    var answer63 = new SqlParameter("@Answer63", string.IsNullOrEmpty(Answer63) ? DBNull.Value : (object)Answer63);
                    var answer64 = new SqlParameter("@Answer64", string.IsNullOrEmpty(Answer64) ? DBNull.Value : (object)Answer64);
                    var answer65 = new SqlParameter("@Answer65", string.IsNullOrEmpty(Answer65) ? DBNull.Value : (object)Answer65);
                    var answer66 = new SqlParameter("@Answer66", string.IsNullOrEmpty(Answer66) ? DBNull.Value : (object)Answer66);
                    var answer67 = new SqlParameter("@Answer67", string.IsNullOrEmpty(Answer67) ? DBNull.Value : (object)Answer67);
                    var answer68 = new SqlParameter("@Answer68", string.IsNullOrEmpty(Answer68) ? DBNull.Value : (object)Answer68);
                    var answer69 = new SqlParameter("@Answer69", string.IsNullOrEmpty(Answer69) ? DBNull.Value : (object)Answer69);
                    var answer70 = new SqlParameter("@Answer70", string.IsNullOrEmpty(Answer70) ? DBNull.Value : (object)Answer70);
                    var answer71 = new SqlParameter("@Answer71", string.IsNullOrEmpty(Answer71) ? DBNull.Value : (object)Answer71);
                    var answer72 = new SqlParameter("@Answer72", string.IsNullOrEmpty(Answer72) ? DBNull.Value : (object)Answer72);
                    var answer73 = new SqlParameter("@Answer73", string.IsNullOrEmpty(Answer73) ? DBNull.Value : (object)Answer73);
                    var answer74 = new SqlParameter("@Answer74", string.IsNullOrEmpty(Answer74) ? DBNull.Value : (object)Answer74);
                    var answer75 = new SqlParameter("@Answer75", string.IsNullOrEmpty(Answer75) ? DBNull.Value : (object)Answer75);
                    var answer76 = new SqlParameter("@Answer76", string.IsNullOrEmpty(Answer76) ? DBNull.Value : (object)Answer76);
                    var answer77 = new SqlParameter("@Answer77", string.IsNullOrEmpty(Answer77) ? DBNull.Value : (object)Answer77);
                    var answer78 = new SqlParameter("@Answer78", string.IsNullOrEmpty(Answer78) ? DBNull.Value : (object)Answer78);
                    var answer79 = new SqlParameter("@Answer79", string.IsNullOrEmpty(Answer79) ? DBNull.Value : (object)Answer79);
                    var answer80 = new SqlParameter("@Answer80", string.IsNullOrEmpty(Answer80) ? DBNull.Value : (object)Answer80);
                    var answer81 = new SqlParameter("@Answer81", string.IsNullOrEmpty(Answer81) ? DBNull.Value : (object)Answer81);
                    var answer82 = new SqlParameter("@Answer82", string.IsNullOrEmpty(Answer82) ? DBNull.Value : (object)Answer82);
                    var answer83 = new SqlParameter("@Answer83", string.IsNullOrEmpty(Answer83) ? DBNull.Value : (object)Answer83);
                    var answer84 = new SqlParameter("@Answer84", string.IsNullOrEmpty(Answer84) ? DBNull.Value : (object)Answer84);
                    var answer85 = new SqlParameter("@Answer85", string.IsNullOrEmpty(Answer85) ? DBNull.Value : (object)Answer85);
                    var answer86 = new SqlParameter("@Answer86", string.IsNullOrEmpty(Answer86) ? DBNull.Value : (object)Answer86);
                    var answer87 = new SqlParameter("@Answer87", string.IsNullOrEmpty(Answer87) ? DBNull.Value : (object)Answer87);
                    var answer88 = new SqlParameter("@Answer88", string.IsNullOrEmpty(Answer88) ? DBNull.Value : (object)Answer88);
                    var answer89 = new SqlParameter("@Answer89", string.IsNullOrEmpty(Answer89) ? DBNull.Value : (object)Answer89);
                    var answer90 = new SqlParameter("@Answer90", string.IsNullOrEmpty(Answer90) ? DBNull.Value : (object)Answer90);
                    var answer91 = new SqlParameter("@Answer91", string.IsNullOrEmpty(Answer91) ? DBNull.Value : (object)Answer91);
                    var answer92 = new SqlParameter("@Answer92", string.IsNullOrEmpty(Answer92) ? DBNull.Value : (object)Answer92);
                    var answer93 = new SqlParameter("@Answer93", string.IsNullOrEmpty(Answer93) ? DBNull.Value : (object)Answer93);
                    var answer94 = new SqlParameter("@Answer94", string.IsNullOrEmpty(Answer94) ? DBNull.Value : (object)Answer94);
                    var answer95 = new SqlParameter("@Answer95", string.IsNullOrEmpty(Answer94) ? DBNull.Value : (object)Answer94);
                    var answer96 = new SqlParameter("@Answer96", string.IsNullOrEmpty(Answer94) ? DBNull.Value : (object)Answer94);
                    var answer97 = new SqlParameter("@Answer97", string.IsNullOrEmpty(Answer94) ? DBNull.Value : (object)Answer94);
                    var batch = new SqlParameter("@Batch", Batch);
                    var trackerNumber = new SqlParameter("@TrackerNumber", TrackerNumber);
                    var iPPBOfficerMobile = new SqlParameter("@IPPBOfficerMobile", string.IsNullOrEmpty(IPPBOfficerMobile) ? DBNull.Value : (object)IPPBOfficerMobile);
                    var trainingType = new SqlParameter("@TrainingType", string.IsNullOrEmpty(TrainingType) ? DBNull.Value : (object)TrainingType);

                    int i = context.Database.ExecuteSqlCommand("USP_AddTrackerReportbatchWise @UserId, @Phase, @FeedbackDate," +
                        "@UpdatedOn,@Answer1,@Answer2,@Answer3,@Answer4,@Answer5,@Answer6,@Answer7,@Answer8,@Answer9,@Answer10,@Answer11,@Answer12," +
                        "@Answer13,@Answer14,@Answer15,@Answer16,@Answer17,@Answer18,@Answer19,@Answer20,@Answer21,@Answer22,@Answer23,@Answer24,@Answer25," +
                        "@Answer26,@Answer27,@Answer28,@Answer29,@Answer30,@Answer31,@Answer32,@Answer33,@Answer34,@Answer35,@Answer36,@Answer37,@Answer38,@Answer39," +
                        "@Answer40,@Answer41,@Answer42,@Answer43,@Answer44,@Answer45,@Answer46,@Answer47,@Answer48,@Answer49,@Answer50,@Answer51,@Answer52,@Answer54,@Answer55,@Answer56," +
                        "@Answer57, @Answer58, @Answer59, @Answer60, @Answer61, @Answer62, @Answer63, @Answer64, @Answer65, @Answer66," +
                        "@Answer67, @Answer68, @Answer69, @Answer70, @Answer71, @Answer72, @Answer73, @Answer74, @Answer75," +
                        "@Answer76, @Answer77, @Answer78, @Answer79, @Answer80, @Answer81, @Answer82, @Answer83, @Answer84," +
                        "@Answer85, @Answer86, @Answer87, @Answer88, @Answer89, @Answer90, @Answer91, @Answer92, @Answer93, @Answer94, @Answer95, @Answer96, @Answer97, @Batch, @TrackerNumber, @IPPBOfficerMobile, @TrainingType",
                        userId, phase, feedBackDate, updatedOn, answer1, answer2, answer3, answer4, answer5, answer6, answer7, answer8, answer9, answer10, answer11,
                        answer12, answer13, answer14, answer15, answer16, answer17, answer18, answer19, answer20, answer21, answer22, answer23, answer24, answer25, answer26, answer27,
                        answer28, answer29, answer30, answer31, answer32, answer33, answer34, answer35, answer36, answer37, answer38, answer39, answer40, answer41, answer42,
                        answer43, answer44, answer45, answer46, answer47, answer48, answer49, answer50, answer51, answer52, answer54, answer55, answer56,
                        answer57, answer58, answer59, answer60, answer61, answer62, answer63, answer64, answer65, answer66,
                        answer67, answer68, answer69, answer70, answer71, answer72, answer73, answer74, answer75,
                        answer76, answer77, answer78, answer79, answer80, answer81, answer82, answer83, answer84,
                        answer85, answer86, answer87, answer88, answer89, answer90, answer91, answer92, answer93, answer94, answer95, answer96, answer97, batch, trackerNumber, iPPBOfficerMobile, trainingType);

                    if (i == 1)
                        res = true;
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }

        public List<TrackerReportBatchWiseView> GetTrackerBatchReports()
        {
            var Report = new List<TrackerReportBatchWiseView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<TrackerReportBatchWiseView>("EXEC USP_GETTrackerReportBatchWise").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        public int GetTrainingExistForTrainer(string PhoneNumber, DateTime StartDate, string CourseCode, string SubscriberId)
        {
            var count = new int();
            try
            {
                using (var context = new UserDBContext())
                {
                    count = udbc.Database.SqlQuery<int>("EXEC USP_TrainingExistForTrainer @PhoneNumber, @StartDate, @CourseCode, @SubscriberId",
                           new SqlParameter("@PhoneNumber", string.IsNullOrEmpty(PhoneNumber) ? DBNull.Value : (object)PhoneNumber),
                           new SqlParameter("@StartDate", StartDate),
                           new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode),
                           new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).FirstOrDefault();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return count;
        }

        public List<TrackerReportTotalView> GetTrackerBatchReportsTotal()
        {
            var Report = new List<TrackerReportTotalView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<TrackerReportTotalView>("EXEC USP_GetTrackerDetailCircleWiseBatchWise").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        public bool AddOpsData(OPSData OPSData)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var certifiedUsersId = new SqlParameter("@CertifiedUsersId", OPSData.CertifiedUsersId);
                    var circleName = new SqlParameter("@CircleName", string.IsNullOrEmpty(OPSData.CircleName) ? DBNull.Value : (object)OPSData.CircleName);
                    var comments = new SqlParameter("@Comments", string.IsNullOrEmpty(OPSData.Comments) ? DBNull.Value : (object)OPSData.Comments);
                    var designation = new SqlParameter("@Designation", string.IsNullOrEmpty(OPSData.Designation) ? DBNull.Value : (object)OPSData.Designation);
                    var dOP_SOL_ID = new SqlParameter("@DOP_SOL_ID", string.IsNullOrEmpty(OPSData.DOP_SOL_ID) ? DBNull.Value : (object)OPSData.DOP_SOL_ID);
                    var emailID = new SqlParameter("@EmailID", string.IsNullOrEmpty(OPSData.EmailID) ? DBNull.Value : (object)OPSData.EmailID);
                    var empID = new SqlParameter("@EmpID", string.IsNullOrEmpty(OPSData.EmpID) ? DBNull.Value : (object)OPSData.EmpID);
                    var employeeDesignation = new SqlParameter("@EmployeeDesignation", string.IsNullOrEmpty(OPSData.EmployeeDesignation) ? DBNull.Value : (object)OPSData.EmployeeDesignation);
                    var facilityID = new SqlParameter("@FacilityID", string.IsNullOrEmpty(OPSData.FacilityID) ? DBNull.Value : (object)OPSData.FacilityID);
                    var facilityType = new SqlParameter("@FacilityType", string.IsNullOrEmpty(OPSData.FacilityType) ? DBNull.Value : (object)OPSData.FacilityType);
                    var fullName = new SqlParameter("@FullName", string.IsNullOrEmpty(OPSData.FullName) ? DBNull.Value : (object)OPSData.FullName);
                    var iPPBSOLID = new SqlParameter("@IPPBSOLID", string.IsNullOrEmpty(OPSData.IPPBSOLID) ? DBNull.Value : (object)OPSData.IPPBSOLID);
                    var iPPBSolName = new SqlParameter("@IPPBSolName", string.IsNullOrEmpty(OPSData.IPPBSolName) ? DBNull.Value : (object)OPSData.IPPBSolName);
                    var isAvailableLaunch = new SqlParameter("@IsAvailableLaunch", OPSData.IsAvailableLaunch);
                    var isCertified = new SqlParameter("@IsCertified", OPSData.IsCertified);
                    var isMobileDevice = new SqlParameter("@IsMobileDevice", OPSData.IsMobileDevice);
                    var mobileNumber = new SqlParameter("@MobileNumber", string.IsNullOrEmpty(OPSData.MobileNumber) ? DBNull.Value : (object)OPSData.MobileNumber);
                    var reporting_ASP_IPO_EmpID = new SqlParameter("@Reporting_ASP_IPO_EmpID", string.IsNullOrEmpty(OPSData.Reporting_ASP_IPO_EmpID) ? DBNull.Value : (object)OPSData.Reporting_ASP_IPO_EmpID);
                    var reporting_ASP_IPO_Name = new SqlParameter("@Reporting_ASP_IPO_Name", string.IsNullOrEmpty(OPSData.Reporting_ASP_IPO_Name) ? DBNull.Value : (object)OPSData.Reporting_ASP_IPO_Name);
                    var reportingOfficerDesignation = new SqlParameter("@ReportingOfficerDesignation", string.IsNullOrEmpty(OPSData.ReportingOfficerDesignation) ? DBNull.Value : (object)OPSData.ReportingOfficerDesignation);
                    var reportingOfficerEmpID = new SqlParameter("@ReportingOfficerEmpID", string.IsNullOrEmpty(OPSData.ReportingOfficerEmpID) ? DBNull.Value : (object)OPSData.ReportingOfficerEmpID);
                    var reportingOfficerName = new SqlParameter("@ReportingOfficerName", string.IsNullOrEmpty(OPSData.ReportingOfficerName) ? DBNull.Value : (object)OPSData.ReportingOfficerName);


                    int i = context.Database.ExecuteSqlCommand("USP_AddOpsData @CertifiedUsersId, @CircleName, @Comments, @Designation, @DOP_SOL_ID, @EmailID, @EmpID, @EmployeeDesignation, @FacilityID, @FacilityType, @FullName, @IPPBSOLID, @IPPBSolName,@IsAvailableLaunch, @IsCertified, @IsMobileDevice, @MobileNumber, @Reporting_ASP_IPO_EmpID, @Reporting_ASP_IPO_Name,@ReportingOfficerDesignation,@ReportingOfficerEmpID,@ReportingOfficerName",
                                                                                    certifiedUsersId, circleName, comments, designation, dOP_SOL_ID, emailID, empID, employeeDesignation, facilityID, facilityType, fullName, iPPBSOLID, iPPBSolName, isAvailableLaunch, isCertified, isMobileDevice, mobileNumber, reporting_ASP_IPO_EmpID, reporting_ASP_IPO_Name, reportingOfficerDesignation, reportingOfficerEmpID, reportingOfficerName);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            { }
            return true;
        }

        public List<OPSBranch> GetIPPBBranch(string CircleName)
        {
            var Report = new List<OPSBranch>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<OPSBranch>("EXEC USP_GETIPPBBranch @CircleName",
                            new SqlParameter("@CircleName", string.IsNullOrEmpty(CircleName) ? DBNull.Value : (object)CircleName)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }
        //candidate name coursecode wise
        public CandidateCourseDetailsView GetCandidateCourseDetailsName(string UserId = null, string CourseCode = null)
        {
            var candidateCourseDetails = new CandidateCourseDetailsView();


            using (var db = new UserDBContext())
            {

                candidateCourseDetails = db.Database.SqlQuery<CandidateCourseDetailsView>("EXEC USP_GetCandidateCertificate @UserId, @CourseCode",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                            , new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode)).FirstOrDefault();
            }

            return candidateCourseDetails;
        }

        public bool AddMTTrainingDetails(Int64 TrainingOrderId, string TrainingWeek, string Circle, string TOFilledByContactNumber, DateTime UpdatedOn, string UpdatedBy, bool Status)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var trainingOrderId = new SqlParameter("@TrainingOrderId", TrainingOrderId);
                    var trainingWeek = new SqlParameter("@TrainingWeek", string.IsNullOrEmpty(TrainingWeek) ? DBNull.Value : (object)TrainingWeek);
                    var circle = new SqlParameter("@Circle", string.IsNullOrEmpty(Circle) ? DBNull.Value : (object)Circle);
                    var tOFilledByContactNumber = new SqlParameter("@TOFilledByContactNumber", string.IsNullOrEmpty(TOFilledByContactNumber) ? DBNull.Value : (object)TOFilledByContactNumber);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var status = new SqlParameter("@Status", Status);


                    int i = context.Database.ExecuteSqlCommand("USP_AddMTTrainingOrder  @TrainingOrderId , @TrainingWeek, @Circle, @TOFilledByContactNumber, @UpdatedOn, @UpdatedBy, @Status",
                        trainingOrderId, trainingWeek, circle, tOFilledByContactNumber, updatedOn, updatedBy, status);


                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return true;
        }
        public bool AddMTTrainingDetailsTrainee(Int64 TraineeId, string Name, string EmpId, string Gender, string Designation, string MobileNumber, string IppbBranch, string TrackerId, Int64 TrainingOrderId, string TrainingType, string TrainingLocation)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var traineeId = new SqlParameter("@TraineeId", TraineeId);
                    var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
                    var empId = new SqlParameter("@EmpId", string.IsNullOrEmpty(EmpId) ? DBNull.Value : (object)EmpId);
                    var gender = new SqlParameter("@Gender", string.IsNullOrEmpty(Gender) ? DBNull.Value : (object)Gender);
                    var designation = new SqlParameter("@Designation", string.IsNullOrEmpty(Designation) ? DBNull.Value : (object)Designation);
                    var mobileNumber = new SqlParameter("@MobileNumber", string.IsNullOrEmpty(MobileNumber) ? DBNull.Value : (object)MobileNumber);
                    var ippbBranch = new SqlParameter("@IppbBranch", string.IsNullOrEmpty(IppbBranch) ? DBNull.Value : (object)IppbBranch);
                    var trackerId = new SqlParameter("@TrackerId", string.IsNullOrEmpty(TrackerId) ? DBNull.Value : (object)TrackerId);
                    var trainingOrderId = new SqlParameter("@TrainingOrderId", TrainingOrderId);
                    var trainingType = new SqlParameter("@TrainingType", string.IsNullOrEmpty(TrainingType) ? DBNull.Value : (object)TrainingType);
                    var trainingLocation = new SqlParameter("@TrainingLocation", string.IsNullOrEmpty(TrainingLocation) ? DBNull.Value : (object)TrainingLocation);




                    int i = context.Database.ExecuteSqlCommand("USP_AddMTTrainingOrderTraineeDetails  @TraineeId , @Name, @EmpId, @Gender, @Designation, @MobileNumber, @IppbBranch, @TrackerId, @TrainingOrderId, @TrainingType, @TrainingLocation",
                        traineeId, name, empId, gender, designation, mobileNumber, ippbBranch, trackerId, trainingOrderId, trainingType, trainingLocation);


                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return true;
        }
        public List<MTTrainingOrderTraineeDetailsView> GetMTTrainingOrder(string UserId)
        {
            var Report = new List<MTTrainingOrderTraineeDetailsView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<MTTrainingOrderTraineeDetailsView>("EXEC USP_GetMTTrainingOrder @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        public List<MTTrainingOrderTraineeDetailsView> GetMTTrainingOrderforCorporates()
        {
            var Report = new List<MTTrainingOrderTraineeDetailsView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<MTTrainingOrderTraineeDetailsView>("EXEC USP_GetMTTrainingOrderForCorporates").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }


        public List<MTTrainingOrderTraineeDetailsView> GetMTTrainingOrderTrainee(long TrainingOrderId)
        {
            var Report = new List<MTTrainingOrderTraineeDetailsView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<MTTrainingOrderTraineeDetailsView>("EXEC USP_GetMTTrainingOrderTrainee @TrainingOrderId",
                            new SqlParameter("@TrainingOrderId", TrainingOrderId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }
        //by vikas pandey
        public bool AddEUTrainingOrder(Int64 EUTrainingOrderId, string Wave, string Circle, string DOContact, string TrainingType, int PACount, int SUFinacleCount, string PAOfficeName,
           int PAState, int PACity, string PADivision, string PAMTName, string PAMTDesignation, string PAMTContact, string PAMTEmail, DateTime? PATrainingStartDate, DateTime? PATrainingEndDate,
           int GDSCount, int PostmenCount, int GDSSUCount, string GDSOfficeName, int GDSState, int GDSCity, string GDSDivision, string GDSMTName, string GDSMTDesignation, string GDSMTContact,
           string GDSMTEmail, DateTime? GDSTrainingStartDate, DateTime? GDSTrainingEndDate, DateTime UpdatedOn, string UpdatedBy, string Status, string Region,
           string[] TrackerId, string[] Name, string[] PhoneNumber, string[] Gender, string[] EmpId, string[] Designation
           , string[] FaciltyId, string[] AccessPoint, string[] Branch, string[] Flag, string[] EUTraineeId)
        //public bool AddEUTrainingOrder(Int64 EUTrainingOrderId, string Wave, string Circle, string DOContact, string TrainingType, int PACount, int SUFinacleCount, string PAOfficeName,
        //    int PAState, int PACity, string PADivision, string PAMTName, string PAMTDesignation, string PAMTContact, string PAMTEmail, DateTime? PATrainingStartDate, DateTime? PATrainingEndDate,
        //    int GDSCount, int PostmenCount, int GDSSUCount, string GDSOfficeName, int GDSState, int GDSCity, string GDSDivision, string GDSMTName, string GDSMTDesignation, string GDSMTContact,
        //    string GDSMTEmail, DateTime? GDSTrainingStartDate, DateTime? GDSTrainingEndDate, DateTime UpdatedOn, string UpdatedBy
        //     )
        {
            try
            {
                //by - vikas pandey
                DataTable DtTrainingOrder = new DataTable();

                DtTrainingOrder.Columns.Add("EUTraineeId");
                DtTrainingOrder.Columns.Add("TrackerId");
                DtTrainingOrder.Columns.Add("Name");
                DtTrainingOrder.Columns.Add("PhoneNumber");
                DtTrainingOrder.Columns.Add("Gender");
                DtTrainingOrder.Columns.Add("EmpId");
                DtTrainingOrder.Columns.Add("Designation");
                DtTrainingOrder.Columns.Add("FaciltyId");
                DtTrainingOrder.Columns.Add("Accesspoint");
                DtTrainingOrder.Columns.Add("Branch");
                DtTrainingOrder.Columns.Add("Flag");

                for (int i = 0; i < Name.Length; i++)
                {
                    DtTrainingOrder.Rows.Add(EUTraineeId[i], TrackerId[i], Name[i], PhoneNumber[i], Gender[i], EmpId[i], Designation[i], FaciltyId[i], AccessPoint[i], Branch[i], Flag[i]);
                }


                using (var context = new UserDBContext())
                {
                    var eUTrainingOrderId = new SqlParameter("@EUTrainingOrderId", EUTrainingOrderId);
                    var wave = new SqlParameter("@Wave", string.IsNullOrEmpty(Wave) ? DBNull.Value : (object)Wave);
                    var circle = new SqlParameter("@Circle", string.IsNullOrEmpty(Circle) ? DBNull.Value : (object)Circle);
                    var dOContact = new SqlParameter("@DOContact", string.IsNullOrEmpty(DOContact) ? DBNull.Value : (object)DOContact);
                    var trainingType = new SqlParameter("@TrainingType", string.IsNullOrEmpty(TrainingType) ? DBNull.Value : (object)TrainingType);
                    var pACount = new SqlParameter("@PACount", PACount);
                    var sUFinacleCount = new SqlParameter("@SUFinacleCount", SUFinacleCount);
                    var pAOfficeName = new SqlParameter("@PAOfficeName", string.IsNullOrEmpty(PAOfficeName) ? DBNull.Value : (object)PAOfficeName);
                    var pAState = new SqlParameter("@PAState", PAState);
                    var pACity = new SqlParameter("@PACity", PACity);
                    var pADivision = new SqlParameter("@PADivision", string.IsNullOrEmpty(PADivision) ? DBNull.Value : (object)PADivision);
                    var pAMTName = new SqlParameter("@PAMTName", string.IsNullOrEmpty(PAMTName) ? DBNull.Value : (object)PAMTName);
                    var pAMTDesignation = new SqlParameter("@PAMTDesignation", string.IsNullOrEmpty(PAMTDesignation) ? DBNull.Value : (object)PAMTDesignation);
                    var pAMTContact = new SqlParameter("@PAMTContact", string.IsNullOrEmpty(PAMTContact) ? DBNull.Value : (object)PAMTContact);
                    var pAMTEmail = new SqlParameter("@PAMTEmail", string.IsNullOrEmpty(PAMTEmail) ? DBNull.Value : (object)PAMTEmail);
                    var pATrainingStartDate = new SqlParameter("@PATrainingStartDate", PATrainingStartDate);
                    var pATrainingEndDate = new SqlParameter("@PATrainingEndDate", PATrainingEndDate);
                    var gDSCount = new SqlParameter("@GDSCount", GDSCount);
                    var postmenCount = new SqlParameter("@PostmenCount", PostmenCount);
                    var gDSSUCount = new SqlParameter("@GDSSUCount", GDSSUCount);
                    var gDSOfficeName = new SqlParameter("@GDSOfficeName", string.IsNullOrEmpty(GDSOfficeName) ? DBNull.Value : (object)GDSOfficeName);
                    var gDSState = new SqlParameter("@GDSState", GDSState);
                    var gDSCity = new SqlParameter("@GDSCity", GDSCity);
                    var gDSDivision = new SqlParameter("@GDSDivision", string.IsNullOrEmpty(GDSDivision) ? DBNull.Value : (object)GDSDivision);
                    var gDSMTName = new SqlParameter("@GDSMTName", string.IsNullOrEmpty(GDSMTName) ? DBNull.Value : (object)GDSMTName);
                    var gDSMTDesignation = new SqlParameter("@GDSMTDesignation", string.IsNullOrEmpty(GDSMTDesignation) ? DBNull.Value : (object)GDSMTDesignation);
                    var gDSMTContact = new SqlParameter("@GDSMTContact", string.IsNullOrEmpty(GDSMTContact) ? DBNull.Value : (object)GDSMTContact);
                    var gDSMTEmail = new SqlParameter("@GDSMTEmail", string.IsNullOrEmpty(GDSMTEmail) ? DBNull.Value : (object)GDSMTEmail);
                    var gDSTrainingStartDate = new SqlParameter("@GDSTrainingStartDate", GDSTrainingStartDate);
                    var gDSTrainingEndDate = new SqlParameter("@GDSTrainingEndDate", GDSTrainingEndDate);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var status = new SqlParameter("@Status", string.IsNullOrEmpty(Status) ? DBNull.Value : (object)Status);
                    var region = new SqlParameter("@Region", string.IsNullOrEmpty(Region) ? DBNull.Value : (object)Region);
                    //TVP Parameter for procedure
                    var dtTrainingOrderTrainee = new SqlParameter("@tvpTraineeDetails", SqlDbType.Structured);
                    dtTrainingOrderTrainee.TypeName = "dbo.tvpTrainee";
                    dtTrainingOrderTrainee.Value = DtTrainingOrder;

                    int i = context.Database.ExecuteSqlCommand("USP_AddEndUserTrainingOrder_Trainee  @EUTrainingOrderId, @Wave, @Circle, @DOContact, @TrainingType, @PACount, @SUFinacleCount, @PAOfficeName, @PAState, @PACity, @PADivision, @PAMTName , @PAMTDesignation , @PAMTContact , @PAMTEmail , @PATrainingStartDate , @PATrainingEndDate , @GDSCount , @PostmenCount , @GDSSUCount , @GDSOfficeName , @GDSState , @GDSCity , @GDSDivision , @GDSMTName , @GDSMTDesignation, @GDSMTContact, @GDSMTEmail, @GDSTrainingStartDate, @GDSTrainingEndDate, @UpdatedOn, @UpdatedBy,@Status,@Region,@tvpTraineeDetails",
                        eUTrainingOrderId, wave, circle, dOContact, trainingType, pACount, sUFinacleCount, pAOfficeName, pAState, pACity, pADivision, pAMTName, pAMTDesignation, pAMTContact, pAMTEmail, pATrainingStartDate,
                        pATrainingEndDate, gDSCount, postmenCount, gDSSUCount, gDSOfficeName, gDSState, gDSCity, gDSDivision, gDSMTName, gDSMTDesignation, gDSMTContact, gDSMTEmail, gDSTrainingStartDate, gDSTrainingEndDate, updatedOn, updatedBy, status, region, dtTrainingOrderTrainee);

                    //int i = context.Database.ExecuteSqlCommand("USP_AddEndUserTrainingOrder  @EUTrainingOrderId, @Wave, @Circle, @DOContact, @TrainingType, @PACount, @SUFinacleCount, @PAOfficeName, @PAState, @PACity, @PADivision, @PAMTName , @PAMTDesignation , @PAMTContact , @PAMTEmail , @PATrainingStartDate , @PATrainingEndDate , @GDSCount , @PostmenCount , @GDSSUCount , @GDSOfficeName , @GDSState , @GDSCity , @GDSDivision , @GDSMTName , @GDSMTDesignation, @GDSMTContact, @GDSMTEmail, @GDSTrainingStartDate, @GDSTrainingEndDate, @UpdatedOn, @UpdatedBy",
                    // eUTrainingOrderId, wave, circle, dOContact, trainingType, pACount, sUFinacleCount, pAOfficeName, pAState, pACity, pADivision, pAMTName, pAMTDesignation, pAMTContact, pAMTEmail, pATrainingStartDate,
                    // pATrainingEndDate, gDSCount, postmenCount, gDSSUCount, gDSOfficeName, gDSState, gDSCity, gDSDivision, gDSMTName, gDSMTDesignation, gDSMTContact, gDSMTEmail, gDSTrainingStartDate, gDSTrainingEndDate, updatedOn, updatedBy);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }
            return true;
        }

        //public bool AddEndUserTrainee(Int64 EUTraineeId, string TrackerId, string Name, string PhoneNumber, string Gender, string EmpId, string Designation, Int64 EUTrainingOrderId, string FaciltyId, string AccessPoint, string Branch)
        //{
        //    try
        //    {
        //        using (var context = new UserDBContext())
        //        {
        //            var eUTraineeId = new SqlParameter("@EUTraineeId", EUTraineeId);
        //            var trackerId = new SqlParameter("@TrackerId", string.IsNullOrEmpty(TrackerId) ? DBNull.Value : (object)TrackerId);
        //            var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
        //            var phoneNumber = new SqlParameter("@PhoneNumber", string.IsNullOrEmpty(PhoneNumber) ? DBNull.Value : (object)PhoneNumber);
        //            var gender = new SqlParameter("@Gender", string.IsNullOrEmpty(Gender) ? DBNull.Value : (object)Gender);
        //            var empId = new SqlParameter("@EmpId", string.IsNullOrEmpty(EmpId) ? DBNull.Value : (object)EmpId);
        //            var designation = new SqlParameter("@Designation", string.IsNullOrEmpty(Designation) ? DBNull.Value : (object)Designation);
        //            var eUTrainingOrderId = new SqlParameter("@EUTrainingOrderId", EUTrainingOrderId);
        //            var faciltyId = new SqlParameter("@FaciltyId", string.IsNullOrEmpty(FaciltyId) ? DBNull.Value : (object)FaciltyId);
        //            var accessPoint = new SqlParameter("@AccessPoint", string.IsNullOrEmpty(AccessPoint) ? DBNull.Value : (object)AccessPoint);
        //            var branch = new SqlParameter("@Branch", string.IsNullOrEmpty(Branch) ? DBNull.Value : (object)Branch);

        //            int i = context.Database.ExecuteSqlCommand("USP_AddEUTraineeDetails  @EUTraineeId , @TrackerId, @Name, @PhoneNumber, @Gender, @EmpId, @Designation, @EUTrainingOrderId, @FaciltyId, @AccessPoint, @Branch",
        //                eUTraineeId, trackerId, name, phoneNumber, gender, empId, designation, eUTrainingOrderId, faciltyId, accessPoint, branch);


        //            if (i > 0)
        //                return true;
        //            else
        //                return false;
        //        }
        //    }
        //    catch (RetryLimitExceededException /* dex */)
        //    {

        //    }
        //    return true;
        //}

        public List<EndUserTrainingOrderView> GetEUTrainingOrder(string UserId)
        {
            var Report = new List<EndUserTrainingOrderView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserTrainingOrderView>("EXEC USP_GetEUTrainingOrder @UserId",
                            new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }
        public List<EndUserTrainingOrderView> GetEUTrainingOrderTrainee(Int64 EUTrainingOrderId)
        {
            var Report = new List<EndUserTrainingOrderView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserTrainingOrderView>("EXEC USP_GetEUTrainingOrderTrainee @EUTrainingOrderId",
                            new SqlParameter("@EUTrainingOrderId", EUTrainingOrderId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }
        public List<EndUserAttendedCount> GetEndUserDashboardAttendedDetails()
        {
            var Report = new List<EndUserAttendedCount>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserAttendedCount>("EXEC USP_EndUserDashboardAttendedDetails").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        public List<EndUserNominatedCount> GetEndUserDashboardNominatedDetails()
        {
            var Report = new List<EndUserNominatedCount>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserNominatedCount>("EXEC USP_EndUserDashboardNominatedDetails").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        public List<EndUserCertifiedCount> GetEndUserDashboardCertifiedDetails()
        {
            var Report = new List<EndUserCertifiedCount>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserCertifiedCount>("EXEC USP_EndUserDashboardCertifiedDetails").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        public List<EndUserAttendedCountTrend> GetEndUserDashboardTrendGrpahAttendedDetails()
        {
            var Report = new List<EndUserAttendedCountTrend>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserAttendedCountTrend>("EXEC USP_EndUserDashboardTrendGrpahAttendedDetails").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        public List<EndUserNominatedCountTrend> GetEndUserDashboardTrendGraphNominatedDetails()
        {
            var Report = new List<EndUserNominatedCountTrend>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserNominatedCountTrend>("EXEC USP_EndUserDashboardTrendGraphNominatedDetails").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }
        //for total nominated trend chart
        public List<EndUserTrendGraphDetail> GetEndUserDashboardTrendGraphNominatedDetailsChart()
        {
            var Report = new List<EndUserTrendGraphDetail>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserTrendGraphDetail>("EXEC USP_GetTotalTraineeNominatedTrendGraph").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        //for total PA nominated trend chart
        public List<EndUserTrendGraphDetail> GetEndUserDashboardTrendGraphPANominatedDetailsChart()
        {
            var Report = new List<EndUserTrendGraphDetail>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserTrendGraphDetail>("EXEC USP_GetTotalPATraineeNominatedTrendGraph").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }
        //for total GDS nominated trend chart
        public List<EndUserTrendGraphDetail> GetEndUserDashboardTrendGraphGDSNominatedDetailsChart()
        {
            var Report = new List<EndUserTrendGraphDetail>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserTrendGraphDetail>("EXEC USP_GetTotalGDSTraineeNominatedTrendGraph").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        //for total  attended trend chart
        public List<EndUserTrendGraphDetail> GetEndUserDashboardTrendGraphAttendedDetailsChart()
        {
            var Report = new List<EndUserTrendGraphDetail>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserTrendGraphDetail>("EXEC USP_GetTotalTraineeAttendedTrendGraph").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        //for total PA attended trend chart
        public List<EndUserTrendGraphDetail> GetEndUserDashboardTrendGraphPAAttendedDetailsChart()
        {
            var Report = new List<EndUserTrendGraphDetail>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserTrendGraphDetail>("EXEC USP_GetTotalPATraineeAttendedTrendGraph").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        //for total GDS attended trend chart
        public List<EndUserTrendGraphDetail> GetEndUserDashboardTrendGraphGDSAttendedDetailsChart()
        {
            var Report = new List<EndUserTrendGraphDetail>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserTrendGraphDetail>("EXEC USP_GetTotalGDSTraineeAttendedTrendGraph").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        //for total  certified trend chart
        public List<EndUserTrendGraphDetail> GetEndUserDashboardTrendGraphCertifiedDetailsChart()
        {
            var Report = new List<EndUserTrendGraphDetail>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserTrendGraphDetail>("EXEC USP_GetTotalTraineeCertifiedTrendGraph").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }


        //for total PA  certified trend chart
        public List<EndUserTrendGraphDetail> GetEndUserDashboardTrendGraphPACertifiedDetailsChart()
        {
            var Report = new List<EndUserTrendGraphDetail>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserTrendGraphDetail>("EXEC USP_GetTotalPATraineeCertifiedTrendGraph").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        //for total GDS  certified trend chart
        public List<EndUserTrendGraphDetail> GetEndUserDashboardTrendGraphGDSCertifiedDetailsChart()
        {
            var Report = new List<EndUserTrendGraphDetail>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserTrendGraphDetail>("EXEC USP_GetTotalGDSTraineeCertifiedTrendGraph").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }
        public List<EndUserCertifiedCountTrend> GetEndUserDashboardTrandGraphCertifiedDetails()
        {
            var Report = new List<EndUserCertifiedCountTrend>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserCertifiedCountTrend>("EXEC USP_EndUserDashboardTrandGraphCertifiedDetails").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        public List<EndUserCounts> GetEndUserDashboardAllDetails()
        {
            var Report = new List<EndUserCounts>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserCounts>("EXEC USP_EndUserDashboardCountsDetails").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        public bool UpdateTrainingOrderStatus(Int64 TrainingId, string Status)
        {
            var training = udbc.EndUserTrainingOrder.Find(TrainingId);
            if (training != null)
            {
                training.Status = Status;
                udbc.Entry(training).State = EntityState.Modified;
                udbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UpdateTrainingOrderRelation(Int64 TrainingId, string MTReckonnId, string BatchName)
        {
            var training = udbc.EndUserTrainingOrder.Find(TrainingId);
            if (training != null)
            {
                training.BatchName = BatchName;
                training.MTReckonnId = MTReckonnId;
                udbc.Entry(training).State = EntityState.Modified;
                udbc.SaveChanges();
                return true;
            }
            return false;
        }

        public List<EndUserTrainingOrderAdminView> GetEUTrainingOrderAdmin()
        {
            var Report = new List<EndUserTrainingOrderAdminView>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Report = udbc.Database
                            .SqlQuery<EndUserTrainingOrderAdminView>("EXEC USP_GetEUTrainingOrderOverAll").ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Report;
        }

        public bool DeleteTrainingBAtch(Int64 BatchId, string TrainingId)
        {
            bool result = false;
            try
            {
                using (var context = new UserDBContext())
                {

                    var batchId = new SqlParameter("@BatchId", BatchId);
                    var trainingId = new SqlParameter("@TrainingId", string.IsNullOrEmpty(TrainingId) ? DBNull.Value : (object)TrainingId);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteTOBatch @BatchId, @TrainingId", batchId, trainingId);

                    if (i > 0)
                        result = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return result;
        }


    }
}