using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartEco.API.Services;
using SmartEco.Common.Models.Requests;
using SmartEco.Common.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using Formatting = Newtonsoft.Json.Formatting;

namespace SmartEco.API.Controllers.Auth
{
    public class AccountController : BaseController
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [SwaggerOperation("Получение токена авторизованного пользователя. Работает только для зарегистрированных пользователей")]
        [HttpPost(nameof(GetToken))]
        public async Task GetToken(PersonRequest personReq)
        {
            var (authResponse, httpStatusCode) = await _authService.GetToken(personReq);
            await SendAuthResponse(authResponse, httpStatusCode);
        }

        [SwaggerOperation("Регистрация нового пользователя")]
        [HttpPost(nameof(Register))]
        public async Task Register(PersonRequest person)
        {

            var (authResponse, httpStatusCode) = await _authService.Register(Url, Request.Scheme, person);
            await SendAuthResponse(authResponse, httpStatusCode);
        }

        [SwaggerOperation("Подтверждение регистрации")]
        [HttpPost(nameof(ConfirmEmail))]
        public async Task ConfirmEmail(ConfirmRequest confirm)
        {
            var (authResponse, httpStatusCode) = await _authService.ConfirmEmail(confirm);
            await SendAuthResponse(authResponse, httpStatusCode);
        }

        [HttpGet(nameof(GetAuthenticated))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool GetAuthenticated()
            => User.Identity is not null && User.Identity.IsAuthenticated;

        private async Task SendAuthResponse(AuthResponse response, HttpStatusCode statusCode)
        {
            Response.StatusCode = (int)statusCode;
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}
