using MedicalSmartBox.API.Telemetry.Domain.Model.Aggregates;
using MedicalSmartBox.API.Telemetry.Interfaces.Rest.Resources;

namespace MedicalSmartBox.API.Telemetry.Interfaces.Rest.Transform;

/// <summary>
///     Assembler converting a TelemetryLog domain aggregate entity into an HTTP response resource.
/// </summary>
public static class TelemetryLogResourceFromEntityAssembler
{
    public static TelemetryLogResource ToResourceFromEntity(TelemetryLog entity)
    {
        return new TelemetryLogResource(
            entity.Id,
            entity.ContainerId,
            entity.TripId,
            entity.TimestampUtc,
            entity.TemperatureCelsius,
            entity.AmbientTemperatureCelsius,
            entity.WeightGrams,
            entity.BatteryPercentage,
            entity.Is12VConnected,
            entity.PeltierPowerPct,
            entity.LidLockEngaged,
            entity.Latitude,
            entity.Longitude,
            entity.FirmwareSignature,
            entity.IsThermalExcursion,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
