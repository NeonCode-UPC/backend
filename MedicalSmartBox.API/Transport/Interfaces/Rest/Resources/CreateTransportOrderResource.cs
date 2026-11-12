using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MedicalSmartBox.API.Transport.Domain.Enums;

namespace MedicalSmartBox.API.Transport.Interfaces.Rest.Resources;

public sealed record CreateTransportOrderResource(
    [param: Required, StringLength(20, MinimumLength = 1)] string OrderCode,
    [param: Required] Guid OriginHospitalId,
    [param: Required] Guid DestinationHospitalId,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] PriorityLevel Priority,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] CargoType CargoType,
    [param: Required, StringLength(255, MinimumLength = 1)] string CargoDescription,
    [param: Range(0.01, 99.99)] double MaxSafeHours,
    [param: Range(0.01, 99.98)] double WarningThresholdHours,
    [param: Required] Guid CreatedByUserId);
