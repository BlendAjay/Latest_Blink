using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.Models;
using AJSolutions.DAL;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Data.Entity;
using AJSolutions;

namespace AJSolutions.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        UserDBContext db = new UserDBContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

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
        UserDBContext udbc = new UserDBContext();
        AdminManager admin = new AdminManager();
        Generic generic = new Generic();
        CMSManager cmsmgr = new CMSManager();
        EMSManager ems = new EMSManager();
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl, string Error)
        {

            //if (User != null)
            //{
            //    if (User.Identity.GetUserId() != null)
            //    {
            //        AuthenticationManager.SignOut();
            //        var error = "Logged Off Please try again";
            //        return RedirectToAction("Login", new { Error = error });
            //        //var userExist = UserManager.FindById(User.Identity.GetUserId());
            //        //if (userExist == null)
            //        //{
            //        //    AuthenticationManager.SignOut();
            //        //    var error = "Please try again";
            //        //    return RedirectToAction("Login", new { Error = error });
            //        //}

            //        //var roles = UserManager.GetRoles(User.Identity.GetUserId());
            //        //var roleId = roles.FirstOrDefault();

            //        //var moduleAccess = admin.GetUserModuleAccess(User.Identity.GetUserId(), DateTime.UtcNow, roleId);

            //        //if (moduleAccess != null)
            //        //{
            //        //    if (moduleAccess.ModuleId.ToUpper() == "CMS")
            //        //    {
            //        //        if (Url.IsLocalUrl(ReturnUrl))
            //        //        {
            //        //            return Redirect(ReturnUrl);
            //        //        }
            //        //        if (roleId.ToUpper() == "ADMIN")
            //        //            return RedirectToAction("Index", "Dashboard", new { area = "CMS" });
            //        //        else if (roleId.ToUpper() == "CLIENT")
            //        //            return RedirectToAction("Client", "Dashboard", new { area = "CMS" });
            //        //        else
            //        //            return View();
            //        //    }
            //        //    else
            //        //        return View();

            //        //}
            //        //else
            //        //    return View();

            //    }
            //}
            ViewBag.ReturnUrl = ReturnUrl;
            if (Error != null)
                ModelState.AddModelError("", Error);
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string ReturnUrl, string PlanType, string TMS)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            LoginViewModel login = null;
            if (!String.IsNullOrEmpty(model.UserName))
            {
                login = admin.GetLoginDetails(model.UserName);
            }

            if (login == null)
            {
                ModelState.AddModelError("", "User Not Found.");
                return View(model);
            }
            ViewBag.PlanType = PlanType;

            ViewBag.TMS = TMS;
            model.UserName = login.UserName;
            model.Email = login.Email;
            //model.PhoneNumber = login.PhoneNumber;
            var user = UserManager.Find(model.UserName, model.Password);

            if (user != null)
            {
                var roles = UserManager.GetRoles(user.Id);
                var roleId = roles.FirstOrDefault();
                if (admin.IsUserDeactivated(user.Id, roleId))
                {
                    ModelState.AddModelError("", "User Not Found.");
                    return View(model);
                }
                //for calling every day employeeconfirmationdate in procedure 
                if (roleId == "Employee" || roleId == "Admin")
                {
                    UserViewModel userDetails = generic.GetUserDetail(user.Id);
                    var logcheck = db.SubscriberLogInHistory.Where(c => c.SubscriberId == userDetails.SubscriberId).FirstOrDefault();
                    if (logcheck == null)
                    {
                        generic.AddSubscriberHitory(userDetails.SubscriberId, DateTime.UtcNow);
                        admin.AddConfirmationNotification();
                    }
                    else
                    {

                        if (logcheck != null)
                        {
                            var Logindate = logcheck.LastLogIn.Value.Date;
                            if (Logindate != DateTime.Now.Date)
                            {
                                admin.AddConfirmationNotification();
                            }
                        }
                    }
                    generic.AddSubscriberHitory(userDetails.SubscriberId, DateTime.UtcNow);

                }
                generic.UpdateUserHistoryLastLoggedOn(user.Id);
            }


            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            //if (PlanType != null)
            //{
            //    return RedirectToAction("CheckOut", "Plan_Pricing", new { area = "Admin", PlanType = PlanType, TMS = TMS });
            //}
            switch (result)
            {
                case SignInStatus.Success:

                    return RedirectToLocal(ReturnUrl, user.Id);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = ReturnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                // return RedirectToLocal(model.ReturnUrl); Open if required block by vishnu
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register

        [HttpGet]
        [AllowAnonymous]
        [EncryptedActionParameter]
        public ActionResult Register(string ModuleName = "CMS", string Status = null)
        {
            ViewBag.Status = Status;
            TempData["ModuleName"] = ModuleName;

            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = new IdentityResult();
                var password = Encrypt(model.Password);
                //var mypass = Decrypt(password);
                LoginViewModel loginEm = admin.GetLoginDetails(model.Email);
                if (loginEm != null)
                {
                    ModelState.AddModelError("", "Email '" + model.Email + "' is already taken.");
                }

                LoginViewModel loginPh = admin.GetLoginDetails(model.PhoneNumber);
                if (loginPh != null)
                {
                    ModelState.AddModelError("", "Phone Number '" + model.PhoneNumber + "' is already taken.");
                }

                if (loginEm == null && loginPh == null)
                {
                    string ModuleName = String.IsNullOrEmpty(Convert.ToString(TempData["ModuleName"])) ? "CMS" : Convert.ToString(TempData["ModuleName"]);
                    model.UserName = admin.GenerateUserName();
                    var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, PhoneNumber = model.PhoneNumber };
                    result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        var status = UserManager.AddToRole(user.Id, "Admin");

                        if (status.Succeeded)
                        {

                            model.UpdatedBy = Convert.ToString(User.Identity.GetUserId());
                            model.UpdatedOn = DateTime.UtcNow;
                            model.Pcode = password;
                            admin.UserRegistration(user.Id, model.Name, DateTime.UtcNow, ModuleName, "ADI", "Admin", user.Id, model.ManagerLevel, model.ReportingAuthority, model.UpdatedOn, model.UpdatedBy, model.RegistrationId, model.Pcode, model.Source);
                            bool i = admin.AddJobOrderType(null, user.Id);

                            //Adding Item Type Maters For Joborders & Tasks 
                            admin.AddItemTypeMaster(0, null, user.Id);

                            //Adding By Default Engagement type masters 
                            var SchemeMasters = db.LeaveSchemeMaster.ToList();
                            {
                                foreach (var item in SchemeMasters)
                                {
                                    var defaultEngagements = db.LeaveType.ToList();
                                    foreach (var qitem in defaultEngagements)
                                    {
                                        Int64 EngagementTypeId = 0;
                                        ems.AddEngagementTypeMaster(EngagementTypeId, qitem.LeaveTypeName, user.Id, qitem.LeaveTypeId, item.SchemeId, qitem.LeaveTypeId, qitem.LeaveTypeCategory, 0, DateTime.Now, null, 0);
                                    }
                                }
                            }
                            //LMS (WIKIPIAN) Plan must be checked here

                            //await RegisterToLMS(model, user.Id);

                            //await RegisterToPrelore(model, user.Id);


                        }
                        //IMP
                        //string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Welcome to Blink", model.UserName, model.PhoneNumber, model.Name);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                        return RedirectToAction("Index", "Dashboard", new { area = "CMS" });
                        //return RedirectToAction("Plan", "Plan_Pricing", new { area = "Admin" });
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult PaymentConfirmation()
        {
            string Result = string.Empty;
            var strPGResponse = Convert.ToString(System.Web.HttpContext.Current.Request["msg"]);
            if (strPGResponse != "" || strPGResponse != null)
            {
                Generic generic = new Generic();
                Result = admin.ConfirmTransaction(strPGResponse);
                string[] status = Result.Split('|');
                string ReferenceId = status[0];
                string transactionstatus = status[1];
                string txn_msg = status[2];
                ViewBag.Result = transactionstatus;
                if (!string.IsNullOrEmpty(ReferenceId) && txn_msg != "txn_msg=failure")
                {
                    var userDetails = udbc.PaymentTransaction.Where(c => c.ReferenceId == ReferenceId).FirstOrDefault();
                    string message1 = "Thank you for payment. Your receipt no: " + userDetails.ClientTxnRefNo + " and " + " reference id: " + userDetails.TSPLTxnId; //eg "message hello ";                
                    // generic.sendSMSMessage(message1, userDetails.PayeePhoneNumber);
                    generic.sendSMS(message1, userDetails.PayeePhoneNumber);
                    return View(userDetails);
                }
                else
                {
                    ViewBag.Message = "Transaction Failed";
                }
            }
            return View();

            ////if (PMId == 0)
            ////{
            //    string Result = string.Empty;

            //    var strPGResponse = Convert.ToString(System.Web.HttpContext.Current.Request["msg"]);
            //    if (strPGResponse != "" || strPGResponse != null)
            //    {
            //        Generic generic = new Generic();
            //        Result = generic.ConfirmTransaction(strPGResponse);

            //        string[] status = Result.Split('|');
            //        string clientRfNo = status[0];
            //        string transactionstatus = status[1];

            //        ViewBag.Result = transactionstatus;
            //        if (!string.IsNullOrEmpty(clientRfNo))
            //        {
            //            var userPaymentTransaction = udbc.PaymentTransaction.Find(clientRfNo);
            //            return View(userPaymentTransaction);
            //        }
            //    }
            ////}
            ////else
            ////{
            ////    if (!string.IsNullOrEmpty(TId))
            ////    {
            ////        var userPaymentTransaction = udbc.PaymentTransaction.Find(TId);
            ////        return View(userPaymentTransaction);
            ////    }

            ////}
            //return View();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            //var result = await UserManager.ConfirmEmailAsync(userId, code);
            //return View(result.Succeeded ? "ConfirmEmail" : "Error");

            var roles = UserManager.GetRoles(userId);
            var roleId = roles.FirstOrDefault();

            var verified = UserManager.IsEmailConfirmed(userId);
            if (verified == true)
            {
                return RedirectToAction("Login");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
                return RedirectToAction("Login");
            else
                return View("Error");
        }

        private async Task<bool> RegisterToLMS(RegisterViewModel model, string UserId)
        {
            bool result = false;
            try
            {
                UserRegView userReg = new UserRegView();
                userReg.UserId = UserId;
                userReg.UserName = model.UserName;
                userReg.UserRole = "Admin";
                userReg.Name = model.Name;
                userReg.Email = model.Email;
                userReg.MobileNumber = model.PhoneNumber;
                userReg.Redirectionurl = "Blink.com";
                userReg.SubscriberId = UserId;
                userReg.Password = model.Password;

                string apiUrl = Global.WikipianUrl() + "Api/Value/PostUserReg";
                HttpResponseMessage responsePostMethod = new HttpResponseMessage();
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    responsePostMethod = await client.PostAsJsonAsync(apiUrl, userReg);
                    if (responsePostMethod.IsSuccessStatusCode)
                    {
                        result = true;
                    }
                }
            }
            catch
            {

            }
            return result;
        }

        public async Task<bool> RegisterToPrelore(RegisterViewModel model, string SubscriberId)
        {
            bool result = false;
            try
            {
                string apiUrl = Global.PreloreUrl() + "Api/Value/PostRegisterClient";

                PreloreAdminView adminProfile = new PreloreAdminView();
                adminProfile.SubscriberId = SubscriberId;
                adminProfile.Name = model.Name;
                adminProfile.MobileNumber = model.PhoneNumber;
                adminProfile.Email = model.Email;
                adminProfile.Password = model.Password;


                HttpResponseMessage responsePostMethod = new HttpResponseMessage();
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    responsePostMethod = await client.PostAsJsonAsync(apiUrl, adminProfile);
                }
                return result;
            }
            catch
            {
            }
            return result;
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = UserManager.FindByEmail(model.UserName);
                LoginViewModel user = null;
                if (!String.IsNullOrEmpty(model.UserName))
                {
                    user = admin.GetLoginDetails(model.UserName);
                }

                if (user == null)
                {
                    ModelState.AddModelError("", "User Not Found.");
                }
                else
                {
                    var userDet = UserManager.FindByName(user.UserName);

                    if (!String.IsNullOrEmpty(userDet.Email))
                    {
                        string code = await UserManager.GeneratePasswordResetTokenAsync(userDet.Id);
                        var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = userDet.Id, code = code }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(userDet.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        return RedirectToAction("ForgotPasswordConfirmation", "Account");
                    }
                    else
                    {
                        //Email the reset request to Admin
                        UserProfile prof = udbc.UserProfile.Where(p => p.UserId == userDet.Id).FirstOrDefault();
                        string adminId = prof.SubscriberId;
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(adminId);
                        CorporateProfile subscriber = udbc.CorporateProfile.Where(s => s.CorporateId == adminId).FirstOrDefault();
                        var msgBody = "Dear " + subscriber.Name + ", <br/> <br/> Greetings from Blink! <br/> <br/> USER '" + prof.Name + "' with User Name '" + userDet.UserName + "' has requested for password reset. Please reset the password and share with the user." +
                           "<br><br>Blink ";
                        await UserManager.SendEmailAsync(adminId, "Reset Password for USER " + userDet.UserName, msgBody);
                        ModelState.AddModelError("", "No valid Email found for this registered user. Request email sent to your Subscriber/Corporate.");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            //if (result.Succeeded)
            //{
            //    UserViewModel userdetails = generic.GetUserDetail(user.Id);
            //    if (userdetails.Role == "Admin")
            //    {
            //        CorporateProfile corporateProfile = db.CorporateProfile.Find(user.Id);
            //        var password = Encrypt(model.Password);
            //        if (corporateProfile != null)
            //        {
            //            corporateProfile.Pcode = password;
            //            db.Entry(corporateProfile).State = EntityState.Modified;
            //            db.SaveChanges();
            //        }
            //    }
            //    return RedirectToAction("ResetPasswordConfirmation", "Account");
            //}
            ViewBag.Status = "True";
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            ViewBag.Status = "True";
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                //    return RedirectToLocal(returnUrl); Open if rquired blocked by vishnu
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = admin.GenerateUserName(), Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        //    return RedirectToLocal(returnUrl); open if required blocked by vishnu
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home", new { arera = "" });
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        public string Encrypt(string str)
        {
            string EncrptKey = "2013;[pnuLIT)WebCodeExpert";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string str)
        {
            str = str.Replace(" ", "+");
            string DecryptKey = "2013;[pnuLIT)WebCodeExpert";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] inputByteArray = new byte[str.Length];

            byKey = System.Text.Encoding.UTF8.GetBytes(DecryptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(str.Replace(" ", "+"));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        private ActionResult RedirectToLocal(string returnUrl, string userId)
        {
            //if (Url.IsLocalUrl(returnUrl))
            //{
            //    return Redirect(returnUrl);
            //}

            var roles = UserManager.GetRoles(userId);
            var roleId = roles.FirstOrDefault();

            if (roleId.ToUpper() == "CANDIDATE")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Candidate" });
            }
            else if (roleId.ToUpper() == "EMPLOYEE")
            {
                return RedirectToAction("MyTraining", "TMS", new { area = "TMS", TId = userId });
                //return RedirectToAction("Index", "Dashboard", new { area = "EMS" });
            }
            else if (roleId.ToUpper() == "ADMINISTRATOR")
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }

            var moduleAccess = admin.GetUserModuleAccess(userId, DateTime.UtcNow, roleId);

            if (moduleAccess != null)
            {
                if (moduleAccess.ModuleId.ToUpper() == "CMS")
                {
                    if (roleId.ToUpper() == "ADMIN")
                        return RedirectToAction("Index", "Dashboard", new { area = "CMS" });
                    else if (roleId.ToUpper() == "CLIENT")
                    {
                        if (cmsmgr.GetCorporateProfile(userId).FirstOrDefault().DepartmentId.ToUpper() == "CLI")
                            return RedirectToAction("Client", "Dashboard", new { area = "CMS" });
                        else
                            return RedirectToAction("Partner", "Dashboard", new { area = "CMS" });
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction("Index", "Home");

            }
            else if (moduleAccess == null && roleId.ToUpper() == "CLIENT")
                return RedirectToAction("Client", "Dashboard", new { area = "CMS" });
            else
                return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject, string userName, string phoneNumber, string Name = "User")
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            var msgBody = "Dear " + Name + " <br/> <br/> Welcome to Blink! Your User Name is " + userName + " and Phone Number is " + phoneNumber + ". <br> Before you get started, please  <a href='" + callbackUrl + "' > CLICK HERE </a> to verify this email address." +
               "<br><br>Blink ";
            //"<br/> <a href='" + callbackUrl + "' > Verify Now </a>";
            //   "<br/> Token will be valid for 48 hours. To regenerate token go to" + " <a href='http://www.jobenablers.com' target='_blank'>Login</a>" + " and put your credentials then it will regenerate your token.";

            //  msgBody = generic.AllEmailFormat(msgBody, callbackUrl, "Verify Now", "Dear", Name, "Compulsary", "Failure to verify your account within 15 days may lead to removal of your registration from our database.", "");

            await UserManager.SendEmailAsync(userID, subject, msgBody);

            return callbackUrl;
        }
        #endregion
    }
}