using MedicalSmartBox.API.Shared.Infrastructure.Interfaces.Asp.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace MedicalSmartBox.API.Shared.Infrastructure.Interfaces.Asp.Configuration;

/// <summary>
/// Registers and exposes HTTP infrastructure shared by every bounded context.
/// </summary>
public static class SharedApiExtensions
{
    public static IServiceCollection AddSharedApi(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddHealthChecks();
        services.AddOpenApi(options => options.AddDocumentTransformer((document, _, _) =>
        {
            document.Info = new()
            {
                Title = "Medical SMARTBOX API",
                Version = "v1.0.0",
                Description = "## IoT Telemetry & Medical Custody Chain Platform\n\nRESTful backend API for **Medical SMARTBOX**, engineered to guarantee thermal preservation, real-time telemetry supervision, and legally immutable chain-of-custody traceability during emergency medical transport of organs, human tissues, and blood components.\n\n### System Architecture\nThe backend implements Domain-Driven Design (DDD) principles with a modular four-layer architecture and strict boundary separation across autonomous Bounded Contexts:\n- **Medical Transport Planning & Dispatching:** Clinical transport order orchestration, clinical urgency prioritization, resource assignment (ambulance vehicle, medical container, driver, paramedic), and route execution.\n- **Smart Container Management:** Inventory tracking, hardware chassis pairing/unlinking with active emergency vehicles, biological payload weight movement audits, and thermal calibration thresholds.\n- **IoT Telemetry Monitoring:** High-frequency time-series telemetry ingestion transmitted by ESP32 embedded microcontrollers (internal/ambient temperature, net payload weight, battery charge, auxiliary 12V vehicle power feed, Peltier cooling duty cycle, lid lock state, and GPS coordinates).\n- **Critical Alerting & Incident Response:** Automated real-time detection of thermal excursions outside the mandatory 2.0°C to 8.0°C window, auxiliary power interruptions, mandatory operator acknowledgement, medical director escalation, and structured contingency resolution.\n- **Chain of Custody & Traceability:** Cryptographically verifiable digital chain of custody, authorized recipient geofence validation, time-based OTP delivery verification (HMAC-SHA256), and digital audit manifests sealed with SHA-256 hashes.\n\n### Standards & Regulatory Compliance\n- **Cold Chain Integrity:** Strict adherence to international and MINSA technical health standards for biological products (2.0°C to 8.0°C preservation range).\n- **Persistence:** Entity Framework Core over MySQL / TiDB Serverless with ACID transactional integrity.\n- **Cryptographic Evidence:** One-time passwords hashed with HMAC-SHA256 and immutable audit manifest digests generated with SHA-256."
            };
            document.Servers = [new OpenApiServer { Url = "/" }];
            document.Tags = new HashSet<OpenApiTag>
            {
                new()
                {
                    Name = "Dispatch Trips",
                    Description = "Comprehensive lifecycle management of emergency medical dispatch trips. Enables assignment of transport resources (ambulances by license plate, smart containers, certified drivers, and paramedics) to approved clinical transport orders, verification of container pre-cooling compliance before departure, live route tracking with dynamic traffic ETA calculations, and destination hospital arrival verification within a 100-meter geofence."
                },
                new()
                {
                    Name = "Transport Orders",
                    Description = "Lifecycle orchestration and clinical prioritization of medical transport orders. Manages transfer requests between origin and destination hospital facilities, clinical priority levels (Routine, Urgent, StatEmergency), biological cargo classification (transplant organs, blood plasma packs, biopsy specimens), cold-ischemia safe thresholds, automated warning margins, and pre-dispatch cancellations."
                },
                new()
                {
                    Name = "Custody Transfers",
                    Description = "Digital chain of custody management and legally immutable transfer of responsibility for medical biological shipments. Provides participant verification, in-transit temperature audit trails, destination hospital geofence validation, time-based one-time password (OTP) issuance and verification for authorized receiving medical personnel, and formal handover acceptance or rejection due to thermal excursion or physical tampering."
                },
                new()
                {
                    Name = "Smart Containers",
                    Description = "Fleet inventory and operational management of IoT-enabled isothermal SmartContainers equipped with ESP32 microcontrollers and environmental telemetry sensors. Supports unit registration by serial number, physical pairing and unlinking with active emergency vehicles, biological payload net weight audit tracking during cargo loading and unloading, and real-time synchronization of thermal and battery metrics."
                },
                new()
                {
                    Name = "Telemetry Logs",
                    Description = "High-frequency time-series IoT telemetry ingestion and query engine transmitted by SmartContainer ESP32 devices. Persists continuous sensor snapshots including internal chamber temperature, ambient temperature, biological payload weight, battery percentage, auxiliary 12V vehicle power feed status, Peltier thermoelectric cooling power level, electromechanical lid lock engagement, GPS coordinates, and cryptographic firmware signatures."
                },
                new()
                {
                    Name = "Critical Incidents",
                    Description = "Proactive real-time monitoring, alerting, and contingency incident response during medical transit. Automatically detects cold-chain thermal breaches (excursions beyond the critical 2.0°C to 8.0°C window), vehicle auxiliary power loss, and suspected physical tampering. Enforces mandatory operator acknowledgements, escalation workflows to medical directors, and structured contingency resolution with quality signoff auditing."
                },
                new()
                {
                    Name = "Digital Audit Manifests",
                    Description = "Issuance and retrieval of cryptographically sealed digital audit manifests for completed custody transfers. Automatically computes average, minimum, and maximum temperatures, total accumulated thermal excursion seconds, compliance verification against MINSA technical standards, and immutable SHA-256 digital seals for clinical governance and regulatory oversight."
                },
                new()
                {
                    Name = "Audit Events",
                    Description = "Immutable chronological security and audit logging ledger. Records timestamped UTC entries detailing the user, executed action, and affected system resource across all critical custody, dispatch, and contingency resolution workflows."
                },
                new()
                {
                    Name = "Custody Traceability",
                    Description = "Consolidated clinical traceability queries, digital audit reports, and on-demand official traceability reports exportable in PDF format for institutional healthcare and regulatory archiving."
                }
            };
            return Task.CompletedTask;
        }));
        return services;
    }

    public static IApplicationBuilder UseSharedApi(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();
        return app;
    }

    public static IEndpointRouteBuilder MapSharedEndpoints(
        this IEndpointRouteBuilder endpoints,
        IHostEnvironment environment,
        bool exposeOpenApiInProduction = false)
    {
        endpoints.MapHealthChecks("/health");

        var exposeOpenApi = environment.IsDevelopment() || exposeOpenApiInProduction;
        if (exposeOpenApi)
        {
            endpoints.MapOpenApi();
            endpoints.MapScalarApiReference(options => options.WithTitle("Medical SmartBox API Documentation"));
        }

        endpoints.MapGet("/", (IHostEnvironment hostEnvironment) =>
        {
            if (hostEnvironment.IsDevelopment() || exposeOpenApiInProduction)
                return Results.Redirect("/scalar");

            return Results.Ok(new
            {
                name = "MedicalSmartBox API",
                status = "running",
                health = "/health",
                openApi = (string?)null,
                documentation = (string?)null
            });
        })
            .WithTags("API Overview")
            .WithSummary("Open API documentation")
            .WithDescription("Redirects to the interactive API documentation when it is enabled.")
            .Produces(StatusCodes.Status200OK)
            .ExcludeFromDescription();

        return endpoints;
    }
}
