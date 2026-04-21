namespace Application.Common.Models;

public record AuthResponse(string AccessToken, DateTime AccessTokenExpiresAt, string RefreshToken);

