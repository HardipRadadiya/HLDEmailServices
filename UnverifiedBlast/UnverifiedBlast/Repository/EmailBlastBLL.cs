using UnverifiedBlast.DataAccess;
using UnverifiedBlast.Helper;
using UnverifiedBlast.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;

namespace UnverifiedBlast.Repository
{
    public class EmailBlastBLL : BaseBLL
    {
        EmailBlastDAL DAL;
        SendEmail objEmail;
        public EmailBlastBLL()
        {
            DAL = new EmailBlastDAL();
            objEmail = new SendEmail();
        }
        public void SendWarmingEmails()
        {
            try
            {
                //TimeSpan start = new TimeSpan(11, 10, 0);
                //TimeSpan end = new TimeSpan(11, 20, 0);
                //bool f = TimeBetween(DateTime.Now, start, end);

                LogFile.AddMessage("Start: Unverified Blast Service on " + DateTime.Now.ToString());

                #region Get Warming email table fields
                List<FieldModel> lstField = new List<FieldModel>();
                DataTable dtfield = new DataTable();
                dtfield = DAL.GetWarmingEmailField();
                if (dtfield != null && dtfield.Rows.Count > 0)
                {
                    foreach (DataRow row in dtfield.Rows)
                    {
                        FieldModel model = new FieldModel();
                        model.Id = SafeValue<int>(row["ID"]);
                        model.FieldName = SafeValue<string>(row["FieldName"]);
                        model.FieldType = SafeValue<string>(row["FieldType"]);
                        lstField.Add(model);
                    }
                }
                #endregion

                #region Get Account details for sending email in a loop
                List<EmailAccountDetails> lstAcDetails = new List<EmailAccountDetails>();
                DataTable dtlstAcDetails = new DataTable();
                dtlstAcDetails = DAL.GetAccountDetails();
                if (dtlstAcDetails != null && dtlstAcDetails.Rows.Count > 0)
                {
                    lstAcDetails = dtlstAcDetails.ToListof<EmailAccountDetails>();
                }
                #endregion

                if (lstAcDetails.Count > 0)
                {
                    #region Get email template tag list
                    var EmailTagList = GetEmailTemplateTagMaster();
                    #endregion

                    foreach (var account in lstAcDetails)
                    {
                        try
                        {
                            List<TimeSloat> lsttimesloat = new List<TimeSloat>();

                            #region Bind Time Sloat in hours
                            //TimeSpan time = account.StartTime;
                            //for (int i = 0; i < 24; i++)
                            //{
                            //    if (time < account.EndTime)
                            //    {
                            //        string[] tm = time.ToString().Split(':');
                            //        DateTime dt = DateTime.Now.Date.AddMinutes((Convert.ToInt32(tm[0]) * 60) + (Convert.ToInt32(tm[1])));
                            //        lsttimesloat.Add(new TimeSloat()
                            //        {
                            //            StartTime = dt,
                            //            EndTime = dt.AddHours(1)
                            //        });
                            //        time = time.Add(TimeSpan.FromHours(1));
                            //        if (DateTime.Now.Date.AddHours(24) < dt.AddHours(1))
                            //            break;

                            //    }
                            //    else
                            //    {
                            //        break;
                            //    }
                            //}
                            #endregion

                            #region Bind Time Sloat in minutes
                            TimeSpan time = account.StartTime;
                            for (int i = 0; i < 500; i++)
                            {
                                if (time < account.EndTime)
                                {
                                    string[] tm = time.ToString().Split(':');
                                    DateTime dt = DateTime.Now.Date.AddMinutes((Convert.ToInt32(tm[0]) * 60) + (Convert.ToInt32(tm[1])));
                                    lsttimesloat.Add(new TimeSloat()
                                    {
                                        StartTime = dt,
                                        EndTime = dt.AddMinutes(5)
                                        //EndTime = dt.AddHours(1)
                                    });
                                    //time = time.Add(TimeSpan.FromHours(1));
                                    time = time.Add(TimeSpan.FromMinutes(5));
                                    //if (DateTime.Now.Date.AddHours(24) < dt.AddHours(1))
                                    if (DateTime.Now.Date.AddHours(24) < dt.AddMinutes(5))
                                        break;

                                }
                                else
                                {
                                    break;
                                }
                            }
                            #endregion

                            #region Get per day send email count

                            List<DateTime> lstTodaySentEmail = new List<DateTime>();
                            DataTable dtTodaySentEmail = new DataTable();
                            dtTodaySentEmail = DAL.GetTodaysSendEmail(account.EmailAccountHost);
                            if (dtTodaySentEmail != null && dtTodaySentEmail.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtTodaySentEmail.Rows)
                                {
                                    lstTodaySentEmail.Add(SafeValue<DateTime>(row["LastEmailed"]));
                                }
                            }
                            #endregion

                            #region Validation for email send limit

                            int perDaySendCount = lstTodaySentEmail.Count();

                            TimeSpan currentTime = DateTime.Now.TimeOfDay;
                            if (account.PerDayEmailSent <= perDaySendCount || (currentTime > account.StartTime && account.EndTime < currentTime))
                                continue;

                            var FindTimeSloat = lsttimesloat.Where(m => m.StartTime <= DateTime.Now && m.EndTime >= DateTime.Now).FirstOrDefault();
                            if (FindTimeSloat == null)
                                continue;

                            int hourlySentCount = lstTodaySentEmail.Where(m => m > FindTimeSloat.StartTime && m <= FindTimeSloat.EndTime).ToList().Count();
                            int topcount = account.PerHourEmailSent - hourlySentCount;
                            if (topcount <= 0)
                                continue;
                            #endregion

                            #region Get dynamic query enter by user
                            List<QueryModel> lstquery = new List<QueryModel>();
                            DataTable dtquery = new DataTable();
                            dtquery = DAL.GetDynamicQuery(account.LaunderingBulkScheduleEmailsId);
                            if (dtquery != null && dtquery.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtquery.Rows)
                                {
                                    QueryModel model = new QueryModel();
                                    model.Id = SafeValue<int>(row["Id"]);
                                    model.LaunderingBulkScheduleEmailsId = SafeValue<int>(row["LaunderingBulkScheduleEmailsId"]);
                                    model.FieldName = SafeValue<string>(row["FieldName"]);
                                    model.Condition = SafeValue<string>(row["Condition"]);
                                    model.Value = SafeValue<string>(row["Value"]);
                                    model.Operator = SafeValue<string>(row["Operator"]);
                                    lstquery.Add(model);
                                }
                            }
                            else
                            {
                                LogFile.AddMessage("**************** ERROR START **************************");
                                LogFile.AddMessage("**DYNAMIC QUERY DATA NOT FOUND IN 'LaunderingBulkScheduleEmailsFilter' TABLE, Reference ID is " + account.LaunderingBulkScheduleEmailsId + " (LaunderingBulkScheduleEmailsId) for the debuge perpose **");
                                LogFile.AddMessage("**************** ERROR END **************************");
                                continue;
                            }
                            #endregion

                            #region Set Query
                            string Query = string.Empty;
                            foreach (var item in lstquery)
                            {
                                Query += SetQuery(item, lstField);
                            }

                            // add condition of do not repeat sent email after input days
                            if (account.DoNotRepeatSentEmailDays > 0)
                                Query += " AND datediff(day,lastemailed,getdate()) > " + account.DoNotRepeatSentEmailDays + " order by LastEmailed asc";
                            else
                                Query += " order by LastEmailed asc";
                            #endregion

                            #region Get Query Result
                            List<WarmingEmailModel> lstQueryResult = new List<WarmingEmailModel>();
                            DataTable dtQueryResult = new DataTable();

                            dtQueryResult = DAL.GetDynamicQueryResult(topcount, Query);
                            if (dtQueryResult != null && dtQueryResult.Rows.Count > 0)
                            {
                                lstQueryResult = dtQueryResult.ToListof<WarmingEmailModel>();
                            }
                            else
                            {
                                // if query data not found then continue to next loop record
                                continue;
                            }
                            #endregion

                            #region Send Email
                            EmailSetting emailModel = new EmailSetting();
                            emailModel.LaunderingEmailTemplateMasterId = account.EmailTemplateMasterID;
                            emailModel.LaunderingBulkScheduleEmailsId = account.LaunderingBulkScheduleEmailsId;
                            emailModel.FromEmail = account.EmailAccountId;
                            emailModel.Password = account.EmailAccountPassword;
                            emailModel.ServerHost = account.EmailAccountHost;
                            emailModel.Subject = account.EmailTemplateSubject;
                            emailModel.BodyHTML = account.EmailTemplateBodyHTML;

                            foreach (var emailBlast in lstQueryResult)
                            {
                                if (account.PerDayEmailSent <= perDaySendCount)
                                    break;

                                emailModel.BodyHTML = ReplaceEmailTag(emailModel.BodyHTML, EmailTagList, emailBlast);
                                emailModel.WarmingEmailListId = emailBlast.ID;
                                emailModel.ToEmail = emailBlast.Email;
                                string message = objEmail.SendEmailToCustomer(emailModel);
                                perDaySendCount = perDaySendCount + 1;

                                if (message.ToLower().Contains("unable to connect"))
                                    break;

                            }
                            #endregion

                        }
                        catch (Exception ex)
                        {
                            LogFile.AddMessage("**************** ERROR START **************************");
                            LogFile.AddMessage("ERROR in Main Loop (Ignoring it and continuing) LaunderingBulkScheduleEmailsId: " + account.LaunderingBulkScheduleEmailsId + "");
                            LogFile.AddMessage("Error Message:" + ex.Message);
                            LogFile.AddMessage("StackTrace Message:" + ex.StackTrace);
                            LogFile.AddMessage("**************** ERROR END   **************************");

                            continue;
                        }
                    }
                }

                LogFile.AddMessage("END: Unverified Blast Service on " + DateTime.Now.ToString());

            }
            catch (Exception ex)
            {
                DAL.InsertErrorLog("UnverifiedBlastService", ex.Message, ex.StackTrace);
            }


        }
        //private bool TimeBetween(DateTime datetime, TimeSpan start, TimeSpan end)
        //{
        //    // convert datetime to a TimeSpan
        //    TimeSpan now = datetime.TimeOfDay;
        //    // see if start comes before end
        //    if (start < end)
        //        return start <= now && now <= end;
        //    // start is after end, so do the inverse comparison
        //    return !(end < now && now < start);
        //}
        private string ReplaceEmailTag(string BodyHTML, List<EmailTagMaster> EmailTagList, WarmingEmailModel emailBlast)
        {
            foreach (var item in EmailTagList)
            {
                switch (item.Name)
                {
                    case Tags.firstname:
                        BodyHTML = BodyHTML.Replace(Tags.firstname, emailBlast.FirstName);
                        break;
                    case Tags.lastname:
                        BodyHTML = BodyHTML.Replace(Tags.lastname, emailBlast.LastName);
                        break;
                    case Tags.email:
                        BodyHTML = BodyHTML.Replace(Tags.email, emailBlast.Email);
                        break;
                    case Tags.url:
                        // don't know which value replace againest this tag
                        break;
                    case Tags.dynamicblog:
                        // don't know which value replace againest this tag
                        break;
                    case Tags.hldeblid:
                        BodyHTML = BodyHTML.Replace(Tags.hldeblid, Convert.ToString(emailBlast.ID));
                        break;
                    default:
                        break;
                }
            }
            return BodyHTML;
        }

        public List<EmailTagMaster> GetEmailTemplateTagMaster()
        {

            List<EmailTagMaster> TagList = new List<EmailTagMaster>();
            DataTable dt = DAL.GetEmailTemplateTagMaster();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    EmailTagMaster TagName = new EmailTagMaster();
                    {
                        TagName.Id = SafeValue<int>(row["Id"]);
                        TagName.Name = SafeValue<string>(row["TagName"]);
                        TagName.Description = SafeValue<string>(row["TagDescription"]);
                        TagList.Add(TagName);
                    };
                }
            }
            return TagList;
        }

        private string SetQuery(QueryModel item, List<FieldModel> lstField)
        {
            string Query = string.Empty;
            var isDateField = lstField.Where(m => m.FieldName == item.FieldName && (m.FieldType == "datetime" || m.FieldType == "date")).FirstOrDefault();
            if (isDateField == null)
                Query = " WarmingEmailList." + item.FieldName + " " + SetOperator(item, lstField, false);
            else
                Query = " cast(WarmingEmailList." + item.FieldName + " as Date)  " + SetOperator(item, lstField, true);
            return Query;
        }

        private string SetValue(string value, bool isDateField)
        {
            string Query = string.Empty;
            if (isDateField)
                Query = "cast('" + value + "' as Date)";
            else
                Query = "'" + value + "'";
            return Query;
        }

        private string SetOperator(QueryModel item, List<FieldModel> lstField, bool isDateField)
        {
            string rtn = string.Empty;
            switch (item.Condition.ToLower())
            {
                case "=":
                    rtn = " = " + SetValue(item.Value, isDateField) + " " + item.Operator;
                    break;
                case "<>":
                    rtn = " <> " + SetValue(item.Value, isDateField) + " " + item.Operator;
                    break;
                case ">":
                    rtn = " > " + SetValue(item.Value, isDateField) + " " + item.Operator;
                    break;
                case "<":
                    rtn = " < " + SetValue(item.Value, isDateField) + " " + item.Operator;
                    break;
                case ">=":
                    rtn = " >= " + SetValue(item.Value, isDateField) + " " + item.Operator;
                    break;
                case "<=":
                    rtn = " <= " + SetValue(item.Value, isDateField) + " " + item.Operator;
                    break;
                case "like":
                    rtn = " like '%" + item.Value + "%' " + item.Operator;
                    break;
                case "not like":
                    rtn = " not like '%" + item.Value + "%' " + item.Operator;
                    break;
                case "is null":
                    rtn = " IS NULL " + item.Operator;
                    break;
                case "is not null":
                    rtn = " IS NOT NULL " + item.Operator;
                    break;
                default:
                    // code block
                    break;
            }

            return rtn;
        }
    }
}
