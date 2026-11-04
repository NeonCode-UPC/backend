using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace MedicalSmartBox.Tests;

public class IncidentApiTests
{
    private static WebApplicationFactory<Program> CreateFactory(SqliteConnection connection)
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
                services.RemoveAll<AppDbContext>();
                services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));
            });
        });
    }

    [Fact]
    public async Task CreateAndRetrieveIncident()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var factory = CreateFactory(connection);
        using (var scope = factory.Services.CreateScope())
            await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync();
        using var client = factory.CreateClient();
        var payload = new { tripId = Guid.NewGuid(), containerId = Guid.NewGuid(), incidentType = "coldChainBreachHigh", severity = "criticalEmergency", triggerTemperatureCelsius = 9.5m };
        var response = await client.PostAsJsonAsync("/api/v1/incidents", payload);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        using var created = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("triggered", created.RootElement.GetProperty("status").GetString());
        var result = await client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        var list = await client.GetFromJsonAsync<JsonElement>("/api/v1/incidents/active");
        Assert.Equal(1, list.GetArrayLength());
    }

    [Fact]
    public async Task InvalidInputReturnsBadRequestAndUnknownIdReturnsNotFound()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var factory = CreateFactory(connection);
        using (var scope = factory.Services.CreateScope())
            await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync();
        using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/v1/incidents", new { })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/v1/incidents", new { tripId = Guid.NewGuid(), containerId = Guid.NewGuid(), incidentType = "invalid", severity = "criticalEmergency" })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/v1/incidents", new { tripId = Guid.Empty, containerId = Guid.NewGuid(), incidentType = "coldChainBreachHigh", severity = "criticalEmergency" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/v1/incidents/{Guid.NewGuid()}")).StatusCode);
    }
}