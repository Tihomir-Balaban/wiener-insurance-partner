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

        return Ok(ToSummaryResponse(result.Value));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await policyService.GetEditViewModelAsync(id);

        if (model is null)
        {
            return NotFound(new
            {
                errors = new[]
                {
                    new
                    {
                        key = string.Empty,
                        message = "Policy was not found."
                    }
                }
            });
        }

        return Ok(new
        {
            id = model.Id,
            partnerId = model.PartnerId,
            policyNumber = model.PolicyNumber,
            policyAmount = model.PolicyAmount,
            rowVersion = model.RowVersion
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditPolicyViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest(new
            {
                errors = new[]
                {
                    new
                    {
                        key = string.Empty,
                        message = "Invalid policy request."
                    }
                }
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                errors = GetModelStateErrors()
            });
        }

        var result = await policyService.UpdateAsync(model);

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
                            message = "Policy was updated, but the updated partner summary could not be loaded."
                        }
                    }
                });
        }

        return Ok(ToSummaryResponse(result.Value));
    }

    private static object ToSummaryResponse(PartnerPolicySummaryViewModel summary)
    {
        return new
        {
            partnerId = summary.PartnerId,
            policyCount = summary.PolicyCount,
            totalPolicyAmount = summary.TotalPolicyAmount,
            isMarked = summary.IsMarked
        };
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