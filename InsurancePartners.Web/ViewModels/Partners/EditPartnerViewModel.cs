using System.ComponentModel.DataAnnotations;
using InsurancePartners.Web.Validation;

namespace InsurancePartners.Web.ViewModels.Partners;

public sealed class EditPartnerViewModel
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Required]
    [StringLength(255, MinimumLength = 2)]
    [RegularExpression(
        @"^[\p{L}\p{N} .'\-]+$",
        ErrorMessage = "First name may contain only letters, numbers, spaces, dots, apostrophes, and hyphens.")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(255, MinimumLength = 2)]
    [RegularExpression(
        @"^[\p{L}\p{N} .'\-]+$",
        ErrorMessage = "Last name may contain only letters, numbers, spaces, dots, apostrophes, and hyphens.")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Address { get; set; }

    [Required]
    [RegularExpression(@"^\d{20}$", ErrorMessage = "Partner number must contain exactly 20 digits.")]
    public string PartnerNumber { get; set; } = string.Empty;

    [ValidOib]
    [Display(Name = "Croatian PIN / OIB")]
    public string? CroatianPIN { get; set; }

    [Range(1, 2, ErrorMessage = "Partner type must be Personal or Legal.")]
    public int PartnerTypeId { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string CreateByUser { get; set; } = string.Empty;

    public bool IsForeign { get; set; }

    [Required]
    [RegularExpression(
        @"^[A-Za-z0-9]{10,20}$",
        ErrorMessage = "External code must be alphanumeric and contain between 10 and 20 characters.")]
    public string ExternalCode { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[MFN]$", ErrorMessage = "Gender must be M, F, or N.")]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public string RowVersion { get; set; } = string.Empty;

    public IReadOnlyList<PartnerTypeOptionViewModel> PartnerTypes { get; set; } = [];
}