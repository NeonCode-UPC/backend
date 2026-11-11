namespace MedicalSmartBox.API.alerting.Domain.Model.Commands;

public record ResolveIncidentCommand(Guid IncidentId, string MitigationAction, string QualitySignoffNotes,
    bool WasThermalIntegrityRestored, Guid ResolvedByUserId);
