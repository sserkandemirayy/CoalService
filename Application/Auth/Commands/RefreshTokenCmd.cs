using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Auth.Commands;

/// <summary>
/// Yeni access token üretir, eski refresh token’ý revoke eder ve yenisini döner.
/// </summary>
public record RefreshTokenCommand(
    string RefreshToken,
    string? IpAddress = null,
    string? UserAgent = null,
    string? Device = null
) : IRequest<Result<AuthResponse>>;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IJwtTokenGenerator _jwt;
    private readonly IDateTimeProvider _clock;
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit;

    public RefreshTokenHandler(
        IJwtTokenGenerator jwt,
        IDateTimeProvider clock,
        IUserRepository users,
        IRefreshTokenRepository refreshRepo,
        IUnitOfWork uow,
        IAuditLogRepository audit)
    {
        _jwt = jwt;
        _clock = clock;
        _users = users;
        _refreshRepo = refreshRepo;
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand req, CancellationToken ct)
    {
        //  Refresh token doðrula
        var oldToken = await _refreshRepo.GetActiveAsync(req.RefreshToken, ct);
        if (oldToken is null)
            return Result<AuthResponse>.Failure("Invalid or expired refresh token");

        //  Kullanýcý doðrula
        var user = await _users.GetByIdAsync(oldToken.UserId, ct);
        if (user is null || !user.IsActive)
            return Result<AuthResponse>.Failure("Invalid user");

        //  Yeni Access + Refresh token oluþtur
        var roles = await _users.GetRoleNamesAsync(user.Id, ct);
        var (access, exp) = _jwt.CreateAccessToken(user, roles);
        var newRefresh = _jwt.GenerateRefreshToken();

        //  Refresh token rotasyonu (device/ip/ua bilgisiyle)
        oldToken.Revoke(DateTime.UtcNow);

        var device = req.Device ?? (req.UserAgent?.Contains("iPhone") == true ? "iPhone" :
                            req.UserAgent?.Contains("Android") == true ? "Android" :
                            req.UserAgent?.Contains("Windows") == true ? "Windows PC" : "Unknown");


        await _refreshRepo.SaveAsync(
            user.Id,
            newRefresh,
            _clock.UtcNow,
            device,
            req.IpAddress,
            req.UserAgent,
            ct
        );

        //  Audit log
        await _audit.AddAsync(AuditLog.Create(
            user.Id,
            "refresh_token_rotate",
            "Auth",
            user.Id,
            req.IpAddress,
            $"Refresh token rotated (UA: {req.UserAgent ?? "unknown"}, Device: {req.Device ?? "N/A"})"
        ), ct);

        await _uow.SaveChangesAsync(ct);

        // Yeni access + refresh döndür
        return Result<AuthResponse>.Success(new AuthResponse(access, exp, newRefresh));
    }
}
