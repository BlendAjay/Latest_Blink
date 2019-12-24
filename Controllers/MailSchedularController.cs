using AJSolutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AJSolutions.Models;
using AJSolutions.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Quartz;
using Quartz.Impl;
using SendGrid;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using SparkPost;
using System.Text;

namespace AJSolutions.Controllers
{
    public class MailSchedularController : Controller
    {
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

        // GET: AdminSchedular
        public ActionResult Index()
        {

            return View();
        }


        public static void SendEmail(string email, string Subject, string MsgBody)
        {

            try
            {
                var transmission = new Transmission();
                transmission.Content.From.Email = ConfigurationManager.AppSettings["From_Email"].ToString();
                transmission.Content.Subject = Subject;
                transmission.Content.Text = MsgBody;
                transmission.Content.Html = MsgBody;
                transmission.Content.Html = MsgBody;
                var recipient = new Recipient
                {
                    Address = new SparkPost.Address { Email = email }
                };
                transmission.Recipients.Add(recipient);

                var client = new Client(ConfigurationManager.AppSettings["Spark_Post_API"].ToString());
                client.ApiKey = ConfigurationManager.AppSettings["Spark_Post_API"].ToString();
                client.CustomSettings.SendingMode = SendingModes.Sync;

                var response = client.Transmissions.Send(transmission);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        //public  void SendApplicationLeaveEmail(string UserId, string UserName, string Employeename, string Reason, string EngagementType, DateTime? From, DateTime? To)
        //{

        //    var msgBody = "Hello  " + UserName + ", <br/> <br/>" +
        //        " You received Leave application from " + Employeename + "  ." +
        //        "<br/><br/><b>Reason:</b> " + Reason +
        //        "<br/><br/><b>Dates:</b> " + From + "-" + To +
        //         "<br/><br/>Thanks & Regards" +
        //        "<br/>Blink";

        //     UserManager.SendEmail(UserId, EngagementType, msgBody);

        //}

        public static void Start()
        {
            try
            {
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                IScheduler scheduler = schedulerFactory.GetScheduler();
                scheduler.Start();

                IJobDetail TotalJeProfileFetch = JobBuilder.Create<MailSchedular>()
                    .WithIdentity("TotalJeProfileFetch")
                    .Build();
                ITrigger trigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithCronSchedule("0 20 11 ? * *")
                    .Build();
                scheduler.ScheduleJob(TotalJeProfileFetch, trigger);


                IJobDetail WeeklyRevenue = JobBuilder.Create<WeeklyRevenueSchedular>()
                   .WithIdentity("WeeklyRevenue")
                   .Build();
                ITrigger WeeklyRevenueTrigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithCronSchedule("0 20 11 ? * *")
                    .Build();
                scheduler.ScheduleJob(WeeklyRevenue, WeeklyRevenueTrigger);

                //IJobDetail LeaveReminders = JobBuilder.Create<LeaveReminder>()
                //    .WithIdentity("LeaveRemind")
                //    .Build();

                //ITrigger LeaveReminderTrigger = TriggerBuilder.Create()
                //   .WithDailyTimeIntervalSchedule
                //      (s =>
                //         s.WithIntervalInHours(24)
                //        .OnEveryDay()
                //        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(04, 00))
                //      )
                //    .Build();

                //scheduler.ScheduleJob(LeaveReminders, LeaveReminderTrigger);

            }
            catch (Exception ex)
            {
                SendEmail(Global.AdminEmail(), "Error Log", ex.ToString());
            }
        }

    }
    internal class SatellitePaymentGenerationJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("test");
        }
    }
}