using Microsoft.AspNetCore.Mvc.Rendering;

namespace FrontEndTicketPro.Models.Admin
{
    public class UsuariosAdminViewModel
    {
        public int? IdUsuario { get; set; }
        public string Busqueda { get; set; }
        public int? FiltroRol { get; set; }
        public List<UsuarioListadoDTO> Usuarios { get; set; }
        public List<SelectListItem> Roles { get; set; }
    }

}
