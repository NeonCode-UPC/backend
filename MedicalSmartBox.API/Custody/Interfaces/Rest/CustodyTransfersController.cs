using MedicalSmartBox.API.Custody.Application.CommandServices;
using MedicalSmartBox.API.Custody.Application.QueryServices;
using MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Resources;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MedicalSmartBox.API.Custody.Interfaces.Rest;

[ApiController]
[Route("api/v1/custody-transfers")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Custody Transfers")]
public sealed class CustodyTransfersController(ICustodyCommandService commandService, ICustodyQueryService queryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get custody transfers")]
    [EndpointDescription("Retrieves all custody transfers, including their current status, participants, temperature readings, and recorded events.")]
    [ProducesResponseType(typeof(IReadOnlyList<CustodyTransferResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CustodyTransferResource>>> GetAll(CancellationToken cancellationToken) =>
        Ok((await queryService.ListTransfersAsync(cancellationToken)).Select(CustodyResourceFromEntityAssembler.ToResourceFromEntity));

    [HttpGet("{identifier}")]
    [EndpointSummary("Get a custody transfer by ID")]
    [EndpointDescription("Retrieves the custody transfer identified by its transfer code.")]
    [ProducesResponseType(typeof(CustodyTransferResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustodyTransferResource>> GetById(string identifier, CancellationToken cancellationToken)
    {
        var transfer = await queryService.FindTransferAsync(identifier, cancellationToken);
        return transfer is null ? NotFound() : Ok(CustodyResourceFromEntityAssembler.ToResourceFromEntity(transfer));
    }

    [HttpPost("{identifier}/temperature-readings")]
    [EndpointSummary("Record custody temperature readings")]
    [EndpointDescription("Appends temperature measurements captured during the custody transfer.")]
    [ProducesResponseType(typeof(CustodyTransferResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [EndpointSummary("Create a custody transfer")]
    [EndpointDescription("Creates a custody transfer with its origin, destination, shipment, and participants.")]
    [ProducesResponseType(typeof(CustodyTransferResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustodyTransferResource>> Create(CreateCustodyTransferResource request, CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateCustodyTransferCommandFromResourceAssembler.ToCommandFromResource(request);
            var transfer = await commandService.CreateTransferAsync(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { identifier = transfer.TransferCode },
                CustodyResourceFromEntityAssembler.ToResourceFromEntity(transfer));
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{identifier}/reception")]
    [EndpointSummary("Prepare custody reception")]
    [EndpointDescription("Registers the recipient and validates the destination geofence before generating a one-time verification code.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [EndpointSummary("Verify the custody reception code")]
    [EndpointDescription("Checks the one-time code provided by the recipient and returns verification status and remaining attempts.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [EndpointSummary("Accept a custody transfer")]
    [EndpointDescription("Confirms delivery after reception checks and creates the digital audit manifest for the completed transfer.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [EndpointSummary("Reject a transfer for thermal excursion")]
    [EndpointDescription("Closes the custody transfer as rejected because its temperature readings exceeded the allowed range.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> RejectThermalExcursion(string identifier, RejectDeliveryResource request, CancellationToken cancellationToken) =>
        Reject(identifier, request, thermalExcursion: true, cancellationToken);

    [HttpPost("{identifier}/rejections/tampering")]
    [EndpointSummary("Reject a transfer for tampering")]
    [EndpointDescription("Closes the custody transfer as rejected when package tampering is reported.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
