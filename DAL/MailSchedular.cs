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
    public class MailSchedular : IJob
    {
        Generic generic = new Generic();
        Student CandidateManger = new Student();

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

                string subject = "Blink Weekly Status of Total JE Profile Fetching";
                MailSchedularController.SendEmail(Email, subject, msgBody);
                Thread.Sleep(5000);
            }

        }


        public string Message()
        {
            var query = generic.GetSubscriberWiseList("560f5938-5cd6-4f45-8100-c599ae51c348");
            //var query = generic.GetSubscriberWiseList("3f98925a-ee50-4a4a-83e6-676fcf8b5173");
            query = query.Where(c => c.DepartmentId == "SRQ").ToList();

            var CandidateList = CandidateManger.GetSubscriberWiseCandidateList("560f5938-5cd6-4f45-8100-c599ae51c348");
            //var CandidateList = CandidateManger.GetSubscriberWiseCandidateList("3f98925a-ee50-4a4a-83e6-676fcf8b5173");
            CandidateList = CandidateList.Where(c => c.ReferenceId != null).ToList();

            //var msgBody = "Hi ";
            var msgBody = "Hi " + "Nibf" + ", <br/> <br/>" +
                " Blink Weekly Status Report as on " + DateTime.Now.Date +
                "<br/><br/> Total JE Profile fetches " + CandidateList.Count() + "<br/><br/>" +
                "<div><table width='600' border='0' align='center' cellpadding='0' cellspacing='0'>" +
                "<tr><th>Candidate Name</th>" +
                    "<th>Total Joined</th></tr>";
            var tr = "";
            foreach (var item in query)
            {
                tr = tr + "<tr><td style='font:bold 12px Arial,Helvetica,sans-serif;color:#fff;background:#006a80;border:solid 1px #006a80;border-radius:1px;color:#062937;padding:-20px'>" +
                    "<label>" + item.Name + " - " + item.EmployeeId + "</label>" +
                    "</td><td style='font:bold 12px Arial,Helvetica,sans-serif;color:#fff;background:#006a80;border:solid 1px #006a80;border-radius:1px;color:#062937;padding:-20px'>" + CandidateList.Where(c => c.ReferenceId == item.EmployeeId).Count() + "</td></tr>";
            }
            msgBody = msgBody + tr + "</table></div>";

            return msgBody;
        }

    }

    //public class LeaveReminder : IJob
    //{
    //    Generic generic = new Generic();
    //    MailSchedularController msc = new MailSchedularController();
    //    public void Execute(IJobExecutionContext  context)
    //    {
    //        using ( UserDBContext udb = new UserDBContext())
    //        {
    //            var LeavePending = udb.TrainerPlanner.Where(t => t.FromDate.Date == DateTime.Now.AddDays(1).Date && t.IsApproved == 0 ).ToList();
    //            if (LeavePending.Count() > 0)
    //            {
    //                foreach (var leave in LeavePending)
    //                {
    //                    EngagementTypeMaster EngagementType = udb.EngagementTypeMaster.Find(leave.EngagementTypeId);
    //                    UserViewModel userDetail = generic.GetUserDetail(leave.TrainerId);
    //                    msc.SendApplicationLeaveEmail(userDetail.ReportingAuthority, userDetail.ReportingAuthorityname, userDetail.Name, leave.Remarks, EngagementType.EngagementType, leave.FromDate, leave.ToDate);
    //                }
    //            }
    //        }
    //    }
    //}
}