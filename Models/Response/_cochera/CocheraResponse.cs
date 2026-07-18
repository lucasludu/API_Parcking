namespace Models.Response._cochera
{
    public class CocheraResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public int CapacidadTotal { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
