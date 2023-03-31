using Refit;
using SmartEco.Common.Models.Requests;
using SmartEco.Common.Models.Responses;
using SmartEco.Web.Models.Auth;

namespace SmartEco.Web.Services.Providers
{
    public interface ISmartEcoApi
    {
        #region Auth
        [Get("/api/Account/GetAuthenticated")]
        Task<bool> GetAuthenticated();

        [Get("/api/Account/GetEmail")]
        Task<PersonViewModel> GetEmail();

        [Post("/api/Account/Register")]
        Task<AuthResponse> Register(PersonRequest person);

        [Post("/api/Account/ConfirmEmail")]
        Task<AuthResponse> ConfirmEmail(ConfirmRequest confirm);

        [Post("/api/Account/GetToken")]
        Task<AuthResponse> GetToken(PersonRequest model);
        #endregion Auth

        #region Generic CRUD
        [Post("/api/{**page}")]
        Task<TResponse> GetObjectsByFilter<TResponse>(string page, object model);

        [Get("/api/{**page}/{id}")]
        Task<TResponse> GetObjectById<TResponse>(string page, long id);

        [Post("/api/{**page}")]
        Task<HttpResponseMessage> CreateObject(string page, object model);

        [Put("/api/{**page}")]
        Task<HttpResponseMessage> UpdateObject(string page, object model);

        [Delete("/api/{**page}/{id}")]
        Task<HttpResponseMessage> DeleteObjectById(string page, long id);
        #endregion Generic CRUD
    }
}
