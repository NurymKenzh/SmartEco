using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartEco.Akimato.Models;
using SmartEcoAPI.Models.Account;

namespace SmartEco.Akimato.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;
        public AccountController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetToken(string Token, string Role)
        {
            if(string.IsNullOrEmpty(Token))
            {
                HttpContext.Session.Remove("Token");
            }
            else
            {
                HttpContext.Session.SetString("Token", Token);
            }
            if (string.IsNullOrEmpty(Role))
            {
                HttpContext.Session.Remove("Role");
            }
            else
            {
                HttpContext.Session.SetString("Role", Role);
            }
            string message = "OK";
            return Json(new
            {
                message
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Person model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync("api/Account/Register", model);

                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var authResponse = await response.Content.ReadAsAsync<AuthResponse>();
                    ViewBag.ResultMessage = authResponse.Message;
                }
                else
                {
                    string OutputViewText = await response.Content.ReadAsStringAsync();
                    ViewBag.ResultMessage = OutputViewText;
                }
            }
            return View(model);
        }
    }
}