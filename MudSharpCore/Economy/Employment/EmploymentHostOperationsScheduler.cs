using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Arenas;
using MudSharp.Economy;
using MudSharp.Economy.Property;
using MudSharp.Economy.Stables;
using MudSharp.Framework;

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
		bool usePhysicalItemMovement = true)
	{
		var context = new EmploymentTaskContext(host, usePhysicalItemMovement);
		var lifecycleCandidates = host.EmploymentContracts
		                             .Where(x => x.Status is EmploymentStatus.Active or EmploymentStatus.Suspended)
		                             .ToList();

		var scheduledTasks = host.TaskBoard.EvaluateScheduledRules(context, now);
		var goalTasks = host.ManagerGoalBoard.EvaluateGoals(context, now);
		var payables = host.Payroll.EvaluatePayroll(now);
		var payrollEndedContracts = lifecycleCandidates
		                            .Where(x => x.Status == EmploymentStatus.Ended)
		                            .ToList();
		var additionalEndedContracts = host.Employment.EvaluateContractLifecycle(now)
		                                  .Where(x => payrollEndedContracts.All(y => y.Id != x.Id))
		                                  .ToList();
		var assignmentAudits = host.TaskBoard.AuditActiveTaskAssignments();

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

	private static IEnumerable<IEmploymentHost> EmploymentHosts(IFuturemud gameworld)
	{
		return EmploymentHostDiscovery.LoadedHosts(gameworld);
	}
}
