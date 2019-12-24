using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AJSolutions.DAL
{
    public static class EmailFormats
    {

        public static string AllEmailFormat(string Message, string url, string action, string greeting, string userName, string NotificationType, string Note = "", string ImgUrl = "")
        {
            // userName = textInfo.ToTitleCase(userName);
            string forgotpwd = "http://www.jobenablers.com/Account/ForgotPassword";
            string fb = "https://www.facebook.com/jobenablers.in/";
            string twitter = "https://twitter.com/JOBENABLERS";
            string linkdin = "https://www.linkedin.com/company/jobenablers%E2%84%A2?trk=company_logo";
            string gooleplus = "https://plus.google.com/+Jobenablers-EF";
            string playstore = "https://play.google.com/store/apps/details?id=com.mobincube.android.sc_3W1A15";
            string unsubscribe = "http://www.jobenablers.com/EmailNotification/EmailSubscription";

            string start = "<div><br><br>" +
                      "<table width='600' border='0' align='center' cellpadding='0' cellspacing='0'>" +
                       "<tbody><tr>" +
                       "<td height='288' valign='top'><table width='100%' border='0' cellspacing='0' cellpadding='0'>" +
                        "<tbody><tr>" +
                        "<td width='79%' height='288' valign='top'>" +
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
                         greeting + " " + userName + " ,</strong><br><br><p align='justify'>" + Message + "</p>";

            string imageMsg = "<center><img src='" + ImgUrl + "' alt='img' border='0'></center>";

            string urlMsg = "<table width='19%' border='0' cellspacing='0' cellpadding='0' align='center'>" +
                            "<tbody><tr>" +
                            "<td width='19%' valign='top' style='padding-right:10px'><a href='" + url + "' target='_blank'><br /><button class='btn btn-success' style='background-color: #204c39;float:right;border: 5px solid #204c39;border-radius: 10px;padding: 5px;color: #ffffff;'>" + action + "</button></a>" +
                            "</td></tr></tbody></table><br>";
            string note = "<br><b>Note:</b>" + Note;


            string end = "<br></td></tr></tbody></table>" +
                        "<br>" +
                            "<a href='" + forgotpwd + "'style='text-decoration:none;color:red;line-height:14px;padding:0 10px' target='_blank'><strong>Forgot Password</strong></a>" +
                        "<br><br>" +
                        "<table width='100%' border='0' cellspacing='0' cellpadding='0' style='padding:-15px'>" +
                        "<tbody><tr>" +
                        "<td style='font:bold 12px Arial,Helvetica,sans-serif;color:#333;background:#225452;border:solid 1px #e1e1e1;border-radius:1px;color:#062937;padding:10px'> " +
                        "<span style='padding:-55px'> " +
                        "<span style='text-decoration:none;color:#fff;font-size:14px;vertical-align:top;line-height:35px'>Follow Us:</span>  " +
                                     "<a href='" + fb + "' target='_blank' ><img src='http://www.jobenablers.com/img/social-button/facebook.png' width='33' height='30' alt='img'></a> &nbsp;&nbsp;" +
                                     "<a href='" + twitter + "' target='_blank' ><img src='http://www.jobenablers.com/img/social-button/twitter.png' width='33'height='30' alt='img'></a>&nbsp;&nbsp;" +
                                     "<a href='" + gooleplus + "' target='_blank' ><img src='http://www.jobenablers.com/img/social-button/googleplus.png' width='33'height='30' alt='img'></a>&nbsp;&nbsp;" +
                                     "<a href='" + linkdin + "' target='_blank'><img src='http://www.jobenablers.com/img/social-button/linkedin.png' width='33' height='30' alt='img'></a>" +
                                     "<span style='text-decoration:none;color:#fff;font-size:14px;vertical-align:top;line-height:35px'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Download App:</span>&nbsp;" +
                                     "<a href='" + playstore + "' target='_blank'><img src='http://www.masonbruce.com/wp-content/uploads/2015/03/android-logo-transparent-background.png' width='30' height='30' alt='img'></a></td>" +
                        "</span>" +
                        "</td></tr></tbody></table>" +
                        "<table width='100%' border='0' align='center' cellpadding='0' cellspacing='0'>" +
                        "<tbody><tr>" +
                         "<td width='87%' height='28' valign='top' style='font:normal 11px Arial,Helvetica,sans-serif;color:#333'><br>Please add <a href='mailto:contact@jobenablers.com' target='_blank'>contact@jobenablers.com</a> to your address book or safe list to " +
                        "prevent future JOBENABLERS™ updates from being classified as Junk / Bulk Mail<br>";
            if (NotificationType != "Compulsary" && NotificationType != null)
            {
                end = end + "<br>" +
                            "<a href='" + unsubscribe + "'style='text-decoration:none;color:red;line-height:14px;padding:0 10px' target='_blank' align='right'><strong>Unsubscribe</strong></a>" +
                        "<br><br>" +
                        "</td></tr></tbody></table>" +
                        "</tr></tbody></table>" +
                        "</td></tr></tbody></table> " +
                        "</td></tr></tbody></table>";
            }
            else
            {
                end = end + "</td></tr></tbody></table>" +
                        "</tr></tbody></table>" +
                        "</td></tr></tbody></table> " +
                        "</td></tr></tbody></table>";
            }
            if (!string.IsNullOrEmpty(Note))
            {
                end = note + end;
            }

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

            Message = emailfooter(Message);

            return Message;
        }

        public static string emailfooter(string Msg)
        {
            string shawacedemy = "<br><br><center>" +
                    "<a href='https://u1669432.ct.sendgrid.net/wf/click?upn=HENOK03r3L8VkzjknITYgOXYM4d49QKUQXXQ3W6ZgT3RY0SG75TvfgI1NW7qkzUurpDu0Fc5fVmZd340Gxyv8PfHL8xOhbuUEESXH6ls5FSsqZ0iaDhokFUvf-2F00i5ByHO5uoHx9MtJPHdgHavgrNA-3D-3D_8eZFKKjQHHM6xiP-2FjTyY2iZ-2FMmvPy7QyNz-2BZk2Cl4vM5aE37uT26s1VsRaqMsXP9zlg2aKLG-2Fkz8Jav6o9F66vXQno6-2Bb84ohY0jNp-2FtKUZUCY6T3L6emG-2Fd8tHUXTRhMlQD6x-2BDbwOl4IUBCfQvG3Zi3T7oXmqMtGDWLVl04oSyjA02BbtApC3kVUYtVtO9vZhZ-2BGoPVBacfSSuwsOHWtFLnaBTnFgC6KjZEoE3OtA-3D'>" +
                    "FREE Online Training - Click here to Explore & Enroll</a></center><br>" +
                    "<center><img src='https://s3-eu-west-1.amazonaws.com/shawaftassets/img/Free-Course-CPL/468x60.png' target='_blank 'alt='img' /></center><br>";
            return (Msg + shawacedemy);

        }
    }
}