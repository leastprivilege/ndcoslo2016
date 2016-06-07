using IdentityServer4.Models;
using System.Collections.Generic;

namespace Host.Configuration
{
    public class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            return new List<Scope>
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                new Scope
                {
                    Name = "api1",
                    DisplayName = "Your API 1",
                    Type = ScopeType.Resource
                }
            };
        }
    }
}