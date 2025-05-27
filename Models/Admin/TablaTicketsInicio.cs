namespace FrontEndTicketPro.Models.Admin
{
    public class TablaTicketsInicio
    {
        public int IdTicket { get; set; }
        public string Titulo { get; set; }
        public string Cliente { get; set; }
        public string Tecnico { get; set; }
        public string Estado { get; set; }
        public string Prioridad { get; set; }
        public DateTime Fecha { get; set; }

    }

    public class TicketsAdminViewModel
    {
        public string? Busqueda { get; set; }
        public string? Estado { get; set; }
        public string? Prioridad { get; set; }
        public List<TablaTicketsInicio> TicketsFiltrados { get; set; } = new();
    }

    public class TecnicoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
