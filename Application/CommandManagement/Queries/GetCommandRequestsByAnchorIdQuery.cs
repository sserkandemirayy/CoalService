using Application.Common.Models;
using Application.DTOs.CommandManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.CommandManagement.Queries;

public sealed record GetCommandRequestsByAnchorIdQuery(
    Guid AnchorId,
    int Page = 1,
    int PageSize = 50
) : IRequest<Result<IReadOnlyList<CommandRequestDto>>>;

public sealed class GetCommandRequestsByAnchorIdQueryHandler : IRequestHandler<GetCommandRequestsByAnchorIdQuery, Result<IReadOnlyList<CommandRequestDto>>>
{
    private readonly ICommandRequestRepository _commands;

    public GetCommandRequestsByAnchorIdQueryHandler(ICommandRequestRepository commands)
    {
        _commands = commands;
    }

    public async Task<Result<IReadOnlyList<CommandRequestDto>>> Handle(GetCommandRequestsByAnchorIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _commands.GetPagedByAnchorIdAsync(request.AnchorId, request.Page, request.PageSize, ct);
        return Result<IReadOnlyList<CommandRequestDto>>.Success(items.Select(x => x.ToDto()).ToList());
    }
}