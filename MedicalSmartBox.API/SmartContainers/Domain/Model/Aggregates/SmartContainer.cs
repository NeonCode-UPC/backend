using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root representing a physical SmartContainer unit in the cold chain fleet.
///     Enforces hardware safety invariants, thermal boundaries, and payload weight tracking.
/// </summary>
public partial class SmartContainer
{
    protected SmartContainer()
    {
        SerialNumber = new SerialNumber("SMB-UNKNOWN");
        FormFactor = EFormFactorContainer.StandardBox_20L;
        Status = EContainerStatus.Available;
        LockState = ELockStateContainer.Unlocked;
        Thresholds = new ThermalThresholds();
        CurrentTemperatureCelsius = 4.00m;
        TareWeightGrams = 10000.00m;
        NetWeightGrams = 0.00m;
        BatteryPercentage = 100.00m;
        BatteryWarningPercentage = 20;
        SignalQuality = "strong";
        Is12VConnected = false;
    }

    public SmartContainer(
        string serialNumber,
        EFormFactorContainer formFactor = EFormFactorContainer.StandardBox_20L,
        decimal tareWeightGrams = 10000.00m,
        decimal initialTemperature = 4.00m,
        ThermalThresholds? thresholds = null)
    {
        SerialNumber = new SerialNumber(serialNumber);
        FormFactor = formFactor;
        Status = EContainerStatus.Available;
        LockState = ELockStateContainer.Unlocked;
        Thresholds = thresholds ?? new ThermalThresholds();
        CurrentTemperatureCelsius = initialTemperature;
        TareWeightGrams = tareWeightGrams >= 0m ? tareWeightGrams : throw new ArgumentOutOfRangeException(nameof(tareWeightGrams), "Tare weight cannot be negative.");
        NetWeightGrams = 0.00m;
        BatteryPercentage = 100.00m;
        BatteryWarningPercentage = 20;
        SignalQuality = "strong";
        Is12VConnected = false;
        AssignedAmbulancePlate = null;
    }

    public ContainerId Id { get; internal set; }
    public SerialNumber SerialNumber { get; private set; }
    public EFormFactorContainer FormFactor { get; private set; }
    public EContainerStatus Status { get; private set; }
    public ELockStateContainer LockState { get; private set; }
    public string? AssignedAmbulancePlate { get; private set; }
    public decimal CurrentTemperatureCelsius { get; private set; }
    public ThermalThresholds Thresholds { get; private set; }
    public decimal TareWeightGrams { get; private set; }
    public decimal NetWeightGrams { get; private set; }
    public decimal BatteryPercentage { get; private set; }
    public int BatteryWarningPercentage { get; private set; }
    public string SignalQuality { get; private set; }
    public bool Is12VConnected { get; private set; }
    public DateTimeOffset? LastTelemetryAt { get; private set; }

    public bool IsLinkedToAmbulance => !string.IsNullOrWhiteSpace(AssignedAmbulancePlate);

    public bool IsBatteryLow => BatteryPercentage <= BatteryWarningPercentage;

    public bool IsThermallySafe => Thresholds.IsSafe(CurrentTemperatureCelsius);

    public void PairToAmbulance(string ambulancePlate)
    {
        if (string.IsNullOrWhiteSpace(ambulancePlate))
            throw new ArgumentException("Ambulance plate cannot be empty.", nameof(ambulancePlate));

        if (IsLinkedToAmbulance && !string.Equals(AssignedAmbulancePlate, ambulancePlate.Trim().ToUpperInvariant(), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Container is already paired with ambulance '{AssignedAmbulancePlate}'. Unlink first.");

        AssignedAmbulancePlate = ambulancePlate.Trim().ToUpperInvariant();
        Status = EContainerStatus.InTransit;
    }

    public void UnlinkFromAmbulance()
    {
        AssignedAmbulancePlate = null;
        Status = EContainerStatus.Available;
    }

    public void CalibrateTare(decimal tareGrams)
    {
        if (tareGrams < 0m)
            throw new ArgumentOutOfRangeException(nameof(tareGrams), "Tare weight cannot be negative.");

        TareWeightGrams = tareGrams;
    }

    public void RecordWeightMovement(decimal netWeightGrams)
    {
        if (netWeightGrams < 0m)
            throw new ArgumentOutOfRangeException(nameof(netWeightGrams), "Net weight cannot be negative.");

        NetWeightGrams = netWeightGrams;
    }

    public void SetLockState(ELockStateContainer newState)
    {
        LockState = newState;
    }

    public void UpdateTelemetry(
        decimal temperatureCelsius,
        decimal batteryPercentage,
        bool is12vConnected,
        decimal netWeightGrams,
        string signalQuality,
        DateTimeOffset timestampUtc)
    {
        CurrentTemperatureCelsius = temperatureCelsius;
        BatteryPercentage = Math.Clamp(batteryPercentage, 0.00m, 100.00m);
        Is12VConnected = is12vConnected;
        NetWeightGrams = Math.Max(0.00m, netWeightGrams);
        if (!string.IsNullOrWhiteSpace(signalQuality))
            SignalQuality = signalQuality;
        LastTelemetryAt = timestampUtc;

        // Reactive status adjustment based on cold chain telemetry
        if (Thresholds.IsExcursion(temperatureCelsius))
        {
            Status = EContainerStatus.Warning;
        }
        else if (IsLinkedToAmbulance && Status == EContainerStatus.Warning)
        {
            Status = EContainerStatus.InTransit;
        }
    }
}
