namespace Application.DTOs.CommandManagement;

// Common
public sealed record PositionDto(decimal X, decimal Y, decimal Z);
public sealed record NetworkConfigDto(string IpAddress, string Netmask, string Gateway, string Dns, bool DhcpEnabled);
public sealed record AnchorUwbRadioDto(bool Enabled, int Channel, decimal TxPower);
public sealed record AnchorBleRadioDto(bool Enabled, decimal TxPower);

public sealed record LowPassFilterDto(bool Enabled, long TimeConstant);

public sealed record I2cDeviceConfigDto(
    int Address,
    string? Name,
    string? Description);

public sealed record BuzzerAlertDto(bool Enabled);
public sealed record LedAlertDto(bool Enabled, string Color);
public sealed record VibrationAlertDto(bool Enabled);

// Request payloads
public sealed record RequestConfigCommandPayloadDto(string? AnchorId, string? TagId);
public sealed record ResetDeviceCommandPayloadDto(string? AnchorId, string? TagId);

public sealed record SetAnchorConfigCommandPayloadDto(
    string AnchorId,
    PositionDto Position,
    NetworkConfigDto Network,
    AnchorUwbRadioDto Uwb,
    AnchorBleRadioDto Ble,
    long HeartbeatInterval,
    long ReportInterval
);

public sealed record SetBleConfigCommandPayloadDto(
    string TagId,
    bool Enabled,
    decimal TxPower,
    long AdvertisementInterval,
    bool MeshEnabled
);

public sealed record SetDioConfigCommandPayloadDto(
    string TagId,
    int Pin,
    string Direction,
    bool PeriodicReportEnabled,
    long PeriodicReportInterval,
    bool ReportOnChange,
    LowPassFilterDto LowPassFilter
);

public sealed record SetDioValueCommandPayloadDto(
    string TagId,
    int Pin,
    bool Value
);

public sealed record SetI2cConfigCommandPayloadDto(
    string TagId,
    bool Enabled,
    int ClockSpeed,
    List<I2cDeviceConfigDto> Devices
);

public sealed record SetProximityConfigCommandPayloadDto(
    string TagId,
    bool Enabled,
    decimal WarningThreshold,
    decimal CriticalThreshold
);

public sealed record SetTagAlertCommandPayloadDto(
    string TagId,
    BuzzerAlertDto Buzzer,
    LedAlertDto Led,
    VibrationAlertDto Vibration
);

public sealed record SetUwbConfigCommandPayloadDto(
    string TagId,
    bool Enabled,
    int Channel,
    decimal TxPower,
    long RangingInterval,
    bool TagToTagEnabled,
    long TagToTagInterval
);

public sealed record WriteI2cDataCommandPayloadDto(
    string TagId,
    int Address,
    int Register,
    List<int> Data
);