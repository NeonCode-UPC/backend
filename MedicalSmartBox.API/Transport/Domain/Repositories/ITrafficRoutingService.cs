using MedicalSmartBox.API.Transport.Domain.ValueObjects;
namespace MedicalSmartBox.API.Transport.Domain.Repositories;

public interface ITrafficRoutingService
{
    Task<DateTime> GetDynamicEtaAsync(GeoLocation origin, GeoLocation dest, CancellationToken cancellationToken = default);
    Task<int> CheckTrafficDelaysAsync(string polyline, CancellationToken cancellationToken = default);
}
