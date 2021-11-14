using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
                    var data = context.ModelState
                        .Where(x => x.Value.ValidationState != ModelValidationState.Valid)
                        .Select(x => new
                        {
                            Field = x.Key,
                            Message = x.Value.Errors.Aggregate("", (s, error) => $"{s}{error.ErrorMessage}, ")[..^2]
                        });

                    var model = new ErrorModel(ErrorMessages.ModelValidation, ErrorCodes.ModelValidation, data);
                    return new BadRequestObjectResult(model);
                };
            });

            return mvc;
        }
    }
}
