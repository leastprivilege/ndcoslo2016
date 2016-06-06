using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Pipeline
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            //app.Use(async (ctx, next) =>
            //{
            //    // inbound
            //    Console.WriteLine(ctx.Request.Path);

            //    await next();

            //    // outbound
            //    Console.WriteLine(ctx.Response.StatusCode);
            //});

            //app.UseMiddleware<LoggingMiddleware>(new LoggingOptions { Name = "NDC" });

            app.UseLogging(new LoggingOptions { Name = "NDC" });

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }

    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LoggingOptions _options;

        public LoggingMiddleware(RequestDelegate next, LoggingOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext ctx)
        {
            // inbound
            Console.WriteLine(_options.Name);
            Console.WriteLine(ctx.Request.Path);

            await _next(ctx);

            // outbound
            Console.WriteLine(ctx.Response.StatusCode);
        }
    }

    public class LoggingOptions
    {
        public string Name { get; set; }
    }

    public static class LoggingExtensions
    {
        public static IApplicationBuilder UseLogging(this IApplicationBuilder app, LoggingOptions options)
        {
            return app.UseMiddleware<LoggingMiddleware>(options);
        }
    }
}
