using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Threading;

namespace WebEnvironmentDataCollector.Util
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html);
    }

    internal class EmailService : IEmailService
    {
        private readonly string smtpHost;
        private readonly int smtpPort;
        private readonly string smtpPass;
        private readonly string smtpUser;

        public EmailService(string smtpHost, int smtpPort, string smtpPass, string smtpUser)
        {
            this.smtpHost = smtpHost;
            this.smtpPort = smtpPort;
            this.smtpPass = smtpPass;
            this.smtpUser = smtpUser;
        }

        public void Send(string to, string subject, string html)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(smtpUser));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(smtpHost, smtpPort, SecureSocketOptions.None);
            smtp.Authenticate(smtpUser, smtpPass);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}