using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using UnverifiedBlast.Model;
using System.IO;
using UnverifiedBlast.DataAccess;

namespace UnverifiedBlast.Helper
{
    public class SendEmail
    {
        EmailBlastDAL DAL = new EmailBlastDAL();
        public string SendEmailToCustomer(EmailSetting modeldata)
        {
            try
            {
                // If available any tag in Email Body then please replace here after send email
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(modeldata.FromEmail, modeldata.ToEmail);
                message.Subject = modeldata.Subject;
                message.Body = modeldata.BodyHTML;
                message.IsBodyHtml = true;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                System.Net.NetworkCredential AuthenticationInfo = new System.Net.NetworkCredential(modeldata.FromEmail, modeldata.Password);
                smtp.Host = modeldata.ServerHost;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = AuthenticationInfo;
                smtp.Send(message);

                #region Add email history
                DAL.SentEmailHistory(new EmailHistoryModel()
                {
                    WarmingEmailListId = modeldata.WarmingEmailListId,
                    LaunderingEmailTemplateMasterId = modeldata.LaunderingEmailTemplateMasterId,
                    LaunderingBulkScheduleEmailsId = modeldata.LaunderingBulkScheduleEmailsId,
                    FromEmail = modeldata.FromEmail,
                    ToEmail = modeldata.ToEmail,
                    ServerHost = modeldata.ServerHost,
                    Type = "sent"
                });
                #endregion

                return "success";

            }
            catch (Exception ex)
            {
                #region Send Error
                try
                {

                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage("bsherp@physiciansalert.net", "bsherp@alertservicescorp.com");
                    message.Subject = "Unverified E-mail Blast Failure for sendPhysicianAlert_NetBlast";
                    string strBody = "";
                    strBody = strBody + "CandidateBlastListId: " + modeldata.WarmingEmailListId + Environment.NewLine;
                    strBody = strBody + "E-mail: " + modeldata.ToEmail + Environment.NewLine;
                    strBody = strBody + "Error Report: " + ex.ToString() + Environment.NewLine;
                    message.Body = (strBody);
                    message.IsBodyHtml = false;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    System.Net.NetworkCredential AuthenticationInfo = new System.Net.NetworkCredential("bsherp@physiciansalert.net", "zAsu5hut");
                    smtp.Host = "smtp.physiciansalert.net";
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = AuthenticationInfo;
                    smtp.Send(message);
                }
                catch (Exception)
                {
                }
                #endregion

                #region Add email history
                DAL.SentEmailHistory(new EmailHistoryModel()
                {
                    WarmingEmailListId = modeldata.WarmingEmailListId,
                    LaunderingEmailTemplateMasterId = modeldata.LaunderingEmailTemplateMasterId,
                    LaunderingBulkScheduleEmailsId = modeldata.LaunderingBulkScheduleEmailsId,
                    FromEmail = modeldata.FromEmail,
                    ToEmail = modeldata.ToEmail,
                    ServerHost = modeldata.ServerHost,
                    ErrorMessage = "Message: " + ex.Message + " InnerException: " + ex.InnerException.Message,
                    Type = "fail"
                });
                #endregion
                return ex.InnerException.Message;
            }
        }


    }

    public class Tags
    {
        public const string firstname = "{firstname}";
        public const string lastname = "{lastname}";
        public const string email = "{email}";
        public const string url = "{url}";
        public const string dynamicblog = "{dynamicblog}";
        public const string hldeblid = "{hldeblid}";
    }
}
