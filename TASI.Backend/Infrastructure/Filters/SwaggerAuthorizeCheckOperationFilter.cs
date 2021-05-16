using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TASI.Backend.Infrastructure.Filters
{
    public class SwaggerAuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.DeclaringType == null) return;
            var hasAuthorize = context.MethodInfo
                                   .DeclaringType
                                   .GetCustomAttributes(true)
                                   .OfType<AuthorizeAttribute>()
                                   .Any() ||
                               context.MethodInfo
                                   .GetCustomAttributes(true)
                                   .OfType<AuthorizeAttribute>()
                                   .Any();

            if (!hasAuthorize) return;
            operation.Responses.TryAdd("401", new OpenApiResponse {Description = "Unauthorized"});
            operation.Responses.TryAdd("403", new OpenApiResponse {Description = "Forbidden"});

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                }
            };
        }
    }
}
