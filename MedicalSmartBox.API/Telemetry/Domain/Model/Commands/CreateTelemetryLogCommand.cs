namespace MedicalSmartBox.API.Telemetry.Domain.Model.Commands;

/// <summary>
///     Command to record a new periodic IoT telemetry snapshot emitted by ESP32 microcontroller.
/// </summary>
public record CreateTelemetryLogCommand(
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
    string FirmwareSignature);
