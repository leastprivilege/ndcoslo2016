using IdentityServer4.Models;
using System;
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
                    ClientId = "spa",
                    RedirectUris = new List<string>
                    {
                        "http://localhost:51961/index.html"
                    },
                    // yolo
                    AllowAccessToAllScopes = true,
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenType = AccessTokenType.Reference,

                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:51961"
                    }
                },

                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Application",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    AllowAccessTokensViaBrowser = false,
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
                        StandardScopes.OfflineAccess.Name,
                        "api1"
                    },
                    //AccessTokenLifetime = 3600, // seconds
                    //RefreshTokenUsage = TokenUsage.ReUse,
                    //UpdateAccessTokenClaimsOnRefresh = true,
                    //RefreshTokenExpiration = TimeSpan.FromDays(30),
                }
            };
        }
    }
}