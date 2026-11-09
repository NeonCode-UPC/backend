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
        services.AddOpenApi();
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
            endpoints.MapScalarApiReference();
        }

        endpoints.MapGet("/", (IHostEnvironment hostEnvironment) => Results.Ok(new
        {
            name = "MedicalSmartBox API",
            status = "running",
            health = "/health",
            openApi = hostEnvironment.IsDevelopment() ? "/openapi/v1.json" : null,
            documentation = hostEnvironment.IsDevelopment() ? "/scalar" : null
        }));

        return endpoints;
    }
}
