using MedicalSmartBox.API.Transport.Domain.Aggregates;
using MedicalSmartBox.API.Transport.Application.ReadModels;
using MedicalSmartBox.API.Transport.Interfaces.Rest.Responses;

namespace MedicalSmartBox.API.Transport.Interfaces.Rest.Transform;

public static class TransportResponseAssembler
{
    public static TransportOrderResponse ToResponse(TransportOrder order) => new(
        order.Id,
        order.OrderCode,
        order.OriginHospitalId,
        order.DestinationHospitalId,
        order.Priority,
        order.Status,
        order.CargoType,
        order.CargoDescription,
        order.MaxIschemiaLimit.MaxSafeHours,
        order.MaxIschemiaLimit.WarningThresholdHours,
        order.CreatedByUserId,
        order.CreatedAt);

    public static TransportOrderResponse ToResponse(TransportOrderReadModel order) => new(
        order.Id,
        order.OrderCode,
        order.OriginHospitalId,
        order.DestinationHospitalId,
        order.Priority,
        order.Status,
        order.CargoType,
        order.CargoDescription,
        order.MaxSafeHours,
        order.WarningThresholdHours,
        order.CreatedByUserId,
        order.CreatedAt);

    public static DispatchTripResponse ToResponse(DispatchTrip trip) => new(
        trip.Id,
        trip.OrderId,
        trip.AssignedVehiclePlate,
        trip.AssignedContainerId,
        trip.AssignedDriverUserId,
        trip.AssignedParamedicUserId,
        trip.Status,
        trip.ScheduledDepartureTime,
        trip.ActualDepartureTime,
        trip.EstimatedArrivalTime,
        trip.ActualArrivalTime,
        trip.Route.DistanceKilometers,
        trip.Route.PlannedDurationMinutes,
        trip.Route.CurrentCongestionDelayMinutes,
        trip.Route.PolylineCoordinates,
        trip.DestinationLocation.Latitude,
        trip.DestinationLocation.Longitude,
        trip.Route.IsSeverelyDelayed());

    public static DispatchTripResponse ToResponse(DispatchTripReadModel trip) => new(
        trip.Id,
        trip.OrderId,
        trip.AssignedVehiclePlate,
        trip.AssignedContainerId,
        trip.AssignedDriverUserId,
        trip.AssignedParamedicUserId,
        trip.Status,
        trip.ScheduledDepartureTime,
        trip.ActualDepartureTime,
        trip.EstimatedArrivalTime,
        trip.ActualArrivalTime,
        trip.DistanceKilometers,
        trip.PlannedDurationMinutes,
        trip.CurrentCongestionDelayMinutes,
        trip.PolylineCoordinates,
        trip.DestinationLatitude,
        trip.DestinationLongitude,
        trip.IsSeverelyDelayed);
}
