using MedicalSmartBox.API.alerting.Domain.Model.Enums;

namespace MedicalSmartBox.API.alerting.Domain.Model.Commands;

public record CreateIncidentCommand(Guid TripId, Guid ContainerId, IncidentType IncidentType,
    IncidentSeverity Severity, decimal? TriggerTemperatureCelsius, decimal? TriggerBatteryPercentage);