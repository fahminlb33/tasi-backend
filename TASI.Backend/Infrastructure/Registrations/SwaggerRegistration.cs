using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using TASI.Backend.Infrastructure.Filters;

namespace TASI.Backend.Infrastructure.Registrations
{
    public static class SwaggerRegistration
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, string title, string version)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = title, 
                    Version = version
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    Description = "Enter 'Bearer' following by space and JWT.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,

                });

                options.OperationFilter<SwaggerAuthorizeCheckOperationFilter>();
                //options.AddFluentValidationRules();
                options.EnableAnnotations();
            });

            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, string name, string version)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/{version}/swagger.json", name));

            return app;
        }
    }
}
