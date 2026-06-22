using System.ComponentModel.DataAnnotations;

namespace InsurancePartners.Web.ViewModels.Policies;

public sealed class CreatePolicyViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Partner is required.")]
    public int PartnerId { get; set; }

    [Required]
    [RegularExpression(
        @"^[A-Za-z0-9]{10,15}$",
        ErrorMessage = "Policy number must be alphanumeric and contain between 10 and 15 characters.")]
    public string PolicyNumber { get; set; } = string.Empty;

    [Range(
        typeof(decimal),
        "0.01",
        "9999999999999999.99",
        ErrorMessage = "Policy amount must be greater than 0.")]
    public decimal PolicyAmount { get; set; }
}