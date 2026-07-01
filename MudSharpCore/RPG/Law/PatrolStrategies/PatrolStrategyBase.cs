using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Movement;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Law.PatrolStrategies;

public abstract class PatrolStrategyBase : IPatrolStrategy
{
    public abstract string Name { get; }

    protected PatrolStrategyBase(IFuturemud gameworld)
    {
        Gameworld = gameworld;
    }

    protected IFuturemud Gameworld { get; }

    protected virtual EnforcementStrategy MinimumEnforcementStrategyToAct => EnforcementStrategy.ArrestAndDetain;

    public virtual void HandlePatrolTick(IPatrol patrol)
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

        switch (patrol.PatrolPhase)
        {
            case PatrolPhase.Preperation:
                PatrolTickPreparationPhase(patrol);
                return;
            case PatrolPhase.Deployment:
                PatrolTickDeploymentPhase(patrol);
                return;
            case PatrolPhase.Return:
                PatrolTickReturnPhase(patrol);
                return;
            case PatrolPhase.Patrol:
                PatrolTickPatrolPhase(patrol);
                return;
        }
    }

    protected virtual bool PatrolTickGeneral(IPatrol patrol)
    {
        ILegalAuthority authority = patrol.LegalAuthority;
        patrol.PatrolLeader ??= patrol.PatrolMembers.GetRandomElement();
        if (patrol.PatrolLeader is null)
        {
            patrol.AbortPatrol();
            return true;
        }

        // Check for presence of criminals
        foreach (ICharacter person in patrol.PatrolLeader.Location.LayerCharacters(patrol.PatrolLeader.RoomLayer))
        {
            if (person == patrol.PatrolLeader ||
                patrol.PatrolMembers.ContainsPhysicalInstance(person) ||
                person.State.IsDead() ||
                person.State.IsInStatis() ||
                person.IdentityIsObscured)
            // TODO - how to see through this
            {
                continue;
            }

            if (person.AffectedBy<InCustodyOfEnforcer>(patrol.LegalAuthority) ||
                person.AffectedBy<WarnedByEnforcer>(authority) ||
                person.AffectedBy<Dragging.DragTarget>(authority) ||
                person.AffectedBy<OnTrial>(authority))
            {
                continue;
            }

            List<ICrime> crimes = authority.KnownCrimesForIndividual(person)
                                           .Where(x => x.CriminalIdentityIsKnown)
                                           .Where(x => !x.HasBeenFinalised)
                                           .ToList();
            if (!crimes.Any(x => !x.BailPosted && x.Law.EnforcementStrategy >= MinimumEnforcementStrategyToAct))
            {
                continue;
            }

            ICrime targetCrime = crimes
                              .Where(x => !x.BailPosted)
                              .WhereMax(x => x.Law.EnforcementStrategy)
                              .OrderByDescending(x => x.Law.EnforcementPriority)
                              .First();
            patrol.ActiveEnforcementTarget = person;
            patrol.ActiveEnforcementCrime = targetCrime;
            patrol.WarnCriminal(person, targetCrime);
            patrol.PatrolLeader.RemoveAllEffects<FollowingPath>(fireRemovalAction: true);
            patrol.LegalAuthority.HandleDiscordNotificationOfEnforcement(targetCrime, patrol);
            return true;
        }

        return false;
    }

    protected abstract void PatrolTickPatrolPhase(IPatrol patrol);

    protected virtual IInventoryPlanTemplate NonLethalWeaponTemplate { get; } = new InventoryPlanTemplate(
        Futuremud.Games.First(), new List<InventoryPlanPhaseTemplate>
        {
            new(1, new List<IInventoryPlanAction>
            {
                new InventoryPlanActionWield(Futuremud.Games.First(), 0, 0,
                    item => item.GetItemType<IMeleeWeapon>()?.WeaponType.Classification ==
                            WeaponClassification.NonLethal, null, null)
            })
        });

    protected virtual IInventoryPlanTemplate LethalWeaponTemplate { get; } = new InventoryPlanTemplate(
        Futuremud.Games.First(), new List<InventoryPlanPhaseTemplate>
        {
            new(1, new List<IInventoryPlanAction>
            {
                new InventoryPlanActionWield(Futuremud.Games.First(), 0, 0,
                    item => item.GetItemType<IMeleeWeapon>()?.WeaponType.Classification !=
                            WeaponClassification.NonLethal, null, null)
                {
                    PrimaryItemFitnessScorer =
                        item => item.GetItemType<IMeleeWeapon>().WeaponType.Classification switch
                        {
                            WeaponClassification.Ceremonial => 100,
                            WeaponClassification.Military => 50,
                            WeaponClassification.Lethal => 30,
                            WeaponClassification.NonLethal => -100,
                            WeaponClassification.Training => -1000,
                            WeaponClassification.Shield => -150,
                            _ => 0
                        }
                }
            })
        });

    protected virtual void EngageInCombat(ICharacter member, ICharacter criminal, ICrime crime, IPatrol patrol)
    {
        if (member.Combat != null && member.CombatTarget == criminal)
        {
            return;
        }

        if (member.CanEngage(criminal))
        {
            member.Engage(criminal);
        }

        if (member.CombatTarget == criminal)
        {
            switch (crime.Law.EnforcementStrategy)
            {
                case EnforcementStrategy.ArrestAndDetainedUnarmedOnly:
                    if (member.CombatSettings.PreferredMeleeMode.In(CombatStrategyMode.GrappleForControl,
                            CombatStrategyMode.GrappleForIncapacitation))
                    {
                        return;
                    }

                    ICharacterCombatSettings grapple = member.Gameworld.CharacterCombatSettings
                                        .FirstOrDefault(x =>
                                            x.PreferredMeleeMode.In(CombatStrategyMode.GrappleForControl,
                                                CombatStrategyMode.GrappleForIncapacitation) &&
                                            x.CanUse(member)
                                        );
                    if (grapple != null)
                    {
                        member.CombatSettings = grapple;
                    }

                    break;
                case EnforcementStrategy.ArrestAndDetainIfArmed:
                case EnforcementStrategy.ArrestAndDetain:
                case EnforcementStrategy.ArrestAndDetainNoWarning:
                    NonLethalWeaponTemplate.CreatePlan(member).ExecuteWholePlan();
                    break;
                case EnforcementStrategy.LethalForceArrestAndDetain:
                case EnforcementStrategy.LethalForceArrestAndDetainNoWarning:
                case EnforcementStrategy.KillOnSight:
                    LethalWeaponTemplate.CreatePlan(member).ExecuteWholePlan();
                    break;
            }
        }
    }

    private static void LeaveCombatIfAble(ICharacter member)
    {
        if (member.Combat?.CanFreelyLeaveCombat(member) == true)
        {
            member.Combat.LeaveCombat(member);
        }
    }

    private static bool IsBeingDraggedByPatrol(IPatrol patrol, ICharacter criminal)
    {
        return patrol.PatrolMembers.Any(x => x.CombinedEffectsOfType<Dragging>().Any(y => y.Target == criminal));
    }

    private bool TryStartDraggingHelplessCriminal(IPatrol patrol, ICharacter criminal)
    {
        if (!criminal.IsHelpless)
        {
            return false;
        }

        foreach (ICharacter member in patrol.PatrolMembers.Where(x => x.ColocatedWith(criminal)))
        {
            LeaveCombatIfAble(member);
        }

        if (criminal.Combat?.Combatants.OfType<ICharacter>().Any(x => patrol.PatrolMembers.ContainsPhysicalInstance(x)) == true)
        {
            return true;
        }

        if (criminal.Combat is not null && criminal.MeleeRange)
        {
            return true;
        }

        ICharacter random = patrol.PatrolMembers
                                   .Where(x => x.ColocatedWith(criminal))
                                   .Where(x => x.State.IsAble())
                                   .FirstOrDefault(x => x.SamePhysicalInstance(patrol.PatrolLeader)) ??
                            patrol.PatrolMembers
                                  .Where(x => x.ColocatedWith(criminal))
                                  .Where(x => x.State.IsAble())
                                  .GetRandomElement();
        if (random is null)
        {
            return true;
        }

        random.ExecuteCommand($"drag {random.BestKeywordFor(criminal)}");
        if (random.CombinedEffectsOfType<Dragging>().Any(x => x.Target == criminal))
        {
            foreach (ICharacter other in patrol.PatrolMembers
                                                .Where(x => !x.SamePhysicalInstance(random))
                                                .Where(x => x.ColocatedWith(random)))
            {
                LeaveCombatIfAble(other);

                other.ExecuteCommand($"drag help {other.BestKeywordFor(random)}");
            }
        }

        return true;
    }

    protected virtual void PatrolTickActiveEnforcement(IPatrol patrol)
    {
        ICharacter criminal = patrol.ActiveEnforcementTarget;
        ICrime crime = patrol.ActiveEnforcementCrime;
        ICharacter leader = patrol.PatrolLeader;

        if (criminal is null ||
            crime is null ||
            criminal.State.IsDead() ||
            criminal.State.IsInStatis() ||
            crime.HasBeenFinalised ||
            crime.BailPosted)
        {
            patrol.InvalidateActiveCrime();
            return;
        }

        if (criminal.AffectedBy<InCustodyOfEnforcer>(patrol.LegalAuthority))
        {
            patrol.InvalidateActiveCrime();
            return;
        }

        if (IsBeingDraggedByPatrol(patrol, criminal))
        {
            criminal.RemoveAllEffects(x => x.IsEffectType<WarnedByEnforcer>(), true);
        }

        FollowingPath effect = patrol.PatrolLeader.CombinedEffectsOfType<FollowingPath>().FirstOrDefault();
        if (effect != null)
        {
            return;
        }

        // Is criminal detained by an enforcer?
        if (IsBeingDraggedByPatrol(patrol, criminal))
        {
            // Get rest of team to join drag
            foreach (ICharacter member in patrol.PatrolMembers)
            {
                LeaveCombatIfAble(member);
            }

            if (!leader.CouldMove(false, null).Success)
            {
                return;
            }

            if (leader.Location == patrol.LegalAuthority.PrisonLocation)
            {
                criminal.RemoveAllEffects<Dragging.DragTarget>(fireRemovalAction: true);
                // Release prisoner
                EnforcerAI ai = ((INPC)patrol.PatrolLeader).AIs.OfType<EnforcerAI>().First();
                foreach (string action in (ai.ThrowInPrisonEchoProg?.Execute<string>(leader, criminal, crime) ??
                                        string.Empty).Split('\n'))
                {
                    leader.ExecuteCommand(action);
                }

                patrol.LegalAuthority.IncarcerateCriminal(criminal);
                patrol.LegalAuthority.OnPrisonerImprisoned?.Execute(criminal);
                patrol.ActiveEnforcementCrime = null;
                patrol.ActiveEnforcementTarget = null;
                return;
            }

            // Move to prison
            List<ICellExit> path = patrol.PatrolLeader.PathBetween(patrol.LegalAuthority.PrisonLocation, 50,
                PathSearch.PathIncludeUnlockableDoors(patrol.PatrolLeader)).ToList();
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

            FollowingPath fp = new(patrol.PatrolLeader, path)
            { UseDoorguards = true, UseKeys = true, OpenDoors = true };
            patrol.PatrolLeader.AddEffect(fp);
            fp.FollowPathAction();
            return;
        }

        // Is criminal incapacitated?
        if (TryStartDraggingHelplessCriminal(patrol, criminal))
        {
            return;
        }

        // Is criminal in combat with enforcers?
        if (criminal.Combat?.Combatants.OfType<ICharacter>().Any(x => patrol.PatrolMembers.ContainsPhysicalInstance(x)) == true)
        {
            // Get the other enforcers to join in up to a limit
            foreach (ICharacter member in patrol.PatrolMembers)
            {
                EngageInCombat(member, criminal, crime, patrol);
            }

            return;
        }

        // Are any of the enforcers engaged with other targets?
        List<ICharacter> engagedPatrolMembers = patrol.PatrolMembers.Where(x =>
                                             x.Combat != null && x.Combat.Combatants.Any(y =>
                                                 y.CombatTarget is ICharacter target && patrol.PatrolMembers.ContainsPhysicalInstance(target)))
                                         .ToList();
        if (engagedPatrolMembers.Any())
        {
            foreach (ICharacter enforcer in patrol.PatrolMembers.Where(x => !engagedPatrolMembers.ContainsPhysicalInstance(x)).ToArray())
            {
                enforcer.ExecuteCommand($"support {enforcer.BestKeywordFor(engagedPatrolMembers.GetRandomElement())}");
            }

            return;
        }

        // Has criminal been warned to surrender?
        if (criminal.AffectedBy<WarnedByEnforcer>(patrol.LegalAuthority))
        // If lethal force detention or greater move to block exits
        {
            return;
        }

        // Else try to apprehend or kill them
        foreach (ICharacter member in patrol.PatrolMembers)
        {
            EngageInCombat(member, criminal, crime, patrol);
        }
    }

    protected virtual void PatrolTickReturnPhase(IPatrol patrol)
    {
        if (patrol.PatrolLeader.Location == patrol.OriginLocation)
        {
            // Patrol is complete
            patrol.ConcludePatrol();
            return;
        }

        FollowingPath fp;
        if (!patrol.PatrolLeader.AffectedBy<FollowingPath>())
        {
            IEnumerable<ICellExit> path = patrol.PatrolLeader.PathBetween(patrol.OriginLocation, 50,
                PathSearch.PathIncludeUnlockableDoors(patrol.PatrolLeader));
            if (!path.Any())
            {
                return;
            }

            fp = new FollowingPath(patrol.PatrolLeader, path)
            { UseDoorguards = true, UseKeys = true, OpenDoors = true };
            patrol.PatrolLeader.AddEffect(fp);
            fp.FollowPathAction();
        }
    }

    protected virtual void PatrolTickDeploymentPhase(IPatrol patrol)
    {
        if (patrol.PatrolLeader.Location == patrol.LegalAuthority.MarshallingLocation)
        {
            patrol.PatrolPhase = PatrolPhase.Patrol;
            patrol.LastArrivedTime = DateTime.UtcNow;
            patrol.LastMajorNode = patrol.LegalAuthority.MarshallingLocation;
            return;
        }

        ICharacter leader = patrol.PatrolLeader;
        if (leader.Location != patrol.LegalAuthority.MarshallingLocation &&
            !leader.CombinedEffectsOfType<FollowingPath>().Any())
        {
            List<ICellExit> path = leader.PathBetween(patrol.LegalAuthority.MarshallingLocation, 50,
                PathSearch.PathIncludeUnlockableDoors(leader)).ToList();
            if (path.Count > 0)
            {
                FollowingPath fp = new(leader, path) { UseDoorguards = true, UseKeys = true, OpenDoors = true };
                leader.AddEffect(fp);
                fp.FollowPathAction();
            }

            return;
        }

        if (patrol.PatrolMembers.All(x => x.Location == patrol.LegalAuthority.MarshallingLocation) &&
            !patrol.PatrolLeader.AffectedBy<FollowingPath>())
        {
            List<ICellExit> path = patrol.PatrolLeader.PathBetween(patrol.NextMajorNode, 50,
                PathSearch.PathIncludeUnlockableDoors(patrol.PatrolLeader)).ToList();

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

            FollowingPath fp = new(patrol.PatrolLeader, path)
            { UseDoorguards = true, UseKeys = true, OpenDoors = true };
            patrol.PatrolLeader.AddEffect(fp);
            fp.FollowPathAction();
        }
    }

    protected virtual void DoPreparationRoomAction(ICharacter member)
    {
        // Do nothing
    }

    public virtual void HandlePatrolCompleted(IPatrol patrol)
    {
        // Do nothing
    }

    public virtual void HandlePatrolAborted(IPatrol patrol)
    {
        // Do nothing
    }

    protected static void PrepareInventoryPlan(ICharacter member, IInventoryPlanTemplate template)
    {
        IInventoryPlan plan = template.CreatePlan(member);
        if (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
        {
            plan.ExecuteWholePlan();
        }

        plan.FinalisePlanNoRestore();
    }

    protected static IEnumerable<IKey> AccessibleKeys(ICharacter member)
    {
        return member.Body.ExternalItems
                     .SelectMany(x => x.ShallowAccessibleItems(member))
                     .SelectNotNull(x => x.GetItemType<IKey>());
    }

    protected static bool KeyWorksForLock(ICharacter member, ILock theLock, IKey key)
    {
        return theLock.CanUnlock(member, key) || theLock.CanLock(member, key);
    }

    protected static bool HasKeyForLock(ICharacter member, ILock theLock)
    {
        return AccessibleKeys(member).Any(x => KeyWorksForLock(member, theLock, x));
    }

    protected static IEnumerable<ILock> DoorLocksForExits(IEnumerable<ICellExit> exits)
    {
        return exits
               .SelectNotNull(x => x.Exit.Door)
               .SelectMany(x => x.Locks)
               .Distinct();
    }

    protected static IEnumerable<ICellExit> DoorExitsForCells(ICharacter member, IEnumerable<ICell> cells)
    {
        return cells
               .SelectMany(x => x.ExitsFor(member, true))
               .Where(x => x.Exit.Door?.Locks.Any() == true)
               .DistinctBy(x => x.Exit);
    }

    protected static bool HasKeysForLocks(ICharacter member, IEnumerable<ILock> locks)
    {
        return locks.Distinct().All(x => HasKeyForLock(member, x));
    }

    protected static bool HasKeysForExits(ICharacter member, IEnumerable<ICellExit> exits)
    {
        return HasKeysForLocks(member, DoorLocksForExits(exits));
    }

    protected static bool HasKeysForCells(ICharacter member, IEnumerable<ICell> cells)
    {
        return HasKeysForExits(member, DoorExitsForCells(member, cells));
    }

    protected static bool PrepareKeysForLocks(ICharacter member, IEnumerable<ILock> locks)
    {
        List<ILock> requiredLocks = locks.Distinct().ToList();
        foreach (ILock theLock in requiredLocks)
        {
            if (HasKeyForLock(member, theLock))
            {
                continue;
            }

            IInventoryPlanTemplate template = new InventoryPlanTemplate(member.Gameworld,
                new InventoryPlanActionHold(member.Gameworld, 0, 0,
                    item => item.GetItemType<IKey>() is IKey key && KeyWorksForLock(member, theLock, key),
                    null,
                    1)
                {
                    ItemsAlreadyInPlaceMultiplier = 10.0
                });
            PrepareInventoryPlan(member, template);
        }

        return HasKeysForLocks(member, requiredLocks);
    }

    protected static bool PrepareKeysForExits(ICharacter member, IEnumerable<ICellExit> exits)
    {
        return PrepareKeysForLocks(member, DoorLocksForExits(exits));
    }

    protected static bool PrepareKeysForCells(ICharacter member, IEnumerable<ICell> cells)
    {
        return PrepareKeysForExits(member, DoorExitsForCells(member, cells));
    }

    protected static void FormParty(IPatrol patrol)
    {
        if (patrol.PatrolLeader.Party != null)
        {
            patrol.PatrolLeader.LeaveParty();
        }

        Party party = new(patrol.PatrolLeader);
        patrol.PatrolLeader.JoinParty(party);
        foreach (ICharacter member in patrol.PatrolMembers.Where(x => x.ColocatedWith(patrol.PatrolLeader)).ToList())
        {
            if (member == patrol.PatrolLeader)
            {
                continue;
            }

            if (member.Party != null)
            {
                member.LeaveParty();
            }

            member.JoinParty(party);
        }
    }

    protected virtual void PatrolTickPreparationPhase(IPatrol patrol)
    {
        foreach (ICharacter member in patrol.PatrolMembers)
        {
            if (member.Location != patrol.LegalAuthority.PreparingLocation &&
                !member.CombinedEffectsOfType<FollowingPath>().Any())
            {
                IEnumerable<ICellExit> path = member.PathBetween(patrol.LegalAuthority.PreparingLocation, 25,
                    PathSearch.PathIncludeUnlockableDoors(member));
                if (path != null)
                {
                    FollowingPath fp = new(member, path) { UseDoorguards = true, UseKeys = true, OpenDoors = true };
                    member.AddEffect(fp);
                    fp.FollowPathAction();
                }

                continue;
            }

            DoPreparationRoomAction(member);
        }

        if (DateTime.UtcNow - patrol.LastArrivedTime > TimeSpan.FromMinutes(1))
        {
            FormParty(patrol);
            patrol.PatrolPhase = PatrolPhase.Deployment;
            patrol.LastArrivedTime = DateTime.UtcNow;
            patrol.LastMajorNode = patrol.PatrolLeader.Location;
            patrol.NextMajorNode = patrol.PatrolRoute.PatrolNodes.First();
        }
    }

    public virtual IEnumerable<ICharacter> SelectEnforcers(IPatrolRoute patrol, IEnumerable<ICharacter> pool,
        int numberToPick)
    {
        return pool.PickUpToRandom(numberToPick);
    }
}
