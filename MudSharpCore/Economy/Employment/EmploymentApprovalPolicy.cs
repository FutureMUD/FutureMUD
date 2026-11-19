#nullable enable

using MudSharp.Character;

namespace MudSharp.Economy.Employment;

internal static class EmploymentApprovalPolicy
{
	internal static EmploymentPrincipal Principal(IEmploymentHost host, ICharacter? actor, string label,
		EmploymentAuthority required = EmploymentAuthority.None)
	{
		if (actor is null)
		{
			return EmploymentPrincipal.HostSystem(label);
		}

		var id = CharacterInstanceIdentityComparer.IdentityId(actor);
		var contract = host.Employment.EmploymentContracts.FirstOrDefault(x => x.Employee.Id == id &&
			x.Status == EmploymentStatus.Active && x.Authority.ContainsAll(new EmploymentAuthoritySet(required))) ??
			host.Employment.EmploymentContracts.FirstOrDefault(x => x.Employee.Id == id && x.Status == EmploymentStatus.Active);
		return new EmploymentPrincipal(actor.IsAdministrator() ? EmploymentPrincipalKind.Implementor : EmploymentPrincipalKind.Character,
			CharacterId: id, EmploymentContractId: contract?.Id, Label: actor.Name);
	}

	internal static bool CanExecute(EmploymentActiveTask task, out string reason)
	{
		if (!task.CanResume(out reason))
		{
			return false;
		}

		var grant = task.AuthorisationGrant;
		var now = DateTimeOffset.UtcNow;
		if (grant.RevokedAt.HasValue)
		{
			reason = $"Task approval was revoked: {grant.RevocationReason}";
			return false;
		}

		if (grant.ExpiresAt <= now)
		{
			reason = "Task approval has expired.";
			return false;
		}

		var principal = grant.AuthorityPrincipal ?? grant.IssuerPrincipal;
		if (!Enum.IsDefined(principal.Kind))
		{
			reason = "The approval issuer identity kind is not supported.";
			return false;
		}
		// An automation identity is provenance, not an authorising human or trusted host capability.
		if (principal.Kind is EmploymentPrincipalKind.ScheduledRule or EmploymentPrincipalKind.ManagerGoal ||
		    grant.Authority.Authorities == EmploymentAuthority.None)
		{
			reason = "Financial action requires an auditable payment authorisation with a verifiable issuer; legacy automation is not an authoriser.";
			return false;
		}

		if (principal.Kind == EmploymentPrincipalKind.HostSystem)
		{
			if (grant.AuthorityPrincipal is null)
			{
				reason = "Task has no explicit trusted host approval.";
				return false;
			}
			reason = string.Empty;
			return true;
		}

		if (principal.CharacterId is not { } characterId)
		{
			reason = "The approval issuer cannot be resolved.";
			return false;
		}

		var contracts = task.Employer.Employment.EmploymentContracts.Where(x => x.Employee.Id == characterId).ToList();
		if (principal.EmploymentContractId is { } contractId &&
		    !contracts.Any(x => x.Id == contractId && x.Status == EmploymentStatus.Active))
		{
			reason = "The original approval issuer's employment contract is no longer active.";
			return false;
		}

		var actor = contracts.Select(x => x.Employee).FirstOrDefault() ??
			EmploymentPersistenceStore.ResolveGameworld(task.Employer)?.TryGetCharacter(characterId, true);
		var required = grant.Authority.Authorities | grant.IssuanceAuthority;
		if (actor is null || !task.Employer.HasAuthority(actor, required) ||
		    principal.Kind == EmploymentPrincipalKind.Implementor && !actor.IsAdministrator() ||
		    grant.IssuanceAuthority.HasFlag(EmploymentAuthority.CreateManagerGoals) && !actor.IsAdministrator() &&
		    !contracts.Any(x => x.Status == EmploymentStatus.Active && x.Authority.ContainsAll(new EmploymentAuthoritySet(required))))
		{
			reason = "The approval issuer no longer has the required host authority.";
			return false;
		}

		reason = string.Empty;
		return true;
	}
}

public sealed partial class EmploymentTaskContext
{
	internal bool FinancialTaskAllowed(IEmploymentActionStep step, out string reason)
	{
		reason = string.Empty;
		if (_currentTask is not EmploymentActiveTask task)
		{
			return true; // Explicit standalone host operations retain their native authority checks.
		}

		if (_currentStepIndex < 0 || _currentStepIndex >= task.ActionPlan.Steps.Count ||
		    !ReferenceEquals(task.ActionPlan.Steps[_currentStepIndex], step))
		{
			reason = "Financial action is not bound to this task step.";
			return false;
		}

		if (!EmploymentApprovalPolicy.CanExecute(task, out reason)) return false;
		if (!task.AuthorisationGrant.CoversAuthority(step.RequiredAuthority))
		{
			reason = "Stored approval does not cover this financial action.";
			return false;
		}
		return true;
	}
}

public sealed partial class EmploymentActiveTask
{
	internal string? ApprovalPlanBinding { get; set; }
	internal bool LegacyApproval { get; set; }
	public bool HasUncertainFinancialSteps => Provenance.UncertainFinancialSteps is { Count: > 0 };

	internal bool CanResume(out string reason, bool forRetry = false, bool explicitRecovery = false)
	{
		reason = Provenance.RecoveryIssue ?? (HasUncertainFinancialSteps
			? "An interrupted financial attempt requires explicit manager reconciliation before this task can resume."
			: Provenance.RequiresExplicitRetry && !explicitRecovery
				? "This reconciled task requires explicit retry or requeue before work can resume."
			: Status is EmploymentTaskStatus.Completed or EmploymentTaskStatus.Cancelled || Status == EmploymentTaskStatus.Failed && !forRetry
				? "Terminal tasks cannot resume execution." : string.Empty);
		return string.IsNullOrEmpty(reason);
	}

	internal void HoldInterruptedFinancialSteps()
	{
		var uncertain = new HashSet<int>(Provenance.UncertainFinancialSteps ?? new HashSet<int>());
		for (var i = 0; i < Math.Min(_stepStates.Count, ActionPlan.Steps.Count); i++)
		{
			if (_stepStates[i] == EmploymentActionStepStatus.InProgress &&
			    (ActionPlan.Steps[i].IsFinancialStep || ActionPlan.Steps[i].RequiresPaymentAuthorisation))
			{
				uncertain.Add(i);
			}
		}
		Provenance = Provenance with { UncertainFinancialSteps = uncertain };
	}

	internal void Quarantine(string reason) => Provenance = Provenance with { RecoveryIssue = reason };

	internal void CommitApprovalTransition(EmploymentTaskProvenance provenance, EmploymentRegisterEntry entry,
		Action? update = null)
	{
		var before = Provenance;
		var states = _stepStates.ToList();
		var operations = _stepOperationalStates.ToList();
		var status = Status;
		var employee = AssignedEmployee;
		var blocked = BlockedReason;
		try
		{
			Provenance = provenance;
			update?.Invoke();
			_persistence?.SaveApprovalTransition(this, entry, before.AuthorisationGrant.Id, before.AuthorisationGrant.Revision);
		}
		catch
		{
			Provenance = before;
			_stepStates.Clear();
			_stepStates.AddRange(states);
			_stepOperationalStates.Clear();
			_stepOperationalStates.AddRange(operations);
			Status = status;
			AssignedEmployee = employee;
			BlockedReason = blocked;
			throw;
		}
		if (Employer.EmploymentRegister is EmploymentRegister register)
		{
			register.AddForCompatibilityLoad([entry]);
		}
		LegacyApproval = LegacyApproval && AuthorisationGrant.Authority.Authorities == EmploymentAuthority.None;
	}

	internal void ApplyReconciliation(int index, bool executed, string reference, string reason)
	{
		var cancelled = Status == EmploymentTaskStatus.Cancelled;
		_stepStates[index] = executed ? EmploymentActionStepStatus.Completed : EmploymentActionStepStatus.Pending;
		_stepOperationalStates[index] = _stepOperationalStates[index] with
		{
			TransactionReference = reference,
			FailureDiagnostic = $"Manager reconciliation: {reason}"
		};
		var uncertain = new HashSet<int>(Provenance.UncertainFinancialSteps ?? new HashSet<int>());
		uncertain.Remove(index);
		Provenance = Provenance with { UncertainFinancialSteps = uncertain, AssignedAt = null };
		AssignedEmployee = null;
		Status = cancelled ? EmploymentTaskStatus.Cancelled : _stepStates.All(x => x == EmploymentActionStepStatus.Completed)
			? EmploymentTaskStatus.Completed : EmploymentTaskStatus.Blocked;
		Provenance = Provenance with { RequiresExplicitRetry = Status == EmploymentTaskStatus.Blocked };
		BlockedReason = Status == EmploymentTaskStatus.Completed ? null : "Financial attempt reconciled; explicit retry is required.";
		if (Status == EmploymentTaskStatus.Completed)
		{
			Provenance = Provenance with { CompletedAt = EmploymentClock.CurrentInstant(Employer) };
		}
	}
}
