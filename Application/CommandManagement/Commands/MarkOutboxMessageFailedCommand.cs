using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.CommandManagement.Commands;

public sealed record MarkOutboxMessageFailedCommand(Guid OutboxMessageId, string? FailureReason) : IRequest<Result<Guid>>;

public sealed class MarkOutboxMessageFailedCommandHandler : IRequestHandler<MarkOutboxMessageFailedCommand, Result<Guid>>
{
    private readonly IOutboxMessageRepository _outbox;
    private readonly IUnitOfWork _uow;

    public MarkOutboxMessageFailedCommandHandler(
        IOutboxMessageRepository outbox,
        IUnitOfWork uow)
    {
        _outbox = outbox;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(MarkOutboxMessageFailedCommand request, CancellationToken ct)
    {
        var message = await _outbox.GetByIdAsync(request.OutboxMessageId, ct);
        if (message is null)
            return Result<Guid>.Failure("Outbox message not found.");

        message.MarkFailed(request.FailureReason);
        await _outbox.UpdateAsync(message, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(message.Id);
    }
}