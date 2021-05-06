using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Alura.ListaLeitura.Seguranca;
using Alura.ListaLeitura.WebApp.Models;
using Alura.WebAPI.WebApp.HttpClients;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AuthApiClient _auth;

        public UsuarioController(AuthApiClient authApiClient)
        {
            _auth = authApiClient;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _auth.PostLoginAync(model);

                if (result.Succeeded)
                {
                    await GuardaCookieTokenAsync(model, result.Token);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(String.Empty, "Erro na autenticação");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                var model = new LoginModel { Login = register.Login, Password = register.Password };
                var result = await _auth.PostRegisterAsync(model);

                if (result.Succeeded)
                {
                    await GuardaCookieTokenAsync(model, result.Token);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(String.Empty, "Erro ao criar cadastro: " + result.Token);
            }
            return View(register);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        private async Task GuardaCookieTokenAsync(LoginModel model, string token)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Login),
                new Claim("Token", token)
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }
    }
}