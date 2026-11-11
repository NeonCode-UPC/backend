namespace MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;

public enum CustodyStatus
{
    InTransit,
    PendingOtpVerification,
    OtpVerifiedLidUnlocked,
    CompletedAccepted,
    RejectedThermalExcursion,
    RejectedTampering
}
