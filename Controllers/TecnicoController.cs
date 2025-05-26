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


    }
}

