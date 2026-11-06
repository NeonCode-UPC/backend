using MedicalSmartBox.API.Custody.Application;
using MedicalSmartBox.API.Custody.Domain;
using MedicalSmartBox.API.Custody.Infrastructure.Persistence;
using MedicalSmartBox.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(builder.Configuration["Custody:OtpHmacKey"]))
    builder.Configuration["Custody:OtpHmacKey"] = "local-development-only-change-this-key";

builder.Services.AddControllers(options =>
    options.Conventions.Add(new KebabCaseRouteNamingConvention()));
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddSharedApi();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<CustodyApplicationService>();
builder.Services.AddCors(options => options.AddPolicy("frontend", policy => policy
    .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var databaseConfigured = builder.Services.AddSharedPersistence(builder.Configuration);
if (databaseConfigured)
{
    builder.Services.AddScoped<ICustodyRepository, CustodyRepository>();
}
else
{
    if (!builder.Environment.IsDevelopment())
        throw new InvalidOperationException(
            "A MedicalSmartBox database connection must be configured outside Development.");

    builder.Services.AddSingleton<ICustodyRepository, InMemoryCustodyRepository>();
}

var app = builder.Build();
app.UseSharedApi();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseAuthorization();
app.MapControllers();
app.MapSharedEndpoints();

app.Run();
