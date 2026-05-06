using Application.Common.Models;
using Application.DTOs.CommandManagement;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.CommandManagement.Commands;

public sealed record CreateWriteI2cDataCommand(
    WriteI2cDataCommandPayloadDto Payload,
    Guid RequestedByUserId
) : IRequest<Result<Guid>>;

public sealed class CreateWriteI2cDataCommandHandler : IRequestHandler<CreateWriteI2cDataCommand, Result<Guid>>
{
    private readonly ITagRepository _tags;
    private readonly ICommandRequestRepository _commands;
    private readonly ICommandStatusHistoryRepository _history;
    private readonly IUnitOfWork _uow;

    public CreateWriteI2cDataCommandHandler(
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

    public async Task<Result<Guid>> Handle(CreateWriteI2cDataCommand request, CancellationToken ct)
    {
        if (request.Payload.Address < 0 || request.Payload.Address > 127)
            return Result<Guid>.Failure("Address must be between 0 and 127.");

        if (request.Payload.Register < 0 || request.Payload.Register > 255)
            return Result<Guid>.Failure("Register must be between 0 and 255.");

        var tagResult = await CommandManagementHelper.GetTagByExternalIdAsync(_tags, request.Payload.TagId, ct);
        if (!tagResult.IsSuccess || tagResult.Value is null)
            return Result<Guid>.Failure(tagResult.Error!);

        var command = CommandRequest.Create(
            RtlsCommandType.WriteI2CData,
            RtlsCommandTargetType.Tag,
            request.RequestedByUserId,
            CommandManagementHelper.Serialize(request.Payload),
            tagResult.Value.Id,
            null);

        await _commands.AddAsync(command, ct);
        await CommandManagementHelper.AddInitialHistoryAsync(_history, command, request.RequestedByUserId, "Write I2C data command created.", ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(command.Id);
    }
}