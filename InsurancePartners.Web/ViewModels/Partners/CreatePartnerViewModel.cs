using System.ComponentModel.DataAnnotations;
using InsurancePartners.Web.Validation;

namespace InsurancePartners.Web.ViewModels.Partners;

public sealed class CreatePartnerViewModel
{
    [Required]
    [StringLength(255, MinimumLength = 2)]
    [RegularExpression(
        @"^[A-Za-z0-9ČĆŽŠĐčćžšđ .'\-]+$",
        ErrorMessage = "First name may contain only letters, numbers, spaces, dots, apostrophes, and hyphens.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(255, MinimumLength = 2)]
    [RegularExpression(
        @"^[A-Za-z0-9ČĆŽŠĐčćžšđ .'\-]+$",
        ErrorMessage = "Last name may contain only letters, numbers, spaces, dots, apostrophes, and hyphens.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Address")]
    public string? Address { get; set; }

    [Required]
    [RegularExpression(@"^\d{20}$", ErrorMessage = "Partner number must contain exactly 20 digits.")]
    [Display(Name = "Partner Number")]
    public string PartnerNumber { get; set; } = string.Empty;

    [ValidOib]
    [Display(Name = "Croatian PIN / OIB")]
    public string? CroatianPIN { get; set; }

    [Range(1, 2, ErrorMessage = "Partner type must be Personal or Legal.")]
    [Display(Name = "Partner Type")]
    public int PartnerTypeId { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    [Display(Name = "Create By User")]
    public string CreateByUser { get; set; } = string.Empty;

    public bool IsForeign { get; set; }

    [Required]
    [RegularExpression(
        @"^[A-Za-z0-9]{10,20}$",
        ErrorMessage = "External code must be alphanumeric and contain between 10 and 20 characters.")]
    [Display(Name = "External Code")]
    public string ExternalCode { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[MFN]$", ErrorMessage = "Gender must be M, F, or N.")]
    public string Gender { get; set; } = string.Empty;

    public IReadOnlyList<PartnerTypeOptionViewModel> PartnerTypes { get; set; } = [];
}