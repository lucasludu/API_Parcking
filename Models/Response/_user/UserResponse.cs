namespace Models.Response._user
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? CocheraId { get; set; }
        public bool IsActive { get; set; }
    }
}
