using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;
using MedicalSmartBox.API.alerting.Domain.Model.Commands;
using MedicalSmartBox.API.alerting.Domain.Repositories;
using MedicalSmartBox.API.alerting.Domain.Services;
using MedicalSmartBox.API.Shared.Domain.Repositories;

namespace MedicalSmartBox.API.alerting.Application.Internal.CommandServices;

public class IncidentCommandService(IIncidentRepository repository, IUnitOfWork unitOfWork) : IIncidentCommandService
{
    public async Task<CriticalIncident> CreateAsync(CreateIncidentCommand command, CancellationToken cancellationToken = default)
    {
        var incident = new CriticalIncident(command.TripId, command.ContainerId, command.IncidentType,
            command.Severity, command.TriggerTemperatureCelsius, command.TriggerBatteryPercentage);
        await repository.AddAsync(incident, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return incident;
    }
}