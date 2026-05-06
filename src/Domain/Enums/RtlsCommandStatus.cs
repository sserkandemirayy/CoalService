namespace Domain.Enums;

public enum RtlsCommandStatus
{
    Pending = 1,
    Queued = 2,
    Sent = 3,
    Succeeded = 4,
    Failed = 5,
    Cancelled = 6
}