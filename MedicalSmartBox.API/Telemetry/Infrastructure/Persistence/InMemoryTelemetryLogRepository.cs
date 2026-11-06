using MedicalSmartBox.API.Telemetry.Domain.Model.Aggregates;
using MedicalSmartBox.API.Telemetry.Domain.Repositories;

namespace MedicalSmartBox.API.Telemetry.Infrastructure.Persistence;

/// <summary>
///     Volatile in-memory repository for local development and Postman testing without a database connection.
/// </summary>
public class InMemoryTelemetryLogRepository : ITelemetryLogRepository
{
    private readonly List<TelemetryLog> _logs = [];
    private int _nextId = 1;

    public InMemoryTelemetryLogRepository()
    {
        // Seed default cold-chain readings for SB-0182 and SB-0197
        var now = DateTime.UtcNow;

        var log1 = new TelemetryLog(
            "SB-0182",
            "TR-0417",
            now.AddMinutes(-30),
            4.2m,
            22.1m,
            18400.0m,
            85.0m,
            true,
            65,
            true,
            -12.0931m,
            -77.0024m,
            "ESP32-SHA256-INITIAL-01");
        log1.Id = _nextId++;
        log1.CreatedAt = now.AddMinutes(-30);
        _logs.Add(log1);

        var log2 = new TelemetryLog(
            "SB-0182",
            "TR-0417",
            now.AddMinutes(-10),
            4.4m,
            22.4m,
            18400.0m,
            83.0m,
            true,
            65,
            true,
            -12.0945m,
            -77.0012m,
            "ESP32-SHA256-INITIAL-02");
        log2.Id = _nextId++;
        log2.CreatedAt = now.AddMinutes(-10);
        _logs.Add(log2);

        var log3 = new TelemetryLog(
            "SB-0197",
            "TR-0398",
            now.AddMinutes(-5),
            5.1m,
            23.0m,
            15200.0m,
            91.0m,
            true,
            60,
            true,
            -12.1102m,
            -76.9921m,
            "ESP32-SHA256-INITIAL-03");
        log3.Id = _nextId++;
        log3.CreatedAt = now.AddMinutes(-5);
        _logs.Add(log3);
    }

    public Task AddAsync(TelemetryLog entity, CancellationToken cancellationToken = default)
    {
        entity.Id = _nextId++;
        entity.CreatedAt = DateTimeOffset.UtcNow;
        _logs.Add(entity);
        return Task.CompletedTask;
    }

    public Task<TelemetryLog?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var log = _logs.FirstOrDefault(l => l.Id == id);
        return Task.FromResult(log);
    }

    public void Update(TelemetryLog entity)
    {
        var index = _logs.FindIndex(l => l.Id == entity.Id);
        if (index >= 0)
        {
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            _logs[index] = entity;
        }
    }

    public void Remove(TelemetryLog entity)
    {
        _logs.RemoveAll(l => l.Id == entity.Id);
    }

    public Task<IEnumerable<TelemetryLog>> ListAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<TelemetryLog>>(_logs.OrderByDescending(l => l.TimestampUtc).ToList());
    }

    public Task<TelemetryLog?> FindLatestByContainerIdAsync(string containerId, CancellationToken cancellationToken = default)
    {
        var log = _logs
            .Where(l => l.ContainerId.Equals(containerId, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(l => l.TimestampUtc)
            .FirstOrDefault();
        return Task.FromResult(log);
    }

    public Task<IEnumerable<TelemetryLog>> FindByContainerIdAsync(string containerId, CancellationToken cancellationToken = default)
    {
        var logs = _logs
            .Where(l => l.ContainerId.Equals(containerId, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(l => l.TimestampUtc)
            .ToList();
        return Task.FromResult<IEnumerable<TelemetryLog>>(logs);
    }

    public Task<IEnumerable<TelemetryLog>> FindByTripIdAsync(string tripId, CancellationToken cancellationToken = default)
    {
        var logs = _logs
            .Where(l => l.TripId != null && l.TripId.Equals(tripId, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(l => l.TimestampUtc)
            .ToList();
        return Task.FromResult<IEnumerable<TelemetryLog>>(logs);
    }
}
