using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.CommandManagement.Commands;

public sealed record MarkCommandFailedByIntegrationCommand(
    Guid CommandRequestId,
    string? ExternalCorrelationId,
    string? FailureReason,
    string? ResponseJson,
    string? Note
) : IRequest<Result<Guid>>;

public sealed class MarkCommandFailedByIntegrationCommandHandler : IRequestHandler<MarkCommandFailedByIntegrationCommand, Result<Guid>>
{
    private readonly ICommandRequestRepository _commands;
    private readonly ICommandStatusHistoryRepository _history;
    private readonly IUnitOfWork _uow;

    public MarkCommandFailedByIntegrationCommandHandler(
        ICommandRequestRepository commands,
        ICommandStatusHistoryRepository history,
        IUnitOfWork uow)
    {
        _commands = commands;
        _history = history;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(MarkCommandFailedByIntegrationCommand request, CancellationToken ct)
    {
        var command = await _commands.GetByIdAsync(request.CommandRequestId, ct);
        if (command is null)
            return Result<Guid>.Failure("Command request not found.");

        var oldStatus = command.Status;
        command.MarkFailed(request.FailureReason, request.ResponseJson, request.ExternalCorrelationId);

        await _commands.UpdateAsync(command, ct);

        var history = CommandStatusHistory.Create(
            command.Id,
            oldStatus,
            command.Status,
            null,
            string.IsNullOrWhiteSpace(request.Note) ? "Integration marked command as failed." : request.Note,
            request.ResponseJson ?? request.FailureReason);

        await _history.AddAsync(history, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(command.Id);
    }
}