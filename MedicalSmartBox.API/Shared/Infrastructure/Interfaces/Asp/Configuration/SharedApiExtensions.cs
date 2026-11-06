using MedicalSmartBox.API.Shared.Infrastructure.Interfaces.Asp.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Routing;

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
        return services;
    }

    public static IApplicationBuilder UseSharedApi(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();
        return app;
    }

    public static IEndpointRouteBuilder MapSharedEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health");
        return endpoints;
    }
}
