using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace ApiHost
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                //var p = new AuthorizationPolicyBuilder()
                //    .RequireClaim("scope", "api1")
                //    .Build();
                //options.Filters.Add(new AuthorizeFilter(p));
            })
            .AddJsonFormatters()
            .AddAuthorization();

        }

        public void Configure(IApplicationBuilder app)
        {
            //app.UseJwtBearerAuthentication(new JwtBearerOptions
            //{
            //    Authority = "http://localhost:5000",
            //    RequireHttpsMetadata = false,
            //    AutomaticAuthenticate = true,
            //    Audience = "http://localhost:5000/resources"
            //});

            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = "http://localhost:5000",
                RequireHttpsMetadata = false,
                ScopeName = "api1",
                AdditionalScopes = new string[] { "api1.readonly" },
                AutomaticAuthenticate = true
            });

            app.UseMvc();
        }
    }
}
