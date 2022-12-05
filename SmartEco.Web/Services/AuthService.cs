using SmartEco.Common.Models.Requests;
using SmartEco.Common.Models.Responses;
using SmartEco.Web.Models.Auth;
using SmartEco.Web.Services.Providers;

namespace SmartEco.Web.Services
{
    public class AuthService
    {
        private readonly ISmartEcoApi _smartEcoApi;

        public AuthService(ISmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        public async Task<string> Register(PersonAuthViewModel model)
        {
            var personRequest = new PersonRequest
            {
                Email = model.Email,
                Password = model.Password
            };
            var authResponse = await _smartEcoApi.Register(personRequest);
            return authResponse?.Message;
        }

        public async Task<string> ConfirmEmail(ConfirmRequest confirm)
        {
            var authResponse = await _smartEcoApi.ConfirmEmail(confirm);
            return authResponse?.Message;
        }

        public async Task<AuthResponse> GetToken(ISession session, string email, string password)
        {
            var person = new PersonRequest
            {
                Email = email,
                Password = password
            };
            var authResponse = await _smartEcoApi.GetToken(person) ?? new AuthResponse();
            SetSession(session, authResponse.AccessToken, authResponse.RoleId, authResponse.Email);
            return authResponse;
        }

        public void ResetSession(ISession session)
        {
            session.Remove("Token");
            session.Remove("Role");
            session.Remove("Email");
        }

        private void SetSession(ISession session, string token, int? role, string email)
        {
            if (token is not null)
                session.SetString("Token", token);
            if (role is not null)
                session.SetInt32("Role", (int)role);
            if (email is not null)
                session.SetString("Email", email);
        }
    }
}
