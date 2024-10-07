namespace EscCase.Common.Entities.App
{
    public class JwtOptions
    {
        public string AccessTokenSecret { get; set; } = string.Empty;
        public double AccessTokenExpirationMinutes { get; set; }
        public double RefreshTokenExpirationMinutes { get; set; }
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
