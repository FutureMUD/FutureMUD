using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.Movement;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.RPG.Law.PatrolStrategies;

namespace MudSharp.RPG.Law;

public class Patrol : SaveableItem, IPatrol
{
    public Patrol(ILegalAuthority authority, IPatrolRoute route, ICharacter leader, IEnumerable<ICharacter> members,
        ICorpseRecoveryReport corpseRecoveryReport = null, ICrime targetCrime = null)
    {
        Gameworld = authority.Gameworld;
        LegalAuthority = authority;
        PatrolRoute = route;
        PatrolStrategy = corpseRecoveryReport != null
            ? new CorpseRecoveryPatrolStrategy(Gameworld)
            : route.PatrolStrategy;
        PatrolPhase = PatrolPhase.Preperation;
        PatrolStartTime = DateTime.UtcNow;
        LastArrivedTime = DateTime.UtcNow;
        PatrolLeader = leader;
        ActiveCorpseRecoveryReport = corpseRecoveryReport;
        TargetCrime = targetCrime;
        _members.AddRange(members);
        using (new FMDB())
        {
            Models.Patrol dbitem = new()
            {
                LegalAuthorityId = authority.Id,
                PatrolRouteId = route.Id,
                PatrolLeaderId = CharacterInstanceIdentityComparer.IdentityId(leader),
                PatrolLeaderInstanceId = CharacterInstanceIdentityComparer.InstanceId(leader),
                PatrolPhase = (int)PatrolPhase.Preperation
            };
            dbitem.PatrolMembers =
                members
                    .Select(x => new PatrolMember
                    {
                        Patrol = dbitem,
                        CharacterId = CharacterInstanceIdentityComparer.IdentityId(x),
                        CharacterInstanceId = CharacterInstanceIdentityComparer.InstanceId(x) ?? 0L
                    })
                    .ToList();

            FMDB.Context.Patrols.Add(dbitem);
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }

        Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += HandlePatrolTick;
        Gameworld.Add(this);
        corpseRecoveryReport?.AssignPatrol(this);
        foreach (ICharacter member in members)
        {
            member.RemoveAllEffects<FollowingPath>(fireRemovalAction: true);
            member.AddEffect(new PatrolMemberEffect(member, this));
        }
    }

    public Patrol(Models.Patrol patrol, ILegalAuthority authority)
    {
        Gameworld = authority.Gameworld;
        LegalAuthority = authority;
        PatrolRoute = authority.PatrolRoutes.First(x => x.Id == patrol.PatrolRouteId);
        _id = patrol.Id;
        PatrolPhase = (PatrolPhase)patrol.PatrolPhase;
        PatrolStartTime = DateTime.UtcNow;
        LastArrivedTime = DateTime.UtcNow;
        LastMajorNode = Gameworld.Cells.Get(patrol.LastMajorNodeId ?? 0);
        NextMajorNode = Gameworld.Cells.Get(patrol.NextMajorNodeId ?? 0);
        _members.AddRange(patrol.PatrolMembers
                                .SelectNotNull(x => CharacterInstanceIdentityComparer.ResolvePhysicalInstance(
                                    Gameworld,
                                    x.CharacterId,
                                    x.CharacterInstanceId,
                                    fallbackToPrimary: x.CharacterInstanceId <= 0))
                                .DistinctPhysicalInstances());
        PatrolLeader = patrol.PatrolLeaderId is null
            ? PatrolMembers.GetRandomElement()
            : CharacterInstanceIdentityComparer.ResolvePhysicalInstance(
                  Gameworld,
                  patrol.PatrolLeaderId.Value,
                  patrol.PatrolLeaderInstanceId,
                  fallbackToPrimary: patrol.PatrolLeaderInstanceId is null) ??
              PatrolMembers.GetRandomElement();
        ActiveCorpseRecoveryReport = authority.CorpseRecoveryReports
            .OfType<CorpseRecoveryReport>()
            .FirstOrDefault(x => x.AssignedPatrolId == _id && x.Status == CorpseRecoveryReportStatus.Assigned);
        PatrolStrategy = ActiveCorpseRecoveryReport != null
            ? new CorpseRecoveryPatrolStrategy(Gameworld)
            : PatrolRoute.PatrolStrategy;
        Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += HandlePatrolTick;
        Gameworld.Add(this);
    }

    #region Overrides of FrameworkItem

    /// <inheritdoc />
    public override string Name => PatrolRoute.Name;

    #endregion

    public ILegalAuthority LegalAuthority { get; set; }
    public IPatrolRoute PatrolRoute { get; }
    public IPatrolStrategy PatrolStrategy { get; }
    private readonly List<ICharacter> _members = new();
    public IEnumerable<ICharacter> PatrolMembers => _members;
    public ICharacter PatrolLeader { get; set; }
    public PatrolPhase PatrolPhase { get; set; }
    public ICell LastMajorNode { get; set; }
    public ICell NextMajorNode { get; set; }
    public DateTime PatrolStartTime { get; }
    public DateTime LastArrivedTime { get; set; }
    public ICharacter ActiveEnforcementTarget { get; set; }
    public ICrime ActiveEnforcementCrime { get; set; }
    public ICrime TargetCrime { get; set; }
    public ICorpseRecoveryReport ActiveCorpseRecoveryReport { get; set; }
    public ICell OriginLocation => LegalAuthority.MarshallingLocation;

    public void Delete()
    {
        Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= HandlePatrolTick;
        Gameworld.SaveManager.Abort(this);
        Gameworld.Destroy(this);
        using (new FMDB())
        {
            Gameworld.SaveManager.Flush();
            Models.Patrol dbitem = FMDB.Context.Patrols.Find(Id);
            if (dbitem != null)
            {
                FMDB.Context.Patrols.Remove(dbitem);
                FMDB.Context.SaveChanges();
            }
        }
    }

    public void ConcludePatrol()
    {
        (PatrolStrategy as PatrolStrategyBase)?.HandlePatrolCompleted(this);
        foreach (ICharacter member in PatrolMembers.ToList())
        {
            member.RemoveAllEffects<PatrolMemberEffect>(fireRemovalAction: true);
        }

        PatrolLeader?.Party?.Disband();
        LegalAuthority.PatrolController.ReportPatrolComplete(this);
        Delete();
    }

    private bool EnsureLeader()
    {
        if (PatrolLeader == null || PatrolLeader.State.IsDead())
        {
            PatrolLeader = PatrolMembers.GetRandomElement();
            Changed = true;
        }

        if (PatrolLeader == null)
        {
            AbortPatrol();
            return false;
        }

        return true;
    }

    private void HandlePatrolTick()
    {
        if (!EnsureLeader())
        {
            return;
        }
        PatrolStrategy.HandlePatrolTick(this);
    }

    public void AbortPatrol()
    {
        (PatrolStrategy as PatrolStrategyBase)?.HandlePatrolAborted(this);
        foreach (ICharacter member in PatrolMembers.ToList())
        {
            member.RemoveAllEffects<PatrolMemberEffect>(fireRemovalAction: true);
        }

        PatrolLeader?.Party?.Disband();
        LegalAuthority.PatrolController.ReportPatrolAborted(this);
        Delete();
    }

    public void CompletePatrol()
    {
        (PatrolStrategy as PatrolStrategyBase)?.HandlePatrolCompleted(this);
        PatrolPhase = PatrolPhase.Return;
        LastMajorNode = NextMajorNode;
        NextMajorNode = null;
    }

    public override void Save()
    {
        Models.Patrol dbitem = FMDB.Context.Patrols.Find(Id);
        dbitem.LastMajorNodeId = LastMajorNode?.Id;
        dbitem.NextMajorNodeId = NextMajorNode?.Id;
        dbitem.PatrolLeaderId = PatrolLeader is null ? null : CharacterInstanceIdentityComparer.IdentityId(PatrolLeader);
        dbitem.PatrolLeaderInstanceId = CharacterInstanceIdentityComparer.InstanceId(PatrolLeader);
        dbitem.PatrolPhase = (int)PatrolPhase;
        dbitem.PatrolMembers.Clear();
        foreach (ICharacter item in PatrolMembers)
        {
            dbitem.PatrolMembers.Add(new PatrolMember
            {
                Patrol = dbitem,
                CharacterId = CharacterInstanceIdentityComparer.IdentityId(item),
                CharacterInstanceId = CharacterInstanceIdentityComparer.InstanceId(item) ?? 0L
            });
        }

        Changed = false;
    }

    public override string FrameworkItemType => "Patrol";

    public void CriminalFailedToComply(ICharacter criminal, ICrime crime)
    {
        // Sanity check if they are complying
        if (criminal.IsHelpless)
        {
            return;
        }

        EnforcerAI ai = ((INPC)PatrolLeader).AIs.OfType<EnforcerAI>().First();
        foreach (string action in (ai.FailToComplyEchoProg?.Execute<string>(PatrolLeader, criminal, crime) ?? string.Empty)
                 .Split('\n'))
        {
            PatrolLeader.ExecuteCommand(action);
        }

        ICrime newcrime = LegalAuthority.CheckPossibleCrime(criminal, CrimeTypes.ResistArrest, null, null, "")
                                     .FirstOrDefault();
        if (newcrime != null)
        {
            ActiveEnforcementCrime = newcrime;
        }
    }

    public void CriminalStartedMoving(ICharacter criminal, ICrime crime)
    {
        EnforcerAI ai = ((INPC)PatrolLeader).AIs.OfType<EnforcerAI>().First();
        foreach (string action in
                 (ai.WarnStartMoveEchoProg?.Execute<string>(PatrolLeader, criminal, crime) ?? string.Empty).Split('\n'))
        {
            PatrolLeader.ExecuteCommand(action);
        }
    }

    public void WarnCriminal(ICharacter criminal, ICrime crime)
    {
        if (crime.Law.EnforcementStrategy.In(EnforcementStrategy.KillOnSight,
                EnforcementStrategy.ArrestAndDetainNoWarning, EnforcementStrategy.LethalForceArrestAndDetainNoWarning))
        // Don't warn kill on sight enemies
        {
            return;
        }

        criminal.AddEffect(new WarnedByEnforcer(criminal, LegalAuthority, crime, this), TimeSpan.FromSeconds(90));

        EnforcerAI ai = ((INPC)PatrolLeader).AIs.OfType<EnforcerAI>().First();
        foreach (string action in
                 (ai.WarnEchoProg?.Execute<string>(PatrolLeader, criminal, crime) ?? string.Empty).Split('\n'))
        {
            PatrolLeader.ExecuteCommand(action);
        }

        criminal.Send($"Hint: Type {"surrender".MXPSend("surrender")} to surrender to the enforcer.".ColourCommand());
    }

    public void InvalidateActiveCrime()
    {
        ActiveEnforcementTarget?.RemoveAllEffects<WarnedByEnforcer>(x => x.WhichCrime == ActiveEnforcementCrime, true);
        ActiveEnforcementTarget = null;
        ActiveEnforcementCrime = null;
    }

    public void RemovePatrolMember(ICharacter character)
    {
        _members.RemovePhysicalInstance(character);
        PatrolLeader?.Party?.Leave(character);
        if (PatrolLeader?.SamePhysicalInstance(character) == true)
        {
            PatrolLeader = PatrolMembers.GetRandomElement();
        }

        Changed = true;
    }
}
