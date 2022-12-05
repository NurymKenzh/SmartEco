using MimeKit;
using MailKit.Net.Smtp;

namespace SmartEco.API.Services
{
    public class EmailService : IEmailService
    {
        const string Theme = "SmartEco",
            FromEmail = "smartecokz@gmail.com",
            //Password = "Qwerty123_",
            Password = "skqjcaiyizgljuak",
            SMTPServer = "smtp.gmail.com";
        const int SMTPPort = 465;

        public async Task<bool> SendAsync(string[] emails, string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress(Theme, FromEmail));
                foreach (var email in emails)
                {
                    emailMessage.To.Add(new MailboxAddress("", email));
                }
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = message
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(SMTPServer, SMTPPort, true);
                    await client.AuthenticateAsync(FromEmail, Password);
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public interface IEmailService
    {
        public Task<bool> SendAsync(string[] emails, string subject, string message);
    }
}
