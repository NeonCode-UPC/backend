namespace MedicalSmartBox.API.alerting.Domain.Model.Enums;

public enum IncidentType
{
    ColdChainBreachHigh,
    ColdChainBreachLow,
    Auxiliary12VPowerLost,
    PayloadTamperingSuspected,
    SevereTrafficDelayExceeded
}