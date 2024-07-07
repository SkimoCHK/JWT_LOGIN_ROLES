namespace JWT_API.Models.Custom
{
    public class RefreshTokenRequest
    {
        public string? tokenExpirado {  get; set; }

        public string? refreshToken { get; set; }
    }
}
