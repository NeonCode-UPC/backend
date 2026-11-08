using MedicalSmartBox.API.Custody.Application.QueryServices;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Resources;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.Custody.Interfaces.Rest;

[ApiController]
[Route("api/v1/digital-audit-manifests")]
public sealed class DigitalAuditManifestsController(CustodyQueryService queryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DigitalAuditManifestResource>>> GetAll(CancellationToken cancellationToken)
    {
        var manifests = await queryService.ListManifestsAsync(cancellationToken);
        var result = new List<DigitalAuditManifestResource>(manifests.Count);
        foreach (var manifest in manifests)
        {
            var transfer = await queryService.FindTransferAsync(manifest.TransferId.ToString("D"), cancellationToken);
            if (transfer is not null) result.Add(CustodyResourceFromEntityAssembler.ToResourceFromEntity(manifest, transfer));
        }
        return Ok(result);
    }

    [HttpGet("{transferId}")]
    public async Task<ActionResult<DigitalAuditManifestResource>> GetByTransfer(string transferId, CancellationToken cancellationToken)
    {
        var manifest = await queryService.FindManifestAsync(transferId, cancellationToken);
        if (manifest is null) return NotFound();
        var transfer = await queryService.FindTransferAsync(transferId, cancellationToken);
        return transfer is null ? NotFound() : Ok(CustodyResourceFromEntityAssembler.ToResourceFromEntity(manifest, transfer));
    }
}
