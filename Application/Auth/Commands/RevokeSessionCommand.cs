using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

public record RevokeSessionCommand(Guid UserId, Guid SessionId) : IRequest<Result<Unit>>;
public record RevokeAllSessionsCommand(Guid UserId) : IRequest<Result<Unit>>;

public class RevokeSessionHandler : IRequestHandler<RevokeSessionCommand, Result<Unit>>
{
    private readonly IRefreshTokenRepository _tokens;
    private readonly IUnitOfWork _uow;

    public RevokeSessionHandler(IRefreshTokenRepository tokens, IUnitOfWork uow)
    { _tokens = tokens; _uow = uow; }

    public async Task<Result<Unit>> Handle(RevokeSessionCommand r, CancellationToken ct)
    {
        await _tokens.RevokeAsync(r.UserId, r.SessionId, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}

public class RevokeAllSessionsHandler : IRequestHandler<RevokeAllSessionsCommand, Result<Unit>>
{
    private readonly IRefreshTokenRepository _tokens;
    private readonly IUnitOfWork _uow;

    public RevokeAllSessionsHandler(IRefreshTokenRepository tokens, IUnitOfWork uow)
    { _tokens = tokens; _uow = uow; }

    public async Task<Result<Unit>> Handle(RevokeAllSessionsCommand r, CancellationToken ct)
    {
        await _tokens.RevokeAllAsync(r.UserId, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
