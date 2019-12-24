using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.WebPages.Html;
using AJSolutions.DAL;
using Quartz;
using Quartz.Impl;

namespace AJSolutions.DAL
{

    public static class Global
    {
        public static string result = String.Empty;

        public static string WebsiteUrl()
        {
            switch (ConfigurationManager.AppSettings["Environment"].ToString().ToLower())
            {

                case "local":
                    result = "https://localhost:55689/";
                    break;
                case "development":
                    result = "http://13.71.94.84";
                    break;
                //case "production":
                //    result = "https://www.Blink.com/";
                //    break;
                default:
                    result = "https://localhost:55689/";
                    break;
            }

            return result;
        }

        public static Int64 basicHead = 0;
        public static Int64 BasicHeadId()
        {
            switch (ConfigurationManager.AppSettings["Environment"].ToString().ToLower())
            {
                case "local":
                    basicHead = 15;
                    break;
                case "development":
                    basicHead = 15;
                    break;
                case "production":
                    basicHead = 3; ;
                    break;
                default:
                    basicHead = 3; ;
                    break;
            }

            return basicHead;
        }

        public static Int64 SpecialAllowancesHeadId()
        {
            switch (ConfigurationManager.AppSettings["Environment"].ToString().ToLower())
            {

                case "local":
                    basicHead = 25;
                    break;
                case "development":
                    basicHead = 25;
                    break;
                case "production":
                    basicHead = 5;
                    break;
                default:
                    basicHead = 5;
                    break;
            }

            return basicHead;
        }

        public static string month;
        public static string Month(short M)
        {
            switch (M)
            {
                case 1:
                    month = "January";
                    break;
                case 2:
                    month = "Febuary";
                    break;
                case 3:
                    month = "March";
                    break;
                case 4:
                    month = "April";
                    break;
                case 5:
                    month = "May";
                    break;
                case 6:
                    month = "June";
                    break;
                case 7:
                    month = "July";
                    break;
                case 8:
                    month = "August";
                    break;
                case 9:
                    month = "September";
                    break;
                case 10:
                    month = "October";
                    break;
                case 11:
                    month = "November";
                    break;
                case 12:
                    month = "December";
                    break;

            }
            return month;

        }

        public static bool IsStatusReportAccess(string SubscriberId)
        {
            if (ConfigurationManager.AppSettings["SubscriberId"].ToString().Contains(SubscriberId))
                return true;
            else return false;
        }
        //public static bool IsIPPBStatusReportAccess(string SubscriberId)
        //{
        //    if (ConfigurationManager.AppSettings["IPPBSubscriberId"].ToString().Contains(SubscriberId))
        //        return true;
        //    else return false;
        //}


        public class MStatus
        {
            public string MaritalStatus { get; set; }
        }
        //Summary
        //To get all list of MaritalStatus   
        public static List<MStatus> GetMaritalList()
        {
            List<MStatus> MaritalList = new List<MStatus>();
            MaritalList.Add(new MStatus { MaritalStatus = "Relationship" });
            MaritalList.Add(new MStatus { MaritalStatus = "Single" });
            MaritalList.Add(new MStatus { MaritalStatus = "Married" });
            MaritalList.Add(new MStatus { MaritalStatus = "Engaged" });
            MaritalList.Add(new MStatus { MaritalStatus = "I dont want to say" });
            MaritalList.Add(new MStatus { MaritalStatus = "Widowed" });

            return MaritalList;
        }

        public class CVisibility
        {
            public int ContentVisiblity { get; set; }
            public string ContentVisiblityName { get; set; }
        }

        public static List<CVisibility> GetContentVisiblity()
        {
            List<CVisibility> ContentVisiblityList = new List<CVisibility>();
            ContentVisiblityList.Add(new CVisibility { ContentVisiblity = 1, ContentVisiblityName = "Private" });
            ContentVisiblityList.Add(new CVisibility { ContentVisiblity = 2, ContentVisiblityName = "Public" });
            ContentVisiblityList.Add(new CVisibility { ContentVisiblity = 3, ContentVisiblityName = "Hidden" });

            return ContentVisiblityList;
        }

        public class CAvailability
        {
            public bool ContentAvailability { get; set; }
            public string AvaibilityName { get; set; }

        }
        public static List<CAvailability> GetContentAvailability()
        {
            List<CAvailability> ContentAvailabilityList = new List<CAvailability>();

            ContentAvailabilityList.Add(new CAvailability { ContentAvailability = true, AvaibilityName = "Yes" });
            ContentAvailabilityList.Add(new CAvailability { ContentAvailability = false, AvaibilityName = "No" });
            return ContentAvailabilityList;
        }

        public class Salary
        {
            public string SalaryRange { get; set; }
        }

        public static List<Salary> GetSalaryRange()
        {
            List<Salary> SalaryRangeList = new List<Salary>();
            SalaryRangeList.Add(new Salary { SalaryRange = "Below 50K" });
            SalaryRangeList.Add(new Salary { SalaryRange = "50K - 1" });
            SalaryRangeList.Add(new Salary { SalaryRange = "1 - 2" });
            SalaryRangeList.Add(new Salary { SalaryRange = "2 - 5" });
            SalaryRangeList.Add(new Salary { SalaryRange = "5  - 10" });
            SalaryRangeList.Add(new Salary { SalaryRange = "10 - 20" });
            SalaryRangeList.Add(new Salary { SalaryRange = "20 - 30" });
            SalaryRangeList.Add(new Salary { SalaryRange = "30 - 40" });
            SalaryRangeList.Add(new Salary { SalaryRange = "40 - 50" });

            return SalaryRangeList;
        }

        public class Status
        {
            public string StatusType { get; set; }
        }

        public static List<Status> GetStatusList()
        {
            List<Status> StatusList = new List<Status>();
            StatusList.Add(new Status { StatusType = "Any" });
            StatusList.Add(new Status { StatusType = "Unassigned" });
            StatusList.Add(new Status { StatusType = "Assigned" });
            StatusList.Add(new Status { StatusType = "Inprogress" });
            StatusList.Add(new Status { StatusType = "Completed" });
            StatusList.Add(new Status { StatusType = "Rejected" });

            return StatusList;
        }

        public class Experience
        {
            public string ExpRange { get; set; }
        }

        public static List<Experience> GetExperienceRange()
        {
            List<Experience> ExperienceRangeList = new List<Experience>();
            ExperienceRangeList.Add(new Experience { ExpRange = "Fresher" });
            ExperienceRangeList.Add(new Experience { ExpRange = "1 - 2" });
            ExperienceRangeList.Add(new Experience { ExpRange = "2 - 3" });
            ExperienceRangeList.Add(new Experience { ExpRange = "3 - 4" });
            ExperienceRangeList.Add(new Experience { ExpRange = "4 - 5" });
            ExperienceRangeList.Add(new Experience { ExpRange = "5 - 6" });
            ExperienceRangeList.Add(new Experience { ExpRange = "6 - 7" });
            ExperienceRangeList.Add(new Experience { ExpRange = "7 - 8" });
            ExperienceRangeList.Add(new Experience { ExpRange = "8 - 9" });
            ExperienceRangeList.Add(new Experience { ExpRange = "9 - 10" });
            ExperienceRangeList.Add(new Experience { ExpRange = "10 - 11" });
            ExperienceRangeList.Add(new Experience { ExpRange = "11 - 12" });
            ExperienceRangeList.Add(new Experience { ExpRange = "12 - 13" });
            ExperienceRangeList.Add(new Experience { ExpRange = "13 - 14" });
            ExperienceRangeList.Add(new Experience { ExpRange = "14 - 15" });
            ExperienceRangeList.Add(new Experience { ExpRange = "15 - 16" });
            ExperienceRangeList.Add(new Experience { ExpRange = "16 - 17" });
            ExperienceRangeList.Add(new Experience { ExpRange = "17 - 18" });
            ExperienceRangeList.Add(new Experience { ExpRange = "18 - 19" });
            ExperienceRangeList.Add(new Experience { ExpRange = "19 - 20" });
            ExperienceRangeList.Add(new Experience { ExpRange = "20 - 21" });
            ExperienceRangeList.Add(new Experience { ExpRange = "21 - 22" });
            ExperienceRangeList.Add(new Experience { ExpRange = "22 - 23" });
            ExperienceRangeList.Add(new Experience { ExpRange = "23 - 24" });
            ExperienceRangeList.Add(new Experience { ExpRange = "24 - 25" });
            ExperienceRangeList.Add(new Experience { ExpRange = "25 - 26" });
            ExperienceRangeList.Add(new Experience { ExpRange = "26 - 27" });
            ExperienceRangeList.Add(new Experience { ExpRange = "27 - 28" });
            ExperienceRangeList.Add(new Experience { ExpRange = "28 - 29" });
            ExperienceRangeList.Add(new Experience { ExpRange = "29 - 30" });

            return ExperienceRangeList;
        }

        public class DurationList
        {
            public int Duration { get; set; }
            public string DurationName { get; set; }
        }

        public static List<DurationList> GetDuration()
        {
            List<DurationList> DurationNameList = new List<DurationList>();
            for (int i = 1; i <= 365; i++)
            {
                DurationNameList.Add(new DurationList { Duration = i, DurationName = i.ToString() });
            }

            return DurationNameList;
        }


        //Summary
        //To get all list of Address Type   
        public class AddressType
        {
            public string addresstypeid { get; set; }
            public string addresstype { get; set; }
        }

        public static List<AddressType> GetAddressType()
        {
            List<AddressType> AddressTypeList = new List<AddressType>();
            AddressTypeList.Add(new AddressType { addresstypeid = "PR", addresstype = "Present" });
            AddressTypeList.Add(new AddressType { addresstypeid = "PE", addresstype = "Permanent" });
            AddressTypeList.Add(new AddressType { addresstypeid = "CO", addresstype = "Correspondence" });
            AddressTypeList.Add(new AddressType { addresstypeid = "CM", addresstype = "Company" });

            return AddressTypeList;
        }

        //Summary
        //To get all list of Gender  

        public class GenderStatus
        {
            public string Genderid { get; set; }
            public string Gender { get; set; }
        }

        public static List<GenderStatus> GetGenderList()
        {
            List<GenderStatus> GenderList = new List<GenderStatus>();
            GenderList.Add(new GenderStatus { Genderid = "MA", Gender = "Male" });
            GenderList.Add(new GenderStatus { Genderid = "FE", Gender = "Female" });
            GenderList.Add(new GenderStatus { Genderid = "TR", Gender = "Transgender" });

            return GenderList;
        }

        public class BloodG
        {
            public string BloodGroup { get; set; }
        }
        //Summary
        //To get all list of BloodGroup   
        public static List<BloodG> GetBloodGroupList()
        {
            List<BloodG> BloodGroupList = new List<BloodG>();
            BloodGroupList.Add(new BloodG { BloodGroup = "A+" });
            BloodGroupList.Add(new BloodG { BloodGroup = "A-" });
            BloodGroupList.Add(new BloodG { BloodGroup = "B+" });
            BloodGroupList.Add(new BloodG { BloodGroup = "B-" });
            BloodGroupList.Add(new BloodG { BloodGroup = "O+" });
            BloodGroupList.Add(new BloodG { BloodGroup = "O-" });
            BloodGroupList.Add(new BloodG { BloodGroup = "AB+" });
            BloodGroupList.Add(new BloodG { BloodGroup = "AB-" });

            return BloodGroupList;
        }

        public class FrequencyType
        {
            public string frequencytype { get; set; }
        }
        public static List<FrequencyType> GetFrequencyTypeList()
        {
            List<FrequencyType> FrequencyTypeList = new List<FrequencyType>();
            FrequencyTypeList.Add(new FrequencyType { frequencytype = "Daily" });
            FrequencyTypeList.Add(new FrequencyType { frequencytype = "Weekly" });
            FrequencyTypeList.Add(new FrequencyType { frequencytype = "Fortnight" });
            FrequencyTypeList.Add(new FrequencyType { frequencytype = "Monthly" });
            FrequencyTypeList.Add(new FrequencyType { frequencytype = "Task based" });
            FrequencyTypeList.Add(new FrequencyType { frequencytype = "Not Required" });
            return FrequencyTypeList;
        }

        public class TaskStatusType
        {
            public short TaskStatus { get; set; }
            public string taskStatusName { get; set; }
        }
        public static List<TaskStatusType> GetTaskStatusList()
        {
            List<TaskStatusType> TaskStatusList = new List<TaskStatusType>();
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 0, taskStatusName = "Waiting for approval" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 1, taskStatusName = "Unaccepted" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 2, taskStatusName = "Assigned" });
            //TaskStatusList.Add(new TaskStatusType { TaskStatus = 3, taskStatusName = "Acknowledge" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 4, taskStatusName = "Inprogress" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 5, taskStatusName = "Rejected" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 6, taskStatusName = "Completed" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 7, taskStatusName = "Discarded" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 8, taskStatusName = "Disapproved" });

            return TaskStatusList;
        }

        public static List<TaskStatusType> GetTaskStatus()
        {
            List<TaskStatusType> TaskStatusList = new List<TaskStatusType>();
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 2, taskStatusName = "Assigned" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 6, taskStatusName = "Completed" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 7, taskStatusName = "Discarded" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 8, taskStatusName = "Disapproved" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 4, taskStatusName = "Inprogress" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 5, taskStatusName = "Rejected" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 1, taskStatusName = "Unaccepted" });
            TaskStatusList.Add(new TaskStatusType { TaskStatus = 0, taskStatusName = "Waiting for approval" });
            // TaskStatusList.Add(new TaskStatusType { TaskStatus = 3, taskStatusName = "Acknowledge" });
            return TaskStatusList;
        }

        public class AuthenticationType
        {
            public short Authenticate { get; set; }
            public string AuthenticateType { get; set; }
        }

        public static List<AuthenticationType> GetAuthenticateTypes()
        {
            List<AuthenticationType> AuthTypeList = new List<AuthenticationType>();
            AuthTypeList.Add(new AuthenticationType { Authenticate = 1, AuthenticateType = "Static IP Based Authentication" });
            AuthTypeList.Add(new AuthenticationType { Authenticate = 2, AuthenticateType = "Office Premises Based Authentication" });
            AuthTypeList.Add(new AuthenticationType { Authenticate = 3, AuthenticateType = "Static IP & Office Premises Based Authentication" });


            return AuthTypeList;
        }

        public class InvoiceStatus
        {
            public string StatusName { get; set; }
        }
        public static List<InvoiceStatus> GetInvoiceStatusList()
        {
            List<InvoiceStatus> InvoiceStatusList = new List<InvoiceStatus>();
            InvoiceStatusList.Add(new InvoiceStatus { StatusName = "Accept" });
            InvoiceStatusList.Add(new InvoiceStatus { StatusName = "Onhold" });
            InvoiceStatusList.Add(new InvoiceStatus { StatusName = "Reject" });
            InvoiceStatusList.Add(new InvoiceStatus { StatusName = "Submit" });
            return InvoiceStatusList;
        }

        public static List<InvoiceStatus> GetInvoiceStateList()
        {
            List<InvoiceStatus> StatusList = new List<InvoiceStatus>();

            StatusList.Add(new InvoiceStatus { StatusName = "Accepted" });
            StatusList.Add(new InvoiceStatus { StatusName = "Onhold" });
            StatusList.Add(new InvoiceStatus { StatusName = "Submitted" });
            StatusList.Add(new InvoiceStatus { StatusName = "Rejected" });
            return StatusList;
        }

        public class ShiftTime
        {
            public TimeSpan Time { get; set; }
        }
        public static List<ShiftTime> GetShiftTimeList()
        {
            List<ShiftTime> ShiftTimeList = new List<ShiftTime>();
            ShiftTimeList.Add(new ShiftTime { Time = new TimeSpan(09, 00, 00) });
            ShiftTimeList.Add(new ShiftTime { Time = new TimeSpan(10, 00, 00) });
            ShiftTimeList.Add(new ShiftTime { Time = new TimeSpan(11, 00, 00) });
            ShiftTimeList.Add(new ShiftTime { Time = new TimeSpan(12, 00, 00) });
            ShiftTimeList.Add(new ShiftTime { Time = new TimeSpan(13, 00, 00) });
            ShiftTimeList.Add(new ShiftTime { Time = new TimeSpan(14, 00, 00) });
            ShiftTimeList.Add(new ShiftTime { Time = new TimeSpan(15, 00, 00) });
            ShiftTimeList.Add(new ShiftTime { Time = new TimeSpan(16, 00, 00) });
            ShiftTimeList.Add(new ShiftTime { Time = new TimeSpan(17, 00, 00) });
            ShiftTimeList.Add(new ShiftTime { Time = new TimeSpan(18, 00, 00) });

            return ShiftTimeList;
        }

        public static string ExternalLink(string uri)
        {
            if (uri.StartsWith("http://")) return uri;
            return string.Format("http://{0}", uri);
        }

        //Summary:
        // Get all year for 1960 to till year     
        public static List<SelectListItem> GetYearOfExperience()
        {
            List<SelectListItem> exp = new List<SelectListItem>();

            IEnumerable<int> result = from value in Enumerable.Range(0, 60)
                                      select value;

            foreach (int value in result)
            {
                exp.Add(new SelectListItem { Value = Convert.ToString(value), Text = Convert.ToString(value) });
            }
            return exp;

        }

        public class PlannerRemarks
        {
            public string Remarks { get; set; }
        }
        public static List<PlannerRemarks> GetPlannerRemarks()
        {
            List<PlannerRemarks> PlannerRemarksList = new List<PlannerRemarks>();
            PlannerRemarksList.Add(new PlannerRemarks { Remarks = "Training" });
            PlannerRemarksList.Add(new PlannerRemarks { Remarks = "Travelling" });
            PlannerRemarksList.Add(new PlannerRemarks { Remarks = "Leave" });
            return PlannerRemarksList;
        }

        public static int size;
        public static int MaxSize(string FileType)
        {

            switch (FileType)
            {
                case "Photo": size = 20; // In KB
                    break;
                case "LOGO": size = 512;
                    break;
                case "CV": size = 2048;
                    break;
                case "ID": size = 512;
                    break;
                case "ATTACHMENT": size = 2048;
                    break;
                case "CSV": size = 1024;
                    break;
                default: size = 512;
                    break;
            }

            return size;

        }

        /// <summary>
        ///Created by : Achal Jha
        ///Created on : 17-05-2017
        ///Reason :     For Payroll
        /// </summary>
        /// <returns></returns>
        ///////////////////////////////////////////
        public class EmpStatus
        {
            public string Status { get; set; }
        }
        public static List<EmpStatus> EmployeementStatus()
        {
            List<EmpStatus> Employementstatus = new List<EmpStatus>();
            Employementstatus.Add(new EmpStatus { Status = "Probation Period" });
            Employementstatus.Add(new EmpStatus { Status = "Confirm" });
            Employementstatus.Add(new EmpStatus { Status = "Contract" });
            return Employementstatus;
        }

        public class CategoryList
        {
            public string Category { get; set; }
        }

        public static List<CategoryList> GETAllCategory()
        {
            List<CategoryList> GETAllCategory = new List<CategoryList>();
            GETAllCategory.Add(new CategoryList { Category = "ST" });
            GETAllCategory.Add(new CategoryList { Category = "SC" });
            GETAllCategory.Add(new CategoryList { Category = "OBC" });
            GETAllCategory.Add(new CategoryList { Category = "UR" });
            return GETAllCategory;
        }

        public class IdList
        {
            public string Identification { get; set; }
        }

        public static List<IdList> GETAllIdentification()
        {
            List<IdList> GETAllIdentification = new List<IdList>();
            GETAllIdentification.Add(new IdList { Identification = "Aadhar Card" });
            GETAllIdentification.Add(new IdList { Identification = "Driving Licence" });
            GETAllIdentification.Add(new IdList { Identification = "National Identity Card" });
            GETAllIdentification.Add(new IdList { Identification = "PAN Card" });
            GETAllIdentification.Add(new IdList { Identification = "Passport" });
            GETAllIdentification.Add(new IdList { Identification = "Voter Card" });
            GETAllIdentification.Add(new IdList { Identification = "Other" });
            return GETAllIdentification;
        }


        public class QualificationList
        {
            public string Qualification { get; set; }
        }

        public static List<QualificationList> GetQualification()
        {
            List<QualificationList> GetQualification = new List<QualificationList>();
            GetQualification.Add(new QualificationList { Qualification = "10th" });
            GetQualification.Add(new QualificationList { Qualification = "12th" });
            GetQualification.Add(new QualificationList { Qualification = "Gaduation" });
            GetQualification.Add(new QualificationList { Qualification = "Post Garduation" });
            return GetQualification;
        }

        public class Option
        {
            public string options { get; set; }

        }
        public static List<Option> GetOption()
        {
            List<Option> GetOption = new List<Option>();

            GetOption.Add(new Option { options = "Yes" });
            GetOption.Add(new Option { options = "No" });
            return GetOption;
        }

        public class Grade
        {
            public string GradeId { get; set; }
            public string GradeName { get; set; }
        }
        public static List<Grade> EmployeementGrade()
        {
            List<Grade> GradeList = new List<Grade>();
            GradeList.Add(new Grade { GradeId = "G1", GradeName = "G1" });
            GradeList.Add(new Grade { GradeId = "G2", GradeName = "G2" });
            GradeList.Add(new Grade { GradeId = "G3", GradeName = "G3" });
            GradeList.Add(new Grade { GradeId = "G4", GradeName = "G4" });
            return GradeList;
        }
        //created by : vikas pandey
        // 17/11/2017
        //reson for populate LeaveCategory dropdown in Leavetypemasters
        public class LeaveCategory
        {
            public string LeaveCategoryId { get; set; }
            public string LeaveCategoryName { get; set; }
        }
        public static List<LeaveCategory> LeaveCategoryL()
        {
            List<LeaveCategory> leavecategoryn = new List<LeaveCategory>();
            leavecategoryn.Add(new LeaveCategory { LeaveCategoryId = "D", LeaveCategoryName = "Deductible " });
            leavecategoryn.Add(new LeaveCategory { LeaveCategoryId = "ND", LeaveCategoryName = "Non-Deductible " });
            return leavecategoryn;
        }
        //created by : vikas pandey
        // 17/11/2017
        //reson for populate LeaveCategory dropdown in Leavetypemasters
        public class LeaveLimit
        {
            public int limit { get; set; }

        }
        public static List<LeaveLimit> LeaveLimitL()
        {
            List<LeaveLimit> leavelimitation = new List<LeaveLimit>();
            leavelimitation.Add(new LeaveLimit { limit = 0 });
            leavelimitation.Add(new LeaveLimit { limit = 1 });
            leavelimitation.Add(new LeaveLimit { limit = 2 });
            leavelimitation.Add(new LeaveLimit { limit = 3 });
            leavelimitation.Add(new LeaveLimit { limit = 4 });
            leavelimitation.Add(new LeaveLimit { limit = 5 });
            leavelimitation.Add(new LeaveLimit { limit = 6 });
            leavelimitation.Add(new LeaveLimit { limit = 7 });
            leavelimitation.Add(new LeaveLimit { limit = 8 });
            leavelimitation.Add(new LeaveLimit { limit = 9 });
            leavelimitation.Add(new LeaveLimit { limit = 10 });
            leavelimitation.Add(new LeaveLimit { limit = 11 });
            leavelimitation.Add(new LeaveLimit { limit = 12 });
            leavelimitation.Add(new LeaveLimit { limit = 13 });
            leavelimitation.Add(new LeaveLimit { limit = 14 });
            leavelimitation.Add(new LeaveLimit { limit = 15 });
            leavelimitation.Add(new LeaveLimit { limit = 16 });
            leavelimitation.Add(new LeaveLimit { limit = 17 });
            leavelimitation.Add(new LeaveLimit { limit = 18 });
            leavelimitation.Add(new LeaveLimit { limit = 19 });
            leavelimitation.Add(new LeaveLimit { limit = 20 });
            leavelimitation.Add(new LeaveLimit { limit = 21 });
            leavelimitation.Add(new LeaveLimit { limit = 22 });
            leavelimitation.Add(new LeaveLimit { limit = 23 });
            leavelimitation.Add(new LeaveLimit { limit = 24 });
            leavelimitation.Add(new LeaveLimit { limit = 25 });
            leavelimitation.Add(new LeaveLimit { limit = 26 });
            leavelimitation.Add(new LeaveLimit { limit = 27 });
            leavelimitation.Add(new LeaveLimit { limit = 28 });
            leavelimitation.Add(new LeaveLimit { limit = 29 });
            leavelimitation.Add(new LeaveLimit { limit = 30 });
            leavelimitation.Add(new LeaveLimit { limit = 31 });
            leavelimitation.Add(new LeaveLimit { limit = 32 });
            leavelimitation.Add(new LeaveLimit { limit = 33 });
            leavelimitation.Add(new LeaveLimit { limit = 34 });
            leavelimitation.Add(new LeaveLimit { limit = 35 });
            leavelimitation.Add(new LeaveLimit { limit = 36 });
            leavelimitation.Add(new LeaveLimit { limit = 37 });
            leavelimitation.Add(new LeaveLimit { limit = 38 });
            leavelimitation.Add(new LeaveLimit { limit = 39 });
            leavelimitation.Add(new LeaveLimit { limit = 40 });
            leavelimitation.Add(new LeaveLimit { limit = 41 });
            leavelimitation.Add(new LeaveLimit { limit = 42 });
            leavelimitation.Add(new LeaveLimit { limit = 43 });
            leavelimitation.Add(new LeaveLimit { limit = 44 });
            leavelimitation.Add(new LeaveLimit { limit = 45 });
            leavelimitation.Add(new LeaveLimit { limit = 46 });
            leavelimitation.Add(new LeaveLimit { limit = 47 });
            leavelimitation.Add(new LeaveLimit { limit = 48 });
            leavelimitation.Add(new LeaveLimit { limit = 49 });
            leavelimitation.Add(new LeaveLimit { limit = 50 });

            return leavelimitation;
        }
        //created by vikas pandey 21/11/2017 for yearendaction
        public class YearendActions
        {
            public string Yearshortname { get; set; }
            public string Yearaction { get; set; }
        }

        public static List<YearendActions> GetYearEndAction()
        {
            List<YearendActions> YearEndActionList = new List<YearendActions>();
            YearEndActionList.Add(new YearendActions { Yearshortname = "E", Yearaction = "Encash" });
            YearEndActionList.Add(new YearendActions { Yearshortname = "L", Yearaction = "Lapse" });
            YearEndActionList.Add(new YearendActions { Yearshortname = "F", Yearaction = "Forward" });

            return YearEndActionList;
        }

        //created by vikas pandey 24/11/2017 for yearendaction
        public class CalYear
        {
            public string CalendarYear { get; set; }
            public string Text { get; set; }

        }

        public static List<CalYear> GetCalendarYear()
        {
            List<CalYear> calyearList = new List<CalYear>();
            calyearList.Add(new CalYear { CalendarYear = "Jan-Dec", Text = "Jan-Dec" });
            calyearList.Add(new CalYear { CalendarYear = "Apr-Mar", Text = "Apr-Mar" });
            return calyearList;
        }
        //created by vikas pandey 24/11/2017 for PayrollDate
        public class PayrollDated
        {
            public Int16 PayrollDate { get; set; }
            public string PayrollDatetext { get; set; }

        }

        public static List<PayrollDated> GetPayrollProcessDate()
        {
            List<PayrollDated> PayrDateList = new List<PayrollDated>();
            PayrDateList.Add(new PayrollDated { PayrollDate = 1, PayrollDatetext = "1st" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 2, PayrollDatetext = "2nd" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 3, PayrollDatetext = "3rd" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 5, PayrollDatetext = "4rt" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 5, PayrollDatetext = "5th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 6, PayrollDatetext = "6th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 7, PayrollDatetext = "7th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 8, PayrollDatetext = "8th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 9, PayrollDatetext = "9th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 10, PayrollDatetext = "10th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 11, PayrollDatetext = "11th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 12, PayrollDatetext = "12th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 13, PayrollDatetext = "13th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 14, PayrollDatetext = "14th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 15, PayrollDatetext = "15th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 16, PayrollDatetext = "16th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 17, PayrollDatetext = "17th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 18, PayrollDatetext = "18th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 19, PayrollDatetext = "19th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 20, PayrollDatetext = "20th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 21, PayrollDatetext = "21st" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 22, PayrollDatetext = "22nd" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 23, PayrollDatetext = "23rd" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 24, PayrollDatetext = "24th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 25, PayrollDatetext = "25th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 26, PayrollDatetext = "26th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 27, PayrollDatetext = "27th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 28, PayrollDatetext = "28th" });
            PayrDateList.Add(new PayrollDated { PayrollDate = -2, PayrollDatetext = "Last But Two Day" });
            PayrDateList.Add(new PayrollDated { PayrollDate = -1, PayrollDatetext = "Last But One Day" });
            PayrDateList.Add(new PayrollDated { PayrollDate = 0, PayrollDatetext = "Last Day Of The Month" });

            return PayrDateList;
        }

        public class PayrollPerweekDay
        {
            public Int16 PayrollworkDay { get; set; }

        }

        public static List<PayrollPerweekDay> GetPayrollPerweekDay()
        {
            List<PayrollPerweekDay> PerweekDayList = new List<PayrollPerweekDay>();
            PerweekDayList.Add(new PayrollPerweekDay { PayrollworkDay = 5 });
            PerweekDayList.Add(new PayrollPerweekDay { PayrollworkDay = 6 });

            return PerweekDayList;
        }

        //Createdby Ajay Kumar Choudhary Creatde on :- 18-05-2017
        // Reason:- For sending Birhtday Mails
        public static void Start()
        {
            try
            {
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Start();

                IJobDetail Birthdayjob = JobBuilder.Create<BirthdayManager>().Build();

                ITrigger firstTrigger = TriggerBuilder.Create()
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(09, 47))
                      )
                    .Build();
                //UserDBContext db = new UserDBContext();
                //var Schedulers = db.MailScheduler.ToList();
                //if (Schedulers.Where(a => a.SchedulerJobName == "Birthdayjob").FirstOrDefault().SchedulerActive)
                //    scheduler.ScheduleJob(Birthdayjob, firstTrigger);

            }
            catch (Exception ex)
            {
                SendEmail(Global.AdminEmail(), "Error Log", ex.ToString());
            }
        }


        public static string AdminEmail()
        {
            if (ConfigurationManager.AppSettings["Environment"].ToString().ToLower() == "development")
                result = "ajay.choudhary@nibf.in";
            else
                result = "ajay.choudhary@nibf.in";

            return result;
        }

        public static void SendEmail(string email, string Subject, string MsgBody)
        {
            try
            {
                //Create the email object first, then add the properties.
                SendGridMessage myMessage = new SendGridMessage();

                myMessage.AddTo(email);
                myMessage.From = new MailAddress("social@nibf.in", "NIBF");
                myMessage.Subject = Subject;
                myMessage.Html = MsgBody;

                // Create credentials, specifying your user name and password.
                var credentials = new NetworkCredential(
                      ConfigurationManager.AppSettings["mailAccount"],
                      ConfigurationManager.AppSettings["mailPassword"]
                      );

                // Create an Web transport for sending email.
                var transportWeb = new Web(credentials);

                // Send the email, which returns an awaitable task.
                transportWeb.DeliverAsync(myMessage);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            //try
            //{
            //    var settings = ConfigurationManager.AppSettings;
            //    var transmission = new Transmission();
            //    transmission.Content.From.Email = ConfigurationManager.AppSettings["From_Email"].ToString();
            //    transmission.Content.Subject = Subject;
            //    transmission.Content.Text = MsgBody;
            //    transmission.Content.Html = MsgBody;

            //    var recipient = new Recipient
            //    {
            //        Address = new Address { Email = email }
            //    };
            //    transmission.Recipients.Add(recipient);

            //    var client = new Client(ConfigurationManager.AppSettings["Spark_Post_API"].ToString());
            //    client.ApiKey = ConfigurationManager.AppSettings["Spark_Post_API"].ToString();
            //    client.CustomSettings.SendingMode = SendingModes.Sync;

            //    var response = client.Transmissions.Send(transmission);
            //}
            //catch (Exception ex)
            //{
            //    string msg = ex.Message;
            //}
        }

        //Created By "Ajay Kumar Choudhary" For Frequency In Question master
        //Summary
        //To get all list of Frequency   
        public class FrequencyList
        {
            public string FrequencyStatus { get; set; }
        }

        public static List<FrequencyList> GetFrequencyList()
        {
            List<FrequencyList> FrequencyList = new List<FrequencyList>();
            FrequencyList.Add(new FrequencyList { FrequencyStatus = "Daily" });
            FrequencyList.Add(new FrequencyList { FrequencyStatus = "Monthly" });

            return FrequencyList;
        }

        public class MarksList
        {
            public Int64 Marks { get; set; }
        }

        public static List<MarksList> GetMarks()
        {
            List<MarksList> MarksfullList = new List<MarksList>();
            for (int i = 0; i <= 100; i++)
            {
                MarksfullList.Add(new MarksList { Marks = i });
            }


            return MarksfullList;
        }

        ///// <summary>
        ///// By: Ajay Kumar Choudhary 
        ///// on: 17-07-2017
        ///// For: Team Member Rights
        ///// </summary>
        //public static class UserRights
        //{
        //    public const Int16 ManageAccount = 1;
        //    public const Int16 ManageInvoice = 3;
        //    public const Int16 ManageJobOrder = 4;
        //}

        //public static class UserRoles
        //{
        //    public const string Admin = "Admin";
        //    public const string Administrator = "Administrator";
        //    public const string Candidate = "Candidate";
        //    public const string Client = "Client";
        //    public const string Employee = "Employee";
        //}



        //public static List<Grade> EmployeementGrade()
        //{
        //    List<Grade> GradeList = new List<Grade>();
        //    GradeList.Add(new Grade { GradeId = "G1",GradeName="G1" });
        //    GradeList.Add(new Grade { GradeId = "G2", GradeName = "G2" });
        //    GradeList.Add(new Grade { GradeId = "G3", GradeName = "G3" });
        //    GradeList.Add(new Grade { GradeId = "G4", GradeName = "G4" });
        //    return GradeList;
        //}
        ///////////////////////////////

        public static string PreloreUrl()
        {
            switch (ConfigurationManager.AppSettings["Environment"].ToString().ToLower())
            {

                case "local":
                    result = "http://localhost:50197/";
                    break;
                case "development":
                    result = "http://preloredev.azurewebsites.net/";
                    break;
                case "production":
                    result = "http://www.prelore.com/";
                    break;
                default:
                    result = "http://www.prelore.com/";
                    break;
            }

            return result;
        }

        public static string WikipianUrl()
        {
            switch (ConfigurationManager.AppSettings["Environment"].ToString().ToLower())
            {

                case "local":
                    result = "http://localhost:55887/";
                    break;
                case "development":
                    result = "http://wikipiandev.azurewebsites.net/";
                    break;
                case "production":
                    result = "http://www.wikipian.com/";
                    break;
                default:
                    result = "http://localhost:55887/";
                    break;
            }

            return result;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@_";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public class PeriodFrequencyList
        {
            public string PeriodFrequencyStatus { get; set; }
        }

        public static List<PeriodFrequencyList> GetPeriodFrequencyList()
        {
            List<PeriodFrequencyList> PeriodFrequencyList = new List<PeriodFrequencyList>();
            PeriodFrequencyList.Add(new PeriodFrequencyList { PeriodFrequencyStatus = "Monthly" });
            PeriodFrequencyList.Add(new PeriodFrequencyList { PeriodFrequencyStatus = "Quarterly" });
            PeriodFrequencyList.Add(new PeriodFrequencyList { PeriodFrequencyStatus = "Annually" });

            return PeriodFrequencyList;
        }

        public class PayrollCategoryList
        {
            public string PayrollCategory { get; set; }
        }

        public static List<PayrollCategoryList> GetPayrollCategoryList()
        {
            List<PayrollCategoryList> PayrollCategoryList = new List<PayrollCategoryList>();
            PayrollCategoryList.Add(new PayrollCategoryList { PayrollCategory = "Earning" });
            PayrollCategoryList.Add(new PayrollCategoryList { PayrollCategory = "Deduction" });

            return PayrollCategoryList;
        }



        public class AttendanceFilter
        {
            public string attendance { get; set; }
        }

        public static List<AttendanceFilter> GetattendanceFilterList()
        {
            List<AttendanceFilter> attendanceList = new List<AttendanceFilter>();
            attendanceList.Add(new AttendanceFilter { attendance = "In Time" });
            attendanceList.Add(new AttendanceFilter { attendance = "Late" });
            attendanceList.Add(new AttendanceFilter { attendance = "Not Yet In" });

            return attendanceList;
        }

        public class LeaveTypeCategory
        {
            public string LeaveTypeCategoryId { get; set; }
            public string LeaveTypeCategoryListName { get; set; }
        }

        public static List<LeaveTypeCategory> GetLeaveTypeCategory()
        {
            List<LeaveTypeCategory> LeaveTypeCategoryList = new List<LeaveTypeCategory>();
            LeaveTypeCategoryList.Add(new LeaveTypeCategory { LeaveTypeCategoryId = "D", LeaveTypeCategoryListName = "Deductible" });
            LeaveTypeCategoryList.Add(new LeaveTypeCategory { LeaveTypeCategoryId = "ND", LeaveTypeCategoryListName = "Non-Deductible" });

            return LeaveTypeCategoryList;
        }
        public class ConfirmationOperation
        {
            public Int16 Status { get; set; }
            public string Operation { get; set; }
        }
        public static List<ConfirmationOperation> GetConfrimationOperations()
        {
            List<ConfirmationOperation> operationlist = new List<ConfirmationOperation>();
            operationlist.Add(new ConfirmationOperation { Status = 1, Operation = "Approve Confirmation" });
            operationlist.Add(new ConfirmationOperation { Status = 2, Operation = "Reject Confirmation" });
            operationlist.Add(new ConfirmationOperation { Status = 3, Operation = "Extend Confirmation" });
            return operationlist;
        }

        public static string JobEnablerUrl()
        {
            switch (ConfigurationManager.AppSettings["Environment"].ToString().ToLower())
            {

                case "local":
                    result = "http://localhost:54360/";
                    break;
                case "development":
                    result = "http://jedev.azurewebsites.net/";
                    break;
                case "production":
                    result = "http://www.jobenablers.com/";
                    break;
                default:
                    result = "http://www.jobenablers.com/";
                    break;
            }

            return result;
        }
    }
}
