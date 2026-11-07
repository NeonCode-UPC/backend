
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
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(order => order.OriginHospitalId)
            .IsRequired();

        builder.Property(order => order.DestinationHospitalId)
            .IsRequired();

        // Audit properties
        builder.Property(order => order.CreatedAt);

        builder.Property(order => order.UpdatedAt);

        // Priority and status
        builder.Property(order => order.Priority)
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
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(order => order.CreatedByUserId)
            .IsRequired();

        // IschemiaTimeLimit owned value object
        builder.OwnsOne(order => order.MaxIschemiaLimit, ischemia =>
        {
            ischemia.Property(limit => limit.MaxSafeHours)
                .HasColumnName("max_safe_hours")
                .IsRequired();

            ischemia.Property(limit => limit.WarningHours)
                .HasColumnName("warning_threshold_hours")
                .IsRequired();
        });

        builder.Navigation(order => order.MaxIschemiaLimit)
            .IsRequired();
    }
}
