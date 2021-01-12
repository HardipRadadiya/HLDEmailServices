using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace UnverifiedBlast.Model
{
	public class FieldModel
	{
		public int Id { get; set; }
		public string FieldName { get; set; }
		public string FieldType { get; set; }
	}
	public class QueryModel
	{
		public int Id { get; set; }
		public int LaunderingBulkScheduleEmailsId { get; set; }
		public string FieldName { get; set; }
		public string Condition { get; set; }
		public string Value { get; set; }
		public DateTime CreatedOn { get; set; }
		public int CreatedBy { get; set; }
		public DateTime ModifiedOn { get; set; }
		public int ModifiedBy { get; set; }
		public string Operator { get; set; }
	}

	public class WarmingEmailModel
	{
		public int ID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Profession { get; set; }
		public string Specialty { get; set; }
		public string Phone { get; set; }
		public bool DoNotEmail { get; set; }
		public bool CandidateOptOut { get; set; }
		public bool BadEmail { get; set; }
		public bool SignedUp { get; set; }
		public DateTime? LastEmailed { get; set; }
		public int TimesEmailed { get; set; }
		public DateTime? LastClicked { get; set; }
		public DateTime? LastOpened { get; set; }
		public string GUID { get; set; }
		public string LastEmailedHost { get; set; }
		public string TypeOfHost { get; set; }
		public bool Quarantined { get; set; }
		public bool DoubleQuarantine { get; set; }
		public DateTime? LastUsageDate { get; set; }
		public bool sellable { get; set; }
		public bool HealthLink { get; set; }
		public string emailSource { get; set; }
		public string emailSourceSubType { get; set; }
		public long HLDProvID { get; set; }
		public int HLDEmailID { get; set; }
		public string WebbulaFlag { get; set; }
		public string StateAbbrev { get; set; }
		public bool IsDuplicate { get; set; }
		public bool AbuseFlag { get; set; }
		public DateTime? AbuseLogged { get; set; }
		public bool Active { get; set; }
		public DateTime? CreatedOn { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? ModifiedOn { get; set; }
		public string ModifiedBy { get; set; }
		public DateTime? DeactivateddOn { get; set; }
		public string DeactivateddBy { get; set; }
		public string Notes { get; set; }
		public bool IsGoodEmail { get; set; }
		public decimal HLDEmailRating { get; set; }
		public DateTime? HLDPermissionDate { get; set; }
		public DateTime? HLDVerificationDate { get; set; }
		public DateTime? HLDEmailSent { get; set; }
	}

	public class EmailSetting
	{
		public int WarmingEmailListId { get; set; }
		public int LaunderingEmailTemplateMasterId { get; set; }
		public int LaunderingBulkScheduleEmailsId { get; set; }
		
		public string FromEmail { get; set; }
		public string ToEmail { get; set; }
		public string Password { get; set; }
		public string ServerHost { get; set; }
		public string Subject { get; set; }
		public string BodyHTML { get; set; }

	}

	public class EmailHistoryModel

	{
		public int WarmingEmailListId { get; set; }
		public int LaunderingEmailTemplateMasterId { get; set; }
		public int LaunderingBulkScheduleEmailsId { get; set; }
		public string FromEmail { get; set; }
		public string ToEmail { get; set; }
		public string ServerHost { get; set; }
		public string ErrorMessage { get; set; }
		public string Type { get; set; }

	}


	public class EmailAccountDetails
	{
		public int LaunderingBulkScheduleEmailsId { get; set; }
		public int EmailTemplateMasterID { get; set; }
		public bool DoNotEmailValue { get; set; }
		public int PerDayEmailSent { get; set; }
		public int PerHourEmailSent { get; set; }
		public int DoNotRepeatSentEmailDays { get; set; }
		public bool IsActive { get; set; }
		public bool IsStart { get; set; }
		public bool IsStop { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsSpamTracking { get; set; }
		
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }
		public string EmailTemplateName { get; set; }
		public string EmailTemplateSubject { get; set; }
		public string EmailTemplateDescription { get; set; }
		public string EmailTemplateBodyHTML { get; set; }



		public string EmailAccountId { get; set; }
		public string EmailAccountPassword { get; set; }
		public string EmailAccountMailServer { get; set; }
		public int EmailAccountPort { get; set; }
		public bool EmailAccountSSL { get; set; }
		public bool EmailAccountFetchEmails { get; set; }
		 
		public bool EmailAccountSendEmail { get; set; }
		public bool EmailAccountReceiveEmail { get; set; }
		public string EmailAccountReceiveEmailServer { get; set; }
		public string EmailAccountHost { get; set; }


	}

	public class TimeSloat
	{
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
	}

	public class EmailTagMaster
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
