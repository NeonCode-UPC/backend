using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Events;

/// <summary>
///     Domain event emitted when a new SmartContainer is provisioned.
/// </summary>
public record SmartContainerCreatedEvent(
    ContainerId ContainerId,
    SerialNumber SerialNumber,
    EFormFactorContainer FormFactor,
    DateTimeOffset OccurredOn);
