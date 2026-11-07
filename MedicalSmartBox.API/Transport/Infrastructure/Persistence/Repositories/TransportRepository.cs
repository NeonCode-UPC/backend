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
            .ToListAsync(cancellationToken);
    }
}