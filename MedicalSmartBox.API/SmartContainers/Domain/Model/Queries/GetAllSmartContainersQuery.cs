namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Queries;

/// <summary>
///     Query to retrieve SmartContainer units with optional status or ambulance filtering.
/// </summary>
public record GetAllSmartContainersQuery(string? Status = null, string? AmbulancePlate = null);
