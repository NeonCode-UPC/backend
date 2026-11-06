using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.Telemetry.Domain.Model.Aggregates;
using MedicalSmartBox.API.Telemetry.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.Telemetry.Domain.Repositories;

/// <summary>
///     Repository contract for TelemetryLog aggregate queries and persistence.
/// </summary>
public interface ITelemetryLogRepository : IBaseRepository<TelemetryLog, int>
{
    Task<TelemetryLog?> FindLatestByContainerIdAsync(ContainerId containerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TelemetryLog>> FindByContainerIdAsync(ContainerId containerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TelemetryLog>> FindByTripIdAsync(TripId tripId, CancellationToken cancellationToken = default);
}
