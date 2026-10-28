using MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Commands;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Queries;

namespace MedicalSmartBox.API.SmartContainers.Application.Services;

/// <summary>
///     Application service contract coordinating SmartBox catalog queries and commands.
/// </summary>
public interface ISmartContainerService
{
    Task<IEnumerable<SmartContainer>> Handle(GetAllSmartContainersQuery query, CancellationToken cancellationToken = default);
    Task<SmartContainer?> Handle(GetSmartContainerByIdQuery query, CancellationToken cancellationToken = default);
    Task<SmartContainer> Handle(CreateSmartContainerCommand command, CancellationToken cancellationToken = default);
}
