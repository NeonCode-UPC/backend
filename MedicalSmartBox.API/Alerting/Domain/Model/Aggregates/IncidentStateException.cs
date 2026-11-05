namespace MedicalSmartBox.API.alerting.Domain.Model.Aggregates;

public class IncidentStateException(string message) : InvalidOperationException(message);