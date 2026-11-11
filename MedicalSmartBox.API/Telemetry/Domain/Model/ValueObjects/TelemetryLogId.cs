namespace MedicalSmartBox.API.Telemetry.Domain.Model.ValueObjects;

/// <summary>
///     Represents the strongly-typed identifier for a TelemetryLog aggregate.
/// </summary>
/// <param name="Value">The underlying database identifier value.</param>
public readonly record struct TelemetryLogId(int Value)
{
    public static implicit operator int(TelemetryLogId id) => id.Value;
    public static implicit operator TelemetryLogId(int value) => new(value);

    public override string ToString() => Value.ToString();
}
