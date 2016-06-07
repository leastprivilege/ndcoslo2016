using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using IdentityModel.Client;

namespace AspNetCoreAuthentication.Controllers
{
    public class HomeController : Controller
    {
        private IAuthorizationService _authorization;

        public HomeController(IAuthorizationService authorization)
        {
            _authorization = authorization;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secure()
        {
            return View();
        }

        [Authorize("SeniorSales")]
        public IActionResult DoSalesStuff()
        {
            return View("Secure");
        }

        public async Task<IActionResult> EditCustomer(int customerId)
        {
            //var c = LoadCustomerFromDb(customerId);

            var c = new Customer
            {
                Id = customerId, Region = "US", SalesRep = "dom"
            };

            if (!(await _authorization.AuthorizeAsync(User, c, CustomerOperations.Edit)))
            {
                return Challenge();
            }

            // ok, do real work now...

            return View();
        }

        public async Task<IActionResult> CallApiAsUser()
        {
            var access_token = await HttpContext.Authentication.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var data = await client.GetStringAsync("http://localhost:48791/test");

            ViewData["data"] = data;
            return View("ApiResult");
        }

        public async Task<IActionResult> CallApi()
        {
            var cid = "mvc";
            var secret = "secret";

            var tokenClient = new TokenClient("http://localhost:5000/connect/token", cid, secret);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");
            var access_token = tokenResponse.AccessToken;

            //var values = new Dictionary<string, string>()
            //{
            //    { "grant_type", "client_credentials" },
            //    { "scope", "api1" },
            //    { "client_id", cid },
            //    { "client_secret", secret }
            //};

            //var client = new HttpClient();
            //var tokenResponse = await client.PostAsync("http://localhost:5000/connect/token", new FormUrlEncodedContent(values));

            //tokenResponse.EnsureSuccessStatusCode();

            //var json = await tokenResponse.Content.ReadAsStringAsync();
            //var response = JObject.Parse(json);
            //var access_token = response["access_token"].ToString();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var data = await client.GetStringAsync("http://localhost:48791/test");

            ViewData["data"] = data;
            return View("ApiResult");
        }

    }
}
