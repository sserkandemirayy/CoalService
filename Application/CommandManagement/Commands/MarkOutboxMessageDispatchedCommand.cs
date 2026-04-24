using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.CommandManagement.Commands;

public sealed record MarkOutboxMessageDispatchedCommand(Guid OutboxMessageId) : IRequest<Result<Guid>>;

public sealed class MarkOutboxMessageDispatchedCommandHandler : IRequestHandler<MarkOutboxMessageDispatchedCommand, Result<Guid>>
{
    private readonly IOutboxMessageRepository _outbox;
    private readonly IUnitOfWork _uow;

    public MarkOutboxMessageDispatchedCommandHandler(
        IOutboxMessageRepository outbox,
        IUnitOfWork uow)
    {
        _outbox = outbox;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(MarkOutboxMessageDispatchedCommand request, CancellationToken ct)
    {
        var message = await _outbox.GetByIdAsync(request.OutboxMessageId, ct);
        if (message is null)
            return Result<Guid>.Failure("Outbox message not found.");

        message.MarkDispatched();
        await _outbox.UpdateAsync(message, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(message.Id);
    }
}