namespace MedicalSmartBox.API.Transport.Domain.ValueObjects;

/// <summary>
/// Represents the geographic coordinates of a transport location.
/// </summary>
public class GeoLocation
{
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }

    public GeoLocation(decimal latitude, decimal longitude)
    {
        if (latitude < -90.0m || latitude > 90.0m)
            throw new ArgumentOutOfRangeException(
                nameof(latitude),
                "Latitude must be between -90.0 and 90.0.");

        if (longitude < -180.0m || longitude > 180.0m)
            throw new ArgumentOutOfRangeException(
                nameof(longitude),
                "Longitude must be between -180.0 and 180.0.");

        Latitude = latitude;
        Longitude = longitude;
    }
}