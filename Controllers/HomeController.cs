using FrontEndTicketPro.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

[SessionAuthorize] // ✅ Aplica la protección a todo el controlador
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // ❌ Ya no se fuerza la sesión como "admin"
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
