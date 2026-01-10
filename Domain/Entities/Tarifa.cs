using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Tarifa : BaseEntity
    {
        public Guid CocheraId { get; set; }
        public Cochera Cochera { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }
        public decimal PrecioHora { get; set; }
        public decimal PrecioEstadia { get; set; } // Tarifa diaria o estadía completa
    }
}
