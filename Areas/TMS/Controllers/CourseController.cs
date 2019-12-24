using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.DAL;
using AJSolutions.Models;
using PagedList;
using System.Threading.Tasks;
using System.Net.Http;
using AJSolutions.Areas.Candidate.Models;
using System.Data.Entity;
using AJSolutions.Areas.EMS.Models;
using System.Globalization;
using Newtonsoft.Json;

namespace AJSolutions.Areas.TMS.Controllers
{
    public class CourseController : Controller
    {
        Generic generic = new Generic();
        UserDBContext userContext = new UserDBContext();
        Student student = new Student();
        AdminManager admin = new AdminManager();
        CMSManager cms = new CMSManager();
        TMSManager tms = new TMSManager();
        EMSManager ems = new EMSManager();
        TMSController train = new TMSController();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: TMS/Course
        [HttpGet]
        public ActionResult Candidate(string sortOrder, int page = 1, string CourseCode = null, Int64 BatchId = 0, string q = "", string result = "NA", int PageSize = 10)
        {
            ViewBag.result = result;
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.Course = CourseCode;
            ViewBag.BatchCode = BatchId;
            ViewBag.SearchText = q;
            ViewBag.Paging = PageSize;
            ViewBag.Page = page;

            PopulateCourse(userDetails.SubscriberId, CourseCode);

            PopulateBatchByCourse(userDetails.SubscriberId, CourseCode, BatchId);

            if (!string.IsNullOrEmpty(CourseCode))
            {
                string CorporateId = userContext.CourseMaster.Find(CourseCode).CorporateId;
                var CandidateList = student.GetCandidateListForAssignCourse(userDetails.SubscriberId, CorporateId, CourseCode).ToList();

                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParam = sortOrder == "Name" ? "Name_desc" : "Name";
                ViewBag.EmailSortParam = sortOrder == "Email" ? "Email_desc" : "Email";
                ViewBag.DesignationSortParam = sortOrder == "Designation" ? "Designation_desc" : "Designation";
                ViewBag.EmployeeSortParam = sortOrder == "RegistrationId" ? "RegistrationId_desc" : "RegistrationId";
                ViewBag.CategorySortParam = sortOrder == "Category" ? "Category_desc" : "Category";
                ViewBag.StateSortParam = sortOrder == "State" ? "State_desc" : "State";
                ViewBag.RegisteredOnSortParam = sortOrder == "RegisteredOn" ? "RegisteredOn_desc" : "RegisteredOn";


                //Apply sorting
                if (CandidateList.Count != 0)
                {
                    switch (sortOrder)
                    {

                        case "Name_desc":
                            CandidateList = CandidateList.OrderByDescending(c => c.Name).ToList();
                            break;
                        case "Email":
                            CandidateList = CandidateList.OrderBy(c => c.Email).ToList();
                            break;
                        case "Email_desc":
                            CandidateList = CandidateList.OrderByDescending(c => c.Email).ToList();
                            break;
                        case "Designation":
                            CandidateList = CandidateList.OrderBy(c => c.Designation).ToList();
                            break;
                        case "Designation_desc":
                            CandidateList = CandidateList.OrderByDescending(c => c.Designation).ToList();
                            break;
                        case "RegisteredOn":
                            CandidateList = CandidateList.OrderBy(c => c.RegisteredOn).ToList();
                            break;
                        case "RegisteredOn_desc":
                            CandidateList = CandidateList.OrderByDescending(c => c.RegisteredOn).ToList();
                            break;
                        case "Category":
                            CandidateList = CandidateList.OrderBy(c => c.BranchCategory).ToList();
                            break;
                        case "Category_desc":
                            CandidateList = CandidateList.OrderByDescending(c => c.BranchCategory).ToList();
                            break;
                        case "State":
                            CandidateList = CandidateList.OrderBy(c => c.BranchState).ToList();
                            break;
                        case "State_desc":
                            CandidateList = CandidateList.OrderByDescending(c => c.BranchState).ToList();
                            break;
                        default:
                            CandidateList = CandidateList.OrderBy(c => c.Name).ToList();
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(q))
                {
                    q = q.ToLower();
                    CandidateList = CandidateList.Where(c => c.Name.ToLower().Contains(q) || c.UserName.Contains(q) || c.RegistrationId.ToLower().Contains(q) || c.Email.ToLower().Contains(q) || c.PhoneNumber.ToLower().Contains(q)).ToList();
                }
                PopulatePaging(PageSize);

                int pageSize = PageSize;

                return View(CandidateList.ToPagedList(page, pageSize));
                //ViewData["CandidateList"] = CandidateList;
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        public async Task<ActionResult> Candidate(string CourseCode, Int64 BatchId, string[] CandidateList, string Sorting, int PageId = 1, int PageSize = 0)
        {
            string result = "Failure";
            try
            {
                if (CandidateList != null)
                {
                    UserViewModel userDetail = generic.GetUserDetail(User.Identity.GetUserId());
                    CourseBatchViewModel courseBatches = tms.GetCourseBatches(userDetail.SubscriberId, BatchId).FirstOrDefault();
                    string[] candidates = CandidateList[0].Split(',');

                    string UserId = User.Identity.GetUserId();
                    UserViewModel userDetails = generic.GetUserDetail(UserId);
                    var candidateDetails = student.GetSubscriberWiseCandidateList(userDetails.SubscriberId, BatchId)
                                            .Where(p => candidates.Contains(p.UserId)).ToList();   //.Where(p => p.BatchId == BatchId).OrderBy(c => c.Name).ToList();

                    for (int i = 0; i < candidates.Length; i++)
                    {
                        admin.AddCandidateCourse(candidates[i], BatchId, 1);

                        var CandidateDetails = generic.GetUserDetail(candidates[i]);

                        //Sending SMS to Candidate while assigning to Batch
                        //if (CandidateDetails.PhoneNumber != null)
                        //{
                        //    string mobile = CandidateDetails.PhoneNumber;
                        //    string message1 = "Hello" + CandidateDetails.Name + ",  you are Assigned to Batch  " + courseBatches.BatchName + "From " + courseBatches.FromDate.ToString("dd-MM-yyyy") + " To " + courseBatches.ToDate.ToString("dd-MM-yyyy") + " Timings " + "From " + courseBatches.FromTime + " To " + courseBatches.ToTime;
                        //    generic.sendSMSMessage(message1, mobile);
                        //}

                    }

                    /// <summary>
                    /// Candidate Course Assignment only if course is already integrated
                    /// Created By: Kulesh Sahu
                    /// Created On: 9-Aug-2017
                    /// Updated By: Kulesh Sahu
                    /// Updated On: 15-Sep-2017
                    /// Reviewed By: 
                    /// Reviewed On:
                    /// </summary>
                    if (candidateDetails.Count() > 0)
                    {
                        string LMSCourseCode = tms.IsCourseIntegrated(CourseCode);
                        if (LMSCourseCode != "NA")
                        {
                            await admin.RegisterForCourse(candidateDetails, LMSCourseCode, User.Identity.GetUserId());
                        }
                    }

                    /// <summary>
                    /// Training already scheduled for the selected batch then getting assigned assessment
                    /// Created By: Rahul Newara
                    /// Created On: 4-Aug-2017
                    /// Updated By: Rahul Newara
                    /// Updated On: 5-Aug-2017
                    /// Reviewed By:
                    /// Reviewed On:
                    /// </summary>
                    List<AssessmentTrainingView> trainAsst = admin.GetBatchAssessments(BatchId).ToList();
                    if (trainAsst.Count() > 0)
                    {

                        //Schedule assessment for the new candidate for current batch
                        await EnrollForAssessment(trainAsst, candidateDetails, BatchId);
                    }

                    result = "Success";
                }

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return RedirectToAction("Candidate", "Course", new { area = "TMS", result, CourseCode = CourseCode, BatchId = BatchId, sortOrder = Sorting, page = PageId, PageSize = PageSize });
        }

        [HttpGet]
        public ActionResult Feedback(string TrainingId, bool Result = false)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();

            ViewBag.Result = Result;
            var status = tms.GetTrackerReports();
            var Userstatus = status.Where(c => c.UserId == userDetails.UserId).FirstOrDefault();
            if (Userstatus == null)
                Userstatus = new TrackerReportView();


            return View(Userstatus);
        }

        [HttpPost]
        public ActionResult Feedback(Int32 Phase, string Answer1, string Answer2, string Answer3, string Answer4, string Answer5, string Answer6, string Answer7, string Answer8, string Answer9, string Answer10, string Answer11, string Answer12, string Answer13, string Answer14, string Answer15, string Answer16, string Answer17, string Answer18,
            string Answer19, string Answer20, string Answer21, string Answer22, string Answer23, string Answer24, string Answer25, string Answer26, string Answer27, string Answer28, string Answer29, string Answer30, string Answer31, string Answer32, string Answer33, string Answer34, string Answer35, string Answer36, string Answer37,
            string Answer38, string Answer39, string Answer40, string Answer41, string Answer42, string Answer43, string Answer44, string Answer45, string Answer46, string Answer47, string Answer48, string Answer49, string Answer50, string Answer51, string Answer52, string Answer54, string Answer55, string Answer56,
            string Answer57, string Answer58, string Answer59, string Answer60, string Answer61, string Answer62, string Answer63, string Answer64, string Answer65, string Answer66, string Answer67, string Answer68, string Answer69, string Answer70, string Answer71, string Answer72, string Answer73, string Answer74, string Answer75,
            string Answer76, string Answer77, string Answer78, string Answer79, string Answer80, string Answer81, string Answer82, string Answer83, string Answer84, string Answer85, string Answer86, string Answer87, string Answer88, string Answer89, string Answer90, string Answer91, string Answer92, string Answer93, string Answer94, string Answer95, string Answer96, string Answer97)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            bool result = tms.AddTrackerReport(userDetails.UserId, Phase, DateTime.Now, DateTime.Now, Answer1, Answer2, Answer3, Answer4, Answer5, Answer6, Answer7, Answer8, Answer9, Answer10, Answer11, Answer12, Answer13, Answer14, Answer15, Answer16, Answer17, Answer18,
             Answer19, Answer20, Answer21, Answer22, Answer23, Answer24, Answer25, Answer26, Answer27, Answer28, Answer29, Answer30, Answer31, Answer32, Answer33, Answer34, Answer35, Answer36, Answer37,
             Answer38, Answer39, Answer40, Answer41, Answer42, Answer43, Answer44, Answer45, Answer46, Answer47, Answer48, Answer49, Answer50, Answer51, Answer52, Answer54, Answer55, Answer56,
             Answer57, Answer58, Answer59, Answer60, Answer61, Answer62, Answer63, Answer64, Answer65, Answer66, Answer67, Answer68, Answer69, Answer70, Answer71, Answer72, Answer73, Answer74, Answer75,
             Answer76, Answer77, Answer78, Answer79, Answer80, Answer81, Answer82, Answer83, Answer84, Answer85, Answer86, Answer87, Answer88, Answer89, Answer90, Answer91, Answer92, Answer93, Answer94, Answer95, Answer96, Answer97);

            return RedirectToAction("Feedback", "Course", new { area = "TMS", Result = result });
        }

        [HttpGet]
        public ActionResult FeedbackBatchWise(Int64 TrainingId = 0, bool Result = false, Int64 Id = 0, string status = "")
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            ViewData["IppbBranch"] = (from i in userContext.BranchMaster select i).AsEnumerable();
            PopulateState(101, null);
            PopulateCity();
            PopulateCircle();
            PopulateBranch();
            PopulateRegion();
            PopulateDivision();
            if (status == "Delete" && TrainingId > 0)
            {
                var trainingorder = userContext.EndUserTrainingOrder.Where(c => c.EUTrainingOrderId == TrainingId).FirstOrDefault();

                if (trainingorder != null)
                {
                    tms.RemoveTrainingOrder(trainingorder.EUTrainingOrderId);
                }
                //tms.RemoveTraining(TrainingId);
                return RedirectToAction("EndUserTrainingDetials", "Course", new { area = "TMS" });
            }
            if (status == "Edit" && TrainingId > 0)
            {
                ViewBag.TrainingOrderId = TrainingId;
                var EuTraining = tms.GetEUTrainingOrder(userDetails.UserId);
                var TrainingOrder = EuTraining.Where(c => c.EUTrainingOrderId == TrainingId).FirstOrDefault();

                var trainee = tms.GetEUTrainingOrderTrainee(TrainingId);
                ViewData["TrainingOrderTrainee"] = trainee;
                var BranchList = userContext.CircleMaster.Where(c => c.Circle == TrainingOrder.Circle).FirstOrDefault();
                Int64 CircleCD = BranchList.CircleId;
                var Branches = userContext.BranchMaster.Where(x => x.CircleId == CircleCD).ToList();
                ViewData["Branch"] = Branches;

                if (TrainingOrder.Region != null)
                    PopulateRegion(TrainingOrder.Region);


                if (TrainingOrder.TrainingType == "PA")
                {
                    PopulateCity(TrainingOrder.PAState, TrainingOrder.PACity);
                    PopulateState(101, TrainingOrder.PAState);
                    var fromDate = TrainingOrder.PATrainingStartDate;
                    var toDate = TrainingOrder.PATrainingEndDate;
                    var schedule = fromDate + " " + "-" + " " + toDate;
                    ViewBag.SchedulePA = schedule;
                    Int64 DivisionId = 0;
                    Int64 n;
                    bool IsNumeric = Int64.TryParse(TrainingOrder.PADivision, out n);
                    if (IsNumeric)
                        DivisionId = Convert.ToInt64(TrainingOrder.PADivision);


                    var division = userContext.DivisionMasters.Where(c => c.DivisionId == DivisionId).FirstOrDefault();

                    if (division != null)
                    {
                        if (TrainingOrder.Region != null)
                        {
                            Int64 RegionId = Convert.ToInt64(TrainingOrder.Region);
                            PopulateDivision(RegionId, TrainingOrder.PADivision);
                        }
                        else
                        {
                            PopulateDivision();
                        }
                    }
                    else
                    {
                        PopulateDivision();
                    }
                }
                else
                {
                    var fromDate = TrainingOrder.GDSTrainingStartDate;
                    var toDate = TrainingOrder.GDSTrainingEndDate;
                    var schedule = fromDate + " " + "-" + " " + toDate;
                    ViewBag.ScheduleGDS = schedule;
                    {
                        PopulateCity(TrainingOrder.GDSState, TrainingOrder.GDSCity);
                        PopulateState(101, TrainingOrder.GDSState);
                    }
                    //Int64 DivisionId = Convert.ToInt64(TrainingOrder.GDSDivision);

                    Int64 DivisionId = 0;
                    Int64 n;
                    bool IsNumeric = Int64.TryParse(TrainingOrder.GDSDivision, out n);
                    if (IsNumeric)
                        DivisionId = Convert.ToInt64(TrainingOrder.GDSDivision);

                    var division = userContext.DivisionMasters.Where(c => c.DivisionId == DivisionId).FirstOrDefault();

                    if (division != null)
                    {
                        if (TrainingOrder.Region != null)
                        {
                            Int64 RegionId = Convert.ToInt64(TrainingOrder.Region);
                            PopulateDivision(RegionId, TrainingOrder.GDSDivision);
                        }
                        else
                        {
                            PopulateDivision();
                        }
                    }
                    else
                    {
                        PopulateDivision();
                    }
                }
                PopulateCircle(TrainingOrder.Circle);
                //var BranchList = userContext.CircleMaster.Where(c => c.Circle == TrainingOrder.Circle).FirstOrDefault();

                //PopulateBranch(BranchList.CircleId, TrainingOrder.Branch);

                return View(TrainingOrder);
            }
            if (status == "Freez")
            {
                Int64 TrainingOrderId = TrainingId;
                var trainingStatus = tms.UpdateTrainingOrderStatus(TrainingOrderId, "Freezed");
                //var EuTraining = tms.GetEUTrainingOrder(userDetails.UserId);
                var TrainingOrders = tms.GetEUTrainingOrder(userDetails.UserId).Where(c => c.EUTrainingOrderId == TrainingOrderId).FirstOrDefault(); ;
                var Trainee = tms.GetEUTrainingOrderTrainee(TrainingOrderId);

                if (TrainingOrders.TrainingType == "PA")
                {
                    var TrainerDetials = admin.GetEmployeeRegistrationDetails(TrainingOrders.PAMTContact).Where(a => a.SubscriberId == userDetails.SubscriberId).ToList(); ;
                    //TrainerDetials = TrainerDetials.Where(a => a.SubscriberId == userDetails.SubscriberId).ToList();
                    string PATrainerId = "";
                    string PATrainerReckonnId = "";
                    if (TrainerDetials.Count() == 0)
                    {
                        string userName = admin.GenerateUserName();
                        if (UserManager.FindByName(userName) != null)
                            userName = admin.GenerateUserName();

                        var user = new ApplicationUser { UserName = userName, Email = TrainingOrders.PAMTEmail, PhoneNumber = TrainingOrders.PAMTContact, EmailConfirmed = true };

                        var Userresult = UserManager.Create(user, "changeme");
                        if (Userresult.Succeeded)
                        {
                            var empstatus = UserManager.AddToRole(user.Id, "Employee");
                            if (empstatus.Succeeded)
                            {
                                //Adding Master trainer
                                EmployeeView emp = new EmployeeView();
                                emp.UserId = user.Id;
                                emp.EmployeeId = "";
                                emp.Emplanelled = false;
                                emp.Name = TrainingOrders.PAMTName;
                                emp.Gender = "";
                                emp.DOB = null;
                                emp.MaritalStatus = "";
                                emp.AlternateEmail = "";
                                emp.Nationality = "";
                                emp.DepartmentId = "FAC";
                                emp.ReportingAuthority = userDetails.SubscriberId;
                                emp.UpdatedOn = DateTime.Now;
                                emp.Deactivated = false;
                                emp.FatherName = "";
                                emp.SpouseName = "";
                                emp.EmergencyContactName = "";
                                emp.EmergencyContactNumber = "";
                                emp.BloodGroup = "";
                                emp.Location = "";
                                emp.PhysicallyChallenged = false;
                                emp.MarriageDate = null;
                                emp.DesignationId = 64;
                                emp.UpdatedBy = userDetails.SubscriberId;

                                bool res = admin.AddEmployee(emp, userDetails.SubscriberId, userDetails.UserId);
                                PATrainerId = user.Id;
                                PATrainerReckonnId = userName;
                            }
                        }
                    }
                    else
                    {
                        PATrainerId = TrainerDetials.Where(a => a.SubscriberId == userDetails.SubscriberId).FirstOrDefault().Id;
                        PATrainerReckonnId = TrainerDetials.Where(a => a.SubscriberId == userDetails.SubscriberId).FirstOrDefault().UserName;
                        //PATrainerId = TrainerDetials.FirstOrDefault().Id;
                    }


                    //Adding Batch
                    string BatchName = "";
                    // "1" is For Wave, "F is For PA Training"
                    BatchName = GetBatchName("2", "F");
                    tms.UpdateTrainingOrderRelation(TrainingOrderId, PATrainerReckonnId, BatchName);
                    CourseBatch coursebatch = new CourseBatch();
                    coursebatch.AccomondationNeeded = false;
                    coursebatch.AttendenceNeeded = true;
                    coursebatch.AvailableTillDate = null;
                    coursebatch.BatchName = BatchName;
                    coursebatch.ContentAvailability = false;
                    coursebatch.ContentLink = null;
                    coursebatch.CountryId = 101;
                    coursebatch.StateId = TrainingOrders.PAState;
                    coursebatch.CityId = TrainingOrders.PACity;

                    //For Prod
                    coursebatch.CourseCode = "CC201808310001";
                    //For DEV
                    //coursebatch.CourseCode = "CC201807210001";
                    coursebatch.FeedbackLink = null;

                    coursebatch.FromDate = TrainingOrders.PATrainingStartDate;
                    coursebatch.FromTime = TrainingOrders.PATrainingStartDate;
                    coursebatch.ToDate = TrainingOrders.PATrainingEndDate;
                    coursebatch.ToTime = TrainingOrders.PATrainingEndDate;
                    coursebatch.WardenId = null;
                    coursebatch.IsDailyAttendence = true;
                    bool Batchresult = tms.AddCourseBatch(coursebatch);
                    if (Batchresult)
                    {
                        //Scheduling Training & Sending SMS Emails
                        string TrainingScheduleId = admin.GetTrainingId();
                        var LastCourseDetails = userContext.CourseBatch.Where(a => a.BatchName == BatchName).FirstOrDefault();
                        string TrainingLocation = TrainingOrders.PAOfficeName + "," + TrainingOrders.PADivision;
                        //Ajay sir's Code
                        //bool ScheduleResult = tms.AddTrainingSchedule(TrainingId, Convert.ToInt32(LastCourseDetails.BatchId), BatchName, BatchName, PATrainerId, null, LastCourseDetails.CountryId, LastCourseDetails.StateId, LastCourseDetails.CityId, TrainingLocation, "Inprogress", null, DateTime.Now, userDetails.UserId, DateTime.Now, userDetails.UserId, null);
                        //var ScheduleAssessment = tms.AddTrainingAssessment(0, "SI Assessment", TrainingId, 100, "0",
                        //               "SI Assessment", DateTime.Now.Date, DateTime.Now.Date, DateTime.Now, DateTime.Now); 
                        //created by -vikas pandey for marging two methods in a single method 
                        bool ScheduleResult = tms.AddTrainingScheduleAssesment(TrainingScheduleId, Convert.ToInt32(LastCourseDetails.BatchId), BatchName, BatchName, PATrainerId, null, LastCourseDetails.CountryId, LastCourseDetails.StateId, LastCourseDetails.CityId, TrainingLocation, "Inprogress", null, DateTime.Now, userDetails.UserId, DateTime.Now, userDetails.UserId, null,
                            0, "SI Assessment", 100, "0", "SI Assessment", DateTime.Now.Date, DateTime.Now.Date, DateTime.Now, DateTime.Now);
                        //end
                        if (TrainingOrders.PAMTContact != null)
                        {
                            var trainerDetails = generic.GetUserDetail(PATrainerId);
                            string message = "Dear Trainer,  \r\n\r\n Training schedule(" + BatchName + " )" + " for PA/GDS/Postman starting from " + TrainingOrders.PATrainingStartDate.ToString("dd-MMM-yyyy") + " To " + TrainingOrders.PATrainingEndDate.ToString("dd-MMM-yyyy") + " has been assigned to you. " + "\r\n Your RECKONN User Id is '" + trainerDetails.UserName + "' and password is 'changeme' ." +
                            "\r\n Kindly verify the candidate list and bring any discrepancy to the notice of your superior officer." +
                            "\r\n Kindly note to mark ATTENDANCE in RECKONN (batchname.reckonn.com) for all the days of Training. "
                            + "\r\n Please contact 180030026170 for any assistance or send mail to ippbsupport@nibf.in \r\n\r\n By IPPB Training team";

                            generic.sendSMS(message, TrainingOrders.PAMTContact);
                        }
                        //string[] PNumber = PhoneNumber[0].Split(',');

                        foreach (var item in Trainee)
                        {
                            string uName = admin.GenerateUserName();
                            if (UserManager.FindByName(uName) != null)
                                uName = admin.GenerateUserName();
                            string CandidateModuleAccess = "SMS";
                            string CandidateRoleId = "Candidate";
                            string CandidateDepartment = "CAN";
                            //Registring Trainee
                            string Pnumber = "";
                            bool validatePhoneNumber = hasSpecialChar(item.PhoneNumber);
                            if (validatePhoneNumber == false)
                            {
                                if (!string.IsNullOrEmpty(item.PhoneNumber) && item.PhoneNumber.Length == 10 && item.PhoneNumber != "0000000000" && item.PhoneNumber != "9999999999" && !item.PhoneNumber.StartsWith("1234"))
                                {
                                    Pnumber = item.PhoneNumber;
                                }
                                else
                                {
                                    Pnumber = GenerateNumber();
                                }
                            }
                            else
                            {
                                Pnumber = GenerateNumber();
                            }

                            var adduser = new ApplicationUser { UserName = uName, Email = null, PhoneNumber = Pnumber, EmailConfirmed = true };
                            string passcode = GenerateNumber();
                            //if (!string.IsNullOrEmpty(item.Name) && item.Name.Length > 6)
                            //{
                            //    passcode = admin.GeneratePassword(item.Name);
                            //}
                            //else
                            //{
                            //    passcode = GenerateNumber();
                            //}
                            var CandidateUserresult = UserManager.Create(adduser, passcode);
                            if (CandidateUserresult.Succeeded)
                            {
                                var Candidatestatus = UserManager.AddToRole(adduser.Id, CandidateRoleId);
                                if (Candidatestatus.Succeeded)
                                {
                                    string SIUSerId = admin.GetSIUserId(item.Branch, TrainingOrders.TrainingType);
                                    item.UpdatedBy = User.Identity.GetUserId();
                                    string Region = Convert.ToString(TrainingOrders.Region);
                                    string divisions = TrainingOrders.PADivision;

                                    //Adding Candidate Details
                                    admin.UserRegistration(adduser.Id, item.Name, DateTime.UtcNow, CandidateModuleAccess, CandidateDepartment, CandidateRoleId, userDetails.SubscriberId,
                                        false, null, DateTime.UtcNow, item.UpdatedBy, SIUSerId, null, item.Branch,
                                        item.EmpId, Region, divisions, null, "8005f39a-aba3-4dd3-b567-cc538c422a78", item.Gender, passcode, "", item.Designation, null, item.TrackerId, item.FaciltyId, item.Accesspoint);

                                    admin.AddCandidateCourse(adduser.Id, Convert.ToInt32(LastCourseDetails.BatchId), 1);
                                }
                            }
                        }
                    }


                }
                else
                {
                    var TrainerDetials = admin.GetEmployeeRegistrationDetails(TrainingOrders.GDSMTContact).Where(a => a.SubscriberId == userDetails.SubscriberId).ToList();
                    // TrainerDetials = TrainerDetials.Where(a => a.SubscriberId == userDetails.SubscriberId).ToList();
                    string GDSTrainerId = "";
                    string GDSTrainerReckonnId = "";

                    if (TrainerDetials.Count() == 0)
                    {
                        string userName = admin.GenerateUserName();
                        if (UserManager.FindByName(userName) != null)
                            userName = admin.GenerateUserName();

                        var user = new ApplicationUser { UserName = userName, Email = TrainingOrders.GDSMTEmail, PhoneNumber = TrainingOrders.GDSMTContact, EmailConfirmed = true };

                        var Userresult = UserManager.Create(user, "changeme");
                        if (Userresult.Succeeded)
                        {
                            var Empstatus = UserManager.AddToRole(user.Id, "Employee");
                            if (Empstatus.Succeeded)
                            {
                                EmployeeView emp = new EmployeeView();
                                emp.UserId = user.Id;
                                emp.EmployeeId = "";
                                emp.Emplanelled = false;
                                emp.Name = TrainingOrders.GDSMTName;
                                emp.Gender = "";
                                emp.DOB = null;
                                emp.MaritalStatus = "";
                                emp.AlternateEmail = "";
                                emp.Nationality = "";
                                emp.DepartmentId = "FAC";
                                emp.ReportingAuthority = userDetails.SubscriberId;
                                emp.UpdatedOn = DateTime.Now;
                                emp.Deactivated = false;
                                emp.FatherName = "";
                                emp.SpouseName = "";
                                emp.EmergencyContactName = "";
                                emp.EmergencyContactNumber = "";
                                emp.BloodGroup = "";
                                emp.Location = "";
                                emp.PhysicallyChallenged = false;
                                emp.MarriageDate = null;
                                emp.DesignationId = 64;
                                emp.UpdatedBy = userDetails.SubscriberId;

                                bool res = admin.AddEmployee(emp, userDetails.SubscriberId, userDetails.UserId);
                            }
                            GDSTrainerId = user.Id;
                            GDSTrainerReckonnId = userName;
                        }
                    }
                    else
                    {
                        GDSTrainerId = TrainerDetials.Where(a => a.SubscriberId == userDetails.SubscriberId).FirstOrDefault().Id;
                        GDSTrainerReckonnId = TrainerDetials.Where(a => a.SubscriberId == userDetails.SubscriberId).FirstOrDefault().UserName;
                        //   GDSTrainerId = Trainer.Id;
                    }

                    //Adding Batch
                    string BatchName = "";
                    // "1" is For Wave, "A is For GDS Training"
                    BatchName = GetBatchName("2", "M");
                    tms.UpdateTrainingOrderRelation(TrainingOrderId, GDSTrainerReckonnId, BatchName);
                    CourseBatch coursebatch = new CourseBatch();
                    coursebatch.AccomondationNeeded = false;
                    coursebatch.AttendenceNeeded = true;
                    coursebatch.AvailableTillDate = null;
                    coursebatch.BatchName = BatchName;
                    coursebatch.ContentAvailability = false;
                    coursebatch.ContentLink = null;
                    coursebatch.CountryId = 101;
                    coursebatch.StateId = TrainingOrders.GDSState;
                    coursebatch.CityId = TrainingOrders.GDSCity;

                    //For Prod
                    coursebatch.CourseCode = "CC201808310001";

                    //For DEV
                    //coursebatch.CourseCode = "CC201807210001";
                    coursebatch.FeedbackLink = null;
                    coursebatch.FromDate = TrainingOrders.GDSTrainingStartDate;
                    coursebatch.FromTime = TrainingOrders.GDSTrainingStartDate;
                    coursebatch.ToDate = TrainingOrders.GDSTrainingEndDate;
                    coursebatch.ToTime = TrainingOrders.GDSTrainingEndDate;
                    coursebatch.WardenId = null;
                    coursebatch.IsDailyAttendence = true;
                    bool Batchresult = tms.AddCourseBatch(coursebatch);
                    if (Batchresult)
                    {
                        //Scheduling Training & Sending SMS Emails
                        string TrainingScheduleId = admin.GetTrainingId();
                        var LastCourseDetails = userContext.CourseBatch.Where(a => a.BatchName == BatchName).FirstOrDefault();
                        string TrainingLocation = TrainingOrders.GDSOfficeName + "," + TrainingOrders.GDSDivision;
                        //Ajay sir's Code
                        //bool ScheduleResult = tms.AddTrainingSchedule(TrainingId, Convert.ToInt32(LastCourseDetails.BatchId), BatchName, BatchName, GDSTrainerId, null, LastCourseDetails.CountryId, LastCourseDetails.StateId, LastCourseDetails.CityId, TrainingLocation, "Inprogress", null, DateTime.Now, userDetails.UserId, DateTime.Now, userDetails.UserId, null);
                        //var ScheduleAssessment = tms.AddTrainingAssessment(0, "SI Assessment", TrainingId, 100, "0",
                        //               "SI Assessment", DateTime.Now.Date, DateTime.Now.Date, DateTime.Now, DateTime.Now);

                        //created by -vikas pandey for marging two methods in a single method 
                        bool ScheduleResult = tms.AddTrainingScheduleAssesment(TrainingScheduleId, Convert.ToInt32(LastCourseDetails.BatchId), BatchName, BatchName, GDSTrainerId, null, LastCourseDetails.CountryId, LastCourseDetails.StateId, LastCourseDetails.CityId, TrainingLocation, "Inprogress", null, DateTime.Now, userDetails.UserId, DateTime.Now, userDetails.UserId, null,
                            0, "SI Assessment", 100, "0", "SI Assessment", DateTime.Now.Date, DateTime.Now.Date, DateTime.Now, DateTime.Now);
                        //end
                        if (TrainingOrders.GDSMTContact != null)
                        {
                            var trainerDetails = generic.GetUserDetail(GDSTrainerId);
                            string message = "Dear Trainer,  \r\n\r\n Training schedule(" + BatchName + " )" + " for PA/GDS/Postman starting from " + TrainingOrders.GDSTrainingStartDate.ToString("dd-MMM-yyyy") + " To " + TrainingOrders.GDSTrainingEndDate.ToString("dd-MMM-yyyy") + " has been assigned to you. " + "\r\n Your RECKONN User Id is '" + trainerDetails.UserName + "' and password is 'changeme' ." +
                                "\r\n Kindly verify the candidate list and bring any discrepancy to the notice of your superior officer." +
                                "\r\n Kindly note to mark ATTENDANCE in RECKONN (www.reckonn.com) for all the days of Training. " +
                                "\r\n Please contact 180030026170 for any assistance or send mail to ippbsupport@nibf.in \r\n\r\n By IPPB Training team";

                            generic.sendSMS(message, TrainingOrders.GDSMTContact);
                        }

                        foreach (var item in Trainee)
                        {
                            string uName = admin.GenerateUserName();
                            if (UserManager.FindByName(uName) != null)
                                uName = admin.GenerateUserName();

                            string CandidateModuleAccess = "SMS";
                            string CandidateRoleId = "Candidate";
                            string CandidateDepartment = "CAN";
                            //Registring Trainee
                            string Pnumber = "";
                            bool validatePhoneNumber = hasSpecialChar(item.PhoneNumber);
                            if (validatePhoneNumber == false)
                            {
                                if (!string.IsNullOrEmpty(item.PhoneNumber) && item.PhoneNumber.Length == 10 && item.PhoneNumber != "0000000000" && item.PhoneNumber != "9999999999" && !item.PhoneNumber.StartsWith("1234"))
                                {
                                    Pnumber = item.PhoneNumber;
                                }
                                else
                                {
                                    Pnumber = GenerateNumber();
                                }
                            }
                            else
                            {
                                Pnumber = GenerateNumber();
                            }
                            var adduser = new ApplicationUser { UserName = uName, Email = null, PhoneNumber = Pnumber, EmailConfirmed = true };
                            string passcode = GenerateNumber();
                            //if (!string.IsNullOrEmpty(item.Name) && item.Name.Length > 6)
                            //{
                            //    passcode = admin.GeneratePassword(item.Name);
                            //}
                            //else
                            //{
                            //    passcode = GenerateNumber();
                            //}
                            var CandidateUserresult = UserManager.Create(adduser, passcode);
                            if (CandidateUserresult.Succeeded)
                            {
                                var Candidatestatus = UserManager.AddToRole(adduser.Id, CandidateRoleId);

                                if (Candidatestatus.Succeeded)
                                {
                                    string SIUSerId = admin.GetSIUserId(item.Branch, TrainingOrders.TrainingType);
                                    // item.UpdatedBy = User.Identity.GetUserId();                                     
                                    //Adding Candidate Details
                                    string Region = Convert.ToString(TrainingOrders.Region);
                                    string divisions = TrainingOrders.GDSDivision;
                                    admin.UserRegistration(adduser.Id, item.Name, DateTime.UtcNow, CandidateModuleAccess, CandidateDepartment, CandidateRoleId, userDetails.SubscriberId,
                                        false, null, DateTime.UtcNow, User.Identity.GetUserId(), SIUSerId, null, item.Branch,
                                       item.EmpId, Region, divisions, null, "8005f39a-aba3-4dd3-b567-cc538c422a78", item.Gender, passcode, "", item.Designation, null, item.TrackerId, item.FaciltyId, item.Accesspoint);
                                    admin.AddCandidateCourse(adduser.Id, Convert.ToInt32(LastCourseDetails.BatchId), 1);
                                }
                            }
                        }
                    }
                }
                return RedirectToAction("EndUserTraineeDetails", "Course", new { area = "TMS", Id = TrainingOrders.EUTrainingOrderId });
            }
            ViewBag.Result = Result;
            ViewBag.TrainingOrderId = Id;

            //var status = tms.GetTrackerBatchReports();
            //var Userstatus = status.Where(c => c.UserId == userDetails.UserId).FirstOrDefault();
            //if (Userstatus == null)
            //    Userstatus = new TrackerReportBatchWiseView();

            return View();
        }

        [HttpPost]
        public ActionResult FeedbackBatchWise(string Wave, string Circle, string DOContact, string TrainingType, string PAOfficeName, string PAState, string PACity,
                string PADivision, string PAMTName, string PAMTDesignation, string PAMTContact, string PAMTEmail, string PATrainingStartDate, string PATrainingEndDate, string GDSOfficeName, string GDSState, string PARegion, string GDSRegion,
                string GDSCity, string GDSDivision, string GDSMTName, string GDSMTDesignation, string GDSMTContact, string GDSMTEmail, string GDSTrainingStartDate, string GDSTrainingEndDate, string Schedule, string GDSSchedule,
                string UpdatedBy, string[] FaciltyId = null, string[] AccessPoint = null, Int64 EUTrainingOrderId = 0, int PACount = 0, int SUFinacleCount = 0, int GDSCount = 0, int PostmenCount = 0, int GDSSUCount = 0, string[] TrackerId = null, string[] Name = null, string[] PhoneNumber = null, string[] Gender = null, string[] EmpId = null, string[] Designation = null, string[] Branch = null, string[] Flag = null, string[] EUTraineeId = null)
        {

            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            DateTime patrainingStartDate = DateTime.Now;
            DateTime patrainingEndDate = DateTime.Now;
            DateTime gdstrainingStartDate = DateTime.Now;
            DateTime gdstrainingendDate = DateTime.Now;
            string Region = "";
            if (TrainingType == "PA")
            {
                string[] strSchedule = Schedule.Split('-');
                DateTime frmdate = DateTime.ParseExact(strSchedule[0].Trim(), "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);

                DateTime todate = DateTime.ParseExact(strSchedule[1].Trim(), "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);

                patrainingStartDate = frmdate;

                patrainingEndDate = todate;

                Region = PARegion;
            }
            else
            {
                string[] strSchedule = GDSSchedule.Split('-');
                DateTime GDSfrmdate = DateTime.ParseExact(strSchedule[0].Trim(), "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);

                DateTime GDStodate = DateTime.ParseExact(strSchedule[1].Trim(), "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);

                gdstrainingStartDate = GDSfrmdate;

                gdstrainingendDate = GDStodate;

                Region = GDSRegion;
            }


            //DateTime patrainingStartDate = DateTime.Now;
            //if (!string.IsNullOrEmpty(PATrainingStartDate))
            //    patrainingStartDate = Convert.ToDateTime(PATrainingStartDate);

            //DateTime patrainingEndDate = DateTime.Now;
            //if (!string.IsNullOrEmpty(PATrainingEndDate))
            //    patrainingEndDate = Convert.ToDateTime(PATrainingEndDate);

            //DateTime gdstrainingStartDate = DateTime.Now;
            //if (!string.IsNullOrEmpty(GDSTrainingStartDate))
            //    gdstrainingStartDate = Convert.ToDateTime(GDSTrainingStartDate);

            //DateTime gdstrainingendDate = DateTime.Now;
            //if (!string.IsNullOrEmpty(GDSTrainingEndDate))
            //    gdstrainingendDate = Convert.ToDateTime(GDSTrainingEndDate);

            int pastate = 0;
            if (!string.IsNullOrEmpty(PAState))
                pastate = Convert.ToInt32(PAState);

            int pacity = 0;
            if (!string.IsNullOrEmpty(PACity))
                pacity = Convert.ToInt32(PACity);

            int gdsstate = 0;
            if (!string.IsNullOrEmpty(GDSState))
                gdsstate = Convert.ToInt32(GDSState);

            int gdscity = 0;
            if (!string.IsNullOrEmpty(GDSCity))
                gdscity = Convert.ToInt32(GDSCity);

            bool result = tms.AddEUTrainingOrder(EUTrainingOrderId, Wave, Circle, DOContact, TrainingType, PACount, SUFinacleCount, PAOfficeName, pastate, pacity, PADivision,
                PAMTName, PAMTDesignation, PAMTContact, PAMTEmail, patrainingStartDate, patrainingEndDate, GDSCount, PostmenCount, GDSSUCount, GDSOfficeName, gdsstate, gdscity,
                GDSDivision, GDSMTName, GDSMTDesignation, GDSMTContact, GDSMTEmail, gdstrainingStartDate, gdstrainingendDate, DateTime.UtcNow, userDetails.UserId, "UnFreezed", Region,
               TrackerId, Name, PhoneNumber, Gender, EmpId, Designation, FaciltyId, AccessPoint, Branch, Flag, EUTraineeId);

            if (result)
            {
                if (EUTrainingOrderId == 0)
                {
                    var TrainingOrderId = userContext.EndUserTrainingOrder.Where(c => c.UpdatedBy == userDetails.UserId).OrderByDescending(c => c.EUTrainingOrderId).FirstOrDefault();
                    Int64 TrainingId = TrainingOrderId.EUTrainingOrderId;
                    return Json(new { Result = true, TrainingId = TrainingId }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = true, TrainingId = EUTrainingOrderId }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return RedirectToAction("FeedbackBatchWise", "Course", new { area = "TMS", Result = false, });
            }

        }


        public string GenerateNumber()
        {
            Random random = new Random();
            string r = "";
            int i;
            for (i = 1; i < 11; i++)
            {
                r += random.Next(0, 9).ToString();
            }
            return r;
        }

        public static bool hasSpecialChar(string input)
        {
            string specialChar = @"\|!#$%&/()=?»«@£§€{}.-;'<>_,";
            foreach (var item in specialChar)
            {
                if (input.Contains(item) || input.Contains(" ")) return true;
            }

            return false;
        }

        [HttpGet]
        public ActionResult EndUserTraineeDetails(Int64 Id = 0, string result = "")
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            if (Id > 0)
            {
                var EuTraining = tms.GetEUTrainingOrder(userDetails.UserId);
                ViewData["TrainingOrders"] = EuTraining.Where(c => c.EUTrainingOrderId == Id).FirstOrDefault();
                var Trainee = tms.GetEUTrainingOrderTrainee(Id);
                return View(Trainee);
            }
            return View();
        }

        [HttpPost]
        public ActionResult EndUserTraineeDetails(Int64 TrainingOrderId = 0)
        {

            return RedirectToAction("MDMDetail", "Course", new { area = "TMS" });
        }

        [HttpGet]
        public ActionResult FeedbackStatus(string UserId)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            var status = tms.GetTrackerReports();
            var Phasestatus = status.Where(c => c.Phase == 0).ToList();
            var Userstatus = status.Where(c => c.UserId == UserId).FirstOrDefault();
            return View(status);
        }

        [HttpGet]
        public ActionResult UserFeedbackStatus(string UserId)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            var status = tms.GetTrackerReports();
            var Userstatus = status.Where(c => c.UserId == UserId).FirstOrDefault();
            return View(Userstatus);
        }

        [HttpGet]
        public ActionResult TrackerDashBoard(string TrainingId, bool Result = false)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            var status = cms.GetMTTrainingOrderDashboard();

            return View(status);
        }

        [HttpGet]
        public ActionResult EndUserDashBoard(string TrainingId, bool Result = false)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            ViewData["Attended"] = tms.GetEndUserDashboardAttendedDetails();
            ViewData["Nominated"] = tms.GetEndUserDashboardNominatedDetails();
            ViewData["Certified"] = tms.GetEndUserDashboardCertifiedDetails();

            ViewBag.TotalPaNominated = tms.GetEndUserDashboardNominatedDetails().Select(a => a.TotalPATraineeNominated).Sum();
            ViewBag.TotalPaAttended = tms.GetEndUserDashboardAttendedDetails().Select(a => a.TotalPATraineeAttended).Sum();
            ViewBag.TotalPaCertified = tms.GetEndUserDashboardCertifiedDetails().Select(a => a.TotalPACertified).Sum();

            ViewBag.TotalGDSNominated = tms.GetEndUserDashboardNominatedDetails().Select(a => a.TotalGDSTraineeNominated).Sum();
            ViewBag.TotalGDSattended = tms.GetEndUserDashboardAttendedDetails().Select(a => a.TotalGDSTraineeAttended).Sum();
            ViewBag.TotalGDSCertified = tms.GetEndUserDashboardCertifiedDetails().Select(a => a.TotalGDSCertified).Sum();

            ViewBag.Totalnominated = tms.GetEndUserDashboardNominatedDetails().Select(a => a.TotalTraineeNominated).Sum();
            ViewBag.TotalAttended = tms.GetEndUserDashboardAttendedDetails().Select(a => a.TotalTraineeAttended).Sum();
            ViewBag.TotalCertified = tms.GetEndUserDashboardCertifiedDetails().Select(a => a.TotalTraineeCertified).Sum();
            return View();
        }

        public ActionResult GetMultiTrendTotal()
        {
            var details = tms.GetEndUserDashboardAllDetails();
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNominated()
        {
            var nominated = tms.GetEndUserDashboardNominatedDetails().OrderByDescending(a => a.TotalTraineeNominated);
            return Json(nominated, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNominatedTrand()
        {
            var nominated = tms.GetEndUserDashboardTrendGraphNominatedDetails();
            return Json(nominated, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNominatedTrandChart()
        {
            var nominated = tms.GetEndUserDashboardTrendGraphNominatedDetailsChart();
            return Json(nominated, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPANominatedTrandChart()
        {
            var nominated = tms.GetEndUserDashboardTrendGraphPANominatedDetailsChart();
            return Json(nominated, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGDSNominatedTrandChart()
        {
            var nominated = tms.GetEndUserDashboardTrendGraphGDSNominatedDetailsChart();
            return Json(nominated, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAttended()
        {
            var attended = tms.GetEndUserDashboardAttendedDetails().OrderByDescending(a => a.TotalTraineeAttended);
            return Json(attended, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAttendedTrand()
        {
            var attended = tms.GetEndUserDashboardTrendGrpahAttendedDetails();
            return Json(attended, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetattendedTrandChart()
        {
            var nominated = tms.GetEndUserDashboardTrendGraphAttendedDetailsChart();
            return Json(nominated, JsonRequestBehavior.AllowGet);
        }

        //
        public ActionResult GetPAattendedTrandChart()
        {
            var certified = tms.GetEndUserDashboardTrendGraphPAAttendedDetailsChart();
            return Json(certified, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGDSattendedTrandChart()
        {
            var certified = tms.GetEndUserDashboardTrendGraphGDSAttendedDetailsChart();
            return Json(certified, JsonRequestBehavior.AllowGet);
        }
        //
        public ActionResult GetCertified()
        {
            var certified = tms.GetEndUserDashboardCertifiedDetails().OrderByDescending(a => a.TotalTraineeCertified);
            return Json(certified, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCertifiedTrand()
        {
            var certified = tms.GetEndUserDashboardTrandGraphCertifiedDetails();
            return Json(certified, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCertifiedTrandChart()
        {
            var certified = tms.GetEndUserDashboardTrendGraphCertifiedDetailsChart();
            return Json(certified, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPACertifiedTrandChart()
        {
            var certified = tms.GetEndUserDashboardTrendGraphPACertifiedDetailsChart();
            return Json(certified, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGDSCertifiedTrandChart()
        {
            var certified = tms.GetEndUserDashboardTrendGraphGDSCertifiedDetailsChart();
            return Json(certified, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendNotification(string TrainingId, string CourseCode)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetail = generic.GetUserDetail(UserId);
            //Sending SMS & Email To Trainer on Scheduling Training
            var Training = userContext.TrainingSchedule.Find(TrainingId);

            var BatchCandidates = admin.GetBatchCandidates(CourseCode);
            BatchCandidates = BatchCandidates.Where(c => c.BatchId == Training.BatchId).ToList();

            var TrainerDetail = generic.GetUserDetail(Training.TrainerId);
            var subscriberDetail = cms.GetCorporateProfile(userDetail.SubscriberId).FirstOrDefault();

            string mobile = admin.GetUserRegistrationDetails(Training.TrainerId).PhoneNumber;
            string email = admin.GetUserRegistrationDetails(Training.TrainerId).Email;
            if (!string.IsNullOrEmpty(mobile))
            {
                string message1 = "Dear Trainer,  \r\n\r\n Training schedule(" + "Batch id EUW0-XXXX" + " )" + " for PA/GDS/Postman starting from " + BatchCandidates.FirstOrDefault().FromDate.ToString("dd-MMM-yyyy") + " To " + BatchCandidates.FirstOrDefault().ToDate.ToString("dd-MMM-yyyy") + " has been assigned to you. " + "\r\n Your RECKONN User Id is REC18RXXXXX and password is ****." +
                    "Kindly ACCEPT the training, ADD CANDIDATES and mark ATTENDANCE in RECKONN (www.reckonn.com). Please contact 180030026170 for any assistance or send mail to ippbsupport@nibf.in \r\n\r\n By IPPB Training team";

                //                Dear Trainer,
                //Training schedule(Batch id EUW0-XXXX) for PA/GDS/Postman starting from dd/mm/yyyy has been assigned to you. 
                //Your RECKONN User Id is REC18RXXXXX and password is ****. Kindly ACCEPT the training, ADD CANDIDATES and mark ATTENDANCE in RECKONN (www.reckonn.com).
                //Please contact 180030026170 for any assistance or send mail to "ippbsupport@nibf.in" 
                //By IPPB Training team
                generic.sendSMS(message1, mobile);
            }

            //Sending SMS & Email to Candidate while assigning to Batch
            if (BatchCandidates.Count > 0)
            {
                foreach (var candidate in BatchCandidates)
                {
                    if (candidate.PhoneNumber != null)
                    {
                        string candidatemobile = candidate.PhoneNumber;
                        string message = "Dear Shri/Smt " + candidate.Name + ", \r\n\r\n Welcome to the Training program for " + Training.SubjectLine + "( " + TrainingId + " )" + " to be held from " + BatchCandidates.FirstOrDefault().FromDate.ToString("dd-MMM-yyyy") + " To " + BatchCandidates.FirstOrDefault().ToDate.ToString("dd-MMM-yyyy") + " At " + Training.Address + ". \r\n You are requested to report at the venue one day before the commencement of the training. Please carry your AADHAR card, PAN card and self attested copies of both. \r\n\r\n All the best! \r\n Training Team, \r\n India Post payments Bank";
                        generic.sendSMS(message, candidatemobile);
                    }
                }
            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MasterTrainerTO(string TrainingId, bool Result = false)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            PopulateCircle();
            ViewData["IppbBranch"] = (from i in userContext.BranchMaster select i).AsEnumerable();
            ViewBag.Result = Result;


            return View();
        }

        [HttpPost]
        public ActionResult MasterTrainerTO(string TrainingWeek, string Circle, string ContactNumber, string[] Branch = null, string[] Trainingtype = null, string[] TrainingLocation = null, string[] NewEmployeeId = null, string[] Name = null, string[] Gender = null, string[] Designation = null, string[] PhoneNumber = null, string[] TrackerId = null, bool Status = true, Int64 TrainingOrderId = 0, Int64 TraineeId = 0)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            var MTDetails = userContext.MTTrainingOrder.OrderByDescending(c => c.TrainingOrderId).FirstOrDefault();

            bool result = tms.AddMTTrainingDetails(TrainingOrderId, TrainingWeek, Circle, ContactNumber, DateTime.UtcNow, userDetails.UserId, Status);
            for (int i = 0; i < Name.Length; i++)
            {
                var MTTraineeDetails = userContext.MTTrainingOrder.OrderByDescending(c => c.TrainingOrderId).FirstOrDefault();

                tms.AddMTTrainingDetailsTrainee(TraineeId, Name[i], NewEmployeeId[i], Gender[i], Designation[i], PhoneNumber[i], Branch[i], TrackerId[i], MTTraineeDetails.TrainingOrderId, Trainingtype[i], TrainingLocation[i]);
            }

            return Json("true", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MDMDetail(string Id, bool Status = false)
        {

            ViewBag.Status = Status;

            string UserId = User.Identity.GetUserId();
            UserViewModel UserDetails = generic.GetUserDetail(UserId);
            ViewData["userprofile"] = UserDetails;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();

            var trainingDetail = userContext.TrainingSchedule.Find(Id);
            ViewBag.BatchId = trainingDetail.BatchId;
            var date = userContext.CourseBatch.Find(trainingDetail.BatchId);
            ViewBag.StartDate = date.FromDate;
            ViewBag.Enddate = date.ToDate;
            ViewBag.TrainingId = Id;
            //ViewData["CourseBatch"] = db.CourseBatch.Find(trainingDetail.BatchId);

            ViewData["TrainerDetail"] = ems.GetSubscriberWiseEmployeeList(UserDetails.SubscriberId).Where(e => e.UserId == trainingDetail.TrainerId).FirstOrDefault();
            var candidate = student.GetSubscriberWiseCandidateList(UserDetails.SubscriberId, trainingDetail.BatchId).OrderBy(c => c.Name).ToList();
            ViewBag.CandidateCount = candidate.Count;
            ViewData["Candidate"] = candidate;

            return View();
        }

        [HttpPost]
        public ActionResult MDMDetail(string TrainingId, string UId, string OtherId)
        {
            UId = UId.TrimEnd(',');
            string[] userid = UId.Trim().Split(',');
            string[] otherId = OtherId.Split(',');
            bool result = false;

            for (int i = 0; i < userid.Length; i++)
            {
                var Trainee = userContext.UserProfile.Find(userid[i]);
                if (Trainee != null)
                {
                    Trainee.OtherId = otherId[i];
                    userContext.Entry(Trainee).State = EntityState.Modified;
                    userContext.SaveChanges();
                }
            }

            return RedirectToAction("MDMDetail", "Course", new { area = "TMS", Id = TrainingId, Status = result });
        }


        [HttpGet]
        public JsonResult GetIppbBranches()
        {
            var branches = userContext.BranchMaster.ToList();
            branches = branches.OrderBy(c => c.Branch).ToList();
            return Json(branches, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult EndUserTrainingDetials(string TrainingId, bool Result = false)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            var EuTraining = tms.GetEUTrainingOrder(userDetails.UserId);
            var PATraining = EuTraining.Where(c => c.TrainingType == "PA").ToList();
            var GDSTraining = EuTraining.Where(c => c.TrainingType == "GDS").ToList();
            ViewData["PATrainingOrders"] = PATraining;
            ViewData["GDSTrainingOrders"] = GDSTraining;
            return View();
        }



        [HttpGet]
        public ActionResult GetMasterTrainerTO()
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            ViewData["MTTrainingOrders"] = tms.GetMTTrainingOrder(userDetails.UserId);

            return View();
        }

        [HttpGet]
        public ActionResult MasterTrainerTrainingOrder(string Id = null)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            ViewData["MTTrainingOrders"] = tms.GetMTTrainingOrderforCorporates();

            return View();
        }

        public void DeleteMtTraningOrder(string TrainingOrderId)
        {
            var Order = userContext.MTTrainingOrder.Find(TrainingOrderId);
            if (Order != null)
            {
                Order.Status = false;

                userContext.Entry(Order).State = EntityState.Modified;
                userContext.SaveChanges();
            }
        }

        [HttpGet]
        public ActionResult GetMasterTrainerTOTrainees(long TrainingOrderId = 0)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();

            if (TrainingOrderId != 0)
            {
                var TrainingOrder = tms.GetMTTrainingOrderforCorporates();
                ViewData["TrainingOrders"] = TrainingOrder.Where(c => c.TrainingOrderId == TrainingOrderId).FirstOrDefault();
                var Trainee = tms.GetMTTrainingOrderTrainee(TrainingOrderId);
                return View(Trainee);
            }
            return View();
        }

        private void PopulateCourse(string SubscriberId, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetCourseDetails(SubscriberId);
            SelectList CourseCode = new SelectList(query, "CourseCode", "CourseName", selectedValue);
            ViewBag.CourseCode = CourseCode;
        }

        private void PopulateState(int CountryId = 0, object selectedState = null)
        {
            var query = generic.GetState(CountryId);
            SelectList States = new SelectList(query, "StateId", "State", selectedState);
            ViewBag.StateId = States;
        }

        private void PopulateCity(int StateId = 0, object selectedCity = null)
        {
            var query = generic.GetCity(StateId);
            SelectList Cities = new SelectList(query, "CityId", "City", selectedCity);
            ViewBag.CityId = Cities;
        }

        private void PopulateCircle(object selectedCircle = null)
        {
            var query = userContext.CircleMaster.ToList();
            SelectList Circles = new SelectList(query, "Circle", "Circle", selectedCircle);
            ViewBag.Circle = Circles;
        }

        private void PopulateBranch(Int64 CircleCode = 0, object selectedBranch = null)
        {
            var query = new List<BranchMaster>();
            query = userContext.BranchMaster.Where(c => c.CircleId == CircleCode).ToList();
            SelectList Branches = new SelectList(query, "Branch", "Branch", selectedBranch);
            ViewBag.Branch = Branches;
        }

        [HttpPost]
        public ActionResult GetBranch(string Circle)
        {
            var BranchList = userContext.CircleMaster.Where(c => c.Circle == Circle).FirstOrDefault();

            List<SelectListItem> Branch = new List<SelectListItem>();
            if (BranchList != null)
            {
                Int64 CircleCD = BranchList.CircleId;
                List<BranchMaster> Branches = userContext.BranchMaster.Where(x => x.CircleId == CircleCD).ToList();
                Branches = Branches.OrderBy(c => c.Branch).ToList();
                Branches.ForEach(x =>
                {
                    Branch.Add(new SelectListItem { Text = x.Branch, Value = x.Branch.ToString() });
                });
            }
            return Json(Branch, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetIPPBBranch(string Circle)
        {
            var BranchList = userContext.CircleMaster.Where(c => c.Circle == Circle).FirstOrDefault();

            List<SelectListItem> Branch = new List<SelectListItem>();
            if (BranchList != null)
            {
                Int64 CircleCD = BranchList.CircleId;
                List<BranchMaster> Branches = userContext.BranchMaster.Where(x => x.CircleId == CircleCD).ToList();
                Branches.ForEach(x =>
                {
                    Branch.Add(new SelectListItem { Text = x.Branch, Value = x.Branch.ToString() });
                });
            }
            return Json(Branch, JsonRequestBehavior.AllowGet);
        }
        private void PopulateBatchByCourse(string SubscriberId = null, string CourseCode = null, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetBatches(SubscriberId, CourseCode).Where(b => b.ToDate >= DateTime.UtcNow.Date).ToList();
            SelectList BatchId = new SelectList(query, "BatchId", "BatchName", selectedValue);
            ViewBag.BatchId = BatchId;
        }

        private void PopulatePaging(object selectedValue = null)
        {
            var PageList = generic.GetPaging();
            ViewBag.PageSize = new SelectList(PageList, "PageSize", "PageSize", selectedValue);
        }

        /// <summary>
        /// Schedule assessment for the new candidate for current batch
        /// Created By: Rahul Newara
        /// Created On: 4-Aug-2017
        /// Updated By: Kulesh
        /// Updated On: 5-Aug-2017
        /// Reviewed By:
        /// Reviewed On:
        /// </summary>
        public async Task<bool> EnrollForAssessment(List<AssessmentTrainingView> AsstTraining,
                                                    List<CandidateViewModel> candidates, Int64 BatchId)
        {
            bool Result = false;
            string UserId = User.Identity.GetUserId();
            string TrainingId = AsstTraining[0].TrainingId;
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            List<CandidateRegisterView> candiReg = new List<CandidateRegisterView>();

            foreach (var Ast in AsstTraining)
            {
                foreach (var candi in candidates)
                {
                    if (candiReg.Count() == 0)
                    {
                        candiReg.Add(new CandidateRegisterView()
                        {
                            CandidateId = candi.UserId,
                            Name = candi.Name,
                            Email = candi.Email,
                            PhoneNumber = candi.PhoneNumber,
                            UserId = userDetails.SubscriberId,
                            Redirectionurl = Global.WebsiteUrl() + "Home/Index",
                            PublicationId = Ast.PublicationId,
                            Password = candi.PCode,
                            Category = !string.IsNullOrEmpty(candi.BatchName) ? candi.BatchName : BatchId.ToString(),
                            TrainingId = Ast.TrainingId,
                            StartDate = Ast.StartDate,
                            EndDate = Ast.EndDate,
                            StartTime = Ast.StartTime,
                            EndTime = Ast.EndTime,
                        });
                    }
                    else
                    {
                        if (candiReg.Where(a => a.CandidateId == candi.UserId && a.PublicationId == Ast.PublicationId).FirstOrDefault() == null)
                        {
                            candiReg.Add(new CandidateRegisterView()
                            {
                                CandidateId = candi.UserId,
                                Name = candi.Name,
                                Email = candi.Email,
                                PhoneNumber = candi.PhoneNumber,
                                UserId = userDetails.SubscriberId,
                                Redirectionurl = Global.WebsiteUrl() + "Home/Index",
                                PublicationId = Ast.PublicationId,
                                Password = candi.PCode,
                                Category = !string.IsNullOrEmpty(candi.BatchName) ? candi.BatchName : BatchId.ToString(),
                                TrainingId = Ast.TrainingId,
                                StartDate = Ast.StartDate,
                                EndDate = Ast.EndDate,
                                StartTime = Ast.StartTime,
                                EndTime = Ast.EndTime,
                            });
                        }
                    }

                }
            }

            string apiUrl = Global.PreloreUrl() + "/Api/Value/PostRegisterCandidate";
            HttpResponseMessage responsePostMethod = new HttpResponseMessage();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                responsePostMethod = await client.PostAsJsonAsync(apiUrl, candiReg);
            }
            return Result;
        }


        public string GetBatchName(string Wavess, string UserType)
        {
            UserDBContext udbc = new UserDBContext();
            string EndUser = "EU";
            string WaveNumber = "W" + Wavess;


            string BatchName = EndUser + WaveNumber + UserType;
            {
                //For Dev Use Course Code = CC201807210001
                // For Prod Use Course code= CC201808310001
                var batchName = from s in udbc.CourseBatch.Where(s => s.BatchName.Contains(BatchName) && s.CourseCode == "CC201808310001")
                                orderby s.BatchName descending
                                select s.BatchName;

                if (batchName != null)
                    BatchName = batchName.FirstOrDefault();
                else
                    BatchName = BatchName + "0001";

                if (BatchName != null)
                {
                    string BatchPartialId = BatchName.Substring(5);
                    int lastVal = Convert.ToInt32(BatchPartialId);
                    lastVal = lastVal + 1;
                    string suffix = string.Empty;

                    for (int i = Convert.ToString(lastVal).Length; i < 4; i++)
                    {
                        suffix = suffix + "0";
                    }

                    BatchName = BatchName.Substring(0, 5) + suffix + Convert.ToString(lastVal);
                }
                return BatchName;
            }
        }

        [HttpGet]
        public ActionResult AddCertifiedUsers(string UserAction = "ADD", Int64 CertifiedUsersId = 0)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            var CertifiedUsers = userContext.OPSData.Find(CertifiedUsersId);
            if (CertifiedUsers == null)
                CertifiedUsers = new OPSData();
            return View(CertifiedUsers);
        }

        [HttpPost]
        public ActionResult AddCertifiedUsers(OPSData OPSData)
        {
            var certifieduser = userContext.OPSData.ToList();
            certifieduser = certifieduser.OrderByDescending(c => c.CertifiedUsersId).ToList();
            var cId = certifieduser.FirstOrDefault();
            if (OPSData.CertifiedUsersId == 0)
            {
                OPSData.CertifiedUsersId = cId.CertifiedUsersId + 1;
            }
            bool result = tms.AddOpsData(OPSData);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CertifiedUsers(string IPPBCircle, string IPPBSolName, int? page, int PageSize = 100, string UserAction = "ADD", Int64 CertifiedUsersId = 0)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            PopulateOPSCircle();
            PopulateOPSBranch(IPPBCircle);
            var CertifiedUsers = userContext.OPSData.ToList();
            if (!string.IsNullOrEmpty(IPPBSolName) || !string.IsNullOrEmpty(IPPBCircle))
            {
                if (!string.IsNullOrEmpty(IPPBCircle))
                    CertifiedUsers = CertifiedUsers.Where(c => c.CircleName.Trim().ToLower() == IPPBCircle.Trim().ToLower()).ToList();

                if (!string.IsNullOrEmpty(IPPBSolName))
                    CertifiedUsers = CertifiedUsers.Where(c => c.IPPBSolName.Trim().ToLower() == IPPBSolName.Trim().ToLower()).ToList();

                PopulateOPSCircle(IPPBCircle);
                PopulateOPSBranch(IPPBCircle, IPPBSolName);
            }

            ViewBag.Page = page;
            PopulateBigDataPaging(PageSize);

            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            if (UserAction == "DELETE" && CertifiedUsersId > 0)
            {
                var Certified = userContext.OPSData.Find(CertifiedUsersId);
                if (Certified != null)
                {
                    userContext.OPSData.Remove(Certified);
                    userContext.SaveChanges();
                }
                ViewBag.Result = "Deleted";

                return View(CertifiedUsers.ToPagedList(pageNumber, pageSize));
            }
            return View(CertifiedUsers.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult CertifiedUsers()
        {

            return RedirectToAction("CertifiedUsers", "Course", new { area = "TMS" });
        }

        [HttpGet]
        public ActionResult CertifiedData(string IPPBSolName, int? page, int PageSize = 100)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            var certifiedData = userContext.CertifiedData.ToList();
            PopulateOPSBranch();
            ViewBag.Page = page;
            PopulateBigDataPaging(PageSize);
            if (!string.IsNullOrEmpty(IPPBSolName))
            {
                certifiedData = certifiedData.Where(c => c.Branch == IPPBSolName).ToList();
                PopulateOPSBranch(IPPBSolName);
            }
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);

            return View(certifiedData.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult CertifyUser(string CertifiedUsersId, bool IsCertified, bool IsAvailableLaunch, bool IsMobileDevice, string Remarks)
        {

            Int64 Id = Convert.ToInt64(CertifiedUsersId);
            var certifieduser = userContext.OPSData.Find(Id);
            if (certifieduser != null)
            {
                certifieduser.IsCertified = IsCertified;
                certifieduser.IsAvailableLaunch = IsAvailableLaunch;
                certifieduser.IsMobileDevice = IsMobileDevice;
                certifieduser.Comments = Remarks;

                userContext.Entry(certifieduser).State = EntityState.Modified;
                userContext.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SyncUser()
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            var certifiedData = tms.GetEUTrainingOrderAdmin();
            return View(certifiedData);
        }

        [HttpPost]
        public ActionResult SyncUser(string TrainingOrderId, string BatchName)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            Int64 TrainingID = Convert.ToInt64(TrainingOrderId);
            var TrainingOrderDetails = userContext.EndUserTrainingOrder.Find(TrainingID);
            if (TrainingOrderDetails != null)
            {
                var TrainingOrder = userContext.EnduUserTrainee.Where(c => c.EUTrainingOrderId == TrainingID).ToList();

                var Batch = userContext.CourseBatch.Where(c => c.BatchName == BatchName).FirstOrDefault();
                if (Batch != null)
                {
                    var canddiatecount = userContext.CandidateCourseDetails.Where(c => c.BatchId == Batch.BatchId).Count();
                    if (canddiatecount > 0)
                    {
                        RemoveUnassignTrainee(Batch.BatchId);
                    }
                    foreach (var item in TrainingOrder)
                    {
                        string uName = admin.GenerateUserName();
                        if (UserManager.FindByName(uName) != null)
                            uName = admin.GenerateUserName();
                        string CandidateModuleAccess = "SMS";
                        string CandidateRoleId = "Candidate";
                        string CandidateDepartment = "CAN";
                        //Registring Trainee
                        string Pnumber = "";
                        bool validatePhoneNumber = hasSpecialChar(item.PhoneNumber);
                        if (validatePhoneNumber == false)
                        {
                            if (!string.IsNullOrEmpty(item.PhoneNumber) && item.PhoneNumber.Length == 10 && item.PhoneNumber != "0000000000" && item.PhoneNumber != "9999999999" && !item.PhoneNumber.StartsWith("1234"))
                            {
                                Pnumber = item.PhoneNumber;
                            }
                            else
                            {
                                Pnumber = GenerateNumber();
                            }
                        }
                        else
                        {
                            Pnumber = GenerateNumber();
                        }
                        var adduser = new ApplicationUser { UserName = uName, Email = null, PhoneNumber = Pnumber, EmailConfirmed = true };
                        string passcode = GenerateNumber();
                        //if (!string.IsNullOrEmpty(item.Name) && item.Name.Length > 6)
                        //{
                        //    passcode = admin.GeneratePassword(item.Name);
                        //}
                        //else
                        //{
                        //    passcode = GenerateNumber();
                        //}
                        var CandidateUserresult = UserManager.Create(adduser, passcode);
                        if (CandidateUserresult.Succeeded)
                        {
                            var Candidatestatus = UserManager.AddToRole(adduser.Id, CandidateRoleId);
                            if (Candidatestatus.Succeeded)
                            {
                                string SIUSerId = admin.GetSIUserId(item.Branch, TrainingOrderDetails.TrainingType);
                                admin.UserRegistration(adduser.Id, item.Name, DateTime.UtcNow, CandidateModuleAccess, CandidateDepartment, CandidateRoleId, userDetails.SubscriberId,
                                                        false, null, DateTime.UtcNow, TrainingOrderDetails.UpdatedBy, SIUSerId, null, item.Branch,
                                                        item.EmpId, null, null, null, "8005f39a-aba3-4dd3-b567-cc538c422a78", item.Gender, passcode, "", item.Designation, null, item.TrackerId, item.FaciltyId, item.Accesspoint);
                                admin.AddCandidateCourse(adduser.Id, Batch.BatchId, 1);
                            }
                        }
                    }

                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DeleteTrainingOrder()
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();

            var Batches = userContext.CourseBatch.Where(c => c.CourseCode == "CC201808310001").ToList();
            ViewData["Batch"] = Batches;

            var TrainingOrder = userContext.EndUserTrainingOrder.ToList();
            return View(TrainingOrder);
        }

        [HttpPost]
        public ActionResult DeleteTrainingOrder(string Batch, Int64 Id = 0)
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            if (Id > 0)
            {
                var TrainingOrder = userContext.EndUserTrainingOrder.Find(Id);
                if (TrainingOrder != null)
                {
                    var Trainees = userContext.EnduUserTrainee.Where(c => c.EUTrainingOrderId == TrainingOrder.EUTrainingOrderId).ToList();
                    if (Trainees != null)
                    {
                        foreach (var item in Trainees)
                        {
                            var traineesId = userContext.EnduUserTrainee.Find(item.EUTrainingOrderId);
                            userContext.EnduUserTrainee.Remove(traineesId);
                            userContext.SaveChanges();
                        }
                        if (TrainingOrder.BatchName != null)
                        {
                            var TrainingId = userContext.TrainingSchedule.Where(c => c.SubjectLine == Batch).FirstOrDefault();
                            bool Delete = tms.DeleteTrainingBAtch(TrainingId.BatchId, TrainingId.TrainingId);
                        }
                        var Training = userContext.EndUserTrainingOrder.Find(Id);
                        userContext.EndUserTrainingOrder.Remove(Training);
                        userContext.SaveChanges();
                    }
                }
            }
            if (!string.IsNullOrEmpty(Batch))
            {
                var TrainingId = userContext.TrainingSchedule.Where(c => c.SubjectLine == Batch).FirstOrDefault();
                if (TrainingId != null)
                {
                    bool Delete = tms.DeleteTrainingBAtch(TrainingId.BatchId, TrainingId.TrainingId);
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public void RemoveUnassignTrainee(Int64 BatchId)
        {
            var trainingorder = userContext.CandidateCourseDetails.Where(c => c.BatchId == BatchId).ToList();
            if (trainingorder != null)
            {
                foreach (var item in trainingorder)
                {
                    var traineesId = userContext.CandidateCourseDetails.Find(item.UserId, BatchId);
                    userContext.CandidateCourseDetails.Remove(traineesId);
                    userContext.SaveChanges();
                }
            }
        }

        private void PopulateBigDataPaging(object selectedValue = null)
        {
            var BigDataPageList = generic.GetBigDataPaging();
            ViewBag.PageSize = new SelectList(BigDataPageList, "BigDataPageSize", "BigDataPageSize", selectedValue);
        }

        private void PopulateOPSBranch(string CircleName = null, object selectedValue = null)
        {
            var branch = tms.GetIPPBBranch(CircleName);
            ViewBag.IPPBSolName = new SelectList(branch, "IPPBSolName", "IPPBSolName", selectedValue);
        }

        private void PopulateOPSCircle(object selectedValue = null)
        {
            var branch = tms.GetIPPBBranch(null).Select(c => new { c.CircleName }).Distinct().ToList();
            ViewBag.CircleName = new SelectList(branch, "CircleName", "CircleName", selectedValue);
        }

        private void PopulateRegion(object selectedCircle = null)
        {
            var query = userContext.RegionMasters.ToList();
            SelectList Regions = new SelectList(query, "RegionId", "Region", selectedCircle);
            ViewBag.Regions = Regions;
        }


        private void PopulateDivision(Int64 RegionId = 0, object selectedValue = null)
        {
            var query = new List<DivisionMasters>();
            query = userContext.DivisionMasters.Where(c => c.RegionId == RegionId).ToList();
            SelectList Divisions = new SelectList(query, "DivisionId", "Division", selectedValue);
            ViewBag.Divisions = Divisions;

            //var Regions = userContext.RegionMasters.Where(c => c.RegionId == RegionId).FirstOrDefault();
            var division = userContext.DivisionMasters.ToList();
            ViewBag.divisions = new SelectList(division, "DivisionId", "Division", selectedValue);
        }


        [HttpGet]
        public JsonResult GetIppbRegions(string Circle)
        {
            List<SelectListItem> RegionId = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(Circle))
            {
                var CircleRegions = userContext.CircleMaster.Where(c => c.Circle == Circle).FirstOrDefault();
                List<RegionMasters> regions = userContext.RegionMasters.Where(x => x.CircleId == CircleRegions.CircleId).ToList();
                regions.ForEach(x =>
                {
                    RegionId.Add(new SelectListItem { Text = x.Region, Value = x.RegionId.ToString() });
                });
            }
            return Json(RegionId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetIppbDivisions(Int64 RegionId = 0)
        {
            List<SelectListItem> DivisionId = new List<SelectListItem>();
            if (RegionId > 0)
            {
                List<DivisionMasters> divisions = userContext.DivisionMasters.Where(x => x.RegionId == RegionId).ToList();
                divisions.ForEach(x =>
                {
                    DivisionId.Add(new SelectListItem { Text = x.Division, Value = x.DivisionId.ToString() });
                });
            }
            return Json(DivisionId, JsonRequestBehavior.AllowGet);
        }
        //public async Task<bool> RegisterForCourse(List<CandidateViewModel> candidates, string CourseCode)
        //{
        //    bool Result = false;
        //    string UserId = User.Identity.GetUserId();
        //    UserViewModel userDetails = generic.GetUserDetail(UserId);
        //    List<UserRegView> userReg = new List<UserRegView>();
        //    foreach (var candi in candidates)
        //    {
        //        if (userReg.Count() == 0)
        //        {
        //            userReg.Add(new UserRegView()
        //            {
        //                UserId = candi.UserId,
        //                UserName = candi.UserName,
        //                UserRole = "Candidate",
        //                Name = candi.Name,
        //                Email = candi.Email,
        //                MobileNumber = candi.PhoneNumber,
        //                Redirectionurl = "reckonn.com",
        //                SubscriberId = candi.SubscriberId,
        //                Password = Global.RandomString(8),
        //                CourseCode = CourseCode
        //            });

        //        }
        //        else
        //        {
        //            if (userReg.Where(a => a.UserId == candi.UserId).FirstOrDefault() == null)
        //            {
        //                userReg.Add(new UserRegView()
        //                {
        //                    UserId = candi.UserId,
        //                    UserName = candi.UserName,
        //                    UserRole = "Candidate",
        //                    Name = candi.Name,
        //                    Email = candi.Email,
        //                    MobileNumber = candi.PhoneNumber,
        //                    Redirectionurl = "reckonn.com",
        //                    SubscriberId = candi.SubscriberId,
        //                    Password = Global.RandomString(8),
        //                    CourseCode = CourseCode
        //                });
        //            }
        //        }
        //    }


        //    string apiUrl = Global.WikipianUrl() + "/Api/Value/PostBulkUserReg";
        //    HttpResponseMessage responsePostMethod = new HttpResponseMessage();
        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(apiUrl);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //        responsePostMethod = await client.PostAsJsonAsync(apiUrl, userReg);
        //        if (responsePostMethod.IsSuccessStatusCode)
        //        {
        //            Result = true;
        //        }
        //    }
        //    return Result;
        //} 
    }
}