using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class CommandRequest : BaseEntity
{
    protected CommandRequest() { }

    public RtlsCommandType CommandType { get; private set; }
    public RtlsCommandStatus Status { get; private set; }
    public RtlsCommandTargetType TargetType { get; private set; }

    public Guid? TagId { get; private set; }
    public Tag? Tag { get; private set; }

    public Guid? AnchorId { get; private set; }
    public Anchor? Anchor { get; private set; }

    public Guid RequestedByUserId { get; private set; }
    public User RequestedByUser { get; private set; } = default!;

    public string PayloadJson { get; private set; } = default!;

    public DateTime RequestedAt { get; private set; }
    public DateTime? QueuedAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? FailedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    public string? ExternalCorrelationId { get; private set; }
    public string? CancelReason { get; private set; }
    public string? FailureReason { get; private set; }
    public string? ResponseJson { get; private set; }

    public int RetryCount { get; private set; }
    public DateTime? LastRetriedAt { get; private set; }

    public ICollection<CommandStatusHistory> StatusHistory { get; private set; } = new List<CommandStatusHistory>();

    public static CommandRequest Create(
        RtlsCommandType commandType,
        RtlsCommandTargetType targetType,
        Guid requestedByUserId,
        string payloadJson,
        Guid? tagId = null,
        Guid? anchorId = null,
        DateTime? requestedAt = null)
    {
        if (requestedByUserId == Guid.Empty)
            throw new ArgumentException("RequestedByUserId is required.", nameof(requestedByUserId));

        if (string.IsNullOrWhiteSpace(payloadJson))
            throw new ArgumentException("PayloadJson is required.", nameof(payloadJson));

        if (targetType == RtlsCommandTargetType.Tag && tagId == null)
            throw new ArgumentException("TagId is required for tag target.", nameof(tagId));

        if (targetType == RtlsCommandTargetType.Anchor && anchorId == null)
            throw new ArgumentException("AnchorId is required for anchor target.", nameof(anchorId));

        return new CommandRequest
        {
            CommandType = commandType,
            Status = RtlsCommandStatus.Pending,
            TargetType = targetType,
            TagId = tagId,
            AnchorId = anchorId,
            RequestedByUserId = requestedByUserId,
            PayloadJson = payloadJson,
            RequestedAt = requestedAt ?? DateTime.UtcNow
        };
    }

    public void MarkQueued()
    {
        if (Status != RtlsCommandStatus.Pending)
            throw new InvalidOperationException("Only pending commands can be queued.");

        Status = RtlsCommandStatus.Queued;
        QueuedAt = DateTime.UtcNow;
    }

    public void MarkSent(string? externalCorrelationId = null)
    {
        if (Status != RtlsCommandStatus.Queued && Status != RtlsCommandStatus.Pending)
            throw new InvalidOperationException("Only pending or queued commands can be marked sent.");

        Status = RtlsCommandStatus.Sent;
        SentAt = DateTime.UtcNow;
        ExternalCorrelationId = string.IsNullOrWhiteSpace(externalCorrelationId) ? null : externalCorrelationId.Trim();
        FailureReason = null;
        FailedAt = null;
    }

    public void MarkSucceeded(string? responseJson = null, string? externalCorrelationId = null)
    {
        if (Status == RtlsCommandStatus.Cancelled)
            throw new InvalidOperationException("Cancelled commands cannot succeed.");

        Status = RtlsCommandStatus.Succeeded;
        CompletedAt = DateTime.UtcNow;
        ResponseJson = string.IsNullOrWhiteSpace(responseJson) ? null : responseJson;
        if (!string.IsNullOrWhiteSpace(externalCorrelationId))
            ExternalCorrelationId = externalCorrelationId.Trim();

        FailureReason = null;
        FailedAt = null;
    }

    public void MarkFailed(string? failureReason = null, string? responseJson = null, string? externalCorrelationId = null)
    {
        if (Status == RtlsCommandStatus.Cancelled)
            throw new InvalidOperationException("Cancelled commands cannot fail.");

        Status = RtlsCommandStatus.Failed;
        FailedAt = DateTime.UtcNow;
        FailureReason = string.IsNullOrWhiteSpace(failureReason) ? null : failureReason.Trim();
        ResponseJson = string.IsNullOrWhiteSpace(responseJson) ? null : responseJson;

        if (!string.IsNullOrWhiteSpace(externalCorrelationId))
            ExternalCorrelationId = externalCorrelationId.Trim();
    }

    public void Cancel(string? reason = null)
    {
        if (Status == RtlsCommandStatus.Succeeded)
            throw new InvalidOperationException("Succeeded command cannot be cancelled.");

        if (Status == RtlsCommandStatus.Cancelled)
            return;

        Status = RtlsCommandStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancelReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
    }

    public void Retry()
    {
        if (Status == RtlsCommandStatus.Succeeded)
            throw new InvalidOperationException("Succeeded command cannot be retried.");

        Status = RtlsCommandStatus.Pending;
        RetryCount++;
        LastRetriedAt = DateTime.UtcNow;

        QueuedAt = null;
        SentAt = null;
        CompletedAt = null;
        FailedAt = null;
        CancelledAt = null;

        CancelReason = null;
        FailureReason = null;
        ResponseJson = null;
        ExternalCorrelationId = null;
    }
}