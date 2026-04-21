using Domain.Abstractions;

namespace Domain.Entities;

public class AnchorHealthEvent : BaseEntity
{
    protected AnchorHealthEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid AnchorId { get; private set; }
    public Anchor Anchor { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }

    public long Uptime { get; private set; }
    public decimal Temperature { get; private set; }
    public decimal CpuUsage { get; private set; }
    public decimal MemoryUsage { get; private set; }
    public int TagCount { get; private set; }
    public decimal PacketLossRate { get; private set; }

    public static AnchorHealthEvent Create(
        Guid rawEventId,
        Guid anchorId,
        DateTime eventTimestamp,
        long uptime,
        decimal temperature,
        decimal cpuUsage,
        decimal memoryUsage,
        int tagCount,
        decimal packetLossRate)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (anchorId == Guid.Empty)
            throw new ArgumentException("AnchorId is required.", nameof(anchorId));
        if (uptime < 0)
            throw new ArgumentOutOfRangeException(nameof(uptime));
        if (cpuUsage < 0 || cpuUsage > 100)
            throw new ArgumentOutOfRangeException(nameof(cpuUsage));
        if (memoryUsage < 0 || memoryUsage > 100)
            throw new ArgumentOutOfRangeException(nameof(memoryUsage));
        if (tagCount < 0)
            throw new ArgumentOutOfRangeException(nameof(tagCount));
        if (packetLossRate < 0 || packetLossRate > 100)
            throw new ArgumentOutOfRangeException(nameof(packetLossRate));

        return new AnchorHealthEvent
        {
            RawEventId = rawEventId,
            AnchorId = anchorId,
            EventTimestamp = eventTimestamp,
            Uptime = uptime,
            Temperature = temperature,
            CpuUsage = cpuUsage,
            MemoryUsage = memoryUsage,
            TagCount = tagCount,
            PacketLossRate = packetLossRate
        };
    }
}