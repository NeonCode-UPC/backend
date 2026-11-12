using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MedicalSmartBox.API.alerting.Domain.Model.Enums;

namespace MedicalSmartBox.API.alerting.Interfaces.REST.Resources;

public class CreateIncidentResource
{
    [Required] public Guid? TripId { get; init; }
    [Required] public Guid? ContainerId { get; init; }
    [Required, EnumDataType(typeof(IncidentType)), JsonConverter(typeof(JsonStringEnumConverter))] public IncidentType? IncidentType { get; init; }
    [Required, EnumDataType(typeof(IncidentSeverity)), JsonConverter(typeof(JsonStringEnumConverter))] public IncidentSeverity? Severity { get; init; }
    public decimal? TriggerTemperatureCelsius { get; init; }
    [Range(0, 100)] public decimal? TriggerBatteryPercentage { get; init; }
}