using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services;

public sealed class SystemResourceMonitor : BackgroundService
{
    private readonly object _sync = new();

    private SystemResourceSnapshot _snapshot;

    private CpuTimes? _previousSystemCpu;
    private TimeSpan? _previousProcessCpu;
    private DateTime? _previousProcessSampleAt;

    public SystemResourceMonitor()
    {
        _snapshot = BuildInitialSnapshot();
    }

    public SystemResourceSnapshot GetSnapshot()
    {
        lock (_sync)
        {
            return _snapshot;
        }
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        InitializeCpuSamples();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(2),
                    stoppingToken);

                var snapshot = CollectSnapshot();

                lock (_sync)
                {
                    _snapshot = snapshot;
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch
            {
                // Health monitor ana uygulamayı hiçbir zaman
                // düşürmemelidir.
            }
        }
    }

    private void InitializeCpuSamples()
    {
        if (TryReadSystemCpu(out var cpu))
        {
            _previousSystemCpu = cpu;
        }

        try
        {
            using var process =
                Process.GetCurrentProcess();

            _previousProcessCpu =
                process.TotalProcessorTime;

            _previousProcessSampleAt =
                DateTime.UtcNow;
        }
        catch
        {
            _previousProcessCpu = null;
            _previousProcessSampleAt = null;
        }
    }

    private SystemResourceSnapshot CollectSnapshot()
    {
        double? systemCpu = null;
        double? processCpu = null;

        if (TryReadSystemCpu(out var currentSystemCpu))
        {
            if (_previousSystemCpu.HasValue)
            {
                var previous =
                    _previousSystemCpu.Value;

                var totalDelta =
                    currentSystemCpu.Total -
                    previous.Total;

                var idleDelta =
                    currentSystemCpu.Idle -
                    previous.Idle;

                if (totalDelta > 0)
                {
                    systemCpu =
                        (totalDelta - idleDelta) *
                        100d /
                        totalDelta;

                    systemCpu =
                        ClampPercentage(systemCpu.Value);
                }
            }

            _previousSystemCpu =
                currentSystemCpu;
        }

        long processMemoryBytes = 0;

        try
        {
            using var process =
                Process.GetCurrentProcess();

            process.Refresh();

            processMemoryBytes =
                process.WorkingSet64;

            var now = DateTime.UtcNow;
            var currentCpu =
                process.TotalProcessorTime;

            if (_previousProcessCpu.HasValue &&
                _previousProcessSampleAt.HasValue)
            {
                var cpuDelta =
                    currentCpu -
                    _previousProcessCpu.Value;

                var elapsed =
                    now -
                    _previousProcessSampleAt.Value;

                if (elapsed.TotalMilliseconds > 0)
                {
                    processCpu =
                        cpuDelta.TotalMilliseconds /
                        (
                            elapsed.TotalMilliseconds *
                            Environment.ProcessorCount
                        ) *
                        100d;

                    processCpu =
                        ClampPercentage(processCpu.Value);
                }
            }

            _previousProcessCpu =
                currentCpu;

            _previousProcessSampleAt =
                now;
        }
        catch
        {
            // ignored intentionally
        }

        TryGetMemoryInfo(
            out var totalMemory,
            out var usedMemory);

        TryGetDiskInfo(
            out var diskName,
            out var diskTotal,
            out var diskFree);

        return new SystemResourceSnapshot(
            systemCpu,
            processCpu,
            totalMemory,
            usedMemory,
            processMemoryBytes,
            diskTotal,
            diskFree,
            diskName,
            GetSystemUptimeSeconds());
    }

    private static SystemResourceSnapshot
        BuildInitialSnapshot()
    {
        long processMemoryBytes = 0;

        try
        {
            using var process =
                Process.GetCurrentProcess();

            processMemoryBytes =
                process.WorkingSet64;
        }
        catch
        {
            // ignored
        }

        TryGetMemoryInfo(
            out var totalMemory,
            out var usedMemory);

        TryGetDiskInfo(
            out var diskName,
            out var diskTotal,
            out var diskFree);

        return new SystemResourceSnapshot(
            null,
            null,
            totalMemory,
            usedMemory,
            processMemoryBytes,
            diskTotal,
            diskFree,
            diskName,
            GetSystemUptimeSeconds());
    }

    // ============================================================
    // CPU
    // ============================================================

    private static bool TryReadSystemCpu(
        out CpuTimes cpu)
    {
        if (OperatingSystem.IsLinux())
            return TryReadLinuxCpu(out cpu);

        if (OperatingSystem.IsWindows())
            return TryReadWindowsCpu(out cpu);

        cpu = default;
        return false;
    }

    private static bool TryReadLinuxCpu(
        out CpuTimes cpu)
    {
        cpu = default;

        try
        {
            const string path = "/proc/stat";

            if (!File.Exists(path))
                return false;

            var firstLine =
                File.ReadLines(path)
                    .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(firstLine))
                return false;

            var parts =
                firstLine.Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 5 ||
                parts[0] != "cpu")
            {
                return false;
            }

            var values =
                parts.Skip(1)
                    .Select(x =>
                        long.TryParse(x, out var value)
                            ? value
                            : 0L)
                    .ToArray();

            var idle =
                values.Length > 3
                    ? values[3]
                    : 0;

            var iowait =
                values.Length > 4
                    ? values[4]
                    : 0;

            var total =
                values.Sum();

            cpu = new CpuTimes(
                idle + iowait,
                total);

            return total > 0;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryReadWindowsCpu(
        out CpuTimes cpu)
    {
        cpu = default;

        try
        {
            if (!GetSystemTimes(
                    out var idleTime,
                    out var kernelTime,
                    out var userTime))
            {
                return false;
            }

            var idle =
                FileTimeToLong(idleTime);

            var kernel =
                FileTimeToLong(kernelTime);

            var user =
                FileTimeToLong(userTime);

            // Windows kernel zamanı idle zamanını da içerir.
            var total =
                kernel + user;

            cpu = new CpuTimes(
                idle,
                total);

            return total > 0;
        }
        catch
        {
            return false;
        }
    }

    // ============================================================
    // MEMORY
    // ============================================================

    private static bool TryGetMemoryInfo(
        out long? totalBytes,
        out long? usedBytes)
    {
        totalBytes = null;
        usedBytes = null;

        if (OperatingSystem.IsLinux())
        {
            // Docker / cgroup v2
            if (TryGetCgroupV2Memory(
                    out var cgroupTotal,
                    out var cgroupUsed))
            {
                totalBytes = cgroupTotal;
                usedBytes = cgroupUsed;
                return true;
            }

            // Docker / cgroup v1
            if (TryGetCgroupV1Memory(
                    out cgroupTotal,
                    out cgroupUsed))
            {
                totalBytes = cgroupTotal;
                usedBytes = cgroupUsed;
                return true;
            }

            return TryGetLinuxMemory(
                out totalBytes,
                out usedBytes);
        }

        if (OperatingSystem.IsWindows())
        {
            return TryGetWindowsMemory(
                out totalBytes,
                out usedBytes);
        }

        return false;
    }

    private static bool TryGetCgroupV2Memory(
        out long? totalBytes,
        out long? usedBytes)
    {
        totalBytes = null;
        usedBytes = null;

        try
        {
            const string maxPath =
                "/sys/fs/cgroup/memory.max";

            const string currentPath =
                "/sys/fs/cgroup/memory.current";

            if (!File.Exists(maxPath) ||
                !File.Exists(currentPath))
            {
                return false;
            }

            var maxText =
                File.ReadAllText(maxPath)
                    .Trim();

            if (maxText.Equals(
                    "max",
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!long.TryParse(
                    maxText,
                    out var max))
            {
                return false;
            }

            if (!long.TryParse(
                    File.ReadAllText(currentPath).Trim(),
                    out var current))
            {
                return false;
            }

            // Çok büyük değerler genellikle gerçek bir
            // container memory limit olmadığı anlamına gelir.
            if (max <= 0 ||
                max >= long.MaxValue / 4)
            {
                return false;
            }

            totalBytes = max;
            usedBytes = current;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryGetCgroupV1Memory(
        out long? totalBytes,
        out long? usedBytes)
    {
        totalBytes = null;
        usedBytes = null;

        try
        {
            const string limitPath =
                "/sys/fs/cgroup/memory/memory.limit_in_bytes";

            const string usagePath =
                "/sys/fs/cgroup/memory/memory.usage_in_bytes";

            if (!File.Exists(limitPath) ||
                !File.Exists(usagePath))
            {
                return false;
            }

            if (!long.TryParse(
                    File.ReadAllText(limitPath).Trim(),
                    out var limit))
            {
                return false;
            }

            if (!long.TryParse(
                    File.ReadAllText(usagePath).Trim(),
                    out var usage))
            {
                return false;
            }

            if (limit <= 0 ||
                limit >= long.MaxValue / 4)
            {
                return false;
            }

            totalBytes = limit;
            usedBytes = usage;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryGetLinuxMemory(
        out long? totalBytes,
        out long? usedBytes)
    {
        totalBytes = null;
        usedBytes = null;

        try
        {
            const string path =
                "/proc/meminfo";

            if (!File.Exists(path))
                return false;

            long? totalKb = null;
            long? availableKb = null;

            foreach (var line in File.ReadLines(path))
            {
                if (line.StartsWith(
                        "MemTotal:",
                        StringComparison.Ordinal))
                {
                    totalKb =
                        ParseMemInfoKb(line);
                }
                else if (line.StartsWith(
                             "MemAvailable:",
                             StringComparison.Ordinal))
                {
                    availableKb =
                        ParseMemInfoKb(line);
                }

                if (totalKb.HasValue &&
                    availableKb.HasValue)
                {
                    break;
                }
            }

            if (!totalKb.HasValue ||
                !availableKb.HasValue)
            {
                return false;
            }

            totalBytes =
                totalKb.Value * 1024L;

            usedBytes =
                Math.Max(
                    0,
                    totalBytes.Value -
                    availableKb.Value * 1024L);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static long? ParseMemInfoKb(
        string line)
    {
        var parts =
            line.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
            return null;

        return long.TryParse(
            parts[1],
            out var value)
                ? value
                : null;
    }

    private static bool TryGetWindowsMemory(
        out long? totalBytes,
        out long? usedBytes)
    {
        totalBytes = null;
        usedBytes = null;

        try
        {
            var status =
                new MemoryStatusEx();

            if (!GlobalMemoryStatusEx(status))
                return false;

            totalBytes =
                checked((long)status.TotalPhysical);

            var available =
                checked((long)status.AvailablePhysical);

            usedBytes =
                Math.Max(
                    0,
                    totalBytes.Value - available);

            return true;
        }
        catch
        {
            return false;
        }
    }

    // ============================================================
    // DISK
    // ============================================================

    private static bool TryGetDiskInfo(
        out string? diskName,
        out long? totalBytes,
        out long? freeBytes)
    {
        diskName = null;
        totalBytes = null;
        freeBytes = null;

        try
        {
            var root =
                Path.GetPathRoot(
                    AppContext.BaseDirectory);

            if (string.IsNullOrWhiteSpace(root))
                return false;

            var drive =
                new DriveInfo(root);

            if (!drive.IsReady)
                return false;

            diskName = drive.Name;
            totalBytes = drive.TotalSize;
            freeBytes = drive.AvailableFreeSpace;

            return true;
        }
        catch
        {
            return false;
        }
    }

    // ============================================================
    // UPTIME
    // ============================================================

    private static long? GetSystemUptimeSeconds()
    {
        if (OperatingSystem.IsLinux())
        {
            try
            {
                const string path = "/proc/uptime";

                if (File.Exists(path))
                {
                    var first =
                        File.ReadAllText(path)
                            .Split(
                                ' ',
                                StringSplitOptions.RemoveEmptyEntries)
                            .FirstOrDefault();

                    if (double.TryParse(
                            first,
                            System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out var seconds))
                    {
                        return (long)seconds;
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        try
        {
            return Environment.TickCount64 / 1000L;
        }
        catch
        {
            return null;
        }
    }

    private static double ClampPercentage(
        double value)
        => Math.Max(
            0d,
            Math.Min(100d, value));

    // ============================================================
    // WINDOWS INTEROP
    // ============================================================

    [DllImport(
        "kernel32.dll",
        SetLastError = true)]
    private static extern bool GetSystemTimes(
        out FileTime idleTime,
        out FileTime kernelTime,
        out FileTime userTime);

    [DllImport(
        "kernel32.dll",
        SetLastError = true,
        CharSet = CharSet.Auto)]
    private static extern bool GlobalMemoryStatusEx(
        [In, Out] MemoryStatusEx buffer);

    private static long FileTimeToLong(
        FileTime fileTime)
    {
        return ((long)fileTime.High << 32) |
               fileTime.Low;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FileTime
    {
        public uint Low;
        public uint High;
    }

    [StructLayout(
        LayoutKind.Sequential,
        CharSet = CharSet.Auto)]
    private sealed class MemoryStatusEx
    {
        public uint Length =
            (uint)Marshal.SizeOf<MemoryStatusEx>();

        public uint MemoryLoad;

        public ulong TotalPhysical;
        public ulong AvailablePhysical;

        public ulong TotalPageFile;
        public ulong AvailablePageFile;

        public ulong TotalVirtual;
        public ulong AvailableVirtual;

        public ulong AvailableExtendedVirtual;
    }

    private readonly record struct CpuTimes(
        long Idle,
        long Total);
}

public sealed record SystemResourceSnapshot(
    double? SystemCpuUsagePercent,
    double? ProcessCpuUsagePercent,
    long? TotalMemoryBytes,
    long? UsedMemoryBytes,
    long ProcessMemoryBytes,
    long? DiskTotalBytes,
    long? DiskFreeBytes,
    string? DiskName,
    long? UptimeSeconds
);