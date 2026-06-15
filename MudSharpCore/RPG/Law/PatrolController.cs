using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.RPG.Law.PatrolStrategies;
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
                              LegalAuthority.GetEnforcementAuthority(x) is not null &&
                              LegalAuthority.Patrols.All(y => !y.PatrolMembers.ContainsPhysicalInstance(x))
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

        if (TryLaunchCrimeTargetedPatrol<ReactivePatrolStrategy>(freeEnforcers, enforcerCounts))
        {
            return;
        }

        if (TryLaunchCrimeTargetedPatrol<InvestigationPatrolStrategy>(freeEnforcers, enforcerCounts))
        {
            return;
        }

        Queue<IPatrolRoute> patrolsToLaunch = new(LegalAuthority.PatrolRoutes
                                                                    .Where(x =>
                                                                        x.PatrolStrategy is not ICrimeTargetedPatrolStrategy &&
                                                                        LegalAuthority.Patrols.All(y =>
                                                                            y.PatrolRoute != x) &&
                                                                        x.ShouldBeginPatrol()
                                                                    )
                                                                    .OrderByDescending(x => x.Priority)
        );

        while (patrolsToLaunch.Count > 0)
        {
            IPatrolRoute whichPatrol = patrolsToLaunch.Dequeue();
            if (LegalAuthority.Patrols.Any(x => x.PatrolRoute == whichPatrol) ||
                !whichPatrol.ShouldBeginPatrol())
            {
                continue;
            }

            if (!TryLaunchPatrol(whichPatrol, freeEnforcers, enforcerCounts, null, null))
            {
                continue;
            }

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
            .Where(x => LegalAuthority.Patrols.All(y => y.PatrolRoute != x))
            .OrderByDescending(x => x.PatrolStrategy is CorpseRecoveryPatrolStrategy)
            .ThenByDescending(x => x.Priority)
            .FirstOrDefault();
        if (route == null)
        {
            return false;
        }

        return TryLaunchPatrol(route, freeEnforcers, enforcerCounts, pendingReport, null);
    }

    private bool TryLaunchCrimeTargetedPatrol<TStrategy>(List<ICharacter> freeEnforcers,
        CollectionDictionary<IEnforcementAuthority, ICharacter> enforcerCounts)
        where TStrategy : class, ICrimeTargetedPatrolStrategy
    {
        foreach (IPatrolRoute route in LegalAuthority.PatrolRoutes
                     .Where(x => x.PatrolStrategy is TStrategy)
                     .Where(x => LegalAuthority.Patrols.All(y => y.PatrolRoute != x))
                     .OrderByDescending(x => x.Priority))
        {
            TStrategy strategy = (TStrategy)route.PatrolStrategy;
            ICrime crime = strategy.SelectDispatchCrimes(LegalAuthority)
                                   .Where(x => strategy.ShouldDispatchForCrime(route, x))
                                   .Where(x => LegalAuthority.Patrols.All(y =>
                                       y.TargetCrime != x ||
                                       y.PatrolStrategy.GetType() != route.PatrolStrategy.GetType()))
                                   .FirstOrDefault();
            if (crime is null)
            {
                continue;
            }

            if (TryLaunchPatrol(route, freeEnforcers, enforcerCounts, null, crime))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryLaunchPatrol(IPatrolRoute route, List<ICharacter> freeEnforcers,
        CollectionDictionary<IEnforcementAuthority, ICharacter> enforcerCounts,
        ICorpseRecoveryReport pendingReport, ICrime targetCrime)
    {
        if (route.PatrollerNumbers.Any(x => enforcerCounts[x.Key].Count() < x.Value))
        {
            return false;
        }

        List<(IEnforcementAuthority Authority, List<ICharacter> Members)> selections = new();
        foreach (KeyValuePair<IEnforcementAuthority, int> requirement in route.PatrollerNumbers)
        {
            List<ICharacter> members = route.PatrolStrategy
                .SelectEnforcers(route, enforcerCounts[requirement.Key], requirement.Value)
                .ToList();
            if (members.Count < requirement.Value)
            {
                return false;
            }

            selections.Add((requirement.Key, members));
        }

        List<ICharacter> patrolMembers = selections.SelectMany(x => x.Members).ToList();
        if (!patrolMembers.Any())
        {
            return false;
        }

        foreach ((IEnforcementAuthority authority, List<ICharacter> members) in selections)
        {
            enforcerCounts.RemoveRange(authority, members);
            freeEnforcers.RemoveAll(members.Contains);
        }

        ICharacter leader = patrolMembers.GetRandomElement();
        Patrol patrol = new(LegalAuthority, route, leader, patrolMembers, pendingReport, targetCrime);
        LegalAuthority.AddPatrol(patrol);
        return true;
    }
}
