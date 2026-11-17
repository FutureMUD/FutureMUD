#nullable enable

using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MudSharp.Economy.Employment;

namespace MudSharp.Commands.Helpers;

internal sealed partial class EmploymentCommandService
{
	private void HandleTaskRequeue(ICharacter actor, IEmploymentHost host, StringStack input, bool retry)
	{
		if (!TryRequireAuthority(actor, host, EmploymentAuthority.AssignTasks, out var denied))
		{
			actor.OutputHandler.Send(denied);
			return;
		}

		var task = ActiveTaskBySelector(host, input.PopSpeech());
		if (task is null)
		{
			actor.OutputHandler.Send("Identify an existing task to retry or requeue.".ColourError());
			return;
		}

		var reason = input.SafeRemainingArgument;
		if (string.IsNullOrWhiteSpace(reason))
		{
			actor.OutputHandler.Send("An audit reason is required to retry or requeue a task.".ColourError());
			return;
		}

		if (task is EmploymentActiveTask concrete && !concrete.CanResume(out var recoveryReason, forRetry: retry, explicitRecovery: true))
		{
			actor.OutputHandler.Send(recoveryReason.ColourError());
			return;
		}

		try
		{
			var success = retry
				? host.TaskBoard.RetryActiveTask(task, actor, reason)
				: host.TaskBoard.RequeueActiveTask(task, actor, reason);
			actor.OutputHandler.Send(success
				? "Task requeued. Existing approval, completed-step and custody checks still apply."
				: "This task cannot be requeued in its current state. Inspect its status and custody evidence.".ColourError());
		}
		catch (DbUpdateConcurrencyException)
		{
			actor.OutputHandler.Send("Task approval changed; reload the task before retrying.".ColourError());
		}
	}

	private void HandleTaskApproval(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		const string syntax = "tasks approval show <task> | revoke <task-id> <grant-id> <revision> <reason> | " +
			"replace <task-id> <grant-id> <revision> <never|ISO-8601-expiry> <reason> | " +
			"reconcile <task-id> <grant-id> <revision> <step-number> <executed|not-executed> <evidence-reference> <reason>";
		var action = input.PopForSwitch();
		var selector = input.PopSpeech();
		var task = ActiveTaskBySelector(host, selector);
		if (task is null || action is not ("show" or "revoke" or "replace" or "reconcile"))
		{
			actor.OutputHandler.Send(syntax.ColourCommand());
			return;
		}

		if (action == "show")
		{
			SendOperationalView(actor, host, (viewer, _) => RenderApproval(viewer, task));
			return;
		}

		if (!Guid.TryParse(selector, out var taskId) || taskId != task.Id ||
		    !Guid.TryParse(input.PopSpeech(), out var grantId) || !int.TryParse(input.PopSpeech(), out var revision) || revision < 1)
		{
			actor.OutputHandler.Send("Use the exact task ID, grant ID and revision from approval show.".ColourError());
			return;
		}
		if (!TryRequireAuthority(actor, host, EmploymentAuthority.AssignTasks | task.ActionPlan.RequiredAuthority.Authorities, out var denied))
		{
			actor.OutputHandler.Send(denied);
			return;
		}

		string message;
		if (action == "reconcile")
		{
			var stepText = input.PopSpeech();
			var outcome = input.PopSpeech().ToLowerInvariant();
			var reference = input.PopSpeech();
			if (!int.TryParse(stepText, out var number) || outcome is not ("executed" or "not-executed") ||
			    string.IsNullOrWhiteSpace(reference) || input.IsFinished)
			{
				actor.OutputHandler.Send(syntax.ColourCommand());
				return;
			}
			host.TaskBoard.ReconcileFinancialStep(task, actor, grantId, revision, number, outcome == "executed",
				reference, input.SafeRemainingArgument, out message);
		}
		else
		{
			DateTimeOffset? expiresAt = null;
			if (action == "replace")
			{
				var expiry = input.PopSpeech();
				if (!expiry.EqualTo("never"))
				{
					if (!DateTimeOffset.TryParseExact(expiry, "O", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
					{
						actor.OutputHandler.Send("Use never or an ISO-8601 round-trip expiry with an explicit UTC offset.".ColourError());
						return;
					}
					expiresAt = parsed;
				}
			}
			host.TaskBoard.ChangeTaskApproval(task, actor, grantId, revision, action == "revoke", expiresAt,
				input.SafeRemainingArgument, out message);
		}
		actor.OutputHandler.Send(message);
	}

	private static string RenderApproval(ICharacter actor, IEmploymentActiveTask task)
	{
		var grant = task.AuthorisationGrant;
		var sb = new StringBuilder();
		sb.AppendLine($"Task: {task.Name.ColourName()} ({task.Id:D})");
		sb.AppendLine($"Grant: {grant.Id:D}; revision {grant.Revision.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Creator: {task.CreatedByPrincipal.Label.ColourName()}; authoriser: {(grant.AuthorityPrincipal ?? grant.IssuerPrincipal).Label.ColourName()}");
		sb.AppendLine($"Authority: {grant.Authority.Authorities.DescribeEnum().ColourName()}");
		sb.AppendLine($"Expiry: {(grant.ExpiresAt?.UtcDateTime.ToString("g", actor) ?? "Never").ColourValue()} UTC");
		sb.AppendLine($"Revoked: {grant.RevokedAt.HasValue.ToColouredString()} {grant.RevocationReason}");
		sb.AppendLine($"Unbounded financial steps: {grant.AllowsUnboundedFinancialSteps.ToColouredString()}");
		foreach (var limit in grant.AmountLimits.OrderBy(x => x.Key))
		{
			sb.AppendLine($"Currency {limit.Key.ToString("N0", actor)} limit: {limit.Value.ToString("G29", actor).ColourValue()}");
		}
		sb.AppendLine($"Payment sources: {grant.PaymentSourceScopes.ListToCommaSeparatedValues().ColourValue()}");
		sb.AppendLine($"Counterparties: {grant.CounterpartyScopes.ListToCommaSeparatedValues().ColourValue()}");
		sb.AppendLine($"Financial steps awaiting reconciliation: {(task.Provenance.UncertainFinancialSteps ?? new HashSet<int>()).OrderBy(x => x).Select(x => (x + 1).ToString("N0", actor)).ListToCommaSeparatedValues().ColourError()}");
		sb.AppendLine($"Explicit retry required: {task.Provenance.RequiresExplicitRetry.ToColouredString()}");
		if (task.Provenance.RecoveryIssue is not null) sb.AppendLine(task.Provenance.RecoveryIssue.ColourError());
		if (task is EmploymentActiveTask concrete && concrete.LegacyApproval)
		{
			sb.AppendLine("Legacy task: replacement explicitly approves the original plan's authored limits; no old approval is inferred.".ColourError());
		}
		sb.AppendLine("Replacement preserves existing limits/scopes, cumulative completed spending and reconciliation holds. It does not retry work.");
		return sb.ToString();
	}
}
