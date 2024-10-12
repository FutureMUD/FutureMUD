using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Combat;
using MudSharp.Construction;
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
		var authority = patrol.LegalAuthority;

		// Check for presence of criminals
		foreach (var person in patrol.PatrolLeader.Location.LayerCharacters(patrol.PatrolLeader.RoomLayer))
		{
			if (person.IdentityIsObscured)
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

			var crimes = authority.KnownCrimesForIndividual(person).ToList();
			if (!crimes.Any(x => !x.BailPosted && x.Law.EnforcementStrategy >= MinimumEnforcementStrategyToAct))
			{
				continue;
			}

			var targetCrime = crimes
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

					var grapple = member.Gameworld.CharacterCombatSettings
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

	protected virtual void PatrolTickActiveEnforcement(IPatrol patrol)
	{
		var criminal = patrol.ActiveEnforcementTarget;
		var crime = patrol.ActiveEnforcementCrime;
		var leader = patrol.PatrolLeader;

		if (criminal.AffectedBy<InCustodyOfEnforcer>(patrol.LegalAuthority))
		{
			patrol.InvalidateActiveCrime();
			return;
		}

		if (patrol.PatrolMembers.Any(x => x.CombinedEffectsOfType<Dragging>().Any(x => x.Target == criminal)))
		{
			criminal.RemoveAllEffects(x => x.IsEffectType<WarnedByEnforcer>(), true);
		}

		var effect = patrol.PatrolLeader.CombinedEffectsOfType<FollowingPath>().FirstOrDefault();
		if (effect != null)
		{
			return;
		}

		// Is criminal detained by an enforcer?
		if (patrol.PatrolMembers.Any(x => x.CombinedEffectsOfType<Dragging>().Any(x => x.Target == criminal)))
		{
			// Get rest of team to join drag
			if (!leader.CouldMove(false, null).Success)
			{
				return;
			}

			if (leader.Location == patrol.LegalAuthority.PrisonLocation)
			{
				criminal.RemoveAllEffects<Dragging.DragTarget>(fireRemovalAction: true);
				// Release prisoner
				var ai = ((INPC)patrol.PatrolLeader).AIs.OfType<EnforcerAI>().First();
				foreach (var action in (ai.ThrowInPrisonEchoProg?.Execute<string>(leader, criminal, crime) ??
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
			var path = patrol.PatrolLeader.PathBetween(patrol.LegalAuthority.PrisonLocation, 50,
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

			var fp = new FollowingPath(patrol.PatrolLeader, path)
				{ UseDoorguards = true, UseKeys = true, OpenDoors = true };
			patrol.PatrolLeader.AddEffect(fp);
			fp.FollowPathAction();
			return;
		}

		// Is criminal in combat with enforcers?
		if (criminal.Combat?.Combatants.Any(x => patrol.PatrolMembers.Contains(x)) == true)
		{
			// Get the other enforcers to join in up to a limit
			foreach (var member in patrol.PatrolMembers)
			{
				EngageInCombat(member, criminal, crime, patrol);
			}

			return;
		}

		// Are any of the enforcers engaged with other targets?
		var engagedPatrolMembers = patrol.PatrolMembers.Where(x =>
			                                 x.Combat != null && x.Combat.Combatants.Any(y =>
				                                 patrol.PatrolMembers.Contains(y.CombatTarget)))
		                                 .ToList();
		if (engagedPatrolMembers.Any())
		{
			foreach (var enforcer in patrol.PatrolMembers.Except(engagedPatrolMembers).ToArray())
			{
				enforcer.ExecuteCommand($"support {enforcer.BestKeywordFor(engagedPatrolMembers.GetRandomElement())}");
			}

			return;
		}

		// Is criminal incapacitated?
		if (criminal.IsHelpless)
		{
			// Grab the criminal
			var random = patrol.PatrolMembers.GetRandomElement();
			if (random.Combat?.CanFreelyLeaveCombat(random) == true)
			{
				random.Combat?.LeaveCombat(random);
			}

			random.ExecuteCommand($"drag {random.BestKeywordFor(criminal)}");
			if (random.CombinedEffectsOfType<Dragging>().Any(x => x.Target == criminal))
			{
				foreach (var other in patrol.PatrolMembers.Except(random))
				{
					if (other.Combat?.CanFreelyLeaveCombat(other) == true)
					{
						other.Combat?.LeaveCombat(other);
					}

					other.ExecuteCommand($"drag help {other.BestKeywordFor(random)}");
				}
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
		foreach (var member in patrol.PatrolMembers)
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
			var path = patrol.PatrolLeader.PathBetween(patrol.OriginLocation, 50,
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

		var leader = patrol.PatrolLeader;
		if (leader.Location != patrol.LegalAuthority.MarshallingLocation &&
		    !leader.CombinedEffectsOfType<FollowingPath>().Any())
		{
			var path = leader.PathBetween(patrol.LegalAuthority.MarshallingLocation, 25,
				PathSearch.PathIncludeUnlockableDoors(leader));
			if (path != null)
			{
				var fp = new FollowingPath(leader, path) { UseDoorguards = true, UseKeys = true, OpenDoors = true };
				leader.AddEffect(fp);
				fp.FollowPathAction();
			}

			return;
		}

		if (patrol.PatrolMembers.All(x => x.Location == patrol.LegalAuthority.MarshallingLocation) &&
		    !patrol.PatrolLeader.AffectedBy<FollowingPath>())
		{
			var path = patrol.PatrolLeader.PathBetween(patrol.NextMajorNode, 50,
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
			
			var fp = new FollowingPath(patrol.PatrolLeader, path)
				{ UseDoorguards = true, UseKeys = true, OpenDoors = true };
			patrol.PatrolLeader.AddEffect(fp);
			fp.FollowPathAction();
		}
	}

	protected virtual void DoPreparationRoomAction(ICharacter member)
	{
		// Do nothing
	}

	protected virtual void PatrolTickPreparationPhase(IPatrol patrol)
	{
		foreach (var member in patrol.PatrolMembers)
		{
			if (member.Location != patrol.LegalAuthority.PreparingLocation &&
			    !member.CombinedEffectsOfType<FollowingPath>().Any())
			{
				var path = member.PathBetween(patrol.LegalAuthority.PreparingLocation, 25,
					PathSearch.PathIncludeUnlockableDoors(member));
				if (path != null)
				{
					var fp = new FollowingPath(member, path) { UseDoorguards = true, UseKeys = true, OpenDoors = true };
					member.AddEffect(fp);
					fp.FollowPathAction();
				}

				continue;
			}

			DoPreparationRoomAction(member);
		}

		if (DateTime.UtcNow - patrol.LastArrivedTime > TimeSpan.FromMinutes(1))
		{
			if (patrol.PatrolLeader.Party != null)
			{
				patrol.PatrolLeader.LeaveParty();
			}

			var party = new Party(patrol.PatrolLeader);
			patrol.PatrolLeader.JoinParty(party);
			foreach (var member in patrol.PatrolMembers.Where(x => x.ColocatedWith(patrol.PatrolLeader)).ToList())
			{
				if (member == patrol.PatrolLeader)
				{
					continue;
				}

				if (member.Party != null)
				{
					member.LeaveParty();
					member.JoinParty(party);
				}
			}

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