using System.Diagnostics;
using InsurancePartners.Web.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InsurancePartners.Web.Controllers;

public sealed class HomeController(ILogger<HomeController> logger) : Controller
{
    public IActionResult Privacy()
    {
        return View();
    }

    public new IActionResult StatusCode(int code)
    {
        Response.StatusCode = code;

        return View(code);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionFeature?.Error is not null)
        {
            logger.LogError(
                exceptionFeature.Error,
                "Unhandled exception while processing path {Path}",
                exceptionFeature.Path);
        }

        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}