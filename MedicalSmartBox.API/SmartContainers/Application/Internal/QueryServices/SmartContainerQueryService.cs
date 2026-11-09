using MedicalSmartBox.API.SmartContainers.Application.QueryServices;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Queries;
using MedicalSmartBox.API.SmartContainers.Domain.Repositories;

namespace MedicalSmartBox.API.SmartContainers.Application.Internal.QueryServices;

/// <summary>
///     Application query service executing SmartContainer reads.
/// </summary>
public class SmartContainerQueryService(ISmartContainerRepository repository)
    : ISmartContainerQueryService
{
    public async Task<IEnumerable<SmartContainer>> Handle(
        GetAllSmartContainersQuery query,
        CancellationToken cancellationToken = default)
    {
        return await repository.ListAsync(cancellationToken);
    }

    public async Task<SmartContainer?> Handle(
        GetSmartContainerByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await repository.FindByIdAsync(query.Id.Value, cancellationToken);
    }

    public async Task<SmartContainer?> Handle(
        GetSmartContainerBySerialNumberQuery query,
        CancellationToken cancellationToken = default)
    {
        return await repository.FindBySerialNumberAsync(query.SerialNumber, cancellationToken);
    }
}
