using System.Security.Cryptography;

namespace MedicalSmartBox.API.Custody.Domain.Model.ValueObjects;

/// <summary>One-time recipient credential with a short lifetime and a three-attempt limit.</summary>
public sealed class OtpToken
{
    private OtpToken() { }

    public OtpToken(string hashedCode, DateTimeOffset expiresAt)
    {
        if (string.IsNullOrWhiteSpace(hashedCode)) throw new ArgumentException("OTP hash is required.", nameof(hashedCode));
        HashedCode = hashedCode;
        ExpiresAtUtc = expiresAt;
    }

    public string HashedCode { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public int MaxAttemptsAllowed => 3;
    public int CurrentAttempts { get; private set; }
    public DateTimeOffset? VerifiedAtUtc { get; private set; }

    public bool ValidateCode(string candidateHash, DateTimeOffset now)
    {
        if (IsExpired(now)) throw new InvalidOperationException("The OTP has expired.");
        if (VerifiedAtUtc is not null) throw new InvalidOperationException("The OTP has already been used.");
        if (CurrentAttempts >= MaxAttemptsAllowed) throw new InvalidOperationException("The maximum number of OTP attempts has been reached.");

        CurrentAttempts++;
        if (!TryDecodeHash(HashedCode, out var expected) || !TryDecodeHash(candidateHash, out var actual) ||
            !CryptographicOperations.FixedTimeEquals(expected, actual)) return false;

        VerifiedAtUtc = now;
        return true;
    }

    public bool IsExpired(DateTimeOffset now) => ExpiresAtUtc <= now;

    private static bool TryDecodeHash(string? hash, out byte[] bytes)
    {
        try { bytes = Convert.FromHexString(hash ?? string.Empty); return bytes.Length == 32; }
        catch (FormatException) { bytes = []; return false; }
    }
}
