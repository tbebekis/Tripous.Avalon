using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tERPWeb.Models;

namespace tERPWeb.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Startup));
    }
    public IActionResult Startup()
    {
        return View();
    }
    public IActionResult MainPage()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
