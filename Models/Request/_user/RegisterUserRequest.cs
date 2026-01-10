namespace Models.Request._user
{
    public class RegisterUserRequest
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public Guid? CocheraId { get; set; }
    }
}
