using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;
using MedicalSmartBox.API.alerting.Domain.Repositories;
using MedicalSmartBox.API.alerting.Domain.Services;

namespace MedicalSmartBox.API.alerting.Application.Internal.QueryServices;

public class IncidentQueryService(IIncidentRepository repository, TimeProvider timeProvider) : IIncidentQueryService
{
    public Task<CriticalIncident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => repository.FindByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<CriticalIncident>> GetAllAsync(CancellationToken cancellationToken = default)
        => repository.GetAllAsync(cancellationToken);

    public async Task<IReadOnlyList<CriticalIncident>> GetOverdueAcknowledgementsAsync(CancellationToken cancellationToken = default)
    {
        var deadline = timeProvider.GetUtcNow().AddMinutes(-2);
        var incidents = await repository.GetActiveIncidentsAsync(cancellationToken);
        return incidents.Where(incident => incident.AcknowledgedAt is null && incident.TriggeredAt <= deadline).ToArray();
    }
    public Task<IReadOnlyList<CriticalIncident>> GetActiveAsync(CancellationToken cancellationToken = default)
        => repository.GetActiveIncidentsAsync(cancellationToken);
}