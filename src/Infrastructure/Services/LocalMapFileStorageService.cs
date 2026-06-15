using Application.Common.Options;
using Domain.Abstractions;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class LocalMapFileStorageService : IMapFileStorageService
{
    private readonly StorageOptions _options;

    public LocalMapFileStorageService(IOptions<StorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<MapFileStorageResult> SaveAsync(
        Guid mapId,
        string originalFileName,
        string contentType,
        Stream stream,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.RootPath))
            throw new InvalidOperationException("Storage RootPath is not configured.");

        var extension = Path.GetExtension(originalFileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var relativeDirectory = Path.Combine("maps", mapId.ToString());
        var absoluteDirectory = Path.Combine(_options.RootPath, relativeDirectory);

        Directory.CreateDirectory(absoluteDirectory);

        var absolutePath = Path.Combine(absoluteDirectory, storedFileName);
        await using var fileStream = File.Create(absolutePath);
        await stream.CopyToAsync(fileStream, ct);

        var relativePath = Path.Combine(relativeDirectory, storedFileName).Replace("\\", "/");
        var size = new FileInfo(absolutePath).Length;

        return new MapFileStorageResult(
            storedFileName,
            relativePath,
            size
        );
    }

    public Task DeleteIfExistsAsync(string storagePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.RootPath))
            return Task.CompletedTask;

        var absolutePath = Path.Combine(_options.RootPath, storagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

        if (File.Exists(absolutePath))
            File.Delete(absolutePath);

        return Task.CompletedTask;
    }
}