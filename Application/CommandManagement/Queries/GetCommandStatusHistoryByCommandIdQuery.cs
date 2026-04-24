using Application.Common.Models;
using Application.DTOs.CommandManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.CommandManagement.Queries;

public sealed record GetCommandStatusHistoryByCommandIdQuery(Guid CommandRequestId)
    : IRequest<Result<IReadOnlyList<CommandStatusHistoryDto>>>;

public sealed class GetCommandStatusHistoryByCommandIdQueryHandler : IRequestHandler<GetCommandStatusHistoryByCommandIdQuery, Result<IReadOnlyList<CommandStatusHistoryDto>>>
{
    private readonly ICommandStatusHistoryRepository _history;

    public GetCommandStatusHistoryByCommandIdQueryHandler(ICommandStatusHistoryRepository history)
    {
        _history = history;
    }

    public async Task<Result<IReadOnlyList<CommandStatusHistoryDto>>> Handle(GetCommandStatusHistoryByCommandIdQuery request, CancellationToken ct)
    {
        var items = await _history.GetByCommandRequestIdAsync(request.CommandRequestId, ct);
        return Result<IReadOnlyList<CommandStatusHistoryDto>>.Success(items.Select(x => x.ToDto()).ToList());
    }
}