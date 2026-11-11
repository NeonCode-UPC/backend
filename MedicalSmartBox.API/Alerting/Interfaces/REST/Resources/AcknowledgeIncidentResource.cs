using System.ComponentModel.DataAnnotations;

namespace MedicalSmartBox.API.alerting.Interfaces.REST.Resources;

public class AcknowledgeIncidentResource
{
    [Required] public Guid? OperatorId { get; init; }
}