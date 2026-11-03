using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.Telemetry.Domain.Model.Aggregates;

namespace MedicalSmartBox.API.Telemetry.Domain.Repositories;

/// <summary>
///     Repository contract for TelemetryLog aggregate queries and persistence.
/// </summary>
public interface ITelemetryLogRepository : IBaseRepository<TelemetryLog>
{
    Task<TelemetryLog?> FindLatestByContainerIdAsync(string containerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TelemetryLog>> FindByContainerIdAsync(string containerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TelemetryLog>> FindByTripIdAsync(string tripId, CancellationToken cancellationToken = default);
}
