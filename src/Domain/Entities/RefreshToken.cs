using BCrypt.Net;
using Domain.Abstractions;

namespace Domain.Entities;

public class RefreshToken : BaseEntity
{
    private RefreshToken() { }

    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    // Ek Bilgiler (Oturum / Cihaz / Ýzleme)
    public string? Device { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime? LastUsedAt { get; private set; }

    // Navigation
    public User User { get; private set; } = default!;

    public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;

    private RefreshToken(Guid userId, string refreshTokenPlain, DateTime now,
                         string? device = null, string? ip = null, string? ua = null)
    {
        UserId = userId;
        TokenHash = BCrypt.Net.BCrypt.HashPassword(refreshTokenPlain);
        CreatedAt = now;
        ExpiresAt = now.AddDays(7); // refresh token ömrü
        Device = device;
        IpAddress = ip;
        UserAgent = ua;
    }

    public static RefreshToken Create(Guid userId, string refreshTokenPlain, DateTime now,
                                      string? device = null, string? ip = null, string? ua = null)
        => new(userId, refreshTokenPlain, now, device, ip, ua);

    public bool Matches(string refreshTokenPlain)
        => BCrypt.Net.BCrypt.Verify(refreshTokenPlain, TokenHash);

    public void Revoke(DateTime? when = null)
        => RevokedAt = when ?? DateTime.UtcNow;

    public void MarkUsed(DateTime when)
        => LastUsedAt = when;
}
