using MedicalSmartBox.API.alerting.Domain.Model.Aggregates;
using MedicalSmartBox.API.alerting.Domain.Model.Commands;
using MedicalSmartBox.API.alerting.Domain.Services;
using MedicalSmartBox.API.alerting.Interfaces.REST.Resources;
using MedicalSmartBox.API.alerting.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.alerting.Interfaces.REST;

[ApiController]
[Route("api/v1/incidents")]
[Route("api/v1/critical-incidents")]
[Produces("application/json")]
[Tags("Critical Incidents")]
public class IncidentController(IIncidentCommandService commandService, IIncidentQueryService queryService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a critical incident")]
    [EndpointDescription("Registers an incident for a trip and container with its severity and optional sensor readings.")]
    [ProducesResponseType<IncidentResource>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateIncidentResource resource, CancellationToken cancellationToken)
    {
        if (resource.TripId == Guid.Empty || resource.ContainerId == Guid.Empty)
            return BadRequest(new { message = "Trip ID and container ID must not be empty." });
        var incident = await commandService.CreateAsync(IncidentResourceAssembler.ToCommand(resource), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = incident.Id }, IncidentResourceAssembler.ToResource(incident));
    }

    [HttpPost("{id:guid}/acknowledge")]
    [EndpointSummary("Acknowledge an incident")]
    [EndpointDescription("Records the operator and UTC time of acknowledgement. This does not send a notification.")]
    [ProducesResponseType<IncidentResource>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Acknowledge(Guid id, [FromBody] AcknowledgeIncidentResource resource, CancellationToken cancellationToken)
    {
        if (resource.OperatorId == Guid.Empty)
            return BadRequest(new { message = "Operator ID must not be empty." });
        try
        {
            var incident = await commandService.AcknowledgeAsync(IncidentResourceAssembler.ToAcknowledgeCommand(id, resource), cancellationToken);
            return incident is null ? NotFound() : Ok(IncidentResourceAssembler.ToResource(incident));
        }
        catch (IncidentStateException)
        {
            return Conflict(new { message = "Incident cannot be acknowledged in its current state." });
        }
    }

    /// <summary>Resolve an incident and record its mitigation and quality signoff.</summary>
    [HttpPost("{id:guid}/resolve")]
    [EndpointSummary("Resolve an incident")]
    [EndpointDescription("Records mitigation, quality signoff and the responsible user; resolved incidents leave the active list.")]
    [ProducesResponseType<IncidentResource>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Resolve(Guid id, [FromBody] ResolveIncidentResource resource, CancellationToken cancellationToken)
    {
        if (resource.ResolvedByUserId == Guid.Empty) return BadRequest(new { message = "Resolver ID must not be empty." });
        try
        {
            var incident = await commandService.ResolveAsync(IncidentResourceAssembler.ToResolveCommand(id, resource), cancellationToken);
            return incident is null ? NotFound() : Ok(IncidentResourceAssembler.ToResource(incident));
        }
        catch (IncidentStateException) { return Conflict(new { message = "Incident cannot be resolved in its current state." }); }
    }

    /// <summary>Escalate an active incident to the medical director level.</summary>
    [HttpPost("{id:guid}/escalate")]
    [EndpointSummary("Escalate an incident")]
    [EndpointDescription("Sets an active incident to medical director escalation. No external notification is sent.")]
    [ProducesResponseType<IncidentResource>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Escalate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var incident = await commandService.EscalateAsync(new EscalateIncidentCommand(id), cancellationToken);
            return incident is null ? NotFound() : Ok(IncidentResourceAssembler.ToResource(incident));
        }
        catch (IncidentStateException) { return Conflict(new { message = "Resolved incidents cannot be escalated." }); }
    }

    /// <summary>List incident history, including resolved incidents.</summary>
    [HttpGet]
    [EndpointSummary("List incident history")]
    [ProducesResponseType<IEnumerable<IncidentResource>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var incidents = await queryService.GetAllAsync(cancellationToken);
        return Ok(incidents.Select(IncidentResourceAssembler.ToResource));
    }
    [HttpGet("{id:guid}")]
    [EndpointSummary("Get an incident by ID")]
    [ProducesResponseType<IncidentResource>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var incident = await queryService.GetByIdAsync(id, cancellationToken);
        return incident is null ? NotFound() : Ok(IncidentResourceAssembler.ToResource(incident));
    }

    [HttpGet("overdue")]
    [EndpointSummary("List incidents overdue for acknowledgement")]
    [EndpointDescription("Lists active incidents without acknowledgement two minutes after they were triggered. This endpoint does not send notifications.")]
    [ProducesResponseType<IEnumerable<IncidentResource>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverdue(CancellationToken cancellationToken)
    {
        var incidents = await queryService.GetOverdueAcknowledgementsAsync(cancellationToken);
        return Ok(incidents.Select(IncidentResourceAssembler.ToResource));
    }
    [HttpGet("active")]
    [EndpointSummary("List active incidents")]
    [ProducesResponseType<IEnumerable<IncidentResource>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var incidents = await queryService.GetActiveAsync(cancellationToken);
        return Ok(incidents.Select(IncidentResourceAssembler.ToResource));
    }
}