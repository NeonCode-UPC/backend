namespace MedicalSmartBox.API.Transport.Domain.ValueObjects;

public class IschemiaTimeLimit
{
    public double MaxSafeHours { get; private set; }
    public double WarningHours { get; private set; }
    
    public IschemiaTimeLimit(double maxSafeHours, double warningHours)
    {
        if (maxSafeHours <= 0)
            throw new ArgumentException("Max safe hours must be greater than zero.", nameof(maxSafeHours));
        
        if (warningHours <= 0||warningHours >= maxSafeHours)
            throw new ArgumentException("Warning hours must be greater than zero and less than max safe hours.", nameof(warningHours));
        
        MaxSafeHours = maxSafeHours;
        WarningHours = warningHours;
    }
    
    public bool IsViolated(double elapsedHours)
    {
        return elapsedHours > MaxSafeHours;
    }
    
    public bool IsApproachingLimit(double elapsedHours)
    {
        return elapsedHours >= WarningHours && !IsViolated(elapsedHours);
    }
}