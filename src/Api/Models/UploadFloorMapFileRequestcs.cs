using Domain.Enums;
using Microsoft.AspNetCore.Http;

public sealed class UploadFloorMapFileRequest
{
    public IFormFile File { get; set; } = default!;
    public FloorMapFileType FileType { get; set; }
    public int Version { get; set; } = 1;
}