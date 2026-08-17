using Application.DTOs.SystemHealth;

namespace Application.Common.SystemHealth;

public interface ISystemHealthService
{
    Task<SystemHealthDto> GetAsync(CancellationToken ct = default);
}

public interface IApiMetricsStore
{
    void RecordRequest(int statusCode, double elapsedMilliseconds);

    ApiMetricsSnapshot GetSnapshot();
}

public sealed record ApiMetricsSnapshot(
    long TotalRequests,
    double AverageResponseTimeMs,
    long TotalServerErrors,
    double ErrorRatePercent,
    int ServerErrorsLast5Minutes,
    int ServerErrorsLast15Minutes
);