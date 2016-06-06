using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreAuthentication
{
    public class BeerRequirement : IAuthorizationRequirement
    {
        public string Type { get; set; }
    }

    public class UserRepository
    {
        public string GetFavBeerType(string sub)
        {
            if (sub == "dom")
            {
                return "Gose";
            }

            return "Lager";
        }
    }

    public static class BeerExtensions
    {
        public static AuthorizationPolicyBuilder AddBeerRequirement(this AuthorizationPolicyBuilder builder, string type)
        {
            return builder.AddRequirements(new BeerRequirement { Type = type });
        }
    }

    public class BeerHandler : AuthorizationHandler<BeerRequirement>
    {
        private UserRepository _repo;

        public BeerHandler(UserRepository repo)
        {
            _repo = repo;
        }

        protected override void Handle(
            AuthorizationContext context, 
            BeerRequirement requirement)
        {
            var sub = context.User.FindFirst("sub")?.Value;
            var favBeer = _repo.GetFavBeerType(sub);

            if (favBeer == "Lager")
            {
                context.Fail();
            }

            if (favBeer == requirement.Type)
            {
                context.Succeed(requirement);
            }
        }
    }
}
