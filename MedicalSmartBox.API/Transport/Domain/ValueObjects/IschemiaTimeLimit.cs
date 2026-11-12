namespace MedicalSmartBox.API.Transport.Domain.ValueObjects;

public sealed record IschemiaTimeLimit
{
    public double MaxSafeHours { get; private init; }
    public double WarningThresholdHours { get; private init; }

    private IschemiaTimeLimit() { }
    
    public IschemiaTimeLimit(double maxSafeHours, double warningThresholdHours)
    {
        if (!double.IsFinite(maxSafeHours) || maxSafeHours <= 0 || maxSafeHours > 99.99
            || Math.Round(maxSafeHours, 2) != maxSafeHours)
            throw new ArgumentOutOfRangeException(nameof(maxSafeHours), "Max safe hours must be between zero and 99.99.");

        if (!double.IsFinite(warningThresholdHours) || warningThresholdHours <= 0 || warningThresholdHours >= maxSafeHours
            || warningThresholdHours > 99.98 || Math.Round(warningThresholdHours, 2) != warningThresholdHours)
            throw new ArgumentException("Warning hours must be greater than zero and less than max safe hours.", nameof(warningThresholdHours));

        MaxSafeHours = maxSafeHours;
        WarningThresholdHours = warningThresholdHours;
    }

    public bool IsViolated(double elapsedHours)
    {
        if (!double.IsFinite(elapsedHours) || elapsedHours < 0)
            throw new ArgumentOutOfRangeException(nameof(elapsedHours), "Elapsed hours cannot be negative or non-finite.");

        return elapsedHours > MaxSafeHours;
    }

    public bool IsApproachingLimit(double elapsedHours)
    {
        if (!double.IsFinite(elapsedHours) || elapsedHours < 0)
            throw new ArgumentOutOfRangeException(nameof(elapsedHours), "Elapsed hours cannot be negative or non-finite.");

        return elapsedHours >= WarningThresholdHours && !IsViolated(elapsedHours);
    }
}
