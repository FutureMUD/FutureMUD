using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        Console.WriteLine($"Patrol '{patrol.Name}' in {LegalAuthority.Name} was aborted.");
        // TODO - what else should we do here?
    }

    public void ReportPatrolComplete(IPatrol patrol)
    {
        LegalAuthority.RemovePatrol(patrol);
        Console.WriteLine($"Patrol '{patrol.Name}' in {LegalAuthority.Name} was completed.");
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
            LegalAuthority.CourtLocation is null ||
            !LegalAuthority.CellLocations.Any()
        )
        {
            return;
        }

        List<ICharacter> freeEnforcers =
            LegalAuthority.Gameworld.NPCs
                          .Where(x =>
                              x.AffectedBy<EnforcerEffect>(LegalAuthority) &&
                              LegalAuthority.Patrols.All(y => !y.PatrolMembers.Contains(x))
                          )
                          .ToList();
        CollectionDictionary<IEnforcementAuthority, ICharacter> enforcerCounts = new();
        foreach (IGrouping<IEnforcementAuthority, ICharacter> group in freeEnforcers.GroupBy(x => LegalAuthority.GetEnforcementAuthority(x)))
        {
            enforcerCounts.AddRange(group.Key, group);
        }

        if (TryLaunchCorpseRecoveryPatrol(freeEnforcers, enforcerCounts))
        {
            return;
        }

        List<ICrime> crimesRequiringInvestigation = LegalAuthority.UnknownCrimes
                                                         .Where(x => x.Law.EnforcementStrategy >
                                                                     EnforcementStrategy.NoActiveEnforcement)
                                                         .OrderByDescending(x => x.Law.EnforcementPriority)
                                                         .ToList();

        Queue<IPatrolRoute> patrolsToLaunch = new(LegalAuthority.PatrolRoutes
                                                                    .Where(x =>
                                                                        LegalAuthority.Patrols.All(y =>
                                                                            y.PatrolRoute != x) &&
                                                                        x.ShouldBeginPatrol()
                                                                    )
                                                                    .OrderByDescending(x => x.Priority)
        );

        while (patrolsToLaunch.Count > 0)
        {
            IPatrolRoute whichPatrol = patrolsToLaunch.Dequeue();
            if (whichPatrol.PatrollerNumbers.Any(x => enforcerCounts[x.Key].Count() < x.Value))
            {
                continue;
            }

            List<ICharacter> patrolMembers = new();
            foreach (KeyValuePair<IEnforcementAuthority, int> requirement in whichPatrol.PatrollerNumbers)
            {
                List<ICharacter> members = whichPatrol.PatrolStrategy
                                         .SelectEnforcers(whichPatrol, enforcerCounts[requirement.Key],
                                             requirement.Value).ToList();
                if (members.Count == 0)
                {
                    continue;
                }
                patrolMembers.AddRange(members);
                enforcerCounts.RemoveRange(requirement.Key, members);
                freeEnforcers.RemoveAll(members.Contains);
            }

            if (patrolMembers.Count == 0)
            {
                continue;
            }

            ICharacter leader = patrolMembers.GetRandomElement();
            Patrol patrol = new(LegalAuthority, whichPatrol, leader, patrolMembers);
            LegalAuthority.AddPatrol(patrol);

            if (freeEnforcers.Count == 0)
            {
                break;
            }
        }
    }

    private bool TryLaunchCorpseRecoveryPatrol(List<ICharacter> freeEnforcers,
        CollectionDictionary<IEnforcementAuthority, ICharacter> enforcerCounts)
    {
        ICorpseRecoveryReport pendingReport = LegalAuthority.CorpseRecoveryReports
            .FirstOrDefault(x => x.Status == CorpseRecoveryReportStatus.Pending);
        if (pendingReport == null)
        {
            return false;
        }

        IPatrolRoute route = LegalAuthority.PatrolRoutes
            .Where(x => x.IsReady && x.PatrolNodes.Any() && x.PatrollerNumbers.Any())
            .OrderByDescending(x => x.PatrolStrategy is PatrolStrategies.CorpseRecoveryPatrolStrategy)
            .ThenByDescending(x => x.Priority)
            .FirstOrDefault();
        if (route == null || route.PatrollerNumbers.Any(x => enforcerCounts[x.Key].Count() < x.Value))
        {
            return false;
        }

        List<ICharacter> patrolMembers = new();
        foreach (KeyValuePair<IEnforcementAuthority, int> requirement in route.PatrollerNumbers)
        {
            List<ICharacter> members = route.PatrolStrategy
                .SelectEnforcers(route, enforcerCounts[requirement.Key], requirement.Value)
                .ToList();
            if (members.Count == 0)
            {
                return false;
            }

            patrolMembers.AddRange(members);
            enforcerCounts.RemoveRange(requirement.Key, members);
            freeEnforcers.RemoveAll(members.Contains);
        }

        if (!patrolMembers.Any())
        {
            return false;
        }

        ICharacter leader = patrolMembers.GetRandomElement();
        Patrol patrol = new(LegalAuthority, route, leader, patrolMembers, pendingReport);
        LegalAuthority.AddPatrol(patrol);
        return true;
    }
}
