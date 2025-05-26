using Microsoft.AspNetCore.Mvc;
using static FrontEndTicketPro.Models.Cliente.ClienteInicio;
using System.Text.Json;
using FrontEndTicketPro.Models.Cliente;
using System.Text;

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

        /* [SessionAuthorize("cliente")]
         public async Task<IActionResult> MiInfo()
         {
             var idUsuario = HttpContext.Session.GetInt32("id_usuario");
             if (idUsuario == null) return RedirectToAction("Index", "Login");

             var client = _httpClientFactory.CreateClient("ApiInsegura");
             string apiUrl = _configuration["ApiBaseUrl"] + $"/cliente/info/{idUsuario}";

             var response = await client.GetAsync(apiUrl);
             if (!response.IsSuccessStatusCode) return View(new MiInfo()); // opcional: mostrar error

             var json = await response.Content.ReadAsStringAsync();
             var data = JsonSerializer.Deserialize<JsonElement>(json);

             var model = new MiInfo
             {
                 NombreUsuario = data.GetProperty("nombre_usuario").GetString(),
                 Email = data.GetProperty("email").GetString(),
                 FechaRegistro = data.GetProperty("fecha_registro").GetString(),
                 Nombre = data.GetProperty("nombre").GetString(),
                 Apellido = data.GetProperty("apellido").GetString(),
                 Empresa = data.GetProperty("empresa").GetString()
             };

             return View(model);
         } 
        */

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
            else
            {             // puedes mostrar un mensaje si falla
                ViewBag.Error = "No se pudo guardar.";
                return View("MiInfo", model);
            }


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

    }
}
