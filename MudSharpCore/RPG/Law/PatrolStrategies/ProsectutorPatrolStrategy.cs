using System;
using System.Linq;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp.RPG.Law.PatrolStrategies;

public class ProsectutorPatrolStrategy : PatrolStrategyBase
{
	/// <inheritdoc />
	public override string Name => "Prosecutor";

	public ProsectutorPatrolStrategy(IFuturemud gameworld) : base(gameworld)
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
			// If we can't find a path, try to get closer at least
			if (path.Count == 0)
			{
				path = patrol.PatrolLeader.PathBetween(patrol.NextMajorNode, 50,
					PathSearch.IgnorePresenceOfDoors).ToList();
				if (path.Count == 0)
				{
					if (DateTime.UtcNow - patrol.LastArrivedTime > TimeSpan.FromMinutes(3))
					{
						// Abort patrol
						patrol.AbortPatrol();
						return;
					}

					return;
				}
			}

			var fp = new FollowingPath(patrol.PatrolLeader, path);
			patrol.PatrolLeader.AddEffect(fp);
			fp.FollowPathAction();
			return;
		}

		// Argue in the trials
		var trial = patrol.PatrolLeader.Location.Characters.SelectNotNull(x => x.EffectsOfType<OnTrial>().FirstOrDefault()).FirstOrDefault();
		if (trial is null)
		{
			return;
		}

		if (trial.Phase.In(TrialPhase.Case, TrialPhase.ClosingArguments) && Dice.Roll(1, 3) == 1)
		{
			trial.HandleArgueCommand(patrol.PatrolLeader, false);
		}
	}
}