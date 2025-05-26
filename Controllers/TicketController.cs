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

        //para las categorias 
        public async Task<IActionResult> CrearTicket()
        {
            var categorias = new List<categoria_ticket>();
            var response = await _http.GetAsync("/api/categoria");

            if (response.IsSuccessStatusCode)
            {
                categorias = await response.Content.ReadFromJsonAsync<List<categoria_ticket>>();
            }

            // Obtener usuarios
            var usuarios = new List<UsuarioDTO>();
            var responseUsuarios = await _http.GetAsync("/api/usuario");

            if (responseUsuarios.IsSuccessStatusCode)
            {
                usuarios = await responseUsuarios.Content.ReadFromJsonAsync<List<UsuarioDTO>>();
            }

            var modelo = new CrearTicketViewModel
            {
                Ticket = new ticket(),
                Categorias = categorias
            };

            ViewBag.Usuarios = usuarios;


            return View(modelo);
        }

        // POST: Ticket/Create
        [HttpPost]
        public async Task<IActionResult> CrearTicket(CrearTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Recargar categorías
                var categorias = new List<categoria_ticket>();
                var response = await _http.GetAsync("/api/categoria");
                if (response.IsSuccessStatusCode)
                {
                    categorias = await response.Content.ReadFromJsonAsync<List<categoria_ticket>>();
                }

                // Recargar usuarios
                var usuarios = new List<UsuarioDTO>();
                var responseUsuarios = await _http.GetAsync("/api/usuario");
                if (responseUsuarios.IsSuccessStatusCode)
                {
                    usuarios = await responseUsuarios.Content.ReadFromJsonAsync<List<UsuarioDTO>>();
                }

                // Restaurar las listas en el modelo
                model.Categorias = categorias;
                ViewBag.Usuarios = usuarios;

                return View(model);
            }

            // Ticket listo para guardar
            model.Ticket.fecha_inicio = DateTime.Now;
            model.Ticket.estado = "No asignado";

            var result = await _http.PostAsJsonAsync("api/ticket", model.Ticket);

            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var errorMsg = await result.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"No se pudo crear el ticket. Respuesta: {errorMsg}");

                // Recargar listas si falla
                model.Categorias = await _http.GetFromJsonAsync<List<categoria_ticket>>("/api/categoria");
                ViewBag.Usuarios = await _http.GetFromJsonAsync<List<UsuarioDTO>>("/api/usuario");

                return View(model);
            }

        }

        public async Task<IActionResult> IndexDetalleTicket(int id_ticket)
        {
            
            var datosDetalleTicket = await _http.GetFromJsonAsync<List<ticketDetalleDTO>>($"api/ticket/ListarDetalle?idTicket={id_ticket}");
            ViewBag.id_ticket = id_ticket; // Guardar el id_ticket en ViewBag para usarlo en la vista
            return View(datosDetalleTicket);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarTicketEstado(int id_ticket, string estado, string nombre_progreso, string descripcion_progreso)
        {

            if (id_ticket <= 0 || string.IsNullOrEmpty(estado))
                return BadRequest("Datos inválidos");

            var data = new ticketEstadoUpdateModel
            {
                id_ticket = id_ticket,
                estado = estado,
                id_usuario_interno = 1, // Cambia esto por el ID del usuario interno que está actualizando el ticket(httpsession)
                nombre_progreso = nombre_progreso,
                descripcion_progreso = descripcion_progreso
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

        public async Task<IActionResult> IndexTareasTicket(int id_ticket)
        {
            var datosTareaTicket = await _http.GetFromJsonAsync<tareaTicketViewModel>($"api/ticket/VerTareasDelTicket?idTicket={id_ticket}");
            ViewBag.id_ticket = id_ticket; // Guardar el id_ticket en ViewBag para usarlo en la vista
            return View(datosTareaTicket);
        }
    }
}
