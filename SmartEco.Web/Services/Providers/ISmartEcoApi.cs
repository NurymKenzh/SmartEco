using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartEco.Common.Models.Requests;
using SmartEco.Common.Models.Responses;
using SmartEco.Web.Models.Auth;

namespace SmartEco.Web.Services.Providers
{
    public class SmartEcoApi : BaseHttpClient, ISmartEcoApi
    {
        private readonly Dictionary<string, string> _headers;

        public SmartEcoApi(HttpClient httpClient, IOptions<MvcNewtonsoftJsonOptions> jsonOptions, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, jsonOptions)
        {
            string token = httpContextAccessor.HttpContext.Session.GetString("Token");
            _headers = new()
            {
                ["Authorization"] = $"Bearer {token}"
            };
        }

        public async Task<bool> GetAuthenticated()
            => await SendRequest<bool>(null, HttpMethod.Get, "api/Account/GetAuthenticated", _headers);

        public async Task<PersonViewModel> GetEmail()
            => await SendRequest<PersonViewModel>(null, HttpMethod.Get, "api/Account/GetEmail", _headers);

        public async Task<AuthResponse> Register(PersonRequest person)
            => await SendRequest<AuthResponse>(person, HttpMethod.Post, "api/Account/Register", _headers);

        public async Task<AuthResponse> ConfirmEmail(ConfirmRequest confirm)
            => await SendRequest<AuthResponse>(confirm, HttpMethod.Post, "api/Account/ConfirmEmail", _headers);

        public async Task<AuthResponse> GetToken(PersonRequest model)
            => await SendRequest<AuthResponse>(model, HttpMethod.Post, "api/Account/GetToken", _headers);

        public async Task<Result> GetObjectsByFilter<Result>(object model, string methodUrl)
            => await SendRequest<Result>(model, HttpMethod.Post, $"api/{methodUrl}", _headers);

        public async Task<Result> GetObjectById<Result>(long id, string methodUrl)
            => await SendRequest<Result>(null, HttpMethod.Get, $"api/{methodUrl}/{id}", _headers);

        public async Task<HttpResponseMessage> CreateObject(object model, string methodUrl)
            => await Execute(model, HttpMethod.Post, $"api/{methodUrl}", _headers);

        public async Task<HttpResponseMessage> UpdateObject(object model, string methodUrl)
            => await Execute(model, HttpMethod.Put, $"api/{methodUrl}", _headers);

        public async Task<HttpResponseMessage> DeleteObjectById(long id, string methodUrl)
            => await Execute(null, HttpMethod.Delete, $"api/{methodUrl}/{id}", _headers);
    }

    public interface ISmartEcoApi
    {
        Task<bool> GetAuthenticated();
        Task<PersonViewModel> GetEmail();
        Task<AuthResponse> Register(PersonRequest person);
        Task<AuthResponse> ConfirmEmail(ConfirmRequest confirm);
        Task<AuthResponse> GetToken(PersonRequest model);

        Task<Result> GetObjectsByFilter<Result>(object model, string methodUrl);
        Task<Result> GetObjectById<Result>(long id, string methodUrl);
        Task<HttpResponseMessage> CreateObject(object model, string methodUrl);
        Task<HttpResponseMessage> UpdateObject(object model, string methodUrl);
        Task<HttpResponseMessage> DeleteObjectById(long id, string methodUrl);
    }
}
