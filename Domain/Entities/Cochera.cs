using Domain.Common;

namespace Domain.Entities
{
    public class Cochera : BaseEntity
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public int CapacidadTotal { get; set; }
        public string ImagenUrl { get; set; }
        
        // Owner (Dueño de la cochera)
        public string OwnerId { get; set; }
        
        // Navigation Properties
        public ICollection<Tarifa> Tarifas { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<ApplicationUser> Empleados { get; set; }
    }
}
