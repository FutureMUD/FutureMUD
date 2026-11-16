#nullable enable
using System;
using System.Linq;
using System.Text.Json;
using MudSharp.Models;

namespace MudSharp.Economy.Employment;

/// <summary>Versioned task-owned approval snapshots. Invalid/legacy rows never acquire authority.</summary>
internal static class EmploymentTaskProvenancePersistence
{
	internal const int Version = 1;

	internal static void Create(EmploymentActiveTask task, EmploymentActiveTaskRecord record)
	{
		if (!task.HasRecordedProvenance)
		{
			return;
		}

		var provenance = task.Provenance;
		var grant = provenance.AuthorisationGrant;
		record.ProvenanceVersion = Version;
		record.SourceKind = (int)provenance.SourceKind;
		record.SourceRuleId = provenance.SourceRuleId?.ToString("D");
		record.SourceGoalId = provenance.SourceGoalId;
		record.CreatedByPrincipal = JsonSerializer.Serialize(provenance.CreatedByPrincipal);
		record.AuthorisedByPrincipal = JsonSerializer.Serialize(provenance.AuthorisedByPrincipal);
		record.AuthorisationGrant = new EmploymentTaskGrantRecord
		{
			Task = record,
			Version = Version,
			PublicId = grant.Id.ToString("D"),
			EmploymentHostStateId = record.EmploymentHostStateId,
			EmploymentActionPlanId = record.EmploymentActionPlanId,
			TaskPublicId = record.PublicId,
			CorrelationId = grant.CorrelationId.ToString("D"),
			IssuerPrincipal = JsonSerializer.Serialize(grant.IssuerPrincipal),
			Authority = (long)grant.Authority.Authorities,
			AllowsUnboundedFinancialSteps = grant.AllowsUnboundedFinancialSteps,
			ExpectedLimitCount = grant.AmountLimits.Count,
			ExpectedPaymentScopeCount = grant.PaymentSourceScopes.Count,
			ExpectedCounterpartyScopeCount = grant.CounterpartyScopes.Count,
			IssuedAtTicks = grant.IssuedAt.UtcTicks,
			ExpiresAtTicks = grant.ExpiresAt?.UtcTicks
		};
		foreach (var limit in grant.AmountLimits)
		{
			record.AuthorisationGrant.Limits.Add(new EmploymentTaskGrantLimitRecord
				{ CurrencyId = limit.Key, Amount = limit.Value });
		}
		foreach (var scope in grant.PaymentSourceScopes)
		{
			record.AuthorisationGrant.Scopes.Add(new EmploymentTaskGrantScopeRecord { Kind = 0, Scope = scope });
		}
		foreach (var scope in grant.CounterpartyScopes)
		{
			record.AuthorisationGrant.Scopes.Add(new EmploymentTaskGrantScopeRecord { Kind = 1, Scope = scope });
		}
		Update(task, record);
		if (Read(record, out var diagnostic) is null)
		{
			throw new InvalidOperationException(diagnostic);
		}
	}

	internal static void Update(EmploymentActiveTask task, EmploymentActiveTaskRecord record)
	{
		record.FinancialRecoveryRequired |= task.RequiresFinancialReconciliation;
		if (!task.HasRecordedProvenance)
		{
			return; // Keep missing/corrupt original evidence, rather than blessing a compatibility grant.
		}
		var provenance = task.Provenance;
		record.Priority = provenance.Priority;
		record.CreatedAtTicks = provenance.CreatedAt.UtcTicks;
		record.DueAtTicks = provenance.DueAt?.UtcTicks;
		record.AssignedAtTicks = provenance.AssignedAt?.UtcTicks;
		record.StartedAtTicks = provenance.StartedAt?.UtcTicks;
		record.CompletedAtTicks = provenance.CompletedAt?.UtcTicks;
		var grant = record.AuthorisationGrant ?? throw new InvalidOperationException("Persisted task approval is missing.");
		// Revocation is sticky, including when a stale instance checkpoints after another reader revoked it.
		if (!grant.RevokedAtTicks.HasValue && provenance.AuthorisationGrant.RevokedAt.HasValue)
		{
			grant.RevokedAtTicks = provenance.AuthorisationGrant.RevokedAt.Value.UtcTicks;
			grant.RevokedByPrincipal = JsonSerializer.Serialize(provenance.AuthorisationGrant.RevokedBy);
			grant.RevocationReason = provenance.AuthorisationGrant.RevocationReason;
		}
	}

	internal static EmploymentTaskProvenance? Read(EmploymentActiveTaskRecord record, out string diagnostic)
	{
		diagnostic = record.ProvenanceVersion is null
			? "Legacy task has no recorded approval provenance."
			: "Persisted task approval is incomplete, unsupported or bound to another host/task/plan.";
		try
		{
			var grant = record.AuthorisationGrant;
			if (record.ProvenanceVersion != Version || record.SourceKind is null ||
			    !Enum.IsDefined((EmploymentTaskSourceKind)record.SourceKind.Value) ||
			    record.Priority is null || record.CreatedAtTicks is null || grant is null || grant.Version != Version ||
			    grant.EmploymentHostStateId != record.EmploymentHostStateId ||
			    grant.EmploymentActionPlanId != record.EmploymentActionPlanId ||
			    !Guid.TryParse(record.PublicId, out var taskId) || !Guid.TryParse(grant.TaskPublicId, out var boundTask) || taskId != boundTask ||
			    !Guid.TryParse(grant.PublicId, out var grantId) || grantId == Guid.Empty ||
			    !Guid.TryParse(record.CorrelationId, out var correlation) ||
			    !Guid.TryParse(grant.CorrelationId, out var boundCorrelation) || correlation != boundCorrelation ||
			    grant.ExpectedLimitCount != grant.Limits.Count ||
			    grant.ExpectedPaymentScopeCount != grant.Scopes.Count(x => x.Kind == 0) ||
			    grant.ExpectedCounterpartyScopeCount != grant.Scopes.Count(x => x.Kind == 1) ||
			    grant.Limits.Any(x => x.CurrencyId <= 0 || x.Amount < 0) ||
			    grant.Scopes.Any(x => x.Kind is not (0 or 1) || string.IsNullOrWhiteSpace(x.Scope)))
			{
				return null;
			}

			var sourceRule = record.SourceRuleId is null ? (Guid?)null : Guid.Parse(record.SourceRuleId);
			var sourceKind = (EmploymentTaskSourceKind)record.SourceKind.Value;
			if ((sourceKind == EmploymentTaskSourceKind.ScheduledRule && sourceRule is null) ||
			    (sourceKind == EmploymentTaskSourceKind.ManagerGoal && record.SourceGoalId is null))
			{
				return null;
			}
			var restored = new EmploymentAuthorisationGrant(grantId, Principal(grant.IssuerPrincipal),
				new EmploymentAuthoritySet((EmploymentAuthority)grant.Authority),
				grant.Limits.ToDictionary(x => x.CurrencyId, x => x.Amount), grant.AllowsUnboundedFinancialSteps,
				Instant(grant.IssuedAtTicks), OptionalInstant(grant.ExpiresAtTicks), correlation,
				grant.Scopes.Where(x => x.Kind == 0).Select(x => x.Scope).ToHashSet(StringComparer.InvariantCultureIgnoreCase),
				grant.Scopes.Where(x => x.Kind == 1).Select(x => x.Scope).ToHashSet(StringComparer.InvariantCultureIgnoreCase),
				OptionalInstant(grant.RevokedAtTicks),
				grant.RevokedAtTicks.HasValue ? Principal(grant.RevokedByPrincipal) : null, grant.RevocationReason);
			var result = new EmploymentTaskProvenance(sourceKind, sourceRule, record.SourceGoalId,
				Principal(record.CreatedByPrincipal), Principal(record.AuthorisedByPrincipal), restored,
				record.Priority.Value, Instant(record.CreatedAtTicks.Value), OptionalInstant(record.DueAtTicks),
				OptionalInstant(record.AssignedAtTicks), OptionalInstant(record.StartedAtTicks), OptionalInstant(record.CompletedAtTicks));
			diagnostic = string.Empty;
			return result;
		}
		catch (Exception ex) when (ex is JsonException or ArgumentException or InvalidOperationException or FormatException)
		{
			return null;
		}
	}

	private static DateTimeOffset Instant(long ticks) => new(ticks, TimeSpan.Zero);
	private static DateTimeOffset? OptionalInstant(long? ticks) => ticks.HasValue ? Instant(ticks.Value) : null;

	private static EmploymentPrincipal Principal(string? json)
	{
		var principal = json is null ? null : JsonSerializer.Deserialize<EmploymentPrincipal>(json);
		if (principal is null || !Enum.IsDefined(principal.Kind) ||
		    (principal.Kind == EmploymentPrincipalKind.Character && principal.CharacterId is not > 0) ||
		    (principal.Kind == EmploymentPrincipalKind.EmploymentContract && principal.EmploymentContractId is not > 0) ||
		    (principal.Kind == EmploymentPrincipalKind.ScheduledRule && principal.ScheduledRuleId is null) ||
		    (principal.Kind == EmploymentPrincipalKind.ManagerGoal && principal.ManagerGoalId is not > 0))
		{
			throw new InvalidOperationException("Invalid recorded employment principal.");
		}
		return principal;
	}
}
