namespace Infrastructure.Services;

public sealed class DatabaseHealthState
{
    private long _failedChecks;
    private long _consecutiveFailedChecks;
    private long _lastSuccessfulCheckUtcTicks;

    public void MarkSuccess()
    {
        Interlocked.Exchange(
            ref _consecutiveFailedChecks,
            0);

        Interlocked.Exchange(
            ref _lastSuccessfulCheckUtcTicks,
            DateTime.UtcNow.Ticks);
    }

    public void MarkFailure()
    {
        Interlocked.Increment(
            ref _failedChecks);

        Interlocked.Increment(
            ref _consecutiveFailedChecks);
    }

    public long FailedChecks =>
        Interlocked.Read(ref _failedChecks);

    public long ConsecutiveFailedChecks =>
        Interlocked.Read(ref _consecutiveFailedChecks);

    public DateTime? LastSuccessfulCheckAt
    {
        get
        {
            var ticks =
                Interlocked.Read(
                    ref _lastSuccessfulCheckUtcTicks);

            if (ticks <= 0)
                return null;

            return new DateTime(
                ticks,
                DateTimeKind.Utc);
        }
    }
}