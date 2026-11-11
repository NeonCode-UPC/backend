using System.Globalization;
using System.Linq;
using System.Resources;
using MedicalSmartBox.API.Shared.Application.Patterns;
using MedicalSmartBox.API.Transport.Application.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace MedicalSmartBox.API.Transport.Interfaces.Rest.Transform;

public static class TransportResultActionAssembler
{
    private static readonly ResourceManager Messages = new(
        "MedicalSmartBox.API.Transport.Resources.Errors.ErrorMessages",
        typeof(TransportResultActionAssembler).Assembly);

    public static IActionResult ToActionResult<T>(
        Result<T, TransportError> result,
        ControllerBase controller,
        Func<T, IActionResult> onSuccess)
    {
        return result.Fold(
            onSuccess,
            error =>
            {
                var (key, statusCode) = error switch
                {
                    TransportError.NotFound => ("NotFound", StatusCodes.Status404NotFound),
                    TransportError.DuplicateOrderCode => ("DuplicateOrderCode", StatusCodes.Status409Conflict),
                    TransportError.OrderAlreadyHasTrip => ("OrderAlreadyHasTrip", StatusCodes.Status409Conflict),
                    TransportError.IschemiaLimitExceeded => ("IschemiaLimitExceeded", StatusCodes.Status422UnprocessableEntity),
                    TransportError.InvalidState => ("InvalidState", StatusCodes.Status409Conflict),
                    TransportError.TrafficServiceUnavailable => ("TrafficServiceUnavailable", StatusCodes.Status503ServiceUnavailable),
                    _ => ("InvalidRequest", StatusCodes.Status400BadRequest)
                };

                var culture = ResolveCulture(controller.Request.Headers);
                var detail = Messages.GetString(key, culture) ?? key;
                return controller.Problem(
                    title: Messages.GetString("RequestFailed", culture) ?? "Transport request failed",
                    detail: detail,
                    statusCode: statusCode);
            });
    }

    private static CultureInfo ResolveCulture(IHeaderDictionary headers)
    {
        if (!headers.TryGetValue("Accept-Language", out var languages))
            return CultureInfo.CurrentUICulture;

        var language = languages.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(value => value.Split(';', StringSplitOptions.RemoveEmptyEntries)[0].Trim())
            .FirstOrDefault();
        if (string.IsNullOrWhiteSpace(language))
            return CultureInfo.CurrentUICulture;

        try
        {
            return CultureInfo.GetCultureInfo(language);
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.CurrentUICulture;
        }
    }
}
