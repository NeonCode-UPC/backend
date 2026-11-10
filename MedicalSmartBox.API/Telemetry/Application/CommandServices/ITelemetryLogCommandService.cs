using MedicalSmartBox.API.Shared.Application.Patterns;
using MedicalSmartBox.API.Telemetry.Application.Errors;
using MedicalSmartBox.API.Telemetry.Domain.Model.Aggregates;
using MedicalSmartBox.API.Telemetry.Domain.Model.Commands;

namespace MedicalSmartBox.API.Telemetry.Application.CommandServices;

/// <summary>
///     Application command service contract for TelemetryLog operations.
/// </summary>
public interface ITelemetryLogCommandService
{
    Task<Result<TelemetryLog, CreateTelemetryLogError>> Handle(
        CreateTelemetryLogCommand command,
        CancellationToken cancellationToken = default);
}
