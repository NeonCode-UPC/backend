using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
/// Registers persistence services used by bounded contexts without referencing their domain types.
/// </summary>
public static class SharedPersistenceExtensions
{
    public static void AddSharedPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MedicalSmartBox");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "The MySQL connection string 'ConnectionStrings:MedicalSmartBox' is required. " +
                "Set it with the ConnectionStrings__MedicalSmartBox environment variable.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySQL(connectionString, mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            }));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
    }
}
