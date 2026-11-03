using System;
using System.ComponentModel.DataAnnotations;

namespace CapStone.Attributes
{
    public class NotFutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            // Handle DateOnly (used in CustomerRequestDTO)
            if (value is DateOnly dateOnly)
            {
                if (dateOnly > DateOnly.FromDateTime(DateTime.UtcNow))
                {
                    return new ValidationResult(ErrorMessage ?? "Date cannot be in the future.");
                }
                return ValidationResult.Success;
            }

            // Handle DateTime
            if (value is DateTime dateTime)
            {
                if (dateTime.Date > DateTime.UtcNow.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "Date cannot be in the future.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
