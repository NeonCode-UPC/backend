namespace MedicalSmartBox.API.SmartContainers.Interfaces.Rest.Resources;

/// <summary>
///     Incoming payload to synchronize real-time telemetry sensor readings into a SmartContainer.
/// </summary>
public record UpdateContainerTelemetrySnapshotResource(
    decimal TemperatureCelsius,
    decimal BatteryPercentage,
    bool Is12vConnected,
    decimal NetWeightGrams,
    string? SignalQuality = "strong",
    DateTimeOffset? TimestampUtc = null);
