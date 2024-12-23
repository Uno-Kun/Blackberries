using Microsoft.Extensions.Options;
using Blackberries.Models;
using System.Net;
using System.Net.Mail;

namespace Blackberries.Services
{
    public class EmailNotificationService
    {
        private static SmtpClient CreateSmtpClient() 
        {
            var smtpSettingsConfig = ServiceProvider.Instance.GetRequiredService<IOptions<SmtpSettingsConfig>>().Value;

            return new SmtpClient { 
                Host = smtpSettingsConfig.Host,
                Port = smtpSettingsConfig.Port,
                EnableSsl = smtpSettingsConfig.EnableSSL,
                Timeout = smtpSettingsConfig.Timeout,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpSettingsConfig.Login, smtpSettingsConfig.Password)
            };
        }

        public static void Send(string subject, string messageBody, string senderName, string receiver) 
        {
            var smtpSettingsConfig = ServiceProvider.Instance.GetRequiredService<IOptions<SmtpSettingsConfig>>().Value;

            using (var smtpClient = CreateSmtpClient()) 
            {
                try
                {
                    var from = new MailAddress(smtpSettingsConfig.Login, senderName);

                    var to = new MailAddress(receiver);

                    using (var mailMessage = new MailMessage(from, to))
                    {
                        mailMessage.Subject = subject;
                        mailMessage.Body = messageBody;
                        mailMessage.IsBodyHtml = false;

                        smtpClient.Send(mailMessage);
                    }
                }
                catch (Exception ex) 
                {
                }
            }
        }
    }
}
