namespace Models.Response._user
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public List<string>? Roles { get; set; }
        public string RefreshToken { get; set; }
    }
}
