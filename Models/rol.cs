using System.ComponentModel.DataAnnotations;

namespace FrontEndTicketPro.Models
{
    public class rol
    {
        [Key]
        public int id_rol { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
    }
}
