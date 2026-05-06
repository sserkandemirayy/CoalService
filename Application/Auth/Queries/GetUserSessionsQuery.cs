using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

public record GetUserSessionsQuery(Guid UserId) : IRequest<IEnumerable<UserSessionDto>>;

public class GetUserSessionsHandler : IRequestHandler<GetUserSessionsQuery, IEnumerable<UserSessionDto>>
{
    private readonly IRefreshTokenRepository _tokens;
    public GetUserSessionsHandler(IRefreshTokenRepository tokens) => _tokens = tokens;

    public async Task<IEnumerable<UserSessionDto>> Handle(GetUserSessionsQuery q, CancellationToken ct)
    {
        var sessions = await _tokens.GetActiveSessionsAsync(q.UserId, ct);
        return sessions.Select(UserSessionDto.FromEntity);
    }
}
