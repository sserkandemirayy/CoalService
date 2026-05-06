using Domain.Entities;

namespace Application.DTOs.CommandManagement;

public static class CommandManagementMappings
{
    public static CommandRequestDto ToDto(this CommandRequest e) => new(
        e.Id,
        e.CommandType.ToString(),
        e.Status.ToString(),
        e.TargetType.ToString(),
        e.TagId,
        e.AnchorId,
        e.RequestedByUserId,
        e.PayloadJson,
        e.RequestedAt,
        e.QueuedAt,
        e.SentAt,
        e.CompletedAt,
        e.FailedAt,
        e.CancelledAt,
        e.ExternalCorrelationId,
        e.CancelReason,
        e.FailureReason,
        e.ResponseJson,
        e.RetryCount,
        e.LastRetriedAt);

    public static CommandStatusHistoryDto ToDto(this CommandStatusHistory e) => new(
        e.Id,
        e.CommandRequestId,
        e.OldStatus?.ToString(),
        e.NewStatus.ToString(),
        e.ChangedByUserId,
        e.ChangedAt,
        e.Note,
        e.DataJson);

    public static OutboxMessageDto ToDto(this OutboxMessage e) => new(
        e.Id,
        e.AggregateType,
        e.AggregateId,
        e.MessageType,
        e.PayloadJson,
        e.Status.ToString(),
        e.OccurredAt,
        e.DispatchedAt,
        e.FailedAt,
        e.FailureReason);
}