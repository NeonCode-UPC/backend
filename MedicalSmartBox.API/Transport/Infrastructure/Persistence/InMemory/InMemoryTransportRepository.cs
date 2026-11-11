using System.Collections.Concurrent;
using MedicalSmartBox.API.Transport.Domain.Aggregates;
using MedicalSmartBox.API.Transport.Domain.Enums;
using MedicalSmartBox.API.Transport.Domain.Repositories;

namespace MedicalSmartBox.API.Transport.Infrastructure.Persistence.InMemory;

public sealed class InMemoryTransportRepository : ITransportRepository
{
    private readonly ConcurrentDictionary<Guid, DispatchTrip> _trips = new();
    private readonly ConcurrentDictionary<Guid, TransportOrder> _orders = new();

    public Task AddAsync(DispatchTrip entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _trips[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task<DispatchTrip?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _trips.TryGetValue(id, out var trip);
        return Task.FromResult(trip);
    }

    public void Update(DispatchTrip entity) => _trips[entity.Id] = entity;

    public void Remove(DispatchTrip entity) => _trips.TryRemove(entity.Id, out _);

    public Task<IEnumerable<DispatchTrip>> ListAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IEnumerable<DispatchTrip>>(_trips.Values.ToArray());
    }

    public Task<DispatchTrip?> GetTripByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        FindByIdAsync(id, cancellationToken);

    public Task<List<DispatchTrip>> GetActiveTripsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_trips.Values
            .Where(trip => trip.Status is not TripStatus.Completed and not TripStatus.Cancelled)
            .OrderBy(trip => trip.EstimatedArrivalTime)
            .ToList());
    }

    public Task<TransportOrder?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<List<TransportOrder>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_orders.Values.OrderByDescending(order => order.CreatedAt).ToList());
    }

    public Task AddOrderAsync(TransportOrder order, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }

    public void UpdateOrder(TransportOrder order) => _orders[order.Id] = order;

    public Task<bool> OrderCodeExistsAsync(string orderCode, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_orders.Values.Any(order =>
            string.Equals(order.OrderCode, orderCode, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<bool> TripExistsForOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_trips.Values.Any(trip => trip.OrderId == orderId));
    }

    public Task SaveTripAsync(DispatchTrip trip, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _trips[trip.Id] = trip;
        return Task.CompletedTask;
    }

    public Task SaveOrderAsync(TransportOrder order, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }
}
