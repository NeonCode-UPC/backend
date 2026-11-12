using MedicalSmartBox.API.Transport.Domain.Repositories;
using MedicalSmartBox.API.Transport.Application.CommandServices;
using MedicalSmartBox.API.Transport.Application.Internal.CommandServices;
using MedicalSmartBox.API.Transport.Application.Internal.QueryServices;
using MedicalSmartBox.API.Transport.Application.QueryServices;
using MedicalSmartBox.API.Transport.Infrastructure.ExternalServices.TrafficRouting;
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
using MedicalSmartBox.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using MedicalSmartBox.API.Telemetry.Application.CommandServices;
using MedicalSmartBox.API.Telemetry.Application.Internal.CommandServices;
using MedicalSmartBox.API.Telemetry.Application.Internal.QueryServices;
using MedicalSmartBox.API.Telemetry.Application.QueryServices;
using MedicalSmartBox.API.Telemetry.Domain.Repositories;
using MedicalSmartBox.API.Telemetry.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using MedicalSmartBox.API.SmartContainers.Application.CommandServices;
using MedicalSmartBox.API.SmartContainers.Application.Internal.CommandServices;
using MedicalSmartBox.API.SmartContainers.Application.Internal.QueryServices;
using MedicalSmartBox.API.SmartContainers.Application.QueryServices;
using MedicalSmartBox.API.SmartContainers.Domain.Repositories;
using MedicalSmartBox.API.SmartContainers.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (int.TryParse(builder.Configuration["PORT"], out var renderPort))
    builder.WebHost.UseUrls($"http://0.0.0.0:{renderPort}");

if (builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(builder.Configuration["Custody:OtpHmacKey"]))
    builder.Configuration["Custody:OtpHmacKey"] = "local-development-only-change-this-key";

builder.Services.AddControllers(options =>
    options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
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

var configuredOrigins = builder.Configuration["Cors:AllowedOrigins"];
var allowedOrigins = string.IsNullOrWhiteSpace(configuredOrigins)
    ? new[] { "http://localhost:5173", "http://127.0.0.1:5173" }
    : configuredOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options => options.AddPolicy("frontend", policy =>
{
    if (string.IsNullOrWhiteSpace(configuredOrigins))
        policy.SetIsOriginAllowed(_ => true);
    else
        policy.WithOrigins(allowedOrigins);

    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
}));

builder.Services.AddSharedPersistence(builder.Configuration);
builder.Services.AddScoped<ICustodyRepository, CustodyRepository>();
builder.Services.AddScoped<ITransportRepository, TransportRepository>();
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<ITelemetryLogRepository, TelemetryLogRepository>();
builder.Services.AddScoped<ISmartContainerRepository, SmartContainerRepository>();

var app = builder.Build();
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseSharedApi();

if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseAuthorization();
app.MapControllers();
app.MapSharedEndpoints(app.Environment, builder.Configuration.GetValue<bool>("OpenApi:ExposeInProduction"));

app.Run();
