using MedicalSmartBox.API.alerting.Domain.Model.Enums;

namespace MedicalSmartBox.API.alerting.Interfaces.REST.Resources;

public record IncidentResource(Guid Id, Guid TripId, Guid ContainerId, IncidentType IncidentType,
    IncidentSeverity Severity, IncidentStatus Status, decimal? TriggerTemperatureCelsius,
    decimal? TriggerBatteryPercentage, EscalationLevel EscalationLevel, DateTimeOffset TriggeredAt,
    DateTimeOffset? AcknowledgedAt, Guid? AcknowledgedByUserId, ContingencyResolutionResource? Resolution, DateTimeOffset AcknowledgementDueAt, bool IsAcknowledgementOverdue);