using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

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

    }
}
