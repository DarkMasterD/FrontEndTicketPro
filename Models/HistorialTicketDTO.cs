namespace FrontEndTicketPro.Models
{
    public class HistorialTicketDTO
    {
        public int IdTicket { get; set; }
        public string Titulo { get; set; }
        public string Estado { get; set; }
        public string Categoria { get; set; }
        public string Prioridad { get; set; }
        public DateTime Fecha { get; set; }
    }
}
