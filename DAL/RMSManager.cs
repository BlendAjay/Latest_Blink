using AJSolutions.Areas.RMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AJSolutions.DAL
{
    public class RMSManager
    {
        UserDBContext udbc = new UserDBContext();

        public bool AddQuestions(Int64 QuestionId, string Question, string Category, string SubscriberId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var questionId = new SqlParameter("@QuestionId", QuestionId);
                    var question = new SqlParameter("@Question", string.IsNullOrEmpty(Question) ? DBNull.Value : (object)Question);
                    var category = new SqlParameter("@Category", string.IsNullOrEmpty(Category) ? DBNull.Value : (object)Category);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddQuestions @QuestionId, @Question, @Category, @SubscriberId", questionId, question, category, subscriberId);

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

        public List<QuestionMaster> GetQuestion(string SubscriberId = null)
        {
            var question = new List<QuestionMaster>();
            using (var db = new UserDBContext())
            {
                question = db.Database
                         .SqlQuery<QuestionMaster>("exec USP_GetQuestions @SubscriberId",
                new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            return question;
        }

        public void Deletequestion(Int64 QuestionId)
        {
            var question = udbc.QuestionMaster.Find(QuestionId);
            if (question != null)
            {
                udbc.QuestionMaster.Remove(question);
                udbc.SaveChanges();
            }
        }

        public bool AddBranch(string BranchCode, string BranchName, string BranchZone, string CorporateId, string SubcriberId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var branchCode = new SqlParameter("@BranchCode", string.IsNullOrEmpty(BranchCode) ? DBNull.Value : (object)BranchCode);
                    var branchName = new SqlParameter("@BranchName", string.IsNullOrEmpty(BranchName) ? DBNull.Value : (object)BranchName);
                    var branchZone = new SqlParameter("@BranchZone", string.IsNullOrEmpty(BranchZone) ? DBNull.Value : (object)BranchZone);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var subscriberId = new SqlParameter("@SubcriberId", string.IsNullOrEmpty(SubcriberId) ? DBNull.Value : (object)SubcriberId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddBranchDetails @BranchCode, @BranchName, @BranchZone, @CorporateId, @SubcriberId", branchCode, branchName, branchZone, corporateId, subscriberId);

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

        public List<BranchDetailsView> GetBranch(string BranchCode = null)
        {
            var branch = new List<BranchDetailsView>();
            using (var db = new UserDBContext())
            {
                branch = db.Database
                         .SqlQuery<BranchDetailsView>("exec USP_GETBranchDetails @BranchCode",
                    new SqlParameter("@BranchCode", string.IsNullOrEmpty(BranchCode) ? DBNull.Value : (object)BranchCode)).ToList();
            }
            return branch;
        }

        public List<BranchDetailsView> GetBranchDetails(string SubcriberId = null)
        {
            var branchdetails = new List<BranchDetailsView>();
            using (var db = new UserDBContext())
            {
                branchdetails = db.Database
                         .SqlQuery<BranchDetailsView>("exec USP_GETBranchDetailsBySubscriber @SubcriberId",
                    new SqlParameter("@SubcriberId", string.IsNullOrEmpty(SubcriberId) ? DBNull.Value : (object)SubcriberId)).ToList();
            }
            return branchdetails;
        }

        public void DeleteBranch(string BranchCode)
        {
            var branch = udbc.BranchDetails.Find(BranchCode);
            if (branch != null)
            {
                udbc.BranchDetails.Remove(branch);
                udbc.SaveChanges();
            }
        }

        public List<Feedbackview> GetQuestionsFrequency(string Frequency, string SubscriberId)
        {
            var question = new List<Feedbackview>();
            using (var db = new UserDBContext())
            {
                question = db.Database
                         .SqlQuery<Feedbackview>("exec USP_GetQuestionsFrequency @Frequency, @SubscriberId",
                         new SqlParameter("@Frequency", string.IsNullOrEmpty(Frequency) ? DBNull.Value : (object)Frequency),
                         new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            return question;
        }

        public bool AddFeedback(Int64 FeedbackId, Int64 TrainerAssignId, Int64 QuestionId, string GapObserved, string SuggestiveMeasures, string Frequency, DateTime FeedBackdate, DateTime UpdatedOn, string SubcriberId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var feedbackId = new SqlParameter("@FeedbackId", FeedbackId);
                    var trainerAssignId = new SqlParameter("@TrainerAssignId", TrainerAssignId);
                    var question = new SqlParameter("@QuestionId", QuestionId);
                    var gapObserved = new SqlParameter("@GapObserved", string.IsNullOrEmpty(GapObserved) ? DBNull.Value : (object)GapObserved);
                    var suggestiveMeasures = new SqlParameter("@SuggestiveMeasures", string.IsNullOrEmpty(SuggestiveMeasures) ? DBNull.Value : (object)SuggestiveMeasures);
                    var frequency = new SqlParameter("@Frequency", string.IsNullOrEmpty(Frequency) ? DBNull.Value : (object)Frequency);
                    var feedBackdate = new SqlParameter("@FeedBackdate", FeedBackdate);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var subcriberId = new SqlParameter("@SubcriberId", string.IsNullOrEmpty(SubcriberId) ? DBNull.Value : (object)SubcriberId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddFeedBack @FeedbackId, @TrainerAssignId, @QuestionId, @GapObserved, @SuggestiveMeasures, @Frequency, @FeedBackdate, @UpdatedOn, @SubcriberId", feedbackId, trainerAssignId, question, gapObserved, suggestiveMeasures, frequency, feedBackdate, updatedOn, subcriberId);

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

        public List<Feedbackview> GetFeedbackDetails(Int64 TrainerAssignId, string Frequency = null)
        {
            var question = new List<Feedbackview>();
            using (var db = new UserDBContext())
            {
                question = db.Database
                         .SqlQuery<Feedbackview>("exec USP_GetFeedBacks @TrainerAssignId, @Frequency",
                         new SqlParameter("@TrainerAssignId", (object)TrainerAssignId),
                         new SqlParameter("@Frequency", string.IsNullOrEmpty(Frequency) ? DBNull.Value : (object)Frequency)).ToList();
            }

            return question;
        }


        public List<Feedbackview> GetFeedback(string TrainerId = null, string Frequency = null, string CorporateId = null)
        {
            var question = new List<Feedbackview>();
            using (var db = new UserDBContext())
            {
                question = db.Database
                         .SqlQuery<Feedbackview>("exec USP_GetALLFeedBacks @TrainerId, @Frequency, @CorporateId",
                         new SqlParameter("@TrainerId", string.IsNullOrEmpty(TrainerId) ? DBNull.Value : (object)TrainerId),
                         new SqlParameter("@Frequency", string.IsNullOrEmpty(Frequency) ? DBNull.Value : (object)Frequency),
                         new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
            }

            return question;
        }

        public List<TrainerAssignView> GetEmployee()
        {
            var employee = new List<TrainerAssignView>();
            using (var db = new UserDBContext())
            {
                employee = db.Database
                         .SqlQuery<TrainerAssignView>("exec USP_GETEmployee").ToList();
            }
            return employee;
        }

        public bool AddTrainer(Int64 TrainerAssignId, string BranchCode, string TrainerId, DateTime? DateOfJoining, DateTime? LeavingDate)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var trainerAssignId = new SqlParameter("@TrainerAssignId", TrainerAssignId);
                    var branchCode = new SqlParameter("@BranchCode", string.IsNullOrEmpty(BranchCode) ? DBNull.Value : (object)BranchCode);
                    var trainerId = new SqlParameter("@TrainerId", string.IsNullOrEmpty(TrainerId) ? DBNull.Value : (object)TrainerId);
                    var dateOfJoining = new SqlParameter("@DateOfJoining", DateOfJoining);
                    var leavingDate = new SqlParameter("@LeavingDate", string.IsNullOrEmpty(LeavingDate.ToString()) ? DBNull.Value : (object)LeavingDate);

                    int i = context.Database.ExecuteSqlCommand("USP_AddAssignTrainer @TrainerAssignId, @BranchCode, @TrainerId, @DateOfJoining, @LeavingDate", trainerAssignId, branchCode, trainerId, dateOfJoining, leavingDate);

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

        public List<TrainerAssignView> GetTrainer(string TrainerId = null)
        {
            var trainer = new List<TrainerAssignView>();
            using (var db = new UserDBContext())
            {
                trainer = db.Database
                         .SqlQuery<TrainerAssignView>("exec USP_GETAssignedTrainer @TrainerId",
                         new SqlParameter("@TrainerId", string.IsNullOrEmpty(TrainerId) ? DBNull.Value : (object)TrainerId)).ToList();
            }

            return trainer;
        }

        public void DeleteTrainer(Int64 TrainerAssignId)
        {
            var trainer = udbc.TrainerAssign.Find(TrainerAssignId);
            if (trainer != null)
            {
                udbc.TrainerAssign.Remove(trainer);
                udbc.SaveChanges();
            }
        }

        public List<BranchDetailsView> GetBranchCode()
        {
            var branchcode = new List<BranchDetailsView>();
            using (var db = new UserDBContext())
            {
                branchcode = db.Database
                         .SqlQuery<BranchDetailsView>("exec USP_GETBranchCode").ToList();
            }
            return branchcode;
        }
    }
}