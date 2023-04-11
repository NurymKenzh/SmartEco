using MimeKit;
using MailKit.Net.Smtp;
using SmartEco.Common.Options;
using Microsoft.Extensions.Options;

namespace SmartEco.Common.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _emailOptions;

        public EmailService(IOptions<EmailOptions> emailOptions) 
            => _emailOptions = emailOptions.Value;

        public async Task<bool> SendAsync(string[] emails, string subject, string message, string? attachmentPath = null)
        {
            try
            {
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress(_emailOptions.DisplayName, _emailOptions.SenderAddress));
                emailMessage.To.AddRange(AddValidMails(emails));
                emailMessage.Subject = subject;

                var builder = new BodyBuilder
                {
                    TextBody = message
                };

                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    var attachment = new MimePart
                    {
                        Content = new MimeContent(File.OpenRead(attachmentPath), ContentEncoding.Default),
                        FileName = Path.GetFileName(attachmentPath)
                    };
                    builder.Attachments.Add(attachment);
                }

                emailMessage.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailOptions.SmtpServer, _emailOptions.SmtpPort, _emailOptions.UseSsl);
                await client.AuthenticateAsync(_emailOptions.SenderName, _emailOptions.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static List<MailboxAddress> AddValidMails(string[] emails)
        {
            var validEmails = new List<MailboxAddress>();
            foreach (var email in emails)
            {
                if (MailboxAddress.TryParse(email, out MailboxAddress mailboxAddress))
                {
                    validEmails.Add(mailboxAddress);
                }
            }
            return validEmails;
        }
    }

    public interface IEmailService
    {
        public Task<bool> SendAsync(string[] emails, string subject, string message, string? fileName = null);
    }
}
