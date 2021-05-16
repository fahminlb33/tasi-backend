using System;
using System.Net.NetworkInformation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Infrastructure.Registrations
{
    public static class HealthChecksRegistration
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration config)
        {
            services.AddHealthChecks()
                .AddCheck("backend", () => HealthCheckResult.Healthy())
                .AddDbContextCheck<TasiContext>("data-context")
                .AddSqlite(config.GetConnectionString("DefaultConnection"), name: "sqlite")
                .AddAsyncCheck("internet", async _ =>
                {
                    try
                    {
                        var result = await new Ping().SendPingAsync("google.com");
                        return result.Status == IPStatus.Success
                            ? HealthCheckResult.Healthy()
                            : HealthCheckResult.Degraded();
                    }
                    catch (Exception ex)
                    {
                        return HealthCheckResult.Degraded(ex.Message);
                    }
                });
            
            services.AddHealthChecksUI(settings =>
                {
                    settings.AddHealthCheckEndpoint("main", "/health");
                }).AddSqliteStorage(config.GetConnectionString("HealthChecksConnection"));

            return services;
        }

        public static IApplicationBuilder UseCustomHealthChecks(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI(options =>
            {
                options.ApiPath = "/health-api";
                options.UIPath = "/health-ui";
                //options.AddCustomStylesheet($"{env.ContentRootPath}/Infrastrcuture")
            });

            return app;
        }

        public static void MapCustomHealthChecks(this IEndpointRouteBuilder route)
        {
            route.MapHealthChecks("/health");
            route.MapHealthChecksUI();
        }
    }
}
