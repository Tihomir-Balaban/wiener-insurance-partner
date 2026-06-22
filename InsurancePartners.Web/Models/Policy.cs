namespace InsurancePartners.Web.Models;

public sealed class Policy
{
    public int Id { get; set; }

    public int PartnerId { get; set; }

    public string PolicyNumber { get; set; } = string.Empty;

    public decimal PolicyAmount { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAtUtc { get; set; }

    public byte[] RowVersion { get; set; } = [];
}