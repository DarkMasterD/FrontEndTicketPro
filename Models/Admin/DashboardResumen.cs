namespace FrontEndTicketPro.Models.Admin
{
    public class DashboardResumen
    {
        public int TicketsNoAsignados { get; set; }
        public int TicketsEnProgreso { get; set; }
        public int TicketsCriticos { get; set; }
        public int TicketsResueltos { get; set; }
    }
}
