using Microsoft.AspNetCore.Mvc;

namespace FrontEndTicketPro.Controllers
{
    public class ClienteController : Controller
    {
        [SessionAuthorize("cliente")]

        public IActionResult Inicio()
        {
            // Esto es solo para desarrollo, luego se puede eliminar
            return View();
        }
    }
}
