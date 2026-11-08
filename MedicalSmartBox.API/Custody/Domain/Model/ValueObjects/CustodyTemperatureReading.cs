namespace MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;

/// <summary>Timestamped temperature evidence provided by the transport telemetry context.</summary>
public sealed record CustodyTemperatureReading(DateTimeOffset RecordedAt, decimal TemperatureCelsius);
