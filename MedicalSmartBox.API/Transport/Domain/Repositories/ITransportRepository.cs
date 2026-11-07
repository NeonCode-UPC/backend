using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.Transport.Domain.Aggregates;

namespace MedicalSmartBox.API.Transport.Domain.Repositories;

public interface ITransportRepository : IBaseRepository<DispatchTrip, Guid>
{
    Task<List<DispatchTrip>> GetActiveTripsAsync(CancellationToken cancellationToken = default);
}