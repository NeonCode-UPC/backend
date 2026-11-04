using System.Text.Json;
using MedicalSmartBox.API.Custody.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalSmartBox.API.Custody.Infrastructure.Persistence.Configuration;

public sealed class CustodyTransferConfiguration : IEntityTypeConfiguration<CustodyTransfer>
{
    public void Configure(EntityTypeBuilder<CustodyTransfer> entity)
    {
        entity.ToTable("custody_transfers");
        entity.HasKey(transfer => transfer.Id);
        entity.Property(transfer => transfer.Id).HasColumnType("char(36)").ValueGeneratedNever();
        entity.Property(transfer => transfer.TransferCode).HasMaxLength(30).IsRequired();
        entity.HasIndex(transfer => transfer.TransferCode).IsUnique();
        entity.Property(transfer => transfer.TripId).HasColumnType("char(36)").ValueGeneratedNever();
        entity.HasIndex(transfer => transfer.TripId).IsUnique();
        entity.Property(transfer => transfer.RecipientHospitalId).HasColumnType("char(36)").ValueGeneratedNever();
        entity.Property(transfer => transfer.AuthorizedRecipientUserId).HasColumnType("char(36)").ValueGeneratedNever();
        entity.Property(transfer => transfer.Route).HasMaxLength(160).IsRequired();
        entity.Property(transfer => transfer.Cargo).HasMaxLength(160).IsRequired();
        entity.Property(transfer => transfer.SmartBox).HasMaxLength(40).IsRequired();
        entity.Property(transfer => transfer.Recipient).HasMaxLength(160);
        entity.Property(transfer => transfer.RecipientNotes).HasMaxLength(500);
        entity.Property(transfer => transfer.Status).HasConversion<string>().HasMaxLength(40).IsRequired();
        entity.Property(transfer => transfer.TemperatureMax).HasPrecision(4, 2);
        entity.Property(transfer => transfer.ReceivedAt).HasColumnName("completed_at");
        entity.Property(transfer => transfer.CreatedAt).HasColumnName("created_at");
        entity.Property(transfer => transfer.UpdatedAt).HasColumnName("updated_at");
        entity.Property<List<CustodyEvent>>("_events")
            .HasField("_events")
            .HasColumnName("events_json")
            .HasColumnType("json")
            .HasConversion(
                events => JsonSerializer.Serialize(events, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<List<CustodyEvent>>(json, (JsonSerializerOptions?)null) ?? new List<CustodyEvent>());
        entity.Property<List<CustodyTemperatureReading>>("_temperatureReadings")
            .HasField("_temperatureReadings")
            .HasColumnName("temperature_readings_json")
            .HasColumnType("json")
            .HasConversion(
                readings => JsonSerializer.Serialize(readings, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<List<CustodyTemperatureReading>>(json, (JsonSerializerOptions?)null) ?? new List<CustodyTemperatureReading>());

        entity.OwnsOne(transfer => transfer.OtpToken, otp =>
        {
            otp.ToTable("custody_otp_tokens");
            otp.WithOwner().HasForeignKey("custody_transfer_id");
            otp.HasKey("custody_transfer_id");
            otp.Property(token => token.HashedCode).HasColumnName("otp_code_hash").HasMaxLength(64).IsRequired(false);
            otp.Property(token => token.ExpiresAtUtc).HasColumnName("otp_expires_at");
            otp.Property(token => token.CurrentAttempts).HasColumnName("otp_attempt_count");
            otp.Property(token => token.VerifiedAtUtc).HasColumnName("otp_verified_at");
            otp.Ignore(token => token.MaxAttemptsAllowed);
        });
        entity.Navigation(transfer => transfer.OtpToken).IsRequired(false);
    }
}
