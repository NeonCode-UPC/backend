
using MedicalSmartBox.API.Shared.Application.Patterns;
using MedicalSmartBox.API.Transport.Domain.Enums;
using MedicalSmartBox.API.Transport.Domain.Entities;
using MedicalSmartBox.API.Transport.Domain.ValueObjects;

namespace MedicalSmartBox.API.Transport.Domain.Aggregates;

public class DispatchTrip
{
    private const double HospitalGeofenceRadiusMeters = 100.0;

    private TransportRoute _route;

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }

    public string AssignedVehiclePlate { get; private set; } = string.Empty;
    public Guid AssignedContainerId { get; private set; }
    public Guid AssignedDriverUserId { get; private set; }
    public Guid AssignedParamedicUserId { get; private set; }

    public TripStatus Status { get; private set; }

    public DateTime ScheduledDepartureTime { get; private set; }
    public DateTime? ActualDepartureTime { get; private set; }
    public DateTime EstimatedArrivalTime { get; private set; }
    public DateTime? ActualArrivalTime { get; private set; }

    public GeoLocation DestinationLocation { get; private set; }

    public DispatchTrip(
        Guid orderId,
        DateTime scheduledDepartureTime,
        DateTime estimatedArrivalTime,
        TransportRoute route,
        GeoLocation destinationLocation)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("Transport order ID is required.");

        if (estimatedArrivalTime <= scheduledDepartureTime)
            throw new ArgumentException(
                "Estimated arrival must be after scheduled departure.");

        _route = route ?? throw new ArgumentNullException(nameof(route));

        DestinationLocation = destinationLocation
            ?? throw new ArgumentNullException(nameof(destinationLocation));

        Id = Guid.NewGuid();
        OrderId = orderId;

        ScheduledDepartureTime = scheduledDepartureTime;
        EstimatedArrivalTime = estimatedArrivalTime;

        Status = TripStatus.Scheduled;
    }

    public void AssignCrewAndResources(
        string vehPlate,
        Guid boxId,
        Guid driverId,
        Guid paramedicId)
    {
        if (Status != TripStatus.Scheduled)
            throw new InvalidOperationException(
                "Resources can only be assigned to scheduled trips.");

        if (string.IsNullOrWhiteSpace(vehPlate))
            throw new ArgumentException("Vehicle plate is required.");

        if (boxId == Guid.Empty)
            throw new ArgumentException("Container ID is required.");

        if (driverId == Guid.Empty)
            throw new ArgumentException("Driver ID is required.");

        if (paramedicId == Guid.Empty)
            throw new ArgumentException("Paramedic ID is required.");

        if (driverId == paramedicId)
            throw new ArgumentException(
                "Driver and paramedic must be different users.");

        AssignedVehiclePlate = vehPlate;
        AssignedContainerId = boxId;
        AssignedDriverUserId = driverId;
        AssignedParamedicUserId = paramedicId;

        Status = TripStatus.ResourcesAssigned;
    }

    public Result<bool, string> StartTrip(bool boxPrecooled)
    {
        if (Status != TripStatus.ResourcesAssigned)
            return new Result<bool, string>.Failure(
                "Trip resources must be assigned before starting.");

        if (!boxPrecooled)
            return new Result<bool, string>.Failure(
                "The medical container has not reached its operational temperature.");

        Status = TripStatus.PrecoolingVerified;
        ActualDepartureTime = DateTime.UtcNow;
        Status = TripStatus.InTransit;

        return new Result<bool, string>.Success(true);
    }

    public void UpdateDynamicEta(DateTime newEta)
    {
        if (Status != TripStatus.InTransit)
            throw new InvalidOperationException(
                "ETA can only be updated while the trip is in transit.");

        if (newEta < DateTime.UtcNow)
            throw new ArgumentException(
                "Estimated arrival cannot be in the past.");

        EstimatedArrivalTime = newEta;
    }

    public Result<bool, string> RegisterHospitalArrival(GeoLocation loc)
    {
        if (Status != TripStatus.InTransit)
            return new Result<bool, string>.Failure(
                "The trip must be in transit to register hospital arrival.");

        if (loc is null)
            return new Result<bool, string>.Failure(
                "Current GPS location is required.");

        double distance = CalculateDistanceInMeters(
            loc,
            DestinationLocation);

        if (distance > HospitalGeofenceRadiusMeters)
            return new Result<bool, string>.Failure(
                "The ambulance is outside the 100-meter hospital geofence.");

        ActualArrivalTime = DateTime.UtcNow;
        Status = TripStatus.ArrivedAtDestination;

        return new Result<bool, string>.Success(true);
    }

    private static double CalculateDistanceInMeters(
        GeoLocation point1,
        GeoLocation point2)
    {
        const double EarthRadiusMeters = 6371000;

        double latitude1 =
            (double)point1.Latitude * Math.PI / 180.0;

        double latitude2 =
            (double)point2.Latitude * Math.PI / 180.0;

        double deltaLatitude =
            ((double)point2.Latitude - (double)point1.Latitude)
            * Math.PI / 180.0;

        double deltaLongitude =
            ((double)point2.Longitude - (double)point1.Longitude)
            * Math.PI / 180.0;

        double a =
            Math.Pow(Math.Sin(deltaLatitude / 2), 2) +
            Math.Cos(latitude1) *
            Math.Cos(latitude2) *
            Math.Pow(Math.Sin(deltaLongitude / 2), 2);

        double c = 2 * Math.Atan2(
            Math.Sqrt(a),
            Math.Sqrt(1 - a));

        return EarthRadiusMeters * c;
    }
}
