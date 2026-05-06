using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.CommandManagement.Commands;

public sealed record MarkCommandSentByIntegrationCommand(
    Guid CommandRequestId,
    string? ExternalCorrelationId,
    string? Note
) : IRequest<Result<Guid>>;

public sealed class MarkCommandSentByIntegrationCommandHandler : IRequestHandler<MarkCommandSentByIntegrationCommand, Result<Guid>>
{
    private readonly ICommandRequestRepository _commands;
    private readonly ICommandStatusHistoryRepository _history;
    private readonly IUnitOfWork _uow;

    public MarkCommandSentByIntegrationCommandHandler(
        ICommandRequestRepository commands,
        ICommandStatusHistoryRepository history,
        IUnitOfWork uow)
    {
        _commands = commands;
        _history = history;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(MarkCommandSentByIntegrationCommand request, CancellationToken ct)
    {
        var command = await _commands.GetByIdAsync(request.CommandRequestId, ct);
        if (command is null)
            return Result<Guid>.Failure("Command request not found.");

        var oldStatus = command.Status;
        command.MarkSent(request.ExternalCorrelationId);

        await _commands.UpdateAsync(command, ct);

        var history = CommandStatusHistory.Create(
            command.Id,
            oldStatus,
            command.Status,
            null,
            string.IsNullOrWhiteSpace(request.Note) ? "Integration marked command as sent." : request.Note,
            command.PayloadJson);

        await _history.AddAsync(history, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(command.Id);
    }
}