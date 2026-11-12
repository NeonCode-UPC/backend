using System.ComponentModel.DataAnnotations;

namespace MedicalSmartBox.API.Transport.Interfaces.Rest.Resources;

public sealed record RegisterHospitalArrivalResource(
    [param: Range(typeof(decimal), "-90", "90")] decimal CurrentLatitude,
    [param: Range(typeof(decimal), "-180", "180")] decimal CurrentLongitude);
