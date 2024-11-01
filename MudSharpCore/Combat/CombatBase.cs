using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Combat.Moves;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using MudSharp.Effects.Interfaces;
using MudSharp.Construction;
using ExpressionEngine;
using MudSharp.Body;
using MudSharp.Commands.Trees;

namespace MudSharp.Combat;

public abstract class CombatBase : ICombat
{
	protected HashSet<ICell> CombatCells { get; } = new();
	protected static ITraitExpression RecoveryTimeExpression { get; set; }
	protected static ITraitExpression PowerMoveStaminaCost { get; set; }
	protected static ITraitExpression GraceMoveStaminaCost { get; set; }
	protected static ITraitExpression StrengthForRelativeStrengthCheck { get; set; }
	protected static Expression RelativeStrengthDefenseStaminaCost { get; set; }

	public static double PowerMoveStaminaMultiplier(ICharacter assailant)
	{
		PowerMoveStaminaCost.Formula.Parameters["encumbrance"] = (int)assailant.Encumbrance;
		PowerMoveStaminaCost.Formula.Parameters["encpercent"] = assailant.EncumbrancePercentage;
		return PowerMoveStaminaCost.Evaluate(assailant, null, TraitBonusContext.CombatPowerMoveStamina);
	}

	public static double GraceMoveStaminaMultiplier(ICharacter assailant)
	{
		GraceMoveStaminaCost.Formula.Parameters["encumbrance"] = (int)assailant.Encumbrance;
		GraceMoveStaminaCost.Formula.Parameters["encpercent"] = assailant.EncumbrancePercentage;
		return GraceMoveStaminaCost.Evaluate(assailant, null, TraitBonusContext.CombatGraceMoveStamina);
	}

	public static double RelativeStrengthDefenseStaminaMultiplier(ICharacter attacker, ICharacter defender)
	{
		RelativeStrengthDefenseStaminaCost.Parameters["attacker"] =
			StrengthForRelativeStrengthCheck.Evaluate(attacker, null,
				TraitBonusContext.CombatRelativeStrengthDefenseStamina);
		RelativeStrengthDefenseStaminaCost.Parameters["defender"] =
			StrengthForRelativeStrengthCheck.Evaluate(defender, null,
				TraitBonusContext.CombatRelativeStrengthDefenseStamina);
		return Convert.ToDouble(RelativeStrengthDefenseStaminaCost.Evaluate());
	}

	public static double CombatSpeedMultiplier { get; set; } = 1.0;

	public static void SetupCombat(IFuturemud gameworld)
	{
		using (new FMDB())
		{
			var expression = gameworld.GetStaticConfiguration("RecoveryTimeExpressionId");
			if (!long.TryParse(expression, out var value))
			{
				throw new ApplicationException(
					"The RecoveryTimeExpressionId setting did not convert to a long integer.");
			}

			RecoveryTimeExpression = gameworld.TraitExpressions.Get(value);
			if (RecoveryTimeExpression == null)
			{
				RecoveryTimeExpression = new TraitExpression("max(1, 25 - recovery)", gameworld);
			}
		}

		CombatSpeedMultiplier = gameworld.GetStaticDouble("CombatSpeedMultiplier");
		PowerMoveStaminaCost =
			new TraitExpression(gameworld.GetStaticConfiguration("PowerMoveStaminaCostExpression"), gameworld);
		GraceMoveStaminaCost =
			new TraitExpression(gameworld.GetStaticConfiguration("GraceMoveStaminaCostExpression"), gameworld);
		StrengthForRelativeStrengthCheck =
			new TraitExpression(gameworld.GetStaticConfiguration("StrengthForRelativeStrengthCheckExpression"),
				gameworld);
		RelativeStrengthDefenseStaminaCost =
			new Expression(gameworld.GetStaticConfiguration("RelativeStrengthDefenseStaminaCostExpression"));
	}

	#region Implementation of ICombat

	protected readonly List<IPerceiver> _combatants = new();
	public IEnumerable<IPerceiver> Combatants => _combatants;
	public abstract void JoinCombat(IPerceiver character, Difficulty initialDelayDifficulty = Difficulty.Automatic);
	public IFuturemud Gameworld { get; init; }

	/// <summary>
	///     Removes a specified combatant from the combat
	/// </summary>
	/// <param name="character">The character leaving the combat</param>
	/// <returns>True if combat ended entirely because of this action</returns>
	public abstract bool LeaveCombat(IPerceiver character);

	public abstract void EndCombat(bool echo);
	public event EventHandler CombatEnds;
	public event CombatMergeDelegate CombatMerged;

	protected void EndCombatEvent()
	{
		CombatEnds?.Invoke(this, null);
		foreach (var cell in CombatCells)
		{
			cell.HandleEvent(EventType.CombatEndedHere, cell);
		}
	}

	public void MergeCombat(ICombat oldCombat)
	{
		CombatMerged?.Invoke(oldCombat, this);
		foreach (var person in oldCombat.Combatants)
		{
			JoinCombat(person);
		}
	}

	public abstract void TruceRequested(IPerceiver combatant);

	public static bool CanFreelyLeaveCombatAgainstOpponent(IPerceiver who, IPerceiver opponent)
	{
		if (opponent is null)
		{
			return true;
		}

		if (opponent.CombatTarget != who)
		{
			return true;
		}

		if (!(opponent is ICharacter character))
		{
			return true;
		}

		if (character.State.HasFlag(CharacterState.Sleeping) || character.State.HasFlag(CharacterState.Unconscious) ||
			character.State.HasFlag(CharacterState.Paralysed) ||
			character.CombatStrategyMode == CombatStrategyMode.Flee)
		{
			return true;
		}

		var maxRangedDistance = character.Body.WieldedItems
										 .SelectNotNull(y => y.GetItemType<IRangedWeapon>())
										 .Select(y => (int)y.WeaponType.DefaultRangeInRooms)
										 .DefaultIfEmpty(0)
										 .Max();

		var distance = character.DistanceBetween(who, 5);
		if (distance == -1)
		{
			distance = 5;
		}

		if (distance >= 5 || maxRangedDistance < distance)
		{
			return true;
		}

		if (maxRangedDistance == 0 && who.Location == opponent.Location && !who.ColocatedWith(opponent) &&
			!opponent.CouldTransitionToLayer(who.RoomLayer))
		{
			return true;
		}

		return false;
	}

	public virtual bool CanFreelyLeaveCombat(IPerceiver who)
	{
		return Combatants.All(x => CanFreelyLeaveCombatAgainstOpponent(who, x));
	}

	public abstract string CombatHeaderDescription { get; }

	public virtual string DescribeFor(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine(CombatHeaderDescription);
		sb.AppendLine();
		if (Combatants.Contains(voyeur))
		{
			if (voyeur.State.IsDisabled())
			{
				sb.AppendLine($"You are {voyeur.State.DescribeEnum().Colour(Telnet.Red)}.");
			}
			else if (voyeur.CombatTarget is null)
			{
				sb.AppendLine($"You are not fighting anyone.");
			}
			else if (voyeur.MeleeRange)
			{
				switch (voyeur.GetFacingFor(voyeur.CombatTarget))
				{
					case Facing.Front:
						sb.AppendLine(
							$"You are in melee with {voyeur.CombatTarget.HowSeen(voyeur)}.");
						break;
					case Facing.RightFlank:
						sb.AppendLine(
							$"You are attacking the right flank of {voyeur.CombatTarget.HowSeen(voyeur)}.");
						break;
					case Facing.Rear:
						sb.AppendLine(
							$"You are attacking {voyeur.CombatTarget.HowSeen(voyeur)} from the rear.");
						break;
					case Facing.LeftFlank:
						sb.AppendLine(
							$"You are attacking the left flank of {voyeur.CombatTarget.HowSeen(voyeur)}.");
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				sb.AppendLine($"You are fighting {voyeur.CombatTarget.HowSeen(voyeur)} at range.");
			}

			var strategy = CombatStrategyFactory.GetStrategy(voyeur.CombatStrategyMode);
			var why = strategy.WhyWontAttack(voyeur);
			if (!string.IsNullOrEmpty(why))
			{
				sb.AppendLine(why);
			}

			var whyTarget = voyeur.CombatTarget is ICharacter tch ? strategy.WhyWontAttack(voyeur, tch) : string.Empty;
			if (!string.IsNullOrEmpty(whyTarget))
			{
				sb.AppendLine($"You won't attack your target {whyTarget}");
			}

			sb.AppendLine();
		}

		sb.AppendLine("Combatants".GetLineWithTitle(voyeur, Telnet.Red, Telnet.White));
		sb.AppendLine();
		foreach (var combatant in Combatants)
		{
			if (combatant is not ICharacter ch)
			{
				continue; // TODO
			}

			if (voyeur == ch)
			{
				continue;
			}

			if (!voyeur.CanSee(ch))
			{
				continue;
			}

			if (ch.State.IsDisabled())
			{
				sb.AppendLine($"{ch.HowSeen(voyeur, true)} is {ch.State.Describe().Colour(Telnet.Red)}.");
				continue;
			}

			if (ch.CombatTarget is null)
			{
				sb.AppendLine($"{ch.HowSeen(voyeur, true)} is not fighting anyone.");
				continue;
			}

			if (ch.MeleeRange)
			{
				switch (ch.GetFacingFor(ch.CombatTarget))
				{
					case Facing.Front:
						sb.AppendLine(
							$"{ch.HowSeen(voyeur, true)} is in melee with {ch.CombatTarget.HowSeen(voyeur)}.");
						continue;
					case Facing.RightFlank:
						sb.AppendLine(
							$"{ch.HowSeen(voyeur, true)} is attacking the right flank of {ch.CombatTarget.HowSeen(voyeur)}.");
						continue;
					case Facing.Rear:
						sb.AppendLine(
							$"{ch.HowSeen(voyeur, true)} is attacking {ch.CombatTarget.HowSeen(voyeur)} from the rear.");
						continue;
					case Facing.LeftFlank:
						sb.AppendLine(
							$"{ch.HowSeen(voyeur, true)} is attacking the left flank of {ch.CombatTarget.HowSeen(voyeur)}.");
						continue;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			sb.AppendLine($"{ch.HowSeen(voyeur, true)} is fighting {ch.CombatTarget.HowSeen(voyeur)} at range.");
		}

		return sb.ToString();
	}

	/// <summary>
	///     If true, this combat would be considered "Friendly", e.g. a spar, training bout, competition etc.
	/// </summary>
	public abstract bool Friendly { get; }

	public abstract string LDescAddendumFor(ICombatant combatant, IPerceiver voyeur);

	public virtual void EndCombatNoHandling()
	{
		foreach (var combatant in Combatants.ToList())
		{
			combatant.Combat = null;
		}
		// TODO - anything else required
		// TODO - combat merge event thrown for effects
	}

	protected virtual TimeSpan GetCombatDelay(IPerceiver perceiver, Difficulty recoverDifficulty,
		double baseTime = 1.0)
	{
		if (perceiver is not IPerceivableHaveTraits haveTraits)
		{
			return TimeSpan.FromSeconds(10);
		}

		var result = Gameworld.GetCheck(CheckType.CombatRecoveryCheck).Check(haveTraits, recoverDifficulty);
		RecoveryTimeExpression.Formula.Parameters["recovery"] = result.CheckDegrees();
#if DEBUG
		var time = TimeSpan.FromSeconds(baseTime * RecoveryTimeExpression.Evaluate(haveTraits) * CombatSpeedMultiplier);
		Console.WriteLine(
			$"Combat Delay {perceiver.HowSeen(perceiver, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)}: {time}");
		return time;
#else
			return TimeSpan.FromSeconds(baseTime*RecoveryTimeExpression.Evaluate(haveTraits)*CombatSpeedMultiplier);
#endif
	}

	public virtual void CombatAction(IPerceiver perceiver, ICombatMove move)
	{
		perceiver.RemoveAllEffects(x => x is IEndOnCombatMove e && e.CausesToEnd(move), true);
		if (move == null)
		{
			if (!perceiver.AffectedBy<IdleCombatant>())
			{
				perceiver.AddEffect(new IdleCombatant(perceiver, this));
			}

			Gameworld.Scheduler.AddOrDelaySchedule(
				new Schedule<IPerceiver>(perceiver, CombatAction, ScheduleType.Combat, TimeSpan.FromMilliseconds(50),
					$"Combat Idle for {perceiver.HowSeen(perceiver, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf)}"),
				perceiver);
			return;
		}

		perceiver.RemoveAllEffects(x => x.IsEffectType<IdleCombatant>());
		var targetResponse = move.CharacterTargets.FirstOrDefault()?.ResponseToMove(move, perceiver);
		var result = move.ResolveMove(targetResponse);

		//Bloody weapons, fists, bullets, blood splash, etc.
		(move as WeaponAttackMove)?.ResolveBloodSpray(result);

		var recovery = result.RecoveryDifficulty;
		if (perceiver is ICharacter character && character.Race.RaceUsesStamina)
		{
#if DEBUG
			Console.WriteLine(
				$"Character {character.CurrentName.GetName(NameStyle.SimpleFull)} used {move.StaminaCost.ToString("N2")} stamina on {move.Description}.");
#endif
			character.SpendStamina(move.StaminaCost);
		}

		if (perceiver.CombatTarget is ICharacter tcharacter && tcharacter.Race.RaceUsesStamina &&
			(targetResponse?.UsesStaminaWithResult(result) ?? false))
		{
#if DEBUG
			Console.WriteLine(
				$"Character {tcharacter.CurrentName.GetName(NameStyle.SimpleFull)} used {targetResponse.StaminaCost.ToString("N2")} stamina on {targetResponse.Description}.");
#endif
			tcharacter.SpendStamina(targetResponse.StaminaCost);
		}

		if (perceiver.CombatTarget?.CheckCombatStatus() == false)
		{
			LeaveCombat(perceiver.CombatTarget);
			return;
		}

		if (!perceiver.CheckCombatStatus())
		{
			LeaveCombat(perceiver);
			return;
		}

		Gameworld.Scheduler.AddOrDelaySchedule(new Schedule<IPerceiver>(perceiver, CombatAction, ScheduleType.Combat,
				GetCombatDelay(perceiver, recovery, move.BaseDelay),
				$"Combat delay for {perceiver.HowSeen(perceiver, flags: PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee)}"),
			perceiver);
	}

	public virtual void CombatAction(IPerceiver perceiver)
	{
		if (perceiver.CombatTarget == null)
		{
			perceiver.AcquireTarget();
		}

		if (perceiver.CombatTarget == null)
		{
			Gameworld.Scheduler.AddSchedule(new Schedule<IPerceiver>(perceiver, CombatAction, ScheduleType.Combat,
				GetCombatDelay(perceiver, Difficulty.Automatic),
				$"Combat delay no target for {perceiver.HowSeen(perceiver, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf)}"));
			if (!perceiver.AffectedBy<IdleCombatant>())
			{
				perceiver.AddEffect(new IdleCombatant(perceiver, this));
			}

			return;
		}

		var move = perceiver.ChooseMove();
		CombatAction(perceiver, move);
		if (move != null)
		{
			CheckCombatEndingConditions(move.Assailant);
			foreach (var target in move.CharacterTargets)
			{
				if (target == null)
				{
					continue;
				}

				CheckCombatEndingConditions(target);
			}
		}
	}

	private void CheckCombatEndingConditions(ICharacter combatant)
	{
		if (combatant.State.HasFlag(CharacterState.Unconscious) || combatant.State.HasFlag(CharacterState.Sleeping) ||
			combatant.CombatStrategyMode == CombatStrategyMode.Flee)
		{
			if (CanFreelyLeaveCombat(combatant))
			{
				LeaveCombat(combatant);
			}
		}
	}

	public IEnumerable<IPerceiver> MeleeProximityOfCombatant(IPerceiver combatant)
	{
		var results = new List<IPerceiver>();
		results.Add(combatant);
		while (true)
		{
			var found = false;
			foreach (var other in Combatants)
			{
				if (results.Contains(other))
				{
					continue;
				}

				if (results.Any(x =>
						(x.CombatTarget == other && x.MeleeRange) || (other.CombatTarget == x && other.MeleeRange)))
				{
					found = true;
					results.Add(other);
				}
			}

			if (!found)
			{
				break;
			}
		}

		return results;
	}

	public void ReevaluateMeleeRange(IPerceiver whoMoved)
	{
		if (whoMoved.CombatTarget != null && !whoMoved.ColocatedWith(whoMoved.CombatTarget))
		{
			whoMoved.MeleeRange = false;
		}

		foreach (var combatant in Combatants.ToList())
		{
			if (combatant.CombatTarget == whoMoved && combatant.MeleeRange && !combatant.ColocatedWith(whoMoved))
			{
				combatant.MeleeRange = false;
			}
		}
	}

	#endregion
}