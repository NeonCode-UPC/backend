using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Commands;

/// <summary>
///     Command to register a new SmartContainer unit into the fleet inventory.
/// </summary>
public record CreateSmartContainerCommand(
    string SerialNumber,
    EFormFactorContainer FormFactor = EFormFactorContainer.StandardBox_20L,
    decimal TareWeightGrams = 10000.00m,
    decimal InitialTemperature = 4.00m,
    decimal MinTemperatureCelsius = ThermalThresholds.DefaultMinSafeTemp,
    decimal MaxTemperatureCelsius = ThermalThresholds.DefaultMaxSafeTemp,
    decimal WarningTemperatureCelsius = ThermalThresholds.DefaultWarningTemp,
    int ExcursionToleranceSeconds = ThermalThresholds.DefaultExcursionToleranceSeconds);
