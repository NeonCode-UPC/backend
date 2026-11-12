using MedicalSmartBox.API.Custody.Domain.Model.Aggregates;
using MedicalSmartBox.API.Custody.Domain.Model.Commands;
using MedicalSmartBox.API.Custody.Domain.Model.Entities;
using MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.Custody.Application.CommandServices;

public interface ICustodyCommandService
{
    Task<CustodyTransfer> CreateTransferAsync(CreateCustodyTransferCommand command, CancellationToken cancellationToken = default);
    Task<(CustodyTransfer Transfer, string? DevelopmentCode)> PrepareReceptionAsync(string identifier, string recipient, bool insideDestinationGeofence, CancellationToken cancellationToken = default);
    Task<CustodyTransfer> RegisterTemperatureReadingsAsync(string identifier, IEnumerable<CustodyTemperatureReading> readings, CancellationToken cancellationToken = default);
    Task<bool> VerifyOtpAsync(string identifier, string code, CancellationToken cancellationToken = default);
    Task<(CustodyTransfer Transfer, DigitalAuditManifest Manifest)> ConfirmReceptionAsync(string identifier, string? notes, bool hasUnresolvedCriticalAlerts, CancellationToken cancellationToken = default);
    Task<CustodyTransfer> RejectDeliveryAsync(string identifier, string reason, bool thermalExcursion, CancellationToken cancellationToken = default);
}
