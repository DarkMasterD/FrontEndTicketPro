using System.ComponentModel.DataAnnotations;
using System.Data;

namespace FrontEndTicketPro.Models
{
    public class usuario_externo
    {
        [Key]
        public int id_usuario_externo { get; set; }
        public int id_usuario { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string empresa { get; set; }
    }
}
