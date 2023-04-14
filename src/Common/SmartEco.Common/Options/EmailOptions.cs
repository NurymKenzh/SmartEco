namespace SmartEco.Common.Options
{
    public class EmailOptions
    {
        public const string Email = "Email";

        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = default;
        public string DisplayName { get; set; } = string.Empty;
        public string SenderAddress { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseSsl { get; set; } = default;
    }
}
