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
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

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

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "oidc",
                SignInScheme = "Cookies",
                Authority = "http://localhost:5000",
                RequireHttpsMetadata = false,
                ClientId = "mvc",
                ClientSecret = "secret",
                ResponseType = "code id_token",
                Scope = {"openid", "profile", "api1", "offline_access"},
                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true, // stores tokens in cookie properties
                Events = new OpenIdConnectEvents
                {
                    //OnTicketReceived = n =>
                    //{
                    //    var ci = new ClaimsIdentity(
                    //        n.Principal.Claims.Where(x => x.Type == "sub"),
                    //        "external", "name", "role");
                    //    var cp = new ClaimsPrincipal(ci);

                    //    n.Principal = cp;

                    //    return Task.CompletedTask;
                    //}
                }
            });

            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationScheme = "Temp",
            //    AutomaticAuthenticate = false
            //});

            //app.UseGoogleAuthentication(new GoogleOptions
            //{
            //    AuthenticationScheme = "Google",
            //    SignInScheme = "Temp",

            //    ClientId = "998042782978-2moehouvbskg2e0ms04pgeqbj7n545h4.apps.googleusercontent.com",
            //    ClientSecret = "-3HkpRE3rATPUp_-Vag7WeoI"
            //});

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