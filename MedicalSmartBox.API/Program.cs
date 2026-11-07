using MedicalSmartBox.API.Transport.Domain.Repositories;
using MedicalSmartBox.API.Transport.Infrastructure.Persistence.Repositories;
using MedicalSmartBox.API.Custody.Application;
using MedicalSmartBox.API.Custody.Domain;
using MedicalSmartBox.API.Custody.Infrastructure.Persistence;
using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(builder.Configuration["Custody:OtpHmacKey"]))
    builder.Configuration["Custody:OtpHmacKey"] = "local-development-only-change-this-key";

builder.Services.AddControllers(options =>
    options.Conventions.Add(new KebabCaseRouteNamingConvention()));
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<CustodyApplicationService>();
builder.Services.AddCors(options => options.AddPolicy("frontend", policy => policy
    .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var connectionString = builder.Configuration.GetConnectionString("MedicalSmartBox");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(
        options => options.UseMySQL(connectionString));

    // Shared
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped(
        typeof(IBaseRepository<,>),
        typeof(BaseRepository<,>));

    // Custody
    builder.Services.AddScoped<ICustodyRepository, CustodyRepository>();

    // Transport
    builder.Services.AddScoped<ITransportRepository, TransportRepository>();
}
else
{
    builder.Services.AddSingleton<ICustodyRepository, InMemoryCustodyRepository>();
}


var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
