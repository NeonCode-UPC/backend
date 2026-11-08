namespace MedicalSmartBox.API.Custody.Domain.Model.Events;

public sealed record CustodyEvent(string Title, string Detail, DateTimeOffset At);
