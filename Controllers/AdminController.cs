using FrontEndTicketPro.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;

namespace FrontEndTicketPro.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AdminController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        [SessionAuthorize("admin")]
        public async Task<IActionResult> Inicio()
        {
            // Simulación para pruebas

            var client = _httpClientFactory.CreateClient("ApiInsegura");
            var apiBaseUrl = _configuration["ApiBaseUrl"];

            var resumen = await client.GetFromJsonAsync<DashboardResumen>($"{apiBaseUrl}/ticket/resumen-dashboard");
            var tickets = await client.GetFromJsonAsync<List<TablaTicketsInicio>>($"{apiBaseUrl}/ticket/resumen-tickets");

            ViewBag.Resumen = resumen;
            ViewBag.Tickets = tickets;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DetalleTicket(int id)
        {
            Console.WriteLine($"ID recibido: {id}");
            var ticket = await _http.GetFromJsonAsync<TicketDetalleViewModel>($"/api/ticket/detalle/{id}");
            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

    }
}
