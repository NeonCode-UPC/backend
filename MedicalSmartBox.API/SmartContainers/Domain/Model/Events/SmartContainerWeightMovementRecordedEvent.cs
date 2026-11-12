using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Events;

/// <summary>
///     Domain event emitted when biological payload weight change is recorded.
/// </summary>
public record SmartContainerWeightMovementRecordedEvent(
    ContainerId ContainerId,
    decimal NetWeightGrams,
    int UnitsDifference,
    string ResponsibleUser,
    DateTimeOffset OccurredOn);
