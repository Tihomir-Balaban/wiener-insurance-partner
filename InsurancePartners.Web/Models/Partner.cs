namespace InsurancePartners.Web.Models;

public sealed class Partner
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string PartnerNumber { get; set; } = string.Empty;

    public string? CroatianPIN { get; set; }

    public int PartnerTypeId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public string CreateByUser { get; set; } = string.Empty;

    public bool IsForeign { get; set; }

    public string ExternalCode { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public DateTime? UpdatedAtUtc { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAtUtc { get; set; }

    public byte[] RowVersion { get; set; } = [];

    public string FullName => $"{FirstName} {LastName}".Trim();
}