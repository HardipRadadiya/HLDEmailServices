using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarmingBlast.Helper;
using WarmingBlast.Model;

namespace WarmingBlast.DataAccess
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
            return new DatabaseHelper(connectionString).DataTable("WarmingBlastService_GetAccountDetails");
        }

        public DataTable GetDynamicQuery(int WarmingBulkScheduleEmailsId)
        {
            DataTable dt = new DataTable();
            SqlParameter[] sqlParam = new SqlParameter[]
            {
                 new SqlParameter() {ParameterName = "@WarmingBulkScheduleEmailsId",Value= WarmingBulkScheduleEmailsId }
            };
            dt = new DatabaseHelper(connectionString).DataTable("WarmingBlastService_GetDynamicQuery", sqlParam);
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
                 new SqlParameter() {ParameterName = "@EmailTemplateMasterID",Value= ExtensionMethods.ToDB(model.EmailTemplateMasterID) },
                 new SqlParameter() {ParameterName = "@WarmingBulkScheduleEmailsId",Value= ExtensionMethods.ToDB(model.WarmingBulkScheduleEmailsId) },
                 new SqlParameter() {ParameterName = "@FromEmail",Value= ExtensionMethods.ToDB(model.FromEmail)},
                 new SqlParameter() {ParameterName = "@ToEmail",Value= ExtensionMethods.ToDB(model.ToEmail) },
                 new SqlParameter() {ParameterName = "@Host",Value= ExtensionMethods.ToDB(model.ServerHost) },
                 new SqlParameter() {ParameterName = "@ErrorMsg",Value= ExtensionMethods.ToDB(model.ErrorMessage) },
                 new SqlParameter() {ParameterName = "@Type",Value= ExtensionMethods.ToDB(model.Type) },
            };
            new DatabaseHelper(connectionString).ExecuteNonQuery("WarmingBlastService_EmailHistory", sqlParam);
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
