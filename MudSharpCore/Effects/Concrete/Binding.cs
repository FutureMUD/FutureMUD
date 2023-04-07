using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class BeingBound : Effect, IAffectProximity
{
	public IBodypart Bodypart { get; set; }
	public ICharacter Binder { get; set; }

	public BeingBound(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return "Being Bound";
	}

	protected override string SpecificEffectType => "BeingBound";

	#endregion

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (Binder == thing)
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
	}
}

public class Binding : CharacterActionWithTarget, IAffectProximity
{
	private static string _effectDurationDiceExpression;

	private static string EffectDurationDiceExpression
	{
		get
		{
			if (_effectDurationDiceExpression == null)
			{
				_effectDurationDiceExpression = Futuremud.Games.First()
				                                         .GetStaticConfiguration("BindingEffectDurationDiceExpression");
			}

			return _effectDurationDiceExpression;
		}
	}

	public static TimeSpan EffectDuration => TimeSpan.FromSeconds(Dice.Roll(EffectDurationDiceExpression));

	public BeingBound TargetEffect { get; set; }

	public IInventoryPlan OriginalInventoryPlan { get; set; }

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (TargetCharacter == thing)
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Binding the wounds of {TargetCharacter.HowSeen(voyeur)}.";
	}

	protected override string SpecificEffectType => "Binding";

	#endregion

	public Binding(ICharacter owner, ICharacter target) : base(owner, target)
	{
		WhyCannotMoveEmoteString = "@ cannot move because $0 $0|are|is binding $1's wounds.";
		CancelEmoteString = "@ $0|stop|stops binding $1's wounds.";
		LDescAddendum = "binding $1's wounds";
		TargetEffect = new BeingBound(target) { Binder = owner };
		ActionDescription = "binding $1's wounds";
		_blocks.Add("general");
		_blocks.Add("movement");
		target.AddEffect(TargetEffect);
		var wounds = TargetCharacter.VisibleWounds(CharacterOwner, WoundExaminationType.Examination)
		                            .Where(x => x.BleedStatus == BleedStatus.Bleeding)
		                            .ToList();
		if (wounds.Any())
		{
			var worstWound =
				wounds.Where(x => x.CanBeTreated(TreatmentType.Trauma) != Difficulty.Impossible)
				      .FirstMax(x => x.Severity);
			TargetEffect.Bodypart = worstWound.Bodypart;
		}
	}

	#region Overrides of TargetedBlockingDelayedAction

	/// <summary>
	///     Fires when an effect is removed, including a matured scheduled effect
	/// </summary>
	public override void RemovalEffect()
	{
		OriginalInventoryPlan?.FinalisePlan();
		ReleaseEventHandlers();
		Target.RemoveEffect(TargetEffect);
	}

	#endregion

	public override void ExpireEffect()
	{
		var wounds = TargetCharacter.VisibleWounds(CharacterOwner, WoundExaminationType.Examination)
		                            .Where(x => x.BleedStatus == BleedStatus.Bleeding)
		                            .ToList();
		if (!wounds.Any())
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ have|has finished binding all of $1's visible bleeding wounds.", CharacterOwner, CharacterOwner,
				TargetCharacter)));
			Owner.RemoveEffect(this, true);
			return;
		}

		var inventoryPlan = Gameworld.BindInventoryPlanTemplate.CreatePlan(CharacterOwner);
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

		var maxDifficulty = wounds.Select(x => x.CanBeTreated(TreatmentType.Trauma))
		                          .Where(x => x != Difficulty.Impossible)
		                          .DefaultIfEmpty()
		                          .Max();
		var treatmentItem =
			CharacterOwner.Body.HeldItems.SelectNotNull(x => x.GetItemType<ITreatment>())
			              .Where(x => x.IsTreatmentType(TreatmentType.Trauma))
			              .FirstMin(x => x.GetTreatmentDifficulty(maxDifficulty));

		var bindCheck = Gameworld.GetCheck(CheckType.BindWoundCheck);
		var worstWound =
			wounds.Where(x => x.CanBeTreated(TreatmentType.Trauma) != Difficulty.Impossible)
			      .FirstMax(x => x.Severity);

		worstWound.Treat(CharacterOwner, TreatmentType.Trauma, treatmentItem,
			bindCheck.Check(CharacterOwner, worstWound.CanBeTreated(TreatmentType.Trauma)), false);

		wounds = TargetCharacter.VisibleWounds(CharacterOwner, WoundExaminationType.Examination)
		                        .Where(x => x.BleedStatus == BleedStatus.Bleeding)
		                        .ToList();
		if (wounds.Any())
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ continue|continues &0's medical efforts, as $1 $1|are|is still bleeding.", CharacterOwner,
				CharacterOwner, TargetCharacter)));
			CharacterOwner.Reschedule(this, TimeSpan.FromSeconds(Dice.Roll(EffectDurationDiceExpression)));
			TargetEffect.Bodypart = wounds.Where(x => x.CanBeTreated(TreatmentType.Trauma) != Difficulty.Impossible)
			                              .FirstMax(x => x.Severity).Bodypart;
			return;
		}

		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ have|has finished binding all of $1's visible bleeding wounds.", CharacterOwner, CharacterOwner,
			TargetCharacter)));
		Owner.RemoveEffect(this, true);
	}
}