using MedicalSmartBox.API.Transport.Domain.Aggregates;
using MedicalSmartBox.API.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalSmartBox.API.Transport.Infrastructure.Persistence.Configuration;

public class DispatchTripConfiguration : IEntityTypeConfiguration<DispatchTrip>
{
    public void Configure(EntityTypeBuilder<DispatchTrip> builder)
    {
        builder.ToTable("dispatch_trips");

        // Primary key
        builder.HasKey(trip => trip.Id);

        builder.Property(trip => trip.Id)
            .IsRequired();

        // Audit properties
        builder.Property(trip => trip.CreatedAt);

        builder.Property(trip => trip.UpdatedAt);

        // Transport order reference
        builder.Property(trip => trip.OrderId)
            .IsRequired();

        // Assigned resources
        builder.Property(trip => trip.AssignedVehiclePlate)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(trip => trip.AssignedContainerId)
            .IsRequired();

        builder.Property(trip => trip.AssignedDriverUserId)
            .IsRequired();

        builder.Property(trip => trip.AssignedParamedicUserId)
            .IsRequired();

        // Trip status
        builder.Property(trip => trip.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        // Departure and arrival times
        builder.Property(trip => trip.ScheduledDepartureTime)
            .IsRequired();

        builder.Property(trip => trip.ActualDepartureTime);

        builder.Property(trip => trip.EstimatedArrivalTime)
            .IsRequired();

        builder.Property(trip => trip.ActualArrivalTime);

        // TransportRoute owned entity
        builder.OwnsOne<TransportRoute>("_route", route =>
        {
            route.Property(r => r.DistanceKm)
                .HasColumnName("distance_km")
                .IsRequired();

            route.Property(r => r.PlannedDurationMinutes)
                .HasColumnName("planned_duration_minutes")
                .IsRequired();

            route.Property(r => r.TrafficDelayMinutes)
                .HasColumnName("traffic_delay_minutes")
                .IsRequired();

            route.Property(r => r.EncodedPolyline)
                .HasColumnName("encoded_polyline")
                .HasColumnType("text");
        });

        builder.Navigation("_route")
            .IsRequired();

        // Destination location owned value object
        builder.OwnsOne(trip => trip.DestinationLocation, location =>
        {
            location.Property(loc => loc.Latitude)
                .HasColumnName("destination_latitude")
                .HasPrecision(10, 7)
                .IsRequired();

            location.Property(loc => loc.Longitude)
                .HasColumnName("destination_longitude")
                .HasPrecision(10, 7)
                .IsRequired();
        });

        builder.Navigation(trip => trip.DestinationLocation)
            .IsRequired();
    }
}
