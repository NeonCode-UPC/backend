using MedicalSmartBox.API.Shared.Domain.Repositories;

namespace MedicalSmartBox.API.SmartContainers.Infrastructure.Persistence;

/// <summary>
///     No-op UnitOfWork implementation for in-memory testing and development mode.
/// </summary>
public class InMemoryUnitOfWork : IUnitOfWork
{
    public Task CompleteAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
