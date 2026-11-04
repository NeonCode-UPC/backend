using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using MedicalSmartBox.API.Telemetry.Domain.Model.Aggregates;
using MedicalSmartBox.API.Telemetry.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MedicalSmartBox.API.Telemetry.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core repository implementation for TelemetryLog queries and persistence.
/// </summary>
public class TelemetryLogRepository(AppDbContext context)
    : BaseRepository<TelemetryLog, int>(context), ITelemetryLogRepository
{
    public async Task<TelemetryLog?> FindLatestByContainerIdAsync(string containerId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TelemetryLog>()
            .Where(t => t.ContainerId == containerId)
            .OrderByDescending(t => t.TimestampUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TelemetryLog>> FindByContainerIdAsync(string containerId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TelemetryLog>()
            .Where(t => t.ContainerId == containerId)
            .OrderByDescending(t => t.TimestampUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TelemetryLog>> FindByTripIdAsync(string tripId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TelemetryLog>()
            .Where(t => t.TripId == tripId)
            .OrderByDescending(t => t.TimestampUtc)
            .ToListAsync(cancellationToken);
    }
}
