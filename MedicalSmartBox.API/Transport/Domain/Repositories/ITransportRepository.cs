using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.Transport.Domain.Aggregates;

namespace MedicalSmartBox.API.Transport.Domain.Repositories;

public interface ITransportRepository : IBaseRepository<DispatchTrip, Guid>
{
    Task<DispatchTrip?> GetTripByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<DispatchTrip>> GetActiveTripsAsync(CancellationToken cancellationToken = default);
    Task<TransportOrder?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TransportOrder>> GetOrdersAsync(CancellationToken cancellationToken = default);
    Task AddOrderAsync(TransportOrder order, CancellationToken cancellationToken = default);
    void UpdateOrder(TransportOrder order);
    Task<bool> OrderCodeExistsAsync(string orderCode, CancellationToken cancellationToken = default);
    Task<bool> TripExistsForOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task SaveTripAsync(DispatchTrip trip, CancellationToken cancellationToken = default);
    Task SaveOrderAsync(TransportOrder order, CancellationToken cancellationToken = default);
}
