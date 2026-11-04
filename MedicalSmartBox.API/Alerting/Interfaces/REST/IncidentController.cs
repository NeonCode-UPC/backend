using MedicalSmartBox.API.alerting.Domain.Services;
using MedicalSmartBox.API.alerting.Interfaces.REST.Resources;
using MedicalSmartBox.API.alerting.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.alerting.Interfaces.REST;

[ApiController]
[Route("api/v1/incidents")]
public class IncidentController(IIncidentCommandService commandService, IIncidentQueryService queryService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<IncidentResource>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateIncidentResource resource, CancellationToken cancellationToken)
    {
        if (resource.TripId == Guid.Empty || resource.ContainerId == Guid.Empty)
            return BadRequest(new { message = "Trip ID and container ID must not be empty." });
        var incident = await commandService.CreateAsync(IncidentResourceAssembler.ToCommand(resource), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = incident.Id }, IncidentResourceAssembler.ToResource(incident));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<IncidentResource>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var incident = await queryService.GetByIdAsync(id, cancellationToken);
        return incident is null ? NotFound() : Ok(IncidentResourceAssembler.ToResource(incident));
    }

    [HttpGet("active")]
    [ProducesResponseType<IEnumerable<IncidentResource>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var incidents = await queryService.GetActiveAsync(cancellationToken);
        return Ok(incidents.Select(IncidentResourceAssembler.ToResource));
    }
}