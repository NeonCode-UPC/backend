using MedicalSmartBox.API.Telemetry.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.Telemetry.Domain.Model.Queries;

/// <summary>
///     Query to retrieve the most recent telemetry snapshot recorded for a specific SmartContainer.
/// </summary>
public record GetLatestTelemetryLogByContainerIdQuery(ContainerId ContainerId)
{
    public GetLatestTelemetryLogByContainerIdQuery(string containerId) : this(new ContainerId(containerId))
    {
    }
}
