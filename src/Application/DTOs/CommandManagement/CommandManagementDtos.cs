namespace Application.DTOs.CommandManagement;

public sealed record CommandRequestDto(
    Guid Id,
    string CommandType,
    string Status,
    string TargetType,
    Guid? TagId,
    Guid? AnchorId,
    Guid RequestedByUserId,
    string PayloadJson,
    DateTime RequestedAt,
    DateTime? QueuedAt,
    DateTime? SentAt,
    DateTime? CompletedAt,
    DateTime? FailedAt,
    DateTime? CancelledAt,
    string? ExternalCorrelationId,
    string? CancelReason,
    string? FailureReason,
    string? ResponseJson,
    int RetryCount,
    DateTime? LastRetriedAt
);

public sealed record CommandStatusHistoryDto(
    Guid Id,
    Guid CommandRequestId,
    string? OldStatus,
    string NewStatus,
    Guid? ChangedByUserId,
    DateTime ChangedAt,
    string? Note,
    string? DataJson
);

public sealed record OutboxMessageDto(
    Guid Id,
    string AggregateType,
    Guid AggregateId,
    string MessageType,
    string PayloadJson,
    string Status,
    DateTime OccurredAt,
    DateTime? DispatchedAt,
    DateTime? FailedAt,
    string? FailureReason
);

public sealed record MarkCommandSentByIntegrationDto(
    string? ExternalCorrelationId,
    string? Note
);

public sealed record MarkCommandSucceededByIntegrationDto(
    string? ExternalCorrelationId,
    string? ResponseJson,
    string? Note
);

public sealed record MarkCommandFailedByIntegrationDto(
    string? ExternalCorrelationId,
    string? FailureReason,
    string? ResponseJson,
    string? Note
);

public sealed record MarkOutboxFailedDto(string? FailureReason);

public sealed record CommandOutboxEnvelopeDto(
    Guid CommandRequestId,
    string CommandType,
    string TargetType,
    Guid? TagId,
    Guid? AnchorId,
    string PayloadJson,
    DateTime RequestedAt
);