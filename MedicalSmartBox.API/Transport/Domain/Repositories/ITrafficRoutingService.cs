using MedicalSmartBox.API.Transport.Domain.ValueObjects;
namespace MedicalSmartBox.API.Transport.Domain.Repositories;

public interface ITrafficRoutingService
{
    Task<DateTime> GetDynamicEtaAsync(GeoLocation origin, GeoLocation dest);
    Task<int> CheckTrafficDelaysAsync(string polyline);
}