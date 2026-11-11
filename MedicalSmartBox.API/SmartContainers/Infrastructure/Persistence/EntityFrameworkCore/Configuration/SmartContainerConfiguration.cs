using MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;
using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalSmartBox.API.SmartContainers.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     EF Core mapping configuration for the <c>smart_containers</c> relational table.
/// </summary>
public class SmartContainerConfiguration : IEntityTypeConfiguration<SmartContainer>
{
    public void Configure(EntityTypeBuilder<SmartContainer> builder)
    {
        builder.ToTable("smart_containers");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => new ContainerId(value))
            .ValueGeneratedOnAdd();

        builder.Property(c => c.SerialNumber)
            .HasConversion(sn => sn.Value, value => new SerialNumber(value))
            .HasMaxLength(SerialNumber.MaxLength)
            .IsRequired();

        builder.HasIndex(c => c.SerialNumber)
            .IsUnique();

        builder.Property(c => c.FormFactor)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.LockState)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.AssignedAmbulancePlate)
            .HasMaxLength(15)
            .IsRequired(false);

        builder.Property(c => c.CurrentTemperatureCelsius)
            .HasPrecision(4, 2)
            .IsRequired();

        builder.Property(c => c.TareWeightGrams)
            .HasPrecision(8, 2)
            .IsRequired();

        builder.Property(c => c.NetWeightGrams)
            .HasPrecision(8, 2)
            .IsRequired();

        builder.Property(c => c.BatteryPercentage)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(c => c.BatteryWarningPercentage)
            .IsRequired();

        builder.Property(c => c.SignalQuality)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Is12VConnected)
            .IsRequired();

        builder.Property(c => c.LastTelemetryAt);

        builder.OwnsOne(c => c.Thresholds, nav =>
        {
            nav.Property(t => t.MinTemperatureCelsius)
                .HasColumnName("min_temperature_celsius")
                .HasPrecision(4, 2)
                .HasDefaultValue(ThermalThresholds.DefaultMinSafeTemp);

            nav.Property(t => t.MaxTemperatureCelsius)
                .HasColumnName("max_temperature_celsius")
                .HasPrecision(4, 2)
                .HasDefaultValue(ThermalThresholds.DefaultMaxSafeTemp);

            nav.Property(t => t.WarningTemperatureCelsius)
                .HasColumnName("warning_temperature_celsius")
                .HasPrecision(4, 2)
                .HasDefaultValue(ThermalThresholds.DefaultWarningTemp);

            nav.Property(t => t.ExcursionToleranceSeconds)
                .HasColumnName("excursion_tolerance_seconds")
                .HasDefaultValue(ThermalThresholds.DefaultExcursionToleranceSeconds);
        });
    }
}
