/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.Controllers;

/// <summary>
/// Home controller.
/// </summary>
public class HomeController: Controller
{
    // ● public
    /// <summary>
    /// Displays the application shell placeholder.
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }
    /// <summary>
    /// Displays the error page.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
