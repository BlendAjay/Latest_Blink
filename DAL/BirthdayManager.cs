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
namespace AJSolutions.DAL
{
    public class BirthdayManager : IJob
    {
        UserDBContext userContext = new UserDBContext();
        Generic generic = new Generic();
        AdminManager admin = new AdminManager();

        //Createdby Ajay Kumar Choudhary Creatde on :- 18-05-2017
        // Reason:- For sending Birhtday Mails
        public void Execute(IJobExecutionContext context123)
        {
            var birthday = userContext.EmployeeBasicDetails.Where(a => a.DOB.Value.Day == DateTime.UtcNow.Day && a.DOB.Value.Month == DateTime.UtcNow.Month).ToList();
            if (birthday != null)
            {
                foreach (var bd in birthday)
                {
                    string MessageBody = "NIBF team wishing you many many happy birthday.";
                    string Name = bd.Name;
                    string msgbody = generic.EmailFormat(MessageBody, "", "", "Hi", Name, "Compulsary", "http://jedev.azurewebsites.net/img/birthday.jpg");
                    string subject = "Happy Birthday " + Name;

                    string Email = admin.GetUserDetails(bd.UserId).FirstOrDefault().Email;
                    Global.SendEmail(Email, subject, msgbody);
                }
            }
        }
    }
}