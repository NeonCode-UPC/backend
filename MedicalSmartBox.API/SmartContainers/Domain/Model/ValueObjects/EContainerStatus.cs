namespace MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

/// <summary>
///     Domain lifecycle and operational status of the SmartContainer.
/// </summary>
public enum EContainerStatus
{
    Available,
    Precooling,
    LockedAndReady,
    InTransit,
    Delivered,
    MaintenanceRequired,
    Warning,
    Offline,
    Unlinked
}
