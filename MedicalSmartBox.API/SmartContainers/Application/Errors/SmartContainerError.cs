namespace MedicalSmartBox.API.SmartContainers.Application.Errors;

/// <summary>
///     Domain errors that can occur during SmartContainer operations.
/// </summary>
public enum SmartContainerError
{
    None,
    NotFound,
    DuplicateSerialNumber,
    AlreadyPaired,
    NotPaired,
    InvalidWeight,
    InvalidLockState,
    ThermalExcursionActive
}
