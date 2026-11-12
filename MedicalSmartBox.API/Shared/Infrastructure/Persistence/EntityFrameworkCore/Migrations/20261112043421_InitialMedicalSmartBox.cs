using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class InitialMedicalSmartBox : Migration
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
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    trip_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    container_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    incident_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    severity = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    trigger_temperature_celsius = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    trigger_battery_percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    escalation_level = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    triggered_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    acknowledged_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    acknowledged_by_user_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_critical_incidents", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "custody_transfers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    transfer_code = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    trip_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    recipient_hospital_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    authorized_recipient_user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    route = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    cargo = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    smart_box = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    departure_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    temperature_max = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    openings = table.Column<int>(type: "int", nullable: false),
                    recipient = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true),
                    recipient_notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    events_json = table.Column<string>(type: "json", nullable: false),
                    temperature_readings_json = table.Column<string>(type: "json", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_custody_transfers", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "digital_audit_manifests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    transfer_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    manifest_code = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    sealed_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    recipient = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    average_temperature_celsius = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    min_temperature_celsius = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    max_temperature_celsius = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    total_excursion_seconds = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    minsa_compliance_verified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cryptographic_hash_sha256 = table.Column<string>(type: "char(64)", nullable: false),
                    cloud_storage_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    is_sealed_and_immutable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    events_json = table.Column<string>(type: "json", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_digital_audit_manifests", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "smart_containers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    serial_number = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    form_factor = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    lock_state = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    assigned_ambulance_plate = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true),
                    current_temperature_celsius = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    min_temperature_celsius = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false, defaultValue: 2.00m),
                    max_temperature_celsius = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false, defaultValue: 8.00m),
                    warning_temperature_celsius = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false, defaultValue: 7.50m),
                    excursion_tolerance_seconds = table.Column<int>(type: "int", nullable: false, defaultValue: 120),
                    tare_weight_grams = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    net_weight_grams = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    battery_percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    battery_warning_percentage = table.Column<int>(type: "int", nullable: false),
                    signal_quality = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    is12_v_connected = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    last_telemetry_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_smart_containers", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "telemetry_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    container_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    trip_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    timestamp_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    temperature_celsius = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ambient_temperature_celsius = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    weight_grams = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    battery_percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    is12_v_connected = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    peltier_power_pct = table.Column<int>(type: "int", nullable: false),
                    lid_lock_engaged = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    latitude = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: true),
                    longitude = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: true),
                    firmware_signature = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    is_thermal_excursion = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telemetry_logs", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transport_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    order_code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    origin_hospital_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    destination_hospital_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    clinical_priority = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    cargo_type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    cargo_description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    max_ischemia_hours = table.Column<double>(type: "double", precision: 4, scale: 2, nullable: false),
                    ischemia_warning_hours = table.Column<double>(type: "double", precision: 4, scale: 2, nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transport_orders", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "contingency_resolutions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    incident_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    mitigation_action = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    quality_signoff_notes = table.Column<string>(type: "longtext", nullable: false),
                    was_thermal_integrity_restored = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    resolved_by_user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    resolved_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contingency_resolutions", x => x.id);
                    table.ForeignKey(
                        name: "fk_contingency_resolutions_critical_incidents_incident_id",
                        column: x => x.incident_id,
                        principalTable: "critical_incidents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "custody_otp_tokens",
                columns: table => new
                {
                    custody_transfer_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    otp_code_hash = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    otp_expires_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    otp_attempt_count = table.Column<int>(type: "int", nullable: false),
                    otp_verified_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_custody_otp_tokens", x => x.custody_transfer_id);
                    table.ForeignKey(
                        name: "fk_custody_otp_tokens_custody_transfers_custody_transfer_id",
                        column: x => x.custody_transfer_id,
                        principalTable: "custody_transfers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "dispatch_trips",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    order_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    assigned_vehicle_plate = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    assigned_container_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    assigned_driver_user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    assigned_paramedic_user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    scheduled_departure_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    actual_departure_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    estimated_arrival_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    actual_arrival_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    destination_latitude = table.Column<decimal>(type: "decimal(10,8)", precision: 10, scale: 8, nullable: false),
                    destination_longitude = table.Column<decimal>(type: "decimal(11,8)", precision: 11, scale: 8, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    distance_km = table.Column<double>(type: "double", precision: 6, scale: 2, nullable: false),
                    planned_duration_minutes = table.Column<int>(type: "int", nullable: false),
                    current_delay_minutes = table.Column<int>(type: "int", nullable: false),
                    polyline_coordinates = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dispatch_trips", x => x.id);
                    table.ForeignKey(
                        name: "fk_dispatch_trips_transport_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "transport_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_contingency_resolutions_incident_id",
                table: "contingency_resolutions",
                column: "incident_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_critical_incidents_container_id",
                table: "critical_incidents",
                column: "container_id");

            migrationBuilder.CreateIndex(
                name: "ix_critical_incidents_status",
                table: "critical_incidents",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_critical_incidents_trip_id",
                table: "critical_incidents",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_custody_transfers_transfer_code",
                table: "custody_transfers",
                column: "transfer_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_custody_transfers_trip_id",
                table: "custody_transfers",
                column: "trip_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_digital_audit_manifests_manifest_code",
                table: "digital_audit_manifests",
                column: "manifest_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_digital_audit_manifests_transfer_id",
                table: "digital_audit_manifests",
                column: "transfer_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_dispatch_trips_order",
                table: "dispatch_trips",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_smart_containers_serial_number",
                table: "smart_containers",
                column: "serial_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_telemetry_logs_container_id",
                table: "telemetry_logs",
                column: "container_id");

            migrationBuilder.CreateIndex(
                name: "ix_telemetry_logs_timestamp_utc",
                table: "telemetry_logs",
                column: "timestamp_utc");

            migrationBuilder.CreateIndex(
                name: "ix_telemetry_logs_trip_id",
                table: "telemetry_logs",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "idx_transport_orders_status_priority",
                table: "transport_orders",
                columns: new[] { "status", "clinical_priority" });

            migrationBuilder.CreateIndex(
                name: "uq_transport_orders_code",
                table: "transport_orders",
                column: "order_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contingency_resolutions");

            migrationBuilder.DropTable(
                name: "custody_otp_tokens");

            migrationBuilder.DropTable(
                name: "digital_audit_manifests");

            migrationBuilder.DropTable(
                name: "dispatch_trips");

            migrationBuilder.DropTable(
                name: "smart_containers");

            migrationBuilder.DropTable(
                name: "telemetry_logs");

            migrationBuilder.DropTable(
                name: "critical_incidents");

            migrationBuilder.DropTable(
                name: "custody_transfers");

            migrationBuilder.DropTable(
                name: "transport_orders");
        }
    }
}
