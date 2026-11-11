using MedicalSmartBox.API.Transport.Domain.Enums;

namespace MedicalSmartBox.API.Transport.Application.ReadModels;

public sealed record TransportOrderReadModel(
    Guid Id,
    string OrderCode,
    Guid OriginHospitalId,
    Guid DestinationHospitalId,
    PriorityLevel Priority,
    TransportOrderStatus Status,
    CargoType CargoType,
    string CargoDescription,
    double MaxSafeHours,
    double WarningThresholdHours,
    Guid CreatedByUserId,
    DateTimeOffset? CreatedAt);
