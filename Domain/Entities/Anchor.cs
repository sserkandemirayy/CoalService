using Domain.Abstractions;
using Domain.Enums;
using System.Security.Claims;

namespace Domain.Entities;

public class Anchor : BaseEntity
{
    protected Anchor() { }

    public string ExternalId { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public string? Name { get; private set; }
    public string? IpAddress { get; private set; }

    public AnchorStatus Status { get; private set; } = AnchorStatus.Offline;
    public DateTime? LastHeartbeatAt { get; private set; }
    public DateTime? LastStatusChangedAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Guid? BranchId { get; private set; }
    public Branch? Branch { get; private set; }

    public Guid? CompanyId { get; private set; }
    public Company? Company { get; private set; }

    public string? MetadataJson { get; private set; }

    public ICollection<BatteryEvent> BatteryEvents { get; private set; } = new List<BatteryEvent>();
    public ICollection<AnchorHeartbeatEvent> HeartbeatEvents { get; private set; } = new List<AnchorHeartbeatEvent>();
    public ICollection<AnchorStatusEvent> StatusEvents { get; private set; } = new List<AnchorStatusEvent>();
    public ICollection<Alarm> Alarms { get; private set; } = new List<Alarm>();

    public ICollection<AnchorHealthEvent> HealthEvents { get; private set; } = new List<AnchorHealthEvent>();
    public ICollection<TagDataEvent> TagDataEvents { get; private set; } = new List<TagDataEvent>();
    public ICollection<UwbRangingEvent> UwbRangingEvents { get; private set; } = new List<UwbRangingEvent>();

    public static Anchor Create(
        string externalId,
        string code,
        string? name = null,
        string? ipAddress = null,
        Guid? companyId = null,
        Guid? branchId = null,
        string? metadataJson = null)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            throw new ArgumentException("ExternalId is required.", nameof(externalId));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));

        return new Anchor
        {
            ExternalId = externalId.Trim(),
            Code = code.Trim(),
            Name = name?.Trim(),
            IpAddress = ipAddress?.Trim(),
            CompanyId = companyId,
            BranchId = branchId,
            MetadataJson = metadataJson,
            Status = AnchorStatus.Offline
        };
    }

    public void UpdateInfo(
        string code,
        string? name,
        string? ipAddress,
        Guid? companyId,
        Guid? branchId,
        string? metadataJson)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));

        Code = code.Trim();
        Name = name?.Trim();
        IpAddress = ipAddress?.Trim();
        CompanyId = companyId;
        BranchId = branchId;
        MetadataJson = metadataJson;
    }

    public void RegisterHeartbeat(DateTime eventAt, string? ipAddress = null)
    {
        LastHeartbeatAt = eventAt;
        LastStatusChangedAt ??= eventAt;
        if (!string.IsNullOrWhiteSpace(ipAddress))
            IpAddress = ipAddress.Trim();

        if (IsActive)
            Status = AnchorStatus.Online;
    }

    public void ChangeStatus(AnchorStatus newStatus, DateTime eventAt)
    {
        Status = newStatus;
        LastStatusChangedAt = eventAt;
    }

    public void Activate() => IsActive = true;

    public void Deactivate()
    {
        IsActive = false;
        Status = AnchorStatus.Offline;
    }
}