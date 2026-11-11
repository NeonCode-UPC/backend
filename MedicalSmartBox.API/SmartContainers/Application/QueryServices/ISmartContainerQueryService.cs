using MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Queries;

namespace MedicalSmartBox.API.SmartContainers.Application.QueryServices;

/// <summary>
///     Application query service contract for SmartContainer read operations.
/// </summary>
public interface ISmartContainerQueryService
{
    Task<IEnumerable<SmartContainer>> Handle(GetAllSmartContainersQuery query, CancellationToken cancellationToken = default);
    Task<SmartContainer?> Handle(GetSmartContainerByIdQuery query, CancellationToken cancellationToken = default);
    Task<SmartContainer?> Handle(GetSmartContainerBySerialNumberQuery query, CancellationToken cancellationToken = default);
}
