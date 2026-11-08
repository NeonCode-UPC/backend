using MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;
using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;
using MedicalSmartBox.API.SmartContainers.Domain.Repositories;

namespace MedicalSmartBox.API.SmartContainers.Infrastructure.Persistence;

/// <summary>
///     Thread-safe in-memory repository for local frontend integration and presentation testing.
/// </summary>
public class InMemorySmartContainerRepository : ISmartContainerRepository
{
    private readonly List<SmartContainer> _containers = [];
    private readonly Lock _syncLock = new();
    private int _nextId = 1;

    public InMemorySmartContainerRepository()
    {
        SeedDefaultContainers();
    }

    private void SeedDefaultContainers()
    {
        // 1. SB-0182: Standard 20L box linked to ambulance AMB-104 (InTransit)
        var box1 = new SmartContainer("SB-0182", EFormFactorContainer.StandardBox_20L, 10000.00m, 4.20m);
        box1.Id = new ContainerId(_nextId++);
        box1.PairToAmbulance("AMB-104");
        box1.RecordWeightMovement(2400.00m);
        box1.UpdateTelemetry(4.20m, 85.00m, true, 2400.00m, "strong", DateTimeOffset.UtcNow.AddMinutes(-5));
        box1.CreatedAt = DateTimeOffset.UtcNow.AddDays(-3);
        _containers.Add(box1);

        // 2. SB-0197: Standard 20L box linked to ambulance AMB-208 (InTransit)
        var box2 = new SmartContainer("SB-0197", EFormFactorContainer.StandardBox_20L, 10000.00m, 5.10m);
        box2.Id = new ContainerId(_nextId++);
        box2.PairToAmbulance("AMB-208");
        box2.RecordWeightMovement(1800.00m);
        box2.UpdateTelemetry(5.10m, 91.00m, true, 1800.00m, "strong", DateTimeOffset.UtcNow.AddMinutes(-12));
        box2.CreatedAt = DateTimeOffset.UtcNow.AddDays(-2);
        _containers.Add(box2);

        // 3. SMB-BOX-2026-0042: Small 5L box available at base
        var box3 = new SmartContainer("SMB-BOX-2026-0042", EFormFactorContainer.SmallBox_5L, 5000.00m, 3.80m);
        box3.Id = new ContainerId(_nextId++);
        box3.UpdateTelemetry(3.80m, 98.00m, false, 0.00m, "strong", DateTimeOffset.UtcNow.AddMinutes(-20));
        box3.CreatedAt = DateTimeOffset.UtcNow.AddDays(-1);
        _containers.Add(box3);
    }

    public Task AddAsync(SmartContainer entity, CancellationToken cancellationToken = default)
    {
        lock (_syncLock)
        {
            if (entity.Id.Value == 0)
                entity.Id = new ContainerId(_nextId++);

            entity.CreatedAt = DateTimeOffset.UtcNow;
            _containers.Add(entity);
        }
        return Task.CompletedTask;
    }

    public Task<SmartContainer?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        lock (_syncLock)
        {
            var item = _containers.FirstOrDefault(c => c.Id.Value == id);
            return Task.FromResult(item);
        }
    }

    public void Update(SmartContainer entity)
    {
        lock (_syncLock)
        {
            var index = _containers.FindIndex(c => c.Id.Value == entity.Id.Value);
            if (index >= 0)
            {
                entity.UpdatedAt = DateTimeOffset.UtcNow;
                _containers[index] = entity;
            }
        }
    }

    public void Remove(SmartContainer entity)
    {
        lock (_syncLock)
        {
            _containers.RemoveAll(c => c.Id.Value == entity.Id.Value);
        }
    }

    public Task<IEnumerable<SmartContainer>> ListAsync(CancellationToken cancellationToken = default)
    {
        lock (_syncLock)
        {
            return Task.FromResult<IEnumerable<SmartContainer>>(_containers.ToList());
        }
    }

    public Task<SmartContainer?> FindBySerialNumberAsync(SerialNumber serialNumber, CancellationToken cancellationToken = default)
    {
        lock (_syncLock)
        {
            var item = _containers.FirstOrDefault(c =>
                c.SerialNumber.Value.Equals(serialNumber.Value, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(item);
        }
    }

    public Task<SmartContainer?> FindByAssignedAmbulancePlateAsync(string ambulancePlate, CancellationToken cancellationToken = default)
    {
        lock (_syncLock)
        {
            var item = _containers.FirstOrDefault(c =>
                c.AssignedAmbulancePlate != null &&
                c.AssignedAmbulancePlate.Equals(ambulancePlate, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(item);
        }
    }

    public Task<bool> ExistsBySerialNumberAsync(SerialNumber serialNumber, CancellationToken cancellationToken = default)
    {
        lock (_syncLock)
        {
            var exists = _containers.Any(c =>
                c.SerialNumber.Value.Equals(serialNumber.Value, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(exists);
        }
    }
}
