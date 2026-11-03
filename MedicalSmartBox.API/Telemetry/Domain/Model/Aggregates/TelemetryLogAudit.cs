using MedicalSmartBox.API.Shared.Domain.Model;

namespace MedicalSmartBox.API.Telemetry.Domain.Model.Aggregates;

/// <summary>
///     Partial class implementing IAuditableEntity for automated audit timestamp tracking.
/// </summary>
public partial class TelemetryLog : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
