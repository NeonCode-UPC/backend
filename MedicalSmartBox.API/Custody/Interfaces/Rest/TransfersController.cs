using MedicalSmartBox.API.Custody.Application.QueryServices;
using MedicalSmartBox.API.Custody.Domain.Model.Aggregates;
using MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;
using MedicalSmartBox.API.Custody.Infrastructure.Reporting;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MedicalSmartBox.API.Custody.Interfaces.Rest;

[ApiController]
[Route("api/v1/transfers")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Custody Traceability")]
public sealed class TransfersController(ICustodyQueryService queryService) : ControllerBase
{
    [HttpGet("{transferId}/audit")]
    [EndpointSummary("Get the complete custody audit")]
    [EndpointDescription("Retrieves the transfer, event history, temperature readings, audit manifest, and temperature compliance summary.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [EndpointSummary("Get a custody traceability report")]
    [EndpointDescription("Retrieves the final traceability report for a completed or rejected custody transfer.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
    [EndpointSummary("Download a custody traceability report")]
    [EndpointDescription("Generates and downloads the final traceability report as a PDF document.")]
    [Produces(MediaTypeNames.Application.Pdf)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
