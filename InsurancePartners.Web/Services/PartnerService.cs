using InsurancePartners.Web.Models;
using InsurancePartners.Web.Repositories;
using InsurancePartners.Web.ViewModels.Partners;

namespace InsurancePartners.Web.Services;

public sealed class PartnerService(
    IPartnerRepository partnerRepository,
    IPolicyRepository policyRepository) : IPartnerService
{
    public Task<IReadOnlyList<PartnerListItemViewModel>> GetAllAsync()
    {
        return partnerRepository.GetAllAsync();
    }

    public Task<PartnerDetailsViewModel?> GetDetailsAsync(int id)
    {
        return partnerRepository.GetDetailsAsync(id);
    }

    public async Task<CreatePartnerViewModel> BuildCreateViewModelAsync(
        CreatePartnerViewModel? model = null)
    {
        model ??= new CreatePartnerViewModel();
        model.PartnerTypes = await partnerRepository.GetPartnerTypesAsync();

        return model;
    }

    public async Task<EditPartnerViewModel?> GetEditViewModelAsync(int id)
    {
        var partner = await partnerRepository.GetByIdAsync(id);

        if (partner is null)
        {
            return null;
        }

        var partnerTypes = await partnerRepository.GetPartnerTypesAsync();

        return new EditPartnerViewModel
        {
            Id = partner.Id,
            FirstName = partner.FirstName,
            LastName = partner.LastName,
            Address = partner.Address,
            PartnerNumber = partner.PartnerNumber.Trim(),
            CroatianPIN = partner.CroatianPIN?.Trim(),
            PartnerTypeId = partner.PartnerTypeId,
            CreateByUser = partner.CreateByUser,
            IsForeign = partner.IsForeign,
            ExternalCode = partner.ExternalCode,
            Gender = partner.Gender,
            RowVersion = Convert.ToBase64String(partner.RowVersion),
            PartnerTypes = partnerTypes
        };
    }

    public async Task<ServiceResult<int>> CreateAsync(CreatePartnerViewModel model)
    {
        var partnerNumber = NormalizeRequired(model.PartnerNumber);
        var externalCode = NormalizeRequired(model.ExternalCode);
        var croatianPin = NormalizeOptional(model.CroatianPIN);

        var uniqueErrors = await ValidateUniquePartnerFieldsAsync(
            partnerNumber,
            externalCode,
            croatianPin);

        if (uniqueErrors.Count > 0)
        {
            return ServiceResult<int>.Failure(uniqueErrors);
        }

        var partner = new Partner
        {
            FirstName = NormalizeRequired(model.FirstName),
            LastName = NormalizeRequired(model.LastName),
            Address = NormalizeOptional(model.Address),
            PartnerNumber = partnerNumber,
            CroatianPIN = croatianPin,
            PartnerTypeId = model.PartnerTypeId,
            CreateByUser = NormalizeRequired(model.CreateByUser),
            IsForeign = model.IsForeign,
            ExternalCode = externalCode,
            Gender = NormalizeRequired(model.Gender).ToUpperInvariant()
        };

        var partnerId = await partnerRepository.CreateAsync(partner);

        return ServiceResult<int>.Success(partnerId);
    }

    public async Task<ServiceResult> UpdateAsync(EditPartnerViewModel model)
    {
        var existingPartner = await partnerRepository.GetByIdAsync(model.Id);

        if (existingPartner is null)
        {
            return ServiceResult.Failure(string.Empty, "Partner was not found.");
        }

        if (!TryConvertRowVersion(model.RowVersion, out var rowVersion))
        {
            return ServiceResult.Failure(
                nameof(EditPartnerViewModel.RowVersion),
                "Invalid row version value.");
        }

        var partnerNumber = NormalizeRequired(model.PartnerNumber);
        var externalCode = NormalizeRequired(model.ExternalCode);
        var croatianPin = NormalizeOptional(model.CroatianPIN);

        var uniqueErrors = await ValidateUniquePartnerFieldsAsync(
            partnerNumber,
            externalCode,
            croatianPin,
            model.Id);

        if (uniqueErrors.Count > 0)
        {
            return ServiceResult.Failure(uniqueErrors);
        }

        var partner = new Partner
        {
            Id = model.Id,
            FirstName = NormalizeRequired(model.FirstName),
            LastName = NormalizeRequired(model.LastName),
            Address = NormalizeOptional(model.Address),
            PartnerNumber = partnerNumber,
            CroatianPIN = croatianPin,
            PartnerTypeId = model.PartnerTypeId,
            CreateByUser = NormalizeRequired(model.CreateByUser),
            IsForeign = model.IsForeign,
            ExternalCode = externalCode,
            Gender = NormalizeRequired(model.Gender).ToUpperInvariant(),
            RowVersion = rowVersion
        };

        var updated = await partnerRepository.UpdateAsync(partner);

        if (!updated)
        {
            return ServiceResult.Failure(
                string.Empty,
                "Partner was modified by another operation. Reload the page and try again.");
        }

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> SoftDeleteAsync(int id)
    {
        var deleted = await partnerRepository.SoftDeleteAsync(id);

        if (!deleted)
        {
            return ServiceResult.Failure(string.Empty, "Partner was not found.");
        }

        await policyRepository.SoftDeleteByPartnerIdAsync(id);

        return ServiceResult.Success();
    }

    private async Task<List<ServiceError>> ValidateUniquePartnerFieldsAsync(
        string partnerNumber,
        string externalCode,
        string? croatianPin,
        int? excludedPartnerId = null)
    {
        var errors = new List<ServiceError>();

        if (await partnerRepository.ExistsByPartnerNumberAsync(partnerNumber, excludedPartnerId))
        {
            errors.Add(new ServiceError(
                nameof(CreatePartnerViewModel.PartnerNumber),
                "Partner number already exists."));
        }

        if (await partnerRepository.ExistsByExternalCodeAsync(externalCode, excludedPartnerId))
        {
            errors.Add(new ServiceError(
                nameof(CreatePartnerViewModel.ExternalCode),
                "External code already exists."));
        }

        if (!string.IsNullOrWhiteSpace(croatianPin)
            && await partnerRepository.ExistsByCroatianPinAsync(croatianPin, excludedPartnerId))
        {
            errors.Add(new ServiceError(
                nameof(CreatePartnerViewModel.CroatianPIN),
                "Croatian PIN / OIB already exists."));
        }

        return errors;
    }

    private static string NormalizeRequired(string value)
    {
        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static bool TryConvertRowVersion(string value, out byte[] rowVersion)
    {
        try
        {
            rowVersion = Convert.FromBase64String(value);

            return rowVersion.Length > 0;
        }
        catch (FormatException)
        {
            rowVersion = [];

            return false;
        }
    }
}