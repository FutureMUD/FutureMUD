#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Combat.Moves;

// XML profile draft. Attacker skill remains CombatAction.TraitDefinitionId; CounterTrait is explicit.
// Formula numbers/IDs are authored native content. Damage and stun are independent expressions.
internal sealed record PhysicalImpactProfile(
	ITraitDefinition CounterTrait, ITraitDefinition GuardTrait, ITraitDefinition AgilityTrait,
	ITraitExpression Damage, ITraitExpression Stun, ITraitExpression CounterDamage, ITraitExpression CounterStun,
	double MinimumStamina, double SizeBonusPerCategory, OpposedOutcomeDegree CounterDegree,
	bool RequireUprightTarget, bool SeatOnMiss, bool SeatOnHit, bool SeatOnCounter,
	bool ShieldBlock, bool AllowUntrained, bool RequireUsableLeg, bool InitiateCombat,
	double SuccessDelaySeconds) : IPhysicalImpactProfile
{
	internal static PhysicalImpactProfile Load(XElement e, IFuturemud world)
	{
		string Required(string key) => e.Attribute(key)?.Value ?? throw new FormatException($"Missing impact {key}.");
		double Number(string key)
		{
			var value = double.Parse(Required(key), CultureInfo.InvariantCulture);
			return double.IsFinite(value) && value >= 0 && value <= 86400 ? value : throw new FormatException($"Invalid impact {key}.");
		}
		bool Flag(string key) => bool.Parse(Required(key));
		ITraitDefinition Trait(string key)
		{
			var trait = world.Traits.Get(long.Parse(Required(key), CultureInfo.InvariantCulture))
				?? throw new FormatException($"Unresolved impact {key}.");
			if (key != "agilitytrait" && (trait is not ISkillDefinition || trait.OwnerScope != TraitOwnerScope.Character))
				throw new FormatException($"Impact {key} must be a character skill.");
			if (key == "agilitytrait" && trait is not IAttributeDefinition) throw new FormatException("Impact agility must be an attribute.");
			return trait;
		}
		ITraitExpression Formula(string key)
		{
			var result = new MudSharp.Body.Traits.TraitExpression(e.Element(key)?.Value ?? throw new FormatException($"Missing {key}."), world);
			if (result.HasErrors()) throw new FormatException(result.Error);
			return result;
		}
		var degree = (OpposedOutcomeDegree)int.Parse(Required("counterdegree"), CultureInfo.InvariantCulture);
		if (!Enum.IsDefined(degree) || degree <= OpposedOutcomeDegree.None) throw new FormatException("Invalid counterdegree.");
		return new(Trait("countertrait"), Trait("guardtrait"), Trait("agilitytrait"), Formula("Damage"), Formula("Stun"),
			Formula("CounterDamage"), Formula("CounterStun"), Number("minimumstamina"), Number("sizebonus"), degree,
			Flag("uprighttarget"), Flag("seatonmiss"), Flag("seatonhit"), Flag("seatoncounter"),
			Flag("shieldblock"), Flag("allowuntrained"), Flag("usableleg"), Flag("initiatecombat"), Number("successdelay"));
	}

	public XElement Save() => new("PhysicalImpact",
		new XAttribute("agilitytrait", AgilityTrait.Id), new XAttribute("countertrait", CounterTrait.Id), new XAttribute("guardtrait", GuardTrait.Id),
		new XAttribute("minimumstamina", MinimumStamina.ToString(CultureInfo.InvariantCulture)),
		new XAttribute("sizebonus", SizeBonusPerCategory.ToString(CultureInfo.InvariantCulture)),
		new XAttribute("counterdegree", (int)CounterDegree), new XAttribute("uprighttarget", RequireUprightTarget),
		new XAttribute("seatonmiss", SeatOnMiss), new XAttribute("seatonhit", SeatOnHit),
		new XAttribute("seatoncounter", SeatOnCounter), new XAttribute("shieldblock", ShieldBlock),
		new XAttribute("allowuntrained", AllowUntrained), new XAttribute("usableleg", RequireUsableLeg),
		new XAttribute("initiatecombat", InitiateCombat), new XAttribute("successdelay", SuccessDelaySeconds.ToString(CultureInfo.InvariantCulture)),
		new XElement("Damage", Damage.OriginalFormulaText), new XElement("Stun", Stun.OriginalFormulaText),
		new XElement("CounterDamage", CounterDamage.OriginalFormulaText), new XElement("CounterStun", CounterStun.OriginalFormulaText));
}

internal static class PhysicalImpactResolver
{
	// Exact native predicates: CharacterCombat.WillAttackTarget checks Body IPacifismEffect.IsSuperPeaceful;
	// CanEngage checks Body IPacifismEffect.IsPeaceful and both rooms' IPeacefulEffect.Applies(actor,target).
	// Do not call CanEngage for the already-selected target: it intentionally returns false in that case.
	internal static bool PeaceAllows(ICharacter a, ICharacter t) =>
		!a.Body.EffectsOfType<IPacifismEffect>().Any(x => x.IsPeaceful || x.IsSuperPeaceful) &&
		!a.Location.EffectsOfType<IPeacefulEffect>().Any(x => x.Applies(a, t)) &&
		!t.Location.EffectsOfType<IPeacefulEffect>().Any(x => x.Applies(a, t));

	private static bool Incapacitated(ICharacter t) =>
		t.State.HasFlag(CharacterState.Sleeping) || t.State.HasFlag(CharacterState.Unconscious) || t.State.HasFlag(CharacterState.Paralysed);

	internal static bool CanAttempt(ICharacter a, ICharacter t, IAuxiliaryCombatAction action, IPhysicalImpactProfile p) =>
		action.MaximumTargets == 1 && a != t && CharacterState.Able.HasFlag(a.State) &&
		!t.State.IsDead() && a.RidingMount is null && t.RidingMount is null &&
		(!p.RequireUprightTarget || t.PositionState.Upright) &&
		a.CanSpendStamina(Math.Max(p.MinimumStamina, AuxiliaryMove.MoveStaminaCost(a, action))) &&
		(!p.RequireUsableLeg || a.Body.Limbs.Any(x => x.LimbType == LimbType.Leg && a.Body.CanUseLimb(x) == CanUseLimbResult.CanUse)) &&
		(a.HasTrait(action.CheckTrait) || t.State.HasFlag(CharacterState.Paralysed) || p.AllowUntrained) && PeaceAllows(a, t);

	internal static bool CanQueue(ICharacter a, ICharacter t, IAuxiliaryCombatAction action, IPhysicalImpactProfile p) =>
		ReferenceEquals(a.Combat, t.Combat) && a.Combat is not null && AuxiliaryMove.CanReachTarget(a, t) && CanAttempt(a, t, action, p);

	internal static bool CanInitiate(ICharacter a, ICharacter t, IAuxiliaryCombatAction action, IPhysicalImpactProfile p) =>
		p.InitiateCombat && a.ColocatedWith(t) && a.RoomLayer == t.RoomLayer && a.CanSee(t) &&
		CanAttempt(a, t, action, p) && VehicleCombatService.Instance.CanCrossVehicleBoundary(a, t, false, false, out _);

	internal static CombatMoveResult Resolve(ICombatMove move, ICharacter a, ICharacter t,
		IAuxiliaryCombatAction action, IPhysicalImpactProfile p, ICombatMove? defense,
		Action<CheckOutcome>? onSuccessfulContact = null)
	{
		using var execution = CommandExecutionScope.EnterMove(move);
		var attackerPin = Pin(a);
		var targetPin = Pin(t);
		var attackTrait = action.CheckTrait;
		var usability = action.UsabilityProg;
		var needsPhysicalContact = false;
		bool Pins() => attackerPin() && targetPin();
		bool Continue() => Pins() && CommandExecutionAuthority.MayExecute(move, a) &&
			CommandExecutionScope.TryContinue() && Pins() &&
			(!needsPhysicalContact || a.CanInteractPlanar(t, PlanarInteractionKind.Physical) &&
			 a.CanInteractPlanar(t, PlanarInteractionKind.Combat)) && Pins();
		bool MutationFence() => Continue() && ReferenceEquals(action.CheckTrait, attackTrait) &&
			ReferenceEquals(action.UsabilityProg, usability) && action is IPhysicalImpactAuxiliaryAction current &&
			ReferenceEquals(current.PhysicalImpact, p) && a.Race.AuxiliaryActions.Any(x => ReferenceEquals(x, action)) &&
			CanQueue(a, t, action, p) && Continue();
		bool Admitted()
		{
			if (!Continue() || !CanQueue(a, t, action, p) || !Continue()) return false;
			if (!ReferenceEquals(action.CheckTrait, attackTrait) || !ReferenceEquals(action.UsabilityProg, usability) ||
				action is not IPhysicalImpactAuxiliaryAction current || !ReferenceEquals(current.PhysicalImpact, p)) return false;
			if (!VehicleCombatService.Instance.CanCrossVehicleBoundary(a, t, false, false, out _)) return false;
			// The same object remains bound; no name-based substitute after callbacks/reload.
			if (!a.Race.AuxiliaryActions.Any(x => ReferenceEquals(x, action)) || !action.UsableMove(a, t, false)) return false;
			return Continue() && CanQueue(a, t, action, p) && Continue();
		}
		var wounds = new List<IWound>();
		var selfWounds = new List<IWound>();
		var attackOutcome = Outcome.NotTested;
		var defenseOutcome = Outcome.NotTested;
		bool usedMagic = false;
		CombatMoveResult Result(bool hit) => new()
		{
			MoveWasSuccessful = hit, AttackerOutcome = attackOutcome, DefenderOutcome = defenseOutcome,
			RecoveryDifficulty = hit ? action.RecoveryDifficultySuccess : action.RecoveryDifficultyFailure,
			DefenderResponseWasUsed = usedMagic,
			WoundsCaused = wounds.ToArray(), SelfWoundsCaused = selfWounds.ToArray()
		};
		CombatMoveResult Interrupted() => CommandExecutionScope.HasCommitted ? Result(false) : CombatMoveResult.Irrelevant;
		bool Seat(ICharacter who)
		{
			if (!Admitted()) return false;
			var permitted = who.CanMovePosition(PositionSitting.Instance, true);
			if (!Continue() || !permitted) return false;
			using var owned = CommandExecutionScope.EnterOwnedOperation(who);
			if (!Continue()) return false;
			CommandExecutionScope.MarkCommitted();
			who.SetPosition(PositionSitting.Instance, PositionModifier.None, null, null);
			return true; // No additional hostile mutation after intentionally changing posture.
		}
		double Evaluate(ITraitExpression formula, ICharacter owner, int degree)
		{
			if (!Admitted()) throw new ImpactInterrupted();
			var result = CommandExecutionScope.EvaluateCallback(() => formula.EvaluateWith(owner, null,
				TraitBonusContext.UnarmedDamageCalculation, ("degree", degree), ("quality", (int)ItemQuality.Standard)), double.NaN);
			if (!Admitted()) throw new ImpactInterrupted();
			if (!double.IsFinite(result) || result < 0) throw new InvalidOperationException("Impact formula must return a finite nonnegative number.");
			return result;
		}

		try
		{
			if (!Admitted()) return CombatMoveResult.Irrelevant;
			// Source phase stumble is self-only. Never admit cross-plane damage.
			if (!a.CanInteractPlanar(t, PlanarInteractionKind.Physical) || !a.CanInteractPlanar(t, PlanarInteractionKind.Combat))
			{
				if (!Continue()) return Interrupted();
				MudSharp.Effects.Concrete.UnseenCombatOpening.Consume(a, t);
				CommandExecutionScope.MarkCommitted();
				Seat(a);
				a.OutputHandler.Send("Your impact passes harmlessly through your target and you lose your footing.");
				return Result(false);
			}
			needsPhysicalContact = true;
			if (t.PositionState == PositionFlying.Instance && a.PositionState != PositionFlying.Instance && t.CanSee(a))
			{
				if (!Admitted()) return Interrupted();
				MudSharp.Effects.Concrete.UnseenCombatOpening.Consume(a, t);
				CommandExecutionScope.MarkCommitted();
				a.OutputHandler.Send("Your target rises beyond your impact.");
				return Result(false);
			}
			using var hostile = HostileAttackAdmission.BeginAttempt(a, t);
			if (!HostileAttackAdmission.TryNotify(a, t, Admitted)) return Interrupted();
			if (!Admitted()) return Interrupted();
			MudSharp.Effects.Concrete.UnseenCombatOpening.Consume(a, t);
			CommandExecutionScope.MarkCommitted();
			if (InterceptGuard(a, t, p.GuardTrait, Admitted)) return Result(false);
			if (!Admitted()) return Interrupted();
			defense = MagicDefenseMove.Revalidate(defense, move);
			if (!Admitted()) return Interrupted();
			using var protection = new MagicDefenseDamageScope(move, defense);
			var sizeBonus = ((int)a.CurrentContextualSize(SizeContext.GrappleAttack) -
				(int)t.CurrentContextualSize(SizeContext.GrappleDefense)) * p.SizeBonusPerCategory;
			var agilityBonus = 2.0 * (a.TraitValue(p.AgilityTrait) - t.TraitValue(p.AgilityTrait));
			var shieldBonus = p.SeatOnHit ? (a.Body.WieldedItems.Any(x => x.IsItemType<IShield>()) ? 20.0 : -10.0) : 0.0;
			if (!Admitted()) return Interrupted();
			CommandExecutionScope.MarkCommitted();
			var attack = CombatCheck.For(a.Gameworld, CheckType.AuxiliaryMoveCheck)
				.Check(a, action.MoveDifficulty, action.CheckTrait, t, a.OffensiveAdvantage + sizeBonus + agilityBonus + shieldBonus +
					(p.AllowUntrained && !a.HasTrait(action.CheckTrait) ? 1.0 : 0.0));
			attackOutcome = attack.Outcome;
			if (!Admitted()) return Interrupted();
			CommandExecutionScope.MarkCommitted(); // An actual check/attempt now owns its ordinary combat cost.
			a.OffensiveAdvantage = 0;
			if (defense is MagicDefenseMove magic)
			{
				var stopped = magic.TryDefend(move, attack, out var protectedResult);
				usedMagic = magic.Committed;
				if (!Admitted()) return Interrupted();
				if (stopped) return new CombatMoveResult { MoveWasSuccessful = protectedResult.MoveWasSuccessful,
					AttackerOutcome = protectedResult.AttackerOutcome, DefenderOutcome = protectedResult.DefenderOutcome,
					RecoveryDifficulty = protectedResult.RecoveryDifficulty, DefenderResponseWasUsed = usedMagic,
					WoundsCaused = protectedResult.WoundsCaused, SelfWoundsCaused = protectedResult.SelfWoundsCaused };
			}
			var defender = Outcome.NotTested;
			if (!Incapacitated(t) && t.HasTrait(p.CounterTrait))
			{
				var roll = CombatCheck.For(a.Gameworld, CheckType.CombatMoveCheck).Check(t,
					Difficulty.Normal, p.CounterTrait, a, t.DefensiveAdvantage);
				if (!Admitted()) return Interrupted();
				defender = roll.Outcome;
			}
			defenseOutcome = defender;
			var opposed = new OpposedOutcome(attack.Outcome, defender);
			if (!Incapacitated(t) && t.HasTrait(p.CounterTrait) &&
				opposed.Outcome == OpposedOutcomeDirection.Opponent && opposed.Degree >= p.CounterDegree)
			{
				var selfDamage = Evaluate(p.CounterDamage, t, (int)opposed.Degree);
				var selfStun = Evaluate(p.CounterStun, t, (int)opposed.Degree);
				Deliver(t, a, selfDamage, selfStun, Admitted, MutationFence, selfWounds);
				if (p.SeatOnCounter && Admitted()) Seat(a);
				Echo(a, t, "@'s impact is countered by $1, throwing &0 off balance.");
				selfWounds.ProcessPassiveWounds();
				return Result(false);
			}
			if (!Incapacitated(t) && !attack.IsPass())
			{
				EchoConfigured(a, t, action, attack, false);
				if (p.SeatOnMiss) Seat(a);
				return Result(false);
			}
			var damage = Evaluate(p.Damage, a, (int)opposed.Degree);
			var stun = Evaluate(p.Stun, a, (int)opposed.Degree);
			IDamage? remainder = null;
			if (p.ShieldBlock && ResolveShield(move, a, t, attack, damage, stun, Admitted, MutationFence, wounds, out remainder)) return Result(false);
			if (!Admitted()) return Interrupted();
			Deliver(a, t, damage, stun, Admitted, MutationFence, wounds, remainder);
			// Full magical absorption prevents contact-dependent control. Ordinary worn/natural armour
			// absorption alone does not imply that a physical body-check failed to make contact.
			var contact = protection.Finish(Result(true));
			contact.DefenderResponseWasUsed = usedMagic;
			if (contact.MoveWasSuccessful && Admitted())
			{
				EchoConfigured(a, t, action, attack, true);
				if (!Admitted()) return Interrupted();
				if (p.SuccessDelaySeconds > 0)
				{
					CommandExecutionScope.MarkCommitted();
					a.Gameworld.Scheduler.DelayScheduleType(t, ScheduleType.Combat,
						TimeSpan.FromSeconds(p.SuccessDelaySeconds * CombatBase.CombatSpeedMultiplier));
				}
				if (p.SeatOnHit && Admitted()) Seat(t);
			}
			if (contact.MoveWasSuccessful && attack.IsPass() && onSuccessfulContact is not null)
			{
				onSuccessfulContact(attack);
			}
			wounds.ProcessPassiveWounds();
			return contact;
		}
		catch (ImpactInterrupted) { return Interrupted(); }
	}

	private static void Echo(ICharacter a, ICharacter t, string text, ICharacter? ward = null) =>
		a.OutputHandler.Handle(new EmoteOutput(new Emote(text, a, a, t, ward), style: OutputStyle.CombatMessage));

	private static void EchoConfigured(ICharacter a, ICharacter t, IAuxiliaryCombatAction action, CheckOutcome roll, bool hit)
	{
		var message = hit ? a.Gameworld.CombatMessageManager.GetMessageFor(a, t, action, roll.Outcome) :
			a.Gameworld.CombatMessageManager.GetFailMessageFor(a, t, action, roll.Outcome);
		Echo(a, t, message);
	}

	private static Func<bool> Pin(ICharacter ch)
	{
		var body = ch.Body; var focus = body.Actor; var instance = ch.InstanceId; var race = ch.Race;
		var room = ch.Location; var layer = ch.RoomLayer; var combat = ch.Combat; var state = ch.State;
		var combatTarget = ch.CombatTarget; var embodied = ch.IsEmbodied;
		return () => ReferenceEquals(ch.Body, body) && ReferenceEquals(body.Actor, focus) &&
			ch.InstanceId == instance && ch.IsEmbodied == embodied && ReferenceEquals(ch.Race, race) && ch.State == state &&
			ReferenceEquals(ch.Location, room) && ch.RoomLayer == layer && ReferenceEquals(ch.Combat, combat) &&
			ReferenceEquals(ch.CombatTarget, combatTarget) &&
			combat is not null && combat.Combatants.Any(x => ReferenceEquals(x, ch));
	}

	private static void Deliver(ICharacter origin, ICharacter victim, double amount, double stun,
		Func<bool> valid, Func<bool> mutationFence, ICollection<IWound> wounds, IDamage? transformed = null)
	{
		if (transformed is null ? amount <= 0 && stun <= 0 : transformed.DamageAmount <= 0 && transformed.StunAmount <= 0 && transformed.PainAmount <= 0) return;
		if (!valid()) throw new ImpactInterrupted();
		var body = victim.Body;
		var part = body.RandomBodypart;
		if (!valid() || part is null || !body.Bodyparts.Contains(part)) throw new ImpactInterrupted();
		var payload = transformed ?? new Damage { DamageType = DamageType.Crushing, DamageAmount = amount, StunAmount = stun,
			PainAmount = amount, ShockAmount = 0, PenetrationOutcome = Outcome.NotTested };
		var damage = new Damage(payload) { ActorOrigin = origin, TargetBody = body, Bodypart = part,
			MutationGuard = new ImpactFence(mutationFence, body) };
		if (!valid()) throw new ImpactInterrupted();
		CommandExecutionScope.MarkCommitted();
		foreach (var wound in body.CommandSufferDamage(damage)) wounds.Add(wound);
	}

	// A shield with native armour partially absorbs through that component, with its remaining damage
	// passed once to the body. A non-armour shield uses the native full-block adaptation.
	private static bool ResolveShield(ICombatMove move, ICharacter a, ICharacter t, CheckOutcome attack,
		double amount, double stun, Func<bool> valid, Func<bool> mutationFence, ICollection<IWound> wounds, out IDamage? passedOn)
	{
		passedOn = null;
		if (Incapacitated(t) || !valid()) return false;
		var shield = t.Body.WieldedItems.Select(x => x.GetItemType<IShield>()).OfType<IShield>()
			.Where(x => t.Body.CanWield(x.Parent, ItemCanWieldFlags.IgnoreFreeHands))
			.OrderByDescending(x => x.ShieldType.BlockBonus).FirstOrDefault();
		if (shield is null) return false;
		var item = shield.Parent;
		bool Current() => valid() && t.Body.WieldedItems.Contains(item) &&
			ReferenceEquals(item.GetItemType<IShield>(), shield) && t.Body.CanWield(item, ItemCanWieldFlags.IgnoreFreeHands);
		bool ShieldFence() => mutationFence() && t.Body.WieldedItems.Contains(item) &&
			ReferenceEquals(item.GetItemType<IShield>(), shield);
		if (!Current()) throw new ImpactInterrupted();
		var blockCost = BlockMove.MoveStaminaCost(a, t, shield);
		if (!Current()) throw new ImpactInterrupted();
		if (!t.CanSpendStamina(blockCost)) return false;
		if (!Current()) throw new ImpactInterrupted();
		var roll = CombatCheck.ForDefense(a.Gameworld, CheckType.BlockCheck, move).Check(t, Difficulty.Normal,
			shield.ShieldType.BlockTrait, a, shield.ShieldType.BlockBonus - t.GetDefensiveAdvantagePenaltyFromTargeting());
		if (!Current()) throw new ImpactInterrupted();
		// Selected ordinary auxiliary defense is not used; explicitly pay only this actual shield check.
		using (CommandExecutionScope.EnterIndependent()) t.Body.SpendStamina(blockCost);
		if (!Current()) throw new ImpactInterrupted();
		if (new OpposedOutcome(attack.Outcome, roll.Outcome).Outcome == OpposedOutcomeDirection.Proponent) return false;
		var damage = new Damage { ActorOrigin = a, DamageType = DamageType.Crushing,
			DamageAmount = amount, StunAmount = stun, PainAmount = amount,
			MutationGuard = new ImpactFence(ShieldFence, null, item) };
		CommandExecutionScope.MarkCommitted();
		if (item.GetItemType<IArmour>() is { } armour)
		{
			var shieldWounds = new List<IWound>();
			var remainder = armour.ArmourType.AbsorbDamage(damage, armour, t, ref shieldWounds, true);
			if (!Current()) throw new ImpactInterrupted();
			(armour as IConditionDegradingComponent)?.UseCondition(
				MudSharp.GameItems.Components.ArmourGameItemComponent.CreateArmourUseContext(damage, remainder));
			foreach (var wound in shieldWounds) wounds.Add(wound);
			if (!Current()) throw new ImpactInterrupted();
			passedOn = remainder;
			if (remainder is not null && (remainder.DamageAmount > 0 || remainder.StunAmount > 0 || remainder.PainAmount > 0)) return false;
			wounds.ProcessPassiveWounds();
			return true;
		}
		foreach (var wound in item.CommandSufferDamage(damage)) wounds.Add(wound);
		wounds.ProcessPassiveWounds();
		return true;
	}

	private static bool InterceptGuard(ICharacter a, ICharacter t, ITraitDefinition trait, Func<bool> valid)
	{
		var candidates = t.Location.Characters.Where(g => g != a && g != t &&
			g.EffectsOfType<IGuardCharacterEffect>().Any(e => e.Targets.Contains(t))).ToArray();
		foreach (var g in candidates)
		{
			var effect = g.EffectsOfType<IGuardCharacterEffect>().FirstOrDefault(e => e.Applies() && e.Targets.Contains(t));
			if (effect is null) continue;
			var pin = PinGuard(g);
			var physicalGuard = PinGuard(g, true);
			var physicalAttacker = PinGuard(a, true);
			var physicalWard = PinGuard(t, true);
			bool ReactionCurrent() => physicalGuard() && physicalAttacker() && physicalWard() &&
				g.EffectsOfType<IGuardCharacterEffect>().Any(e => ReferenceEquals(e, effect)) && effect.Targets.Contains(t) && effect.Applies() &&
				physicalGuard() && physicalAttacker() && physicalWard() && CommandExecutionAuthority.IsCurrent(g, false);
			bool Eligible() => pin() && CommandExecutionAuthority.IsCurrent(g, false) && CharacterState.Able.HasFlag(g.State) && g.PositionState.Upright &&
				g.ColocatedWith(t) && g.RoomLayer == t.RoomLayer && g.CanSee(a) && PeaceAllows(g, a) &&
				g.CanInteractPlanar(a, PlanarInteractionKind.Combat) && g.CanInteractPlanar(a, PlanarInteractionKind.Physical) &&
				g.EffectsOfType<IGuardCharacterEffect>().Any(e => ReferenceEquals(e, effect)) && effect.Applies() && effect.Targets.Contains(t);
			if (!Eligible() || !valid()) continue;
			CheckOutcome roll;
			using (CommandExecutionScope.EnterIndependent())
			{
				// Guard acts autonomously; never inherit the attacker's accepted-order capability.
				roll = g.Gameworld.GetCheck(CheckType.GenericSkillCheck).Check(g, Difficulty.Normal, trait, a);
			}
			// Original admission is checked only AFTER restoring the attacker's scope. Calling it
			// inside EnterIndependent would run attacker-authored usability callbacks without its order fence.
			if (!valid() || !Eligible() || !valid() || !roll.IsPass()) continue;
			if (ReferenceEquals(g.CombatTarget, a) && ReferenceEquals(g.Combat, a.Combat))
			{
				CommandExecutionScope.MarkCommitted();
				return true; // Guard is already engaged; existing scheduler provides its attack.
			}
			bool canEngage;
			using (CommandExecutionScope.EnterIndependent()) canEngage = g.CanEngage(a);
			if (!canEngage || !valid() || !Eligible() || !valid()) continue;
			CommandExecutionScope.MarkCommitted();
			bool stopped;
			using (CommandExecutionScope.EnterIndependentPhysicalOperation(g, ReactionCurrent)) stopped = g.Engage(a, false);
			if (stopped)
			{
				Echo(g, a, "@ step|steps in to stop $1's attack on $2.", t);
				return true;
			}
			if (!valid()) throw new ImpactInterrupted();
		}
		return false;
	}

	private static Func<bool> PinGuard(ICharacter g, bool allowCombatChanges = false)
	{
		var body = g.Body; var focus = body.Actor; var instance = g.InstanceId; var room = g.Location; var layer = g.RoomLayer;
		var combat = g.Combat; var target = g.CombatTarget; var state = g.State; var embodied = g.IsEmbodied;
		return () => ReferenceEquals(g.Body, body) && ReferenceEquals(body.Actor, focus) &&
			g.InstanceId == instance && ReferenceEquals(g.Location, room) && g.RoomLayer == layer &&
			(allowCombatChanges || ReferenceEquals(g.Combat, combat) && ReferenceEquals(g.CombatTarget, target)) && g.State == state && g.IsEmbodied == embodied;
	}

	private sealed class ImpactFence(Func<bool> valid, IBody? body, IHaveWounds? expectedItem = null) : IDamageMutationGuard
	{
		public void Validate(IHaveWounds recipient, IDamage damage)
		{
			if (!valid()) throw new ImpactInterrupted();
			// Native injury processing propagates this packet to bones and organs before
			// creating its surface wound. Only current parts of this exact body qualify.
			if (body is not null && (!ReferenceEquals(damage.TargetBody, body) ||
				!body.Bodyparts.Any(x => ReferenceEquals(x, damage.Bodypart)) &&
				!body.Bones.Any(x => ReferenceEquals(x, damage.Bodypart)) &&
				!body.Organs.Any(x => ReferenceEquals(x, damage.Bodypart)))) throw new ImpactInterrupted();
			if (!valid()) throw new ImpactInterrupted();
			if (expectedItem is not null && !ReferenceEquals(recipient, expectedItem)) throw new ImpactInterrupted();
			if (recipient is IBody actual && body is not null && !ReferenceEquals(actual, body)) throw new ImpactInterrupted();
		}
	}
	private sealed class ImpactInterrupted : InvalidOperationException { }
}

