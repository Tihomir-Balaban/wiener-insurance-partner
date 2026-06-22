namespace InsurancePartners.Web.ViewModels.Policies;

public sealed class PolicyListItemViewModel
{
    public int Id { get; set; }

    public int PartnerId { get; set; }

    public string PolicyNumber { get; set; } = string.Empty;

    public decimal PolicyAmount { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}