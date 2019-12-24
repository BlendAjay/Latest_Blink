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
    public class InvoiceController : Controller
    {
        AdminManager admin = new AdminManager();
        Generic generic = new Generic();
        CMSManager cmsMgr = new CMSManager();
        EMSManager emsMgr = new EMSManager();
        UserDBContext db = new UserDBContext();

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

        // GET: CMS/Invoice
        [HttpGet]
        //[Authorize(Roles = "Admin,Client,Employee")]
        public ActionResult Index(string Status, string Id, string InvoiceTo, string InvoiceNumber, bool data = false, string UserAction = "Add")
        {
            ViewBag.Result = "Failed";
            if (data == true)
            {
                ViewBag.Result = "Succeeded";
            }

            string UserId = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = generic.GetUserDetail(UserId);
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;

            ViewData["JOItems"] = cmsMgr.GetJobOrderItems(Id).AsEnumerable();
            ViewData["ItemType"] = (from i in db.ItemTypeMasters.Where(i => i.CorporateId == UserDetail.SubscriberId) select i).AsEnumerable();
            ViewData["InvItems"] = cmsMgr.GetInvoiceItems(InvoiceNumber).AsEnumerable();
            ViewData["InvTaxs"] = admin.GetInvoiceTaxes(InvoiceNumber).AsEnumerable();
            ViewData["TaxType"] = admin.GetTaxMaster().AsEnumerable();
            ViewData["ItemDuration"] = Global.GetDuration();
            ViewBag.UserAction = UserAction;
            if (UserAction == "Edit")
            {
                var Invoice = admin.GetInvoiceDetails(UserId, InvoiceNumber);
                ViewBag.InvoiceDate = Invoice.InvoiceDate;
                ViewBag.Currency = Invoice.Currency;
                ViewBag.InvoiceNumber = InvoiceNumber;
                ViewBag.InvoiceTo = Invoice.InvoiceTo;
                ViewBag.ReferenceId = Invoice.ReferenceId;
                ViewBag.NetAmount = Invoice.NetAmount;
                ViewBag.AdditionalCost = Invoice.AdditionalCost;
                ViewBag.Deductions = Invoice.Deductions;
                ViewData["Content"] = (from i in db.InVoiceAttachment.Where(i => i.ReferenceId == Invoice.ReferenceId) select i).FirstOrDefault();
                PopulateCurrency(Invoice.Currency);
                return View(Invoice);
            }
            else
            {
                ViewBag.InvoiceTo = InvoiceTo;
                ViewBag.ReferenceId = Id;
                ViewBag.InvoiceDate = DateTime.Now.Date;
                ViewBag.AdditionalCost = 0;
                ViewBag.Deductions = 0;
                PopulateCurrency();
            }

            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Client,Employee")]
        public ActionResult Index(GenerateInvoice GenerateInvoice, string[] ItemId, string[] ItemType, string[] ItemDescription, string[] Unit, string[] UnitPrice, string[] ItemDuration, string[] Actions, string[] ActionTax,
                                            Int64[] Taxation, Int64[] TaxationId, float[] TaxactionAmount, string[] CalculatedTax, float NetAmount = 0, float AdditionalCost = 0, float Deductions = 0, float GrandTotal = 0)
        {
            string body = "";
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            GenerateInvoice.SubscriberId = userDetails.SubscriberId;
            GenerateInvoice.CorporateId = userDetails.UserId;
            GenerateInvoice.Status = "Submitted";
            GenerateInvoice.Acknowledge = false;
            GenerateInvoice.Total = NetAmount;
            GenerateInvoice.NetAmount = GrandTotal;

            if (string.IsNullOrEmpty(GenerateInvoice.InvoiceNumber))
            {
                body = "Invoice has been created Invoice No. ";
            }
            else
            {
                body = "Invoice has been edited Invoice No. ";
            }

            if (GenerateInvoice.InvoiceNumber == null)
            {
                GenerateInvoice.InvoiceNumber = admin.GetInvoiceNumber();
            }


            var result = admin.AddInvoice(GenerateInvoice, ItemId, ItemType, ItemDescription, Unit, UnitPrice, ItemDuration, Actions, Taxation, CalculatedTax, ActionTax);

            admin.AddNotification(userDetails.SubscriberId, userDetails.UserId, body + GenerateInvoice.InvoiceNumber + " From ", "INVOICE", GenerateInvoice.InvoiceNumber, false, DateTime.Now);

            if (result == true)
            {
                if (!string.IsNullOrEmpty(GenerateInvoice.ReferenceId))
                {
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
                        admin.uploadInvFile(GenerateInvoice.ReferenceId, attachment);
                    }
                }
            }
            string message1 = "You have received an Invoice (" + GenerateInvoice.InvoiceNumber + ") from " + userDetails.Name; //eg "message hello ";                
         //   generic.sendSMSMessage(message1, generic.GetUserDetail(userDetails.SubscriberId).PhoneNumber);
            generic.sendSMS(message1, generic.GetUserDetail(userDetails.SubscriberId).PhoneNumber);
            return RedirectToAction("MyInvoices", "Invoice", new { area = "CMS", data = result });
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult Tax(Int64 TaxationId = 0)
        {
            var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetail;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewData["taxmaster"] = admin.GetTaxMaster().AsEnumerable().Where(i => i.CorporateId == userDetail.SubscriberId).ToList();
            return View(db.TaxMaster.Find(TaxationId));
        }

        [HttpPost]
        public ActionResult Tax(TaxMaster taxation)
        {
            var UserDetail = generic.GetUserDetail(User.Identity.GetUserId());
            taxation.CorporateId = UserDetail.SubscriberId;
            admin.AddTaxMaster(taxation.TaxationId, taxation.TaxName, taxation.TaxationValue, taxation.CorporateId);
            return RedirectToAction("Tax", "Invoice");
        }
        public ActionResult RemoveTax(Int64 TaxationId)
        {
            var removeItem = db.TaxMaster.Find(TaxationId);

            if (removeItem != null)
            {
                db.TaxMaster.Remove(removeItem);
                db.SaveChanges();
            }
            return RedirectToAction("Tax", "Invoice", new { area = "CMS" });
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult GenerateInvoice(string Status, string Id, string InvoiceTo, bool CV, string InvoiceNumber, bool data = false, string UserAction = "Add")
        {
            ViewBag.Result = "Failed";
            if (data == true)
            {
                ViewBag.Result = "Succeeded";
            }

            string UserId = User.Identity.GetUserId();
            var UserDetail = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = UserDetail;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(UserDetail.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(UserDetail.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewData["TaxType"] = admin.GetTaxMaster().AsEnumerable();
            ViewData["JOItems"] = cmsMgr.GetJobOrderItems(Id).AsEnumerable();
            ViewData["InvItems"] = cmsMgr.GetInvoiceItems(InvoiceNumber).AsEnumerable();
            ViewData["InvTaxs"] = admin.GetInvoiceTaxes(InvoiceNumber).AsEnumerable();

            ViewBag.InvoiceTo = InvoiceTo;
            ViewBag.ReferenceId = Id;


            if (UserAction == "Edit")
            {
                var Invoice = admin.GetInvoiceDetails(UserId, InvoiceNumber);
                ViewBag.InvoiceDate = Invoice.InvoiceDate;
                ViewBag.Currency = Invoice.Currency;
                ViewBag.InvoiceNumber = InvoiceNumber;
                ViewBag.InvoiceTo = Invoice.InvoiceTo;
                ViewBag.ReferenceId = Invoice.ReferenceId;
                ViewBag.AdditionalCost = Invoice.AdditionalCost;
                ViewBag.Deductions = Invoice.Deductions;
                ViewBag.NetAmount = Invoice.NetAmount;
                ViewData["Content"] = (from i in db.InVoiceAttachment.Where(i => i.ReferenceId == Invoice.ReferenceId) select i).FirstOrDefault();

                return View(Invoice);
            }
            else
            {
                var JobOrder = cmsMgr.GetJobOrderDetails(UserDetail.SubscriberId, Id, CV);
                ViewBag.InvoiceSubject = JobOrder.Subject;
                ViewBag.InvoiceDate = DateTime.Now.Date;
                ViewBag.Currency = JobOrder.Currency;
                ViewBag.AdditionalCost = 0;
                ViewBag.Deductions = 0;
                ViewBag.NetAmount = JobOrder.TotalCost;
            }


            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult GenerateInvoice(GenerateInvoice GenerateInvoice, string[] ItemId, string[] ItemType, string[] ItemDescription, string[] Unit, string[] UnitPrice, string[] ItemDuration, string[] Actions, string[] ActionTax,
                                            Int64[] Taxation, Int64[] TaxationId, float[] TaxactionAmount, string[] CalculatedTax, float NetAmount = 0, float AdditionalCost = 0, float Deductions = 0, float GrandTotal = 0)
        {
            string body = "";
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            GenerateInvoice.SubscriberId = userDetails.SubscriberId;
            GenerateInvoice.CorporateId = userDetails.UserId;
            GenerateInvoice.Status = "Submitted";
            GenerateInvoice.Acknowledge = false;
            GenerateInvoice.Total = NetAmount;
            GenerateInvoice.NetAmount = GrandTotal;

            if (string.IsNullOrEmpty(GenerateInvoice.InvoiceNumber))
            {
                body = "Invoice has been created Invoice No. ";
            }
            else
            {
                body = "Invoice has been edited Invoice No. ";
            }

            if (GenerateInvoice.InvoiceNumber == null)
            {
                GenerateInvoice.InvoiceNumber = admin.GetInvoiceNumber();
            }


            var result = admin.AddInvoice(GenerateInvoice, ItemId, ItemType, ItemDescription, Unit, UnitPrice, ItemDuration, Actions, Taxation, CalculatedTax, ActionTax);

            admin.AddNotification(userDetails.SubscriberId, userDetails.UserId, body + GenerateInvoice.InvoiceNumber + " From ", "INVOICE", GenerateInvoice.InvoiceNumber, false, DateTime.Now);

            if (result == true)
            {
                if (!string.IsNullOrEmpty(GenerateInvoice.ReferenceId))
                {
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase attachment = Request.Files[file] as HttpPostedFileBase;
                        admin.uploadInvFile(GenerateInvoice.ReferenceId, attachment);
                    }
                }
            }

            return RedirectToAction("MyJobOrders", "JobOrder", new { area = "CMS", IsClientView = false, data = result });
        }
        [HttpGet]
        //[Authorize(Roles = "Admin,Client,Employee")]
        public ActionResult MyInvoices(string Invoice, string Id, string ReferenceId, string Tag, string Status, string cid, string InvoiceNumber, string UserAction = "Add")
        {
            var userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewBag.UserId = userDetails.SubscriberId;
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;

            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            ViewData["Content"] = db.InVoiceAttachment.ToList();
            //If Client has team members with all rights
            if (userDetails.CorporateId != null && userDetails.CorporateId != userDetails.SubscriberId)
            {
                userDetails.UserId = userDetails.CorporateId;
            }
            if (Invoice != null)
            {
                PopulateEmployee(userDetails.SubscriberId);
                PopulateInvoiceStates(Status);
                return View(admin.GETInvoiceFor(Invoice));
            }
            if (Id != null && ReferenceId != null && Status != null)
            {
                admin.UpdateInvoiceStatus(Id, ReferenceId, Status);
            }
            if (Tag == "Incoming")
            {
                PopulateEmployee(userDetails.SubscriberId);
                PopulateInvoiceStates(Status);
            }
            else
            {
                PopulateVander(userDetails.SubscriberId);
                PopulateInvoiceStates(Status);
            }
            if (UserAction == "Delete" && InvoiceNumber != null && ReferenceId != null)
            {
                if (admin.RemoveInvoice(ReferenceId, InvoiceNumber, userDetails.SubscriberId))
                {
                    admin.AddNotification(userDetails.SubscriberId, userDetails.UserId, "Invoice " + InvoiceNumber + " has been deleted by ", "Invoice", InvoiceNumber, false, DateTime.Now);
                }
            }
            return View(admin.GetInvoice(userDetails.UserId, Tag, Status, cid).OrderByDescending(c => c.InvoiceNumber));


        }



        [HttpPost]
        //[Authorize(Roles = "Admin,Client")]
        public ActionResult MyInvoices(string Tag, string Status)
        {
            ViewBag.Tag = Tag;
            return RedirectToAction("MyInvoices", "Invoice", new { area = "CMS", Tag, Status });
        }


        [HttpGet]
        //[Authorize(Roles = "Admin,Client,Employee")]
        public ActionResult InvoiceDetails(string Id, string AddressType = "")
        {
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;

            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            ViewData["InvoiceItems"] = admin.GetInvoiceItems(Id).AsEnumerable();
            ViewData["InvoiceTaxes"] = admin.GetInvoiceTaxes(Id).AsEnumerable();
            ViewData["Content"] = db.InVoiceAttachment.ToList();
            return View(admin.GetInvoiceDetails(userDetails.SubscriberId, Id));

        }


        public ActionResult Payment(string Id, string res = null)
        {
            ViewBag.Status = res;
            UserViewModel userDetails = generic.GetUserDetail(User.Identity.GetUserId());
            ViewData["UserProfile"] = userDetails;
            ViewData["EmpDetails"] = emsMgr.GetEmployeeBasicDetails(userDetails.UserId).FirstOrDefault();
            ViewData["CompanyLogo"] = cmsMgr.GetCompanyLogo(userDetails.SubscriberId).FirstOrDefault();
            var plandetail = admin.GetUserplanDetails(userDetails.SubscriberId).Where(c => c.AddOnId == 3).FirstOrDefault();
            ViewData["plandetail"] = plandetail;
            PopulatePaymentModeType();

            var invoiceDetails = admin.GetInvoiceDetails(userDetails.SubscriberId, Id);

            return View(invoiceDetails);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Client")]
        public ActionResult Payment(string InvoiceNumber, float NetAmount, short PaymentModeId, string BankName, string ReferenceNumber, string PayerRemarks, string PaymentDate)
        {
            string Result = string.Empty;
            try
            {
                var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
                var userPrimaryDetail = cmsMgr.GetSubscriberWiseClientList(userDetail.SubscriberId).Where(s => s.CorporateId == userDetail.UserId).FirstOrDefault();
                string strReturnUrl = Global.WebsiteUrl() + "CMS/Invoice/PaymentConfirmation";
                DateTime? Pdate = DateTime.Now;
                if (!string.IsNullOrEmpty(PaymentDate))
                {
                    Pdate = DateTime.ParseExact(PaymentDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                if (db.PaymentModeMaster.Find(PaymentModeId).PaymentMode.ToUpper() == "ONLINE")
                {

                    Result = generic.PaymentRequest(InvoiceNumber, userDetail.Name, Convert.ToString(NetAmount), userPrimaryDetail.Email, userPrimaryDetail.PhoneNumber, PayerRemarks, strReturnUrl);

                    if (!Result.ToUpper().StartsWith("ERROR"))
                    {
                        bool result = admin.UpdateInvoicePayment(InvoiceNumber, BankName, ReferenceNumber, Pdate, PayerRemarks, PaymentModeId);
                        if (result)
                            return Redirect(Url.Content(Global.WebsiteUrl() + "Request.aspx?Id=" + Result));
                    }

                    return RedirectToAction("Payment", "Invoice", new { Area = "CMS", Id = InvoiceNumber, res = Result });

                }

                bool results = admin.UpdateInvoicePayment(InvoiceNumber, BankName, ReferenceNumber, Pdate, PayerRemarks, PaymentModeId);
                if (results)
                    return RedirectToAction("PaymentConfirmation", "Invoice", new { Area = "CMS", PMId = PaymentModeId, Id = InvoiceNumber });

            }
            catch (Exception ex)
            {
                Result = ex.ToString();
            }
            return RedirectToAction("Payment", "Invoice", new { Area = "CMS", Id = InvoiceNumber, res = Result });
        }

        [HttpGet]
        public ActionResult PaymentConfirmation(short PMId = 0, string Id = null)
        {

            if (PMId == 0)
            {
                string Result = string.Empty;

                var strPGResponse = Convert.ToString(System.Web.HttpContext.Current.Request["msg"]);
                if (strPGResponse != "" || strPGResponse != null)
                {
                    Generic generic = new Generic();
                    Result = generic.ConfirmTransaction(strPGResponse);

                    string[] status = Result.Split('|');
                    string clientRfNo = status[0];
                    string transactionstatus = status[1];

                    ViewBag.Result = transactionstatus;
                    if (!string.IsNullOrEmpty(clientRfNo))
                    {

                        var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
                        var userPrimaryDetail = cmsMgr.GetSubscriberWiseClientList(userDetail.SubscriberId).Where(s => s.CorporateId == userDetail.UserId).FirstOrDefault();
                        ViewData["userPrimaryDetail"] = userPrimaryDetail;

                        var invoiceDetails = admin.GetInvoiceDetails(userDetail.SubscriberId, Id);
                        admin.UpdateInvoiceStatus(invoiceDetails.InvoiceNumber, invoiceDetails.ReferenceId, "Paid");
                        return View(invoiceDetails);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var userDetail = generic.GetUserDetail(User.Identity.GetUserId());
                    var userPrimaryDetail = cmsMgr.GetSubscriberWiseClientList(userDetail.SubscriberId).Where(s => s.CorporateId == userDetail.UserId).FirstOrDefault();
                    ViewData["userPrimaryDetail"] = userPrimaryDetail;

                    var invoiceDetails = admin.GetInvoiceDetails(userDetail.SubscriberId, Id);
                    admin.UpdateInvoiceStatus(invoiceDetails.InvoiceNumber, invoiceDetails.ReferenceId, "Paid");
                    return View(invoiceDetails);
                }

            }
            return View();
        }

        public ActionResult getMyInvoices(string Tag, string Status, string CorporateId)
        {
            String UserId = User.Identity.GetUserId();
            var Invoices = admin.GetInvoice(UserId, Tag, Status, CorporateId);
            return Json(Invoices, JsonRequestBehavior.AllowGet);
        }

        private void PopulateInvoiceStatus(object selectedValue = null)
        {
            var InvoiceStatusList = Global.GetInvoiceStatusList();
            ViewBag.InvoiceStatusList = new SelectList(InvoiceStatusList, "StatusName", "StatusName", selectedValue);

        }


        private void PopulateInvoiceStates(object selectedValue = null)
        {
            var InvoiceStates = Global.GetInvoiceStateList();
            ViewBag.Status = new SelectList(InvoiceStates, "StatusName", "StatusName", selectedValue);
        }


        private void PopulateEmployee(string SubscriberId, object selectedValue = null)
        {
            EMSManager emsMgr = new EMSManager();
            var query = emsMgr.GetSubscriberWiseEmployeeList(SubscriberId).Where(e => e.DepartmentId != "ADI").ToList();
            ViewBag.Name = new SelectList(query, "UserId", "Name", selectedValue);
        }

        private void PopulateVander(string SubscriberId, object selectedValue = null)
        {
            EMSManager emsMgr = new EMSManager();
            var Clients = cmsMgr.GetSubscriberWiseClientList(SubscriberId).Where(c => c.DepartmentId == "CLI").ToList();
            ViewBag.Name = new SelectList(Clients, "CorporateId", "Name", selectedValue);
        }
        private void PopulatePaymentModeType(object selectedPaymentModeType = null)
        {
            EMSManager ems = new EMSManager();
            var query = ems.PaymentModeTypeList();
            SelectList PaymentModeId = new SelectList(query, "PaymentModeId", "PaymentMode", selectedPaymentModeType);
            ViewBag.PaymentModeId = PaymentModeId;
        }

        //private void PopulateTaxation(object selectedTaxation = null)
        //{
        //    var taxationList = admin.GetTaxMaster();
        //    SelectList taxList = new SelectList(taxationList, "TaxationId", "TaxName", selectedTaxation);
        //    ViewBag.Taxation = taxList;
        //}

        [HttpGet]
        public JsonResult GetTaxes()
        {
            var taxationList = admin.GetTaxMaster().Where(t => t.CorporateId == generic.GetUserDetail(User.Identity.GetUserId()).SubscriberId).ToList();
            return Json(taxationList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTaxationValue(Int64 TaxationId)
        {
            var taxationValue = db.TaxMaster.Find(TaxationId).TaxationValue;
            return Json(taxationValue, JsonRequestBehavior.AllowGet);
        }

        private void PopulateCurrency(Object selectedCurrency = null)
        {
            var query = generic.GetCurrency();
            SelectList CurrencyList = new SelectList(query, "Currency", "Currency", selectedCurrency);
            ViewBag.Currency = CurrencyList;
        }
    }
}