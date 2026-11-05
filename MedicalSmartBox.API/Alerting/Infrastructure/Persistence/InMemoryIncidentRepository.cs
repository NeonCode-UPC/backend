using System.Collections.Concurrent;
using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;
using MedicalSmartBox.API.alerting.Domain.Model.Enums;
using MedicalSmartBox.API.alerting.Domain.Repositories;

namespace MedicalSmartBox.API.alerting.Infrastructure.Persistence;

// Development fallback only: data is lost when the application restarts.
public class InMemoryIncidentRepository : IIncidentRepository
{
    private readonly ConcurrentDictionary<Guid, CriticalIncident> _incidents = new();

    public Task AddAsync(CriticalIncident incident, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_incidents.TryAdd(incident.Id, incident)) throw new InvalidOperationException("Incident already exists.");
        return Task.CompletedTask;
    }

    public Task<CriticalIncident?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _incidents.TryGetValue(id, out var incident);
        return Task.FromResult(incident);
    }

    public Task<IReadOnlyList<CriticalIncident>> GetActiveIncidentsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<CriticalIncident> incidents = _incidents.Values
            .Where(item => item.Status != IncidentStatus.Resolved)
            .OrderByDescending(item => item.TriggeredAt).ToArray();
        return Task.FromResult(incidents);
    }
}