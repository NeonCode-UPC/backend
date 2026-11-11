using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;
using MedicalSmartBox.API.alerting.Domain.Repositories;
using MedicalSmartBox.API.alerting.Domain.Services;

namespace MedicalSmartBox.API.alerting.Application.Internal.QueryServices;

public class IncidentQueryService(IIncidentRepository repository) : IIncidentQueryService
{
    public Task<CriticalIncident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => repository.FindByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<CriticalIncident>> GetAllAsync(CancellationToken cancellationToken = default)
        => repository.GetAllAsync(cancellationToken);

    public Task<IReadOnlyList<CriticalIncident>> GetActiveAsync(CancellationToken cancellationToken = default)
        => repository.GetActiveIncidentsAsync(cancellationToken);
}