using Application.Common.Models;
using Application.DTOs.CommandManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.CommandManagement.Queries;

public sealed record GetPendingOutboxMessagesQuery(int Take = 50)
    : IRequest<Result<IReadOnlyList<OutboxMessageDto>>>;

public sealed class GetPendingOutboxMessagesQueryHandler : IRequestHandler<GetPendingOutboxMessagesQuery, Result<IReadOnlyList<OutboxMessageDto>>>
{
    private readonly IOutboxMessageRepository _outbox;

    public GetPendingOutboxMessagesQueryHandler(IOutboxMessageRepository outbox)
    {
        _outbox = outbox;
    }

    public async Task<Result<IReadOnlyList<OutboxMessageDto>>> Handle(GetPendingOutboxMessagesQuery request, CancellationToken ct)
    {
        var take = request.Take <= 0 ? 50 : request.Take;
        var items = await _outbox.GetPendingAsync(take, ct);
        return Result<IReadOnlyList<OutboxMessageDto>>.Success(items.Select(x => x.ToDto()).ToList());
    }
}