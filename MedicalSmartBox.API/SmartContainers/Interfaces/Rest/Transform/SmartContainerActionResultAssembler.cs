using MedicalSmartBox.API.Shared.Application.Patterns;
using MedicalSmartBox.API.SmartContainers.Application.Errors;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.SmartContainers.Interfaces.Rest.Transform;

/// <summary>
///     Translates application Result into ASP.NET Core IActionResult with RFC 7807 ProblemDetails.
/// </summary>
public static class SmartContainerActionResultAssembler
{
    public static IActionResult ToActionResult(
        Result<SmartContainer, SmartContainerError> result,
        ControllerBase controller,
        string? createdAtActionName = null)
    {
        return result.Fold<IActionResult>(
            success =>
            {
                var resource = SmartContainerResourceFromEntityAssembler.ToResourceFromEntity(success);
                if (!string.IsNullOrWhiteSpace(createdAtActionName))
                {
                    return controller.CreatedAtAction(
                        createdAtActionName,
                        new { id = success.Id.Value },
                        resource);
                }
                return controller.Ok(resource);
            },
            failure => failure switch
            {
                SmartContainerError.NotFound => controller.NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "SmartContainer Not Found",
                    Detail = "The specified SmartContainer identifier was not found in the fleet."
                }),
                SmartContainerError.DuplicateSerialNumber => controller.Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Duplicate Serial Number",
                    Detail = "A SmartContainer with the specified serial number is already registered."
                }),
                SmartContainerError.AlreadyPaired => controller.Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Container Already Paired",
                    Detail = "This SmartContainer is already assigned to an ambulance and must be unlinked first."
                }),
                _ => controller.BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid Operation",
                    Detail = failure.ToString()
                })
            });
    }
}
