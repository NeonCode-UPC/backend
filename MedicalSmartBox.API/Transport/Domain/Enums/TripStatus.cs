namespace MedicalSmartBox.API.Transport.Domain.Enums;

public enum TripStatus
{
    Scheduled,
    ResourcesAssigned,
    PrecoolingVerified,
    InTransit,
    ArrivedAtDestination,
    Completed,
    Cancelled
}