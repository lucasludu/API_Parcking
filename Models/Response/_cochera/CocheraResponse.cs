namespace Models.Response._cochera
{
    public class CocheraResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public int CapacidadTotal { get; set; }
        public string ImagenUrl { get; set; }
        public string OwnerId { get; set; }
        public bool IsActive { get; set; }
    }
}
