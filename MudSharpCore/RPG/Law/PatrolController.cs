using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp.RPG.Law;

public class PatrolController : IPatrolController
{
	public PatrolController(ILegalAuthority authority)
	{
		LegalAuthority = authority;
		LegalAuthority.Gameworld.HeartbeatManager.FuzzyThirtySecondHeartbeat += PatrolOverwatchTick;
	}

	public ILegalAuthority LegalAuthority { get; init; }

	public void ReportPatrolAborted(IPatrol patrol)
	{
		LegalAuthority.RemovePatrol(patrol);
		// TODO - what else should we do here?
	}

	public void ReportPatrolComplete(IPatrol patrol)
	{
		LegalAuthority.RemovePatrol(patrol);
		// TODO - what else should we do here?
	}

	public void PatrolOverwatchTick()
	{
		if (
			LegalAuthority.PrisonLocation is null ||
			LegalAuthority.PrisonerBelongingsStorageLocation is null ||
			LegalAuthority.MarshallingLocation is null ||
			LegalAuthority.EnforcerStowingLocation is null ||
			LegalAuthority.PreparingLocation is null ||
			!LegalAuthority.CellLocations.Any()
		)
		{
			return;
		}

		var freeEnforcers =
			LegalAuthority.Gameworld.NPCs
			              .Where(x =>
				              x.AffectedBy<EnforcerEffect>(LegalAuthority) &&
				              LegalAuthority.Patrols.All(y => !y.PatrolMembers.Contains(x))
			              )
			              .ToList();
		var enforcerCounts = new CollectionDictionary<IEnforcementAuthority, ICharacter>();
		foreach (var group in freeEnforcers.GroupBy(x => LegalAuthority.GetEnforcementAuthority(x)))
		{
			enforcerCounts.AddRange(group.Key, group);
		}

		var crimesRequiringInvestigation = LegalAuthority.UnknownCrimes
		                                                 .Where(x => x.Law.EnforcementStrategy >
		                                                             EnforcementStrategy.NoActiveEnforcement)
		                                                 .OrderByDescending(x => x.Law.EnforcementPriority)
		                                                 .ToList();

		var patrolsToLaunch = new Queue<IPatrolRoute>(LegalAuthority.PatrolRoutes
		                                                            .Where(x =>
			                                                            LegalAuthority.Patrols.All(y =>
				                                                            y.PatrolRoute != x) &&
			                                                            x.ShouldBeginPatrol()
		                                                            )
		                                                            .OrderByDescending(x => x.Priority)
		);

		while (patrolsToLaunch.Count > 0)
		{
			var whichPatrol = patrolsToLaunch.Dequeue();
			if (whichPatrol.PatrollerNumbers.Any(x => enforcerCounts[x.Key].Count() < x.Value))
			{
				continue;
			}

			var patrolMembers = new List<ICharacter>();
			foreach (var requirement in whichPatrol.PatrollerNumbers)
			{
				var members = whichPatrol.PatrolStrategy
				                         .SelectEnforcers(whichPatrol, enforcerCounts[requirement.Key],
					                         requirement.Value).ToList();
				patrolMembers.AddRange(members);
				enforcerCounts.RemoveRange(requirement.Key, members);
				freeEnforcers.RemoveAll(members.Contains);
			}

			var leader = patrolMembers.GetRandomElement();
			var patrol = new Patrol(LegalAuthority, whichPatrol, leader, patrolMembers);
			LegalAuthority.AddPatrol(patrol);

			if (freeEnforcers.Count == 0)
			{
				break;
			}
		}
	}
}