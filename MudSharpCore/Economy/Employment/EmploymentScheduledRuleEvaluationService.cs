using System.Runtime.CompilerServices;
using MudSharp.Economy.Property;

#nullable enable

namespace MudSharp.Economy.Employment;

public static class EmploymentScheduledRuleEvaluationService
{
	private sealed class ScheduledRuleHostRegistry
	{
		private readonly HashSet<EmploymentHostKey> _hostKeys;
		private readonly object _syncRoot = new();

		public ScheduledRuleHostRegistry(IEnumerable<EmploymentHostKey> hostKeys)
		{
			_hostKeys = new HashSet<EmploymentHostKey>(hostKeys);
		}

		public bool Contains(IEmploymentHost host)
		{
			lock (_syncRoot)
			{
				return _hostKeys.Contains(EmploymentHostKey.For(host));
			}
		}

		public void Add(IEmploymentHost host)
		{
			lock (_syncRoot)
			{
				_hostKeys.Add(EmploymentHostKey.For(host));
			}
		}

		public void Remove(IEmploymentHost host)
		{
			lock (_syncRoot)
			{
				_hostKeys.Remove(EmploymentHostKey.For(host));
			}
		}
	}

	private static readonly ConditionalWeakTable<IFuturemud, ScheduledRuleHostRegistry> HostRegistries = new();

	public static int EvaluateAll(IFuturemud gameworld)
	{
		var spawned = 0;
		var registry = gameworld is Futuremud
			? HostRegistries.GetValue(gameworld,
				_ => new ScheduledRuleHostRegistry(EmploymentPersistenceStore.LoadScheduledRuleHostKeys()))
			: null;
		foreach (var host in EmploymentHosts(gameworld))
		{
			if (registry is not null && !registry.Contains(host))
			{
				continue;
			}

			var taskBoard = host.TaskBoard;
			if (taskBoard.ScheduledRules.Count == 0)
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

	internal static void RegisterHost(IEmploymentHost host)
	{
		var gameworld = ResolveGameworld(host);
		if (gameworld is not Futuremud)
		{
			return;
		}

		HostRegistries.GetValue(gameworld,
			_ => new ScheduledRuleHostRegistry(EmploymentPersistenceStore.LoadScheduledRuleHostKeys())).Add(host);
	}

	internal static void UnregisterHost(IEmploymentHost host)
	{
		var gameworld = ResolveGameworld(host);
		if (gameworld is null || !HostRegistries.TryGetValue(gameworld, out var registry))
		{
			return;
		}

		registry.Remove(host);
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
