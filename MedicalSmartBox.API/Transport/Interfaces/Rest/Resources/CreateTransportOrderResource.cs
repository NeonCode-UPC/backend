using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MedicalSmartBox.API.Transport.Domain.Enums;

namespace MedicalSmartBox.API.Transport.Interfaces.Rest.Resources;

public sealed record CreateTransportOrderResource(
    [property: Required, StringLength(20, MinimumLength = 1)] string OrderCode,
    [property: Required] Guid OriginHospitalId,
    [property: Required] Guid DestinationHospitalId,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] PriorityLevel Priority,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] CargoType CargoType,
    [property: Required, StringLength(255, MinimumLength = 1)] string CargoDescription,
    [property: Range(0.01, 99.99)] double MaxSafeHours,
    [property: Range(0.01, 99.98)] double WarningThresholdHours,
    [property: Required] Guid CreatedByUserId);
