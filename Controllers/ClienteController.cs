using Microsoft.AspNetCore.Mvc;

namespace FrontEndTicketPro.Controllers
{
    public class ClienteController : Controller
    {
        [SessionAuthorize("cliente")]

        public IActionResult Inicio()
        {
            return View();
        }
    }
}
