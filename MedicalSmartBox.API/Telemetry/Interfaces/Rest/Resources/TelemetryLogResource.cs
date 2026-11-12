namespace MedicalSmartBox.API.Telemetry.Interfaces.Rest.Resources;

/// <summary>
///     HTTP response resource representing a single persisted TelemetryLog entry.
/// </summary>
public record TelemetryLogResource(
    int Id,
    string ContainerId,
    string? TripId,
    DateTime TimestampUtc,
    decimal TemperatureCelsius,
    decimal AmbientTemperatureCelsius,
    decimal WeightGrams,
    decimal BatteryPercentage,
    bool Is12VConnected,
    int PeltierPowerPct,
    bool LidLockEngaged,
    decimal? Latitude,
    decimal? Longitude,
    string FirmwareSignature,
    bool IsThermalExcursion,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt);
