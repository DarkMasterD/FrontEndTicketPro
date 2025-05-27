namespace FrontEndTicketPro.Models.Admin
{
    public class TicketFiltroViewModel
    {
        public int? IdTicket { get; set; }
        public string BuscarTexto { get; set; }
        public string Estado { get; set; }
        public string Prioridad { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        public List<TablaTicketsInicio> TicketsFiltrados { get; set; }
    }
}
