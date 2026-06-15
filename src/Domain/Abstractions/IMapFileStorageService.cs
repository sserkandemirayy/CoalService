namespace Domain.Abstractions;

public interface IMapFileStorageService
{
    Task<MapFileStorageResult> SaveAsync(
        Guid mapId,
        string originalFileName,
        string contentType,
        Stream stream,
        CancellationToken ct = default);

    Task DeleteIfExistsAsync(string storagePath, CancellationToken ct = default);
}

public sealed record MapFileStorageResult(
    string StoredFileName,
    string StoragePath,
    long FileSize
);