using System.Text.Json.Serialization;
using MedicalSmartBox.API.Transport.Domain.Enums;

namespace MedicalSmartBox.API.Transport.Interfaces.Rest.Responses;

public sealed record DispatchTripResponse(
    Guid Id,
    Guid OrderId,
    string AssignedVehiclePlate,
    Guid AssignedContainerId,
    Guid AssignedDriverUserId,
    Guid AssignedParamedicUserId,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] TripStatus Status,
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
