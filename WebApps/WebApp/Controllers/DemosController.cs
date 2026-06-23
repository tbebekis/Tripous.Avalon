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
    /// Displays the core tp demo.
    /// </summary>
    [Route("/demo/tp")]
    public IActionResult Tp()
    {
        return View("Core/Tp");
    }
    /// <summary>
    /// Displays the tp.Ready demo.
    /// </summary>
    [Route("/demo/tp-ready")]
    public IActionResult TpReady()
    {
        return View("Core/TpReady");
    }
    /// <summary>
    /// Displays the tp helper functions demo.
    /// </summary>
    [Route("/demo/tp-helpers")]
    public IActionResult TpHelpers()
    {
        return View("Core/TpHelpers");
    }
    /// <summary>
    /// Displays the tp DOM helper functions demo.
    /// </summary>
    [Route("/demo/tp-dom")]
    public IActionResult TpDom()
    {
        return View("Core/TpDom");
    }
    /// <summary>
    /// Displays the tp.Object class demo.
    /// </summary>
    [Route("/demo/tp-object")]
    public IActionResult TpObject()
    {
        return View("Core/TpObject");
    }
    /// <summary>
    /// Displays the screen overlay and spinner demo.
    /// </summary>
    [Route("/demo/tp-ui-overlay-spinner")]
    public IActionResult TpUiOverlaySpinner()
    {
        return View("UI/TpUiOverlaySpinner");
    }
}
