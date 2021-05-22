using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TASI.Backend.Infrastructure.DataAnnotations
{
    public class NotEnumValueAttribute : ValidationAttribute
    {
        public object[] Values { get; set; }
        public Type Type { get; set; }

        public NotEnumValueAttribute(Type type, object[] values)
        {
            Type = type;
            Values = values;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Value must not be empty.");
            }

            if (Enum.TryParse(Type, value.ToString(), out var result))
            {
                if (Values.Any(x => x.Equals(result)))
                {
                    return new ValidationResult("The specified value is not valid");
                }
            }

            return ValidationResult.Success;
        }
        
    }
}
