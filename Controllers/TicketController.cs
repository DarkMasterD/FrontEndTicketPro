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

        public async Task<IActionResult> IndexDetalleTicket(int id_ticket)
        {
            //SE TIENE QUE RECUPERAR EL ID DEL TICKET DE LA PANTALLA ANTERIOR
            var datosDetalleTicket = await _http.GetFromJsonAsync<List<ticketDetalleDTO>>($"api/ticket/ListarDetalle?idTicket={id_ticket}");
            ViewBag.id_ticket = id_ticket; // Guardar el id_ticket en ViewBag para usarlo en la vista
            return View(datosDetalleTicket);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarTicketEstado(int id_ticket, string estado)
        {

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
                return RedirectToAction("IndexDetalleTicket", new { id_ticket = id_ticket}); // o la vista donde estás mostrando el ticket
            }
            else
            {
                TempData["Error"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("IndexDetalleTicket");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CerrarTicket(int id_ticket)
        {
            if (id_ticket <= 0)
                return BadRequest("ID inválido");

            var data = new ticketEstadoUpdateModel
            {
                id_ticket = id_ticket,
                estado = "Resuelto",
                cerrar_ticket = true 
            };

            var response = await _http.PostAsJsonAsync("api/ticket/actualizarTicket", data);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] = "Ticket cerrado correctamente";
                return RedirectToAction("IndexDetalleTicket", new { id_ticket = id_ticket });
            }
            else
            {
                TempData["Error"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("IndexDetalleTicket", new { id_ticket = id_ticket });
            }
        }
    }
}
