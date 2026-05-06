using Application.Common.Models;
using Application.DTOs.CommandManagement;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.CommandManagement.Commands;

public sealed record CreateSetDioConfigCommand(
    SetDioConfigCommandPayloadDto Payload,
    Guid RequestedByUserId
) : IRequest<Result<Guid>>;

public sealed class CreateSetDioConfigCommandHandler : IRequestHandler<CreateSetDioConfigCommand, Result<Guid>>
{
    private readonly ITagRepository _tags;
    private readonly ICommandRequestRepository _commands;
    private readonly ICommandStatusHistoryRepository _history;
    private readonly IUnitOfWork _uow;

    public CreateSetDioConfigCommandHandler(
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

    public async Task<Result<Guid>> Handle(CreateSetDioConfigCommand request, CancellationToken ct)
    {
        if (request.Payload.Pin < 0)
            return Result<Guid>.Failure("Pin cannot be negative.");

        if (request.Payload.PeriodicReportInterval < 0 || request.Payload.LowPassFilter.TimeConstant < 0)
            return Result<Guid>.Failure("Intervals cannot be negative.");

        if (!Enum.TryParse<DioPinDirection>(request.Payload.Direction, true, out _))
            return Result<Guid>.Failure("Invalid DIO direction.");

        var tagResult = await CommandManagementHelper.GetTagByExternalIdAsync(_tags, request.Payload.TagId, ct);
        if (!tagResult.IsSuccess || tagResult.Value is null)
            return Result<Guid>.Failure(tagResult.Error!);

        var command = CommandRequest.Create(
            RtlsCommandType.SetDIOConfig,
            RtlsCommandTargetType.Tag,
            request.RequestedByUserId,
            CommandManagementHelper.Serialize(request.Payload),
            tagResult.Value.Id,
            null);

        await _commands.AddAsync(command, ct);
        await CommandManagementHelper.AddInitialHistoryAsync(_history, command, request.RequestedByUserId, "DIO config command created.", ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(command.Id);
    }
}