using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace TASI.Backend.Infrastructure.Registrations
{
    public static class ValidatorRegistration
    {
        public static IServiceCollection AddCustomValidators(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

            return services;
        }

        public static IMvcBuilder AddCustomValidator(this IMvcBuilder mvc)
        {
            mvc.AddFluentValidation(options =>
            {
                options.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            });

            return mvc;
        }
    }
}
