namespace MedicalSmartBox.API.Custody.Interfaces.Rest.Resources;

public sealed record CustodyTransferResource(
    string Id,
    Guid TransferId,
    Guid TripId,
    string Route,
    string Cargo,
    string DepartureAt,
    string? ReceivedAt,
    string? Recipient,
    string Status,
    string TemperatureMax,
    string TemperatureRange,
    int Openings,
    string SmartBox,
    int OtpAttempts,
    int OtpMaxAttempts,
    IReadOnlyList<CustodyEventResource> Events,
    IReadOnlyList<CustodyTemperatureReadingResource> TemperatureReadings);

public sealed record CustodyEventResource(
    string Date,
    string Time,
    string Title,
    string Detail,
    string Type);

public sealed record DigitalAuditManifestResource(
    string Id,
    Guid ManifestId,
    string TransferId,
    string SealedAt,
    string Hash,
    string Status,
    string Recipient,
    string TemperatureMax,
    int Openings,
    string Cargo,
    bool IsSealedAndImmutable,
    decimal AverageTemperatureCelsius,
    decimal MinTemperatureCelsius,
    decimal MaxTemperatureCelsius,
    int TotalExcursionSeconds,
    bool MinsaComplianceVerified,
    bool HashVerified,
    string? CloudStorageUrl);

public sealed record AuditEventResource(
    string Id,
    string Date,
    string Time,
    string User,
    string Event,
    string Resource);
