using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DatabaseContext.Logger;
using DatabaseContext.Managers;
using DatabaseContext.Models;
using DatabaseContext.Models.Enums;
using MailService.Properties;

namespace MailService
{
    public class MailSender
    {
        private static EmailManager emailManager = new EmailManager();

        private static SmtpClient smtpClient = new SmtpClient()
        {
            Credentials = new System.Net.NetworkCredential()
            {
                Password = Settings.Default.Password,
                UserName = Settings.Default.UserName
            },
            Port = Settings.Default.SMTPPort,
            EnableSsl = Settings.Default.EnableSSL,
            Host = Settings.Default.SMTPHost
        };

        private static bool stop = false;
        public static void StartMailSenderSheduler()
        {
            while (!stop)
            {
                try
                {
                    ProcessMailSending();
                    Thread.Sleep(Settings.Default.RunTaskFrequency);
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogException(ex);
                }
            }
        }

        private static void ProcessMailSending()
        {
            try
            {
                var emailsToSend = emailManager.GetAllToSend();
                foreach (var email in emailsToSend)
                {
                    SendEmail(email);
                }
                emailManager.SaveAll(emailsToSend);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex);
            }
        }

        private static void SendEmail(Email email)
        {
            try
            {
                MailMessage mail = new MailMessage(Settings.Default.EmailFrom, email.RecipientEmail);
                mail.Body = email.Body;
                mail.IsBodyHtml = true;
                mail.Sender = new MailAddress(Settings.Default.EmailFrom);
                mail.Subject = email.Subject;
                smtpClient.Send(mail);
                email.SentOn = DateTime.Now.ToUniversalTime();
                email.Status = EmailStatus.Sent;
                LoggerHelper.LogException($"Notification #{email.Id} has been sent successfully.");
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex);
                email.Status = EmailStatus.Error;
            }
        }

        public static void RequestStop()
        {
            stop = true;
        }

        public static void RequestContinue()
        {
            stop = false;
        }
    }
}
