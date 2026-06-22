using InsurancePartners.Web.ViewModels.Partners;

namespace InsurancePartners.Web.Services;

public interface IPartnerService
{
    Task<IReadOnlyList<PartnerListItemViewModel>> GetAllAsync();

    Task<PartnerDetailsViewModel?> GetDetailsAsync(int id);

    Task<CreatePartnerViewModel> BuildCreateViewModelAsync(CreatePartnerViewModel? model = null);

    Task<EditPartnerViewModel?> GetEditViewModelAsync(int id);

    Task<EditPartnerViewModel> BuildEditViewModelAsync(EditPartnerViewModel model);

    Task<ServiceResult<int>> CreateAsync(CreatePartnerViewModel model);

    Task<ServiceResult> UpdateAsync(EditPartnerViewModel model);

    Task<ServiceResult> SoftDeleteAsync(int id);
}