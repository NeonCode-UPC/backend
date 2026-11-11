namespace MedicalSmartBox.API.Transport.Domain.ValueObjects;

/// <summary>
/// Represents the geographic coordinates of a transport location.
/// </summary>
public class GeoLocation : IEquatable<GeoLocation>
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

    public double DistanceToMeters(GeoLocation other)
    {
        ArgumentNullException.ThrowIfNull(other);

        const double earthRadiusMeters = 6_371_000;
        var latitude1 = (double)Latitude * Math.PI / 180;
        var latitude2 = (double)other.Latitude * Math.PI / 180;
        var deltaLatitude = ((double)other.Latitude - (double)Latitude) * Math.PI / 180;
        var deltaLongitude = ((double)other.Longitude - (double)Longitude) * Math.PI / 180;
        var a = Math.Pow(Math.Sin(deltaLatitude / 2), 2)
            + Math.Cos(latitude1) * Math.Cos(latitude2) * Math.Pow(Math.Sin(deltaLongitude / 2), 2);

        return earthRadiusMeters * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    public bool Equals(GeoLocation? other) =>
        other is not null && Latitude == other.Latitude && Longitude == other.Longitude;

    public override bool Equals(object? obj) => obj is GeoLocation other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
}
