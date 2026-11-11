using MedicalSmartBox.API.Transport.Domain.Enums;

namespace MedicalSmartBox.API.Transport.Application.ReadModels;

public sealed record DispatchTripReadModel(
    Guid Id,
    Guid OrderId,
    string AssignedVehiclePlate,
    Guid AssignedContainerId,
    Guid AssignedDriverUserId,
    Guid AssignedParamedicUserId,
    TripStatus Status,
    DateTime ScheduledDepartureTime,
    DateTime? ActualDepartureTime,
    DateTime EstimatedArrivalTime,
    DateTime? ActualArrivalTime,
    double DistanceKilometers,
    int PlannedDurationMinutes,
    int CurrentCongestionDelayMinutes,
    string PolylineCoordinates,
    decimal DestinationLatitude,
    decimal DestinationLongitude,
    bool IsSeverelyDelayed);
