using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;
    private readonly IUserRepository _users;

    public JwtTokenGenerator(IOptions<JwtSettings> options, IUserRepository users)
    {
        _settings = options.Value;
        _users = users;
    }

    public (string accessToken, DateTime expiresAt) CreateAccessToken(User user, IEnumerable<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_settings.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        // Roller
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // Permissionlar
        var permissions = _users.GetPermissionsAsync(user.Id).Result; // async -> sync
        claims.AddRange(permissions.Select(p => new Claim("permission", p)));

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return (jwt, expires);
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64); // güçlü RNG
        return Convert.ToBase64String(bytes);
    }
}
