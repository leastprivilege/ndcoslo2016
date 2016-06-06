using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClaimsConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var claims = new Claim[] {
                new Claim("sub", "1389jnjk4h484ij4"),
                new Claim("name", "Dom", ClaimValueTypes.String, "Dom's mum"),
                new Claim("age", "old"),
                new Claim("email", "Dom@gmail.com"),
                new Claim("role", "Geek"),
            };

            var ci = new ClaimsIdentity(claims, "password", "email", "role");
            var cp = new ClaimsPrincipal(ci);

            Console.WriteLine(cp.Identity.Name);
            Console.WriteLine(cp.IsInRole("Geek"));



            //Console.WriteLine("{0}, {1}, {2}", 
            //    claim.Type,
            //    claim.Value,
            //    claim.Issuer);
        }
    }
}
