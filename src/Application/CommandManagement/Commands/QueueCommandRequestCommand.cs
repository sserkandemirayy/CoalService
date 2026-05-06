using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.CommandManagement.Commands;

public sealed record QueueCommandRequestCommand(Guid CommandRequestId, Guid ChangedByUserId) : IRequest<Result<Guid>>;

public sealed class QueueCommandRequestCommandHandler : IRequestHandler<QueueCommandRequestCommand, Result<Guid>>
{
    private readonly ICommandRequestRepository _commands;
    private readonly ICommandStatusHistoryRepository _history;
    private readonly IOutboxMessageRepository _outbox;
    private readonly IUnitOfWork _uow;

    public QueueCommandRequestCommandHandler(
        ICommandRequestRepository commands,
        ICommandStatusHistoryRepository history,
        IOutboxMessageRepository outbox,
        IUnitOfWork uow)
    {
        _commands = commands;
        _history = history;
        _outbox = outbox;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(QueueCommandRequestCommand request, CancellationToken ct)
    {
        var command = await _commands.GetByIdAsync(request.CommandRequestId, ct);
        if (command is null)
            return Result<Guid>.Failure("Command request not found.");

        var oldStatus = command.Status;
        command.MarkQueued();

        await _commands.UpdateAsync(command, ct);

        var history = CommandStatusHistory.Create(
            command.Id,
            oldStatus,
            command.Status,
            request.ChangedByUserId,
            "Command request queued.",
            command.PayloadJson);

        await _history.AddAsync(history, ct);

        var outbox = OutboxMessage.Create(
            "CommandRequest",
            command.Id,
            "RtlsCommandQueued",
            CommandManagementHelper.BuildOutboxPayload(command));

        await _outbox.AddAsync(outbox, ct);

        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(command.Id);
    }
}