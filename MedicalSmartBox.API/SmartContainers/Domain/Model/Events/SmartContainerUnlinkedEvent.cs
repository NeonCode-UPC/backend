using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Events;

/// <summary>
///     Domain event emitted when a SmartContainer is unlinked from an ambulance.
/// </summary>
public record SmartContainerUnlinkedEvent(
    ContainerId ContainerId,
    DateTimeOffset OccurredOn);
