using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;
using MedicalSmartBox.API.alerting.Domain.Model.Enums;
using Xunit;

namespace MedicalSmartBox.Tests;

public class CriticalIncidentTests
{
    [Fact]
    public void NewIncidentStartsTriggeredAtOperationalLevel()
    {
        var trip = Guid.NewGuid();
        var container = Guid.NewGuid();
        var incident = new CriticalIncident(trip, container, IncidentType.ColdChainBreachHigh,
            IncidentSeverity.CriticalEmergency, 9.5m, 75m);
        Assert.NotEqual(Guid.Empty, incident.Id);
        Assert.Equal(trip, incident.TripId);
        Assert.Equal(container, incident.ContainerId);
        Assert.Equal(IncidentStatus.Triggered, incident.Status);
        Assert.Equal(EscalationLevel.OperationalCabAlert, incident.EscalationLevel);
        Assert.Equal(9.5m, incident.TriggerTemperatureCelsius);
        Assert.Equal(75m, incident.TriggerBatteryPercentage);
        Assert.Null(incident.AcknowledgedAt);
    }

    [Fact]
    public void EmptyIdentifiersAreRejected()
    {
        Assert.Throws<ArgumentException>(() => new CriticalIncident(Guid.Empty, Guid.NewGuid(), IncidentType.ColdChainBreachHigh, IncidentSeverity.CriticalEmergency));
        Assert.Throws<ArgumentException>(() => new CriticalIncident(Guid.NewGuid(), Guid.Empty, IncidentType.ColdChainBreachHigh, IncidentSeverity.CriticalEmergency));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void BatteryOutsidePercentageRangeIsRejected(int battery)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CriticalIncident(Guid.NewGuid(), Guid.NewGuid(), IncidentType.Auxiliary12VPowerLost, IncidentSeverity.ModerateAlert, batteryPercentage: battery));
    }

    [Fact]
    public void UnknownClassificationIsRejected()
    {
        Assert.Throws<ArgumentException>(() => new CriticalIncident(Guid.NewGuid(), Guid.NewGuid(), (IncidentType)999, IncidentSeverity.ModerateAlert));
        Assert.Throws<ArgumentException>(() => new CriticalIncident(Guid.NewGuid(), Guid.NewGuid(), IncidentType.ColdChainBreachLow, (IncidentSeverity)999));
    }
}