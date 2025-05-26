namespace FrontEndTicketPro.Models
{
    public class TicketClienteDTO
    {
        public int id_ticket { get; set; }
        public string titulo { get; set; }
        public string estado { get; set; }
        public string categoria { get; set; }
        public string prioridad { get; set; }
        public DateTime fecha_inicio { get; set; }

        public string GetPrioridadColor()
        {
            return prioridad switch
            {
                "Crítico" => "danger",
                "Importante" => "primary",
                "Baja" => "warning",
                _ => "secondary"
            };
        }

        public string GetEstadoColor()
        {
            return estado switch
            {
                "En Progreso" => "info",
                "No asignado" => "warning",
                "Resuelto" => "success",
                "En espera de información del cliente" => "secondary",
                _ => "dark"
            };
        }
    }
}
