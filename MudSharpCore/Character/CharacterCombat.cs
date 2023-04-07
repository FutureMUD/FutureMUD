using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Health;
using MudSharp.RPG.Law;
using LegalAuthority = MudSharp.RPG.Law.LegalAuthority;

namespace MudSharp.Character;

public partial class Character
{
	private ICharacterCombatSettings _combatSettings;

	public override ICharacterCombatSettings CombatSettings
	{
		get => _combatSettings;
		set
		{
			_combatSettings = value;
			if (Combat != null)
			{
				CombatStrategyMode = MeleeRange ? value.PreferredMeleeMode : value.PreferredRangedMode;
			}

			Changed = true;
		}
	}

	public int CombatBurdenOffense { get; set; }
	public int CombatBurdenDefense { get; set; }

	protected bool HandleCombatEvent(EventType type, params dynamic[] arguments)
	{
		if (type == EventType.CharacterEnterCellFinish && Combat != null &&
		    CombatStrategyMode == CombatStrategyMode.Flee)
		{
			if (Combat.Combatants.All(x =>
			    {
				    if (x.CombatTarget != this)
				    {
					    return true;
				    }

				    return x is ICharacter character &&
				           (character.State.HasFlag(CharacterState.Sleeping) ||
				            character.State.HasFlag(CharacterState.Unconscious) ||
				            character.State.HasFlag(CharacterState.Paralysed));
			    }))
			{
				Combat.TruceRequested(this);
				return true;
			}
		}

		if (type == EventType.ReadyGunEmpty)
		{
			if (EffectsOfType<FirearmNeedsReloading>().Any(x => x.Firearm == arguments[1]))
			{
				return false;
			}

			AddEffect(new FirearmNeedsReloading(this, arguments[1]), TimeSpan.FromSeconds(300));
			return true;
		}

		return false;
	}

	#region Implementation of ICombatant

	private DefenseType _preferredDefenseType = DefenseType.None;

	public override DefenseType PreferredDefenseType
	{
		get => _preferredDefenseType;
		set
		{
			_preferredDefenseType = value;
			Changed = true;
		}
	}

	public override ICombatMove ResponseToMove(ICombatMove move, IPerceiver assailant)
	{
		return CombatStrategyFactory.GetStrategy(CombatStrategyMode).ResponseToMove(move, this, assailant);
	}

	public override ICombatMove ChooseMove()
	{
		return CombatStrategyFactory.GetStrategy(CombatStrategyMode).ChooseMove(this);
	}

	private void ForceAcquireTargetCheckForOpponents()
	{
		if (Combat != null)
		{
			foreach (var combatant in Combat.Combatants.Where(x => x.CombatTarget == this).ToList())
			{
				combatant.AcquireTarget();
			}
		}
	}

	public override void AcquireTarget()
	{
		if (Combat == null)
		{
			return;
		}

		if (!State.IsAble())
		{
			return;
		}

		if (CombatTarget != null)
		{
			if (!ColocatedWith(CombatTarget) && MeleeRange)
			{
				MeleeRange = false;
			}

			if (!ColocatedWith(CombatTarget) && CombatTarget.CombatStrategyMode == CombatStrategyMode.Flee)
			{
				if (CombatStrategyMode.IsMeleeDesiredStrategy() &&
				    !CombatTarget.DistanceBetweenLessThanOrEqual(this, 1))
				{
					CombatTarget = null;
					return;
				}

				var maxRange =
					Body.WieldedItems.Select(
						    x => (int?)x.GetItemType<IRangedWeapon>()?.WeaponType.DefaultRangeInRooms ?? 0)
					    .DefaultIfEmpty(0)
					    .Max();
				// TODO - natural ranged attacks
				if (!CombatTarget.DistanceBetweenLessThanOrEqual(this, (uint)maxRange))
				{
					CombatTarget = null;
					return;
				}
			}
		}

		var targetingMe = Combat.Combatants.Where(x => x.CombatTarget == this && CanEngage(x)).ToList();
		if (targetingMe.Any())
		{
			if (targetingMe.Any(x => x.ColocatedWith(this)))
			{
				var newTarget = targetingMe.Where(x => x.ColocatedWith(this)).GetRandomElement();
				Engage(newTarget, false);
				CombatTarget = newTarget;
				return;
			}

			Engage(targetingMe.GetRandomElement(), false);
			return;
		}

		HandleEvent(EventType.NoNaturalTargets, this);
	}

	public override bool CheckCombatStatus()
	{
		if (Combat == null)
		{
			return false;
		}

		if (State == CharacterState.Dead)
		{
			return false;
		}

		if (CombatTarget == null && Combat.Combatants.All(x => x.CombatTarget != this))
		{
			MeleeRange = false;
			if (State.HasFlag(CharacterState.Sleeping) || State.HasFlag(CharacterState.Unconscious) ||
			    State.HasFlag(CharacterState.Paralysed))
			{
				return false;
			}
		}

		if (CombatTarget != null && !CombatTarget.ColocatedWith(this) && MeleeRange)
		{
			MeleeRange = false;
		}

		foreach (var combatant in Combat.Combatants
		                                .Where(x => x.CombatTarget == this && !x.ColocatedWith(this) && x.MeleeRange)
		                                .ToList())
		{
			combatant.MeleeRange = false;
		}

		if (CombatTarget == null || !WillAttackTarget())
		{
			if (CombatTarget == null && Combat.Combatants.All(x => x.CombatTarget != this))
			{
				return false;
			}

			AcquireTarget();
			if (CombatTarget == null)
			{
				return false;
			}
		}

		return true;
	}

	public override ItemQuality NaturalWeaponQuality(INaturalAttack attack)
	{
		// todo - effects that modify natural weapon quality
		return
			Merits.OfType<INaturalAttackQualityMerit>()
			      .Where(x => x.Applies(this))
			      .Aggregate(attack.Quality, (x, y) => y.GetQuality(x));
	}

	private bool WillAttackTarget()
	{
		if (CombatTarget == null)
		{
			return false;
		}

		if (Body.EffectsOfType<IPacifismEffect>().Any(x => x.IsSuperPeaceful))
		{
			return false;
		}

		if (CombatSettings.AttackUnarmedOrHelpless)
		{
			return true;
		}

		var charTarget = CombatTarget as ICharacter;
		if ((charTarget?.Race.CombatSettings.CanUseWeapons ?? false) &&
		    charTarget.Body.HeldOrWieldedItems.Any(
			    x => x.IsItemType<IMeleeWeapon>() || x.IsItemType<IRangedWeapon>()))
		{
			return false;
		}

		if (!(charTarget?.PositionState.Upright ?? true))
		{
			return false;
		}

		return CharacterState.Able.HasFlag(charTarget?.State ?? CharacterState.Able);
	}

	public override bool CanTruce()
	{
		// TODO - Merits, Effects, etc
		return !Merits.Any(x => x is ICombatRecklessnessMerit && x.Applies(this)) &&
		       EffectsOfType<IRageEffect>().All(x => !x.IsRaging) && base.CanTruce();
	}

	public override string WhyCannotTruce()
	{
		// TODO - Merits, Effects, etc
		if (Merits.Any(x => x is ICombatRecklessnessMerit && x.Applies(this)))
		{
			return "Never give up, never surrender!";
		}

		if (EffectsOfType<IRageEffect>().Any(x => x.IsRaging))
		{
			return "You are filled with too much rage to consider a cease to hostilities.";
		}

		return base.WhyCannotTruce();
	}

	public override Facing GetFacingFor(ICombatant opponent, bool reset = false)
	{
		if (opponent == null)
		{
			return Facing.Front;
		}

		if ((opponent.CombatStrategyMode == CombatStrategyMode.Flee ||
		     opponent.CombatStrategyMode == CombatStrategyMode.FullSkirmish) &&
		    !CombatSettings.ForbiddenIntentions.HasFlag(CombatMoveIntentions.Flank))
		{
			return Facing.Rear;
		}

		var returnValue = Effects.OfType<IFixedFacingEffect>().FirstOrDefault(x => x.AppliesTo(opponent));
		if (returnValue != null)
		{
			var value = returnValue.Facing;
			if (reset)
			{
				EffectHandler.RemoveEffect(returnValue);
			}

			return value;
		}

		return Facing.Front;
	}

	public override bool CanEngage(IPerceiver target)
	{
#if DEBUG
		if (!Location.Characters.Contains(this))
		{
			throw new ApplicationException("Ghosting NPC");
		}
#endif
		if (target == this)
		{
			return false;
		}

		if (target == CombatTarget)
		{
			return false;
		}

		if (Location.EffectsOfType<IPeacefulEffect>().Any(x => x.Applies(this, target)))
		{
			return false;
		}

		if (target.Location.EffectsOfType<IPeacefulEffect>().Any(x => x.Applies(this, target)))
		{
			return false;
		}

		if (Body.EffectsOfType<IPacifismEffect>().Any(x => x.IsPeaceful))
		{
			return false;
		}

		if ((Combat?.Friendly ?? false) && (target.Combat == null || target.Combat != Combat))
		{
			return false;
		}

		if (CombinedEffects()
		    .Any(x =>
			    (x.IsBlockingEffect("combat-engage") || x.IsBlockingEffect("general")) &&
			    !x.CanBeStoppedByPlayer))
		{
			return false;
		}

		if (ShouldFall())
		{
			return false;
		}

		return !EffectsOfType<IRecentlyRescuedTargetEffect>()
			.Any(
				x =>
					x.Rescued == target && x.Rescuer.Combat == Combat &&
					x.Rescuer.Location == Location && CharacterState.Able.HasFlag(x.Rescuer.State));
	}

	public override string WhyCannotEngage(IPerceiver target)
	{
		if (target == this)
		{
			return "You cannot engage yourself in combat.";
		}

		if (target == CombatTarget)
		{
			return $"You are already engaged with {target.HowSeen(this)}.";
		}

		if (Location.EffectsOfType<IPeacefulEffect>().Any(x => x.Applies(this, target)))
		{
			return "This location is too peaceful for any fighting to occur";
		}

		if (target.Location.EffectsOfType<IPeacefulEffect>().Any(x => x.Applies(this, target)))
		{
			return "Your target's location is too peaceful for any fighting to occur";
		}

		if (Body.EffectsOfType<IPacifismEffect>().Any(x => x.IsPeaceful))
		{
			return "You are feeling much too peaceful to fight anyone. Why can't we all just get along?";
		}

		if ((Combat?.Friendly ?? false) && (target.Combat == null || target.Combat != Combat))
		{
			return "You cannot engage people outside your friendly bout, they must join willingly.";
		}

		if (CombinedEffects()
		    .Any(x =>
			    (x.IsBlockingEffect("combat-engage") || x.IsBlockingEffect("general")) &&
			    !x.CanBeStoppedByPlayer))
		{
			return
				$"You cannot engage in combat until you stop {CombinedEffects().Where(x => (x.IsBlockingEffect("combat-engage") || x.IsBlockingEffect("general")) && !x.CanBeStoppedByPlayer).Select(x => x.BlockingDescription("general", this)).ListToString()}.";
		}

		if (ShouldFall())
		{
			return "You cannot engage in combat while you're tumbling through the air!";
		}

		return EffectsOfType<IRecentlyRescuedTargetEffect>()
			.Any(
				x =>
					x.Rescued == target && x.Rescuer.Combat == Combat &&
					x.Rescuer.Location == Location && CharacterState.Able.HasFlag(x.Rescuer.State))
			? $"You cannot switch back to {target.HowSeen(this)} so soon after they were rescued from you, at least while their rescuers are still here."
			: "You cannot initiate combat for an unknown reason.";
	}

	public override bool Engage(IPerceiver target, bool ranged)
	{
		if (!CanEngage(target))
		{
			return false;
		}

		ICombat combat;

		// Friendly fights automatically end when a non-friendly engagement occurs
		if (Combat != target.Combat && (target.Combat?.Friendly ?? false))
		{
			foreach (var person in target.Combat.Combatants)
			{
				person.Send(
					$"{HowSeen(person, true, DescriptionType.Possessive)} unfriendly attack has ended your friendly bout.");
			}

			target.Combat.EndCombat(true);
		}

		var ambush = target.CanSee(this);
		RemoveAllEffects(x => x.IsEffectType<IHideEffect>());

		// Merge/Create combats and ensure both assailant and target are in the same fight
		if (Combat != null && target.Combat != null)
		{
			if (Combat != target.Combat)
			{
				Combat.MergeCombat(target.Combat);
			}

			combat = Combat;
			OutputHandler.Handle(new EmoteOutput(new Emote("@ switch|switches target to $0!", this, target)));
		}
		else
		{
			OutputHandler.Handle(
				new EmoteOutput(new Emote("@ engage|engages $0 in combat!", this, target)));
			target.HandleEvent(EventType.EngagedInCombat, this, target);
			HandleEvent(EventType.EngageInCombat, this, target);
			foreach (var witness in Location.EventHandlers)
			{
				witness.HandleEvent(EventType.EngagedInCombatWitness, this, target, witness);
			}

			combat = target.Combat ?? new SimpleMeleeCombat(Gameworld);
		}

		combat.JoinCombat(this, ranged ? Difficulty.Automatic : Difficulty.Easy);
		combat.JoinCombat(target, ambush ? Difficulty.ExtremelyHard : Difficulty.Normal);

		if (!combat.Friendly)
		{
			if (target is ICharacter tch)
			{
				CrimeExtensions.CheckPossibleCrimeAllAuthorities(this, CrimeTypes.Assault, tch, null, "");
			}
			else if (target is IGameItem item)
			{
				CrimeExtensions.CheckPossibleCrimeAllAuthorities(this, CrimeTypes.DestructionOfProperty, null, item,
					"");
			}
		}

		// Handle target changes
		var oldTarget = CombatTarget;
		CombatTarget = target;
		oldTarget?.CheckCombatStatus();
		target.HandleEvent(EventType.TargettedInCombat, this, target);
		if (oldTarget != target)
		{
			oldTarget?.HandleEvent(EventType.NoLongerTargettedInCombat, this, oldTarget, target);
		}

		if (target.CombatTarget == this && target.MeleeRange)
		{
			EngageAlreadyInMelee();
		}
		else
		{
			if (target.CombatTarget == null)
			{
				target.CombatTarget = this;
			}

			if (target.CombatTarget == this)
			{
				EngageMutually();
			}
			else
			{
				EngageAlreadyEngaged();
			}
		}

		EffectHandler.RemoveAllEffects(x =>
			x.GetSubtype<Rescue>()?.RescueTarget == CombatTarget ||
			(x.GetSubtype<GuardCharacter>()?.Targets.Contains(CombatTarget) ?? false));
		return true;
	}

	private void EngageAlreadyInMelee()
	{
		MeleeRange = true;
		CombatStrategyMode = CombatSettings.PreferredMeleeMode;
		if (Aim != null && (Aim.Target != CombatTarget || !Aim.Weapon.WeaponType.FireableInMelee))
		{
			Aim?.ReleaseEvents();
			Aim = null;
		}
	}

	private void EngageAlreadyEngaged()
	{
		var ranged = false;
		if (!CombatTarget.ColocatedWith(this))
		{
			ranged = true;
		}
		else if (CombatSettings.PreferredRangedMode.IsRangedStartDesiringStrategy())
		{
			ranged = true;
		}

		if (ranged)
		{
			MeleeRange = false;
			CombatStrategyMode = CombatSettings.PreferredRangedMode;
		}
		else
		{
			MeleeRange = true;
			CombatStrategyMode = CombatSettings.PreferredMeleeMode;
			if (Aim != null && (Aim.Target != CombatTarget || !Aim.Weapon.WeaponType.FireableInMelee))
			{
				Aim?.ReleaseEvents();
				Aim = null;
			}
		}
	}

	private void EngageMutually()
	{
		var grappling = EffectsOfType<IGrappling>().Any(x => x.Target == CombatTarget) ||
		                EffectsOfType<IBeingGrappled>().Any(x => x.Grappling.Owner == CombatTarget);
		var ranged = false;
		if (!CombatTarget.ColocatedWith(this))
		{
			ranged = true;
		}
		else if (!grappling && CombatSettings.PreferredRangedMode.IsRangedStartDesiringStrategy() &&
		         CombatTarget.CombatSettings.PreferredRangedMode.IsRangedStartDesiringStrategy())
		{
			ranged = true;
		}

		if (ranged)
		{
			MeleeRange = false;
			CombatTarget.MeleeRange = false;
			CombatStrategyMode = CombatSettings.PreferredRangedMode;
			CombatTarget.CombatStrategyMode = CombatTarget.CombatSettings.PreferredRangedMode;
		}
		else
		{
			MeleeRange = true;
			CombatTarget.MeleeRange = true;
			CombatStrategyMode = CombatSettings.PreferredMeleeMode;
			CombatTarget.CombatStrategyMode = CombatTarget.CombatSettings.PreferredMeleeMode;
			if (grappling && CombatTarget is ICharacter chTarget)
			{
				AddEffect(new ClinchEffect(this, chTarget));
				chTarget.AddEffect(new ClinchEffect(chTarget, this));
			}

			if (Aim != null && (Aim.Target != CombatTarget || !Aim.Weapon.WeaponType.FireableInMelee))
			{
				Aim?.ReleaseEvents();
				Aim = null;
			}

			if (CombatTarget.Aim != null && (CombatTarget.Aim.Target != CombatTarget ||
			                                 !CombatTarget.Aim.Weapon.WeaponType.FireableInMelee))
			{
				CombatTarget.Aim?.ReleaseEvents();
				CombatTarget.Aim = null;
			}
		}
	}

	public override bool TakeOrQueueCombatAction(ISelectedCombatAction action)
	{
		if (AffectedBy<IdleCombatant>())
		{
			Combat?.CombatAction(this, action.GetMove(this));
			return false;
		}

		AddEffect(action);
		return true;
	}

	#region Overrides of PerceiverItem

	private bool _meleeRange;

	public override bool MeleeRange
	{
		get => _meleeRange;
		set
		{
			if (_meleeRange != value && CombatStrategyMode != CombatStrategyMode.Flee)
			{
				CombatStrategyMode = value ? CombatSettings.PreferredMeleeMode : CombatSettings.PreferredRangedMode;
			}

			if (value)
			{
				Movement?.CancelForMoverOnly(this);
				QueuedMoveCommands.Clear();
				EffectHandler.RemoveAllEffects(x => x.IsEffectType<IRemoveOnMeleeCombat>(), true);
				Body.RemoveAllEffects(x => x.IsEffectType<IRemoveOnMeleeCombat>(), true);
				Cover?.ReleaseEvents();
				Cover = null;
			}
			else
			{
				HandleEvent(EventType.NoLongerEngagedInMelee, this);
			}

			_meleeRange = value;
		}
	}

	#endregion

	#endregion

	#region IHaveAllies Implementation

	public bool IsAlly(ICharacter person)
	{
		return _allyIDs.Contains(person.Id);
	}

	public bool IsTrustedAlly(ICharacter person)
	{
		return _trustedAllyIDs.Contains(person.Id);
	}

	public void SetTrusted(long id, bool trusted)
	{
		if (_allyIDs.Contains(id))
		{
			if (trusted)
			{
				_trustedAllyIDs.Add(id);
			}
			else
			{
				_trustedAllyIDs.Remove(id);
			}
		}

		AlliesChanged = true;
	}

	public void SetAlly(ICharacter person)
	{
		_allyIDs.Add(person.Id);
		AlliesChanged = true;
	}

	public void SetAlly(long id)
	{
		_allyIDs.Add(id);
		AlliesChanged = true;
	}

	public void RemoveAlly(ICharacter person)
	{
		_allyIDs.Remove(person.Id);
		_trustedAllyIDs.Remove(person.Id);
		AlliesChanged = true;
	}

	public void RemoveAlly(long id)
	{
		_allyIDs.Remove(id);
		_trustedAllyIDs.Remove(Id);
		AlliesChanged = true;
	}

	public IEnumerable<long> TrustedAllyIDs => _trustedAllyIDs;
	private readonly HashSet<long> _trustedAllyIDs = new();
	public IEnumerable<long> AllyIDs => _allyIDs;
	private readonly HashSet<long> _allyIDs = new();

	public bool AlliesChanged
	{
		get => _alliesChanged;
		set
		{
			if (value)
			{
				Changed = true;
			}

			_alliesChanged = value;
		}
	}

	private bool _alliesChanged;

	public void SaveAllies(MudSharp.Models.Character dbchar)
	{
		foreach (var ally in dbchar.AlliesCharacter.Where(x => !AllyIDs.Contains(x.AllyId)).ToList())
		{
			dbchar.AlliesCharacter.Remove(ally);
		}

		foreach (var ally in AllyIDs.Where(x => dbchar.AlliesCharacter.All(y => y.AllyId != x)).ToList())
		{
			dbchar.AlliesCharacter.Add(new Ally
			{
				AllyId = ally,
				CharacterId = _id
			});
		}

		foreach (var ally in dbchar.AlliesCharacter)
		{
			ally.Trusted = TrustedAllyIDs.Contains(ally.AllyId);
		}

		AlliesChanged = false;
	}

	#endregion

	private readonly List<IMortalPerceiver> _seenTargets = new();
	public IEnumerable<IMortalPerceiver> SeenTargets => _seenTargets;

	public void SeeTarget(IMortalPerceiver target)
	{
		if (!_seenTargets.Contains(target))
		{
			_seenTargets.Add(target);
			target.OnQuit += Target_OnQuit;
			target.OnDeleted += Target_OnQuit;
			target.OnLocationChanged += Target_OnLocationChanged;
			target.OnDeath += Target_OnDeath;
		}
	}

	private void Target_OnDeath(IPerceivable owner)
	{
		var corpse = (owner as ICharacter)?.Corpse;
		if (corpse == null)
		{
			Target_OnQuit(owner);
			return;
		}

		var mp = (IMortalPerceiver)owner;
		LoseTarget(mp);
		SeeTarget(corpse.Parent);
	}

	private void Target_OnLocationChanged(ILocateable locatable, ICellExit exit)
	{
		CheckTarget((IMortalPerceiver)locatable);
	}

	private void Target_OnQuit(IPerceivable owner)
	{
		LoseTarget((IMortalPerceiver)owner);
	}

	public void LoseTarget(IMortalPerceiver target)
	{
		_seenTargets.Remove(target);
		target.OnQuit -= Target_OnQuit;
		target.OnDeleted -= Target_OnQuit;
		target.OnLocationChanged -= Target_OnLocationChanged;
		target.OnDeath -= Target_OnDeath;
	}

	public void CheckTarget(IMortalPerceiver target)
	{
		if (!this.DistanceBetweenLessThanOrEqual(target, MaximumPerceptionRange))
		{
			LoseTarget(target);
		}
	}

	private void CheckAllTargets()
	{
		// TODO - perhaps consider keeping targets in some circumstances, for now let's not do something computationally expensive.
		foreach (var target in SeenTargets.ToList())
		{
			LoseTarget(target);
		}
	}

	public uint MaximumPerceptionRange => 5; // TODO
}