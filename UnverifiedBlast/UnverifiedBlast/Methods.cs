using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnverifiedBlast
{
    class Methods
    {
        static public void sendPhysicianAlert_NetBlast()
        {
            string TodaysDay = DateTime.Now.DayOfWeek.ToString();
            // If statement only sends on Sunday through Thursday
            //LAF:  2020-05-21 Removed date restriction.
            //            if (TodaysDay != "Friday" & TodaysDay != "Saturday")
            //            {
            #region Send Blast
            System.Random RandNum = new System.Random();
            ASCDataContext db = new ASCDataContext();
            DateTime TodayDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            int blasthistorycount = (from b in db.WarmingEmailLists
                                     where (b.LastEmailed.Value.Date == TodayDate.Date) &
                                     b.LastEmailedHost == "physiciansalert.net"     //LAF:  2020-06-18 removed the smtp. from string
                                     select b).Count();
            //Specialty Sending Variables
            string IncludeSpecialty = "Internal";
            string ExcludeSpecialty = "Pediatric";
            DateTime passDate = new DateTime(2020, 10, 7);
            DateTime fromDate = new DateTime(2020, 09, 13);
            DateTime toDate = new DateTime(2020, 09, 14);

            List<WarmingEmailList> candidateblastlist = (from b in db.WarmingEmailLists
                                                         //  join c in db.List_ID_1908_for_Ardents on b.Email equals c.EMAIL
                                                         //where b.Quarantined == true & b.DoubleQuarantine.HasValue == false & b.DoNotEmail == false & b.BadEmail == false & b.CandidateOptOut == false & b.SignedUp == false & (b.DrBillDatabase == false || b.DrBillDatabase.HasValue == false)
                                                         where //b.DoNotEmail == true &     //include all warming and laundering
                                                             // b.BadEmail == false &
                                                             //b.CandidateOptOut == false &
                                                             // b.AbuseFlag == false &
                                                             //  b.SignedUp == false &
                                                             //LAF: 2020-08-10 - Connected updated table layout and added check for Active records
                                                             //  b.Active == true &
                                                             //LAF:  2020-08-27:  changes to send via specialty and IDs:
                                                             //FOR TESTING                                    b.ID < 100 & 
                                                             //For ID Range (like for Int Med)                b.ID > 6000 & b.ID < 2709040 &
                                                              b.Specialty.Contains(IncludeSpecialty) &
                                                            b.Specialty.Contains(ExcludeSpecialty) == false &
                                                            b.LastEmailed.Value.Date >= fromDate.Date &
                                                             b.LastEmailed.Value.Date < toDate.Date &
                                                             b.LastEmailedHost == "physiciansalert.net" // &
                                                         //   b.ID > 2604990
                                                         //LAF:  2020-08-27:  end of specialty changes

                                                         //   (b.TimesEmailed == 0 || b.TimesEmailed == null)  
                                                         //(b.Quarantined == false || b.Quarantined.HasValue == false) //&         --LAF:  2020-08-27 Opted to not quarantine since we are not certain of the dates marked.
                                                         // (b.BulkDentalList== false || b.BulkDentalList.HasValue == false)// &      --LAF: 2020-05-21 Commented out
                                                         //   b.DrBillDatabase == false// & b.Profession == 1
                                                         orderby b.LastEmailed, b.Email ascending
                                                         select b).ToList();
            //For Testing Use 2 lines below:
            Console.WriteLine(candidateblastlist.Count);
            //Console.ReadLine();


            //LAF: 2020-05-18 Changed to 1000
            //int EmailCount = 10000 - blasthistory.UnverifiedCand;
            int EmailCount = 15840 - blasthistorycount;     //LAF:  2020-08-30 warming the domain - starting at 10,000

            // Set email count to 1 for testing purposes.
            //EmailCount = 1;

            //EmailCount = 1000;
            if (EmailCount > 0)
            {
                if (EmailCount > 1500)
                    EmailCount = 1500;

                candidateblastlist = (from b in candidateblastlist
                                      select b).Take(EmailCount).ToList();

                foreach (var b in candidateblastlist)
                {

                    string strEmail = b.Email.Trim();
                    try
                    {
                        //LAF:  2020-08-10 - modifield from to bsmith (not bsherp) to match signature.
                        System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage("bsmith@physiciansalert.net", strEmail);
                        //System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage("bsherp@physiciansalert.net", "bsherp@alertservicescorp.com");
                        //message.CC.Add("kperpetua@alertservicescorp.com");
                        //message.Subject = "Find the ideal medical practice.";
                        message.Subject = "It is never too early to start looking for a practice.";
                        //                            string MessageBody = "<p><a href='http://www.physiciansalert.net/?hldblid=" + b.ID + "'><img alt='PhysiciansAlert' border='0' src='http://www.physiciansalert.net/images/paubl/" + b.ID + ".aspx' height='30px' style='vertical-align: top; display: block; font-family: arial; font-size: 13px; color: #fff;' /></a></p>";
                        string MessageBody = "<p><a href='http://www.physiciansalert.net/?hldblid=" + b.ID + "'><img alt='PhysiciansAlert' border='0' src='http://www.physiciansalert.net/images/paubl/0.aspx' height='30px' style='vertical-align: top; display: block; font-family: arial; font-size: 13px; color: #fff;' /></a></p>";
                        //MessageBody = MessageBody + "<p>If you are looking for a new practice, or if you are looking for a partner for your existing practice, please go to:</p>";
                        MessageBody = MessageBody + "<p>Employers have already begun to interview physicians for their 2019-2021 openings. In fact, some of the best practices for 2019 have already been filled. In order to ensure that you are considered for the practice that you want, it is important that you register as soon as possible:</p>";
                        MessageBody = MessageBody + "<p><a href='http://www.physiciansalert.net/register/candidateregistration.aspx?hldblid=" + b.ID + "'>http://www.physiciansalert.net/</a></p>";
                        MessageBody = MessageBody + "<p>We work with Physicians, Nurse Practitioners, Physician Assistants, and CRNAs. This is the best way to learn about positions directly from the hospitals and medical groups who are recruiting for your specialty in specific locations.</p>";
                        MessageBody = MessageBody + "<p>If you have a colleague who is looking for a position, or who didn't receive this email, please forward it to him/her.</p>";
                        MessageBody = MessageBody + "<p>My father and brother-in-law are both physicians, so I know from their experience how challenging it can be to identify the right practice when you are being overwhelmed by too many unattractive offers.</p>";
                        MessageBody = MessageBody + "<p>We are not a search firm, and there is no cost for physicians. Our concept is simple - Create a profile, and tell us where you want to practice. We will notify organizations in those areas – and nowhere else – that you are interested in hearing what positions they might have available. No search firms, and no games – just a chance to find a great practice.</p>";
                        MessageBody = MessageBody + "<p>Brian Smith<br/>";
                        MessageBody = MessageBody + "Director of Physician Services<br/>";
                        MessageBody = MessageBody + "PhysiciansAlert.net</p>";
                        // MessageBody = MessageBody + "<p><a href='http://www.physiciansalert.net/optout.aspx?email=" + strEmail + "'>Unsubscribe</a></p>";
                        MessageBody = MessageBody + "<p><a href='mailto:hldbounce@hcpnavigator.com?Subject=Unsubscribe-PA-Net-" + strEmail + "'>Unsubscribe</a></p>";

                        message.Body = ("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'><html xmlns='http://www.w3.org/1999/xhtml'><head><title></title></head><body>" + MessageBody + "</body></html>");
                        message.IsBodyHtml = true;
                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        System.Net.NetworkCredential AuthenticationInfo = new System.Net.NetworkCredential("bsherp@physiciansalert.net", "zAsu5hut");
                        smtp.Host = "smtp.physiciansalert.net";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = AuthenticationInfo;
                        smtp.Send(message);

                        Console.WriteLine("Sent " + b.ID);
                        Console.WriteLine("Saving Sent Record: " + b.ID);
                        if (b.TimesEmailed.HasValue == false)
                            b.TimesEmailed = 1;
                        else
                            b.TimesEmailed = b.TimesEmailed + 1;
                        int RandomDays = RandNum.Next(-14, -1);
                        // b.LastEmailed = DateTime.Now.AddDays(RandomDays);
                        b.LastEmailed = DateTime.Now;
                        b.LastEmailedHost = "physiciansalert.net";
                        //if (b.BulkMailList == true)                               //LAF:  2020-06-19 Turned this off so I can use the field for tracking the right Warming values.
                        //{
                        //    b.DoNotEmail = false;
                        //    b.LastEmailed = DateTime.Now.AddDays(RandomDays);
                        //}
                        //blasthistory.UnverifiedCand = blasthistory.UnverifiedCand + 1;

                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage("bsherp@physiciansalert.net", "bsherp@alertservicescorp.com");
                        message.Subject = "Unverified E-mail Blast Failure for sendPhysicianAlert_NetBlast";
                        string strBody = "";
                        strBody = strBody + "CandidateBlastListId: " + b.ID + Environment.NewLine;
                        strBody = strBody + "E-mail: " + strEmail + Environment.NewLine;
                        strBody = strBody + "Error Report: " + ex.ToString() + Environment.NewLine;
                        message.Body = (strBody);
                        message.IsBodyHtml = false;
                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        System.Net.NetworkCredential AuthenticationInfo = new System.Net.NetworkCredential("bsherp@physiciansalert.net", "zAsu5hut");
                        smtp.Host = "smtp.physiciansalert.net";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = AuthenticationInfo;
                        smtp.Send(message);
                        b.DoNotEmail = true;
                        b.BadEmail = true;
                        db.SubmitChanges();
                    }
                }


            }
            db.Dispose();
            #endregion
            //            }
        }

        static public void sendPhysicianAlert_OrgBlast()
        {
            string TodaysDay = DateTime.Now.DayOfWeek.ToString();
            // If statement only sends on Sunday through Thursday
            //LAF:  2020-05-21 Removed date restriction.
            //            if (TodaysDay != "Friday" & TodaysDay != "Saturday")
            //            {
            #region Send Blast
            System.Random RandNum = new System.Random();
            ASCDataContext db = new ASCDataContext();
            DateTime TodayDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            int blasthistorycount = (from b in db.WarmingEmailLists
                                     where (b.LastEmailed.Value.Date == TodayDate.Date) &
                                     b.LastEmailedHost == "physiciansalert.org"     //LAF:  2020-06-18 removed the smtp. from string
                                     select b).Count();
            //Specialty Sending Variables
            string IncludeSpecialty = "Internal";
            string ExcludeSpecialty = "Pediatric";
            DateTime passDate = new DateTime(2020, 10, 6);
            DateTime passtoDate = new DateTime(2020, 10, 16);
            DateTime fromDate = new DateTime(2020, 09, 17);
            DateTime toDate = new DateTime(2020, 10, 29);

            List<WarmingEmailList> candidateblastlist = (from b in db.WarmingEmailLists
                                                         //where b.Quarantined == true & b.DoubleQuarantine.HasValue == false & b.DoNotEmail == false & b.BadEmail == false & b.CandidateOptOut == false & b.SignedUp == false & (b.DrBillDatabase == false || b.DrBillDatabase.HasValue == false)
                                                         where b.DoNotEmail == true &     //include all warming and laundering
                                                             //b.BadEmail == false &
                                                             //b.CandidateOptOut == false &
                                                               b.AbuseFlag == false &
                                                             //  b.SignedUp == false &
                                                             //LAF: 2020-08-10 - Connected updated table layout and added check for Active records
                                                             // b.Active == true &
                                                             //LAF:  2020-08-27:  changes to send via specialty and IDs:
                                                             //For Test Sending                                b.ID < 100 & 
                                                             //For ID Range Sending                            b.ID > 6000 & b.ID < 2709040 &
                                                               b.Specialty.Contains(IncludeSpecialty) &
                                                             b.Specialty.Contains(ExcludeSpecialty) == false &
                                                             //b.LastEmailed.Value.Date > passDate.Date &
                                                             //b.LastEmailed.Value.Date< passtoDate.Date

                                                              b.LastEmailed.Value.Date > fromDate.Date &
                                                                 b.LastEmailed.Value.Date < toDate.Date &
                                                                 b.LastEmailedHost == "physiciansalert.net" //&
                                                         //   b.ID < 2604990
                                                         //LAF:  2020-08-27:  end of specialty changes
                                                         //(b.TimesEmailed == 0 || b.TimesEmailed == null) &
                                                         //(b.Quarantined == false || b.Quarantined.HasValue == false) //&         --LAF:  2020-08-27 Opted to not quarantine since we are not certain of the dates marked.
                                                         // (b.BulkDentalList== false || b.BulkDentalList.HasValue == false)// &      --LAF: 2020-05-21 Commented out
                                                         //   b.DrBillDatabase == false// & b.Profession == 1
                                                         orderby b.LastEmailed, b.Email ascending
                                                         select b).ToList();
            //For Testing Use 2 lines below:
            Console.WriteLine(candidateblastlist.Count);
            //Console.ReadLine();


            //LAF: 2020-05-18 Changed to 1000
            //int EmailCount = 10000 - blasthistory.UnverifiedCand;
            int EmailCount = 683 - blasthistorycount;     //LAF:          SET THE TOTAL TO BE SENT PER DAY!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            // Set email count to 1 for testing purposes.
            //EmailCount = 1;

            //EmailCount = 1000;
            if (EmailCount > 0)
            {
                if (EmailCount > 500)
                    EmailCount = 500;

                candidateblastlist = (from b in candidateblastlist
                                      select b).Take(EmailCount).ToList();

                foreach (var b in candidateblastlist)
                {

                    string strEmail = b.Email.Trim();
                    try
                    {
                        //LAF:  2020-08-10 - modifield from to bsmith (not bsherp) to match signature.
                        System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage("bsmith@physiciansalert.org", strEmail);

                        message.Subject = "Find jobs not posted on job boards.";

                        string MessageBody = "<p><a href='http://www.physiciansalert.org/?hldeid=" + b.ID + "'><img alt='PhysiciansAlert' border='0' src='http://www.physiciansalert.org/images/paubl/0.aspx' height='30px' style='vertical-align: top; display: block; font-family: arial; font-size: 13px; color: #fff;' /></a></p>";
                        MessageBody = MessageBody + "<p>Most hospital’s recruiting budgets are smaller than ever before. This means that they cannot afford to post all of their open positions. PhysiciansAlert is the best way to find a physician job, because you can notify hospitals in specific geographic areas about your interest.</p>";
                        MessageBody = MessageBody + "<p><a href='http://www.physiciansalert.org/register/candidateregistration.aspx?hldeid=" + b.ID + "'>http://www.physiciansalert.org/</a></p>";
                        MessageBody = MessageBody + "<p>We work with Physicians, Nurse Practitioners, Physician Assistants, and CRNAs. This is the best way to learn about unposted job openings.</p>";
                        MessageBody = MessageBody + "<p>If you have a colleague who is looking for a position, or who didn't receive this email, please forward it to him/her.</p>";
                        MessageBody = MessageBody + "<p>We are not a search firm, and there is no cost for physicians. Our concept is simple - Create a profile, and tell us where you want to practice. We will notify organizations in those areas – and nowhere else – that you are interested in hearing what positions they might have available. No search firms, and no games – just a chance to find a great practice.</p>";
                        MessageBody = MessageBody + "<p>Brian Smith<br/>";
                        MessageBody = MessageBody + "Director of Physician Outreach<br/>";
                        MessageBody = MessageBody + "PhysiciansAlert.org</p>";
                        MessageBody = MessageBody + "<p><a href='http://www.physiciansalert.org/optout.aspx?email=" + strEmail + "'>Unsubscribe</a></p>";

                        message.Body = ("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'><html xmlns='http://www.w3.org/1999/xhtml'><head><title></title></head><body>" + MessageBody + "</body></html>");
                        message.IsBodyHtml = true;
                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        System.Net.NetworkCredential AuthenticationInfo = new System.Net.NetworkCredential("blast@physiciansalert.org", "U7xpMd9!xm5Prdm");

                        smtp.Host = "smtp.physiciansalert.org";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = AuthenticationInfo;
                        smtp.Send(message);

                        Console.WriteLine("Sent " + b.ID);
                        Console.WriteLine("Saving Sent Record: " + b.ID);
                        if (b.TimesEmailed.HasValue == false)
                            b.TimesEmailed = 1;
                        else
                            b.TimesEmailed = b.TimesEmailed + 1;
                        int RandomDays = RandNum.Next(-14, -1);
                        // b.LastEmailed = DateTime.Now.AddDays(RandomDays);
                        b.LastEmailed = DateTime.Now;
                        b.LastEmailedHost = "physiciansalert.org";
                        //if (b.BulkMailList == true)                               //LAF:  2020-06-19 Turned this off so I can use the field for tracking the right Warming values.
                        //{
                        //    b.DoNotEmail = false;
                        //    b.LastEmailed = DateTime.Now.AddDays(RandomDays);
                        //}
                        //blasthistory.UnverifiedCand = blasthistory.UnverifiedCand + 1;

                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage("bsherp@physiciansalert.net", "bsherp@alertservicescorp.com");
                        message.Subject = "Unverified E-mail Blast Failure for sendPhysicianAlert_OrgBlast";
                        string strBody = "";
                        strBody = strBody + "CandidateBlastListId: " + b.ID + Environment.NewLine;
                        strBody = strBody + "E-mail: " + strEmail + Environment.NewLine;
                        strBody = strBody + "Error Report: " + ex.ToString() + Environment.NewLine;
                        message.Body = (strBody);
                        message.IsBodyHtml = false;
                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        System.Net.NetworkCredential AuthenticationInfo = new System.Net.NetworkCredential("bsherp@physiciansalert.net", "zAsu5hut");
                        smtp.Host = "smtp.physiciansalert.net";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = AuthenticationInfo;
                        smtp.Send(message);
                        b.DoNotEmail = true;
                        b.BadEmail = true;
                        db.SubmitChanges();
                    }
                }


            }
            db.Dispose();
            #endregion
            //            }
        }

        static public void sendPhysicianAlert_NetNurseBlast()
        {
            string TodaysDay = DateTime.Now.DayOfWeek.ToString();
            #region Send Nurse Blast
            System.Random RandNum = new System.Random();
            ASCDataContext db = new ASCDataContext();
            DateTime TodayDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            int blasthistorycount = (from b in db.WarmingEmailLists
                                     where (b.LastEmailed.Value.Date == TodayDate.Date) &
                                     b.LastEmailedHost == "physiciansalert.net"
                                     select b).Count();

            int EmailCount = 10000 - blasthistorycount;       //LAF:  SET THE TOTAL TO BE SENT PER DAY!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //EmailCount = 1;                      // Set email count to 1 for testing purposes.

            string IncludeStandardizedTitle = "NP";
            string ExcludeCreatedBy = "email_Auxiliary_Nurse_Email";
           // string IncludeCreatedBy = "email_Auxiliary_Nurse_Email";

            DateTime passDate = new DateTime(2020, 10, 6);
            DateTime passtoDate = new DateTime(2020, 10, 16);
            DateTime fromDate = new DateTime(2020, 09, 17);
            DateTime toDate = new DateTime(2020, 10, 29);

            List<WarmingEmailList> candidateblastlist = (from b in db.WarmingEmailLists
                                                         where b.DoNotEmail == true         //True for Laundering;  False for Warming
                                                            & b.BadEmail == false           //Not flagged as a Bad Email
                                                            & b.CandidateOptOut == false    //Not opted out
                                                            & b.SignedUp == false           //Not signed as Candidate  
                                                            & b.AbuseFlag == false          //Not flagged as abuse
                                                            & b.Active == true              //Must be active email
                                                            & (b.Quarantined == false || b.Quarantined.HasValue == false)       //Not quarantined
                                                            & (b.TimesEmailed == 0 || b.TimesEmailed == null)                   //No email sent
                                                            & b.Profession.Contains(IncludeStandardizedTitle)       //Nurse Professions
                                                             & b.CreatedBy.Contains(ExcludeCreatedBy) == false
                                                         
                                                         // & b.CreatedBy.Equals(IncludeCreatedBy)                  //To isolate the purchased list of Nurses

                                                         // & b.LastEmailed.Value.Date > fromDate.Date           //Dates
                                                         // & b.LastEmailed.Value.Date < toDate.Date             //Dates
                                                         // & b.LastEmailed.Value.Date > passDate.Date           //Dates
                                                         // & b.LastEmailed.Value.Date< passtoDate.Date          //Dates
                                                         // & b.ID < 2604990                                     //IDs
                                                         // & b.LastEmailedHost == "physiciansalert.net"         //Last Emailed Host

                                                         orderby b.LastEmailed, b.Email ascending
                                                         select b).ToList();
            //For Testing Use 2 lines below:
            Console.WriteLine(candidateblastlist.Count);
            //Console.ReadLine();


            if (EmailCount > 0)
            {
                if (EmailCount > 1000)
                    EmailCount = 1000;

                candidateblastlist = (from b in candidateblastlist
                                      select b).Take(EmailCount).ToList();

                foreach (var b in candidateblastlist)
                {

                    string strEmail = b.Email.Trim();
                    try
                    {
                        System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage("hfoster@physiciansalert.net", strEmail);         //hfoster for Nurses

                        message.Subject = "It's never too early to start looking for your next nursing position";

                        string MessageBody = "<p><a href='http://www.physiciansalert.net/?hldeid=" + b.ID + "'><img alt='PhysiciansAlert' border='0' src='http://www.physiciansalert.net/images/paubl/0.aspx' height='30px' style='vertical-align: top; display: block; font-family: arial; font-size: 13px; color: #fff;' /></a></p>";
                        MessageBody = MessageBody + "<p>Employers have already begun to interview nurses for their 2021 openings. In fact, some of the positions throughout the country have already been filled. To ensure you are considered for the best nursing position, register for PracticeAlert to get the most up-to-date information about available nursing jobs:</p>";
                        MessageBody = MessageBody + "<p><a href='http://www.physiciansalert.net/register/candidateregistration.aspx?hldeid=" + b.ID + "'>http://www.physiciansalert.net/</a></p>";
                        MessageBody = MessageBody + "<p>The best way to learn about open nursing positions is directly from the hospitals and medical groups who are recruiting for nursing positions in your specific location. PracticeAlert works with physicians, nurse practitioners, physician assistants, and CRNAs to help make those connections.</p>";
                        MessageBody = MessageBody + "<p>We are not a search firm, and there is no cost for nurses to sign up. Our concept is simple: create a profile and tell us where you are seeking your next nursing job. PracticeAlert will then notify organizations in your area to help you find the best job possible.</p>";
                        MessageBody = MessageBody + "<p>Harold Foster<br/>";
                        MessageBody = MessageBody + "Director of Physician Outreach<br/>";
                        MessageBody = MessageBody + "PhysiciansAlert.net</p>";
                        MessageBody = MessageBody + "<p><a href='http://www.physiciansalert.net/optout.aspx?email=" + strEmail + "'>Unsubscribe</a></p>";

                        message.Body = ("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'><html xmlns='http://www.w3.org/1999/xhtml'><head><title></title></head><body>" + MessageBody + "</body></html>");
                        message.IsBodyHtml = true;
                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        System.Net.NetworkCredential AuthenticationInfo = new System.Net.NetworkCredential("bsherp@physiciansalert.net", "zAsu5hut");

                        smtp.Host = "smtp.physiciansalert.net";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = AuthenticationInfo;
                        smtp.Send(message);

                        Console.WriteLine("Sent " + b.ID);
                        Console.WriteLine("Saving Sent Record: " + b.ID);
                        if (b.TimesEmailed.HasValue == false)
                            b.TimesEmailed = 1;
                        else
                            b.TimesEmailed = b.TimesEmailed + 1;
                        int RandomDays = RandNum.Next(-14, -1);
                        b.LastEmailed = DateTime.Now;
                        b.LastEmailedHost = "physiciansalert.net";

                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage("bsherp@physiciansalert.net", "bsherp@alertservicescorp.com");
                        message.Subject = "Unverified Nurses E-mail Blast Failure for sendPhysicianAlert_Net_NurseBlast";
                        string strBody = "";
                        strBody = strBody + "CandidateBlastListId: " + b.ID + Environment.NewLine;
                        strBody = strBody + "E-mail: " + strEmail + Environment.NewLine;
                        strBody = strBody + "Error Report: " + ex.ToString() + Environment.NewLine;
                        message.Body = (strBody);
                        message.IsBodyHtml = false;
                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        System.Net.NetworkCredential AuthenticationInfo = new System.Net.NetworkCredential("bsherp@physiciansalert.net", "zAsu5hut");
                        smtp.Host = "smtp.physiciansalert.net";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = AuthenticationInfo;
                        smtp.Send(message);
                        b.DoNotEmail = true;
                        b.BadEmail = true;
                        db.SubmitChanges();
                    }
                }


            }
            db.Dispose();
            #endregion
            //            }
        }

        static public void sendPhysicianAlert_OrgNurseBlast()
        {
            string TodaysDay = DateTime.Now.DayOfWeek.ToString();
            #region Send Nurse Blast
            System.Random RandNum = new System.Random();
            ASCDataContext db = new ASCDataContext();
            DateTime TodayDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            int blasthistorycount = (from b in db.WarmingEmailLists
                                     where (b.LastEmailed.Value.Date == TodayDate.Date) &
                                     b.LastEmailedHost == "physiciansalert.org"
                                     select b).Count();

            int EmailCount = 5000 - blasthistorycount;       //LAF:  SET THE TOTAL TO BE SENT PER DAY!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //EmailCount = 1;                      // Set email count to 1 for testing purposes.

            string IncludeStandardizedTitle = "PA";  //--, 'NP', 'APN')

            string ExcludeCreatedBy = "email_Auxiliary_Nurse_Email";

            DateTime passDate = new DateTime(2020, 10, 6);
            DateTime passtoDate = new DateTime(2020, 10, 16);
            DateTime fromDate = new DateTime(2020, 12, 9);
            DateTime toDate = new DateTime(2020, 12, 11);

            List<WarmingEmailList> candidateblastlist = (from b in db.WarmingEmailLists
                                                         where b.DoNotEmail == true         //True for Laundering;  False for Warming
                                                            & b.BadEmail == false           //Not flagged as a Bad Email
                                                            & b.CandidateOptOut == false    //Not opted out
                                                            & b.SignedUp == false           //Not signed as Candidate  
                                                            & b.AbuseFlag == false          //Not flagged as abuse
                                                            & b.Active == true              //Must be active email
                                                            & (b.Quarantined == true || b.Quarantined.HasValue == true)       //Not quarantined
                                                         //   & (b.TimesEmailed == 0 || b.TimesEmailed == null)                   //No email sent
                                                            & (b.TimesEmailed == 1)  
                                                             & b.Profession.Contains(IncludeStandardizedTitle)       //Nurse Professions
                                                           & b.CreatedBy.Contains(ExcludeCreatedBy) == false

                                                          & b.LastEmailed.Value.Date > fromDate.Date           //Dates
                                                          & b.LastEmailed.Value.Date < toDate.Date             //Dates
                                                         // & b.LastEmailed.Value.Date > passDate.Date           //Dates
                                                         // & b.LastEmailed.Value.Date< passtoDate.Date          //Dates
                                                         // & b.ID < 2604990                                     //IDs
                                                          & b.LastEmailedHost == "physiciansalert.org"         //Last Emailed Host

                                                         orderby b.LastEmailed, b.Email ascending
                                                         select b).ToList();
            //For Testing Use 2 lines below:
            Console.WriteLine(candidateblastlist.Count);
            //Console.ReadLine();


            if (EmailCount > 0)
            {
                if (EmailCount > 1000)
                    EmailCount = 1000;

                candidateblastlist = (from b in candidateblastlist
                                      select b).Take(EmailCount).ToList();

                foreach (var b in candidateblastlist)
                {

                    string strEmail = b.Email.Trim();
                    try
                    {
                        System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage("hfoster@physiciansalert.org", strEmail);         //hfoster for Nurses

                        message.Subject = "It's never too early to start looking for your next nursing position";

                        string MessageBody = "<p><a href='http://www.physiciansalert.org/?hldeid=" + b.ID + "'><img alt='PhysiciansAlert' border='0' src='http://www.physiciansalert.org/images/paubl/0.aspx' height='30px' style='vertical-align: top; display: block; font-family: arial; font-size: 13px; color: #fff;' /></a></p>";
                        MessageBody = MessageBody + "<p>Employers have already begun to interview nurses for their 2021 openings. In fact, some of the positions throughout the country have already been filled. To ensure you are considered for the best nursing position, register for PracticeAlert to get the most up-to-date information about available nursing jobs:</p>";
                        MessageBody = MessageBody + "<p><a href='http://www.physiciansalert.org/register/candidateregistration.aspx?hldeid=" + b.ID + "'>http://www.physiciansalert.org/</a></p>";
                        MessageBody = MessageBody + "<p>The best way to learn about open nursing positions is directly from the hospitals and medical groups who are recruiting for nursing positions in your specific location. PracticeAlert works with physicians, nurse practitioners, physician assistants, and CRNAs to help make those connections.</p>";
                        MessageBody = MessageBody + "<p>We are not a search firm, and there is no cost for nurses to sign up. Our concept is simple: create a profile and tell us where you are seeking your next nursing job. PracticeAlert will then notify organizations in your area to help you find the best job possible.</p>";
                        MessageBody = MessageBody + "<p>Harold Foster<br/>";
                        MessageBody = MessageBody + "Director of Physician Outreach<br/>";
                        MessageBody = MessageBody + "PhysiciansAlert.org</p>";
                        MessageBody = MessageBody + "<p><a href='http://www.physiciansalert.org/optout.aspx?email=" + strEmail + "'>Unsubscribe</a></p>";

                        message.Body = ("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'><html xmlns='http://www.w3.org/1999/xhtml'><head><title></title></head><body>" + MessageBody + "</body></html>");
                        message.IsBodyHtml = true;
                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        System.Net.NetworkCredential AuthenticationInfo = new System.Net.NetworkCredential("blast@physiciansalert.org", "U7xpMd9!xm5Prdm");

                        smtp.Host = "smtp.physiciansalert.org";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = AuthenticationInfo;
                        smtp.Send(message);

                        Console.WriteLine("Sent " + b.ID);
                        Console.WriteLine("Saving Sent Record: " + b.ID);
                        if (b.TimesEmailed.HasValue == false)
                            b.TimesEmailed = 1;
                        else
                            b.TimesEmailed = b.TimesEmailed + 1;
                        int RandomDays = RandNum.Next(-14, -1);
                        b.LastEmailed = DateTime.Now;
                        b.LastEmailedHost = "physiciansalert.org";

                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage("bsherp@physiciansalert.net", "bsherp@alertservicescorp.com");
                        message.Subject = "Unverified Nurses E-mail Blast Failure for sendPhysicianAlert_Org_NurseBlast";
                        string strBody = "";
                        strBody = strBody + "CandidateBlastListId: " + b.ID + Environment.NewLine;
                        strBody = strBody + "E-mail: " + strEmail + Environment.NewLine;
                        strBody = strBody + "Error Report: " + ex.ToString() + Environment.NewLine;
                        message.Body = (strBody);
                        message.IsBodyHtml = false;
                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        System.Net.NetworkCredential AuthenticationInfo = new System.Net.NetworkCredential("bsherp@physiciansalert.net", "zAsu5hut");
                        smtp.Host = "smtp.physiciansalert.net";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = AuthenticationInfo;
                        smtp.Send(message);
                        b.DoNotEmail = true;
                        b.BadEmail = true;
                        db.SubmitChanges();
                    }
                }


            }
            db.Dispose();
            #endregion
            //            }
        }


    }
}
