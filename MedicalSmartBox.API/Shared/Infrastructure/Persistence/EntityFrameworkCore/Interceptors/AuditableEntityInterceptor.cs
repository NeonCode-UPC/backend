using MedicalSmartBox.API.Shared.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor 
{
    private static void ApplyAuditTimestamps(DbContext? context)
    {
        if (context is null) return;

        var now = DateTimeOffset.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified) 
                entry.Entity.UpdatedAt = now;
            if (entry.State is EntityState.Added) entry.Entity.CreatedAt ??= now;
        }
        
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, 
        InterceptionResult<int> result)
    {
        ApplyAuditTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditTimestamps(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);   
    }
}