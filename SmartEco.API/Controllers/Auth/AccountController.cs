using Microsoft.AspNetCore.Mvc;
using SmartEco.API.Services;
using SmartEco.Common.Models.Requests;
using SmartEco.Common.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartEco.API.Controllers.Auth
{
    public class AccountController : BaseController
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
            => _authService = authService;

        [SwaggerOperation("Получение токена авторизованного пользователя. Работает только для зарегистрированных пользователей")]
        [HttpPost(nameof(GetToken))]
        public async Task<ActionResult<AuthResponse>> GetToken(PersonRequest personReq)
            => AuthResponseResult(await _authService.GetToken(personReq));

        [SwaggerOperation("Регистрация нового пользователя")]
        [HttpPost(nameof(Register))]
        public async Task<ActionResult<AuthResponse>> Register(PersonRequest person)
            => AuthResponseResult(await _authService.Register(Url, Request.Scheme, person));

        [SwaggerOperation("Подтверждение регистрации")]
        [HttpPost(nameof(ConfirmEmail))]
        public async Task<ActionResult<AuthResponse>> ConfirmEmail(ConfirmRequest confirm)
            => AuthResponseResult(await _authService.ConfirmEmail(confirm));

        [HttpGet(nameof(GetAuthenticated))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool GetAuthenticated()
            => User.Identity is not null && User.Identity.IsAuthenticated;

        private ObjectResult AuthResponseResult((int statusCode, AuthResponse authResponse) result)
            => StatusCode(result.statusCode, result.authResponse);
    }
}
