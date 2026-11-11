using System.Text.Json.Serialization;
using MedicalSmartBox.API.Transport.Domain.Enums;

namespace MedicalSmartBox.API.Transport.Interfaces.Rest.Responses;

public sealed record TransportOrderResponse(
    Guid Id,
    string OrderCode,
    Guid OriginHospitalId,
    Guid DestinationHospitalId,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] PriorityLevel Priority,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] TransportOrderStatus Status,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] CargoType CargoType,
    string CargoDescription,
    double MaxSafeHours,
    double WarningThresholdHours,
    Guid CreatedByUserId,
    DateTimeOffset? CreatedAt);
