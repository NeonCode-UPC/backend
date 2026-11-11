using System.ComponentModel.DataAnnotations;

namespace MedicalSmartBox.API.Transport.Interfaces.Rest.Resources;

public sealed record AssignTripResourcesResource(
    [property: Required] Guid OrderId,
    [property: Required, StringLength(10, MinimumLength = 1)] string VehiclePlate,
    [property: Required] Guid ContainerId,
    [property: Required] Guid DriverUserId,
    [property: Required] Guid ParamedicUserId,
    [property: Required] DateTime ScheduledDepartureTime,
    [property: Required] DateTime EstimatedArrivalTime,
    [property: Range(0.01, 9999.99)] double DistanceKilometers,
    [property: Range(1, 1_000_000)] int PlannedDurationMinutes,
    [property: Required, StringLength(10000, MinimumLength = 1)] string PolylineCoordinates,
    [property: Range(typeof(decimal), "-90", "90")] decimal DestinationLatitude,
    [property: Range(typeof(decimal), "-180", "180")] decimal DestinationLongitude);
