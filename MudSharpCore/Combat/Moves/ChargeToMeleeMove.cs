using MudSharp.Body.Position;
using MudSharp.Body;
using MudSharp.Construction.Boundary;
using MudSharp.Character.Heritage;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Combat.Moves;

public class ChargeToMeleeMove : CombatMoveBase
{
    public override string Description => "Charging into melee combat";
	public bool IsMountedCharge => MountedCombatService.Instance.ResolveContext(Assailant) is not null;
	public bool IsBehemothCharge => Assailant.CombatTarget is ICharacter target && CanBehemothCharge(Assailant, target);
	public bool IsImpactCharge => IsMountedCharge || IsBehemothCharge;

    private bool _calculatedStamina = false;
    private double _staminaCost = 0.0;

    public override double StaminaCost
    {
        get
        {
            if (!_calculatedStamina)
            {
                _staminaCost = MoveStaminaCost(Assailant);
                _calculatedStamina = true;
            }

            return _staminaCost;
        }
    }

    public static double BaseStaminaCost(IFuturemud gameworld)
    {
        return gameworld.GetStaticDouble("ChargeToMeleeStaminaCost");
    }

    public static double MoveStaminaCost(ICharacter assailant)
    {
        return BaseStaminaCost(assailant.Gameworld) * CombatBase.GraceMoveStaminaMultiplier(assailant);
    }

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
		if (Assailant.CombatTarget is ICharacter boundaryTarget &&
		    !VehicleCombatService.Instance.CanCrossVehicleBoundary(Assailant, boundaryTarget, false, false,
			    out var boundaryReason))
		{
			Assailant.OutputHandler.Send(boundaryReason);
			return CombatMoveResult.Irrelevant;
		}
        if (Assailant.CombatTarget is not ICharacter target)
        {
            _delay = 0;
            return CombatMoveResult.Irrelevant;
        }

        if (!Assailant.ColocatedWith(target) || Assailant.MeleeRange || target.Combat != Assailant.Combat)
        {
            _delay = 0;
            return CombatMoveResult.Irrelevant;
        }

        ICombatMove response = defenderMove;

		var mountedContext = MountedCombatService.Instance.ResolveContext(Assailant);
		var behemothAttack = mountedContext is null ? GetBehemothChargeAttack(target) : null;
		if (mountedContext is not null && response is EvadeMountedChargeMove evade)
		{
			return ResolveMountedEvasion(target, mountedContext, evade);
		}

		if (mountedContext is not null && response is CounterMountedChargeMove counter)
		{
			return ResolveMountedCounterCharge(target, mountedContext, counter);
		}

        if (response is SkirmishResponseMove skirmish)
        {
			var outcome = HandleSkirmish(target, skirmish);
			ResolveCaughtSkirmishImpact(target, mountedContext, behemothAttack, outcome);
			return outcome;
        }

        if (response is SkirmishAndFire skirmishAndFire)
        {
			CombatMoveResult outcome = HandleSkirmish(target, skirmishAndFire);
            skirmishAndFire.ResolveMove(new HelplessDefenseMove { Assailant = Assailant });
			ResolveCaughtSkirmishImpact(target, mountedContext, behemothAttack, outcome);
            return outcome;
        }

		var mountedWeaponAttack = GetMountedWeaponAttack(target);
        Assailant.MeleeRange = true;
        if (target.CombatTarget == Assailant)
        {
            target.MeleeRange = true;
        }
		// Ordinary charges retain their established spacing-only behaviour. Mounted charges earn advantage
		// through the opposed impact resolution below.

        if (response is ReceiveChargeMove receiveCharge)
        {
			SendChargeMessage(target, mountedContext, Outcome.MinorPass);
			HandleReceiveCharge(receiveCharge, target);
			mountedContext = MountedCombatService.Instance.ResolveContext(Assailant);
			if (mountedContext is not null)
			{
				ResolveMountedImpact(target, mountedContext, true);
			}
			ResolveBehemothImpact(target, behemothAttack);
			ResolveMountedWeaponAttack(mountedWeaponAttack, target,
				new HelplessDefenseMove { Assailant = target });
            _delay = 0;
            return new CombatMoveResult { MoveWasSuccessful = true };
        }

        if (response is StandAndFireMove standAndFire)
        {
			SendChargeMessage(target, mountedContext, Outcome.NotTested);
            standAndFire.ResolveMove(new HelplessDefenseMove { Assailant = Assailant });
			mountedContext = MountedCombatService.Instance.ResolveContext(Assailant);
			if (mountedContext is not null)
			{
				ResolveMountedImpact(target, mountedContext, false);
			}
			ResolveBehemothImpact(target, behemothAttack);
			ResolveMountedWeaponAttack(mountedWeaponAttack, target,
				new HelplessDefenseMove { Assailant = target });
            _delay = 0;
            return new CombatMoveResult { MoveWasSuccessful = true };
        }

        if (response == null || response is HelplessDefenseMove)
        {
            // Unopposed - they may already be engaged in melee or just be ambivalent
			SendChargeMessage(target, mountedContext, Outcome.NotTested);
			if (mountedContext is not null)
			{
				ResolveMountedImpact(target, mountedContext, false);
			}
			ResolveBehemothImpact(target, behemothAttack);
			ResolveMountedWeaponAttack(mountedWeaponAttack, target,
				new HelplessDefenseMove { Assailant = target });
            _delay = 0;
            return new CombatMoveResult { MoveWasSuccessful = true };
        }

		// A defender can legitimately select an ordinary melee defence while a charge closes.
		// It has no special interception semantics, so resolve the charge as a normal close rather
		// than leaving a reachable combat path to throw.
		SendChargeMessage(target, mountedContext, Outcome.NotTested);
		if (mountedContext is not null)
		{
			ResolveMountedImpact(target, mountedContext, false);
		}
		ResolveBehemothImpact(target, behemothAttack);
		ResolveMountedWeaponAttack(mountedWeaponAttack, target, response);
		_delay = 0;
		return new CombatMoveResult { MoveWasSuccessful = true };
    }

	private (IMeleeWeapon Weapon, IWeaponAttack Attack, bool Couched)? GetMountedWeaponAttack(ICharacter target)
	{
		if (MountedCombatService.Instance.ResolveContext(Assailant) is null)
		{
			return null;
		}

		var attack = Assailant.Body.WieldedItems
			.SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
			.SelectMany(weapon => weapon.WeaponType.Attacks
				.Where(candidate => CouchedLanceMove.CanCouch(Assailant, weapon, candidate, target) ||
				                    MountedWeaponAttackMove.CanUse(Assailant, weapon, candidate, target))
				.Select(candidate => (Weapon: weapon, Attack: candidate,
					Couched: candidate.MoveType == BuiltInCombatMoveType.CouchedLanceAttack)))
			.OrderByDescending(x => x.Couched)
			.ThenByDescending(x => x.Weapon.WeaponType.Reach)
			.FirstOrDefault();

		return attack.Weapon is null || attack.Attack is null ? null : attack;
	}

	private void ResolveMountedWeaponAttack((IMeleeWeapon Weapon, IWeaponAttack Attack, bool Couched)? attack,
		ICharacter target,
		ICombatMove defense)
	{
		if (attack is null)
		{
			return;
		}

		if (attack.Value.Couched)
		{
			new CouchedLanceMove(Assailant, attack.Value.Weapon, attack.Value.Attack, target).ResolveMove(defense);
			return;
		}

		new MountedWeaponAttackMove(Assailant, attack.Value.Weapon, attack.Value.Attack, target, true)
			.ResolveMove(defense);
	}

	private CombatMoveResult ResolveMountedEvasion(ICharacter target, MountedCombatContext context,
		EvadeMountedChargeMove response)
	{
		var (attackRoll, defenseRoll, opposed) = ResolveMountedContest(target, context, false,
			CheckType.OpposeMountedChargeCheck);
		if (opposed.Outcome == OpposedOutcomeDirection.Proponent)
		{
			SendChargeMessage(target, context, attackRoll.Outcome);
			SetMeleeRange(target);
			ApplyMountedImpact(target, context, Math.Max(1, (int)opposed.Degree));
			ResolveMountedWeaponAttack(GetMountedWeaponAttack(target), target,
				new HelplessDefenseMove { Assailant = target });
			_delay = 0.0;
			return new CombatMoveResult
			{
				MoveWasSuccessful = true,
				AttackerOutcome = attackRoll,
				DefenderOutcome = defenseRoll
			};
		}

		SendChargeMessage(target, context, defenseRoll.Outcome, true);
		target.DefensiveAdvantage += 2.0 + (int)opposed.Degree;
		_delay = 1.0;
		return new CombatMoveResult
		{
			MoveWasSuccessful = false,
			AttackerOutcome = attackRoll,
			DefenderOutcome = defenseRoll,
			RecoveryDifficulty = Difficulty.Hard
		};
	}

	private CombatMoveResult ResolveMountedCounterCharge(ICharacter target, MountedCombatContext context,
		CounterMountedChargeMove response)
	{
		var defenderContext = MountedCombatService.Instance.ResolveContext(target);
		if (defenderContext is null)
		{
			return ResolveMountedEvasion(target, context,
				new EvadeMountedChargeMove { Assailant = target, PrimaryTarget = Assailant });
		}

		var attackCheck = GetMountedCheck(MountedCombatService.Instance.ChargeCheckType(context));
		var defenseCheck = GetMountedCheck(MountedCombatService.Instance.ChargeCheckType(defenderContext));
		var attackRoll = attackCheck.Check(Assailant, Difficulty.Normal, null, target,
			context.Momentum + SizeMomentumBonus(context, target));
		var defenseRoll = defenseCheck.Check(target, Difficulty.Normal, null, Assailant,
			defenderContext.Momentum + SizeMomentumBonus(defenderContext, Assailant));
		var opposed = new OpposedOutcome(attackRoll, defenseRoll);
		SetMeleeRange(target);

		if (opposed.Outcome == OpposedOutcomeDirection.Proponent)
		{
			SendChargeMessage(target, context, attackRoll.Outcome);
			ApplyMountedImpact(target, context, Math.Max(1, (int)opposed.Degree));
			ResolveMountedWeaponAttack(GetMountedWeaponAttack(target), target,
				new HelplessDefenseMove { Assailant = target });
		}
		else if (opposed.Outcome == OpposedOutcomeDirection.Opponent)
		{
			SendChargeMessage(target, context, defenseRoll.Outcome, true);
			Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ are|is broken out of &0's charge by $1's counter-charge!", Assailant, Assailant, target),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			Assailant.DoCombatKnockdown(Math.Max(1, (int)opposed.Degree));
			target.OffensiveAdvantage += 2.0 + (int)opposed.Degree;
		}
		else
		{
			SendChargeMessage(target, context, Outcome.NotTested);
		}

		_delay = 0.0;
		return new CombatMoveResult
		{
			MoveWasSuccessful = opposed.Outcome != OpposedOutcomeDirection.Opponent,
			AttackerOutcome = attackRoll,
			DefenderOutcome = defenseRoll,
			RecoveryDifficulty = opposed.Outcome == OpposedOutcomeDirection.Opponent
				? Difficulty.Hard
				: Difficulty.Normal
		};
	}

	private void ResolveMountedImpact(ICharacter target, MountedCombatContext context, bool braced)
	{
		var (_, _, opposed) = ResolveMountedContest(target, context, braced,
			CheckType.OpposeMountedChargeCheck);
		if (opposed.Outcome != OpposedOutcomeDirection.Proponent)
		{
			target.DefensiveAdvantage += 1.0 + (int)opposed.Degree;
			return;
		}

		ApplyMountedImpact(target, context, Math.Max(1, (int)opposed.Degree));
	}

	public static bool CanBehemothCharge(ICharacter assailant, ICharacter target)
	{
		return MountedCombatService.Instance.ResolveContext(assailant) is null &&
		       !assailant.MeleeRange &&
		       assailant.ColocatedWith(target) &&
		       assailant.CurrentContextualSize(SizeContext.GrappleAttack) >
		       target.CurrentContextualSize(SizeContext.GrappleDefense) &&
		       UsableBehemothChargeAttacks(assailant, target).Any();
	}

	private static IEnumerable<INaturalAttack> UsableBehemothChargeAttacks(ICharacter assailant,
		ICharacter target)
	{
		// Behemoth Charge is selected only by ChargeToMeleeMove, so ordinary attack intention filters
		// must not make the racial capability disappear. Body availability, position and usability
		// progs still gate the natural attack exactly as they do for other racial attacks.
		return assailant.Race.NaturalWeaponAttacks
			.Where(x => x.Attack.MoveType == BuiltInCombatMoveType.BehemothChargeAttack)
			.Where(x => NaturalAttack.IsValidTarget(x.Attack, target))
			.Where(x => assailant.Body.Bodyparts.Contains(x.Bodypart))
			.Where(x => assailant.Body.CanUseBodypart(x.Bodypart) == CanUseBodypartResult.CanUse)
			.Where(x => !assailant.Body.HeldItemsFor(x.Bodypart).Any())
			.Where(x => !assailant.Body.WieldedItemsFor(x.Bodypart).Any())
			.Where(x => x.Attack.RequiredPositionStates.Contains(assailant.PositionState))
			.Where(x => x.Attack.UsabilityProg?.ExecuteBool(assailant, null, target) ?? true);
	}

	private void ResolveCaughtSkirmishImpact(ICharacter target, MountedCombatContext mountedContext,
		INaturalAttack behemothAttack, CombatMoveResult outcome)
	{
		if (!outcome.MoveWasSuccessful)
		{
			return;
		}

		if (mountedContext is not null)
		{
			ResolveMountedImpact(target, mountedContext, false);
			ResolveMountedWeaponAttack(GetMountedWeaponAttack(target), target,
				new HelplessDefenseMove { Assailant = target });
		}

		ResolveBehemothImpact(target, behemothAttack);
	}

	private INaturalAttack GetBehemothChargeAttack(ICharacter target)
	{
		if (!CanBehemothCharge(Assailant, target))
		{
			return null;
		}

		return UsableBehemothChargeAttacks(Assailant, target)
			.Where(x => Assailant.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(Assailant, x.Attack)))
			.GetWeightedRandom(x => x.Attack.Weighting);
	}

	private void ResolveBehemothImpact(ICharacter target, INaturalAttack attack)
	{
		if (attack is null || target.State.HasFlag(CharacterState.Dead) ||
		    Assailant.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		var move = new MountedImpactNaturalAttackMove(Assailant, attack, target, true,
			SizeContext.GrappleAttack, true);
		move.ResolveMove(target.ResponseToMove(move, Assailant));
	}

	private (CheckOutcome Attack, CheckOutcome Defense, OpposedOutcome Opposed) ResolveMountedContest(
		ICharacter target, MountedCombatContext context, bool braced, CheckType defenseCheckType)
	{
		var attackCheck = GetMountedCheck(MountedCombatService.Instance.ChargeCheckType(context));
		var defenseCheck = GetMountedCheck(defenseCheckType);
		var attackDifficulty = context.Domain.In(MountedCombatDomain.Aerial, MountedCombatDomain.Aquatic)
			? Difficulty.Hard
			: Difficulty.Normal;
		var defenseDifficulty = braced ? Difficulty.Easy : Difficulty.Normal;
		var attackRoll = attackCheck.Check(Assailant, attackDifficulty, null, target,
			context.Momentum + SizeMomentumBonus(context, target));
		var defenseRoll = defenseCheck.Check(target, defenseDifficulty, null, Assailant,
			braced ? 3.0 : 0.0);
		return (attackRoll, defenseRoll, new OpposedOutcome(attackRoll, defenseRoll));
	}

	private void ApplyMountedImpact(ICharacter target, MountedCombatContext context, int degrees)
	{
		Assailant.OffensiveAdvantage += 2.0 + degrees + Math.Min(3.0, context.Momentum * 0.5);
		target.DefensiveAdvantage -= 1.0 + degrees;

		if (TryResolveMountAttack(target, context))
		{
			return;
		}

		var targetSize = target.CurrentContextualSize(SizeContext.GrappleDefense);
		var sizeDifference = (int)context.EffectiveSize - (int)targetSize;
		if (sizeDifference >= 2 || degrees >= 3 || target.RidingMount is not null ||
		    VehicleCombatService.Instance.VehicleFor(target) is not null)
		{
			target.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ are|is knocked sprawling by the force of $1's charge!", target, target, Assailant),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			target.DoCombatKnockdown(Math.Max(1, degrees + Math.Max(0, sizeDifference - 1)));
		}
	}

	private bool TryResolveMountAttack(ICharacter target, MountedCombatContext context)
	{
		if (context.Mount is not { } mount || mount.SamePhysicalInstance(target))
		{
			return false;
		}

		var moveType = context.Domain switch
		{
			MountedCombatDomain.Aerial => BuiltInCombatMoveType.AerialSweepAttack,
			MountedCombatDomain.Aquatic => BuiltInCombatMoveType.AquaticChargeAttack,
			_ => BuiltInCombatMoveType.MountedTrampleAttack
		};
		var targetSize = target.CurrentContextualSize(SizeContext.GrappleDefense);
		if (moveType == BuiltInCombatMoveType.MountedTrampleAttack && context.EffectiveSize <= targetSize)
		{
			return false;
		}

		var attack = mount.Race
			.UsableNaturalWeaponAttacks(mount, target, false, moveType)
			.Where(x => mount.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(mount, x.Attack)))
			.GetWeightedRandom(x => x.Attack.Weighting);
		if (attack is null)
		{
			return false;
		}

		var move = new MountedImpactNaturalAttackMove(mount, attack, target, true);
		move.ResolveMove(target.ResponseToMove(move, mount));
		return true;
	}

	private ICheck GetMountedCheck(CheckType checkType)
	{
		var check = Gameworld.GetCheck(checkType);
		return check.Type == checkType ? check : Gameworld.GetCheck(CheckType.GenericSkillCheck);
	}

	private static double SizeMomentumBonus(MountedCombatContext context, ICharacter target)
	{
		return Math.Clamp((int)context.EffectiveSize -
		                  (int)target.CurrentContextualSize(SizeContext.GrappleDefense), -3, 5);
	}

	private void SetMeleeRange(ICharacter target)
	{
		Assailant.MeleeRange = true;
		if (target.CombatTarget == Assailant || target.CombatTarget is null)
		{
			target.MeleeRange = true;
		}
	}

	private void SendChargeMessage(ICharacter target, MountedCombatContext context, Outcome outcome,
		bool failure = false)
	{
		var moveType = context is null
			? BuiltInCombatMoveType.ChargeToMelee
			: MountedCombatService.Instance.ChargeMessageType(context);
		var message = failure
			? Gameworld.CombatMessageManager.GetFailMessageFor(Assailant, target, null, null, moveType, outcome, null)
			: Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null, moveType, outcome, null);
		if (message.StartsWith("Error -", StringComparison.Ordinal))
		{
			message = FallbackChargeMessage(context, failure);
		}

		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(message, Assailant, Assailant, target,
			context?.Conveyance ?? Assailant), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
	}

	private static string FallbackChargeMessage(MountedCombatContext context, bool failure)
	{
		if (context is null)
		{
			return failure
				? "$0 attempt|attempts to charge into melee range with $1, but fall|falls short"
				: "$0 charge|charges into melee range with $1";
		}

		return (context.Domain, failure) switch
		{
			(MountedCombatDomain.Aerial, false) => "$0 dive|dives from above at $1 on $2",
			(MountedCombatDomain.Aerial, true) => "$0 dive|dives at $1 on $2, but &1 slip|slips clear",
			(MountedCombatDomain.Aquatic, false) => "$0 surge|surges through the water at $1 on $2",
			(MountedCombatDomain.Aquatic, true) => "$0 surge|surges at $1 on $2, but &1 evade|evades",
			(MountedCombatDomain.GroundVehicle, false) => "$0 drive|drives $2 straight at $1",
			(MountedCombatDomain.GroundVehicle, true) => "$0 drive|drives $2 at $1, but &1 evade|evades",
			(MountedCombatDomain.AquaticVehicle, false) => "$0 drive|drives $2 through the water at $1",
			(MountedCombatDomain.AquaticVehicle, true) => "$0 drive|drives $2 at $1, but &1 evade|evades",
			(_, false) => "$0 thunder|thunders at $1 astride $2",
			_ => "$0 thunder|thunders at $1 astride $2, but &1 evade|evades"
		};
	}

    private void HandleReceiveCharge(ReceiveChargeMove receiveCharge, ICharacter target)
    {
        ICombatMove receiveChargeDefense = Assailant.ResponseToMove(receiveCharge, target);
        receiveCharge.ResolveMove(receiveChargeDefense);
    }

    private CombatMoveResult HandleSkirmish(ICharacter target, ICombatMove response)
    {
        if (response == null || response is HelplessDefenseMove || target.CurrentSpeed == null)
        {
            Assailant.OutputHandler.Handle(
                new EmoteOutput(
                    new Emote(
                        Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
                            BuiltInCombatMoveType.ChargeToMelee, Outcome.MajorPass, null), Assailant, Assailant,
                        target), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
            Assailant.MeleeRange = true;
            target.MeleeRange = true;
            Assailant.OffensiveAdvantage += Gameworld.GetStaticDouble("OffensiveAdvantageFromCharge");
            _delay = 0;
            return new CombatMoveResult
            {
                MoveWasSuccessful = true
            };
        }

        ICharacter assailantMover = Assailant.GetCombatMover();
        IPositionState assailantPosition = assailantMover.PositionState;
        bool assailantHadSpeed = assailantMover.CurrentSpeeds.TryGetValue(assailantPosition, out IMoveSpeed oldAssailantSpeed);
        IMoveSpeed assailantTempSpeed =
                    assailantMover.Speeds.Where(x => x.Position == assailantPosition).FirstMin(x => x.Multiplier);
        if (assailantTempSpeed != null)
        {
            assailantMover.CurrentSpeeds[assailantPosition] = assailantTempSpeed;
        }

        double speed = Assailant.ApplyMovementSpeedCheck(assailantMover.MoveSpeed(null), false);

        if (assailantTempSpeed != null)
        {
            if (assailantHadSpeed)
            {
                assailantMover.CurrentSpeeds[assailantPosition] = oldAssailantSpeed;
            }
            else
            {
                assailantMover.CurrentSpeeds.Remove(assailantPosition);
            }
        }

        ICharacter targetMover = target.GetCombatMover();
        IPositionState targetPosition = targetMover.PositionState;
        bool targetHadSpeed = targetMover.CurrentSpeeds.TryGetValue(targetPosition, out IMoveSpeed oldTargetSpeed);
        IMoveSpeed targetTempSpeed =
                    targetMover.Speeds.Where(x => x.Position == targetPosition).FirstMin(x => x.Multiplier);
        if (targetTempSpeed != null)
        {
            targetMover.CurrentSpeeds[targetPosition] = targetTempSpeed;
        }
        double moveTypeMultiplier;
        double locationMultiplier = target.CombatSettings.SkirmishToOtherLocations && target.Movement == null &&
                targetMover.Movement == null
                ? 1.0
                : 1.25;
        switch (target.CombatSettings.PreferredMeleeMode)
        {
            case CombatStrategyMode.Skirmish:
            case CombatStrategyMode.Swooper:
                moveTypeMultiplier = 1.3;
                break;
            case CombatStrategyMode.FullSkirmish:
                moveTypeMultiplier = 1.05;
                break;
            case CombatStrategyMode.Flee:
                moveTypeMultiplier = 1.05;
                break;
            default:
                moveTypeMultiplier = 1.5;
                break;
        }

        double targetBaseSpeed = target.ApplyMovementSpeedCheck(targetMover.MoveSpeed(null), true);
        double targetspeed = targetBaseSpeed * moveTypeMultiplier * locationMultiplier;

        if (targetTempSpeed != null)
        {
            if (targetHadSpeed)
            {
                targetMover.CurrentSpeeds[targetPosition] = oldTargetSpeed;
            }
            else
            {
                targetMover.CurrentSpeeds.Remove(targetPosition);
            }
        }

        if (speed <= targetspeed)
        {
            Assailant.OutputHandler.Handle(
                new EmoteOutput(
                    new Emote(
                        Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
                            BuiltInCombatMoveType.ChargeToMelee, Outcome.MajorPass, null), Assailant, Assailant,
                        target), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
            Assailant.MeleeRange = true;
            if (target.CombatTarget == Assailant || target.CombatTarget == null)
            {
                target.MeleeRange = true;
            }

            Assailant.OffensiveAdvantage += Gameworld.GetStaticDouble("OffensiveAdvantageFromCharge");
            _delay = 0;
            return new CombatMoveResult
            {
                MoveWasSuccessful = true
            };
        }

        Assailant.OutputHandler.Handle(
            new EmoteOutput(
                new Emote(
                    Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
                        BuiltInCombatMoveType.ChargeToMelee, Outcome.MajorFail, null), Assailant, Assailant, target),
                style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
        if (target.CombatSettings.SkirmishToOtherLocations && target.Movement == null && targetMover.Movement == null &&
            speed <= targetspeed * 1.25)
        {
            ICellExit exit = target.Location.ExitsFor(target).Where(x => target.CanCross(x).Success).GetRandomElement();
            if (exit != null)
            {
                target.Move(exit, new Emote("fleeing from $0", target, Assailant));
                foreach (
                    ICharacter other in
                    target.Combat.Combatants.OfType<ICharacter>()
                          .Where(
                              x =>
                                  x.ColocatedWith(target) && x.CombatTarget == target &&
                                  x.CombatSettings.PursuitMode == PursuitMode.AlwaysPursue && x.Movement == null)
                          .ToList())
                {
                    other.Move(exit, new Emote("pursuing $0", other, target));
                }
            }
        }

        _delay = 1.0;
        return new CombatMoveResult
        {
            MoveWasSuccessful = false,
            RecoveryDifficulty = Difficulty.Normal
        };
    }

    #region Overrides of CombatMoveBase

    private double _delay;
    public override double BaseDelay => _delay;

    #endregion
}
