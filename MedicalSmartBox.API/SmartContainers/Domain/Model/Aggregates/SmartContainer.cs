using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root representing a Medical SmartBox IoT container in the catalog.
/// </summary>
public class SmartContainer
{
    public SmartContainer(int id, SerialNumber serialNumber, string model, string status = "unlinked", double currentTemperature = 4.0, int batteryLevel = 100, string? ambulancePlate = null)
    {
        Id = id;
        SerialNumber = serialNumber;
        Model = string.IsNullOrWhiteSpace(model) ? "Standard Medical SmartBox 20L" : model;
        Status = status;
        CurrentTemperature = currentTemperature;
        BatteryLevel = batteryLevel;
        AmbulancePlate = ambulancePlate;
    }

    public int Id { get; private set; }

    public SerialNumber SerialNumber { get; private set; }

    public string Model { get; private set; }

    public string Status { get; private set; }

    public double CurrentTemperature { get; private set; }

    public int BatteryLevel { get; private set; }

    public string? AmbulancePlate { get; private set; }

    public void LinkToAmbulance(string plate)
    {
        AmbulancePlate = plate.Trim().ToUpperInvariant();
        Status = "online";
    }

    public void UnlinkFromAmbulance()
    {
        AmbulancePlate = null;
        Status = "unlinked";
    }
}
