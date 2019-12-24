using AJSolutions.Areas.LMS.Models;
using AJSolutions.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AJSolutions.DAL;
using System.Data.Entity;
using System.Net;
using System.Web.Script.Serialization;

namespace AJSolutions.DAL
{
    public class LMSManager
    {
        UserDBContext udbc = new UserDBContext();
        BlobManager blobManager = new BlobManager();

        public bool AddLectureMaster(string LectureId, string LectureName, string LectureDescription, string Keywords, bool Permission, int LectureStatus, int Weightage, bool IsDelete, string SubscriberId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var lectureid = new SqlParameter("@LectureId", string.IsNullOrEmpty(LectureId) ? DBNull.Value : (object)LectureId);
                    var lecturename = new SqlParameter("@LectureName", string.IsNullOrEmpty(LectureName) ? DBNull.Value : (object)LectureName);
                    var lecturedescription = new SqlParameter("@LectureDescription", string.IsNullOrEmpty(LectureDescription) ? DBNull.Value : (object)LectureDescription);
                    var keywords = new SqlParameter("@Keywords", string.IsNullOrEmpty(Keywords) ? DBNull.Value : (object)Keywords);
                    var permission = new SqlParameter("@Permission", Permission);
                    var lecturestatus = new SqlParameter("@LectureStatus", LectureStatus);
                    var weightage = new SqlParameter("@Weightage", Weightage);
                    var isdelete = new SqlParameter("@IsDelete", IsDelete);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddLectureMaster @LectureId, @LectureName, @LectureDescription, @Keywords, @Permission, @LectureStatus, @Weightage, @IsDelete, @SubscriberId", lectureid, lecturename, lecturedescription, keywords, permission, lecturestatus, weightage, isdelete, subscriberId);

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


        public bool IsdeleteLecture(string LectureId)
        {
            using (var db = new UserDBContext())
            {
                var lecture = db.LectureMaster.Find(LectureId);
                if (lecture != null)
                {
                    lecture.IsDelete = true;
                    db.Entry(lecture).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }

            }

            return false;
        }

        public List<LectureMaster> GetLectureMaster(string SubscriberId, string LectureId = null)
        {
            var lecture = new List<LectureMaster>();
            using (var db = new UserDBContext())
            {
                lecture = db.Database
                         .SqlQuery<LectureMaster>("exec USP_GetLectureMaster @SubscriberId, @LectureId",
                         new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (Object)SubscriberId),
                         new SqlParameter("@LectureId", string.IsNullOrEmpty(LectureId) ? DBNull.Value : (Object)LectureId)).ToList();

            }

            return lecture;
        }

        public bool AddTopicMaster(string TopicId, string TopicName, string TopicDescription, string SubscriberId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var topicid = new SqlParameter("@TopicId", string.IsNullOrEmpty(TopicId) ? DBNull.Value : (object)TopicId);
                    var topicname = new SqlParameter("@TopicName", string.IsNullOrEmpty(TopicName) ? DBNull.Value : (object)TopicName);
                    var topicdescription = new SqlParameter("@TopicDescription", string.IsNullOrEmpty(TopicDescription) ? DBNull.Value : (object)TopicDescription);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddTopicMaster @TopicId, @TopicName, @TopicDescription, @SubscriberId", topicid, topicname, topicdescription, subscriberId);

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

        public List<TopicMaster> GetTopicMaster(string SubscriberId, string TopicId = null)
        {
            var topic = new List<TopicMaster>();
            using (var db = new UserDBContext())
            {
                topic = db.Database
                         .SqlQuery<TopicMaster>("exec USP_GetTopicMaster @SubscriberId, @TopicId",
                         new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (Object)SubscriberId),
                         new SqlParameter("@TopicId", string.IsNullOrEmpty(TopicId) ? DBNull.Value : (object)TopicId)).ToList();
            }

            return topic;
        }

        public bool AddTopicLecture(Int64 TopicLectureId, string LectureId, string TopicId, int SortOrder, bool LectureType, string SubscriberId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var topiclectureid = new SqlParameter("@TopicLectureId", TopicLectureId);
                    var lectureid = new SqlParameter("@LectureId", string.IsNullOrEmpty(LectureId) ? DBNull.Value : (object)LectureId);
                    var topicid = new SqlParameter("@TopicId", string.IsNullOrEmpty(TopicId) ? DBNull.Value : (object)TopicId);
                    var sortorder = new SqlParameter("@SortOrder", SortOrder);
                    var lecturetype = new SqlParameter("@LectureType", LectureType);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddTopicLecture @TopicLectureId, @LectureId, @TopicId, @SortOrder, @LectureType, @SubscriberId", topiclectureid, lectureid, topicid, sortorder, lecturetype, subscriberId);

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

        public List<TopicLecturesView> GetTopicLecture(string Id)
        {
            var topiclecture = new List<TopicLecturesView>();
            try
            {
                using (WebClient webClient = new System.Net.WebClient())
                {
                    var url = Global.WikipianUrl() + "api/value/GetTopicLectures?Id=" + Id;
                    var myString = webClient.DownloadString(url);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    TopicLecturesView[] myData = js.Deserialize<TopicLecturesView[]>(myString);
                    topiclecture = myData.ToList();
                }
            }
            catch (Exception)
            {

            }

            return topiclecture;
        }

        public List<TopicLecturesView> GetCandidateTopicLecture(string Id, string UserId)
        {
            var topiclecture = new List<TopicLecturesView>();
            try
            {
                using (WebClient webClient = new System.Net.WebClient())
                {
                    var url = Global.WikipianUrl() + "api/value/GetCandidateTopicLecture?Id=" + Id + "&UserId=" + UserId;
                    var myString = webClient.DownloadString(url);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    TopicLecturesView[] myData = js.Deserialize<TopicLecturesView[]>(myString);
                    topiclecture = myData.ToList();
                }
            }
            catch (Exception)
            {

            }

            return topiclecture;
        }

        public CourseMasterViewModel GetNavigationSettings(string Id)
        {
            var NavigationSettings = new CourseMasterViewModel();
            try
            {
                using (WebClient webClient = new System.Net.WebClient())
                {
                    var url = Global.WikipianUrl() + "api/value/GetNavigationSettings?Id=" + Id;
                    var myString = webClient.DownloadString(url);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    CourseMasterViewModel myData = js.Deserialize<CourseMasterViewModel>(myString);
                    NavigationSettings = myData;
                }
            }
            catch (Exception)
            {

            }

            return NavigationSettings;
        }


        public List<TopicLecturesView> TopicLectureForSubscriberId(string SubscriberId)
        {
            var topiclecture = new List<TopicLecturesView>();
            using (var db = new UserDBContext())
            {
                topiclecture = db.Database
                         .SqlQuery<TopicLecturesView>("exec USP_GetTopicLectureForSubscriberId @SubscriberId",
                            new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (Object)SubscriberId)).ToList();
            }

            return topiclecture;
        }

        public void RemoveTopicLectures(Int64 TopicLectureId)
        {
            var tlecture = udbc.TopicLectures.Find(TopicLectureId);
            if (tlecture != null)
            {
                udbc.TopicLectures.Remove(tlecture);
                udbc.SaveChanges();
            }
        }

        public bool AddCourseTopic(Int64 CourseTopicId, string TopicId, string CourseCode, int TopicSortOrder, bool TopicType, string UserId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var coursetopicid = new SqlParameter("@CourseTopicId", CourseTopicId);
                    var topicid = new SqlParameter("@TopicId", string.IsNullOrEmpty(TopicId) ? DBNull.Value : (object)TopicId);
                    var coursecode = new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode);
                    var topicsortorder = new SqlParameter("@TopicSortOrder", TopicSortOrder);
                    var topictype = new SqlParameter("@TopicType", TopicType);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);


                    int i = context.Database.ExecuteSqlCommand("USP_AddCourseTopic @CourseTopicId, @TopicId, @CourseCode, @TopicSortOrder, @TopicType, @UserId", coursetopicid, topicid, coursecode, topicsortorder, topictype, userId);

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

        public List<COURSETOPICSVIEW> GetCourseTopic(string CourseCode)
        {
            List<COURSETOPICSVIEW> courseTopics = new List<COURSETOPICSVIEW>();
            try
            {
                using (WebClient webClient = new System.Net.WebClient())
                {
                    var url = Global.WikipianUrl() + "api/value/GetCourseTopics?CourseCode=" + CourseCode;
                    var myString = webClient.DownloadString(url);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    COURSETOPICSVIEW[] myData = js.Deserialize<COURSETOPICSVIEW[]>(myString);
                    courseTopics = myData.ToList();
                }
            }
            catch (Exception)
            {

            }
            return courseTopics;
        }

        public void RemoveCourseTopics(Int64 CourseTopicId)
        {
            var tlecture = udbc.COURSETOPICS.Find(CourseTopicId);
            if (tlecture != null)
            {
                udbc.COURSETOPICS.Remove(tlecture);
                udbc.SaveChanges();
            }
        }

        public string GetLectureId()
        {
            UserDBContext udbc = new UserDBContext();
            string year = Convert.ToString(DateTime.UtcNow.Year).Substring(2);
            string quarter = "L1";
            int month = DateTime.UtcNow.Month;
            if (month > 3 && month <= 6)
            {
                quarter = "L2";
            }
            else if (month > 6 && month <= 9)
            {
                quarter = "L3";
            }
            else if (month > 9 && month <= 12)
            {
                quarter = "L4";
            }

            string LectureId = "LE" + year + quarter + "000001";

            var Lectures = from s in udbc.LectureMaster.Where(s => s.LectureId.Substring(0, 6) == "LE" + year + quarter)
                           orderby s.LectureId descending
                           select s.LectureId;

            var Lecture = Lectures.FirstOrDefault();

            if (Lecture != null)
            {
                string LecturePartialId = Lecture.Substring(7);
                int lastVal = Convert.ToInt32(LecturePartialId);
                lastVal = lastVal + 1;
                string suffix = string.Empty;

                for (int i = Convert.ToString(lastVal).Length; i < 6; i++)
                {
                    suffix = suffix + "0";
                }

                LectureId = Lecture.Substring(0, 6) + suffix + Convert.ToString(lastVal);
            }
            return LectureId;
        }

        public string GetTopicId()
        {
            UserDBContext udbc = new UserDBContext();
            string year = Convert.ToString(DateTime.UtcNow.Year).Substring(2);
            string quarter = "T1";
            int month = DateTime.UtcNow.Month;
            if (month > 3 && month <= 6)
            {
                quarter = "T2";
            }
            else if (month > 6 && month <= 9)
            {
                quarter = "T3";
            }
            else if (month > 9 && month <= 12)
            {
                quarter = "T4";
            }

            string TopicId = "TP" + year + quarter + "000001";

            var Topics = from s in udbc.TopicMaster.Where(s => s.TopicId.Substring(0, 6) == "TP" + year + quarter)
                         orderby s.TopicId descending
                         select s.TopicId;

            var Topic = Topics.FirstOrDefault();

            if (Topic != null)
            {
                string TopicPartialId = Topic.Substring(7);
                int lastVal = Convert.ToInt32(TopicPartialId);
                lastVal = lastVal + 1;
                string suffix = string.Empty;

                for (int i = Convert.ToString(lastVal).Length; i < 6; i++)
                {
                    suffix = suffix + "0";
                }

                TopicId = Topic.Substring(0, 6) + suffix + Convert.ToString(lastVal);
            }
            return TopicId;
        }
        //AddReview

        public bool AddReview(string UserId, string CourseCode, string Comments, DateTime CommentedOn)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var coursecode = new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode);
                    var comment = new SqlParameter("@Comments", string.IsNullOrEmpty(Comments) ? DBNull.Value : (object)Comments);
                    var commentedOn = new SqlParameter("@CommentedOn", CommentedOn);
                    int i = context.Database.ExecuteSqlCommand("SP_AddReview @UserId, @CourseCode, @Comments, @CommentedOn", userid, coursecode, comment, commentedOn);
                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return res;
        }
        //GetReview

        public List<DiscussionForumView> GetReview(string CourseCode)
        {
            var details = new List<DiscussionForumView>();
            using (var db = new UserDBContext())
            {
                details = db.Database
                    .SqlQuery<DiscussionForumView>("exec SP_GetReview @CourseCode",
                            new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode)).ToList();
            }
            return details;

        }
        //Add Reply
        public bool AddReply(Int64 CommentId, string Reply, string UserId, DateTime RepliedOn)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var commentid = new SqlParameter("@CommentId", CommentId);
                    var reply = new SqlParameter("@Reply", string.IsNullOrEmpty(Reply) ? DBNull.Value : (object)Reply);
                    var repliedOn = new SqlParameter("@RepliedOn", RepliedOn);
                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);

                    int i = context.Database.ExecuteSqlCommand("SP_AddReviewReply @CommentId, @Reply, @RepliedOn, @UserId", commentid, reply, repliedOn, userid);
                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {

            }

            return res;
        }

        public GetCountComments SPCountComments(string CourseCode)
        {

            var Count = new GetCountComments();

            using (var db = new UserDBContext())
            {
                Count = db.Database
                          .SqlQuery<GetCountComments>("EXEC SPCountComments @CourseCode ",
                          new SqlParameter("@CourseCode", string.IsNullOrEmpty(CourseCode) ? DBNull.Value : (object)CourseCode)
                          ).FirstOrDefault();
            }

            return Count;
        }

        public List<ReviewReplyView> GetReviewReply()
        {
            var details = new List<ReviewReplyView>();
            using (var db = new UserDBContext())
            {
                details = db.Database
                    .SqlQuery<ReviewReplyView>("exec SP_GetReviewReply").ToList();
            }
            return details;

        }

        public void RemoveComments(Int64 CommentId)
        {

            try
            {
                using (var context = new UserDBContext())
                {

                    var commentId = new SqlParameter("@CommentId", CommentId);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteComment @CommentId", commentId);


                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
        }

        public string uploadFile(string LectureId, HttpPostedFileBase upload)
        {
            string res = "Failure";
            if (upload != null && upload.ContentLength > 0)
            {

                var file = udbc.LectureContentUpload.Where(d => d.LectureId == LectureId).FirstOrDefault();

                if (file != null)
                {
                    Int64 FileId = file.FileId;

                    blobManager.DeleteBlob(LectureId, GetFileName(FileId).ToLower());
                    udbc.LectureContentUpload.Remove(file);
                    udbc.SaveChanges();
                }


                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");


                bool added = AddDocument(LectureId, imgFileName.ToLower(), upload.ContentType);

                if (added)
                {

                    string fileName = ReplaceFileName(LectureId);
                    blobManager.UploadBlob(LectureId.ToLower(), fileName.ToLower(), upload);

                }

                res = "Succeed";
            }


            return res;
        }

        public string GetFileName(Int64 FileId)
        {
            string fileName = null;
            var image = udbc.LectureContentUpload.Find(FileId);

            if (image != null)
            {
                fileName = "LectureContentUpload/" + image.FileId + "/" + image.FileName;
            }
            return fileName;
        }

        public string ReplaceFileName(string LectureId)
        {
            string fileName = null;
            var image = udbc.LectureContentUpload.Where(f => f.LectureId == LectureId);

            if (image != null)
            {
                string imgFileName = image.FirstOrDefault().FileName;
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");

                if (image.Count() > 0)
                    fileName = "LectureContentUpload/" + image.FirstOrDefault().FileId + "/" + imgFileName;
            }
            return fileName;
        }

        public bool AddDocument(string LectureId, string FileName, string ContentType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var lectureid = new SqlParameter("@LectureId", string.IsNullOrEmpty(LectureId) ? DBNull.Value : (object)LectureId);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);

                    int i = context.Database.ExecuteSqlCommand("USP_AddContent @LectureId, @FileName, @ContentType",
                        lectureid, fileName, contentType);

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

        public bool UpdateDocument(Int64 FileId, string LectureId, string FileName, string ContentType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var fileId = new SqlParameter("@FileId", FileId);
                    var lectureid = new SqlParameter("@LectureId", string.IsNullOrEmpty(LectureId) ? DBNull.Value : (object)LectureId);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);

                    int i = context.Database.ExecuteSqlCommand("USP_UpdateContent @FileId, @LectureId, @FileName, @ContentType",
                       fileId, lectureid, fileName, contentType);

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

        public bool DeleteDocument(Int64 FileId)
        {
            bool res = false;

            try
            {
                var file = udbc.LectureContentUpload.Find(FileId);
                if (file.FileName != null)
                {
                    blobManager.DeleteBlob(file.LectureId, GetFileName(FileId));
                }
                using (var context = new UserDBContext())
                {
                    var fileId = new SqlParameter("@FileId", FileId);


                    int i = context.Database.ExecuteSqlCommand("USP_DeleteContent @FileId", fileId);

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

        public List<CourseMasterViewModel> GetUserReckWikiCourseSubscriptionDetails(string UserId)
        {
            var courseMaster = new List<CourseMasterViewModel>();
            try
            {
                using (WebClient webClient = new System.Net.WebClient())
                {
                    var url = Global.WikipianUrl() + "api/value/GetUserReckWikiCourseSubscriptionDetails?RefUserId=" + UserId;
                    var myString = webClient.DownloadString(url);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    CourseMasterViewModel[] myData = js.Deserialize<CourseMasterViewModel[]>(myString);
                    courseMaster = myData.ToList();
                }
            }
            catch (Exception)
            {

            }

            return courseMaster;
        }

        public List<CourseMasterViewModel> GetLMSCourseMasters(string UserId)
        {
            List<CourseMasterViewModel> LMSCourse = new List<CourseMasterViewModel>();
            try
            {
                using (WebClient webClient = new System.Net.WebClient())
                {
                    var url = Global.WikipianUrl() + "api/value/GetCourses?SubscriberId=" + UserId;
                    var myString = webClient.DownloadString(url);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    CourseMasterViewModel[] myData = js.Deserialize<CourseMasterViewModel[]>(myString);
                    LMSCourse = myData.ToList();
                }
            }
            catch (Exception)
            {

            }
            return LMSCourse;
        }
    }
}