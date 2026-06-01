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
			if (!host.TaskBoard.ScheduledRules.Any())
			{
				continue;
			}

			try
			{
				var context = new EmploymentTaskContext(host, usePhysicalItemMovement: true);
				var now = EmploymentClock.CurrentInstant(host);
				var created = host.TaskBoard.EvaluateScheduledRules(context, now);
				spawned += created.Count;
				host.DebugEmployment(
					$"Central scheduled-rule evaluator inspected {host.TaskBoard.ScheduledRules.Count:N0} rule(s) and spawned {created.Count:N0} task(s).",
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

	private static IEnumerable<IEmploymentHost> EmploymentHosts(IFuturemud gameworld)
	{
		foreach (var shop in gameworld.Shops)
		{
			yield return shop;
		}

		foreach (var auction in gameworld.AuctionHouses)
		{
			yield return auction;
		}

		foreach (var arena in gameworld.CombatArenas.OfType<ICombatArena>())
		{
			yield return arena;
		}

		foreach (var bank in gameworld.Banks)
		{
			yield return bank;
		}

		foreach (var stable in gameworld.Stables.OfType<IStable>())
		{
			yield return stable;
		}

		foreach (var hotel in gameworld.Properties.Select(x => x.Hotel).Where(x => x is not null).Cast<IHotel>())
		{
			yield return hotel;
		}
	}
}
