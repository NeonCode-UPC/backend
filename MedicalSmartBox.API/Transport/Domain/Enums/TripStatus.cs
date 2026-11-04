namespace MedicalSmartBox.API.Transport.Domain.Enums;

public enum TripStatus
{
    Scheduled,
    ResourceAssigned,
    PrecoolingVerified,
    InTransit,
    ArrivedDestination,
    Completed,
    Cancelled
}