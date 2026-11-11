using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;
using MedicalSmartBox.API.alerting.Domain.Model.Commands;
using MedicalSmartBox.API.alerting.Domain.Repositories;
using MedicalSmartBox.API.alerting.Domain.Services;
using MedicalSmartBox.API.Shared.Domain.Repositories;

namespace MedicalSmartBox.API.alerting.Application.Internal.CommandServices;

public class IncidentCommandService(IIncidentRepository repository, IUnitOfWork unitOfWork) : IIncidentCommandService
{
    public async Task<CriticalIncident?> ResolveAsync(ResolveIncidentCommand command, CancellationToken cancellationToken = default)
    {
        var incident = await repository.FindByIdAsync(command.IncidentId, cancellationToken);
        if (incident is null) return null;
        incident.Resolve(command.MitigationAction, command.QualitySignoffNotes,
            command.WasThermalIntegrityRestored, command.ResolvedByUserId);
        await unitOfWork.CompleteAsync(cancellationToken);
        return incident;
    }

    public async Task<CriticalIncident?> EscalateAsync(EscalateIncidentCommand command, CancellationToken cancellationToken = default)
    {
        var incident = await repository.FindByIdAsync(command.IncidentId, cancellationToken);
        if (incident is null) return null;
        incident.EscalateToMedicalDirector();
        await unitOfWork.CompleteAsync(cancellationToken);
        return incident;
    }
    public async Task<CriticalIncident?> AcknowledgeAsync(AcknowledgeIncidentCommand command, CancellationToken cancellationToken = default)
    {
        var incident = await repository.FindByIdAsync(command.IncidentId, cancellationToken);
        if (incident is null) return null;
        incident.Acknowledge(command.OperatorId);
        await unitOfWork.CompleteAsync(cancellationToken);
        return incident;
    }

    public async Task<CriticalIncident> CreateAsync(CreateIncidentCommand command, CancellationToken cancellationToken = default)
    {
        var incident = new CriticalIncident(command.TripId, command.ContainerId, command.IncidentType,
            command.Severity, command.TriggerTemperatureCelsius, command.TriggerBatteryPercentage);
        await repository.AddAsync(incident, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return incident;
    }
}