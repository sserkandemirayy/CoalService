namespace Infrastructure.Security;

public class JwtSettings
{
    public string Issuer { get; set; } = "AuthService";
    public string Audience { get; set; } = "AuthServiceAudience";
    public string Secret { get; set; } = "super_long_development_secret_change_me";
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;
}
