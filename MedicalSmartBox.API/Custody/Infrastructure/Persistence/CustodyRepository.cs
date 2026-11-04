using MedicalSmartBox.API.Custody.Domain;
using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace MedicalSmartBox.API.Custody.Infrastructure.Persistence;

public sealed class CustodyRepository(AppDbContext context, IUnitOfWork unitOfWork) : ICustodyRepository
{
    public async Task<IReadOnlyList<CustodyTransfer>> ListTransfersAsync(CancellationToken cancellationToken = default) =>
        await context.Set<CustodyTransfer>().AsNoTracking().OrderByDescending(transfer => transfer.DepartureAt)
            .ToListAsync(cancellationToken);

    public async Task<CustodyTransfer?> FindTransferAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var query = context.Set<CustodyTransfer>().AsQueryable();
        if (Guid.TryParse(identifier, out var id)) return await query.SingleOrDefaultAsync(transfer => transfer.Id == id, cancellationToken);
        return await query.SingleOrDefaultAsync(transfer => transfer.TransferCode == identifier, cancellationToken);
    }

    public async Task<IReadOnlyList<DigitalAuditManifest>> ListManifestsAsync(CancellationToken cancellationToken = default) =>
        await context.Set<DigitalAuditManifest>().AsNoTracking().OrderByDescending(manifest => manifest.GeneratedAt)
            .ToListAsync(cancellationToken);

    public async Task<DigitalAuditManifest?> FindManifestByTransferAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var transfer = await FindTransferAsync(identifier, cancellationToken);
        return transfer is null
            ? null
            : await context.Set<DigitalAuditManifest>().AsNoTracking()
                .SingleOrDefaultAsync(manifest => manifest.TransferId == transfer.Id, cancellationToken);
    }

    public async Task<IReadOnlyList<CustodyEvent>> GetAuditEventsAsync(CancellationToken cancellationToken = default)
    {
        var transfers = await context.Set<CustodyTransfer>().AsNoTracking().ToListAsync(cancellationToken);
        return transfers.SelectMany(transfer => transfer.Events)
            .OrderByDescending(custodyEvent => custodyEvent.At).ToArray();
    }

    public async Task AddTransferAsync(CustodyTransfer transfer, CancellationToken cancellationToken = default) =>
        await context.Set<CustodyTransfer>().AddAsync(transfer, cancellationToken);

    public async Task AddManifestAsync(DigitalAuditManifest manifest, CancellationToken cancellationToken = default) =>
        await context.Set<DigitalAuditManifest>().AddAsync(manifest, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in context.ChangeTracker.Entries<CustodyTransfer>().Where(entry => entry.State != EntityState.Added))
        {
            entry.Property<List<CustodyEvent>>("_events").IsModified = true;
            entry.Property<List<CustodyTemperatureReading>>("_temperatureReadings").IsModified = true;
        }
        return unitOfWork.CompleteAsync(cancellationToken);
    }
}
