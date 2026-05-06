namespace Domain.Enums;

public enum AlarmType
{
    EmergencyButtonPressed = 1,
    ProximityAlert = 2,
    FallDetected = 3,
    InactivityDetected = 4,
    ShockDetected = 5,
    AbnormalPositionDetected = 6,
    LowBattery = 7,
    AnchorOffline = 8,
    AnchorError = 9
}