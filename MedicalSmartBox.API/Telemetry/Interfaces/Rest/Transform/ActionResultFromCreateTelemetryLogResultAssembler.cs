using MedicalSmartBox.API.Shared.Application.Patterns;
using MedicalSmartBox.API.Telemetry.Application.Errors;
using MedicalSmartBox.API.Telemetry.Domain.Model.Aggregates;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSmartBox.API.Telemetry.Interfaces.Rest.Transform;

/// <summary>
///     Assembler converting application Result outcomes into HTTP ActionResults.
/// </summary>
public static class ActionResultFromCreateTelemetryLogResultAssembler
{
    public static IActionResult ToActionResult(
        Result<TelemetryLog, CreateTelemetryLogError> result,
        ControllerBase controller,
        string getByIdActionName)
    {
        return result.Fold<IActionResult>(
            entity => controller.CreatedAtAction(
                getByIdActionName,
                new { id = entity.Id },
                TelemetryLogResourceFromEntityAssembler.ToResourceFromEntity(entity)),
            error => error switch
            {
                CreateTelemetryLogError.InvalidContainerId => controller.BadRequest(new { message = "A valid SmartContainer identifier is required." }),
                CreateTelemetryLogError.InvalidFirmwareSignature => controller.BadRequest(new { message = "A valid ESP32 firmware signature is required." }),
                CreateTelemetryLogError.InvalidSensorPayload => controller.BadRequest(new { message = "One or more telemetry sensor values are outside allowed physical ranges." }),
                _ => controller.Problem(
                    title: "Unexpected Server Error",
                    detail: "An unexpected error occurred while persisting the telemetry log.",
                    statusCode: StatusCodes.Status500InternalServerError)
            });
    }
}
