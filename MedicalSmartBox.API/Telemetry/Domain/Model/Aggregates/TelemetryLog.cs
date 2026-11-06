using MedicalSmartBox.API.Telemetry.Domain.Model.Commands;

namespace MedicalSmartBox.API.Telemetry.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root persisting periodic IoT telemetry sensor snapshots
///     in the <c>telemetry_logs</c> time-series table.
/// </summary>
public partial class TelemetryLog
{
    private const decimal DefaultMinSafeTempCelsius = 2.00m;
    private const decimal DefaultMaxSafeTempCelsius = 8.00m;
    private const int MaxSignatureLength = 128;

    /// <summary>
    ///     Parameterless constructor for EF Core materialization.
    /// </summary>
    protected TelemetryLog()
    {
        ContainerId = string.Empty;
        FirmwareSignature = string.Empty;
    }

    public TelemetryLog(CreateTelemetryLogCommand command)
        : this(
            command.ContainerId,
            command.TripId,
            command.TimestampUtc,
            command.TemperatureCelsius,
            command.AmbientTemperatureCelsius,
            command.WeightGrams,
            command.BatteryPercentage,
            command.Is12VConnected,
            command.PeltierPowerPct,
            command.LidLockEngaged,
            command.Latitude,
            command.Longitude,
            command.FirmwareSignature)
    {
    }

    public TelemetryLog(
        string containerId,
        string? tripId,
        DateTime timestampUtc,
        decimal temperatureCelsius,
        decimal ambientTemperatureCelsius,
        decimal weightGrams,
        decimal batteryPercentage,
        bool is12VConnected,
        int peltierPowerPct,
        bool lidLockEngaged,
        decimal? latitude,
        decimal? longitude,
        string firmwareSignature)
    {
        if (string.IsNullOrWhiteSpace(containerId))
            throw new ArgumentException("ContainerId is required.", nameof(containerId));

        if (weightGrams < 0m)
            throw new ArgumentOutOfRangeException(nameof(weightGrams), "Weight cannot be negative.");

        if (batteryPercentage is < 0.00m or > 100.00m)
            throw new ArgumentOutOfRangeException(nameof(batteryPercentage), "Battery percentage must be between 0 and 100.");

        if (peltierPowerPct is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(peltierPowerPct), "Peltier power percentage must be between 0 and 100.");

        if (latitude is < -90.0m or > 90.0m)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90.0 and 90.0.");

        if (longitude is < -180.0m or > 180.0m)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180.0 and 180.0.");

        if (string.IsNullOrWhiteSpace(firmwareSignature) || firmwareSignature.Length > MaxSignatureLength)
            throw new ArgumentException($"FirmwareSignature is required and cannot exceed {MaxSignatureLength} characters.", nameof(firmwareSignature));

        ContainerId = containerId.Trim();
        TripId = string.IsNullOrWhiteSpace(tripId) ? null : tripId.Trim();
        TimestampUtc = DateTime.SpecifyKind(timestampUtc, DateTimeKind.Utc);
        TemperatureCelsius = temperatureCelsius;
        AmbientTemperatureCelsius = ambientTemperatureCelsius;
        WeightGrams = weightGrams;
        BatteryPercentage = batteryPercentage;
        Is12VConnected = is12VConnected;
        PeltierPowerPct = peltierPowerPct;
        LidLockEngaged = lidLockEngaged;
        Latitude = latitude;
        Longitude = longitude;
        FirmwareSignature = firmwareSignature.Trim();
        IsThermalExcursion = IsExcursion(DefaultMinSafeTempCelsius, DefaultMaxSafeTempCelsius);
    }

    public int Id { get; }
    public string ContainerId { get; private set; }
    public string? TripId { get; private set; }
    public DateTime TimestampUtc { get; private set; }
    public decimal TemperatureCelsius { get; private set; }
    public decimal AmbientTemperatureCelsius { get; private set; }
    public decimal WeightGrams { get; private set; }
    public decimal BatteryPercentage { get; private set; }
    public bool Is12VConnected { get; private set; }
    public int PeltierPowerPct { get; private set; }
    public bool LidLockEngaged { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public string FirmwareSignature { get; private set; }
    public bool IsThermalExcursion { get; private set; }

    public bool IsExcursion(decimal minTemp = DefaultMinSafeTempCelsius, decimal maxTemp = DefaultMaxSafeTempCelsius) =>
        TemperatureCelsius < minTemp || TemperatureCelsius > maxTemp;

    public bool ValidateIntegrity() =>
        !string.IsNullOrWhiteSpace(ContainerId) &&
        !string.IsNullOrWhiteSpace(FirmwareSignature) &&
        FirmwareSignature.Length <= MaxSignatureLength &&
        BatteryPercentage is >= 0.00m and <= 100.00m &&
        PeltierPowerPct is >= 0 and <= 100 &&
        WeightGrams >= 0.00m;
}
