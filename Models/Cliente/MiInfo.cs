namespace FrontEndTicketPro.Models.Cliente
{
    public class MiInfo
    {
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string FechaRegistro { get; set; }

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Empresa { get; set; }
        public List<ContactoUsuario> Contactos { get; set; } = new();
        public ContactoUsuario NuevoContacto { get; set; } = new();
    }
}
