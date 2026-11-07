namespace MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

/// <summary>
///     Electromechanical lid lock state.
/// </summary>
public enum ELockStateContainer
{
    Unlocked,
    Locked,
    Tampered,
    Error
}
