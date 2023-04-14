using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Refit;
using SmartEco.Web.Controllers.Auth;
using SmartEco.Web.Models;
using SmartEco.Web.Services.Providers;
using System.Diagnostics;
using System.Net;

namespace SmartEco.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ISmartEcoApi _smartEcoApi;

        public HomeController(ISmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        public async Task<IActionResult> Index()
        {
            var isAuthenticated = await _smartEcoApi.GetAuthenticated();
            return isAuthenticated ? View() : RedirectToLogin();
        }

        public new IActionResult Directories()
        {
            return View();
        }

        public IActionResult Data()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandler = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandler?.Error as ApiException;
            if (exception?.StatusCode is HttpStatusCode.Unauthorized)
                return RedirectToLogin();

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = (int?)exception?.StatusCode,
                Message = exception?.Message ?? exceptionHandler?.Error?.Message
            });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        private IActionResult RedirectToLogin()
            => RedirectToAction(
                nameof(AccountController.Login),
                GetName<AccountController>());
    }
}