using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Commands;

/// <summary>
///     Command to link a SmartContainer to an ambulance for dispatch transit.
/// </summary>
public record PairContainerToAmbulanceCommand(
    ContainerId ContainerId,
    string AmbulancePlate);
