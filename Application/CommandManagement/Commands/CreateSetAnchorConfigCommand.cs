using Application.Common.Models;
using Application.DTOs.CommandManagement;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.CommandManagement.Commands;

public sealed record CreateSetAnchorConfigCommand(
    SetAnchorConfigCommandPayloadDto Payload,
    Guid RequestedByUserId
) : IRequest<Result<Guid>>;

public sealed class CreateSetAnchorConfigCommandHandler : IRequestHandler<CreateSetAnchorConfigCommand, Result<Guid>>
{
    private readonly IAnchorRepository _anchors;
    private readonly ICommandRequestRepository _commands;
    private readonly ICommandStatusHistoryRepository _history;
    private readonly IUnitOfWork _uow;

    public CreateSetAnchorConfigCommandHandler(
        IAnchorRepository anchors,
        ICommandRequestRepository commands,
        ICommandStatusHistoryRepository history,
        IUnitOfWork uow)
    {
        _anchors = anchors;
        _commands = commands;
        _history = history;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(CreateSetAnchorConfigCommand request, CancellationToken ct)
    {
        if (request.Payload.HeartbeatInterval < 0 || request.Payload.ReportInterval < 0)
            return Result<Guid>.Failure("Intervals cannot be negative.");

        var anchorResult = await CommandManagementHelper.GetAnchorByExternalIdAsync(_anchors, request.Payload.AnchorId, ct);
        if (!anchorResult.IsSuccess || anchorResult.Value is null)
            return Result<Guid>.Failure(anchorResult.Error!);

        var command = CommandRequest.Create(
            RtlsCommandType.SetAnchorConfig,
            RtlsCommandTargetType.Anchor,
            request.RequestedByUserId,
            CommandManagementHelper.Serialize(request.Payload),
            null,
            anchorResult.Value.Id);

        await _commands.AddAsync(command, ct);
        await CommandManagementHelper.AddInitialHistoryAsync(_history, command, request.RequestedByUserId, "Anchor config command created.", ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(command.Id);
    }
}