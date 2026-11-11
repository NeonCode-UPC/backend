namespace MedicalSmartBox.API.SmartContainers.Interfaces.Rest.Resources;

/// <summary>
///     Payload to log a biological payload weight difference.
/// </summary>
public record RecordWeightMovementResource(
    decimal NetWeightGrams,
    int UnitsDifference,
    string? ResponsibleUser = "Clinical Pharmacist",
    string? Reason = "Routine cold chain supply dispatch");
