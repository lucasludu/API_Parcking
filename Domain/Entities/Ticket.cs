using Domain.Common;
using Domain.Enums;
using System;

namespace Domain.Entities
{
    public class Ticket : BaseEntity
    {
        public Guid CocheraId { get; set; }
        public Cochera Cochera { get; set; }

        public Guid? LugarId { get; set; }
        public Lugar Lugar { get; set; }
        
        public string Patente { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaSalida { get; set; }
        public decimal? Total { get; set; }
        
        // Enum: Activo, Pagado, Cancelado
        public TicketEstado Estado { get; set; } 

        public string UsuarioEntradaId { get; set; } // Empleado que registró la entrada
    }
}
