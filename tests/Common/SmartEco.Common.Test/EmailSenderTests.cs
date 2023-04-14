using SmartEco.Common.Services;
using System.Collections;
using IOptions = Microsoft.Extensions.Options.Options;

namespace SmartEco.Common.Test
{
    public class EmailSenderTests
    {
        private readonly string[] validToEmails = { "ilyatstr@gmail.com" };
        private readonly string attachmentPathValid = @"C:\Users\AvtrElias\Desktop\test_attachment.txt";

        // Checking for the correct sending of an email
        [Fact]
        public async Task SendAsync_WithValidArguments_ReturnsTrue()
        {
            // Arrange
            var emailOptions = EmailOptionsMock.Create();
            var emailService = new EmailService(IOptions.Create(emailOptions));
            string[] validEmails = validToEmails;
            string subject = "Test email";
            string message = "This is a test email.";

            // Act
            var result = await emailService.SendAsync(validEmails, subject, message);

            // Assert
            Assert.True(result);
        }

        // Checking for correct handling if the SMTP port is invalid
        [Theory]
        [MemberData(nameof(InvalidOptions))]
        public async Task SendAsync_WithInvalidPortOptions_ReturnsTrue(int port, bool expected)
        {
            // Arrange
            var emailOptions = EmailOptionsMock.Create(smtpPort: port);
            var emailService = new EmailService(IOptions.Create(emailOptions));
            string[] validEmails = validToEmails;
            string subject = "Test email";
            string message = "This is a test email.";

            // Act
            var result = await emailService.SendAsync(validEmails, subject, message);

            // Assert
            Assert.Equal(expected, result);
        }

        // Checking for correct handling if the SMTP servers is invalid
        [Theory]
        [ClassData(typeof(EmailSenderTestServerOptions))]
        public async Task SendAsync_WithInvalidServerOptions_ReturnsTrue(string server, bool expected)
        {
            // Arrange
            var emailOptions = EmailOptionsMock.Create(smtpServer: server);
            var emailService = new EmailService(IOptions.Create(emailOptions));
            string[] validEmails = validToEmails;
            string subject = "Test email";
            string message = "This is a test email.";

            // Act
            var result = await emailService.SendAsync(validEmails, subject, message);

            // Assert
            Assert.Equal(expected, result);
        }

        // Checking for correct handling if the sender address of email is invalid
        [Fact]
        public async Task SendAsync_WithInvalidOptions_ReturnsFalse()
        {
            // Arrange
            var emailOptions = EmailOptionsMock.Create(senderAddress: "invalid-sender-email");
            var emailService = new EmailService(IOptions.Create(emailOptions));
            string[] validEmails = validToEmails;
            string subject = "Test email";
            string message = "This is a test email.";

            // Act
            var result = await emailService.SendAsync(validEmails, subject, message);

            // Assert
            Assert.True(result);
        }

        // Checking for correct handling if the list of email addresses is invalid
        [Theory]
        [InlineData(new[] { "invalid-to-email" }, false)]
        [InlineData(new[] { "invalid-to-email", "another-invalid-to-email" }, false)]
        public async Task SendAsync_WithInvalidEmail_ReturnsTrue(string[] emails, bool expected)
        {
            // Arrange
            var emailOptions = EmailOptionsMock.Create();
            var emailService = new EmailService(IOptions.Create(emailOptions));
            string[] invalidEmails = emails;
            string subject = "Test email";
            string message = "This is a test email.";

            // Act
            var result = await emailService.SendAsync(invalidEmails, subject, message);

            // Assert
            Assert.Equal(expected, result);
        }

        // Checking for the correct sending of an email with attachment
        [Fact]
        public async Task SendAsync_WithAttachment_ReturnsTrue()
        {
            // Arrange
            var emailOptions = EmailOptionsMock.Create();
            var emailService = new EmailService(IOptions.Create(emailOptions));
            string[] validEmails = validToEmails;
            string subject = "Test email";
            string message = "This is a test email.";
            string attachmentPath = attachmentPathValid;

            // Act
            var result = await emailService.SendAsync(validEmails, subject, message, attachmentPath);

            // Assert
            Assert.True(result);
        }

        public static IEnumerable<object[]> InvalidOptions
            => new List<object[]>
            {
                new object[] { 400, false },
                new object[] { 500, false }
            };
    }

    public class EmailSenderTestServerOptions : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "smtp.mail.yahoo.com", false };
            yield return new object[] { "smtp-mail.outlook.com", false };
            yield return new object[] { "smtp.office365.com", false };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}