using System.Security.Cryptography;
using System.Text;
using MedicalSmartBox.API.Custody.Domain;

namespace MedicalSmartBox.API.Custody.Application;

public sealed class CustodyApplicationService(
    ICustodyRepository repository,
    IConfiguration configuration,
    IHostEnvironment environment,
    TimeProvider timeProvider)
{
    public Task<IReadOnlyList<CustodyTransfer>> ListTransfersAsync(CancellationToken cancellationToken = default) =>
        repository.ListTransfersAsync(cancellationToken);

    public Task<IReadOnlyList<DigitalAuditManifest>> ListManifestsAsync(CancellationToken cancellationToken = default) =>
        repository.ListManifestsAsync(cancellationToken);

    public Task<IReadOnlyList<CustodyEvent>> GetAuditEventsAsync(CancellationToken cancellationToken = default) =>
        repository.GetAuditEventsAsync(cancellationToken);

    public Task<CustodyTransfer?> FindTransferAsync(string identifier, CancellationToken cancellationToken = default) =>
        repository.FindTransferAsync(identifier, cancellationToken);

    public Task<DigitalAuditManifest?> FindManifestAsync(string identifier, CancellationToken cancellationToken = default) =>
        repository.FindManifestByTransferAsync(identifier, cancellationToken);

    public async Task<(CustodyTransfer Transfer, string? DevelopmentCode)> PrepareReceptionAsync(
        string identifier, string recipient, bool insideDestinationGeofence, CancellationToken cancellationToken = default)
    {
        var transfer = await RequireTransferAsync(identifier, cancellationToken);
        var now = timeProvider.GetUtcNow();
        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        transfer.PrepareReception(recipient, HashOtp(code), now.AddMinutes(15), insideDestinationGeofence, now);
        await repository.SaveChangesAsync(cancellationToken);
        return (transfer, environment.IsDevelopment() ? code : null);
    }

    public async Task<bool> VerifyOtpAsync(string identifier, string code, CancellationToken cancellationToken = default)
    {
        var transfer = await RequireTransferAsync(identifier, cancellationToken);
        var verified = transfer.VerifyOtp(HashOtp(code), timeProvider.GetUtcNow());
        await repository.SaveChangesAsync(cancellationToken);
        return verified;
    }

    public async Task<(CustodyTransfer Transfer, DigitalAuditManifest Manifest)> ConfirmReceptionAsync(
        string identifier, string? notes, bool hasUnresolvedCriticalAlerts, CancellationToken cancellationToken = default)
    {
        var transfer = await RequireTransferAsync(identifier, cancellationToken);
        var now = timeProvider.GetUtcNow();
        transfer.ConfirmCustodyAcceptance(notes, hasUnresolvedCriticalAlerts, now);

        // The current API has no telemetry query adapter. These values preserve the
        // available transfer snapshot until that bounded context exposes its read port.
        var manifest = DigitalAuditManifest.Seal(transfer, now, transfer.TemperatureMax,
            transfer.TemperatureMax, 0, 2m, 8m);
        await repository.AddManifestAsync(manifest, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return (transfer, manifest);
    }

    private async Task<CustodyTransfer> RequireTransferAsync(string identifier, CancellationToken cancellationToken) =>
        await repository.FindTransferAsync(identifier, cancellationToken)
        ?? throw new KeyNotFoundException($"Custody transfer '{identifier}' was not found.");

    private string HashOtp(string code)
    {
        if (code.Length != 6 || code.Any(character => character is < '0' or > '9'))
            throw new ArgumentException("OTP must contain exactly 6 digits.", nameof(code));

        var key = configuration["Custody:OtpHmacKey"];
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Configure Custody:OtpHmacKey before enabling OTP verification.");
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(code))).ToLowerInvariant();
    }
}
