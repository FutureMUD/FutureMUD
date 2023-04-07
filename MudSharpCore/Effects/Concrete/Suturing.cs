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

public class Suturing : CharacterActionWithTarget, IAffectProximity
{
	private static string _effectDurationDiceExpression;

	private static string EffectDurationDiceExpression
	{
		get
		{
			if (_effectDurationDiceExpression == null)
			{
				_effectDurationDiceExpression = Futuremud.Games.First()
				                                         .GetStaticConfiguration(
					                                         "SuturingEffectDurationDiceExpression");
			}

			return _effectDurationDiceExpression;
		}
	}

	public static TimeSpan EffectDuration => TimeSpan.FromSeconds(Dice.Roll(EffectDurationDiceExpression));

	public IInventoryPlan OriginalInventoryPlan { get; set; }

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Suturing the wounds of {TargetCharacter.HowSeen(voyeur)}.";
	}

	protected override string SpecificEffectType => "Suturing";

	#endregion

	public Suturing(ICharacter owner, ICharacter target) : base(owner, target)
	{
		WhyCannotMoveEmoteString = "@ cannot move because $0 $0|are|is suturing $1's wounds.";
		CancelEmoteString = "@ $0|stop|stops suturing $1's wounds.";
		LDescAddendum = "suturing $1's wounds";
		ActionDescription = "suturing $1's wounds";
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
		                            .Where(x => x.BleedStatus == BleedStatus.TraumaControlled &&
		                                        x.CanBeTreated(TreatmentType.Close) != Difficulty.Impossible)
		                            .ToList();
		if (!wounds.Any())
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ have|has finished suturing all of $1's visible wounds.", CharacterOwner, CharacterOwner,
				TargetCharacter)));
			Owner.RemoveEffect(this, true);
			return;
		}

		var inventoryPlan = Gameworld.SutureInventoryPlanTemplate.CreatePlan(CharacterOwner);
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

		var maxDifficulty = wounds.Select(x => x.CanBeTreated(TreatmentType.Close))
		                          .Where(x => x != Difficulty.Impossible)
		                          .DefaultIfEmpty()
		                          .Max();
		var treatmentItem =
			CharacterOwner.Body.HeldItems.SelectNotNull(x => x.GetItemType<ITreatment>())
			              .Where(x => x.IsTreatmentType(TreatmentType.Close))
			              .FirstMin(x => x.GetTreatmentDifficulty(maxDifficulty));

		if (treatmentItem == null)
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ stop|stops suturing $1's wounds because #0 no longer has the correct tools.", CharacterOwner,
				CharacterOwner, TargetCharacter)));
			Owner.RemoveEffect(this, true);
			return;
		}

		var sutureCheck = Gameworld.GetCheck(CheckType.SutureWoundCheck);
		var worstWound =
			wounds.Where(x => x.CanBeTreated(TreatmentType.Close) != Difficulty.Impossible)
			      .FirstMax(x => x.Severity);
		worstWound.Treat(CharacterOwner, TreatmentType.Close, treatmentItem,
			sutureCheck.Check(CharacterOwner, worstWound.CanBeTreated(TreatmentType.Close)), false);

		if (TargetCharacter.VisibleWounds(CharacterOwner, WoundExaminationType.Examination)
		                   .Any(x => x.BleedStatus == BleedStatus.TraumaControlled))
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ continue|continues &0's medical efforts, as $1 still $1|have|has wounds that need suturing.",
				CharacterOwner, CharacterOwner, TargetCharacter)));
			CharacterOwner.Reschedule(this, TimeSpan.FromSeconds(Dice.Roll(EffectDurationDiceExpression)));
			return;
		}

		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ have|has finished suturing all of $1's visible wounds.", CharacterOwner, CharacterOwner,
			TargetCharacter)));
		Owner.RemoveEffect(this, true);
	}
}