using MedicalSmartBox.API.Telemetry.Domain.Model.Aggregates;
using MedicalSmartBox.API.Telemetry.Domain.Model.Queries;

namespace MedicalSmartBox.API.Telemetry.Application.QueryServices;

/// <summary>
///     Application query service contract for TelemetryLog queries.
/// </summary>
public interface ITelemetryLogQueryService
{
    Task<TelemetryLog?> Handle(GetTelemetryLogByIdQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<TelemetryLog>> Handle(GetAllTelemetryLogsByContainerIdQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<TelemetryLog>> Handle(GetAllTelemetryLogsByTripIdQuery query, CancellationToken cancellationToken = default);
    Task<TelemetryLog?> Handle(GetLatestTelemetryLogByContainerIdQuery query, CancellationToken cancellationToken = default);
}
