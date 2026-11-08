using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MedicalSmartBox.API.Custody.Domain.Model.Aggregates;
using MedicalSmartBox.API.Custody.Domain.Model.Events;
using MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.Custody.Domain.Model.Entities;

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
        IsSealedAndImmutable = true;
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
    public bool IsSealedAndImmutable { get; private set; }
    public IReadOnlyCollection<CustodyEvent> Events => _events.AsReadOnly();

    public static DigitalAuditManifest Seal(CustodyTransfer transfer, DateTimeOffset now,
        decimal averageTemperatureCelsius, decimal minTemperatureCelsius, int totalExcursionSeconds,
        decimal allowedMinTemperatureCelsius, decimal allowedMaxTemperatureCelsius, string? cloudStorageUrl = null)
    {
        if (transfer.Status != CustodyStatus.CompletedAccepted || transfer.Recipient is null)
            throw new InvalidOperationException("Only accepted custody transfers can be sealed.");
        if (totalExcursionSeconds < 0) throw new ArgumentOutOfRangeException(nameof(totalExcursionSeconds));

        var events = transfer.Events.ToArray();
        var manifestId = Guid.CreateVersion7();
        var manifestCode = $"MAN-{now:yyyy}-MINSA-{Guid.NewGuid():N}"[..25];
        var complianceVerified = minTemperatureCelsius >= allowedMinTemperatureCelsius &&
                                 transfer.TemperatureMax <= allowedMaxTemperatureCelsius && totalExcursionSeconds == 0;
        var hash = ComputeHash(manifestId, manifestCode, transfer, now, averageTemperatureCelsius,
            minTemperatureCelsius, transfer.TemperatureMax, totalExcursionSeconds, complianceVerified, events);
        return new(manifestId, transfer.Id, manifestCode, now, transfer.Recipient,
            averageTemperatureCelsius, minTemperatureCelsius, transfer.TemperatureMax, totalExcursionSeconds,
            complianceVerified, events, hash, cloudStorageUrl);
    }

    public bool CalculateThermalCompliance(decimal allowedMinTemperatureCelsius, decimal allowedMaxTemperatureCelsius) =>
        MinTemperatureCelsius >= allowedMinTemperatureCelsius &&
        MaxTemperatureCelsius <= allowedMaxTemperatureCelsius && TotalExcursionSeconds == 0;

    public bool VerifySeal(CustodyTransfer transfer)
    {
        if (!IsSealedAndImmutable || transfer.Id != TransferId) return false;
        if (!_events.SequenceEqual(transfer.Events)) return false;
        var calculated = ComputeHash(Id, ManifestCode, transfer, GeneratedAt, AverageTemperatureCelsius,
            MinTemperatureCelsius, MaxTemperatureCelsius, TotalExcursionSeconds, MinsaComplianceVerified,
            _events);
        try
        {
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(CryptographicHashSha256), Convert.FromHexString(calculated));
        }
        catch (FormatException) { return false; }
    }

    private static string ComputeHash(Guid manifestId, string manifestCode, CustodyTransfer transfer,
        DateTimeOffset generatedAt, decimal averageTemperatureCelsius, decimal minTemperatureCelsius,
        decimal maxTemperatureCelsius, int totalExcursionSeconds, bool minsaComplianceVerified,
        IReadOnlyCollection<CustodyEvent> events)
    {
        var payload = new
        {
            ManifestId = manifestId,
            ManifestCode = manifestCode,
            TransferId = transfer.Id,
            transfer.TransferCode,
            transfer.TripId,
            transfer.Route,
            transfer.Cargo,
            transfer.SmartBox,
            transfer.DepartureAt,
            transfer.ReceivedAt,
            transfer.Recipient,
            transfer.RecipientNotes,
            transfer.TemperatureMax,
            transfer.Openings,
            AverageTemperatureCelsius = averageTemperatureCelsius,
            MinTemperatureCelsius = minTemperatureCelsius,
            MaxTemperatureCelsius = maxTemperatureCelsius,
            TotalExcursionSeconds = totalExcursionSeconds,
            MinsaComplianceVerified = minsaComplianceVerified,
            TemperatureReadings = transfer.TemperatureReadings.OrderBy(reading => reading.RecordedAt),
            Events = events,
            GeneratedAt = generatedAt
        };
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload))))
            .ToLowerInvariant();
    }
}
