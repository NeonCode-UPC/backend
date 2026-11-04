using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MedicalSmartBox.API.Custody.Domain;

/// <summary>Immutable snapshot proving how a custody transfer was completed.</summary>
public sealed class DigitalAuditManifest
{
    private DigitalAuditManifest() { }

    private DigitalAuditManifest(string id, string transferId, DateTimeOffset sealedAt, string recipient,
        decimal temperatureMax, int openings, string cargo, IReadOnlyCollection<CustodyEvent> events, string hash)
    {
        Id = id;
        TransferId = transferId;
        SealedAt = sealedAt;
        Recipient = recipient;
        TemperatureMax = temperatureMax;
        Openings = openings;
        Cargo = cargo;
        Events = events;
        Hash = hash;
    }

    public string Id { get; private set; } = string.Empty;
    public string TransferId { get; private set; } = string.Empty;
    public DateTimeOffset SealedAt { get; private set; }
    public string Recipient { get; private set; } = string.Empty;
    public decimal TemperatureMax { get; private set; }
    public int Openings { get; private set; }
    public string Cargo { get; private set; } = string.Empty;
    public IReadOnlyCollection<CustodyEvent> Events { get; private set; } = [];
    public string Hash { get; private set; } = string.Empty;
    public string Status => "valid";

    public static DigitalAuditManifest Seal(CustodyTransfer transfer, DateTimeOffset now)
    {
        if (transfer.Status != CustodyStatus.Closed || transfer.Recipient is null)
            throw new InvalidOperationException("Only closed custody transfers can be sealed.");

        var events = transfer.Events.ToArray();
        var canonical = JsonSerializer.Serialize(new
        {
            transfer.Id, transfer.Route, transfer.Cargo, transfer.SmartBox, transfer.TemperatureMax,
            transfer.Openings, transfer.Recipient, SealedAt = now, Events = events
        });
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
        return new(Guid.NewGuid().ToString("N"), transfer.Id, now, transfer.Recipient,
            transfer.TemperatureMax, transfer.Openings, transfer.Cargo, events, hash);
    }
}
