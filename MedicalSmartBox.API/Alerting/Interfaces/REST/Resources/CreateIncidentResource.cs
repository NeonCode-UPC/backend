using System.ComponentModel.DataAnnotations;
using MedicalSmartBox.API.alerting.Domain.Model.Enums;

namespace MedicalSmartBox.API.alerting.Interfaces.REST.Resources;

public class CreateIncidentResource
{
    [Required] public Guid? TripId { get; init; }
    [Required] public Guid? ContainerId { get; init; }
    [Required, EnumDataType(typeof(IncidentType))] public IncidentType? IncidentType { get; init; }
    [Required, EnumDataType(typeof(IncidentSeverity))] public IncidentSeverity? Severity { get; init; }
    public decimal? TriggerTemperatureCelsius { get; init; }
    [Range(0, 100)] public decimal? TriggerBatteryPercentage { get; init; }
}