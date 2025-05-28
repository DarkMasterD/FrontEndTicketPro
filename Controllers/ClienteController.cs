using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;
using FrontEndTicketPro.Models;
using FrontEndTicketPro.Models.Cliente;
using static FrontEndTicketPro.Models.Cliente.ClienteInicio;

namespace FrontEndTicketPro.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ClienteController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [SessionAuthorize("cliente")]
        public async Task<IActionResult> Inicio()
        {
            var idUsuario = HttpContext.Session.GetInt32("id_usuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var client = _httpClientFactory.CreateClient("ApiInsegura");
            string baseUrl = _configuration["ApiBaseUrl"];

            var viewModel = new ClienteInicio();

            // Obtener nombre
            var nombreResp = await client.GetAsync($"{baseUrl}/cliente/nombre/{idUsuario}");
            if (nombreResp.IsSuccessStatusCode)
            {
                var json = await nombreResp.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(json);
                viewModel.NombreCliente = data.GetProperty("nombre").GetString();
            }

            // Obtener resumen
            var resumenResp = await client.GetAsync($"{baseUrl}/cliente/resumen/{idUsuario}");
            if (resumenResp.IsSuccessStatusCode)
            {
                var json = await resumenResp.Content.ReadAsStringAsync();
                var resumen = JsonSerializer.Deserialize<JsonElement>(json);
                viewModel.NoAsignado = resumen.GetProperty("no_asignado").GetInt32();
                viewModel.EnProgreso = resumen.GetProperty("en_progreso").GetInt32();
                viewModel.Resuelto = resumen.GetProperty("resuelto").GetInt32();
            }

            // Obtener últimos tickets
            var ticketsResp = await client.GetAsync($"{baseUrl}/cliente/ultimos/{idUsuario}");
            if (ticketsResp.IsSuccessStatusCode)
            {
                var json = await ticketsResp.Content.ReadAsStringAsync();
                viewModel.UltimosTickets = JsonSerializer.Deserialize<List<TicketResumen>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return View(viewModel);
        }

        [SessionAuthorize("cliente")]
        public async Task<IActionResult> CrearTicketCliente()
        {
            var _http = _httpClientFactory.CreateClient();
            _http.BaseAddress = new Uri("https://localhost:7141");

            int? id_usuario = HttpContext.Session.GetInt32("id_usuario");
            if (id_usuario == null)
            {
                TempData["Error"] = "No se ha iniciado sesión correctamente.";
                return RedirectToAction("Inicio");
            }

            var responseUsuario = await _http.GetAsync($"/api/usuario/{id_usuario.Value}");
            if (!responseUsuario.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudieron obtener los datos del usuario.";
                return RedirectToAction("Inicio");
            }

            var usuario = await responseUsuario.Content.ReadFromJsonAsync<UsuarioDTO>();
            var categorias = await _http.GetFromJsonAsync<List<categoria_ticket>>("/api/categoria");

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
            var http = _httpClientFactory.CreateClient();
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
            var http = _httpClientFactory.CreateClient();
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

        [SessionAuthorize("cliente")]
        public async Task<IActionResult> MiInfo()
        {
            try
            {
                var idUsuario = HttpContext.Session.GetInt32("id_usuario");
                if (idUsuario == null) return RedirectToAction("Index", "Login");

                var client = _httpClientFactory.CreateClient("ApiInsegura");
                var baseUrl = _configuration["ApiBaseUrl"];

                var model = new MiInfo();

                // Datos personales
                var infoResp = await client.GetAsync($"{baseUrl}/cliente/info/{idUsuario}");
                if (infoResp.IsSuccessStatusCode)
                {
                    var json = await infoResp.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    model.NombreUsuario = data.GetProperty("nombre_usuario").GetString();
                    model.Email = data.GetProperty("email").GetString();
                    model.FechaRegistro = data.GetProperty("fecha_registro").GetString();
                    model.Nombre = data.GetProperty("nombre").GetString();
                    model.Apellido = data.GetProperty("apellido").GetString();
                    model.Empresa = data.GetProperty("empresa").GetString();
                }

                // Contactos
                var contactoResp = await client.GetAsync($"{baseUrl}/cliente/contacto/{idUsuario}");
                if (contactoResp.IsSuccessStatusCode)
                {
                    var json = await contactoResp.Content.ReadAsStringAsync();
                    model.Contactos = JsonSerializer.Deserialize<List<ContactoUsuario>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error interno: " + ex.Message;
                return RedirectToAction("Inicio");
            }
        }

        [HttpPost]
        [SessionAuthorize("cliente")]
        public async Task<IActionResult> GuardarMiInfo(MiInfo model)
        {
            var idUsuario = HttpContext.Session.GetInt32("id_usuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var client = _httpClientFactory.CreateClient("ApiInsegura");
            string apiUrl = _configuration["ApiBaseUrl"] + $"/cliente/info/{idUsuario}";

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Exito"] = "¡Datos actualizados correctamente!";
                return RedirectToAction("MiInfo");
            }

            ViewBag.Error = "No se pudo guardar.";
            return View("MiInfo", model);
        }

        [HttpPost]
        [SessionAuthorize("cliente")]
        public async Task<IActionResult> AgregarContacto(MiInfo model)
        {
            var idUsuario = HttpContext.Session.GetInt32("id_usuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            model.NuevoContacto.IdUsuario = idUsuario.Value;

            var client = _httpClientFactory.CreateClient("ApiInsegura");
            var apiUrl = _configuration["ApiBaseUrl"] + "/cliente/contacto";

            var json = JsonSerializer.Serialize(model.NuevoContacto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Contacto guardado correctamente.";
            }
            else
            {
                var errorApi = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Error al guardar contacto: " + errorApi;
            }

            return RedirectToAction("MiInfo");
        }

        [HttpPost]
        [SessionAuthorize("cliente")]
        public async Task<IActionResult> CambiarContrasenia(CambioContrasenia model)
        {
            var idUsuario = HttpContext.Session.GetInt32("id_usuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var client = _httpClientFactory.CreateClient("ApiInsegura");
            var apiUrl = _configuration["ApiBaseUrl"] + "/cliente/cambiar-contrasenia";

            var datos = new
            {
                id_usuario = idUsuario,
                actual = model.Actual,
                nueva = model.Nueva,
                confirmacion = model.Confirmacion
            };

            var json = JsonSerializer.Serialize(datos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Exito"] = "¡Contraseña actualizada correctamente!";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Error al cambiar contraseña: " + error;
            }

            return RedirectToAction("MiInfo");
        }

        [HttpPost]
        [SessionAuthorize("cliente")]
        public async Task<IActionResult> EditarContacto(ContactoEdit model)
        {
            var client = _httpClientFactory.CreateClient("ApiInsegura");
            var apiUrl = _configuration["ApiBaseUrl"] + $"/cliente/contacto/" + model.IdContacto;

            var json = JsonSerializer.Serialize(new
            {
                email = model.Email,
                telefono = model.Telefono
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
                TempData["Exito"] = "Contacto actualizado correctamente.";
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Error al editar contacto: " + error;
            }

            return RedirectToAction("MiInfo");
        }
    }
}

