using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalSmartBox.API.Transport.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("202611110001_AddTransportDestinationCoordinates")]
public sealed class AddTransportDestinationCoordinates : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "destination_latitude",
            table: "dispatch_trips",
            type: "decimal(10,8)",
            precision: 10,
            scale: 8,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "destination_longitude",
            table: "dispatch_trips",
            type: "decimal(11,8)",
            precision: 11,
            scale: 8,
            nullable: true);

        migrationBuilder.Sql("""
            UPDATE dispatch_trips AS trip
            INNER JOIN transport_orders AS transport_order ON transport_order.id = trip.order_id
            INNER JOIN hospital_institutions AS hospital ON hospital.id = transport_order.destination_hospital_id
            SET trip.destination_latitude = hospital.latitude,
                trip.destination_longitude = hospital.longitude;
            """);

        migrationBuilder.AlterColumn<decimal>(
            name: "destination_latitude",
            table: "dispatch_trips",
            type: "decimal(10,8)",
            precision: 10,
            scale: 8,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(10,8)",
            oldPrecision: 10,
            oldScale: 8,
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "destination_longitude",
            table: "dispatch_trips",
            type: "decimal(11,8)",
            precision: 11,
            scale: 8,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(11,8)",
            oldPrecision: 11,
            oldScale: 8,
            oldNullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "destination_latitude", table: "dispatch_trips");
        migrationBuilder.DropColumn(name: "destination_longitude", table: "dispatch_trips");
    }
}
