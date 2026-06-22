using InsurancePartners.Web.Services;
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
}