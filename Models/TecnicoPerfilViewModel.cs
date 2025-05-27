using System.ComponentModel.DataAnnotations;

namespace FrontEndTicketPro.Models
{
    public class TecnicoPerfilViewModel
    {
        public int IdUsuario { get; set; }
        [Required(ErrorMessage = "Los nombres son requeridos")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Apellidos { get; set; }


        [Required(ErrorMessage = "La dirección es requerida")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El usuario es requerido")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Usuario { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; }

        public List<ContactoViewModel> Contactos { get; set; } = new List<ContactoViewModel>();
    }

    public class ContactoViewModel
    {
        public int Id { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        public string Email { get; set; }

        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Formato: 9999-9999")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        public string Telefono { get; set; }
    }
}
