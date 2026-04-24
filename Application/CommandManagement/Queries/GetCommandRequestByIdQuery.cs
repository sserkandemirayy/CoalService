using Application.Common.Models;
using Application.DTOs.CommandManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.CommandManagement.Queries;

public sealed record GetCommandRequestByIdQuery(Guid Id) : IRequest<Result<CommandRequestDto>>;

public sealed class GetCommandRequestByIdQueryHandler : IRequestHandler<GetCommandRequestByIdQuery, Result<CommandRequestDto>>
{
    private readonly ICommandRequestRepository _commands;

    public GetCommandRequestByIdQueryHandler(ICommandRequestRepository commands)
    {
        _commands = commands;
    }

    public async Task<Result<CommandRequestDto>> Handle(GetCommandRequestByIdQuery request, CancellationToken ct)
    {
        var item = await _commands.GetByIdAsync(request.Id, ct);
        if (item is null)
            return Result<CommandRequestDto>.Failure("Command request not found.");

        return Result<CommandRequestDto>.Success(item.ToDto());
    }
}