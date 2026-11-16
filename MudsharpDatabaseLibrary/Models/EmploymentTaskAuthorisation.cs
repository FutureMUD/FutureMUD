#nullable enable
using System.Collections.Generic;

namespace MudSharp.Models;

public partial class EmploymentActiveTaskRecord
{
	public int? ProvenanceVersion { get; set; }
	public int? SourceKind { get; set; }
	public string? SourceRuleId { get; set; }
	public long? SourceGoalId { get; set; }
	public string? CreatedByPrincipal { get; set; }
	public string? AuthorisedByPrincipal { get; set; }
	public int? Priority { get; set; }
	public long? CreatedAtTicks { get; set; }
	public long? DueAtTicks { get; set; }
	public long? AssignedAtTicks { get; set; }
	public long? StartedAtTicks { get; set; }
	public long? CompletedAtTicks { get; set; }
	public bool FinancialRecoveryRequired { get; set; }
	public virtual EmploymentTaskGrantRecord? AuthorisationGrant { get; set; }
}

/// <summary>A task-owned approval snapshot, independent of the issuer's later deletion.</summary>
public class EmploymentTaskGrantRecord
{
	public long EmploymentActiveTaskId { get; set; }
	public int Version { get; set; }
	public string PublicId { get; set; } = string.Empty;
	public long EmploymentHostStateId { get; set; }
	public long EmploymentActionPlanId { get; set; }
	public string TaskPublicId { get; set; } = string.Empty;
	public string CorrelationId { get; set; } = string.Empty;
	public string IssuerPrincipal { get; set; } = string.Empty;
	public long Authority { get; set; }
	public bool AllowsUnboundedFinancialSteps { get; set; }
	public int ExpectedLimitCount { get; set; }
	public int ExpectedPaymentScopeCount { get; set; }
	public int ExpectedCounterpartyScopeCount { get; set; }
	public long IssuedAtTicks { get; set; }
	public long? ExpiresAtTicks { get; set; }
	public long? RevokedAtTicks { get; set; }
	public string? RevokedByPrincipal { get; set; }
	public string? RevocationReason { get; set; }
	public virtual EmploymentActiveTaskRecord Task { get; set; } = null!;
	public virtual ICollection<EmploymentTaskGrantLimitRecord> Limits { get; set; } = new HashSet<EmploymentTaskGrantLimitRecord>();
	public virtual ICollection<EmploymentTaskGrantScopeRecord> Scopes { get; set; } = new HashSet<EmploymentTaskGrantScopeRecord>();
}

public class EmploymentTaskGrantLimitRecord
{
	public long EmploymentActiveTaskId { get; set; }
	public long CurrencyId { get; set; }
	public decimal Amount { get; set; }
	public virtual EmploymentTaskGrantRecord Grant { get; set; } = null!;
}

public class EmploymentTaskGrantScopeRecord
{
	public long EmploymentActiveTaskId { get; set; }
	public int Kind { get; set; }
	public string Scope { get; set; } = string.Empty;
	public virtual EmploymentTaskGrantRecord Grant { get; set; } = null!;
}
