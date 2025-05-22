using Microsoft.AspNetCore.Mvc;
using FrontEndTicketPro.Models;
using System.Net.Sockets;

namespace FrontEndTicketPro.Controllers
{
    public class TicketController : Controller
    {
        private readonly HttpClient _http;

        public TicketController(IHttpClientFactory clientFactory)
        {
            _http = clientFactory.CreateClient();
            _http.BaseAddress = new Uri("https://localhost:7141"); // aqui deben tener la dirección que les da al ejecutar la API
        }

        public async Task<IActionResult> Index()
        {
            var tickets = await _http.GetFromJsonAsync<List<ticket>>("api/ticket");
            return View(tickets);
        }

        public async Task<IActionResult> IndexDetalleTicket()
        {
            return View();
        }
    }
}
