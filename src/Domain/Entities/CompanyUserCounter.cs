namespace Domain.Entities;

public class CompanyUserCounter
{
    private CompanyUserCounter()
    {
    }

    public Guid CompanyId { get; private set; }

    public long LastValue { get; private set; }

    public Company Company { get; private set; } = default!;

    public static CompanyUserCounter Create(
        Guid companyId,
        long lastValue = 0)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException(
                "CompanyId is required.",
                nameof(companyId));

        if (lastValue < 0)
            throw new ArgumentOutOfRangeException(
                nameof(lastValue),
                "LastValue cannot be negative.");

        return new CompanyUserCounter
        {
            CompanyId = companyId,
            LastValue = lastValue
        };
    }

    public void EnsureAtLeast(long value)
    {
        if (value > LastValue)
            LastValue = value;
    }
}