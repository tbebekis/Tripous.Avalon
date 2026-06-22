using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

/// <summary>
/// Provides pages used for Tripous Web JavaScript and control demos.
/// </summary>
public class DemosController : Controller
{
    // ● public
    /// <summary>
    /// Displays the demos index page.
    /// </summary>
    [Route("/demos")]
    public IActionResult Index()
    {
        return View();
    }
    /// <summary>
    /// Displays the core tp object demo.
    /// </summary>
    [Route("/demo/tp")]
    public IActionResult Tp()
    {
        return View("Core/Tp");
    }
}
