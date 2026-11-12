using MedicalSmartBox.API.Transport.Application.Queries;
using MedicalSmartBox.API.Transport.Application.QueryServices;
using MedicalSmartBox.API.Transport.Application.ReadModels;
using MedicalSmartBox.API.Transport.Domain.Aggregates;
using MedicalSmartBox.API.Transport.Domain.Repositories;

namespace MedicalSmartBox.API.Transport.Application.Internal.QueryServices;

public sealed class TransportQueryService(ITransportRepository repository) : ITransportQueryService
{
    public async Task<TransportOrderReadModel?> Handle(GetTransportOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var order = await repository.GetOrderByIdAsync(query.Id, cancellationToken);
        return order is null ? null : ToReadModel(order);
    }

    public async Task<IReadOnlyList<TransportOrderReadModel>> Handle(GetTransportOrdersQuery query, CancellationToken cancellationToken = default) =>
        (await repository.GetOrdersAsync(cancellationToken)).Select(ToReadModel).ToArray();

    public async Task<DispatchTripReadModel?> Handle(GetTripByIdQuery query, CancellationToken cancellationToken = default)
    {
        var trip = await repository.GetTripByIdAsync(query.Id, cancellationToken);
        return trip is null ? null : ToReadModel(trip);
    }

    public async Task<IReadOnlyList<DispatchTripReadModel>> Handle(GetActiveTripsQuery query, CancellationToken cancellationToken = default) =>
        (await repository.GetActiveTripsAsync(cancellationToken)).Select(ToReadModel).ToArray();

    private static TransportOrderReadModel ToReadModel(TransportOrder order) => new(
        order.Id, order.OrderCode, order.OriginHospitalId, order.DestinationHospitalId,
        order.Priority, order.Status, order.CargoType, order.CargoDescription,
        order.MaxIschemiaLimit.MaxSafeHours, order.MaxIschemiaLimit.WarningThresholdHours,
        order.CreatedByUserId, order.CreatedAt);

    private static DispatchTripReadModel ToReadModel(DispatchTrip trip) => new(
        trip.Id, trip.OrderId, trip.AssignedVehiclePlate, trip.AssignedContainerId,
        trip.AssignedDriverUserId, trip.AssignedParamedicUserId, trip.Status,
        trip.ScheduledDepartureTime, trip.ActualDepartureTime, trip.EstimatedArrivalTime,
        trip.ActualArrivalTime, trip.Route.DistanceKilometers, trip.Route.PlannedDurationMinutes,
        trip.Route.CurrentCongestionDelayMinutes, trip.Route.PolylineCoordinates,
        trip.DestinationLocation.Latitude, trip.DestinationLocation.Longitude,
        trip.Route.IsSeverelyDelayed());
}
