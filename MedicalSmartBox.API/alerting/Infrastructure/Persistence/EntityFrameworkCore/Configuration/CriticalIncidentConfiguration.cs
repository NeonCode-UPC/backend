using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalSmartBox.API.alerting.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public class CriticalIncidentConfiguration : IEntityTypeConfiguration<CriticalIncident>
{
    public void Configure(EntityTypeBuilder<CriticalIncident> builder)
    {
        builder.ToTable("critical_incidents");
        builder.HasKey(incident => incident.Id);
        builder.Property(incident => incident.Id).ValueGeneratedNever();
        builder.Property(incident => incident.TripId).IsRequired();
        builder.Property(incident => incident.ContainerId).IsRequired();
        builder.Property(incident => incident.IncidentType).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(incident => incident.Severity).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(incident => incident.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(incident => incident.EscalationLevel).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(incident => incident.TriggerTemperatureCelsius).HasPrecision(6, 2);
        builder.Property(incident => incident.TriggerBatteryPercentage).HasPrecision(5, 2);
        builder.Property(incident => incident.TriggeredAt).HasConversion(
            value => value.UtcDateTime,
            value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));
        builder.HasIndex(incident => incident.Status);
        builder.HasIndex(incident => incident.ContainerId);
        builder.HasIndex(incident => incident.TripId);
    }
}