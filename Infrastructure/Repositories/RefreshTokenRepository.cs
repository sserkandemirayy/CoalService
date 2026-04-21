using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;
    public RefreshTokenRepository(AppDbContext db) => _db = db;

    //public async Task SaveAsync(Guid userId, string refreshTokenPlain, DateTime now, CancellationToken ct = default)
    //{
    //    var token = RefreshToken.Create(userId, refreshTokenPlain, now);
    //    await _db.RefreshTokens.AddAsync(token, ct);
    //}

    public async Task SaveAsync(Guid userId, string refreshTokenPlain, DateTime now,
                            string? device = null, string? ip = null, string? ua = null,
                            CancellationToken ct = default)
    {
        var token = RefreshToken.Create(userId, refreshTokenPlain, now, device, ip, ua);
        await _db.RefreshTokens.AddAsync(token, ct);
    }


    public async Task<RefreshToken?> GetActiveAsync(string refreshTokenPlain, CancellationToken ct = default)
    {
        var tokens = await _db.RefreshTokens
            .Where(t => t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

        return tokens.FirstOrDefault(t => t.Matches(refreshTokenPlain));
    }

    public async Task<RefreshToken?> GetAsync(string refreshTokenPlain, CancellationToken ct = default)
    {
        var tokens = await _db.RefreshTokens
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

        return tokens.FirstOrDefault(t => t.Matches(refreshTokenPlain));
    }

    public async Task RotateAsync(RefreshToken existing, string newRefreshPlain, CancellationToken ct = default)
    {
        existing.Revoke(DateTime.UtcNow);
        _db.RefreshTokens.Update(existing);

        var newToken = RefreshToken.Create(existing.UserId, newRefreshPlain, DateTime.UtcNow);
        await _db.RefreshTokens.AddAsync(newToken, ct);
    }

    public async Task<RefreshToken?> RevokeAsync(string refreshTokenPlain, CancellationToken ct = default)
    {
        var token = await GetActiveAsync(refreshTokenPlain, ct);
        if (token is null) return null;

        token.Revoke(DateTime.UtcNow);
        _db.RefreshTokens.Update(token);
        return token;
    }

    // Yeni: UserId + SessionId ile Revoke (örneðin session yönetiminde)
    public async Task RevokeAsync(Guid userId, Guid sessionId, CancellationToken ct = default)
    {
        var token = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == sessionId && t.RevokedAt == null, ct);

        if (token != null)
        {
            token.Revoke(DateTime.UtcNow);
            _db.RefreshTokens.Update(token);
        }
    }

    public async Task RevokeAllAsync(Guid userId, CancellationToken ct = default)
    {
        var tokens = await _db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var t in tokens)
            t.Revoke(DateTime.UtcNow);

        _db.RefreshTokens.UpdateRange(tokens);
    }

    public async Task<List<RefreshToken>> GetActiveSessionsAsync(Guid userId, CancellationToken ct = default)
    {
        return await _db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }
}
