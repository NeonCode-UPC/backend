namespace MedicalSmartBox.API.Transport.Application.Errors;

public enum TransportError
{
    InvalidRequest,
    NotFound,
    DuplicateOrderCode,
    OrderAlreadyHasTrip,
    IschemiaLimitExceeded,
    InvalidState,
    TrafficServiceUnavailable
}
