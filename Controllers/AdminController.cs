using FrontEndTicketPro.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace FrontEndTicketPro.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _http;
        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient();
            _http.BaseAddress = new Uri("https://localhost:7141"); // Usa el puerto real de tu API
        }
        public async Task<IActionResult> Inicio()
        {
            // Simulación para pruebas
            HttpContext.Session.SetString("Rol", "admin");

            var resumen = await _http.GetFromJsonAsync<DashboardResumen>("api/ticket/resumen-dashboard");
            var tickets = await _http.GetFromJsonAsync<List<TablaTicketsInicio>>("api/ticket/resumen-tickets");

            ViewBag.Resumen = resumen;
            ViewBag.Tickets = tickets;

            return View();
        }


    }
}
