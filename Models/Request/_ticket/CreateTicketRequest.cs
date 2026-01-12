namespace Models.Request._ticket
{
    public class CreateTicketRequest
    {
        public Guid CocheraId { get; set; }
        public Guid? LugarId { get; set; } // Optional, can be null if just tracking entry without spot
        public string Patente { get; set; }
    }
}
