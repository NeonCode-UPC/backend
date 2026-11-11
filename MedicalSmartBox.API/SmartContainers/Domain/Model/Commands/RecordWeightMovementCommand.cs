using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Commands;

/// <summary>
///     Command to register a payload weight modification with custodial accountability.
/// </summary>
public record RecordWeightMovementCommand(
    ContainerId ContainerId,
    decimal NetWeightGrams,
    int UnitsDifference,
    string ResponsibleUser,
    string Reason);
