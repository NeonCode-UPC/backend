
using MedicalSmartBox.API.Transport.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalSmartBox.API.Transport.Infrastructure.Persistence.Configuration;

public class TransportOrderConfiguration : IEntityTypeConfiguration<TransportOrder>
{
    public void Configure(EntityTypeBuilder<TransportOrder> builder)
    {
        builder.ToTable("transport_orders");

        // Primary key
        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .IsRequired();

        // Order information
        builder.Property(order => order.OrderCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(order => order.OrderCode)
            .IsUnique()
            .HasDatabaseName("uq_transport_orders_code");

        builder.HasIndex(order => new { order.Status, order.Priority })
            .HasDatabaseName("idx_transport_orders_status_priority");

        builder.Property(order => order.OriginHospitalId)
            .IsRequired();

        builder.Property(order => order.DestinationHospitalId)
            .IsRequired();

        // Audit properties
        builder.Property(order => order.CreatedAt);

        builder.Property(order => order.UpdatedAt);

        // Priority and status
        builder.Property(order => order.Priority)
            .HasColumnName("clinical_priority")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(order => order.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        // Cargo information
        builder.Property(order => order.CargoType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(order => order.CargoDescription)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(order => order.CreatedByUserId)
            .IsRequired();

        // IschemiaTimeLimit owned value object
        builder.OwnsOne(order => order.MaxIschemiaLimit, ischemia =>
        {
            ischemia.WithOwner().HasForeignKey("id");
            ischemia.HasKey("id");
            ischemia.Property<Guid>("id")
                .HasColumnName("id")
                .IsRequired();

            ischemia.Property(limit => limit.MaxSafeHours)
                .HasColumnName("max_ischemia_hours")
                .HasPrecision(4, 2)
                .IsRequired();

            ischemia.Property(limit => limit.WarningThresholdHours)
                .HasColumnName("ischemia_warning_hours")
                .HasPrecision(4, 2)
                .IsRequired();
        });

        builder.Navigation(order => order.MaxIschemiaLimit)
            .IsRequired();
    }
}
