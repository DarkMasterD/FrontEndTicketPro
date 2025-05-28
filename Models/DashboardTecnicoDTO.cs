namespace FrontEndTicketPro.Models
{
    public class DashboardTecnicoDTO
    {
        public List<ResumenTicketDTO> Resumen { get; set; }
        public List<TicketTecnicoDTO> Tickets { get; set; }
    }

    public class ResumenTicketDTO
    {
        public string Estado { get; set; }
        public int Cantidad { get; set; }
    }

    public class TicketTecnicoDTO
    {
        public int Id_Ticket { get; set; }
        public string Titulo { get; set; }
        public string Estado { get; set; }
    }

}
