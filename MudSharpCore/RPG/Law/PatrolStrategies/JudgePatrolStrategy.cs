using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.NPC;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.RPG.Law.PatrolStrategies;

public class JudgePatrolStrategy : PatrolStrategyBase
{
	public override string Name => "Judge";

	public JudgePatrolStrategy(IFuturemud gameworld) : base(gameworld)
	{
	}

	protected override void PatrolTickPreparationPhase(IPatrol patrol)
	{
		if (patrol.PatrolMembers.All(x => x.Location == patrol.LegalAuthority.CourtLocation))
		{
			patrol.PatrolPhase = PatrolPhase.Patrol;
			patrol.LastArrivedTime = DateTime.UtcNow;
			patrol.LastMajorNode = patrol.LegalAuthority.CourtLocation;
			patrol.NextMajorNode = patrol.LegalAuthority.CourtLocation;
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

		// Patrol can only be completed if there is no trial on-going
		if (patrol.PatrolLeader.Location.LayerCharacters(patrol.PatrolLeader.RoomLayer).All(x => !x.EffectsOfType<OnTrial>(y => y.LegalAuthority == patrol.LegalAuthority).Any()))
		{
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

	/// <inheritdoc />
	public override IEnumerable<ICharacter> SelectEnforcers(IPatrolRoute patrol, IEnumerable<ICharacter> pool, int numberToPick)
	{
		return base.SelectEnforcers(patrol, pool.OfType<INPC>().Where(x => x.AIs.Any(y => y.AIType == "Judge")), numberToPick);
	}
}