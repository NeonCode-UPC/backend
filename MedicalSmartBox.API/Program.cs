using MedicalSmartBox.API.Custody.Application;
using MedicalSmartBox.API.Custody.Domain;
using MedicalSmartBox.API.Custody.Infrastructure.Persistence;
using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using MedicalSmartBox.API.Telemetry.Application.CommandServices;
using MedicalSmartBox.API.Telemetry.Application.Internal.CommandServices;
using MedicalSmartBox.API.Telemetry.Application.Internal.QueryServices;
using MedicalSmartBox.API.Telemetry.Application.QueryServices;
using MedicalSmartBox.API.Telemetry.Domain.Repositories;
using MedicalSmartBox.API.Telemetry.Infrastructure.Persistence;
using MedicalSmartBox.API.Telemetry.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(builder.Configuration["Custody:OtpHmacKey"]))
    builder.Configuration["Custody:OtpHmacKey"] = "local-development-only-change-this-key";

builder.Services.AddControllers(options =>
    options.Conventions.Add(new KebabCaseRouteNamingConvention()));
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<CustodyApplicationService>();

// Telemetry Application Services
builder.Services.AddScoped<ITelemetryLogCommandService, TelemetryLogCommandService>();
builder.Services.AddScoped<ITelemetryLogQueryService, TelemetryLogQueryService>();

builder.Services.AddCors(options => options.AddPolicy("frontend", policy => policy
    .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var connectionString = builder.Configuration.GetConnectionString("MedicalSmartBox");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString));
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
    builder.Services.AddScoped<ICustodyRepository, CustodyRepository>();
    builder.Services.AddScoped<ITelemetryLogRepository, TelemetryLogRepository>();
}
else
{
    builder.Services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
    builder.Services.AddSingleton<ICustodyRepository, InMemoryCustodyRepository>();
    builder.Services.AddSingleton<ITelemetryLogRepository, InMemoryTelemetryLogRepository>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
