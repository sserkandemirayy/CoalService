using Application.Common.Models;
using Application.DTOs.CommandManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.CommandManagement.Queries;

public sealed record GetCommandRequestsByTagIdQuery(
    Guid TagId,
    int Page = 1,
    int PageSize = 50
) : IRequest<Result<IReadOnlyList<CommandRequestDto>>>;

public sealed class GetCommandRequestsByTagIdQueryHandler : IRequestHandler<GetCommandRequestsByTagIdQuery, Result<IReadOnlyList<CommandRequestDto>>>
{
    private readonly ICommandRequestRepository _commands;

    public GetCommandRequestsByTagIdQueryHandler(ICommandRequestRepository commands)
    {
        _commands = commands;
    }

    public async Task<Result<IReadOnlyList<CommandRequestDto>>> Handle(GetCommandRequestsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _commands.GetPagedByTagIdAsync(request.TagId, request.Page, request.PageSize, ct);
        return Result<IReadOnlyList<CommandRequestDto>>.Success(items.Select(x => x.ToDto()).ToList());
    }
}