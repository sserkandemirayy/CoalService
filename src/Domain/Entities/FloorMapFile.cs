using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class FloorMapFile : BaseEntity
{
    protected FloorMapFile() { }

    public Guid FloorMapId { get; private set; }
    public FloorMap FloorMap { get; private set; } = default!;

    public FloorMapFileType FileType { get; private set; }

    public string OriginalFileName { get; private set; } = default!;
    public string StoredFileName { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public string StoragePath { get; private set; } = default!;
    public long FileSize { get; private set; }

    public int Version { get; private set; } = 1;
    public bool IsActive { get; private set; } = true;

    public static FloorMapFile Create(
        Guid floorMapId,
        FloorMapFileType fileType,
        string originalFileName,
        string storedFileName,
        string contentType,
        string storagePath,
        long fileSize,
        int version)
    {
        if (floorMapId == Guid.Empty)
            throw new ArgumentException("FloorMapId is required.", nameof(floorMapId));

        return new FloorMapFile
        {
            FloorMapId = floorMapId,
            FileType = fileType,
            OriginalFileName = originalFileName.Trim(),
            StoredFileName = storedFileName.Trim(),
            ContentType = contentType.Trim(),
            StoragePath = storagePath.Trim(),
            FileSize = fileSize,
            Version = version <= 0 ? 1 : version,
            IsActive = true
        };
    }

    public void Deactivate() => IsActive = false;
}