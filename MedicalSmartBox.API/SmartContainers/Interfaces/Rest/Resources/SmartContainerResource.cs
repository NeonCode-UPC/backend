namespace MedicalSmartBox.API.SmartContainers.Interfaces.Rest.Resources;

/// <summary>
///     DTO resource representing a SmartBox container in API responses for the catalog view.
/// </summary>
public record SmartContainerResource(
    int Id,
    string SerialNumber,
    string Model,
    string Status,
    double CurrentTemperature,
    int BatteryLevel,
    string? AmbulancePlate);
