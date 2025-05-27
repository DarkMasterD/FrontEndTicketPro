using FrontEndTicketPro.Models;
using FrontEndTicketPro.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace FrontEndTicketPro.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        //Agregue esta linea
        private readonly HttpClient _http;

        public AdminController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            //Agregue esta linea
            _http = httpClientFactory.CreateClient("ApiInsegura");
            _http.BaseAddress = new Uri(configuration["ApiBaseUrl"]);
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

        public async Task<IActionResult> TodosTickets(string buscarTexto, string estado, string prioridad, DateTime? fechaDesde, DateTime? fechaHasta, int? idTicket)
        {
            var modelo = new TicketsAdminViewModel
            {
                Busqueda = buscarTexto,
                Estado = estado,
                Prioridad = prioridad,
            };

            var tickets = await _http.GetFromJsonAsync<List<TablaTicketsInicio>>("api/ticket/gestion-tickets");
            if (tickets != null)
            {
                var resultado = tickets.AsQueryable();

                if (!string.IsNullOrWhiteSpace(buscarTexto))
                    resultado = resultado.Where(t => t.Titulo.ToLower().Contains(buscarTexto.ToLower()) || t.Cliente.ToLower().Contains(buscarTexto.ToLower()));

                if (!string.IsNullOrWhiteSpace(estado))
                    resultado = resultado.Where(t => t.Estado.ToLower() == estado.ToLower());

                if (!string.IsNullOrWhiteSpace(prioridad))
                    resultado = resultado.Where(t => t.Prioridad.ToLower() == prioridad.ToLower());

                if (fechaDesde.HasValue)
                    resultado = resultado.Where(t => t.Fecha >= fechaDesde.Value);

                if (fechaHasta.HasValue)
                    resultado = resultado.Where(t => t.Fecha <= fechaHasta.Value);

                if (idTicket.HasValue)
                    resultado = resultado.Where(t => t.IdTicket == idTicket.Value);

                modelo.TicketsFiltrados = resultado.ToList();
            }
            else
            {
                modelo.TicketsFiltrados = new List<TablaTicketsInicio>();
            }

            return View("GestionTickets", modelo); 
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTecnicos()
        {
            try
            {
                var tecnicos = await _http.GetFromJsonAsync<List<TecnicoDTO>>("api/TareaTicket/usuarios-internos");
                return Json(tecnicos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener técnicos: {ex.Message}");
            }
        }

        public async Task<IActionResult> Usuarios(string busqueda, int? filtroRol)
        {
            var response = await _http.GetFromJsonAsync<List<UsuarioListadoDTO>>(
                $"api/usuarios/listar?busqueda={busqueda}&rolId={filtroRol}");

            var roles = await _http.GetFromJsonAsync<List<RolDTO>>("api/Rol");

            var viewModel = new UsuariosAdminViewModel
            {
                Busqueda = busqueda,
                FiltroRol = filtroRol,
                Usuarios = response,
                Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.IdRol.ToString(),
                    Text = r.Nombre
                }).ToList()
            };

            return View("GestionUsuarios", viewModel);
        }

        public IActionResult SeleccionarTipo()
        {
            return View("SeleccionarTipoUsuario"); 
        }

        public IActionResult CrearInterno()
        {
            // Retorna la vista del formulario para crear usuario interno
            return View("CrearInterno");
        }

        public IActionResult CrearExterno()
        {
            var modelo = new UsuarioExternoViewModel(); // crea una instancia vacía
            return View("CrearExterno", modelo);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarExterno(UsuarioExternoViewModel model)
        {

            if (model.IdUsuario.HasValue)
            {
                // Aquí deberías hacer PUT a api/usuarios/actualizar-externo

                var response = await _http.PutAsJsonAsync("api/usuarios/actualizar-externo", model);
                var rawJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", $"Error al actualizar usuario: {rawJson}");
                    return View("CrearExterno", model);
                }

                TempData["mensaje"] = "Usuario actualizado correctamente.";
                return RedirectToAction("Usuarios");

            }
            else
            {
                // flujo actual: POST para crear nuevo
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("❌ ModelState inválido");
                    return View("CrearExterno", model);
                }

                if (!ModelState.IsValid)
                    return View("CrearExterno", model);

                var response = await _http.PostAsJsonAsync("api/usuarios/crear-externo", model);
                var rawJson = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    ModelState.AddModelError("", rawJson); // el mensaje claro ya viene del backend
                    return View("CrearExterno", model);
                }

                if (response.IsSuccessStatusCode)
                {
                    var resultado = JsonSerializer.Deserialize<UsuarioCreadoResponse>(rawJson);

                    if (resultado == null || resultado.idUsuario == 0)
                    {
                        ModelState.AddModelError("", "No se pudo obtener el ID del usuario creado.");
                        Console.WriteLine($"Respuesta: {rawJson}");
                        return View("CrearExterno", model);
                    }

                    return RedirectToAction("ContactosUsuario", new { id = resultado.idUsuario });
                }
                else
                {
                    ModelState.AddModelError("", "Error desde API: " + rawJson);
                    return View("CrearExterno", model);
                }
            }       
        }
        public async Task<IActionResult> ContactosUsuario(int id)
        {
            var contactos = await _http.GetFromJsonAsync<List<ContactoUsuarioDTO>>($"api/contactos/usuario/{id}");

            var modelo = new ContactosUsuarioViewModel
            {
                IdUsuario = id,
                Correos = contactos.Where(c => !string.IsNullOrEmpty(c.email)).ToList(),
                Telefonos = contactos.Where(c => !string.IsNullOrEmpty(c.telefono)).ToList()
            };

            return View("AgregarContacto", modelo);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarContactos(ContactosUsuarioViewModel model)
        {
            if (!string.IsNullOrEmpty(model.NuevoCorreo))
            {
                var contactoCorreo = new ContactoUsuarioDTO
                {
                    id_usuario = model.IdUsuario,
                    email = model.NuevoCorreo,
                    telefono = null
                };

                await _http.PostAsJsonAsync("api/contactos/crear", contactoCorreo);
            }

            if (!string.IsNullOrEmpty(model.NuevoTelefono))
            {
                var contactoTelefono = new ContactoUsuarioDTO
                {
                    id_usuario = model.IdUsuario,
                    telefono = model.NuevoTelefono,
                    email = null
                };

                await _http.PostAsJsonAsync("api/contactos/crear", contactoTelefono);
            }


            return RedirectToAction("Usuarios");
        }

        [HttpPost]
        public async Task<IActionResult> AgregarContactoTemporal(ContactosUsuarioViewModel model)
        {
            bool huboError = false;

            if (!string.IsNullOrEmpty(model.NuevoCorreo))
            {
                var response = await _http.PostAsJsonAsync("api/contactos/crear", new
                {
                    id_usuario = model.IdUsuario,
                    email = model.NuevoCorreo
                });

                if (!response.IsSuccessStatusCode)
                {
                    var mensaje = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("NuevoCorreo", $"Error al guardar correo: {mensaje}");
                    huboError = true;
                }
            }

            if (!string.IsNullOrEmpty(model.NuevoTelefono))
            {
                var response = await _http.PostAsJsonAsync("api/contactos/crear", new
                {
                    id_usuario = model.IdUsuario,
                    telefono = model.NuevoTelefono
                });

                if (!response.IsSuccessStatusCode)
                {
                    var mensaje = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("NuevoTelefono", $"Error al guardar teléfono: {mensaje}");
                    huboError = true;
                }
            }

            if (huboError)
            {
                // Recargar contactos actuales
                var contactos = await _http.GetFromJsonAsync<List<ContactoUsuarioDTO>>($"api/contactos/usuario/{model.IdUsuario}");

                model.Correos = contactos.Where(c => !string.IsNullOrEmpty(c.email)).ToList();
                model.Telefonos = contactos.Where(c => !string.IsNullOrEmpty(c.telefono)).ToList();

                return View("AgregarContacto", model); // Repite vista con errores visibles
            }

            return RedirectToAction("ContactosUsuario", new { id = model.IdUsuario });
        }

        [HttpGet]
        public async Task<IActionResult> EditarExterno(int id)
        {
            var response = await _http.GetAsync($"api/usuarios/externo/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["mensaje"] = "No se pudo cargar el usuario.";
                return RedirectToAction("Usuarios");
            }

            var usuario = await response.Content.ReadFromJsonAsync<UsuarioExternoViewModel>();
            return View("CrearExterno", usuario); // reutiliza la misma vista
        }


    }
}
