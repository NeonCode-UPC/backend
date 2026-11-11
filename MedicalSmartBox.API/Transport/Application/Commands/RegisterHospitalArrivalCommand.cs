namespace MedicalSmartBox.API.Transport.Application.Commands;

public sealed record RegisterHospitalArrivalCommand(
    Guid TripId,
    decimal CurrentLatitude,
    decimal CurrentLongitude);
