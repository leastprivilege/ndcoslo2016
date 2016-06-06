using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreAuthentication;

namespace AspNetCoreAuthentication
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(source =>
                {
                    source.Path = "appsettings.json";
                    source.ReloadOnChange = true;
                })
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddTransient<IAuthorizationHandler, BeerHandler>();
            services.AddTransient<UserRepository>();
            services.AddTransient<IAuthorizationHandler, CustomerAuthorizationHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SeniorSales", builder =>
                {
                    builder.RequireAuthenticatedUser()
                        .AddBeerRequirement("IPA")
                        .RequireClaim("role", "Geek");
                        //.RequireAssertion(context =>
                        //{
                        //    var ageValue = context.User.FindFirst("age")?.Value;
                        //    var age = Int32.Parse(ageValue);
                        //    return age >= 18 || context.User.HasClaim("age", "old");
                        //});
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,

                LoginPath = new PathString("/account/login"),
                AccessDeniedPath = new PathString("/account/forbidden"),

                CookieName = "foo"
            });

            app.UseClaimsTransformation(ctx =>
            {
                ctx.Principal.Identities.First().AddClaim(
                    new Claim("now", DateTime.Now.ToString()));

                return Task.FromResult(ctx.Principal);
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}