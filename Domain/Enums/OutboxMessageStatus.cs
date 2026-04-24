namespace Domain.Enums;

public enum OutboxMessageStatus
{
    Pending = 1,
    Dispatched = 2,
    Failed = 3
}