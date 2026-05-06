using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class OutboxMessage : BaseEntity
{
    protected OutboxMessage() { }

    public string AggregateType { get; private set; } = default!;
    public Guid AggregateId { get; private set; }
    public string MessageType { get; private set; } = default!;
    public string PayloadJson { get; private set; } = default!;

    public OutboxMessageStatus Status { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public DateTime? DispatchedAt { get; private set; }
    public DateTime? FailedAt { get; private set; }
    public string? FailureReason { get; private set; }

    public static OutboxMessage Create(
        string aggregateType,
        Guid aggregateId,
        string messageType,
        string payloadJson,
        DateTime? occurredAt = null)
    {
        if (string.IsNullOrWhiteSpace(aggregateType))
            throw new ArgumentException("AggregateType is required.", nameof(aggregateType));
        if (aggregateId == Guid.Empty)
            throw new ArgumentException("AggregateId is required.", nameof(aggregateId));
        if (string.IsNullOrWhiteSpace(messageType))
            throw new ArgumentException("MessageType is required.", nameof(messageType));
        if (string.IsNullOrWhiteSpace(payloadJson))
            throw new ArgumentException("PayloadJson is required.", nameof(payloadJson));

        return new OutboxMessage
        {
            AggregateType = aggregateType.Trim(),
            AggregateId = aggregateId,
            MessageType = messageType.Trim(),
            PayloadJson = payloadJson,
            Status = OutboxMessageStatus.Pending,
            OccurredAt = occurredAt ?? DateTime.UtcNow
        };
    }

    public void MarkDispatched()
    {
        Status = OutboxMessageStatus.Dispatched;
        DispatchedAt = DateTime.UtcNow;
        FailureReason = null;
        FailedAt = null;
    }

    public void MarkFailed(string? failureReason)
    {
        Status = OutboxMessageStatus.Failed;
        FailedAt = DateTime.UtcNow;
        FailureReason = string.IsNullOrWhiteSpace(failureReason) ? null : failureReason.Trim();
    }
}