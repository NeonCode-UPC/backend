using System.Net.Http;
using System.Text.Json;
using MedicalSmartBox.API.Shared.Application.Patterns;
using MedicalSmartBox.API.Transport.Application.CommandServices;
using MedicalSmartBox.API.Transport.Application.Commands;
using MedicalSmartBox.API.Transport.Application.Errors;
using MedicalSmartBox.API.Transport.Domain.Aggregates;
using MedicalSmartBox.API.Transport.Domain.Entities;
using MedicalSmartBox.API.Transport.Domain.Enums;
using MedicalSmartBox.API.Transport.Domain.Repositories;
using MedicalSmartBox.API.Transport.Domain.ValueObjects;

namespace MedicalSmartBox.API.Transport.Application.Internal.CommandServices;

public sealed class TransportCommandService(
    ITransportRepository repository,
    ITrafficRoutingService trafficRoutingService)
    : ITransportCommandService
{
    public async Task<Result<TransportOrder, TransportError>> Handle(
        CreateTransportOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.OrderCode))
            return Failure<TransportOrder>(TransportError.InvalidRequest);

        var orderCode = command.OrderCode.Trim();
        if (await repository.OrderCodeExistsAsync(orderCode, cancellationToken))
            return Failure<TransportOrder>(TransportError.DuplicateOrderCode);

        try
        {
            var order = new TransportOrder(
                orderCode,
                command.OriginHospitalId,
                command.DestinationHospitalId,
                command.Priority,
                command.CargoType,
                command.CargoDescription,
                new IschemiaTimeLimit(command.MaxSafeHours, command.WarningThresholdHours),
                command.CreatedByUserId);

            await repository.AddOrderAsync(order, cancellationToken);
            await repository.SaveOrderAsync(order, cancellationToken);
            return new Result<TransportOrder, TransportError>.Success(order);
        }
        catch (ArgumentException)
        {
            return Failure<TransportOrder>(TransportError.InvalidRequest);
        }
    }

    public async Task<Result<DispatchTrip, TransportError>> Handle(
        AssignTripResourcesCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await repository.GetOrderByIdAsync(command.OrderId, cancellationToken);
        if (order is null)
            return Failure<DispatchTrip>(TransportError.NotFound);

        if (await repository.TripExistsForOrderAsync(command.OrderId, cancellationToken))
            return Failure<DispatchTrip>(TransportError.OrderAlreadyHasTrip);

        try
        {
            var route = new TransportRoute(
                command.DistanceKilometers,
                command.PlannedDurationMinutes,
                command.PolylineCoordinates);

            if (!order.ValidateIschemiaFeasibility(
                    route.PlannedDurationMinutes + route.CurrentCongestionDelayMinutes))
                return Failure<DispatchTrip>(TransportError.IschemiaLimitExceeded);

            var trip = new DispatchTrip(
                command.OrderId,
                command.ScheduledDepartureTime,
                command.EstimatedArrivalTime,
                route,
                new GeoLocation(command.DestinationLatitude, command.DestinationLongitude));
            trip.AssignCrewAndResources(
                command.VehiclePlate,
                command.ContainerId,
                command.DriverUserId,
                command.ParamedicUserId);
            order.MarkAssigned();

            await repository.AddAsync(trip, cancellationToken);
            repository.UpdateOrder(order);
            await repository.SaveTripAsync(trip, cancellationToken);
            return new Result<DispatchTrip, TransportError>.Success(trip);
        }
        catch (ArgumentException)
        {
            return Failure<DispatchTrip>(TransportError.InvalidRequest);
        }
        catch (InvalidOperationException)
        {
            return Failure<DispatchTrip>(TransportError.InvalidState);
        }
    }

    public async Task<Result<DispatchTrip, TransportError>> Handle(
        StartTripCommand command,
        CancellationToken cancellationToken = default)
    {
        var trip = await repository.GetTripByIdAsync(command.TripId, cancellationToken);
        if (trip is null)
            return Failure<DispatchTrip>(TransportError.NotFound);

        var order = await repository.GetOrderByIdAsync(trip.OrderId, cancellationToken);
        if (order is null)
            return Failure<DispatchTrip>(TransportError.NotFound);

        if (order.Status != TransportOrderStatus.Assigned)
            return Failure<DispatchTrip>(TransportError.InvalidState);

        var result = trip.StartTrip(command.ContainerPrecooled);
        if (result is Result<bool, string>.Failure)
            return Failure<DispatchTrip>(TransportError.InvalidState);

        try
        {
            order.MarkInTransit();
        }
        catch (InvalidOperationException)
        {
            return Failure<DispatchTrip>(TransportError.InvalidState);
        }

        repository.Update(trip);
        repository.UpdateOrder(order);
        await repository.SaveTripAsync(trip, cancellationToken);
        return new Result<DispatchTrip, TransportError>.Success(trip);
    }

    public async Task<Result<DispatchTrip, TransportError>> Handle(
        UpdateDynamicEtaCommand command,
        CancellationToken cancellationToken = default)
    {
        var trip = await repository.GetTripByIdAsync(command.TripId, cancellationToken);
        if (trip is null)
            return Failure<DispatchTrip>(TransportError.NotFound);

        if (trip.Status != TripStatus.InTransit)
            return Failure<DispatchTrip>(TransportError.InvalidState);

        try
        {
            var origin = new GeoLocation(command.OriginLatitude, command.OriginLongitude);
            var eta = await trafficRoutingService.GetDynamicEtaAsync(
                origin, trip.DestinationLocation, cancellationToken);
            var delay = await trafficRoutingService.CheckTrafficDelaysAsync(
                trip.Route.PolylineCoordinates, cancellationToken);

            var order = await repository.GetOrderByIdAsync(trip.OrderId, cancellationToken);
            if (order is null)
                return Failure<DispatchTrip>(TransportError.NotFound);

            var departureTime = trip.ActualDepartureTime ?? DateTime.UtcNow;
            var estimatedMinutes = (int)Math.Ceiling((eta - departureTime).TotalMinutes);
            if (!order.ValidateIschemiaFeasibility(estimatedMinutes))
                return Failure<DispatchTrip>(TransportError.IschemiaLimitExceeded);

            trip.UpdateDynamicEta(eta);
            trip.RecalculateRoute(delay);
            repository.Update(trip);
            await repository.SaveTripAsync(trip, cancellationToken);
            return new Result<DispatchTrip, TransportError>.Success(trip);
        }
        catch (ArgumentException)
        {
            return Failure<DispatchTrip>(TransportError.InvalidRequest);
        }
        catch (InvalidOperationException)
        {
            return Failure<DispatchTrip>(TransportError.InvalidState);
        }
        catch (HttpRequestException)
        {
            return Failure<DispatchTrip>(TransportError.TrafficServiceUnavailable);
        }
        catch (JsonException)
        {
            return Failure<DispatchTrip>(TransportError.TrafficServiceUnavailable);
        }
    }

    public async Task<Result<DispatchTrip, TransportError>> Handle(
        RegisterHospitalArrivalCommand command,
        CancellationToken cancellationToken = default)
    {
        var trip = await repository.GetTripByIdAsync(command.TripId, cancellationToken);
        if (trip is null)
            return Failure<DispatchTrip>(TransportError.NotFound);

        GeoLocation location;
        try
        {
            location = new GeoLocation(command.CurrentLatitude, command.CurrentLongitude);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Failure<DispatchTrip>(TransportError.InvalidRequest);
        }

        var result = trip.RegisterHospitalArrival(location);
        if (result is Result<bool, string>.Failure)
            return Failure<DispatchTrip>(TransportError.InvalidState);

        repository.Update(trip);
        await repository.SaveTripAsync(trip, cancellationToken);
        return new Result<DispatchTrip, TransportError>.Success(trip);
    }

    public async Task<Result<DispatchTrip, TransportError>> Handle(
        CompleteTripCommand command,
        CancellationToken cancellationToken = default)
    {
        var trip = await repository.GetTripByIdAsync(command.TripId, cancellationToken);
        if (trip is null)
            return Failure<DispatchTrip>(TransportError.NotFound);

        var order = await repository.GetOrderByIdAsync(trip.OrderId, cancellationToken);
        if (order is null)
            return Failure<DispatchTrip>(TransportError.NotFound);

        if (order.Status != TransportOrderStatus.InTransit)
            return Failure<DispatchTrip>(TransportError.InvalidState);

        try
        {
            trip.MarkCompleted();
            order.MarkCompleted();
        }
        catch (InvalidOperationException)
        {
            return Failure<DispatchTrip>(TransportError.InvalidState);
        }

        repository.Update(trip);
        repository.UpdateOrder(order);
        await repository.SaveTripAsync(trip, cancellationToken);
        return new Result<DispatchTrip, TransportError>.Success(trip);
    }

    public async Task<Result<TransportOrder, TransportError>> Handle(
        CancelTransportOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await repository.GetOrderByIdAsync(command.OrderId, cancellationToken);
        if (order is null)
            return Failure<TransportOrder>(TransportError.NotFound);

        if (await repository.TripExistsForOrderAsync(command.OrderId, cancellationToken))
            return Failure<TransportOrder>(TransportError.InvalidState);

        try
        {
            order.CancelOrder(command.Reason);
        }
        catch (ArgumentException)
        {
            return Failure<TransportOrder>(TransportError.InvalidRequest);
        }
        catch (InvalidOperationException)
        {
            return Failure<TransportOrder>(TransportError.InvalidState);
        }

        repository.UpdateOrder(order);
        await repository.SaveOrderAsync(order, cancellationToken);
        return new Result<TransportOrder, TransportError>.Success(order);
    }

    private static Result<TEntity, TransportError>.Failure Failure<TEntity>(TransportError error) => new(error);
}
