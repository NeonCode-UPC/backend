using MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.Custody.Domain.Model.Commands;

public sealed record CreateCustodyTransferCommand(
    string TransferCode,
    Guid TripId,
    Guid RecipientHospitalId,
    Guid AuthorizedRecipientUserId,
    string Route,
    string Cargo,
    string SmartBox,
    decimal TemperatureMax,
    int Openings,
    DateTimeOffset DepartureAt,
    IReadOnlyList<CustodyTemperatureReading>? TemperatureReadings = null);
