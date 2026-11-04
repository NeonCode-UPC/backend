using MedicalSmartBox.API.Custody.Application;
using MedicalSmartBox.API.Custody.Domain;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.Custody.Presentation;

[ApiController]
[Route("api/v1/custody-transfers")]
public sealed class CustodyTransfersController(CustodyApplicationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustodyTransferDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok((await service.ListTransfersAsync(cancellationToken)).Select(CustodyContractMapper.ToDto));

    [HttpGet("{identifier}")]
    public async Task<ActionResult<CustodyTransferDto>> GetById(string identifier, CancellationToken cancellationToken)
    {
        var transfer = await service.FindTransferAsync(identifier, cancellationToken);
        return transfer is null ? NotFound() : Ok(CustodyContractMapper.ToDto(transfer));
    }

    [HttpPost("{identifier}/temperature-readings")]
    public async Task<IActionResult> RegisterTemperatureReadings(string identifier, RegisterTemperatureReadingsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var transfer = await service.RegisterTemperatureReadingsAsync(identifier, request.Readings, cancellationToken);
            return Ok(CustodyContractMapper.ToDto(transfer));
        }
        catch (Exception exception) when (exception is KeyNotFoundException or ArgumentException or InvalidOperationException)
        {
            return ToError(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult<CustodyTransferDto>> Create(CreateCustodyTransferRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateCustodyTransferCommand(request.TransferCode, request.TripId,
                request.RecipientHospitalId, request.AuthorizedRecipientUserId, request.Route, request.Cargo,
                request.SmartBox, request.TemperatureMax, request.Openings, request.DepartureAt, request.TemperatureReadings);
            var transfer = await service.CreateTransferAsync(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { identifier = transfer.TransferCode }, CustodyContractMapper.ToDto(transfer));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{identifier}/reception")]
    public async Task<IActionResult> PrepareReception(string identifier, PrepareReceptionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var (transfer, developmentCode) = await service.PrepareReceptionAsync(identifier,
                request.Recipient, request.InsideDestinationGeofence, cancellationToken);
            return Ok(new { transfer = CustodyContractMapper.ToDto(transfer), developmentCode });
        }
        catch (Exception exception) when (exception is KeyNotFoundException or ArgumentException or InvalidOperationException)
        {
            return ToError(exception);
        }
    }

    [HttpPost("{identifier}/otp-verifications")]
    public async Task<IActionResult> VerifyOtp(string identifier, VerifyOtpRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var verified = await service.VerifyOtpAsync(identifier, request.Code, cancellationToken);
            var transfer = await service.FindTransferAsync(identifier, cancellationToken);
            var attempts = transfer?.OtpToken?.CurrentAttempts ?? 0;
            return Ok(new
            {
                verified,
                status = verified ? "otp-verified" : "pending-otp",
                attempts,
                remainingAttempts = Math.Max(0, 3 - attempts),
                message = verified ? null : "Código incorrecto."
            });
        }
        catch (Exception exception) when (exception is KeyNotFoundException or ArgumentException or InvalidOperationException)
        {
            return ToError(exception);
        }
    }

    [HttpPost("{identifier}/acceptance")]
    public async Task<IActionResult> Accept(string identifier, AcceptReceptionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var (transfer, manifest) = await service.ConfirmReceptionAsync(identifier, request.RecipientNotes,
                request.HasUnresolvedCriticalAlerts, cancellationToken);
            return Ok(new
            {
                transfer = CustodyContractMapper.ToDto(transfer),
                manifest = CustodyContractMapper.ToDto(manifest, transfer)
            });
        }
        catch (Exception exception) when (exception is KeyNotFoundException or ArgumentException or InvalidOperationException)
        {
            return ToError(exception);
        }
    }

    [HttpPost("{identifier}/rejections/thermal-excursion")]
    public Task<IActionResult> RejectThermalExcursion(string identifier, RejectDeliveryRequest request, CancellationToken cancellationToken) =>
        Reject(identifier, request, thermalExcursion: true, cancellationToken);

    [HttpPost("{identifier}/rejections/tampering")]
    public Task<IActionResult> RejectTampering(string identifier, RejectDeliveryRequest request, CancellationToken cancellationToken) =>
        Reject(identifier, request, thermalExcursion: false, cancellationToken);

    private async Task<IActionResult> Reject(string identifier, RejectDeliveryRequest request, bool thermalExcursion, CancellationToken cancellationToken)
    {
        try
        {
            var transfer = await service.RejectDeliveryAsync(identifier, request.Reason, thermalExcursion, cancellationToken);
            return Ok(CustodyContractMapper.ToDto(transfer));
        }
        catch (Exception exception) when (exception is KeyNotFoundException or ArgumentException or InvalidOperationException)
        {
            return ToError(exception);
        }
    }

    private IActionResult ToError(Exception exception) => exception switch
    {
        KeyNotFoundException => NotFound(new { message = exception.Message }),
        _ => BadRequest(new { message = exception.Message })
    };
}

public sealed record CreateCustodyTransferRequest(string TransferCode, Guid TripId, Guid RecipientHospitalId,
    Guid AuthorizedRecipientUserId, string Route, string Cargo, string SmartBox, decimal TemperatureMax,
    int Openings, DateTimeOffset DepartureAt, IReadOnlyList<CustodyTemperatureReading>? TemperatureReadings);
public sealed record RegisterTemperatureReadingsRequest(IReadOnlyList<CustodyTemperatureReading> Readings);

public sealed record PrepareReceptionRequest(string Recipient, bool InsideDestinationGeofence);
public sealed record VerifyOtpRequest(string Code);
public sealed record AcceptReceptionRequest(string? RecipientNotes, bool HasUnresolvedCriticalAlerts = false);
public sealed record RejectDeliveryRequest(string Reason);
