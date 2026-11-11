using MedicalSmartBox.API.alerting.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalSmartBox.API.alerting.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public class ContingencyResolutionConfiguration : IEntityTypeConfiguration<ContingencyResolution>
{
    public void Configure(EntityTypeBuilder<ContingencyResolution> builder)
    {
        builder.ToTable("contingency_resolutions");
        builder.HasKey(resolution => resolution.Id);
        builder.Property(resolution => resolution.Id).ValueGeneratedNever();
        builder.Property(resolution => resolution.MitigationAction).HasMaxLength(500).IsRequired();
        builder.Property(resolution => resolution.QualitySignoffNotes).IsRequired();
        builder.Property(resolution => resolution.ResolvedAt).HasConversion(
            value => value.UtcDateTime,
            value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));
        builder.HasIndex(resolution => resolution.IncidentId).IsUnique();
    }
}
