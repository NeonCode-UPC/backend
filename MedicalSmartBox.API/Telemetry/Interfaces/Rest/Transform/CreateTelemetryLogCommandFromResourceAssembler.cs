using MedicalSmartBox.API.Telemetry.Domain.Model.Commands;
using MedicalSmartBox.API.Telemetry.Interfaces.Rest.Resources;

namespace MedicalSmartBox.API.Telemetry.Interfaces.Rest.Transform;

/// <summary>
///     Assembler transforming a CreateTelemetryLogResource payload into a domain CreateTelemetryLogCommand.
/// </summary>
public static class CreateTelemetryLogCommandFromResourceAssembler
{
    public static CreateTelemetryLogCommand ToCommandFromResource(CreateTelemetryLogResource resource)
    {
        return new CreateTelemetryLogCommand(
            resource.ContainerId,
            resource.TripId,
            resource.TimestampUtc ?? DateTime.UtcNow,
            resource.TemperatureCelsius,
            resource.AmbientTemperatureCelsius,
            resource.WeightGrams,
            resource.BatteryPercentage,
            resource.Is12VConnected,
            resource.PeltierPowerPct,
            resource.LidLockEngaged,
            resource.Latitude,
            resource.Longitude,
            resource.FirmwareSignature);
    }
}
