using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Response._lugar
{
    public class LugarResponse
    {
        public Guid Id { get; set; }
        public string Identificador { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Estado { get; set; } = string.Empty; // "Disponible", "Ocupado", "Deshabilitado"
        public string? Patente { get; set; }
    }
}
