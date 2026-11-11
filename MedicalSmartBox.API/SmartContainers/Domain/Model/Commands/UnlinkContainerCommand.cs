using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Commands;

/// <summary>
///     Command to decouple a SmartContainer from its assigned ambulance.
/// </summary>
public record UnlinkContainerCommand(ContainerId ContainerId);
