using MedicalSmartBox.API.Transport.Domain.Enums;

namespace MedicalSmartBox.API.Transport.Application.Commands;

public sealed record CreateTransportOrderCommand(
    string OrderCode,
    Guid OriginHospitalId,
    Guid DestinationHospitalId,
    PriorityLevel Priority,
    CargoType CargoType,
    string CargoDescription,
    double MaxSafeHours,
    double WarningThresholdHours,
    Guid CreatedByUserId);
