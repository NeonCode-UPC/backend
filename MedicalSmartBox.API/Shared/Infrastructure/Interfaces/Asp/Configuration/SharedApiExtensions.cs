using MedicalSmartBox.API.Shared.Infrastructure.Interfaces.Asp.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Routing;
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
                Title = "Medical SmartBox API",
                Version = "v1",
                Description = "API for managing cold-chain custody transfers, shipment traceability, audit events, and IoT telemetry."
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
        IHostEnvironment environment)
    {
        endpoints.MapHealthChecks("/health");

        if (environment.IsDevelopment())
        {
            endpoints.MapOpenApi();
            endpoints.MapScalarApiReference(options => options.WithTitle("Medical SmartBox API Documentation"));
        }

        endpoints.MapGet("/", (IHostEnvironment hostEnvironment) => Results.Ok(new
        {
            name = "MedicalSmartBox API",
            status = "running",
            health = "/health",
            openApi = hostEnvironment.IsDevelopment() ? "/openapi/v1.json" : null,
            documentation = hostEnvironment.IsDevelopment() ? "/scalar" : null
        }))
            .WithTags("API Overview")
            .WithSummary("Check API status and find useful links")
            .WithDescription("Confirms that Medical SmartBox API is running. Use /health for a health check, /scalar for interactive documentation, and /openapi/v1.json to view the API specification.")
            .Produces(StatusCodes.Status200OK)
            .ExcludeFromDescription();

        return endpoints;
    }
}
