using FrontEndTicketPro.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using System.Diagnostics;
using System.Diagnostics;

namespace FrontEndTicketPro.Controllers
{
    public class TecnicoController : Controller
    {

        private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClient _http;

        public TecnicoController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _http = _clientFactory.CreateClient("ApiInsegura");
        }
        [SessionAuthorize("tecnico")]


    public async Task<IActionResult> Inicio()
    {
        int? idInterno = HttpContext.Session.GetInt32("idUsuarioInterno");
        Debug.WriteLine("✔ idUsuarioInterno en sesión: " + idInterno);

        if (idInterno == null)
        return RedirectToAction("Login", "Cuenta");

        try
        {
             var response = await _http.GetAsync($"https://localhost:7141/api/Tecnico/dashboard-tecnico/{idInterno.Value}");
             var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"❌ Estado: {response.StatusCode}");
                Debug.WriteLine($"❌ Error body: {json}");

                TempData["mensaje"] = $"Error {response.StatusCode}";
                return RedirectToAction("Error");
            }

            Debug.WriteLine("✅ Respuesta recibida:");
            Debug.WriteLine(json);

            var dashboard = JsonSerializer.Deserialize<DashboardTecnicoDTO>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
                Debug.WriteLine("Resumen:");
                foreach (var r in dashboard.Resumen)
                    Debug.WriteLine($"{r.Estado}: {r.Cantidad}");

                Debug.WriteLine("Tickets:");
                foreach (var t in dashboard.Tickets)
                    Debug.WriteLine($"{t.Id_Ticket} - {t.Titulo} ({t.Estado})");


                return View(dashboard);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("❌ EXCEPCIÓN:");
            Debug.WriteLine(ex.Message);

            TempData["mensaje"] = "Excepción al conectar con la API: " + ex.Message;
            return RedirectToAction("Error");
        }
    }


    [SessionAuthorize("tecnico")]
        public async Task<IActionResult> MiInformacion()
        {
            var http = _clientFactory.CreateClient();
            http.BaseAddress = new Uri("https://localhost:7141");

            int? idUsuario = HttpContext.Session.GetInt32("id_usuario");
            if (idUsuario == null)
            {
                TempData["Error"] = "No se pudo cargar el perfil.";
                return RedirectToAction("Inicio");
            }

            var response = await http.GetAsync($"/api/usuario/perfil-tecnico/{idUsuario.Value}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al obtener el perfil.";
                return RedirectToAction("Inicio");
            }

            var perfil = await response.Content.ReadFromJsonAsync<TecnicoPerfilViewModel>();
            return View(perfil);
        }


    }
}

