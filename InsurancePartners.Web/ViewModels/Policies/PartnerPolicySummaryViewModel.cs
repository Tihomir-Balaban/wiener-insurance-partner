namespace InsurancePartners.Web.ViewModels.Policies;

public sealed class PartnerPolicySummaryViewModel
{
    public int PartnerId { get; set; }

    public int PolicyCount { get; set; }

    public decimal TotalPolicyAmount { get; set; }

    public bool IsMarked { get; set; }
}