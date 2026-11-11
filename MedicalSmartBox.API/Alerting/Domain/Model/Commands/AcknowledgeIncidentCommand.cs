namespace MedicalSmartBox.API.alerting.Domain.Model.Commands;

public record AcknowledgeIncidentCommand(Guid IncidentId, Guid OperatorId);