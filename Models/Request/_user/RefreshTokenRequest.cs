namespace Models.Request._user
{
    public class RefreshTokenRequest
    {
        // El JWT expirado
        public string Token { get; set; }
        // El token de refresco que tenía guardado
        public string RefreshToken { get; set; } 
    }
}
