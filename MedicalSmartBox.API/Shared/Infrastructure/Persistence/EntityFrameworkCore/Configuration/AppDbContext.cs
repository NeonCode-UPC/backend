using System.Text.Json;
using MedicalSmartBox.API.Custody.Domain;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CustodyTransfer> CustodyTransfers => Set<CustodyTransfer>();
    public DbSet<DigitalAuditManifest> DigitalAuditManifests => Set<DigitalAuditManifest>();

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseSnakeCaseNamingConvention();

        modelBuilder.Entity<CustodyTransfer>(entity =>
        {
            entity.ToTable("custody_transfers");
            entity.HasKey(transfer => transfer.Id);
            entity.Property(transfer => transfer.Id).ValueGeneratedNever();
            entity.HasIndex(transfer => transfer.TransferCode).IsUnique();
            entity.HasIndex(transfer => transfer.TripId).IsUnique();
            entity.Property(transfer => transfer.TransferCode).HasMaxLength(30).IsRequired();
            entity.Property(transfer => transfer.TripId).ValueGeneratedNever();
            entity.Property(transfer => transfer.RecipientHospitalId).ValueGeneratedNever();
            entity.Property(transfer => transfer.AuthorizedRecipientUserId).ValueGeneratedNever();
            entity.Property(transfer => transfer.Route).HasMaxLength(160).IsRequired();
            entity.Property(transfer => transfer.Cargo).HasMaxLength(160).IsRequired();
            entity.Property(transfer => transfer.SmartBox).HasMaxLength(40).IsRequired();
            entity.Property(transfer => transfer.Recipient).HasMaxLength(160);
            entity.Property(transfer => transfer.RecipientNotes).HasMaxLength(500);
            entity.Property(transfer => transfer.Status).HasConversion<string>().HasMaxLength(40);
            entity.Property(transfer => transfer.TemperatureMax).HasPrecision(4, 2);
            entity.Property(transfer => transfer.ReceivedAt).HasColumnName("completed_at");
            entity.Property<List<CustodyEvent>>("_events")
                .HasField("_events")
                .HasColumnName("events_json")
                .HasConversion(
                    events => JsonSerializer.Serialize(events, (JsonSerializerOptions?)null),
                    json => JsonSerializer.Deserialize<List<CustodyEvent>>(json, (JsonSerializerOptions?)null) ?? []);
            entity.OwnsOne(transfer => transfer.OtpToken, otp =>
            {
                otp.Property(token => token.HashedCode).HasColumnName("otp_code_hash").HasMaxLength(128).IsRequired(false);
                otp.Property(token => token.ExpiresAtUtc).HasColumnName("otp_expires_at");
                otp.Property(token => token.CurrentAttempts).HasColumnName("otp_attempt_count");
                otp.Property(token => token.VerifiedAtUtc).HasColumnName("otp_verified_at");
                otp.Ignore(token => token.MaxAttemptsAllowed);
            });
            entity.Navigation(transfer => transfer.OtpToken).IsRequired(false);
        });

        modelBuilder.Entity<DigitalAuditManifest>(entity =>
        {
            entity.ToTable("digital_audit_manifests");
            entity.HasKey(manifest => manifest.Id);
            entity.HasIndex(manifest => manifest.TransferId).IsUnique();
            entity.Property(manifest => manifest.Id).ValueGeneratedNever();
            entity.Property(manifest => manifest.ManifestCode).HasMaxLength(30).IsRequired();
            entity.HasIndex(manifest => manifest.ManifestCode).IsUnique();
            entity.Property(manifest => manifest.CryptographicHashSha256).HasMaxLength(64).IsRequired();
            entity.Property(manifest => manifest.IsSealedAndImmutable).HasColumnName("is_sealed_and_immutable");
            entity.Property(manifest => manifest.Recipient).HasMaxLength(160).IsRequired();
            entity.Property(manifest => manifest.CloudStorageUrl).HasMaxLength(500);
            entity.Property(manifest => manifest.AverageTemperatureCelsius).HasPrecision(4, 2);
            entity.Property(manifest => manifest.MinTemperatureCelsius).HasPrecision(4, 2);
            entity.Property(manifest => manifest.MaxTemperatureCelsius).HasPrecision(4, 2);
            entity.Property<List<CustodyEvent>>("_events")
                .HasField("_events")
                .HasColumnName("events_json")
                .HasConversion(
                    events => JsonSerializer.Serialize(events, (JsonSerializerOptions?)null),
                    json => JsonSerializer.Deserialize<List<CustodyEvent>>(json, (JsonSerializerOptions?)null) ?? []);
        });
    }

}
