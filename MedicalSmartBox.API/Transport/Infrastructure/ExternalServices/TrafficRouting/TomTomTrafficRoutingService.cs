using System.Net.Http;
using System.Globalization;
using System.Text.Json;
using MedicalSmartBox.API.Transport.Domain.Repositories;
using MedicalSmartBox.API.Transport.Domain.ValueObjects;

namespace MedicalSmartBox.API.Transport.Infrastructure.ExternalServices.TrafficRouting;

public sealed class TomTomTrafficRoutingService(HttpClient httpClient, IConfiguration configuration)
    : ITrafficRoutingService
{
    public async Task<DateTime> GetDynamicEtaAsync(
        GeoLocation origin,
        GeoLocation dest,
        CancellationToken cancellationToken = default)
    {
        var key = GetApiKey();
        var points = string.Create(CultureInfo.InvariantCulture,
            $"{origin.Latitude},{origin.Longitude}:{dest.Latitude},{dest.Longitude}");
        var url = $"routing/1/calculateRoute/{points}/json?traffic=true&travelMode=car&computeTravelTimeFor=all&key={Uri.EscapeDataString(key)}";

        using var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        using var document = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty("routes", out var routes)
            || routes.GetArrayLength() == 0
            || !routes[0].TryGetProperty("summary", out var summary)
            || !summary.TryGetProperty("travelTimeInSeconds", out var travelTime)
            || !travelTime.TryGetInt32(out var seconds)
            || seconds <= 0)
            throw new HttpRequestException("TomTom returned no usable route estimate.");

        return DateTime.UtcNow.AddSeconds(seconds);
    }

    public async Task<int> CheckTrafficDelaysAsync(
        string polyline,
        CancellationToken cancellationToken = default)
    {
        var points = DecodePolyline(polyline).ToArray();
        if (points.Length < 2)
            throw new ArgumentException("The route polyline must contain at least two points.", nameof(polyline));

        var key = GetApiKey();
        var waypoints = SampleRoutePoints(points, maxPoints: 10);
        var routePoints = string.Join(":", waypoints.Select(point =>
            string.Create(CultureInfo.InvariantCulture, $"{point.Latitude},{point.Longitude}")));
        var url = $"routing/1/calculateRoute/{routePoints}/json?traffic=true&travelMode=car&computeTravelTimeFor=all&key={Uri.EscapeDataString(key)}";

        using var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        using var document = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty("routes", out var routes)
            || routes.GetArrayLength() == 0
            || !routes[0].TryGetProperty("summary", out var summary)
            || !summary.TryGetProperty("trafficDelayInSeconds", out var delay)
            || !delay.TryGetInt32(out var seconds))
            throw new HttpRequestException("TomTom returned no usable traffic delay estimate.");

        return Math.Max(0, (int)Math.Ceiling(seconds / 60d));
    }

    private string GetApiKey()
    {
        var key = configuration["TomTom:ApiKey"];
        if (string.IsNullOrWhiteSpace(key))
            throw new HttpRequestException("TomTom:ApiKey is not configured.");

        return key;
    }

    private static IReadOnlyList<(double Latitude, double Longitude)> SampleRoutePoints(
        IReadOnlyList<(double Latitude, double Longitude)> points,
        int maxPoints)
    {
        var sampleCount = Math.Min(points.Count, maxPoints);
        if (sampleCount == 1)
            return [points[0]];

        return Enumerable.Range(0, sampleCount)
            .Select(index => points[(int)Math.Round(index * (points.Count - 1d) / (sampleCount - 1))])
            .Distinct()
            .ToArray();
    }

    private static IEnumerable<(double Latitude, double Longitude)> DecodePolyline(string encoded)
    {
        if (string.IsNullOrWhiteSpace(encoded))
            yield break;

        var index = 0;
        var latitude = 0;
        var longitude = 0;
        while (index < encoded.Length)
        {
            latitude += DecodeSignedValue(encoded, ref index);
            longitude += DecodeSignedValue(encoded, ref index);
            yield return (latitude / 100_000d, longitude / 100_000d);
        }
    }

    private static int DecodeSignedValue(string encoded, ref int index)
    {
        var result = 0;
        var shift = 0;
        int chunk;
        do
        {
            if (index >= encoded.Length || shift > 30)
                throw new ArgumentException("The route polyline is malformed.", nameof(encoded));

            chunk = encoded[index++] - 63;
            if (chunk < 0 || chunk > 63)
                throw new ArgumentException("The route polyline is malformed.", nameof(encoded));

            result |= (chunk & 0x1f) << shift;
            shift += 5;
        } while (chunk >= 0x20);

        return (result & 1) != 0 ? ~(result >> 1) : result >> 1;
    }
}
