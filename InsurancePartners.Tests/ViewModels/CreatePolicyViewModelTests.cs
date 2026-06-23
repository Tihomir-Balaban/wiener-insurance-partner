using FluentAssertions;
using InsurancePartners.Web.ViewModels.Policies;

namespace InsurancePartners.Tests.ViewModels;

public sealed class CreatePolicyViewModelTests
{
    [Fact]
    public void Validate_Should_Pass_WhenModelIsValid()
    {
        var model = BuildValidModel();

        var results = ModelValidationTestHelper.Validate(model);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Validate_Should_Fail_WhenPartnerIdIsMissing()
    {
        var model = BuildValidModel();
        model.PartnerId = 0;

        var results = ModelValidationTestHelper.Validate(model);

        ModelValidationTestHelper
            .HasErrorFor(results, nameof(CreatePolicyViewModel.PartnerId))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_WhenPolicyNumberIsTooShort()
    {
        var model = BuildValidModel();
        model.PolicyNumber = "POL123";

        var results = ModelValidationTestHelper.Validate(model);

        ModelValidationTestHelper
            .HasErrorFor(results, nameof(CreatePolicyViewModel.PolicyNumber))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_WhenPolicyNumberContainsInvalidCharacters()
    {
        var model = BuildValidModel();
        model.PolicyNumber = "POLICY-0001";

        var results = ModelValidationTestHelper.Validate(model);

        ModelValidationTestHelper
            .HasErrorFor(results, nameof(CreatePolicyViewModel.PolicyNumber))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_WhenPolicyAmountIsZero()
    {
        var model = BuildValidModel();
        model.PolicyAmount = 0;

        var results = ModelValidationTestHelper.Validate(model);

        ModelValidationTestHelper
            .HasErrorFor(results, nameof(CreatePolicyViewModel.PolicyAmount))
            .Should()
            .BeTrue();
    }

    private static CreatePolicyViewModel BuildValidModel()
    {
        return new CreatePolicyViewModel
        {
            PartnerId = 1,
            PolicyNumber = "POLICY0001",
            PolicyAmount = 1000m
        };
    }
}