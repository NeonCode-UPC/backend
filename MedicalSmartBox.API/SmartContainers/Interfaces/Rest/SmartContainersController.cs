using System.Net.Mime;
using MedicalSmartBox.API.SmartContainers.Application.CommandServices;
using MedicalSmartBox.API.SmartContainers.Application.QueryServices;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Commands;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Queries;
using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;
using MedicalSmartBox.API.SmartContainers.Interfaces.Rest.Resources;
using MedicalSmartBox.API.SmartContainers.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.SmartContainers.Interfaces.Rest;

/// <summary>
///     REST controller exposing endpoints for SmartContainer inventory, pairing and telemetry monitoring.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Smart Containers")]
public class SmartContainersController(
    ISmartContainerCommandService commandService,
    ISmartContainerQueryService queryService)
    : ControllerBase
{
    /// <summary>
    ///     Retrieves all SmartContainer units registered in the cold chain fleet with optional filtering.
    /// </summary>
    [HttpGet]
    [EndpointSummary("List all SmartContainers")]
    [EndpointDescription("Returns fleet inventory of medical isothermal boxes with battery, thermal and locking state.")]
    [ProducesResponseType(typeof(IEnumerable<SmartContainerResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllContainers(
        [FromQuery] string? status,
        [FromQuery] string? ambulancePlate,
        CancellationToken cancellationToken)
    {
        var query = new GetAllSmartContainersQuery(status, ambulancePlate);
        var containers = await queryService.Handle(query, cancellationToken);
        var resources = containers.Select(SmartContainerResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    /// <summary>
    ///     Retrieves a single SmartContainer by its database identifier.
    /// </summary>
    [HttpGet("{id:int}")]
    [EndpointSummary("Get SmartContainer by ID")]
    [EndpointDescription("Retrieves detailed operational and sensor parameters for a specific SmartContainer.")]
    [ProducesResponseType(typeof(SmartContainerResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContainerById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetSmartContainerByIdQuery(new ContainerId(id)), cancellationToken);
        if (result is null) return NotFound();

        return Ok(SmartContainerResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    /// <summary>
    ///     Retrieves a SmartContainer by its hardware chassis serial number.
    /// </summary>
    [HttpGet("serial/{serialNumber}")]
    [EndpointSummary("Get SmartContainer by Serial Number")]
    [EndpointDescription("Finds a SmartContainer matching the specified alphanumeric serial number (e.g. SB-0182).")]
    [ProducesResponseType(typeof(SmartContainerResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContainerBySerialNumber([FromRoute] string serialNumber, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetSmartContainerBySerialNumberQuery(new SerialNumber(serialNumber)), cancellationToken);
        if (result is null) return NotFound();

        return Ok(SmartContainerResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    /// <summary>
    ///     Registers a new SmartContainer unit into the fleet.
    /// </summary>
    [HttpPost]
    [EndpointSummary("Register a new SmartContainer")]
    [EndpointDescription("Creates a new SmartContainer aggregate with calibrated tare weight and thermal thresholds.")]
    [ProducesResponseType(typeof(SmartContainerResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateContainer([FromBody] CreateSmartContainerResource resource, CancellationToken cancellationToken)
    {
        var command = CreateSmartContainerCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await commandService.Handle(command, cancellationToken);

        return SmartContainerActionResultAssembler.ToActionResult(
            result,
            this,
            nameof(GetContainerById));
    }

    /// <summary>
    ///     Pairs a SmartContainer to an ambulance for dispatch transit.
    /// </summary>
    [HttpPost("{id:int}/pair")]
    [EndpointSummary("Pair container to an ambulance")]
    [EndpointDescription("Associates an isothermal box with an active ambulance vehicle by license plate.")]
    [ProducesResponseType(typeof(SmartContainerResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PairContainer(
        [FromRoute] int id,
        [FromBody] PairContainerResource resource,
        CancellationToken cancellationToken)
    {
        var command = new PairContainerToAmbulanceCommand(new ContainerId(id), resource.AmbulancePlate);
        var result = await commandService.Handle(command, cancellationToken);

        return SmartContainerActionResultAssembler.ToActionResult(result, this);
    }

    /// <summary>
    ///     Unlinks a SmartContainer from its current vehicle, returning it to Available state.
    /// </summary>
    [HttpPost("{id:int}/unlink")]
    [EndpointSummary("Unlink container from ambulance")]
    [EndpointDescription("Releases the ambulance assignment from the SmartContainer upon successful custody transfer.")]
    [ProducesResponseType(typeof(SmartContainerResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnlinkContainer([FromRoute] int id, CancellationToken cancellationToken)
    {
        var command = new UnlinkContainerCommand(new ContainerId(id));
        var result = await commandService.Handle(command, cancellationToken);

        return SmartContainerActionResultAssembler.ToActionResult(result, this);
    }

    /// <summary>
    ///     Logs a payload weight modification during cargo loading or delivery.
    /// </summary>
    [HttpPost("{id:int}/weight-movements")]
    [EndpointSummary("Record payload weight movement")]
    [EndpointDescription("Updates net payload weight with audit tracking of biological units.")]
    [ProducesResponseType(typeof(SmartContainerResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecordWeightMovement(
        [FromRoute] int id,
        [FromBody] RecordWeightMovementResource resource,
        CancellationToken cancellationToken)
    {
        var command = new RecordWeightMovementCommand(
            new ContainerId(id),
            resource.NetWeightGrams,
            resource.UnitsDifference,
            resource.ResponsibleUser ?? "Pharmacist",
            resource.Reason ?? "Clinical transfer");

        var result = await commandService.Handle(command, cancellationToken);
        return SmartContainerActionResultAssembler.ToActionResult(result, this);
    }

    /// <summary>
    ///     Synchronizes real-time IoT telemetry snapshot readings into the SmartContainer.
    /// </summary>
    [HttpPost("{id:int}/telemetry-snapshot")]
    [EndpointSummary("Record telemetry snapshot on container")]
    [EndpointDescription("Applies live temperature, battery percentage, power feed and weight updates to the aggregate.")]
    [ProducesResponseType(typeof(SmartContainerResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecordTelemetrySnapshot(
        [FromRoute] int id,
        [FromBody] UpdateContainerTelemetrySnapshotResource resource,
        CancellationToken cancellationToken)
    {
        var command = new UpdateContainerTelemetrySnapshotCommand(
            new ContainerId(id),
            resource.TemperatureCelsius,
            resource.BatteryPercentage,
            resource.Is12vConnected,
            resource.NetWeightGrams,
            resource.SignalQuality ?? "strong",
            resource.TimestampUtc ?? DateTimeOffset.UtcNow);

        var result = await commandService.Handle(command, cancellationToken);
        return SmartContainerActionResultAssembler.ToActionResult(result, this);
    }

    /// <summary>
    ///     Updates container assignment or parameters directly from frontend REST calls.
    /// </summary>
    [HttpPut("{id:int}")]
    [EndpointSummary("Update container parameters")]
    [EndpointDescription("Updates operational properties such as ambulance plate or weight.")]
    [ProducesResponseType(typeof(SmartContainerResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateContainer(
        [FromRoute] int id,
        [FromBody] SmartContainerResource resource,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(resource.AmbulancePlate))
        {
            var pairResult = await commandService.Handle(
                new PairContainerToAmbulanceCommand(new ContainerId(id), resource.AmbulancePlate),
                cancellationToken);
            return SmartContainerActionResultAssembler.ToActionResult(pairResult, this);
        }

        if (string.IsNullOrWhiteSpace(resource.AmbulancePlate) && string.IsNullOrWhiteSpace(resource.AssignedAmbulancePlate))
        {
            var unlinkResult = await commandService.Handle(
                new UnlinkContainerCommand(new ContainerId(id)),
                cancellationToken);
            return SmartContainerActionResultAssembler.ToActionResult(unlinkResult, this);
        }

        var container = await queryService.Handle(new GetSmartContainerByIdQuery(new ContainerId(id)), cancellationToken);
        if (container is null) return NotFound();

        return Ok(SmartContainerResourceFromEntityAssembler.ToResourceFromEntity(container));
    }
}
