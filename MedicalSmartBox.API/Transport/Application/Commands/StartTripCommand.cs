namespace MedicalSmartBox.API.Transport.Application.Commands;

public sealed record StartTripCommand(Guid TripId, bool ContainerPrecooled);
