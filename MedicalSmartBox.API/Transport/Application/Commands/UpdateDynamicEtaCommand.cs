namespace MedicalSmartBox.API.Transport.Application.Commands;

public sealed record UpdateDynamicEtaCommand(
    Guid TripId,
    decimal OriginLatitude,
    decimal OriginLongitude);
