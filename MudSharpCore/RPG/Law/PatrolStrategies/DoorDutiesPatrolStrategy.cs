using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using System.Globalization;

namespace MudSharp.RPG.Law.PatrolStrategies;

public class DoorDutiesPatrolStrategy : PatrolStrategyBase, IConfigurablePatrolStrategy
{
	public override string Name => "DoorDuties";

	public DoorDutiesPatrolStrategy(IFuturemud gameworld, string strategyData = null)
		: base(gameworld)
	{
		LoadStrategyData(strategyData);
	}

	public DoorguardAccessMode AccessMode { get; private set; } = DoorguardAccessMode.NormalRules;

	public string HelpText => @"Door duties configuration:

	#3access normal|enforcers|everyone#0 - sets who patrol-managed doorguards open for

Normal uses the assigned DoorguardAI's existing will-open rules, such as clan-brother checks. Enforcers opens for characters with enforcement authority for this legal authority. Everyone opens for anyone.";

	private static ICell DutyLocation(IPatrol patrol)
	{
		return patrol.PatrolRoute.PatrolNodes.FirstOrDefault();
	}

	private void EnableDoorGuardMode(IPatrol patrol)
	{
		ICell dutyLocation = DutyLocation(patrol);
		if (dutyLocation is null)
		{
			return;
		}

		ICell[] dutyCells = { dutyLocation };
		foreach (ICharacter member in patrol.PatrolMembers.Where(x => x.Location == dutyLocation))
		{
			if (!member.AffectedBy<IDoorguardModeEffect>() && HasKeysForCells(member, dutyCells))
			{
				member.AddEffect(new PatrolDoorguardMode(member, patrol.LegalAuthority, AccessMode));
			}
		}
	}

	private static void DisableDoorGuardMode(IPatrol patrol)
	{
		foreach (ICharacter member in patrol.PatrolMembers)
		{
			member.RemoveAllEffects<PatrolDoorguardMode>(fireRemovalAction: true);
		}
	}

	private static void CompleteDoorDuty(IPatrol patrol)
	{
		DisableDoorGuardMode(patrol);
		patrol.CompletePatrol();
	}

	private static void AbortDoorDuty(IPatrol patrol)
	{
		DisableDoorGuardMode(patrol);
		patrol.AbortPatrol();
	}

	public override void HandlePatrolCompleted(IPatrol patrol)
	{
		DisableDoorGuardMode(patrol);
	}

	public override void HandlePatrolAborted(IPatrol patrol)
	{
		DisableDoorGuardMode(patrol);
	}

	public bool ReadyToBegin(IPatrolRoute patrol)
	{
		return true;
	}

	public string SaveStrategyData()
	{
		return new XElement("DoorDuties",
			new XAttribute("access", (int)AccessMode)
		).ToString();
	}

	private void LoadStrategyData(string strategyData)
	{
		if (string.IsNullOrWhiteSpace(strategyData))
		{
			return;
		}

		XElement root;
		try
		{
			root = XElement.Parse(strategyData);
		}
		catch
		{
			return;
		}

		if (int.TryParse(root.Attribute("access")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) &&
		    Enum.IsDefined(typeof(DoorguardAccessMode), value))
		{
			AccessMode = (DoorguardAccessMode)value;
			return;
		}

		if (Enum.TryParse(root.Attribute("access")?.Value, true, out DoorguardAccessMode mode))
		{
			AccessMode = mode;
		}
	}

	public bool BuildingCommand(ICharacter actor, IPatrolRoute patrol, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "access":
			case "mode":
			case "open":
				return BuildingCommandAccess(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandAccess(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Should patrol-managed doorguards open using normal rules, for enforcers only, or for everyone?");
			return false;
		}

		switch (command.SafeRemainingArgument.ToLowerInvariant())
		{
			case "normal":
			case "rules":
			case "normalrules":
			case "clan":
			case "clanbrother":
			case "clan brother":
				AccessMode = DoorguardAccessMode.NormalRules;
				break;
			case "enforcer":
			case "enforcers":
			case "enforcersonly":
			case "enforcers only":
			case "law":
				AccessMode = DoorguardAccessMode.EnforcersOnly;
				break;
			case "everyone":
			case "all":
			case "anyone":
			case "public":
				AccessMode = DoorguardAccessMode.Everyone;
				break;
			default:
				actor.OutputHandler.Send("That is not a valid access mode. Use normal, enforcers or everyone.");
				return false;
		}

		actor.OutputHandler.Send($"Patrol-managed doorguards will now open for {DescribeAccessMode(actor)}.");
		return true;
	}

	private string DescribeAccessMode(ICharacter actor)
	{
		return AccessMode switch
		{
			DoorguardAccessMode.NormalRules => "normal DoorguardAI rules".ColourValue(),
			DoorguardAccessMode.EnforcersOnly => "enforcers for this legal authority".ColourValue(),
			DoorguardAccessMode.Everyone => "everyone".ColourValue(),
			_ => AccessMode.DescribeEnum().ColourValue()
		};
	}

	public string ShowConfiguration(ICharacter actor, IPatrolRoute patrol)
	{
		StringBuilder sb = new();
		sb.AppendLine("Door Duties Configuration".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Door Access: {DescribeAccessMode(actor)}");
		return sb.ToString();
	}

	protected override void PatrolTickActiveEnforcement(IPatrol patrol)
	{
		DisableDoorGuardMode(patrol);
		base.PatrolTickActiveEnforcement(patrol);
	}

	protected override void PatrolTickPreparationPhase(IPatrol patrol)
	{
		ICell dutyLocation = DutyLocation(patrol);
		if (dutyLocation is null)
		{
			AbortDoorDuty(patrol);
			return;
		}

		ICell[] dutyCells = { dutyLocation };
		if (patrol.PatrolMembers.All(x => x.Location == dutyLocation) &&
			HasKeysForCells(patrol.PatrolLeader, dutyCells))
		{
			patrol.PatrolPhase = PatrolPhase.Patrol;
			patrol.LastArrivedTime = DateTime.UtcNow;
			patrol.LastMajorNode = patrol.LegalAuthority.MarshallingLocation;
			patrol.NextMajorNode = dutyLocation;
			EnableDoorGuardMode(patrol);
			return;
		}

		foreach (ICharacter member in patrol.PatrolMembers)
		{
			if (member.Location != patrol.LegalAuthority.PreparingLocation)
			{
				if (!member.CombinedEffectsOfType<FollowingPath>().Any())
				{
					List<ICellExit> path = member.PathBetween(patrol.LegalAuthority.PreparingLocation, 25,
						PathSearch.PathIncludeUnlockableDoors(member)).ToList();
					if (path.Any())
					{
						BeginPatrolPath(member, path);
					}
				}

				continue;
			}

			if (member == patrol.PatrolLeader)
			{
				PrepareKeysForCells(member, dutyCells);
			}
		}

		if (patrol.PatrolMembers.Any(x => x.Location != patrol.LegalAuthority.PreparingLocation))
		{
			if (DateTime.UtcNow - patrol.LastArrivedTime > TimeSpan.FromMinutes(3))
			{
				AbortDoorDuty(patrol);
			}

			return;
		}

		if (!HasKeysForCells(patrol.PatrolLeader, dutyCells))
		{
			if (DateTime.UtcNow - patrol.LastArrivedTime > TimeSpan.FromMinutes(3))
			{
				AbortDoorDuty(patrol);
			}

			return;
		}

		if (DateTime.UtcNow - patrol.LastArrivedTime <= TimeSpan.FromMinutes(1))
		{
			return;
		}

		FormParty(patrol);
		patrol.PatrolPhase = PatrolPhase.Deployment;
		patrol.LastArrivedTime = DateTime.UtcNow;
		patrol.LastMajorNode = patrol.PatrolLeader.Location;
		patrol.NextMajorNode = dutyLocation;
	}

	protected override void PatrolTickPatrolPhase(IPatrol patrol)
	{
		if (patrol.ActiveEnforcementTarget != null)
		{
			return;
		}

		ICell dutyLocation = DutyLocation(patrol);
		if (dutyLocation is null)
		{
			AbortDoorDuty(patrol);
			return;
		}

		if (patrol.PatrolLeader.Location == dutyLocation)
		{
			EnableDoorGuardMode(patrol);
		}

		if (DateTime.UtcNow - patrol.LastArrivedTime >= patrol.PatrolRoute.LingerTimeMajorNode)
		{
			CompleteDoorDuty(patrol);
			return;
		}

		if (!patrol.PatrolRoute.TimeOfDays.Contains(patrol.PatrolLeader.Location.CurrentTimeOfDay))
		{
			CompleteDoorDuty(patrol);
			return;
		}

		if (patrol.PatrolLeader.Location != dutyLocation)
		{
			FollowingPath effect = patrol.PatrolLeader.CombinedEffectsOfType<FollowingPath>().FirstOrDefault();
			if (effect != null)
			{
				return;
			}

			List<ICellExit> path = patrol.PatrolLeader
											 .PathBetween(dutyLocation, 20,
												 PathSearch.PathIncludeUnlockableDoors(patrol.PatrolLeader))
											 .ToList();
			if (path.Count == 0)
			{
				path = patrol.PatrolLeader.PathBetween(dutyLocation, 50,
					PathSearch.IgnorePresenceOfDoors).ToList();
				if (path.Count == 0)
				{
					if (DateTime.UtcNow - patrol.LastArrivedTime > TimeSpan.FromMinutes(3))
					{
						AbortDoorDuty(patrol);
						return;
					}

					return;
				}
			}

			BeginPatrolPath(patrol.PatrolLeader, path);
		}
	}

	public override IEnumerable<ICharacter> SelectEnforcers(IPatrolRoute patrol, IEnumerable<ICharacter> pool,
		int numberToPick)
	{
		ICell node = patrol.PatrolNodes.First();
		List<ICharacter> selected = new();
		selected.AddRange(pool.Where(x => x.Location == node).PickUpToRandom(numberToPick));
		if (selected.Count >= numberToPick)
		{
			return selected;
		}

		return selected.Concat(pool.Except(selected).PickUpToRandom(numberToPick - selected.Count));
	}
}
