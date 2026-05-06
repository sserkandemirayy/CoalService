using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class RawEvent : BaseEntity
{
    protected RawEvent() { }

    public Guid ExternalEventId { get; private set; }
    public string EventType { get; private set; } = default!;
    public DateTime EventTimestamp { get; private set; }

    public string? TagExternalId { get; private set; }
    public string? AnchorExternalId { get; private set; }

    public string PayloadJson { get; private set; } = default!;
    public DateTime ReceivedAt { get; private set; }
    public RawEventProcessingStatus ProcessingStatus { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static RawEvent Create(
        Guid externalEventId,
        string eventType,
        DateTime eventTimestamp,
        string payloadJson,
        string? tagExternalId = null,
        string? anchorExternalId = null,
        DateTime? receivedAt = null)
    {
        if (externalEventId == Guid.Empty)
            throw new ArgumentException("ExternalEventId is required.", nameof(externalEventId));
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("EventType is required.", nameof(eventType));
        if (string.IsNullOrWhiteSpace(payloadJson))
            throw new ArgumentException("PayloadJson is required.", nameof(payloadJson));

        return new RawEvent
        {
            ExternalEventId = externalEventId,
            EventType = eventType.Trim(),
            EventTimestamp = eventTimestamp,
            PayloadJson = payloadJson,
            TagExternalId = tagExternalId?.Trim(),
            AnchorExternalId = anchorExternalId?.Trim(),
            ReceivedAt = receivedAt ?? DateTime.UtcNow,
            ProcessingStatus = RawEventProcessingStatus.Pending
        };
    }

    public void MarkProcessed()
    {
        ProcessingStatus = RawEventProcessingStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    public void MarkFailed(string errorMessage)
    {
        ProcessingStatus = RawEventProcessingStatus.Failed;
        ProcessedAt = DateTime.UtcNow;
        ErrorMessage = errorMessage;
    }

    public void MarkIgnored(string? reason = null)
    {
        ProcessingStatus = RawEventProcessingStatus.Ignored;
        ProcessedAt = DateTime.UtcNow;
        ErrorMessage = reason;
    }
}