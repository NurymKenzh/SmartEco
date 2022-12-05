using Newtonsoft.Json;

namespace SmartEco.Common.Models.Responses
{
    public class AuthResponse
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("roleId")]
        public int? RoleId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
