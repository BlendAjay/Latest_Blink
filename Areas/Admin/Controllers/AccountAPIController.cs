using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using AJSolutions.Models;
using AJSolutions.DAL;
using AJSolutions.Result;
using Newtonsoft.Json;
using AJSolutions.Areas.Admin.Models;
using System.Globalization;

namespace AJSolutions.Areas.Admin.Controllers
{
    [System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/Account")]
    public class AccountAPIController : ApiController
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        UserDBContext db = new UserDBContext();
        Generic generic = new Generic();
        PMSManager pms = new PMSManager();
        AdminManager admin = new AdminManager();
        TMSManager tms = new TMSManager();
        EMSManager emp = new EMSManager();

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
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
                return _userManager ?? System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: api/AccountAPI
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/AccountAPI/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/AccountAPI
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("GetUserDetails")]
        public IHttpActionResult Post(UserAttendanceViewModel login)
        {
            LogInData logR = new LogInData();

            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            DateTime date = DateTime.Now.Date;
            ApplicationUser userExists = UserManager.FindByEmail(login.Email);
            
            if (userExists != null)
            {
                var user = UserManager.Find(userExists.UserName, login.Password);
                var userDetail = generic.GetUserDetail(user.Id); 
                if (user != null)
                {
                    string name = null;
                    if (userDetail.Role == "Admin")
                    {
                        name = db.CorporateProfile.Where(c => c.CorporateId == user.Id).FirstOrDefault().Name;
                    }
                    else
                    {
                        name = db.EmployeeBasicDetails.Where(e => e.UserId == user.Id).FirstOrDefault().Name;
                    } 
                    var deviceinfo = db.DeviceDetail.Where(c => c.UserId == user.Id).FirstOrDefault();
                    if (deviceinfo == null)
                    {
                        pms.AddDeviceInfromation(user.Id, login.AndroidId, login.AndroidDeviceName);
                        var getcheckindetails = db.BiometricCheckInCheckOut.Where(c => c.UserId == user.Id && c.CheckInDate == date).FirstOrDefault();

                        if (getcheckindetails != null)
                        {
                            var getcheckoutdetails = db.BiometricCheckInCheckOut.Where(c => c.UserId == user.Id && c.CheckOutDate == date).FirstOrDefault();
                            if (getcheckoutdetails != null)
                            {
                                logR.CheckedStatus = 2;
                                logR.CheckOutTime = getcheckindetails.CheckOutTime;
                                logR.CurrentDate = indianTime.Date;
                                logR.ServerTime = indianTime.TimeOfDay;
                                logR.Status = 2;
                                logR.Message = "LogIn Successful";
                                logR.BearerCode = logintoken(login.Email, login.Password);
                                logR.UserName = name;
                                logR.Phonenumber = user.PhoneNumber;
                                logR.UserId = user.Id;
                            }

                            else
                            {
                                logR.CheckedStatus = 1;
                                logR.CheckInTime = getcheckindetails.CheckInTime;
                                logR.CurrentDate = indianTime.Date;
                                logR.ServerTime = indianTime.TimeOfDay;
                                logR.Status = 2;
                                logR.Message = "LogIn Successful";
                                logR.BearerCode = logintoken(login.Email, login.Password);
                                logR.UserName = name;
                                logR.Phonenumber = user.PhoneNumber;
                                logR.UserId = user.Id;
                            }
                        }
                        else
                        {
                            logR.CheckedStatus = 0;
                            logR.CheckInTime = getcheckindetails.CheckInTime;
                            logR.CurrentDate = indianTime.Date;
                            logR.ServerTime = indianTime.TimeOfDay;
                            logR.Status = 2;
                            logR.Message = "LogIn Successful";
                            logR.BearerCode = logintoken(login.Email, login.Password);
                            logR.UserName = name; // user.UserName;
                            logR.Phonenumber = user.PhoneNumber;
                            logR.UserId = user.Id;
                        }
                    }
                    else
                    {
                        if (deviceinfo.DeviceId == login.AndroidId)
                        {
                            var getcheckindetails = db.BiometricCheckInCheckOut.Where(c => c.UserId == user.Id && c.CheckInDate == date).FirstOrDefault();
                            if (getcheckindetails != null)
                            {
                                var getcheckoutdetails = db.BiometricCheckInCheckOut.Where(c => c.UserId == user.Id && c.CheckOutDate == date).FirstOrDefault();
                                if (getcheckoutdetails != null)
                                {
                                    logR.CheckedStatus = 2;
                                    logR.CheckOutTime = getcheckindetails.CheckOutTime;
                                    logR.CurrentDate = indianTime.Date;
                                    logR.ServerTime = indianTime.TimeOfDay;
                                    logR.Status = 2;
                                    logR.Message = "LogIn Successful";
                                    logR.BearerCode = logintoken(login.Email, login.Password);
                                    logR.UserName = name;
                                    logR.Phonenumber = user.PhoneNumber;
                                    logR.UserId = user.Id;
                                }

                                else
                                {
                                    logR.CheckedStatus = 1;
                                    logR.CheckInTime = getcheckindetails.CheckInTime;
                                    logR.CurrentDate = indianTime.Date;
                                    logR.ServerTime = indianTime.TimeOfDay;
                                    logR.Status = 2;
                                    logR.Message = "LogIn Successful";
                                    logR.BearerCode = logintoken(login.Email, login.Password);
                                    logR.UserName = name;
                                    logR.Phonenumber = user.PhoneNumber;
                                    logR.UserId = user.Id;
                                }
                            }
                            else
                            {
                                logR.CheckedStatus = 0;
                                //logR.CheckInTime = getcheckindetails.CheckInTime;
                                logR.CurrentDate = indianTime.Date;
                                logR.ServerTime = indianTime.TimeOfDay;
                                logR.Status = 2;
                                logR.Message = "LogIn Successful";
                                logR.BearerCode = logintoken(login.Email, login.Password);
                                logR.UserName = name;
                                logR.Phonenumber = user.PhoneNumber;
                                logR.UserId = user.Id;
                            }

                        }

                        else
                        {
                            logR.Status = 3;
                            logR.Message = "Device is not registered";
                            logR.DeviceName = deviceinfo.DeviceName;
                            logR.UserId = user.Id;
                        }

                    }
                    //for valid login

                }
                else
                {
                    // for invalid password
                    logR.Status = 1;
                    logR.Message = "Invalid Password";
                }
            }
            else
            {
                logR.Status = 0; //Invalid Email or User doesn't exists
                logR.Message = "Invalid Email";
            }
            return new LogInResult(logR, Request);
        }


        // POST: api/AccountAPI
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("CheckInDetail")]
        public IHttpActionResult PostCheckinDetail(UserAttendanceViewModel login)
        {
            LogInData logR = new LogInData();
            var userdetails = generic.GetUserDetail(login.UserId);
            var Ipdata = db.IpMasters.Where(c => c.UserId == userdetails.SubscriberId).ToList();
            var finalipMatch = Ipdata.Where(c => c.LatitudeFrom <= login.Latitude && c.LatitudeTo >= login.Latitude && c.LongitudeFrom <= login.Longitude && c.LongitudeTo >= login.Longitude).FirstOrDefault();
            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

            var getcheckindetails = db.BiometricCheckInCheckOut.Where(c => c.UserId == login.UserId && c.CheckInDate == indianTime.Date).FirstOrDefault();
            if (finalipMatch != null)
            {
                if (getcheckindetails != null)
                {
                    var getcheckoutdetails = db.BiometricCheckInCheckOut.Where(c => c.UserId == login.UserId && c.CheckOutDate == indianTime.Date).FirstOrDefault();
                    if (getcheckoutdetails != null)
                    {
                        logR.CheckedStatus = 2; // checkin / checkout done
                        logR.CheckOutTime = getcheckindetails.CheckOutTime;
                        logR.CheckInTime = getcheckindetails.CheckInTime;//mj
                        logR.CurrentDate = indianTime.Date;
                        logR.ServerTime = indianTime.TimeOfDay;
                        logR.LocationStatus = 1; //Inside of premises

                    }

                    else
                    {
                        logR.CheckedStatus = 1; // Only checked in 
                        logR.CheckInTime = getcheckindetails.CheckInTime;
                        logR.CurrentDate = indianTime.Date;
                        logR.ServerTime = indianTime.TimeOfDay;
                        logR.LocationStatus = 1;//Inside of premises


                    }
                }
                else
                {
                    logR.CheckedStatus = 0; //No checkin /checkout found
                    logR.CurrentDate = indianTime.Date;
                    logR.ServerTime = indianTime.TimeOfDay;
                    logR.LocationStatus = 1;//Inside of premises

                }
            }
            else
            {
                logR.LocationStatus = 0;//outside of premises
                logR.CurrentDate = indianTime.Date;
                logR.ServerTime = indianTime.TimeOfDay;

            }
            return new LogInResult(logR, Request);
        }

        //[System.Web.Http.Authorize]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("CheckInCheckOut")]
        public IHttpActionResult PostCheckInCheckOut(UserAttendanceViewModel login)
        {
            LogInData logR = new LogInData();

            string status = "NoMatch";


            var userdetails = generic.GetUserDetail(login.UserId);

            Int64 branchId = db.EmpJoiningDetail.Where(e => e.UserId == userdetails.UserId).FirstOrDefault().BranchId;

            var Ipdata = db.IpMasters.Where(c => c.UserId == userdetails.SubscriberId && c.BranchId == branchId).ToList();

            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

            DateTime currentDate = indianTime.Date;
            TimeSpan CurrentTime = indianTime.TimeOfDay;

            if (Ipdata != null)
            {
                var branchlocation = Ipdata.FirstOrDefault();

                if (branchlocation.Authenticate == 1)
                {
                    if (login.LoggedInIp == branchlocation.IPAddressFrom || login.LoggedInIp == branchlocation.IPAddressTo)
                    {
                        status = submitAttendance(login.UserId, userdetails.SubscriberId, login.LoggedInIp, login.Latitude, login.Longitude, currentDate, CurrentTime);
                    }

                }
                else if (branchlocation.Authenticate == 2)
                {

                    if (login.Latitude >= branchlocation.LatitudeFrom && login.Latitude <= branchlocation.LatitudeTo && login.Longitude >= branchlocation.LongitudeFrom && login.Longitude <= branchlocation.LongitudeTo)
                    {

                        status = submitAttendance(login.UserId, userdetails.SubscriberId, login.LoggedInIp, login.Latitude, login.Longitude, currentDate, CurrentTime);
                    }

                }
                else if (branchlocation.Authenticate == 3)
                {
                    if ((login.LoggedInIp == branchlocation.IPAddressFrom || login.LoggedInIp == branchlocation.IPAddressTo) && (login.Latitude >= branchlocation.LatitudeFrom && login.Latitude <= branchlocation.LatitudeTo && login.Longitude >= branchlocation.LongitudeFrom && login.Longitude <= branchlocation.LongitudeTo))
                    {

                        status = submitAttendance(login.UserId, userdetails.SubscriberId, login.LoggedInIp, login.Latitude, login.Longitude, currentDate, CurrentTime);
                    }

                }

                if (status == "CheckedIn")
                {
                    logR.CheckedStatus = 1;//checked in
                    logR.CheckInTime = CurrentTime;
                    logR.CurrentDate = indianTime.Date;
                    logR.ServerTime = CurrentTime;
                    logR.LocationStatus = 1;
                }
                else
                {
                    logR.CheckedStatus = 2; //checked Out
                    logR.CheckInTime = db.BiometricCheckInCheckOut.Where(c => c.UserId == login.UserId && c.CheckInDate == indianTime.Date).FirstOrDefault().CheckInTime;
                    logR.CheckOutTime = CurrentTime;
                    logR.CurrentDate = indianTime.Date;
                    logR.ServerTime = CurrentTime;
                    logR.LocationStatus = 1;

                }

            }
            else
            {

                logR.LocationStatus = 0;
                logR.CurrentDate = indianTime.Date;
                logR.ServerTime = indianTime.TimeOfDay;

            }

            //var userdetails = generic.GetUserDetail(login.UserId);
            //var Ipdata = db.IpMasters.Where(c => c.UserId == userdetails.SubscriberId).ToList();

            //TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            //DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            //if (Ipdata != null)
            //{
            //    var finalipMatch = Ipdata.Where(c => c.LatitudeFrom <= login.Latitude && c.LatitudeTo >= login.Latitude && c.LongitudeFrom <= login.Longitude && c.LongitudeTo >= login.Longitude).FirstOrDefault();
            //    //var finalipMatch = Ipdata.Where(c => c.IPAddressFrom == IpRangeofUser || c.IPAddressTo == IpRangeofUser).FirstOrDefault();
            //    if (finalipMatch != null)
            //    {

            //        var getcheckindetails = db.BiometricCheckInCheckOut.Where(c => c.UserId == login.UserId && c.CheckInDate == indianTime.Date).FirstOrDefault();
            //        if (getcheckindetails == null)
            //        {
            //            //Default 0 fro adding new record
            //            Int64 BiometricId = 0;


            //            DateTime checkindate = indianTime.Date;
            //            TimeSpan CheckInTime = indianTime.TimeOfDay;

            //            Int64 ShiftId = 1;
            //            bool result = admin.AddEmployeeCheckinCheckout(BiometricId, login.UserId, checkindate, CheckInTime, null, null, login.LoggedInIp, userdetails.SubscriberId, ShiftId);
            //            logR.CheckedStatus = 1;//checked in
            //            logR.CheckInTime = CheckInTime;
            //            logR.CurrentDate = checkindate;
            //            logR.ServerTime = indianTime.TimeOfDay;
            //            logR.LocationStatus = 1;
            //        }
            //        else
            //        {
            //            var BiometricDetails = db.BiometricCheckInCheckOut.Where(c => c.UserId == login.UserId).ToList().OrderByDescending(d => d.CheckInDate).FirstOrDefault();
            //            //DateTime? checkoutdate = DateTime.Now.Date;
            //            //TimeSpan? CheckoutTime = DateTime.Now.TimeOfDay;
            //            DateTime checkoutdate = indianTime.Date;
            //            TimeSpan CheckoutTime = indianTime.TimeOfDay;


            //            Int64 ShiftId = 1;
            //            bool res = admin.AddEmployeeCheckinCheckout(BiometricDetails.BiometricId, login.UserId, BiometricDetails.CheckInDate, BiometricDetails.CheckInTime, checkoutdate, CheckoutTime, login.LoggedInIp, BiometricDetails.SubscriberId, ShiftId);
            //            logR.CheckedStatus = 2; //checked Out
            //            logR.CheckInTime = getcheckindetails.CheckInTime;
            //            logR.CheckOutTime = CheckoutTime;
            //            logR.CurrentDate = checkoutdate;
            //            logR.ServerTime = indianTime.TimeOfDay;
            //            logR.LocationStatus = 1;
            //        }

            //    }
            //    else
            //    {
            //        logR.LocationStatus = 0;
            //        logR.CurrentDate = indianTime.Date;
            //        logR.ServerTime = indianTime.TimeOfDay;
            //    }
            //}
            return new LogInResult(logR, Request);
        }

        public string submitAttendance(string UserId, string SubscriberId, string LoggedInIp, float Latitude, float Longitude, DateTime CurrentDate, TimeSpan CurrentTime)
        {

            Int64 ShiftId = 1;

            var getcheckindetails = db.BiometricCheckInCheckOut.Where(c => c.UserId == UserId && c.CheckInDate == CurrentDate).FirstOrDefault();


            if (getcheckindetails == null)
            {
                bool result = admin.AddEmployeeCheckinCheckout(0, UserId, CurrentDate, CurrentTime, null, null, LoggedInIp, SubscriberId, ShiftId);

                return "CheckedIn";
            }
            else
            {
                bool res = admin.AddEmployeeCheckinCheckout(getcheckindetails.BiometricId, UserId, getcheckindetails.CheckInDate, getcheckindetails.CheckInTime, CurrentDate, CurrentTime, LoggedInIp, getcheckindetails.SubscriberId, ShiftId);

                return "CheckedOut";
            }
        }

        // GET api/<controller>
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("EngagementTypeList/{UserId}")]
        public IHttpActionResult PostEngagementTypeList(string UserId)
        {

            string corporateId = emp.GetEmployeeBasicDetails(UserId).Select(e => e.SubscriberId).FirstOrDefault().ToString();
            var employeeDetails = db.EmpJoiningDetail.Where(c => c.UserId == UserId).FirstOrDefault();
            var entity = db.EngagementTypeMaster.Where(e => e.CorporateId == corporateId && e.SchemeId == employeeDetails.SchemeId).ToList();
            LeaveDetails lv = new LeaveDetails();
            lv.Leavetype = entity;
            return new LeaveResult(lv, Request);

        }
        //api method for trainnerplanner (applied leave list of employee)
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("EmployeeLeaveDetails/{UserId}")]
        public IHttpActionResult PostEmployeeLeaveDetails(string UserId)
        {

            var leavedetail = tms.GetTrainerPlaner(UserId);
            LeaveDetails lv = new LeaveDetails();
            lv.EmpLeaveDetails = leavedetail;
            return new LeaveResult(lv, Request);

        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ApplyLeave")]

        public IHttpActionResult PostApplyLeave(TrainerPlanner trainerPlan)
        {
            var myerror = new Error();

            try
            {
                //Make an entry into Trainer Planner
                string userId = trainerPlan.TrainerId;
                UserViewModel userDetail = generic.GetUserDetail(userId);

                var employeeDetails = db.EmpJoiningDetail.Where(c => c.UserId == userId).FirstOrDefault();
                if (employeeDetails != null)
                {
                    trainerPlan.SchemeId = employeeDetails.SchemeId;
                }

                bool result = tms.AddTrainerPlan(trainerPlan);


                if (result == true)
                {
                    EngagementTypeMaster EngagementType = db.EngagementTypeMaster.Find(trainerPlan.EngagementTypeId);
                    if (userDetail.Role != "Admin")
                    {
                        SendApplicationLeaveEmailToken(userDetail.ReportingAuthority, userDetail.ReportingAuthorityname, userDetail.Name, trainerPlan.Remarks, EngagementType.EngagementType, trainerPlan.FromDate, trainerPlan.ToDate);
                    }
                }

                myerror.Status = true;
                myerror.Message = "Engagement applied successfully ";
                return new ErrorResult(myerror, Request);
            }
            catch (Exception ex)
            {
                myerror.Status = true;
                myerror.Message = "Engagement applied failed " + ex;
                return new ErrorResult(myerror, Request);
            }
        }


        private void SendApplicationLeaveEmailToken(string UserId, string UserName, string Employeename, string Reason, string EngagementType, DateTime? From, DateTime? To)
        {

            var msgBody = "Hello  " + UserName + ", <br/> <br/>" +
                " You received Leave application from " + Employeename + "  ." +
                "<br/><br/><b>Reason:</b> " + Reason +
                "<br/><br/><b>Dates:</b> " + From + "-" + To +
                 "<br/><br/>Thanks & Regards" +
                "<br/>RECKONN";

            UserManager.SendEmail(UserId, EngagementType, msgBody);

        }

        // PUT: api/AccountAPI/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE: api/AccountAPI/5
        public void Delete(int id)
        {
        }
        public Dictionary<string, string> logintoken(string Email, string Password)
        {
            Dictionary<string, string> tokenDetails = null;
            var client = new HttpClient();
            var login = new Dictionary<string, string>
                   {
                       {"grant_type", "password"},
                       {"username", Email},
                       {"password", Password},
                   };

            var resp = client.PostAsync(Global.WebsiteUrl() + "token", new FormUrlEncodedContent(login));
            resp.Wait(TimeSpan.FromSeconds(10));

            if (resp.IsCompleted)
            {
                if (resp.Result.Content.ReadAsStringAsync().Result.Contains("access_token"))
                {
                    tokenDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp.Result.Content.ReadAsStringAsync().Result);

                }
            }
            return tokenDetails;
        }
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("GetAttendanceData")]
        public IHttpActionResult PostAttendance(CheckinCheckoutdataAppView chkinout)
        {
            var subscriberid = generic.GetUserDetail(chkinout.UserId).SubscriberId;
            var AttendanceData = emp.GetEmployeeAttendanceApp(chkinout.UserId, subscriberid, chkinout.Month, chkinout.Year);

            Attendance atdata = new Attendance();
            atdata.AttendanceData = AttendanceData;
            return new AttendanceResult(atdata, Request);

        }
    }
}
