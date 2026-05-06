using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.CommandManagement.Commands;

public sealed record CancelCommandRequestCommand(
    Guid CommandRequestId,
    Guid ChangedByUserId,
    string? Reason
) : IRequest<Result<Guid>>;

public sealed class CancelCommandRequestCommandHandler : IRequestHandler<CancelCommandRequestCommand, Result<Guid>>
{
    private readonly ICommandRequestRepository _commands;
    private readonly ICommandStatusHistoryRepository _history;
    private readonly IUnitOfWork _uow;

    public CancelCommandRequestCommandHandler(
        ICommandRequestRepository commands,
        ICommandStatusHistoryRepository history,
        IUnitOfWork uow)
    {
        _commands = commands;
        _history = history;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(CancelCommandRequestCommand request, CancellationToken ct)
    {
        var command = await _commands.GetByIdAsync(request.CommandRequestId, ct);
        if (command is null)
            return Result<Guid>.Failure("Command request not found.");

        var oldStatus = command.Status;
        command.Cancel(request.Reason);

        await _commands.UpdateAsync(command, ct);

        var history = CommandStatusHistory.Create(
            command.Id,
            oldStatus,
            command.Status,
            request.ChangedByUserId,
            "Command request cancelled.",
            command.PayloadJson);

        await _history.AddAsync(history, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(command.Id);
    }
}