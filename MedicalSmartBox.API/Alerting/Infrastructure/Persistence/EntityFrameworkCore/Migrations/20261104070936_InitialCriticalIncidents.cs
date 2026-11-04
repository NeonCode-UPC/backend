using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalSmartBox.API.alerting.Infrastructure.Persistence.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCriticalIncidents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "critical_incidents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TripId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ContainerId = table.Column<Guid>(type: "char(36)", nullable: false),
                    IncidentType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Severity = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    TriggerTemperatureCelsius = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    TriggerBatteryPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    EscalationLevel = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    TriggeredAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AcknowledgedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    AcknowledgedByUserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_critical_incidents", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_critical_incidents_ContainerId",
                table: "critical_incidents",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_critical_incidents_Status",
                table: "critical_incidents",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_critical_incidents_TripId",
                table: "critical_incidents",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "critical_incidents");
        }
    }
}
