using AutoWrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TASI.Backend.Infrastructure.Registrations
{
    public static class ResponseWrapperRegistration
    {
        public static void UseCustomResponseWrapper(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseApiResponseAndExceptionWrapper(new AutoWrapperOptions
            {
                IsDebug = env.IsDevelopment(),
                UseApiProblemDetailsException = env.IsDevelopment()
            });
        }
    }
}
