using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;
using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;
using MedicalSmartBox.API.SmartContainers.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MedicalSmartBox.API.SmartContainers.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core relational repository implementation for the SmartContainer aggregate root.
/// </summary>
public class SmartContainerRepository(AppDbContext context)
    : BaseRepository<SmartContainer, int>(context), ISmartContainerRepository
{
    public async Task<SmartContainer?> FindBySerialNumberAsync(SerialNumber serialNumber, CancellationToken cancellationToken = default)
    {
        return await Context.Set<SmartContainer>()
            .FirstOrDefaultAsync(c => c.SerialNumber == serialNumber, cancellationToken);
    }

    public async Task<SmartContainer?> FindByAssignedAmbulancePlateAsync(string ambulancePlate, CancellationToken cancellationToken = default)
    {
        var normalized = ambulancePlate.Trim().ToUpperInvariant();
        return await Context.Set<SmartContainer>()
            .FirstOrDefaultAsync(c => c.AssignedAmbulancePlate == normalized, cancellationToken);
    }

    public async Task<bool> ExistsBySerialNumberAsync(SerialNumber serialNumber, CancellationToken cancellationToken = default)
    {
        return await Context.Set<SmartContainer>()
            .AnyAsync(c => c.SerialNumber == serialNumber, cancellationToken);
    }
}
