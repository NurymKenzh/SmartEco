using Newtonsoft.Json;
using Refit;
using SmartEco.Common.Models.Requests;
using SmartEco.Common.Models.Responses;
using SmartEco.Web.Models.Auth;
using SmartEco.Web.Services.Providers;
using System.Net;

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
            var authResponse = await GetAuthResponse(personRequest, _smartEcoApi.Register);
            return authResponse?.Message;
        }

        public async Task<string> ConfirmEmail(ConfirmRequest confirm)
        {
            var authResponse = await GetAuthResponse(confirm, _smartEcoApi.ConfirmEmail);
            return authResponse?.Message;
        }

        public async Task<AuthResponse> GetToken(ISession session, string email, string password)
        {
            var person = new PersonRequest
            {
                Email = email,
                Password = password
            };
            var authResponse = await GetAuthResponse(person, _smartEcoApi.GetToken);
            SetSession(session, authResponse.AccessToken, authResponse.RoleId, authResponse.Email);
            return authResponse;
        }

        public void ResetSession(ISession session)
        {
            session.Remove("Token");
            session.Remove("Role");
            session.Remove("Email");
        }

        private static void SetSession(ISession session, string token, int? role, string email)
        {
            if (token is not null)
                session.SetString("Token", token);
            if (role is not null)
                session.SetInt32("Role", (int)role);
            if (email is not null)
                session.SetString("Email", email);
        }

        //For display error if invalid request data (BadRequest)
        private static async Task<AuthResponse> GetAuthResponse<TRequest>(TRequest request, Func<TRequest, Task<AuthResponse>> authRequest)
        {
            try
            {
                return await authRequest(request);
            }
            catch (ApiException exception)
            {
                if (exception.StatusCode is HttpStatusCode.BadRequest)
                    return JsonConvert.DeserializeObject<AuthResponse>(exception.Content);

                throw exception;
            }
        }
    }
}
