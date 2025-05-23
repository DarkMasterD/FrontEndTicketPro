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
            var datosDetalleTicket = await _http.GetFromJsonAsync<List<ticketDetalleDTO>>("api/ticket/ListarDetalle");
            return View(datosDetalleTicket);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarTicketEstado(int id_ticket, string estado)
        {
            //CAMBIAR ESTO CUANDO HAYA VISTA DE TICKETS
            id_ticket = 2; // Cambiar por el id del ticket que se va a actualizar


            if (id_ticket <= 0 || string.IsNullOrEmpty(estado))
                return BadRequest("Datos inválidos");

            var data = new ticketEstadoUpdateModel
            {
                id_ticket = id_ticket,
                estado = estado
            };

            var response = await _http.PostAsJsonAsync("api/ticket/actualizarTicket", data);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] = "Ticket actualizado correctamente";
                return RedirectToAction("IndexDetalleTicket"); // o la vista donde estás mostrando el ticket
            }
            else
            {
                TempData["Error"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("IndexDetalleTicket");
            }
        }
    }
}
