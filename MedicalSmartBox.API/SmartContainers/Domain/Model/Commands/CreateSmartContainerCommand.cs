using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Commands;

/// <summary>
///     Command to register a new SmartBox in the catalog.
/// </summary>
public record CreateSmartContainerCommand(SerialNumber SerialNumber, string Model, string? AmbulancePlate = null);
