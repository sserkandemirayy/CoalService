namespace Domain.Enums;

public enum RtlsCommandType
{
    RequestConfig = 1,
    ResetDevice = 2,
    SetAnchorConfig = 3,
    SetBLEConfig = 4,
    SetDIOConfig = 5,
    SetDIOValue = 6,
    SetI2CConfig = 7,
    SetProximityConfig = 8,
    SetTagAlert = 9,
    SetUWBConfig = 10,
    WriteI2CData = 11
}