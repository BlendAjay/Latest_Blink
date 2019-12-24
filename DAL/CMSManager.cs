using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AJSolutions.Areas.CMS.Models;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using AJSolutions.Models;
using AJSolutions.Areas.Admin.Models;
using System.Data.Entity;
using AJSolutions.Areas.EMS.Models;
using System.Data;
namespace AJSolutions.DAL
{

    public class CMSManager
    {
        UserDBContext udbc = new UserDBContext();
        Generic generic = new Generic();
        BlobManager blobManager = new BlobManager();
        AdminManager admin = new AdminManager();
        public List<CorporateViewModel> GetCorporateProfile(string UserId = null)
        {

            var profile = new List<CorporateViewModel>();

            using (var db = new UserDBContext())
            {
                profile = db.Database.SqlQuery<CorporateViewModel>("EXEC USP_GETCORPORATEPROFILE @CorporateId", new SqlParameter("@CorporateId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).ToList();
            }

            return profile;
        }

        //bank Details
        public bool AddBankDetails(string CorporateId, string BankName, string AccountNumber, string AccountOwner, string IfscCode, string BranchCode, string BranchAddress, string ContactNumber)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var bankname = new SqlParameter("@BankName", string.IsNullOrEmpty(BankName) ? DBNull.Value : (object)BankName);
                    var accountnumber = new SqlParameter("@AccountNumber", string.IsNullOrEmpty(AccountNumber) ? DBNull.Value : (object)AccountNumber);
                    var accountowner = new SqlParameter("@AccountOwner", string.IsNullOrEmpty(AccountOwner) ? DBNull.Value : (object)AccountOwner);
                    var ifsccode = new SqlParameter("@IfscCode", string.IsNullOrEmpty(IfscCode) ? DBNull.Value : (object)IfscCode);
                    var branchcode = new SqlParameter("@BranchCode", string.IsNullOrEmpty(BranchCode) ? DBNull.Value : (object)BranchCode);
                    var branchaddress = new SqlParameter("@BranchAddress", string.IsNullOrEmpty(BranchAddress) ? DBNull.Value : (object)BranchAddress);
                    var contactnumber = new SqlParameter("@ContactNumber", string.IsNullOrEmpty(ContactNumber) ? DBNull.Value : (object)ContactNumber);


                    int i = context.Database.ExecuteSqlCommand("USP_ADDBANKDETAILS @CorporateId, @BankName, @AccountNumber, @AccountOwner, @IfscCode, @BranchCode, @BranchAddress, @ContactNumber", corporateid, bankname, accountnumber, accountowner, ifsccode, branchcode, branchaddress, contactnumber);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }


        public BankDetails GetBankDetails(string CorporateId)
        {
            var bankdetails = new BankDetails();
            using (var db = new UserDBContext())
            {
                bankdetails = db.Database
                         .SqlQuery<BankDetails>("exec USP_GetBANKDEATAILS @CorporateId",
                            new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).FirstOrDefault();
            }

            return bankdetails;
        }


        //Company profile
        public bool AddCompanyProfile(string CorporateId, string CompanyName, string CompanyType,
                                      string CompanySize, string Website, DateTime UpdatedOn, string UpdatedBy,
                                      string ProvidentFund, string PANCardNo, string TaxDeductionAccNo,
                                      string EmployeeStateInsurance, string GSTTax)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var companyname = new SqlParameter("@CompanyName", string.IsNullOrEmpty(CompanyName) ? DBNull.Value : (object)CompanyName);
                    var companytype = new SqlParameter("@CompanyType", string.IsNullOrEmpty(CompanyType) ? DBNull.Value : (object)CompanyType);
                    var companysize = new SqlParameter("@CompanySize", string.IsNullOrEmpty(CompanySize) ? DBNull.Value : (object)CompanySize);
                    var website = new SqlParameter("@Website", string.IsNullOrEmpty(Website) ? DBNull.Value : (object)Website);
                    var updatedon = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var providentFund = new SqlParameter("@ProvidentFund", string.IsNullOrEmpty(ProvidentFund) ? DBNull.Value : (object)ProvidentFund);
                    var pANCardNo = new SqlParameter("@PANCardNo", string.IsNullOrEmpty(PANCardNo) ? DBNull.Value : (object)PANCardNo);
                    var taxDeductionAccNo = new SqlParameter("@TaxDeductionAccNo", string.IsNullOrEmpty(TaxDeductionAccNo) ? DBNull.Value : (object)TaxDeductionAccNo);
                    var employeeStateInsurance = new SqlParameter("@EmployeeStateInsurance", string.IsNullOrEmpty(EmployeeStateInsurance) ? DBNull.Value : (object)EmployeeStateInsurance);
                    var gstTax = new SqlParameter("@GSTTax", string.IsNullOrEmpty(GSTTax) ? DBNull.Value : (object)GSTTax);


                    int i = context.Database.ExecuteSqlCommand("USP_AddCompanyProfiles @CorporateId, @CompanyName, @CompanyType, @CompanySize," +
                                                                "@Website, @UpdatedOn, @UpdatedBy, @ProvidentFund, @PANCardNo, @TaxDeductionAccNo" +
                                                                ",@EmployeeStateInsurance, @GSTTax", corporateid, companyname, companytype,
                                                                companysize, website, updatedon, updatedBy, providentFund, pANCardNo,
                                                                taxDeductionAccNo, employeeStateInsurance, gstTax);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }

        //Company profiles
        public CompanyProfile GetCompanyProfile(string CorporateId = null)
        {
            var companyprofiles = new CompanyProfile();
            using (var db = new UserDBContext())
            {
                companyprofiles = db.Database
                         .SqlQuery<CompanyProfile>("exec USP_GetCompanyProfiles @CorporateId",
                            new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).FirstOrDefault();
            }

            return companyprofiles;
        }

        public List<CompanyProfile> GetCompanyProfileList(string CorporateId = null)
        {
            var companyprofiles = new List<CompanyProfile>();
            using (var db = new UserDBContext())
            {
                companyprofiles = db.Database
                         .SqlQuery<CompanyProfile>("exec USP_GetCompanyProfiles @CorporateId",
                            new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
            }

            return companyprofiles;
        }

        //Corporate Profile
        public bool AddCorporateProfile(string CorporateId, string Name, string AlternateContact, string AlternateEmail, string Nationality, string DepartmentId, string SubscriberId, DateTime UpdatedOn, string UpdatedBy)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
                    var alternatecontact = new SqlParameter("@AlternateContact", string.IsNullOrEmpty(AlternateContact) ? DBNull.Value : (object)AlternateContact);
                    var alternateemail = new SqlParameter("@AlternateEmail", string.IsNullOrEmpty(AlternateEmail) ? DBNull.Value : (object)AlternateEmail);
                    var nationality = new SqlParameter("@Nationality", string.IsNullOrEmpty(Nationality) ? DBNull.Value : (object)Nationality);
                    var departmentid = new SqlParameter("@DepartmentId", string.IsNullOrEmpty(DepartmentId) ? DBNull.Value : (object)DepartmentId);
                    var subscriberid = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                    var updatedon = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCorporateProfiles @CorporateId, @Name, @AlternateContact, @AlternateEmail, @Nationality, @DepartmentId, @SubscriberId, @UpdatedOn, @UpdatedBy", corporateid, name, alternatecontact, alternateemail, nationality, departmentid, subscriberid, updatedon, updatedBy);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }

        //Address Details
        public void AddAddressDetails(string CorporateId, string AddressType, string AddressLine1, string AddressLine2, int City, int State, string PostalCode, int Country, string FaxNo)
        {
            try
            {
                using (var context = new UserDBContext())
                {

                    var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var addresstype = new SqlParameter("@AddressType", string.IsNullOrEmpty(AddressType) ? DBNull.Value : (object)AddressType);
                    var addressline1 = new SqlParameter("@AddressLine1", string.IsNullOrEmpty(AddressLine1) ? DBNull.Value : (object)AddressLine1);
                    var addressline2 = new SqlParameter("@AddressLine2", string.IsNullOrEmpty(AddressLine2) ? DBNull.Value : (object)AddressLine2);
                    var city = new SqlParameter("@City", City);
                    var state = new SqlParameter("@State", State);
                    var postalcode = new SqlParameter("@PostalCode", string.IsNullOrEmpty(PostalCode) ? DBNull.Value : (object)PostalCode);
                    var country = new SqlParameter("@Country", Country);
                    var faxNo = new SqlParameter("@FaxNo", string.IsNullOrEmpty(FaxNo) ? DBNull.Value : (object)FaxNo);


                    int i = context.Database.ExecuteSqlCommand("USP_ADDAddress @CorporateId, @AddressType, @AddressLine1, @AddressLine2, @City, @State, @PostalCode, @Country, @FaxNo", corporateid, addresstype, addressline1, addressline2, city, state, postalcode, country, faxNo);

                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

        }

        //Address Details
        public List<AddressViewModel> GetAddressDetails(string CorporateId)
        {
            var AddressList = new List<AddressViewModel>();
            using (var db = new UserDBContext())
            {
                AddressList = db.Database
                         .SqlQuery<AddressViewModel>("exec USP_GetAddresses @CorporateId",
                            new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
            }

            return AddressList;
        }

        public AddressViewModel GetAddressDetail(string CorporateId)
        {
            var AddressList = new AddressViewModel();
            using (var db = new UserDBContext())
            {
                AddressList = db.Database
                         .SqlQuery<AddressViewModel>("exec USP_GetAddresses @CorporateId",
                            new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).FirstOrDefault();
            }

            return AddressList;
        }

        public bool RemoveAddressDetails(string CorporateId, string AddressType)
        {
            bool result = false;
            try
            {
                using (var context = new UserDBContext())
                {

                    var corporateid = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var addresstype = new SqlParameter("@AddressType", string.IsNullOrEmpty(AddressType) ? DBNull.Value : (object)AddressType);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteAddress @CorporateId, @AddressType", corporateid, addresstype);

                    if (i > 0)
                        result = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return result;
        }

        public bool AddMeetingMinutes(Int64 MeetingId, string MeetingSubject, DateTime? MeetingDate, string MeetingRemarks, string MeetingHost, string Participants, string SubscriberId, string InternalRemarks, DateTime UpdatedOn, string UpdatedBy, string Location)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var meetingId = new SqlParameter("@MeetingId", MeetingId);
                    var meetingSubject = new SqlParameter("MeetingSubject", string.IsNullOrEmpty(MeetingSubject) ? DBNull.Value : (object)MeetingSubject);
                    var meetingDate = new SqlParameter("@MeetingDate", MeetingDate);
                    var meetingRemarks = new SqlParameter("@MeetingRemarks", string.IsNullOrEmpty(MeetingRemarks) ? DBNull.Value : (object)MeetingRemarks);
                    var meetingHost = new SqlParameter("@MeetingHost", string.IsNullOrEmpty(MeetingHost) ? DBNull.Value : (object)MeetingHost);
                    var participants = new SqlParameter("@Participants", string.IsNullOrEmpty(Participants) ? DBNull.Value : (object)Participants);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                    var internalRemarks = new SqlParameter("@InternalRemarks", string.IsNullOrEmpty(InternalRemarks) ? DBNull.Value : (object)InternalRemarks);
                    var updatedon = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var location = new SqlParameter("@Location", string.IsNullOrEmpty(Location) ? DBNull.Value : (object)Location);

                    int i = context.Database.ExecuteSqlCommand("USP_AddMeetingMinutes @MeetingId, @MeetingSubject, @MeetingDate, @MeetingRemarks, @MeetingHost, @Participants, @SubscriberId , @InternalRemarks, @UpdatedOn, @UpdatedBy, @Location",
                                                                                        meetingId, meetingSubject, meetingDate, meetingRemarks, meetingHost, participants, subscriberId, internalRemarks, updatedon, updatedBy, location);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }

        public List<MeetingMinutesView> GetMeetingMinutes(string MeetingHost = null, Int64 MeetingId = 0)
        {
            var details = new List<MeetingMinutesView>();
            using (var db = new UserDBContext())
            {
                details = db.Database
                         .SqlQuery<MeetingMinutesView>("exec USP_GetMeetingMInutes @MeetingHost, @MeetingId",
                         new SqlParameter("@MeetingHost", string.IsNullOrEmpty(MeetingHost) ? DBNull.Value : (object)MeetingHost),
                         new SqlParameter("@MeetingId", MeetingId)).ToList();
            }

            return details;
        }

        //public string CreatJobOrder(JobOrder jobOrder)
        //{
        //    if (GetJobOrderExist(jobOrder.JobOrderNumber))
        //    {
        //        var jobOrderDetail = udbc.JobOrder.Find(jobOrder.JobOrderNumber);

        //        jobOrderDetail.Accomodation = jobOrder.Accomodation;
        //        jobOrderDetail.Attendance = jobOrder.Attendance;
        //        jobOrderDetail.ClientId = jobOrder.ClientId;
        //        jobOrderDetail.Conditions = jobOrder.Conditions;
        //        jobOrderDetail.Currency = jobOrder.Currency;              
        //        jobOrderDetail.Description = jobOrder.Description;
        //        jobOrderDetail.Duration = jobOrder.Duration;
        //        jobOrderDetail.ExpRange = jobOrder.ExpRange;
        //        jobOrderDetail.Feedback = jobOrder.Feedback;
        //        jobOrderDetail.FunctionalPosition = jobOrder.FunctionalPosition;
        //        jobOrderDetail.Industry = jobOrder.Industry;
        //        jobOrderDetail.JobOrderTypeId = jobOrder.JobOrderTypeId;
        //        jobOrderDetail.JOPostedOn = jobOrder.JOPostedOn;               
        //        jobOrderDetail.SalaryRange = jobOrder.SalaryRange;
        //        jobOrderDetail.StartDate = jobOrder.StartDate;
        //        jobOrderDetail.Subject = jobOrder.Subject;
        //        jobOrderDetail.SubscriberId = jobOrder.SubscriberId;
        //        jobOrderDetail.TotalCost = jobOrder.TotalCost;
        //        jobOrderDetail.UpdatedBy = jobOrder.UpdatedBy;
        //        jobOrderDetail.UpdatedOn = DateTime.UtcNow;


        //        udbc.Entry(jobOrderDetail).State = EntityState.Modified;
        //        udbc.SaveChanges();
        //        return jobOrder.JobOrderNumber;
        //    }
        //    else
        //    {
        //        jobOrder.JobOrderNumber = GetJobOrderNumber();
        //        jobOrder.SubscriberId = generic.GetSubscriberId(jobOrder.ClientId);
        //        jobOrder.JobOrderStatus = "Unassigned";
        //        jobOrder.JOPostedOn = DateTime.UtcNow;
        //        jobOrder.UpdatedBy = jobOrder.UpdatedBy;
        //        jobOrder.UpdatedOn = DateTime.UtcNow;
        //        udbc.JobOrder.Add(jobOrder);
        //        udbc.SaveChanges();
        //        return jobOrder.JobOrderNumber;
        //    }

        //}

        public string CreatJobOrder(JobOrder jobOrder, string[] ItemId, string[] ItemType, string[] ItemDescription, string[] Unit, string[] UnitPrice, string[] ItemDuration, string[] Actions)
        {
            try
            {

                DataTable JOItem = new DataTable();
                JOItem.Columns.Add("JobOrderNumber");
                JOItem.Columns.Add("ItemId");
                JOItem.Columns.Add("ItemTypeId");
                JOItem.Columns.Add("ItemDescription");
                JOItem.Columns.Add("Unit");
                JOItem.Columns.Add("UnitPrice");
                JOItem.Columns.Add("Duration");
                JOItem.Columns.Add("Actions");


                string jobNumber = "";
                string itemDuration = "";
                int itemId = 0;


                if (jobOrder.JobOrderNumber == null)
                {
                    jobNumber = GetJobOrderNumber();
                }
                else
                {
                    jobNumber = jobOrder.JobOrderNumber;
                }

                if (ItemType != null)
                {
                    for (int i = 0; i < ItemType.Length; i++)
                    {
                        if (ItemDuration[i].ToString() == "NA")
                        {
                            itemDuration = "0";
                        }
                        else
                        {
                            itemDuration = ItemDuration[i].ToString();
                        }

                        if (ItemId[i].ToString() == "0")
                        {
                            itemId = 0;
                        }
                        else
                        {
                            itemId = Convert.ToInt32(ItemId[i]);
                        }

                        if ((ItemType[i] != "0" || !string.IsNullOrEmpty(ItemDescription[i])) && (!string.IsNullOrEmpty(Unit[i]) && !string.IsNullOrEmpty(UnitPrice[i])))
                        {
                            JOItem.Rows.Add(jobNumber, itemId, ItemType[i], ItemDescription[i],
                                                      Unit[i], UnitPrice[i], itemDuration, Actions[i]);
                        }

                    }
                }


                using (var context = new UserDBContext())
                {

                    var jobOrderNumber = new SqlParameter("@JobOrderNumber", string.IsNullOrEmpty(jobNumber) ? DBNull.Value : (object)jobNumber);
                    var clientId = new SqlParameter("@ClientId", string.IsNullOrEmpty(jobOrder.ClientId) ? DBNull.Value : (object)jobOrder.ClientId);
                    var subject = new SqlParameter("@Subject", string.IsNullOrEmpty(jobOrder.Subject) ? DBNull.Value : (object)jobOrder.Subject);
                    var functionalPosition = new SqlParameter("@FunctionalPosition", string.IsNullOrEmpty(jobOrder.FunctionalPosition) ? DBNull.Value : (object)jobOrder.FunctionalPosition);
                    var currency = new SqlParameter("@Currency", string.IsNullOrEmpty(jobOrder.Currency) ? DBNull.Value : (object)jobOrder.Currency);
                    var totalCost = new SqlParameter("@TotalCost", jobOrder.TotalCost);
                    var description = new SqlParameter("@Description", string.IsNullOrEmpty(jobOrder.Description) ? DBNull.Value : (object)jobOrder.Description);
                    var conditions = new SqlParameter("@Conditions", string.IsNullOrEmpty(jobOrder.Conditions) ? DBNull.Value : (object)jobOrder.Conditions);
                    var jOPostedOn = new SqlParameter("@JOPostedOn", DateTime.Now);
                    var salaryRange = new SqlParameter("@SalaryRange", string.IsNullOrEmpty(jobOrder.SalaryRange) ? DBNull.Value : (object)jobOrder.SalaryRange);
                    var expRange = new SqlParameter("@ExpRange", string.IsNullOrEmpty(jobOrder.ExpRange) ? DBNull.Value : (object)jobOrder.ExpRange);
                    var industry = new SqlParameter("@Industry", string.IsNullOrEmpty(jobOrder.Industry) ? DBNull.Value : (object)jobOrder.Industry);
                    var feedback = new SqlParameter("@Feedback", jobOrder.Feedback);
                    var attendance = new SqlParameter("@Attendance", jobOrder.Attendance);
                    var accomodation = new SqlParameter("@Accomodation", jobOrder.Accomodation);
                    var jobOrderTypeId = new SqlParameter("@JobOrderTypeId", jobOrder.JobOrderTypeId);
                    var startDate = new SqlParameter("@StartDate", DBNull.Value);
                    if (jobOrder.StartDate != null)
                        startDate = new SqlParameter("@StartDate", jobOrder.StartDate);
                    var duration = new SqlParameter("@Duration", jobOrder.Duration);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(jobOrder.SubscriberId) ? DBNull.Value : (object)jobOrder.SubscriberId);
                    var jobOrderStatus = new SqlParameter("@JobOrderStatus", string.IsNullOrEmpty(jobOrder.JobOrderStatus) ? DBNull.Value : (object)jobOrder.JobOrderStatus);
                    var updatedOn = new SqlParameter("@UpdatedOn", DateTime.Now);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(jobOrder.UpdatedBy) ? DBNull.Value : (object)jobOrder.UpdatedBy);
                    var completedOn = new SqlParameter("@CompletedOn", DBNull.Value);
                    if (jobOrder.CompletedOn != null)
                        completedOn = new SqlParameter("@CompletedOn", jobOrder.CompletedOn);
                    var jobItemsTb = new SqlParameter("@JobItemsTb", SqlDbType.Structured);
                    jobItemsTb.TypeName = "dbo.tvpJobOrderItem";
                    jobItemsTb.Value = JOItem;

                    int i = context.Database.ExecuteSqlCommand("USP_AddJOBORDER @JobOrderNumber, @ClientId, @Subject, @FunctionalPosition, @Currency, @TotalCost, @Description, " +
                                                                " @Conditions, @JOPostedOn,  @SalaryRange, @ExpRange, @Industry, @Feedback,  @Attendance,  @Accomodation, " +
                                                                " @JobOrderTypeId,@StartDate, @Duration, @SubscriberId, @JobOrderStatus, @UpdatedOn, @UpdatedBy,@CompletedOn, @JobItemsTb",
                                                               jobOrderNumber, clientId, subject, functionalPosition, currency, totalCost, description,
                                                               conditions, jOPostedOn, salaryRange, expRange, industry, feedback, attendance, accomodation,
                                                               jobOrderTypeId, startDate, duration, subscriberId, jobOrderStatus, updatedOn, updatedBy, completedOn, jobItemsTb);

                    if (i > 0)
                    {
                        return jobNumber;
                    }
                }
                return null;
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return null;
        }

        public string uploadFile(string JobOrderNumber, HttpPostedFileBase upload)
        {
            string res = "Failure";
            if (upload != null && upload.ContentLength > 0)
            {

                var file = udbc.JobOrderAttachment.Where(d => d.JobOrderNumber == JobOrderNumber).FirstOrDefault();

                if (file != null)
                {
                    Int64 FileId = file.FileId;

                    blobManager.DeleteBlob(JobOrderNumber.ToLower(), GetFileName(FileId).ToLower());
                    udbc.JobOrderAttachment.Remove(file);
                    udbc.SaveChanges();
                }


                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");


                bool added = AddJOAttachmentToDB(JobOrderNumber, imgFileName.ToLower(), upload.ContentType);

                if (added)
                {
                    blobManager.UploadBlob(JobOrderNumber.ToLower(), ReplaceFileName(JobOrderNumber).ToLower(), upload);

                }

                res = "Succeed";
            }


            return res;
        }


        public string uploadTaskFile(string TaskId, HttpPostedFileBase upload)
        {
            string res = "Failure";
            if (upload != null && upload.ContentLength > 0)
            {

                var file = udbc.TaskFinalAttachment.Where(d => d.TaskId == TaskId).FirstOrDefault();

                if (file != null)
                {
                    Int64 FileId = file.FileId;

                    blobManager.DeleteBlob(TaskId.ToLower(), GetTaskFileName(FileId).ToLower());
                    udbc.TaskFinalAttachment.Remove(file);
                    udbc.SaveChanges();
                }


                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");


                bool added = AddTaskFinalAttachmentToDB(TaskId, imgFileName.ToLower(), upload.ContentType);

                if (added)
                {
                    blobManager.UploadBlob(TaskId.ToLower(), ReplaceFileNameFinalTask(TaskId).ToLower(), upload);

                }

                res = "Succeed";
            }


            return res;
        }

        public bool AddTaskFinalAttachmentToDB(string TaskId, string FileName, string ContentType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var taskId = new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);

                    int i = context.Database.ExecuteSqlCommand("USP_AddTaskFinalAttachment @TaskId, @FileName, @ContentType",
                        taskId, fileName, contentType);

                    if (i == 1)
                        res = true;
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }

        public List<TaskFinalAttachment> GetTaskFinalAttachments(string TaskId)
        {
            var taskAttachment = new List<TaskFinalAttachment>();
            try
            {
                using (var context = new UserDBContext())
                {
                    taskAttachment = udbc.Database
                            .SqlQuery<TaskFinalAttachment>("EXEC USP_GetTaskFinalAttachments @TaskId",
                             new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return taskAttachment;
        }

        public string GetTaskFileName(Int64 FileId)
        {
            string fileName = null;
            var image = udbc.TaskFinalAttachment.Find(FileId);

            if (image != null)
            {
                fileName = "finalattachment/" + image.FileId + "/" + image.FileName;
            }
            return fileName;
        }

        public string GetFileName(Int64 FileId)
        {
            string fileName = null;
            var image = udbc.JobOrderAttachment.Find(FileId);

            if (image != null)
            {
                fileName = "attachment/" + image.FileId + "/" + image.FileName;
            }
            return fileName;
        }

        public string GetFileNameCompanyLogo(Int64 FileId)
        {
            string fileName = null;
            var image = udbc.AdminLogoFile.Find(FileId);

            if (image != null)
            {
                fileName = "attachment/" + image.FileId + "/" + image.FileName;
            }
            return fileName;
        }

        public string ReplaceFileNameFinalTask(string TaskId)
        {
            string fileName = null;
            var image = udbc.TaskFinalAttachment.Where(f => f.TaskId == TaskId);

            if (image != null)
            {
                if (image.Count() > 0)
                {
                    string imgFileName = image.FirstOrDefault().FileName;
                    imgFileName = imgFileName.Replace(' ', '_');
                    imgFileName = imgFileName.Replace("'", "_");


                    fileName = "finalattachment/" + image.FirstOrDefault().FileId + "/" + imgFileName;
                }
            }
            return fileName;
        }

        public string ReplaceFileNameCompanyLogo(string CorporateId)
        {
            string fileName = null;
            var image = udbc.AdminLogoFile.Where(f => f.CorporateId == CorporateId);

            if (image != null)
            {
                if (image.Count() > 0)
                {
                    string imgFileName = image.FirstOrDefault().FileName;
                    imgFileName = imgFileName.Replace(' ', '_');
                    imgFileName = imgFileName.Replace("'", "_");


                    fileName = "attachment/" + image.FirstOrDefault().FileId + "/" + imgFileName;
                }
            }
            return fileName;
        }

        public string ReplaceFileName(string JoborderNumber)
        {
            string fileName = null;
            var image = udbc.JobOrderAttachment.Where(f => f.JobOrderNumber == JoborderNumber);

            if (image != null)
            {
                if (image.Count() > 0)
                {
                    string imgFileName = image.FirstOrDefault().FileName;
                    imgFileName = imgFileName.Replace(' ', '_');
                    imgFileName = imgFileName.Replace("'", "_");


                    fileName = "attachment/" + image.FirstOrDefault().FileId + "/" + imgFileName;
                }
            }
            return fileName;
        }

        public bool AddJOAttachmentToDB(string JobOrderNumber, string FileName, string ContentType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var jobOrderNumber = new SqlParameter("@JobOrderNumber", string.IsNullOrEmpty(JobOrderNumber) ? DBNull.Value : (object)JobOrderNumber);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);

                    int i = context.Database.ExecuteSqlCommand("USP_AddJoborderAttachment @JobOrderNumber, @FileName, @ContentType",
                        jobOrderNumber, fileName, contentType);

                    if (i == 1)
                        res = true;
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }

        public bool AddCompanyLogo(string CorporateId, string FileName, string ContentType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCompanylogo @CorporateId, @FileName, @ContentType",
                        corporateId, fileName, contentType);

                    if (i == 1)
                        res = true;
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }

        public List<AdminLogoFile> GetCompanyLogo(string CorporateId)
        {
            var logo = new List<AdminLogoFile>();
            using (var db = new UserDBContext())
            {
                logo = db.Database.SqlQuery<AdminLogoFile>("EXEC USP_GetCompanyLogo @CorporateId",
                    new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).ToList();
            }
            return logo;
        }


        public bool RemoveJobOrder(string JobOrderNumber, string SubscriberId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    if (!string.IsNullOrEmpty(ReplaceFileName(JobOrderNumber)))
                        blobManager.DeleteBlob(JobOrderNumber.ToLower(), ReplaceFileName(JobOrderNumber).ToLower());

                    var jobOrderNumber = new SqlParameter("@JobOrderNumber", string.IsNullOrEmpty(JobOrderNumber) ? DBNull.Value : (object)JobOrderNumber);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteJobOrder @JobOrderNumber, @SubscriberId",
                        jobOrderNumber, subscriberId);

                    if (i > 0)
                    {
                        res = true;
                    }
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }

        public bool RemoveTask(string TaskId, string SubscriberId)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    if (!string.IsNullOrEmpty(ReplaceFileName(TaskId)))
                        blobManager.DeleteBlob(TaskId.ToLower(), admin.ReplaceFileName(TaskId).ToLower());

                    var taskId = new SqlParameter("@TaskId", string.IsNullOrEmpty(TaskId) ? DBNull.Value : (object)TaskId);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteTask @TaskId, @SubscriberId",
                        taskId, subscriberId);

                    if (i > 0)
                    {
                        res = true;
                    }
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }
        public void CreateJobOrderItem(string JobOrderNumber, short ItemId, Int64 ItemTypeId, string Description, int Unit, float UnitPrice, int Duration)
        {
            JobOrderItems JOItem = new JobOrderItems();

            JOItem.JobOrderNumber = JobOrderNumber;
            JOItem.ItemId = ItemId;
            JOItem.ItemTypeId = ItemTypeId;
            JOItem.ItemDescription = Description;
            JOItem.Unit = Unit;
            JOItem.UnitPrice = UnitPrice;
            JOItem.Duration = Duration;
            udbc.JobOrderItems.Add(JOItem);
            udbc.SaveChanges();
        }

        public bool GetJobOrderExist(string JobOrderNumber)
        {
            var joborder = udbc.JobOrder.Find(JobOrderNumber);
            if (joborder != null)
            {
                return true;
            }
            return false;

        }


        public string GetJobOrderNumber()
        {
            string year = Convert.ToString(DateTime.UtcNow.Year).Substring(2);
            string quarter = "Q1";
            int month = DateTime.UtcNow.Month;
            if (month > 3 && month <= 6)
            {
                quarter = "Q2";
            }
            else if (month > 6 && month <= 9)
            {
                quarter = "Q3";
            }
            else if (month > 9 && month <= 12)
            {
                quarter = "Q4";
            }

            string jobOrderId = "JO" + year + quarter + "000001";

            var jobOrders = from s in udbc.JobOrder.Where(s => s.JobOrderNumber.Substring(0, 6) == "JO" + year + quarter)
                            orderby s.JobOrderNumber descending
                            select s.JobOrderNumber;

            var jobOrder = jobOrders.FirstOrDefault();

            if (jobOrder != null)
            {
                string jobOrderPartialId = jobOrder.Substring(6);
                int lastVal = Convert.ToInt32(jobOrderPartialId);
                lastVal = lastVal + 1;
                string suffix = string.Empty;

                for (int i = Convert.ToString(lastVal).Length; i < 6; i++)
                {
                    suffix = suffix + "0";
                }

                jobOrderId = jobOrder.Substring(0, 6) + suffix + Convert.ToString(lastVal);
            }
            return jobOrderId;
        }


        public List<JobOrderViewModel> GetJobOrder()
        {

            var JOList = new List<JobOrderViewModel>();

            using (var db = new UserDBContext())
            {

                JOList = db.Database
                          .SqlQuery<JobOrderViewModel>("EXEC USP_TEMPGETORDER ").ToList();
            }

            return JOList;
        }

        public List<JobOrderViewModel> GetJobOrders(string UserId, bool IsClientView = false)
        {

            var JOList = new List<JobOrderViewModel>();

            using (var db = new UserDBContext())
            {

                JOList = db.Database
                          .SqlQuery<JobOrderViewModel>("EXEC USP_GETJOBORDERS @ClientId , @IsClientView ",
                           new SqlParameter("@ClientId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                           , new SqlParameter("@IsClientView", IsClientView)).ToList();
            }

            return JOList;
        }

        public JobOrderViewModel GetJobOrderDetails(string UserId, string JobOrderId, bool IsClientView = false)
        {

            var JobOrder = new JobOrderViewModel();

            using (var db = new UserDBContext())
            {

                JobOrder = db.Database
                          .SqlQuery<JobOrderViewModel>("EXEC USP_GETJOBORDERDETAILS @ClientId , @IsClientView , @JobOrderId ",
                           new SqlParameter("@ClientId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)
                           , new SqlParameter("@IsClientView", IsClientView)
                           , new SqlParameter("@JobOrderId", JobOrderId)).FirstOrDefault();
            }

            return JobOrder;
        }

        public bool UpdateJobStatus(string SubscriberId, string JobOrderNumber, string JobOrderStatus, DateTime UpdatedOn, string UpdatedBy, DateTime? CompletedOn)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                    var jobOrderNumber = new SqlParameter("@JobOrderNumber", string.IsNullOrEmpty(JobOrderNumber) ? DBNull.Value : (object)JobOrderNumber);
                    var jobOrderStatus = new SqlParameter("@JobOrderStatus", string.IsNullOrEmpty(JobOrderStatus) ? DBNull.Value : (object)JobOrderStatus);
                    var updatedon = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var completedOn = new SqlParameter("@CompletedOn", string.IsNullOrEmpty(CompletedOn.ToString()) ? DBNull.Value : (object)CompletedOn);

                    int i = context.Database.ExecuteSqlCommand("USP_UpdateJobStatus  @SubscriberId , @JobOrderNumber, @JobOrderStatus, @UpdatedOn, @UpdatedBy, @CompletedOn", subscriberId, jobOrderNumber, jobOrderStatus, updatedon, updatedBy, completedOn);
                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
            }

            return res;

        }

        public List<JobOrderItemsView> GetJobOrderItems(string JobOrderId)
        {

            var JobOrderItems = new List<JobOrderItemsView>();

            using (var db = new UserDBContext())
            {

                JobOrderItems = db.Database
                          .SqlQuery<JobOrderItemsView>("EXEC USP_GETJOBORDERITEMLIST @JobOrderId",
                          new SqlParameter("@JobOrderId", string.IsNullOrEmpty(JobOrderId) ? DBNull.Value : (object)JobOrderId)).ToList();
            }

            return JobOrderItems;
        }

        public List<InvoiceItemsView> GetInvoiceItems(string InvoiceNumber)
        {

            var InvoiceItems = new List<InvoiceItemsView>();

            using (var db = new UserDBContext())
            {

                InvoiceItems = db.Database
                          .SqlQuery<InvoiceItemsView>("EXEC USP_GETINVOICEITEMLIST @InvoiceNumber",
                          new SqlParameter("@InvoiceNumber", string.IsNullOrEmpty(InvoiceNumber) ? DBNull.Value : (object)InvoiceNumber)).ToList();
            }

            return InvoiceItems;
        }

        public List<ClientViewModel> GetSubscriberWiseClientList(string SubscriberId)
        {

            var Clients = new List<ClientViewModel>();

            using (var db = new UserDBContext())
            {

                Clients = db.Database
                          .SqlQuery<ClientViewModel>("EXEC USP_GetSubsciberWiseClientList @SubscriberId",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            return Clients;
        }

        public List<EmployeeViewModel> GetSubscriberWiseCoAdminList(string SubscriberId)
        {
            List<EmployeeViewModel> CoAdminList = new List<EmployeeViewModel>();

            using (var db = new UserDBContext())
            {
                CoAdminList = db.Database
                         .SqlQuery<EmployeeViewModel>("exec USP_GetSubsciberWiseEmployeeList @SubscriberId",
                           new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            return CoAdminList;
        }

        public string GetUserName(string UserId)
        {
            var corporateprofile = GetCorporateProfile(UserId).FirstOrDefault();
            if (corporateprofile != null)
            {
                return corporateprofile.Name;
            }
            else
            {
                EMSManager ems = new EMSManager();
                if (ems.GetEmployeeBasicDetails(UserId) != null)
                    return ems.GetEmployeeBasicDetails(UserId).FirstOrDefault().Name;
            }
            return string.Empty;
        }

        public GetJoborderStatusCountView GetJobOrderStatusCount(string ClientId)
        {

            var Count = new GetJoborderStatusCountView();

            using (var db = new UserDBContext())
            {
                Count = db.Database
                          .SqlQuery<GetJoborderStatusCountView>("EXEC USP_GetJobOrderStatusCount @ClientId",
                          new SqlParameter("@ClientId", string.IsNullOrEmpty(ClientId) ? DBNull.Value : (object)ClientId)
                          ).FirstOrDefault();
            }

            return Count;
        }

        public GetInvoiceStatusCountView GetInvoicetatusCount(string InvoiceTo)
        {

            var InvoiceCount = new GetInvoiceStatusCountView();

            using (var db = new UserDBContext())
            {
                InvoiceCount = db.Database
                          .SqlQuery<GetInvoiceStatusCountView>("EXEC USP_GetInvoiceStatusCount @InvoiceTo",
                          new SqlParameter("@InvoiceTo", string.IsNullOrEmpty(InvoiceTo) ? DBNull.Value : (object)InvoiceTo)
                          ).FirstOrDefault();
            }

            return InvoiceCount;
        }

        public GetTrainingStatusCountView GetTrainingstatusCount(string ClientId)
        {

            var TrainingCount = new GetTrainingStatusCountView();

            using (var db = new UserDBContext())
            {
                TrainingCount = db.Database
                          .SqlQuery<GetTrainingStatusCountView>("EXEC USP_GetTrainingStatusCount @ClientId",
                          new SqlParameter("@ClientId", string.IsNullOrEmpty(ClientId) ? DBNull.Value : (object)ClientId)
                          ).FirstOrDefault();
            }

            return TrainingCount;
        }
        public GetTaskCountView GetTaskCount(string AssignedTo)
        {

            var TaskCount = new GetTaskCountView();

            using (var db = new UserDBContext())
            {
                TaskCount = db.Database
                          .SqlQuery<GetTaskCountView>("EXEC USP_GetTaskCount @AssignedTo",
                          new SqlParameter("@AssignedTo", string.IsNullOrEmpty(AssignedTo) ? DBNull.Value : (object)AssignedTo)
                          ).FirstOrDefault();
            }

            return TaskCount;
        }

        public GetInvoiceStatusCountView GetEMPInvoicetatusCount(string CorporateId)
        {

            var EmpInvoiceCount = new GetInvoiceStatusCountView();

            using (var db = new UserDBContext())
            {
                EmpInvoiceCount = db.Database
                          .SqlQuery<GetInvoiceStatusCountView>("EXEC USP_GetEmpInvoiceStatusCount @CorporateId",
                          new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)
                          ).FirstOrDefault();
            }

            return EmpInvoiceCount;
        }
        public GetTrainingStatusCountView GetTrainingCount(string TrainerId)
        {

            var TrainingCount = new GetTrainingStatusCountView();

            using (var db = new UserDBContext())
            {
                TrainingCount = db.Database
                          .SqlQuery<GetTrainingStatusCountView>("EXEC USP_GetTrainingCount @TrainerId",
                          new SqlParameter("@TrainerId", string.IsNullOrEmpty(TrainerId) ? DBNull.Value : (object)TrainerId)
                          ).FirstOrDefault();
            }

            return TrainingCount;
        }

        public List<JOCommentsForumView> GetJOComments(string JobOrderNumber)
        {
            var JOComments = new List<JOCommentsForumView>();
            using (var db = new UserDBContext())
            {
                JOComments = db.Database.SqlQuery<JOCommentsForumView>("EXEC USP_GetJOComments @JobOrderNumber",
                    new SqlParameter("@JobOrderNumber", string.IsNullOrEmpty(JobOrderNumber) ? DBNull.Value : (object)JobOrderNumber)).ToList();
            }
            return JOComments;
        }

        public List<JOReplyForumView> GetJOReplies()
        {
            var JOReplies = new List<JOReplyForumView>();
            using (var db = new UserDBContext())
            {
                JOReplies = db.Database.SqlQuery<JOReplyForumView>("EXEC USP_GetJOReplies").ToList();
            }
            return JOReplies;
        }

        public bool AddJOComments(string JobOrderNumber, string Comment, DateTime CommentedOn, string UserId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var jobOrderNumber = new SqlParameter("@JobOrderNumber", string.IsNullOrEmpty(JobOrderNumber) ? DBNull.Value : (object)JobOrderNumber);
                    var comment = new SqlParameter("@Comment", string.IsNullOrEmpty(Comment) ? DBNull.Value : (object)Comment);
                    var commentedOn = new SqlParameter("@CommentedOn", CommentedOn);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddJOComments @JobOrderNumber, @Comment, @CommentedOn, @UserId", jobOrderNumber, comment, commentedOn, userId);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }

        public bool AddJOReplies(Int64 JOCommentId, string Reply, DateTime RepliedOn, string UserId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var joCommentId = new SqlParameter("@JOCommentId", JOCommentId);
                    var reply = new SqlParameter("@Reply", string.IsNullOrEmpty(Reply) ? DBNull.Value : (object)Reply);
                    var repliedOn = new SqlParameter("@RepliedOn", RepliedOn);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddJOReplies @JOCommentId, @Reply, @RepliedOn, @UserId", joCommentId, reply, repliedOn, userId);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }

        public JOCommentsCount GetJOCommentsCount(string JobOrderNumber)
        {

            var Count = new JOCommentsCount();

            using (var db = new UserDBContext())
            {
                Count = db.Database
                          .SqlQuery<JOCommentsCount>("EXEC USP_GetJOCommentsCount @JobOrderNumber ",
                          new SqlParameter("@JobOrderNumber", string.IsNullOrEmpty(JobOrderNumber) ? DBNull.Value : (object)JobOrderNumber)
                          ).FirstOrDefault();
            }

            return Count;
        }

        public void RemoveJOComment(Int64 JOCommentId)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var joCommentId = new SqlParameter("@JOCommentId", JOCommentId);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteJOComment @JOCommentId", joCommentId);
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
        }

        public void RemoveJOReply(Int64 JOReplyId)
        {
            try
            {
                using (var context = new UserDBContext())
                {
                    var joReplyId = new SqlParameter("@JOReplyId", JOReplyId);

                    int i = context.Database.ExecuteSqlCommand("USP_DeleteJOReply @JOReplyId", joReplyId);
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
        }

        public List<JobOrderAttachment> GetJobOrderAttachments(string JobOrderNumber)
        {
            var jobOrderAttachment = new List<JobOrderAttachment>();
            using (var db = new UserDBContext())
            {
                jobOrderAttachment = db.Database.SqlQuery<JobOrderAttachment>("EXEC USP_GetJobOrderAttachments @JobOrderNumber",
                    new SqlParameter("@JobOrderNumber", string.IsNullOrEmpty(JobOrderNumber) ? DBNull.Value : (object)JobOrderNumber)).ToList();
            }
            return jobOrderAttachment;
        }

        public bool UpdatePassword(string UserId, string newPassword)
        {
            var userProfile = udbc.UserProfile.Where(p => p.UserId == UserId).FirstOrDefault();
            if (userProfile != null)
            {
                userProfile.PCode = newPassword;
                udbc.Entry(userProfile).State = EntityState.Modified;
                udbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ActivateDeactivateUser(string UserId, string action, string Role)
        {
            if (Role == "Candidate")
            {
                var userProfile = udbc.UserProfile.Where(p => p.UserId == UserId).FirstOrDefault();
                if (userProfile != null)
                {
                    if (action == "activate")
                    {
                        userProfile.Deactivated = false;
                    }
                    if (action == "deactivate")
                    {
                        userProfile.Deactivated = true;
                    }
                    udbc.Entry(userProfile).State = EntityState.Modified;
                    udbc.SaveChanges();
                    return true;
                }
            }
            else if (Role == "Employee")
            {
                var empProfile = udbc.EmployeeBasicDetails.Where(p => p.UserId == UserId).FirstOrDefault();
                if (empProfile != null)
                {
                    if (action == "activate")
                    {
                        empProfile.Deactivated = false;
                    }
                    if (action == "deactivate")
                    {
                        empProfile.Deactivated = true;
                    }
                    udbc.Entry(empProfile).State = EntityState.Modified;
                    udbc.SaveChanges();
                    return true;
                }
            }
            else if (Role == "Corporate")
            {
                var corporateProfile = udbc.CorporateProfile.Where(p => p.CorporateId == UserId).FirstOrDefault();
                if (corporateProfile != null)
                {
                    if (action == "activate")
                    {
                        corporateProfile.Deactivated = false;
                    }
                    if (action == "deactivate")
                    {
                        corporateProfile.Deactivated = true;
                    }
                    udbc.Entry(corporateProfile).State = EntityState.Modified;
                    udbc.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public JobOrderViewModel GetJobOrderByJONumber(string JobOrderNumber)
        {
            var jobOrder = new JobOrderViewModel();
            using (var db = new UserDBContext())
            {
                jobOrder = db.Database.SqlQuery<JobOrderViewModel>("EXEC USP_GetJobOrderByJONumber @JobOrderNumber",
                    new SqlParameter("@JobOrderNumber", string.IsNullOrEmpty(JobOrderNumber) ? DBNull.Value : (object)JobOrderNumber)).FirstOrDefault();
            }
            return jobOrder;
        }

        public string AddCandidateLeads(long LeadId, string Name, string EmailId, string PhoneNumber, string UserId, Int16 Status, string Comments, DateTime UpdatedOn, string UpdatedBy, DateTime SubmittedOn, string SubmittedBy, string SubscriberId, string ReferenceId)
        {
            string res = "Failed:";

            try
            {
                using (var context = new UserDBContext())
                {
                    var leadId = new SqlParameter("@LeadId", LeadId);
                    var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
                    var emailId = new SqlParameter("@EmailId", string.IsNullOrEmpty(EmailId) ? DBNull.Value : (object)EmailId);
                    var phoneNumber = new SqlParameter("@PhoneNumber", string.IsNullOrEmpty(PhoneNumber) ? DBNull.Value : (object)PhoneNumber);
                    var userid = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var status = new SqlParameter("@Status", Status);
                    var comments = new SqlParameter("@Comments", string.IsNullOrEmpty(Comments) ? DBNull.Value : (object)Comments);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var submittedOn = new SqlParameter("@SubmittedOn", SubmittedOn);
                    var submittedBy = new SqlParameter("@SubmittedBy", string.IsNullOrEmpty(SubmittedBy) ? DBNull.Value : (object)SubmittedBy);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                    var referenceId = new SqlParameter("@ReferenceId", string.IsNullOrEmpty(ReferenceId) ? DBNull.Value : (object)ReferenceId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateLeads @LeadId, @UserId, @Name, @EmailId, @PhoneNumber, @Status, @Comments, @UpdatedOn, @UpdatedBy, @SubmittedOn, @SubmittedBy, @SubscriberId, @ReferenceId", leadId, userid, name, emailId, phoneNumber, status, comments, updatedOn, updatedBy, submittedOn, submittedBy, subscriberId, referenceId);

                    if (i == 1)
                        res = "Succeed:Data saved successfully";
                }
            }
            catch (Exception ex)
            {
                res = res + ex.ToString();
            }

            return res;
        }

        public List<CandidateLeadsListView> GetCandidateLeads(string SubmittedBy = null)
        {
            var leads = new List<CandidateLeadsListView>();
            using (var db = new UserDBContext())
            {
                leads = db.Database
                         .SqlQuery<CandidateLeadsListView>("exec USP_GetCandidateLeads @SubmittedBy",
                            new SqlParameter("@SubmittedBy", string.IsNullOrEmpty(SubmittedBy) ? DBNull.Value : (object)SubmittedBy)).ToList();
            }

            return leads;
        }

        public bool AddCandidateLeadsDetails(long LeadId, string UserId, string Name, string PhoneNumber, string EmailId, string FatherName, string FatherOccupation, string MotherName, string MotherOccupation,
                                                string IdName, string IdNumber, string Gender, DateTime? DOB, string Category, string Religion, bool DifferentlyAbled, string MaritalStatus, string MediumOfEducation,
                                                bool Relocate, string BelowPoverty, string FamilyIncome, string Qualification, Int16 Status, string Comments, DateTime UpdatedOn,
                                                string UpdatedBy, DateTime SubmittedOn, string SubmittedBy, string ReferenceId, string SubscriberId, Int16 RelocateId)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var leadId = new SqlParameter("@LeadId", LeadId);
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
                    var phoneNumber = new SqlParameter("@PhoneNumber", string.IsNullOrEmpty(PhoneNumber) ? DBNull.Value : (object)PhoneNumber);
                    var emailId = new SqlParameter("@EmailId", string.IsNullOrEmpty(EmailId) ? DBNull.Value : (object)EmailId);
                    var fatherName = new SqlParameter("@FatherName", string.IsNullOrEmpty(FatherName) ? DBNull.Value : (object)FatherName);
                    var fatherOccupation = new SqlParameter("@FatherOccupation", string.IsNullOrEmpty(FatherOccupation) ? DBNull.Value : (object)FatherOccupation);
                    var motherName = new SqlParameter("@MotherName", string.IsNullOrEmpty(MotherName) ? DBNull.Value : (object)MotherName);
                    var motherOccupation = new SqlParameter("@MotherOccupation", string.IsNullOrEmpty(MotherOccupation) ? DBNull.Value : (object)MotherOccupation);
                    var idName = new SqlParameter("@IdName", string.IsNullOrEmpty(IdName) ? DBNull.Value : (object)IdName);
                    var idNumber = new SqlParameter("@IdNumber", string.IsNullOrEmpty(IdNumber) ? DBNull.Value : (object)IdNumber);
                    var gender = new SqlParameter("@Gender", string.IsNullOrEmpty(Gender) ? DBNull.Value : (object)Gender);
                    var dOB = new SqlParameter("@DOB", string.IsNullOrEmpty(DOB.ToString()) ? DBNull.Value : (object)DOB);
                    var category = new SqlParameter("@Category", string.IsNullOrEmpty(Category) ? DBNull.Value : (object)Category);
                    var religion = new SqlParameter("@Religion", string.IsNullOrEmpty(Religion) ? DBNull.Value : (object)Religion);
                    var differentlyAbled = new SqlParameter("@DifferentlyAbled", DifferentlyAbled);
                    var maritalStatus = new SqlParameter("@MaritalStatus", string.IsNullOrEmpty(MaritalStatus) ? DBNull.Value : (object)MaritalStatus);
                    var mediumOfEducation = new SqlParameter("@MediumOfEducation", string.IsNullOrEmpty(MediumOfEducation) ? DBNull.Value : (object)MediumOfEducation);
                    var relocate = new SqlParameter("@Relocate", Relocate);
                    var belowPoverty = new SqlParameter("@BelowPoverty", string.IsNullOrEmpty(BelowPoverty) ? DBNull.Value : (object)BelowPoverty);
                    var familyIncome = new SqlParameter("@FamilyIncome", string.IsNullOrEmpty(FamilyIncome) ? DBNull.Value : (object)FamilyIncome);
                    var qualification = new SqlParameter("@Qualification", string.IsNullOrEmpty(Qualification) ? DBNull.Value : (object)Qualification);
                    var status = new SqlParameter("@Status", Status);
                    var comments = new SqlParameter("@Comments", string.IsNullOrEmpty(Comments) ? DBNull.Value : (object)Comments);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);
                    var submittedOn = new SqlParameter("@SubmittedOn", SubmittedOn);
                    var submittedBy = new SqlParameter("@SubmittedBy", string.IsNullOrEmpty(SubmittedBy) ? DBNull.Value : (object)SubmittedBy);
                    var referenceId = new SqlParameter("@ReferenceId", string.IsNullOrEmpty(ReferenceId) ? DBNull.Value : (object)ReferenceId);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                    var relocateId = new SqlParameter("@RelocateId", RelocateId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCandidateLeadsDetails @LeadId, @UserId, @Name, @PhoneNumber, @EmailId, @FatherName, @FatherOccupation, @MotherName, @MotherOccupation, @IdName, @IdNumber, @Gender, @DOB, @Category, @Religion, @DifferentlyAbled, @MaritalStatus, @MediumOfEducation, @Relocate, @BelowPoverty, @FamilyIncome, @Qualification,  @Status, @Comments, @UpdatedOn, @UpdatedBy, @SubmittedOn, @SubmittedBy,@ReferenceId, @SubscriberId,  @RelocateId  ",
                                                               leadId, userId, name, phoneNumber, emailId, fatherName, fatherOccupation, motherName, motherOccupation, idName, idNumber, gender, dOB, category, religion,
                                                               differentlyAbled, maritalStatus, mediumOfEducation, relocate, belowPoverty, familyIncome, qualification, status, comments, updatedOn, updatedBy, submittedOn, submittedBy, referenceId,
                                                               subscriberId, relocateId);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }


        public List<CandidateLeadsView> GetCandidateLeadsforSubscriber(string SubscriberId = null)
        {
            var leads = new List<CandidateLeadsView>();
            using (var db = new UserDBContext())
            {
                leads = db.Database
                         .SqlQuery<CandidateLeadsView>("exec USP_GetCandidateLeadsForSubscriber @SubscriberId",
                            new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            return leads;
        }

        public CandidateLeadsView GetCandidateLeadsToApprove(Int64 LeadId)
        {
            var leads = new CandidateLeadsView();
            using (var db = new UserDBContext())
            {
                leads = db.Database
                         .SqlQuery<CandidateLeadsView>("exec USP_GetCandidateLeadstoApprove @LeadId",
                            new SqlParameter("@LeadId", LeadId)).FirstOrDefault();
            }

            return leads;
        }

        public void DeleteCandidateLeads(Int64 LeadId)
        {
            var lead = udbc.CandidateLeads.Find(LeadId);
            if (lead != null)
            {
                udbc.CandidateLeads.Remove(lead);
                udbc.SaveChanges();
            }
        }

        public string uploadLogo(string CorporateId, HttpPostedFileBase upload)
        {
            string res = "Failure";
            if (upload != null && upload.ContentLength > 0)
            {

                var file = udbc.AdminLogoFile.Where(d => d.CorporateId == CorporateId).FirstOrDefault();

                if (file != null)
                {
                    Int64 FileId = file.FileId;

                    blobManager.DeleteBlob(CorporateId.ToLower(), GetFileNameCompanyLogo(FileId).ToLower());
                    udbc.AdminLogoFile.Remove(file);
                    udbc.SaveChanges();
                }


                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");


                bool added = AddCompanyLogo(CorporateId, imgFileName.ToLower(), upload.ContentType);

                if (added)
                {
                    blobManager.UploadBlob(CorporateId, ReplaceFileNameCompanyLogo(CorporateId), upload);

                }

                res = "Succeed";
            }


            return res;
        }


        public string uploadJobOrderFile(string JobOrderNumber, HttpPostedFileBase upload)
        {
            string res = "Failure";
            if (upload != null && upload.ContentLength > 0)
            {

                var file = udbc.JobOrderFinalAttachment.Where(d => d.JobOrderNumber == JobOrderNumber).FirstOrDefault();

                if (file != null)
                {
                    Int64 FileId = file.FileId;

                    blobManager.DeleteBlob(JobOrderNumber.ToLower(), GetJobOrderFileName(FileId).ToLower());
                    udbc.JobOrderFinalAttachment.Remove(file);
                    udbc.SaveChanges();
                }


                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");


                bool added = AddJobOrdergFinalAttachmentToDB(JobOrderNumber, imgFileName.ToLower(), upload.ContentType);

                if (added)
                {
                    blobManager.UploadBlob(JobOrderNumber.ToLower(), ReplaceFileNameFinalJobOrder(JobOrderNumber).ToLower(), upload);
                }

                res = "Succeed";
            }


            return res;
        }

        public bool AddJobOrdergFinalAttachmentToDB(string JobOrderNumber, string FileName, string ContentType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var jobOrderNumber = new SqlParameter("@JobOrderNumber", string.IsNullOrEmpty(JobOrderNumber) ? DBNull.Value : (object)JobOrderNumber);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);

                    int i = context.Database.ExecuteSqlCommand("USP_AddJobOrderFinalAttachment @JobOrderNumber, @FileName, @ContentType",
                        jobOrderNumber, fileName, contentType);

                    if (i == 1)
                        res = true;
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }

        public List<JobOrderFinalAttachment> GetJobOrderFinalAttachments(string JobOrderNumber)
        {
            var Attachment = new List<JobOrderFinalAttachment>();
            try
            {
                using (var context = new UserDBContext())
                {
                    Attachment = udbc.Database
                            .SqlQuery<JobOrderFinalAttachment>("EXEC USP_GetJobOrderFinalAttachments @JobOrderNumber",
                             new SqlParameter("@JobOrderNumber", string.IsNullOrEmpty(JobOrderNumber) ? DBNull.Value : (object)JobOrderNumber)).ToList();
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Attachment;
        }

        public string GetJobOrderFileName(Int64 FileId)
        {
            string fileName = null;
            var image = udbc.JobOrderFinalAttachment.Find(FileId);

            if (image != null)
            {
                fileName = "finalattachment/" + image.FileId + "/" + image.FileName;
            }
            return fileName;
        }

        public string ReplaceFileNameFinalJobOrder(string JobOrderNumber)
        {
            string fileName = null;
            var image = udbc.JobOrderFinalAttachment.Where(f => f.JobOrderNumber == JobOrderNumber);

            if (image != null)
            {
                if (image.Count() > 0)
                {
                    string imgFileName = image.FirstOrDefault().FileName;
                    imgFileName = imgFileName.Replace(' ', '_');
                    imgFileName = imgFileName.Replace("'", "_");


                    fileName = "finalattachment/" + image.FirstOrDefault().FileId + "/" + imgFileName;
                }
            }
            return fileName;
        }

        //Adding Team Meber
        //by Ajay Kumar Choudhary

        public bool AddTeamMember(string MemberId, string CorporateId, string SubscriberId, string Name, string EmailId, string PhoneNumber, Int16 EmpRoleId, string Designation, DateTime UpdatedOn, string UpdatedBy)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var memberId = new SqlParameter("@MemberId", string.IsNullOrEmpty(MemberId) ? DBNull.Value : (object)MemberId);
                    var corporateId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId);
                    var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                    var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
                    var emailId = new SqlParameter("@EmailId", string.IsNullOrEmpty(EmailId) ? DBNull.Value : (object)EmailId);
                    var phoneNumber = new SqlParameter("@PhoneNumber", string.IsNullOrEmpty(PhoneNumber) ? DBNull.Value : (object)PhoneNumber);
                    var empRoleId = new SqlParameter("@EmpRoleId", EmpRoleId);
                    var designation = new SqlParameter("@Designation", string.IsNullOrEmpty(Designation) ? DBNull.Value : (object)Designation);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);

                    int i = context.Database.ExecuteSqlCommand("USP_AddTeamMember @MemberId, @CorporateId, @SubscriberId, @Name, @EmailId, @PhoneNumber, @EmpRoleId, @Designation, @UpdatedOn, @UpdatedBy",
                                                                                    memberId, corporateId, subscriberId, name, emailId, phoneNumber, empRoleId, designation, updatedOn, updatedBy);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }


        public List<ClientTeamMemberProfileView> GetTeamMember(string CorporateId, string MemberId = null)
        {
            var team = new List<ClientTeamMemberProfileView>();
            using (var db = new UserDBContext())
            {
                team = db.Database
                         .SqlQuery<ClientTeamMemberProfileView>("exec USP_GetClientTeamMember @CorporateId, @MemberId",
                            new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId),
                            new SqlParameter("@MemberId", string.IsNullOrEmpty(MemberId) ? DBNull.Value : (object)MemberId)).ToList();
            }

            return team;
        }


        public bool AddTeamMemberRights(string UserId, Int16 EmpRightsId, DateTime UpdatedOn, string UpdatedBy)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                    var empRightsId = new SqlParameter("@EmpRightsId", EmpRightsId);
                    var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                    var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);

                    int i = context.Database.ExecuteSqlCommand("USP_AddTeamMemberRights @UserId, @EmpRightsId, @UpdatedOn, @UpdatedBy",
                                                                                    userId, empRightsId, updatedOn, updatedBy);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }

        public List<ClientTeamMemberRights> GetTeamMemberRights(string MemberId)
        {

            var rights = (from s in udbc.ClientTeamMemberRights.Where(s => s.UserId == MemberId)
                          select s).ToList();
            return rights;
        }

        public bool result;
        public bool GetCorporateExist(string CorporateId)
        {
            var corporate = from s in udbc.CorporateProfile.Where(s => s.CorporateId == CorporateId)
                            select s;
            if (corporate.Count() > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }


        //by Ajay Kumar Choudhary
        //on 21-07-2017
        //For Add Channel Partner
        public bool AddChannelPartner(string UserId, string Name, DateTime RegisteredOn, string ModuleId, string DepartmentId, string RoleId, string ReferenceId, string SubscriberId, DateTime UpdatedOn, string UpdatedBy)
        {
            using (var context = new UserDBContext())
            {
                var userId = new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId);
                var name = new SqlParameter("@Name", string.IsNullOrEmpty(Name) ? DBNull.Value : (object)Name);
                var registeredOn = new SqlParameter("@RegisteredOn", RegisteredOn);
                var moduleId = new SqlParameter("@ModuleId", string.IsNullOrEmpty(ModuleId) ? DBNull.Value : (object)ModuleId);
                var departmentId = new SqlParameter("@DepartmentId", string.IsNullOrEmpty(DepartmentId) ? DBNull.Value : (object)DepartmentId);
                var roleId = new SqlParameter("@RoleId", string.IsNullOrEmpty(RoleId) ? DBNull.Value : (object)RoleId);
                var referenceId = new SqlParameter("@ReferenceId", string.IsNullOrEmpty(ReferenceId) ? DBNull.Value : (object)ReferenceId);
                var subscriberId = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);
                var updatedOn = new SqlParameter("@UpdatedOn", UpdatedOn);
                var updatedBy = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(UpdatedBy) ? DBNull.Value : (object)UpdatedBy);

                int i = context.Database.ExecuteSqlCommand("USP_AddChannelPartner @UserId, @Name, @RegisteredOn, @ModuleId, @DepartmentId, @RoleId, @ReferenceId, @SubscriberId, @UpdatedOn, @UpdatedBy", userId, name, registeredOn, moduleId, departmentId, roleId, referenceId, subscriberId, updatedOn, updatedBy);

                if (i > 0)
                    return true;
                else
                    return false;
            }
        }

        //public List<ClientTeamRights> GetTeamMemberRights(string MemberId)
        //{
        //    var rights = new List<ClientTeamRights>();
        //    using (var db = new UserDBContext())
        //    {
        //        rights = db.Database
        //                 .SqlQuery<ClientTeamRights>("exec USP_GetMemberRights @MemberId",
        //                    new SqlParameter("@MemberId", string.IsNullOrEmpty(MemberId) ? DBNull.Value : (object)MemberId)).ToList();
        //    }

        //    return rights;
        //}

        public string GetFileTourDetail(Int64 FileId = 0)
        {
            string fileName = null;
            var image = udbc.TourAttachment.Find(FileId);

            if (image != null)
            {
                string imgFileName = image.FileName;
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");
                fileName = "tourattachment/" + image.FileId + "/" + imgFileName;
            }
            return fileName;
        }

        public CompanyDetailsView GetCompanyDetails(string CorporateId)
        {
            var details = new CompanyDetailsView();
            using (var db = new UserDBContext())
            {
                details = db.Database.SqlQuery<CompanyDetailsView>("EXEC USP_GetCompanyDetails @CorporateId",
                    new SqlParameter("@CorporateId", string.IsNullOrEmpty(CorporateId) ? DBNull.Value : (object)CorporateId)).FirstOrDefault();
            }
            return details;
        }
        //methods for ADD InstructorLeadProfile 
        public bool AddInstructorLeadProfile(InstructorLeadProfileView instructor, HttpPostedFileBase upload)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {
                    var InstructorId = new SqlParameter("@InstructorId", string.IsNullOrEmpty(instructor.InstructorId) ? DBNull.Value : (object)instructor.InstructorId);
                    var name = new SqlParameter("@Name", string.IsNullOrEmpty(instructor.Name) ? DBNull.Value : (object)instructor.Name);
                    var countryid = new SqlParameter("@CountryId", instructor.CountryId);
                    var stateid = new SqlParameter("@StateId", instructor.StateId);
                    var cityid = new SqlParameter("@CityId", instructor.CityId);
                    var zone = new SqlParameter("@Zone", string.IsNullOrEmpty(instructor.Zone) ? DBNull.Value : (object)instructor.Zone);
                    var qualification = new SqlParameter("@Qualification", string.IsNullOrEmpty(instructor.Qualification) ? DBNull.Value : (object)instructor.Qualification);
                    var experience = new SqlParameter("@Experience", instructor.Experience);
                    var domain = new SqlParameter("@DomainExpertize", string.IsNullOrEmpty(instructor.DomainExpertize) ? DBNull.Value : (object)instructor.DomainExpertize);
                    var organization = new SqlParameter("@Organization", string.IsNullOrEmpty(instructor.Organization) ? DBNull.Value : (object)instructor.Organization);
                    var language = new SqlParameter("@LanguageKnown", string.IsNullOrEmpty(instructor.LanguageKnown) ? DBNull.Value : (object)instructor.LanguageKnown);
                    var specilization = new SqlParameter("@Specialization", string.IsNullOrEmpty(instructor.Specialization) ? DBNull.Value : (object)instructor.Specialization);
                    var nibfproject = new SqlParameter("@NibfProject", string.IsNullOrEmpty(instructor.NibfProject) ? DBNull.Value : (object)instructor.NibfProject);
                    var traininglocation = new SqlParameter("@TrainingLocation", string.IsNullOrEmpty(instructor.TrainingLocation) ? DBNull.Value : (object)instructor.TrainingLocation);
                    var remarks = new SqlParameter("@Remarks", instructor.Remarks);
                    var subscriberid = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(instructor.SubscriberId) ? DBNull.Value : (object)instructor.SubscriberId);
                    var empanelled = new SqlParameter("@Empanelled", instructor.Empanelled);
                    var updatedon = new SqlParameter("@UpdatedOn", DateTime.Now);
                    var updatedby = new SqlParameter("@UpdatedBy", string.IsNullOrEmpty(instructor.UpdatedBy) ? DBNull.Value : (object)instructor.UpdatedBy);
                    var gender = new SqlParameter("@Gender", string.IsNullOrEmpty(instructor.Gender) ? DBNull.Value : (object)instructor.Gender);

                    var dob = new SqlParameter("@Dob", instructor.DOB);
                    if (instructor.DOB == null)
                        dob = new SqlParameter("@Dob", DBNull.Value);

                    var readytorelocate = new SqlParameter("@ReadyToReallocate", instructor.ReadyToReallocate);


                    int i = context.Database.ExecuteSqlCommand("USP_ADDInstructorLeadProfile @InstructorId, @Name, @CountryId, @StateId, @CityId, @Zone, @Qualification, @Experience, @DomainExpertize, @Organization, @LanguageKnown, @Specialization, @NibfProject, @TrainingLocation, @Remarks, @SubscriberId, @Empanelled, @UpdatedOn, @UpdatedBy,@Gender,@Dob,@ReadyToReallocate",
                                                               InstructorId, name, countryid, stateid, cityid, zone, qualification, experience
                                                               , domain, organization, language, specilization, nibfproject, traininglocation, remarks,
                                                               subscriberid, empanelled, updatedon, updatedby, gender, dob, readytorelocate
                                                             );

                    if (i == 1)
                        res = true;
                    if (res == true)
                    {
                        if (upload != null)
                        {
                            uploadInstructorAttachment(instructor.InstructorId, upload, instructor.SubscriberId);
                        }

                    }
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }
        public string uploadInstructorAttachment(string InstructorId, HttpPostedFileBase upload, string SubscriberId)
        {
            string res = "Failure";
            if (upload != null && upload.ContentLength > 0)
            {
                Int64 FileId = 0;
                var file = udbc.InstructorAttachment.Where(d => d.InstructorId == InstructorId).FirstOrDefault();

                if (file != null)
                {
                    FileId = file.FileId;

                    blobManager.DeleteBlob(SubscriberId.ToLower(), GetInstructorFileName(FileId).ToLower());
                    udbc.InstructorAttachment.Remove(file);
                    udbc.SaveChanges();
                }


                string imgFileName = System.IO.Path.GetFileName(upload.FileName);
                imgFileName = imgFileName.Replace(' ', '_');
                imgFileName = imgFileName.Replace("'", "_");


                bool added = AddInstructorAttachmentToDB(InstructorId, imgFileName.ToLower(), upload.ContentType);

                if (added)
                {
                    FileId = udbc.InstructorAttachment.Where(i => i.InstructorId == InstructorId).Select(i => i.FileId).FirstOrDefault();
                    blobManager.UploadBlob(SubscriberId.ToLower(), GetInstructorFileName(FileId).ToLower(), upload);
                }

                res = "Succeed";
            }


            return res;
        }
        public bool AddInstructorAttachmentToDB(string InstructorId, string FileName, string ContentType)
        {
            bool res = false;

            try
            {

                using (var context = new UserDBContext())
                {
                    var instructorid = new SqlParameter("@InstructorId", string.IsNullOrEmpty(InstructorId) ? DBNull.Value : (object)InstructorId);
                    var fileName = new SqlParameter("@FileName", string.IsNullOrEmpty(FileName) ? DBNull.Value : (object)FileName);
                    var contentType = new SqlParameter("@ContentType", string.IsNullOrEmpty(ContentType) ? DBNull.Value : (object)ContentType);

                    int i = context.Database.ExecuteSqlCommand("USP_AddInstructorAttachment @InstructorId, @FileName, @ContentType",
                        instructorid, fileName, contentType);

                    if (i == 1)
                        res = true;
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }
        public string GetInstructorFileName(Int64 FileId)
        {
            string fileName = null;
            var image = udbc.InstructorAttachment.Find(FileId);

            if (image != null)
            {
                fileName = "Instructor/" + image.FileId;
            }
            return fileName;
        }
        public List<InstructorLeadProfileView> GetInstructerLead(string SubscriberId, string searchkey = null)
        {
            var team = new List<InstructorLeadProfileView>();
            using (var db = new UserDBContext())
            {
                team = db.Database
                         .SqlQuery<InstructorLeadProfileView>("exec USP_GetInstructorLead @SubscriberId ,@Searchkey",
                            new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId),
                            new SqlParameter("@Searchkey", string.IsNullOrEmpty(searchkey) ? DBNull.Value : (object)searchkey)).ToList();
            }

            return team;
        }

        public bool AddQualification(string Qualification)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var qualification = new SqlParameter("@Qualification", string.IsNullOrEmpty(Qualification) ? DBNull.Value : (object)Qualification);
                    int i = context.Database.ExecuteSqlCommand("dbo.USP_AddQualification @Qualification",
                                                                                    qualification);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }
        //domain
        public bool AddDomain(string Domain)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var domain = new SqlParameter("@Domain", string.IsNullOrEmpty(Domain) ? DBNull.Value : (object)Domain);
                    int i = context.Database.ExecuteSqlCommand("dbo.USP_AddDomain @Domain",
                                                                                    domain);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }
        //Addspecialization
        public bool Addspecialization(string Specialization)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var specialization = new SqlParameter("@Specialization", string.IsNullOrEmpty(Specialization) ? DBNull.Value : (object)Specialization);
                    int i = context.Database.ExecuteSqlCommand("USP_AddSpecialization @Specialization",
                                                                                    specialization);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }
        //Addproject
        public bool Addproject(string Project)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var project = new SqlParameter("@Project", string.IsNullOrEmpty(Project) ? DBNull.Value : (object)Project);
                    int i = context.Database.ExecuteSqlCommand("dbo.USP_AddProjects @Project",
                                                                                    project);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }
        //addorganization
        public bool AddOrganization(string Organization)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {
                    var organization = new SqlParameter("@Organization", string.IsNullOrEmpty(Organization) ? DBNull.Value : (object)Organization);
                    int i = context.Database.ExecuteSqlCommand("USP_AddOrganization @Organization",
                                                                                    organization);

                    if (i == 1)
                        res = true;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return res;
        }

        public bool AddCorporateBranch(string BranchName, string SubscriberId, Int64 BranchId = 0)
        {
            bool res = false;

            try
            {
                using (var context = new UserDBContext())
                {

                    var branchId = new SqlParameter("@BranchId", BranchId);
                    var branchName = new SqlParameter("@BranchName", string.IsNullOrEmpty(BranchName) ? DBNull.Value : (object)BranchName);
                    var subscriberId = new SqlParameter("@CorporateId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);

                    int i = context.Database.ExecuteSqlCommand("USP_AddCorporateBranch @BranchId, @BranchName, @CorporateId",
                        branchId, branchName, subscriberId);

                    if (i > 0)
                    {
                        res = true;
                    }
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }

        public UserIdByName GetUserIdByReckonnId(string ReckonnId)
        {
            var Userid = new UserIdByName();
            using (var db = new UserDBContext())
            {
                Userid = db.Database
                         .SqlQuery<UserIdByName>("exec USP_GetUserIdUserNameWise @UserName",
                            new SqlParameter("@UserName", string.IsNullOrEmpty(ReckonnId) ? DBNull.Value : (object)ReckonnId)).FirstOrDefault();
            }

            return Userid;
        }

        public GetMTTrainingOrderCount GetMTTrainingOrderDashboard()
        {
            var Counts = new GetMTTrainingOrderCount();
            using (var db = new UserDBContext())
            {
                Counts = db.Database
                         .SqlQuery<GetMTTrainingOrderCount>("exec USP_IPPBMTDetails").FirstOrDefault();
            }

            return Counts;
        }

        public GetMTTrainingOrderAttendedExport GetMTTrainingOrderAttendedExport()
        {
            var Counts = new GetMTTrainingOrderAttendedExport();
            using (var db = new UserDBContext())
            {
                Counts = db.Database
                         .SqlQuery<GetMTTrainingOrderAttendedExport>("exec USP_MTTraineeAttended").FirstOrDefault();
            }

            return Counts;
        }

    }
}