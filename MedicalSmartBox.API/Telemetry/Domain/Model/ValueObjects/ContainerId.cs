namespace MedicalSmartBox.API.Telemetry.Domain.Model.ValueObjects;

/// <summary>
///     Represents the strongly-typed identifier for a SmartContainer reference in telemetry.
/// </summary>
/// <param name="Value">The alphanumeric container serial identifier (e.g. SB-0182).</param>
public readonly record struct ContainerId(string Value)
{
    public static implicit operator string(ContainerId id) => id.Value;
    public static implicit operator ContainerId(string value) => new(value);

    public override string ToString() => Value;
}
