using Application.Common.Models;
using Application.DTOs.CommandManagement;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.CommandManagement.Commands;

public sealed record CreateSetProximityConfigCommand(
    SetProximityConfigCommandPayloadDto Payload,
    Guid RequestedByUserId
) : IRequest<Result<Guid>>;

public sealed class CreateSetProximityConfigCommandHandler : IRequestHandler<CreateSetProximityConfigCommand, Result<Guid>>
{
    private readonly ITagRepository _tags;
    private readonly ICommandRequestRepository _commands;
    private readonly ICommandStatusHistoryRepository _history;
    private readonly IUnitOfWork _uow;

    public CreateSetProximityConfigCommandHandler(
        ITagRepository tags,
        ICommandRequestRepository commands,
        ICommandStatusHistoryRepository history,
        IUnitOfWork uow)
    {
        _tags = tags;
        _commands = commands;
        _history = history;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(CreateSetProximityConfigCommand request, CancellationToken ct)
    {
        if (request.Payload.WarningThreshold < 0 || request.Payload.CriticalThreshold < 0)
            return Result<Guid>.Failure("Threshold values cannot be negative.");

        if (request.Payload.CriticalThreshold >= request.Payload.WarningThreshold)
            return Result<Guid>.Failure("CriticalThreshold must be smaller than WarningThreshold.");

        var tagResult = await CommandManagementHelper.GetTagByExternalIdAsync(_tags, request.Payload.TagId, ct);
        if (!tagResult.IsSuccess || tagResult.Value is null)
            return Result<Guid>.Failure(tagResult.Error!);

        var command = CommandRequest.Create(
            RtlsCommandType.SetProximityConfig,
            RtlsCommandTargetType.Tag,
            request.RequestedByUserId,
            CommandManagementHelper.Serialize(request.Payload),
            tagResult.Value.Id,
            null);

        await _commands.AddAsync(command, ct);
        await CommandManagementHelper.AddInitialHistoryAsync(_history, command, request.RequestedByUserId, "Proximity config command created.", ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(command.Id);
    }
}