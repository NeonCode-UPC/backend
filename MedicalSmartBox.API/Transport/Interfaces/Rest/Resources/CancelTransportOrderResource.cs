using System.ComponentModel.DataAnnotations;

namespace MedicalSmartBox.API.Transport.Interfaces.Rest.Resources;

public sealed record CancelTransportOrderResource(
    [param: Required, StringLength(500, MinimumLength = 1)] string Reason);
