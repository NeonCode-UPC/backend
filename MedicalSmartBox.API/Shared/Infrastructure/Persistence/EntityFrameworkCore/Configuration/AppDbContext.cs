using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>Shared EF Core context that discovers module mappings without depending on bounded contexts.</summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.UseSnakeCaseNamingConvention();
    }
}
