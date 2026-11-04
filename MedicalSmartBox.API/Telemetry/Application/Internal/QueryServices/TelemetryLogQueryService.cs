using MedicalSmartBox.API.Telemetry.Application.QueryServices;
using MedicalSmartBox.API.Telemetry.Domain.Model.Aggregates;
using MedicalSmartBox.API.Telemetry.Domain.Model.Queries;
using MedicalSmartBox.API.Telemetry.Domain.Repositories;

namespace MedicalSmartBox.API.Telemetry.Application.Internal.QueryServices;

/// <summary>
///     Query service handling retrieval operations for TelemetryLog entries.
/// </summary>
public class TelemetryLogQueryService(ITelemetryLogRepository telemetryLogRepository)
    : ITelemetryLogQueryService
{
    public async Task<TelemetryLog?> Handle(GetTelemetryLogByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await telemetryLogRepository.FindByIdAsync(query.Id, cancellationToken);
    }

    public async Task<IEnumerable<TelemetryLog>> Handle(GetAllTelemetryLogsByContainerIdQuery query, CancellationToken cancellationToken = default)
    {
        return await telemetryLogRepository.FindByContainerIdAsync(query.ContainerId, cancellationToken);
    }

    public async Task<IEnumerable<TelemetryLog>> Handle(GetAllTelemetryLogsByTripIdQuery query, CancellationToken cancellationToken = default)
    {
        return await telemetryLogRepository.FindByTripIdAsync(query.TripId, cancellationToken);
    }

    public async Task<TelemetryLog?> Handle(GetLatestTelemetryLogByContainerIdQuery query, CancellationToken cancellationToken = default)
    {
        return await telemetryLogRepository.FindLatestByContainerIdAsync(query.ContainerId, cancellationToken);
    }
}
