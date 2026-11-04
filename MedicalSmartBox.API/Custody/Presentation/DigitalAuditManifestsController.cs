using MedicalSmartBox.API.Custody.Application;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.Custody.Presentation;

[ApiController]
[Route("api/v1/digital-audit-manifests")]
public sealed class DigitalAuditManifestsController(CustodyApplicationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DigitalAuditManifestDto>>> GetAll(CancellationToken cancellationToken)
    {
        var manifests = await service.ListManifestsAsync(cancellationToken);
        var result = new List<DigitalAuditManifestDto>(manifests.Count);
        foreach (var manifest in manifests)
        {
            var transfer = await service.FindTransferAsync(manifest.TransferId.ToString("D"), cancellationToken);
            if (transfer is not null) result.Add(CustodyContractMapper.ToDto(manifest, transfer));
        }
        return Ok(result);
    }

    [HttpGet("{transferId}")]
    public async Task<ActionResult<DigitalAuditManifestDto>> GetByTransfer(string transferId, CancellationToken cancellationToken)
    {
        var manifest = await service.FindManifestAsync(transferId, cancellationToken);
        if (manifest is null) return NotFound();
        var transfer = await service.FindTransferAsync(transferId, cancellationToken);
        return transfer is null ? NotFound() : Ok(CustodyContractMapper.ToDto(manifest, transfer));
    }
}
