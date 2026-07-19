using System.Runtime.CompilerServices;

#nullable enable

namespace MudSharp.Economy.Employment;

public sealed record EmploymentHostOperationsResult(
	IEmploymentHost Host,
	DateTimeOffset EvaluatedAt,
	IReadOnlyCollection<IEmploymentActiveTask> ScheduledRuleTasks,
	IReadOnlyCollection<IEmploymentActiveTask> ManagerGoalTasks,
	IReadOnlyCollection<IEmploymentPayable> PayablesCreated,
	IReadOnlyCollection<IEmploymentContract> ContractsEnded,
	IReadOnlyCollection<EmploymentTaskAssignmentAuditResult> AssignmentAudits)
{
	public int ScheduledRuleTaskCount => ScheduledRuleTasks.Count;
	public int ManagerGoalTaskCount => ManagerGoalTasks.Count;
	public int PayableCount => PayablesCreated.Count;
	public int ContractEndCount => ContractsEnded.Count;
	public int AssignmentAuditCount => AssignmentAudits.Count;
}

public sealed record EmploymentHostOperationsSummary(IReadOnlyCollection<EmploymentHostOperationsResult> Results)
{
	public int ScheduledRuleTaskCount => Results.Sum(x => x.ScheduledRuleTaskCount);
	public int ManagerGoalTaskCount => Results.Sum(x => x.ManagerGoalTaskCount);
	public int PayableCount => Results.Sum(x => x.PayableCount);
	public int ContractEndCount => Results.Sum(x => x.ContractEndCount);
	public int AssignmentAuditCount => Results.Sum(x => x.AssignmentAuditCount);
}

public static class EmploymentHostOperationsScheduler
{
	private sealed class HostEvaluationState
	{
		public object SyncRoot { get; } = new();
		public DateTimeOffset? LastGeneralEvaluation { get; set; }
		public DateTimeOffset? LastManagerGoalEvaluation { get; set; }
	}

	private static readonly ConditionalWeakTable<IEmploymentHost, HostEvaluationState> EvaluationStates = new();

	public static EmploymentHostOperationsSummary EvaluateAll(IFuturemud gameworld, bool usePhysicalItemMovement = true)
	{
		var results = new List<EmploymentHostOperationsResult>();
		foreach (var host in EmploymentHosts(gameworld))
		{
			try
			{
				results.Add(EvaluateHost(host, EmploymentClock.CurrentInstant(host), usePhysicalItemMovement));
			}
			catch (Exception ex)
			{
				host.DebugEmployment(
					$"Central employment operations scheduler failed for {host.EmploymentHostName}: {ex.Message}",
					gameworld);
			}
		}

		return new EmploymentHostOperationsSummary(results);
	}

	public static EmploymentHostOperationsResult EvaluateHost(IEmploymentHost host, DateTimeOffset now,
		bool usePhysicalItemMovement = true, bool evaluateManagerGoals = true,
		bool evaluateGeneralOperations = true)
	{
		var hasScheduledRules = evaluateGeneralOperations && host.TaskBoard.ScheduledRules.Count > 0;
		var hasManagerGoals = evaluateManagerGoals && host.ManagerGoalBoard.Goals.Any(IsEvaluableGoal);
		var hasContracts = evaluateGeneralOperations && host.EmploymentContracts.Any(IsLiveContract);
		var hasAuditableTasks = evaluateGeneralOperations && host.TaskBoard.ActiveTasks.Any(IsAuditableTask);
		if (!hasScheduledRules && !hasManagerGoals && !hasContracts && !hasAuditableTasks)
		{
			return EmptyResult(host, now);
		}

		var context = hasScheduledRules || hasManagerGoals
			? new EmploymentTaskContext(host, usePhysicalItemMovement)
			: null;
		var lifecycleCandidates = hasContracts
			? host.EmploymentContracts.Where(IsLiveContract).ToList()
			: [];
		// Scheduled-rule evaluation already audits assignments before inspecting rules.
		var assignmentAudits = hasAuditableTasks && !hasScheduledRules
			? host.TaskBoard.AuditActiveTaskAssignments()
			: Array.Empty<EmploymentTaskAssignmentAuditResult>();
		var scheduledTasks = hasScheduledRules
			? host.TaskBoard.EvaluateScheduledRules(context!, now)
			: Array.Empty<IEmploymentActiveTask>();
		var goalTasks = hasManagerGoals
			? host.ManagerGoalBoard.EvaluateGoals(context!, now)
			: Array.Empty<IEmploymentActiveTask>();
		var payables = hasContracts
			? host.Payroll.EvaluatePayroll(now)
			: Array.Empty<IEmploymentPayable>();
		var payrollEndedContracts = lifecycleCandidates
		                            .Where(x => x.Status == EmploymentStatus.Ended)
		                            .ToList();
		var additionalEndedContracts = (hasContracts
				? host.Employment.EvaluateContractLifecycle(now)
				: Array.Empty<IEmploymentContract>())
		                                  .Where(x => payrollEndedContracts.All(y => y.Id != x.Id))
		                                  .ToList();

		var result = new EmploymentHostOperationsResult(
			host,
			now,
			scheduledTasks,
			goalTasks,
			payables,
			payrollEndedContracts.Concat(additionalEndedContracts).ToList(),
			assignmentAudits);
		host.DebugEmployment(
			$"Central employment operations scheduler evaluated {host.EmploymentHostName}: " +
			$"{result.ScheduledRuleTaskCount:N0} scheduled task(s), {result.ManagerGoalTaskCount:N0} manager-goal task(s), " +
			$"{result.PayableCount:N0} payable(s), {result.ContractEndCount:N0} ended contract(s), " +
			$"{result.AssignmentAuditCount:N0} assignment audit action(s).");
		return result;
	}

	public static bool TryEvaluateHost(IEmploymentHost host, DateTimeOffset now,
		TimeSpan minimumInterval, out EmploymentHostOperationsResult? result,
		bool usePhysicalItemMovement = true, bool evaluateManagerGoals = true)
	{
		var hasGeneralWork = host.TaskBoard.ScheduledRules.Count > 0 ||
		                     host.EmploymentContracts.Any(IsLiveContract) ||
		                     host.TaskBoard.ActiveTasks.Any(IsAuditableTask);
		var hasManagerGoals = evaluateManagerGoals && host.ManagerGoalBoard.Goals.Any(IsEvaluableGoal);
		if (!hasGeneralWork && !hasManagerGoals)
		{
			result = null;
			return false;
		}

		var state = EvaluationStates.GetOrCreateValue(host);
		lock (state.SyncRoot)
		{
			var evaluateGeneral = hasGeneralWork && IsDue(state.LastGeneralEvaluation, now, minimumInterval);
			var evaluateGoals = hasManagerGoals && IsDue(state.LastManagerGoalEvaluation, now, minimumInterval);
			if (!evaluateGeneral && !evaluateGoals)
			{
				result = null;
				return false;
			}

			result = EvaluateHost(host, now, usePhysicalItemMovement, evaluateGoals, evaluateGeneral);
			if (evaluateGeneral)
			{
				state.LastGeneralEvaluation = now;
			}

			if (evaluateGoals)
			{
				state.LastManagerGoalEvaluation = now;
			}

			return true;
		}
	}

	private static bool IsDue(DateTimeOffset? lastEvaluation, DateTimeOffset now, TimeSpan minimumInterval)
	{
		return !lastEvaluation.HasValue || now < lastEvaluation.Value || now - lastEvaluation.Value >= minimumInterval;
	}

	private static bool IsLiveContract(IEmploymentContract contract)
	{
		return contract.Status is EmploymentStatus.Active or EmploymentStatus.Suspended;
	}

	private static bool IsEvaluableGoal(IManagerGoal goal)
	{
		return goal.Status is ManagerGoalStatus.Active or ManagerGoalStatus.Satisfied;
	}

	private static bool IsAuditableTask(IEmploymentActiveTask task)
	{
		return task.Status is EmploymentTaskStatus.Assigned or EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked;
	}

	private static EmploymentHostOperationsResult EmptyResult(IEmploymentHost host, DateTimeOffset now)
	{
		return new EmploymentHostOperationsResult(
			host,
			now,
			Array.Empty<IEmploymentActiveTask>(),
			Array.Empty<IEmploymentActiveTask>(),
			Array.Empty<IEmploymentPayable>(),
			Array.Empty<IEmploymentContract>(),
			Array.Empty<EmploymentTaskAssignmentAuditResult>());
	}

	private static IEnumerable<IEmploymentHost> EmploymentHosts(IFuturemud gameworld)
	{
		return EmploymentHostDiscovery.LoadedHosts(gameworld);
	}
}
