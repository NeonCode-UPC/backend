using System.Net.Mime;
using MedicalSmartBox.API.Transport.Application.CommandServices;
using MedicalSmartBox.API.Transport.Application.Commands;
using MedicalSmartBox.API.Transport.Application.Queries;
using MedicalSmartBox.API.Transport.Application.QueryServices;
using MedicalSmartBox.API.Transport.Interfaces.Rest.Resources;
using MedicalSmartBox.API.Transport.Interfaces.Rest.Responses;
using MedicalSmartBox.API.Transport.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.Transport.Interfaces.Rest.Controllers;

[ApiController]
[Route("api/v1/transport-orders")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Transport Orders")]
public sealed class TransportOrdersController(
    ITransportCommandService commandService,
    ITransportQueryService queryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List transport orders")]
    [EndpointDescription("Returns transport orders with their hospitals, clinical priority, biological cargo, ischemia limits, status, and creator.")]
    [ProducesResponseType(typeof(IReadOnlyList<TransportOrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var orders = await queryService.Handle(new GetTransportOrdersQuery(), cancellationToken);
        return Ok(orders.Select(TransportResponseAssembler.ToResponse));
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Get a transport order")]
    [EndpointDescription("Retrieves one transport order by its GUID.")]
    [ProducesResponseType(typeof(TransportOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order = await queryService.Handle(new GetTransportOrderByIdQuery(id), cancellationToken);
        return order is null ? NotFound() : Ok(TransportResponseAssembler.ToResponse(order));
    }

    [HttpPost]
    [EndpointSummary("Create a transport order")]
    [EndpointDescription("Creates a medical transport order with origin and destination hospitals, cargo type, priority, creator, and cold-ischemia limits.")]
    [ProducesResponseType(typeof(TransportOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTransportOrderResource resource,
        CancellationToken cancellationToken)
    {
        var command = new CreateTransportOrderCommand(
            resource.OrderCode,
            resource.OriginHospitalId,
            resource.DestinationHospitalId,
            resource.Priority,
            resource.CargoType,
            resource.CargoDescription,
            resource.MaxSafeHours,
            resource.WarningThresholdHours,
            resource.CreatedByUserId);

        var result = await commandService.Handle(command, cancellationToken);
        return TransportResultActionAssembler.ToActionResult(
            result,
            this,
            order => CreatedAtAction(
                nameof(GetById),
                new { id = order.Id },
                TransportResponseAssembler.ToResponse(order)));
    }

    [HttpPost("{id:guid}/cancel")]
    [EndpointSummary("Cancel a transport order")]
    [EndpointDescription("Cancels an order that has not yet been assigned to a dispatch trip.")]
    [ProducesResponseType(typeof(TransportOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelTransportOrderResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(
            new CancelTransportOrderCommand(id, resource.Reason), cancellationToken);
        return TransportResultActionAssembler.ToActionResult(
            result,
            this,
            order => Ok(TransportResponseAssembler.ToResponse(order)));
    }
}
