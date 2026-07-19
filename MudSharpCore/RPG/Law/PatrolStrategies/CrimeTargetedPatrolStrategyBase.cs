using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Movement;
using MudSharp.TimeAndDate;
using System.Globalization;

namespace MudSharp.RPG.Law.PatrolStrategies;

public abstract class CrimeTargetedPatrolStrategyBase : ArmedPatrolStrategy, ICrimeTargetedPatrolStrategy
{
	protected CrimeTargetedPatrolStrategyBase(IFuturemud gameworld, string strategyData, string rootName) : base(gameworld)
	{
		RootName = rootName;
		LoadStrategyData(strategyData);
	}

	protected string RootName { get; }
	protected int CoverageRadius { get; set; } = 8;

	protected virtual void LoadStrategyData(string strategyData)
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

		if (int.TryParse(root.Attribute("coverageRadius")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int radius))
		{
			CoverageRadius = Math.Max(0, radius);
		}

		LoadAdditionalStrategyData(root);
	}

	protected virtual void LoadAdditionalStrategyData(XElement root)
	{
	}

	public abstract IEnumerable<ICrime> SelectDispatchCrimes(ILegalAuthority authority);

	public virtual bool ShouldDispatchForCrime(IPatrolRoute patrol, ICrime crime)
	{
		return CrimeHasLocation(crime) &&
		       RouteCoversCrime(patrol, crime) &&
		       patrol.IsReady &&
		       patrol.PatrolNodes.Any() &&
		       patrol.PatrollerNumbers.Any() &&
		       ReadyToBegin(patrol) &&
		       (patrol is not PatrolRoute concreteRoute || concreteRoute.StartPatrolProg?.Execute<bool?>() != false) &&
		       patrol.LegalAuthority.EnforcementZones.Any() &&
		       patrol.TimeOfDays.Contains(patrol.LegalAuthority.EnforcementZones.First().CurrentTimeOfDay);
	}

	protected static bool CrimeHasLocation(ICrime crime)
	{
		return crime?.CrimeLocation is not null;
	}

	protected bool RouteCoversCrime(IPatrolRoute patrol, ICrime crime)
	{
		ICell location = crime.CrimeLocation;
		return patrol.PatrolNodes.Any(x => x == location || PathLengthBetween(x, location, CoverageRadius) is not null);
	}

	protected static int? PathLengthBetween(ICell origin, ICell destination, int maximumDistance)
	{
		if (origin == destination)
		{
			return 0;
		}

		if (maximumDistance <= 0)
		{
			return null;
		}

		List<ICellExit> path = origin.PathBetween(destination, (uint)maximumDistance, PathSearch.IgnorePresenceOfDoors).ToList();
		return path.Any() ? path.Count : null;
	}

	protected List<ICell> PatrolAreaNodes(IPatrol patrol, ICrime crime)
	{
		ICell location = crime.CrimeLocation;
		List<ICell> nodes = new() { location };
		nodes.AddRange(patrol.PatrolRoute.PatrolNodes
		                     .Where(x => x != location)
		                     .Select(x => (Cell: x, Distance: PathLengthBetween(x, location, CoverageRadius)))
		                     .Where(x => x.Distance is not null)
		                     .OrderBy(x => x.Distance!.Value)
		                     .Select(x => x.Cell));
		return nodes.Distinct().ToList();
	}

	protected ICell NextAreaNode(IPatrol patrol, ICrime crime, ICell current)
	{
		List<ICell> nodes = PatrolAreaNodes(patrol, crime);
		if (nodes.Count == 0)
		{
			return crime.CrimeLocation;
		}

		int index = nodes.IndexOf(current);
		if (index < 0 || index + 1 >= nodes.Count)
		{
			return nodes[0];
		}

		return nodes[index + 1];
	}

	public override void HandlePatrolTick(IPatrol patrol)
	{
		if (patrol.ActiveEnforcementTarget != null)
		{
			PatrolTickActiveEnforcement(patrol);
			return;
		}

		if (PatrolTickGeneral(patrol))
		{
			return;
		}

		if (!TargetCrimeStillValid(patrol))
		{
			patrol.CompletePatrol();
			return;
		}

		switch (patrol.PatrolPhase)
		{
			case PatrolPhase.Preperation:
				PatrolTickPreparationPhase(patrol);
				return;
			case PatrolPhase.Deployment:
				PatrolTickDeploymentPhase(patrol);
				return;
			case PatrolPhase.Patrol:
				PatrolTickPatrolPhase(patrol);
				return;
			case PatrolPhase.Return:
				PatrolTickReturnPhase(patrol);
				return;
		}
	}

	protected virtual bool TargetCrimeStillValid(IPatrol patrol)
	{
		return patrol.TargetCrime is not null &&
		       patrol.TargetCrime.IsKnownCrime &&
		       !patrol.TargetCrime.HasBeenFinalised &&
		       patrol.TargetCrime.CrimeLocation is not null;
	}

	protected override void PatrolTickPreparationPhase(IPatrol patrol)
	{
		base.PatrolTickPreparationPhase(patrol);
		if (patrol.PatrolPhase == PatrolPhase.Deployment && patrol.TargetCrime?.CrimeLocation is not null)
		{
			patrol.NextMajorNode = patrol.TargetCrime.CrimeLocation;
		}
	}

	protected override void PatrolTickDeploymentPhase(IPatrol patrol)
	{
		MoveLeaderToTargetNode(patrol, patrol.TargetCrime.CrimeLocation);
	}

	protected override void PatrolTickPatrolPhase(IPatrol patrol)
	{
		if (patrol.NextMajorNode is null)
		{
			patrol.NextMajorNode = patrol.TargetCrime.CrimeLocation;
		}

		MoveLeaderToTargetNode(patrol, patrol.NextMajorNode);
	}

	protected void MoveLeaderToTargetNode(IPatrol patrol, ICell destination)
	{
		if (destination is null)
		{
			patrol.CompletePatrol();
			return;
		}

		if (patrol.PatrolLeader.Location == destination)
		{
			patrol.PatrolPhase = PatrolPhase.Patrol;
			HandleArrivedAtTargetNode(patrol, destination);
			return;
		}

		if (patrol.PatrolLeader.CombinedEffectsOfType<FollowingPath>().Any())
		{
			return;
		}

		List<ICellExit> path = patrol.PatrolLeader.PathBetween(destination, 50,
			PathSearch.PathIncludeUnlockableDoors(patrol.PatrolLeader)).ToList();
		if (!path.Any())
		{
			path = patrol.PatrolLeader.PathBetween(destination, 50, PathSearch.IgnorePresenceOfDoors).ToList();
			if (!path.Any())
			{
				if (DateTime.UtcNow - patrol.LastArrivedTime > TimeSpan.FromMinutes(3))
				{
					patrol.CompletePatrol();
				}

				return;
			}
		}

		BeginPatrolPath(patrol.PatrolLeader, path);
	}

	protected virtual void HandleArrivedAtTargetNode(IPatrol patrol, ICell node)
	{
		if (patrol.LastMajorNode != node)
		{
			patrol.LastMajorNode = node;
			patrol.LastArrivedTime = DateTime.UtcNow;
		}
	}

	public virtual string SaveStrategyData()
	{
		XElement root = new(RootName,
			new XAttribute("coverageRadius", CoverageRadius.ToString(CultureInfo.InvariantCulture)));
		SaveAdditionalStrategyData(root);
		return root.ToString();
	}

	protected virtual void SaveAdditionalStrategyData(XElement root)
	{
	}

	public virtual bool BuildingCommand(ICharacter actor, IPatrolRoute patrol, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "radius":
			case "coverage":
				return BuildingCommandRadius(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	protected bool BuildingCommandRadius(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int value) || value < 0)
		{
			actor.OutputHandler.Send("How many rooms away from a patrol node should this patrol respond to?");
			return false;
		}

		CoverageRadius = value;
		actor.OutputHandler.Send(
			$"This patrol strategy will now respond to crimes within {CoverageRadius.ToString("N0", actor).ColourValue()} rooms of any patrol node.");
		return true;
	}

	public virtual string ShowConfiguration(ICharacter actor, IPatrolRoute patrol)
	{
		return $"Coverage Radius: {CoverageRadius.ToString("N0", actor).ColourValue()} rooms\n";
	}

	public virtual bool ReadyToBegin(IPatrolRoute patrol)
	{
		return CoverageRadius >= 0;
	}

	public abstract string HelpText { get; }
}
