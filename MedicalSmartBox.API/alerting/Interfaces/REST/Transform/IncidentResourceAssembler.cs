using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;
using MedicalSmartBox.API.alerting.Domain.Model.Commands;
using MedicalSmartBox.API.alerting.Interfaces.REST.Resources;

namespace MedicalSmartBox.API.alerting.Interfaces.REST.Transform;

public static class IncidentResourceAssembler
{
    public static CreateIncidentCommand ToCommand(CreateIncidentResource resource) => new(
        resource.TripId!.Value, resource.ContainerId!.Value, resource.IncidentType!.Value,
        resource.Severity!.Value, resource.TriggerTemperatureCelsius, resource.TriggerBatteryPercentage);

    public static IncidentResource ToResource(CriticalIncident incident) => new(
        incident.Id, incident.TripId, incident.ContainerId, incident.IncidentType, incident.Severity,
        incident.Status, incident.TriggerTemperatureCelsius, incident.TriggerBatteryPercentage,
        incident.EscalationLevel, incident.TriggeredAt, incident.AcknowledgedAt, incident.AcknowledgedByUserId);
}