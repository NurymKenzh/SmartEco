using SmartEco.Common.Options;

namespace SmartEco.Common.Test
{
    internal static class EmailOptionsMock
    {
        internal static EmailOptions Create(
            string smtpServer = "smtp.gmail.com",
            int smtpPort = 465,
            string displayName = "SmartEco",
            string senderAddress = "smartecokz@gmail.com",
            string senderName = "smartecokz@gmail.com",
            string password = "skqjcaiyizgljuak",
            bool useSsl = true)
            => new()
            {
                SmtpServer = smtpServer,
                SmtpPort = smtpPort,
                DisplayName = displayName,
                SenderAddress = senderAddress,
                SenderName = senderName,
                Password = password,
                UseSsl = useSsl
            };
    }
}
