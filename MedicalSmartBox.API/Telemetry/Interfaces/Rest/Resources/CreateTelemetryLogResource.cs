namespace MedicalSmartBox.API.Telemetry.Interfaces.Rest.Resources;

/// <summary>
///     HTTP request payload transmitted by ESP32 IoT microcontroller to record periodic telemetry.
/// </summary>
public record CreateTelemetryLogResource(
    string ContainerId,
    string? TripId,
    DateTime? TimestampUtc,
    decimal TemperatureCelsius,
    decimal AmbientTemperatureCelsius,
    decimal WeightGrams,
    decimal BatteryPercentage,
    bool Is12VConnected,
    int PeltierPowerPct,
    bool LidLockEngaged,
    decimal? Latitude,
    decimal? Longitude,
    string FirmwareSignature);
