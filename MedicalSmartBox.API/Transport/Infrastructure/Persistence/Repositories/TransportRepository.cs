using Microsoft.EntityFrameworkCore;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using MedicalSmartBox.API.Transport.Domain.Aggregates;
using MedicalSmartBox.API.Transport.Domain.Enums;
using MedicalSmartBox.API.Transport.Domain.Repositories;

namespace MedicalSmartBox.API.Transport.Infrastructure.Persistence.Repositories;

public class TransportRepository(AppDbContext context)
    : BaseRepository<DispatchTrip, Guid>(context), ITransportRepository
{
    public async Task<List<DispatchTrip>> GetActiveTripsAsync(
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<DispatchTrip>()
            .Where(trip =>
                trip.Status != TripStatus.Completed &&
                trip.Status != TripStatus.Cancelled)
            .OrderBy(trip => trip.EstimatedArrivalTime)
            .ToListAsync(cancellationToken);
    }

    public Task<DispatchTrip?> GetTripByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Set<DispatchTrip>().FirstOrDefaultAsync(trip => trip.Id == id, cancellationToken);

    public Task<TransportOrder?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Set<TransportOrder>().FirstOrDefaultAsync(order => order.Id == id, cancellationToken);

    public Task<List<TransportOrder>> GetOrdersAsync(CancellationToken cancellationToken = default) =>
        Context.Set<TransportOrder>()
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddOrderAsync(TransportOrder order, CancellationToken cancellationToken = default) =>
        await Context.Set<TransportOrder>().AddAsync(order, cancellationToken);

    public void UpdateOrder(TransportOrder order) => Context.Set<TransportOrder>().Update(order);

    public Task<bool> OrderCodeExistsAsync(string orderCode, CancellationToken cancellationToken = default) =>
        Context.Set<TransportOrder>().AnyAsync(order => order.OrderCode == orderCode, cancellationToken);

    public Task<bool> TripExistsForOrderAsync(Guid orderId, CancellationToken cancellationToken = default) =>
        Context.Set<DispatchTrip>().AnyAsync(trip => trip.OrderId == orderId, cancellationToken);

    public Task SaveTripAsync(DispatchTrip trip, CancellationToken cancellationToken = default)
    {
        if (Context.Entry(trip).State == EntityState.Detached)
            Context.Update(trip);

        return Context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveOrderAsync(TransportOrder order, CancellationToken cancellationToken = default)
    {
        if (Context.Entry(order).State == EntityState.Detached)
            Context.Update(order);

        return Context.SaveChangesAsync(cancellationToken);
    }
}
