namespace MedicalSmartBox.API.Custody.Domain;

using MedicalSmartBox.API.Shared.Domain.Model;

public enum CustodyStatus
{
    InTransit,
    PendingOtpVerification,
    OtpVerifiedLidUnlocked,
    CompletedAccepted,
    RejectedThermalExcursion,
    RejectedTampering
}

/// <summary>Aggregate root enforcing the medical handover and custody audit invariants.</summary>
public sealed class CustodyTransfer : IAuditableEntity
{
    private readonly List<CustodyEvent> _events = [];

    private CustodyTransfer() { }

    public CustodyTransfer(Guid id, string transferCode, Guid tripId, Guid recipientHospitalId,
        Guid authorizedRecipientUserId, string route, string cargo, string smartBox,
        decimal temperatureMax, int openings, DateTimeOffset departureAt)
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
        TemperatureMax = temperatureMax;
        Openings = openings;
        DepartureAt = departureAt;
        Status = CustodyStatus.InTransit;
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
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public IReadOnlyCollection<CustodyEvent> Events => _events.AsReadOnly();

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
        _events.Add(new("Reception prepared", $"OTP issued to {Recipient}", now));
    }

    public bool VerifyOtp(string codeHash, DateTimeOffset now)
    {
        if (Status != CustodyStatus.PendingOtpVerification || OtpToken is null)
            throw new InvalidOperationException("Reception has not been prepared.");
        if (!OtpToken.ValidateCode(codeHash, now))
        {
            _events.Add(new("OTP verification failed", $"Attempt {OtpToken.CurrentAttempts} of 3", now));
            return false;
        }

        Status = CustodyStatus.OtpVerifiedLidUnlocked;
        _events.Add(new("OTP verified", "Recipient identity verified; lid unlocked", now));
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
        _events.Add(new("Custody accepted", RecipientNotes ?? "No receiving observations", now));
    }

    public void RejectDeliveryDueToThermalDamage(string reason, DateTimeOffset now) =>
        Reject(CustodyStatus.RejectedThermalExcursion, "Delivery rejected: thermal excursion", reason, now);

    public void RejectDeliveryDueToTampering(string reason, DateTimeOffset now) =>
        Reject(CustodyStatus.RejectedTampering, "Delivery rejected: tampering", reason, now);

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

public sealed record CustodyEvent(string Title, string Detail, DateTimeOffset At);
