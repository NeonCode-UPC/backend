namespace MedicalSmartBox.API.SmartContainers.Interfaces.Rest.Resources;

/// <summary>
///     REST presentation resource for SmartContainer representations.
///     Provides dual-attribute compatibility with both frontend Vue models and database columns.
/// </summary>
public record SmartContainerResource(
    int Id,
    string SerialNumber,
    string FormFactor,
    string Status,
    string ContainerStatus,
    string LockState,
    string? AmbulancePlate,
    string? AssignedAmbulancePlate,
    decimal CurrentTemperatureCelsius,
    decimal CurrentTemperature,
    decimal MinTemperatureCelsius,
    decimal MaxTemperatureCelsius,
    decimal TargetMinTemperature,
    decimal TargetMaxTemperature,
    decimal WarningTemperatureCelsius,
    decimal WarningTemperature,
    int ExcursionToleranceSeconds,
    decimal TareWeightGrams,
    decimal TareWeight,
    decimal NetWeightGrams,
    decimal NetWeight,
    decimal BatteryPercentage,
    decimal BatteryLevel,
    int BatteryWarningPercentage,
    string SignalQuality,
    bool Is12vConnected,
    string LidStatus,
    string Model,
    DateTimeOffset? LastTelemetryAt,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt);
