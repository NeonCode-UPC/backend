namespace MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing a SmartBox hardware serial number (e.g., 'SB-0182').
/// </summary>
public sealed record SerialNumber
{
    public SerialNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Serial number cannot be empty.", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public string Value { get; }

    public override string ToString() => Value;
}
