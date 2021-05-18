using AutoWrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TASI.Backend.Infrastructure.Configs;

namespace TASI.Backend.Infrastructure.Registrations
{
    public static class ResponseWrapperRegistration
    {
        public static void UseCustomResponseWrapper(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseApiResponseAndExceptionWrapper<MapResponseObject>(new AutoWrapperOptions
            {
                IsDebug = env.IsDevelopment(),
                UseApiProblemDetailsException = env.IsDevelopment()
            });
        }
    }
}
