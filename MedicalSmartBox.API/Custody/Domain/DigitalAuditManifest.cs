using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MedicalSmartBox.API.Custody.Domain;

/// <summary>Immutable audit snapshot created when a custody transfer is finalized.</summary>
public sealed class DigitalAuditManifest
{
    private DigitalAuditManifest() { }

    private DigitalAuditManifest(Guid id, Guid transferId, string manifestCode, DateTimeOffset generatedAt,
        string recipient, decimal averageTemperatureCelsius, decimal minTemperatureCelsius,
        decimal maxTemperatureCelsius, int totalExcursionSeconds, bool minsaComplianceVerified,
        IReadOnlyCollection<CustodyEvent> events, string hash, string? cloudStorageUrl)
    {
        Id = id;
        TransferId = transferId;
        ManifestCode = manifestCode;
        GeneratedAt = generatedAt;
        Recipient = recipient;
        AverageTemperatureCelsius = averageTemperatureCelsius;
        MinTemperatureCelsius = minTemperatureCelsius;
        MaxTemperatureCelsius = maxTemperatureCelsius;
        TotalExcursionSeconds = totalExcursionSeconds;
        MinsaComplianceVerified = minsaComplianceVerified;
        _events = events.ToList();
        CryptographicHashSha256 = hash;
        CloudStorageUrl = cloudStorageUrl;
    }

    private List<CustodyEvent> _events = [];

    public Guid Id { get; private set; }
    public Guid TransferId { get; private set; }
    public string ManifestCode { get; private set; } = string.Empty;
    public DateTimeOffset GeneratedAt { get; private set; }
    public string Recipient { get; private set; } = string.Empty;
    public decimal AverageTemperatureCelsius { get; private set; }
    public decimal MinTemperatureCelsius { get; private set; }
    public decimal MaxTemperatureCelsius { get; private set; }
    public int TotalExcursionSeconds { get; private set; }
    public bool MinsaComplianceVerified { get; private set; }
    public string CryptographicHashSha256 { get; private set; } = string.Empty;
    public string? CloudStorageUrl { get; private set; }
    public bool IsSealedAndImmutable => !string.IsNullOrWhiteSpace(CryptographicHashSha256);
    public IReadOnlyCollection<CustodyEvent> Events => _events.AsReadOnly();

    public static DigitalAuditManifest Seal(CustodyTransfer transfer, DateTimeOffset now,
        decimal averageTemperatureCelsius, decimal minTemperatureCelsius, int totalExcursionSeconds,
        decimal allowedMinTemperatureCelsius, decimal allowedMaxTemperatureCelsius, string? cloudStorageUrl = null)
    {
        if (transfer.Status != CustodyStatus.CompletedAccepted || transfer.Recipient is null)
            throw new InvalidOperationException("Only accepted custody transfers can be sealed.");
        if (totalExcursionSeconds < 0) throw new ArgumentOutOfRangeException(nameof(totalExcursionSeconds));

        var events = transfer.Events.ToArray();
        var payload = new
        {
            transfer.Id, transfer.TransferCode, transfer.TripId, transfer.Route, transfer.Cargo,
            transfer.SmartBox, transfer.DepartureAt, transfer.ReceivedAt, transfer.Recipient,
            transfer.RecipientNotes, transfer.TemperatureMax, transfer.Openings,
            AverageTemperatureCelsius = averageTemperatureCelsius,
            MinTemperatureCelsius = minTemperatureCelsius,
            MaxTemperatureCelsius = transfer.TemperatureMax,
            TotalExcursionSeconds = totalExcursionSeconds,
            MinsaComplianceVerified = minTemperatureCelsius >= allowedMinTemperatureCelsius &&
                                      transfer.TemperatureMax <= allowedMaxTemperatureCelsius && totalExcursionSeconds == 0,
            Events = events,
            GeneratedAt = now
        };
        var canonical = JsonSerializer.Serialize(payload);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
        var manifestCode = $"MAN-{now:yyyy}-MINSA-{transfer.TransferCode[^Math.Min(4, transfer.TransferCode.Length)..]}";
        return new(Guid.NewGuid(), transfer.Id, manifestCode, now, transfer.Recipient,
            averageTemperatureCelsius, minTemperatureCelsius, transfer.TemperatureMax, totalExcursionSeconds,
            payload.MinsaComplianceVerified, events, hash, cloudStorageUrl);
    }

    public bool CalculateThermalCompliance(decimal allowedMinTemperatureCelsius, decimal allowedMaxTemperatureCelsius) =>
        MinTemperatureCelsius >= allowedMinTemperatureCelsius &&
        MaxTemperatureCelsius <= allowedMaxTemperatureCelsius && TotalExcursionSeconds == 0;
}
