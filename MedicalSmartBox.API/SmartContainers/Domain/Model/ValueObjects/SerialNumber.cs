namespace MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the physical chassis serial number burned into ESP32 firmware.
/// </summary>
public sealed record SerialNumber
{
    public const int MaxLength = 30;

    public string Value { get; }

    public SerialNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Serial number cannot be null or empty.", nameof(value));

        var trimmed = value.Trim().ToUpperInvariant();
        if (trimmed.Length > MaxLength)
            throw new ArgumentOutOfRangeException(nameof(value), $"Serial number cannot exceed {MaxLength} characters.");

        Value = trimmed;
    }

    public static implicit operator string(SerialNumber serial) => serial.Value;
    public static implicit operator SerialNumber(string value) => new(value);

    public override string ToString() => Value;
}
