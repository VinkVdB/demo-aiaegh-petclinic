using Microsoft.AspNetCore.Mvc;

namespace PetClinic.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Welcome page - equivalent to Spring WelcomeController
    /// </summary>
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View("Welcome");
    }

    /// <summary>
    /// Error demonstration page - equivalent to Spring CrashController
    /// </summary>
    [HttpGet("/oups")]
    public IActionResult Error()
    {
        throw new InvalidOperationException(
            "Expected: controller used to showcase what happens when an exception is thrown");
    }
}
