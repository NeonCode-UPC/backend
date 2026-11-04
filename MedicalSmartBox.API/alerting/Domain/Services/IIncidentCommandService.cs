using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;
using MedicalSmartBox.API.alerting.Domain.Model.Commands;

namespace MedicalSmartBox.API.alerting.Domain.Services;

public interface IIncidentCommandService
{
    Task<CriticalIncident> CreateAsync(CreateIncidentCommand command, CancellationToken cancellationToken = default);
}