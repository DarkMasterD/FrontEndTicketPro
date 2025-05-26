using FrontEndTicketPro.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace FrontEndTicketPro.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public ClienteController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [SessionAuthorize("cliente")]
        public IActionResult Inicio()
        {
            return View();
        }

        [SessionAuthorize("cliente")]
        public async Task<IActionResult> CrearTicketCliente()
        {
            var _http = _clientFactory.CreateClient();
            _http.BaseAddress = new Uri("https://localhost:7141");

            // ✅ Validar existencia y tipo del ID en sesión
            int? id_usuario = HttpContext.Session.GetInt32("id_usuario");

            if (id_usuario == null)
            {
                TempData["Error"] = "No se ha iniciado sesión correctamente.";
                return RedirectToAction("Inicio");
            }

            // Obtener usuario
            var responseUsuario = await _http.GetAsync($"/api/usuario/{id_usuario.Value}");
            if (!responseUsuario.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudieron obtener los datos del usuario.";
                return RedirectToAction("Inicio");
            }

            var usuario = await responseUsuario.Content.ReadFromJsonAsync<UsuarioDTO>();

            // Obtener categorías
            var categorias = await _http.GetFromJsonAsync<List<categoria_ticket>>("/api/categoria");

            // Enviar a la vista
            ViewBag.IdUsuario = usuario.id_usuario;
            ViewBag.Nombre = usuario.nombre_usuario;
            ViewBag.Correo = usuario.email;
            ViewBag.Telefono = usuario.telefono;

            var model = new CrearTicketViewModel
            {
                Ticket = new ticket(),
                Categorias = categorias
            };

            return View(model);
        }

        [SessionAuthorize("cliente")]
        public async Task<IActionResult> MisTickets()
        {
            var http = _clientFactory.CreateClient();
            http.BaseAddress = new Uri("https://localhost:7141");

            int? idUsuario = HttpContext.Session.GetInt32("id_usuario");

            if (idUsuario == null)
            {
                TempData["Error"] = "No se ha iniciado sesión correctamente.";
                return RedirectToAction("Inicio");
            }

            var response = await http.GetAsync($"/api/ticket/tickets-cliente/{idUsuario.Value}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudieron obtener los tickets.";
                return RedirectToAction("Inicio");
            }

            var tickets = await response.Content.ReadFromJsonAsync<List<TicketClienteDTO>>();
            return View(tickets);
        }

        [SessionAuthorize("cliente")]
        public async Task<IActionResult> DetalleTicket(int id)
        {
            var http = _clientFactory.CreateClient();
            http.BaseAddress = new Uri("https://localhost:7141");

            var response = await http.GetAsync($"/api/ticket/detalle/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo obtener el detalle del ticket.";
                return RedirectToAction("MisTickets");
            }

            var detalle = await response.Content.ReadFromJsonAsync<TicketDetalleViewModel>();

            var respProgreso = await http.GetAsync($"/api/ticket/progreso/{id}");
            if (respProgreso.IsSuccessStatusCode)
            {
                var lista = await respProgreso.Content.ReadFromJsonAsync<List<ProgresoDTO>>();
                detalle.Progresos = lista;
            }

            return View(detalle);
        }
    }
}
