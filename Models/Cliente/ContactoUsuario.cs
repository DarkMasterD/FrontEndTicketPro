using System.Text.Json.Serialization;

namespace FrontEndTicketPro.Models.Cliente
{
    public class ContactoUsuario
    {

        public int IdContactoUsuario { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }


        [System.Text.Json.Serialization.JsonPropertyName("id_usuario")]
        public int IdUsuario { get; set; }


    }
}
