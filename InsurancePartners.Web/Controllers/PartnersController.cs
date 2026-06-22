using InsurancePartners.Web.Services;
using InsurancePartners.Web.ViewModels.Partners;
using Microsoft.AspNetCore.Mvc;

namespace InsurancePartners.Web.Controllers;

public sealed class PartnersController(IPartnerService partnerService) : Controller
{
    public async Task<IActionResult> Index(int? highlightId = null)
    {
        var partners = await partnerService.GetAllAsync();

        ViewBag.HighlightId = highlightId;

        return View(partners);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var partner = await partnerService.GetDetailsAsync(id);

        if (partner is null)
        {
            return NotFound();
        }

        return PartialView("_DetailsModalContent", partner);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = await partnerService.BuildCreateViewModelAsync();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePartnerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = await partnerService.BuildCreateViewModelAsync(model);

            return View(model);
        }

        var result = await partnerService.CreateAsync(model);

        if (!result.Succeeded)
        {
            AddServiceErrors(result.Errors);

            model = await partnerService.BuildCreateViewModelAsync(model);

            return View(model);
        }

        TempData["SuccessMessage"] = "Partner was created successfully.";

        return RedirectToAction(
            nameof(Index),
            new { highlightId = result.Value });
    }

    private void AddServiceErrors(IReadOnlyList<ServiceError> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(error.Key, error.Message);
        }
    }
}