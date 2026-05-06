using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class I2cDataEvent : BaseEntity
{
    protected I2cDataEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }

    public int Address { get; private set; }
    public int Register { get; private set; }
    public I2cDataDirection Direction { get; private set; }
    public bool Ack { get; private set; }
    public string DataJson { get; private set; } = default!;

    public static I2cDataEvent Create(
        Guid rawEventId,
        Guid tagId,
        DateTime eventTimestamp,
        int address,
        int register,
        I2cDataDirection direction,
        bool ack,
        string dataJson)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (address < 0 || address > 127)
            throw new ArgumentOutOfRangeException(nameof(address));
        if (register < 0 || register > 255)
            throw new ArgumentOutOfRangeException(nameof(register));
        if (string.IsNullOrWhiteSpace(dataJson))
            throw new ArgumentException("DataJson is required.", nameof(dataJson));

        return new I2cDataEvent
        {
            RawEventId = rawEventId,
            TagId = tagId,
            EventTimestamp = eventTimestamp,
            Address = address,
            Register = register,
            Direction = direction,
            Ack = ack,
            DataJson = dataJson
        };
    }
}