using MedicalSmartBox.API.Telemetry.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.Telemetry.Domain.Model.Queries;

/// <summary>
///     Query to retrieve all telemetry snapshots recorded for a specific dispatch trip.
/// </summary>
public record GetAllTelemetryLogsByTripIdQuery(TripId TripId)
{
    public GetAllTelemetryLogsByTripIdQuery(string tripId) : this(new TripId(tripId))
    {
    }
}
