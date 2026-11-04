using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using MedicalSmartBox.API.Custody.Domain;

namespace MedicalSmartBox.API.Custody.Infrastructure.Reporting;

public static partial class TraceabilityReportPdf
{
    private const double PlotX = 52;
    private const double PlotY = 465;
    private const double PlotWidth = 490;
    private const double PlotHeight = 145;

    public static byte[] Create(CustodyTransfer transfer, DigitalAuditManifest? manifest)
    {
        var content = new StringBuilder();
        AddText(content, 18, 50, 795, "Medical SmartBox - Informe de trazabilidad");
        AddText(content, 11, 50, 766, $"Traslado: {transfer.TransferCode}    Ruta: {transfer.Route}");
        AddText(content, 11, 50, 748, $"Carga: {transfer.Cargo}    Contenedor: {transfer.SmartBox}");
        AddText(content, 11, 50, 730, $"Salida: {transfer.DepartureAt:yyyy-MM-dd HH:mm} UTC    Recepcion: {transfer.ReceivedAt:yyyy-MM-dd HH:mm} UTC");
        AddText(content, 11, 50, 712, $"Responsable: {transfer.Recipient ?? "-"}    Aperturas: {transfer.Openings}    Estado: {GetStatusLabel(transfer.Status)}");
        if (manifest is not null)
        {
            AddText(content, 10, 50, 694, $"Manifiesto: {manifest.ManifestCode}    SHA-256: {manifest.CryptographicHashSha256}");
            AddText(content, 10, 50, 677, $"Promedio: {manifest.AverageTemperatureCelsius:F1} C    Minima: {manifest.MinTemperatureCelsius:F1} C    Maxima: {manifest.MaxTemperatureCelsius:F1} C    Excursion: {manifest.TotalExcursionSeconds}s");
            AddText(content, 10, 50, 660, $"Cumplimiento termico MINSA: {(manifest.MinsaComplianceVerified ? "Verificado" : "Desviacion registrada")}");
        }

        DrawTemperatureChart(content, transfer.TemperatureReadings.OrderBy(reading => reading.RecordedAt).ToArray());
        AddText(content, 12, 50, 437, "Eventos de cadena de custodia");
        var y = 417d;
        foreach (var item in transfer.Events.OrderBy(item => item.At).Take(16))
        {
            AddText(content, 9, 50, y, $"{item.At:yyyy-MM-dd HH:mm:ss} UTC  {item.Title}: {item.Detail}");
            y -= 16;
        }
        if (transfer.Events.Count > 16)
            AddText(content, 9, 50, y, $"... Hay {transfer.Events.Count - 16} eventos mas disponibles en la API de auditoria.");

        var stream = Encoding.ASCII.GetBytes(content.ToString());
        var objects = new[]
        {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>",
            $"<< /Length {stream.Length} >>\nstream\n{content}\nendstream",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"
        };
        return BuildPdf(objects);
    }

    private static void DrawTemperatureChart(StringBuilder content, IReadOnlyList<CustodyTemperatureReading> readings)
    {
        AddText(content, 12, 50, 635, "Historial de temperatura (C)");
        content.AppendLine("0.35 w 0.25 G");
        content.AppendLine($"{PlotX.ToString(CultureInfo.InvariantCulture)} {PlotY.ToString(CultureInfo.InvariantCulture)} {PlotWidth.ToString(CultureInfo.InvariantCulture)} {PlotHeight.ToString(CultureInfo.InvariantCulture)} re S");
        foreach (var limit in new[] { 2m, 8m })
        {
            var y = PlotY + (double)limit / 10 * PlotHeight;
            content.AppendLine($"0.75 G {PlotX} {y.ToString(CultureInfo.InvariantCulture)} m {PlotX + PlotWidth} {y.ToString(CultureInfo.InvariantCulture)} l S");
        }

        if (readings.Count == 0) return;
        content.AppendLine("0.05 0.48 0.42 RG 1.8 w");
        for (var index = 0; index < readings.Count; index++)
        {
            var x = readings.Count == 1 ? PlotX + PlotWidth / 2 : PlotX + PlotWidth * index / (readings.Count - 1);
            var y = PlotY + Math.Clamp((double)readings[index].TemperatureCelsius / 10, 0, 1) * PlotHeight;
            content.AppendLine($"{x.ToString(CultureInfo.InvariantCulture)} {y.ToString(CultureInfo.InvariantCulture)} {(index == 0 ? "m" : "l")}");
        }
        content.AppendLine("S");
        content.AppendLine("0 G 0.6 w");
    }

    private static void AddText(StringBuilder content, int fontSize, double x, double y, string text)
    {
        var normalized = RemoveDiacritics(text).Replace("→", "-").Replace("–", "-").Replace("°", "");
        var safe = PdfEscapeRegex().Replace(normalized, "\\$1");
        content.AppendLine($"BT /F1 {fontSize} Tf {x.ToString(CultureInfo.InvariantCulture)} {y.ToString(CultureInfo.InvariantCulture)} Td ({safe}) Tj ET");
    }

    private static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var result = new StringBuilder(normalized.Length);
        foreach (var character in normalized)
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark && character <= 127)
                result.Append(character);
        return result.ToString();
    }

    private static byte[] BuildPdf(IReadOnlyList<string> objects)
    {
        using var output = new MemoryStream();
        Write(output, "%PDF-1.4\n");
        var offsets = new List<long> { 0 };
        for (var index = 0; index < objects.Count; index++)
        {
            offsets.Add(output.Position);
            Write(output, $"{index + 1} 0 obj\n{objects[index]}\nendobj\n");
        }
        var xref = output.Position;
        Write(output, $"xref\n0 {objects.Count + 1}\n0000000000 65535 f \n");
        foreach (var offset in offsets.Skip(1)) Write(output, $"{offset:0000000000} 00000 n \n");
        Write(output, $"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");
        return output.ToArray();
    }

    private static void Write(Stream stream, string text)
    {
        var bytes = Encoding.ASCII.GetBytes(text);
        stream.Write(bytes, 0, bytes.Length);
    }

    private static string GetStatusLabel(CustodyStatus status) => status switch
    {
        CustodyStatus.InTransit => "En transito",
        CustodyStatus.PendingOtpVerification => "Verificacion OTP pendiente",
        CustodyStatus.OtpVerifiedLidUnlocked => "OTP verificado",
        CustodyStatus.CompletedAccepted => "Cerrada",
        CustodyStatus.RejectedThermalExcursion => "Rechazada por excursion termica",
        CustodyStatus.RejectedTampering => "Rechazada por manipulacion",
        _ => status.ToString()
    };

    [GeneratedRegex("([\\\\()])")]
    private static partial Regex PdfEscapeRegex();
}
