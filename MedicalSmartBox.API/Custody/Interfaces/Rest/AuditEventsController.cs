using MedicalSmartBox.API.Custody.Application.QueryServices;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Resources;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.Custody.Interfaces.Rest;

[ApiController]
[Route("api/v1/audit-events")]
public sealed class AuditEventsController(CustodyQueryService queryService) : ControllerBase
{
    [HttpGet]
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
