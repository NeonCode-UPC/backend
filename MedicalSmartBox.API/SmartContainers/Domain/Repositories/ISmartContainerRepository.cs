using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;
using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.SmartContainers.Domain.Repositories;

/// <summary>
///     Persistence contract for the SmartContainer aggregate root.
/// </summary>
public interface ISmartContainerRepository : IBaseRepository<SmartContainer, int>
{
    Task<SmartContainer?> FindBySerialNumberAsync(SerialNumber serialNumber, CancellationToken cancellationToken = default);
    Task<SmartContainer?> FindByAssignedAmbulancePlateAsync(string ambulancePlate, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySerialNumberAsync(SerialNumber serialNumber, CancellationToken cancellationToken = default);
}
