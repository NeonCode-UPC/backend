using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

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

        // Oracle's MySql.Data uses Windows-specific Socket.IOControl for Keepalive which throws
        // PlatformNotSupportedException on Linux containers. Explicitly ensure Keepalive is 0.
        var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString)
        {
            Keepalive = 0
        };
        var sanitizedConnectionString = connectionStringBuilder.ConnectionString;

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySQL(sanitizedConnectionString, mySqlOptions =>
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
