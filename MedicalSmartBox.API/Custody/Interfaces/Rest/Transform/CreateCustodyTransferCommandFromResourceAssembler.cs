using MedicalSmartBox.API.Custody.Domain.Model.Commands;
using MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Resources;

namespace MedicalSmartBox.API.Custody.Interfaces.Rest.Transform;

public static class CreateCustodyTransferCommandFromResourceAssembler
{
    public static CreateCustodyTransferCommand ToCommandFromResource(CreateCustodyTransferResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        var temperatureReadings = resource.TemperatureReadings?
            .Select(reading => new CustodyTemperatureReading(reading.RecordedAt, reading.TemperatureCelsius))
            .ToArray();

        return new CreateCustodyTransferCommand(
            resource.TransferCode,
            resource.TripId,
            resource.RecipientHospitalId,
            resource.AuthorizedRecipientUserId,
            resource.Route,
            resource.Cargo,
            resource.SmartBox,
            resource.TemperatureMax,
            resource.Openings,
            resource.DepartureAt,
            temperatureReadings);
    }
}
