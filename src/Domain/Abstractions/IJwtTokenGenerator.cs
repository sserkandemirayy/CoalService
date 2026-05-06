using Domain.Entities;

namespace Domain.Abstractions;

public interface IJwtTokenGenerator
{
    (string accessToken, DateTime expiresAt) CreateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
}
