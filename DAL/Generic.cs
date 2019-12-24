using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AJSolutions.Areas.Admin.Models;
using AJSolutions.Models;
using System.Data.SqlClient;
using System.Data.Entity;
using DotNetIntegrationKit;
using System.Globalization;
using System.Data;
using System.Reflection;
using System.Net;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;


namespace AJSolutions.DAL
{
    public class Generic
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        private const string strMerchantCode = "L3470";
        private const string strSchemeCode = "NIBF";
        private const string strKEY = "5599569174MHYEJX"; //"1671766618YOTTIM";//
        private const string strIV = "4272145358TUOJWX"; //"8977398069VCCASM";//

        UserDBContext db = new UserDBContext();


        private const string serverURL1 = "msg.msgclub.net";
        private const string authkey1 = "73c881ac71fb9a430e28572fcedc46e";
        private const string senderId1 = "NIBFCG";
        private const string routeId1 = "1";
        private const string smsContentType1 = "english";
        private const string groupId1 = "";
        private const string scheduledate1 = "";
        private const string signature1 = "";
        private const string groupName = "";


        public void sendSMSMessage(string message1, string mobileNos1)
        {
            Sendsms.HitApi hitAPI = new Sendsms.HitApi();
            Console.WriteLine("GetAPI Return Value ::" + hitAPI.hitGetApi(serverURL1, authkey1, message1, senderId1, routeId1, mobileNos1, smsContentType1, groupId1, scheduledate1, signature1, groupName));
        }

        public void sendSMS(string message, string mobileNo)
        {
            string url = "https://merasandesh.com/api/sendsms?username=" + ConfigurationManager.AppSettings["SMSUser"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPass"].ToString() + "&senderid=" + ConfigurationManager.AppSettings["SenderId"].ToString() + "&message=" + message + "&numbers=" + mobileNo + "&unicode=2";

            using (WebClient webClient = new System.Net.WebClient())
            {
                WebClient n = new WebClient();
                string result = n.DownloadString(url);
            }
        }


        //Summary
        //To get list of all roles    
        public List<AddUserRoleViewModel> GetRoles()
        {

            var roles = new List<AddUserRoleViewModel>();

            using (var db = new UserDBContext())
            {
                roles = db.Database
                          .SqlQuery<AddUserRoleViewModel>("EXEC USP_GETROLES").ToList();
            }

            return roles;
        }

        public List<ModuleMaster> GetModules()
        {

            List<ModuleMaster> ModuleMaster = db.ModuleMaster.ToList();

            return ModuleMaster;
        }

        public void UpdateUserHistoryLastLoggedOn(string Id)
        {
            UserHistory history = db.UserHistory.Find(Id);
            if (history != null)
            {
                history.LastLogin = DateTime.UtcNow;

                db.Entry(history).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        //for updateing userhistory subscriberwise

        public bool AddSubscriberHitory(string SubscriberId, DateTime LastLogIn)
        {
            bool res = false;
            try
            {
                using (var context = new UserDBContext())
                {

                    var subscriberid = new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId);

                    var logindate = new SqlParameter("@LogInDate", LastLogIn);
                    int i = context.Database.ExecuteSqlCommand("USP_AddSubscriberLogin  @SubscriberId, @LogInDate", subscriberid, logindate);

                    if (i > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return res;
        }
        //Summary
        //To get list of all access Levels    
        public List<DepartmentMaster> GetDepartments()
        {
            List<DepartmentMaster> departmentList = db.DepartmentMaster.ToList();
            return departmentList;
        }


        public List<DepartmentModuleViewModel> GetModuleWiseDepartments(string ModuleId = null)
        {

            var deparments = new List<DepartmentModuleViewModel>();

            using (var db = new UserDBContext())
            {

                deparments = db.Database
                          .SqlQuery<DepartmentModuleViewModel>("exec USP_GET_DEPARTMENT @ModuleId",
                           new SqlParameter("@ModuleId", string.IsNullOrEmpty(ModuleId) ? DBNull.Value : (object)ModuleId)).ToList();
            }

            if (ModuleId != "CMS")
            {
                DepartmentModuleViewModel t5default = new DepartmentModuleViewModel { Department = "SELECT DEPARTMENTS" };
                deparments.Add(t5default);
                // deparments = deparments.OrderBy(s => s.Department).ToList();
            }

            return deparments;
        }

        /// <summary>
        /// Fetching the list of all Job order Type
        /// </summary>
        /// <returns></returns>

        public List<JobOrderTypeMaster> GetJobOrderType(string SubscriberId)
        {
            List<JobOrderTypeMaster> OrderTypes = db.JobOrderTypeMaster.Where(j => j.CorporateId == SubscriberId).ToList();
            OrderTypes = OrderTypes.OrderBy(s => s.JobOrderType).ToList();
            return OrderTypes;
        }

        public List<JobOrderTypeMaster> GetJobOrderTypeFill()
        {
            List<JobOrderTypeMaster> OrderTypes = db.JobOrderTypeMaster.ToList();

            //JobOrderTypeMaster t5default = new JobOrderTypeMaster { JobOrderType = "Any" };
            //OrderTypes.Add(t5default);
            return OrderTypes;
        }


        public List<ClientViewModel> GetSubscriberWiseClientList(string SubscriberId, bool defaultValueRequired = true)
        {

            var Clients = new List<ClientViewModel>();


            using (var db = new UserDBContext())
            {

                Clients = db.Database
                          .SqlQuery<ClientViewModel>("EXEC USP_GetSubsciberWiseClientList @SubscriberId",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }
            Clients = Clients.Where(c => c.DepartmentId == "CLI" || c.CorporateId == SubscriberId).ToList();
            if (defaultValueRequired)
            {
                ClientViewModel t5default = new ClientViewModel { Name = "Any" };
                Clients.Add(t5default);
            }
            Clients = Clients.OrderBy(s => s.Name).ToList();
            return Clients;
        }


        public List<ClientViewModel> GetSubscriberWiseClientListBulkUpload(string SubscriberId, bool defaultValueRequired = true)
        {

            var Clients = new List<ClientViewModel>();


            using (var db = new UserDBContext())
            {

                Clients = db.Database
                          .SqlQuery<ClientViewModel>("EXEC USP_GetSubsciberWiseClientListBulkUpload @SubscriberId",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }
            Clients = Clients.Where(c => c.DepartmentId == "CLI" || c.CorporateId == SubscriberId).ToList();
            if (defaultValueRequired)
            {
                ClientViewModel t5default = new ClientViewModel { Name = "Any" };
                Clients.Add(t5default);
            }
            Clients = Clients.OrderBy(s => s.Name).ToList();
            return Clients;
        }


        public List<AdminViewModel> GetSubscriberWiseList(string SubscriberId)
        {

            var admin = new List<AdminViewModel>();

            using (var db = new UserDBContext())
            {

                admin = db.Database
                          .SqlQuery<AdminViewModel>("EXEC USP_GetSubsciberWiseList @SubscriberId",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }
            //admin = admin.Where(c => c.DepartmentId == "CLI" && c.DepartmentId == "").ToList();
            //AdminViewModel t5default = new AdminViewModel { Name = "Any" };
            //admin.Add(t5default);

            return admin;
        }

        public class CoAddressType
        {
            public string addresstypeid { get; set; }
            public string addresstype { get; set; }
        }

        public static List<CoAddressType> GetCoAdminAddressType()
        {
            List<CoAddressType> AddressTypeList = new List<CoAddressType>();
            AddressTypeList.Add(new CoAddressType { addresstypeid = "PR", addresstype = "Present" });
            AddressTypeList.Add(new CoAddressType { addresstypeid = "PE", addresstype = "Permanent" });
            AddressTypeList.Add(new CoAddressType { addresstypeid = "CO", addresstype = "Correspondence" });
            return AddressTypeList;
        }

        //Createdby Ajay Kumar Choudhary Creatde on :- 18-05-2017
        // Reason:- For sending Birhtday Mails

        //Summary
        //Birthday Email Notifications 
        public string EmailFormat(string Message, string url, string action, string greeting, string userName, string NotificationType, string ImgUrl = "")
        {
            userName = textInfo.ToTitleCase(userName);

            string start = "<div><br><br>" +
                      "<table width='600' border='0' align='center' cellpadding='0' cellspacing='0'>" +
                       "<tbody><tr>" +
                       "<td height='588' valign='top'><table width='100%' border='0' cellspacing='0' cellpadding='0'>" +
                        "<tbody><tr>" +
                        "<td width='79%' height='588' valign='top'>" +
                        "<table width='100%' border='0' cellspacing='0' cellpadding='0' style='border:solid 1px #225452;padding:15px'>" +
                        "<tbody><tr><td>" +
                        "<table width='100%' border='0' cellspacing='0' cellpadding='0' style='padding:-15px'>" +
                        "<tbody><tr>" +
                        "<td style='font:bold 12px Arial,Helvetica,sans-serif;color:#333;background:#225452;border:solid 1px #e1e1e1;border-radius:1px;color:#062937;padding:-20px'>" +
                        "<a href='http://www.jobenablers.com' target='_blank'><img src='http://www.jobenablers.com/Images/JELogo.png' alt='img' border='0'></a>" +
                        "</td></tr></tbody></table>" +
                        "<table width='100%' border='0' cellspacing='0' cellpadding='0'>" +
                        "<tbody><tr>" +
                        "<td height='85' style='font:normal 12px Arial,Helvetica,sans-serif;color:#333;padding:5px'><strong style='color:#062937'><br>" +
                         greeting + " " + userName + " ,</strong><br> <br><p align='justify'>" + Message + "</p>";

            string imageMsg = "<center><img src='" + ImgUrl + "' alt='img' border='0'></center>";

            string urlMsg = "<table width='50%' border='0' cellspacing='0' cellpadding='0' align='center'>" +
                            "<tbody><tr>" +
                            "<td  valign='top' style='padding-right:10px' align='center'><center><a href='" + url + "' target='_blank'><br /><button class=contact@jobenablers.com'btn btn-success' style='background-color: #204c39;float:right;border: 5px solid #204c39;border-radius: 10px;padding: 5px;color: #ffffff;'>" + action + "</button></a></center>" +
                            "</td></tr></tbody></table><br>";

            string end = "<br></td></tr></tbody></table>" +
                        "<table width='100%' border='0' cellspacing='0' cellpadding='0'>" +
                        " <tbody><tr>" +
                        "<td style='font:bold 12px Arial,Helvetica,sans-serif;color:#333;background:#f0f0f0;border:solid 1px #e1e1e1;border-radius:4px;color:#062937;padding:10px'>SEARCH</td>" +
                        "</tr></tbody></table>" +
                        "<table width='100%' border='0' cellspacing='0' cellpadding='0'>" +
                        "<tbody><tr>" +
                        "<td style='font:bold 12px Arial,Helvetica,sans-serif;color:#333;background:#f0f0f0;border:solid 1px #e1e1e1;border-radius:4px;color:#062937;padding:10px'>FEATURED SERVICES</td>" +
                        " </tr></tbody></table>" +
                        "<table width='100%' border='0' align='center' cellpadding='0' cellspacing='0'>" +
                        "<tbody><tr>" +
                        "<td width='87%' height='28' valign='top' style='font:normal 11px Arial,Helvetica,sans-serif;color:#333'><br>Please add <a href='mailto:contact@jobenablers.com' target='_blank'>contact@jobenablers.com</a> to your address book or safe list to " +
                        "prevent future JOBENABLERS™ updates from being classified as Junk / Bulk Mail<br>";


            end = end + "</td></tr></tbody></table>" +
                    "</tr></tbody></table>" +
                    "</td></tr></tbody></table> " +
                    "</td></tr></tbody></table>";
            if (!string.IsNullOrEmpty(ImgUrl) && !string.IsNullOrEmpty(url))
            {
                Message = start + urlMsg + imageMsg + end;
            }
            else if (!string.IsNullOrEmpty(ImgUrl) && string.IsNullOrEmpty(url))
            {
                Message = start + imageMsg + end;
            }
            else if (string.IsNullOrEmpty(ImgUrl) && !string.IsNullOrEmpty(url))
            {
                Message = start + urlMsg + end;
            }
            else
            {
                Message = start + end;
            }
            return Message;
        }


        public string GetSubscriberId(string UserId)
        {
            return db.CorporateProfile.Find(UserId).SubscriberId;
        }
        public string GetName(string SubscriberId)
        {
            return db.CorporateProfile.Find(SubscriberId).Name;
        }
        public string GetUserName(string UserId)
        {
            return db.UserProfile.Find(UserId).Name;
        }

        public List<MambersView> GetEmployee(string SubscriberId)
        {
            var Employee = new List<MambersView>();

            using (var db = new UserDBContext())
            {

                Employee = db.Database
                          .SqlQuery<MambersView>("exec USP_GetAssignToMamber  @SubscriberId",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            MambersView t5default = new MambersView { Name = String.Empty };
            //Employee.Add(t5default);
            Employee = Employee.Where(aj => aj.Deactivated == false).ToList();
            Employee = Employee.OrderBy(s => s.Name).ToList();
            return Employee;

        }

        public List<MambersView> GetEmployeewithDepartment(string SubscriberId)
        {
            var Employee = new List<MambersView>();

            using (var db = new UserDBContext())
            {

                Employee = db.Database
                          .SqlQuery<MambersView>("exec USP_GetEmployeeWithDepartment  @SubscriberId",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            MambersView t5default = new MambersView { Name = String.Empty };
            //Employee.Add(t5default);
            Employee = Employee.Where(aj => aj.Deactivated == false).ToList();
            Employee = Employee.OrderBy(s => s.Name).ToList();
            return Employee;
        }

        public List<MambersView> GetDeactivatedEmployeewithDepartment(string SubscriberId)
        {
            var Employee = new List<MambersView>();

            using (var db = new UserDBContext())
            {

                Employee = db.Database
                          .SqlQuery<MambersView>("exec USP_GetEmployeeWithDepartment  @SubscriberId",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            MambersView t5default = new MambersView { Name = String.Empty };
            //Employee.Add(t5default);
            Employee = Employee.Where(aj => aj.Deactivated == true).ToList();
            Employee = Employee.OrderBy(s => s.Name).ToList();
            return Employee;
        }

        public List<JobOrder> GetJobOrder()
        {
            List<JobOrder> JobOrdeList = db.JobOrder.ToList();

            return JobOrdeList;
        }

        public List<TaskMaster> GetTask()
        {
            List<TaskMaster> Task = db.TaskMaster.ToList();

            return Task;
        }

        public List<CountryMaster> GetCountry()
        {

            List<CountryMaster> CountryMaster = db.CountryMaster.ToList();

            return CountryMaster;
        }

        public List<StatesMaster> GetState(int CountryId)
        {

            List<StatesMaster> StatesMaster = (from s in db.StatesMaster.Where(s => s.CountryId == CountryId) select s).ToList();

            return StatesMaster;
        }

        public List<CityMaster> GetCity(int StateId)
        {

            List<CityMaster> CitiesMaster = (from s in db.CityMaster.Where(s => s.StateId == StateId) select s).ToList();

            return CitiesMaster;
        }

        //Summary
        //To get all list of Company Type 
        public List<CompanyType> GetCompanyType()
        {
            List<CompanyType> CompanyTypeList = new List<CompanyType>();
            CompanyTypeList.Add(new CompanyType { CType = "Private" });
            CompanyTypeList.Add(new CompanyType { CType = "Public" });
            CompanyTypeList.Add(new CompanyType { CType = "Government" });
            CompanyTypeList.Add(new CompanyType { CType = "Non Profitable" });
            //CompanyTypeList.Add(new CompanyType { CType = "Partnerships limited by Shares" });
            //CompanyTypeList.Add(new CompanyType { CType = "Property management companies" });
            //CompanyTypeList.Add(new CompanyType { CType = "Community Interest Companies (CICs)" });
            //CompanyTypeList.Add(new CompanyType { CType = "Charitable Incorporated Organisation (CIO)" });
            return CompanyTypeList;
        }

        //Summary
        //To get all list of Company Size 
        public List<CompanySizeList> GetCompanySize()
        {
            List<CompanySizeList> CompanysizeList = new List<CompanySizeList>();
            CompanysizeList.Add(new CompanySizeList { CompanySize = "Micro" });
            CompanysizeList.Add(new CompanySizeList { CompanySize = "Small" });
            CompanysizeList.Add(new CompanySizeList { CompanySize = "Medium" });
            CompanysizeList.Add(new CompanySizeList { CompanySize = "Large" });
            CompanysizeList.Add(new CompanySizeList { CompanySize = "Enterprise" });
            return CompanysizeList;
        }

        public List<QualificationMaster> GetQualification()
        {

            List<QualificationMaster> QualificationMaster = (from s in db.QualificationMaster select s).ToList();

            return QualificationMaster;
        }

        public List<OrganizationMaster> GetOrganization()
        {

            List<OrganizationMaster> OrganizationMaster = (from s in db.OrganizationMaster select s).ToList();

            return OrganizationMaster;
        }

        public List<DomainMaster> GetDomain()
        {

            List<DomainMaster> DomainMaster = (from s in db.DomainMaster select s).ToList();

            return DomainMaster;
        }

        public List<SpecializationMaster> GetSpecialization()
        {

            List<SpecializationMaster> SpecializationMaster = (from s in db.SpecializationMaster select s).ToList();

            return SpecializationMaster;
        }

        public List<ProjectMaster> GetProject()
        {

            List<ProjectMaster> ProjectMaster = (from s in db.ProjectMaster select s).ToList();

            return ProjectMaster;
        }

        public class Languages
        {
            public string Language { get; set; }
        }

        public static List<Languages> Language()
        {
            List<Languages> LanguageList = new List<Languages>();
            LanguageList.Add(new Languages { Language = "Hindi" });
            LanguageList.Add(new Languages { Language = "English" });
            LanguageList.Add(new Languages { Language = "Bengali" });
            LanguageList.Add(new Languages { Language = "Telugu" });
            LanguageList.Add(new Languages { Language = "Marathi" });
            LanguageList.Add(new Languages { Language = "Tamil" });
            LanguageList.Add(new Languages { Language = "Urdu" });
            LanguageList.Add(new Languages { Language = "Kannada" });
            LanguageList.Add(new Languages { Language = "Gujrati" });
            LanguageList.Add(new Languages { Language = "Odia" });
            LanguageList.Add(new Languages { Language = "Malayalam" });
            LanguageList.Add(new Languages { Language = "Sanskrit" });
            return LanguageList;
        }
        //ZONE for trainers
        public class Zones
        {
            public string Zone { get; set; }
        }

        public static List<Zones> Zone()
        {
            List<Zones> ZoneList = new List<Zones>();
            ZoneList.Add(new Zones { Zone = "North" });
            ZoneList.Add(new Zones { Zone = "South" });
            ZoneList.Add(new Zones { Zone = "East" });
            ZoneList.Add(new Zones { Zone = "West" });
            ZoneList.Add(new Zones { Zone = "North-East" });
            ZoneList.Add(new Zones { Zone = "South-East" });
            ZoneList.Add(new Zones { Zone = "North-West" });
            ZoneList.Add(new Zones { Zone = "South-West" });
            ZoneList.Add(new Zones { Zone = "Central" });
            return ZoneList;
        }

        public DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        {
            DataTable dt = new DataTable();


            PropertyInfo[] columns = null;

            if (Linqlist == null) return dt;

            foreach (T Record in Linqlist)
            {

                if (columns == null)
                {
                    columns = ((Type)Record.GetType()).GetProperties();
                    foreach (PropertyInfo GetProperty in columns)
                    {
                        Type colType = GetProperty.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dt.Columns.Add(new DataColumn(GetProperty.Name, colType));
                    }
                }

                DataRow dr = dt.NewRow();

                foreach (PropertyInfo pinfo in columns)
                {
                    dr[pinfo.Name] = pinfo.GetValue(Record, null) == null ? DBNull.Value : pinfo.GetValue
                    (Record, null);
                }

                dt.Rows.Add(dr);
            }
            return dt;
        }

        public class Page
        {
            public int PageSize { get; set; }
        }

        public List<Page> GetPaging()
        {
            List<Page> PageList = new List<Page>();
            PageList.Add(new Page { PageSize = 1 });
            PageList.Add(new Page { PageSize = 10 });
            PageList.Add(new Page { PageSize = 20 });
            PageList.Add(new Page { PageSize = 50 });
            PageList.Add(new Page { PageSize = 100 });

            return PageList;
        }

        //Summary:
        // Get all year for 1960 to till year     
        public List<Years> GetYear()
        {
            List<Years> yearofPassing = new List<Years>();

            IEnumerable<int> result = from value in Enumerable.Range(1960, DateTime.UtcNow.Year + 6 - 1960)
                                      select value;

            foreach (int value in result)
            {
                yearofPassing.Add(new Years { Year = Convert.ToString(value) });
            }
            return yearofPassing;

        }


        //Summary
        //To get all list of Currency     
        public List<AllCurrency> GetCurrency()
        {
            List<AllCurrency> CurrencyList = db.Currency.ToList();
            return CurrencyList;
        }

        //Summary:
        // Get all the possible list to populate Nationalties   
        public List<Nationalities> GetNationalityList()
        {
            List<Nationalities> Nlist = db.Nationalities.ToList();
            return Nlist;
        }
        /// <summary>
        /// Get the basic data of any time of users
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>

        public UserViewModel GetUserDetail(string UserId)
        {
            UserViewModel user;
            using (var db = new UserDBContext())
            {

                user = db.Database
                          .SqlQuery<UserViewModel>("exec GetUserData  @UserId",
                          new SqlParameter("@UserId", string.IsNullOrEmpty(UserId) ? DBNull.Value : (object)UserId)).FirstOrDefault();
            }
            return user;
        }

        #region Payment Gateway Functions




        public string PaymentRequest(string MerchantTxnRefNo, string PayeeName, string Amount, string EmailAddress, string MobileNumber, string Comment, string strReturnUrl)
        {

            string shoppingCartDetail = strSchemeCode + "_" + Amount + "_0.0";

            string result = PostRequest("T", strMerchantCode, MerchantTxnRefNo, PayeeName, Amount, Comment, "INR", strReturnUrl, shoppingCartDetail, DateTime.UtcNow, EmailAddress, MobileNumber, PayeeName, strKEY, strIV);

            return result;
        }

        public string PostRequest(string RequestType, string MerchantCode, string MerchantTxnRefNo, string PayeeName, string Amount, string Comments, string CurrencyCode, string ReturnUrl,
                                    string ShoppingCartDetails, DateTime PaymentDate, string Email, string MobileNumber, string CustomerName, string Key, string IV)
        {
            RequestURL objRequestURL = new RequestURL();

            String response = objRequestURL.SendRequest
                                           (
                                                  RequestType
                                                , MerchantCode
                                                , MerchantTxnRefNo
                                                , PayeeName
                                                , Amount
                                                , CurrencyCode
                                                , MerchantTxnRefNo
                                                , ReturnUrl
                                                , ""
                                                , ""
                                                , ShoppingCartDetails
                                                , PaymentDate.ToString("dd-MM-yyyy")
                                                , Email
                                                , MobileNumber
                                                , ""
                                                , PayeeName
                                                , ""
                                                , ""
                                                , Key
                                                , IV
                                                );

            return response;
            //else
            //{
            //    if (RequestType.ToUpper() == "T")
            //    {

            //        Response.Write("<form name='s1_2' id='s1_2' action='" + response + "' method='post'> ");

            //        Response.Write("<script type='text/javascript' language='javascript' >document.getElementById('s1_2').submit();");

            //        Response.Write("</script>");
            //        Response.Write("<script language='javascript' >");
            //        Response.Write("</script>");
            //        Response.Write("</form> ");

            //    }

        }







        #region Variable Declaration
        string strPG_TxnStatus = string.Empty,
        strPG_ClintTxnRefNo = string.Empty,
                strPG_TPSLTxnBankCode = string.Empty,
                strPG_TPSLTxnID = string.Empty,
                strPG_TxnAmount = string.Empty,
                strPG_TxnDateTime = string.Empty,
                strPG_TxnDate = string.Empty,
                strPG_TxnTime = string.Empty,
                strPG_TxnMsg = string.Empty;
        string[] strSplitDecryptedResponse;
        string[] strArrPG_TxnDateTime;

        #endregion


        public string ConfirmTransaction(string PGResponse)
        {
            RequestURL objRequestURL = new RequestURL();

            string strDecryptedVal = objRequestURL.VerifyPGResponse(PGResponse, strKEY, strIV);

            if (strDecryptedVal.StartsWith("ERROR"))
            {
                return strPG_ClintTxnRefNo + "|" + "Transaction Fail :: <br/>" + "Response :: <br/>" + strDecryptedVal;
            }
            else
            {
                strSplitDecryptedResponse = strDecryptedVal.Split('|');
                GetPGRespnseData(strSplitDecryptedResponse, strDecryptedVal);

                if (strPG_TxnStatus == "0300")
                {
                    return strPG_ClintTxnRefNo + "|" + "Transaction Success  " + strPG_TxnStatus;
                }
                else
                {
                    return strPG_ClintTxnRefNo + "|" + "Transaction Fail :: <br/>" + "Response :: <br/>"
            + strDecryptedVal;
                }
            }
        }

        public void GetPGRespnseData(string[] parameters, string pgResponsedValue)
        {
            string[] strGetMerchantParamForCompare;
            for (int i = 0; i < parameters.Length; i++)
            {
                strGetMerchantParamForCompare = parameters[i].ToString().Split('=');
                if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "TXN_STATUS")
                {
                    strPG_TxnStatus = Convert.ToString(strGetMerchantParamForCompare[1]);
                }
                else if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "TXN_MSG")
                {
                    strPG_TxnMsg = Convert.ToString(strGetMerchantParamForCompare[1]);
                }
                else if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "CLNT_TXN_REF")
                {
                    strPG_ClintTxnRefNo = Convert.ToString(strGetMerchantParamForCompare[1]);
                }
                else if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "TPSL_BANK_CD")
                {
                    strPG_TPSLTxnBankCode = Convert.ToString(strGetMerchantParamForCompare[1]);
                }
                else if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "TPSL_TXN_ID")
                {
                    strPG_TPSLTxnID = Convert.ToString(strGetMerchantParamForCompare[1]);
                }
                else if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "TXN_AMT")
                {
                    strPG_TxnAmount = Convert.ToString(strGetMerchantParamForCompare[1]);
                }
                else if (Convert.ToString(strGetMerchantParamForCompare[0]).ToUpper().Trim() == "TPSL_TXN_TIME")
                {
                    strPG_TxnDateTime = Convert.ToString(strGetMerchantParamForCompare[1]);
                    strArrPG_TxnDateTime = strPG_TxnDateTime.Split(' ');
                    strPG_TxnDate = Convert.ToString(strArrPG_TxnDateTime[0]);
                    strPG_TxnTime = Convert.ToString(strArrPG_TxnDateTime[1]);
                }
            }

            Student student = new Student();

            DateTime? strPGTxnDate = null;
            if (!string.IsNullOrEmpty(strPG_TxnDateTime))
                strPGTxnDate = Convert.ToDateTime(strPG_TxnDateTime);

            var studentFee = db.FeeDetails.Find(strPG_ClintTxnRefNo);
            if (studentFee != null)
            {
                if (strPG_TxnMsg == "success")
                {
                    student.AddCandidateFeeDetails(strPG_ClintTxnRefNo, studentFee.UserId, Convert.ToSingle(strPG_TxnAmount), studentFee.CourseCode, studentFee.BatchId, studentFee.TransactionDate, strPGTxnDate, studentFee.PaymentModeId, studentFee.BankName, strPG_TPSLTxnID, "Initiate", strPG_TPSLTxnBankCode, studentFee.Remarks, pgResponsedValue, studentFee.TotalInstallment, studentFee.InstallmentNumber, studentFee.RemainingAmount);
                }
                else
                {
                    student.AddCandidateFeeDetails(strPG_ClintTxnRefNo, studentFee.UserId, Convert.ToSingle(strPG_TxnAmount), studentFee.CourseCode, studentFee.BatchId, studentFee.TransactionDate, strPGTxnDate, studentFee.PaymentModeId, studentFee.BankName, strPG_TPSLTxnID, "Failed", strPG_TPSLTxnBankCode, studentFee.Remarks, pgResponsedValue, studentFee.TotalInstallment, studentFee.InstallmentNumber, studentFee.RemainingAmount);
                }

            }
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>

        public class Months
        {
            public string MonthName { get; set; }
            public int MonthId { get; set; }
        }

        public List<Months> GetMonths()
        {
            List<Months> MonthList = new List<Months>();
            MonthList.Add(new Months { MonthId = 1, MonthName = "January" });
            MonthList.Add(new Months { MonthId = 2, MonthName = "February" });
            MonthList.Add(new Months { MonthId = 3, MonthName = "March" });
            MonthList.Add(new Months { MonthId = 4, MonthName = "April" });
            MonthList.Add(new Months { MonthId = 5, MonthName = "May" });
            MonthList.Add(new Months { MonthId = 6, MonthName = "June" });
            MonthList.Add(new Months { MonthId = 7, MonthName = "July" });
            MonthList.Add(new Months { MonthId = 8, MonthName = "August" });
            MonthList.Add(new Months { MonthId = 9, MonthName = "September" });
            MonthList.Add(new Months { MonthId = 10, MonthName = "October" });
            MonthList.Add(new Months { MonthId = 11, MonthName = "November" });
            MonthList.Add(new Months { MonthId = 12, MonthName = "December" });
            return MonthList;
        }



        public List<Months> GetPayoutMonths()
        {
            int year = DateTime.Now.Year;
            int fyear = year;
            if (DateTime.Now.Month <= 4)
            {
                fyear = fyear - 1;
            }
            else
            {
                fyear = year;
                year = year + 1;
            }

            List<Months> MonthList = new List<Months>();
            MonthList.Add(new Months { MonthId = 4, MonthName = "Apr-" + fyear.ToString() });
            MonthList.Add(new Months { MonthId = 5, MonthName = "May-" + fyear.ToString() });
            MonthList.Add(new Months { MonthId = 6, MonthName = "Jun-" + fyear.ToString() });
            MonthList.Add(new Months { MonthId = 7, MonthName = "Jul-" + fyear.ToString() });
            MonthList.Add(new Months { MonthId = 8, MonthName = "Aug-" + fyear.ToString() });
            MonthList.Add(new Months { MonthId = 9, MonthName = "Sep-" + fyear.ToString() });
            MonthList.Add(new Months { MonthId = 10, MonthName = "Oct-" + fyear.ToString() });
            MonthList.Add(new Months { MonthId = 11, MonthName = "Nov-" + fyear.ToString() });
            MonthList.Add(new Months { MonthId = 12, MonthName = "Dec-" + fyear.ToString() });
            MonthList.Add(new Months { MonthId = 1, MonthName = "Jan-" + year.ToString() });
            MonthList.Add(new Months { MonthId = 2, MonthName = "Feb-" + year.ToString() });
            MonthList.Add(new Months { MonthId = 3, MonthName = "Mar-" + year.ToString() });
            return MonthList;
        }

        public class Years
        {
            public string Year { get; set; }
        }
        /// <summary>
        /// Changed by : Achal Kumar Jha
        /// Change For : Payroll System
        /// Changed On : 29-05-2017
        /// </summary>
        public class PayrollSalaryType
        {
            public string salraryTypeId { get; set; }
            public string salraryTypeName { get; set; }
        }

        public List<PayrollSalaryType> GetPayrollSalaryType()
        {
            List<PayrollSalaryType> PayrollTypeList = new List<PayrollSalaryType>();
            PayrollTypeList.Add(new PayrollSalaryType { salraryTypeId = "0", salraryTypeName = "Earning" });
            PayrollTypeList.Add(new PayrollSalaryType { salraryTypeId = "1", salraryTypeName = "Deductions" });
            return PayrollTypeList;
        }

        /// <summary>
        /// Changed by : Achal Kumar Jha
        /// Change For : Payroll System
        /// Changed On : 30-05-2017
        /// </summary>
        public class SalarycalculationOn
        {
            public string SalarycalculationOnId { get; set; }
            public string SalarycalculationOnName { get; set; }
        }
        public List<SalarycalculationOn> GetSalaryCalculationOnList()
        {
            List<SalarycalculationOn> SalaryCalculationOnList = new List<SalarycalculationOn>();
            SalaryCalculationOnList.Add(new SalarycalculationOn { SalarycalculationOnId = "0", SalarycalculationOnName = "Total No of Days" });
            SalaryCalculationOnList.Add(new SalarycalculationOn { SalarycalculationOnId = "1", SalarycalculationOnName = "Total No of Working Days" });
            return SalaryCalculationOnList;
        }
        public class HolidaysInSalary
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
        public List<HolidaysInSalary> GetHolidaysInSalaryList()
        {
            List<HolidaysInSalary> HolidaysInSalaryList = new List<HolidaysInSalary>();
            HolidaysInSalaryList.Add(new HolidaysInSalary { Id = "0", Name = "Include" });
            HolidaysInSalaryList.Add(new HolidaysInSalary { Id = "1", Name = "Exclude" });
            return HolidaysInSalaryList;
        }

        /// <summary>
        /// Changed by : Achal Kumar Jha
        /// Change For : Payroll Head System
        /// Changed On : 31-05-2017
        /// </summary>
        public List<MambersView> GetPayrollHeadLists(string SubscriberId)
        {
            var Employee = new List<MambersView>();

            using (var db = new UserDBContext())
            {

                Employee = db.Database
                          .SqlQuery<MambersView>("exec USP_GetAssignToMamber  @SubscriberId",
                          new SqlParameter("@SubscriberId", string.IsNullOrEmpty(SubscriberId) ? DBNull.Value : (object)SubscriberId)).ToList();
            }

            MambersView t5default = new MambersView { Name = String.Empty };
            //Employee.Add(t5default);
            Employee = Employee.OrderBy(s => s.Name).ToList();
            return Employee;

        }
        public class PayrollHeadCalculationCriteria
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
        public List<PayrollHeadCalculationCriteria> GetHeadCalculationMethodList()
        {
            List<PayrollHeadCalculationCriteria> MethodList = new List<PayrollHeadCalculationCriteria>();
            MethodList.Add(new PayrollHeadCalculationCriteria { Id = "0", Name = "Calculated on Gross" });
            MethodList.Add(new PayrollHeadCalculationCriteria { Id = "1", Name = "Calculated on Basic" });
            MethodList.Add(new PayrollHeadCalculationCriteria { Id = "2", Name = "Fixed" });
            return MethodList;
        }

        public class Category
        {
            public Int16 CategoryType { get; set; }
            public string CategoryTypeName { get; set; }
        }

        public List<Category> GetCategory()
        {
            List<Category> CategoryLists = new List<Category>();
            CategoryLists.Add(new Category { CategoryType = 1, CategoryTypeName = "Earning" });
            CategoryLists.Add(new Category { CategoryType = 2, CategoryTypeName = "Deduction" });

            return CategoryLists;
        }

        public class ReligionList
        {
            public Int16 ReligionId { get; set; }
            public string Religion { get; set; }
        }

        public List<ReligionList> GetReligions()
        {
            List<ReligionList> ReligionList = new List<ReligionList>();
            ReligionList.Add(new ReligionList { ReligionId = 1, Religion = "Hinduism" });
            ReligionList.Add(new ReligionList { ReligionId = 2, Religion = "Islam" });
            ReligionList.Add(new ReligionList { ReligionId = 3, Religion = "Christianity" });
            ReligionList.Add(new ReligionList { ReligionId = 4, Religion = "Sikhism" });
            ReligionList.Add(new ReligionList { ReligionId = 5, Religion = "Buddism" });
            ReligionList.Add(new ReligionList { ReligionId = 6, Religion = "Jainism" });
            ReligionList.Add(new ReligionList { ReligionId = 7, Religion = "Zoroastrianism" });
            ReligionList.Add(new ReligionList { ReligionId = 8, Religion = "Other / UnSpecified" });
            return ReligionList;
        }

        public class RelocateList
        {
            public Int16 RelocateId { get; set; }
            public string Relocate { get; set; }
        }

        public List<RelocateList> GetRelocateList()
        {
            List<RelocateList> RelocateList = new List<RelocateList>();
            RelocateList.Add(new RelocateList { RelocateId = 1, Relocate = "Across India" });
            RelocateList.Add(new RelocateList { RelocateId = 2, Relocate = "Within State" });
            RelocateList.Add(new RelocateList { RelocateId = 3, Relocate = "Within District" });
            return RelocateList;
        }

        public List<LeaveType> GetLeavetype()
        {
            List<LeaveType> LeaveMaster = db.LeaveType.ToList();
            return LeaveMaster;
        }

        public class AttedanceSessions
        {
            public string Session { get; set; }
        }

        public List<AttedanceSessions> GetAttedanceSessions()
        {
            List<AttedanceSessions> Sessions = new List<AttedanceSessions>();
            Sessions.Add(new AttedanceSessions { Session = "Full Day" });
            Sessions.Add(new AttedanceSessions { Session = "Morning" });
            Sessions.Add(new AttedanceSessions { Session = "Noon" });
            return Sessions;
        }


        public class EmailList
        {
            public string Email { get; set; }
        }
        public List<EmailList> GetEmails()
        {
            List<EmailList> AllEmail = new List<EmailList>();
            AllEmail.Add(new EmailList { Email = "ajay.choudhary@nibf.in" });
            //AllEmail.Add(new EmailList { Email = "sailaja.deol@nibf.in" });
            //AllEmail.Add(new EmailList { Email = "biswajit.chattaraj@nibf.in" });
            //AllEmail.Add(new EmailList { Email = "satendra.shrivastava@nibf.in" });
            //AllEmail.Add(new EmailList { Email = "jayesh.surisetti@nibf.in" });
            return AllEmail;
        }

        public class BigDataPage
        {
            public int BigDataPageSize { get; set; }
        }

        public List<BigDataPage> GetBigDataPaging()
        {
            List<BigDataPage> BigDataPageList = new List<BigDataPage>();
            BigDataPageList.Add(new BigDataPage { BigDataPageSize = 500 });
            BigDataPageList.Add(new BigDataPage { BigDataPageSize = 1000 });
            BigDataPageList.Add(new BigDataPage { BigDataPageSize = 2500 });
            BigDataPageList.Add(new BigDataPage { BigDataPageSize = 5000 });
            BigDataPageList.Add(new BigDataPage { BigDataPageSize = 10000 });
            BigDataPageList.Add(new BigDataPage { BigDataPageSize = 15000 });
            BigDataPageList.Add(new BigDataPage { BigDataPageSize = 20000 });

            return BigDataPageList;
        }

    }

    public class CompanyType
    {
        public string CType { get; set; }
    }

    public class CompanySizeList
    {
        public string CompanySize { get; set; }
    }

    public static class JEExtensions
    {

        public static MvcHtmlString EncodedActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            string queryString = string.Empty;
            string htmlAttributesString = string.Empty;
            if (routeValues != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(routeValues);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    if (i > 0)
                    {
                        queryString += "?";
                    }
                    queryString += d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }

            if (htmlAttributes != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(htmlAttributes);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    htmlAttributesString += " " + d.Keys.ElementAt(i) + "= '" + d.Values.ElementAt(i) + "'";
                }
            }

            //<a href="/Answer?questionId=14">What is Entity Framework??</a>
            StringBuilder ancor = new StringBuilder();
            ancor.Append("<a ");
            if (htmlAttributesString != string.Empty)
            {
                ancor.Append(htmlAttributesString);
            }
            ancor.Append(" href='");
            if (controllerName != string.Empty)
            {
                ancor.Append("/" + controllerName);
            }

            if (actionName != "Index")
            {
                ancor.Append("/" + actionName);
            }
            if (queryString != string.Empty)
            {
                // ancor.Append("?q=" + Encrypt(queryString));
                ancor.Append("?q=" + HttpUtility.UrlEncode(Encrypt(queryString)));
            }
            ancor.Append("'");
            ancor.Append(">");
            ancor.Append(linkText);
            ancor.Append("</a>");
            return new MvcHtmlString(ancor.ToString());
        }


        private static string Encrypt(string plainText)
        {
            string key = "jdsg432387#";
            byte[] EncryptKey = { };
            byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
            EncryptKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByte = Encoding.UTF8.GetBytes(plainText);
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, des.CreateEncryptor(EncryptKey, IV), CryptoStreamMode.Write);
            cStream.Write(inputByte, 0, inputByte.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }

        public static string GetEncryptedData(object routeValues)
        {
            string queryString = string.Empty;
            if (routeValues != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(routeValues);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    queryString += d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }

            return HttpUtility.UrlEncode(Encrypt(queryString));
        }

        public static string ExternalLink(this UrlHelper helper, string uri)
        {
            if (uri.StartsWith("http://")) return uri;
            return string.Format("http://{0}", uri);
        }

    }

}