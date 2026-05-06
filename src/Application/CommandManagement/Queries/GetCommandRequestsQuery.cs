using Application.Common.Models;
using Application.DTOs.CommandManagement;
using Domain.Abstractions;
using Domain.Enums;
using MediatR;

namespace Application.CommandManagement.Queries;

public sealed record GetCommandRequestsQuery(
    string? CommandType,
    string? Status,
    string? TargetType,
    Guid? TagId,
    Guid? AnchorId,
    Guid? RequestedByUserId,
    int Page = 1,
    int PageSize = 50
) : IRequest<Result<IReadOnlyList<CommandRequestDto>>>;

public sealed class GetCommandRequestsQueryHandler : IRequestHandler<GetCommandRequestsQuery, Result<IReadOnlyList<CommandRequestDto>>>
{
    private readonly ICommandRequestRepository _commands;

    public GetCommandRequestsQueryHandler(ICommandRequestRepository commands)
    {
        _commands = commands;
    }

    public async Task<Result<IReadOnlyList<CommandRequestDto>>> Handle(GetCommandRequestsQuery request, CancellationToken ct)
    {
        RtlsCommandType? commandType = null;
        RtlsCommandStatus? status = null;
        RtlsCommandTargetType? targetType = null;

        if (!string.IsNullOrWhiteSpace(request.CommandType))
        {
            if (!Enum.TryParse<RtlsCommandType>(request.CommandType, true, out var parsed))
                return Result<IReadOnlyList<CommandRequestDto>>.Failure("Invalid command type.");
            commandType = parsed;
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<RtlsCommandStatus>(request.Status, true, out var parsed))
                return Result<IReadOnlyList<CommandRequestDto>>.Failure("Invalid command status.");
            status = parsed;
        }

        if (!string.IsNullOrWhiteSpace(request.TargetType))
        {
            if (!Enum.TryParse<RtlsCommandTargetType>(request.TargetType, true, out var parsed))
                return Result<IReadOnlyList<CommandRequestDto>>.Failure("Invalid target type.");
            targetType = parsed;
        }

        var (items, _) = await _commands.GetPagedAsync(
            commandType,
            status,
            targetType,
            request.TagId,
            request.AnchorId,
            request.RequestedByUserId,
            request.Page,
            request.PageSize,
            ct);

        return Result<IReadOnlyList<CommandRequestDto>>.Success(items.Select(x => x.ToDto()).ToList());
    }
}