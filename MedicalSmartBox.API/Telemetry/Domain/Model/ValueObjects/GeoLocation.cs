namespace MedicalSmartBox.API.Telemetry.Domain.Model.ValueObjects;

/// <summary>
///     Value object encapsulating geographic GPS coordinates transmitted by SIM7600G IoT module.
/// </summary>
/// <param name="Latitude">Latitude coordinate between -90.0 and 90.0 degrees.</param>
/// <param name="Longitude">Longitude coordinate between -180.0 and 180.0 degrees.</param>
public record GeoLocation
{
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }

    public GeoLocation()
    {
        Latitude = 0.0m;
        Longitude = 0.0m;
    }

    public GeoLocation(decimal latitude, decimal longitude)
    {
        if (latitude is < -90.0m or > 90.0m)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90.0 and 90.0.");

        if (longitude is < -180.0m or > 180.0m)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180.0 and 180.0.");

        Latitude = latitude;
        Longitude = longitude;
    }
}
