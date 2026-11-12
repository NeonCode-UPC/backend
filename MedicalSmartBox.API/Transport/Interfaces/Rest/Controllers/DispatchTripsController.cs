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
[Route("api/v1/dispatch-trips")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Dispatch Trips")]
public sealed class DispatchTripsController(
    ITransportCommandService commandService,
    ITransportQueryService queryService) : ControllerBase
{
    [HttpGet("active")]
    [EndpointSummary("List active dispatch trips")]
    [EndpointDescription("Returns scheduled and ongoing trips that have not been completed or cancelled, ordered by estimated arrival.")]
    [ProducesResponseType(typeof(IReadOnlyList<DispatchTripResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var trips = await queryService.Handle(new GetActiveTripsQuery(), cancellationToken);
        return Ok(trips.Select(TransportResponseAssembler.ToResponse));
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Get a dispatch trip")]
    [EndpointDescription("Retrieves a dispatch trip, assigned crew and vehicle, route, ETA, and destination geofence coordinates by GUID.")]
    [ProducesResponseType(typeof(DispatchTripResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var trip = await queryService.Handle(new GetTripByIdQuery(id), cancellationToken);
        return trip is null ? NotFound() : Ok(TransportResponseAssembler.ToResponse(trip));
    }

    [HttpPost]
    [EndpointSummary("Assign resources and create a dispatch trip")]
    [EndpointDescription("Assigns a vehicle, smart container, driver, and paramedic to an order and creates its trip with route and destination details.")]
    [ProducesResponseType(typeof(DispatchTripResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AssignResources(
        [FromBody] AssignTripResourcesResource resource,
        CancellationToken cancellationToken)
    {
        var command = new AssignTripResourcesCommand(
            resource.OrderId,
            resource.VehiclePlate,
            resource.ContainerId,
            resource.DriverUserId,
            resource.ParamedicUserId,
            resource.ScheduledDepartureTime,
            resource.EstimatedArrivalTime,
            resource.DistanceKilometers,
            resource.PlannedDurationMinutes,
            resource.PolylineCoordinates,
            resource.DestinationLatitude,
            resource.DestinationLongitude);

        var result = await commandService.Handle(command, cancellationToken);
        return TransportResultActionAssembler.ToActionResult(
            result,
            this,
            trip => CreatedAtAction(
                nameof(GetById),
                new { id = trip.Id },
                TransportResponseAssembler.ToResponse(trip)));
    }

    [HttpPost("{id:guid}/start")]
    [EndpointSummary("Start a dispatch trip")]
    [EndpointDescription("Starts a resources-assigned trip only after the medical container has reached its required pre-cooled temperature.")]
    [ProducesResponseType(typeof(DispatchTripResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Start(
        Guid id,
        [FromBody] StartTripResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(
            new StartTripCommand(id, resource.ContainerPrecooled), cancellationToken);
        return TransportResultActionAssembler.ToActionResult(
            result,
            this,
            trip => Ok(TransportResponseAssembler.ToResponse(trip)));
    }

    [HttpPatch("{id:guid}/eta")]
    [EndpointSummary("Refresh a trip ETA using live traffic")]
    [EndpointDescription("Recalculates ETA and route traffic delay through TomTom using the ambulance's current coordinates and the stored route.")]
    [ProducesResponseType(typeof(DispatchTripResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> UpdateEta(
        Guid id,
        [FromBody] UpdateDynamicEtaResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(
            new UpdateDynamicEtaCommand(id, resource.OriginLatitude, resource.OriginLongitude),
            cancellationToken);
        return TransportResultActionAssembler.ToActionResult(
            result,
            this,
            trip => Ok(TransportResponseAssembler.ToResponse(trip)));
    }

    [HttpPost("{id:guid}/arrival")]
    [EndpointSummary("Register arrival inside the destination hospital geofence")]
    [EndpointDescription("Registers hospital arrival when the current GPS coordinates are within 100 meters of the destination.")]
    [ProducesResponseType(typeof(DispatchTripResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterArrival(
        Guid id,
        [FromBody] RegisterHospitalArrivalResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(
            new RegisterHospitalArrivalCommand(id, resource.CurrentLatitude, resource.CurrentLongitude),
            cancellationToken);
        return TransportResultActionAssembler.ToActionResult(
            result,
            this,
            trip => Ok(TransportResponseAssembler.ToResponse(trip)));
    }

    [HttpPost("{id:guid}/complete")]
    [EndpointSummary("Complete an arrived dispatch trip")]
    [EndpointDescription("Completes a trip after arrival has been registered and updates the transport order status.")]
    [ProducesResponseType(typeof(DispatchTripResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(new CompleteTripCommand(id), cancellationToken);
        return TransportResultActionAssembler.ToActionResult(
            result,
            this,
            trip => Ok(TransportResponseAssembler.ToResponse(trip)));
    }
}
