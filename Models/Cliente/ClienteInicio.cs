namespace FrontEndTicketPro.Models.Cliente
{
    public class ClienteInicio
    {

      
            public string NombreCliente { get; set; }
            public int NoAsignado { get; set; }
            public int EnProgreso { get; set; }
            public int Resuelto { get; set; }

            public List<TicketResumen> UltimosTickets { get; set; }
        

    }
}
