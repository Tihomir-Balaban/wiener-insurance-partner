using System.ComponentModel.DataAnnotations;

namespace InsurancePartners.Web.Validation;

public sealed class ValidOibAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        return value is string oib && OibValidator.IsValid(oib);
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a valid Croatian OIB.";
    }
}