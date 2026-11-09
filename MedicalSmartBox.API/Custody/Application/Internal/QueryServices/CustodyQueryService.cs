using MedicalSmartBox.API.Custody.Domain.Model.Aggregates;
using MedicalSmartBox.API.Custody.Domain.Model.Entities;
using MedicalSmartBox.API.Custody.Domain.Model.Events;
using MedicalSmartBox.API.Custody.Domain.Repositories;
using MedicalSmartBox.API.Custody.Application.QueryServices;

namespace MedicalSmartBox.API.Custody.Application.Internal.QueryServices;

public sealed class CustodyQueryService(ICustodyRepository repository) : ICustodyQueryService
{
    public Task<IReadOnlyList<CustodyTransfer>> ListTransfersAsync(CancellationToken cancellationToken = default) =>
        repository.ListTransfersAsync(cancellationToken);

    public Task<IReadOnlyList<DigitalAuditManifest>> ListManifestsAsync(CancellationToken cancellationToken = default) =>
        repository.ListManifestsAsync(cancellationToken);

    public Task<IReadOnlyList<CustodyEvent>> GetAuditEventsAsync(CancellationToken cancellationToken = default) =>
        repository.GetAuditEventsAsync(cancellationToken);

    public Task<CustodyTransfer?> FindTransferAsync(string identifier, CancellationToken cancellationToken = default) =>
        repository.FindTransferAsync(identifier, cancellationToken);

    public Task<DigitalAuditManifest?> FindManifestAsync(string identifier, CancellationToken cancellationToken = default) =>
        repository.FindManifestByTransferAsync(identifier, cancellationToken);
}
