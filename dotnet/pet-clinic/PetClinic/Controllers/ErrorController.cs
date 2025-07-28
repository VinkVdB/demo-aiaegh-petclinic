using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PetClinic.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Global error handler - equivalent to Spring's error page
    /// </summary>
    [Route("/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("Error occurred for request: {RequestId}", requestId);
        
        ViewBag.RequestId = requestId;
        return View("Error");
    }
}
