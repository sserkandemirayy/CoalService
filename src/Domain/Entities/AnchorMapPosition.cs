using Domain.Abstractions;

namespace Domain.Entities;

public class AnchorMapPosition : BaseEntity
{
    protected AnchorMapPosition() { }

    public Guid FloorMapId { get; private set; }
    public FloorMap FloorMap { get; private set; } = default!;

    public Guid AnchorId { get; private set; }
    public Anchor Anchor { get; private set; } = default!;

    public decimal X { get; private set; }
    public decimal Y { get; private set; }
    public decimal Z { get; private set; }

    public decimal? Rotation { get; private set; }
    public string? MetadataJson { get; private set; }

    public static AnchorMapPosition Create(
        Guid floorMapId,
        Guid anchorId,
        decimal x,
        decimal y,
        decimal z,
        decimal? rotation = null,
        string? metadataJson = null)
    {
        return new AnchorMapPosition
        {
            FloorMapId = floorMapId,
            AnchorId = anchorId,
            X = x,
            Y = y,
            Z = z,
            Rotation = rotation,
            MetadataJson = metadataJson
        };
    }

    public void Update(decimal x, decimal y, decimal z, decimal? rotation, string? metadataJson)
    {
        X = x;
        Y = y;
        Z = z;
        Rotation = rotation;
        MetadataJson = metadataJson;
    }
}