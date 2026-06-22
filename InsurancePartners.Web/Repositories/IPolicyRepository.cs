using InsurancePartners.Web.Models;
using InsurancePartners.Web.ViewModels.Policies;

namespace InsurancePartners.Web.Repositories;

public interface IPolicyRepository
{
    Task<IReadOnlyList<PolicyListItemViewModel>> GetByPartnerIdAsync(int partnerId);

    Task<Policy?> GetByIdAsync(int id);

    Task<int> CreateAsync(Policy policy);

    Task<bool> UpdateAsync(Policy policy);

    Task<bool> SoftDeleteAsync(int id);

    Task<int> SoftDeleteByPartnerIdAsync(int partnerId);

    Task<bool> ExistsByPolicyNumberAsync(string policyNumber, int? excludedPolicyId = null);

    Task<PartnerPolicySummaryViewModel> GetPartnerPolicySummaryAsync(int partnerId);
}