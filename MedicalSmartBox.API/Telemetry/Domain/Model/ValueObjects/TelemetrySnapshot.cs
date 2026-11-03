namespace MedicalSmartBox.API.Telemetry.Domain.Model.ValueObjects;

/// <summary>
///     Value object encapsulating physical environmental and mechanical sensor readings.
/// </summary>
public record TelemetrySnapshot
{
    public decimal TemperatureCelsius { get; init; }
    public decimal AmbientTemperatureCelsius { get; init; }
    public decimal WeightGrams { get; init; }
    public decimal BatteryPercentage { get; init; }
    public bool Is12VConnected { get; init; }
    public int PeltierPowerPct { get; init; }
    public bool LidLockEngaged { get; init; }

    public TelemetrySnapshot()
    {
    }

    public TelemetrySnapshot(
        decimal temperatureCelsius,
        decimal ambientTemperatureCelsius,
        decimal weightGrams,
        decimal batteryPercentage,
        bool is12VConnected,
        int peltierPowerPct,
        bool lidLockEngaged)
    {
        if (weightGrams < 0m)
            throw new ArgumentOutOfRangeException(nameof(weightGrams), "Weight cannot be negative.");

        if (batteryPercentage is < 0.00m or > 100.00m)
            throw new ArgumentOutOfRangeException(nameof(batteryPercentage), "Battery percentage must be between 0 and 100.");

        if (peltierPowerPct is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(peltierPowerPct), "Peltier power percentage must be between 0 and 100.");

        TemperatureCelsius = temperatureCelsius;
        AmbientTemperatureCelsius = ambientTemperatureCelsius;
        WeightGrams = weightGrams;
        BatteryPercentage = batteryPercentage;
        Is12VConnected = is12VConnected;
        PeltierPowerPct = peltierPowerPct;
        LidLockEngaged = lidLockEngaged;
    }
}
