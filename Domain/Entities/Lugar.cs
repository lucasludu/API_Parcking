using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Lugar : BaseEntity
    {
        public string Identificador { get; set; } // e.g., "A1", "B2"
        
        public Guid CocheraId { get; set; }
        public Cochera Cochera { get; set; }
        
        public bool IsActive { get; set; } = true; // Enabled/Disabled for maintenance etc.
        public bool Eliminado { get; set; } = false; // Soft delete if needed, though BaseEntity likely handles some
        
        // Navigation properties
        public ICollection<Ticket> Tickets { get; set; }
    }
}
