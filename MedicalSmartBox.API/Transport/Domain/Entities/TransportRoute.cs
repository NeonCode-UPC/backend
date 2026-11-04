namespace MedicalSmartBox.API.Transport.Domain.Entities;

public class TransportRoute
{
    public Guid Id { get; private set; }
    public double DistanceKm { get; private set; }
    public int PlannedDurationMinutes { get; private set; }
    public int TrafficDelayMinutes { get; private set; }
    public string EncodedPolyline { get; private set; }

    public TransportRoute(double distanceKm, int plannedDurationMinutes, string encodedPolyline)
    {
        if (distanceKm <= 0)
            throw new ArgumentException("Distance must be greater than zero.", nameof(distanceKm));
        
        if (plannedDurationMinutes <= 0)
            throw new ArgumentException("Planned duration must be greater than zero.", nameof(plannedDurationMinutes));
        
        if (string.IsNullOrWhiteSpace(encodedPolyline))
            throw new ArgumentException("Encoded polyline cannot be null or empty.", nameof(encodedPolyline));

        Id = Guid.NewGuid();
        DistanceKm = distanceKm;
        PlannedDurationMinutes = plannedDurationMinutes;
        TrafficDelayMinutes = 0; // Valor 0 predeterminado. Se puede cambiar 
        EncodedPolyline = encodedPolyline;
    }
    
    public void RecalculateRoute(double newDistanceKm, int newPlannedDurationMinutes, string newEncodedPolyline)
    {
        if (newDistanceKm <= 0)
            throw new ArgumentException("Distance must be greater than zero.", nameof(newDistanceKm));
        
        if (newPlannedDurationMinutes <= 0)
            throw new ArgumentException("Planned duration must be greater than zero.", nameof(newPlannedDurationMinutes));
        
        if (string.IsNullOrWhiteSpace(newEncodedPolyline))
            throw new ArgumentException("Encoded polyline cannot be null or empty.", nameof(newEncodedPolyline));

        DistanceKm = newDistanceKm;
        PlannedDurationMinutes = newPlannedDurationMinutes;
        EncodedPolyline = newEncodedPolyline;
    }

    public bool IsSeverelyDelayed()
    {
        return TrafficDelayMinutes >= 30;
    }
}