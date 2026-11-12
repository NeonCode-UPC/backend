using System.ComponentModel.DataAnnotations;

namespace MedicalSmartBox.API.Transport.Interfaces.Rest.Resources;

public sealed record AssignTripResourcesResource(
    [param: Required] Guid OrderId,
    [param: Required, StringLength(10, MinimumLength = 1)] string VehiclePlate,
    [param: Required] Guid ContainerId,
    [param: Required] Guid DriverUserId,
    [param: Required] Guid ParamedicUserId,
    [param: Required] DateTime ScheduledDepartureTime,
    [param: Required] DateTime EstimatedArrivalTime,
    [param: Range(0.01, 9999.99)] double DistanceKilometers,
    [param: Range(1, 1_000_000)] int PlannedDurationMinutes,
    [param: Required, StringLength(10000, MinimumLength = 1)] string PolylineCoordinates,
    [param: Range(typeof(decimal), "-90", "90")] decimal DestinationLatitude,
    [param: Range(typeof(decimal), "-180", "180")] decimal DestinationLongitude);
