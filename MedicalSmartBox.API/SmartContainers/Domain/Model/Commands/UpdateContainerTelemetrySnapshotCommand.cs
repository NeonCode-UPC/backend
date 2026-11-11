using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Commands;

/// <summary>
///     Command to synchronize dynamic IoT telemetry sensor readings into the SmartContainer aggregate.
/// </summary>
public record UpdateContainerTelemetrySnapshotCommand(
    ContainerId ContainerId,
    decimal TemperatureCelsius,
    decimal BatteryPercentage,
    bool Is12vConnected,
    decimal NetWeightGrams,
    string? SignalQuality,
    DateTimeOffset TimestampUtc);
