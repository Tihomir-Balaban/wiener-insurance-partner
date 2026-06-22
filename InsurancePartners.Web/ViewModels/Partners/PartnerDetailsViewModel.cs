using InsurancePartners.Web.ViewModels.Policies;

namespace InsurancePartners.Web.ViewModels.Partners;

public sealed class PartnerDetailsViewModel
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string PartnerNumber { get; set; } = string.Empty;

    public string? CroatianPIN { get; set; }

    public string PartnerTypeName { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public string CreateByUser { get; set; } = string.Empty;

    public bool IsForeign { get; set; }

    public string ExternalCode { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public int PolicyCount { get; set; }

    public decimal TotalPolicyAmount { get; set; }

    public bool IsMarked { get; set; }

    public IReadOnlyList<PolicyListItemViewModel> Policies { get; set; } = [];
}