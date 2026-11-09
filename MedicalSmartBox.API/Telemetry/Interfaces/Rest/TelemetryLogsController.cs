using System.Net.Mime;
using MedicalSmartBox.API.Telemetry.Application.CommandServices;
using MedicalSmartBox.API.Telemetry.Application.QueryServices;
using MedicalSmartBox.API.Telemetry.Domain.Model.Queries;
using MedicalSmartBox.API.Telemetry.Interfaces.Rest.Resources;
using MedicalSmartBox.API.Telemetry.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.Telemetry.Interfaces.Rest;

/// <summary>
///     REST controller exposing IoT telemetry ingestion and time-series query endpoints.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Telemetry Logs")]
public class TelemetryLogsController(
    ITelemetryLogCommandService telemetryLogCommandService,
    ITelemetryLogQueryService telemetryLogQueryService)
    : ControllerBase
{
    /// <summary>
    ///     Records a new IoT telemetry sensor snapshot transmitted by ESP32 microcontroller.
    /// </summary>
    [HttpPost]
    [EndpointSummary("Record a new IoT telemetry snapshot")]
    [EndpointDescription("Receives and persists environmental and mechanical sensor readings from a SmartContainer ESP32 device.")]
    [ProducesResponseType(typeof(TelemetryLogResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTelemetryLog(
        [FromBody] CreateTelemetryLogResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateTelemetryLogCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await telemetryLogCommandService.Handle(command, cancellationToken);

        return ActionResultFromCreateTelemetryLogResultAssembler.ToActionResult(
            result,
            this,
            nameof(GetTelemetryLogById));
    }

    /// <summary>
    ///     Retrieves a single telemetry sensor snapshot by its database identifier.
    /// </summary>
    [HttpGet("{id:int}")]
    [EndpointSummary("Get telemetry log by ID")]
    [EndpointDescription("Retrieves a single telemetry sensor snapshot by its monotonic database identifier.")]
    [ProducesResponseType(typeof(TelemetryLogResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTelemetryLogById(int id, CancellationToken cancellationToken)
    {
        var query = new GetTelemetryLogByIdQuery(id);
        var result = await telemetryLogQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();

        var resource = TelemetryLogResourceFromEntityAssembler.ToResourceFromEntity(result);
        return Ok(resource);
    }

    /// <summary>
    ///     Retrieves telemetry log entries filtered by container identifier or dispatch trip identifier.
    /// </summary>
    [HttpGet]
    [EndpointSummary("Get telemetry logs filtered by container or trip")]
    [EndpointDescription("Retrieves time-series telemetry records filtered by SmartContainer identifier or active dispatch trip identifier.")]
    [ProducesResponseType(typeof(IEnumerable<TelemetryLogResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTelemetryLogs(
        [FromQuery] string? containerId,
        [FromQuery] string? tripId,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(containerId))
        {
            var byContainerQuery = new GetAllTelemetryLogsByContainerIdQuery(containerId);
            var logs = await telemetryLogQueryService.Handle(byContainerQuery, cancellationToken);
            var resources = logs.Select(TelemetryLogResourceFromEntityAssembler.ToResourceFromEntity);
            return Ok(resources);
        }

        if (!string.IsNullOrWhiteSpace(tripId))
        {
            var byTripQuery = new GetAllTelemetryLogsByTripIdQuery(tripId);
            var logs = await telemetryLogQueryService.Handle(byTripQuery, cancellationToken);
            var resources = logs.Select(TelemetryLogResourceFromEntityAssembler.ToResourceFromEntity);
            return Ok(resources);
        }

        return BadRequest(new { message = "Specify either a containerId or tripId query parameter." });
    }

    /// <summary>
    ///     Retrieves the latest telemetry log snapshot recorded for a specific SmartContainer.
    /// </summary>
    [HttpGet("containers/{containerId}/latest")]
    [EndpointSummary("Get latest telemetry snapshot for a container")]
    [EndpointDescription("Retrieves the most recent environmental sensor snapshot recorded for the specified container.")]
    [ProducesResponseType(typeof(TelemetryLogResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatestTelemetryLogByContainerId(
        string containerId,
        CancellationToken cancellationToken)
    {
        var query = new GetLatestTelemetryLogByContainerIdQuery(containerId);
        var result = await telemetryLogQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();

        var resource = TelemetryLogResourceFromEntityAssembler.ToResourceFromEntity(result);
        return Ok(resource);
    }
}
