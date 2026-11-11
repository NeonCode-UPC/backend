using System.Text.Json;
using MedicalSmartBox.API.Custody.Domain.Model.Entities;
using MedicalSmartBox.API.Custody.Domain.Model.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalSmartBox.API.Custody.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public sealed class DigitalAuditManifestConfiguration : IEntityTypeConfiguration<DigitalAuditManifest>
{
    public void Configure(EntityTypeBuilder<DigitalAuditManifest> entity)
    {
        entity.ToTable("digital_audit_manifests");
        entity.HasKey(manifest => manifest.Id);
        entity.Property(manifest => manifest.Id).HasColumnType("char(36)").ValueGeneratedNever();
        entity.Property(manifest => manifest.TransferId).HasColumnType("char(36)").IsRequired();
        entity.HasIndex(manifest => manifest.TransferId).IsUnique();
        entity.Property(manifest => manifest.ManifestCode).HasMaxLength(30).IsRequired();
        entity.HasIndex(manifest => manifest.ManifestCode).IsUnique();
        entity.Property(manifest => manifest.CryptographicHashSha256).HasColumnName("cryptographic_hash_sha256")
            .HasColumnType("char(64)").IsRequired();
        entity.Property(manifest => manifest.IsSealedAndImmutable).HasColumnName("is_sealed_and_immutable");
        entity.Property(manifest => manifest.Recipient).HasMaxLength(160).IsRequired();
        entity.Property(manifest => manifest.CloudStorageUrl).HasMaxLength(500);
        entity.Property(manifest => manifest.AverageTemperatureCelsius).HasPrecision(4, 2);
        entity.Property(manifest => manifest.MinTemperatureCelsius).HasPrecision(4, 2);
        entity.Property(manifest => manifest.MaxTemperatureCelsius).HasPrecision(4, 2);
        entity.Property(manifest => manifest.TotalExcursionSeconds).HasDefaultValue(0);
        entity.Property(manifest => manifest.MinsaComplianceVerified).HasColumnName("minsa_compliance_verified");
        entity.Property(manifest => manifest.GeneratedAt).HasColumnName("sealed_at");
        entity.Ignore(manifest => manifest.Events);
        entity.Property<List<CustodyEvent>>("_events")
            .HasField("_events")
            .HasColumnName("events_json")
            .HasColumnType("json")
            .HasConversion(
                events => JsonSerializer.Serialize(events, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<List<CustodyEvent>>(json, (JsonSerializerOptions?)null) ?? new List<CustodyEvent>());
    }
}
