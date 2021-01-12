using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnverifiedBlast.Helper;
using UnverifiedBlast.Model;

namespace UnverifiedBlast.DataAccess
{
    public class EmailBlastDAL
    {
        string connectionString = string.Empty;
        public EmailBlastDAL()
        {
            connectionString = HLDEmailConnectionString();
        }
        public static string HLDEmailConnectionString()
        {

            string connection = ConfigurationManager.ConnectionStrings["HLDEmailConnectionString"].ConnectionString;
            return connection;

        }

        public DataTable GetWarmingEmailField()
        {
            return new DatabaseHelper(connectionString).DataTable("UnverifiedBlastService_GetWarmingEmailField");
        }

        public DataTable GetAccountDetails()
        {
            return new DatabaseHelper(connectionString).DataTable("UnverifiedBlastService_GetAccountDetails");
        }

        public DataTable GetDynamicQuery(int LaunderingBulkScheduleEmailsId)
        {
            DataTable dt = new DataTable();
            SqlParameter[] sqlParam = new SqlParameter[]
            {
                 new SqlParameter() {ParameterName = "@LaunderingBulkScheduleEmailsId",Value= LaunderingBulkScheduleEmailsId }
            };
            dt = new DatabaseHelper(connectionString).DataTable("UnverifiedBlastService_GetDynamicQuery", sqlParam);
            return dt;
        }

        public DataTable GetDynamicQueryResult(int TopRecord, string Where)
        {
            DataTable dt = new DataTable();
            SqlParameter[] sqlParam = new SqlParameter[]
            {
                 new SqlParameter() {ParameterName = "@TopRecord",Value= TopRecord },
                 new SqlParameter() {ParameterName = "@Where",Value= Where }
            };
            dt = new DatabaseHelper(connectionString).DataTable("UnverifiedBlastService_GetDynamicQueryResult", sqlParam);
            return dt;
        }

        public void InsertErrorLog(string URL, string ErrorMessage, string StackTrace)
        {
            DataTable dt = new DataTable();
            SqlParameter[] sqlParam = new SqlParameter[]
            {
                new SqlParameter() {ParameterName = "@URL",Value= URL },
                 new SqlParameter() {ParameterName = "@ErrorMessage",Value= ErrorMessage },
                 new SqlParameter() {ParameterName = "@StackTrace",Value= StackTrace }
            };
            new DatabaseHelper(connectionString).ExecuteNonQuery("Insert_ErrorLog", sqlParam);
        }

        public void SentEmailHistory(EmailHistoryModel model)
        {
            DataTable dt = new DataTable();
            SqlParameter[] sqlParam = new SqlParameter[]
            {
                new SqlParameter() {ParameterName = "@WarmingEmailListId",Value= ExtensionMethods.ToDB(model.WarmingEmailListId) },
                 new SqlParameter() {ParameterName = "@LaunderingEmailTemplateMasterId",Value= ExtensionMethods.ToDB(model.LaunderingEmailTemplateMasterId) },
                 new SqlParameter() {ParameterName = "@LaunderingBulkScheduleEmailsId",Value= ExtensionMethods.ToDB(model.LaunderingBulkScheduleEmailsId) },
                 new SqlParameter() {ParameterName = "@FromEmail",Value= ExtensionMethods.ToDB(model.FromEmail)},
                 new SqlParameter() {ParameterName = "@ToEmail",Value= ExtensionMethods.ToDB(model.ToEmail) },
                 new SqlParameter() {ParameterName = "@Host",Value= ExtensionMethods.ToDB(model.ServerHost) },
                 new SqlParameter() {ParameterName = "@ErrorMsg",Value= ExtensionMethods.ToDB(model.ErrorMessage) },
                 new SqlParameter() {ParameterName = "@Type",Value= ExtensionMethods.ToDB(model.Type) },
            };
            new DatabaseHelper(connectionString).ExecuteNonQuery("UnverifiedBlastService_EmailHistory", sqlParam);
        }

        public DataTable GetTodaysSendEmail(string EmailAccountHost)
        {
            DataTable dt = new DataTable();
            SqlParameter[] sqlParam = new SqlParameter[]
            {
                 new SqlParameter() {ParameterName = "@Host",Value= EmailAccountHost } 
            };
            dt = new DatabaseHelper(connectionString).DataTable("UnverifiedBlastService_GetTodaysSendEmail", sqlParam);
            return dt;
        }

        public DataTable GetEmailTemplateTagMaster()
        {
            DataTable dt = new DataTable();
            dt = new DatabaseHelper(connectionString).DataTable("GetEmailTemplateTagMaster");
            return dt;
        }


    }
}
