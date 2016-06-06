using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System;

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
                    new Claim("sub", userName),
                    new Claim("name", "Brock"),
                    new Claim("locale", "simplified english"),
                    new Claim("role", "Geek"),
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

        public IActionResult forbidden()
        {
            return View();
        }

        public IActionResult Google()
        {
            //HttpContext.Authentication.ChallengeAsync("Google");

            var props = new AuthenticationProperties
            {
                RedirectUri = "/account/register"
            };

            return Challenge(props, "Google");
        }

        public async Task<IActionResult> Register()
        {
            var extUser = await HttpContext.Authentication.AuthenticateAsync("Temp");
            var extSub = extUser.FindFirst(ClaimTypes.NameIdentifier);
            var issuer = extSub.Issuer;

            // go to db, ui etc

            var claims = new List<Claim>
                {
                    new Claim("sub", "jd88j8jj80"),
                    new Claim("name", "Brock"),
                    new Claim("locale", "simplified english"),
                    new Claim("role", "Geek"),
                    new Claim("email", extUser.FindFirst(ClaimValueTypes.Email).Value)
                };

            var id = new ClaimsIdentity(claims, "google", "name", "role");
            var p = new ClaimsPrincipal(id);

            await HttpContext.Authentication.SignInAsync("Cookies", p);
            await HttpContext.Authentication.SignOutAsync("Temp");

            return Redirect("/home/secure");
        }

        public IActionResult Oidc()
        {
            var url = "http://localhost:5000/connect/authorize" +
                    "?client_id=mvc" +
                    "&redirect_uri=http://localhost:3308/Account/OidcCallback" +
                    "&response_type=id_token" +
                    "&response_mode=form_post" +
                    "&nonce=123dontusethis" +
                    "&scope=openid";

            return Redirect(url);
        }

        [HttpPost]
        public async Task<IActionResult> OidcCallback(string id_token, string error)
        {
            var cert = "MIIDBTCCAfGgAwIBAgIQNQb+T2ncIrNA6cKvUA1GWTAJBgUrDgMCHQUAMBIxEDAOBgNVBAMTB0RldlJvb3QwHhcNMTAwMTIwMjIwMDAwWhcNMjAwMTIwMjIwMDAwWjAVMRMwEQYDVQQDEwppZHNydjN0ZXN0MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqnTksBdxOiOlsmRNd+mMS2M3o1IDpK4uAr0T4/YqO3zYHAGAWTwsq4ms+NWynqY5HaB4EThNxuq2GWC5JKpO1YirOrwS97B5x9LJyHXPsdJcSikEI9BxOkl6WLQ0UzPxHdYTLpR4/O+0ILAlXw8NU4+jB4AP8Sn9YGYJ5w0fLw5YmWioXeWvocz1wHrZdJPxS8XnqHXwMUozVzQj+x6daOv5FmrHU1r9/bbp0a1GLv4BbTtSh4kMyz1hXylho0EvPg5p9YIKStbNAW9eNWvv5R8HN7PPei21AsUqxekK0oW9jnEdHewckToX7x5zULWKwwZIksll0XnVczVgy7fCFwIDAQABo1wwWjATBgNVHSUEDDAKBggrBgEFBQcDATBDBgNVHQEEPDA6gBDSFgDaV+Q2d2191r6A38tBoRQwEjEQMA4GA1UEAxMHRGV2Um9vdIIQLFk7exPNg41NRNaeNu0I9jAJBgUrDgMCHQUAA4IBAQBUnMSZxY5xosMEW6Mz4WEAjNoNv2QvqNmk23RMZGMgr516ROeWS5D3RlTNyU8FkstNCC4maDM3E0Bi4bbzW3AwrpbluqtcyMN3Pivqdxx+zKWKiORJqqLIvN8CT1fVPxxXb/e9GOdaR8eXSmB0PgNUhM4IjgNkwBbvWC9F/lzvwjlQgciR7d4GfXPYsE1vf8tmdQaY8/PtdAkExmbrb9MihdggSoGXlELrPA91Yce+fiRcKY3rQlNWVd4DOoJ/cPXsXwry8pWjNCo5JD8Q+RQ5yZEy7YPoifwemLhTdsBz3hlZr28oCGJ3kbnpW0xGvQb3VHSTVVbeei0CfXoW6iz1";
            var x509 = new X509Certificate2(Convert.FromBase64String(cert));
            var param = new TokenValidationParameters
            {
                ValidIssuer = "http://localhost:5000",
                ValidAudience = "mvc",
                IssuerSigningKey = new X509SecurityKey(x509)
            };
            var h = new JwtSecurityTokenHandler();
            h.InboundClaimTypeMap.Clear();

            SecurityToken t;
            var cp = h.ValidateToken(id_token, param, out t);
            
            if (cp.FindFirst("nonce").Value != "123dontusethis")
            {
                throw new Exception("Bad nonce, Dr Jones");
            }

            await HttpContext.Authentication.SignInAsync("Cookies", cp);

            return Redirect("/Home/Secure");
        }
    }
}