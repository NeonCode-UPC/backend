using MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.Custody.Interfaces.Rest.Resources;

public sealed record CreateCustodyTransferResource(
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
    IReadOnlyList<CustodyTemperatureReadingResource>? TemperatureReadings);

public sealed record RegisterTemperatureReadingsResource(
    IReadOnlyList<CustodyTemperatureReadingResource> Readings);

public sealed record PrepareReceptionResource(string Recipient, bool InsideDestinationGeofence);
public sealed record VerifyOtpResource(string Code);
public sealed record AcceptReceptionResource(string? RecipientNotes, bool HasUnresolvedCriticalAlerts = false);
public sealed record RejectDeliveryResource(string Reason);

public sealed record CustodyTemperatureReadingResource(
    DateTimeOffset RecordedAt,
    decimal TemperatureCelsius);
