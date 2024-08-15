using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class CleaningWounds : CharacterActionWithTarget, IAffectProximity
{
	private static string _effectDurationDiceExpression;

	private static string EffectDurationDiceExpression =>
		_effectDurationDiceExpression ?? (_effectDurationDiceExpression = Futuremud.Games.First()
			.GetStaticConfiguration("CleanWoundsEffectDurationDiceExpression"));

	public static TimeSpan EffectDuration => TimeSpan.FromSeconds(Dice.Roll(EffectDurationDiceExpression));

	public IInventoryPlan OriginalInventoryPlan { get; set; }

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Cleaning the wounds of {TargetCharacter.HowSeen(voyeur)}.";
	}

	protected override string SpecificEffectType => "Cleaning";

	#endregion

	public CleaningWounds(ICharacter owner, ICharacter target) : base(owner, target)
	{
		WhyCannotMoveEmoteString = "@ cannot move because $0 $0|are|is cleaning $1's wounds.";
		CancelEmoteString = "@ $0|stop|stops cleaning $1's wounds.";
		LDescAddendum = "cleaning $1's wounds";
		ActionDescription = "cleaning $1's wounds";
		_blocks.Add("general");
		_blocks.Add("movement");
	}

	#region Overrides of TargetedBlockingDelayedAction

	/// <summary>
	///     Fires when an effect is removed, including a matured scheduled effect
	/// </summary>
	public override void RemovalEffect()
	{
		OriginalInventoryPlan?.FinalisePlan();
		ReleaseEventHandlers();
	}

	#endregion

	private void CleanAntiseptic(IEnumerable<IWound> wounds, ITreatment treatmentItem, ICheck check)
	{
		var antisepticWounds = wounds.Where(x => x.CanBeTreated(TreatmentType.Antiseptic) != Difficulty.Impossible)
		                             .ToList();
		var worstWound = antisepticWounds.FirstMax(x => x.Severity);
		worstWound.Treat(CharacterOwner, TreatmentType.Antiseptic, treatmentItem,
			check.Check(CharacterOwner, worstWound.CanBeTreated(TreatmentType.Antiseptic)), false);
	}

	private void CleanNormal(IEnumerable<IWound> wounds, ITreatment treatmentItem, ICheck check)
	{
		var cleanWounds = wounds.Where(x => x.CanBeTreated(TreatmentType.Clean) != Difficulty.Impossible)
		                        .ToList();
		var worstWound = cleanWounds.FirstMax(x => x.Severity);
		worstWound.Treat(CharacterOwner, TreatmentType.Clean, treatmentItem,
			check.Check(CharacterOwner, worstWound.CanBeTreated(TreatmentType.Clean)), false);
	}

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (TargetCharacter == thing)
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
	}

	public enum PeekCanCleanReason
	{
		CanClean,
		NoWounds,
		AntisepticWoundsNoTreatment
	}

	public static (bool Success, PeekCanCleanReason Reason) PeekCanClean(ICharacter ch, ICharacter tch)
	{
		var wounds = tch.VisibleWounds(ch, WoundExaminationType.Examination).ToList();
		var cleanWounds = wounds.Where(x => x.CanBeTreated(TreatmentType.Clean) != Difficulty.Impossible).ToList();
		var antisepticWounds = wounds.Where(x => x.CanBeTreated(TreatmentType.Antiseptic) != Difficulty.Impossible)
		                             .ToList();

		if (!cleanWounds.Any() && !antisepticWounds.Any())
		{
			return (false, PeekCanCleanReason.NoWounds);
		}

		var plan = ch.Gameworld.CleanWoundInventoryPlanTemplate.CreatePlan(ch);
		ITreatment treatmentItem = null;
		if (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			treatmentItem = plan.PeekPlanResults()
			                    .FirstOrDefault(x => x.OriginalReference?.ToString() == "treatment")
			                    ?.PrimaryTarget?.GetItemType<ITreatment>();
		}

		if (treatmentItem?.IsTreatmentType(TreatmentType.Antiseptic) != true && !cleanWounds.Any() &&
		    antisepticWounds.Any())
		{
			return (false, PeekCanCleanReason.AntisepticWoundsNoTreatment);
		}

		return (true, PeekCanCleanReason.CanClean);
	}

	public override void ExpireEffect()
	{
		var wounds = TargetCharacter.VisibleWounds(CharacterOwner, WoundExaminationType.Examination)
		                            .Where(x => x.CanBeTreated(TreatmentType.Clean) != Difficulty.Impossible ||
		                                        x.CanBeTreated(TreatmentType.Antiseptic) != Difficulty.Impossible)
		                            .ToList();
		if (!wounds.Any())
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ have|has finished cleaning all of $1's visible wounds.", CharacterOwner, CharacterOwner,
				TargetCharacter)));
			Owner.RemoveEffect(this, true);
			return;
		}

		var inventoryPlan = Gameworld.CleanWoundInventoryPlanTemplate.CreatePlan(CharacterOwner);
		OriginalInventoryPlan ??= inventoryPlan;

		ITreatment treatmentItem = null;
		if (inventoryPlan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			var results = inventoryPlan.ExecuteWholePlan();
			treatmentItem = results.FirstOrDefault(x => x.OriginalReference?.ToString() == "treatment")?.PrimaryTarget
			                       ?.GetItemType<ITreatment>();
		}

		if (inventoryPlan != OriginalInventoryPlan)
		{
			inventoryPlan.FinalisePlanNoRestore();
		}

		if (treatmentItem == null && wounds.All(x => x.CanBeTreated(TreatmentType.Clean) == Difficulty.Impossible))
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote("@ stop|stops cleaning $1's wounds.",
				CharacterOwner, CharacterOwner, TargetCharacter)));
			CharacterOwner.Send("You require antiseptics to treat your patient any further.".Colour(Telnet.Yellow));
			Owner.RemoveEffect(this, true);
			return;
		}

		var cleanCheck = Gameworld.GetCheck(CheckType.CleanWoundCheck);

		if (treatmentItem?.IsTreatmentType(TreatmentType.Antiseptic) == true)
		{
			CleanAntiseptic(wounds, treatmentItem, cleanCheck);
		}
		else
		{
			CleanNormal(wounds, treatmentItem, cleanCheck);
		}

		var (canContinue, reason) = PeekCanClean(CharacterOwner, TargetCharacter);
		if (canContinue)
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ continue|continues to clean $1's wounds.", CharacterOwner, CharacterOwner,
				TargetCharacter)));
			CharacterOwner.Reschedule(this, TimeSpan.FromSeconds(Dice.Roll(EffectDurationDiceExpression)));
			return;
		}

		if (reason == PeekCanCleanReason.AntisepticWoundsNoTreatment)
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote("@ stop|stops cleaning $1's wounds.",
				CharacterOwner, CharacterOwner, TargetCharacter)));
			CharacterOwner.Send("You require antiseptics to treat your patient any further.".Colour(Telnet.Yellow));
		}
		else
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ have|has finished cleaning all of $1's visible wounds.", CharacterOwner, CharacterOwner,
				TargetCharacter)));
		}

		Owner.RemoveEffect(this, true);
	}
}