namespace MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

/// <summary>
///     Value object encapsulating the safe thermal boundaries and excursion tolerance.
/// </summary>
public sealed record ThermalThresholds
{
    public const decimal DefaultMinSafeTemp = 2.00m;
    public const decimal DefaultMaxSafeTemp = 8.00m;
    public const decimal DefaultWarningTemp = 7.50m;
    public const int DefaultExcursionToleranceSeconds = 120;

    public decimal MinTemperatureCelsius { get; }
    public decimal MaxTemperatureCelsius { get; }
    public decimal WarningTemperatureCelsius { get; }
    public int ExcursionToleranceSeconds { get; }

    public ThermalThresholds(
        decimal minTemperatureCelsius = DefaultMinSafeTemp,
        decimal maxTemperatureCelsius = DefaultMaxSafeTemp,
        decimal warningTemperatureCelsius = DefaultWarningTemp,
        int excursionToleranceSeconds = DefaultExcursionToleranceSeconds)
    {
        if (minTemperatureCelsius >= maxTemperatureCelsius)
            throw new ArgumentException("Minimum temperature must be strictly less than maximum temperature.");

        if (warningTemperatureCelsius < minTemperatureCelsius || warningTemperatureCelsius > maxTemperatureCelsius)
            throw new ArgumentException("Warning threshold must fall within the minimum and maximum temperature range.");

        if (excursionToleranceSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(excursionToleranceSeconds), "Tolerance seconds must be positive.");

        MinTemperatureCelsius = minTemperatureCelsius;
        MaxTemperatureCelsius = maxTemperatureCelsius;
        WarningTemperatureCelsius = warningTemperatureCelsius;
        ExcursionToleranceSeconds = excursionToleranceSeconds;
    }

    public bool IsSafe(decimal temperature) =>
        temperature >= MinTemperatureCelsius && temperature <= MaxTemperatureCelsius;

    public bool IsWarning(decimal temperature) =>
        temperature >= WarningTemperatureCelsius && temperature <= MaxTemperatureCelsius;

    public bool IsExcursion(decimal temperature) =>
        temperature < MinTemperatureCelsius || temperature > MaxTemperatureCelsius;
}
