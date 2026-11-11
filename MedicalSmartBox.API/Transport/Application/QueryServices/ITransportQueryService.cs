using MedicalSmartBox.API.Transport.Application.Queries;
using MedicalSmartBox.API.Transport.Application.ReadModels;

namespace MedicalSmartBox.API.Transport.Application.QueryServices;

public interface ITransportQueryService
{
    Task<TransportOrderReadModel?> Handle(GetTransportOrderByIdQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransportOrderReadModel>> Handle(GetTransportOrdersQuery query, CancellationToken cancellationToken = default);
    Task<DispatchTripReadModel?> Handle(GetTripByIdQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DispatchTripReadModel>> Handle(GetActiveTripsQuery query, CancellationToken cancellationToken = default);
}
