using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TASI.Backend.Infrastructure.DataAnnotations
{
    public class MustEnumValueAttribute : ValidationAttribute
    {
        public object[] Values { get; set; }
        public Type Type { get; set; }

        public MustEnumValueAttribute(Type type, object[] values)
        {
            Type = type;
            Values = values;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Value must not be empty.");
            }

            if (!Enum.TryParse(Type, value.ToString(), out var result))
            {
                return new ValidationResult("The specified value is not valid");
            }

            return Values.Any(x => x.Equals(result))
                ? ValidationResult.Success
                : new ValidationResult("The specified value is not valid");
        }
    }
}
