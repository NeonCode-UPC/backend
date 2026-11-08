using MedicalSmartBox.API.Custody.Application.QueryServices;
using MedicalSmartBox.API.Custody.Domain.Model.Aggregates;
using MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;
using MedicalSmartBox.API.Custody.Infrastructure.Reporting;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.Custody.Interfaces.Rest;

[ApiController]
[Route("api/v1/transfers")]
public sealed class TransfersController(CustodyQueryService queryService) : ControllerBase
{
    [HttpGet("{transferId}/audit")]
    public async Task<IActionResult> GetAudit(string transferId, CancellationToken cancellationToken)
    {
        var transfer = await queryService.FindTransferAsync(transferId, cancellationToken);
        if (transfer is null) return NotFound(new { message = $"Transfer '{transferId}' was not found." });
        var manifest = await queryService.FindManifestAsync(transferId, cancellationToken);
        return Ok(new
        {
            transfer = CustodyResourceFromEntityAssembler.ToResourceFromEntity(transfer),
            events = transfer.Events.Select(CustodyResourceFromEntityAssembler.ToResourceFromEntity),
            telemetry = transfer.TemperatureReadings,
            manifest = manifest is null ? null : CustodyResourceFromEntityAssembler.ToResourceFromEntity(manifest, transfer),
            thermalSummary = manifest is null ? null : new
            {
                manifest.MinTemperatureCelsius,
                manifest.AverageTemperatureCelsius,
                manifest.MaxTemperatureCelsius,
                manifest.TotalExcursionSeconds,
                manifest.MinsaComplianceVerified
            }
        });
    }

    [HttpGet("{transferId}/traceability-report")]
    public async Task<IActionResult> GetTraceabilityReport(string transferId, CancellationToken cancellationToken)
    {
        var transfer = await queryService.FindTransferAsync(transferId, cancellationToken);
        if (transfer is null) return NotFound(new { message = $"Transfer '{transferId}' was not found." });
        if (transfer.Status is not (CustodyStatus.CompletedAccepted or CustodyStatus.RejectedTampering or CustodyStatus.RejectedThermalExcursion))
            return Conflict(new { message = "A final traceability report is available after custody has been closed." });

        var manifest = await queryService.FindManifestAsync(transferId, cancellationToken);
        return Ok(new
        {
            transfer = CustodyResourceFromEntityAssembler.ToResourceFromEntity(transfer),
            events = transfer.Events.Select(CustodyResourceFromEntityAssembler.ToResourceFromEntity),
            temperatureReadings = transfer.TemperatureReadings,
            manifest = manifest is null ? null : CustodyResourceFromEntityAssembler.ToResourceFromEntity(manifest, transfer),
            generatedAt = DateTimeOffset.UtcNow
        });
    }

    [HttpGet("{transferId}/traceability-report/pdf")]
    public async Task<IActionResult> DownloadTraceabilityReport(string transferId, CancellationToken cancellationToken)
    {
        var transfer = await queryService.FindTransferAsync(transferId, cancellationToken);
        if (transfer is null) return NotFound(new { message = $"Transfer '{transferId}' was not found." });
        if (transfer.Status is not (CustodyStatus.CompletedAccepted or CustodyStatus.RejectedTampering or CustodyStatus.RejectedThermalExcursion))
            return Conflict(new { message = "A final traceability report is available after custody has been closed." });

        var manifest = await queryService.FindManifestAsync(transferId, cancellationToken);
        var bytes = TraceabilityReportPdf.Create(transfer, manifest);
        return File(bytes, "application/pdf", $"traceability-{transfer.TransferCode}.pdf");
    }
}
