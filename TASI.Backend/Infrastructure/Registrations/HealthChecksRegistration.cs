using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Infrastructure.Registrations
{
    public static class HealthChecksRegistration
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck("backend", () => HealthCheckResult.Healthy())
                .AddDbContextCheck<TasiContext>("data-context");

                services.AddHealthChecksUI(settings =>
                {
                    settings.AddHealthCheckEndpoint("main", "/health");
                }).AddInMemoryStorage();

            return services;
        }

        public static IApplicationBuilder UseCustomHealthChecks(this IApplicationBuilder app)
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
