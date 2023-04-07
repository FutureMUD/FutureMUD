using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp.RPG.Law.PatrolStrategies;

public class StationEnforcerPatrolStrategy : PatrolStrategyBase
{
	public override string Name => "StationEnforcer";

	public StationEnforcerPatrolStrategy(IFuturemud gameworld) : base(gameworld)
	{
	}

	#region Overrides of PatrolStrategyBase

	protected override void PatrolTickPreparationPhase(IPatrol patrol)
	{
		if (patrol.PatrolMembers.All(x => x.Location == patrol.PatrolRoute.PatrolNodes.First()))
		{
			patrol.PatrolPhase = PatrolPhase.Patrol;
			patrol.LastArrivedTime = DateTime.UtcNow;
			patrol.LastMajorNode = patrol.LegalAuthority.MarshallingLocation;
			patrol.NextMajorNode = patrol.PatrolRoute.PatrolNodes.First();
			return;
		}

		base.PatrolTickPreparationPhase(patrol);
	}

	protected override void PatrolTickPatrolPhase(IPatrol patrol)
	{
		if (patrol.ActiveEnforcementTarget != null)
		{
			return;
		}

		if (DateTime.UtcNow - patrol.LastArrivedTime >= patrol.PatrolRoute.LingerTimeMajorNode)
		{
			patrol.CompletePatrol();
			return;
		}

		if (!patrol.PatrolRoute.TimeOfDays.Contains(patrol.PatrolLeader.Location.CurrentTimeOfDay))
		{
			patrol.CompletePatrol();
			return;
		}

		if (patrol.PatrolLeader.Location != patrol.NextMajorNode)
		{
			var effect = patrol.PatrolLeader.CombinedEffectsOfType<FollowingPath>().FirstOrDefault();
			if (effect != null)
			{
				return;
			}

			var path = patrol.PatrolLeader
			                 .PathBetween(patrol.NextMajorNode, 20,
				                 PathSearch.PathIncludeUnlockedDoors(patrol.PatrolLeader))
			                 .ToList();
			if (!path.Any())
			{
				// Abort the patrol - no viable routes found
				patrol.AbortPatrol();
				return;
			}

			var fp = new FollowingPath(patrol.PatrolLeader, path);
			patrol.PatrolLeader.AddEffect(fp);
			fp.FollowPathAction();
			return;
		}
	}

	public override IEnumerable<ICharacter> SelectEnforcers(IPatrolRoute patrol, IEnumerable<ICharacter> pool,
		int numberToPick)
	{
		var node = patrol.PatrolNodes.First();
		var selected = new List<ICharacter>();
		selected.AddRange(pool.Where(x => x.Location == node).PickUpToRandom(numberToPick));
		if (selected.Count >= numberToPick)
		{
			return selected;
		}

		return selected.Concat(pool.Except(selected).PickUpToRandom(numberToPick - selected.Count));
	}

	#endregion
}