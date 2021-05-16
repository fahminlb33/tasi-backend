using System.Linq;
using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TASI.Backend.Domain;
using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Infrastructure.Registrations
{
    public static class ValidatorRegistration
    {
        public static IMvcBuilder AddCustomValidator(this IMvcBuilder mvc)
        {
            mvc.ConfigureApiBehaviorOptions(setup =>
            {
                setup.InvalidModelStateResponseFactory = context =>
                {
                    return new BadRequestObjectResult(new ErrorModel(ErrorMessages.ModelValidation, ErrorCodes.ModelValidation,
                        context.ModelState.Select(x => new
                        {
                            Field = x.Key,
                            Message = x.Value.Errors.Aggregate("", (s, error) => $"{s}{error.ErrorMessage}, ")[..^2]
                        })));
                };
            });
            mvc.AddFluentValidation(options =>
            {
                options.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            });

            return mvc;
        }
    }
}
