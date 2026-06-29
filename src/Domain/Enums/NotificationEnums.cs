namespace Domain.Enums;

public enum NotificationSeverity
{
    Info = 1,
    Success = 2,
    Warning = 3,
    Critical = 4
}

public enum NotificationType
{
    System = 1,
    Alarm = 2,
    Emergency = 3,
    Battery = 4,
    Anchor = 5,
    Tracking = 6,
    Command = 7,
    Maintenance = 8,
    Announcement = 9,
    Custom = 10
}

public enum NotificationTargetType
{
    Users = 1,
    Roles = 2,
    Company = 3,
    Branch = 4,
    Broadcast = 5
}