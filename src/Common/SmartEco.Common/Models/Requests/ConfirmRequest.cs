
namespace SmartEco.Common.Models.Requests
{
    public class ConfirmRequest
    {
        public string? Code { get; set; }
        public string? EmailСiphered { get; set; }
        public string? PasswordСiphered { get; set; }
    }
}
