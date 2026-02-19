using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Lugar : BaseEntity
    {
        public string Identificador { get; set; } // e.g., "A1", "B2"
        
        public Guid CocheraId { get; set; }
        public Cochera Cochera { get; set; }
        

        public bool Eliminado { get; set; } = false; // Soft delete if needed, though BaseEntity likely handles some

        [Timestamp]
        public byte[] RowVersion { get; set; } // For concurrency control

        // Navigation properties
        public ICollection<Ticket> Tickets { get; set; }
    }
}
