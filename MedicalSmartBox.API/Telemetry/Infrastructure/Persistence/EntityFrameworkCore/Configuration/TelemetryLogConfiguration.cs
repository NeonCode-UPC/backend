using MedicalSmartBox.API.Telemetry.Domain.Model.Aggregates;
using MedicalSmartBox.API.Telemetry.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalSmartBox.API.Telemetry.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Entity Framework Core mapping configuration for TelemetryLog aggregate.
/// </summary>
public class TelemetryLogConfiguration : IEntityTypeConfiguration<TelemetryLog>
{
    public void Configure(EntityTypeBuilder<TelemetryLog> builder)
    {
        builder.ToTable("telemetry_logs");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, value => new TelemetryLogId(value))
            .ValueGeneratedOnAdd();

        builder.Property(t => t.ContainerId)
            .HasConversion(id => id.Value, value => new ContainerId(value))
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(t => t.TripId)
            .HasConversion(id => id.HasValue ? id.Value.Value : null, value => value != null ? new TripId(value) : (TripId?)null)
            .HasMaxLength(64);

        builder.Property(t => t.TimestampUtc).IsRequired();

        builder.Property(t => t.TemperatureCelsius).HasPrecision(5, 2).IsRequired();
        builder.Property(t => t.AmbientTemperatureCelsius).HasPrecision(5, 2).IsRequired();
        builder.Property(t => t.WeightGrams).HasPrecision(8, 2).IsRequired();
        builder.Property(t => t.BatteryPercentage).HasPrecision(5, 2).IsRequired();

        builder.Property(t => t.Is12VConnected).IsRequired();
        builder.Property(t => t.PeltierPowerPct).IsRequired();
        builder.Property(t => t.LidLockEngaged).IsRequired();

        builder.Property(t => t.Latitude).HasPrecision(10, 6);
        builder.Property(t => t.Longitude).HasPrecision(10, 6);

        builder.Property(t => t.FirmwareSignature).IsRequired().HasMaxLength(128);
        builder.Property(t => t.IsThermalExcursion).IsRequired();

        builder.HasIndex(t => t.ContainerId);
        builder.HasIndex(t => t.TripId);
        builder.HasIndex(t => t.TimestampUtc);
    }
}
