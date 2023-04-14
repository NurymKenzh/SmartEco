
namespace SmartEco.Common.Models.Responses
{
    public class AuthResponse
    {
        public string? AccessToken { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }

        public int? RoleId { get; set; }

        public string? Message { get; set; }
    }
}
