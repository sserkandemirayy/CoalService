using System.Collections.Concurrent;
using Application.Common.SystemHealth;

namespace Infrastructure.Services;

public sealed class ApiMetricsStore : IApiMetricsStore
{
    private readonly ConcurrentQueue<ServerErrorMetric> _serverErrors = new();

    private long _totalRequests;
    private long _totalElapsedMicroseconds;
    private long _totalServerErrors;

    public void RecordRequest(
        int statusCode,
        double elapsedMilliseconds)
    {
        Interlocked.Increment(ref _totalRequests);

        var elapsedMicroseconds =
            Math.Max(0L, (long)(elapsedMilliseconds * 1000d));

        Interlocked.Add(
            ref _totalElapsedMicroseconds,
            elapsedMicroseconds);

        if (statusCode >= 500)
        {
            Interlocked.Increment(ref _totalServerErrors);

            _serverErrors.Enqueue(
                new ServerErrorMetric(DateTime.UtcNow));
        }

        PruneOldErrors();
    }

    public ApiMetricsSnapshot GetSnapshot()
    {
        PruneOldErrors();

        var totalRequests =
            Interlocked.Read(ref _totalRequests);

        var totalElapsedMicroseconds =
            Interlocked.Read(ref _totalElapsedMicroseconds);

        var totalServerErrors =
            Interlocked.Read(ref _totalServerErrors);

        var averageResponseTimeMs =
            totalRequests == 0
                ? 0d
                : (totalElapsedMicroseconds / 1000d) / totalRequests;

        var errorRatePercent =
            totalRequests == 0
                ? 0d
                : totalServerErrors * 100d / totalRequests;

        var now = DateTime.UtcNow;
        var last5 = now.AddMinutes(-5);
        var last15 = now.AddMinutes(-15);

        var errors = _serverErrors.ToArray();

        var errorsLast5Minutes =
            errors.Count(x => x.Timestamp >= last5);

        var errorsLast15Minutes =
            errors.Count(x => x.Timestamp >= last15);

        return new ApiMetricsSnapshot(
            totalRequests,
            Math.Round(averageResponseTimeMs, 2),
            totalServerErrors,
            Math.Round(errorRatePercent, 2),
            errorsLast5Minutes,
            errorsLast15Minutes);
    }

    private void PruneOldErrors()
    {
        var threshold = DateTime.UtcNow.AddMinutes(-15);

        while (_serverErrors.TryPeek(out var item) &&
               item.Timestamp < threshold)
        {
            _serverErrors.TryDequeue(out _);
        }
    }

    private sealed record ServerErrorMetric(
        DateTime Timestamp);
}