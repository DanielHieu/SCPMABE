using System.Net;
using System.Net.Mail;

namespace ScpmaBe.Services.Common
{
    public interface IEmailHelper
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true, List<string> cc = null, List<string> bcc = null);
    }

    /// <summary>
    /// Helper class for sending emails
    /// </summary>
    public class EmailHelper : IEmailHelper
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _enableSsl;
        private readonly string _senderEmail;
        private readonly string _senderName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailHelper"/> class.
        /// </summary>
        /// <param name="smtpHost">SMTP server host</param>
        /// <param name="smtpPort">SMTP server port</param>
        /// <param name="smtpUsername">SMTP authentication username</param>
        /// <param name="smtpPassword">SMTP authentication password</param>
        /// <param name="enableSsl">Whether to use SSL for the SMTP connection</param>
        /// <param name="senderEmail">Default sender email address</param>
        /// <param name="senderName">Default sender display name</param>
        public EmailHelper(AppSettings appSettings)
        {
            _smtpHost = appSettings.EmailSettings.SmtpServer;
            _smtpPort = appSettings.EmailSettings.SmtpPort;
            _smtpUsername = appSettings.EmailSettings.Username;
            _smtpPassword = appSettings.EmailSettings.Password;
            _enableSsl = true;
            _senderEmail = appSettings.EmailSettings.SenderEmail;
            _senderName = appSettings.EmailSettings.SenderName;
        }

        /// <summary>
        /// Sends an email asynchronously with the specified parameters.
        /// </summary>
        /// <param name="to">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body content</param>
        /// <param name="isHtml">Whether the body content is HTML</param>
        /// <param name="cc">Optional CC recipients</param>
        /// <param name="bcc">Optional BCC recipients</param>
        /// <param name="attachments">Optional file attachments</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if the email was sent successfully, otherwise false</returns>
        public async Task<bool> SendEmailAsync(
            string to,
            string subject,
            string body,
            bool isHtml = true,
            List<string> cc = null,
            List<string> bcc = null)
        {
            try
            {
                using var message = CreateEmailMessage(to, subject, body, isHtml, cc, bcc);
                using var client = CreateSmtpClient();

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private MailMessage CreateEmailMessage(
            string to,
            string subject,
            string body,
            bool isHtml,
            List<string> cc,
            List<string> bcc)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_senderEmail, _senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(to);

            if (cc != null)
            {
                foreach (var ccAddress in cc)
                {
                    if (!string.IsNullOrWhiteSpace(ccAddress))
                    {
                        message.CC.Add(ccAddress);
                    }
                }
            }

            if (bcc != null)
            {
                foreach (var bccAddress in bcc)
                {
                    if (!string.IsNullOrWhiteSpace(bccAddress))
                    {
                        message.Bcc.Add(bccAddress);
                    }
                }
            }

            return message;
        }

        private SmtpClient CreateSmtpClient()
        {
            var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = _enableSsl
            };

            return client;
        }
    }
}
