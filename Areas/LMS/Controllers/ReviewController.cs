using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AJSolutions.Models;
using AJSolutions.DAL;
using AJSolutions.Areas.LMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
namespace AJSolutions.Areas.LMS.Controllers
{
    public class ReviewController : Controller
    {
        Generic generic = new Generic();
        AdminManager admin = new AdminManager();
        TMSManager tms = new TMSManager();
        private UserDBContext db = new UserDBContext();
        LMSManager lms = new LMSManager();
        EMSManager ems = new EMSManager();
        CMSManager cms = new CMSManager();
        // GET: LMS/Review
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Employee,Candidate")]
        public ActionResult Discussion(string CourseCode)
        {
            ViewData["CourseDetail"] = admin.GetCourseMasterDetails(CourseCode);
            string userId = User.Identity.GetUserId();

            var userdetails = generic.GetUserDetail(userId);
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewData["Reply"] = lms.GetReviewReply();
            ViewBag.CommentsCount = lms.SPCountComments(CourseCode).TOTALCOMMENTS;
            List<DiscussionForumView> Details = lms.GetReview(CourseCode);
            return View(Details);

        }

        [HttpPost]
        public ActionResult Discussion(string CourseCode, string Comments, string Reply, Int64 CommentId = 0)
        {
            string userId = User.Identity.GetUserId();
            var result = false;
            if (!string.IsNullOrEmpty(CourseCode) && !string.IsNullOrEmpty(Comments))
                result = lms.AddReview(userId, CourseCode, Comments, DateTime.Now);

            if (CommentId != 0 && !string.IsNullOrEmpty(Reply))
                lms.AddReply(CommentId, Reply, userId, DateTime.Now);

            return RedirectToAction("Discussion", "Review", new { CourseCode = CourseCode, area = "LMS", status = result });
        }


        public ActionResult RemoveReplies(string CC, Int64 RId)
        {
            ReviewReply review = db.ReviewReply.Find(RId);
            if (review != null)
            {
                db.ReviewReply.Remove(review);
                db.SaveChanges();
            }

            return RedirectToAction("Discussion", "Review", new { CourseCode = CC, area = "LMS" });
        }

        public ActionResult RemoveComment(string CC, Int64 CId)
        {
            lms.RemoveComments(CId);

            return RedirectToAction("Discussion", "Review", new { CourseCode = CC, area = "LMS" });
        }


    }
}
