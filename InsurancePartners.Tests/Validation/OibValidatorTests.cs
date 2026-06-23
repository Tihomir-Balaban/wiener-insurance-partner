using FluentAssertions;
using InsurancePartners.Web.Validation;

namespace InsurancePartners.Tests.Validation;

public sealed class OibValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValid_Should_ReturnTrue_WhenOibIsEmptyBecauseFieldIsOptional(string? oib)
    {
        var result = OibValidator.IsValid(oib);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("33392005961")]
    public void IsValid_Should_ReturnTrue_WhenOibChecksumIsValid(string oib)
    {
        var result = OibValidator.IsValid(oib);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("33392005960")]
    [InlineData("123")]
    [InlineData("1234567890A")]
    [InlineData("123456789012")]
    public void IsValid_Should_ReturnFalse_WhenOibIsInvalid(string oib)
    {
        var result = OibValidator.IsValid(oib);

        result.Should().BeFalse();
    }
}