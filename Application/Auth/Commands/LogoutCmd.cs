using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Auth.Commands;

public record LogoutCommand(string RefreshToken, Guid? PerformedByUserId = null, string? IpAddress = null)
    : IRequest<Result<Unit>>;

public class LogoutHandler : IRequestHandler<LogoutCommand, Result<Unit>>
{
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit;

    public LogoutHandler(IRefreshTokenRepository refreshRepo, IUnitOfWork uow, IAuditLogRepository audit)
    {
        _refreshRepo = refreshRepo;
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<Unit>> Handle(LogoutCommand req, CancellationToken ct)
    {
        var token = await _refreshRepo.GetAsync(req.RefreshToken, ct);
        if (token is null)
            return Result<Unit>.Failure("Token not found");

        token.Revoke(DateTime.UtcNow);

        await _audit.AddAsync(AuditLog.Create(
            req.PerformedByUserId,
            "logout",
            "Auth",
            token.UserId,
            req.IpAddress,
            "User logged out"
        ), ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
