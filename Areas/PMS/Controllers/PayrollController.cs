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
using System.IO;
using System.Net.Mail;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharp;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using System.Net;
using System.Configuration;
using System.Diagnostics;
using SparkPost;
using System.Threading.Tasks;
using System.Web.UI;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Web.Configuration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using PagedList;
using System.Data;
using System.Data.SqlClient;
using AJSolutions.Areas.EMS.Models;
using AJSolutions.Areas.PMS.Models;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
namespace AJSolutions.Areas.PMS.Controllers
{
    public class PayrollController : Controller
    {
        // GET: PMS/Payroll
        EMSManager ems = new EMSManager();
        AdminManager admin = new AdminManager();
        Generic generic = new Generic();
        CMSManager cmsMgr = new CMSManager();
        EMSManager emsMgr = new EMSManager();
        PMSManager pms = new PMSManager();
        TMSManager tmsMgr = new TMSManager();
        BlobManager blobmanager = new BlobManager();
        UserDBContext userContext = new UserDBContext();
        private UserDBContext db = new UserDBContext();


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

        public ActionResult Index()
        {
            string UserId = User.Identity.GetUserId();
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult MyGrade(Int64 GradeId = 0, string UserAction = "Add", string ReturnResult = "")
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            if (!string.IsNullOrEmpty(ReturnResult))
            {
                ViewBag.ReturnResult = ReturnResult;
            }
            if (UserAction == "Delete" && GradeId > 0)
            {
                var details = RemoveGrade(GradeId);
            }

            GradeMaster GMaster = new GradeMaster();
            var grademaster = admin.GetGrade(UserDetail.SubscriberId);
            ViewData["GMaster"] = grademaster.ToList();
            if (GradeId != 0)
            {
                GMaster = grademaster.Where(a => a.GradeId == GradeId).FirstOrDefault();
            }

            return View(GMaster);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult MyGrade(GradeMaster GM)
        {

            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            GM.CorporateId = UserDetail.SubscriberId;
            string Status = "NA";

            var StatusExist = from jc in db.GradeMaster.Where(jc => jc.CorporateId == GM.CorporateId && jc.GradeName == GM.GradeName)
                              select jc;
            if (StatusExist.Count() == 0)
            {
                var result = emsMgr.AddGrade(GM.GradeId, GM.GradeName, GM.CorporateId);
                if (result)
                    Status = "Succeeded";
                else
                    Status = "Unsucceeded";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Status = "Exists";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveGrade(Int64 GradeId)
        {
            var removeItem = db.GradeMaster.Find(GradeId);
            var GradeIdDetails = db.EmpJoiningDetail.Where(c => c.GradeId == GradeId).ToList();
            if (GradeIdDetails == null)
            {
                return RedirectToAction("MyGrade", "Payroll", new { area = "PMS", ReturnResult = "INUSE" });
            }
            else
            {
                if (removeItem != null)
                {
                    db.GradeMaster.Remove(removeItem);
                    db.SaveChanges();
                }
                return RedirectToAction("MyGrade", "Payroll", new { area = "PMS", ReturnResult = "DELETED" });
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult EmployeePayroll()
        {
            EMSManager emsMgr = new EMSManager();
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            string UserId = User.Identity.GetUserId();
            ViewBag.UserRole = userDetail.Role;
            string SubscriberId = userDetail.SubscriberId;
            PopulateYear();
            PopulateMonth();
            if (userDetail.DepartmentId != "ADI")
            {
                PopulateEmployeeForManager(SubscriberId);
            }
            else
            {
                PopulateEmployee(SubscriberId);
            }
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult CorporatePayrollSettings()
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            string UserId = User.Identity.GetUserId();
            ViewBag.UserRole = userDetail.Role;
            string SubscriberId = userDetail.SubscriberId;
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult EmployeePayrollSettings()
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            string UserId = User.Identity.GetUserId();
            ViewBag.UserRole = userDetail.Role;
            string SubscriberId = userDetail.SubscriberId;
            if (userDetail.DepartmentId != "ADI")
            {
                PopulateEmployeeForManager(SubscriberId);
            }
            else
            {
                PopulateEmployee(SubscriberId);
            }
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult PayrollLeavsSetting(string LeaveId, string SalaryCalCulatedOn, string HolidayinSalary)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            var myLeaveList = admin.GetPayrollLeavsList(userdetails.SubscriberId);
            ViewData["LeaveList"] = myLeaveList.ToList();
            salryCalculatedOn(SalaryCalCulatedOn);
            holidayInSalary(HolidayinSalary);
            return View(db.PayrollLeavsSettings.Find(userdetails.SubscriberId, Convert.ToInt64(LeaveId)));
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult PayrollLeavsSetting(string LeaveName, string LeaveId, string NoofDays, string SalaryCalculatedon, string HolidayinSalary)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            salryCalculatedOn();
            holidayInSalary();
            var myLeaveList = admin.GetPayrollLeavsList(userdetails.SubscriberId);
            ViewData["LeaveList"] = myLeaveList.ToList();
            bool result = false;
            Int16 noofDays = 0, leaveId = 0, salaryCalculatedon = 0, holidayinSalary = 0;
            if (NoofDays != string.Empty || NoofDays != null)
            {
                noofDays = Convert.ToInt16(NoofDays);
            }
            if (SalaryCalculatedon != string.Empty || SalaryCalculatedon != null)
            {
                salaryCalculatedon = Convert.ToInt16(SalaryCalculatedon);
            }
            if (HolidayinSalary != string.Empty || HolidayinSalary != null)
            {
                holidayinSalary = Convert.ToInt16(HolidayinSalary);
            }
            if (LeaveId != string.Empty || LeaveId != null)
            {
                leaveId = Convert.ToInt16(LeaveId);
            }

            var Maxid = admin.MaxLeaveId(userdetails.SubscriberId);
            Int16 MaxLeaveId = 0;
            if (LeaveId == string.Empty || LeaveId == null)
            {
                MaxLeaveId = Convert.ToInt16(Convert.ToInt32(Maxid.MaxId) + 1);
            }
            else
            {
                MaxLeaveId = Convert.ToInt16(LeaveId);
            }
            result = admin.AddPayrollLeavesTypeMaster(userdetails.SubscriberId, LeaveName, noofDays, salaryCalculatedon, MaxLeaveId, holidayinSalary);
            return RedirectToAction("PayrollLeavsSetting", "Payroll", new { area = "PMS" });
        }


        public ActionResult RemoveLeaveSettings(string SubscriberId, Int64 LeaveId)
        {
            var removeItem = db.PayrollLeavsSettings.Find(SubscriberId, LeaveId);

            if (removeItem != null)
            {
                db.PayrollLeavsSettings.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("PayrollLeavsSetting", "Payroll", new { area = "PMS" });
        }

        /// <summary>
        /// Created by : Achal Jha
        /// Created on : 31-05-2016
        /// Created For: EPayroll Payroll Head Settings
        /// </summary>
        /// <param name="SubscriberId"></param>
        /// <param name="selectedValue"></param>
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult PayrollHeadSettings(string HeadId, DateTime? hfDateFrom = null, string DeductionCriteria = null)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            PayrollHeadSettingHeadList(HeadId);
            PayrollHeadCalculationMethodList(DeductionCriteria);
            var myHeadList = admin.GetPayrollHeadSettingList(userdetails.SubscriberId);
            ViewData["HeadList"] = myHeadList.ToList();
            if (hfDateFrom == null)
            {
                return View();
            }
            else
            {
                DateTime date = Convert.ToDateTime(hfDateFrom);
                //DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                date.ToString("yyyy-MM-dd");
                //return View(db.PayrollHeadSettings.Find(userdetails.SubscriberId,hfDateFrom, Convert.ToInt16(HeadId)));
                return View(admin.GetPayrollHeadSettingList(userdetails.SubscriberId, date, HeadId).FirstOrDefault());
            }
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult PayrollHeadSettings(Int16 HeadId, string hidheadText, Int16 MethodName, float Deduction)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            DateTime current = System.DateTime.Now;
            ViewData["UserProfile"] = userdetails;
            PayrollHeadSettingHeadList(HeadId.ToString());
            PayrollHeadCalculationMethodList(MethodName.ToString());
            var myHeadList = admin.GetPayrollHeadList();
            ViewData["HeadList"] = myHeadList.ToList();
            bool result = false;
            result = admin.AddPayrollHeadSettings(userdetails.SubscriberId, current, HeadId, hidheadText, Deduction, MethodName);
            return RedirectToAction("PayrollHeadSettings", "Payroll", new { area = "PMS" });
        }

        public ActionResult RemoveHeadSettings(string SubscriberId, DateTime? DateFrom = null, string HeadId = null)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            var removeItem = admin.GetPayrollHeadSettingList(userdetails.SubscriberId, DateFrom, HeadId).FirstOrDefault();

            if (removeItem != null)
            {
                db.PayrollHeadSettings.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("PayrollHeadSettings", "Payroll", new { area = "PMS" });
        }


        /// <summary>
        /// CreateBy : Vikash Das
        /// CreatedOn : 10-08-2017
        /// Purpose : Add letter Header,Content Body, and Footer
        /// </summary>
        /// <param name="ContentId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult DesignLetter(string savestatus, Int64 TemplateId = 0)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            LetterDesignView contentL = new LetterDesignView();
            string UserId = User.Identity.GetUserId();
            ViewBag.Status = savestatus;
            PopulateLetterTypeList(userdetails.UserId);
            ViewData["LetterPlaceHolder"] = admin.LetterFieldName();
            if (TemplateId != 0)
            {
                contentL = admin.GetLetterContent().Where(c => c.TemplateId == TemplateId).FirstOrDefault();
                PopulateLetterTypeList(userdetails.UserId, contentL.LetterTypeId);
            }
            return View(contentL);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AllowAnonymous]
        public ActionResult DesignLetter(CorporateTemplate corporateTemplate, HttpPostedFileBase[] files)
        {
            string UserId = User.Identity.GetUserId();
            string Status = "";
            bool result = false;
            var StatusExist = from lh in db.CorporateTemplate.Where(lh => lh.Name.ToUpper() == corporateTemplate.Name.ToUpper()
                                                                    && lh.CorporateId == UserId)
                              select lh;

            if (StatusExist.Count() != 0 && corporateTemplate.TemplateId == 0)
            {
                Status = "Exists";
                return RedirectToAction("DesignLetter", "Payroll", new { savestatus = Status });
            } if (StatusExist.Count() == 0 || corporateTemplate.TemplateId != 0)
            {
                result = admin.AddCorporateTemplate(corporateTemplate, UserId);
                if (result == true)
                {
                    if (corporateTemplate.SameAsCompanyLogo == false)
                    {
                        if (files != null)
                        {
                            foreach (string file in Request.Files)
                            {
                                HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
                                if (attachment != null && attachment.ContentLength > 0)
                                {
                                    string res = admin.uploadLetterLogoAttachmentFile(UserId, attachment, corporateTemplate.TemplateId);
                                }
                            }
                        }
                    }
                    var file1 = db.LetterLogoAttachment.Where(c => c.TemplateId == corporateTemplate.TemplateId).FirstOrDefault();
                    if (file1 != null)
                    {
                        if (corporateTemplate.SameAsCompanyLogo == true)
                        {
                            Int64 FileId = file1.FileId;
                            blobmanager.DeleteBlob(UserId.ToLower(), admin.GetLogoFileName(FileId).ToLower());
                            db.LetterLogoAttachment.Remove(file1);
                            db.SaveChanges();
                        }
                    }
                    Status = "Succeeded";
                    return RedirectToAction("DesignLetter", "Payroll", new { savestatus = Status });
                }
                else
                {
                    Status = "Unsucceeded";
                    return RedirectToAction("DesignLetter", "Payroll", new { savestatus = Status });
                }
            }
            return RedirectToAction("LContent", "Payroll", new { savestatus = Status });
        }


        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult MyLetter(Int64 TemplateId = 0)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            CorporateTemplate template = new CorporateTemplate();
            var temp = db.CorporateTemplate.Where(c => c.CorporateId == userdetails.UserId).ToList();
            if (TemplateId != 0)
            {
                template = temp.Where(c => c.TemplateId == TemplateId).FirstOrDefault();
            }
            ViewData["TemplateList"] = temp;
            return View(template);
        }


        /// <summary>
        /// Created By : Vikash Das
        /// Created On : 19-09-2017
        /// Purpose :  Genreate the letter based on employee
        /// </summary>
        /// <param name="savestatus"></param>
        /// <param name="TemplateId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult GenreateCorporateLetter(string savestatus, Int64 TemplateId = 0, string UserId = null)
        {
            UserViewModel userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewData["LetterPlaceHolder"] = admin.LetterFieldName();
            PopulateLetterTemplate(userdetails.UserId);
            PopulateEmployeeList(userdetails.SubscriberId);
            ViewBag.status = savestatus;
            ViewData["LetterList"] = db.CorporateLetter.Where(c => c.CorporateId == userdetails.UserId).ToList();
            CorporateTemplateView Template = new CorporateTemplateView();
            if (TemplateId != 0)
            {
                CorporateLetter Corporateletter = db.CorporateLetter.Where(c => c.TemplateId == TemplateId && c.UserId == UserId).FirstOrDefault();
                Int64 letterId = Corporateletter.LetterId;
                var CorporateTemplatelist = db.CorporateTemplate.Find(TemplateId);
                var letterheader = "";
                if (CorporateTemplatelist.Header != null)
                {
                    letterheader = CorporateTemplatelist.Header;
                    ViewData["HeaderDetail"] = letterheader;
                }
                string letterContent = "";
                ViewBag.LetterId = letterId;
                if (CorporateTemplatelist.Content != null)
                {
                    var con = HttpUtility.UrlDecode(CorporateTemplatelist.Content.Trim());
                    CorporateTemplate Header = db.CorporateTemplate.Find(CorporateTemplatelist.TemplateId);
                    var url = "";
                    Int64 FileId = 0;
                    if (CorporateTemplatelist.SameAsCompanyLogo == false)
                    {
                        var letterlogo = db.LetterLogoAttachment.Where(c => c.TemplateId == Header.TemplateId).FirstOrDefault();
                        if (letterlogo != null)
                        {
                            url = blobmanager.DownloadPublicBlob(CorporateTemplatelist.CorporateId.ToLower(), admin.GetLogoFileName(letterlogo.FileId).ToLower());
                            FileId = letterlogo.FileId;
                        }
                    }
                    else
                    {
                        var Adminlogo = db.AdminLogoFile.Where(c => c.CorporateId == CorporateTemplatelist.CorporateId).FirstOrDefault();
                        if (Adminlogo != null)
                        {
                            url = blobmanager.DownloadPublicBlob(userdetails.UserId.ToLower(), cmsMgr.GetFileNameCompanyLogo(Adminlogo.FileId).ToLower());
                            FileId = Adminlogo.FileId;
                        }
                    }
                    var headercon = "";
                    if (FileId != 0)
                    {

                        headercon = "<div style='height:100px;width:100%;'>" +
                                        "<div style='height:100px;width:20%;float:left;'>" +
                                        "<img src='" + url + "' style='height:100px;width:80px;'></div>" +
                                        "<div style='height:100px;width:80%;text-align:center;'>" + letterheader + "</div></div>";
                    }
                    else if (letterheader != null)
                    {
                        headercon = "<div style='height:100px;width:100%;'>" +
                                        "<div style='height:100px;width:20%;float:left;'>" +
                                        "<img style='height:100px;width:80px;border: 0;display:none'></div>" +
                                        "<div style='height:100px;width:80%;text-align:center;'>" + letterheader + "</div></div>";
                    }
                    else
                    {
                        headercon = "<div style='height:100px;width:100%;'>" +
                                 "<div style='height:100px;width:20%;float:left;'>" +
                                 "<div style='height:100px;width:80%;text-align:center;'>" + headercon + "</div></div>";
                    }
                    string[] Content;
                    if (!String.IsNullOrEmpty(con))
                    {
                        string[] pagebrk = { "<div style='page-break-after: always'><span style='display: none;'>&nbsp;</span></div>" };
                        pagebrk[0] = pagebrk[0].Replace("'", "\"");
                        Content = con.Split(pagebrk, StringSplitOptions.RemoveEmptyEntries);
                        if (Content.Length > 0)
                        {
                            for (int i = 0; i < Content.Length; i++)
                            {
                                if (i == 0)
                                {
                                    letterContent = letterContent + headercon + Content[i];
                                }
                                else
                                {
                                    letterContent = letterContent + pagebrk[0] + headercon + Content[i];
                                }
                            }
                        }
                        else
                        {
                            letterContent = headercon + con;
                        }
                    }
                    else
                    {
                        letterContent = headercon;
                    }
                }
                EmpDetailPlaceholderView emp = admin.GetplaceHolderValue(UserId).FirstOrDefault();
                var reference = Corporateletter.ReferenceNo;
                ViewData["employee"] = emp;
                var currentdate = DateTime.Now.ToString("dd-MMM-yy");
                letterContent = letterContent.Replace("[[Name]]", string.IsNullOrEmpty(emp.Name) ? string.Empty : emp.Name);
                letterContent = letterContent.Replace("[[DOB]]", string.IsNullOrEmpty(emp.DOB) ? string.Empty : emp.DOB);
                letterContent = letterContent.Replace("[[FatherName]]", string.IsNullOrEmpty(emp.FatherName) ? string.Empty : emp.FatherName);
                letterContent = letterContent.Replace("[[Gender]]", string.IsNullOrEmpty(emp.Gender) ? string.Empty : (emp.Gender == "MA" ? "Male" : "Female"));
                letterContent = letterContent.Replace("[[AlternateEmail]]", string.IsNullOrEmpty(emp.AlternateEmail) ? string.Empty : emp.AlternateEmail);
                letterContent = letterContent.Replace("[[AlternateContact]]", string.IsNullOrEmpty(emp.AlternateContact) ? string.Empty : emp.AlternateContact);
                letterContent = letterContent.Replace("[[ConfirmationDate]]", string.IsNullOrEmpty(emp.ConfirmationDate) ? string.Empty : emp.ConfirmationDate);
                letterContent = letterContent.Replace("[[JoiningDate]]", string.IsNullOrEmpty(emp.JoiningDate) ? string.Empty : emp.JoiningDate);
                letterContent = letterContent.Replace("[[ProbationPeriod]]", string.IsNullOrEmpty(emp.ProbationPeriod) ? string.Empty : emp.ProbationPeriod);
                letterContent = letterContent.Replace("[[AddressLine1]]", string.IsNullOrEmpty(emp.AddressLine1) ? string.Empty : emp.AddressLine1);
                letterContent = letterContent.Replace("[[AddressLine2]]", string.IsNullOrEmpty(emp.AddressLine2) ? string.Empty : emp.AddressLine2);
                letterContent = letterContent.Replace("[[PostalCode]]", string.IsNullOrEmpty(emp.PostalCode) ? string.Empty : emp.PostalCode);
                letterContent = letterContent.Replace("[[StatusName]]", string.IsNullOrEmpty(emp.StatusName) ? string.Empty : emp.StatusName);
                letterContent = letterContent.Replace("[[DesignationName]]", string.IsNullOrEmpty(emp.DesignationName) ? string.Empty : emp.DesignationName);
                letterContent = letterContent.Replace("[[CurrentDate]]", currentdate);
                letterContent = letterContent.Replace("[[WorkLocation]]", string.IsNullOrEmpty(emp.WorkLocation) ? string.Empty : emp.WorkLocation);
                letterContent = letterContent.Replace("[[CountryId]]", string.IsNullOrEmpty(emp.Country) ? string.Empty : emp.Country);
                letterContent = letterContent.Replace("[[StateId]]", string.IsNullOrEmpty(emp.State) ? string.Empty : emp.State);
                letterContent = letterContent.Replace("[[CityId]]", string.IsNullOrEmpty(emp.City) ? string.Empty : emp.City);
                letterContent = letterContent.Replace("[[CompanyAddressLine1]]", string.IsNullOrEmpty(emp.CompanyAddressLine1) ? string.Empty : emp.CompanyAddressLine1);
                letterContent = letterContent.Replace("[[CompanyAddressLine2]]", string.IsNullOrEmpty(emp.CompanyAddressLine2) ? string.Empty : emp.CompanyAddressLine2);
                letterContent = letterContent.Replace("[[CompanyPostalCode]]", string.IsNullOrEmpty(emp.CompanyPostalCode) ? string.Empty : emp.CompanyPostalCode);

                letterContent = letterContent.Replace("[[ReferenceNo]]", string.IsNullOrEmpty(reference) ? string.Empty : reference);
                Template.Content = letterContent.Replace("&nbsp;", " ");
                Template.LetterTypeId = Corporateletter.LetterTypeId;
                Template.LetterId = letterId;
                ViewBag.Content = letterContent;
                ViewBag.TemplateId = TemplateId;
                ViewBag.UserId = UserId;
            }
            return View(Template);
        }

        [HttpPost]
        public ActionResult GenreateCorporateLetter(CorporateLetter cletter, Int64 LetterId = 0)
        {
            string CorporateId = User.Identity.GetUserId();
            string Status = "";
            if (LetterId != 0)
            {
                Export(cletter.TemplateId, cletter.UserId);
                //Status = "Succeeded";
                //return RedirectToAction("GenreateCorporateLetter", "Payroll", new { savestatus = Status });
            }
            else
            {
                if (cletter.TemplateId != 0)
                {
                    CorporateTemplate temp = db.CorporateTemplate.Find(cletter.TemplateId);
                    if (cletter.UserId != null)
                    {
                        EmployeeBasicDetails emp = db.EmployeeBasicDetails.Find(cletter.UserId);
                        temp.Name = temp.Name.Replace("Template ", "");
                        temp.Name = temp.Name.Replace("Template1 ", "");
                        cletter.Name = temp.Name + "of(" + emp.Name + ")";
                    }
                }


                cletter.LetterTypeId = db.CorporateTemplate.Find(cletter.TemplateId).LetterTypeId;
                cletter.ReferenceNo = admin.GenerateReferenceNo(CorporateId, cletter.LetterTypeId);
                bool res = admin.AddCorporateLetter(cletter, CorporateId);
                if (res == true)
                {
                    Status = "succeeded";
                    return RedirectToAction("GenreateCorporateLetter", "Payroll", new { TemplateId = cletter.TemplateId, UserId = cletter.UserId, savestatus = Status });
                }
                else
                {
                    Status = "Unsucceeded";
                    return RedirectToAction("GenreateCorporateLetter", "Payroll", new { savestatus = Status });
                }
            }
            return RedirectToAction("GenreateCorporateLetter", "Payroll");
        }

        public FileResult Export(Int64 TemplateId = 0, string UserId = null)
        {
            string UId = User.Identity.GetUserId();
            UserViewModel userdetails = generic.GetUserDetail(UId);
            Byte[] bytes;
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                CorporateLetter Corporateletter = db.CorporateLetter.Where(c => c.TemplateId == TemplateId && c.UserId == UserId).FirstOrDefault();
                Int64 letterId = Corporateletter.LetterId;
                var CorporateTemplatelist = db.CorporateTemplate.Find(TemplateId);
                var letterheader = "";
                if (CorporateTemplatelist.Header != null)
                {
                    letterheader = CorporateTemplatelist.Header;
                    ViewData["HeaderDetail"] = letterheader;
                }
                string letterContent = "";
                ViewBag.LetterId = letterId;
                if (CorporateTemplatelist.Content != null)
                {
                    var con = HttpUtility.UrlDecode(CorporateTemplatelist.Content.Trim());
                    CorporateTemplate Header = db.CorporateTemplate.Find(CorporateTemplatelist.TemplateId);
                    var url = "";
                    Int64 FileId = 0;
                    if (CorporateTemplatelist.SameAsCompanyLogo == false)
                    {
                        var letterlogo = db.LetterLogoAttachment.Where(c => c.TemplateId == Header.TemplateId).FirstOrDefault();
                        if (letterlogo != null)
                        {
                            url = blobmanager.DownloadPublicBlob(CorporateTemplatelist.CorporateId.ToLower(), admin.GetLogoFileName(letterlogo.FileId).ToLower());
                            FileId = letterlogo.FileId;
                        }
                    }
                    else
                    {
                        var Adminlogo = db.AdminLogoFile.Where(c => c.CorporateId == CorporateTemplatelist.CorporateId).FirstOrDefault();
                        if (Adminlogo != null)
                        {
                            url = blobmanager.DownloadPublicBlob(userdetails.UserId.ToLower(), cmsMgr.GetFileNameCompanyLogo(Adminlogo.FileId).ToLower());
                            FileId = Adminlogo.FileId;
                        }
                    }
                    var headercon = "";
                    if (FileId != 0)
                    {
                        headercon = "<div style='height:100px;width:100%;'>" +
                                        "<div style='height:100px;width:100%;'>" +
                                        "<img src='" + url + "' style='height:100px;width:80px;'/></div></div>" +
                                        "<div style='height:50px;width:80%;text-align:center;float:left;'>" + letterheader + "</div>";
                        //"<div style='height:100px;width:100%;'>" +
                        //        "<div style='height:100px;width:20%;float:left;'>" +
                        //        "<img src= '" + url + "' style='height:100px;width:80px;' /></div>" +
                        //        "<div style='height:100px;width:80%;text-align:center;margin:40px;'>" + letterheader +
                        //        "</div></div>";
                    }
                    else if (letterheader != null)
                    {
                        headercon = "<div style='height:100px;width:100%;'>" +
                                        "<div style='height:100px;width:20%;float:left;'>" +
                                        "<img style='height:100px;width:80px;border: 0;display:none'/></div>" +
                                        "<div style='height:100px;width:80%;text-align:center;'>" + letterheader + "</div></div>";
                    }
                    else
                    {
                        headercon = "<div style='height:100px;width:100%;'>" +
                                 "<div style='height:100px;width:20%;float:left;'>" +
                                 "<div style='height:100px;width:80%;text-align:center;'>" + headercon + "</div></div>";
                    }
                    string[] Content;
                    if (!String.IsNullOrEmpty(con))
                    {
                        string[] pagebrk = { "<div style='page-break-after: always'><span style='display: none;'>&nbsp;</span></div>" };
                        pagebrk[0] = pagebrk[0].Replace("'", "\"");
                        Content = con.Split(pagebrk, StringSplitOptions.RemoveEmptyEntries);
                        if (Content.Length > 0)
                        {
                            for (int i = 0; i < Content.Length; i++)
                            {
                                if (i == 0)
                                {
                                    letterContent = letterContent + headercon + Content[i];
                                }
                                else
                                {
                                    letterContent = letterContent + pagebrk[0] + headercon + Content[i];
                                }
                            }
                        }
                        else
                        {
                            letterContent = headercon + con;
                        }
                    }
                    else
                    {
                        letterContent = headercon;
                    }
                }
                EmpDetailPlaceholderView emp = admin.GetplaceHolderValue(UserId).FirstOrDefault();
                var reference = Corporateletter.ReferenceNo;
                ViewData["employee"] = emp;
                var currentdate = DateTime.Now.ToString("dd-MMM-yy");
                letterContent = letterContent.Replace("[[Name]]", string.IsNullOrEmpty(emp.Name) ? string.Empty : emp.Name);
                letterContent = letterContent.Replace("[[DOB]]", string.IsNullOrEmpty(emp.DOB) ? string.Empty : emp.DOB);
                letterContent = letterContent.Replace("[[FatherName]]", string.IsNullOrEmpty(emp.FatherName) ? string.Empty : emp.FatherName);
                letterContent = letterContent.Replace("[[Gender]]", string.IsNullOrEmpty(emp.Gender) ? string.Empty : (emp.Gender == "MA" ? "Male" : "Female"));
                letterContent = letterContent.Replace("[[AlternateEmail]]", string.IsNullOrEmpty(emp.AlternateEmail) ? string.Empty : emp.AlternateEmail);
                letterContent = letterContent.Replace("[[AlternateContact]]", string.IsNullOrEmpty(emp.AlternateContact) ? string.Empty : emp.AlternateContact);
                letterContent = letterContent.Replace("[[ConfirmationDate]]", string.IsNullOrEmpty(emp.ConfirmationDate) ? string.Empty : emp.ConfirmationDate);
                letterContent = letterContent.Replace("[[JoiningDate]]", string.IsNullOrEmpty(emp.JoiningDate) ? string.Empty : emp.JoiningDate);
                letterContent = letterContent.Replace("[[ProbationPeriod]]", string.IsNullOrEmpty(emp.ProbationPeriod) ? string.Empty : emp.ProbationPeriod);
                letterContent = letterContent.Replace("[[AddressLine1]]", string.IsNullOrEmpty(emp.AddressLine1) ? string.Empty : emp.AddressLine1);
                letterContent = letterContent.Replace("[[AddressLine2]]", string.IsNullOrEmpty(emp.AddressLine2) ? string.Empty : emp.AddressLine2);
                letterContent = letterContent.Replace("[[PostalCode]]", string.IsNullOrEmpty(emp.PostalCode) ? string.Empty : emp.PostalCode);
                letterContent = letterContent.Replace("[[StatusName]]", string.IsNullOrEmpty(emp.StatusName) ? string.Empty : emp.StatusName);
                letterContent = letterContent.Replace("[[DesignationName]]", string.IsNullOrEmpty(emp.DesignationName) ? string.Empty : emp.DesignationName);
                letterContent = letterContent.Replace("[[CurrentDate]]", currentdate);
                letterContent = letterContent.Replace("[[WorkLocation]]", string.IsNullOrEmpty(emp.WorkLocation) ? string.Empty : emp.WorkLocation);
                letterContent = letterContent.Replace("[[CountryId]]", string.IsNullOrEmpty(emp.Country) ? string.Empty : emp.Country);
                letterContent = letterContent.Replace("[[StateId]]", string.IsNullOrEmpty(emp.State) ? string.Empty : emp.State);
                letterContent = letterContent.Replace("[[CityId]]", string.IsNullOrEmpty(emp.City) ? string.Empty : emp.City);
                letterContent = letterContent.Replace("[[CompanyAddressLine1]]", string.IsNullOrEmpty(emp.CompanyAddressLine1) ? string.Empty : emp.CompanyAddressLine1);
                letterContent = letterContent.Replace("[[CompanyAddressLine2]]", string.IsNullOrEmpty(emp.CompanyAddressLine2) ? string.Empty : emp.CompanyAddressLine2);
                letterContent = letterContent.Replace("[[CompanyPostalCode]]", string.IsNullOrEmpty(emp.CompanyPostalCode) ? string.Empty : emp.CompanyPostalCode);
                letterContent = letterContent.Replace("[[ReferenceNo]]", string.IsNullOrEmpty(reference) ? string.Empty : reference);
                letterContent = letterContent.Replace("<p>&nbsp;</p>", "<br/>");
                letterContent = letterContent.Replace("&nbsp;", " ");
                ViewBag.Content = letterContent;
                ViewBag.TemplateId = TemplateId;
                ViewBag.UserId = UserId;
                string GridHtml = letterContent;
                iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 20.0f, 20.0f, 20.0f, 20.0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                StringReader sr = new StringReader(GridHtml);

                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                string Name = Corporateletter.Name;
                Name = Name.Replace("'", "_");
                Name = Name.Replace(' ', '_');
                bytes = stream.ToArray();
                if (UserId != null && Corporateletter.CorporateId != null && Corporateletter.LetterTypeId != 0)
                {
                    var result = admin.uploadSendLetter(Corporateletter.CorporateId, UserId, Name, Corporateletter.LetterTypeId, bytes);
                    if (result == "Succeed")
                    {
                        byte[] file1 = null;
                        if (UserId != null)
                        {
                            Int64 fileId = db.SendLetter.Where(c => c.LetterTypeId == Corporateletter.LetterTypeId && c.UserId == UserId && c.CorporateId == Corporateletter.CorporateId).FirstOrDefault().FileId;
                            string fileName = "Letters/" + UserId + "/" + fileId + "/" + Name + ".pdf";

                            file1 = blobmanager.DownloadBlobClient(Corporateletter.CorporateId.ToLower(), fileName.ToLower());
                        }
                        string to = db.EmployeeBasicDetails.Find(UserId).AlternateEmail;//"019vikasdas@gmail.com";
                        var settings = ConfigurationManager.AppSettings;
                        var transmission = new Transmission();
                        transmission.Content.From.Email = ConfigurationManager.AppSettings["From_Email"].ToString();
                        transmission.Content.Subject = "Check Your LetterName";
                        transmission.Content.Text = "I have send attachment please check";
                        transmission.Content.Html = "I have send attachment please check";

                        transmission.Content.Attachments.Add(new SparkPost.Attachment()
                        {
                            Data = Convert.ToBase64String(file1),
                            Name = Corporateletter.Name,
                            Type = "application/pdf"
                        });
                        var recipient = new Recipient
                        {
                            Address = new SparkPost.Address { Email = to }
                        };
                        transmission.Recipients.Add(recipient);
                        var client = new Client(ConfigurationManager.AppSettings["Spark_Post_API"].ToString());
                        client.ApiKey = ConfigurationManager.AppSettings["Spark_Post_API"].ToString();
                        client.CustomSettings.SendingMode = SendingModes.Sync;
                        var response = client.Transmissions.Send(transmission);
                    }
                }
                return File(stream.ToArray(), "application/pdf", "Grid.pdf");
            }
        }

        //------------------End-------------------//

        /// <summary>
        /// CreateBy : Preeti Singh
        /// CreatedOn : 18-08-2017
        /// Purpose : Add Designation
        /// </summary>

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult Designation(Int64 DesignationId = 0)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var designation = admin.GetDesignation(userDetail.SubscriberId);
            ViewData["designation"] = designation;
            var designationforId = designation.Where(c => c.DesignationId == DesignationId).FirstOrDefault();

            return View(designationforId);

        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult Designation(string DesignationName, string DesignationId)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            bool result = ems.AddDesignation(DesignationName, userDetail.SubscriberId, DesignationId);
            var designation = admin.GetDesignation(userDetail.SubscriberId);
            return RedirectToAction("designation", "Payroll");
        }

        public ActionResult RemoveDesignation(Int64 DesignationId)
        {
            var removeItem = db.Designation.Find(DesignationId);
            if (removeItem != null)
            {
                db.Designation.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("Designation", "Payroll", new { area = "PMS" });
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult StatusMaster(Int16 StatusId = 0)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var statusMaster = admin.GetStatusMaster(userDetail.SubscriberId);
            ViewData["statusMaster"] = statusMaster.ToList();
            ViewBag.StatusId = StatusId;
            if (StatusId == 0)
            {
                return View(db.StatusMaster.Find(StatusId));
            }
            else
            {
                return View(db.StatusMaster.Find(Convert.ToInt16(StatusId)));
            }
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult StatusMaster(string StatusName, Int16 StatusId)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            bool result = ems.AddStatusMaster(StatusName, userDetail.SubscriberId, StatusId);
            var status = admin.GetStatusMaster(userDetail.SubscriberId);
            return RedirectToAction("StatusMaster", "Payroll");
        }

        public ActionResult RemoveStatusMaster(Int16 StatusId)
        {
            var removeItem = db.StatusMaster.Find(StatusId);

            if (removeItem != null)
            {
                db.StatusMaster.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("StatusMaster", "Payroll", new { area = "PMS" });

        }

        /// <summary>
        /// CreateBy : Anamika Pandey
        /// CreatedOn : 20-08-2017
        /// Purpose : Add Asset Status
        /// </summary>

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult AssetStatus(Int64 AssetStatusId = 0)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewBag.UserId = UserDetail.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            AssetStatus assetStatus = new AssetStatus();
            var assetStatuses = admin.GetAssetStatus(UserDetail.SubscriberId);
            ViewData["assetStatus"] = assetStatuses.ToList();
            if (AssetStatusId != 0)
            {
                assetStatus = assetStatuses.Where(a => a.AssetStatusId == AssetStatusId).FirstOrDefault();
            }

            return View(assetStatus);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult AssetStatus(AssetStatus AS)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            AS.CorporateId = UserDetail.SubscriberId;
            string Status = "NA";

            var StatusExist = from jc in db.AssetStatus.Where(jc => jc.CorporateId == AS.CorporateId && jc.AssetStatusName == AS.AssetStatusName)
                              select jc;
            if (StatusExist.Count() == 0)
            {
                var result = admin.AddAssetStatus(AS.AssetStatusId, AS.AssetStatusName, AS.CorporateId);
                if (result)
                    Status = "Succeeded";
                else
                    Status = "Unsucceeded";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Status = "Exists";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult RemoveAssetStatus(Int64 AssetStatusId)
        {
            var removeItem = db.AssetStatus.Find(AssetStatusId);

            if (removeItem != null)
            {
                db.AssetStatus.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("AssetStatus", "Payroll", new { area = "PMS" });
        }


        /// <summary>
        /// CreateBy : Anamika Pandey
        /// CreatedOn : 21-08-2017
        /// Purpose : Relation for nominee
        /// </summary>

        [HttpGet]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Relation(int RelationId = 0)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            Relation relation = new Relation();
            var Relations = admin.GetRelation(UserDetail.SubscriberId);
            ViewData["relation"] = Relations.ToList();
            if (RelationId != 0)
            {
                relation = Relations.Where(a => a.RelationId == RelationId).FirstOrDefault();
            }

            return View(relation);
        }

        [HttpPost]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Relation(Relation RS)
        {

            //admin.AddRelation(RS.RelationId, RS.RelationType);
            //return RedirectToAction("Relation", "Payroll", new { area = "PMS" });

            string Status = "NA";

            var StatusExist = from jc in db.Relation.Where(jc => jc.RelationType == RS.RelationType)
                              select jc;
            if (StatusExist.Count() == 0)
            {
                var result = admin.AddRelation(RS.RelationId, RS.RelationType);
                if (result)
                    Status = "Succeeded";
                else
                    Status = "Unsucceeded";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Status = "Exists";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult RemoveRelation(Int32 RelationId)
        {
            var removeItem = db.Relation.Find(RelationId);

            if (removeItem != null)
            {
                db.Relation.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("Relation", "Payroll", new { area = "PMS" });
        }

        /// <summary>
        /// Created By Rahul Haldkar
        /// Created On 19-08-2017
        /// Purpose :- Admin add new assets group
        /// </summary>
        /// <param name="AssetGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult AddAssetGroup(Int32 AssetGroupId = 0)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            ViewData["AssetGroupList"] = db.AssetGroup.Where(c => c.CorporateId == userDetail.SubscriberId).ToList();
            AssetGroup assetsgroup = db.AssetGroup.Where(c => c.AssetGroupId == AssetGroupId).FirstOrDefault();
            if (assetsgroup != null)
            {
                ViewBag.AssetGroupId = assetsgroup.AssetGroupId;
            }
            return View(assetsgroup);
        }

        [HttpPost]
        public ActionResult AddAssetGroup(AssetGroup assetgroups)
        {
            string CorporateId = User.Identity.GetUserId();
            bool res = admin.AddAssetsGroup(assetgroups, CorporateId);
            return RedirectToAction("AddAssetGroup", "Payroll", new { area = "PMS" });
        }

        public ActionResult RemoveAssetGroup(Int32 AssetGroupId)
        {
            var assetstype = db.AssetType.Where(c => c.AssetGroupId == AssetGroupId).ToList();
            if (assetstype != null)
            {
                foreach (var item in assetstype)
                {
                    RemoveAssetType(item.AssteTypeId);
                }
            }
            var removeItem = db.AssetGroup.Find(AssetGroupId);
            if (removeItem != null)
            {
                db.AssetGroup.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("AddAssetGroup", "Payroll", new { area = "PMS" });
        }

        /// <summary>
        /// Created By Rahul Haldkar
        /// Created On 21-08-2017
        /// Purpose :- Admin add new assets group
        /// </summary>
        /// <param name="AssetTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult AddAssetType(Int32 AssetTypeId = 0)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewBag.UserId = userDetail.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            ViewData["AssetTypeList"] = db.AssetType.Where(c => c.CorporateId == userDetail.SubscriberId).ToList(); ;
            PopulateAssetsGroup(userDetail.SubscriberId);
            AssetType assetstype = db.AssetType.Where(c => c.AssteTypeId == AssetTypeId).FirstOrDefault();
            if (assetstype != null)
            {
                ViewBag.AssetTypeId = assetstype.AssteTypeId;
                PopulateAssetsGroup(userDetail.SubscriberId, assetstype.AssetGroupId);
            }
            return View(assetstype);
        }

        [HttpPost]
        public ActionResult AddAssetType(AssetType assettypes)
        {
            string CorporateId = User.Identity.GetUserId();
            bool res = admin.AddAssetsType(assettypes, CorporateId);
            return RedirectToAction("AddAssetType", "Payroll", new { area = "PMS" });
        }

        public ActionResult RemoveAssetType(Int32 AssteTypeId = 0)
        {
            var removeItem = db.AssetType.Find(AssteTypeId);
            if (removeItem != null)
            {
                db.AssetType.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("AddAssetType", "Payroll", new { area = "PMS" });
        }


        /// <summary>
        /// CreateBy : Anamika Pandey
        /// CreatedOn : 22-08-2017
        /// Purpose : Organization Leaving Reasons
        /// </summary>

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult LeavingReason(Int64 ReasonId = 0)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewBag.UserId = UserDetail.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            ViewBag.CorporateId = UserDetail.SubscriberId;
            LeavingReason leavingReason = new LeavingReason();
            var leavingreasons = admin.GetLeavingReason(UserDetail.SubscriberId);
            ViewData["leavingReason"] = leavingreasons.ToList();
            if (ReasonId != 0)
            {
                leavingReason = leavingreasons.Where(a => a.ReasonId == ReasonId).FirstOrDefault();
            }

            return View(leavingReason);

        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult LeavingReason(LeavingReason LRS)
        {
            //    admin.AddLeavingReason(LRS.ReasonId, LRS.Reason, LRS.Description, LRS.CorporateId);
            //    return RedirectToAction("LeavingReason", "Payroll", new { area = "PMS" });

            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            LRS.CorporateId = UserDetail.SubscriberId;
            string Status = "NA";

            var StatusExist = from jc in db.LeavingReason.Where(jc => jc.CorporateId == LRS.CorporateId && jc.Reason == LRS.Reason)
                              select jc;
            if (StatusExist.Count() == 0)
            {
                var result = admin.AddLeavingReason(LRS.ReasonId, LRS.Reason, LRS.Description, LRS.CorporateId);
                if (result)
                    Status = "Succeeded";
                else
                    Status = "Unsucceeded";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Status = "Exists";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult RemoveLeavingReason(Int64 ReasonId)
        {
            var removeItem = db.LeavingReason.Find(ReasonId);

            if (removeItem != null)
            {
                db.LeavingReason.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("LeavingReason", "Payroll", new { area = "PMS" });
        }


        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult ClaimCategory(Int64 CategoryId = 0)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewBag.UserId = UserDetail.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            ViewBag.CorporateId = UserDetail.SubscriberId;

            ClaimCategory CLCategory = new ClaimCategory();
            var claimCategory = admin.GetClaimCategory(UserDetail.SubscriberId);
            ViewData["CLCategory"] = claimCategory.ToList();
            PopulateCategoryType();
            if (CategoryId != 0)
            {
                CLCategory = claimCategory.Where(a => a.CategoryId == CategoryId).FirstOrDefault();
                PopulateCategoryType(CLCategory.CategoryType);
            }

            return View(CLCategory);
            //return View();

        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult ClaimCategory(ClaimCategory CLC)
        {
            //admin.AddClaimCategory(CLC.CategoryId, CLC.CategoryName, CLC.CategoryType, CLC.CorporateId);
            //return RedirectToAction("ClaimCategory", "Payroll", new { area = "PMS" });

            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            CLC.CorporateId = UserDetail.SubscriberId;
            string Status = "NA";

            var StatusExist = from jc in db.ClaimCategory.Where(jc => jc.CorporateId == CLC.CorporateId && jc.CategoryName == CLC.CategoryName && jc.CategoryType == CLC.CategoryType)
                              select jc;
            if (StatusExist.Count() == 0)
            {
                var result = admin.AddClaimCategory(CLC.CategoryId, CLC.CategoryName, CLC.CategoryType, CLC.CorporateId);
                if (result)
                    Status = "Succeeded";
                else
                    Status = "Unsucceeded";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Status = "Exists";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveClaimCategory(Int64 CategoryId)
        {
            var removeItem = db.ClaimCategory.Find(CategoryId);

            if (removeItem != null)
            {
                db.ClaimCategory.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("ClaimCategory", "Payroll", new { area = "PMS" });
        }

        /// <summary>
        /// CreateBy : Anamika Pandey
        /// CreatedOn : 23-08-2017
        /// Purpose : Hold Salary Reasons
        /// </summary>

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult HoldSalaryReason(Int64 ReasonId = 0)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewBag.UserId = UserDetail.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            ViewBag.CorporateId = UserDetail.SubscriberId;
            HoldSalaryReason holdSalaryReason = new HoldSalaryReason();
            var holdsalaryreasons = admin.GetHoldSalaryReason(UserDetail.SubscriberId);
            ViewData["holdSalaryReason"] = holdsalaryreasons.ToList();
            if (ReasonId != 0)
            {
                holdSalaryReason = holdsalaryreasons.Where(a => a.ReasonId == ReasonId).FirstOrDefault();
            }

            return View(holdSalaryReason);

        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult HoldSalaryReason(HoldSalaryReason HSR)
        {
            //admin.AddHoldSalaryReason(HSR.ReasonId, HSR.HoldReason, HSR.Description, HSR.CorporateId);
            //return RedirectToAction("HoldSalaryReason", "Payroll", new { area = "PMS" });

            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            HSR.CorporateId = UserDetail.SubscriberId;
            string Status = "NA";

            var StatusExist = db.HoldSalaryReason.Where(jc => jc.CorporateId == HSR.CorporateId && jc.HoldReason == HSR.HoldReason).ToList();
            if (StatusExist.Count() == 0)
            {
                var result = admin.AddHoldSalaryReason(HSR.ReasonId, HSR.HoldReason, HSR.Description, HSR.CorporateId);
                if (result)
                    Status = "Succeeded";
                else
                    Status = "Unsucceeded";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Status = "Exists";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveHoldSalaryReason(Int64 ReasonId)
        {
            var removeItem = db.HoldSalaryReason.Find(ReasonId);

            if (removeItem != null)
            {
                db.HoldSalaryReason.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("HoldSalaryReason", "Payroll", new { area = "PMS" });
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult PayRollLookUps(Int64 LookUpsId = 0)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewBag.UserId = UserDetail.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            ViewBag.CorporateId = UserDetail.SubscriberId;
            PayRollLookUps payRollLookUp = new PayRollLookUps();
            var payrolllookup = admin.GetPayRollLookUps(UserDetail.SubscriberId);
            ViewData["payRollLookUp"] = payrolllookup.ToList();
            if (LookUpsId != 0)
            {
                payRollLookUp = payrolllookup.Where(a => a.LookUpsId == LookUpsId).FirstOrDefault();
            }

            return View(payRollLookUp);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult PayRollLookUps(PayRollLookUps PRL)
        {
            //String UserId = User.Identity.GetUserId();
            //admin.AddPayRollLookUps(PRL.LookUpsId, PRL.LookUpsName, PRL.CorporateId);
            //return RedirectToAction("PayRollLookUps", "Payroll", new { area = "PMS" });

            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            PRL.CorporateId = UserDetail.SubscriberId;
            string Status = "NA";

            var StatusExist = from jc in db.PayRollLookUps.Where(jc => jc.CorporateId == PRL.CorporateId && jc.LookUpsName == PRL.LookUpsName)
                              select jc;
            if (StatusExist.Count() == 0)
            {
                var result = admin.AddPayRollLookUps(PRL.LookUpsId, PRL.LookUpsName, PRL.CorporateId);
                if (result)
                    Status = "Succeeded";
                else
                    Status = "Unsucceeded";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Status = "Exists";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemovePayRollLookUps(Int64 LookUpsId)
        {
            var removeItem = db.PayRollLookUps.Find(LookUpsId);

            if (removeItem != null)
            {
                db.PayRollLookUps.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("PayRollLookUps", "Payroll", new { area = "PMS" });
        }

        /// <summary>
        /// Create By: Vikash Das
        /// Create On: 1-09-2017
        /// Purpose: Get the image in blob
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ContentFileId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetImageURL(string Id, Int64 ContentFileId)
        {
            var url = "";
            if (!string.IsNullOrEmpty(Id))
            {
                if (ContentFileId != 0)
                {
                    url = blobmanager.DownloadPublicBlob(Id.ToLower(), admin.GetLogoFileName(ContentFileId).ToLower());
                }
            }
            return Json(url, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult Preview(Int64 TemplateId)
        {
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewBag.UserId = userdetails.SubscriberId;
            if (TemplateId != 0)
            {
                CorporateTemplate temp = db.CorporateTemplate.Find(TemplateId);
                if (temp.TemplateId != 0)
                {
                    LetterLogoAttachment letterlogo = db.LetterLogoAttachment.Where(c => c.TemplateId == temp.TemplateId && c.CorporateId == userdetails.UserId).FirstOrDefault();
                    var letterheader = "";
                    if (temp.Header != null)
                    {
                        letterheader = temp.Header;
                        ViewData["HeaderDetail"] = letterheader;
                    } string letterContent = "";
                    if (temp.Content != null)
                    {
                        var con = HttpUtility.UrlDecode(temp.Content.Trim());
                        CorporateTemplate Header = db.CorporateTemplate.Find(temp.TemplateId);
                        Int64 FileId = 0;
                        var url = "";
                        if (temp.SameAsCompanyLogo == false)
                        {
                            var letterlog = db.LetterLogoAttachment.Where(c => c.TemplateId == Header.TemplateId).FirstOrDefault();
                            if (letterlog != null)
                            {
                                url = blobmanager.DownloadPublicBlob(temp.CorporateId.ToLower(), admin.GetLogoFileName(letterlogo.FileId).ToLower());
                                FileId = letterlogo.FileId;
                            }
                        }
                        else
                        {
                            var Adminlogo = db.AdminLogoFile.Where(c => c.CorporateId == temp.CorporateId).FirstOrDefault();
                            if (Adminlogo != null)
                            {
                                url = blobmanager.DownloadPublicBlob(userdetails.UserId.ToLower(), cmsMgr.GetFileNameCompanyLogo(Adminlogo.FileId).ToLower());
                                FileId = Adminlogo.FileId;
                            }
                        }
                        var headercon = "<div style='height:100px;width:90%;'>";
                        if (FileId != 0)
                        {
                            headercon = headercon + "<div style='height:100px;width:20%;float:left;'>" +
                                                "<img src='" + url + "' style='height:100px;width:80px;'></div>";
                        }
                        else if (temp.Header != null)
                        {
                            headercon = headercon + "<div style='height:100px;width:20%;float:left;'>" +
                                                "<img style='height:100px;width:80px;border: 0;display:none'></div>";
                        }
                        headercon = headercon + "<div style='height:100px;width:80%;text-align:center;'>" + letterheader + "</div></div>";

                        string[] Content;
                        if (!String.IsNullOrEmpty(con))
                        {
                            string[] pagebrk = { "<div style='page-break-after: always'><span style='display: none;'>&nbsp;</span></div>" };
                            pagebrk[0] = pagebrk[0].Replace("'", "\"");
                            string divstyle = "<div class='container-fluid' style='height:100%;width:110%;margin: 2mm 2mm 2mm 2mm; border:solid;'>";
                            Content = con.Split(pagebrk, StringSplitOptions.RemoveEmptyEntries);
                            if (Content.Length > 0)
                            {
                                for (int i = 0; i < Content.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        letterContent = "<div id='divsection' class = 'container-fluid'>" + divstyle + letterContent + headercon + Content[i] + "</div></div>";
                                    }
                                    else
                                    {
                                        letterContent = letterContent + pagebrk[0] + " <div id='divsection' class = 'container-fluid'>" + divstyle + headercon + Content[i] + "</div></div>";
                                    }
                                }
                            }
                            else
                            {
                                letterContent = headercon + con;
                            }
                        }
                        else
                        {
                            letterContent = headercon;
                        }
                    }
                    //letterContent = letterContent.Replace("<li style='text-align: justify;'>", "<li>");
                    letterContent = letterContent.Replace("<ol>", "<ol style='margin:0 0 0 10px;'>");
                    temp.Content = letterContent;
                    return View(temp);
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetState(string CountryId)
        {
            int countryId;
            List<SelectListItem> StateId = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(CountryId))
            {
                countryId = Convert.ToInt32(CountryId);
                List<StatesMaster> States = db.StatesMaster.Where(x => x.CountryId == countryId).ToList();
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
                List<CityMaster> Cities = db.CityMaster.Where(x => x.StateId == stateId).ToList();
                Cities.ForEach(x =>
                {
                    CityId.Add(new SelectListItem { Text = x.City, Value = x.CityId.ToString() });
                });
            }
            return Json(CityId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult LetterType(string savestatus, Int64 LetterTypeId = 0)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewBag.UserId = UserDetail.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            ViewBag.Status = savestatus;
            LetterType letterType = new LetterType();
            if (LetterTypeId != 0)
            {
                letterType = db.LetterType.Find(LetterTypeId);
                ViewBag.LetterTypeId = letterType.LetterTypeId;
            }
            ViewData["LetterTypeList"] = db.LetterType.Where(c => c.CorporateId == UserDetail.UserId).ToList();
            return View(letterType);
        }

        [HttpPost]
        public ActionResult LetterType(LetterType LT)
        {
            string Status = "";
            string CorporateId = User.Identity.GetUserId();

            var StatusExist = from jc in db.LetterType.Where(jc => jc.LetterTypeName.ToUpper() == LT.LetterTypeName.ToUpper() && jc.CorporateId == CorporateId)
                              select jc;
            if (StatusExist.Count() > 0 && LT.LetterTypeId == 0)
            {
                Status = "Exists";
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            else if (StatusExist.Count() == 0 || LT.LetterTypeId != 0)
            {
                var result = admin.AddLetterType(LT, CorporateId);
                if (result == true)
                {
                    List<CorporateTemplate> updatetemplate = db.CorporateTemplate.Where(c => c.LetterTypeId == LT.LetterTypeId && c.CorporateId == LT.CorporateId).ToList();
                    foreach (var item in updatetemplate)
                    {
                        item.LetterTypeId = LT.LetterTypeId;
                        admin.AddCorporateTemplate(item, CorporateId);
                    }
                    List<CorporateLetter> updateletter = db.CorporateLetter.Where(x => x.LetterTypeId == LT.LetterTypeId && x.CorporateId == LT.CorporateId).ToList();

                    foreach (var item1 in updateletter)
                    {
                        item1.LetterTypeId = LT.LetterTypeId;
                        admin.AddCorporateLetter(item1, CorporateId);
                    }
                    Status = "Succeeded";
                }
                else
                {
                    Status = "Unsucceeded";
                }
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("LetterType", "Payroll");
        }

        public ActionResult RemoveLetterType(Int64 LetterTypeId = 0)
        {
            var removeItem = db.LetterType.Find(LetterTypeId);
            string Status = "";
            var statuscount = db.CorporateLetter.Where(c => c.LetterTypeId == LetterTypeId).ToList();
            var status = db.CorporateTemplate.Where(c => c.LetterTypeId == LetterTypeId).ToList();
            if (statuscount.Count != 0 && status.Count != 0)
            {
                Status = "Exists";
                return RedirectToAction("LetterType", "Payroll", new { area = "PMS", savestatus = Status });
            }
            else
            {
                if (removeItem != null)
                {
                    db.LetterType.Remove(removeItem);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("LetterType", "Payroll", new { area = "PMS" });
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult IssueLetter(string UserId = null)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewBag.UserId = UserDetail.SubscriberId;
            List<SendLetter> sendletterlist = new List<SendLetter>();
            if (UserId != null)
            {
                ViewData["Issueletterlist"] = admin.GetIssueLetterList(UserId);
                sendletterlist = db.SendLetter.Where(c => c.UserId == UserId).ToList();
            }
            return View(sendletterlist);
        }

        [RequireHttps]
        [HttpGet]
        public ActionResult BioMetric(string Eid, string data)
        {

            string ippublish = Request.UserHostAddress;
            ViewBag.ippublish = ippublish;
            //100.73.130.130

            string UserId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(UserId))
                return RedirectToAction("Login", "Account", new { area = "" });

            var userdetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            ViewBag.Result = data;

            //Get IP Address of logged in user
            var LoggedInIp = GetUserIP();
            ViewBag.LoggedInIp = LoggedInIp;
            //ViewBag.MACAddress = GetMACAddress("10.230.20.10");

            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

            DateTime currentDate = indianTime.Date;

            if (userdetails.Role == "Employee")
            {
              
                var getcheckindetails = db.BiometricCheckInCheckOut.Where(c => c.UserId == UserId && c.CheckInDate == currentDate).FirstOrDefault();
                if (getcheckindetails != null)
                {
                    ViewBag.CheckIndetails = getcheckindetails.CheckInDate;

                    if (getcheckindetails.CheckInDate != null)
                    {
                        ViewBag.CheckOutdetails = getcheckindetails.CheckOutDate;
                    }

                }
                ViewBag.CurrentDate = currentDate;
                ViewData["Biometric"] = db.BiometricCheckInCheckOut.Where(c => c.UserId == UserId).ToList();
                
            }
            else
            {
                PopulateEmployeeBiometric(userdetails.SubscriberId);              
                ViewData["Biometric"] = db.BiometricCheckInCheckOut.Where(c => c.UserId == Eid);

            }
            return View();
        }

        [HttpPost]
        public ActionResult BioMetric(string LoggedInIp, DateTime?  CheckIndetails, float Latitude = 0, float Longitude = 0, Int64 BiometricId = 0)
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);

            Int64 branchId = db.EmpJoiningDetail.Where(e => e.UserId == userdetails.UserId).FirstOrDefault().BranchId;

            var Ipdata = db.IpMasters.Where(c => c.UserId == userdetails.SubscriberId && c.BranchId == branchId).ToList();

            string result = "NoMatch";

            if (Ipdata != null)
            {
                var branchlocation = Ipdata.FirstOrDefault();


                if (branchlocation.Authenticate == 1)
                {
                    if (LoggedInIp == branchlocation.IPAddressFrom || LoggedInIp == branchlocation.IPAddressTo)
                    {
                        result = submitAttendance(UserId, userdetails.SubscriberId, LoggedInIp, CheckIndetails, Latitude, Longitude, BiometricId);
                    }

                }
                else if (branchlocation.Authenticate == 2)
                {

                    if (Latitude >= branchlocation.LatitudeFrom && Latitude <= branchlocation.LatitudeTo && Longitude >= branchlocation.LongitudeFrom && Longitude <= branchlocation.LongitudeTo)
                    {

                        result = submitAttendance(UserId, userdetails.SubscriberId, LoggedInIp, CheckIndetails, Latitude, Longitude, BiometricId);
                    }

                }
                else if (branchlocation.Authenticate == 3)
                {
                    if ((LoggedInIp == branchlocation.IPAddressFrom || LoggedInIp == branchlocation.IPAddressTo) && (Latitude >= branchlocation.LatitudeFrom && Latitude <= branchlocation.LatitudeTo && Longitude >= branchlocation.LongitudeFrom && Longitude <= branchlocation.LongitudeTo))
                    {

                        result = submitAttendance(UserId, userdetails.SubscriberId, LoggedInIp, CheckIndetails, Latitude, Longitude, BiometricId);
                    }

                }
            }


            return Json(result, JsonRequestBehavior.AllowGet);

            //return RedirectToAction("BioMetric", "Payroll", new { area = "PMS" });
        }

        public string submitAttendance(string UserId, string SubscriberId, string LoggedInIp, DateTime? CheckIndetails, float Latitude, float Longitude, Int64 BiometricId)
        {
            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

            DateTime currentDate = indianTime.Date;
            TimeSpan CurrentTime = indianTime.TimeOfDay;
            Int64 ShiftId = 1;

            if ( CheckIndetails == null)
            {

                bool result = admin.AddEmployeeCheckinCheckout(BiometricId, UserId, currentDate, CurrentTime, null, null, LoggedInIp, SubscriberId, ShiftId);
                return "CheckedIn";
            }
            else
            {
                var BiometricDetails = db.BiometricCheckInCheckOut.Where(c => c.UserId == UserId).ToList().OrderByDescending(d => d.CheckInDate).FirstOrDefault();

                bool res = admin.AddEmployeeCheckinCheckout(BiometricDetails.BiometricId, UserId, BiometricDetails.CheckInDate, BiometricDetails.CheckInTime, currentDate, CurrentTime, LoggedInIp, BiometricDetails.SubscriberId, ShiftId);
                return "CheckedOut";
            }
        }

        public DateTime IndianStandard(DateTime currentDate)
        {
            TimeZoneInfo mountain = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime utc = currentDate;
            return TimeZoneInfo.ConvertTimeFromUtc(utc, mountain);
        }

        [HttpGet]
        public ActionResult BioMetricResult(string Month, string Eid)
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            PopulateEmployeeBiometric(userdetails.SubscriberId);
            PopulateBiometricMonth();
            PopulateDepartment("EMS");
            ViewBag.CurrentYear = DateTime.Now.Year;

            if (!string.IsNullOrEmpty(Month))
            {
                ViewBag.CurrentMonth = Convert.ToInt32(Month);
            }
            else
            {
                ViewBag.CurrentMonth = DateTime.Now.Month;
            }
            if (!string.IsNullOrEmpty(Eid))
            {
                var Biometric = db.BiometricCheckInCheckOut.Where(c => c.UserId == Eid).ToList();
                return View(Biometric);
            }
            else
            {
                var Biometric = db.BiometricCheckInCheckOut.Where(c => c.SubscriberId == userdetails.SubscriberId).ToList();
                return View(Biometric);
            }
        }

        [HttpPost]
        public ActionResult BioMetricResult(string Eid, string Month, string Schedule, string DepartmentId)
        {
            string UId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UId);
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            PopulateEmployeeBiometric(userdetails.SubscriberId);
            PopulateBiometricMonth();
            PopulateDepartment("EMS");
            var htmlContent = "";
            var tabcontent = "";
            List<EmployeeBiometricView> TotalEmployees = admin.GetEmployeewithDepartmentForBiometric(userdetails.SubscriberId);

            foreach (var Bio in TotalEmployees)
            {
                //Show each training in a table
                List<BiometricCheckInCheckOutview> CandAtt = Bio.BiometricCheckInCheckOutview.Where(c => c.Deactivated == false ).OrderBy(list => list.Name).ToList();
           
                if (!string.IsNullOrEmpty(DepartmentId))
                {
                    CandAtt = CandAtt.Where(c => c.DepartmentId == DepartmentId).ToList();
                }
                if (!string.IsNullOrEmpty(Eid))
                {
                    CandAtt = CandAtt.Where(c => c.UserId == Eid).ToList();
                }
                if (CandAtt.Count() > 0)
                {
                    List<string> UserId = new List<string>();
                    foreach (var row in CandAtt)
                    {
                        UserId.Add(row.UserId);
                    }
                    UserId = UserId.Distinct().ToList();

                    List<DateTime> AttendanceDates = new List<DateTime>();
                    var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var toDate = fromDate.AddMonths(1);
                    var diff = toDate.Subtract(fromDate).Days;
                    //var totaldiff = diff + 1;
                    var date = fromDate.Date.AddDays(-1);

                    if (!string.IsNullOrEmpty(Schedule))
                    {
                        string[] strSchedule = Schedule.Split('-');

                        DateTime Rangefrmdate = DateTime.ParseExact(strSchedule[0].Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture);

                        DateTime Rangetodate = DateTime.ParseExact(strSchedule[1].Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture);

                        if (Rangefrmdate != null && Rangetodate != null)
                        {
                            fromDate = Rangefrmdate;
                            toDate = Rangetodate;
                            diff = toDate.Subtract(fromDate).Days;
                            //totaldiff = diff + 1;
                            date = fromDate.Date.AddDays(-1);
                        }
                    }

                    else if (!string.IsNullOrEmpty(Month))
                    {
                        var month = Convert.ToInt32(Month);

                        fromDate = new DateTime(DateTime.Now.Year, month, 1);
                        toDate = fromDate.AddMonths(1);
                        diff = toDate.Subtract(fromDate).Days;
                        date = fromDate.Date.AddDays(-1);
                    }

                    //List<DateTime> AttendanceDates = new List<DateTime>();
                    //var fromDate = DateTime.Now.AddDays(-5);
                    //var toDate = DateTime.Now.AddMonths(1);
                    //var diff = toDate.Subtract(fromDate).Days + 1;
                    //var date = fromDate.Date;

                    for (int z = 0; z < diff; z++)
                    {
                        date = date.AddDays(1);
                        AttendanceDates.Add(date);
                    }
                    AttendanceDates = AttendanceDates.Distinct().OrderBy(d => d.Date).ToList();

                    //tabcontent = "<div class='row'><div class='fancy-title title-bottom-border'><h5>" + Bio.CompanyName + " : " + (Bio.Name) + "</h5></div><small style='float:right'>Total Employee : " + Bio.TotalEmployee + "</small><br/><div class='table-responsive'><table id='fixTable_" + "BioMetric" + "' class='table table-bordered nobottommargin'>";
                    tabcontent = "<div class='row'><div class='fancy-title title-bottom-border'></div><small style='float:right'>Total Employee : " + Bio.TotalEmployee + "</small><br/><div class='table-responsive'><table id='fixTable_" + "BioMetric" + "' class='table table-bordered nobottommargin'>";
                    tabcontent = tabcontent + "<tr><th>Employee</th>";

                    foreach (var dt in AttendanceDates)
                    {
                        if (fromDate <= dt && dt <= toDate)
                            tabcontent = tabcontent + "<th>" + dt.ToString("dd-MMM-yyyy") + "</th>";
                    }
                    //tabcontent = tabcontent + "<th>Present/Total Days</th></tr>";

                    for (int i = 0; i < UserId.Count(); i++)
                    {
                        string IDS = UserId[i];
                        var detail = db.EmployeeBasicDetails.Where(c => c.UserId == IDS).FirstOrDefault();
                        tabcontent = tabcontent + "<tr><td><b>" + detail.Name + "</b></td>";
                        //tabcontent = tabcontent + "<tr><td><b>" + UserId[i] + "</b></td>";
                        for (int j = 0; j < AttendanceDates.Count(); j++)
                        {
                            BiometricCheckInCheckOutview AttByNameNDate = Bio.BiometricCheckInCheckOutview.Where(list => list.UserId == UserId[i] && list.CheckInDate == AttendanceDates[j]).FirstOrDefault();
                            if (AttByNameNDate != null)
                            {
                                if (AttByNameNDate.CheckOutDate != null)
                                {
                                    tabcontent = tabcontent + "<td> <span>In: </span>" + AttByNameNDate.CheckInTime.Value.ToString(@"hh\:mm") + "<br />" + "Out: " + AttByNameNDate.CheckOutTime.Value.ToString(@"hh\:mm") + "</td>";
                                }
                                else
                                {
                                    tabcontent = tabcontent + "<td> <span>In: </span>" + AttByNameNDate.CheckInTime.Value.ToString(@"hh\:mm") + "<br />" + "<span style='color: red;'>Out: " + " " + "</span></td>";
                                }
                            }
                            else
                            {
                                tabcontent = tabcontent + "<td>-</td>";
                            }
                        }
                    }

                }
                else
                {
                    tabcontent = "<div class='row'><div class='fancy-title title-bottom-border'></div><small style='float:right'>Total Employee : " + Bio.TotalEmployee + "</small><br/><div class='feature-box fbox-center fbox-bg fbox-border fbox-effect'><div class='fbox-icon'>";
                    tabcontent = tabcontent + "<i class='icon-thumbs-down2'></i></div><h3>No Entry Recorded Employee<span class='subtitle'></span></h3></div><br /><br />";
                }
                htmlContent = htmlContent + tabcontent;

            }

            return Json(htmlContent, JsonRequestBehavior.AllowGet);
        }
        //Created by vikas pandey 
        //for employee checkin checkout details export report
        //29/12/2017
        public ActionResult DownloadEmployeeAttandance(string Eid, string CorporateId, string Month, string Schedule, string DepartmentId)
        {
            if (string.IsNullOrEmpty(Eid))
            {
                Eid = null;
            }
            if (string.IsNullOrEmpty(DepartmentId))
            {
                DepartmentId = null;
            }
            var fromDate = new DateTime(DateTime.Now.Year, Convert.ToInt32(Month), 1);
            var toDate = fromDate.AddMonths(1).AddDays(-1);
            //var diff = toDate.Subtract(fromDate).Days;
            //var totaldiff = diff + 1;
            //var date = fromDate.Date.AddDays(-1);

            if (!string.IsNullOrEmpty(Schedule) && Schedule != "undefined")
            {
                string[] strSchedule = Schedule.Split('-');

                DateTime Rangefrmdate = DateTime.ParseExact(strSchedule[0].Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime Rangetodate = DateTime.ParseExact(strSchedule[1].Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture);

                if (Rangefrmdate != null && Rangetodate != null)
                {
                    fromDate = Rangefrmdate;
                    toDate = Rangetodate;
                    //diff = toDate.Subtract(fromDate).Days;
                    //totaldiff = diff + 1;
                    //date = fromDate.Date.AddDays(-1);
                }
            }
            //else
            //{
            //    //var month = Convert.ToInt32(Month);
            //    fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //    toDate = fromDate.AddMonths(1);
            //    diff = toDate.Subtract(fromDate).Days;
            //    date = fromDate.Date.AddDays(-1);
            //}
            var UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/EmployeeCheckInOut.rdlc";

            var sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            rptViewer.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);

            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@CorporateId", userDetails.SubscriberId));
            cmd.Connection = thisConnection;
            string MyDataSource1 = "USP_GetClientDetails";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Parameters.Add(new SqlParameter("@SubscriberId", userDetails.SubscriberId));
            cmd2.Parameters.Add(new SqlParameter("@FromDate", Convert.ToDateTime(fromDate)));
            cmd2.Parameters.Add(new SqlParameter("@ToDate", Convert.ToDateTime(toDate)));
            cmd2.Parameters.Add(new SqlParameter("@DepartmentId", DepartmentId));
            cmd2.Parameters.Add(new SqlParameter("@Eid", Eid));
            cmd2.Connection = thisConnection;
            string MyDataSource2 = "GetEmployeeCheckInCheckOutDetails";
            cmd2.CommandText = string.Format(MyDataSource2);
            cmd2.CommandType = CommandType.StoredProcedure;
            cmd2.CommandTimeout = 600;
            SqlDataAdapter daN2 = new SqlDataAdapter(cmd2);
            System.Data.DataSet DataSet2 = new System.Data.DataSet();
            daN2.Fill(DataSet2);
            ReportDataSource reportDataSource2 = new ReportDataSource();
            reportDataSource2.Name = "DataSet2";
            reportDataSource2.Value = DataSet2.Tables[0];
            ReportParameter[] parms = new ReportParameter[6];
            parms[0] = new ReportParameter("SubscriberId", userDetails.SubscriberId);
            parms[1] = new ReportParameter("FromDate", fromDate.ToString());
            parms[2] = new ReportParameter("ToDate", toDate.ToString());
            parms[3] = new ReportParameter("DepartmentId", DepartmentId);
            parms[4] = new ReportParameter("Eid", Eid);
            parms[5] = new ReportParameter("CorporateId", userDetails.SubscriberId);
            rptViewer.LocalReport.SetParameters(parms);
            rptViewer.LocalReport.DataSources.Add(reportDataSource);
            rptViewer.LocalReport.DataSources.Add(reportDataSource2);
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
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=EmployeeCheckInCheckOutDetails.xls");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            rptViewer.LocalReport.ReleaseSandboxAppDomain();
            thisConnection.Close();
            return View();
        }

        [HttpGet]
        public ActionResult CurrentDayAttendance(string Eid)
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            PopulateEmployeeBiometric(userdetails.SubscriberId);
            PopulateDepartment("EMS");
            PopulateattendanceFilter();
            ViewBag.CurrentYear = DateTime.Now.Year;

            if (!string.IsNullOrEmpty(Eid))
            {
                var Biometric = db.BiometricCheckInCheckOut.Where(c => c.UserId == Eid).ToList();
                return View(Biometric);
            }
            else
            {
                var Biometric = db.BiometricCheckInCheckOut.Where(c => c.SubscriberId == userdetails.SubscriberId).ToList();
                return View(Biometric);
            }
        }

        [HttpPost]
        public ActionResult CurrentDayAttendance(string Eid, string DepartmentId, string attendanceFilter)
        {
            string Id = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(Id);
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            PopulateEmployeeBiometric(userdetails.SubscriberId);
            PopulateBiometricMonth();
            PopulateDepartment("EMS");
            var htmlContent = "";
            var tabcontent = "";

            EmployeeBiometricView CorporateDetail = admin.GetEmployeewithDepartmentForBiometric(userdetails.SubscriberId).FirstOrDefault();
            if (CorporateDetail != null)
            {
                //Show each training in a table
                var CandAtt = CorporateDetail.BiometricCheckInCheckOutview.Where(c => c.Deactivated == false).OrderBy(list => list.Name).Select(list => new { list.UserId, list.Name, list.DepartmentId }).Distinct().ToList();
                var OfficeShift = db.ShiftMaster.Where(c => c.CorporateId == userdetails.SubscriberId).FirstOrDefault();

                if (!string.IsNullOrEmpty(DepartmentId))
                {
                    CandAtt = CandAtt.Where(c => c.DepartmentId == DepartmentId).ToList();
                }

                if (!string.IsNullOrEmpty(Eid))
                {
                    CandAtt = CandAtt.Where(c => c.UserId == Eid).ToList();
                }

                if (CandAtt.Count() > 0)
                {
                    tabcontent = "<div class='row'><div class='fancy-title title-bottom-border'></div><small style='float:right'>Total Employee : " + CorporateDetail.TotalEmployee + "</small><br/><div class='table-responsive'><table id='fixTable_" + "BioMetric" + "' class='table table-bordered nobottommargin'>";
                    tabcontent = tabcontent + "<tr><th>Employee</th>";


                    tabcontent = tabcontent + "<th>" + DateTime.Now.ToString("dd-MMM-yyyy") + "</th></tr>";

                    List<BiometricCheckInCheckOutview> AttByNameNDate = CorporateDetail.BiometricCheckInCheckOutview.Where(list => list.Deactivated == false && list.CheckInDate.Value.Date == DateTime.Now.Date).ToList();

                    //var CheckInList = (from a in CandAtt
                    //                   join b in AttByNameNDate on a.UserId equals b.UserId into t
                    //                   from rt in t.DefaultIfEmpty()
                    //                   select new { a.UserId, a.Name, rt.CheckInTime, rt.CheckOutTime, rt.CheckInDate, rt.CheckOutDate }).ToList();

                    var CheckInList = (from a in CandAtt
                                       join b in AttByNameNDate on a.UserId equals b.UserId
                                       select new { a.UserId, a.Name, b.CheckInTime, b.CheckOutTime, b.CheckInDate, b.CheckOutDate }).ToList();

                    foreach (var attendance in CheckInList)
                    {

                        // var detail = db.EmployeeBasicDetails.Where(c => c.UserId == attendance.UserId).FirstOrDefault();


                        if (!string.IsNullOrEmpty(attendanceFilter) && attendanceFilter == "In Time")
                        {
                            if (attendance.CheckInTime != null && attendance.CheckInTime <= OfficeShift.StartTime)
                            {
                                tabcontent = tabcontent + "<tr><td><b>" + attendance.Name + "</b></td>";
                                if (attendance.CheckOutTime != null)
                                {
                                    tabcontent = tabcontent + "<td> <span>In: </span>" + attendance.CheckInTime.Value.ToString(@"hh\:mm") + "<br />" + "Out: " + attendance.CheckOutTime.Value.ToString(@"hh\:mm") + "</td></tr>";
                                }
                                else
                                {
                                    tabcontent = tabcontent + "<td> <span>In: </span>" + attendance.CheckInTime.Value.ToString(@"hh\:mm") + "<br />" + "<span style='color: red;'>Out: " + " " + "</span></td></tr>";
                                }
                            }
                        }

                        else if (!string.IsNullOrEmpty(attendanceFilter) && attendanceFilter == "Late")
                        {
                            if (attendance.CheckInTime != null && attendance.CheckInTime > OfficeShift.StartTime)
                            {
                                tabcontent = tabcontent + "<tr><td><b>" + attendance.Name + "</b></td>";
                                if (attendance.CheckOutTime != null)
                                {
                                    tabcontent = tabcontent + "<td> <span>In: </span>" + attendance.CheckInTime.Value.ToString(@"hh\:mm") + "<br />" + "Out: " + attendance.CheckOutTime.Value.ToString(@"hh\:mm") + "</td></tr>";
                                }
                                else
                                {
                                    tabcontent = tabcontent + "<td> <span>In: </span>" + attendance.CheckInTime.Value.ToString(@"hh\:mm") + "<br />" + "<span style='color: red;'>Out: " + " " + "</span></td></tr>";
                                }
                            }
                        }
                        else
                        {
                            if (attendance.CheckInTime == null)
                            {
                                tabcontent = tabcontent + "<tr><td><b>" + attendance.Name + "</b></td>";
                                tabcontent = tabcontent + "<td>-</td></tr>";
                            }
                        }

                    }
                    htmlContent = htmlContent + tabcontent;

                }
            }

            return Json(htmlContent, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult AddIp(string UserAction, Int64 IpId = 0, bool Data = false)
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();

            PopulateBranches(userdetails.SubscriberId);
            PopulateAuthType();
            if (Data == true)
            {
                ViewBag.Data = "Succeed";
            }
            if (UserAction == "Delete" && IpId > 0)
            {
                admin.DeleteIp(IpId);
                ViewBag.Result = "Deleted";
                return View();
            }
            var Ipdata = admin.GetBranchLocationDetail(userdetails.SubscriberId);
            ViewData["Ip"] = Ipdata;
            if (IpId > 0)
            {
                var SelectedIpdata = Ipdata.Where(c => c.IpId == IpId).FirstOrDefault();
                PopulateBranches(userdetails.SubscriberId, SelectedIpdata.BranchId);
                PopulateAuthType(SelectedIpdata.Authenticate);
                string[] ipsFrom = SelectedIpdata.IPAddressFrom.Split('.');
                ViewBag.ipsFrom1 = ipsFrom[0];
                ViewBag.ipsFrom2 = ipsFrom[1];
                ViewBag.ipsFrom3 = ipsFrom[2];
                ViewBag.ipsFrom4 = ipsFrom[3];

                string[] ipsTo = SelectedIpdata.IPAddressTo.Split('.');
                ViewBag.ipsTo1 = ipsTo[0];
                ViewBag.ipsTo2 = ipsTo[1];
                ViewBag.ipsTo3 = ipsTo[2];
                ViewBag.ipsTo4 = ipsTo[3];
                return View(SelectedIpdata);
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddIp(Int64 WorkLocation, Int16 Authenticate, string firstIp, string secondIp, string thirdIp, string forthIp, string ipsTo1, string ipsTo2, string ipsTo3, string ipsTo4, string UserId, float LatitudeFrom, float LatitudeTo, float LongitudeFrom, float LongitudeTo, Int64 IpId = 0)
        {
            string IpAddressFrom = (firstIp + "." + secondIp + "." + thirdIp + "." + forthIp);
            string IpAddressTo = (ipsTo1 + "." + ipsTo2 + "." + ipsTo3 + "." + ipsTo4);
            bool res = admin.AddIpAddress(IpId, WorkLocation, Authenticate, IpAddressFrom, IpAddressTo, LatitudeFrom, LatitudeTo, LongitudeFrom, LongitudeTo, UserId);

            return RedirectToAction("AddIp", "Payroll", new { area = "PMS", Data = res });
        }

        public string GetUserIP()
        {
            string strIP = String.Empty;
            HttpRequest httpReq = System.Web.HttpContext.Current.Request;

            //test for non-standard proxy server designations of client's IP
            if (httpReq.ServerVariables["HTTP_CLIENT_IP"] != null)
            {
                strIP = httpReq.ServerVariables["HTTP_CLIENT_IP"].ToString();
            }
            else if (httpReq.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                strIP = httpReq.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            //test for host address reported by the server
            else if
            (
                //if exists
                (httpReq.UserHostAddress.Length != 0)
                &&
                //and if not localhost IPV6 or localhost name
                ((httpReq.UserHostAddress != "::1") || (httpReq.UserHostAddress != "localhost"))
            )
            {
                strIP = httpReq.UserHostAddress;
            }
            //finally, if all else fails, get the IP from a web scrape of another server
            else
            {
                WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                using (WebResponse response = request.GetResponse())
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    strIP = sr.ReadToEnd();
                }
                //scrape ip from the html
                int i1 = strIP.IndexOf("Address: ") + 9;
                int i2 = strIP.LastIndexOf("</body>");
                strIP = strIP.Substring(i1, i2 - i1);
            }
            return strIP;
        }

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);

        public string GetMACAddress(string sName)
        {
            string s = string.Empty;
            System.Net.IPHostEntry Tempaddr = null;
            Tempaddr = (System.Net.IPHostEntry)System.Net.Dns.GetHostEntry(sName);
            System.Net.IPAddress[] TempAd = Tempaddr.AddressList;
            string[] Ipaddr = new string[3];
            foreach (IPAddress TempA in TempAd)
            {
                Ipaddr[1] = TempA.ToString();
                byte[] ab = new byte[6];
                int len = ab.Length;
                // int r = SendARP((int)TempA.Address, 0, ab, ref len);
                string sMAC = BitConverter.ToString(ab, 0, 6);
                Ipaddr[2] = sMAC;
                s = sMAC;
            }
            return s;
        }

        [HttpGet]
        public ActionResult AddShift(string UserAction, Int64 ShiftId = 0, bool Data = false)
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            if (Data == true)
            {
                ViewBag.Data = "Succeed";
            }
            if (UserAction == "Delete" && ShiftId > 0)
            {
                admin.DeleteShift(ShiftId);
                ViewBag.Result = "Deleted";
                return View();
            }
            var AllShifts = db.ShiftMaster.Where(c => c.CorporateId == UserId).ToList();
            ViewData["Shifts"] = AllShifts;
            var SelectedShift = AllShifts.Where(c => c.ShiftId == ShiftId).FirstOrDefault();
            if (SelectedShift != null)
            {
                ViewBag.StartTime = SelectedShift.StartTime;
                ViewBag.EndTime = SelectedShift.EndTime;
            }
            return View(SelectedShift);
        }

        [HttpPost]
        public ActionResult AddShift(string Shift, string CorporateId, string StartTime, string EndTime, Int64 ShiftId = 0)
        {
            //TimeSpan? Start = Convert.ToDateTime(StartTime).TimeOfDay;
            //TimeSpan? End = Convert.ToDateTime(EndTime).TimeOfDay;

            TimeSpan? Start = DateTime.ParseExact(StartTime, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            TimeSpan? End = DateTime.ParseExact(EndTime, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;

            bool res = admin.AddShifts(ShiftId, Shift, CorporateId, Start, End);

            return RedirectToAction("AddShift", "Payroll", new { area = "PMS", Data = res });
        }


        //[Authorize(Roles = "Admin,Employee")]
        public ActionResult EmployeesLeaveRecords(string sortOrder, string UserName, string Department, string DepartmentId, string Name, string Designation, string UName, string Schedule, string LeaveTypeId, int? page, string Role = "", int PageSize = 20, Int16 SchemeId = 0)
        {
            string gender = "TR";
            var userdetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userdetails;
            ViewBag.UserId = userdetails.SubscriberId;
            var empDetails = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["EmpDetails"] = empDetails;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            //var plandetail = admin.GetUserplanDetails(userdetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            //ViewData["plandetail"] = plandetail;
            PopulateSchema();
            //PopulateLeaveType();
            PopulateDepartment("EMS");

            var EmployeeList = emsMgr.GetSubscriberWiseEmployeeList(userdetails.SubscriberId).Where(c => c.DepartmentId != "ADI" && c.Deactivated == false).ToList();


            //DateTime? da = null;
            //DateTime? dt = null;
            string leaveyear = db.CompanySetting.Where(c => c.CorporateId == userdetails.SubscriberId).FirstOrDefault().CalendarYear;
            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            if (leaveyear == "Mar-Apr" && Month <= 3)
                Year = Year - 1;

            if (!string.IsNullOrEmpty(DepartmentId) && SchemeId != 0)
                EmployeeList = EmployeeList.Where(e => e.DepartmentId == DepartmentId && e.SchemeId == SchemeId).ToList();
            else if (!string.IsNullOrEmpty(DepartmentId))
                EmployeeList = EmployeeList.Where(e => e.DepartmentId == DepartmentId).ToList();
            else if (SchemeId != 0)
                EmployeeList = EmployeeList.Where(e => e.SchemeId == SchemeId).ToList();


            List<EmployeeLeaveSummariesViewModel> empLeaveRecords = null;
            if (userdetails.Role == "Admin")
            {
                empLeaveRecords = ems.GetEmployeeLeaveSummary(userdetails.SubscriberId, Year, SchemeId, DepartmentId, null);
            }
            else
            {
                EmployeeList = EmployeeList.Where(e => e.UserId == userdetails.UserId).ToList();
                SchemeId = EmployeeList.FirstOrDefault().SchemeId;
                gender = empDetails.Gender;
                empLeaveRecords = ems.GetEmployeeLeaveSummary(userdetails.SubscriberId, Year, SchemeId, DepartmentId, userdetails.UserId);
            }

            var CorporateLeaves = db.EngagementTypeMaster.Where(c => c.CorporateId == userdetails.SubscriberId && c.SchemeId == SchemeId && c.LeaveTypeCategory == "D").ToList();

            if (gender == "MA")
                CorporateLeaves = CorporateLeaves.Where(c => c.LeaveTypeId != "ML").ToList();
            else if (gender == "FE")
                CorporateLeaves = CorporateLeaves.Where(c => c.LeaveTypeId != "PL").ToList();
            else
                CorporateLeaves = CorporateLeaves.Where(c => c.LeaveTypeId != "PL" && c.LeaveTypeId != "ML").ToList();

            ViewData["CorporateLeaves"] = CorporateLeaves;
            ViewData["EmpLeaveRecords"] = empLeaveRecords;
            //ViewBag.UserId = userdetails.SubscriberId;

            //ViewBag.Name = Name;
            //ViewBag.Page = page;
            //ViewBag.ModelData = "NA";
            //var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //var toDate = fromDate.AddMonths(1);
            //var diff = toDate.Subtract(fromDate).Days;
            //var date = fromDate.Date.AddDays(-1);
            //if (!String.IsNullOrEmpty(Name) || !String.IsNullOrEmpty(DepartmentId)
            //    || !String.IsNullOrEmpty(LeaveTypeId) || SchemeId > 0 || !string.IsNullOrEmpty(Schedule))
            //{
            //    ViewBag.ModelData = "Reocrd";
            //}

            //if (Role == "Employee")
            //{
            //    EmployeeList = EmployeeList.Where(c => c.UserId == userdetails.UserId).ToList();
            //    if (!String.IsNullOrEmpty(LeaveTypeId))
            //    {
            //        ViewData["CorporateLeaves"] = db.EngagementTypeMaster.Where(c => c.CorporateId == userdetails.SubscriberId && c.ShortName == LeaveTypeId).ToList();
            //    }
            //}
            //else
            //{
            //    if (userdetails.Role == "Admin")
            //    {
            //        ViewBag.EmCount = EmployeeList == null ? 0 : EmployeeList.Count();
            //        if (!String.IsNullOrEmpty(Name))
            //        {
            //            EmployeeList = EmployeeList.Where(s => s.Name.ToLower().Contains(Name.ToLower())).ToList();
            //        }
            //        if (!String.IsNullOrEmpty(DepartmentId))
            //        {
            //            EmployeeList = EmployeeList.Where(s => s.DepartmentId.ToUpper().Contains(DepartmentId.ToUpper())).ToList();
            //        }
            //        if (!String.IsNullOrEmpty(LeaveTypeId))
            //        {
            //            ViewData["CorporateLeaves"] = db.EngagementTypeMaster.Where(c => c.CorporateId == userdetails.SubscriberId && c.LeaveTypeId == LeaveTypeId).ToList();
            //        }
            //        if (SchemeId > 0)
            //        {
            //            ViewData["CorporateLeaves"] = db.EngagementTypeMaster.Where(c => c.CorporateId == userdetails.SubscriberId && c.SchemeId == SchemeId).ToList();
            //            EmployeeList = EmployeeList.Where(s => s.SchemeId == SchemeId).ToList();
            //        }
            //    }

            //    else if (userdetails.ManagerLevel == true)
            //    {
            //        EmployeeList = EmployeeList.Where(c => c.ReportingAuthority == userdetails.UserId).ToList();
            //        ViewBag.EmCount = EmployeeList == null ? 0 : EmployeeList.Count();
            //        if (!String.IsNullOrEmpty(Name))
            //        {
            //            EmployeeList = EmployeeList.Where(s => s.Name.ToLower().Contains(Name.ToLower())).ToList();
            //        }
            //        if (!String.IsNullOrEmpty(DepartmentId))
            //        {
            //            EmployeeList = EmployeeList.Where(s => s.DepartmentId.ToUpper().Contains(DepartmentId.ToUpper())).ToList();
            //        }
            //        if (!String.IsNullOrEmpty(LeaveTypeId))
            //        {
            //            ViewData["CorporateLeaves"] = db.EngagementTypeMaster.Where(c => c.CorporateId == userdetails.SubscriberId && c.ShortName == LeaveTypeId).ToList();
            //        }
            //    }
            //    else if (userdetails.Role == "Employee")
            //    {
            //        EmployeeList = EmployeeList.Where(c => c.UserId == userdetails.UserId).ToList();
            //        if (!String.IsNullOrEmpty(LeaveTypeId))
            //        {
            //            ViewData["CorporateLeaves"] = db.EngagementTypeMaster.Where(c => c.CorporateId == userdetails.SubscriberId && c.ShortName == LeaveTypeId).ToList();
            //        }
            //    }
            //}
            //if (!string.IsNullOrEmpty(Schedule))
            //{
            //    string[] strSchedule = Schedule.Split('-');

            //    DateTime Rangefrmdate = DateTime.ParseExact(strSchedule[0].Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture);

            //    DateTime Rangetodate = DateTime.ParseExact(strSchedule[1].Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture);

            //    if (Rangefrmdate != null && Rangetodate != null)
            //    {
            //        fromDate = Rangefrmdate;
            //        toDate = Rangetodate;
            //        diff = toDate.Subtract(fromDate).Days;
            //        date = fromDate.Date.AddDays(-1);
            //    }
            //    trainer = ems.GetEmployeeEngagement(userdetails.SubscriberId, fromDate, toDate);
            //}


            PopulatePaging(PageSize);
            ViewBag.Paging = PageSize;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            //  ViewData["EngagementDetails"] = trainer;
            return View(EmployeeList.ToPagedList(pageNumber, pageSize));
            //return View(EmployeeList);
        }

        [HttpGet]
        public ActionResult AddPayrollheads(string UserAction, Int16 PayrollHeadID = 0, bool Data = false)
        {
            string UserId = User.Identity.GetUserId();
            var userdetails = generic.GetUserDetail(UserId);
            ViewBag.UserId = userdetails.SubscriberId;
            ViewData["UserProfile"] = userdetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userdetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userdetails.SubscriberId).FirstOrDefault();
            PopulateMarksList();
            PopulatePeriod();
            PopulatePayrollCategory();
            if (Data == true)
            {
                ViewBag.Data = "Succeed";
            }
            if (UserAction == "Delete" && PayrollHeadID > 0)
            {
                admin.DeletePayrollHeads(PayrollHeadID);
                ViewBag.Result = "Deleted";
                return View();
            }
            var AllPayrollHeads = db.PayrollHeads.ToList();
            ViewData["AllPayrollHeads"] = AllPayrollHeads;
            var SelectedPayrollHead = AllPayrollHeads.Where(c => c.PayrollHeadID == PayrollHeadID).FirstOrDefault();
            if (PayrollHeadID > 0)
            {
                PopulateMarksList(SelectedPayrollHead.DefaultPercentage);
                PopulatePeriod(SelectedPayrollHead.Period);
                PopulatePayrollCategory(SelectedPayrollHead.Category);
            }
            return View(SelectedPayrollHead);
        }

        [HttpPost]
        public ActionResult AddPayrollheads(string HeadName, string PayrollCategory, string PeriodFrequencyStatus, bool TaxExemption, bool AvailableByDefault, float MaxLimit = 0, Int16 PayrollHeadID = 0, float Marks = 0)
        {
            bool res = admin.AddPayrollHeads(PayrollHeadID, HeadName, Marks, PayrollCategory, MaxLimit, PeriodFrequencyStatus, TaxExemption, AvailableByDefault);

            return RedirectToAction("AddPayrollheads", "Payroll", new { area = "PMS", Data = res });
        }

        [HttpGet]
        public ActionResult Payrollheads(Int64 CorporatePayrollHeadID = 0, string useraction = "Add", string savestatus = "")//, Int16 SchemeId = 0
        {
            PopulateMarksList();
            PopulatePeriod();
            PopulatePayrollCategory();
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            PopulatePayrollHeadName(userDetail.SubscriberId, useraction);
            ViewBag.Status = savestatus;
            ViewData["UserProfile"] = userDetail;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var Allpayrolls = userContext.CorporatePayrollHead.Where(c => c.CorporateId == userDetail.SubscriberId).ToList();
            if (useraction == "Edit")
            {
                var newAllPayrolls = Allpayrolls.Where(c => c.CorporatePayrollHeadID == CorporatePayrollHeadID).FirstOrDefault();
                ViewBag.HeadId = newAllPayrolls.PayrollHeadID;
                ViewBag.TaxExemption = newAllPayrolls.TaxExemption;
                ViewBag.MaxLimit = newAllPayrolls.MaxLimit;
                PopulatePayrollHeadName(userDetail.SubscriberId, useraction, newAllPayrolls.PayrollHeadID);
                PopulatePayrollCategory(newAllPayrolls.PayrollCategory);
                PopulatePeriod(newAllPayrolls.Period);
                PopulateMarksList(newAllPayrolls.PayrollPercent);
                ViewBag.CorporatePayrollHeadID = newAllPayrolls.CorporatePayrollHeadID;
                ViewBag.PayrollHeadId = newAllPayrolls.PayrollHeadID;
                ViewBag.Category = newAllPayrolls.PayrollCategory;
                ViewBag.PayrollHead = newAllPayrolls.PayrollHeadName;
                ViewBag.Mark = newAllPayrolls.PayrollPercent;
                ViewBag.Period = newAllPayrolls.Period;
                ViewBag.Maxlimit = newAllPayrolls.MaxLimit;
            }
            //admin.GetAllPayrollHeads(userDetail.SubscriberId).ToList();
            ViewData["Allpayrolls"] = Allpayrolls;
            return View(db.CorporatePayrollHead.Find(CorporatePayrollHeadID));
        }

        [HttpPost]
        public ActionResult Payrollheads(string CorporateId, string PayrollHead, CorporatePayrollHead corporatePayrollHead, string PeriodFrequencyStatus,
                                         string Maxlimits, string Category, string PayrollPercentage, string Periods, Int64 Marks = 0)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserDetail.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();

            if (corporatePayrollHead != null)
            {
                if (PeriodFrequencyStatus != null && (corporatePayrollHead.Period == "" || corporatePayrollHead.Period == null))
                {
                    corporatePayrollHead.Period = PeriodFrequencyStatus;
                }
                if (Marks != 0 && (corporatePayrollHead.PayrollPercent == 0))
                {
                    corporatePayrollHead.PayrollPercent = Marks;
                }
                if (PayrollHead != "" && (corporatePayrollHead.PayrollHeadName == "0" || corporatePayrollHead.PayrollHeadName == "" || corporatePayrollHead.PayrollHeadName == null))
                {
                    corporatePayrollHead.PayrollHeadName = PayrollHead;
                }
                if (Maxlimits != "" && (corporatePayrollHead.MaxLimit == 0))
                {
                    corporatePayrollHead.MaxLimit = Convert.ToSingle(Maxlimits);
                }
                if (Category != "" && (corporatePayrollHead.PayrollCategory == "" || corporatePayrollHead.PayrollCategory == null))
                {
                    corporatePayrollHead.PayrollCategory = Category;
                }
                if (PayrollPercentage != "" && (corporatePayrollHead.PayrollPercent == 0))
                {
                    corporatePayrollHead.PayrollPercent = Convert.ToSingle(PayrollPercentage);
                }
                if (Periods != "" && (corporatePayrollHead.Period == "" || corporatePayrollHead.Period == null))
                {
                    corporatePayrollHead.Period = Periods;
                }
                if (corporatePayrollHead.CorporatePayrollHeadID == 0)
                {
                    Regex reNum = new Regex(@"^\d+$");
                    bool isNumeric = reNum.Match(corporatePayrollHead.PayrollHeadName).Success;

                    var ExistingCheck = db.PayrollHeads.ToList();
                    if (isNumeric)
                    {
                        ExistingCheck = ExistingCheck.Where(c => c.PayrollHeadID == Convert.ToInt16(corporatePayrollHead.PayrollHeadName)).ToList();
                    }
                    else
                    {
                        ExistingCheck = ExistingCheck.Where(c => c.HeadName == corporatePayrollHead.PayrollHeadName).ToList();
                    }

                    if (ExistingCheck.Count() > 0)
                    {
                        corporatePayrollHead.PayrollHeadID = Convert.ToInt16(ExistingCheck.FirstOrDefault().PayrollHeadID);
                        corporatePayrollHead.PayrollHeadName = ExistingCheck.FirstOrDefault().HeadName;
                    }
                }
                bool res = admin.AddCorporatePayrollHeads(corporatePayrollHead.CorporatePayrollHeadID, UserDetail.SubscriberId,
                    corporatePayrollHead.PayrollHeadID, corporatePayrollHead.PayrollHeadName, corporatePayrollHead.PayrollPercent,
                    corporatePayrollHead.PayrollCategory, corporatePayrollHead.MaxLimit, corporatePayrollHead.Period,
                    corporatePayrollHead.TaxExemption, DateTime.Now);
            }
            return RedirectToAction("Payrollheads", "Payroll");
        }

        public ActionResult RemovePayrollHeads(Int64 CorporatePayrollHeadID)
        {
            var removeItem = db.CorporatePayrollHead.Find(CorporatePayrollHeadID);
            var EmployeeSalaryProcessCheck = db.EmployeeSalaryHeads.Where(c => c.CorporatePayrollHeadID == CorporatePayrollHeadID).ToList();

            if (EmployeeSalaryProcessCheck.Count() > 0)
            {
                string Status = "AllReadyUse";
                return RedirectToAction("Payrollheads", "Payroll", new { savestatus = Status });
            }
            if (removeItem != null)
            {
                db.CorporatePayrollHead.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("Payrollheads", "Payroll");
        }

        //public ActionResult RemovePayrollHeads(Int16 PayrollHeadID)
        //{
        //    var removeItem = db.CorporatePayrollHead.Find(PayrollHeadID);

        //    if (removeItem != null)
        //    {
        //        db.CorporatePayrollHead.Remove(removeItem);
        //        db.SaveChanges();
        //    }
        //    return RedirectToAction("Payrollheads", "Payroll");
        //}


        [HttpGet]
        public ActionResult SalaryProcess(string DepartmentId, string UserId, string DeactivatedUserId, Int16 Month = 0, string proess = "Intiate")
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserDetail.UserId).FirstOrDefault();
            PopulateEmployeeBiometric(UserDetail.SubscriberId);
            PopulateDepartment("EMS");
            PopulateDeactivatedEmployee(UserDetail.SubscriberId);
            PopulateMonth();
            var companySetting = db.CompanySetting.Where(c => c.CorporateId == UserDetail.SubscriberId).FirstOrDefault();
            ViewBag.PayoutDate = 31;
            if (companySetting != null && companySetting.AutoProcessPayrollDate != 0)
                ViewBag.PayoutDate = companySetting.AutoProcessPayrollDate;
            if (proess == "Processed")
            {
                if (string.IsNullOrEmpty(UserId))
                    UserId = DeactivatedUserId;


                int PayoutYear = DateTime.Now.Year;

                if (Month < 4)
                {
                    PayoutYear = PayoutYear - 1;
                }


                ViewData["PayrollHead"] = db.CorporatePayrollHead.Where(h => h.CorporateId == UserDetail.SubscriberId).ToList();
                ViewData["EmployeeMonthlySalary"] = pms.GetEmployeeMonthlySalary(UserDetail.SubscriberId, DepartmentId, UserId, Month, PayoutYear);
                ViewData["EmpMonthlySalaryHeads"] = pms.GetEmployeeMonthlySalaryDetails(UserDetail.SubscriberId, DepartmentId, UserId, Month, PayoutYear);
            }

            //var employeelist = ems.GetSubscriberWiseEmployeeList(UserDetail.SubscriberId);
            //var companypayrollheads = admin.GetAllPayrollHeads(UserDetail.SubscriberId).ToList();
            //var companydetails = cmsMgr.GetCompanyDetails(UserDetail.SubscriberId);
            //ViewData["Companydetails"] = companydetails;
            //if (!string.IsNullOrEmpty(UserId))
            //{
            //    var employee = employeelist.Where(e => e.UserId == UserId).FirstOrDefault();
            //    var employeesalarydetails = ems.GetEmployeeSalaryDetails(employee.UserId);
            //    List<EmployeeSalaryHeadsView> GrossEarning = new List<EmployeeSalaryHeadsView>();
            //    GrossEarning = ems.GetEmployeesalaryheadsDetails(employeesalarydetails.ESID).Where(c => c.PayrollCategory == "Earning").ToList();
            //    ViewData["GrossEarning"] = GrossEarning;
            //    EmployeeSalaryHeadsView[] GrossEarningHeads = GrossEarning.ToArray();
            //    float GrossEarningAmount = GrossEarningHeads.Sum(c => c.Amount);

            //    List<EmployeeSalaryHeadsView> GrossDeduction = new List<EmployeeSalaryHeadsView>();
            //    GrossDeduction = ems.GetEmployeesalaryheadsDetails(employeesalarydetails.ESID).Where(c => c.PayrollCategory == "Deduction").ToList();
            //    ViewData["GrossDeduction"] = GrossDeduction;
            //    EmployeeSalaryHeadsView[] GrossDeductions = GrossDeduction.ToArray();
            //    float GrossDeductionAmount = GrossDeductions.Sum(c => c.Amount);

            //    float TotalGrossCTC = GrossEarningAmount - GrossDeductionAmount;
            //    ViewBag.TotalGrossCTC = TotalGrossCTC;
            //    float GrossSalaryPerday = 0;
            //    float NetSalary = 0;
            //    int totalworkingdays = 0;
            //    // Employee Leave Calculations
            //    //if companyattendance is checked in company Settings
            //    //Start
            //    if (companydetails.CompanyAttendance == true)
            //    {
            //        DateTime fromDate = DateTime.Now;
            //        DateTime toDate = DateTime.Now;
            //        int month = Convert.ToInt32(Month);
            //        if (!string.IsNullOrEmpty(Month))
            //        {
            //            if (companydetails.LeavesCalculationCriteria == true)
            //            {
            //                var firstDay = new DateTime(DateTime.Now.Year, month, 1);
            //                var day29 = firstDay.AddDays(28);
            //                var day30 = firstDay.AddDays(29);
            //                var day31 = firstDay.AddDays(30);
            //                int totalsunday = 0;
            //                int totalsaturday = 0;

            //                if (companydetails.WorkingDayPerWeek == 6)
            //                {

            //                    if ((day29.Month == month && day29.DayOfWeek == DayOfWeek.Sunday)
            //                    || (day30.Month == month && day30.DayOfWeek == DayOfWeek.Sunday)
            //                    || (day31.Month == month && day31.DayOfWeek == DayOfWeek.Sunday))
            //                    {
            //                        totalsunday = 5;
            //                    }
            //                    else
            //                    {
            //                        totalsunday = 4;
            //                    }
            //                    fromDate = new DateTime(DateTime.Now.Year, month, 1);
            //                    toDate = fromDate.AddMonths(1);
            //                    var employeeAttendance = admin.GetBiometricData(UserDetail.SubscriberId);
            //                    employeeAttendance = employeeAttendance.Where(c => c.EmployeeId == UserId).ToList();
            //                    employeeAttendance = employeeAttendance.Where(c => c.CheckInDate >= fromDate && c.CheckOutDate <= toDate).ToList();
            //                    totalworkingdays = employeeAttendance.Count() - totalsunday;

            //                    //After Calculating Leaves

            //                    var CorporateEngagements = db.EngagementTypeMaster.Where(c => c.CorporateId == UserDetail.SubscriberId).ToList();

            //                    //Calculatng Monthly Leaves
            //                    List<TrainerPlannerView> Monthleaves = new List<TrainerPlannerView>();
            //                    Monthleaves = tmsMgr.GetTrainerPlaner(UserId);
            //                    Monthleaves = Monthleaves.Where(c => c.FromDate == fromDate && c.ToDate == toDate).ToList();
            //                    TrainerPlannerView[] MonthleavesList = Monthleaves.ToArray();

            //                    foreach (var item in MonthleavesList)
            //                    {
            //                        var leaves = item.EngagementTypeId;

            //                        var TotalLeavesConsumed = tmsMgr.GetTrainerLeaveCalculation(UserId).ToList();
            //                        var CalculatingLeaves = TotalLeavesConsumed.Where(c => c.SchemeId == employee.SchemeId && c.EngagementTypeId == item.EngagementTypeId).ToList();
            //                        var leavetype = CorporateEngagements.Where(c => c.EngagementTypeId == item.EngagementTypeId).FirstOrDefault();




            //                    }

            //                    //var selectedengagement = CorporateEngagements.Where(c => c.EngagementTypeId == EngagementTypeId && c.SchemeId == EmpScheme).FirstOrDefault();
            //                    //int totalLeaves = selectedengagement.LeaveLimit;
            //                    //EmloyeeLeaves Leaves = new EmloyeeLeaves();
            //                    //Leaves.TotalLeaves = totalLeaves;
            //                    ////var remaining = db.TrainerPlannerSummary.Where(c => c.TrainerId == UserId && c.EngagementTypeId == EngagementTypeId && c.SchemeId == EmpScheme).FirstOrDefault();
            //                    //var remainingleaves = tmsMgr.GetTrainerLeaveCalculation(UserId).ToList();
            //                    //var remaining = remainingleaves.Where(c => c.SchemeId == EmpScheme && c.EngagementTypeId == EngagementTypeId).FirstOrDefault();
            //                    //if (remaining == null)
            //                    //{
            //                    //    Leaves.OutstandingLeaves = totalLeaves;
            //                    //    return Json(Leaves, JsonRequestBehavior.AllowGet);
            //                    //}
            //                    //else
            //                    //{
            //                    //    Leaves.OutstandingLeaves = (totalLeaves - remaining.TotalDays);
            //                    //    return Json(Leaves, JsonRequestBehavior.AllowGet);
            //                    //}
            //                }
            //                else
            //                {
            //                    if ((day29.Month == month && day29.DayOfWeek == DayOfWeek.Sunday)
            //                    || (day30.Month == month && day30.DayOfWeek == DayOfWeek.Sunday)
            //                    || (day31.Month == month && day31.DayOfWeek == DayOfWeek.Sunday))
            //                    {
            //                        totalsunday = 5;
            //                    }
            //                    else
            //                    {
            //                        totalsunday = 4;
            //                    }
            //                    if ((day29.Month == month && day29.DayOfWeek == DayOfWeek.Saturday)
            //                    || (day30.Month == month && day30.DayOfWeek == DayOfWeek.Saturday)
            //                    || (day31.Month == month && day31.DayOfWeek == DayOfWeek.Saturday))
            //                    {
            //                        totalsaturday = 5;
            //                    }
            //                    else
            //                    {
            //                        totalsaturday = 4;
            //                    }
            //                    fromDate = new DateTime(DateTime.Now.Year, month, 1);
            //                    toDate = fromDate.AddMonths(1);
            //                    var employeeAttendance = admin.GetBiometricData(UserDetail.SubscriberId);
            //                    employeeAttendance = employeeAttendance.Where(c => c.EmployeeId == UserId).ToList();
            //                    employeeAttendance = employeeAttendance.Where(c => c.CheckInDate >= fromDate && c.CheckOutDate <= toDate).ToList();
            //                    totalworkingdays = employeeAttendance.Count() - (totalsunday + totalsaturday);

            //                    //After Calculating Leaves
            //                }
            //                GrossSalaryPerday = TotalGrossCTC / totalworkingdays;
            //                NetSalary = GrossSalaryPerday * totalworkingdays;

            //            }
            //            else
            //            {
            //                int totaldays = DateTime.DaysInMonth(DateTime.Now.Year, month);
            //                GrossSalaryPerday = TotalGrossCTC / totaldays;
            //                NetSalary = GrossSalaryPerday * totalworkingdays;
            //            }
            //        }
            //    }
            //    //END
            //}
            return View();
        }

        [HttpPost]
        public ActionResult SalaryProcess(string DepartmentId, string UserId, string DeactivatedUserId, Int16 Month = 0)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());

            if (string.IsNullOrEmpty(UserId))
                UserId = DeactivatedUserId;


            // int PayoutYear = DateTime.Now.AddMonths(-1).Year;

            int PayoutYear = DateTime.Now.Year;

            if (Month < 4)
            {
                PayoutYear = PayoutYear - 1;
            }
            if (Month < DateTime.Now.Month && PayoutYear <= DateTime.Now.Year)
            {

                pms.ProcessSalary(UserDetail.SubscriberId, DepartmentId, UserId, Month, UserDetail.UserId, PayoutYear);


                //ViewData["UserProfile"] = UserDetail;
                //ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
                //ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserDetail.UserId).FirstOrDefault();
                //PopulateEmployeeBiometric(UserDetail.SubscriberId,UserId);
                //PopulateDepartment("EMS",null,DepartmentId);
                //PopulateDeactivatedEmployee(UserDetail.SubscriberId,DeactivatedUserId);
                //PopulateMonth(Month);



                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            return Json("ProcessMonth", JsonRequestBehavior.AllowGet);
        }

        //PId = Employee Monthly Payout Id
        [HttpGet]
        public ActionResult Edit(Int64 PId)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserDetail.UserId).FirstOrDefault();
            var EmpSalary = pms.GetEmployeeMonthlySalary(PId);

            ViewData["EmpMonthlySalaryHeads"] = pms.GetEmployeeMonthlySalaryDetails(UserDetail.SubscriberId, EmpSalary.DepartmentId, EmpSalary.UserId, EmpSalary.PayoutMonth, EmpSalary.PayoutYear);

            return View(EmpSalary);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            //foreach (var key in form.AllKeys)
            //{
            //    var value = form[key];
            //    // etc.
            //}
            Int16 lwpDays = 0;
            float netSalary = 0;
            Int64 payoutId = 0;
            int payoutMonth = 0;

            foreach (var key in form.Keys)
            {
                string headid = key.ToString();
                int n;
                var isNumeric = int.TryParse(headid, out  n);
                if (isNumeric)
                {
                    var value = Convert.ToSingle(string.IsNullOrEmpty(form[key.ToString()]) ? "0" : form[key.ToString()]);
                    pms.UpdateSalaryHead(Convert.ToInt64(headid), value);

                }
                else if (headid == "LWP")
                {
                    lwpDays = Convert.ToInt16(string.IsNullOrEmpty(form[key.ToString()]) ? "0" : form[key.ToString()]);
                }
                else if (headid == "NetSalary")
                {
                    netSalary = Convert.ToSingle(string.IsNullOrEmpty(form[key.ToString()]) ? "0" : form[key.ToString()]);
                }
                else if (headid == "EmployeeMonthlySalaryPayoutId")
                {
                    payoutId = Convert.ToInt64(string.IsNullOrEmpty(form[key.ToString()]) ? "0" : form[key.ToString()]);
                }
                else if (headid == "PayoutMonth")
                {
                    payoutMonth = Convert.ToInt32(string.IsNullOrEmpty(form[key.ToString()]) ? "0" : form[key.ToString()]);
                }

            }
            pms.UpdateSalary(payoutId, lwpDays, netSalary);

            return RedirectToAction("SalaryProcess", "Payroll", new { Month = payoutMonth, proess = "Processed" });
        }

        [HttpPost]
        public ActionResult FreezeSalary(string DepartmentId, string UserId, string DeactivatedUserId, Int16 Month = 0)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());

            if (string.IsNullOrEmpty(UserId))
                UserId = DeactivatedUserId;


            // int PayoutYear = DateTime.Now.AddMonths(-1).Year;

            int PayoutYear = DateTime.Now.Year;

            if (Month < 4)
            {
                PayoutYear = PayoutYear - 1;
            }


            pms.FreezeSalary(UserDetail.SubscriberId, DepartmentId, UserId, Month, UserDetail.UserId, PayoutYear);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SalaryStatement(Int16 Month = 0, bool flag = true)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserDetail.UserId).FirstOrDefault();

            int PayoutYear = DateTime.Now.Year;

            if (Month < 4)
            {
                PayoutYear = PayoutYear - 1;
            }

            ViewBag.FinYear = Global.Month(Month) + " " + PayoutYear;

            PopulateMonth(Month);
            ViewBag.Flag = flag;

            ViewBag.PayoutMonth = Month;

            ViewData["EmployeeSalaryProcessedDetail"] = pms.ProcessedSalaryStatement(Month, PayoutYear);

            return View();
        }

        [HttpPost]
        public ActionResult SalaryStatement(Int16 Month)
        {

            return RedirectToAction("SalaryStatement", "Payroll", new { Month = Month, flag = true });
        }

        [HttpGet]
        public ActionResult EmployeeSalaryStatement(DateTime pdate, Int16 Month)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());

            int PayoutYear = DateTime.Now.Year;

            if (Month < 4)
            {
                PayoutYear = PayoutYear - 1;
            }

            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/SalaryStatement.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@SubscriberId", UserDetail.SubscriberId));
            cmd.Parameters.Add(new SqlParameter("@PayoutMonth", Month));
            cmd.Parameters.Add(new SqlParameter("@PayoutYear", PayoutYear));
            cmd.Parameters.Add(new SqlParameter("@UpdatedOn", pdate));
            cmd.Connection = thisConnection;

            string MyDataSource1 = "USP_GetSalaryStatements";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];


            ReportParameter[] parms = new ReportParameter[4];
            parms[0] = new ReportParameter("SubscriberId", UserDetail.SubscriberId);
            parms[1] = new ReportParameter("PayoutMonth", Month.ToString());
            parms[2] = new ReportParameter("PayoutYear", PayoutYear.ToString());
            parms[3] = new ReportParameter("UpdatedOn", pdate.ToString());
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
            byte[] bytes = rptViewer.LocalReport.Render("EXCEL", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=SalaryStatement" + Global.Month(Month) + "" + PayoutYear + ".xls");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            return RedirectToAction("SalaryStatement", "Payroll", new { Month = Month, Flage = true });
        }

        [HttpGet]
        public ActionResult Payslip(string UId, Int16 M = 0)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = UserDetail;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserDetail.UserId).FirstOrDefault();

            ViewBag.UserId = UserDetail.UserId;
            ViewBag.PayoutMonth = 6;
            //  PopulateDepartment("EMS");
            PopulateDeactivatedEmployee(UserDetail.SubscriberId);
            PopulateMonth();
            //var companySetting = db.CompanySetting.Where(c => c.CorporateId == UserDetail.SubscriberId).FirstOrDefault();


            //    if (string.IsNullOrEmpty(UserId))
            //        UserId = DeactivatedUserId;

            //    int PayoutYear = DateTime.Now.Year;

            //    if (Month > 3)
            //    {
            //        PayoutYear = PayoutYear - 1;
            //    }
            if (UserDetail.Role == "Employee")
            {
                PopulateEmployeeBiometric(UserDetail.SubscriberId, UserDetail.UserId);
                ViewBag.VisibilityFlag = "none";
            }
            else
            {
                PopulateEmployeeBiometric(UserDetail.SubscriberId);
                ViewBag.VisibilityFlag = "block";
            }

            return View();
        }

        public ActionResult DownloadPayslip(string UserId, string DeactiveUser, Int16 Month = 0)
        {

            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());

            int PayoutYear = DateTime.Now.Year;

            if (Month < 4)
            {
                PayoutYear = PayoutYear - 1;
            }


            ReportViewer rptViewer = new ReportViewer();
            rptViewer.LocalReport.ReportPath = "Report/EmployeePayslip.rdlc";
            string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(thisConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Add(new SqlParameter("@SubscriberId", UserDetail.SubscriberId));
            cmd.Parameters.Add(new SqlParameter("@PayoutMonth", Month));
            cmd.Parameters.Add(new SqlParameter("@PayoutYear", PayoutYear));
            cmd.Parameters.Add(new SqlParameter("@DepartmentId", DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", UserId));
            cmd.Connection = thisConnection;

            string MyDataSource1 = "USP_GetEmployeeMonthlySalary";
            cmd.CommandText = string.Format(MyDataSource1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN = new SqlDataAdapter(cmd);
            System.Data.DataSet DataSet1 = new System.Data.DataSet();
            daN.Fill(DataSet1);
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = DataSet1.Tables[0];

            SqlCommand cmd2 = new SqlCommand();
            cmd2.Parameters.Add(new SqlParameter("@SubscriberId", UserDetail.SubscriberId));
            cmd2.Parameters.Add(new SqlParameter("@PayoutMonth", Month));
            cmd2.Parameters.Add(new SqlParameter("@PayoutYear", PayoutYear));
            cmd2.Parameters.Add(new SqlParameter("@DepartmentId", DBNull.Value));
            cmd2.Parameters.Add(new SqlParameter("@EmployeeId", UserId));
            cmd2.Connection = thisConnection;
            string MyDataSource2 = "USP_GetEmployeeMonthlyDeduction";
            cmd2.CommandText = string.Format(MyDataSource2);
            cmd2.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN2 = new SqlDataAdapter(cmd2);
            System.Data.DataSet DataSet2 = new System.Data.DataSet();
            daN2.Fill(DataSet2);
            ReportDataSource reportDataSource2 = new ReportDataSource();
            reportDataSource2.Name = "DataSet2";
            reportDataSource2.Value = DataSet2.Tables[0];

            SqlCommand cmd3 = new SqlCommand();
            cmd3.Parameters.Add(new SqlParameter("@SubscriberId", UserDetail.SubscriberId));
            cmd3.Parameters.Add(new SqlParameter("@PayoutMonth", Month));
            cmd3.Parameters.Add(new SqlParameter("@PayoutYear", PayoutYear));
            cmd3.Parameters.Add(new SqlParameter("@DepartmentId", DBNull.Value));
            cmd3.Parameters.Add(new SqlParameter("@EmployeeId", UserId));
            cmd3.Connection = thisConnection;
            string MyDataSource3 = "USP_GetEmployeeMonthlyEarning";
            cmd3.CommandText = string.Format(MyDataSource3);
            cmd3.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter daN3 = new SqlDataAdapter(cmd3);
            System.Data.DataSet DataSet3 = new System.Data.DataSet();
            daN3.Fill(DataSet3);
            ReportDataSource reportDataSource3 = new ReportDataSource();
            reportDataSource3.Name = "DataSet3";
            reportDataSource3.Value = DataSet3.Tables[0];



            ReportParameter[] parms = new ReportParameter[5];
            parms[0] = new ReportParameter("SubscriberId", UserDetail.SubscriberId);
            parms[1] = new ReportParameter("PayoutMonth", Month.ToString());
            parms[2] = new ReportParameter("PayoutYear", PayoutYear.ToString());
            parms[3] = new ReportParameter("DepartmentId", string.Empty);
            parms[4] = new ReportParameter("EmployeeId", UserId);
            rptViewer.LocalReport.SetParameters(parms);

            rptViewer.LocalReport.DataSources.Add(reportDataSource);
            rptViewer.LocalReport.DataSources.Add(reportDataSource2);
            rptViewer.LocalReport.DataSources.Add(reportDataSource3);

            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            var reList = rptViewer.LocalReport.ListRenderingExtensions();
            string mimeType = string.Empty;
            string encoding = string.Empty;
            rptViewer.LocalReport.Refresh();
            byte[] bytes = rptViewer.LocalReport.Render("PDF", null);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            //Response.AddHeader("content-disposition", "attachment; filename=Payslip" + Month.ToString() + PayoutYear.ToString() + ".pdf");
            Response.AddHeader("content-disposition", "attachment; filename=Payslip.pdf");
            Response.BinaryWrite(bytes); // create the file
            Response.Flush();


            //ViewData["UserProfile"] = UserDetail;
            //ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            //ViewData["EmpDetails"] = ems.GetEmployeeBasicDetails(UserDetail.UserId).FirstOrDefault();


            //PopulateDeactivatedEmployee(UserDetail.SubscriberId);
            //PopulateMonth();

            //if (UserDetail.Role == "Employee")
            //{
            //    PopulateEmployeeBiometric(UserDetail.SubscriberId, UserDetail.UserId);
            //    ViewBag.VisibilityFlag = "none";
            //}
            //else
            //{
            //    PopulateEmployeeBiometric(UserDetail.SubscriberId);
            //    ViewBag.VisibilityFlag = "block";
            //}

            return View();
        }

        [HttpGet]
        public ActionResult EmployeeConfirmation()
        {
            string UserId = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetail;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            var employeeconfirmation = admin.GetEmployeeConfirmationDetails(UserId);
            return View(employeeconfirmation);
        }
        private void PopulateMarksList(Object selectName = null)
        {
            var MarksfullList = Global.GetMarks();
            SelectList Marks = new SelectList(MarksfullList, "Marks", "Marks", selectName);
            ViewBag.Marks = Marks;
        }

        private void PopulatePeriod(Object selectName = null)
        {
            var PeriodFrequencyList = Global.GetPeriodFrequencyList();
            SelectList PeriodFrequencyStatus = new SelectList(PeriodFrequencyList, "PeriodFrequencyStatus", "PeriodFrequencyStatus", selectName);
            ViewBag.PeriodFrequencyStatus = PeriodFrequencyStatus;
        }

        private void PopulateLeaveType(object selectedLeave = null)
        {
            var query = generic.GetLeavetype();
            SelectList leavetype = new SelectList(query, "LeaveTypeId", "LeaveTypeName", selectedLeave);
            ViewBag.LeaveTypeId = leavetype;
        }

        private void PopulateattendanceFilter(object selectedLeave = null)
        {
            var query = Global.GetattendanceFilterList();
            SelectList attendanceFilter = new SelectList(query, "attendance", "attendance", selectedLeave);
            ViewBag.attendanceFilter = attendanceFilter;
        }

        private void PopulatePayrollCategory(Object selectName = null)
        {
            var PayrollCategoryList = Global.GetPayrollCategoryList();
            SelectList PayrollCategory = new SelectList(PayrollCategoryList, "PayrollCategory", "PayrollCategory", selectName);
            ViewBag.PayrollCategory = PayrollCategory;
        }

        private void PopulateEmployeeForManager(string SubscriberId, object selectedValue = null)
        {
            Generic generic = new Generic();
            var AssignedToList = generic.GetEmployee(SubscriberId).Where(e => e.ManagerLevel == false).ToList();
            ViewBag.EmployeeList = new SelectList(AssignedToList, "UserId", "Name", selectedValue);
        }

        private void PopulateEmployee(string SubscriberId, object selectedValue = null)
        {
            Generic generic = new Generic();
            var AssignedToList = generic.GetEmployee(SubscriberId);
            ViewBag.EmployeeList = new SelectList(AssignedToList, "UserId", "Name", selectedValue);
        }

        private void PopulateEmployeeBiometric(string SubscriberId, object selectedValue = null)
        {
            Generic generic = new Generic();
            var AssignedToList = generic.GetEmployeewithDepartment(SubscriberId);
            SelectList UserId = new SelectList(AssignedToList, "UserId", "Name", selectedValue);
            ViewBag.UserId = UserId;
        }

        private void PopulateDeactivatedEmployee(string SubscriberId, object selectedValue = null)
        {
            Generic generic = new Generic();
            var AssignedToList = generic.GetDeactivatedEmployeewithDepartment(SubscriberId);
            SelectList DeactivatedUserId = new SelectList(AssignedToList, "UserId", "Name", selectedValue);
            ViewBag.DeactivatedUserId = DeactivatedUserId;
        }


        private void PopulateBiometricMonth(object selectedValue = null)
        {
            Generic generic = new Generic();
            var months = generic.GetMonths();
            SelectList Month = new SelectList(months, "MonthId", "MonthName", selectedValue);
            ViewBag.Month = Month;
        }

        private void PopulateYear()
        {
            Generic generic = new Generic();
            var YearList = generic.GetYear().ToList();
            ViewBag.YearList = new SelectList(YearList, "Year", "Year");
        }

        private void PopulateMonth(object selectedValue = null)
        {
            Generic generic = new Generic();
            var months = generic.GetPayoutMonths();
            SelectList Month = new SelectList(months, "MonthId", "MonthName", selectedValue);
            ViewBag.Month = Month;
        }


        private void salryCalculatedOn(string salryCalculatedOn = null)
        {
            Generic generic = new Generic();
            var SalaryCalCulatedOn = generic.GetSalaryCalculationOnList().ToList();
            ViewBag.SalaryCalCulatedOn = new SelectList(SalaryCalCulatedOn, "SalarycalculationOnId", "SalarycalculationOnName", salryCalculatedOn);
        }

        private void holidayInSalary(string holiday = null)
        {
            Generic generic = new Generic();
            var holidayinSalary = generic.GetHolidaysInSalaryList().ToList();
            ViewBag.HolidayinSalary = new SelectList(holidayinSalary, "Id", "Name", holiday);
        }


        private void PayrollHeadSettingHeadList(string HeadId = null)
        {
            var HeadList = admin.GetPayrollHeadList().ToList();
            ViewBag.HeadId = new SelectList(HeadList, "HeadId", "HeadName", HeadId);
        }

        private void PayrollHeadCalculationMethodList(string Id = null)
        {
            Generic generic = new Generic();
            var methodList = generic.GetHeadCalculationMethodList().ToList();
            ViewBag.MethodName = new SelectList(methodList, "Id", "Name", Id);
        }


        //----Vikash Das ------------//

        private void PopulateLetterTemplate(string UserId = null, object selectedvalue = null, Int64 TemplateId = 0)
        {
            var query = tmsMgr.GetLetterTemplateDetail(UserId, TemplateId);
            SelectList TemplateList = new SelectList(query, "TemplateId", "Name", selectedvalue);
            ViewBag.TemplateId = TemplateList;
        }

        private void PopulateEmployeeList(string UserId = null, object selectedvalue = null)
        {
            var query = tmsMgr.GetEmployeeDetailList(UserId);
            SelectList EmployeeList = new SelectList(query, "UserId", "Name", selectedvalue);
            ViewBag.UserId = EmployeeList;
        }

        private void PopulateLetterTypeList(string UserId = null, object selectedvalue = null, Int64 LetterTypeId = 0)
        {
            var query = tmsMgr.GetLetterList(UserId, LetterTypeId);
            SelectList LetterTypeList = new SelectList(query, "LetterTypeId", "LetterTypeName", selectedvalue);
            ViewBag.LetterTypeId = LetterTypeList;
        }

        private void PopulateAssetsGroup(string CorporateId, object selectedvalue = null)
        {
            var query = tmsMgr.GetAssetGroupDetail(CorporateId);
            SelectList AssetGroupList = new SelectList(query, "AssetGroupId", "AssetGroupName", selectedvalue);
            ViewBag.AssetGroupId = AssetGroupList;
        }

        private void PopulateCategoryType(object selectedvalue = null)
        {
            var query = generic.GetCategory();
            SelectList CategoryTypeList = new SelectList(query, "CategoryType", "CategoryTypeName", selectedvalue);
            ViewBag.CategoryType = CategoryTypeList;
        }

        private void PopulateGrade(object selectedValue = null)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            AdminManager AdminManager = new DAL.AdminManager();
            var GradeList = AdminManager.GetGrade(UserDetail.SubscriberId);
            SelectList EmployementGrade = new SelectList(GradeList, "GradeId", "GradeName", selectedValue);
            ViewBag.GradeId = EmployementGrade;
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

        private void PopulatePaging(object selectedValue = null)
        {
            var PageList = generic.GetPaging();
            ViewBag.PageSize = new SelectList(PageList, "PageSize", "PageSize", selectedValue);
        }

        private void PopulateSchema(object selectedvalue = null, Int16 SchemeId = 0)
        {
            var query = tmsMgr.GetSchemaList(SchemeId);
            SelectList SchemaList = new SelectList(query, "SchemeId", "SchemeName", selectedvalue);
            ViewBag.SchemeId = SchemaList;
        }

        private void PopulateDepartment(string ModuleId, string DepartmentId = null, object selectedDepartment = null)
        {
            Generic generic = new Generic();
            var query = generic.GetModuleWiseDepartments(ModuleId);
            if (ModuleId == "CMS")
                if (!string.IsNullOrEmpty(DepartmentId))
                {
                    query = query.Where(q => q.DepartmentId == DepartmentId).ToList();
                }
                else
                    query = query.Where(q => q.DepartmentId == "CLI").ToList();

            SelectList Departments;
            if (!string.IsNullOrEmpty(DepartmentId))
                Departments = new SelectList(query, "DepartmentId", "Department", selectedDepartment);
            else
                Departments = new SelectList(query, "DepartmentId", "RoleDepartment", selectedDepartment);

            ViewBag.DepartmentId = Departments;
        }

        private void PopulatePayrollHeadName(string SubscriberId, string useraction, Object selectName = null)
        {
            var PayrollHeadNameList = admin.GetAllPayrollHeads(SubscriberId).Where(c => c.PayrollHeadID != 1 && c.PayrollHeadID != 2 && c.PayrollHeadID != 15 && c.PayrollHeadID != 16);
            if (useraction == "Edit")
            {
                PayrollHeadNameList = admin.GetAllPayrollHeads(SubscriberId).Where(c => c.PayrollHeadID != 1 && c.PayrollHeadID != 2);
            }
            SelectList PayrollHeadName = new SelectList(PayrollHeadNameList, "PayrollHeadID", "PayrollHeadName", selectName);
            ViewBag.PayrollHeadName = PayrollHeadName;
        }


        private void PopulateBranches(string CorporateId, object selectedvalue = null)
        {
            var operation = userContext.CorporateBranch.Where(c => c.CorporateId == CorporateId).ToList();
            ViewBag.BranchId = new SelectList(operation, "BranchId", "BranchName", selectedvalue);
        }

        private void PopulateAuthType(object selectedvalue = null)
        {
            var auth = Global.GetAuthenticateTypes();
            ViewBag.Authenticate = new SelectList(auth, "Authenticate", "AuthenticateType", selectedvalue);
        }
    }
}