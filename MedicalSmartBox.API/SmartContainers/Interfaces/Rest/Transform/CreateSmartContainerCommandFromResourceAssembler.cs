using MedicalSmartBox.API.SmartContainers.Domain.Model.Commands;
using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;
using MedicalSmartBox.API.SmartContainers.Interfaces.Rest.Resources;

namespace MedicalSmartBox.API.SmartContainers.Interfaces.Rest.Transform;

/// <summary>
///     Transforms incoming REST payload into domain CreateSmartContainerCommand.
/// </summary>
public static class CreateSmartContainerCommandFromResourceAssembler
{
    public static CreateSmartContainerCommand ToCommandFromResource(CreateSmartContainerResource resource)
    {
        var formFactor = Enum.TryParse<EFormFactorContainer>(resource.FormFactor, true, out var parsed)
            ? parsed
            : EFormFactorContainer.StandardBox_20L;

        return new CreateSmartContainerCommand(
            SerialNumber: resource.SerialNumber,
            FormFactor: formFactor,
            TareWeightGrams: resource.TareWeightGrams,
            InitialTemperature: resource.InitialTemperature,
            MinTemperatureCelsius: resource.MinTemperatureCelsius,
            MaxTemperatureCelsius: resource.MaxTemperatureCelsius,
            WarningTemperatureCelsius: resource.WarningTemperatureCelsius,
            ExcursionToleranceSeconds: resource.ExcursionToleranceSeconds);
    }
}
