using Microsoft.AspNetCore.Mvc;

namespace FrontEndTicketPro.Controllers
{
    public class TecnicoController : Controller
    {
        [SessionAuthorize("tecnico")]

        public IActionResult Inicio()
        {
            // Esto es solo para desarrollo, luego se puede eliminar
            return View();
        }
    }
}

