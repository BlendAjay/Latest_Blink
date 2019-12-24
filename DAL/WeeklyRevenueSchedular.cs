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
using System.Globalization;
using System.Threading;
using System;
using AJSolutions.Controllers;


namespace AJSolutions.DAL
{
    public class WeeklyRevenueSchedular : IJob
    {
        Generic generic = new Generic();
        Student CandidateManger = new Student();
        UserDBContext db = new UserDBContext();

        public void Execute(IJobExecutionContext context123)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            var Emails = generic.GetEmails();
            foreach (var email in Emails)
            {
                string Email = email.Email;

                string Name = "User";
                if (!string.IsNullOrEmpty(Email))
                    Name = textInfo.ToTitleCase(Email);

                string msgBody = Message();

                string subject = "Reckonn Total Weekly Revenue Status";
                MailSchedularController.SendEmail(Email, subject, msgBody);
                Thread.Sleep(5000);
            }

        }


        public string Message()
        {
            DateTime WeekFirstdate = DateTime.Now.Date.AddDays(0 * (Int32)DateTime.Now.DayOfWeek);
            DateTime LastWeekFirstdate = WeekFirstdate.Date.AddDays(-7 * (Int32)DateTime.Now.DayOfWeek);
            DateTime LastWeekLastdate = WeekFirstdate.Date.AddDays(-2 * (Int32)DateTime.Now.DayOfWeek);

            var TotalTransaction = CandidateManger.GetCandidatePaymentTransaction("560f5938-5cd6-4f45-8100-c599ae51c348").Where(c => c.Status == "Approved" && c.Status == "Succeeded");
            TotalTransaction = TotalTransaction.Where(c => c.PaymentDate >= WeekFirstdate && c.PaymentDate <= DateTime.Today).ToList();
            float TotalRevenue = TotalTransaction.Sum(c => c.FeePaid);
            var Courses = db.CourseMaster.Where(c => c.SubscriberId == "560f5938-5cd6-4f45-8100-c599ae51c348").ToList();

            var msgBody = "Hi " + "Nibf" + ", <br/> <br/>" +
                " Blink Weekly Status Report as on " + DateTime.Now.Date +
                "<br/><br/> <h3>Total Weekly  Revenue: " + TotalRevenue + "</h3><br/><br/>" +
                "<div><table width='600' border='0' align='center' cellpadding='0' cellspacing='0'>";
            var tr = "";
            foreach (var item in Courses)
            {
                var batches = db.CourseBatch.Where(c => c.CourseCode == item.CourseCode).ToList();
                //batches = batches.Where(c => c.FromDate >= WeekFirstdate && c.ToDate <= DateTime.Today).ToList();
                tr = tr + "<tr><th>Batch</th>" +
                    "<th>Total Revenue</th></tr>";

                foreach (var batch in batches)
                {
                    var Revenue = TotalTransaction.Where(c => c.BatchId == batch.BatchId).ToList();
                    float CurrentWeekRevenue = Revenue.Sum(c => c.FeePaid);
                    tr = tr + "<tr><td style='font:bold 12px Arial,Helvetica,sans-serif;color:#fff;background:#006a80;border:solid 1px #006a80;border-radius:1px;color:#062937;padding:-20px'>" +
                        "<label>" + batch.BatchName + " - " + batch.BatchId + "</label>" +
                        "</td><td style='font:bold 12px Arial,Helvetica,sans-serif;color:#fff;background:#006a80;border:solid 1px #006a80;border-radius:1px;color:#062937;padding:-20px'>" + @CurrentWeekRevenue + "</td></tr>";
                }
            }
            msgBody = msgBody + tr + "</table></div>";

            return msgBody;
        }

    }
}