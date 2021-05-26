using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TASI.Backend.Domain;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Infrastructure.Registrations
{
    public static class NotFoundMiddlewareRegistration
    {
        public static IApplicationBuilder UseCustomNotFoundMiddleware(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    var originalPath = context.Request.Path.Value;
                    await context.Response.WriteAsJsonAsync(new ErrorModel(ErrorMessages.NotFound, ErrorCodes.NotFound, originalPath));
                    await next();
                }
            });

            return app;
        }
    }
}
