using InsurancePartners.Web.ViewModels.Policies;

namespace InsurancePartners.Web.Services;

public interface IPolicyService
{
    Task<IReadOnlyList<PolicyListItemViewModel>> GetByPartnerIdAsync(int partnerId);

    Task<EditPolicyViewModel?> GetEditViewModelAsync(int id);

    Task<ServiceResult<PartnerPolicySummaryViewModel>> CreateAsync(CreatePolicyViewModel model);

    Task<ServiceResult<PartnerPolicySummaryViewModel>> UpdateAsync(EditPolicyViewModel model);

    Task<ServiceResult<PartnerPolicySummaryViewModel>> SoftDeleteAsync(int id);

    Task<PartnerPolicySummaryViewModel> GetPartnerPolicySummaryAsync(int partnerId);
}