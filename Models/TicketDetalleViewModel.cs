namespace FrontEndTicketPro.Models
{
    public class TicketDetalleViewModel
    {
        public int IdTicket { get; set; }
        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public string Servicio { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public string Prioridad { get; set; }
        public string Estado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? UrlArchivo { get; set; }

        public List<ProgresoDTO> Progresos { get; set; } = new();

        public string GetPrioridadColor() => Prioridad switch
        {
            "Crítico" => "danger",
            "Importante" => "primary",
            "Baja" => "warning",
            _ => "secondary"
        };

        public string GetEstadoColor() => Estado switch
        {
            "En Progreso" => "info",
            "No asignado" => "warning",
            "Resuelto" => "success",
            "En espera de información del cliente" => "secondary",
            _ => "dark"
        };
    }
    public class ProgresoDTO
    {
        public string NombreTecnico { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
