namespace MedicalSmartBox.API.Transport.Domain.ValueObjects;

public class IschemiaTimeLimit
{
    public double MaxSafeHours { get; private set; }
    public double WarningThresholdHours { get; private set; }
    
    public IschemiaTimeLimit(double maxSafeHours, double warningHours)
    {
        if (!double.IsFinite(maxSafeHours) || maxSafeHours <= 0 || maxSafeHours > 99.99
            || Math.Round(maxSafeHours, 2) != maxSafeHours)
            throw new ArgumentOutOfRangeException(nameof(maxSafeHours), "Max safe hours must be between zero and 99.99.");

        if (!double.IsFinite(warningHours) || warningHours <= 0 || warningHours >= maxSafeHours
            || warningHours > 99.98 || Math.Round(warningHours, 2) != warningHours)
            throw new ArgumentException("Warning hours must be greater than zero and less than max safe hours.", nameof(warningHours));

        MaxSafeHours = maxSafeHours;
        WarningThresholdHours = warningHours;
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
