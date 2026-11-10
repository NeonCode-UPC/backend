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
        var containers = await repository.ListAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            containers = containers.Where(c =>
                c.Status.ToString().Equals(query.Status.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.AmbulancePlate))
        {
            containers = containers.Where(c =>
                c.AssignedAmbulancePlate != null &&
                c.AssignedAmbulancePlate.Equals(query.AmbulancePlate.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return containers;
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
