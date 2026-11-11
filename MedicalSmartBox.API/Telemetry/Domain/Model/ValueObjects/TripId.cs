namespace MedicalSmartBox.API.Telemetry.Domain.Model.ValueObjects;

/// <summary>
///     Represents the strongly-typed identifier for a medical transport dispatch trip reference.
/// </summary>
/// <param name="Value">The alphanumeric dispatch trip code (e.g. TR-0417).</param>
public readonly record struct TripId(string Value)
{
    public static implicit operator string(TripId id) => id.Value;
    public static implicit operator TripId(string value) => new(value);

    public override string ToString() => Value;
}
