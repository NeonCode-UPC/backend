using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Queries;

/// <summary>
///     Query to retrieve a single SmartContainer by its hardware serial number.
/// </summary>
public record GetSmartContainerBySerialNumberQuery(SerialNumber SerialNumber);
