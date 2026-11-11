namespace MedicalSmartBox.API.alerting.Domain.Model.Entities;

/// <summary>Records the mitigation and quality signoff for a critical incident.</summary>
public class ContingencyResolution
{
    public Guid Id { get; private set; }
    public Guid IncidentId { get; private set; }
    public string MitigationAction { get; private set; } = string.Empty;
    public string QualitySignoffNotes { get; private set; } = string.Empty;
    public bool WasThermalIntegrityRestored { get; private set; }
    public Guid ResolvedByUserId { get; private set; }
    public DateTimeOffset ResolvedAt { get; private set; }

    private ContingencyResolution() { }

    public ContingencyResolution(Guid incidentId, string mitigationAction, string qualitySignoffNotes,
        bool wasThermalIntegrityRestored, Guid resolvedByUserId)
    {
        if (incidentId == Guid.Empty) throw new ArgumentException("Incident ID is required.", nameof(incidentId));
        if (resolvedByUserId == Guid.Empty) throw new ArgumentException("Resolver ID is required.", nameof(resolvedByUserId));
        if (string.IsNullOrWhiteSpace(mitigationAction) || mitigationAction.Trim().Length > 500)
            throw new ArgumentException("Mitigation action is required and must contain at most 500 characters.", nameof(mitigationAction));
        if (string.IsNullOrWhiteSpace(qualitySignoffNotes))
            throw new ArgumentException("Quality signoff notes are required.", nameof(qualitySignoffNotes));

        Id = Guid.NewGuid();
        IncidentId = incidentId;
        MitigationAction = mitigationAction.Trim();
        QualitySignoffNotes = qualitySignoffNotes.Trim();
        WasThermalIntegrityRestored = wasThermalIntegrityRestored;
        ResolvedByUserId = resolvedByUserId;
        ResolvedAt = DateTimeOffset.UtcNow;
    }
}
