using System.Security.Cryptography;
using System.Text;
using MedicalSmartBox.API.Custody.Domain.Model.Aggregates;
using MedicalSmartBox.API.Custody.Domain.Model.Entities;
using MedicalSmartBox.API.Custody.Domain.Model.Events;
using MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;
using MedicalSmartBox.API.Custody.Domain.Repositories;

namespace MedicalSmartBox.API.Custody.Infrastructure.Persistence.InMemory.Repositories;

/// <summary>Volatile local adapter used when no MySQL connection is configured.</summary>
public sealed class InMemoryCustodyRepository : ICustodyRepository
{
    private readonly Dictionary<Guid, CustodyTransfer> _transfers = [];
    private readonly Dictionary<Guid, DigitalAuditManifest> _manifests = [];

    public InMemoryCustodyRepository(IConfiguration configuration, TimeProvider timeProvider)
    {
        var departure = new DateTimeOffset(2026, 9, 29, 13, 5, 0, TimeSpan.Zero);
        var active = new CustodyTransfer(Guid.Parse("8b12164a-1d73-4c88-9a38-f84a59610817"), "TR-0417",
            Guid.Parse("f8a1b123-4545-4f10-bf20-9d4f120c0455"), Guid.Parse("2d5dc124-bbb7-4d1e-8432-8d8b877816e1"),
            Guid.Parse("66c777e0-74d3-4f65-a87f-3a0f81fdc042"), "Lima → Arequipa", "Vacunas", "SB-0182", 4.4m, 0, departure,
            [new(departure.AddMinutes(15), 4.2m), new(departure.AddMinutes(45), 4.4m), new(departure.AddMinutes(75), 4.1m)]);
        _transfers.Add(active.Id, active);

        var samplesDeparture = departure.AddHours(2);
        var samples = new CustodyTransfer(Guid.Parse("a7c262bb-2887-4fc5-9c79-8b7fca0bc833"), "TR-0398",
            Guid.Parse("7e2e73cb-c774-47b8-a4bf-d4b275681afe"), Guid.Parse("b316e9d3-7f86-41e1-8f64-19c813c00db4"),
            Guid.Parse("498f5e86-a65e-4891-a996-49658929c680"), "Lima → Arequipa", "Muestras", "SB-0197", 5.1m, 0, samplesDeparture,
            [new(samplesDeparture.AddMinutes(15), 4.8m), new(samplesDeparture.AddMinutes(40), 5.1m)]);
        _transfers.Add(samples.Id, samples);

        var receivedAt = timeProvider.GetUtcNow().AddDays(-1);
        var closed = new CustodyTransfer(Guid.Parse("41cf7321-3230-4e0f-8f1e-0298c62fcfd2"), "TR-0244",
            Guid.Parse("e1c58246-0dd6-4dde-b2a1-73b7cc18da4c"), Guid.Parse("83be7a50-d39a-4514-adc2-497db2c5eb59"),
            Guid.Parse("32a146a4-ed8d-48a0-9fc1-b6957a2d16b7"), "Callao → Chincha", "Hemocomponentes", "SB-0165", 5.6m, 0, receivedAt.AddHours(-3),
            [new(receivedAt.AddHours(-2.75), 4.4m), new(receivedAt.AddHours(-1.5), 5.6m), new(receivedAt.AddMinutes(-20), 4.8m)]);
        var otp = "307158";
        var key = configuration["Custody:OtpHmacKey"] ?? "local-development-only-change-this-key";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(otp))).ToLowerInvariant();
        closed.PrepareReception("Dr. Jorge Paredes Lino", hash, receivedAt.AddMinutes(15), true, receivedAt.AddMinutes(-10));
        closed.VerifyOtp(hash, receivedAt.AddMinutes(-9));
        closed.ConfirmCustodyAcceptance(null, false, receivedAt);
        var manifest = DigitalAuditManifest.Seal(closed, receivedAt, 4.8m, 4.1m, 0, 2m, 8m);
        _transfers.Add(closed.Id, closed);
        _manifests.Add(manifest.Id, manifest);

        var excursionDeparture = receivedAt.AddDays(-4).AddHours(-4);
        var excursion = new CustodyTransfer(Guid.Parse("6d5f2cd0-4405-4817-8eec-d24acec20a3c"), "TR-0233",
            Guid.Parse("f6a2cf99-d2f7-440c-9c37-245891726fc2"), Guid.Parse("4c516c8c-9297-44ef-b977-11837de4e614"),
            Guid.Parse("0214b47f-f26b-4535-9fc8-a1b37d714e8c"), "Lima → Ica", "Hemocomponentes", "SB-0144", 9.2m, 1, excursionDeparture,
            [new(excursionDeparture.AddMinutes(20), 4.6m), new(excursionDeparture.AddHours(1), 9.2m)]);
        excursion.RejectDeliveryDueToThermalDamage("Excursión térmica detectada durante el traslado.", excursionDeparture.AddHours(2));
        _transfers.Add(excursion.Id, excursion);

        var currentDeparture = timeProvider.GetUtcNow().AddHours(-3);
        var current = new CustodyTransfer(Guid.Parse("d39bb8c6-33a7-46e7-aea0-b2bcba234c10"), "TR-0251",
            Guid.Parse("f2569529-3916-4d8e-a363-91d2ddcc8eb2"), Guid.Parse("1c4f88d8-3171-4e22-9cd6-c38cb05c1cd6"),
            Guid.Parse("447dd9fc-765b-4376-bcef-2ad6537a52e2"), "Lima → Trujillo", "Vacunas", "SB-0201", 5.3m, 0, currentDeparture,
            [new(currentDeparture.AddMinutes(15), 4.9m), new(currentDeparture.AddHours(1), 5.3m)]);
        _transfers.Add(current.Id, current);
    }

    public Task<IReadOnlyList<CustodyTransfer>> ListTransfersAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CustodyTransfer>>(_transfers.Values.OrderByDescending(item => item.DepartureAt).ToArray());

    public Task<CustodyTransfer?> FindTransferAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var transfer = Guid.TryParse(identifier, out var id)
            ? _transfers.GetValueOrDefault(id)
            : _transfers.Values.SingleOrDefault(item => item.TransferCode.Equals(identifier, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(transfer);
    }

    public Task<IReadOnlyList<DigitalAuditManifest>> ListManifestsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<DigitalAuditManifest>>(_manifests.Values.OrderByDescending(item => item.GeneratedAt).ToArray());

    public async Task<DigitalAuditManifest?> FindManifestByTransferAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var transfer = await FindTransferAsync(identifier, cancellationToken);
        return transfer is null ? null : _manifests.Values.SingleOrDefault(manifest => manifest.TransferId == transfer.Id);
    }

    public Task<IReadOnlyList<CustodyEvent>> GetAuditEventsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CustodyEvent>>(_transfers.Values.SelectMany(transfer => transfer.Events)
            .OrderByDescending(item => item.At).ToArray());

    public Task AddTransferAsync(CustodyTransfer transfer, CancellationToken cancellationToken = default)
    {
        if (_transfers.Values.Any(item => item.TransferCode == transfer.TransferCode || item.TripId == transfer.TripId))
            throw new InvalidOperationException("The transfer code or trip is already registered for custody.");
        _transfers.Add(transfer.Id, transfer);
        return Task.CompletedTask;
    }

    public Task AddManifestAsync(DigitalAuditManifest manifest, CancellationToken cancellationToken = default)
    {
        if (_manifests.Values.Any(item => item.TransferId == manifest.TransferId))
            throw new InvalidOperationException("A manifest already exists for this transfer.");
        _manifests.Add(manifest.Id, manifest);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
