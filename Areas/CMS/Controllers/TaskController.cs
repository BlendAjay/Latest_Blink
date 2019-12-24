using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.Areas.Admin.Models;
using AJSolutions.DAL;
using AJSolutions.Models;
using System.Globalization;

namespace AJSolutions.Areas.CMS.Controllers
{
    public class TaskController : Controller
    {
        AdminManager admin = new AdminManager();
        Generic generic = new Generic();
        CMSManager cmsMgr = new CMSManager();
        EMSManager emsMgr = new EMSManager();
        TMSManager tmsMgr = new TMSManager();
        UserDBContext userContext = new UserDBContext();

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

        // GET: CMS/Task
        public ActionResult Index()
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            return View();
        }

        //[Authorize(Roles = "Admin,Client,Employee")]
        public ActionResult MyTasks(string Id, string TaskId, string AssignedTo, string JobOrderNumber, int JobOrderTypeId = 0, int StatusType = 0, short TaskStatus = -1, string UserAction = "Add")
        {
            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewBag.UserId = userDetails.SubscriberId;
            ViewData["UserProfile"] = userDetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewData["Attachment"] = admin.GetTaskAttachments(TaskId);
            ViewData["FinalAttach"] = cmsMgr.GetTaskFinalAttachments(TaskId);
            PopulateTaskType(userDetails.SubscriberId, JobOrderTypeId);
            PopulateAssignedToList(UserId, AssignedTo);
            PopulateJobOrderlist(UserId, JobOrderNumber);
            PopulateTaskStatuslist(TaskStatus);
            if (!string.IsNullOrEmpty(TaskId))
            {
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
                    cmsMgr.uploadTaskFile(TaskId, attachment);
                }
            }
            if (Id != null && TaskStatus > -1)
            {
                admin.UpdateTaskStatus(Id, TaskStatus, AssignedTo);
                AssignedTo = null;
                TaskStatus = -1;
            }

            //if (userDetails.DepartmentId == "ADI")
            //{
            var Tasks = admin.GetTaskFORAssignedTo(UserId);

            if (!string.IsNullOrEmpty(AssignedTo) && AssignedTo != "undefined")
                Tasks = Tasks.Where(t => t.AssignedTo == AssignedTo).ToList();
            if (!string.IsNullOrEmpty(JobOrderNumber) && JobOrderNumber != "undefined")
                Tasks = Tasks.Where(t => t.JobOrderNumber == JobOrderNumber).ToList();
            if (TaskStatus != -1)
                Tasks = Tasks.Where(t => t.TaskStatus == TaskStatus).ToList();
            if (JobOrderTypeId != 0)
                Tasks = Tasks.Where(t => t.TaskTypeId == JobOrderTypeId).ToList();
            if (UserAction == "Delete" && TaskId != null)
            {
                if (cmsMgr.RemoveTask(TaskId, userDetails.SubscriberId))
                {
                    admin.AddNotification(userDetails.SubscriberId, UserId, " has deleted Task: " + TaskId, "Task", TaskId, false, DateTime.Now);
                }
                ViewBag.Result = "Deleted";
            }
            return View(Tasks);
            //}
            //else
            //{
            //    var Tasks = admin.GetTaskFORAssignedTo(UserId);

            //    if (!string.IsNullOrEmpty(AssignedTo) && AssignedTo != "undefined")
            //        Tasks = Tasks.Where(t => t.AssignedTo == AssignedTo).ToList();
            //    if (!string.IsNullOrEmpty(JobOrderNumber) && JobOrderNumber != "undefined")
            //        Tasks = Tasks.Where(t => t.JobOrderNumber == JobOrderNumber).ToList();
            //    if (TaskStatus != -1)
            //        Tasks = Tasks.Where(t => t.TaskStatus == TaskStatus).ToList();
            //    if (JobOrderTypeId != 0)
            //        Tasks = Tasks.Where(t => t.TaskTypeId == JobOrderTypeId).ToList();
            //    if (UserAction == "Delete" && TaskId != null)
            //    {
            //        if (cmsMgr.RemoveTask(TaskId, userDetails.SubscriberId))
            //        {
            //            admin.AddNotification(userDetails.SubscriberId, UserId, " has deleted Task: " + TaskId, "Task", TaskId, false, DateTime.Now);
            //        }
            //    }
            //    return View(Tasks);
            //}

        }

        //Commented by Ajay//
        //[HttpGet]
        ////[Authorize(Roles = "Admin,Client,Employee")]
        //public ActionResult Invoice(string Status, string Id, string InvoiceTo, string InvoiceNumber, bool CV = false, string UserAction = "Add")
        //{
        //    string UserId = User.Identity.GetUserId();
        //    ViewData["UserProfile"] = generic.GetUserDetail(UserId);
        //    ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(UserId).FirstOrDefault();
        //    ViewData["TaxType"] = admin.GetTaxMaster().AsEnumerable();
        //    ViewData["InvTaxs"] = admin.GetInvoiceTaxes(InvoiceNumber).AsEnumerable();
        //    ViewData["TaskItems"] = userContext.TaskItems.Where(item => item.TaskId == Id).ToList();
        //    ViewData["InvItems"] = cmsMgr.GetInvoiceItems(InvoiceNumber).AsEnumerable();


        //    if (UserAction == "Edit")
        //    {
        //        var Invoice = admin.GetInvoiceDetails(UserId, InvoiceNumber);
        //        ViewBag.InvoiceDate = Invoice.InvoiceDate;
        //        ViewBag.Currency = Invoice.Currency;
        //        ViewBag.InvoiceNumber = InvoiceNumber;
        //        ViewBag.InvoiceTo = Invoice.InvoiceTo;
        //        ViewBag.ReferenceId = Invoice.ReferenceId;
        //        ViewBag.NetAmount = Invoice.NetAmount;
        //        ViewBag.AdditionalCost = Invoice.AdditionalCost;
        //        ViewBag.Deductions = Invoice.Deductions;
        //        ViewData["Content"] = (from i in userContext.InVoiceAttachment.Where(i => i.ReferenceId == Invoice.ReferenceId) select i).FirstOrDefault();
        //        return View(Invoice);
        //    }
        //    else
        //    {
        //        var task = admin.GetTaskDetails(User.Identity.GetUserId(), Id);
        //        ViewBag.InvoiceTo = InvoiceTo;
        //        ViewBag.ReferenceId = Id;
        //        ViewBag.InvoiceSubject = task.Subject;
        //        ViewBag.InvoiceDate = DateTime.Now.Date;
        //        ViewBag.Currency = task.Currency;
        //        ViewBag.NetAmount = task.TaskAmount;
        //        ViewBag.AdditionalCost = 0;
        //        ViewBag.Deductions = 0;
        //    }


        //    return View();
        //}


        //[HttpPost]
        ////[Authorize(Roles = "Admin,Client,Employee")]
        //public ActionResult Invoice(GenerateInvoice GenerateInvoice, string[] ItemId, string[] ItemType, string[] ItemDescription, string[] Unit, string[] UnitPrice, string[] ItemDuration, string[] Actions, string[] ActionTax,
        //                                    Int64[] Taxation, Int64[] TaxationId, float[] TaxactionAmount, string[] CalculatedTax, float NetAmount = 0, float AdditionalCost = 0, float Deductions = 0, float GrandTotal = 0, Int16 PaymentModeId = 0)
        //{
        //    string body = "";
        //    string messageType = "";
        //    UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
        //    ViewData["UserProfile"] = userDetails;
        //    GenerateInvoice.SubscriberId = userDetails.SubscriberId;
        //    GenerateInvoice.CorporateId = userDetails.UserId;
        //    GenerateInvoice.Status = "Submitted";
        //    GenerateInvoice.Acknowledge = false;
        //    GenerateInvoice.Total = NetAmount;
        //    GenerateInvoice.NetAmount = GrandTotal;

        //    if (GenerateInvoice.ReferenceId.StartsWith("JO"))
        //        messageType = "Joborder: " + GenerateInvoice.ReferenceId;
        //    else
        //        messageType = "Task: " + GenerateInvoice.ReferenceId;



        //    if (string.IsNullOrEmpty(GenerateInvoice.InvoiceNumber))
        //    {
        //        body = " has raised invoice ( " + GenerateInvoice.InvoiceNumber + " ) against " + messageType;
        //    }
        //    else
        //    {
        //        body = " has modified Invoice: " + GenerateInvoice.InvoiceNumber;
        //    }

        //    if (GenerateInvoice.InvoiceNumber == null)
        //    {
        //        GenerateInvoice.InvoiceNumber = admin.GetInvoiceNumber();
        //    }


        //    var result = admin.AddInvoice(GenerateInvoice, ItemId, ItemType, ItemDescription, Unit, UnitPrice, ItemDuration, Actions, Taxation, CalculatedTax, ActionTax);

        //    admin.AddNotification(userDetails.SubscriberId, userDetails.UserId, body, "Invoice", GenerateInvoice.InvoiceNumber, false, DateTime.Now);

        //    if (result == true)
        //    {
        //        if (!string.IsNullOrEmpty(GenerateInvoice.ReferenceId))
        //        {
        //            foreach (string file in Request.Files)
        //            {                        
        //                HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
        //                admin.uploadInvFile(GenerateInvoice.ReferenceId, attachment);
        //            }
        //        }
        //    }

        //    //string message1 = "You have received an Invoice (" + GenerateInvoice.InvoiceNumber + ") from " + userDetails.Name; //eg "message hello ";                
        //    //generic.sendSMSMessage(message1, generic.GetUserDetail(userDetails.SubscriberId).PhoneNumber);
        //    return RedirectToAction("MyTasks", "Task", new { area = "CMS" });
        //}

        [HttpGet]
        //[Authorize(Roles = "Admin,Client,Employee")]
        public ActionResult TaskUpdate(string TaskId, string Subject, string Description, string Venue,
                                 string JobOrderNumber, string City, string State, string Country, string CreatedBy, string InvoiceFrequency,
                                 string AssignedTo, DateTime? StartDate = null, int Duration = 0, short TaskStatus = 0, int TaskTypeId = 0,
                                  float TaskAmount = 0)
        {

            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            PopulateJobOrderType(userDetails.SubscriberId, TaskTypeId);
            ViewBag.UserId = userDetails.SubscriberId;

            CreatedBy = userDetails.UserId;
            ViewBag.UserName = userDetails.Name;
            ViewBag.UserRole = userDetails.Role;
            ViewBag.TaskId = TaskId;
            ViewBag.Subject = Subject;
            ViewBag.Description = Description;
            ViewBag.Venue = Venue;
            ViewBag.StartDate = StartDate;
            ViewBag.Duration = Duration;
            PopulateJobOrder(userDetails.SubscriberId, JobOrderNumber);
            ViewBag.City = City;
            ViewBag.State = State;
            ViewBag.Country = Country;
            ViewBag.Duration = Duration;
            PopulateTaskStatus(TaskStatus);
            ViewBag.SubscriberId = userDetails.SubscriberId;
            ViewBag.CreatedBy = CreatedBy;

            PopulateInvoiceFrequency(InvoiceFrequency);
            PopulateAssignedTo(AssignedTo);
            ViewBag.TaskAmount = TaskAmount;
            ViewBag.DeptId = userDetails.DepartmentId;
            var task = admin.GetTask(TaskId, userDetails.SubscriberId);
            return View(task);

        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Client,Employee")]
        public ActionResult TaskUpdate(string TaskId, string AssignedTo, short TaskStatus = 0)
        {
            string UserId = User.Identity.GetUserId();
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            AssignedTo = User.Identity.GetUserId();
            bool result = admin.UpdateTaskStatus(TaskId, TaskStatus, AssignedTo);
            return RedirectToAction("MyTasks", "Task", new { Area = "CMS" });
        }

        //[Authorize(Roles = "Admin,Client,Employee")]
        public ActionResult TaskDetails(string Id, string AssignedTo, short TaskStatus = 0)
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            string UserId = User.Identity.GetUserId();
            ViewBag.UserId = userDetails.SubscriberId;
            // var taskCommentsCount = new TaskCommentsCount();
            //var TasktrainingCount = new TaskTrainingCount();
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;

            if (Id != null && TaskStatus > 0)
            {
                admin.UpdateTaskStatus(Id, TaskStatus, AssignedTo);
            }
            ViewData["TaskItems"] = admin.GetTaskItems(Id);
            var TaskComment = emsMgr.GetTaskComments(Id);
            ViewData["TaskReplies"] = emsMgr.GetTaskReplies();
            ViewData["Attachment"] = admin.GetTaskAttachments(Id).FirstOrDefault();
            ViewData["FinalAttach"] = cmsMgr.GetTaskFinalAttachments(Id);
            //int taskCommentsCount = emsMgr.GetTaskCommentsCount(Id);
            ViewData["TaskComments"] = TaskComment;
            ViewBag.CommentsCount = TaskComment.Count();//taskCommentsCount.TotalTaskComments;          
            ViewBag.TrainingCount = tmsMgr.GetTaskTrainingCount(Id);
            return View(admin.GetTaskMasters(userDetails.SubscriberId, Id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult TaskDetails(string Id, string Comment, string Reply, Int64 TaskCommentId = 0)
        {
            TaskMaster task = new TaskMaster();
            string UserId = User.Identity.GetUserId();
            task = userContext.TaskMaster.Find(Id);
            var user = generic.GetUserDetail(UserId);
            string body = " has commented for the Task: ";
            bool Status = false;
            string CommentOrReplyBy = UserId;
            var result = false;
            if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Comment))
            {
                result = emsMgr.AddTaskComments(Id, Comment, DateTime.Now, UserId);
                if (user.Role == "Admin")
                {
                    admin.AddNotification(task.AssignedTo, CommentOrReplyBy, body + task.TaskId, "Task", task.TaskId, Status, DateTime.Now);

                }
                else
                {
                    admin.AddNotification(task.CreatedBy, CommentOrReplyBy, body + task.TaskId, "Task", task.TaskId, Status, DateTime.Now);
                }

            }

            if (TaskCommentId != 0 && !string.IsNullOrEmpty(Reply))
            {
                body = " has replied  for the comment of Task: ";
                result = emsMgr.AddTaskReplies(TaskCommentId, Reply, DateTime.Now, UserId);
                if (user.Role == "Admin")
                {
                    admin.AddNotification(task.AssignedTo, CommentOrReplyBy, body + task.TaskId, "Task", task.TaskId, Status, DateTime.Now);
                }
                else
                {
                    admin.AddNotification(task.CreatedBy, CommentOrReplyBy, body + task.TaskId, "Task", task.TaskId, Status, DateTime.Now);
                }
            }

            return RedirectToAction("TaskDetails", "Task", new { area = "CMS", Id = Id, AssignedTo = "", TaskStatus = 0 });
        }


        [HttpPost]
        public ActionResult RemoveTaskComment(string Id, Int64 TaskCommentId)
        {
            emsMgr.RemoveTaskComment(TaskCommentId);
            return Json("Succeed", JsonRequestBehavior.AllowGet);
            // return RedirectToAction("TaskDetails", "Task", new { area = "CMS", Id = Id, AssignedTo = "", TaskStatus = 0 });
        }

        [HttpPost]
        public ActionResult RemoveTaskReply(string Id, Int64 TaskReplyId)
        {
            emsMgr.RemoveTaskReply(TaskReplyId);
            return Json("Succeed", JsonRequestBehavior.AllowGet);

            //return RedirectToAction("TaskDetails", "Task", new { area = "CMS", Id = Id, AssignedTo = "", TaskStatus = 0 });
        }

        [HttpGet]
        //[Authorize(Roles = "Admin, Employee")]
        public ActionResult Task(string TaskId, DateTime? StartDate, string JobOrderNumber, string useraction = "Add", bool result = false)
        {

            string UserId = User.Identity.GetUserId();
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewBag.UserId = userDetails.SubscriberId;
            ViewData["UserProfile"] = userDetails;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            ViewBag.UserRole = userDetails.Role;
            string SubscriberId = userDetails.SubscriberId;

            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            if (string.IsNullOrEmpty(TaskId))
                TaskId = "0"; // For new task get null model from below fuction

            var gettask = admin.GetTaskMasters(SubscriberId, TaskId).FirstOrDefault();

            ViewData["TaskItems"] = admin.GetTaskItems(TaskId).AsEnumerable();

            ViewData["ItemType"] = (from i in userContext.ItemTypeMasters.Where(i => i.CorporateId == userDetails.SubscriberId) select i).AsEnumerable();

            ViewData["ItemDuration"] = Global.GetDuration();
            ViewBag.result = result;

            if (useraction == "Edit")
            {
                if (gettask != null)
                {
                    PopulateCurrency(gettask.Currency);
                    PopulateJobOrder(gettask.SubscriberId, gettask.JobOrderNumber);
                    PopulateCountry(gettask.Country);
                    PopulateState(Convert.ToInt32(gettask.Country), gettask.State);
                    PopulateCity(Convert.ToInt32(gettask.State), gettask.City);
                    PopulateTaskStatus(gettask.TaskStatus);
                    PopulateJobOrderType(gettask.SubscriberId, gettask.TaskTypeId);
                    PopulateInvoiceFrequency(gettask.InvoiceFrequency);
                    if (gettask.StartDate != null)
                    {
                        ViewBag.startDate = gettask.StartDate.Value.ToString("dd-MM-yyyy");
                    }
                    if (userDetails.DepartmentId != "ADI")
                    {
                        PopulateAssignedToForManager(gettask.SubscriberId, gettask.AssignedTo);
                    }
                    else
                    {
                        PopulateAssignedTo(gettask.SubscriberId, gettask.AssignedTo);
                    }
                }

                ViewData["Content"] = (from i in userContext.TaskAttachment.Where(i => i.TaskId == TaskId) select i).FirstOrDefault();
            }
            else
            {
                PopulateCurrency();
                PopulateJobOrder(SubscriberId, JobOrderNumber);

                PopulateCountry();
                PopulateState();
                PopulateCity();

                PopulateTaskStatus();

                PopulateJobOrderType(SubscriberId);
                PopulateInvoiceFrequency();
                if (userDetails.UserId != userDetails.SubscriberId && userDetails.DepartmentId != "ADI")
                {
                    PopulateAssignedToForManager(SubscriberId);
                }
                else
                {
                    PopulateAssignedTo(SubscriberId);
                }
            }

            return View(gettask);
        }


        [HttpPost]
        //[Authorize(Roles = "Admin, Employee")]
        public ActionResult Task(TaskMaster task, string StartDate, string[] ItemId, string[] ItemType, string[] ItemDescription, string[] Unit, string[] UnitPrice, string[] ItemDuration, string[] Actions, string fileName, bool Status = false, string State = "", string City = "")
        {
            string body = " has assigned you a Task: ";
            string TaskId = null;

            if (!string.IsNullOrEmpty(task.TaskId))
            {
                body = " has modified Task:  ";
            }


            if (string.IsNullOrEmpty(task.TaskId))
            {
                TaskId = admin.GetTaskId();
                task.TaskId = TaskId;
                task.TaskStatus = 1;
            }



            task.UpdatedBy = User.Identity.GetUserId();
            task.CreatedBy = User.Identity.GetUserId();
            task.StartDate = null;
            if (!String.IsNullOrEmpty(StartDate))
            {
                task.StartDate = DateTime.ParseExact(StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            task.SubscriberId = userDetails.SubscriberId;
            task.City = City;
            task.State = State;
            string mobile = admin.GetUserDetailsWithPhone(task.AssignedTo).PhoneNumber;
            bool result = admin.AddTaskMasters(task, ItemId, ItemType, ItemDescription, Unit, UnitPrice, ItemDuration, Actions, userDetails.DepartmentId);

            if (!string.IsNullOrEmpty(task.TaskId))
            {
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
                    admin.uploadFile(task.TaskId, attachment);
                }
            }
            string message1 = "A task (" + task.TaskId + ") has been assigned to you by " + generic.GetUserDetail(task.SubscriberId).Name; //eg "message hello ";                
          //  generic.sendSMSMessage(message1, mobile);
            generic.sendSMS(message1, mobile);
            admin.AddNotification(task.AssignedTo, task.CreatedBy, body + task.TaskId, "Task", task.TaskId, Status, DateTime.Now);

            return RedirectToAction("Task", "Task", new { Area = "CMS", result });
        }


        private void PopulateInvoiceStatus(object selectedValue = null)
        {
            var InvoiceStatusList = Global.GetInvoiceStatusList();
            ViewBag.InvoiceStatusList = new SelectList(InvoiceStatusList, "StatusName", "StatusName", selectedValue);
        }


        private void PopulateCourseMastereStatus(object selectedValue = null)
        {
            Student obj = new Student();
            var CourseMastereList = obj.GetCourseMastereList();
            ViewBag.Course = new SelectList(CourseMastereList, "CourseCode", "CourseName", selectedValue);
        }

        private void PopulateJobOrderType(string SubscriberId, object selectedValue = null)
        {
            var JobOrderTypeList = generic.GetJobOrderType(SubscriberId);
            ViewBag.JobOrderTypeList = new SelectList(JobOrderTypeList, "JobOrderTypeId", "JobOrderType", selectedValue);
        }

        private void PopulateInvoiceFrequency(object selectedValue = null)
        {
            var InvoiceFrequencyList = Global.GetFrequencyTypeList();
            ViewBag.InvoiceFrequencyList = new SelectList(InvoiceFrequencyList, "frequencytype", "frequencytype", selectedValue);
        }

        private void PopulateDurationList(Object selectName = null)
        {
            var DurationNameList = Global.GetDuration();
            SelectList Duration = new SelectList(DurationNameList, "Duration", "DurationName", selectName);
            ViewBag.Duration = Duration;
        }

        private void PopulateFeedBackFrequency(object selectedValue = null)
        {
            var FeedBackFrequencyList = Global.GetFrequencyTypeList();
            ViewBag.FeedBackFrequencyList = new SelectList(FeedBackFrequencyList, "frequencytype", "frequencytype", selectedValue);
        }

        private void PopulateTaskStatus(object selectedValue = null)
        {
            var TaskStatusList = Global.GetTaskStatus();
            ViewBag.TaskStatusList = new SelectList(TaskStatusList, "TaskStatus", "taskStatusName", selectedValue);
        }

        //private void PopulateAssignedToForManager(string SubscriberId, object selectedValue = null)
        //{
        //    Generic generic = new Generic();
        //    var AssignedToList = emsMgr.GetSubscriberWiseEmployeeList(SubscriberId).Where(e => e.ManagerLevel == false).ToList();
        //    ViewBag.AssignedToList = new SelectList(AssignedToList, "UserId", "Name", selectedValue);
        //}


        private void PopulateAssignedToForManager(string SubscriberId, object selectedValue = null)
        {
            Generic generic = new Generic();
            var AssignedToList = generic.GetEmployee(SubscriberId).Where(e => e.ManagerLevel == false).ToList(); ;
            ViewBag.AssignedToList = new SelectList(AssignedToList, "UserId", "Name", selectedValue);
        }

        private void PopulateAssignedTo(string SubscriberId, object selectedValue = null)
        {
            Generic generic = new Generic();
            var AssignedToList = generic.GetEmployee(SubscriberId);
            ViewBag.AssignedToList = new SelectList(AssignedToList, "UserId", "Name", selectedValue);
        }

        private void PopulateJobOrder(string SubscriberId, object selectedValue = null)
        {
            Generic generic = new Generic();
            var JobOrderList = generic.GetJobOrder().Where(c => c.SubscriberId == SubscriberId && c.JobOrderStatus != "Unassigned" && c.JobOrderStatus != "Completed").ToList();
            ViewBag.JobOrderList = new SelectList(JobOrderList, "JobOrderNumber", "JobOrderNumber", selectedValue);
        }


        private void PopulateAssignedToList(string SubscriberId, object selectedValue = null)
        {
            Generic generic = new Generic();
            var AssignedTo = generic.GetEmployee(SubscriberId);
            ViewBag.AssignedTo = new SelectList(AssignedTo, "UserId", "Name", selectedValue);
        }

        private void PopulateCurrency(Object selectedCurrency = null)
        {
            var query = generic.GetCurrency();
            SelectList CurrencyList = new SelectList(query, "Currency", "Currency", selectedCurrency);
            ViewBag.Currency = CurrencyList;
        }

        private void PopulateTaskType(string SubscriberId, object selectedValue = null)
        {
            var JobOrderTypeId = generic.GetJobOrderType(SubscriberId);
            ViewBag.JobOrderTypeId = new SelectList(JobOrderTypeId, "JobOrderTypeId", "JobOrderType", selectedValue);
        }

        private void PopulateJobOrderlist(string SubscriberId, object selectedValue = null)
        {
            Generic generic = new Generic();
            var JobOrderNumber = generic.GetJobOrder().Where(c => c.SubscriberId == SubscriberId).ToList();
            ViewBag.JobOrderNumber = new SelectList(JobOrderNumber, "JobOrderNumber", "JobOrderNumber", selectedValue);
        }

        private void PopulateTaskStatuslist(object selectedValue = null)
        {
            var TaskStatus = Global.GetTaskStatus();
            ViewBag.TaskStatus = new SelectList(TaskStatus, "TaskStatus", "taskStatusName", selectedValue);
        }

        [HttpPost]
        public ActionResult GetState(string CountryId)
        {
            int countryId;
            List<SelectListItem> StateId = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(CountryId))
            {
                countryId = Convert.ToInt32(CountryId);
                List<StatesMaster> States = userContext.StatesMaster.Where(x => x.CountryId == countryId).ToList();
                States.ForEach(x =>
                {
                    StateId.Add(new SelectListItem { Text = x.State, Value = x.StateId.ToString() });
                });
            }
            return Json(StateId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCity(string StateId)
        {
            int stateId;
            List<SelectListItem> CityId = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(StateId))
            {
                stateId = Convert.ToInt32(StateId);
                List<CityMaster> Cities = userContext.CityMaster.Where(x => x.StateId == stateId).ToList();
                Cities.ForEach(x =>
                {
                    CityId.Add(new SelectListItem { Text = x.City, Value = x.CityId.ToString() });
                });
            }
            return Json(CityId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UploadAttachment(string TaskId, string AssignTo)
        {
            string UserId = User.Identity.GetUserId();
            var UserDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewBag.TaskId = TaskId;
            ViewBag.AssignTo = AssignTo;
            return View();
        }

        [HttpPost]
        public ActionResult UploadAttachment(string TaskId, string AssignTo, string fileName, short TaskStatus = 6)
        {
            if (!string.IsNullOrEmpty(TaskId))
            {
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
                    cmsMgr.uploadTaskFile(TaskId, attachment);
                }
            }
            if (TaskId != null && TaskStatus > -1)
            {
                admin.UpdateTaskStatus(TaskId, TaskStatus, AssignTo);
                AssignTo = null;
                TaskStatus = -1;
            }

            return RedirectToAction("MyTasks", "Task");
        }

        //private void PopulateCountry(object selectedCountry = null)
        //{

        //    var query = generic.GetCountry();
        //    SelectList Countries = new SelectList(query, "CountryId", "Country", selectedCountry);
        //    ViewBag.CountryId = Countries;
        //}

        //private void PopulateState(int CountryId = 0, object selectedState = null)
        //{
        //    var query = generic.GetState(CountryId);
        //    SelectList States = new SelectList(query, "StateId", "State", selectedState);
        //    ViewBag.StateId = States;
        //}

        //private void PopulateCity(int StateId = 0, object selectedCity = null)
        //{
        //    var query = generic.GetCity(StateId);
        //    SelectList Cities = new SelectList(query, "CityId", "City", selectedCity);
        //    ViewBag.CityId = Cities;
        //}


        private void PopulateCountry(object selectedCountry = null)
        {

            var query = generic.GetCountry();
            SelectList Countries = new SelectList(query, "CountryId", "Country", selectedCountry);
            ViewBag.CountryId = Countries;
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


        private string GetUserRole(string UserId)
        {
            if (!String.IsNullOrEmpty(UserId))
            {
                if (UserManager.IsInRole(UserId, "Admin"))
                    return "Admin";
                else if (UserManager.IsInRole(UserId, "Client"))
                    return "Client";
                else if (UserManager.IsInRole(UserId, "Employee"))
                    return "Employee";
            }
            return string.Empty;
        }

    }
}