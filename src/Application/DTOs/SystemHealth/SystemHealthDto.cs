namespace Application.DTOs.SystemHealth;

public sealed record SystemHealthDto(
    string OverallStatus,
    DateTime CheckedAt,
    ApiHealthDto Api,
    DatabaseHealthDto Database,
    ServerHealthDto Server
);

public sealed record ApiHealthDto(
    string Status,
    DateTime StartedAt,
    long UptimeSeconds,
    double ProcessMemoryMb,
    double? ProcessCpuUsagePercent,
    long TotalRequests,
    double AverageResponseTimeMs,
    double ErrorRatePercent,
    long TotalServerErrors,
    int ServerErrorsLast5Minutes,
    int ServerErrorsLast15Minutes
);

public sealed record DatabaseHealthDto(
    string Status,
    bool Connected,
    string ConnectionState,
    double? ResponseTimeMs,
    int? ActiveConnections,
    long FailedChecksSinceStart,
    long ConsecutiveFailedChecks,
    DateTime? LastSuccessfulCheckAt,
    string? Error
);

public sealed record ServerHealthDto(
    string Status,
    string MachineName,
    string OperatingSystem,
    bool IsContainer,
    double? CpuUsagePercent,
    double? MemoryUsagePercent,
    double? UsedMemoryMb,
    double? TotalMemoryMb,
    double ProcessMemoryMb,
    double? DiskUsagePercent,
    double? DiskFreeGb,
    double? DiskTotalGb,
    string? DiskName,
    long? UptimeSeconds
);