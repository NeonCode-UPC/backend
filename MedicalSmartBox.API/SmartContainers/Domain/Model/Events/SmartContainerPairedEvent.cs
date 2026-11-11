using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Events;

/// <summary>
///     Domain event emitted when a SmartContainer is paired with an ambulance.
/// </summary>
public record SmartContainerPairedEvent(
    ContainerId ContainerId,
    string AmbulancePlate,
    DateTimeOffset OccurredOn);
