using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;
using MedicalSmartBox.API.alerting.Domain.Model.Commands;
using MedicalSmartBox.API.alerting.Interfaces.REST.Resources;

namespace MedicalSmartBox.API.alerting.Interfaces.REST.Transform;

public static class IncidentResourceAssembler
{
    public static CreateIncidentCommand ToCommand(CreateIncidentResource resource) => new(
        resource.TripId!.Value, resource.ContainerId!.Value, resource.IncidentType!.Value,
        resource.Severity!.Value, resource.TriggerTemperatureCelsius, resource.TriggerBatteryPercentage);

    public static AcknowledgeIncidentCommand ToAcknowledgeCommand(Guid id, AcknowledgeIncidentResource resource) =>
        new(id, resource.OperatorId!.Value);

    public static ResolveIncidentCommand ToResolveCommand(Guid id, ResolveIncidentResource resource) =>
        new(id, resource.MitigationAction, resource.QualitySignoffNotes,
            resource.WasThermalIntegrityRestored!.Value, resource.ResolvedByUserId!.Value);

    public static IncidentResource ToResource(CriticalIncident incident) => new(
        incident.Id, incident.TripId, incident.ContainerId, incident.IncidentType, incident.Severity,
        incident.Status, incident.TriggerTemperatureCelsius, incident.TriggerBatteryPercentage,
        incident.EscalationLevel, incident.TriggeredAt, incident.AcknowledgedAt, incident.AcknowledgedByUserId,
        incident.Resolution is null ? null : new ContingencyResolutionResource(incident.Resolution.Id,
            incident.Resolution.MitigationAction, incident.Resolution.QualitySignoffNotes,
            incident.Resolution.WasThermalIntegrityRestored, incident.Resolution.ResolvedByUserId, incident.Resolution.ResolvedAt),
        incident.TriggeredAt.AddMinutes(2),
        incident.AcknowledgedAt is null && incident.Status != MedicalSmartBox.API.alerting.Domain.Model.Enums.IncidentStatus.Resolved
            && DateTimeOffset.UtcNow >= incident.TriggeredAt.AddMinutes(2));
}