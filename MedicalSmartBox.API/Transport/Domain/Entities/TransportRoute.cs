namespace MedicalSmartBox.API.Transport.Domain.Entities;

public sealed record TransportRoute
{
    public double DistanceKilometers { get; private init; }
    public int PlannedDurationMinutes { get; private init; }
    public int CurrentCongestionDelayMinutes { get; private init; }
    public string PolylineCoordinates { get; private init; }

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

        DistanceKilometers = distanceKilometers;
        PlannedDurationMinutes = plannedDurationMinutes;
        CurrentCongestionDelayMinutes = 0;
        PolylineCoordinates = polylineCoordinates;
    }

    public TransportRoute WithCongestionDelay(int congestionMinutes)
    {
        if (congestionMinutes < 0)
            throw new ArgumentOutOfRangeException(nameof(congestionMinutes), "Congestion delay cannot be negative.");

        return this with { CurrentCongestionDelayMinutes = congestionMinutes };
    }

    public bool IsSeverelyDelayed()
    {
        return CurrentCongestionDelayMinutes >= 30;
    }
}
