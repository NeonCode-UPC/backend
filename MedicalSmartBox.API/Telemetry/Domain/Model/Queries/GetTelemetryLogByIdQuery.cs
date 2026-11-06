using MedicalSmartBox.API.Telemetry.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.Telemetry.Domain.Model.Queries;

/// <summary>
///     Query to retrieve a telemetry snapshot by its strongly-typed identifier.
/// </summary>
public record GetTelemetryLogByIdQuery(TelemetryLogId Id)
{
    public GetTelemetryLogByIdQuery(int id) : this(new TelemetryLogId(id))
    {
    }
}
