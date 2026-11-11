namespace MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

/// <summary>
///     Strongly-typed identifier for the SmartContainer aggregate root.
/// </summary>
/// <param name="Value">The underlying monotonic database identifier.</param>
public readonly record struct ContainerId(int Value)
{
    public static implicit operator int(ContainerId id) => id.Value;
    public static implicit operator ContainerId(int value) => new(value);

    public override string ToString() => Value.ToString();
}
