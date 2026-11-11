using System.ComponentModel.DataAnnotations;

namespace MedicalSmartBox.API.alerting.Interfaces.REST.Resources;

public class ResolveIncidentResource
{
    [Required, StringLength(500, MinimumLength = 1)] public string MitigationAction { get; init; } = string.Empty;
    [Required] public string QualitySignoffNotes { get; init; } = string.Empty;
    [Required] public bool? WasThermalIntegrityRestored { get; init; }
    [Required] public Guid? ResolvedByUserId { get; init; }
}
