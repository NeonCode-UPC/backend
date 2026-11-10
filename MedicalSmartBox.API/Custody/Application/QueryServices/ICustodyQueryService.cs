using MedicalSmartBox.API.Custody.Domain.Model.Aggregates;
using MedicalSmartBox.API.Custody.Domain.Model.Entities;
using MedicalSmartBox.API.Custody.Domain.Model.Events;

namespace MedicalSmartBox.API.Custody.Application.QueryServices;

public interface ICustodyQueryService
{
    Task<IReadOnlyList<CustodyTransfer>> ListTransfersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DigitalAuditManifest>> ListManifestsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustodyEvent>> GetAuditEventsAsync(CancellationToken cancellationToken = default);
    Task<CustodyTransfer?> FindTransferAsync(string identifier, CancellationToken cancellationToken = default);
    Task<DigitalAuditManifest?> FindManifestAsync(string identifier, CancellationToken cancellationToken = default);
}
