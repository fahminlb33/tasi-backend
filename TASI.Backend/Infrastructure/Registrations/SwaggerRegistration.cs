using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using TASI.Backend.Infrastructure.Filters;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

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

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Enter JWT token from user login API.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                });

                options.OperationFilter<SwaggerOptionalRouteParameterOperationFilter>();
                options.OperationFilter<SwaggerAuthorizeCheckOperationFilter>();

                options.EnableAnnotations();
                options.DescribeAllParametersInCamelCase();
                options.AddEnumsWithValuesFixFilters();
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
