using FluentAssertions;
using InsurancePartners.Web.ViewModels.Partners;

namespace InsurancePartners.Tests.ViewModels;

public sealed class CreatePartnerViewModelTests
{
    [Fact]
    public void Validate_Should_Pass_WhenModelIsValidAndOibIsEmpty()
    {
        var model = BuildValidModel();

        var results = ModelValidationTestHelper.Validate(model);

        results.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Tihomir")]
    [InlineData("Željko")]
    [InlineData("Đuro")]
    [InlineData("Ana-Marija")]
    [InlineData("O'Connor")]
    public void Validate_Should_Pass_WhenFirstNameContainsAllowedCharacters(string firstName)
    {
        var model = BuildValidModel();
        model.FirstName = firstName;

        var results = ModelValidationTestHelper.Validate(model);

        ModelValidationTestHelper
            .HasErrorFor(results, nameof(CreatePartnerViewModel.FirstName))
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Validate_Should_Fail_WhenPartnerNumberIsNotExactlyTwentyDigits()
    {
        var model = BuildValidModel();
        model.PartnerNumber = "123456789";

        var results = ModelValidationTestHelper.Validate(model);

        ModelValidationTestHelper
            .HasErrorFor(results, nameof(CreatePartnerViewModel.PartnerNumber))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_WhenPartnerNumberContainsLetters()
    {
        var model = BuildValidModel();
        model.PartnerNumber = "1234567890123456789A";

        var results = ModelValidationTestHelper.Validate(model);

        ModelValidationTestHelper
            .HasErrorFor(results, nameof(CreatePartnerViewModel.PartnerNumber))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_WhenCroatianPinChecksumIsInvalid()
    {
        var model = BuildValidModel();
        model.CroatianPIN = "33392005960";

        var results = ModelValidationTestHelper.Validate(model);

        ModelValidationTestHelper
            .HasErrorFor(results, nameof(CreatePartnerViewModel.CroatianPIN))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Validate_Should_Pass_WhenCroatianPinChecksumIsValid()
    {
        var model = BuildValidModel();
        model.CroatianPIN = "33392005961";

        var results = ModelValidationTestHelper.Validate(model);

        ModelValidationTestHelper
            .HasErrorFor(results, nameof(CreatePartnerViewModel.CroatianPIN))
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Validate_Should_Fail_WhenExternalCodeIsTooShort()
    {
        var model = BuildValidModel();
        model.ExternalCode = "ABC123";

        var results = ModelValidationTestHelper.Validate(model);

        ModelValidationTestHelper
            .HasErrorFor(results, nameof(CreatePartnerViewModel.ExternalCode))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_WhenGenderIsInvalid()
    {
        var model = BuildValidModel();
        model.Gender = "X";

        var results = ModelValidationTestHelper.Validate(model);

        ModelValidationTestHelper
            .HasErrorFor(results, nameof(CreatePartnerViewModel.Gender))
            .Should()
            .BeTrue();
    }

    private static CreatePartnerViewModel BuildValidModel()
    {
        return new CreatePartnerViewModel
        {
            FirstName = "Tihomir",
            LastName = "Balaban",
            Address = "Test Street 1",
            PartnerNumber = "12345678901234567890",
            CroatianPIN = null,
            PartnerTypeId = 1,
            CreateByUser = "test@example.com",
            IsForeign = false,
            ExternalCode = "ABC1234567",
            Gender = "M"
        };
    }
}