using Microsoft.Extensions.Options;
using Moq;
using SmartEco.Common.Options;
using SmartEco.Common.Services;
using System.Collections;

namespace SmartEco.Common.Test
{
    /// <summary>
    /// Each test class is a unique test collection and tests under it will run in sequence
    /// So if put all of tests in same collection then it will run sequentially:
    /// 1.Initialization in the constructor (EmailSenderTests)
    /// 2. Running the first test (SendAsync_WithValidArguments_ReturnsTrue)
    /// 3.Initialization in the constructor (EmailSenderTests)
    /// 4. Running the first test (SendAsync_WithInvalidPortOptions_ReturnsTrue) etc.
    /// </summary>
    public class EmailSenderTests
    {
        private readonly string[] validToEmails = { "example@gmail.com" };
        private readonly string attachmentPathValid = @"C:\Users\%UserName%\Desktop\test_attachment.txt";
        private readonly EmailOptions _emailOptions;
        private readonly Mock<IOptions<EmailOptions>> _mockEmailOptions;

        public EmailSenderTests()
        {
            _emailOptions = SetDefaultEmailOptions();
            _mockEmailOptions = new Mock<IOptions<EmailOptions>>();
            _mockEmailOptions.Setup(x => x.Value).Returns(_emailOptions);
        }

        // Checking for the correct sending of an email
        [Fact]
        public async Task SendAsync_WithValidArguments_ReturnsTrue()
        {
            // Arrange
            var emailService = new EmailService(_mockEmailOptions.Object);
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
            _emailOptions.SmtpPort = port;
            var emailService = new EmailService(_mockEmailOptions.Object);
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
            _emailOptions.SmtpServer = server;
            var emailService = new EmailService(_mockEmailOptions.Object);
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
        public async Task SendAsync_WithInvalidSenderOptions_ReturnsFalse()
        {
            // Arrange
            _emailOptions.SenderAddress = "invalid-sender-email";
            var emailService = new EmailService(_mockEmailOptions.Object);
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
            var emailService = new EmailService(_mockEmailOptions.Object);
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
            var emailService = new EmailService(_mockEmailOptions.Object);
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

        private static EmailOptions SetDefaultEmailOptions()
            => new()
            {
                SmtpServer = "smtp.gmail.com",
                SmtpPort = 465,
                DisplayName = "SmartEco",
                SenderAddress = "smartecokz@gmail.com",
                SenderName = "smartecokz@gmail.com",
                Password = "skqjcaiyizgljuak",
                UseSsl = true
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