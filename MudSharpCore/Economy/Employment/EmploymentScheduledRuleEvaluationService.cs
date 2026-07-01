using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Arenas;
using MudSharp.Economy;
using MudSharp.Economy.Stables;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Economy.Employment;

public static class EmploymentScheduledRuleEvaluationService
{
	public static int EvaluateAll(IFuturemud gameworld)
	{
		var spawned = 0;
		foreach (var host in EmploymentHosts(gameworld))
		{
			if (!ShouldEvaluateHost(host))
			{
				host.DebugEmployment(
					"Central scheduled-rule evaluator skipped this host because it has no persisted scheduled-rule state.",
					gameworld);
				continue;
			}

			var taskBoard = host.TaskBoard;
			if (!taskBoard.ScheduledRules.Any())
			{
				continue;
			}

			try
			{
				var context = new EmploymentTaskContext(host, usePhysicalItemMovement: true);
				var now = EmploymentClock.CurrentInstant(host);
				var created = taskBoard.EvaluateScheduledRules(context, now);
				spawned += created.Count;
				host.DebugEmployment(
					$"Central scheduled-rule evaluator inspected {taskBoard.ScheduledRules.Count:N0} rule(s) and spawned {created.Count:N0} task(s).",
					gameworld);
			}
			catch (Exception ex)
			{
				host.DebugEmployment(
					$"Central scheduled-rule evaluator failed for {host.EmploymentHostName}: {ex.Message}",
					gameworld);
			}
		}

		return spawned;
	}

	private static bool ShouldEvaluateHost(IEmploymentHost host)
	{
		var hostGameworld = ResolveGameworld(host);
		if (hostGameworld is null)
		{
			return host.TaskBoard.ScheduledRules.Any();
		}

		return EmploymentPersistenceStore.HasScheduledRules(host);
	}

	private static IFuturemud? ResolveGameworld(IEmploymentHost host)
	{
		return host switch
		{
			IHaveFuturemud have => have.Gameworld,
			IHotel { Property: IHaveFuturemud property } => property.Gameworld,
			_ => null
		};
	}

	private static IEnumerable<IEmploymentHost> EmploymentHosts(IFuturemud gameworld)
	{
		return EmploymentHostDiscovery.LoadedHosts(gameworld);
	}
}
