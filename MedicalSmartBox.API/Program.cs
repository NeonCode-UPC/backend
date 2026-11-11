using MedicalSmartBox.API.Transport.Domain.Repositories;
using MedicalSmartBox.API.Transport.Application.CommandServices;
using MedicalSmartBox.API.Transport.Application.Internal.CommandServices;
using MedicalSmartBox.API.Transport.Application.Internal.QueryServices;
using MedicalSmartBox.API.Transport.Application.QueryServices;
using MedicalSmartBox.API.Transport.Infrastructure.ExternalServices.TrafficRouting;
using MedicalSmartBox.API.Transport.Infrastructure.Persistence.InMemory;
using MedicalSmartBox.API.Transport.Infrastructure.Persistence.Repositories;
using MedicalSmartBox.API.alerting.Application.Internal.CommandServices;
using MedicalSmartBox.API.alerting.Application.Internal.QueryServices;
using MedicalSmartBox.API.alerting.Domain.Repositories;
using MedicalSmartBox.API.alerting.Domain.Services;
using MedicalSmartBox.API.alerting.Infrastructure.Persistence;
using MedicalSmartBox.API.alerting.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using MedicalSmartBox.API.Custody.Application.CommandServices;
using MedicalSmartBox.API.Custody.Application.QueryServices;
using MedicalSmartBox.API.Custody.Application.Internal.CommandServices;
using MedicalSmartBox.API.Custody.Application.Internal.QueryServices;
using MedicalSmartBox.API.Custody.Domain.Repositories;
using MedicalSmartBox.API.Custody.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using MedicalSmartBox.API.Custody.Infrastructure.Persistence.InMemory.Repositories;
using MedicalSmartBox.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using MedicalSmartBox.API.Telemetry.Application.CommandServices;
using MedicalSmartBox.API.Telemetry.Application.Internal.CommandServices;
using MedicalSmartBox.API.Telemetry.Application.Internal.QueryServices;
using MedicalSmartBox.API.Telemetry.Application.QueryServices;
using MedicalSmartBox.API.Telemetry.Domain.Repositories;
using MedicalSmartBox.API.Telemetry.Infrastructure.Persistence;
using MedicalSmartBox.API.Telemetry.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using MedicalSmartBox.API.SmartContainers.Application.CommandServices;
using MedicalSmartBox.API.SmartContainers.Application.Internal.CommandServices;
using MedicalSmartBox.API.SmartContainers.Application.Internal.QueryServices;
using MedicalSmartBox.API.SmartContainers.Application.QueryServices;
using MedicalSmartBox.API.SmartContainers.Domain.Repositories;
using MedicalSmartBox.API.SmartContainers.Infrastructure.Persistence;
using MedicalSmartBox.API.SmartContainers.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(builder.Configuration["Custody:OtpHmacKey"]))
    builder.Configuration["Custody:OtpHmacKey"] = "local-development-only-change-this-key";

builder.Services.AddControllers(options =>
    options.Conventions.Add(new KebabCaseRouteNamingConvention()));
builder.Services.AddAuthorization();
builder.Services.AddSharedApi();
builder.Services.AddSingleton(TimeProvider.System);

// Alerting Application Services
builder.Services.AddScoped<IIncidentCommandService, IncidentCommandService>();
builder.Services.AddScoped<IIncidentQueryService, IncidentQueryService>();

// Custody Application Services
builder.Services.AddScoped<ICustodyCommandService, CustodyCommandService>();
builder.Services.AddScoped<ICustodyQueryService, CustodyQueryService>();

// Telemetry Application Services
builder.Services.AddScoped<ITelemetryLogCommandService, TelemetryLogCommandService>();
builder.Services.AddScoped<ITelemetryLogQueryService, TelemetryLogQueryService>();

// SmartContainer Application Services
builder.Services.AddScoped<ISmartContainerCommandService, SmartContainerCommandService>();
builder.Services.AddScoped<ISmartContainerQueryService, SmartContainerQueryService>();

// Transport Application Services
builder.Services.AddScoped<ITransportCommandService, TransportCommandService>();
builder.Services.AddScoped<ITransportQueryService, TransportQueryService>();
builder.Services.AddHttpClient<ITrafficRoutingService, TomTomTrafficRoutingService>(client =>
    client.BaseAddress = new Uri("https://api.tomtom.com/"));

builder.Services.AddCors(options => options.AddPolicy("frontend", policy => policy
    .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var databaseConfigured = builder.Services.AddSharedPersistence(builder.Configuration);
if (databaseConfigured)
{
    builder.Services.AddScoped<ICustodyRepository, CustodyRepository>();
    // Transport
    builder.Services.AddScoped<ITransportRepository, TransportRepository>();
    builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
    builder.Services.AddScoped<ITelemetryLogRepository, TelemetryLogRepository>();
    builder.Services.AddScoped<ISmartContainerRepository, SmartContainerRepository>();
}
else
{
    builder.Services.AddScoped<IUnitOfWork, MedicalSmartBox.API.Telemetry.Infrastructure.Persistence.InMemoryUnitOfWork>();
    builder.Services.AddSingleton<ICustodyRepository, InMemoryCustodyRepository>();
    builder.Services.AddSingleton<IIncidentRepository, InMemoryIncidentRepository>();
    builder.Services.AddSingleton<ITelemetryLogRepository, InMemoryTelemetryLogRepository>();
    builder.Services.AddSingleton<ISmartContainerRepository, InMemorySmartContainerRepository>();
    builder.Services.AddSingleton<ITransportRepository, InMemoryTransportRepository>();
}

var app = builder.Build();
app.UseSharedApi();

app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseAuthorization();
app.MapControllers();
app.MapSharedEndpoints(app.Environment);

app.Run();
