using InsurancePartners.Web.Models;
using InsurancePartners.Web.ViewModels.Partners;

namespace InsurancePartners.Web.Repositories;

public interface IPartnerRepository
{
    Task<IReadOnlyList<PartnerListItemViewModel>> GetAllAsync();

    Task<Partner?> GetByIdAsync(int id);

    Task<PartnerDetailsViewModel?> GetDetailsAsync(int id);

    Task<IReadOnlyList<PartnerTypeOptionViewModel>> GetPartnerTypesAsync();

    Task<int> CreateAsync(Partner partner);

    Task<bool> UpdateAsync(Partner partner);

    Task<bool> SoftDeleteAsync(int id);

    Task<bool> ExistsByPartnerNumberAsync(string partnerNumber, int? excludedPartnerId = null);

    Task<bool> ExistsByExternalCodeAsync(string externalCode, int? excludedPartnerId = null);

    Task<bool> ExistsByCroatianPinAsync(string? croatianPin, int? excludedPartnerId = null);
}