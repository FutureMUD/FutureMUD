#nullable enable

using Microsoft.EntityFrameworkCore;

namespace MudSharp.Economy.Employment;

public sealed partial class EmploymentTaskBoard
{
	private bool CanRecoverApproval(IEmploymentActiveTask task, ICharacter actor, Guid expectedGrantId,
		int expectedRevision, string reason, out EmploymentActiveTask concrete, out string message, bool reconcile = false)
	{
		concrete = task as EmploymentActiveTask ?? null!;
		message = "Task approval cannot be changed.";
		if (concrete is null || !_activeTasks.Contains(task) || !ReferenceEquals(task.Employer, _host)) return false;
		if (actor is null || !IsAuthorised(actor, EmploymentAuthority.AssignTasks | task.ActionPlan.RequiredAuthority.Authorities))
		{
			message = "Approval recovery requires task assignment and every authority required by the original task plan.";
			return false;
		}
		if (task.Status == EmploymentTaskStatus.Completed || task.Status == EmploymentTaskStatus.Cancelled && !reconcile ||
		    task.Provenance.RecoveryIssue is not null)
		{
			message = task.Provenance.RecoveryIssue ?? "Terminal task approvals cannot be changed.";
			return false;
		}
		if (task.AuthorisationGrant.Id != expectedGrantId || task.AuthorisationGrant.Revision != expectedRevision)
		{
			message = "Task approval changed; show it again before making a recovery decision.";
			return false;
		}
		if (string.IsNullOrWhiteSpace(reason))
		{
			message = "An audit reason is required.";
			return false;
		}
		return true;
	}

	public bool ChangeTaskApproval(IEmploymentActiveTask task, ICharacter actor, Guid expectedGrantId,
		int expectedRevision, bool revoke, DateTimeOffset? expiresAt, string reason, out string message)
	{
		if (!CanRecoverApproval(task, actor, expectedGrantId, expectedRevision, reason, out var concrete, out message)) return false;
		var now = DateTimeOffset.UtcNow;
		if (!revoke && expiresAt <= now)
		{
			message = "A replacement approval must expire in the future, or explicitly have no expiry.";
			return false;
		}
		var previous = task.AuthorisationGrant;
		var principal = EmploymentApprovalPolicy.Principal(_host, actor, "Task approval recovery",
			task.ActionPlan.RequiredAuthority.Authorities | EmploymentAuthority.AssignTasks);
		var replacement = revoke ? previous with
		{
			RevokedAt = now, RevocationReason = reason.Trim(), Revision = checked(previous.Revision + 1)
		} : (concrete.LegacyApproval
			? CreateTaskProvenance(_host, task.ActionPlan, task.CorrelationId, task.SourceKind,
				task.CreatedByPrincipal, principal, task.Priority, now).AuthorisationGrant
			: previous) with
		{
			Id = Guid.NewGuid(), IssuerPrincipal = principal, AuthorityPrincipal = principal,
			IssuanceAuthority = EmploymentAuthority.AssignTasks, IssuedAt = now, ExpiresAt = expiresAt,
			RevokedAt = null, RevocationReason = null, Revision = checked(previous.Revision + 1)
		};
		var provenance = task.Provenance with
		{
			AuthorisedByPrincipal = revoke ? task.AuthorisedByPrincipal : principal,
			AuthorisationGrant = replacement
		};
		var description = $"{(revoke ? "Revoked" : "Replaced")} task {task.Id:D} approval: {reason.Trim()}. " +
			$"Before={EmploymentApprovalJson.Write(previous)}; After={EmploymentApprovalJson.Write(replacement)}";
		var entry = new EmploymentRegisterEntry(task.CorrelationId, EmploymentRegisterEntryType.TaskApprovalChanged,
			_host, actor, description, EmploymentClock.CurrentInstant(_host));
		try { concrete.CommitApprovalTransition(provenance, entry); }
		catch (DbUpdateConcurrencyException) { message = "Task approval changed; show it again before making a recovery decision."; return false; }
		catch (InvalidOperationException ex) { message = ex.Message; return false; }
		message = $"Task approval {(revoke ? "revoked" : "replaced")}. Grant {replacement.Id:D}, revision {replacement.Revision:N0}. " +
			"This does not retry the task or clear a financial reconciliation hold.";
		return true;
	}

	public bool ReconcileFinancialStep(IEmploymentActiveTask task, ICharacter actor, Guid expectedGrantId,
		int expectedRevision, int stepNumber, bool executed, string reference, string reason, out string message)
	{
		if (!CanRecoverApproval(task, actor, expectedGrantId, expectedRevision, reason, out var concrete, out message, reconcile: true)) return false;
		var index = stepNumber - 1;
		if (index < 0 || index >= task.ActionPlan.Steps.Count ||
		    task.Provenance.UncertainFinancialSteps?.Contains(index) != true || string.IsNullOrWhiteSpace(reference))
		{
			message = "Identify an interrupted financial step and supply its reconciliation evidence reference.";
			return false;
		}
		if (executed && (task.ActionPlan.Steps[index] is PurchaseActionStep { IsExecutablePurchase: true } or PhysicalFloatActionStep) &&
		    string.IsNullOrWhiteSpace(task.StepOperationalStates[index].SelectedResources) &&
		    string.IsNullOrWhiteSpace(task.StepOperationalStates[index].LoadedAssets))
		{
			message = "Native execution was confirmed but durable task item/cash custody evidence is missing. " +
				"The attempt remains held pending verified task-custody repair; native inventory/cash changes alone cannot clear it.";
			return false;
		}
		var provenance = task.Provenance with
		{
			AuthorisationGrant = task.AuthorisationGrant with { Revision = checked(task.AuthorisationGrant.Revision + 1) }
		};
		var description = $"Reconciled task {task.Id:D}, step {stepNumber:N0}, grant {expectedGrantId:D}/{expectedRevision:N0}: " +
			$"{(executed ? "confirmed executed; no native operation repeated" : "confirmed not executed; eligible for normal retry")}. " +
			$"Evidence: {reference.Trim()}. Reason: {reason.Trim()}";
		var entry = new EmploymentRegisterEntry(task.CorrelationId, EmploymentRegisterEntryType.TaskFinancialStepReconciled,
			_host, actor, description, EmploymentClock.CurrentInstant(_host));
		try { concrete.CommitApprovalTransition(provenance, entry, () => concrete.ApplyReconciliation(index, executed, reference.Trim(), reason.Trim())); }
		catch (DbUpdateConcurrencyException) { message = "Task approval changed; show it again before making a recovery decision."; return false; }
		catch (InvalidOperationException ex) { message = ex.Message; return false; }
		message = "Financial attempt reconciled and audited. No native payment was executed by this command.";
		return true;
	}
}

internal sealed class UnresolvedEmploymentStep : EmploymentActionStepBase
{
	internal UnresolvedEmploymentStep() : base(EmploymentActionStepType.CataloguedShell,
		EmploymentAuthoritySet.Empty, [], true, true) { }
	public override bool CanExecute(IEmploymentTaskContext context, ICharacter actor, out string reason)
	{
		reason = "A stored action step cannot be reconstructed; its task is quarantined.";
		return false;
	}
	public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor) =>
		EmploymentActionStepResult.Blocked("A stored action step cannot be reconstructed.");
}
