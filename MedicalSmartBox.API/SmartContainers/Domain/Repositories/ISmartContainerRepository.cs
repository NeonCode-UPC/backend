using MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;
using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Repositories;

/// <summary>
///     Repository contract for SmartContainer persistence.
/// </summary>
public interface ISmartContainerRepository
{
    Task<IEnumerable<SmartContainer>> ListAsync(CancellationToken cancellationToken = default);
    Task<SmartContainer?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SmartContainer?> FindBySerialNumberAsync(SerialNumber serialNumber, CancellationToken cancellationToken = default);
    Task<SmartContainer> AddAsync(SmartContainer container, CancellationToken cancellationToken = default);
}
