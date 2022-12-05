using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SmartEco.Web.Controllers.Auth;
using SmartEco.Web.Models;
using SmartEco.Web.Services.Providers;
using System.Diagnostics;

namespace SmartEco.Web.Controllers.Menu
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
            return isAuthenticated 
                ? View() 
                : RedirectToAction(
                    nameof(AccountController.Login),
                    GetName<AccountController>());
        }

        public IActionResult Directories()
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
    }
}