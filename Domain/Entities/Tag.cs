using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Tag : BaseEntity
{
    protected Tag() { }

    public string ExternalId { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public string? Name { get; private set; }
    public string? SerialNumber { get; private set; }
    public TagType TagType { get; private set; } = TagType.Personnel;
    public TagStatus Status { get; private set; } = TagStatus.Inactive;
    public bool IsActive { get; private set; } = true;

    public int? BatteryLevel { get; private set; }
    public DateTime? LastSeenAt { get; private set; }
    public DateTime? LastEventAt { get; private set; }

    public string? MetadataJson { get; private set; }

    public ICollection<TagAssignment> Assignments { get; private set; } = new List<TagAssignment>();
    public ICollection<LocationEvent> LocationEvents { get; private set; } = new List<LocationEvent>();
    public ICollection<BatteryEvent> BatteryEvents { get; private set; } = new List<BatteryEvent>();
    public ICollection<ImuEvent> ImuEvents { get; private set; } = new List<ImuEvent>();

    // Proximity için ayrı navigation'lar
    public ICollection<ProximityEvent> PrimaryProximityEvents { get; private set; } = new List<ProximityEvent>();
    public ICollection<ProximityEvent> PeerProximityEvents { get; private set; } = new List<ProximityEvent>();

    public ICollection<EmergencyEvent> EmergencyEvents { get; private set; } = new List<EmergencyEvent>();
    public ICollection<CurrentLocation> CurrentLocations { get; private set; } = new List<CurrentLocation>();

    // Alarm için ayrı navigation'lar
    public ICollection<Alarm> PrimaryAlarms { get; private set; } = new List<Alarm>();
    public ICollection<Alarm> PeerAlarms { get; private set; } = new List<Alarm>();

    public ICollection<TagDataEvent> TagDataEvents { get; private set; } = new List<TagDataEvent>();
    public ICollection<UwbRangingEvent> UwbRangingEvents { get; private set; } = new List<UwbRangingEvent>();
    public ICollection<UwbTagToTagRangingEvent> PrimaryUwbTagToTagRangingEvents { get; private set; } = new List<UwbTagToTagRangingEvent>();
    public ICollection<UwbTagToTagRangingEvent> PeerUwbTagToTagRangingEvents { get; private set; } = new List<UwbTagToTagRangingEvent>();

    public ICollection<BleConfigEvent> BleConfigEvents { get; private set; } = new List<BleConfigEvent>();
    public ICollection<UwbConfigEvent> UwbConfigEvents { get; private set; } = new List<UwbConfigEvent>();
    public ICollection<DioConfigEvent> DioConfigEvents { get; private set; } = new List<DioConfigEvent>();
    public ICollection<I2cConfigEvent> I2cConfigEvents { get; private set; } = new List<I2cConfigEvent>();

    public ICollection<TagBleConfigSnapshot> BleConfigSnapshots { get; private set; } = new List<TagBleConfigSnapshot>();
    public ICollection<TagUwbConfigSnapshot> UwbConfigSnapshots { get; private set; } = new List<TagUwbConfigSnapshot>();
    public ICollection<TagDioConfigSnapshot> DioConfigSnapshots { get; private set; } = new List<TagDioConfigSnapshot>();
    public ICollection<TagI2cConfigSnapshot> I2cConfigSnapshots { get; private set; } = new List<TagI2cConfigSnapshot>();

    public static Tag Create(
        string externalId,
        string code,
        string? name = null,
        string? serialNumber = null,
        TagType tagType = TagType.Personnel,
        string? metadataJson = null)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            throw new ArgumentException("ExternalId is required.", nameof(externalId));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));

        return new Tag
        {
            ExternalId = externalId.Trim(),
            Code = code.Trim(),
            Name = name?.Trim(),
            SerialNumber = serialNumber?.Trim(),
            TagType = tagType,
            MetadataJson = metadataJson,
            Status = TagStatus.Inactive,
            IsActive = true
        };
    }

    public void UpdateInfo(string code, string? name, string? serialNumber, TagType tagType, string? metadataJson)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));

        Code = code.Trim();
        Name = name?.Trim();
        SerialNumber = serialNumber?.Trim();
        TagType = tagType;
        MetadataJson = metadataJson;
    }

    public void MarkSeen(DateTime eventAt)
    {
        LastSeenAt = eventAt;
        LastEventAt = eventAt;
        if (IsActive)
            Status = TagStatus.Online;
    }

    public void UpdateBattery(int batteryLevel, DateTime eventAt)
    {
        if (batteryLevel < 0 || batteryLevel > 100)
            throw new ArgumentOutOfRangeException(nameof(batteryLevel));

        BatteryLevel = batteryLevel;
        LastEventAt = eventAt;
        if (LastSeenAt == null || eventAt > LastSeenAt)
            LastSeenAt = eventAt;
    }

    public void SetStatus(TagStatus status, DateTime? eventAt = null)
    {
        Status = status;
        if (eventAt.HasValue)
            LastEventAt = eventAt.Value;
    }

    public void Activate() => IsActive = true;

    public void Deactivate()
    {
        IsActive = false;
        Status = TagStatus.Inactive;
    }
}