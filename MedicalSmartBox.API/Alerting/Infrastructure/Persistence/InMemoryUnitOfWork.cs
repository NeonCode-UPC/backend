using MedicalSmartBox.API.Shared.Domain.Repositories;

namespace MedicalSmartBox.API.alerting.Infrastructure.Persistence;

// In-memory incident changes are applied directly; there is no database transaction.
public class InMemoryUnitOfWork : IUnitOfWork
{
    public Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}