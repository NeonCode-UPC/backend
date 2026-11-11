namespace MedicalSmartBox.API.Transport.Application.Commands;

public sealed record CancelTransportOrderCommand(Guid OrderId, string Reason);
