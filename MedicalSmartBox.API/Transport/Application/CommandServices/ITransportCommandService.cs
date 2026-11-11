using MedicalSmartBox.API.Shared.Application.Patterns;
using MedicalSmartBox.API.Transport.Application.Commands;
using MedicalSmartBox.API.Transport.Application.Errors;
using MedicalSmartBox.API.Transport.Domain.Aggregates;

namespace MedicalSmartBox.API.Transport.Application.CommandServices;

public interface ITransportCommandService
{
    Task<Result<TransportOrder, TransportError>> Handle(CreateTransportOrderCommand command, CancellationToken cancellationToken = default);
    Task<Result<DispatchTrip, TransportError>> Handle(AssignTripResourcesCommand command, CancellationToken cancellationToken = default);
    Task<Result<DispatchTrip, TransportError>> Handle(StartTripCommand command, CancellationToken cancellationToken = default);
    Task<Result<DispatchTrip, TransportError>> Handle(UpdateDynamicEtaCommand command, CancellationToken cancellationToken = default);
    Task<Result<DispatchTrip, TransportError>> Handle(RegisterHospitalArrivalCommand command, CancellationToken cancellationToken = default);
    Task<Result<DispatchTrip, TransportError>> Handle(CompleteTripCommand command, CancellationToken cancellationToken = default);
    Task<Result<TransportOrder, TransportError>> Handle(CancelTransportOrderCommand command, CancellationToken cancellationToken = default);
}
