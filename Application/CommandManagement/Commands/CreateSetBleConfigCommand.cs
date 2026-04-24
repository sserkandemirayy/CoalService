using Application.Common.Models;
using Application.DTOs.CommandManagement;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.CommandManagement.Commands;

public sealed record CreateSetBleConfigCommand(
    SetBleConfigCommandPayloadDto Payload,
    Guid RequestedByUserId
) : IRequest<Result<Guid>>;

public sealed class CreateSetBleConfigCommandHandler : IRequestHandler<CreateSetBleConfigCommand, Result<Guid>>
{
    private readonly ITagRepository _tags;
    private readonly ICommandRequestRepository _commands;
    private readonly ICommandStatusHistoryRepository _history;
    private readonly IUnitOfWork _uow;

    public CreateSetBleConfigCommandHandler(
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

    public async Task<Result<Guid>> Handle(CreateSetBleConfigCommand request, CancellationToken ct)
    {
        if (request.Payload.AdvertisementInterval < 0)
            return Result<Guid>.Failure("AdvertisementInterval cannot be negative.");

        var tagResult = await CommandManagementHelper.GetTagByExternalIdAsync(_tags, request.Payload.TagId, ct);
        if (!tagResult.IsSuccess || tagResult.Value is null)
            return Result<Guid>.Failure(tagResult.Error!);

        var command = CommandRequest.Create(
            RtlsCommandType.SetBLEConfig,
            RtlsCommandTargetType.Tag,
            request.RequestedByUserId,
            CommandManagementHelper.Serialize(request.Payload),
            tagResult.Value.Id,
            null);

        await _commands.AddAsync(command, ct);
        await CommandManagementHelper.AddInitialHistoryAsync(_history, command, request.RequestedByUserId, "BLE config command created.", ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(command.Id);
    }
}