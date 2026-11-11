using MedicalSmartBox.API.Custody.Application.QueryServices;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Resources;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MedicalSmartBox.API.Custody.Interfaces.Rest;

[ApiController]
[Route("api/v1/audit-events")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Audit Events")]
public sealed class AuditEventsController(ICustodyQueryService queryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get custody audit events")]
    [EndpointDescription("Retrieves all custody events across transfers, ordered from newest to oldest.")]
    [ProducesResponseType(typeof(IReadOnlyList<AuditEventResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AuditEventResource>>> GetAll(CancellationToken cancellationToken)
    {
        var transfers = await queryService.ListTransfersAsync(cancellationToken);
        var events = transfers.SelectMany(transfer => transfer.Events.Select(item => (Transfer: transfer, Event: item)))
            .OrderByDescending(item => item.Event.At)
            .Select((item, index) => CustodyResourceFromEntityAssembler.ToAuditResourceFromEntity(item.Transfer, item.Event, index))
            .ToArray();
        return Ok(events);
    }
}
