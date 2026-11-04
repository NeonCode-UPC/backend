using MedicalSmartBox.API.Custody.Application;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.Custody.Presentation;

[ApiController]
[Route("api/v1/audit-events")]
public sealed class AuditEventsController(CustodyApplicationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuditEventDto>>> GetAll(CancellationToken cancellationToken)
    {
        var transfers = await service.ListTransfersAsync(cancellationToken);
        var events = transfers.SelectMany(transfer => transfer.Events.Select(item => (Transfer: transfer, Event: item)))
            .OrderByDescending(item => item.Event.At)
            .Select((item, index) => CustodyContractMapper.ToAuditDto(item.Transfer, item.Event, index))
            .ToArray();
        return Ok(events);
    }
}
