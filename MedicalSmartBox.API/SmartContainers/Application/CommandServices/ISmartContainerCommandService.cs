using MedicalSmartBox.API.Shared.Application.Patterns;
using MedicalSmartBox.API.SmartContainers.Application.Errors;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Commands;

namespace MedicalSmartBox.API.SmartContainers.Application.CommandServices;

/// <summary>
///     Application command service contract for SmartContainer lifecycle modifications.
/// </summary>
public interface ISmartContainerCommandService
{
    Task<Result<SmartContainer, SmartContainerError>> Handle(CreateSmartContainerCommand command, CancellationToken cancellationToken = default);
    Task<Result<SmartContainer, SmartContainerError>> Handle(PairContainerToAmbulanceCommand command, CancellationToken cancellationToken = default);
    Task<Result<SmartContainer, SmartContainerError>> Handle(UnlinkContainerCommand command, CancellationToken cancellationToken = default);
    Task<Result<SmartContainer, SmartContainerError>> Handle(RecordWeightMovementCommand command, CancellationToken cancellationToken = default);
    Task<Result<SmartContainer, SmartContainerError>> Handle(UpdateContainerTelemetrySnapshotCommand command, CancellationToken cancellationToken = default);
}
