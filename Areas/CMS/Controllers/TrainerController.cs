using AJSolutions.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.Models;
using System.Threading.Tasks;
using PagedList;
//using System.Data.Linq.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;


namespace AJSolutions.Areas.CMS.Controllers
{
    public class TrainerController : Controller
    {
        Generic generic = new Generic();
        Student student = new Student();
        AdminManager admin = new AdminManager();
        UserDBContext db = new UserDBContext();
        CMSManager cms = new CMSManager();
        EMSManager ems = new EMSManager();

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

        // GET: CMS/Trainer
        //[Authorize(Roles = "Employee,Admin")]
        public ActionResult Index(int? page, int PageSize = 10, string Qualification = null, string Organization = null, string Name = null, string Domain = null, string Zone = null, string LanguageKnown = null, string City = null)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            if (userDetails.Role == "Employee")
            {
                var empdetails = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
                ViewData["EmpDetails"] = empdetails;
                ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            }
            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            ViewBag.Page = page;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            PopulateDomain();
            PopulateLanguageKnown();
            PopulateQualification();
            PopulateOrganization();
            PopulateZone();
            var InstructorLead = cms.GetInstructerLead(userDetails.SubscriberId);
            if (!string.IsNullOrEmpty(Qualification))
            {
                ViewBag.Qualifications = Qualification;
                var values = InstructorLead.Select(x => "," + x + ",");
                var query = from c in InstructorLead
                            where values.Any(i => ("," + c.Qualification + ",").Contains(Qualification))
                            select c;
                //InstructorLead = InstructorLead.Where(c => c.Qualification == Qualification).ToList();

                //var query = from c in InstructorLead
                //           where c.Qualification.Contains(Qualification) 

                //            select c;
                InstructorLead = query.ToList();
            }
            if (!string.IsNullOrEmpty(Organization))
            {
                ViewBag.Organizations = Organization;
                var values = InstructorLead.Select(x => "," + x + ",");
                var query = from c in InstructorLead
                            where values.Any(i => ("," + c.Organization + ",").Contains(Organization))
                            select c;
                //InstructorLead = InstructorLead.Where(c => c.Organization == Organization).ToList();
                //var query = from c in InstructorLead
                //            where  (c.Organization.Split(',').Contains(Organization) || c.Organization == Organization)
                //            select c;
                InstructorLead = query.ToList();

            }
            if (!string.IsNullOrEmpty(LanguageKnown))
            {
                ViewBag.LanguageKnowns = LanguageKnown;
                var values = InstructorLead.Select(x => "," + x + ",");
                var query = from c in InstructorLead
                            where values.Any(i => ("," + c.LanguageKnown + ",").Contains(LanguageKnown))
                            select c;
                //InstructorLead = InstructorLead.Where(c => c.Organization == Organization).ToList();
                //var query = from c in InstructorLead
                //            where  (c.Organization.Split(',').Contains(Organization) || c.Organization == Organization)
                //            select c;
                InstructorLead = query.ToList();

            }
            if (!string.IsNullOrEmpty(Name))
            {
                ViewBag.Name = Name;
                InstructorLead = (from u in InstructorLead where u.IName.Contains(Name.ToUpper()) select u).ToList();

            }
            if (!string.IsNullOrEmpty(City))
            {

                ViewBag.Cites = City;
                InstructorLead = (from u in InstructorLead where (u.City ?? "").Contains(City.ToUpper()) select u).ToList();

            }
            if (!string.IsNullOrEmpty(Zone))
            {
                ViewBag.Zones = Zone;
                InstructorLead = InstructorLead.Where(c => c.Zone == Zone).ToList();
            }
            if (!string.IsNullOrEmpty(Domain))
            {
                ViewBag.Domains = Domain;
                // InstructorLead = InstructorLead.Where(c => c.DomainExpertize == Domain).ToList();
                var values = InstructorLead.Select(x => "," + x + ",");
                var query = from c in InstructorLead
                            where values.Any(i => ("," + c.DomainExpertize + ",").Contains(Domain))
                            select c;
                //List<InstructorLeadProfileView> inmt= new List<InstructorLeadProfileView>(); 
                ////InstructorLead = (from u in InstructorLead where u.DomainExpertize.Contains(Domain) select u).ToList();
                //foreach (var item in InstructorLead)
                //{
                //    var InstructorLeadss = (from u in InstructorLead where item.DomainExpertize.StartsWith(Domain) select u);
                //    if(InstructorLeadss != null)
                //    {
                //        inmt.Add(item);
                //    }
                //}
                //InstructorLead = inmt;
                InstructorLead = query.ToList();
            }
            ViewBag.EmCount = InstructorLead.Count();
            return View(InstructorLead.ToPagedList(pageNumber, pageSize));
        }
        //[Authorize(Roles = "Employee,Admin")]
        public ActionResult Add(string InstructorId, string UserAction, string Status)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            var instructorleads = cms.GetInstructerLead(userDetails.SubscriberId).Where(i => i.InstructorId == InstructorId).FirstOrDefault();
            if (!string.IsNullOrEmpty(Status))
            {
                ViewBag.Status = Status;
            }
            if (userDetails.Role == "Employee")
            {
                var empdetails = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
                ViewData["EmpDetails"] = empdetails;
                ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            }
            if (InstructorId != null)
            {
                if (instructorleads.DOB != null)
                {
                    ViewBag.dob = instructorleads.DOB.Value.ToShortDateString();
                }

                PopulateCountry(101);
                PopulateState(101, instructorleads.StateId);
                PopulateCity(instructorleads.StateId, instructorleads.CityId);
                PopulateZone(instructorleads.Zone);
                PopulateGenderStatus(instructorleads.Gender);
                //qualification
                if (instructorleads.Qualification != null)
                {
                    var Qualification = instructorleads.Qualification;
                    string[] qualification = Qualification.Split(',');
                    PopulateQualification(qualification);
                }
                else
                {
                    PopulateQualification();
                }
                if (instructorleads.LanguageKnown != null)
                {
                    var LanguageKnown = instructorleads.LanguageKnown;
                    string[] languageknown = LanguageKnown.Split(',');
                    PopulateLanguageKnown(languageknown);
                }
                else
                {
                    PopulateLanguageKnown();
                }

                if (instructorleads.DomainExpertize != null)
                {
                    var Domain = instructorleads.DomainExpertize;
                    string[] domain = Domain.Split(',');
                    PopulateDomain(domain);
                }
                else
                {
                    PopulateDomain();
                }
                if (instructorleads.Organization != null)
                {
                    var Organization = instructorleads.Organization;
                    string[] organization = Organization.Split(',');
                    PopulateOrganization(organization);
                }
                else
                {
                    PopulateOrganization();
                }
                if (instructorleads.NibfProject != null)
                {
                    var NibfProject = instructorleads.NibfProject;
                    string[] nibfproject = NibfProject.Split(',');
                    PopulateNibfProject(nibfproject);
                }
                else
                {
                    PopulateNibfProject();
                }
                if (instructorleads.Specialization != null)
                {
                    var Specialization = instructorleads.Specialization;
                    string[] specialization = Specialization.Split(',');
                    PopulateSpecialization(specialization);
                }
                else
                {
                    PopulateSpecialization();
                }


                ViewBag.UserAction = UserAction;
            }
            else
            {
                PopulateCountry(101);
                PopulateState(101);
                PopulateCity();
                PopulateQualification();
                PopulateLanguageKnown();
                PopulateDomain();
                PopulateOrganization();
                PopulateNibfProject();
                PopulateSpecialization();
                PopulateZone();
                PopulateGenderStatus();
            }

            return View(instructorleads);
        }
        [HttpPost]
        //[Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult> Add(InstructorLeadProfileView instructor, string[] Qualification
            , string[] Domain, string[] Organization, string[] LanguageKnown, string[] Specialization, string[] NibfProject, HttpPostedFileBase uploadPhoto, string UserAction, string fileName, string InstructorId, int TrainnerCity = 0)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            bool Empanelled = false;
            // string UpdatedBy = User.Identity.GetUserId();
            var RStatus = "";
            //foreach (string file in Request.Files)
            //{
            //    HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
            //    //tms.uploadFile(TrainingId, attachment);
            //}
            if (UserAction == "Edit")
            {
                instructor.InstructorId = InstructorId;
                //instructor.CityId = TrainnerCity;
                if (Qualification != null)
                {
                    instructor.Qualification = string.Join(",", Qualification);
                    foreach (string item in Qualification)
                    {
                        var qualification = db.QualificationMaster.Where(q => q.Qualification.Contains(item)).Count();
                        if (qualification == 0)
                        {
                            cms.AddQualification(item);
                        }
                    }
                }
                if (Domain != null)
                {
                    instructor.DomainExpertize = string.Join(",", Domain);

                    foreach (string item in Domain)
                    {
                        var domainexpertize = db.DomainMaster.Where(q => q.Domain.Contains(item)).Count();
                        if (domainexpertize == 0)
                        {
                            cms.AddDomain(item);
                        }
                    }
                }
                if (Organization != null)
                {
                    instructor.Organization = string.Join(",", Organization);
                    foreach (string item in Organization)
                    {
                        var organization = db.OrganizationMaster.Where(q => q.Organization.Contains(item)).Count();
                        if (organization == 0)
                        {
                            cms.AddOrganization(item);
                        }
                    }
                }
                if (LanguageKnown != null)
                {

                    instructor.LanguageKnown = string.Join(",", LanguageKnown);
                }
                if (Specialization != null)
                {
                    foreach (string item in Specialization)
                    {
                        var specialization = db.SpecializationMaster.Where(q => q.Specialization.Contains(item)).Count();
                        if (specialization == 0)
                        {
                            cms.Addspecialization(item);
                        }
                    }
                    instructor.Specialization = string.Join(",", Specialization);
                }
                if (NibfProject != null)
                {
                    foreach (string item in NibfProject)
                    {
                        var project = db.ProjectMaster.Where(q => q.Project.Contains(item)).Count();
                        if (project == 0)
                        {
                            cms.Addproject(item);
                        }
                    }
                    instructor.NibfProject = string.Join(",", NibfProject);
                }
                instructor.Empanelled = Empanelled;
                instructor.SubscriberId = userDetails.SubscriberId;
                instructor.UpdatedBy = UserId;

                bool res = cms.AddInstructorLeadProfile(instructor, uploadPhoto);

            }
            else
            {

                string RoleId = "Employee";
                //string savestatus = "";
                // if (employee.UserId == null) { 
                var EmailExists = admin.GetLoginDetails(instructor.Email);
                var instructorleads = cms.GetInstructerLead(userDetails.SubscriberId);
                InstructorLeadProfileView instructorexist = new InstructorLeadProfileView();
                if (EmailExists != null)
                {
                    instructorexist = instructorleads.Where(i => i.InstructorId == EmailExists.Id).FirstOrDefault();

                }
                if (EmailExists != null && instructorleads == null)
                {
                    instructor.InstructorId = EmailExists.Id;
                    //RStatus = "EmailId Exist";

                    // return Json("EmailExists", JsonRequestBehavior.AllowGet);
                    if (Qualification != null)
                    {
                        instructor.Qualification = string.Join(",", Qualification);
                        foreach (string item in Qualification)
                        {
                            var qualification = db.QualificationMaster.Where(q => q.Qualification.Contains(item)).Count();
                            if (qualification == 0)
                            {
                                cms.AddQualification(item);
                            }
                        }
                    }
                    if (Domain != null)
                    {
                        instructor.DomainExpertize = string.Join(",", Domain);

                        foreach (string item in Domain)
                        {
                            var domainexpertize = db.DomainMaster.Where(q => q.Domain.Contains(item)).Count();
                            if (domainexpertize == 0)
                            {
                                cms.AddDomain(item);
                            }
                        }
                    }
                    if (Organization != null)
                    {
                        instructor.Organization = string.Join(",", Organization);
                        foreach (string item in Organization)
                        {
                            var organization = db.OrganizationMaster.Where(q => q.Organization.Contains(item)).Count();
                            if (organization == 0)
                            {
                                cms.AddOrganization(item);
                            }
                        }
                    }
                    if (LanguageKnown != null)
                    {

                        instructor.LanguageKnown = string.Join(",", LanguageKnown);
                    }
                    if (Specialization != null)
                    {
                        foreach (string item in Specialization)
                        {
                            var specialization = db.SpecializationMaster.Where(q => q.Specialization.Contains(item)).Count();
                            if (specialization == 0)
                            {
                                cms.Addspecialization(item);
                            }
                        }
                        instructor.Specialization = string.Join(",", Specialization);
                    }
                    if (NibfProject != null)
                    {
                        foreach (string item in NibfProject)
                        {
                            var project = db.ProjectMaster.Where(q => q.Project.Contains(item)).Count();
                            if (project == 0)
                            {
                                cms.Addproject(item);
                            }
                        }
                        instructor.NibfProject = string.Join(",", NibfProject);
                    }
                    instructor.Empanelled = Empanelled;
                    instructor.SubscriberId = userDetails.SubscriberId;
                    instructor.UpdatedBy = UserId;
                    instructor.CityId = TrainnerCity;

                    bool res = cms.AddInstructorLeadProfile(instructor, uploadPhoto);

                }
                else
                {
                    var user = new ApplicationUser { UserName = instructor.Email, Email = instructor.Email, PhoneNumber = instructor.PhoneNumber, EmailConfirmed = true };
                    var result = await UserManager.CreateAsync(user, "changeme");
                    var status = UserManager.AddToRole(user.Id, RoleId);
                    instructor.InstructorId = user.Id;

                    if (status.Succeeded)
                    {

                        //savestatus = "Succeeded";
                        //bool res = admin.AddEmployee(emp, UserId, UpdatedBy);
                        //if (RoleId.ToUpper() == "EMPLOYEE")
                        //    emsMgr.AddProfileTypeDetails(0, user.Id, "Default");

                        ////Sending Email To User
                        //string callbackUrl = await SendEmailConfirmationTokenAsync(subscriberDetail.Name, user.Id, "Account activation", userName, emp.AlternateContact, emp.Name);

                        //Sending SMS To User
                        //string mobile = emp.AlternateContact;
                        //string message1 = "Hello" + emp.Name + ",  you are added as a Employee in RECKONN by " + generic.GetUserDetail(userDetail.SubscriberId).Name;              
                        //generic.sendSMSMessage(message1, mobile);
                        if (Qualification != null)
                        {
                            instructor.Qualification = string.Join(",", Qualification);
                            foreach (string item in Qualification)
                            {
                                var qualification = db.QualificationMaster.Where(q => q.Qualification.Contains(item)).Count();
                                if (qualification == 0)
                                {
                                    cms.AddQualification(item);
                                }
                            }
                        }
                        if (Domain != null)
                        {
                            instructor.DomainExpertize = string.Join(",", Domain);

                            foreach (string item in Domain)
                            {
                                var domainexpertize = db.DomainMaster.Where(q => q.Domain.Contains(item)).Count();
                                if (domainexpertize == 0)
                                {
                                    cms.AddDomain(item);
                                }
                            }
                        }
                        if (Organization != null)
                        {
                            instructor.Organization = string.Join(",", Organization);
                            foreach (string item in Organization)
                            {
                                var organization = db.OrganizationMaster.Where(q => q.Organization.Contains(item)).Count();
                                if (organization == 0)
                                {
                                    cms.AddOrganization(item);
                                }
                            }
                        }
                        if (LanguageKnown != null)
                        {

                            instructor.LanguageKnown = string.Join(",", LanguageKnown);
                        }
                        if (Specialization != null)
                        {
                            foreach (string item in Specialization)
                            {
                                var specialization = db.SpecializationMaster.Where(q => q.Specialization.Contains(item)).Count();
                                if (specialization == 0)
                                {
                                    cms.Addspecialization(item);
                                }
                            }
                            instructor.Specialization = string.Join(",", Specialization);
                        }
                        if (NibfProject != null)
                        {
                            foreach (string item in NibfProject)
                            {
                                var project = db.ProjectMaster.Where(q => q.Project.Contains(item)).Count();
                                if (project == 0)
                                {
                                    cms.Addproject(item);
                                }
                            }
                            instructor.NibfProject = string.Join(",", NibfProject);
                        }
                        instructor.Empanelled = Empanelled;
                        instructor.SubscriberId = userDetails.SubscriberId;
                        instructor.UpdatedBy = UserId;
                        instructor.CityId = TrainnerCity;

                        bool res = cms.AddInstructorLeadProfile(instructor, uploadPhoto);

                    }
                }
            }
            return RedirectToAction("Add", "Trainer", new { area = "CMS", Status = RStatus });
        }
        //[Authorize(Roles = "Employee,Admin")]
        public ActionResult InstructorLeadsDetails(string InstructorId)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            if (userDetails.Role == "Employee")
            {
                var empdetails = ems.GetEmployeeBasicDetails(UserId).FirstOrDefault();
                ViewData["EmpDetails"] = empdetails;
                ViewData["CompanyLogo"] = cms.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            }
            var instructorleads = cms.GetInstructerLead(userDetails.SubscriberId).Where(i => i.InstructorId == InstructorId).FirstOrDefault();
            return View(instructorleads);

        }

        public ActionResult DownloadLeads(string SearchKey = null)
        {
            if (SearchKey == "undefined")
            {
                SearchKey = null;
            }

            var userdetail = generic.GetUserDetail(User.Identity.GetUserId());
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/InstructorLeads.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@SubscriberId", userdetail.SubscriberId));
            cmd.Parameters.Add(new SqlParameter("@Searchkey", SearchKey));
            cmd.Connection = thisConnection;
            string MyDataSource1 = "USP_GetInstructorLead";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];
            ReportParameter[] parms = new ReportParameter[2];
            parms[0] = new ReportParameter("SubscriberId", userdetail.SubscriberId);
            parms[1] = new ReportParameter("Searchkey", SearchKey);
            rptViewer.LocalReport.SetParameters(parms);
            rptViewer.LocalReport.DataSources.Add(reportDataSource);
            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            string extension = string.Empty;
            byte[] bytes = rptViewer.LocalReport.Render("Excel", null);
            Response.Buffer = true;
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=InstructorLeads.xls");
            Response.BinaryWrite(bytes); // create the file   
            Response.Flush();
            return View();

        }
        [HttpPost]
        public ActionResult GetCity(string StateId)
        {
            int stateId;
            List<SelectListItem> CityId = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(StateId))
            {
                stateId = Convert.ToInt32(StateId);
                List<CityMaster> Cities = db.CityMaster.Where(x => x.StateId == stateId).ToList();
                Cities.ForEach(x =>
                {
                    CityId.Add(new SelectListItem { Text = x.City, Value = x.CityId.ToString() });
                });
            }
            return Json(CityId, JsonRequestBehavior.AllowGet);
        }
        //for Empanelled
        [HttpPost]
        //[Authorize(Roles = "Employee,Admin")]
        public ActionResult Empanelled(string UserId, string EmailId, string PhoneNumber, string Name, string Location)
        {
            string userId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(userId);
            EmployeeView emp = new EmployeeView();
            emp.UserId = UserId;
            emp.EmployeeId = null;
            emp.Emplanelled = true;
            emp.Name = Name;
            emp.DOB = null;
            emp.Gender = null;
            emp.MaritalStatus = null;
            emp.AlternateContact = PhoneNumber;
            emp.AlternateEmail = EmailId;
            emp.Nationality = "Indian";
            //emp.SubscriberId = userId;
            emp.DepartmentId = "FAC";
            emp.ManagerLevel = false;
            emp.ReportingAuthority = userDetails.SubscriberId;
            //emp.UpdatedBy = userId;
            emp.UpdatedOn = DateTime.UtcNow;
            emp.Deactivated = false;
            emp.FatherName = null;
            emp.SpouseName = null;
            emp.EmergencyContactName = null;
            emp.EmergencyContactNumber = null;
            emp.BloodGroup = null;
            emp.PhysicallyChallenged = false;
            emp.Location = Location;
            emp.MarriageDate = null;
            emp.DesignationId = 42;
            bool res = admin.AddEmployee(emp, userId, userId);
            if (res == true)
            {
                var query = (from q in db.InstructorLeadProfile
                             where q.InstructorId == UserId
                             select q).First();
                query.Empanelled = true;
                db.SaveChanges();
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UserExist(string UserName)
        {

            var EmailExists = admin.GetLoginDetails(UserName);
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            var instructorleads = cms.GetInstructerLead(userDetails.SubscriberId);
            InstructorLeadProfileView instructorexist = new InstructorLeadProfileView();
            if (EmailExists != null)
            {
                instructorexist = instructorleads.Where(i => i.InstructorId == EmailExists.Id).FirstOrDefault();

            }

            if (EmailExists != null && instructorexist != null)
            {


                return Json("EmailExists", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        //check phonenumberexist
        [HttpPost]
        public ActionResult PhoneExist(string phonenumber)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            var phoneexist = admin.GetLoginDetails(phonenumber);
            var instructorleads = cms.GetInstructerLead(userDetails.SubscriberId);
            InstructorLeadProfileView instructorexist = new InstructorLeadProfileView();
            if (phoneexist != null)
            {
                instructorexist = instructorleads.Where(i => i.InstructorId == phoneexist.Id).FirstOrDefault();

            }
            if (phoneexist != null && instructorexist != null)
            {


                return Json("PhoneExist", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
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

        private void PopulateQualification(object[] selectedQualification = null)
        {
            var query = generic.GetQualification();
            MultiSelectList Qualifications = new MultiSelectList(query, "Qualification", "Qualification", selectedQualification);
            ViewBag.Qualification = Qualifications;
        }

        private void PopulateDomain(object[] selectedDomain = null)
        {
            var query = generic.GetDomain();
            MultiSelectList Domains = new MultiSelectList(query, "Domain", "Domain", selectedDomain);
            ViewBag.Domain = Domains;
        }

        private void PopulateOrganization(object[] selectedOrganization = null)
        {
            var query = generic.GetOrganization();
            MultiSelectList Organizations = new MultiSelectList(query, "Organization", "Organization", selectedOrganization);
            ViewBag.Organization = Organizations;
        }

        private void PopulateLanguageKnown(object[] selectedLanguage = null)
        {
            var query = Generic.Language();
            MultiSelectList LanguageList = new MultiSelectList(query, "Language", "Language", selectedLanguage);
            ViewBag.LanguageKnown = LanguageList;
        }
        //for zone
        private void PopulateZone(object selectedZone = null)
        {
            var query = Generic.Zone();
            SelectList ZoneList = new SelectList(query, "Zone", "Zone", selectedZone);
            ViewBag.Zone = ZoneList;
        }
        private void PopulateSpecialization(object[] selectedSpecialization = null)
        {
            var query = generic.GetSpecialization();
            MultiSelectList Specializations = new MultiSelectList(query, "Specialization", "Specialization", selectedSpecialization);
            ViewBag.Specialization = Specializations;
        }

        private void PopulateNibfProject(object[] selectedProject = null)
        {
            var query = generic.GetProject();
            MultiSelectList Projects = new MultiSelectList(query, "Project", "Project", selectedProject);
            ViewBag.NibfProject = Projects;
        }
        private void PopulatePaging(object selectedValue = null)
        {

            var PageList = generic.GetPaging();
            ViewBag.PageSize = new SelectList(PageList, "PageSize", "PageSize", selectedValue);
        }
        private void PopulateGenderStatus(object selectedValue = null)
        {
            var GenderList = Global.GetGenderList();
            SelectList Gender = new SelectList(GenderList, "Genderid", "Gender", selectedValue);
            ViewBag.Gender = Gender;
        }
    }
}