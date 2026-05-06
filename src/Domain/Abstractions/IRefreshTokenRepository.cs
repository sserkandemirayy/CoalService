using Domain.Entities;

namespace Domain.Abstractions;

public interface IRefreshTokenRepository
{

    Task SaveAsync(Guid userId, string refreshTokenPlain, DateTime now,
                           string? device = null, string? ip = null, string? ua = null,
                           CancellationToken ct = default);
    Task<RefreshToken?> GetActiveAsync(string refreshTokenPlain, CancellationToken ct = default);
    Task<RefreshToken?> GetAsync(string refreshTokenPlain, CancellationToken ct = default);
    Task RotateAsync(RefreshToken existing, string newRefreshPlain, CancellationToken ct = default);
    Task<RefreshToken?> RevokeAsync(string refreshTokenPlain, CancellationToken ct = default);

    Task RevokeAsync(Guid userId, Guid sessionId, CancellationToken ct = default);

    Task RevokeAllAsync(Guid userId, CancellationToken ct = default);
    Task<List<RefreshToken>> GetActiveSessionsAsync(Guid userId, CancellationToken ct = default);
}
