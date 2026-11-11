namespace MedicalSmartBox.API.Transport.Domain.Entities;

public class TransportRoute
{
    public Guid Id { get; private set; }
    public double DistanceKilometers { get; private set; }
    public int PlannedDurationMinutes { get; private set; }
    public int CurrentCongestionDelayMinutes { get; private set; }
    public string PolylineCoordinates { get; private set; }

    private TransportRoute()
    {
        PolylineCoordinates = string.Empty;
    }

    public TransportRoute(double distanceKilometers, int plannedDurationMinutes, string polylineCoordinates)
    {
        if (!double.IsFinite(distanceKilometers) || distanceKilometers <= 0 || distanceKilometers > 9999.99
            || Math.Round(distanceKilometers, 2) != distanceKilometers)
            throw new ArgumentOutOfRangeException(nameof(distanceKilometers), "Distance must be greater than zero and fit the database precision.");

        if (plannedDurationMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(plannedDurationMinutes), "Planned duration must be greater than zero.");

        if (string.IsNullOrWhiteSpace(polylineCoordinates))
            throw new ArgumentException("Polyline coordinates cannot be null or empty.", nameof(polylineCoordinates));

        Id = Guid.NewGuid();
        DistanceKilometers = distanceKilometers;
        PlannedDurationMinutes = plannedDurationMinutes;
        CurrentCongestionDelayMinutes = 0;
        PolylineCoordinates = polylineCoordinates;
    }

    public void RecalculateRoute(int congestionMinutes)
    {
        if (congestionMinutes < 0)
            throw new ArgumentOutOfRangeException(nameof(congestionMinutes), "Congestion delay cannot be negative.");

        CurrentCongestionDelayMinutes = congestionMinutes;
    }

    public bool IsSeverelyDelayed()
    {
        return CurrentCongestionDelayMinutes >= 30;
    }
}
