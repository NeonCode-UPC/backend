namespace MedicalSmartBox.API.Telemetry.Application.Errors;

/// <summary>
///     Domain errors returned when recording a telemetry snapshot.
/// </summary>
public enum CreateTelemetryLogError
{
    InvalidContainerId,
    InvalidFirmwareSignature,
    InvalidSensorPayload,
    UnexpectedError
}
