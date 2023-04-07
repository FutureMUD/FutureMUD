using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class TendingWounds : CharacterActionWithTarget, IAffectProximity
{
	private static string _effectDurationDiceExpression;

	private static string EffectDurationDiceExpression
	{
		get
		{
			if (_effectDurationDiceExpression == null)
			{
				_effectDurationDiceExpression = Futuremud.Games.First()
				                                         .GetStaticConfiguration("TendingEffectDurationDiceExpression");
			}

			return _effectDurationDiceExpression;
		}
	}

	public static TimeSpan EffectDuration => TimeSpan.FromSeconds(Dice.Roll(EffectDurationDiceExpression));

	public IInventoryPlan OriginalInventoryPlan { get; set; }

	private readonly List<IWound> _tendedWounds = new();

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Tending to the wounds of {TargetCharacter.HowSeen(voyeur)}.";
	}

	protected override string SpecificEffectType => "TendingWounds";

	#endregion

	public TendingWounds(ICharacter owner, ICharacter target) : base(owner, target)
	{
		WhyCannotMoveEmoteString = "@ cannot move because $0 $0|are|is tending to $1's wounds.";
		CancelEmoteString = "@ $0|stop|stops tending to $1's wounds.";
		LDescAddendum = "tending to $1's wounds";
		ActionDescription = "tending to $1's wounds";
		_blocks.Add("general");
		_blocks.Add("movement");
	}

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (TargetCharacter == thing)
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
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

	public override void ExpireEffect()
	{
		var wounds = TargetCharacter.VisibleWounds(CharacterOwner, WoundExaminationType.Examination)
		                            .Where(x => x.CanBeTreated(TreatmentType.Tend) != Difficulty.Impossible)
		                            .Where(x => !_tendedWounds.Contains(x))
		                            .ToList();
		if (!wounds.Any())
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote(
					"@ have|has finished tending to all of $1's visible wounds.",
					CharacterOwner, CharacterOwner, TargetCharacter)));
			Owner.RemoveEffect(this, true);
			return;
		}

		var worstWound = wounds.FirstMax(x => x.Severity)!;

		var inventoryPlan = Gameworld.TendInventoryPlanTemplate.CreatePlan(CharacterOwner);
		if (OriginalInventoryPlan == null)
		{
			OriginalInventoryPlan = inventoryPlan;
		}

		if (inventoryPlan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			inventoryPlan.ExecuteWholePlan();
		}

		if (inventoryPlan != OriginalInventoryPlan)
		{
			inventoryPlan.FinalisePlanNoRestore();
		}

		var maxDifficulty = wounds.Select(x => x.CanBeTreated(TreatmentType.Tend))
		                          .Where(x => x != Difficulty.Impossible)
		                          .DefaultIfEmpty()
		                          .Max();
		var treatmentItem =
			CharacterOwner.Body.HeldItems.SelectNotNull(x => x.GetItemType<ITreatment>())
			              .Where(x => x.IsTreatmentType(TreatmentType.Tend))
			              .FirstMin(x => x.GetTreatmentDifficulty(maxDifficulty));

		if (treatmentItem == null)
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote(
					"@ stop|stops tending to $1's wounds because #0 no longer has the correct tools.",
					CharacterOwner, CharacterOwner, TargetCharacter)));
			Owner.RemoveEffect(this, true);
			return;
		}

		var tendCheck = Gameworld.GetCheck(CheckType.TendWoundCheck);
		worstWound.Treat(CharacterOwner, TreatmentType.Tend, treatmentItem,
			tendCheck.Check(CharacterOwner, worstWound.CanBeTreated(TreatmentType.Tend)), false);
		_tendedWounds.Add(worstWound);

		if (TargetCharacter.VisibleWounds(CharacterOwner, WoundExaminationType.Examination)
		                   .Any(x => x.CanBeTreated(TreatmentType.Tend) != Difficulty.Impossible))
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote(
					"@ continue|continues &0's medical efforts, as $1 still $1|have|has wounds that need tending to.",
					CharacterOwner, CharacterOwner, TargetCharacter)));
			CharacterOwner.Reschedule(this, TimeSpan.FromSeconds(Dice.Roll(EffectDurationDiceExpression)));
			return;
		}

		CharacterOwner.OutputHandler.Handle(new EmoteOutput(
			new Emote(
				"@ have|has finished tending to all of $1's visible wounds.",
				CharacterOwner, CharacterOwner, TargetCharacter)));
		Owner.RemoveEffect(this, true);
	}
}