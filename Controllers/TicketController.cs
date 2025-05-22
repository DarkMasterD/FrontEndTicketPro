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

        //public async Task<IActionResult> CrearTicket()
        //{
        //    return View();
        //}

        //para las categorias 
        public async Task<IActionResult> CrearTicket()
        {
            var categorias = new List<categoria_ticket>();
            var response = await _http.GetAsync("/api/categoria");

            if (response.IsSuccessStatusCode)
            {
                categorias = await response.Content.ReadFromJsonAsync<List<categoria_ticket>>();
            }

            var modelo = new CrearTicketViewModel
            {
                Ticket = new ticket(),
                Categorias = categorias
            };

            return View(modelo); // 🔴 ESTA línea debe existir, no `View();`
        }

    }
}
