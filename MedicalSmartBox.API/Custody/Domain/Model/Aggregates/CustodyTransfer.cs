using MedicalSmartBox.API.Shared.Domain.Model;
using MedicalSmartBox.API.Custody.Domain.Model.Events;
using MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;

namespace MedicalSmartBox.API.Custody.Domain.Model.Aggregates;

/// <summary>Aggregate root enforcing the medical handover and custody audit invariants.</summary>
public sealed class CustodyTransfer : IAuditableEntity
{
    private readonly List<CustodyEvent> _events = [];
    private readonly List<CustodyTemperatureReading> _temperatureReadings = [];

    private CustodyTransfer() { }

    public CustodyTransfer(Guid id, string transferCode, Guid tripId, Guid recipientHospitalId,
        Guid authorizedRecipientUserId, string route, string cargo, string smartBox,
        decimal temperatureMax, int openings, DateTimeOffset departureAt,
        IEnumerable<CustodyTemperatureReading>? temperatureReadings = null)
    {
        if (id == Guid.Empty) throw new ArgumentException("A transfer identifier is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(transferCode)) throw new ArgumentException("A transfer code is required.", nameof(transferCode));
        Id = id;
        TransferCode = transferCode;
        TripId = tripId;
        RecipientHospitalId = recipientHospitalId;
        AuthorizedRecipientUserId = authorizedRecipientUserId;
        Route = route;
        Cargo = cargo;
        SmartBox = smartBox;
        _temperatureReadings.AddRange((temperatureReadings ?? []).OrderBy(reading => reading.RecordedAt));
        if (_temperatureReadings.Any(reading => reading.RecordedAt < departureAt))
            throw new ArgumentException("Temperature evidence cannot precede the transfer departure.", nameof(temperatureReadings));
        TemperatureMax = _temperatureReadings.Count > 0 ? _temperatureReadings.Max(reading => reading.TemperatureCelsius) : temperatureMax;
        Openings = openings;
        DepartureAt = departureAt;
        Status = CustodyStatus.InTransit;
        _events.Add(new("Traslado iniciado", $"Salida registrada desde origen · {TransferCode}", departureAt));
    }

    public Guid Id { get; private set; }
    public string TransferCode { get; private set; } = string.Empty;
    public Guid TripId { get; private set; }
    public Guid RecipientHospitalId { get; private set; }
    public Guid AuthorizedRecipientUserId { get; private set; }
    public string Route { get; private set; } = string.Empty;
    public string Cargo { get; private set; } = string.Empty;
    public string SmartBox { get; private set; } = string.Empty;
    public DateTimeOffset DepartureAt { get; private set; }
    public DateTimeOffset? ReceivedAt { get; private set; }
    public decimal TemperatureMax { get; private set; }
    public int Openings { get; private set; }
    public string? Recipient { get; private set; }
    public string? RecipientNotes { get; private set; }
    public CustodyStatus Status { get; private set; }
    public OtpToken? OtpToken { get; private set; }
    public IReadOnlyCollection<CustodyTemperatureReading> TemperatureReadings => _temperatureReadings.AsReadOnly();
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public IReadOnlyCollection<CustodyEvent> Events => _events.AsReadOnly();

    public void RecordTemperatureReadings(IEnumerable<CustodyTemperatureReading> readings)
    {
        EnsureOpen();
        var incoming = readings.ToArray();
        if (incoming.Length == 0) throw new ArgumentException("At least one temperature reading is required.", nameof(readings));
        if (!incoming.SequenceEqual(incoming.OrderBy(reading => reading.RecordedAt)))
            throw new ArgumentException("Temperature readings must be in chronological order.", nameof(readings));
        var previous = _temperatureReadings.LastOrDefault()?.RecordedAt ?? DepartureAt.AddTicks(-1);
        if (incoming.Any(reading => reading.RecordedAt <= previous || reading.RecordedAt < DepartureAt))
            throw new ArgumentException("Temperature evidence must be later than the last recorded reading.", nameof(readings));

        _temperatureReadings.AddRange(incoming);
        TemperatureMax = Math.Max(TemperatureMax, incoming.Max(reading => reading.TemperatureCelsius));
        _events.Add(new("Evidencia térmica registrada", $"Se adjuntaron {incoming.Length} lecturas de temperatura", incoming[^1].RecordedAt));
    }

    public void PrepareReception(string recipient, string otpHash, DateTimeOffset expiresAt,
        bool insideDestinationGeofence, DateTimeOffset now)
    {
        EnsureOpen();
        if (!insideDestinationGeofence) throw new InvalidOperationException("The transfer is outside the destination geofence.");
        if (string.IsNullOrWhiteSpace(recipient)) throw new ArgumentException("A receiving custodian is required.", nameof(recipient));
        if (string.IsNullOrWhiteSpace(otpHash)) throw new ArgumentException("An OTP hash is required.", nameof(otpHash));
        if (expiresAt <= now) throw new ArgumentOutOfRangeException(nameof(expiresAt), "OTP expiry must be in the future.");

        Recipient = recipient.Trim();
        OtpToken = new OtpToken(otpHash, expiresAt);
        Status = CustodyStatus.PendingOtpVerification;
        _events.Add(new("Recepción preparada", $"Código OTP emitido para {Recipient}", now));
    }

    public bool VerifyOtp(string codeHash, DateTimeOffset now)
    {
        if (Status != CustodyStatus.PendingOtpVerification || OtpToken is null)
            throw new InvalidOperationException("Reception has not been prepared.");
        if (!OtpToken.ValidateCode(codeHash, now))
        {
            _events.Add(new("Verificación OTP fallida", $"Intento {OtpToken.CurrentAttempts} de 3", now));
            return false;
        }

        Status = CustodyStatus.OtpVerifiedLidUnlocked;
        _events.Add(new("Código OTP verificado", "Identidad del receptor verificada; tapa desbloqueada", now));
        return true;
    }

    public void ConfirmCustodyAcceptance(string? notes, bool hasUnresolvedCriticalAlerts, DateTimeOffset now)
    {
        if (Status != CustodyStatus.OtpVerifiedLidUnlocked)
            throw new InvalidOperationException("Verify the recipient OTP before confirming reception.");
        if (hasUnresolvedCriticalAlerts && string.IsNullOrWhiteSpace(notes))
            throw new InvalidOperationException("A receiving note is required while critical alerts remain unresolved.");

        Status = CustodyStatus.CompletedAccepted;
        RecipientNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        ReceivedAt = now;
        ClearOtp();
        _events.Add(new("Recepción aceptada", RecipientNotes ?? "Sin observaciones de recepción", now));
    }

    public void RejectDeliveryDueToThermalDamage(string reason, DateTimeOffset now) =>
        Reject(CustodyStatus.RejectedThermalExcursion, "Entrega rechazada por excursión térmica", reason, now);

    public void RejectDeliveryDueToTampering(string reason, DateTimeOffset now) =>
        Reject(CustodyStatus.RejectedTampering, "Entrega rechazada por manipulación", reason, now);

    private void Reject(CustodyStatus status, string title, string reason, DateTimeOffset now)
    {
        EnsureOpen();
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("A rejection reason is required.", nameof(reason));
        Status = status;
        RecipientNotes = reason.Trim();
        ReceivedAt = now;
        ClearOtp();
        _events.Add(new(title, RecipientNotes, now));
    }

    private void ClearOtp()
    {
        OtpToken = null;
    }

    private void EnsureOpen()
    {
        if (Status is CustodyStatus.CompletedAccepted or CustodyStatus.RejectedThermalExcursion or CustodyStatus.RejectedTampering)
            throw new InvalidOperationException("Custody is already finalized.");
    }

}
