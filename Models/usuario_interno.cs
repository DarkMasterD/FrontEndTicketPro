﻿using System.ComponentModel.DataAnnotations;

namespace FrontEndTicketPro.Models
{
    public class usuario_interno
    {
        [Key]
        public int id_usuario_interno { get; set; }
        public int id_usuario { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string direccion { get; set; }
        public string dui { get; set; }
        public int id_rol { get; set; }
    }
}
