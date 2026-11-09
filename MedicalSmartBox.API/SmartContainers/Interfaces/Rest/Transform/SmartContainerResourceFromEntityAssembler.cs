using MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;
using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;
using MedicalSmartBox.API.SmartContainers.Interfaces.Rest.Resources;

namespace MedicalSmartBox.API.SmartContainers.Interfaces.Rest.Transform;

/// <summary>
///     Transforms SmartContainer aggregate domain entities into REST representation resources.
/// </summary>
public static class SmartContainerResourceFromEntityAssembler
{
    public static SmartContainerResource ToResourceFromEntity(SmartContainer entity)
    {
        var model = entity.FormFactor == EFormFactorContainer.SmallBox_5L
            ? "Compact Medical SmartBox 5L"
            : "Standard Medical SmartBox 20L";

        var lidStatus = entity.LockState == ELockStateContainer.Locked ? "closed" : "open";
        var statusStr = entity.Status.ToString();

        return new SmartContainerResource(
            Id: entity.Id.Value,
            SerialNumber: entity.SerialNumber.Value,
            FormFactor: entity.FormFactor.ToString(),
            Status: statusStr,
            ContainerStatus: statusStr,
            LockState: entity.LockState.ToString(),
            AmbulancePlate: entity.AssignedAmbulancePlate,
            AssignedAmbulancePlate: entity.AssignedAmbulancePlate,
            CurrentTemperatureCelsius: entity.CurrentTemperatureCelsius,
            CurrentTemperature: entity.CurrentTemperatureCelsius,
            MinTemperatureCelsius: entity.Thresholds.MinTemperatureCelsius,
            MaxTemperatureCelsius: entity.Thresholds.MaxTemperatureCelsius,
            TargetMinTemperature: entity.Thresholds.MinTemperatureCelsius,
            TargetMaxTemperature: entity.Thresholds.MaxTemperatureCelsius,
            WarningTemperatureCelsius: entity.Thresholds.WarningTemperatureCelsius,
            WarningTemperature: entity.Thresholds.WarningTemperatureCelsius,
            ExcursionToleranceSeconds: entity.Thresholds.ExcursionToleranceSeconds,
            TareWeightGrams: entity.TareWeightGrams,
            TareWeight: Math.Round(entity.TareWeightGrams / 1000.00m, 2),
            NetWeightGrams: entity.NetWeightGrams,
            NetWeight: Math.Round(entity.NetWeightGrams / 1000.00m, 2),
            BatteryPercentage: entity.BatteryPercentage,
            BatteryLevel: entity.BatteryPercentage,
            BatteryWarningPercentage: entity.BatteryWarningPercentage,
            SignalQuality: entity.SignalQuality,
            Is12vConnected: entity.Is12VConnected,
            LidStatus: lidStatus,
            Model: model,
            LastTelemetryAt: entity.LastTelemetryAt,
            CreatedAt: entity.CreatedAt,
            UpdatedAt: entity.UpdatedAt);
    }
}
