using InsurancePartners.Web.Models;
using InsurancePartners.Web.Repositories;
using InsurancePartners.Web.ViewModels.Policies;

namespace InsurancePartners.Web.Services;

public sealed class PolicyService(
    IPolicyRepository policyRepository,
    IPartnerRepository partnerRepository) : IPolicyService
{
    public Task<IReadOnlyList<PolicyListItemViewModel>> GetByPartnerIdAsync(int partnerId)
    {
        return policyRepository.GetByPartnerIdAsync(partnerId);
    }

    public async Task<EditPolicyViewModel?> GetEditViewModelAsync(int id)
    {
        var policy = await policyRepository.GetByIdAsync(id);

        if (policy is null)
        {
            return null;
        }

        return new EditPolicyViewModel
        {
            Id = policy.Id,
            PartnerId = policy.PartnerId,
            PolicyNumber = policy.PolicyNumber,
            PolicyAmount = policy.PolicyAmount,
            RowVersion = Convert.ToBase64String(policy.RowVersion)
        };
    }

    public async Task<ServiceResult<PartnerPolicySummaryViewModel>> CreateAsync(
        CreatePolicyViewModel model)
    {
        var partner = await partnerRepository.GetByIdAsync(model.PartnerId);

        if (partner is null)
        {
            return ServiceResult<PartnerPolicySummaryViewModel>.Failure(
                nameof(CreatePolicyViewModel.PartnerId),
                "Partner was not found.");
        }

        var policyNumber = NormalizeRequired(model.PolicyNumber);

        if (await policyRepository.ExistsByPolicyNumberAsync(policyNumber))
        {
            return ServiceResult<PartnerPolicySummaryViewModel>.Failure(
                nameof(CreatePolicyViewModel.PolicyNumber),
                "Policy number already exists.");
        }

        var policy = new Policy
        {
            PartnerId = model.PartnerId,
            PolicyNumber = policyNumber,
            PolicyAmount = model.PolicyAmount
        };

        await policyRepository.CreateAsync(policy);

        var summary = await policyRepository.GetPartnerPolicySummaryAsync(model.PartnerId);

        return ServiceResult<PartnerPolicySummaryViewModel>.Success(summary);
    }

    public async Task<ServiceResult<PartnerPolicySummaryViewModel>> UpdateAsync(
        EditPolicyViewModel model)
    {
        var existingPolicy = await policyRepository.GetByIdAsync(model.Id);

        if (existingPolicy is null)
        {
            return ServiceResult<PartnerPolicySummaryViewModel>.Failure(
                string.Empty,
                "Policy was not found.");
        }

        if (!TryConvertRowVersion(model.RowVersion, out var rowVersion))
        {
            return ServiceResult<PartnerPolicySummaryViewModel>.Failure(
                nameof(EditPolicyViewModel.RowVersion),
                "Invalid row version value.");
        }

        var policyNumber = NormalizeRequired(model.PolicyNumber);

        if (await policyRepository.ExistsByPolicyNumberAsync(policyNumber, model.Id))
        {
            return ServiceResult<PartnerPolicySummaryViewModel>.Failure(
                nameof(EditPolicyViewModel.PolicyNumber),
                "Policy number already exists.");
        }

        var policy = new Policy
        {
            Id = model.Id,
            PartnerId = existingPolicy.PartnerId,
            PolicyNumber = policyNumber,
            PolicyAmount = model.PolicyAmount,
            RowVersion = rowVersion
        };

        var updated = await policyRepository.UpdateAsync(policy);

        if (!updated)
        {
            return ServiceResult<PartnerPolicySummaryViewModel>.Failure(
                string.Empty,
                "Policy was modified by another operation. Reload the page and try again.");
        }

        var summary = await policyRepository.GetPartnerPolicySummaryAsync(existingPolicy.PartnerId);

        return ServiceResult<PartnerPolicySummaryViewModel>.Success(summary);
    }

    public async Task<ServiceResult<PartnerPolicySummaryViewModel>> SoftDeleteAsync(int id)
    {
        var existingPolicy = await policyRepository.GetByIdAsync(id);

        if (existingPolicy is null)
        {
            return ServiceResult<PartnerPolicySummaryViewModel>.Failure(
                string.Empty,
                "Policy was not found.");
        }

        var deleted = await policyRepository.SoftDeleteAsync(id);

        if (!deleted)
        {
            return ServiceResult<PartnerPolicySummaryViewModel>.Failure(
                string.Empty,
                "Policy was not found.");
        }

        var summary = await policyRepository.GetPartnerPolicySummaryAsync(existingPolicy.PartnerId);

        return ServiceResult<PartnerPolicySummaryViewModel>.Success(summary);
    }

    public Task<PartnerPolicySummaryViewModel> GetPartnerPolicySummaryAsync(int partnerId)
    {
        return policyRepository.GetPartnerPolicySummaryAsync(partnerId);
    }

    private static string NormalizeRequired(string value)
    {
        return value.Trim();
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