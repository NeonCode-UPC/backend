using System.ComponentModel.DataAnnotations;

namespace MedicalSmartBox.API.Transport.Interfaces.Rest.Resources;

public sealed record UpdateDynamicEtaResource(
    [param: Range(typeof(decimal), "-90", "90")] decimal OriginLatitude,
    [param: Range(typeof(decimal), "-180", "180")] decimal OriginLongitude);
