using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Application.Common.SystemHealth;
using Application.DTOs.SystemHealth;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class SystemHealthService
    : ISystemHealthService
{
    private readonly AppDbContext _db;
    private readonly IApiMetricsStore _apiMetrics;
    private readonly DatabaseHealthState _databaseState;
    private readonly SystemResourceMonitor _resourceMonitor;

    private readonly DateTime _processStartedAt;

    public SystemHealthService(
        AppDbContext db,
        IApiMetricsStore apiMetrics,
        DatabaseHealthState databaseState,
        SystemResourceMonitor resourceMonitor)
    {
        _db = db;
        _apiMetrics = apiMetrics;
        _databaseState = databaseState;
        _resourceMonitor = resourceMonitor;

        _processStartedAt =
            GetProcessStartedAt();
    }

    public async Task<SystemHealthDto> GetAsync(
        CancellationToken ct = default)
    {
        var checkedAt =
            DateTime.UtcNow;

        var resources =
            _resourceMonitor.GetSnapshot();

        var api =
            BuildApiHealth(
                resources,
                checkedAt);

        var database =
            await BuildDatabaseHealthAsync(ct);

        var server =
            BuildServerHealth(resources);

        var overallStatus =
            CalculateOverallStatus(
                api.Status,
                database.Status,
                server.Status);

        return new SystemHealthDto(
            overallStatus,
            checkedAt,
            api,
            database,
            server);
    }

    // ============================================================
    // API
    // ============================================================

    private ApiHealthDto BuildApiHealth(
        SystemResourceSnapshot resources,
        DateTime now)
    {
        var metrics =
            _apiMetrics.GetSnapshot();

        var uptimeSeconds =
            Math.Max(
                0,
                (long)(now - _processStartedAt)
                    .TotalSeconds);

        var processMemoryMb =
            BytesToMb(
                resources.ProcessMemoryBytes);

        var status =
            CalculateApiStatus(
                metrics,
                resources.ProcessCpuUsagePercent);

        return new ApiHealthDto(
            status,
            _processStartedAt,
            uptimeSeconds,
            Round(processMemoryMb),
            RoundNullable(
                resources.ProcessCpuUsagePercent),
            metrics.TotalRequests,
            Round(metrics.AverageResponseTimeMs),
            Round(metrics.ErrorRatePercent),
            metrics.TotalServerErrors,
            metrics.ServerErrorsLast5Minutes,
            metrics.ServerErrorsLast15Minutes);
    }

    // ============================================================
    // DATABASE
    // ============================================================

    private async Task<DatabaseHealthDto>
        BuildDatabaseHealthAsync(
            CancellationToken ct)
    {
        var stopwatch =
            new Stopwatch();

        var connection =
            _db.Database.GetDbConnection();

        var openedHere =
            connection.State != ConnectionState.Open;

        try
        {
            stopwatch.Start();

            if (openedHere)
            {
                await connection.OpenAsync(ct);
            }

            await using (var command =
                         connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT 1;";

                command.CommandTimeout = 5;

                await command.ExecuteScalarAsync(ct);
            }

            stopwatch.Stop();

            var responseTimeMs =
                stopwatch.Elapsed.TotalMilliseconds;

            int? activeConnections = null;

            // DB bağlantısı sağlıklı olsa bile pg_stat_activity
            // permission nedeniyle okunamayabilir.
            // Böyle bir durumda DB'yi unhealthy yapmıyoruz.
            try
            {
                await using var command =
                    connection.CreateCommand();

                command.CommandText = """
                    SELECT COUNT(*)
                    FROM pg_stat_activity
                    WHERE datname = current_database();
                    """;

                command.CommandTimeout = 5;

                var result =
                    await command.ExecuteScalarAsync(ct);

                if (result is not null &&
                    result != DBNull.Value)
                {
                    activeConnections =
                        Convert.ToInt32(result);
                }
            }
            catch
            {
                activeConnections = null;
            }

            var connectionState =
                connection.State.ToString();

            _databaseState.MarkSuccess();

            var status =
                CalculateDatabaseStatus(
                    responseTimeMs);

            return new DatabaseHealthDto(
                status,
                true,
                connectionState,
                Round(responseTimeMs),
                activeConnections,
                _databaseState.FailedChecks,
                _databaseState.ConsecutiveFailedChecks,
                _databaseState.LastSuccessfulCheckAt,
                null);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _databaseState.MarkFailure();

            return new DatabaseHealthDto(
                "Critical",
                false,
                connection.State.ToString(),
                stopwatch.ElapsedMilliseconds > 0
                    ? Round(
                        stopwatch.Elapsed.TotalMilliseconds)
                    : null,
                null,
                _databaseState.FailedChecks,
                _databaseState.ConsecutiveFailedChecks,
                _databaseState.LastSuccessfulCheckAt,
                ex.Message);
        }
        finally
        {
            if (openedHere &&
                connection.State != ConnectionState.Closed)
            {
                try
                {
                    await connection.CloseAsync();
                }
                catch
                {
                    // Health endpoint DB close hatası
                    // nedeniyle uygulamayı düşürmemeli.
                }
            }
        }
    }

    // ============================================================
    // SERVER
    // ============================================================

    private ServerHealthDto BuildServerHealth(
        SystemResourceSnapshot resources)
    {
        double? memoryUsagePercent = null;

        if (resources.TotalMemoryBytes.HasValue &&
            resources.UsedMemoryBytes.HasValue &&
            resources.TotalMemoryBytes.Value > 0)
        {
            memoryUsagePercent =
                resources.UsedMemoryBytes.Value *
                100d /
                resources.TotalMemoryBytes.Value;
        }

        double? diskUsagePercent = null;

        if (resources.DiskTotalBytes.HasValue &&
            resources.DiskFreeBytes.HasValue &&
            resources.DiskTotalBytes.Value > 0)
        {
            var used =
                resources.DiskTotalBytes.Value -
                resources.DiskFreeBytes.Value;

            diskUsagePercent =
                used *
                100d /
                resources.DiskTotalBytes.Value;
        }

        var status =
            CalculateServerStatus(
                resources.SystemCpuUsagePercent,
                memoryUsagePercent,
                diskUsagePercent);

        return new ServerHealthDto(
            status,
            Environment.MachineName,
            RuntimeInformation.OSDescription,
            IsRunningInContainer(),
            RoundNullable(
                resources.SystemCpuUsagePercent),
            RoundNullable(
                memoryUsagePercent),
            resources.UsedMemoryBytes.HasValue
                ? Round(
                    BytesToMb(
                        resources.UsedMemoryBytes.Value))
                : null,
            resources.TotalMemoryBytes.HasValue
                ? Round(
                    BytesToMb(
                        resources.TotalMemoryBytes.Value))
                : null,
            Round(
                BytesToMb(
                    resources.ProcessMemoryBytes)),
            RoundNullable(
                diskUsagePercent),
            resources.DiskFreeBytes.HasValue
                ? Round(
                    BytesToGb(
                        resources.DiskFreeBytes.Value))
                : null,
            resources.DiskTotalBytes.HasValue
                ? Round(
                    BytesToGb(
                        resources.DiskTotalBytes.Value))
                : null,
            resources.DiskName,
            resources.UptimeSeconds);
    }

    // ============================================================
    // STATUS RULES
    // ============================================================

    private static string CalculateApiStatus(
        ApiMetricsSnapshot metrics,
        double? processCpu)
    {
        if (metrics.ErrorRatePercent >= 5 ||
            metrics.AverageResponseTimeMs >= 2000 ||
            processCpu >= 95)
        {
            return "Critical";
        }

        if (metrics.ErrorRatePercent >= 1 ||
            metrics.AverageResponseTimeMs >= 500 ||
            processCpu >= 80 ||
            metrics.ServerErrorsLast5Minutes >= 5)
        {
            return "Warning";
        }

        return "Healthy";
    }

    private static string CalculateDatabaseStatus(
        double responseTimeMs)
    {
        if (responseTimeMs >= 1000)
            return "Critical";

        if (responseTimeMs >= 250)
            return "Warning";

        return "Healthy";
    }

    private static string CalculateServerStatus(
        double? cpu,
        double? memory,
        double? disk)
    {
        if (cpu >= 95 ||
            memory >= 95 ||
            disk >= 95)
        {
            return "Critical";
        }

        if (cpu >= 80 ||
            memory >= 85 ||
            disk >= 85)
        {
            return "Warning";
        }

        return "Healthy";
    }

    private static string CalculateOverallStatus(
        params string[] statuses)
    {
        if (statuses.Any(x =>
                x.Equals(
                    "Critical",
                    StringComparison.OrdinalIgnoreCase)))
        {
            return "Critical";
        }

        if (statuses.Any(x =>
                x.Equals(
                    "Warning",
                    StringComparison.OrdinalIgnoreCase)))
        {
            return "Warning";
        }

        return "Healthy";
    }

    // ============================================================
    // HELPERS
    // ============================================================

    private static DateTime GetProcessStartedAt()
    {
        try
        {
            using var process =
                Process.GetCurrentProcess();

            return process.StartTime.ToUniversalTime();
        }
        catch
        {
            return DateTime.UtcNow;
        }
    }

    private static bool IsRunningInContainer()
    {
        var value =
            Environment.GetEnvironmentVariable(
                "DOTNET_RUNNING_IN_CONTAINER");

        return string.Equals(
                   value,
                   "true",
                   StringComparison.OrdinalIgnoreCase) ||
               string.Equals(
                   value,
                   "1",
                   StringComparison.OrdinalIgnoreCase);
    }

    private static double BytesToMb(
        long bytes)
        => bytes / 1024d / 1024d;

    private static double BytesToGb(
        long bytes)
        => bytes / 1024d / 1024d / 1024d;

    private static double Round(
        double value)
        => Math.Round(value, 2);

    private static double? RoundNullable(
        double? value)
        => value.HasValue
            ? Math.Round(value.Value, 2)
            : null;
}