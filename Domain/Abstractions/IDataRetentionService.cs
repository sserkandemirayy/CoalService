namespace Domain.Abstractions;

public interface IDataRetentionService
{
    Task RunAsync(CancellationToken ct = default);
}
