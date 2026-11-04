using MedicalSmartBox.API.alerting.Domain.Model.Enums;
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
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

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
        Severity = severity;
        Status = IncidentStatus.Triggered;
        EscalationLevel = EscalationLevel.OperationalCabAlert;
        TriggerTemperatureCelsius = temperatureCelsius;
        TriggerBatteryPercentage = batteryPercentage;
        TriggeredAt = DateTimeOffset.UtcNow;
    }
}