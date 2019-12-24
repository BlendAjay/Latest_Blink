using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using AJSolutions.DAL;
using System.IO;
using AJSolutions.Models;

namespace AJSolutions.Controllers
{
    public class FileController : Controller
    {
        UserDBContext db = new UserDBContext();
        BlobManager blobManager = new BlobManager();
        LMSManager lmsMgr = new LMSManager();
        CMSManager cmsMgr = new CMSManager();
        TMSManager tms = new TMSManager();
        AdminManager admin = new AdminManager();
        Generic genric = new Generic();
        //        CorporateJobPost corporateJobPost = new CorporateJobPost();
        //        JobSeekerManager jobSeekerManager = new JobSeekerManager();
        //        CorporateCompany company = new CorporateCompany();
        //        CorporatePersonnel corporatePersonnel = new CorporatePersonnel();
        //        CorporateJobPost jobPost = new CorporateJobPost();
        //        JE5StateCriteria stateCriteria = new JE5StateCriteria();
        //        JobCycleManager jobCycleManager = new JobCycleManager();
        //        DigiLockerManager DGLManager = new DigiLockerManager();
        //        PlanManager planManager = new PlanManager();
        //        // GET: File
        //        public ActionResult Index(int id)
        //        {
        //            var fileToRetrieve = db.JobSeekerPhotoFile.Find(id);


        //            return File(fileToRetrieve.Content, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }



        //        [EncryptedActionParameter]
        //        public ActionResult DownloadProfile(string id, string CorporateId, string IsNonCountable = "false")
        //        {
        //            var fileToRetrieve = db.JobSeekerPhotoFile.Find(Convert.ToInt32(id));

        //            if (IsNonCountable == "false")
        //            {
        //                var planStatus = planManager.GetSubscriptionStatus(CorporateId, true, Convert.ToInt32(PlanFeatures.ExperiencedProfile)); //11 to check experienced profile
        //                if (!Convert.ToBoolean(planStatus["UserAccess"]))
        //                    return RedirectToAction("JobSeekerDetail", "SearchJob", new { JobSeekerId = fileToRetrieve.JobSeekerId, UpgradePlan = "Required" });


        //                string updatedBy = User.Identity.GetUserId();

        //                jobCycleManager.UpdateProfileUnlocked(CorporateId, fileToRetrieve.JobSeekerId, updatedBy);
        //            }

        //            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.JobSeekerId, jobSeekerManager.GetFileName(fileToRetrieve.JobSeekerId, Convert.ToInt32(Purpose.CV)));

        //            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }

        //        public ActionResult Identity(int id)
        //        {
        //            var fileToRetrieve = db.JobSeekerPhotoFile.Find(id);

        //            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.JobSeekerId, jobSeekerManager.GetFileName(fileToRetrieve.JobSeekerId, Convert.ToInt32(Purpose.ID)));

        //            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }

        //        public ActionResult Photo(int id)
        //        {
        //            var fileToRetrieve = db.JobSeekerPhotoFile.Find(id);

        //            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.JobSeekerId, jobSeekerManager.GetFileName(fileToRetrieve.JobSeekerId, Convert.ToInt32(Purpose.PHOTO)));

        //            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }

        //        public ActionResult Corporate(int id)
        //        {
        //            var fileToRetrieve = db.T5CorporateFile.Find(id);

        //            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.CorporateId, corporatePersonnel.GetFileName(fileToRetrieve.CorporateId, Convert.ToInt32(Purpose.PHOTO)));

        //            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }

        //        public ActionResult CorporateLogo(int id)
        //        {
        //            var fileToRetrieve = db.T5CorporateLogoFile.Find(id);

        //            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.CorporateId, company.GetFileName(fileToRetrieve.CorporateId, Convert.ToInt32(Purpose.LOGO)));

        //            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }

        //        public ActionResult CorporateBanner(int id)
        //        {
        //            var fileToRetrieve = db.T5CorporateLogoFile.Find(id);

        //            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.CorporateId, company.GetFileName(fileToRetrieve.CorporateId, Convert.ToInt32(Purpose.Banner)));

        //            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }

        //        public ActionResult CorporateFavicon(int id)
        //        {
        //            var fileToRetrieve = db.T5CorporateLogoFile.Find(id);

        //            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.CorporateId, company.GetFileName(fileToRetrieve.CorporateId, Convert.ToInt32(Purpose.Favicon)));

        //            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }

        //        public ActionResult JobPost(int id)
        //        {
        //            var fileToRetrieve = db.T5CorporateCompanyLogo.Find(id);

        //            string CorporateId = db.T5CorporateJobPost.Find(fileToRetrieve.JobPostId).CorporateId;

        //            byte[] file = blobManager.DownloadBlobClient(CorporateId, jobPost.GetFileName(CorporateId, fileToRetrieve.JobPostId, Convert.ToInt32(Purpose.LOGO)));


        //            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }

        //        public ActionResult Banner(int id)
        //        {
        //            var fileToRetrieve = db.T5CorporateCompanyLogo.Find(id);

        //            string CorporateId = db.T5CorporateJobPost.Find(fileToRetrieve.JobPostId).CorporateId;

        //            byte[] file = blobManager.DownloadBlobClient(CorporateId, jobPost.GetFileName(CorporateId, fileToRetrieve.JobPostId, Convert.ToInt32(Purpose.Banner)));


        //            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }

        //        public ActionResult SmallBanner(int id)
        //        {
        //            var fileToRetrieve = db.T5CorporateCompanyLogo.Find(id);

        //            string CorporateId = db.T5CorporateJobPost.Find(fileToRetrieve.JobPostId).CorporateId;

        //            byte[] file = blobManager.DownloadBlobClient(CorporateId, jobPost.GetFileName(CorporateId, fileToRetrieve.JobPostId, Convert.ToInt32(Purpose.SmallBanner)));

        //            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }

        //        public ActionResult JobStateAttachment(int id)
        //        {
        //            var fileToRetrieve = db.T5JobApplicationAttachment.Find(id);

        //            var CorporateDetail = (from a in db.T5JobApplicationHistory
        //                                   join j in db.T5CorporateJobPost
        //                                   on a.JobPostId equals j.JobPostId
        //                                   where a.JobCycleId == fileToRetrieve.JobCycleId
        //                                   select new { a, j }).FirstOrDefault();

        //            string CorporateId = CorporateDetail.j.CorporateId;
        //            string JobSeekerId = CorporateDetail.a.JobSeekerId;

        //            string JobPostId = CorporateDetail.a.JobPostId;


        //            byte[] file = blobManager.DownloadBlobClient(CorporateId, jobCycleManager.GetFileName(JobPostId, JobSeekerId, fileToRetrieve.JobCycleId, Convert.ToInt32(Purpose.ATTACHMENT)));


        //            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }

        //        public ActionResult JobCategoryAttachment(int id)
        //        {
        //            var fileToRetrieve = db.T5CorporateJobCategoryAttachment.Find(id);

        //            string CorporateId = db.T5CorporateJobCategory.Find(fileToRetrieve.CategoryId).CorporateId;

        //            byte[] file = blobManager.DownloadBlobClient(CorporateId, stateCriteria.GetFileName(fileToRetrieve.CategoryId, Convert.ToInt32(Purpose.ATTACHMENT)));

        //            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }

        //        public ActionResult HirerPhoto(int id)
        //        {
        //            var fileToRetrieve = db.T5CorporateHirerPhotoFile.Find(id);

        //            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.UserId, corporatePersonnel.GetPhotoFileName(fileToRetrieve.UserId, Convert.ToInt32(Purpose.PHOTO)));

        //            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        //        }


        //        [EncryptedActionParameter]
        public ActionResult GetLectureContent(Int64 id)
        {
            var fileToRetrieve = db.LectureContentUpload.Find(id);

            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.LectureId.ToLower(), lmsMgr.GetFileName(fileToRetrieve.FileId).ToLower());

            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        }


        public string Urls(Int64 id)
        {
            var fileToRetrieve = db.LectureContentUpload.Find(id);

            string fileurl = blobManager.DownloadPublicBlob(fileToRetrieve.LectureId.ToLower(), lmsMgr.GetFileName(fileToRetrieve.FileId).ToLower());

            return fileurl;
        }

        public ActionResult JobOrderAttachment(int id)
        {
            var fileToRetrieve = db.JobOrderAttachment.Find(id);

            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.JobOrderNumber.ToLower(), cmsMgr.GetFileName(fileToRetrieve.FileId).ToLower());

            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        }

        public ActionResult TaskAttachment(int id)
        {
            var fileToRetrieve = db.TaskAttachment.Find(id);

            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.TaskId.ToLower(), admin.GetFileName(fileToRetrieve.FileId).ToLower());

            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        }

        public ActionResult TaskFinalAttachment(int id)
        {
            var fileToRetrieve = db.TaskFinalAttachment.Find(id);

            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.TaskId.ToLower(), cmsMgr.GetTaskFileName(fileToRetrieve.FileId).ToLower());

            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        }

        public ActionResult TrainingFinalAttachment(int id)
        {
            var fileToRetrieve = db.TrainingScheduleFinalAttachment.Find(id);

            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.TrainingId.ToLower(), tms.GetTrainingFileName(fileToRetrieve.FileId).ToLower());

            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        }

        public ActionResult JobOrderFinalAttachment(int id)
        {
            var fileToRetrieve = db.JobOrderFinalAttachment.Find(id);

            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.JobOrderNumber.ToLower(), cmsMgr.GetJobOrderFileName(fileToRetrieve.FileId).ToLower());

            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        }

        public ActionResult InvoiceAttachment(int id)
        {
            var fileToRetrieve = db.InVoiceAttachment.Find(id);

            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.ReferenceId.ToLower(), admin.GetFileInvName(fileToRetrieve.FileId).ToLower());

            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        }

        public ActionResult TrainingScheduleAttachment(int id)
        {
            var fileToRetrieve = db.TrainingScheduleAttachment.Find(id);

            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.TrainingId.ToLower(), admin.GetTrainingscheduleFile(fileToRetrieve.FileId).ToLower());

            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        }

        public ActionResult CompanyLogo(int id)
        {
            var fileToRetrieve = db.AdminLogoFile.Find(id);

            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.CorporateId.ToLower(), cmsMgr.GetFileNameCompanyLogo(fileToRetrieve.FileId).ToLower());

            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        }

        public ActionResult SendLetter(Int64 Id)
        {
            if (Id != 0)
            {
                var fileToRetrieve = db.SendLetter.Find(Id);
                string Name = fileToRetrieve.FileName;
                Name = Name.Replace("'", "_");
                Name = Name.Replace(' ', '_');
                string fileName = "Letters/" + fileToRetrieve.UserId + "/" + fileToRetrieve.FileId + "/" + Name + ".pdf";
                byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.CorporateId.ToLower(), fileName.ToLower());
                Response.Clear();
                MemoryStream ms = new MemoryStream(file);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + Name + ".pdf" + "");
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.End();
                return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
            }
            return RedirectToAction("User", "Employees");
        }

        public ActionResult TourAttachmentFile(int id)
        {
            var fileToRetrieve = db.TourAttachment.Find(id);
            var TourFile = db.EmployeeTour.Find(fileToRetrieve.TourId);
            byte[] file = blobManager.DownloadBlobClient(TourFile.UserId.ToLower(), cmsMgr.GetFileTourDetail(fileToRetrieve.FileId).ToLower());

            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        }
        //for trainer cv attachment
        public ActionResult DownloadCV(Int64 Id)
        {
            if (Id != 0)
            {
                string UserId = User.Identity.GetUserId();
                UserViewModel userDetails = genric.GetUserDetail(UserId);
                var fileToRetrieve = db.InstructorAttachment.Find(Id);
                string Name = fileToRetrieve.FileName;
                Name = Name.Replace("'", "_");
                Name = Name.Replace(' ', '_');
                string fileName = "Instructor/" + fileToRetrieve.FileId;
                byte[] file = blobManager.DownloadBlobClient(userDetails.SubscriberId.ToLower(), fileName.ToLower());
                return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
            }
            return RedirectToAction("Index", "Trainer");
        }

        public ActionResult EngagementAttachment(int id)
        {
            var fileToRetrieve = db.TrainerPlannerAttachment.Find(id);

            byte[] file = blobManager.DownloadBlobClient(fileToRetrieve.PlannerId.ToString(), admin.GetEngagementFileName(fileToRetrieve.FileId).ToLower());

            return File(file, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        }

    }


}