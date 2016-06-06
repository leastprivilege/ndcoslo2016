using IdentityServer4.Models;
using System.Collections.Generic;

namespace Host.Configuration
{
    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Application",
                    RedirectUris = new List<string>
                    {
                        "http://localhost:3308/signin-oidc",
                        "http://localhost:3308/Account/OidcCallback"
                    },
                    AllowedScopes = new List<string>
                    {
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name
                    },
                }
            };
        }
    }
}