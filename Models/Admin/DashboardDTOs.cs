namespace FrontEndTicketPro.Models.Admin
{
    public class TicketsPorCategoria
    {
        public string Categoria { get; set; }
        public int Tickets { get; set; }
    }

    public class RendimientoTecnico
    {
        public string Tecnico { get; set; }
        public int TicketsResueltos { get; set; }
    }

  
}
