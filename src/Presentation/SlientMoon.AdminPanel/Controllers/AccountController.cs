using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.AdminPanel.Models;
using SlientMoon.Application.Features.Auth.Commands.Login;
using SlientMoon.Application.Interfaces.Services;

namespace SlientMoon.AdminPanel.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IDispatcher _dispatcher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IValidator<LoginViewModel> _loginValidator;

        public AccountController(
            IDispatcher dispatcher,
            IJwtTokenService jwtTokenService,
            IValidator<LoginViewModel> loginValidator)
        {
            _dispatcher = dispatcher;
            _jwtTokenService = jwtTokenService;
            _loginValidator = loginValidator;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var validationResult = await _loginValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);

                return View(model);
            }

            var loginResult = await _dispatcher.Send(new LoginCommand
            {
                Email = model.Email,
                Password = model.Password
            });

            if (!loginResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Email və ya parol yanlışdır.");
                return View(model);
            }

            var principal = _jwtTokenService.ValidateToken(loginResult.Value.AccessToken);

            var role = principal?.FindFirst("role")?.Value;
            if (principal is null || role != "Admin")
            {
                ModelState.AddModelError(string.Empty, "Bu panelə girişiniz yoxdur.");
                return View(model);
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
