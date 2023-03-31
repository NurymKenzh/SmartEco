using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartEco.Common.Models.Requests;
using SmartEco.Web.Helpers.Constants;
using SmartEco.Web.Models.Auth;
using SmartEco.Web.Services;

namespace SmartEco.Web.Controllers.Auth
{
    public class AccountController : BaseController
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login()
        {
            return View(Views.Get(HttpContext, Auth));
        }

        [HttpPost]
        public async Task<IActionResult> Login(PersonAuthViewModel person)
        {
            var authResponse = await _authService.GetToken(HttpContext.Session, person.Email, person.Password);
            if (string.IsNullOrEmpty(authResponse.AccessToken))
            {
                ViewBag.Message = authResponse.Message;
                return View(Views.Get(HttpContext, Auth), person);
            }
            else
            {
                return RedirectToAction(
                    nameof(Index),
                    GetName<HomeController>());
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(Views.Get(HttpContext, Auth));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(PersonAuthViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                ViewBag.ResultMessage = await _authService.Register(model);
            }
            return View(Views.Get(HttpContext, Auth), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(ConfirmRequest confirm)
        {
            ViewBag.ResultMessage = await _authService.ConfirmEmail(confirm);
            return View(Views.Get(HttpContext, Auth));
        }

        [HttpGet]
        public string GetEmail()
            => HttpContext.Session.GetString("Email");

        [HttpPost]
        public void Logout()
            => _authService.ResetSession(HttpContext.Session);
    }
}
