using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartEco.Akimato.Models;

namespace SmartEco.Akimato.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public HomeController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await _HttpApiClient.GetAsync("api/Account/GetAuthenticated");
            string OutputViewText = await response.Content.ReadAsStringAsync();
            OutputViewText = OutputViewText.Replace("\"", "");
            if (Convert.ToBoolean(OutputViewText) == true)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        public IActionResult Data()
        {
            return View();
        }
        public IActionResult Directories()
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
