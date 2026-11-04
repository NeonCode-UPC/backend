using System.Globalization;
using MedicalSmartBox.API.Custody.Domain;

namespace MedicalSmartBox.API.Custody.Application;

public sealed record CustodyTransferDto(string Id, Guid TransferId, Guid TripId, string Route, string Cargo,
    string DepartureAt, string? ReceivedAt, string? Recipient, string Status, string TemperatureMax,
    string TemperatureRange, int Openings, string SmartBox, IReadOnlyList<CustodyEventDto> Events);

public sealed record CustodyEventDto(string Date, string Time, string Title, string Detail, string Type);

public sealed record DigitalAuditManifestDto(string Id, Guid ManifestId, string TransferId, string SealedAt,
    string Hash, string Status, string Recipient, string TemperatureMax, int Openings, string Cargo,
    bool IsSealedAndImmutable, decimal AverageTemperatureCelsius, decimal MinTemperatureCelsius,
    decimal MaxTemperatureCelsius, int TotalExcursionSeconds, bool MinsaComplianceVerified,
    string? CloudStorageUrl);

public sealed record AuditEventDto(string Id, string Date, string Time, string User, string Event, string Resource);

public static class CustodyContractMapper
{
    private static readonly CultureInfo Spanish = CultureInfo.GetCultureInfo("es-PE");
    private static readonly TimeZoneInfo LimaTimeZone = ResolveLimaTimeZone();

    public static CustodyTransferDto ToDto(CustodyTransfer transfer) => new(
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
        transfer.Events.Select(ToDto).ToArray());

    public static CustodyEventDto ToDto(CustodyEvent custodyEvent)
    {
        var local = TimeZoneInfo.ConvertTime(custodyEvent.At, LimaTimeZone);
        return new(local.ToString("dd/MM/yyyy", Spanish), local.ToString("HH:mm", Spanish),
            custodyEvent.Title, custodyEvent.Detail, custodyEvent.Title.Contains("rejected", StringComparison.OrdinalIgnoreCase) ? "warning" :
            custodyEvent.Title.Contains("accepted", StringComparison.OrdinalIgnoreCase) ? "success" : "normal");
    }

    public static DigitalAuditManifestDto ToDto(DigitalAuditManifest manifest, CustodyTransfer transfer) => new(
        manifest.ManifestCode,
        manifest.Id,
        transfer.TransferCode,
        FormatTimestamp(manifest.GeneratedAt),
        manifest.CryptographicHashSha256,
        manifest.IsSealedAndImmutable ? "valid" : "invalid",
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
        manifest.CloudStorageUrl);

    public static AuditEventDto ToAuditDto(CustodyTransfer transfer, CustodyEvent custodyEvent, int index)
    {
        var item = ToDto(custodyEvent);
        return new($"{transfer.Id:N}-{index}", item.Date, item.Time, transfer.Recipient ?? "Sistema",
            custodyEvent.Title, transfer.TransferCode);
    }

    public static string ToStatusCode(CustodyStatus status) => status switch
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
