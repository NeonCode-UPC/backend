using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;
using MedicalSmartBox.API.alerting.Domain.Model.Enums;
using MedicalSmartBox.API.alerting.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MedicalSmartBox.Tests;

public class IncidentRepositoryTests
{
    [Fact]
    public async Task CanSaveAndReadIncidentsFromDatabase()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();
        var repository = new IncidentRepository(context);
        var incident = new CriticalIncident(Guid.NewGuid(), Guid.NewGuid(), IncidentType.ColdChainBreachHigh, IncidentSeverity.CriticalEmergency, 9.5m);
        await repository.AddAsync(incident);
        await new UnitOfWork(context).CompleteAsync();
        context.ChangeTracker.Clear();
        var found = await repository.FindByIdAsync(incident.Id);
        Assert.NotNull(found);
        Assert.Equal(incident.Id, found.Id);
        Assert.Equal(9.5m, found.TriggerTemperatureCelsius);
        Assert.NotNull(found.CreatedAt);
        var active = await repository.GetActiveIncidentsAsync();
        Assert.Single(active);
        Assert.Equal(incident.Id, active[0].Id);
    }

    [Fact]
    public async Task MissingIncidentReturnsNull()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();
        Assert.Null(await new IncidentRepository(context).FindByIdAsync(Guid.NewGuid()));
    }
}