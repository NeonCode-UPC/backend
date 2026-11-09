using MedicalSmartBox.API.Shared.Domain.Repositories;

namespace MedicalSmartBox.API.Telemetry.Infrastructure.Persistence;

/// <summary>
///     In-memory unit of work stub used when running locally without a MySQL connection.
/// </summary>
public class InMemoryUnitOfWork : IUnitOfWork
{
    public Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
