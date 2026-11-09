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

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(builder.Configuration["Custody:OtpHmacKey"]))
    builder.Configuration["Custody:OtpHmacKey"] = "local-development-only-change-this-key";

builder.Services.AddControllers(options =>
    options.Conventions.Add(new KebabCaseRouteNamingConvention()));
builder.Services.AddAuthorization();
builder.Services.AddSharedApi();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<ICustodyCommandService, CustodyCommandService>();
builder.Services.AddScoped<ICustodyQueryService, CustodyQueryService>();

// Telemetry Application Services
builder.Services.AddScoped<ITelemetryLogCommandService, TelemetryLogCommandService>();
builder.Services.AddScoped<ITelemetryLogQueryService, TelemetryLogQueryService>();
builder.Services.AddCors(options => options.AddPolicy("frontend", policy => policy
    .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var databaseConfigured = builder.Services.AddSharedPersistence(builder.Configuration);
if (databaseConfigured)
{
    builder.Services.AddScoped<ICustodyRepository, CustodyRepository>();
    builder.Services.AddScoped<ITelemetryLogRepository, TelemetryLogRepository>();
}
else
{
    if (!builder.Environment.IsDevelopment())
        throw new InvalidOperationException(
            "A MedicalSmartBox database connection must be configured outside Development.");

    builder.Services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
    builder.Services.AddSingleton<ICustodyRepository, InMemoryCustodyRepository>();
    builder.Services.AddSingleton<ITelemetryLogRepository, InMemoryTelemetryLogRepository>();
}

var app = builder.Build();
app.UseSharedApi();

app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseAuthorization();
app.MapControllers();
app.MapSharedEndpoints(app.Environment);

app.Run();
