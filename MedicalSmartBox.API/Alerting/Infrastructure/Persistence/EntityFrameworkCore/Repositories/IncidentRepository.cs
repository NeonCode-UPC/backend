using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;
using MedicalSmartBox.API.alerting.Domain.Model.Enums;
using MedicalSmartBox.API.alerting.Domain.Repositories;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace MedicalSmartBox.API.alerting.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class IncidentRepository(AppDbContext context) : IIncidentRepository
{
    public async Task AddAsync(CriticalIncident incident, CancellationToken cancellationToken = default)
    {
        await context.CriticalIncidents.AddAsync(incident, cancellationToken);
    }

    public Task<CriticalIncident?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.CriticalIncidents.FirstOrDefaultAsync(incident => incident.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<CriticalIncident>> GetActiveIncidentsAsync(CancellationToken cancellationToken = default)
    {
        return await context.CriticalIncidents.AsNoTracking()
            .Where(incident => incident.Status != IncidentStatus.Resolved)
            .OrderByDescending(incident => incident.TriggeredAt)
            .ToListAsync(cancellationToken);
    }
}