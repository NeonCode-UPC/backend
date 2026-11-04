using System.Security.Cryptography;

namespace MedicalSmartBox.API.Custody.Domain;

public enum CustodyStatus { InTransit, PendingOtpVerification, Closed, Exception }

/// <summary>Aggregate root that controls the handover protocol for one medical shipment.</summary>
public sealed class CustodyTransfer
{
    private readonly List<CustodyEvent> _events = [];
    private string? _otpHash;
    private DateTimeOffset? _otpExpiresAt;
    private int _otpAttempts;
    private bool _otpVerified;

    private CustodyTransfer() { }

    public CustodyTransfer(string id, string route, string cargo, string smartBox, decimal temperatureMax, int openings, string? recipient = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Transfer id is required.", nameof(id));
        Id = id;
        Route = route;
        Cargo = cargo;
        SmartBox = smartBox;
        TemperatureMax = temperatureMax;
        Openings = openings;
        Recipient = recipient;
        Status = CustodyStatus.InTransit;
    }

    public string Id { get; private set; } = string.Empty;
    public string Route { get; private set; } = string.Empty;
    public string Cargo { get; private set; } = string.Empty;
    public string SmartBox { get; private set; } = string.Empty;
    public decimal TemperatureMax { get; private set; }
    public int Openings { get; private set; }
    public string? Recipient { get; private set; }
    public DateTimeOffset? ReceivedAt { get; private set; }
    public CustodyStatus Status { get; private set; }
    public IReadOnlyCollection<CustodyEvent> Events => _events.AsReadOnly();

    public void PrepareReception(string recipient, string otpHash, DateTimeOffset expiresAt, bool insideDestinationGeofence, DateTimeOffset now)
    {
        EnsureNotClosed();
        if (!insideDestinationGeofence) throw new InvalidOperationException("The transfer is outside the destination geofence.");
        if (string.IsNullOrWhiteSpace(recipient)) throw new ArgumentException("A receiving custodian is required.", nameof(recipient));
        Recipient = recipient;
        _otpHash = otpHash;
        _otpExpiresAt = expiresAt;
        _otpAttempts = 0;
        _otpVerified = false;
        Status = CustodyStatus.PendingOtpVerification;
        _events.Add(new("Reception prepared", $"OTP issued to {recipient}", now));
    }

    public bool VerifyOtp(string codeHash, DateTimeOffset now)
    {
        if (Status != CustodyStatus.PendingOtpVerification || _otpHash is null) throw new InvalidOperationException("Reception has not been prepared.");
        if (_otpExpiresAt <= now) throw new InvalidOperationException("The OTP has expired.");
        if (++_otpAttempts > 3) throw new InvalidOperationException("The maximum number of OTP attempts has been reached.");
        if (!CryptographicOperations.FixedTimeEquals(Convert.FromHexString(_otpHash), Convert.FromHexString(codeHash))) return false;
        _otpVerified = true;
        _events.Add(new("OTP verified", "Recipient identity verified", now));
        return true;
    }

    public void ConfirmReception(DateTimeOffset now)
    {
        EnsureNotClosed();
        if (Status != CustodyStatus.PendingOtpVerification || !_otpVerified) throw new InvalidOperationException("Verify the recipient OTP before confirming reception.");
        Status = CustodyStatus.Closed;
        ReceivedAt = now;
        _otpHash = null;
        _otpExpiresAt = null;
        _otpVerified = false;
        _events.Add(new("Custody closed", $"Received by {Recipient}", now));
    }

    private void EnsureNotClosed()
    {
        if (Status == CustodyStatus.Closed) throw new InvalidOperationException("Custody is already closed.");
    }
}

public sealed record CustodyEvent(string Title, string Detail, DateTimeOffset At);
