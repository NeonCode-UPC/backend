namespace MedicalSmartBox.API.alerting.Interfaces.REST.Resources;

public record ContingencyResolutionResource(Guid Id, string MitigationAction, string QualitySignoffNotes,
    bool WasThermalIntegrityRestored, Guid ResolvedByUserId, DateTimeOffset ResolvedAt);
