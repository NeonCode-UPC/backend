using System.Globalization;
using MedicalSmartBox.API.Custody.Domain.Model.Aggregates;
using MedicalSmartBox.API.Custody.Domain.Model.Entities;
using MedicalSmartBox.API.Custody.Domain.Model.Events;
using MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;
using MedicalSmartBox.API.Custody.Interfaces.Rest.Resources;

namespace MedicalSmartBox.API.Custody.Interfaces.Rest.Transform;

public static class CustodyResourceFromEntityAssembler
{
    private static readonly CultureInfo Spanish = CultureInfo.GetCultureInfo("es-PE");
    private static readonly TimeZoneInfo LimaTimeZone = ResolveLimaTimeZone();

    public static CustodyTransferResource ToResourceFromEntity(CustodyTransfer transfer) => new(
        transfer.TransferCode,
        transfer.Id,
        transfer.TripId,
        transfer.Route,
        transfer.Cargo,
        FormatTimestamp(transfer.DepartureAt),
        transfer.ReceivedAt is null ? null : FormatTimestamp(transfer.ReceivedAt.Value),
        transfer.Recipient,
        ToStatusCode(transfer.Status),
        $"{transfer.TemperatureMax.ToString("0.0", Spanish)} °C",
        "2–8 °C",
        transfer.Openings,
        transfer.SmartBox,
        transfer.OtpToken?.CurrentAttempts ?? 0,
        3,
        transfer.Events.Select(ToResourceFromEntity).ToArray(),
        transfer.TemperatureReadings
            .Select(reading => new CustodyTemperatureReadingResource(reading.RecordedAt, reading.TemperatureCelsius))
            .ToArray());

    public static CustodyEventResource ToResourceFromEntity(CustodyEvent custodyEvent)
    {
        var local = TimeZoneInfo.ConvertTime(custodyEvent.At, LimaTimeZone);
        var type = custodyEvent.Title.Contains("rechazada", StringComparison.OrdinalIgnoreCase) ? "warning" :
            custodyEvent.Title.Contains("aceptada", StringComparison.OrdinalIgnoreCase) ? "success" : "normal";

        return new CustodyEventResource(
            local.ToString("dd/MM/yyyy", Spanish),
            local.ToString("HH:mm", Spanish),
            custodyEvent.Title,
            custodyEvent.Detail,
            type);
    }

    public static DigitalAuditManifestResource ToResourceFromEntity(
        DigitalAuditManifest manifest,
        CustodyTransfer transfer) => new(
        manifest.ManifestCode,
        manifest.Id,
        transfer.TransferCode,
        FormatTimestamp(manifest.GeneratedAt),
        manifest.CryptographicHashSha256,
        manifest.VerifySeal(transfer) ? "valid" : "invalid",
        manifest.Recipient,
        $"{manifest.MaxTemperatureCelsius.ToString("0.0", Spanish)} °C",
        transfer.Openings,
        transfer.Cargo,
        manifest.IsSealedAndImmutable,
        manifest.AverageTemperatureCelsius,
        manifest.MinTemperatureCelsius,
        manifest.MaxTemperatureCelsius,
        manifest.TotalExcursionSeconds,
        manifest.MinsaComplianceVerified,
        manifest.VerifySeal(transfer),
        manifest.CloudStorageUrl);

    public static AuditEventResource ToAuditResourceFromEntity(
        CustodyTransfer transfer,
        CustodyEvent custodyEvent,
        int index)
    {
        var item = ToResourceFromEntity(custodyEvent);
        return new AuditEventResource(
            $"{transfer.Id:N}-{index}",
            item.Date,
            item.Time,
            transfer.Recipient ?? "Sistema",
            custodyEvent.Title,
            transfer.TransferCode);
    }

    private static string ToStatusCode(CustodyStatus status) => status switch
    {
        CustodyStatus.InTransit => "in-transit",
        CustodyStatus.PendingOtpVerification => "pending-otp",
        CustodyStatus.OtpVerifiedLidUnlocked => "otp-verified",
        CustodyStatus.CompletedAccepted => "closed",
        CustodyStatus.RejectedThermalExcursion => "exception",
        CustodyStatus.RejectedTampering => "exception",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };

    private static string FormatTimestamp(DateTimeOffset value)
    {
        var local = TimeZoneInfo.ConvertTime(value, LimaTimeZone);
        return $"{local.ToString("dd/MM/yyyy", Spanish)} · {local.ToString("HH:mm", Spanish)}";
    }

    private static TimeZoneInfo ResolveLimaTimeZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("America/Lima"); }
        catch (TimeZoneNotFoundException) { return TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"); }
    }
}
