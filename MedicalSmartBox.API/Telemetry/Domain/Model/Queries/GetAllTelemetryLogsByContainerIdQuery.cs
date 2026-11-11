using MedicalSmartBox.API.Telemetry.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.Telemetry.Domain.Model.Queries;

/// <summary>
///     Query to retrieve all telemetry snapshots recorded for a specific SmartContainer.
/// </summary>
public record GetAllTelemetryLogsByContainerIdQuery(ContainerId ContainerId)
{
    public GetAllTelemetryLogsByContainerIdQuery(string containerId) : this(new ContainerId(containerId))
    {
    }
}
