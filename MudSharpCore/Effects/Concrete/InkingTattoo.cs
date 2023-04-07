using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Disfigurements;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class HavingTattooInked : Effect, IAffectProximity
{
	public ICharacter Tattooist { get; }
	public ITattoo Tattoo { get; }

	public HavingTattooInked(ICharacter owner, ICharacter tattooist, ITattoo tattoo) : base(owner)
	{
		Tattooist = tattooist;
		Tattoo = tattoo;
	}

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (Tattooist == thing)
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
	}

	protected override string SpecificEffectType => "HavingTattooInked";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"{Tattooist.HowSeen(voyeur, true)} is inking {Tattoo.ShortDescription.SubstituteWrittenLanguage(voyeur, Gameworld).Colour(Telnet.BoldOrange)}.";
	}

	public override bool IsBlockingEffect(string blockingType)
	{
		return string.IsNullOrEmpty(blockingType) || blockingType.EqualTo("general");
	}

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return "having a tattoo inked";
	}

	public override IEnumerable<string> Blocks => new[] { "general" };
}

public class InkingTattoo : CharacterActionWithTargetAndTool, IAffectProximity
{
	private static TimeSpan _effectTimespan;

	public static TimeSpan EffectTimespan
	{
		get
		{
			if (_effectTimespan == default)
			{
				_effectTimespan =
					TimeSpan.FromSeconds(Futuremud.Games.First().GetStaticDouble("InkingTattooTickDurationSeconds"));
			}

			return _effectTimespan;
		}
	}

	protected override string SpecificEffectType => "InkingTattoo";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Inking {Tattoo.ShortDescription.SubstituteWrittenLanguage(voyeur, Gameworld).Colour(Telnet.BoldOrange)} on {TargetCharacter.HowSeen(voyeur)}.";
	}

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (TargetCharacter == thing)
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
	}

	public ITattoo Tattoo { get; }
	public HavingTattooInked TargetEffect { get; }
	public int ExpiryCount { get; protected set; }

	public InkingTattoo(ICharacter owner, ICharacter target, ITattoo tattoo, IGameItem needleItem) : base(owner, target,
		new[] { (needleItem, DesiredItemState.Held) })
	{
		Tattoo = tattoo;
		WhyCannotMoveEmoteString = "@ cannot move because #0 %0|are|is inking a tattoo on $1.";
		LDescAddendum = "inking a tattoo on $1";
		_blocks.Add("general");
		_blocks.Add("movement");
		ActionDescription = $"inking a tattoo on $1's {Tattoo.Bodypart.FullDescription()}";
		TargetEffect = new HavingTattooInked(TargetCharacter, owner, tattoo);
		TargetCharacter.AddEffect(TargetEffect);
	}

	public override void InitialEffect()
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ begin|begins inking $2 on $1's {Tattoo.Bodypart.FullDescription()}.", CharacterOwner, CharacterOwner,
			Target,
			new DummyPerceivable(
				perceiver => Tattoo.ShortDescription.SubstituteWrittenLanguage(perceiver, Gameworld)
				                   .Colour(Telnet.BoldOrange), perceiver => string.Empty))));
	}

	public override void RemovalEffect()
	{
		ReleaseEventHandlers();
		Target.RemoveEffect(TargetEffect);
	}

	public override void ExpireEffect()
	{
		// TODO - tool usage
		Tattoo.CompletionPercentage += 1.0 / Tattoo.TattooTemplate.TicksToCompleteTattoo;
		Tattoo.TimeOfInscription = CharacterOwner.Location.DateTime();
		Tattoo.TattooistSkill = CharacterOwner.GetTrait(TattooTemplate.TattooistTrait)?.Value ?? 0.0;
		Gameworld.GetCheck(CheckType.InkTattooCheck).Check(CharacterOwner,
			(Difficulty)(int)(Tattoo.TattooistSkill / Gameworld.GetStaticDouble("TattooSkillPerDifficulty")),
			TargetCharacter);
		TargetCharacter.Body.TattoosChanged = true;
		if (Tattoo.CompletionPercentage > 1.0)
		{
			Tattoo.CompletionPercentage = 1.0;
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ have|has finished inking $2 on $1's {Tattoo.Bodypart.FullDescription()}.", CharacterOwner,
				CharacterOwner, Target,
				new DummyPerceivable(
					perceiver => Tattoo.ShortDescription.SubstituteWrittenLanguage(perceiver, Gameworld)
					                   .Colour(Telnet.BoldOrange), perceiver => string.Empty))));
			Owner.RemoveEffect(this, true);
			return;
		}

		var inkPlan = Tattoo.TattooTemplate.GetInkPlan(CharacterOwner);
		switch (inkPlan.PlanIsFeasible())
		{
			case GameItems.Inventory.InventoryPlanFeasibility.Feasible:
				break;
			case GameItems.Inventory.InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
			case GameItems.Inventory.InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
			case GameItems.Inventory.InventoryPlanFeasibility.NotFeasibleMissingItems:
				CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
					$"@ stop|stops inking $2 on $1's {Tattoo.Bodypart.FullDescription()} because #0 don't|doesn't have enough ink.",
					CharacterOwner, CharacterOwner, Target,
					new DummyPerceivable(
						perceiver => Tattoo.ShortDescription.SubstituteWrittenLanguage(perceiver, Gameworld)
						                   .Colour(Telnet.BoldOrange), perceiver => string.Empty))));
				Owner.RemoveEffect(this, true);
				break;
		}

		inkPlan.ExecuteWholePlan();
		inkPlan.FinalisePlanNoRestore();
		Owner.Reschedule(this, EffectTimespan);
		if (++ExpiryCount % 10 == 0)
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote($"@ continue|continues inking $2 on $1's {Tattoo.Bodypart.FullDescription()}.",
					CharacterOwner, CharacterOwner, Target,
					new DummyPerceivable(
						perceiver => Tattoo.ShortDescription.SubstituteWrittenLanguage(perceiver, Gameworld)
						                   .Colour(Telnet.BoldOrange), perceiver => string.Empty)),
				flags: OutputFlags.Insigificant));
			ExpiryCount = 0;
		}
	}
}