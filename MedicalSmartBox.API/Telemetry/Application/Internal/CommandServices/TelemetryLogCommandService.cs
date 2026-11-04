using MedicalSmartBox.API.Shared.Application.Patterns;
using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.Telemetry.Application.CommandServices;
using MedicalSmartBox.API.Telemetry.Application.Errors;
using MedicalSmartBox.API.Telemetry.Domain.Model.Aggregates;
using MedicalSmartBox.API.Telemetry.Domain.Model.Commands;
using MedicalSmartBox.API.Telemetry.Domain.Repositories;

namespace MedicalSmartBox.API.Telemetry.Application.Internal.CommandServices;

/// <summary>
///     Command service handling creation and persistence of TelemetryLog snapshots.
/// </summary>
public class TelemetryLogCommandService(
    ITelemetryLogRepository telemetryLogRepository,
    IUnitOfWork unitOfWork,
    ILogger<TelemetryLogCommandService> logger)
    : ITelemetryLogCommandService
{
    public async Task<Result<TelemetryLog, CreateTelemetryLogError>> Handle(
        CreateTelemetryLogCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.ContainerId))
            return new Result<TelemetryLog, CreateTelemetryLogError>.Failure(
                CreateTelemetryLogError.InvalidContainerId);

        if (string.IsNullOrWhiteSpace(command.FirmwareSignature))
            return new Result<TelemetryLog, CreateTelemetryLogError>.Failure(
                CreateTelemetryLogError.InvalidFirmwareSignature);

        TelemetryLog telemetryLog;
        try
        {
            telemetryLog = new TelemetryLog(command);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Sensor payload validation failed for SmartContainer {ContainerId}",
                command.ContainerId);
            return new Result<TelemetryLog, CreateTelemetryLogError>.Failure(
                CreateTelemetryLogError.InvalidSensorPayload);
        }

        try
        {
            await telemetryLogRepository.AddAsync(telemetryLog, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            return new Result<TelemetryLog, CreateTelemetryLogError>.Success(telemetryLog);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while persisting telemetry log for SmartContainer {ContainerId}",
                command.ContainerId);
            return new Result<TelemetryLog, CreateTelemetryLogError>.Failure(
                CreateTelemetryLogError.UnexpectedError);
        }
    }
}
