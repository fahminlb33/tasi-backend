using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace TASI.Backend.Infrastructure.Registrations
{
    public static class ValidatorRegistration
    {
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
