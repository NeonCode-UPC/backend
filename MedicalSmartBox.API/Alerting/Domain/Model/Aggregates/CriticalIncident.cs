using MedicalSmartBox.API.alerting.Domain.Model.Enums;
using MedicalSmartBox.API.alerting.Domain.Model.Entities;
using MedicalSmartBox.API.Shared.Domain.Model;

namespace MedicalSmartBox.API.alerting.Domain.Model.Aggregates;

public class CriticalIncident : IAuditableEntity
{
    public Guid Id { get; private set; }
    public Guid TripId { get; private set; }
    public Guid ContainerId { get; private set; }
    public IncidentType IncidentType { get; private set; }
    public IncidentSeverity Severity { get; private set; }
    public IncidentStatus Status { get; private set; }
    public decimal? TriggerTemperatureCelsius { get; private set; }
    public decimal? TriggerBatteryPercentage { get; private set; }
    public EscalationLevel EscalationLevel { get; private set; }
    public DateTimeOffset TriggeredAt { get; private set; }
    public DateTimeOffset? AcknowledgedAt { get; private set; }
    public Guid? AcknowledgedByUserId { get; private set; }
    public ContingencyResolution? Resolution { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public void Acknowledge(Guid operatorId)
    {
        if (operatorId == Guid.Empty) throw new ArgumentException("Operator ID is required.", nameof(operatorId));
        if (Status == IncidentStatus.Acknowledged) return;
        if (Status != IncidentStatus.Triggered && Status != IncidentStatus.Escalated)
            throw new IncidentStateException("Only active incidents can be acknowledged.");

        Status = IncidentStatus.Acknowledged;
        AcknowledgedAt = DateTimeOffset.UtcNow;
        AcknowledgedByUserId = operatorId;
    }

    public void EscalateToMedicalDirector()
    {
        if (Status == IncidentStatus.Resolved)
            throw new IncidentStateException("Resolved incidents cannot be escalated.");
        EscalationLevel = EscalationLevel.MedicalDirectorEscalation;
        Status = IncidentStatus.Escalated;
    }

    public void Resolve(string mitigationAction, string qualitySignoffNotes, bool wasThermalIntegrityRestored, Guid resolvedByUserId)
    {
        if (Status == IncidentStatus.Resolved)
            throw new IncidentStateException("Incident is already resolved.");
        Resolution = new ContingencyResolution(Id, mitigationAction, qualitySignoffNotes,
            wasThermalIntegrityRestored, resolvedByUserId);
        Status = IncidentStatus.Resolved;
    }
    private CriticalIncident() { }

    public CriticalIncident(Guid tripId, Guid containerId, IncidentType incidentType,
        IncidentSeverity severity, decimal? temperatureCelsius = null, decimal? batteryPercentage = null)
    {
        if (tripId == Guid.Empty) throw new ArgumentException("Trip ID is required.", nameof(tripId));
        if (containerId == Guid.Empty) throw new ArgumentException("Container ID is required.", nameof(containerId));
        if (!Enum.IsDefined(incidentType)) throw new ArgumentException("Invalid incident type.", nameof(incidentType));
        if (!Enum.IsDefined(severity)) throw new ArgumentException("Invalid incident severity.", nameof(severity));
        if (batteryPercentage is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(batteryPercentage));

        Id = Guid.NewGuid();
        TripId = tripId;
        ContainerId = containerId;
        IncidentType = incidentType;
        Severity = severity; Status = IncidentStatus.Triggered;
        EscalationLevel = EscalationLevel.OperationalCabAlert;
        TriggerTemperatureCelsius = temperatureCelsius;
        TriggerBatteryPercentage = batteryPercentage;
        TriggeredAt = DateTimeOffset.UtcNow;
    }
}