﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCoreAuthentication.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!string.IsNullOrWhiteSpace(userName) &&
                userName == password)
            {
                var claims = new List<Claim>
                {
                    new Claim("sub", "99udjj3899jjjj"),
                    new Claim("name", "Brock"),
                    new Claim("locale", "simplified english"),
                    new Claim("role", "Geek")
                };

                var id = new ClaimsIdentity(claims, "password", "name", "role");
                var p = new ClaimsPrincipal(id);

                await HttpContext.Authentication.SignInAsync("Cookies", p);

                // might throw exception
                return LocalRedirect(returnUrl);
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            return Redirect("/");
        }

    }
}