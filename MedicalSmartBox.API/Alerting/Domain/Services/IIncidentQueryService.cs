using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;

namespace MedicalSmartBox.API.alerting.Domain.Services;

public interface IIncidentQueryService
{
    Task<CriticalIncident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CriticalIncident>> GetActiveAsync(CancellationToken cancellationToken = default);
}