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
                    AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256()),
                        new Secret("new_secret".Sha256())
                    },
                    Claims = new List<System.Security.Claims.Claim>
                    {
                        new System.Security.Claims.Claim("level", "topsecret")
                    },
                    RedirectUris = new List<string>
                    {
                        "http://localhost:3308/signin-oidc",
                        "http://localhost:3308/Account/OidcCallback"
                    },
                    AllowedScopes = new List<string>
                    {
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        "api1"
                    },
                }
            };
        }
    }
}