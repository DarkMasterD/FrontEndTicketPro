using FrontEndTicketPro.Models;
using Microsoft.AspNetCore.Mvc;

namespace FrontEndTicketPro.Controllers
{
    public class TecnicoController : Controller
    {

        private readonly IHttpClientFactory _clientFactory;

        public TecnicoController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        [SessionAuthorize("tecnico")]

        public IActionResult Inicio()
        {
            // Esto es solo para desarrollo, luego se puede eliminar
            return View();
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
        // TécnicoController.cs
        [SessionAuthorize("tecnico")]
        public async Task<IActionResult> HistorialTecnico()
        {
            var http = _clientFactory.CreateClient();
            http.BaseAddress = new Uri("https://localhost:7141");

            int? idTecnico = HttpContext.Session.GetInt32("id_usuario");
            if (idTecnico == null)
            {
                TempData["Error"] = "Sesión inválida.";
                return RedirectToAction("Inicio");
            }

            var response = await http.GetAsync($"/api/ticket/historial-tecnico/{idTecnico}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo obtener el historial.";
                return RedirectToAction("Inicio");
            }

            var historial = await response.Content.ReadFromJsonAsync<List<HistorialTicketDTO>>();
            return View(historial);
        }


    }
}

