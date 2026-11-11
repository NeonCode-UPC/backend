namespace MedicalSmartBox.API.Transport.Application.Commands;

public sealed record AssignTripResourcesCommand(
    Guid OrderId,
    string VehiclePlate,
    Guid ContainerId,
    Guid DriverUserId,
    Guid ParamedicUserId,
    DateTime ScheduledDepartureTime,
    DateTime EstimatedArrivalTime,
    double DistanceKilometers,
    int PlannedDurationMinutes,
    string PolylineCoordinates,
    decimal DestinationLatitude,
    decimal DestinationLongitude);
