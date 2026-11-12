using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class BaseRepository<TEntity, TKey>(AppDbContext context) : IBaseRepository<TEntity, TKey> where TEntity : class
{
    protected readonly AppDbContext Context = context;
    
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public async Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entityType = Context.Model.FindEntityType(typeof(TEntity))
            ?? throw new InvalidOperationException($"Entity type '{typeof(TEntity).Name}' is not part of the EF Core model.");
        var primaryKey = entityType.FindPrimaryKey()
            ?? throw new InvalidOperationException($"Entity type '{typeof(TEntity).Name}' does not have a primary key.");

        if (primaryKey.Properties.Count != 1)
            throw new InvalidOperationException($"Repository lookup for '{typeof(TEntity).Name}' requires a single-column primary key.");

        var keyProperty = primaryKey.Properties[0];
        object? keyValue = id;
        if (keyValue is not null && !keyProperty.ClrType.IsInstanceOfType(keyValue))
        {
            var converter = keyProperty.GetTypeMapping().Converter;
            if (converter is null || !converter.ProviderClrType.IsInstanceOfType(keyValue))
                throw new ArgumentException(
                    $"Key value type '{keyValue.GetType().Name}' does not match the model key type '{keyProperty.ClrType.Name}'.",
                    nameof(id));

            keyValue = converter.ConvertFromProvider(keyValue);
        }

        return await Context.Set<TEntity>().FindAsync([keyValue], cancellationToken);
    }

    public void Update(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
    }

    public void Remove(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
    }

    public async Task<IEnumerable<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().ToListAsync(cancellationToken);
    }
}
