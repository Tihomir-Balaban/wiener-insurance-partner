using InsurancePartners.Web.Services;
using InsurancePartners.Web.ViewModels.Policies;
using Microsoft.AspNetCore.Mvc;

namespace InsurancePartners.Web.Controllers;

public sealed class PoliciesController(IPolicyService policyService) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePolicyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                errors = GetModelStateErrors()
            });
        }

        var result = await policyService.CreateAsync(model);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                errors = result.Errors.Select(error => new
                {
                    key = error.Key,
                    message = error.Message
                })
            });
        }

        if (result.Value is null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    errors = new[]
                    {
                        new
                        {
                            key = string.Empty,
                            message = "Policy was created, but the updated partner summary could not be loaded."
                        }
                    }
                });
        }

        return Ok(new
        {
            partnerId = result.Value.PartnerId,
            policyCount = result.Value.PolicyCount,
            totalPolicyAmount = result.Value.TotalPolicyAmount,
            isMarked = result.Value.IsMarked
        });
    }

    private IEnumerable<object> GetModelStateErrors()
    {
        return ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error => new
            {
                key = entry.Key,
                message = error.ErrorMessage
            }));
    }
}