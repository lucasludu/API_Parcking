namespace Models.Request._user
{
    public class RefreshTokenRequest
    {
        // El JWT expirado
        public string Token { get; set; } = string.Empty;
        // El token de refresco que tenía guardado
        public string RefreshToken { get; set; } = string.Empty; 
    }
}
