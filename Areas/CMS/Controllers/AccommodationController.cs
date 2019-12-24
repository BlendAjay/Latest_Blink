using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AJSolutions.Models;
using AJSolutions.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PagedList;
using System.Globalization;

namespace AJSolutions.Areas.CMS.Controllers
{
    public class AccommodationController : Controller
    {
        HMSManager hostalmgr = new HMSManager();
        EMSManager ems = new EMSManager();
        Generic generic = new Generic();
        TMSManager tms = new TMSManager();
        CMSManager cms = new CMSManager();
        AdminManager admin = new AdminManager();
        UserDBContext userContext = new UserDBContext();
        public ActionResult Index()
        {
            return View();
        }

        //GET: method for checkin Student batch wise list  
        [HttpGet]
        public ActionResult CandidateCheckIn(string sortOrder, string CourseCode, int? page, Int64 BatchId = 0, string FilterName = "", int PageSize = 10,bool status = false)
        {
            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetails.SubscriberId;
            ViewData["UserProfile"] = userDetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            PopulateCourse(userDetails.SubscriberId, CourseCode);
            PopulateBatchByCourse(userDetails.SubscriberId, CourseCode, BatchId, false);
            //PopulateAssignedCourse(userDetails.SubscriberId, CourseCode);
            //PopulateAssignedCourseBatch(userDetails.SubscriberId, CourseCode, BatchId);
            ViewBag.TBatchId = BatchId;
            ViewBag.Course = CourseCode;
            ViewBag.Batch = BatchId;
            List<CandidateCourseDetailsView> Details = hostalmgr.GetBatchWiseStudent(CourseCode, BatchId).Where(c => c.CheckInDate == null).ToList();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = sortOrder == "Name" ? "Name_desc" : "Name";

            ViewBag.Name = FilterName;
            ViewBag.SaveStatus = status;
            //Apply filter
            if (!String.IsNullOrEmpty(FilterName))
            {
                Details = Details.Where(s => s.Name.ToLower().Contains(FilterName.ToLower())).ToList();
            }

            //Apply sorting
            if (Details.Count != 0)
            {
                switch (sortOrder)
                {
                    case "Name":
                        Details = Details.OrderBy(c => c.Name).ToList();
                        break;
                    case "Name_desc":
                        Details = Details.OrderByDescending(c => c.Name).ToList();
                        break;
                    default:
                        Details = Details.OrderBy(c => c.Name).ToList();
                        break;
                }
            }

            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);

            return View(Details.ToPagedList(pageNumber, pageSize));
        }

        //POST: method for candidate checkins
        [HttpPost]
        public ActionResult CandidateCheckIn(string id, string[] userId, string[] CheckInDate, string[] Users, Int64 TBatchId = 0)
        {
            id = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(id);
            bool result = false;
            for (int i = 0; i < Users.Length; i++)
            {
                for (int j = 0; j < userId.Length; j++)
                {
                    if (userId[j] == Users[i])
                    {
                        //DateTime? CinDateTime = null;
                        //if (!String.IsNullOrEmpty(CheckInDate[i]))
                        //{
                        //    CinDateTime = DateTime.ParseExact(CheckInDate[i], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        //}
                        if (!String.IsNullOrEmpty(CheckInDate[i]))
                        {
                            DateTime? CinDateTime = Convert.ToDateTime(CheckInDate[i]);
                            result = hostalmgr.AddCheckIn(userId[j], CinDateTime, TBatchId, userDetails.UserId, DateTime.UtcNow);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return RedirectToAction("CandidateCheckIn", "Accommodation", new { area = "CMS", status = result });
        }

        //GET: method for checkincheckout details
        [HttpGet]
        public ActionResult CandidateCheckOut(string sortOrder, string FilterName, string CourseCode, string status, int? page, Int64 BatchId = 0, int PageSize = 10)
        {
            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetails.SubscriberId;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            PopulateCourse(userDetails.SubscriberId, CourseCode);
            PopulateBatchByCourse(userDetails.SubscriberId, CourseCode, BatchId, false);
            //PopulateAssignedCourse(userDetails.SubscriberId, CourseCode);
            //PopulateAssignedCourseBatch(userDetails.SubscriberId, CourseCode, BatchId);
            ViewBag.TBatchId = BatchId;

            ViewBag.Course = CourseCode;
            ViewBag.Batch = BatchId;
            ViewBag.status = status;
            List<CheckInCheckOutView> Details = hostalmgr.GetCandidateCheckIn(userDetails.SubscriberId).Where(c => c.CourseCode == CourseCode && c.BatchId == BatchId && c.CheckOutDate == null).ToList();

            if (UserId != userDetails.SubscriberId && userDetails.DepartmentId != "ADI")
                Details = Details.Where(c => c.WardenId == UserId).ToList();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = sortOrder == "Name" ? "Name_desc" : "Name";

            ViewBag.Name = FilterName;

            //Apply filter
            if (!String.IsNullOrEmpty(FilterName))
            {
                Details = Details.Where(s => s.Name.ToLower().Contains(FilterName.ToLower())).ToList();
            }

            //Apply sorting
            if (Details.Count != 0)
            {
                switch (sortOrder)
                {
                    case "Name":
                        Details = Details.OrderBy(c => c.Name).ToList();
                        break;
                    case "Name_desc":
                        Details = Details.OrderByDescending(c => c.Name).ToList();
                        break;
                    default:
                        Details = Details.OrderBy(c => c.Name).ToList();
                        break;
                }
            }

            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);

            return View(Details.ToPagedList(pageNumber, pageSize));

        }

        //POST: method for candidate checkouts
        [HttpPost]
        public ActionResult CandidateCheckOut(string[] userId, string[] CheckOutDate, string[] Users, Int64 TBatchId = 0)
        {
            string id = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(id);
            //UserId = UserId.TrimEnd(',');
            //string[] userid = UserId.Trim().Split(',');
            bool result = false;
            for (int i = 0; i < Users.Length; i++)
            {
                for (int j = 0; j < userId.Length; j++)
                {
                    if (userId[j] == Users[i])
                    {
                        //DateTime? CinDateTime = null;
                        //if (!String.IsNullOrEmpty(CheckInDate[i]))
                        //{
                        //    CinDateTime = DateTime.ParseExact(CheckInDate[i], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        //}
                        if (!String.IsNullOrEmpty(CheckOutDate[i]))
                        {
                            DateTime? CoutDateTime = Convert.ToDateTime(CheckOutDate[i]);
                            result = hostalmgr.AddCheckOut(userId[j], CoutDateTime, TBatchId, userDetails.UserId, DateTime.UtcNow);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return RedirectToAction("CandidateCheckOut", "Accommodation", new { area = "CMS", status = result });
        }

        // check in check out for multiple candidates
        public ActionResult BulkCheckInCheckOut(string CourseCode, Int64 BatchId = 0)
        {
            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(UserId);
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            PopulateAssignedCourse(userDetails.SubscriberId, CourseCode);
            PopulateAssignedCourseBatch(userDetails.SubscriberId, CourseCode, BatchId);
            List<CandidateCourseDetailsView> Details = hostalmgr.GetBatchWiseStudent(CourseCode, BatchId);

            return View(Details);
        }

        //GET: method for checkout details
        [HttpGet]
        public ActionResult AccomodationHistory(string sortOrder, string FilterName, String CourseCode, int? page, Int64 BatchId = 0, int PageSize = 10)
        {
            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userDetails.SubscriberId;
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            PopulateAssignedCourse(userDetails.SubscriberId, CourseCode);
            PopulateAssignedCourseBatch(userDetails.SubscriberId, CourseCode, BatchId);
            ViewBag.Course = CourseCode;
            ViewBag.Batch = BatchId;
            List<CheckInCheckOutView> Details = hostalmgr.GetCandidateCheckIn(userDetails.SubscriberId).Where(c => c.CourseCode == CourseCode && c.BatchId == BatchId).ToList();

            if (UserId != userDetails.SubscriberId && userDetails.DepartmentId != "ADI")
                Details = Details.Where(c => c.WardenId == UserId).ToList();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.CheckInSortParam = sortOrder == "CheckIn" ? "CheckIn_desc" : "CheckIn";
            ViewBag.CheckOutSortParam = sortOrder == "CheckOut" ? "CheckOut_desc" : "CheckOut";

            ViewBag.Name = FilterName;

            //Apply filter
            if (!String.IsNullOrEmpty(FilterName))
            {
                Details = Details.Where(s => s.Name.ToLower().Contains(FilterName.ToLower())).ToList();
            }

            //Apply sorting
            if (Details.Count != 0)
            {
                switch (sortOrder)
                {
                    case "Name":
                        Details = Details.OrderBy(c => c.Name).ToList();
                        break;
                    case "Name_desc":
                        Details = Details.OrderByDescending(c => c.Name).ToList();
                        break;
                    case "CheckIn":
                        Details = Details.OrderBy(c => c.CheckInDate).ToList();
                        break;
                    case "CheckIn_desc":
                        Details = Details.OrderByDescending(c => c.CheckInDate).ToList();
                        break;
                    case "CheckOut":
                        Details = Details.OrderBy(c => c.CheckOutDate).ToList();
                        break;
                    case "CheckOut_desc":
                        Details = Details.OrderByDescending(c => c.CheckOutDate).ToList();
                        break;
                    default:
                        Details = Details.OrderBy(c => c.Name).ToList();
                        break;
                }
            }
            else
            {

            }

            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);

            return View(Details.ToPagedList(pageNumber, pageSize));
        }

        //method for papulate course
        private void PopulateAssignedCourse(string SubscriberId, object selectedValue = null)
        {
            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(UserId);

            var query = tms.GetCourseBatches(SubscriberId);
            if (UserId != userDetails.SubscriberId && userDetails.DepartmentId != "ADI")
                query = query.Where(c => c.WardenId == UserId).ToList();
            SelectList CourseCode = new SelectList(query.Select(c => new { c.CourseCode, c.CourseName }).Distinct(), "CourseCode", "CourseName", selectedValue);
            ViewBag.CourseCode = CourseCode;
        }

        //method for papulate batch
        private void PopulateAssignedCourseBatch(string SubscriberId, string CourseCode = null, object selectedValue = null)
        {
            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(UserId);

            var query = tms.GetCourseBatches(SubscriberId);
            if (!string.IsNullOrEmpty(CourseCode))
                query = query.Where(b => b.CourseCode == CourseCode && b.AttendenceNeeded == true).ToList();
            if (UserId != userDetails.SubscriberId && userDetails.DepartmentId != "ADI")
                query = query.Where(b => b.WardenId == UserId).ToList();

            SelectList BatchId = new SelectList(query, "BatchId", "BatchName", selectedValue);
            ViewBag.BatchId = BatchId;
        }

        [HttpPost]
        public ActionResult GetBatch(string CourseCode)
        {
            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(UserId);

            List<SelectListItem> BatchId = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(CourseCode))
            {
                List<CourseBatch> Batches = (from b in userContext.CourseBatch
                                             join c in userContext.CourseMaster
                                             on b.CourseCode equals c.CourseCode
                                             where b.CourseCode == CourseCode && b.AccomondationNeeded == true
                                             select b).ToList();

                if (UserId != userDetails.SubscriberId && userDetails.DepartmentId != "ADI")
                    Batches = Batches.Where(b => b.WardenId == UserId).ToList();

                Batches.ForEach(x =>
                {
                    BatchId.Add(new SelectListItem { Text = x.BatchName, Value = x.BatchId.ToString() });
                });
            }
            return Json(BatchId, JsonRequestBehavior.AllowGet);
        }
        private void PopulatePaging(object selectedValue = null)
        {
            var PageList = generic.GetPaging();
            ViewBag.PageSize = new SelectList(PageList, "PageSize", "PageSize", selectedValue);
        }

        private void PopulateCourse(string SubscriberId, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetCourseBatches(SubscriberId);
            SelectList CourseCode = new SelectList(query.Select(c => new { c.CourseCode, c.CourseName }).Distinct(), "CourseCode", "CourseName", selectedValue);
            ViewBag.CourseCode = CourseCode;
            //var query = tms.GetCourseDetails(SubscriberId).OrderBy(c => c.CourseName).ToList();
            //SelectList CourseCode = new SelectList(query, "CourseCode", "CourseName", selectedValue);
            //ViewBag.CourseCode = CourseCode;
        }

        private void PopulateBatchByCourse(string SubscriberId = null, string CourseCode = null, object selectedValue = null, bool DateFilter = false)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetBatches(SubscriberId, CourseCode);
            if (DateFilter)
                query = query.Where(b => b.ToDate >= DateTime.UtcNow).ToList();

            SelectList BatchId = new SelectList(query, "BatchId", "BatchName", selectedValue);
            ViewBag.BatchId = BatchId;
        }


        ////method for papulate course
        //private void PopulateCourse(string sid, object selectedValue = null)
        //{

        //    var query = tms.GetStudentCourseDetails(sid);
        //    SelectList CourseCode = new SelectList(query, "CourseCode", "CourseName", selectedValue);
        //    ViewBag.CourseCode = CourseCode;
        //}

        ////method for papulate batch
        //private void PopulateBatch(string CourseCode = null, object selectedValue = null)
        //{
        //    TMSManager tms = new TMSManager();
        //    var query = tms.GetBatches(CourseCode);
        //    SelectList BatchId = new SelectList(query, "BatchId", "BatchName", selectedValue);
        //    ViewBag.BatchId = BatchId;
        //}

    }
}
