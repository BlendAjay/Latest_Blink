using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AJSolutions.Areas.LMS.Models;
using AJSolutions.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using AJSolutions.Models;

namespace AJSolutions.Areas.LMS.Controllers
{
    public class CourseController : Controller
    {
        LMSManager lms = new LMSManager();
        CMSManager cms = new CMSManager();
        AdminManager admin = new AdminManager();
        Generic generic = new Generic();
        UserDBContext db = new UserDBContext();
        //GET: LMS/Course
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult LectureMaster(string Id, string LectureId, string Keywords, bool status = false, string UserAction = "Add", Int64 FileId = 0)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.UserAction = UserAction;
            ViewBag.Result = "Failed";
            ViewBag.Keywords = Keywords;
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            ViewData["LectureMaster"] = lms.GetLectureMaster(userDetails.SubscriberId, null);
            
            LectureContentUpload Content = new LectureContentUpload();

            if (Id != null && Id != string.Empty)
            {
                var contents = db.LectureContentUpload.Find(FileId);

                Content.LectureId = contents.LectureId;
                Content.FileId = contents.FileId;
                Content.FileName = contents.FileName;
                Content.ContentType = contents.ContentType;
            }
            ViewData["Content"] = Content;
            if (UserAction == "Delete" && !string.IsNullOrEmpty(LectureId))
            {
                lms.IsdeleteLecture(LectureId);
                return View();
            }

            return View(db.LectureMaster.Find(LectureId));
           
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult LectureMaster(LectureMaster LectureMaster, string Name, HttpPostedFileBase uploadPhoto,  string Keywords)
        {
         //   HttpFileCollectionBase files = Request.Files;  

            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            LectureMaster.SubscriberId = userDetails.SubscriberId;
            if (string.IsNullOrEmpty(LectureMaster.LectureId))
            {
                LectureMaster.LectureId = lms.GetLectureId();
            }
            if (string.IsNullOrEmpty(LectureMaster.SubscriberId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            else
            {


                bool result = lms.AddLectureMaster(LectureMaster.LectureId, LectureMaster.LectureName, LectureMaster.LectureDescription, LectureMaster.Keywords, LectureMaster.Permission, LectureMaster.LectureStatus, LectureMaster.Weightage, LectureMaster.IsDelete, LectureMaster.SubscriberId);

                if (result == true)
                {
                    lms.uploadFile(LectureMaster.LectureId,  uploadPhoto);
                }
                return RedirectToAction("LectureMaster", "Course", new { area = "LMS", status = result });
            }
        }
        //[Authorize(Roles = "Admin,Candidate")]
        public ActionResult LectureDetails(string Id, string LectureId, string UserAction = "Add")
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewData["LectureMaster"] = lms.GetLectureMaster(userDetails.SubscriberId, Id).FirstOrDefault();

            ViewData["Contents"] = db.LectureContentUpload.Where(u => u.LectureId == Id).ToList();
            

            return View(db.LectureMaster.Find(Id));
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult TopicDetails(string Id, string TopicId, string UserAction = "Add")
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewBag.UserAction = UserAction;
            ViewData["TopicMaster"] = lms.GetTopicMaster(userDetails.SubscriberId, Id).FirstOrDefault();
            return View(db.TopicMaster.Find(TopicId));
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult TopicMaster(string TopicId, bool status = false, string UserAction = "Add", Int64 TopicLectureId = 0)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewBag.UserAction = UserAction;
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            ViewData["TopicMaster"] = lms.GetTopicMaster(userDetails.SubscriberId, null);
            return View(db.TopicMaster.Find(TopicId));
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult TopicMaster(TopicMaster TopicMaster)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            TopicMaster.SubscriberId = userDetails.SubscriberId;
            if (string.IsNullOrEmpty(TopicMaster.TopicId))
            {
                TopicMaster.TopicId = lms.GetTopicId();
            }
            if (string.IsNullOrEmpty(TopicMaster.SubscriberId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            else
            {
                bool result = lms.AddTopicMaster(TopicMaster.TopicId, TopicMaster.TopicName, TopicMaster.TopicDescription, TopicMaster.SubscriberId);
                return RedirectToAction("TopicMaster", "Course", new { area = "LMS", status = result });
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult AddTopicLecture(string Id, string LectureId, bool status = false, string UserAction = "Add", Int64 TopicLectureId = 0)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewBag.UserAction = UserAction;
            ViewBag.LectureId = LectureId;
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            if (UserAction == "Delete" && TopicLectureId != 0)
            {
                lms.RemoveTopicLectures(TopicLectureId);
                return RedirectToAction("AddTopicLecture", "Course", new { area = "LMS" });
            }
            var topiclecturelist = lms.TopicLectureForSubscriberId(userDetails.SubscriberId).Where(t => t.TopicId == Id).ToList();
            ViewData["TopicLectures"] = topiclecturelist;
            ViewData["TopicMaster"] = lms.GetTopicMaster(userDetails.SubscriberId, Id).FirstOrDefault();
            if (topiclecturelist.Count == 0)
            {
                ViewData["LectureMaster"] = lms.GetLectureMaster(userDetails.SubscriberId, null).ToList();
            }
            else
            {
                //ViewData["LectureMaster"] = lms.GetLectureMaster(userDetails.SubscriberId, null).Select(l => l.LectureId).Except(topiclecturelist.Select(t => t.LectureId).ToList());
               var lectures = lms.GetLectureMaster(userDetails.SubscriberId, null).Where(l => !topiclecturelist.Select(t => t.LectureId).Contains(l.LectureId)).ToList();
               ViewData["LectureMaster"] = lectures;
                //return View(db.TopicLectures.Find(TopicLectureId));
            }
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult AddTopicLecture(TopicLectures TopicLectures, string LectureId)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            TopicLectures.SubscriberId = userDetails.SubscriberId;
            LectureId = LectureId.TrimEnd(',');

            string[] lectureId = LectureId.Trim().Split(',');


            if (string.IsNullOrEmpty(TopicLectures.SubscriberId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            else
            {
                bool result = false;
                for (int i = 0; i < lectureId.Length; i++)
                {
                    result = lms.AddTopicLecture(TopicLectures.TopicLectureId, lectureId[i].ToString(), TopicLectures.TopicId, TopicLectures.SortOrder, TopicLectures.LectureType, TopicLectures.SubscriberId);
                }

                return RedirectToAction("AddTopicLecture", "Course", new { area = "LMS", status = result });
            }
        }


        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult AddCourseTopic(string Id, string TopicId, string CourseCode, bool status = false, string UserAction = "Add", Int64 CourseTopicId = 0)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewBag.UserAction = UserAction;
            ViewBag.TopicId = TopicId;
            ViewBag.Result = "Failed";
            if (status == true)
            {
                ViewBag.Result = "Succeeded";
            }
            if (UserAction == "Delete" && CourseTopicId != 0)
            {
                lms.RemoveCourseTopics(CourseTopicId);
                return RedirectToAction("AddCourseTopic", "Course", new { area = "LMS" });
            }

            var coursetopic = lms.GetCourseTopic(CourseCode).Where(u => u.UserId == userDetails.UserId).ToList();
            ViewData["COURSETOPICS"] = coursetopic;
            ViewData["CourseMaster"] = db.CourseMaster.Find(Id);
            if (coursetopic.Count == 0)
            {
                ViewData["TopicMaster"] = lms.GetTopicMaster(userDetails.SubscriberId, null).ToList();
            }
            else
            {
                var topics = lms.GetTopicMaster(userDetails.SubscriberId, null).Where(l => !coursetopic.Select(t => t.TopicId).Contains(l.TopicId)).ToList();
                ViewData["TopicMaster"] = topics;
            }
            //ViewData["TopicMaster"] = lms.GetTopicMaster(userDetails.SubscriberId, null)
                                                                                        //.Select(l => l.TopicId)
                                                                                        //.Except(coursetopic.Select(t => t.TopicId).ToList());
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult AddCourseTopic(COURSETOPICS COURSETOPICS, string TopicId)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            TopicId = TopicId.TrimEnd(',');

            string[] topicid = TopicId.Trim().Split(',');

            bool result = false;
            for (int i = 0; i < topicid.Length; i++)
            {
                result = lms.AddCourseTopic(COURSETOPICS.CourseTopicId, topicid[i].ToString(), COURSETOPICS.CourseCode, COURSETOPICS.TopicSortOrder, COURSETOPICS.TopicType, userDetails.UserId);
            }

            return RedirectToAction("AddCourseTopic", "Course", new { area = "LMS", status = result });

        }

        public ActionResult Lectures(string UserAction, string LectureId)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["LectureMaster"] = lms.GetLectureMaster(userDetails.SubscriberId, null);
            if (UserAction == "Delete" && !string.IsNullOrEmpty(LectureId))
            {
                lms.IsdeleteLecture(LectureId);
                return RedirectToAction("Lectures", "Course", new { area = "LMS" });
            }
            return View();
        }

        public ActionResult Topics()
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["TopicMaster"] = lms.GetTopicMaster(userDetails.SubscriberId, null);
            return View();
        }
    }
}