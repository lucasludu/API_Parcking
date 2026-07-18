namespace Models.Request._lugar
{
    public class CreateLugarRequest
    {
        public string Identificador { get; set; } = string.Empty;
        public Guid CocheraId { get; set; }
    }
}
