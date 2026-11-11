using System.ComponentModel.DataAnnotations;

namespace MedicalSmartBox.API.Transport.Interfaces.Rest.Resources;

public sealed record UpdateDynamicEtaResource(
    [property: Range(typeof(decimal), "-90", "90")] decimal OriginLatitude,
    [property: Range(typeof(decimal), "-180", "180")] decimal OriginLongitude);
