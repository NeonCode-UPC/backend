using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;

namespace MedicalSmartBox.API.alerting.Domain.Repositories;

public interface IIncidentRepository
{
    Task AddAsync(CriticalIncident incident, CancellationToken cancellationToken = default);
    Task<CriticalIncident?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CriticalIncident>> GetActiveIncidentsAsync(CancellationToken cancellationToken = default);
}