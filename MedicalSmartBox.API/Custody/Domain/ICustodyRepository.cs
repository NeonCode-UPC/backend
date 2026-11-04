namespace MedicalSmartBox.API.Custody.Domain;

public interface ICustodyRepository
{
    Task<IReadOnlyList<CustodyTransfer>> ListTransfersAsync(CancellationToken cancellationToken = default);
    Task<CustodyTransfer?> FindTransferAsync(string identifier, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DigitalAuditManifest>> ListManifestsAsync(CancellationToken cancellationToken = default);
    Task<DigitalAuditManifest?> FindManifestByTransferAsync(string identifier, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustodyEvent>> GetAuditEventsAsync(CancellationToken cancellationToken = default);
    Task AddTransferAsync(CustodyTransfer transfer, CancellationToken cancellationToken = default);
    Task AddManifestAsync(DigitalAuditManifest manifest, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
