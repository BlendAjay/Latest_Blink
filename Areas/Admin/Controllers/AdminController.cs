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
using System.Threading.Tasks;


namespace AJSolutions.Areas.Admin.Controllers
{
   
    public class AdminController : Controller
    {

        AdminManager admin = new AdminManager();
        Generic generic = new Generic();
        CMSManager cms = new CMSManager();

        UserDBContext userContext = new UserDBContext();

        EMSManager emsMgr = new EMSManager();

        Student student = new Student();



        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

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

        Generic Gen = new Generic();
        // GET: Admin/Admin
        public ActionResult Index()
        {

            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;

            var details = admin.GetCountValue();
            return View(details);

        }

        public ActionResult Employees()
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            var EmployeeList = admin.GetAllEmployeeList().Where(c => c.DepartmentId != "ADI").ToList();

            return View(EmployeeList);

        }

        public ActionResult Candidates()
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            var CandidateList = admin.GetAllCandidateList().ToList();

            return View(CandidateList);
        }


        public ActionResult ThirdParty()
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            var vendorList = admin.GetAllClientList().Where(c => c.Department != "Client" && c.Department != "Admin" && c.Department != null).ToList();
            return View(vendorList);
        }

        public ActionResult Clients()
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            var clientList = admin.GetAllClientList().Where(c => c.Department == "Client").ToList();

            return View(clientList);
        }

        public ActionResult Admins()
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            var AdminList = admin.GetAllClientList().Where(c => c.Department == "Admin").ToList();
            return View(AdminList);

        }
        public ActionResult AdminProfile(string SubscriberId)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserRole = GetUserRole(userdetails.SubscriberId);
            ViewData["Address"] = userContext.Address.Where(u => u.CorporateId == SubscriberId).ToList();
            ViewData["Company"] = userContext.CompanyProfile.Find(SubscriberId);
            return View(generic.GetUserDetail(SubscriberId));
        }

        [HttpGet]
        ////[Authorize(Roles = "Administrator")]
        public ActionResult Role(string Role, string RoleId, string useraction = "Add")
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.Role = Role;
            ViewBag.RoleId = RoleId;
            ViewBag.Action = useraction;
            var myRole = Gen.GetRoles();
            return View(myRole);
        }

        [HttpPost]
        ////[Authorize(Roles = "Administrator")]
        public ActionResult Role(AddUserRoleViewModel UserRole, string Role, string RoleId, string useraction)
        {
            bool result = admin.AddRole(RoleId, Role);

            return RedirectToAction("Role", "Admin");
        }

        ////[Authorize(Roles = "Administrator")]
        public ActionResult RemoveRole(string RoleId)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            bool result = admin.DeleteRole(RoleId);
            return RedirectToAction("Role", "Admin");
        }



        [HttpGet]
        ////[Authorize(Roles = "Administrator")]
        public ActionResult ModuleRoles()
        {
            PopulateRoles();
            PopulateModule();
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            return View(admin.GetAllModulesRoles());
        }

        [HttpPost]
        ////[Authorize(Roles = "Administrator")]
        public ActionResult ModuleRoles(string RoleId, string ModuleId)
        {
            bool result = admin.AddModuleRole(ModuleId, RoleId);

            return RedirectToAction("ModuleRoles");
        }

        //[Authorize(Roles = "Administrator")]
        public ActionResult RemoveModuleRoles(string RoleId, string ModuleId)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            bool result = admin.DeleteModuleRole(RoleId, ModuleId);
            return RedirectToAction("ModuleRoles");
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Registration(UserRegistrationViewModel model, string RoleAccess)
        {

            if (ModelState.IsValid)
            {

                var user = new ApplicationUser { UserName = admin.GenerateUserName(), Email = model.Email, PhoneNumber = model.PhoneNumber, EmailConfirmed = true };
                var result = await UserManager.CreateAsync(user, "cahngeme");
                if (result.Succeeded)
                {
                    var status = UserManager.AddToRole(user.Id, "Admin");

                    if (status.Succeeded)
                    {
                        // Assign user to Role
                        //string role = admin.GetAllRoleAcessLevelMapping(RoleAccess).FirstOrDefault().Role ;

                        // UserManager.AddToRole(user.Id, role);

                        admin.AddUserProfileDetails(user.Id, model.Name, RoleAccess);
                    }

                }

            }
            //PopulateRoleAccessLevel();
            return View();
        }

        ////[Authorize(Roles = "Administrator")]
        public ActionResult UserDetail()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            return View(admin.GetUserDetails());
        }

        [HttpGet]
        //[Authorize(Roles = "Administrator")]
        public ActionResult EducationLevelMaster(string EducationLevelName, string useraction = "Add", short EducationLevelId = 0)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.EducationLevelName = EducationLevelName;
            ViewBag.EducationLevelId = EducationLevelId;
            ViewBag.Action = useraction;
            var educationlevel = admin.GetEducationLevel();
            return View(educationlevel);
        }


        [HttpPost]
        //[Authorize(Roles = "Administrator")]
        public ActionResult EducationLevelMaster(EducationLevelMaster EducationLevelMaster, string EducationLevelName, string useraction, short EducationLevelId = 0)
        {
            bool result = admin.AddEducationLevel(EducationLevelId, EducationLevelName);

            return RedirectToAction("EducationLevelMaster", "Admin");
        }

        [HttpGet]
        //[Authorize(Roles = "Administrator")]
        public ActionResult IdentificationTypeMaster(string IdentificationTypeName, string useraction = "Add", short IdentificationTypeId = 0)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.IdentificationTypeName = IdentificationTypeName;
            ViewBag.IdentificationTypeId = IdentificationTypeId;
            ViewBag.Action = useraction;
            var identificationtype = admin.GetIdentificationType();
            return View(identificationtype);
        }

        [HttpPost]
        //[Authorize(Roles = "Administrator")]
        public ActionResult IdentificationTypeMaster(IdentificationTypeMaster IdentificationTypeMaster, string IdentificationTypeName, string useraction, short IdentificationTypeId = 0)
        {
            bool result = admin.AddIdentificationType(IdentificationTypeId, IdentificationTypeName);

            return RedirectToAction("IdentificationTypeMaster", "Admin");
        }




        private void PopulateDepartment(object selectedDepartment = null)
        {
            var query = generic.GetDepartments();
            SelectList DepartmentList = new SelectList(query, "DepartmentId", "Department", selectedDepartment);
            ViewBag.DepartmentId = DepartmentList;
        }


        [AllowAnonymous]
        public async Task<ActionResult> GeneratePassword(string username, DateTime? Registered)
        {
            var user = await UserManager.FindByNameAsync(username);
            if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                //  Warning("User doesn't exist with email: " + username, true);
                return RedirectToAction("UserDetail", "Admin");
            }

            //if (Registered.ToString() == null || Registered.ToString() == "")
            //{
            //    UserManager.AddToRole(user.Id, "JobSeeker");
            //    generic.UpdateUserHistoryRegistedOn(user.Id);
            //}

            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code, title = "GENERATE" }, protocol: Request.Url.Scheme);

            var msgBody = "You are invited by NIBF. Update your profile for Employee Record." +
                "<br/><br/><br/>You can start updating your profile by generating password" +
                // string note = "Please email us on social@nibf.in if you face any issues." +
           "<br/> Token will be valid for 48 hours. To regenerate token go to" + " <a href='http://www.jobenablers.com' target='_blank'>Login</a>" + " and put your credentials then it will regenerate your token.";

            await UserManager.SendEmailAsync(user.Id, "Invited by NIBF", msgBody);
            // Success("Invitation sent successfully", true);
            return RedirectToAction("UserDetail", "Admin");
        }

        [HttpGet]
        public ActionResult ItemTypeMaster(Int64 ItemTypeId = 0)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewData["ItemType"] = admin.GetItemTypeMaster().Where(i => i.CorporateId == userDetail.SubscriberId).ToList();
            return View(userContext.ItemTypeMasters.Find(ItemTypeId));
        }

        [HttpPost]
        public ActionResult ItemTypeMaster(ItemTypeMasters ITM)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ITM.CorporateId = UserDetail.SubscriberId;
            admin.AddItemTypeMaster(ITM.ItemTypeId, ITM.ItemTypeName, ITM.CorporateId);
            return RedirectToAction("ItemTypeMaster", "Admin");
        }
        //get method for EngagementType

        //

        public ActionResult RemoveItemType(Int64 Id)
        {
            var removeItem = userContext.ItemTypeMasters.Find(Id);

            if (removeItem != null)
            {
                userContext.ItemTypeMasters.Remove(removeItem);
                userContext.SaveChanges();
            }
            return RedirectToAction("ItemTypeMaster", "Admin");
        }

        //Created by ; vikas pandey
        //17/11/2017
        //LeaveSchemeMaster
        //[Authorize(Roles = "Administrator")]
        public ActionResult LeaveScheme(int SchemeId = 0)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewData["SchemeType"] = userContext.LeaveSchemeMaster.ToList();
            return View(userContext.LeaveSchemeMaster.Find(SchemeId));
        }
        [HttpPost]
        //[Authorize(Roles = "Administrator")]
        public ActionResult LeaveScheme(LeaveSchemeMaster Leavescheme)
        {
            admin.AddLeaveSchemeType(Leavescheme.SchemeId, Leavescheme.SchemeName);
            return RedirectToAction("LeaveScheme", "Admin");
        }

        //[Authorize(Roles = "Administrator")]
        public ActionResult RemoveLeaveScheme(int Id)
        {
            var removeItem = userContext.LeaveSchemeMaster.Find(Id);

            if (removeItem != null)
            {
                userContext.LeaveSchemeMaster.Remove(removeItem);
                userContext.SaveChanges();
            }
            return RedirectToAction("LeaveScheme", "Admin");
        }


        //Created By Vikas Pandey
        //17/11/2017

        //[Authorize(Roles = "Administrator")]
        public ActionResult LeaveTypeMaster(string LeaveTypeId = "")
        {
            PopulateLeaveCategory(LeaveTypeId);
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            PopulateLeaveCategory();
            ViewData["LeaveType"] = userContext.LeaveType.ToList();
            return View(userContext.LeaveType.Find(LeaveTypeId));
        }
        [HttpPost]
        //[Authorize(Roles = "Administrator")]
        public ActionResult LeaveTypeMaster(LeaveType leave, string LeaveCategory)
        {
            admin.AddLeaveType(leave.LeaveTypeId, leave.LeaveTypeName, LeaveCategory);
            return RedirectToAction("LeaveTypeMaster", "Admin");
        }

        //[Authorize(Roles = "Administrator")]
        public ActionResult RemoveLeaveTypeMaster(string Id = "")
        {
            var removeLeavetype = userContext.LeaveType.Find(Id);

            if (removeLeavetype != null)
            {
                userContext.LeaveType.Remove(removeLeavetype);
                userContext.SaveChanges();
            }
            return RedirectToAction("LeaveTypeMaster", "Admin");
        }

        //[Authorize(Roles = "Administrator")]
        public ActionResult IncomeTaxSlab(string UserAction, bool Data = false, Int64 IncomeTaxSlabId = 0)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            if (Data == true)
            {
                ViewBag.Data = "Succeed";
            }
            if (UserAction == "Delete" && IncomeTaxSlabId > 0)
            {
                admin.DeleteIncomeTaxSlab(IncomeTaxSlabId);
                ViewBag.Result = "Deleted";
                return View();
            }
            var AllIncomeTaxSlab = userContext.IncomeTaxSlab.ToList();
            ViewData["Slabs"] = AllIncomeTaxSlab;
            var SelectedIncomeTaxSlabId = AllIncomeTaxSlab.Where(c => c.IncomeTaxSlabId == IncomeTaxSlabId).FirstOrDefault();
            return View(SelectedIncomeTaxSlabId);
        }

        [HttpPost]
        //[Authorize(Roles = "Administrator")]
        public ActionResult IncomeTaxSlab(float IncomeTaxSlabFrom, float IncomeTaxSlabTo, Int16 IncomeTaxRate, Int16 Educationcess, Int16 SecondaryAndHigherEducationCess, Int64 IncomeTaxSlabId = 0)
        {
            bool res = admin.AddIncomeTaxSlabs(IncomeTaxSlabId, IncomeTaxSlabFrom, IncomeTaxSlabTo, IncomeTaxRate, Educationcess, SecondaryAndHigherEducationCess);
            return RedirectToAction("IncomeTaxSlab", "Admin");
        }

        private void PopulateModule(object selectedModule = null)
        {
            Generic generic = new Generic();
            var query = generic.GetModules();
            SelectList ModuleList = new SelectList(query, "ModuleId", "Module", selectedModule);
            ViewBag.ModuleId = ModuleList;
        }

        private void PopulateRoles(object selectedRole = null)
        {
            Generic generic = new Generic();
            var query = generic.GetRoles();
            SelectList RoleList = new SelectList(query, "RoleId", "Role", selectedRole);
            ViewBag.RoleId = RoleList;
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

        private void PopulateFeedBackFrequency(object selectedValue = null)
        {
            var FeedBackFrequencyList = Global.GetFrequencyTypeList();
            ViewBag.FeedBackFrequencyList = new SelectList(FeedBackFrequencyList, "frequencytype", "frequencytype", selectedValue);
        }

        private void PopulateTaskStatus(object selectedValue = null)
        {
            var TaskStatusList = Global.GetTaskStatusList();
            ViewBag.TaskStatusList = new SelectList(TaskStatusList, "TaskStatus", "taskStatusName", selectedValue);
        }


        private void PopulateJobOrder(object selectedValue = null)
        {
            Generic generic = new Generic();
            var JobOrderList = generic.GetJobOrder();
            ViewBag.JobOrderList = new SelectList(JobOrderList, "JobOrderNumber", "JobOrderNumber", selectedValue);
        }
        private void PopulateLeaveCategory(object selectedValue = null)
        {
            var LeaveCategory = Global.LeaveCategoryL();
            ViewBag.LeaveCategory = new SelectList(LeaveCategory, "LeaveCategoryId", "LeaveCategoryName", selectedValue);
        }

        [HttpGet]
        public JsonResult GetItemTypes()
        {
            var itemTypeList = admin.GetItemTypeMaster().Where(t => t.CorporateId == generic.GetUserDetail(User.Identity.GetUserId()).SubscriberId).ToList();
            return Json(itemTypeList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDuration()
        {

            return Json(Global.GetDuration(), JsonRequestBehavior.AllowGet);
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
        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        #endregion

        private void PopulateSalaryType(string HeadType = null)
        {
            Generic generic = new Generic();
            var TypeList = generic.GetPayrollSalaryType().ToList();
            ViewBag.SalaryType = new SelectList(TypeList, "salraryTypeId", "salraryTypeName", HeadType);
        }

        public ActionResult AdminMasters()
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewData["CompanyLogo"] = cms.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            return View();
        }
        private void PopulateCurrency(Object selectedCurrency = null)
        {
            var query = generic.GetCurrency();
            SelectList CurrencyList = new SelectList(query, "Currency", "Currency", selectedCurrency);
            ViewBag.Currency = CurrencyList;
        }
    }
}
