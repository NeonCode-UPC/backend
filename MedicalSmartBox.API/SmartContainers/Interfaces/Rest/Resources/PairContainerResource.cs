namespace MedicalSmartBox.API.SmartContainers.Interfaces.Rest.Resources;

/// <summary>
///     Payload to link a SmartContainer to an ambulance.
/// </summary>
public record PairContainerResource(string AmbulancePlate);
