using Microsoft.EntityFrameworkCore;

namespace MedicalSmartBox.API.Telemetry.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
///     Extension methods for registering Telemetry entity configurations in EF Core ModelBuilder.
/// </summary>
public static class ModelBuilderExtensions
{
    public static void ApplyTelemetryConfiguration(this ModelBuilder builder)
    {
        builder.ApplyConfiguration(new TelemetryLogConfiguration());
    }
}
