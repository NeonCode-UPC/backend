using MedicalSmartBox.API.Custody.Application.CommandServices;
using MedicalSmartBox.API.Custody.Application.QueryServices;
using MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Resources;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.Custody.Interfaces.Rest;

[ApiController]
[Route("api/v1/custody-transfers")]
public sealed class CustodyTransfersController(CustodyCommandService commandService, CustodyQueryService queryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustodyTransferResource>>> GetAll(CancellationToken cancellationToken) =>
        Ok((await queryService.ListTransfersAsync(cancellationToken)).Select(CustodyResourceFromEntityAssembler.ToResourceFromEntity));

    [HttpGet("{identifier}")]
    public async Task<ActionResult<CustodyTransferResource>> GetById(string identifier, CancellationToken cancellationToken)
    {
        var transfer = await queryService.FindTransferAsync(identifier, cancellationToken);
        return transfer is null ? NotFound() : Ok(CustodyResourceFromEntityAssembler.ToResourceFromEntity(transfer));
    }

    [HttpPost("{identifier}/temperature-readings")]
    public async Task<IActionResult> RegisterTemperatureReadings(string identifier, RegisterTemperatureReadingsResource request,
        CancellationToken cancellationToken)
    {
        try
        {
            var readings = request.Readings
                .Select(reading => new CustodyTemperatureReading(reading.RecordedAt, reading.TemperatureCelsius));
            var transfer = await commandService.RegisterTemperatureReadingsAsync(identifier, readings, cancellationToken);
            return Ok(CustodyResourceFromEntityAssembler.ToResourceFromEntity(transfer));
        }
        catch (Exception exception) when (exception is KeyNotFoundException or ArgumentException or InvalidOperationException)
        {
            return ToError(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult<CustodyTransferResource>> Create(CreateCustodyTransferResource request, CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateCustodyTransferCommandFromResourceAssembler.ToCommandFromResource(request);
            var transfer = await commandService.CreateTransferAsync(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { identifier = transfer.TransferCode },
                CustodyResourceFromEntityAssembler.ToResourceFromEntity(transfer));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{identifier}/reception")]
    public async Task<IActionResult> PrepareReception(string identifier, PrepareReceptionResource request, CancellationToken cancellationToken)
    {
        try
        {
            var (transfer, developmentCode) = await commandService.PrepareReceptionAsync(identifier,
                request.Recipient, request.InsideDestinationGeofence, cancellationToken);
            return Ok(new { transfer = CustodyResourceFromEntityAssembler.ToResourceFromEntity(transfer), developmentCode });
        }
        catch (Exception exception) when (exception is KeyNotFoundException or ArgumentException or InvalidOperationException)
        {
            return ToError(exception);
        }
    }

    [HttpPost("{identifier}/otp-verifications")]
    public async Task<IActionResult> VerifyOtp(string identifier, VerifyOtpResource request, CancellationToken cancellationToken)
    {
        try
        {
            var verified = await commandService.VerifyOtpAsync(identifier, request.Code, cancellationToken);
            var transfer = await queryService.FindTransferAsync(identifier, cancellationToken);
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
    public async Task<IActionResult> Accept(string identifier, AcceptReceptionResource request, CancellationToken cancellationToken)
    {
        try
        {
            var (transfer, manifest) = await commandService.ConfirmReceptionAsync(identifier, request.RecipientNotes,
                request.HasUnresolvedCriticalAlerts, cancellationToken);
            return Ok(new
            {
                transfer = CustodyResourceFromEntityAssembler.ToResourceFromEntity(transfer),
                manifest = CustodyResourceFromEntityAssembler.ToResourceFromEntity(manifest, transfer)
            });
        }
        catch (Exception exception) when (exception is KeyNotFoundException or ArgumentException or InvalidOperationException)
        {
            return ToError(exception);
        }
    }

    [HttpPost("{identifier}/rejections/thermal-excursion")]
    public Task<IActionResult> RejectThermalExcursion(string identifier, RejectDeliveryResource request, CancellationToken cancellationToken) =>
        Reject(identifier, request, thermalExcursion: true, cancellationToken);

    [HttpPost("{identifier}/rejections/tampering")]
    public Task<IActionResult> RejectTampering(string identifier, RejectDeliveryResource request, CancellationToken cancellationToken) =>
        Reject(identifier, request, thermalExcursion: false, cancellationToken);

    private async Task<IActionResult> Reject(string identifier, RejectDeliveryResource request, bool thermalExcursion, CancellationToken cancellationToken)
    {
        try
        {
            var transfer = await commandService.RejectDeliveryAsync(identifier, request.Reason, thermalExcursion, cancellationToken);
            return Ok(CustodyResourceFromEntityAssembler.ToResourceFromEntity(transfer));
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
