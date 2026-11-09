namespace MedicalSmartBox.API.SmartContainers.Interfaces.Rest.Resources;

/// <summary>
///     Request payload to register a new SmartContainer unit into inventory.
/// </summary>
public record CreateSmartContainerResource(
    string SerialNumber,
    string? FormFactor = "StandardBox_20L",
    decimal TareWeightGrams = 10000.00m,
    decimal InitialTemperature = 4.00m,
    decimal MinTemperatureCelsius = 2.00m,
    decimal MaxTemperatureCelsius = 8.00m,
    decimal WarningTemperatureCelsius = 7.50m,
    int ExcursionToleranceSeconds = 120);
