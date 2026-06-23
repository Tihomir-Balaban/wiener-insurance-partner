using System.ComponentModel.DataAnnotations;

namespace InsurancePartners.Tests;

internal static class ModelValidationTestHelper
{
    public static IReadOnlyList<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        Validator.TryValidateObject(
            model,
            context,
            results,
            validateAllProperties: true);

        return results;
    }

    public static bool HasErrorFor(
        IReadOnlyList<ValidationResult> results,
        string memberName)
    {
        return results.Any(result => result.MemberNames.Contains(memberName));
    }
}