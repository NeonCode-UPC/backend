using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
/// Registers persistence services used by bounded contexts without referencing their domain types.
/// </summary>
public static class SharedPersistenceExtensions
{
    /// <returns>True when a database connection is configured; false when the host should use its demo adapter.</returns>
    public static bool AddSharedPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MedicalSmartBox");
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
        return true;
    }
}
